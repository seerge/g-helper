using GHelper.Gpu.NVidia;
using GHelper.Helpers;
using GHelper.USB;
using Ryzen;

namespace GHelper.Mode
{
    public class ModeControl
    {

        static SettingsForm settings = Program.settingsForm;

        private static bool customFans = false;
        private static int customPower = 0;

        private int _cpuUV = 0;
        private int _igpuUV = 0;

        static System.Timers.Timer reapplyTimer = default!;

        public ModeControl()
        {
            reapplyTimer = new System.Timers.Timer(AppConfig.GetMode("reapply_time", 30) * 1000);
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


        public void ResetPerformanceMode()
        {
            ResetRyzen();

            Program.acpi.DeviceSet(AsusACPI.PerformanceMode, Modes.GetCurrentBase(), "Mode");

            // Default power mode
            AppConfig.RemoveMode("powermode");
            PowerNative.SetPowerMode(Modes.GetCurrentBase());
        }

        public void SetPerformanceMode(int mode = -1, bool notify = false)
        {

            int oldMode = Modes.GetCurrent();
            if (mode < 0) mode = oldMode;

            if (!Modes.Exists(mode)) mode = 0;

            settings.ShowMode(mode);

            Modes.SetCurrent(mode);


            Task.Run(async () =>
            {
                bool reset = AppConfig.IsResetRequired() && (Modes.GetBase(oldMode) == Modes.GetBase(mode)) && customPower > 0 && !AppConfig.IsMode("auto_apply_power");

                customFans = false;
                customPower = 0;
                SetModeLabel();

                // Workaround for not properly resetting limits on G14 2024
                if (reset)
                {
                    Program.acpi.DeviceSet(AsusACPI.PerformanceMode, (Modes.GetBase(oldMode) != 1) ? AsusACPI.PerformanceTurbo : AsusACPI.PerformanceBalanced, "ModeReset");
                    await Task.Delay(TimeSpan.FromMilliseconds(1500));
                }

                int status = Program.acpi.DeviceSet(AsusACPI.PerformanceMode, AppConfig.IsManualModeRequired() ? AsusACPI.PerformanceManual : Modes.GetBase(mode), "Mode");
                // Vivobook fallback
                if (status != 1) Program.acpi.SetVivoMode(Modes.GetBase(mode));

                SetGPUClocks();

                await Task.Delay(TimeSpan.FromMilliseconds(100));
                AutoFans();
                await Task.Delay(TimeSpan.FromMilliseconds(1000));
                AutoPower();


            });


            if (AppConfig.Is("xgm_fan") && Program.acpi.IsXGConnected()) XGM.Reset();

            if (notify)
                Program.toast.RunToast(Modes.GetCurrentName(), SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online ? ToastIcon.Charger : ToastIcon.Battery);



            // Power plan from config or defaulting to balanced
            if (AppConfig.GetModeString("scheme") is not null)
                PowerNative.SetPowerPlan(AppConfig.GetModeString("scheme"));
            else
                PowerNative.SetBalancedPowerPlan();

            // Windows power mode
            if (AppConfig.GetModeString("powermode") is not null)
                PowerNative.SetPowerMode(AppConfig.GetModeString("powermode"));
            else
                PowerNative.SetPowerMode(Modes.GetBase(mode));

            // CPU Boost setting override
            if (AppConfig.GetMode("auto_boost") != -1)
                PowerNative.SetCPUBoost(AppConfig.GetMode("auto_boost"));

            //BatteryControl.SetBatteryChargeLimit();

            /*
            if (NativeMethods.PowerGetEffectiveOverlayScheme(out Guid activeScheme) == 0)
            {
                Debug.WriteLine("Effective :" + activeScheme);
            }
            */

            settings.FansInit();
        }


        public void CyclePerformanceMode(bool back = false)
        {
            SetPerformanceMode(Modes.GetNext(back), true);
        }

        public void AutoFans(bool force = false)
        {
            customFans = false;

            if (AppConfig.IsMode("auto_apply") || force)
            {

                bool xgmFan = false;
                if (AppConfig.Is("xgm_fan") && Program.acpi.IsXGConnected())
                {
                    XGM.SetFan(AppConfig.GetFanConfig(AsusFan.XGM));
                    xgmFan = true;
                }

                int cpuResult = Program.acpi.SetFanCurve(AsusFan.CPU, AppConfig.GetFanConfig(AsusFan.CPU));
                int gpuResult = Program.acpi.SetFanCurve(AsusFan.GPU, AppConfig.GetFanConfig(AsusFan.GPU));

                if (AppConfig.Is("mid_fan"))
                    Program.acpi.SetFanCurve(AsusFan.Mid, AppConfig.GetFanConfig(AsusFan.Mid));


                // Alternative way to set fan curve
                if (cpuResult != 1 || gpuResult != 1)
                {
                    cpuResult = Program.acpi.SetFanRange(AsusFan.CPU, AppConfig.GetFanConfig(AsusFan.CPU));
                    gpuResult = Program.acpi.SetFanRange(AsusFan.GPU, AppConfig.GetFanConfig(AsusFan.GPU));

                    // Something went wrong, resetting to default profile
                    if (cpuResult != 1 || gpuResult != 1)
                    {
                        Program.acpi.DeviceSet(AsusACPI.PerformanceMode, Modes.GetCurrentBase(), "Reset Mode");
                        settings.LabelFansResult("Model doesn't support custom fan curves");
                    }
                }
                else
                {
                    settings.LabelFansResult("");
                    customFans = true;
                }

                // force set PPTs for missbehaving bios on FX507/517 series
                if ((AppConfig.IsPowerRequired() || xgmFan) && !AppConfig.IsMode("auto_apply_power"))
                {
                    Task.Run(async () =>
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1));
                        Program.acpi.DeviceSet(AsusACPI.PPT_APUA0, 80, "PowerLimit Fix A0");
                        Program.acpi.DeviceSet(AsusACPI.PPT_APUA3, 80, "PowerLimit Fix A3");
                    });
                }

            }

