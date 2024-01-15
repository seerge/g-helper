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

        const int CodeM1 = 0x028f;
        const int CodeM2 = 0x028e;

        public static Dictionary<int, string> BindingCodes = new Dictionary<int, string>
        {
            { 0x028f, "M1" },
            { 0x028e, "M2" },
            { 0x0000, "-------" },
            { 0x0101, "A" },
            { 0x0102, "B" },
            { 0x0103, "X" },
            { 0x0104, "Y" },
            { 0x0105, "left bumper" },
            { 0x0106, "right bumper" },
            { 0x0107, "left stick click" },
            { 0x0108, "right stick click" },
            { 0x0109, "dpad up" },
            { 0x010A, "dpad down" },
            { 0x010B, "dpad left" },
            { 0x010C, "dpad right" },
            { 0x0111, "view button" },
            { 0x0112, "menu button" },
            { 0x0113, "XBox/Steam" },
            { 0x0276, "Esc" },
            { 0x0250, "F1" },
            { 0x0260, "F2" },
            { 0x0240, "F3" },
            { 0x020C, "F4" },
            { 0x0203, "F5" },
            { 0x020b, "F6" },
            { 0x0280, "F7" },
            { 0x020a, "F8" },
            { 0x0201, "F9" },
            { 0x0209, "F10" },
            { 0x0278, "F11" },
            { 0x0207, "F12" },
            { 0x020E, "`" },
            { 0x0216, "1" },
            { 0x021E, "2" },
            { 0x0226, "3" },
            { 0x0225, "4" },
            { 0x022E, "5" },
            { 0x0236, "6" },
            { 0x023D, "7" },
            { 0x023E, "8" },
            { 0x0246, "9" },
            { 0x0245, "0" },
            { 0x024E, "-" },
            { 0x0255, "=" },
            { 0x0266, "Backspace" },
            { 0x020D, "Tab" },
            { 0x0215, "Q" },
            { 0x021D, "W" },
            { 0x0224, "E" },
            { 0x022D, "R" },
            { 0x022C, "T" },
            { 0x0235, "Y" },
            { 0x023C, "U" },
            { 0x0244, "O" },
            { 0x024D, "P" },
            { 0x0254, "[" },
            { 0x025B, "]" },
            { 0x025D, "|" },
            { 0x0258, "Caps" },
            { 0x021C, "A" },
            { 0x021B, "S" },
            { 0x0223, "D" },
            { 0x022B, "F" },
            { 0x0234, "G" },
            { 0x0233, "H" },
            { 0x023B, "J" },
            { 0x0242, "k" },
            { 0x024b, "l" },
            { 0x024c, ";" },
            { 0x0252, "'" },
            { 0x025A, "Enter" },
            { 0x0288, "LShift" },
            { 0x0222, "X" },
            { 0x021A, "Z" },
            { 0x0221, "C" },
            { 0x022A, "V" },
            { 0x0232, "B" },
            { 0x0231, "N" },
            { 0x023A, "M" },
            { 0x0241, "," },
            { 0x0249, "." },
            { 0x0289, "RShift" },
            { 0x028C, "LCtl" },
            { 0x0282, "Meta" },
            { 0x028A, "LAlt" },
            { 0x0229, "Space" },
            { 0x028B, "RAlt" },
            { 0x0284, "App menu" },
            { 0x028D, "RCtl" },
            { 0x02C3, "PrntScn" },
            { 0x027E, "ScrLk" },
            { 0x02C2, "Insert" },
            { 0x0294, "Home" },
            { 0x0296, "PgUp" },
            { 0x02C0, "Delete" },
            { 0x0295, "End" },
            { 0x0297, "PgDwn" },
            { 0x0298, "UpArrow" },
            { 0x0299, "DownArrow" },
            { 0x0291, "LeftArrow" },
            { 0x029B, "RightArrow" },
            { 0x0277, "NumLock" },
            { 0x0290, "NumSlash" },
            { 0x027C, "NumStar" },
            { 0x027B, "NumHyphen" },
            { 0x0270, "Num0" },
            { 0x0269, "Num1" },
            { 0x0272, "Num2" },
            { 0x027A, "Num3" },
            { 0x026B, "Num4" },
            { 0x0273, "Num5" },
            { 0x0274, "Num6" },
            { 0x026C, "Num7" },
            { 0x0275, "Num8" },
            { 0x027D, "Num9" },
            { 0x0279, "NumPlus" },
            { 0x0281, "NumEnter" },
            { 0x0271, "NumPeriod" },
            { 0x0301, "Mouse left click" },
            { 0x0302, "Mouse right click" },
            { 0x0303, "Mouse middle click" },
            { 0x0304, "Mouse scroll up" },
            { 0x0305, "Mouse scroll down" },
            { 0x0516, "Screenshot" },
            { 0x0519, "Show keyboard" },
            { 0x051c, "Show desktop" },
            { 0x051e, "Begin recording" },
            { 0x0501, "Mic off" },
            { 0x0502, "Vol Down" },
            { 0x0503, "Vol Up" }
        };

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
                ApplyMode(_autoMode, "ControllerAuto");
                Logger.WriteLine(fps.ToString());
            }

        }

        public void Init()
        {
            if (AppConfig.IsAlly()) settings.VisualiseAlly(true);
            else return;

            SetDeadzones();
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

        static private byte[] DecodeBinding(int binding)
        {
            byte command = (byte)(binding & 0xFF);
            byte device = (byte)((binding >> 8) & 0xFF);

            byte[] code = new byte[10];
            code[0] = device;
            switch (device)
            {
                case 0x02:
                    code[2] = command;
                    break;
                case 0x03:
                    code[4] = command;
                    break;
                case 0x05:
                    code[3] = command;
                    break;
                default:
                    code[1] = command;
                    break;
            }

            return code;
        }

        static public void SetBindings()
        {
            int M1 = AppConfig.Get("bind_m1", CodeM1);
            int M2 = AppConfig.Get("bind_m2", CodeM2);

            byte[] bindings = new byte[50];
            byte[] init = new byte[] { AsusHid.INPUT_ID, 0xd1, 0x02, 0x08, 0x2c };

            init.CopyTo(bindings, 0);
            DecodeBinding(M2).CopyTo(bindings, 5);
            if (M2 == CodeM2) DecodeBinding(M2).CopyTo(bindings, 16);
            DecodeBinding(M1).CopyTo(bindings, 27);
            if (M1 == CodeM1) DecodeBinding(M2).CopyTo(bindings, 16);

            AsusHid.WriteInput(bindings, "Bind");
        }

        static public void SetDeadzones()
        {
            AsusHid.WriteInput(new byte[] { AsusHid.INPUT_ID, 0xd1, 4, 4,
                (byte)AppConfig.Get("ls_min", 0),
                (byte)AppConfig.Get("ls_max", 100),
                (byte)AppConfig.Get("rs_min", 0),
                (byte)AppConfig.Get("rs_max", 100)
            }, "StickDeadzone");

            AsusHid.WriteInput(new byte[] { AsusHid.INPUT_ID, 0xd1, 5, 4,
                (byte)AppConfig.Get("lt_min", 0),
                (byte)AppConfig.Get("lt_max", 100),
                (byte)AppConfig.Get("rt_min", 0),
                (byte)AppConfig.Get("rt_max", 100)
            }, "TriggerDeadzone");

            AsusHid.WriteInput(new byte[] { AsusHid.INPUT_ID, 0xd1, 6, 2,
                (byte)AppConfig.Get("vibra", 100),
                (byte)AppConfig.Get("vibra", 100)
            }, "Vibration");

        }

        private void ApplyMode(ControllerMode applyMode, string log = "ControllerMode")
        {
            AsusHid.WriteInput(new byte[] { AsusHid.INPUT_ID, 0xd1, 1, 1, (byte)applyMode }, log);
            SetBindings();
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
                ApplyMode(mode);
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
