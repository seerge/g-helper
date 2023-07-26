using GHelper.Display;
using GHelper.Helpers;
using GHelper.Mode;
using Microsoft.Win32;
using System.Diagnostics;
using System.Management;

namespace GHelper.Input
{

    public class InputDispatcher
    {
        System.Timers.Timer timer = new System.Timers.Timer(1000);
        public static bool backlightActivity = true;

        public static Keys keyProfile = Keys.F5;
        public static Keys keyApp = Keys.F12;

        static ModeControl modeControl = Program.modeControl;
        static ScreenControl screenControl = new ScreenControl();

        KeyboardListener listener;
        KeyboardHook hook = new KeyboardHook();

        public InputDispatcher()
        {

            byte[] result = Program.acpi.DeviceInit();
            Debug.WriteLine($"Init: {BitConverter.ToString(result)}");

            Program.acpi.SubscribeToEvents(WatcherEventArrived);
            //Task.Run(Program.acpi.RunListener);

            hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(KeyPressed);

            RegisterKeys();

            timer.Elapsed += Timer_Elapsed;

        }

        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (GetBacklight() == 0) return;

            TimeSpan iddle = NativeMethods.GetIdleTime();
            int kb_timeout;

            if (SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online)
                kb_timeout = AppConfig.Get("keyboard_ac_timeout", 0);
            else
                kb_timeout = AppConfig.Get("keyboard_timeout", 60);

            if (kb_timeout == 0) return;

            if (backlightActivity && iddle.TotalSeconds > kb_timeout)
            {
                backlightActivity = false;
                AsusUSB.ApplyBrightness(0, "Timeout");
            }

            if (!backlightActivity && iddle.TotalSeconds < kb_timeout)
            {
                backlightActivity = true;
                SetBacklightAuto();
            }

            //Debug.WriteLine(iddle.TotalSeconds);
        }

        public void Init()
        {
            if (listener is not null) listener.Dispose();

            Program.acpi.DeviceInit();

            if (!OptimizationService.IsRunning())
                listener = new KeyboardListener(HandleEvent);
            else
                Logger.WriteLine("Optimization service is running");

            InitBacklightTimer();
        }

        public void InitBacklightTimer()
        {
            timer.Enabled = AppConfig.Get("keyboard_timeout") > 0 && SystemInformation.PowerStatus.PowerLineStatus != PowerLineStatus.Online ||
                            AppConfig.Get("keyboard_ac_timeout") > 0 && SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online;
        }



        public void RegisterKeys()
        {
            hook.UnregisterAll();

            // CTRL + SHIFT + F5 to cycle profiles
            if (AppConfig.Get("keybind_profile") != -1) keyProfile = (Keys)AppConfig.Get("keybind_profile");
            if (AppConfig.Get("keybind_app") != -1) keyApp = (Keys)AppConfig.Get("keybind_app");

            string actionM1 = AppConfig.GetString("m1");
            string actionM2 = AppConfig.GetString("m2");

            if (keyProfile != Keys.None) hook.RegisterHotKey(ModifierKeys.Shift | ModifierKeys.Control, keyProfile);
            if (keyApp != Keys.None) hook.RegisterHotKey(ModifierKeys.Shift | ModifierKeys.Control, keyApp);

            hook.RegisterHotKey(ModifierKeys.Control, Keys.VolumeDown);
            hook.RegisterHotKey(ModifierKeys.Control, Keys.VolumeUp);
            hook.RegisterHotKey(ModifierKeys.Shift, Keys.VolumeDown);
            hook.RegisterHotKey(ModifierKeys.Shift, Keys.VolumeUp);

            if (!AppConfig.ContainsModel("Z13"))
                if (actionM1 is not null && actionM1.Length > 0) hook.RegisterHotKey(ModifierKeys.None, Keys.VolumeDown);
            if (actionM2 is not null && actionM2.Length > 0) hook.RegisterHotKey(ModifierKeys.None, Keys.VolumeUp);

            // FN-Lock group

            if (AppConfig.Is("fn_lock") && !AppConfig.ContainsModel("VivoBook"))
                for (Keys i = Keys.F1; i <= Keys.F11; i++) hook.RegisterHotKey(ModifierKeys.None, i);

        }

        static void CustomKey(string configKey = "m3")
        {
            string command = AppConfig.GetString(configKey + "_custom");
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
                KeyboardHook.KeyPress((Keys)intKey);
            else
                LaunchProcess(command);

        }

