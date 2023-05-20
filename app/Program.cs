using Microsoft.Win32;
using NAudio.CoreAudioApi;
using System.Diagnostics;
using System.Globalization;
using System.Management;
using System.Reflection;
using System.Security.Principal;
using Tools;

namespace GHelper
{

    static class Program
    {
        public static NotifyIcon trayIcon = new NotifyIcon
        {
            Text = "G-Helper",
            Icon = Properties.Resources.standard,
            Visible = true
        };

        public static AsusACPI? acpi;
        public static AppConfig config = new AppConfig();

        public static SettingsForm settingsForm = new SettingsForm();

        public static IntPtr unRegPowerNotify;

        private static long lastAuto;
        private static long lastTheme;
        private static long lastAdmin;

        private static bool isOptimizationRunning = OptimizationService.IsRunning();

        private static PowerLineStatus isPlugged = PowerLineStatus.Unknown;

        // The main entry point for the application
        public static void Main(string[] args)
        {

            string action = "";
            if (args.Length > 0) action = args[0];

            Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentUICulture;
            Debug.WriteLine(CultureInfo.CurrentUICulture);

            //Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("fr");

            CheckProcesses();

            try
            {
                acpi = new AsusACPI();
            }
            catch
            {
                DialogResult dialogResult = MessageBox.Show(Properties.Strings.ACPIError, Properties.Strings.StartupError, MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Process.Start(new ProcessStartInfo("https://www.asus.com/support/FAQ/1047338/") { UseShellExecute = true });
                }

                Application.Exit();
                return;
            }

            Logger.WriteLine("------------");
            Logger.WriteLine("App launched: " + config.GetModel() + " :" + Assembly.GetExecutingAssembly().GetName().Version.ToString() + (IsUserAdministrator() ? "A" : ""));

            Application.EnableVisualStyles();

            HardwareControl.RecreateGpuControl();

            var ds = settingsForm.Handle;

            trayIcon.MouseClick += TrayIcon_MouseClick;

            acpi.SubscribeToEvents(WatcherEventArrived);

            settingsForm.InitAura();
            settingsForm.InitMatrix();
            settingsForm.SetStartupCheck(Startup.IsScheduled());

            SetAutoModes();

            // Subscribing for system power change events
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
            SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;

            // Subscribing for monitor power on events
            var settingGuid = new NativeMethods.PowerSettingGuid();
            unRegPowerNotify = NativeMethods.RegisterPowerSettingNotification(ds, settingGuid.ConsoleDisplayState, NativeMethods.DEVICE_NOTIFY_WINDOW_HANDLE);

            // CTRL + SHIFT + F5 to cycle profiles
            Keys keybind_profile = (config.getConfig("keybind_profile") != -1) ? (Keys)config.getConfig("keybind_profile") : Keys.F5;
            if (keybind_profile != 0)
            {
                var ghk = new KeyHandler(KeyHandler.SHIFT | KeyHandler.CTRL, keybind_profile, ds);
                ghk.Register();
            }


            if (Environment.CurrentDirectory.Trim('\\') == Application.StartupPath.Trim('\\') || action.Length > 0)
            {
                SettingsToggle(action);
            }

            if (!isOptimizationRunning) AsusUSB.RunListener(HandleEvent);

            Application.Run();

        }



        static void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {

            if (Math.Abs(DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastTheme) < 2000) return;

            switch (e.Category)
            {
                case UserPreferenceCategory.General:
                    bool changed = settingsForm.InitTheme();
                    if (changed)
                    {
                        Debug.WriteLine("Theme Changed");
                        lastTheme = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    }

                    if (settingsForm.fans is not null && settingsForm.fans.Text != "")
                        settingsForm.fans.InitTheme();

                    if (settingsForm.keyb is not null && settingsForm.keyb.Text != "")
                        settingsForm.keyb.InitTheme();

                    break;
            }
        }



        public static void SetAutoModes()
        {

            if (Math.Abs(DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastAuto) < 3000) return;
            lastAuto = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            isPlugged = SystemInformation.PowerStatus.PowerLineStatus;
            Logger.WriteLine("AutoSetting for " + isPlugged.ToString());

            settingsForm.SetBatteryChargeLimit(config.getConfig("charge_limit"));
            settingsForm.AutoPerformance();

            bool switched = settingsForm.AutoGPUMode();

            if (!switched)
            {
                settingsForm.InitGPUMode();
                settingsForm.AutoScreen();
            }

            settingsForm.AutoKeyboard();
            settingsForm.matrix.SetMatrix();
        }

        private static void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (SystemInformation.PowerStatus.PowerLineStatus == isPlugged) return;

            Logger.WriteLine("Windows - Power Mode Changed");
            SetAutoModes();
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

