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
            return AppConfig.Get("toggle_clamshell_mode") != 0;
        }

        public bool IsChargerConnected()
        {
            return SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online;
        }

        public bool IsInClamshellMode()
        {
            return IsExternalDisplayConnected() && IsChargerConnected();
        }

        public void ToggleLidAction()
        {
            if (IsInClamshellMode() && IsClamshellEnabled())
            {
                PowerNative.SetLidAction(0, true);
                Logger.WriteLine("Engaging Clamshell Mode");
            }
            else
            {
                PowerNative.SetLidAction(1, true);
                Logger.WriteLine("Disengaging Clamshell Mode");
            }
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
    }
}