            SetModeLabel();

        }

        public void AutoPower(bool launchAsAdmin = false)
        {

            customPower = 0;

            bool applyPower = AppConfig.IsMode("auto_apply_power");
            bool applyFans = AppConfig.IsMode("auto_apply");

            if (applyPower && !applyFans && (AppConfig.IsFanRequired() || AppConfig.IsManualModeRequired()))
            {
                AutoFans(true);
                Thread.Sleep(500);
            }

            if (applyPower) SetPower(launchAsAdmin);

            Thread.Sleep(500);
            SetGPUPower();
            AutoRyzen();

        }

        public void SetModeLabel()
        {
            settings.SetModeLabel(Properties.Strings.PerformanceMode + ": " + Modes.GetCurrentName() + (customFans ? "+" : "") + ((customPower > 0) ? " " + customPower + "W" : ""));
        }

        public void SetPower(bool launchAsAdmin = false)
        {

            bool allAMD = Program.acpi.IsAllAmdPPT();

            int limit_total = AppConfig.GetMode("limit_total");
            int limit_cpu = AppConfig.GetMode("limit_cpu");
            int limit_slow = AppConfig.GetMode("limit_slow");
            int limit_fast = AppConfig.GetMode("limit_fast");

            if (limit_slow < 0 || allAMD) limit_slow = limit_total;

            if (limit_total > AsusACPI.MaxTotal) return;
            if (limit_total < AsusACPI.MinTotal) return;

            if (limit_cpu > AsusACPI.MaxCPU) return;
            if (limit_cpu < AsusACPI.MinCPU) return;

            if (limit_fast > AsusACPI.MaxTotal) return;
            if (limit_fast < AsusACPI.MinTotal) return;

            if (limit_slow > AsusACPI.MaxTotal) return;
            if (limit_slow < AsusACPI.MinTotal) return;

            // SPL and SPPT 
            if (Program.acpi.DeviceGet(AsusACPI.PPT_APUA0) >= 0)
            {
                Program.acpi.DeviceSet(AsusACPI.PPT_APUA3, limit_total, "PowerLimit A3");
                Program.acpi.DeviceSet(AsusACPI.PPT_APUA0, limit_slow, "PowerLimit A0");
                customPower = limit_total;
            }
            else if (RyzenControl.IsAMD())
            {

                if (ProcessHelper.IsUserAdministrator())
                {
                    var stapmResult = SendCommand.set_stapm_limit((uint)limit_total * 1000);
                    Logger.WriteLine($"STAPM: {limit_total} {stapmResult}");

                    var stapmResult2 = SendCommand.set_stapm2_limit((uint)limit_total * 1000);
                    Logger.WriteLine($"STAPM2: {limit_total} {stapmResult2}");

                    var slowResult = SendCommand.set_slow_limit((uint)limit_total * 1000);
                    Logger.WriteLine($"SLOW: {limit_total} {slowResult}");

                    var fastResult = SendCommand.set_fast_limit((uint)limit_total * 1000);
                    Logger.WriteLine($"FAST: {limit_total} {fastResult}");

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
            Task.Run(() =>
            {

                int core = AppConfig.GetMode("gpu_core");
                int memory = AppConfig.GetMode("gpu_memory");
                int clock_limit = AppConfig.GetMode("gpu_clock_limit");

                if (core == -1 && memory == -1 && clock_limit == -1) return;
                //if ((gpu_core > -5 && gpu_core < 5) && (gpu_memory > -5 && gpu_memory < 5)) launchAsAdmin = false;

                if (Program.acpi.DeviceGet(AsusACPI.GPUEco) == 1) { Logger.WriteLine("Clocks: Eco"); return; }
                if (HardwareControl.GpuControl is null) { Logger.WriteLine("Clocks: NoGPUControl"); return; }
                if (!HardwareControl.GpuControl!.IsNvidia) { Logger.WriteLine("Clocks: NotNvidia"); return; }

                using NvidiaGpuControl nvControl = (NvidiaGpuControl)HardwareControl.GpuControl;
                try
                {
                    int statusLimit = nvControl.SetMaxGPUClock(clock_limit);
                    int statusClocks = nvControl.SetClocks(core, memory);
                    if ((statusLimit != 0 || statusClocks != 0) && launchAsAdmin) ProcessHelper.RunAsAdmin("gpu");
                }
                catch (Exception ex)
                {
                    Logger.WriteLine("Clocks Error:" + ex.ToString());
                }

                settings.GPUInit();
            });
        }

        public void SetGPUPower()
        {

            int gpu_boost = AppConfig.GetMode("gpu_boost");
            int gpu_temp = AppConfig.GetMode("gpu_temp");
            int gpu_power = AppConfig.GetMode("gpu_power");

            int boostResult = -1;

            if (gpu_power >= AsusACPI.MinGPUPower && gpu_power <= AsusACPI.MaxGPUPower && Program.acpi.DeviceGet(AsusACPI.GPU_POWER) >= 0)
                Program.acpi.DeviceSet(AsusACPI.GPU_POWER, gpu_power, "PowerLimit TGP (GPU VAR)");

            if (gpu_boost >= AsusACPI.MinGPUBoost && gpu_boost <= AsusACPI.MaxGPUBoost && Program.acpi.DeviceGet(AsusACPI.PPT_GPUC0) >= 0) 
                boostResult = Program.acpi.DeviceSet(AsusACPI.PPT_GPUC0, gpu_boost, "PowerLimit C0 (GPU BOOST)");

            if (gpu_temp >= AsusACPI.MinGPUTemp && gpu_temp <= AsusACPI.MaxGPUTemp && Program.acpi.DeviceGet(AsusACPI.PPT_GPUC2) >= 0)
                Program.acpi.DeviceSet(AsusACPI.PPT_GPUC2, gpu_temp, "PowerLimit C2 (GPU TEMP)");

            // Fallback
            if (boostResult == 0)
                Program.acpi.DeviceSet(AsusACPI.PPT_GPUC0, gpu_boost, "PowerLimit C0");

        }

        public void SetCPUTemp(int? cpuTemp, bool log = true)
        {
            if (cpuTemp >= RyzenControl.MinTemp && cpuTemp < RyzenControl.MaxTemp)
            {
                var resultCPU = SendCommand.set_tctl_temp((uint)cpuTemp);
                if (log) Logger.WriteLine($"CPU Temp: {cpuTemp} {resultCPU}");

                var restultAPU = SendCommand.set_apu_skin_temp_limit((uint)cpuTemp);
                if (log) Logger.WriteLine($"APU Temp: {cpuTemp} {restultAPU}");

                reapplyTimer.Enabled = AppConfig.IsMode("auto_uv");

            }
            else
            {
                reapplyTimer.Enabled = false;
            }
        }

        public void SetUV(int cpuUV)
        {
            if (!RyzenControl.IsSupportedUV()) return;

            if (cpuUV >= RyzenControl.MinCPUUV && cpuUV <= RyzenControl.MaxCPUUV)
            {
                var uvResult = SendCommand.set_coall(cpuUV);
                Logger.WriteLine($"UV: {cpuUV} {uvResult}");
                if (uvResult == Smu.Status.OK) _cpuUV = cpuUV;
            }
        }

        public void SetUViGPU(int igpuUV)
        {
            if (!RyzenControl.IsSupportedUViGPU()) return;

            if (igpuUV >= RyzenControl.MinIGPUUV && igpuUV <= RyzenControl.MaxIGPUUV)
            {
                var iGPUResult = SendCommand.set_cogfx(igpuUV);
                Logger.WriteLine($"iGPU UV: {igpuUV} {iGPUResult}");
                if (iGPUResult == Smu.Status.OK) _igpuUV = igpuUV;
            }
        }


        public void SetRyzen(bool launchAsAdmin = false)
        {
            if (!ProcessHelper.IsUserAdministrator())
            {
                if (launchAsAdmin) ProcessHelper.RunAsAdmin("uv");
                return;
            }

            if (!RyzenControl.IsRingExsists()) return;

            try
            {
                SetUV(AppConfig.GetMode("cpu_uv", 0));
                SetUViGPU(AppConfig.GetMode("igpu_uv", 0));
                SetCPUTemp(AppConfig.GetMode("cpu_temp"));
            }
            catch (Exception ex)
            {
                Logger.WriteLine("UV Error: " + ex.ToString());
            }
        }

        public void ResetRyzen()
        {
            if (_cpuUV != 0) SetUV(0);
            if (_igpuUV != 0) SetUViGPU(0);
        }

        public void AutoRyzen()
        {
            if (!RyzenControl.IsAMD()) return;

            if (AppConfig.IsMode("auto_uv")) SetRyzen();
            else ResetRyzen();
        }

    }
}