        static bool IsManualBrightness()
        {
            return AppConfig.ContainsModel("TUF") && !AppConfig.ContainsModel("FA506");
        }

        public static bool NoMKeys()
        {
            return AppConfig.ContainsModel("Z13") ||
                   AppConfig.ContainsModel("FA506") ||
                   AppConfig.ContainsModel("FX506") ||
                   AppConfig.ContainsModel("Duo") ||
                   AppConfig.ContainsModel("FX505");
        }

        public void KeyPressed(object sender, KeyPressedEventArgs e)
        {

            if (e.Modifier == ModifierKeys.None)
            {
                Logger.WriteLine(e.Key.ToString());

                if (NoMKeys())
                {
                    switch (e.Key)
                    {
                        case Keys.F2:
                            KeyboardHook.KeyPress(Keys.VolumeDown);
                            return;
                        case Keys.F3:
                            KeyboardHook.KeyPress(Keys.VolumeUp);
                            return;
                        case Keys.F4:
                            KeyProcess("m3");
                            return;
                    }
                }

                if (AppConfig.ContainsModel("Z13") || AppConfig.ContainsModel("Duo"))
                {
                    switch (e.Key)
                    {
                        case Keys.F11:
                            HandleEvent(199);
                            return;
                    }
                }

                if (AppConfig.ContainsModel("GA401I") && !AppConfig.ContainsModel("GA401IHR"))
                {
                    switch (e.Key)
                    {
                        case Keys.F2:
                            KeyboardHook.KeyPress(Keys.MediaPreviousTrack);
                            return;
                        case Keys.F3:
                            KeyboardHook.KeyPress(Keys.MediaPlayPause);
                            return;
                        case Keys.F4:
                            KeyboardHook.KeyPress(Keys.MediaNextTrack);
                            return;
                    }
                }


                switch (e.Key)
                {
                    case Keys.F1:
                        KeyboardHook.KeyPress(Keys.VolumeMute);
                        break;
                    case Keys.F2:
                        SetBacklight(-1, true);
                        break;
                    case Keys.F3:
                        SetBacklight(1, true);
                        break;
                    case Keys.F4:
                        KeyProcess("fnf4");
                        break;
                    case Keys.F5:
                        KeyProcess("fnf5");
                        break;
                    case Keys.F6:
                        KeyboardHook.KeyPress(Keys.Snapshot);
                        break;
                    case Keys.F7:
                        if (IsManualBrightness()) Program.toast.RunToast(ScreenBrightness.Adjust(-10) + "%", ToastIcon.BrightnessDown);
                        HandleOptimizationEvent(16);
                        break;
                    case Keys.F8:
                        if (IsManualBrightness()) Program.toast.RunToast(ScreenBrightness.Adjust(+10) + "%", ToastIcon.BrightnessUp);
                        HandleOptimizationEvent(32);
                        break;
                    case Keys.F9:
                        KeyboardHook.KeyWinPress(Keys.P);
                        break;
                    case Keys.F10:
                        HandleOptimizationEvent(107);
                        break;
                    case Keys.F11:
                        HandleOptimizationEvent(108);
                        break;
                    case Keys.F12:
                        KeyboardHook.KeyWinPress(Keys.A);
                        break;
                    case Keys.VolumeDown:
                        KeyProcess("m1");
                        break;
                    case Keys.VolumeUp:
                        KeyProcess("m2");
                        break;
                    default:
                        break;
                }
            }

            if (e.Modifier == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                if (e.Key == keyProfile) modeControl.CyclePerformanceMode();
                if (e.Key == keyApp) Program.SettingsToggle();
            }

            if (e.Modifier == (ModifierKeys.Control))
            {
                switch (e.Key)
                {
                    case Keys.VolumeDown:
                        // Screen brightness down on CTRL+VolDown
                        HandleOptimizationEvent(16);
                        break;
                    case Keys.VolumeUp:
                        // Screen brightness up on CTRL+VolUp
                        HandleOptimizationEvent(32);
                        break;
                }
            }

            if (e.Modifier == (ModifierKeys.Shift))
            {
                switch (e.Key)
                {
                    case Keys.VolumeDown:
                        // Keyboard backlight down on SHIFT+VolDown
                        SetBacklight(-1);
                        break;
                    case Keys.VolumeUp:
                        // Keyboard backlight up on SHIFT+VolUp
                        SetBacklight(1);
                        break;
                }
            }
        }


