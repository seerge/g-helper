using GHelper.Display;
using GHelper.Mode;
using Microsoft.Win32;

namespace GHelper.Helpers
{
    internal class ClamshellModeControl
    {

        public ClamshellModeControl()
        {
            //Save current setting if hibernate or shutdown to prevent reverting the user set option.
            CheckAndSaveLidAction();
        }

        public bool IsExternalDisplayConnected()
        {
            try
            {
                var devicesList = ScreenInterrogatory.GetAllDevices();
                var devices = devicesList.ToArray();

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
            } catch (Exception ex)
            {
                Logger.WriteLine(ex.ToString());
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
            return IsExternalDisplayConnected() && (IsChargerConnected() || AppConfig.Is("clamshell_battery"));
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
            if (PowerNative.GetLidAction(true) == GetDefaultLidAction()) return;
            PowerNative.SetLidAction(GetDefaultLidAction(), true);
            Logger.WriteLine("Disengaging Clamshell Mode");
        }

        public static void EnableClamshellMode()
        {
            if (PowerNative.GetLidAction(true) == 0) return;
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

            if (Program.settingsForm.Visible)
                ScreenControl.InitScreen();

            if (AppConfig.IsForceMiniled())
                ScreenControl.InitMiniled();

        }

        private static int CheckAndSaveLidAction()
        {
            if (AppConfig.Get("clamshell_default_lid_action", -1) != -1)
            {
                //Seting was alredy set. Do not touch it
                return AppConfig.Get("clamshell_default_lid_action", -1);
            }

            int val = PowerNative.GetLidAction(true);
            //If it is 0 then it is likely already set by clamshell mdoe
            //If 0 was set by the user, then why do they even use clamshell mode?
            //We only care about hibernate or shutdown setting here
            if (val == 2 || val == 3)
            {
                AppConfig.Set("clamshell_default_lid_action", val);
                return val;
            }

            return 1;
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
