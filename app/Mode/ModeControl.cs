using GHelper.Gpu.NVidia;
using GHelper.Helpers;
using Ryzen;

namespace GHelper.Mode
{
    public class ModeControl
    {

        static SettingsForm settings = Program.settingsForm;

        private static bool customFans = false;
        private static int customPower = 0;

        static System.Timers.Timer reapplyTimer = default!;

        public ModeControl()
        {
            reapplyTimer = new System.Timers.Timer(5000);
            reapplyTimer.Elapsed += ReapplyTimer_Elapsed;
            reapplyTimer.Enabled = false;
        }

        private void ReapplyTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            SetCPUTemp(AppConfig.GetMode("cpu_temp"), false);
        }

        public void AutoPerformance(bool powerChanged = false)
        {
            var Plugged = SystemInformation.PowerStatus.PowerLineStatus;

            int mode = AppConfig.Get("performance_" + (int)Plugged);

            if (mode != -1)
                SetPerformanceMode(mode, powerChanged);
            else
                SetPerformanceMode(Modes.GetCurrent());
        }

        public void SetPerformanceMode(int mode = -1, bool notify = false)
        {

            int oldMode = Modes.GetCurrent();
            if (mode < 0) mode = oldMode;

            if (!Modes.Exists(mode)) mode = 0;

            customFans = false;
            customPower = 0;

            settings.ShowMode(mode);
            SetModeLabel();

            Modes.SetCurrent(mode);

            Program.acpi.DeviceSet(AsusACPI.PerformanceMode, IsManualModeRequired() ? AsusACPI.PerformanceManual : Modes.GetBase(mode), "Mode");

            if (AppConfig.Is("xgm_fan") && Program.acpi.IsXGConnected()) AsusUSB.ResetXGM();

            if (notify)
                Program.toast.RunToast(Modes.GetCurrentName(), SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online ? ToastIcon.Charger : ToastIcon.Battery);

            SetGPUClocks();
            AutoFans();
            AutoPower(1000);

            if (AppConfig.Get("auto_apply_power_plan") != 0)
            {
                if (AppConfig.GetModeString("scheme") is not null)
                    NativeMethods.SetPowerScheme(AppConfig.GetModeString("scheme"));
                else
                    NativeMethods.SetPowerScheme(Modes.GetBase(mode));
            }

            if (AppConfig.GetMode("auto_boost") != -1)
            {
                NativeMethods.SetCPUBoost(AppConfig.GetMode("auto_boost"));
            }

            /*
            if (NativeMethods.PowerGetEffectiveOverlayScheme(out Guid activeScheme) == 0)
            {
                Debug.WriteLine("Effective :" + activeScheme);
            }
            */

            settings.FansInit();
        }


        public void CyclePerformanceMode()
        {
            SetPerformanceMode(Modes.GetNext(Control.ModifierKeys == Keys.Shift), true);
        }

