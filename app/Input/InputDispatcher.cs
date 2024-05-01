using GHelper.Display;
using GHelper.Helpers;
using GHelper.Mode;
using GHelper.USB;
using Microsoft.Win32;
using System.Diagnostics;
using System.Management;
using System.Text.RegularExpressions;

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

        static bool isTUF = AppConfig.IsTUF();

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
                Aura.ApplyBrightness(0, "Timeout");
            }

            if (!backlightActivity && iddle.TotalSeconds < kb_timeout)
            {
                backlightActivity = true;
                SetBacklightAuto();
            }

            //Logger.WriteLine("Iddle: " + iddle.TotalSeconds);
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

            if (AppConfig.IsVivoZenbook())
                Program.acpi.DeviceSet(AsusACPI.FnLock, AppConfig.Is("fn_lock") ^ AppConfig.IsInvertedFNLock() ? 1 : 0, "FnLock");

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

            if (keyProfile != Keys.None)
            {
                hook.RegisterHotKey(ModifierKeys.Shift | ModifierKeys.Control, keyProfile);
                hook.RegisterHotKey(ModifierKeys.Shift | ModifierKeys.Control | ModifierKeys.Alt, keyProfile);
            }

            if (keyApp != Keys.None) hook.RegisterHotKey(ModifierKeys.Shift | ModifierKeys.Control, keyApp);

            if (!AppConfig.Is("skip_hotkeys"))
            {
                hook.RegisterHotKey(ModifierKeys.Shift | ModifierKeys.Control | ModifierKeys.Alt, Keys.F13);

                hook.RegisterHotKey(ModifierKeys.Shift | ModifierKeys.Control | ModifierKeys.Alt, Keys.F14);
                hook.RegisterHotKey(ModifierKeys.Shift | ModifierKeys.Control | ModifierKeys.Alt, Keys.F15);

                hook.RegisterHotKey(ModifierKeys.Shift | ModifierKeys.Control | ModifierKeys.Alt, Keys.F16);
                hook.RegisterHotKey(ModifierKeys.Shift | ModifierKeys.Control | ModifierKeys.Alt, Keys.F17);
                hook.RegisterHotKey(ModifierKeys.Shift | ModifierKeys.Control | ModifierKeys.Alt, Keys.F18);
                hook.RegisterHotKey(ModifierKeys.Shift | ModifierKeys.Control | ModifierKeys.Alt, Keys.F19);
                hook.RegisterHotKey(ModifierKeys.Shift | ModifierKeys.Control | ModifierKeys.Alt, Keys.F20);



                hook.RegisterHotKey(ModifierKeys.Control, Keys.VolumeDown);
                hook.RegisterHotKey(ModifierKeys.Control, Keys.VolumeUp);
                hook.RegisterHotKey(ModifierKeys.Shift, Keys.VolumeDown);
                hook.RegisterHotKey(ModifierKeys.Shift, Keys.VolumeUp);
                hook.RegisterHotKey(ModifierKeys.Shift | ModifierKeys.Control, Keys.F20);
            }

            if (!AppConfig.IsZ13() && !AppConfig.IsAlly())
            {
                if (actionM1 is not null && actionM1.Length > 0) hook.RegisterHotKey(ModifierKeys.None, Keys.VolumeDown);
                if (actionM2 is not null && actionM2.Length > 0) hook.RegisterHotKey(ModifierKeys.None, Keys.VolumeUp);
            }

            if (AppConfig.IsAlly())
            {
                hook.RegisterHotKey(ModifierKeys.Shift | ModifierKeys.Control | ModifierKeys.Alt, Keys.F1);
                hook.RegisterHotKey(ModifierKeys.Shift | ModifierKeys.Control | ModifierKeys.Alt, Keys.F2);
                hook.RegisterHotKey(ModifierKeys.Shift | ModifierKeys.Control | ModifierKeys.Alt, Keys.F3);
                hook.RegisterHotKey(ModifierKeys.Shift | ModifierKeys.Control | ModifierKeys.Alt, Keys.F4);
            }

            // FN-Lock group

            if (AppConfig.Is("fn_lock") && !AppConfig.IsVivoZenbook())
                for (Keys i = Keys.F1; i <= Keys.F11; i++) hook.RegisterHotKey(ModifierKeys.None, i);

            // Arrow-lock group
            if (AppConfig.Is("arrow_lock") && AppConfig.IsDUO())
            {
                hook.RegisterHotKey(ModifierKeys.None, Keys.Left);
                hook.RegisterHotKey(ModifierKeys.None, Keys.Right);
                hook.RegisterHotKey(ModifierKeys.None, Keys.Up);
                hook.RegisterHotKey(ModifierKeys.None, Keys.Down);
            }

        }


        public static int[] ParseHexValues(string input)
        {
            string pattern = @"\b(0x[0-9A-Fa-f]{1,2}|[0-9A-Fa-f]{1,2})\b";

            if (!Regex.IsMatch(input, $"^{pattern}(\\s+{pattern})*$")) return new int[0];

            MatchCollection matches = Regex.Matches(input, pattern);

            int[] hexValues = new int[matches.Count];

            for (int i = 0; i < matches.Count; i++)
            {
                string hexValueStr = matches[i].Value;
                int hexValue = int.Parse(hexValueStr.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
                    ? hexValueStr.Substring(2)
                    : hexValueStr, System.Globalization.NumberStyles.HexNumber);

                hexValues[i] = hexValue;
            }

            return hexValues;
        }


        static void CustomKey(string configKey = "m3")
        {
            string command = AppConfig.GetString(configKey + "_custom");
            int[] hexKeys = new int[0];

            try
            {
                hexKeys = ParseHexValues(command);
            }
            catch
            {
            }

            switch (hexKeys.Length)
            {
                case 1:
                    KeyboardHook.KeyPress((Keys)hexKeys[0]);
                    break;
                case 2:
                    KeyboardHook.KeyKeyPress((Keys)hexKeys[0], (Keys)hexKeys[1]);
                    break;
                case 3:
                    KeyboardHook.KeyKeyKeyPress((Keys)hexKeys[0], (Keys)hexKeys[1], (Keys)hexKeys[2]);
                    break;
                default:
                    LaunchProcess(command);
                    break;
            }

        }


        static void SetBrightness(int delta)
        {
            int brightness = -1;

            if (isTUF) brightness = ScreenBrightness.Get();
            if (AppConfig.SwappedBrightness()) delta = -delta;

            Program.acpi.DeviceSet(AsusACPI.UniversalControl, delta > 0 ? AsusACPI.Brightness_Up : AsusACPI.Brightness_Down, "Brightness");

            if (isTUF)
            {
                if (AppConfig.SwappedBrightness()) return;
                if (delta < 0 && brightness <= 0) return;
                if (delta > 0 && brightness >= 100) return;

                Thread.Sleep(100);
                if (brightness == ScreenBrightness.Get())
                    Program.toast.RunToast(ScreenBrightness.Adjust(delta) + "%", (delta < 0) ? ToastIcon.BrightnessDown : ToastIcon.BrightnessUp);
            }

        }

        static void SetBrightnessDimming(int delta)
        {
            int brightness = VisualControl.SetBrightness(delta: delta);
            if (brightness >= 0)
                Program.toast.RunToast(brightness + "%", (delta < 0) ? ToastIcon.BrightnessDown : ToastIcon.BrightnessUp);
        }

        public void KeyPressed(object sender, KeyPressedEventArgs e)
        {

            Logger.WriteLine(e.Key.ToString() + " " + e.Modifier.ToString());

            if (e.Modifier == ModifierKeys.None)
            {
                if (AppConfig.NoMKeys())
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

                if (AppConfig.IsZ13() || AppConfig.IsDUO())
                {
                    switch (e.Key)
                    {
                        case Keys.F11:
                            HandleEvent(199);
                            return;
                    }
                }

                if (AppConfig.NoAura())
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
                        SetBrightness(-10);
                        break;
                    case Keys.F8:
                        SetBrightness(+10);
                        break;
                    case Keys.F9:
                        KeyboardHook.KeyKeyPress(Keys.LWin, Keys.P);
                        break;
                    case Keys.F10:
                        ToggleTouchpadEvent(true);
                        break;
                    case Keys.F11:
                        SleepEvent();
                        break;
                    case Keys.VolumeDown:
                        KeyProcess("m1");
                        break;
                    case Keys.VolumeUp:
                        KeyProcess("m2");
                        break;
                    case Keys.Left:
                        KeyboardHook.KeyPress(Keys.Home);
                        break;
                    case Keys.Right:
                        KeyboardHook.KeyPress(Keys.End);
                        break;
                    case Keys.Up:
                        KeyboardHook.KeyPress(Keys.PageUp);
                        break;
                    case Keys.Down:
                        KeyboardHook.KeyPress(Keys.PageDown);
                        break;
                    default:
                        break;
                }

            }

            if (e.Modifier == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                if (e.Key == keyProfile) modeControl.CyclePerformanceMode();
                if (e.Key == keyApp) Program.SettingsToggle();
                if (e.Key == Keys.F20) KeyProcess("m3");
            }

            if (e.Modifier == (ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt))
            {
                if (e.Key == keyProfile) modeControl.CyclePerformanceMode(true);

                switch (e.Key)
                {
                    case Keys.F1:
                        SetBrightness(-10);
                        break;
                    case Keys.F2:
                        SetBrightness(10);
                        break;
                    case Keys.F3:
                        Program.settingsForm.gpuControl.ToggleXGM(true);
                        break;
                    case Keys.F4:
                        Program.settingsForm.BeginInvoke(Program.settingsForm.allyControl.ToggleModeHotkey);
                        break;
                    case Keys.F13:
                        ToggleScreenRate();
                        break;
                    case Keys.F14:
                        Program.settingsForm.gpuControl.SetGPUMode(AsusACPI.GPUModeEco);
                        break;
                    case Keys.F15:
                        Program.settingsForm.gpuControl.SetGPUMode(AsusACPI.GPUModeStandard);
                        break;
                    case Keys.F16:
                        modeControl.SetPerformanceMode(2, true);
                        break;
                    case Keys.F17:
                        modeControl.SetPerformanceMode(0, true);
                        break;
                    case Keys.F18:
                        modeControl.SetPerformanceMode(1, true);
                        break;
                    case Keys.F19:
                        modeControl.SetPerformanceMode(3, true);
                        break;
                    case Keys.F20:
                        modeControl.SetPerformanceMode(4, true);
                        break;
                }
            }


            if (e.Modifier == (ModifierKeys.Control))
            {
                switch (e.Key)
                {
                    case Keys.VolumeDown:
                        // Screen brightness down on CTRL+VolDown
                        SetBrightness(-10);
                        break;
                    case Keys.VolumeUp:
                        // Screen brightness up on CTRL+VolUp
                        SetBrightness(+10);
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
                if (name == "fne")
                    action = "calculator";
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
                case "lock":
                    Logger.WriteLine("Screen lock");
                    NativeMethods.LockScreen();
                    break;
                case "screen":
                    Logger.WriteLine("Screen off toggle");
                    NativeMethods.TurnOffScreen();
                    break;
                case "miniled":
                    if (ScreenCCD.GetHDRStatus()) return;
                    string miniledName = screenControl.ToogleMiniled();
                    Program.toast.RunToast(miniledName, miniledName == Properties.Strings.OneZone ? ToastIcon.BrightnessDown : ToastIcon.BrightnessUp);
                    break;
                case "aura":
                    Program.settingsForm.BeginInvoke(Program.settingsForm.CycleAuraMode);
                    break;
                case "visual":
                    Program.settingsForm.BeginInvoke(Program.settingsForm.CycleVisualMode);
                    break;
                case "performance":
                    modeControl.CyclePerformanceMode(Control.ModifierKeys == Keys.Shift);
                    break;
                case "ghelper":
                    try
                    {
                        Program.settingsForm.BeginInvoke(delegate
                        {
                            Program.SettingsToggle();
                        });
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                    break;
                case "fnlock":
                    ToggleFnLock();
                    break;
                case "micmute":
                    bool muteStatus = Audio.ToggleMute();
                    Program.toast.RunToast(muteStatus ? Properties.Strings.Muted : Properties.Strings.Unmuted, muteStatus ? ToastIcon.MicrophoneMute : ToastIcon.Microphone);
                    if (AppConfig.IsVivoZenbook()) Program.acpi.DeviceSet(AsusACPI.MicMuteLed, muteStatus ? 1 : 0, "MicmuteLed");
                    break;
                case "brightness_up":
                    SetBrightness(+10);
                    break;
                case "brightness_down":
                    SetBrightness(-10);
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
                case "calculator":
                    LaunchProcess("calc");
                    break;
                case "controller":
                    Program.settingsForm.BeginInvoke(Program.settingsForm.allyControl.ToggleModeHotkey);
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

        static void ToggleTouchpadEvent(bool hotkey = false)
        {
            if (hotkey || !AppConfig.IsHardwareTouchpadToggle()) ToggleTouchpad();
            Thread.Sleep(200);
            Program.toast.RunToast(GetTouchpadState() ? Properties.Strings.On : Properties.Strings.Off, ToastIcon.Touchpad);
        }

        static void ToggleTouchpad()
        {
            KeyboardHook.KeyKeyKeyPress(Keys.LWin, Keys.LControlKey, Keys.F24, 50);
        }

        static void SleepEvent()
        {
            Program.acpi.DeviceSet(AsusACPI.UniversalControl, AsusACPI.KB_Sleep, "Sleep");
        }

        public static void ToggleArrowLock()
        {
            int arLock = AppConfig.Is("arrow_lock") ? 0 : 1;
            AppConfig.Set("arrow_lock", arLock);

            Program.settingsForm.BeginInvoke(Program.inputDispatcher.RegisterKeys);
            Program.toast.RunToast("Arrow-Lock " + (arLock == 1 ? Properties.Strings.On : Properties.Strings.Off), ToastIcon.FnLock);
        }

        public static void ToggleFnLock()
        {
            bool fnLock = !AppConfig.Is("fn_lock");
            AppConfig.Set("fn_lock", fnLock ? 1 : 0);

            if (AppConfig.IsVivoZenbook())
                Program.acpi.DeviceSet(AsusACPI.FnLock, fnLock ^ AppConfig.IsInvertedFNLock() ? 1 : 0, "FnLock");
            else
                Program.settingsForm.BeginInvoke(Program.inputDispatcher.RegisterKeys);

            Program.settingsForm.BeginInvoke(Program.settingsForm.VisualiseFnLock);

            Program.toast.RunToast(fnLock ? Properties.Strings.FnLockOn : Properties.Strings.FnLockOff, ToastIcon.FnLock);
        }

        public static void TabletMode()
        {
            if (AppConfig.Is("disable_tablet")) return;

            bool touchpadState = GetTouchpadState();
            bool tabletState = Program.acpi.DeviceGet(AsusACPI.TabletState) > 0;

            Logger.WriteLine("Tablet: " + tabletState + " Touchpad: " + touchpadState);

            if (tabletState && touchpadState || !tabletState && !touchpadState) ToggleTouchpad();

        }

        static void HandleEvent(int EventID)
        {
            // The ROG Ally uses different M-key codes.
            // We'll special-case the translation of those.
            if (AppConfig.IsAlly())
            {
                switch (EventID)
                {

                    // This is both the M1 and M2 keys.
                    // There's a way to differentiate, apparently, but it isn't over USB or any other obvious protocol.
                    case 165:
                        KeyProcess("paddle");
                        return;
                    // The Command Center ("play-looking") button below the select key.
                    case 166:
                        KeyProcess("cc");
                        return;
                    // The M4/ROG key.
                    case 56:
                        KeyProcess("m4");
                        return;
                    case 162:
                        OnScreenKeyboard.Show();
                        return;
                    case 124:
                        KeyProcess("m3");
                        return;

                }
            }
            // All other devices seem to use the same HID key-codes,
            // so we can process them all the same.
            else
            {
                switch (EventID)
                {
                    case 134:     // FN + F12 ON OLD DEVICES
                        KeyProcess("m4");
                        return;
                    case 124:    // M3
                        KeyProcess("m3");
                        return;
                    case 56:    // M4 / Rog button
                        KeyProcess("m4");
                        return;
                    case 55:    // Arconym
                        KeyProcess("m6");
                        return;
                    case 136:    // FN + F12
                        if (!AppConfig.IsNoAirplaneMode()) Program.acpi.DeviceSet(AsusACPI.UniversalControl, AsusACPI.Airplane, "Airplane");
                        return;
                    case 181:    // FN + Numpad Enter
                        KeyProcess("fne");
                        return;
                    case 174:   // FN+F5
                        modeControl.CyclePerformanceMode(Control.ModifierKeys == Keys.Shift);
                        return;
                    case 179:   // FN+F4
                    case 178:   // FN+F4
                        KeyProcess("fnf4");
                        return;
                    case 138:   // Fn + V
                        KeyProcess("fnv");
                        return;
                    case 158:   // Fn + C
                        KeyProcess("fnc");
                        return;
                    case 78:    // Fn + ESC
                        ToggleFnLock();
                        return;
                    case 75:    // Fn + ESC
                        ToggleArrowLock();
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
                    case 46: // Fn + F4 Vivobook Brightness down
                        if (Control.ModifierKeys == Keys.Control && AppConfig.IsOLED())
                        {
                            SetBrightnessDimming(-10);
                        }
                        break;
                    case 47: // Fn + F5 Vivobook Brightness up
                        if (Control.ModifierKeys == Keys.Control && AppConfig.IsOLED())
                        {
                            SetBrightnessDimming(10);
                        }
                        break;
                }
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
                    if (Control.ModifierKeys == Keys.Shift)
                    {
                        if (AppConfig.IsDUO()) SetScreenpad(-10);
                        else Program.settingsForm.BeginInvoke(Program.settingsForm.CycleMatrix, -1);
                    }
                    else if (Control.ModifierKeys == Keys.Control && AppConfig.IsOLED())
                    {
                        SetBrightnessDimming(-10);
                    }
                    else
                    {
                        Program.acpi.DeviceSet(AsusACPI.UniversalControl, AsusACPI.Brightness_Down, "Brightness");
                    }
                    break;
                case 32: // FN+F8
                    if (Control.ModifierKeys == Keys.Shift)
                    {
                        if (AppConfig.IsDUO()) SetScreenpad(10);
                        else Program.settingsForm.BeginInvoke(Program.settingsForm.CycleMatrix, 1);
                    }
                    else if (Control.ModifierKeys == Keys.Control && AppConfig.IsOLED())
                    {
                        SetBrightnessDimming(10);
                    }
                    else
                    {
                        Program.acpi.DeviceSet(AsusACPI.UniversalControl, AsusACPI.Brightness_Up, "Brightness");
                    }
                    break;
                case 133: // Camera Toggle
                    ToggleCamera();
                    break;
                case 107: // FN+F10
                    ToggleTouchpadEvent();
                    break;
                case 108: // FN+F11
                    SleepEvent();
                    break;
                case 106: // Screenpad button on DUO
                    if (Control.ModifierKeys == Keys.Shift)
                        ToggleScreenpad();
                    else
                        SetScreenpad(100);
                    break;
                case 51:    // Fn+F6 on old TUFs
                case 53:    // Fn+F6 on GA-502DU model
                    NativeMethods.TurnOffScreen();
                    return;


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
            if (init) Aura.Init();
            Aura.ApplyBrightness(GetBacklight(), "Auto", init);
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
                Aura.ApplyBrightness(backlight, "HotKey");
            }

            if (!OptimizationService.IsOSDRunning())
            {
                string[] backlightNames = new string[] { Properties.Strings.BacklightOff, Properties.Strings.BacklightLow, Properties.Strings.BacklightMid, Properties.Strings.BacklightMax };
                Program.toast.RunToast(backlightNames[backlight], delta > 0 ? ToastIcon.BacklightUp : ToastIcon.BacklightDown);
            }

        }

        public static void ToggleScreenpad()
        {
            int toggle = AppConfig.Is("screenpad_toggle") ? 0 : 1;
            int brightness = toggle == 0 ? -10 : AppConfig.Get("screenpad", 100);

            Debug.WriteLine($"Screenpad toggle = {toggle}");

            ApplyScreenpadAction(brightness, true);

            AppConfig.Set("screenpad_toggle", toggle);

            Program.toast.RunToast($"Screen Pad " + (toggle == 1 ? "On" : "Off"), toggle > 0 ? ToastIcon.BrightnessUp : ToastIcon.BrightnessDown);
        }

        public static void ToggleScreenRate()
        {
            AppConfig.Set("screen_auto", 0);
            screenControl.ToggleScreenRate();
        }

        public static void ToggleCamera()
        {
            if (!ProcessHelper.IsUserAdministrator()) return;

            string CameraRegistryKeyPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\webcam";
            string CameraRegistryValueName = "Value";

            try
            {
                var status = (string?)Registry.GetValue(CameraRegistryKeyPath, CameraRegistryValueName, "");

                if (status == "Allow") status = "Deny";
                else if (status == "Deny") status = "Allow";
                else
                {
                    Logger.WriteLine("Unknown camera status");
                    return;
                }

                Registry.SetValue(CameraRegistryKeyPath, CameraRegistryValueName, status, RegistryValueKind.String);
                Program.acpi.DeviceSet(AsusACPI.CameraLed, (status == "Deny" ? 1 : 0), "Camera");
                Program.toast.RunToast($"Camera " + (status == "Deny" ? "Off" : "On"));

            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.ToString());
            }

        }

        private static System.Threading.Timer screenpadActionTimer;
        private static int screenpadBrightnessToSet;
        public static void ApplyScreenpadAction(int brightness, bool doToggle = false)
        {
            var delay = AppConfig.Get("screenpad_delay", 1500);

            //Action
            Action<int> action = (b) =>
            {
                if (b >= 0) Program.acpi.DeviceSet(AsusACPI.ScreenPadToggle, 1, "ScreenpadOn");
                Program.acpi.DeviceSet(AsusACPI.ScreenPadBrightness, Math.Max(b * 255 / 100, 0), "Screenpad");
                if (b < 0) Program.acpi.DeviceSet(AsusACPI.ScreenPadToggle, 0, "ScreenpadOff");
            };

            if(delay <= 0 || (brightness > 0 && brightness < 100 && doToggle == false)) //instant action
            {
                action(brightness);
            }
            else //delayed action
            {
                //Timer Approach
                if (screenpadActionTimer == null)
                {
                    screenpadActionTimer = new System.Threading.Timer(_ => action(screenpadBrightnessToSet), null, Timeout.Infinite, Timeout.Infinite);
                }
                //Start Timer
                screenpadBrightnessToSet = brightness;
                screenpadActionTimer.Change(delay, Timeout.Infinite);
            }
        }

        public static void SetScreenpad(int delta)
        {
            int brightness = AppConfig.Get("screenpad", 100);

            if (delta == 100)
            {
                if (brightness < 0) brightness = 100;
                else if (brightness >= 100) brightness = 0;
                else brightness = -10;
            }
            else
            {
                brightness = Math.Max(Math.Min(100, brightness + delta), -10);
            }

            AppConfig.Set("screenpad", brightness);

            ApplyScreenpadAction(brightness);

            string toast;

            if (brightness < 0) toast = "Off";
            else if (brightness == 0) toast = "Hidden";
            else toast = brightness.ToString() + "%";

            Program.toast.RunToast($"Screen Pad {toast}", delta > 0 ? ToastIcon.BrightnessUp : ToastIcon.BrightnessDown);
        }

        static void LaunchProcess(string command = "")
        {
            if (string.IsNullOrEmpty(command)) return;
            try
            {
                RestrictedProcessHelper.RunAsRestrictedUser(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "cmd.exe"), "/C " + command);
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Failed to run: {command} {ex.Message}");
            }
        }

        static void WatcherEventArrived(object sender, EventArrivedEventArgs e)
        {
            if (e.NewEvent is null) return;
            int EventID = int.Parse(e.NewEvent["EventID"].ToString());
            Logger.WriteLine("WMI event " + EventID);
            if (AppConfig.NoWMI()) return;
            HandleEvent(EventID);
        }
    }
}