        public static void KeyProcess(string name = "m3")
        {
            string action = AppConfig.GetString(name);

            if (action is null || action.Length <= 1)
            {
                if (name == "m4")
                    action = "ghelper";
                if (name == "fnf4")
                    action = "aura";
                if (name == "fnf5")
                    action = "performance";
                if (name == "m3" && !OptimizationService.IsRunning())
                    action = "micmute";
                if (name == "fnc")
                    action = "fnlock";
            }

            switch (action)
            {
                case "mute":
                    KeyboardHook.KeyPress(Keys.VolumeMute);
                    break;
                case "play":
                    KeyboardHook.KeyPress(Keys.MediaPlayPause);
                    break;
                case "screenshot":
                    KeyboardHook.KeyPress(Keys.Snapshot);
                    break;
                case "screen":
                    Logger.WriteLine("Screen off toggle");
                    NativeMethods.TurnOffScreen(Program.settingsForm.Handle);
                    break;
                case "miniled":
                    screenControl.ToogleMiniled();
                    break;
                case "aura":
                    Program.settingsForm.BeginInvoke(Program.settingsForm.CycleAuraMode);
                    break;
                case "performance":
                    modeControl.CyclePerformanceMode();
                    break;
                case "ghelper":
                    Program.settingsForm.BeginInvoke(delegate
                    {
                        Program.SettingsToggle();
                    });
                    break;
                case "fnlock":
                    ToggleFnLock();
                    break;
                case "micmute":
                    bool muteStatus = Audio.ToggleMute();
                    Program.toast.RunToast(muteStatus ? "Muted" : "Unmuted", muteStatus ? ToastIcon.MicrophoneMute : ToastIcon.Microphone);
                    break;
                case "brightness_up":
                    HandleOptimizationEvent(32);
                    break;
                case "brightness_down":
                    HandleOptimizationEvent(16);
                    break;
                case "screenpad_up":
                    SetScreenpad(10);
                    break;
                case "screenpad_down":
                    SetScreenpad(-10);
                    break;
                case "custom":
                    CustomKey(name);
                    break;

                default:
                    break;
            }
        }