        static void CustomKey(string configKey = "m3")
        {
            string command = config.getConfigString(configKey + "_custom");
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
            string action = config.getConfigString(name);

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
                    settingsForm.BeginInvoke(settingsForm.ToogleMiniled);
                    break;
                case "aura":
                    settingsForm.BeginInvoke(settingsForm.CycleAuraMode);
                    break;
                case "performance":
                    settingsForm.BeginInvoke(settingsForm.CyclePerformanceMode);
                    break;
                case "ghelper":
                    settingsForm.BeginInvoke(delegate
                    {
                        SettingsToggle();
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
                        settingsForm.BeginInvoke(settingsForm.RunToast, muteStatus ? "Muted" : "Unmuted", ToastIcon.Microphone);
                    }
                    break;

                default:
                    break;
            }
        }

        static void TabletMode()
        {
            bool touchpadState, tabletState;

            using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\PrecisionTouchPad\Status", false))
            {
                touchpadState = (key?.GetValue("Enabled")?.ToString() == "1");
            }

            tabletState = acpi.DeviceGet(AsusACPI.TabletState) > 0;

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
                    settingsForm.BeginInvoke(settingsForm.CyclePerformanceMode);
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

            int brightness = config.getConfig("keyboard_brightness");

            switch (EventID)
            {
                case 197: // FN+F2
                    brightness = Math.Max(0, brightness - 1);
                    config.setConfig("keyboard_brightness", brightness);
                    AsusUSB.ApplyBrightness(brightness);
                    settingsForm.BeginInvoke(settingsForm.RunToast, "Down", ToastIcon.Backlight);
                    break;
                case 196: // FN+F3
                    brightness = Math.Min(3, brightness + 1);
                    config.setConfig("keyboard_brightness", brightness);
                    AsusUSB.ApplyBrightness(brightness);
                    settingsForm.BeginInvoke(settingsForm.RunToast, "Up", ToastIcon.Backlight);
                    break;
                case 16: // FN+F7
                    ScreenBrightness.Adjust(-10);
                    settingsForm.BeginInvoke(settingsForm.RunToast, "Down", ToastIcon.Brightness);
                    break;
                case 32: // FN+F8
                    ScreenBrightness.Adjust(+10);
                    settingsForm.BeginInvoke(settingsForm.RunToast, "Up", ToastIcon.Brightness);
                    break;
                case 107: // FN+F10
                    AsusUSB.TouchpadToggle();
                    settingsForm.BeginInvoke(settingsForm.RunToast, "Toggle", ToastIcon.Touchpad);
                    break;
                case 108: // FN+F11
                    Application.SetSuspendState(PowerState.Suspend, true, true);
                    break;
            }
        }

        static void WatcherEventArrived(object sender, EventArrivedEventArgs e)
        {
            if (e.NewEvent is null) return;
            int EventID = int.Parse(e.NewEvent["EventID"].ToString());
            Logger.WriteLine("WMI event " + EventID);
            HandleEvent(EventID);
        }

        static void SettingsToggle(string action = "")
        {
            if (settingsForm.Visible)
                settingsForm.Hide();
            else
            {
                settingsForm.Show();
                settingsForm.Activate();
                settingsForm.VisualiseGPUMode();

                switch (action)
                {
                    case "gpu":
                        Startup.ReScheduleAdmin();
                        settingsForm.FansToggle();
                        break;
                    case "gpurestart":
                        settingsForm.RestartGPU(false);
                        break;
                }
            }
        }

        static void TrayIcon_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                SettingsToggle();
            }

        }



        static void OnExit(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            NativeMethods.UnregisterPowerSettingNotification(unRegPowerNotify);
            Application.Exit();
        }


        static void CheckProcesses()
        {
            Process currentProcess = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(currentProcess.ProcessName);

            if (processes.Length > 1)
            {
                foreach (Process process in processes)
                    if (process.Id != currentProcess.Id)
                        try
                        {
                            process.Kill();
                        }
                        catch (Exception ex)
                        {
                            Logger.WriteLine(ex.ToString());
                            MessageBox.Show(Properties.Strings.AppAlreadyRunningText, Properties.Strings.AppAlreadyRunning, MessageBoxButtons.OK);
                            Application.Exit();
                            return;
                        }
            }
        }

        public static bool IsUserAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static void RunAsAdmin(string? param = null)
        {

            if (Math.Abs(DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastAdmin) < 2000) return;
            lastAdmin = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            // Check if the current user is an administrator
            if (!IsUserAdministrator())
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.FileName = Application.ExecutablePath;
                startInfo.Arguments = param;
                startInfo.Verb = "runas";
                Process.Start(startInfo);
                Application.Exit();
            }
        }
    }

}