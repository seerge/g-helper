using Microsoft.Win32;
using NAudio.CoreAudioApi;
using System.Diagnostics;
using System.Management;
using Tools;

namespace GHelper
{
    public class InputDispatcher
    {

        private static bool isOptimizationRunning = OptimizationService.IsRunning();
        private static nint windowHandle;

        public InputDispatcher(nint handle)
        {

            windowHandle = handle;

            Program.acpi.SubscribeToEvents(WatcherEventArrived);

            if (!isOptimizationRunning) AsusUSB.RunListener(HandleEvent);

            // CTRL + SHIFT + F5 to cycle profiles
            Keys keybind_profile = (AppConfig.getConfig("keybind_profile") != -1) ? (Keys)AppConfig.getConfig("keybind_profile") : Keys.F5;
            if (keybind_profile != 0)
            {
                KeyHandler ghk = new KeyHandler(KeyHandler.SHIFT | KeyHandler.CTRL, keybind_profile, windowHandle);
                ghk.Register();
            }

            /*
            KeyHandler m1 = new KeyHandler(0, Keys.VolumeDown, ds);
            m1.Register();
            KeyHandler m2 = new KeyHandler(0, Keys.VolumeUp, ds);
            m2.Register();
            */

        }

        static void CustomKey(string configKey = "m3")
        {
            string command = AppConfig.getConfigString(configKey + "_custom");
            int intKey;

            try
            {
                intKey = Convert.ToInt32(command, 16);
            }
            catch
            {
                intKey = -1;
            }


            if (intKey > 0)
                NativeMethods.KeyPress(intKey);
            else
                LaunchProcess(command);

        }

        static void KeyProcess(string name = "m3")
        {
            string action = AppConfig.getConfigString(name);

            if (action is null || action.Length <= 1)
            {
                if (name == "m4")
                    action = "ghelper";
                if (name == "fnf4")
                    action = "aura";
                if (name == "m3" && !isOptimizationRunning)
                    action = "micmute";
            }

            switch (action)
            {
                case "mute":
                    NativeMethods.KeyPress(NativeMethods.VK_VOLUME_MUTE);
                    break;
                case "play":
                    NativeMethods.KeyPress(NativeMethods.VK_MEDIA_PLAY_PAUSE);
                    break;
                case "screenshot":
                    NativeMethods.KeyPress(NativeMethods.VK_SNAPSHOT);
                    break;
                case "screen":
                    NativeMethods.TurnOffScreen(Program.settingsForm.Handle);
                    break;
                case "miniled":
                    Program.settingsForm.BeginInvoke(Program.settingsForm.ToogleMiniled);
                    break;
                case "aura":
                    Program.settingsForm.BeginInvoke(Program.settingsForm.CycleAuraMode);
                    break;
                case "performance":
                    Program.settingsForm.BeginInvoke(Program.settingsForm.CyclePerformanceMode);
                    break;
                case "ghelper":
                    Program.settingsForm.BeginInvoke(delegate
                    {
                        Program.SettingsToggle();
                    });
                    break;
                case "custom":
                    CustomKey(name);
                    break;
                case "micmute":
                    using (var enumerator = new MMDeviceEnumerator())
                    {
                        var commDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications);
                        bool muteStatus = !commDevice.AudioEndpointVolume.Mute;
                        commDevice.AudioEndpointVolume.Mute = muteStatus;
                        Program.settingsForm.BeginInvoke(Program.settingsForm.RunToast, muteStatus ? "Muted" : "Unmuted", muteStatus ? ToastIcon.MicrophoneMute : ToastIcon.Microphone);
                    }
                    break;

                default:
                    break;
            }
        }

        static bool GetTouchpadState()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\PrecisionTouchPad\Status", false))
            {
                return (key?.GetValue("Enabled")?.ToString() == "1");
            }
        }

        static void TabletMode()
        {
            bool touchpadState = GetTouchpadState();
            bool tabletState = Program.acpi.DeviceGet(AsusACPI.TabletState) > 0;

            Logger.WriteLine("Tablet: " + tabletState + " Touchpad: " + touchpadState);

            if ((tabletState && touchpadState) || (!tabletState && !touchpadState)) AsusUSB.TouchpadToggle();

        }

        static void HandleEvent(int EventID)
        {
            switch (EventID)
            {
                case 124:    // M3
                    KeyProcess("m3");
                    return;
                case 56:    // M4 / Rog button
                    KeyProcess("m4");
                    return;
                case 174:   // FN+F5
                    Program.settingsForm.BeginInvoke(Program.settingsForm.CyclePerformanceMode);
                    return;
                case 179:   // FN+F4
                    KeyProcess("fnf4");
                    return;
                case 189: // Tablet mode 
                    TabletMode();
                    return;
            }

            if (isOptimizationRunning) return;

            // Asus Optimization service Events 

            int backlight = AppConfig.getConfig("keyboard_brightness");

            string[] backlightNames = new string[] { "Off", "Low", "Mid", "Max" };

            int brightness;

            switch (EventID)
            {
                case 197: // FN+F2
                    backlight = Math.Max(0, backlight - 1);
                    AppConfig.setConfig("keyboard_brightness", backlight);
                    AsusUSB.ApplyBrightness(backlight);
                    Program.settingsForm.BeginInvoke(Program.settingsForm.RunToast, backlightNames[backlight], ToastIcon.BacklightDown);
                    break;
                case 196: // FN+F3
                    backlight = Math.Min(3, backlight + 1);
                    AppConfig.setConfig("keyboard_brightness", backlight);
                    AsusUSB.ApplyBrightness(backlight);
                    Program.settingsForm.BeginInvoke(Program.settingsForm.RunToast, backlightNames[backlight], ToastIcon.BacklightUp);
                    break;
                case 16: // FN+F7
                    brightness = ScreenBrightness.Adjust(-10);
                    Program.settingsForm.BeginInvoke(Program.settingsForm.RunToast, brightness + "%", ToastIcon.BrightnessDown);
                    break;
                case 32: // FN+F8
                    brightness = ScreenBrightness.Adjust(+10);
                    Program.settingsForm.BeginInvoke(Program.settingsForm.RunToast, brightness + "%", ToastIcon.BrightnessUp);
                    break;
                case 107: // FN+F10
                    bool touchpadState = GetTouchpadState();
                    AsusUSB.TouchpadToggle();
                    Program.settingsForm.BeginInvoke(Program.settingsForm.RunToast, touchpadState ? "Off" : "On", ToastIcon.Touchpad);
                    break;
                case 108: // FN+F11
                    Program.acpi.DeviceSet(AsusACPI.UniversalControl, 0x6c, "Sleep");
                    //NativeMethods.SetSuspendState(false, true, true);
                    break;
            }
        }


        static void LaunchProcess(string command = "")
        {
            string executable = command.Split(' ')[0];
            string arguments = command.Substring(executable.Length).Trim();

            try
            {
                Process proc = Process.Start(executable, arguments);
            }
            catch
            {
                Logger.WriteLine("Failed to run  " + command);
            }


        }



        static void WatcherEventArrived(object sender, EventArrivedEventArgs e)
        {
            if (e.NewEvent is null) return;
            int EventID = int.Parse(e.NewEvent["EventID"].ToString());
            Logger.WriteLine("WMI event " + EventID);
            HandleEvent(EventID);
        }
    }
}
