using Microsoft.Win32;
using System.Diagnostics;
using System.Management;

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

        public static ASUSWmi wmi;
        public static AppConfig config = new AppConfig();

        public static SettingsForm settingsForm = new SettingsForm();
        public static ToastForm toast = new ToastForm();

        public static IntPtr unRegPowerNotify;

        private static long lastAuto;
        private static long lastTheme;
        private static PowerLineStatus isPlugged = PowerLineStatus.Unknown;

        // The main entry point for the application
        public static void Main()
        {

            if (Process.GetProcesses().Count(p => p.ProcessName == "GHelper") > 1)
            {
                MessageBox.Show("G-Helper is already running. Check system tray for an icon.", "App already running", MessageBoxButtons.OK);
                Application.Exit();
                return;
            }


            try
            {
                wmi = new ASUSWmi();
            }
            catch
            {
                DialogResult dialogResult = MessageBox.Show("Can't connect to ASUS ACPI. Application can't function without it. Try to install Asus System Controll Interface", "Startup Error", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Process.Start(new ProcessStartInfo("https://www.asus.com/support/FAQ/1047338/") { UseShellExecute = true });
                }

                Application.Exit();
                return;

            }


            Application.EnableVisualStyles();

            SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(SystemEvents_UserPreferenceChanged);

            var ds = settingsForm.Handle;

            trayIcon.MouseClick += TrayIcon_MouseClick;


            wmi.SubscribeToEvents(WatcherEventArrived);

            settingsForm.InitGPUMode();
            settingsForm.InitAura();
            settingsForm.InitMatrix();

            settingsForm.SetStartupCheck(Startup.IsScheduled());

            SetAutoModes();
            HardwareMonitor.RecreateGpuTemperatureProvider();

            // Subscribing for system power change events
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;


            if (Environment.CurrentDirectory.Trim('\\') == Application.StartupPath.Trim('\\'))
            {
                SettingsToggle();
            }

            Application.Run();


        }


        static void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {

            if (Math.Abs(DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastTheme) < 2000) return;
            lastTheme = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            switch (e.Category)
            {
                case UserPreferenceCategory.General:
                    Debug.WriteLine("Theme Changed");
                    Thread.Sleep(1000);
                    settingsForm.InitTheme();

                    if (settingsForm.fans is not null && settingsForm.fans.Text != "")
                        settingsForm.fans.InitTheme();

                    if (settingsForm.keyb is not null && settingsForm.keyb.Text != "")
                        settingsForm.keyb.InitTheme();

                    break;
            }
        }



        public static void SetAutoModes()
        {

            if (Math.Abs(DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastAuto) < 2000) return;
            lastAuto = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            isPlugged = SystemInformation.PowerStatus.PowerLineStatus;
            Logger.WriteLine("AutoSetting for " + isPlugged.ToString());

            settingsForm.SetBatteryChargeLimit(config.getConfig("charge_limit"));
            settingsForm.AutoPerformance();

            bool switched = settingsForm.AutoGPUMode();
            if (!switched) settingsForm.AutoScreen();

            settingsForm.SetMatrix();
        }

        private static void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (SystemInformation.PowerStatus.PowerLineStatus == isPlugged) return;

            Logger.WriteLine("Windows - Power Mode Changed");
            settingsForm.AutoKeyboard();
            SetAutoModes();
        }


        static void LaunchProcess(string fileName = "")
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = fileName;
            start.WindowStyle = ProcessWindowStyle.Hidden;
            start.CreateNoWindow = true;
            try
            {
                Process proc = Process.Start(start);
            }
            catch
            {
                Logger.WriteLine("Failed to run " + fileName);
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
                    action = "performance";
                if (name == "fnf4")
                    action = "aura";
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
                case "aura":
                    settingsForm.BeginInvoke(settingsForm.CycleAuraMode);
                    break;
                case "performance":
                    settingsForm.BeginInvoke(settingsForm.CyclePerformanceMode);
                    break;
                case "ghelper":
                    settingsForm.BeginInvoke(SettingsToggle);
                    break;
                case "custom":
                    CustomKey(name);
                    break;
                default:
                    break;
            }
        }


        static void WatcherEventArrived(object sender, EventArrivedEventArgs e)
        {
            var collection = (ManagementEventWatcher)sender;

            if (e.NewEvent is null) return;

            int EventID = int.Parse(e.NewEvent["EventID"].ToString());

            Logger.WriteLine("WMI event " + EventID);

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
            }


        }

        static void SettingsToggle()
        {
            if (settingsForm.Visible)
                settingsForm.Hide();
            else
            {
                settingsForm.Show();
                settingsForm.Activate();
            }

            settingsForm.VisualiseGPUMode();

        }

        static void TrayIcon_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
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
    }

}