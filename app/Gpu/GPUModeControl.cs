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
                DialogResult dialogResult = MessageBox.Show(Properties.Strings.AlertUltimateOff, Properties.Strings.AlertUltimateTitle, MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    status = Program.acpi.DeviceSet(AsusACPI.GPUMux, 1, "GPUMux");
                    restart = true;
                    changed = true;
                }
            }
            else if (GPUMode == AsusACPI.GPUModeUltimate)
            {
                DialogResult dialogResult = MessageBox.Show(Properties.Strings.AlertUltimateOn, Properties.Strings.AlertUltimateTitle, MessageBoxButtons.YesNo);
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
                changed = SetGPUEco(1);
            }
            else if (GPUMode == AsusACPI.GPUModeStandard)
            {
                settings.VisualiseGPUMode(GPUMode);
                changed = SetGPUEco(0);
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



        // Returns true if the switch was started (work dispatched), false if cancelled by a guard.
        public bool SetGPUEco(int eco)
        {

            if (eco == 1 && NvDDS.TryGetState(out _, out var mux) && mux == NvDDS.DDSMuxState.DGpu)
            {
                NvDDS.OpenTray();
                MessageBox.Show(
                    settings,
                    "NVIDIA GPU is currently used for display. Please switch Display Mode to Optimus or Auto first.",
                    "NVIDIA Display Mode",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                InitGPUMode();    
                return false;
            }

            settings.LockGPUModes();

            Task.Run(async () =>
            {

                int status = 1;

                if (eco == 1)
                {
                    HardwareControl.KillGPUApps();
                    HardwareControl.DisposeGpuControl();
                    if (AppConfig.IsNVPlatform()) NvidiaGpuControl.StopNVService();
                }

                Logger.WriteLine($"Running eco command {eco}");

                try
                {

                    status = Program.acpi.SetGPUEco(eco);

                    if (eco == 0)
                    {
                        NvpcfPnp.Refresh();
                    }

                    await Task.Delay(TimeSpan.FromMilliseconds(AppConfig.Get("refresh_delay", 500)));

                    settings.Invoke(delegate
                    {
                        InitGPUMode();
                        ScreenControl.AutoScreen();
                    });

                    if (eco == 0)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(AppConfig.Get("nv_delay", 5000)));

                        if (AppConfig.IsNVPlatform())
                        {
                            NvidiaGpuControl.RestartNVService();
                            await Task.Delay(TimeSpan.FromMilliseconds(1000));
                        } else {
                            NvidiaGpuControl.FixNvContainer();
                        }

                        HardwareControl.RecreateGpuControl();
                        Program.modeControl.SetGPUClocks(false);
                    }

                    if (AppConfig.IsModeReapplyRequired())
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(3000));
                        Program.modeControl.AutoPerformance();
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteLine("Error setting GPU Eco: " + ex.Message);
                }

            });

            return true;
        }

        public static bool IsPlugged()
        {
            if (SystemInformation.PowerStatus.PowerLineStatus != PowerLineStatus.Online) return false;
            if (!AppConfig.Is("optimized_usbc")) return true;

            if (AppConfig.ContainsModel("FA507")) Thread.Sleep(1000);

            int chargerMode = Program.acpi.DeviceGet(AsusACPI.ChargerMode);
            Logger.WriteLine("ChargerStatus: " + chargerMode);

            if (chargerMode <= 0) return true;
            return (chargerMode & AsusACPI.ChargerBarrel) > 0;

        }

        public bool AutoGPUMode(bool optimized = false, int delay = 0)
        {

            bool GpuAuto = AppConfig.Is("gpu_auto");
            bool ForceGPU = AppConfig.IsForceSetGPUMode() && !GpuAuto;

            int GpuMode = AppConfig.Get("gpu_mode");

            if (!GpuAuto && !ForceGPU) return false;

            int eco = Program.acpi.DeviceGet(AsusACPI.GPUEco);
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
                        if (delay > 0) Thread.Sleep(delay);
                        return SetGPUEco(0);
                    }
                if (eco == 0)
                    if ((GpuAuto && !IsPlugged()) || (ForceGPU && GpuMode == AsusACPI.GPUModeEco))
                    {

                        if (Program.acpi.IsXGConnected()) return false;
                        if (HardwareControl.IsUsedGPU())
                        {
                            DialogResult dialogResult = MessageBox.Show(Properties.Strings.AlertDGPU, Properties.Strings.AlertDGPUTitle, MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.No) return false;
                        }

                        if (delay > 0) Thread.Sleep(delay);
                        return SetGPUEco(1);
                    }
            }

            return false;

        }


        public void ToggleXGM(bool silent = false)
        {

            Task.Run(async () =>
            {
                settings.LockGPUModes();

                if (Program.acpi.DeviceGet(AsusACPI.GPUXG) == 1)
                {
                    XGM.Reset();
                    HardwareControl.KillGPUApps();

                    if (silent)
                    {
                        Program.acpi.DeviceSet(AsusACPI.GPUXG, 0, "GPU XGM");
                        await Task.Delay(TimeSpan.FromSeconds(15));
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
                            await Task.Delay(TimeSpan.FromSeconds(15));
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

                    await Task.Delay(TimeSpan.FromSeconds(15));

                    if (AppConfig.IsMode("auto_apply"))
                        XGM.SetFan(AppConfig.GetFanConfig(AsusFan.XGM));

                    HardwareControl.RecreateGpuControl();

                }

                settings.Invoke(delegate
                {
                    InitGPUMode();
                });
            });
        }

        public void KillGPUApps()
        {
            if (HardwareControl.GpuControl is not null)
            {
                HardwareControl.GpuControl.KillGPUApps();
            }
        }

    }
}
