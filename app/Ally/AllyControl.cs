using GHelper.USB;

namespace GHelper.Ally
{

    public enum ControllerMode : int
    {
        Gamepad = 1,
        WASD = 2,
        Mouse = 3,
    }

    public class AllyControl
    {
        SettingsForm settings;
        ControllerMode mode = ControllerMode.Gamepad;
        public AllyControl(SettingsForm settingsForm)
        {
            settings = settingsForm;
        }

        public void Init()
        {
            if (!AppConfig.IsAlly())
            {
                settings.VisualiseAlly(false);
                return;
            }

            mode = (ControllerMode)AppConfig.Get("controller_mode", (int)ControllerMode.Gamepad);
            SetMode(mode);
        }

        private void SetMode(ControllerMode mode)
        {
            AppConfig.Set("controller_mode", (int)mode);
            AsusHid.WriteInput(new byte[] { AsusHid.INPUT_ID, 0xd1, 1, 1, (byte)mode }, "ControllerMode");
            settings.VisualiseController(mode);
        }

        public void ToggleMode()
        {
            if (mode == ControllerMode.Mouse)
                mode = ControllerMode.Gamepad;
            else
                mode++;

            SetMode(mode);
        }

    }
}
