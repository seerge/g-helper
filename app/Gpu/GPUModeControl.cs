using GHelper.Display;
using GHelper.Gpu.NVidia;
using GHelper.Helpers;
using GHelper.USB;
using System.Diagnostics;

namespace GHelper.Gpu
{
    public class GPUModeControl
    {
        SettingsForm settings;

        public static int gpuMode;
        public static bool? gpuExists = null;

        static bool nvRestartPending;
        public bool IsSwitching { get; private set; }

        // InitGPUMode() is normally run fire-and-forget in the background (see call sites
        // below) - wrap it so a failure is logged like every other error path in this file
        // instead of becoming a silent unobserved task exception.
        private void InitGPUModeLogged()
        {
            try { InitGPUMode(); }
            catch (Exception ex) { Logger.WriteLine("Error initializing GPU mode UI: " + ex.Message); }
        }

        // Owned here rather than threaded through every caller: any two overlapping eco
        // switches are wrong regardless of what triggered them (power event, manual toggle,
        // Optimized-mode auto-apply), so SetGPUEco always supersedes its own previous run.
        private static CancellationTokenSource? ecoCts;
        private static readonly Lock ecoCtsLock = new();

        // The eco value an in-flight RunGPUEcoSequence is driving toward, or null once it has
        // either committed the ACPI write or been abandoned. AutoGPUMode must trust this over
        // a live ACPI read: a sequence can take seconds (Nvidia service stop/restart) before it
        // actually writes the eco flag, and during that window the live flag still reads the
        // OLD value - a caller deciding "no switch needed" from that stale read would let the
        // stale in-flight sequence commit its (by-then-wrong) target uncontested, e.g. a quick
        // unplug-then-replug leaving the laptop stuck in Eco after the replug.
        private static int? pendingEcoTarget;


        public GPUModeControl(SettingsForm settingsForm)
        {
            settings = settingsForm;
        }

        public void InitGPUMode()
        {
            if (AppConfig.NoGpu())
            {
                settings.HideGPUModes(false);
                return;
            }

            int eco = Program.acpi.DeviceGet(AsusACPI.GPUEco);
            int mux = Program.acpi.DeviceGet(AsusACPI.GPUMux);

            Logger.WriteLine("Eco flag : " + eco);
            Logger.WriteLine("Mux flag : " + mux);

            if (eco == 1 && HardwareControl.GpuControl?.IsValid == true)
            {
                Logger.WriteLine("Eco half-state");
                if (AppConfig.IsEcoBootFix())
                {
                    HardwareControl.DisposeGpuControl();
                    Task.Run(() => Program.acpi.DeviceSet(AsusACPI.GPUEco, eco, "GPUEco Force Fix"));
                }
            }

            settings.VisualiseGPUButtons(eco >= 0, mux >= 0);

            if (mux == 0)
            {
                gpuMode = AsusACPI.GPUModeUltimate;
            }
            else
            {
                if (eco == 1)
                    gpuMode = AsusACPI.GPUModeEco;
                else
                    gpuMode = AsusACPI.GPUModeStandard;

                // GPU mode not supported
                if (eco < 0 && mux < 0)
                {
                    if (gpuExists is null) gpuExists = Program.acpi.GetFan(AsusFan.GPU) >= 0;
                    settings.HideGPUModes((bool)gpuExists);
                }
            }

            AppConfig.Set("gpu_mode", gpuMode);
            settings.VisualiseGPUMode(gpuMode);

            Aura.CustomRGB.ApplyGPUColor(gpuMode);

            CheckGpuError();

        }



        public void SetGPUMode(int GPUMode, int auto = 0)
        {

            int CurrentGPU = AppConfig.Get("gpu_mode");
            AppConfig.Set("gpu_auto", auto);

            if (CurrentGPU == GPUMode)
            {
                settings.VisualiseGPUMode();
                return;
            }

            var restart = false;
            var changed = false;

            int status;

            if (CurrentGPU == AsusACPI.GPUModeUltimate)
            {
                DialogResult dialogResult = settings.ShowMessage(Properties.Strings.AlertUltimateOff, Properties.Strings.AlertUltimateTitle, MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    status = Program.acpi.DeviceSet(AsusACPI.GPUMux, 1, "GPUMux");
                    restart = true;
                    changed = true;
                }
            }
            else if (GPUMode == AsusACPI.GPUModeUltimate)
            {
                if (Program.acpi.DeviceGet(AsusACPI.GPUMux) < 0)
                {
                    Logger.WriteLine("Mux not supported");
                    settings.VisualiseGPUMode();
                    return;
                }

                DialogResult dialogResult = settings.ShowMessage(Properties.Strings.AlertUltimateOn, Properties.Strings.AlertUltimateTitle, MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Program.acpi.SetGPUEco(0);
                    Thread.Sleep(500);

                    int eco = Program.acpi.DeviceGet(AsusACPI.GPUEco);
                    Logger.WriteLine("Eco flag : " + eco);
                    if (eco == 1)
                    {
                        settings.VisualiseGPUMode();
                        return;
                    }

                    status = Program.acpi.DeviceSet(AsusACPI.GPUMux, 0, "GPUMux");
                    restart = true;
                    changed = true;
                }

            }
            else if (GPUMode == AsusACPI.GPUModeEco)
            {
                settings.VisualiseGPUMode(GPUMode);
                SetGPUEco(1);
                changed = true;
            }
            else if (GPUMode == AsusACPI.GPUModeStandard)
            {
                settings.VisualiseGPUMode(GPUMode);
                SetGPUEco(0);
                changed = true;
            }

            if (changed)
            {
                AppConfig.Set("gpu_mode", GPUMode);
            }

            if (restart)
            {
                settings.VisualiseGPUMode();
                Process.Start("shutdown", "/r /t 1");
            }

        }