        public void AutoFans(bool force = false)
        {
            customFans = false;

            if (AppConfig.IsMode("auto_apply") || force)
            {

                bool xgmFan = false;
                if (AppConfig.Is("xgm_fan") && Program.acpi.IsXGConnected())
                {
                    AsusUSB.SetXGMFan(AppConfig.GetFanConfig(AsusFan.XGM));
                    xgmFan = true;
                }

                int cpuResult = Program.acpi.SetFanCurve(AsusFan.CPU, AppConfig.GetFanConfig(AsusFan.CPU));
                int gpuResult = Program.acpi.SetFanCurve(AsusFan.GPU, AppConfig.GetFanConfig(AsusFan.GPU));

                if (AppConfig.Is("mid_fan"))
                    Program.acpi.SetFanCurve(AsusFan.Mid, AppConfig.GetFanConfig(AsusFan.Mid));


                // something went wrong, resetting to default profile
                if (cpuResult != 1 || gpuResult != 1)
                {
                    int mode = Modes.GetCurrentBase();
                    Logger.WriteLine("ASUS BIOS rejected fan curve, resetting mode to " + mode);
                    Program.acpi.DeviceSet(AsusACPI.PerformanceMode, mode, "Reset Mode");
                    settings.LabelFansResult("ASUS BIOS rejected fan curve");
                }
                else
                {
                    settings.LabelFansResult("");
                    customFans = true;
                }

                // force set PPTs for missbehaving bios on FX507/517 series
                if ((AppConfig.ContainsModel("FX507") || AppConfig.ContainsModel("FX517") || xgmFan) && !AppConfig.IsMode("auto_apply_power"))
                {
                    Task.Run(async () =>
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1));
                        Program.acpi.DeviceSet(AsusACPI.PPT_TotalA0, 80, "PowerLimit Fix A0");
                        Program.acpi.DeviceSet(AsusACPI.PPT_APUA3, 80, "PowerLimit Fix A3");
                    });
                }

            }

            SetModeLabel();

        }

        private static bool IsManualModeRequired()
        {
            if (!AppConfig.IsMode("auto_apply_power"))
                return false;

            return
                AppConfig.Is("manual_mode") ||
                AppConfig.ContainsModel("GU603") ||
                AppConfig.ContainsModel("GU604") ||
                AppConfig.ContainsModel("G733");
        }

        public void AutoPower(int delay = 0)
        {

            customPower = 0;

            bool applyPower = AppConfig.IsMode("auto_apply_power");
            bool applyFans = AppConfig.IsMode("auto_apply");
            //bool applyGPU = true;

            if (applyPower)
            {
                // force fan curve for misbehaving bios PPTs on G513
                if (AppConfig.ContainsModel("G513") && !applyFans)
                {
                    delay = 500;
                    AutoFans(true);
                }

                // Fix for models that don't support PPT settings in all modes, setting a "manual" mode for them
                if (IsManualModeRequired() && !applyFans)
                {
                    AutoFans(true);
                }
            }

            if (delay > 0)
            {
                var timer = new System.Timers.Timer(delay);
                timer.Elapsed += delegate
                {
                    timer.Stop();
                    timer.Dispose();

                    if (applyPower) SetPower();
                    SetGPUPower();
                    AutoUV();
                };
                timer.Start();
            }
            else
            {
                if (applyPower) SetPower(true);
                SetGPUPower();
                AutoUV();
            }

        }

        public void SetModeLabel()
        {
            settings.SetModeLabel(Properties.Strings.PerformanceMode + ": " + Modes.GetCurrentName() + (customFans ? "+" : "") + ((customPower > 0) ? " " + customPower + "W" : ""));
        }

        public void SetPower(bool launchAsAdmin = false)
        {

            int limit_total = AppConfig.GetMode("limit_total");
            int limit_cpu = AppConfig.GetMode("limit_cpu");
            int limit_fast = AppConfig.GetMode("limit_fast");

            if (limit_total > AsusACPI.MaxTotal) return;
            if (limit_total < AsusACPI.MinTotal) return;

            if (limit_cpu > AsusACPI.MaxCPU) return;
            if (limit_cpu < AsusACPI.MinCPU) return;

            if (limit_fast > AsusACPI.MaxTotal) return;
            if (limit_fast < AsusACPI.MinTotal) return;

            // SPL + SPPT togeher in one slider
            if (Program.acpi.DeviceGet(AsusACPI.PPT_TotalA0) >= 0)
            {
                Program.acpi.DeviceSet(AsusACPI.PPT_TotalA0, limit_total, "PowerLimit A0");
                Program.acpi.DeviceSet(AsusACPI.PPT_APUA3, limit_total, "PowerLimit A3");
                customPower = limit_total;
            }
            else if (RyzenControl.IsAMD())
            {

                if (ProcessHelper.IsUserAdministrator())
                {
                    SendCommand.set_stapm_limit((uint)limit_total * 1000);
                    SendCommand.set_stapm2_limit((uint)limit_total * 1000);
                    SendCommand.set_slow_limit((uint)limit_total * 1000);
                    SendCommand.set_fast_limit((uint)limit_total * 1000);
                    customPower = limit_total;
                }
                else if (launchAsAdmin)
                {
                    ProcessHelper.RunAsAdmin("cpu");
                    return;
                }
            }

            if (Program.acpi.IsAllAmdPPT()) // CPU limit all amd models
            {
                Program.acpi.DeviceSet(AsusACPI.PPT_CPUB0, limit_cpu, "PowerLimit B0");
                customPower = limit_cpu;
            }
            else if (Program.acpi.DeviceGet(AsusACPI.PPT_APUC1) >= 0) // FPPT boost for non all-amd models
            {
                Program.acpi.DeviceSet(AsusACPI.PPT_APUC1, limit_fast, "PowerLimit C1");
                customPower = limit_fast;
            }


            SetModeLabel();

        }

        public void SetGPUClocks(bool launchAsAdmin = true)
        {

            int gpu_core = AppConfig.GetMode("gpu_core");
            int gpu_memory = AppConfig.GetMode("gpu_memory");

            if (gpu_core == -1 && gpu_memory == -1) return;

            //if ((gpu_core > -5 && gpu_core < 5) && (gpu_memory > -5 && gpu_memory < 5)) launchAsAdmin = false;

            if (Program.acpi.DeviceGet(AsusACPI.GPUEco) == 1) return;
            if (HardwareControl.GpuControl is null) return;
            if (!HardwareControl.GpuControl!.IsNvidia) return;

            using NvidiaGpuControl nvControl = (NvidiaGpuControl)HardwareControl.GpuControl;
            try
            {
                int getStatus = nvControl.GetClocks(out int current_core, out int current_memory);
                if (getStatus != -1)
                {
                    if (Math.Abs(gpu_core - current_core) < 5 && Math.Abs(gpu_memory - current_memory) < 5) return;
                }

                int setStatus = nvControl.SetClocks(gpu_core, gpu_memory);
                if (launchAsAdmin && setStatus == -1) ProcessHelper.RunAsAdmin("gpu");

            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.ToString());
            }
        }

        public void SetGPUPower()
        {

            int gpu_boost = AppConfig.GetMode("gpu_boost");
            int gpu_temp = AppConfig.GetMode("gpu_temp");


            if (gpu_boost < AsusACPI.MinGPUBoost || gpu_boost > AsusACPI.MaxGPUBoost) return;
            if (gpu_temp < AsusACPI.MinGPUTemp || gpu_temp > AsusACPI.MaxGPUTemp) return;

            if (Program.acpi.DeviceGet(AsusACPI.PPT_GPUC0) >= 0)
                Program.acpi.DeviceSet(AsusACPI.PPT_GPUC0, gpu_boost, "PowerLimit C0");

            if (Program.acpi.DeviceGet(AsusACPI.PPT_GPUC2) >= 0)
                Program.acpi.DeviceSet(AsusACPI.PPT_GPUC2, gpu_temp, "PowerLimit C2");

        }


        public void AutoUV()
        {
            if (!AppConfig.IsMode("auto_uv")) return;
            SetUV();
        }

        public void SetCPUTemp(int? cpuTemp, bool log = true)
        {
            if (cpuTemp >= RyzenControl.MinTemp && cpuTemp <= RyzenControl.MaxTemp)
            {
                var resultCPU = SendCommand.set_tctl_temp((uint)cpuTemp);
                if (log) Logger.WriteLine($"CPU Temp: {cpuTemp} {resultCPU}");

                var restultAPU = SendCommand.set_apu_skin_temp_limit((uint)cpuTemp);
                if (log) Logger.WriteLine($"APU Temp: {cpuTemp} {restultAPU}");

                reapplyTimer.Enabled = AppConfig.IsMode("auto_uv");

            } else
            {
                reapplyTimer.Enabled = false;
            }
        }

        public void SetUV(bool launchAsAdmin = false)
        {

            if (!ProcessHelper.IsUserAdministrator())
            {
                if (launchAsAdmin) ProcessHelper.RunAsAdmin("uv");
                return;
            }

            if (!RyzenControl.IsAMD()) return;

            int cpuUV = AppConfig.GetMode("cpu_uv", 0);
            int igpuUV = AppConfig.GetMode("igpu_uv", 0);
            int cpuTemp = AppConfig.GetMode("cpu_temp");

            try
            {
                if (cpuUV >= RyzenControl.MinCPUUV && cpuUV <= RyzenControl.MaxCPUUV)
                {
                    var uvResult = SendCommand.set_coall(cpuUV);
                    Logger.WriteLine($"UV: {cpuUV} {uvResult}");
                }

                if (igpuUV >= RyzenControl.MinIGPUUV && igpuUV <= RyzenControl.MaxIGPUUV)
                {
                    var iGPUResult = SendCommand.set_cogfx(igpuUV);
                    Logger.WriteLine($"iGPU UV: {igpuUV} {iGPUResult}");
                }

                SetCPUTemp(cpuTemp);

            }
            catch (Exception ex)
            {
                Logger.WriteLine("UV Error: " + ex.ToString());
            }
        }

    }
}
