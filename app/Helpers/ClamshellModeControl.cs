using GHelper.Display;
using GHelper.Mode;
using Microsoft.Win32;

namespace GHelper.Helpers
{
    internal class ClamshellModeControl
    {
        public bool IsExternalDisplayConnected()
        {
            var devices = ScreenInterrogatory.GetAllDevices().ToArray();

            string internalName = AppConfig.GetString("internal_display");

            foreach (var device in devices)
            {
                if (device.outputTechnology != ScreenInterrogatory.DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY.DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INTERNAL &&
                    device.outputTechnology != ScreenInterrogatory.DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY.DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EMBEDDED
                    && device.monitorFriendlyDeviceName != internalName)
                {
                    Logger.WriteLine("Found external screen: " + device.monitorFriendlyDeviceName + ":" + device.outputTechnology.ToString());

                    //Already found one, we do not have to check whether there are more
                    return true;
                }

            }

            return false;
        }

        public bool IsClamshellEnabled()
        {
            return AppConfig.Is("toggle_clamshell_mode");
        }

        public bool IsChargerConnected()
        {
            return SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online;
        }

        public bool IsClamshellReady()
        {
            return IsExternalDisplayConnected() && IsChargerConnected();
        }

        public void ToggleLidAction()
        {
            if (!IsClamshellEnabled())
            {
                return;
            }

            if (IsClamshellReady())
            {
                EnableClamshellMode();
            }
            else
            {
                DisableClamshellMode();
            }
        }
        public static void DisableClamshellMode()
        {
            PowerNative.SetLidAction(GetDefaultLidAction(), true);
            Logger.WriteLine("Disengaging Clamshell Mode");
        }

        public static void EnableClamshellMode()
        {
            PowerNative.SetLidAction(0, true);
            Logger.WriteLine("Engaging Clamshell Mode");
        }

        public void UnregisterDisplayEvents()
        {
            SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;
        }

        public void RegisterDisplayEvents()
        {
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
        }

        private void SystemEvents_DisplaySettingsChanged(object? sender, EventArgs e)
        {
            Logger.WriteLine("Display configuration changed.");

            if (IsClamshellEnabled())
                ToggleLidAction();
        }

        //Power users can change that setting.
        //0 = Do nothing
        //1 = Sleep (default)
        //2 = Hibernate
        //3 = Shutdown
        private static int GetDefaultLidAction()
        {
            int val = AppConfig.Get("clamshell_default_lid_action", 1);

            if (val < 0 || val > 3)
            {
                val = 1;
            }

            return val;
        }
    }
}