        // Cancelling the previous run whenever a new one starts is what lets an in-flight eco
        // switch (e.g. mid Nvidia-service-stop after an unplug) abandon itself cleanly instead
        // of racing a fresher switch triggered by an immediate replug - whatever triggered it.
        public void SetGPUEco(int eco, int delay = 0)
        {

            settings.LockGPUModes();

            CancellationTokenSource cts;
            lock (ecoCtsLock)
            {
                ecoCts?.Cancel();
                ecoCts?.Dispose();
                cts = new CancellationTokenSource();
                ecoCts = cts;
                pendingEcoTarget = eco;
            }

            // Screen refresh only depends on power line status, not on GPU/eco state, so it
            // runs as an independent sibling task instead of being sequenced behind, or bolted
            // onto, the GPU/nvidia work below. Called directly (not via settings.Invoke) since
            // ChangeDisplaySettingsEx is a slow blocking OS call - InitScreen() already
            // marshals its own narrow UI update internally, so forcing the whole call onto the
            // UI thread here would freeze the window for the duration of the mode change.
            Task.Run(() => ScreenControl.AutoScreen());
            Task.Run(() => RunGPUEcoSequence(eco, delay, cts));
        }

        private async Task RunGPUEcoSequence(int eco, int delay, CancellationTokenSource cts)
        {
            var token = cts.Token;
            try
            {
                IsSwitching = true;
                // Was previously a blocking Thread.Sleep on the caller's thread; now an
                // awaitable delay that a superseding request can cancel outright instead of
                // letting a now-pointless switch fire after the fact.
                if (delay > 0) await Task.Delay(delay, token);
                token.ThrowIfCancellationRequested();

                if (eco == 1)
                {
                    HardwareControl.KillGPUApps();
                    HardwareControl.DisposeGpuControl();
                    // Awaited via Process.WaitForExitAsync internally - frees this thread for
                    // the duration of the service stop instead of blocking it synchronously.
                    if (AppConfig.IsNVPlatform()) await NvidiaGpuControl.StopNVServiceAsync(token);
                }

                token.ThrowIfCancellationRequested();

                Logger.WriteLine($"Running eco command {eco}");

                int status = Program.acpi.SetGPUEco(eco);

                // Wait for the ACPI eco flag to actually reflect the change instead of
                // guessing how long that takes. refresh_delay is now only the safety-net
                // timeout, not the wait itself.
                await AsyncHelper.PollUntilAsync(
                    () => Program.acpi.DeviceGet(AsusACPI.GPUEco) == eco,
                    intervalMs: 100,
                    timeoutMs: AppConfig.Get("refresh_delay", 500),
                    token: token);

                // Fire-and-forget: InitGPUMode() calls Aura.CustomRGB.ApplyGPUColor(), which does
                // a synchronous USB HID write + ACPI call - real blocking I/O that nothing below
                // depends on finishing, so it shouldn't hold up the Nvidia restart step. Its own
                // UI-touching calls (VisualiseGPUButtons/VisualiseGPUMode) already self-marshal
                // via InvokeRequired, so this is safe to run off the UI thread too.
                _ = Task.Run(InitGPUModeLogged, token);

                if (eco == 0)
                {
                    if (AppConfig.IsNVPlatform() || nvRestartPending)
                    {
                        settings.LockGPUModes(Properties.Strings.RestartingNVServices);

                        // Restart-Service awaits until it has actually restarted (via
                        // Process.WaitForExitAsync, not a blocked thread); on failure (GPU/PCIe
                        // link not re-enumerated yet) it returns quickly, so retry with backoff
                        // instead of a single flat wait-then-try.
                        bool restarted = await AsyncHelper.PollUntilAsync(
                            async ct =>
                            {
                                try
                                {
                                    if (AppConfig.IsNVPlatform()) await NvidiaGpuControl.RestartNVServiceAsync(ct);
                                    else await NvidiaGpuControl.RestartNvContainerAsync(ct);
                                    return true;
                                }
                                catch (OperationCanceledException) { throw; }
                                catch (Exception ex)
                                {
                                    Logger.WriteLine("NV service restart attempt failed: " + ex.Message);
                                    return false;
                                }
                            },
                            intervalMs: 500,
                            timeoutMs: AppConfig.Get("nv_delay", 5000),
                            token: token);

                        if (!restarted) Logger.WriteLine("NV service did not restart within timeout");

                        nvRestartPending = false;
                        _ = Task.Run(InitGPUModeLogged, token);
                    }

                    token.ThrowIfCancellationRequested();

                    await HardwareControl.RecreateGpuControlWithRetry(3, 2, token);
                    CheckStandardHalfState(token);
                    _ = Program.modeControl.ApplyGPUSettingsAsync();
                }

                if (AppConfig.IsModeReapplyRequired())
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(1000), token);

                    // Reapply the currently active mode's settings (power limits reset by the
                    // GPU eco switch on these models) - not AutoPerformance(), which re-derives
                    // the mode from the power-source config and can switch away from whatever
                    // mode is actually active (e.g. reverting a manual pick made in the
                    // meantime). Mode selection stays isolated from GPU state changes.
                    Program.modeControl.SetPerformanceMode();
                }
            }
            catch (OperationCanceledException)
            {
                // Superseded by a newer eco switch - not an error, the newer run owns the target state now.
                Logger.WriteLine($"Eco command {eco} superseded, abandoning");
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Error setting GPU Eco: " + ex.Message);
            }
            finally
            {
                // Only clear if nothing superseded us in the meantime (a newer SetGPUEco call
                // would have already replaced ecoCts with its own source and set its own
                // target) - otherwise we'd wipe out the newer, still-pending target.
                lock (ecoCtsLock)
                {
                    if (ReferenceEquals(ecoCts, cts))
                    {
                        pendingEcoTarget = null;
                        IsSwitching = false;
                    }
                }
            }
        }

        public static bool IsPlugged() =>
            Program.currentSource == Program.PowerSource.Barrel ||
            (Program.currentSource == Program.PowerSource.USBC && !AppConfig.Is("optimized_usbc"));

        public static bool suspended = false;

        public bool AutoGPUMode(bool optimized = false, int delay = 0)
        {

            bool GpuAuto = AppConfig.Is("gpu_auto");
            bool ForceGPU = AppConfig.IsForceSetGPUMode() && !GpuAuto;

            int GpuMode = AppConfig.Get("gpu_mode");

            if (!GpuAuto && !ForceGPU) return false;

            if (suspended)
            {
                Logger.WriteLine("Skipping GPU Mode switch: Suspend");
                return false;
            }

            // Trust an in-flight sequence's target over the live ACPI flag: the flag can lag
            // several seconds behind (Nvidia service stop/restart) while a switch is already
            // underway, and deciding "no change needed" from that stale read would let a now-
            // stale in-flight switch commit uncontested moments later.
            int eco = pendingEcoTarget ?? Program.acpi.DeviceGet(AsusACPI.GPUEco);
            int mux = Program.acpi.DeviceGet(AsusACPI.GPUMux);

            if (mux == 0)
            {
                if (optimized) SetGPUMode(AsusACPI.GPUModeStandard, 1);
                return false;
            }
            else
            {

                if (eco == 1)
                    if ((GpuAuto && IsPlugged()) || (ForceGPU && GpuMode == AsusACPI.GPUModeStandard))
                    {
                        // Delay no longer blocks this thread - it's handed to the eco
                        // sequence itself, which awaits it and can be cancelled mid-wait.
                        SetGPUEco(0, delay);
                        return true;
                    }
                if (eco == 0)
                    if ((GpuAuto && !IsPlugged()) || (ForceGPU && GpuMode == AsusACPI.GPUModeEco))
                    {

                        if (Program.acpi.IsXGConnected()) return false;
                        if (HardwareControl.IsUsedGPU())
                        {
                            DialogResult dialogResult = settings.ShowMessage(Properties.Strings.AlertDGPU, Properties.Strings.AlertDGPUTitle, MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.No) return false;
                        }
                        else if (GpuAuto && Program.acpi.IsExternalDisplayConnected())
                        {
                            DialogResult dialogResult = settings.ShowMessage(Properties.Strings.AlertExternalDisplay, Properties.Strings.AlertDGPUTitle, MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.No) return false;
                        }

                        SetGPUEco(1, delay);
                        return true;
                    }
            }

            return false;

        }


        private static CancellationTokenSource? xgmCts;
        private static readonly Lock xgmCtsLock = new();

        public void ToggleXGM(bool silent = false, CancellationToken token = default)
        {
            CancellationTokenSource cts;
            lock (xgmCtsLock)
            {
                xgmCts?.Cancel();
                xgmCts = CancellationTokenSource.CreateLinkedTokenSource(token);
                cts = xgmCts;
            }
            var ct = cts.Token;

            Task.Run(async () =>
            {
                try
                {
                    settings.LockGPUModes();

                    if (Program.acpi.DeviceGet(AsusACPI.GPUXG) == 1)
                    {
                        XGM.Reset();
                        HardwareControl.KillGPUApps();

                        if (silent)
                        {
                            Program.acpi.DeviceSet(AsusACPI.GPUXG, 0, "GPU XGM");
                            await Task.Delay(TimeSpan.FromSeconds(15), ct);
                        }
                        else
                        {
                            DialogResult dialogResult = DialogResult.No;
                            settings.Invoke((MethodInvoker)delegate
                            {
                                dialogResult = MessageBox.Show(settings, "Did you close all applications running on XG Mobile?", "Disabling XG Mobile", MessageBoxButtons.YesNo);
                            });

                            if (dialogResult == DialogResult.Yes)
                            {
                                Program.acpi.DeviceSet(AsusACPI.GPUXG, 0, "GPU XGM");
                                await Task.Delay(TimeSpan.FromSeconds(15), ct);
                                ct.ThrowIfCancellationRequested();
                                HardwareControl.RecreateGpuControl();
                            }
                        }
                    }
                    else
                    {

                        if (AppConfig.Is("xgm_special"))
                            Program.acpi.DeviceSet(AsusACPI.GPUXG, 0x101, "GPU XGM");
                        else
                            Program.acpi.DeviceSet(AsusACPI.GPUXG, 1, "GPU XGM");

                        XGM.Init();

                        await Task.Delay(TimeSpan.FromSeconds(15), ct);
                        ct.ThrowIfCancellationRequested();
                        await HardwareControl.RecreateGpuControlWithRetry(6, 5, ct);

                        if (AppConfig.IsApplyFans())
                            XGM.SetFan(AppConfig.GetFanConfig(AsusFan.XGM));

                    }

                    ct.ThrowIfCancellationRequested();
                    settings.Invoke(delegate
                    {
                        InitGPUMode();
                    });
                }
                catch (OperationCanceledException) { }
            }, ct);
        }

        public void KillGPUApps()
        {
            if (HardwareControl.GpuControl is not null)
            {
                HardwareControl.GpuControl.KillGPUApps();
            }
        }

        public void CaptureNvBootState()
        {
            nvRestartPending = Program.acpi.IsNVidiaGPU() && Program.acpi.DeviceGet(AsusACPI.GPUEco) == 1;
        }

        private static CancellationTokenSource? standardHalfStateCts;
        private static readonly Lock standardHalfStateLock = new();

        public void CheckStandardHalfState(CancellationToken token = default)
        {
            if (gpuMode != AsusACPI.GPUModeStandard || HardwareControl.GpuControl is not null) return;

            Logger.WriteLine("Standard half-state");
            if (!AppConfig.IsStandardForceFix()) return;

            CancellationTokenSource cts;
            lock (standardHalfStateLock)
            {
                standardHalfStateCts?.Cancel();
                standardHalfStateCts = CancellationTokenSource.CreateLinkedTokenSource(token);
                cts = standardHalfStateCts;
            }
            var ct = cts.Token;

            Task.Run(async () =>
            {
                try
                {
                    Program.acpi.DeviceSet(AsusACPI.GPUEco, 0, "GPUStandard Force Fix");
                    await Task.Delay(TimeSpan.FromMilliseconds(AppConfig.Get("nv_delay", 5000)), ct);
                    ct.ThrowIfCancellationRequested();
                    HardwareControl.RecreateGpuControl();
                }
                catch (OperationCanceledException) { }
            }, ct);
        }

        public void StandardModeFix()
        {
            if (!AppConfig.IsStandardModeFix()) return;
            if (Program.acpi.DeviceGet(AsusACPI.GPUMux) == 0) return; // Ultimate mode

            Logger.WriteLine("Forcing Standard Mode on shutdown");
            Program.acpi.SetGPUEco(0);
        }

        public static string? gpuError = null;

        public static void CheckGpuError() => Task.Run(() =>
        {
            string? error = DeviceHelper.GetGpuError();
            if (gpuError == error) return;
            gpuError = error;
            if (error != null) Logger.WriteLine(error);
            Program.settingsForm.VisualiseGPUMode();
        });

    }
}
