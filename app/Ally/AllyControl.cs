using GHelper.Gpu.AMD;
using GHelper.Input;
using GHelper.USB;

namespace GHelper.Ally
{

    public enum ControllerMode : int
    {
        Auto = 0,
        Gamepad = 1,
        Mouse = 3,
    }

    public class AllyControl
    {
        System.Timers.Timer timer = default!;
        static AmdGpuControl amdControl = new AmdGpuControl();

        SettingsForm settings;

        static ControllerMode mode = ControllerMode.Auto;
        static ControllerMode _autoMode = ControllerMode.Auto;
        static int _autoCount = 0;

        static int fpsLimit = -1;

        public AllyControl(SettingsForm settingsForm)
        {
            if (!AppConfig.IsAlly()) return;

            settings = settingsForm;

            timer = new System.Timers.Timer(500);
            timer.Elapsed += Timer_Elapsed;

        }

        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            float fps = amdControl.GetFPS();

            ControllerMode _newMode = (fps > 0) ? ControllerMode.Gamepad : ControllerMode.Mouse;

            if (_autoMode != _newMode) _autoCount++;
            else _autoCount = 0;

            if (_autoCount > 2)
            {
                _autoMode = _newMode;
                _autoCount = 0;
                AsusHid.WriteInput(new byte[] { AsusHid.INPUT_ID, 0xd1, 1, 1, (byte)_autoMode }, "ControllerAuto " + _autoMode);
                Logger.WriteLine(fps.ToString());
            }

        }

        public void Init()
        {
            if (AppConfig.IsAlly()) settings.VisualiseAlly(true);
            else return;

            Deadzones();
            SetMode((ControllerMode)AppConfig.Get("controller_mode", (int)ControllerMode.Auto));
            
            settings.VisualiseBacklight(InputDispatcher.GetBacklight());

            fpsLimit = amdControl.GetFPSLimit();
            Logger.WriteLine($"FPS Limit: {fpsLimit}");
            settings.VisualiseFPSLimit(fpsLimit);

        }

        public void ToggleFPSLimit()
        {
            switch (fpsLimit)
            {
                case 30:
                    fpsLimit = 40;
                    break;
                case 40:
                    fpsLimit = 60;
                    break;
                case 60:
                    fpsLimit = 120;
                    break;
                default:
                    fpsLimit = 30;
                    break;
            }

            int result = amdControl.SetFPSLimit(fpsLimit);
            Logger.WriteLine($"FPS Limit {fpsLimit}: {result}");

            settings.VisualiseFPSLimit(fpsLimit);

        }


        public void ToggleBacklight()
        {
            InputDispatcher.SetBacklight(4, true);
            settings.VisualiseBacklight(InputDispatcher.GetBacklight());
        }

        static public void Deadzones()
        {
            AsusHid.WriteInput(new byte[] { AsusHid.INPUT_ID, 0xd1, 4, 4, 
                (byte)AppConfig.Get("ls_min", 0), 
                (byte)AppConfig.Get("ls_max", 100), 
                (byte)AppConfig.Get("rs_min", 0), 
                (byte)AppConfig.Get("rs_max", 100) }, 
                "StickDeadzone");

            AsusHid.WriteInput(new byte[] { AsusHid.INPUT_ID, 0xd1, 5, 4,
                (byte)AppConfig.Get("lt_min", 0),
                (byte)AppConfig.Get("lt_max", 100),
                (byte)AppConfig.Get("rt_min", 0),
                (byte)AppConfig.Get("rt_max", 100) },
                "TriggerDeadzone");

            AsusHid.WriteInput(new byte[] { AsusHid.INPUT_ID, 0xd1, 6, 2,
                (byte)AppConfig.Get("vibra", 100),
                (byte)AppConfig.Get("vibra", 100) },
                "Vibration");

        }

        private void SetMode(ControllerMode mode)
        {
            if (mode == ControllerMode.Auto)
            {
                _autoMode = ControllerMode.Auto;
                amdControl.StartFPS();
                timer.Start();
            }
            else
            {
                timer.Stop();
                amdControl.StopFPS();

                AsusHid.WriteInput(new byte[] { AsusHid.INPUT_ID, 0xd1, 1, 1, (byte)mode }, "ControllerMode");
            }

            AppConfig.Set("controller_mode", (int)mode);
            settings.VisualiseController(mode);
        }

        public void ToggleMode()
        {

            switch (mode)
            {
                case ControllerMode.Auto:
                    mode = ControllerMode.Gamepad;
                    break;
                case ControllerMode.Gamepad:
                    mode = ControllerMode.Mouse;
                    break;
                case ControllerMode.Mouse:
                    mode = ControllerMode.Auto;
                    break;
            }

            SetMode(mode);
        }

    }
}
