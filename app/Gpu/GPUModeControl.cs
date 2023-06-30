using GHelper.Display;
using GHelper.Gpu.NVidia;
using GHelper.Helpers;
using System.Diagnostics;

namespace GHelper.Gpu
{
    public class GPUModeControl
    {
        SettingsForm settings;
        ScreenControl screenControl = new ScreenControl();

        public GPUModeControl(SettingsForm settingsForm)
        {
            settings = settingsForm;
        }

        public void InitGPUMode()
        {
            int eco = Program.acpi.DeviceGet(AsusACPI.GPUEco);
            int mux = Program.acpi.DeviceGet(AsusACPI.GPUMux);

            Logger.WriteLine("Eco flag : " + eco);
            Logger.WriteLine("Mux flag : " + mux);

            int GpuMode;

            if (mux == 0)
            {
                GpuMode = AsusACPI.GPUModeUltimate;
            }
            else
            {
                if (eco == 1)
                    GpuMode = AsusACPI.GPUModeEco;
                else
                    GpuMode = AsusACPI.GPUModeStandard;

                // Ultimate mode not suported
                if (mux != 1) settings.HideUltimateMode();
                // GPU mode not supported
                if (eco < 0 && mux < 0) settings.HideGPUModes();
            }

            AppConfig.Set("gpu_mode", GpuMode);

            InitXGM();
            settings.VisualiseGPUMode(GpuMode);
        }


        public void SetGPUMode(int GPUMode)
        {

            int CurrentGPU = AppConfig.Get("gpu_mode");
            AppConfig.Set("gpu_auto", 0);

            if (CurrentGPU == GPUMode)
            {
                settings.VisualiseGPUMode();
                return;
            }

            var restart = false;
            var changed = false;

            if (CurrentGPU == AsusACPI.GPUModeUltimate)
            {
                DialogResult dialogResult = MessageBox.Show(Properties.Strings.AlertUltimateOff, Properties.Strings.AlertUltimateTitle, MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Program.acpi.DeviceSet(AsusACPI.GPUMux, 1, "GPUMux");
                    restart = true;
                    changed = true;
                }
            }
            else if (GPUMode == AsusACPI.GPUModeUltimate)
            {
                DialogResult dialogResult = MessageBox.Show(Properties.Strings.AlertUltimateOn, Properties.Strings.AlertUltimateTitle, MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Program.acpi.DeviceSet(AsusACPI.GPUMux, 0, "GPUMux");
                    restart = true;
                    changed = true;
                }

            }
            else if (GPUMode == AsusACPI.GPUModeEco)
            {
                settings.VisualiseGPUMode(GPUMode);
                SetGPUEco(1, true);
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



        public void SetGPUEco(int eco, bool hardWay = false)
        {

            settings.LockGPUModes();

            Task.Run(async () =>
            {

                int status = 1;

                if (eco == 1)
                {
                    if (NvidiaSmi.GetDisplayActiveStatus())
                    {
                        DialogResult dialogResult = MessageBox.Show(Properties.Strings.EnableOptimusText, Properties.Strings.EnableOptimusTitle, MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.No)
                        {
                            InitGPUMode();
                            return;
                        }
                    }

                    HardwareControl.KillGPUApps();
                }

                Logger.WriteLine($"Running eco command {eco}");

                status = Program.acpi.SetGPUEco(eco);

                if (status == 0 && eco == 1 && hardWay) RestartGPU();

                await Task.Delay(TimeSpan.FromMilliseconds(100));
                
                settings.Invoke(delegate
                {
                    InitGPUMode();
                    screenControl.AutoScreen();
                });

                if (eco == 0)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(3000));
                    HardwareControl.RecreateGpuControl();
                    Program.modeControl.SetGPUClocks(false);
                }

            });


        }

        public static bool IsPlugged()
        {
            bool optimizedUSBC = AppConfig.Get("optimized_usbc") != 1;

            return SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online &&
                   (optimizedUSBC || Program.acpi.DeviceGet(AsusACPI.ChargerMode) < AsusACPI.ChargerUSB);

        }

