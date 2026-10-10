using GHelper.Gpu.AMD;

namespace GHelper.Mode
{
    public class AutoTDPControl
    {
        public static AmdGpuControl amdControl = new AmdGpuControl();

        static System.Timers.Timer timer = default!;

        SettingsForm settings;

        static int tdpMin = 6;
        static int tdpStable = tdpMin;
        static int tdpCurrent = -1;

        static bool autoTDP = false;

        static int fpsLimit = -1;

        static int _upCount = 0;
        static int _downCount = 0;

        public AutoTDPControl(SettingsForm settingsForm)
        {
            settings = settingsForm;

            if (timer is null)
            {
                timer = new System.Timers.Timer(300);
                timer.Elapsed += Timer_Elapsed;
            }
        }

        public void Init()
        {
            if (!AppConfig.IsAutoTDP()) return;

            settings.VisualiseAutoTDPPanel();

            fpsLimit = amdControl.GetFPSLimit();
            settings.VisualiseFPSLimit(fpsLimit);
        }

        private int GetMaxTDP()
        {
            int tdp = AppConfig.GetMode("limit_total");
            if (tdp > 0) return tdp;

            if (!AppConfig.IsAlly()) return AsusACPI.MaxTotal;

            switch (Modes.GetCurrentBase())
            {
                case 1:
                    return 25;
                case 2:
                    return 10;
                default:
                    return 15;
            }
        }

        private int GetTDP()
        {
            if (tdpCurrent < 0) tdpCurrent = GetMaxTDP();
            return tdpCurrent;
        }

        private void SetTDP(int tdp, string log)
        {
            if (tdp < tdpStable) tdp = tdpStable;

            int max = GetMaxTDP();
            if (tdp > max) tdp = max;

            if (tdp == tdpCurrent) return;
            if (!autoTDP) return;

            Program.acpi.DeviceSet(AsusACPI.PPT_APUA0, tdp, log);
            Program.acpi.DeviceSet(AsusACPI.PPT_APUA3, tdp, null);
            Program.acpi.DeviceSet(AsusACPI.PPT_APUC1, tdp, null);

            tdpCurrent = tdp;
        }

        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (!autoTDP || fpsLimit <= 0 || fpsLimit > 120) return;

            float fps = amdControl.GetFPS();
            int power = amdControl.GetiGpuPower();

            if (fps <= Math.Min(fpsLimit * 0.9, fpsLimit - 4)) _upCount++;
            else _upCount = 0;

            if (fps >= Math.Min(fpsLimit * 0.95, fpsLimit - 2)) _downCount++;
            else _downCount = 0;

            var tdp = GetTDP();
            if (_upCount >= 1)
            {
                _downCount = 0;
                _upCount = 0;
                SetTDP(tdp + 1, $"AutoTDP+ [{power}]{fps}");
            }

            if (_downCount >= 8 && power < tdp)
            {
                _upCount = 0;
                _downCount--;
                SetTDP(tdp - 1, $"AutoTDP- [{power}]{fps}");
            }
        }

        public void ToggleAutoTDP()
        {
            autoTDP = !autoTDP;
            tdpCurrent = -1;

            if (autoTDP)
            {
                if (!AppConfig.IsAlly()) amdControl.StartFPS();
                timer.Start();
            }
            else
            {
                timer.Stop();
                if (!AppConfig.IsAlly()) amdControl.StopFPS();
                Program.modeControl.SetPerformanceMode();
            }

            settings.VisualiseAutoTDP(autoTDP);
        }

        public void ToggleFPSLimit(bool toast = false)
        {
            switch (fpsLimit)
            {
                case 30:
                    fpsLimit = 40;
                    break;
                case 40:
                    fpsLimit = 45;
                    break;
                case 45:
                    fpsLimit = 50;
                    break;
                case 50:
                    fpsLimit = 60;
                    break;
                case 60:
                    fpsLimit = 75;
                    break;
                case 75:
                    fpsLimit = 90;
                    break;
                case 90:
                    fpsLimit = 120;
                    break;
                case 120:
                    fpsLimit = -1;
                    break;
                default:
                    fpsLimit = 30;
                    break;
            }

            int result = (fpsLimit > 0) ? amdControl.SetFPSLimit(fpsLimit) : amdControl.ResetFPSLimit();
            Logger.WriteLine($"FPS Limit {fpsLimit}: {result}");

            settings.VisualiseFPSLimit(fpsLimit);
            if (toast) Program.toast.RunToast("FPS Limit " + ((fpsLimit > 0 && fpsLimit <= 120) ? fpsLimit : "OFF"));

        }

    }
}
