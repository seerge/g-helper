using GHelper.Gpu.AMD;
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
        AmdGpuControl amdControl;

        SettingsForm settings;

        ControllerMode mode = ControllerMode.Auto;

        ControllerMode _autoMode = ControllerMode.Gamepad;
        int _autoCount = 0;

        public AllyControl(SettingsForm settingsForm)
        {
            if (!AppConfig.IsAlly()) return;

            settings = settingsForm;

            amdControl = new AmdGpuControl();

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

            mode = (ControllerMode)AppConfig.Get("controller_mode", (int)ControllerMode.Auto);
            SetMode(mode);
        }

        private void SetMode(ControllerMode mode)
        {
            if (mode == ControllerMode.Auto)
            {
                amdControl.StartFPS();
                timer.Start();
            } else
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