        public bool AutoGPUMode()
        {

            bool GpuAuto = AppConfig.Is("gpu_auto");
            bool ForceGPU = AppConfig.ContainsModel("503");

            int GpuMode = AppConfig.Get("gpu_mode");

            if (!GpuAuto && !ForceGPU) return false;

            int eco = Program.acpi.DeviceGet(AsusACPI.GPUEco);
            int mux = Program.acpi.DeviceGet(AsusACPI.GPUMux);

            if (mux == 0) // GPU in Ultimate, ignore
                return false;
            else
            {

                if (ReEnableGPU()) return true;

                if (eco == 1)
                    if ((GpuAuto && IsPlugged()) || (ForceGPU && GpuMode == AsusACPI.GPUModeStandard))
                    {
                        SetGPUEco(0);
                        return true;
                    }
                if (eco == 0)
                    if ((GpuAuto && !IsPlugged()) || (ForceGPU && GpuMode == AsusACPI.GPUModeEco))
                    {

                        if (HardwareControl.IsUsedGPU())
                        {
                            DialogResult dialogResult = MessageBox.Show(Properties.Strings.AlertDGPU, Properties.Strings.AlertDGPUTitle, MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.No) return false;
                        }

                        SetGPUEco(1);
                        return true;
                    }
            }

            return false;

        }


        public void RestartGPU(bool confirm = true)
        {
            if (HardwareControl.GpuControl is null) return;
            if (!HardwareControl.GpuControl!.IsNvidia) return;

            if (confirm)
            {
                DialogResult dialogResult = MessageBox.Show(Properties.Strings.RestartGPU, Properties.Strings.EcoMode, MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No) return;
            }

            ProcessHelper.RunAsAdmin("gpurestart");

            if (!ProcessHelper.IsUserAdministrator()) return;

            Logger.WriteLine("Trying to restart dGPU");

            Task.Run(async () =>
            {
                settings.LockGPUModes("Restarting GPU ...");

                var nvControl = (NvidiaGpuControl)HardwareControl.GpuControl;
                bool status = nvControl.RestartGPU();

                settings.Invoke(delegate
                {
                    //labelTipGPU.Text = status ? "GPU Restarted, you can try Eco mode again" : "Failed to restart GPU"; TODO
                    InitGPUMode();
                });
            });

        }


        public bool ReEnableGPU()
        {

            if (AppConfig.Get("gpu_reenable") != 1) return false;
            if (Screen.AllScreens.Length <= 1) return false;

            Logger.WriteLine("Re-enabling gpu for 503 model");

            Thread.Sleep(1000);
            SetGPUEco(1);
            Thread.Sleep(1000);
            SetGPUEco(0);
            return true;
        }

        public void InitXGM()
        {
            bool connected = Program.acpi.IsXGConnected();
            int activated = Program.acpi.DeviceGet(AsusACPI.GPUXG);
            settings.VisualizeXGM(connected, activated == 1);
        }

        public void ToggleXGM()
        {

            Task.Run(async () =>
            {
                settings.LockGPUModes();

                if (Program.acpi.DeviceGet(AsusACPI.GPUXG) == 1)
                {
                    HardwareControl.KillGPUApps();
                    DialogResult dialogResult = MessageBox.Show("Did you close all applications running on XG Mobile?", "Disabling XG Mobile", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        Program.acpi.DeviceSet(AsusACPI.GPUXG, 0, "GPU XGM");
                        await Task.Delay(TimeSpan.FromSeconds(15));
                    }
                }
                else
                {
                    Program.acpi.DeviceSet(AsusACPI.GPUXG, 1, "GPU XGM");
                    AsusUSB.ApplyXGMLight(AppConfig.Is("xmg_light"));

                    await Task.Delay(TimeSpan.FromSeconds(15));

                    if (AppConfig.IsMode("auto_apply"))
                        AsusUSB.SetXGMFan(AppConfig.GetFanConfig(AsusFan.XGM));
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