        static bool GetTouchpadState()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\PrecisionTouchPad\Status", false))
            {
                Logger.WriteLine("Touchpad status:" + key?.GetValue("Enabled")?.ToString());
                return key?.GetValue("Enabled")?.ToString() == "1";
            }
        }

        static void ToggleFnLock()
        {
            int fnLock = AppConfig.Is("fn_lock") ? 0 : 1;
            AppConfig.Set("fn_lock", fnLock);

            if (AppConfig.ContainsModel("VivoBook"))
                Program.acpi.DeviceSet(AsusACPI.FnLock, fnLock == 1 ? 0 : 1, "FnLock");
            else
                Program.settingsForm.BeginInvoke(Program.inputDispatcher.RegisterKeys);

            Program.toast.RunToast("Fn-Lock " + (fnLock == 1 ? "On" : "Off"), ToastIcon.FnLock);
        }

        public static void TabletMode()
        {
            if (AppConfig.Is("disable_tablet")) return;

            bool touchpadState = GetTouchpadState();
            bool tabletState = Program.acpi.DeviceGet(AsusACPI.TabletState) > 0;

            Logger.WriteLine("Tablet: " + tabletState + " Touchpad: " + touchpadState);

            if (tabletState && touchpadState || !tabletState && !touchpadState) AsusUSB.TouchpadToggle();

        }

        static void HandleEvent(int EventID)
        {
            switch (EventID)
            {
                case 124:    // M3
                    KeyProcess("m3");
                    return;
                case 56:    // M4 / Rog button
                case 181:    // FN + Numpad Enter
                    KeyProcess("m4");
                    return;
                case 174:   // FN+F5
                    modeControl.CyclePerformanceMode();
                    return;
                case 179:   // FN+F4
                case 178:   // FN+F4
                    KeyProcess("fnf4");
                    return;
                case 158:   // Fn + C
                    KeyProcess("fnc");
                    return;
                case 78:    // Fn + ESC
                    ToggleFnLock();
                    return;
                case 189: // Tablet mode 
                    TabletMode();
                    return;
                case 197: // FN+F2
                    SetBacklight(-1);
                    return;
                case 196: // FN+F3
                    SetBacklight(1);
                    return;
                case 199: // ON Z13 - FN+F11 - cycles backlight
                    SetBacklight(4);
                    return;
                case 53:    // FN+F6 on GA-502DU model
                    NativeMethods.TurnOffScreen(Program.settingsForm.Handle);
                    return;
            }

            if (!OptimizationService.IsRunning())
                HandleOptimizationEvent(EventID);

        }

        // Asus Optimization service Events 
        static void HandleOptimizationEvent(int EventID)
        {
            switch (EventID)
            {
                case 16: // FN+F7
                    //ScreenBrightness.Adjust(-10);
                    Program.acpi.DeviceSet(AsusACPI.UniversalControl, AsusACPI.Brightness_Down, "Brightness");
                    break;
                case 32: // FN+F8
                    //ScreenBrightness.Adjust(+10);
                    Program.acpi.DeviceSet(AsusACPI.UniversalControl, AsusACPI.Brightness_Up, "Brightness");
                    break;
                case 107: // FN+F10
                    AsusUSB.TouchpadToggle();
                    Thread.Sleep(50);
                    Program.toast.RunToast(GetTouchpadState() ? "On" : "Off", ToastIcon.Touchpad);
                    break;
                case 108: // FN+F11
                    Program.acpi.DeviceSet(AsusACPI.UniversalControl, AsusACPI.KB_Sleep, "Sleep");
                    break;
            }
        }


        public static int GetBacklight()
        {
            int backlight_power = AppConfig.Get("keyboard_brightness", 1);
            int backlight_battery = AppConfig.Get("keyboard_brightness_ac", 1);
            bool onBattery = SystemInformation.PowerStatus.PowerLineStatus != PowerLineStatus.Online;

            int backlight;

            //backlight = onBattery ? Math.Min(backlight_battery, backlight_power) : Math.Max(backlight_battery, backlight_power);
            backlight = onBattery ? backlight_battery : backlight_power;

            return Math.Max(Math.Min(3, backlight), 0);
        }

        public static void SetBacklightAuto(bool init = false)
        {
            if (init) AsusUSB.Init();

            //if (!OptimizationService.IsRunning()) 
            AsusUSB.ApplyBrightness(GetBacklight(), "Auto");
        }

        public static void SetBacklight(int delta, bool force = false)
        {
            int backlight_power = AppConfig.Get("keyboard_brightness", 1);
            int backlight_battery = AppConfig.Get("keyboard_brightness_ac", 1);
            bool onBattery = SystemInformation.PowerStatus.PowerLineStatus != PowerLineStatus.Online;

            int backlight = onBattery ? backlight_battery : backlight_power;

            if (delta >= 4)
                backlight = ++backlight % 4;
            else
                backlight = Math.Max(Math.Min(3, backlight + delta), 0);

            if (onBattery)
                AppConfig.Set("keyboard_brightness_ac", backlight);
            else
                AppConfig.Set("keyboard_brightness", backlight);

            if (force || !OptimizationService.IsRunning())
            {
                AsusUSB.ApplyBrightness(backlight, "HotKey");
            }

            string[] backlightNames = new string[] { "Off", "Low", "Mid", "Max" };
            Program.toast.RunToast(backlightNames[backlight], delta > 0 ? ToastIcon.BacklightUp : ToastIcon.BacklightDown);

        }

        public static void SetScreenpad(int delta)
        {
            int brightness = AppConfig.Get("screenpad", 100);
            brightness = Math.Max(Math.Min(100, brightness + delta), 0);

            AppConfig.Set("screenpad", brightness);

            Program.acpi.DeviceSet(AsusACPI.ScreenPadBrightness, (brightness * 255 / 100), "Screenpad");
            if (brightness == 0) Program.acpi.DeviceSet(AsusACPI.ScreenPadToggle, brightness, "ScreenpadToggle");

            Program.toast.RunToast($"Screen Pad {brightness}", delta > 0 ? ToastIcon.BrightnessUp : ToastIcon.BrightnessDown);

        }


        static void LaunchProcess(string command = "")
        {

            try
            {

                //string executable = command.Split(' ')[0];
                //string arguments = command.Substring(executable.Length).Trim();

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.FileName = command;
                //startInfo.Arguments = arguments;
                Process proc = Process.Start(startInfo);
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
