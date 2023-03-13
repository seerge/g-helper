using Microsoft.Win32;
using System.Diagnostics;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
public static class Logger
{
    public static void WriteLine(string logMessage)
    {
        Debug.WriteLine(logMessage);

        var appPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\GHelper";
        var logFile = appPath + "\\log.txt";

        if (!Directory.Exists(appPath)) Directory.CreateDirectory(appPath);

        using (StreamWriter w = File.AppendText(logFile))
        {
            w.WriteLine($"{DateTime.Now}: {logMessage}");
        }

    }

}

namespace GHelper
{
    static class Program
    {


        // Native methods for sleep detection

        [DllImport("Powrprof.dll", SetLastError = true)]
        static extern uint PowerRegisterSuspendResumeNotification(uint flags, ref DEVICE_NOTIFY_SUBSCRIBE_PARAMETERS receipient, ref IntPtr registrationHandle);


        private const int WM_POWERBROADCAST = 536; // (0x218)
        private const int PBT_APMPOWERSTATUSCHANGE = 10; // (0xA) - Power status has changed.
        private const int PBT_APMRESUMEAUTOMATIC = 18; // (0x12) - Operation is resuming automatically from a low-power state.This message is sent every time the system resumes.
        private const int PBT_APMRESUMESUSPEND = 7; // (0x7) - Operation is resuming from a low-power state.This message is sent after PBT_APMRESUMEAUTOMATIC if the resume is triggered by user input, such as pressing a key.
        private const int PBT_APMSUSPEND = 4; // (0x4) - System is suspending operation.
        private const int PBT_POWERSETTINGCHANGE = 32787; // (0x8013) - A power setting change event has been received.
        private const int DEVICE_NOTIFY_CALLBACK = 2;

        [StructLayout(LayoutKind.Sequential)]
        struct DEVICE_NOTIFY_SUBSCRIBE_PARAMETERS
        {
            public DeviceNotifyCallbackRoutine Callback;
            public IntPtr Context;
        }

        public delegate int DeviceNotifyCallbackRoutine(IntPtr context, int type, IntPtr setting);

        //

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

        private static IntPtr unRegPowerNotify;
        private static IntPtr ds;

        private static long lastAuto;

        // The main entry point for the application
        public static void Main()
        {
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

            ds = settingsForm.Handle;

            trayIcon.MouseClick += TrayIcon_MouseClick; ;

            wmi.SubscribeToEvents(WatcherEventArrived);

            settingsForm.InitGPUMode();
            settingsForm.InitAura();
            settingsForm.InitMatrix();

            settingsForm.VisualiseGPUAuto(config.getConfig("gpu_auto"));
            settingsForm.VisualiseScreenAuto(config.getConfig("screen_auto"));
            settingsForm.SetStartupCheck(Startup.IsScheduled());

            SetAutoModes();
            HardwareMonitor.RecreateGpuTemperatureProvider();

            // Subscribing for native power change events

            /*
            IntPtr registrationHandle = new IntPtr();
            DEVICE_NOTIFY_SUBSCRIBE_PARAMETERS recipient = new DEVICE_NOTIFY_SUBSCRIBE_PARAMETERS();
            recipient.Callback = new DeviceNotifyCallbackRoutine(DeviceNotifyCallback);
            recipient.Context = IntPtr.Zero;

            IntPtr pRecipient = Marshal.AllocHGlobal(Marshal.SizeOf(recipient));
            Marshal.StructureToPtr(recipient, pRecipient, false);

            uint result = PowerRegisterSuspendResumeNotification(DEVICE_NOTIFY_CALLBACK, ref recipient, ref registrationHandle);
            */

            // Subscribing for monitor power on events
            var settingGuid = new NativeMethods.PowerSettingGuid();
            unRegPowerNotify = NativeMethods.RegisterPowerSettingNotification(ds, settingGuid.ConsoleDisplayState, NativeMethods.DEVICE_NOTIFY_WINDOW_HANDLE);

            // Subscribing for system power change events
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;


            CheckForUpdates();
            Application.Run();

        }

        private static int DeviceNotifyCallback(IntPtr context, int type, IntPtr setting)
        {
            Logger.WriteLine($"Power callback {type}");
            switch (type)
            {
                case PBT_APMRESUMEAUTOMATIC:
                    settingsForm.BeginInvoke(delegate
                    {
                        SetAutoModes();
                    });
                    break;
            }

            return 0;
        }


        static async void CheckForUpdates()
        {

            var assembly = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            settingsForm.SetVersionLabel("Version: " + assembly);

            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "C# App");
                    var json = await httpClient.GetStringAsync("https://api.github.com/repos/seerge/g-helper/releases/latest");
                    var config = JsonSerializer.Deserialize<JsonElement>(json);
                    var tag = config.GetProperty("tag_name").ToString().Replace("v", "");
                    var url = config.GetProperty("assets")[0].GetProperty("browser_download_url").ToString();

                    var gitVersion = new Version(tag);
                    var appVersion = new Version(assembly);

                    var result = gitVersion.CompareTo(appVersion);
                    if (result > 0)
                    {
                        settingsForm.SetVersionLabel("Download Update: " + tag, url);
                    }

                }
            }
            catch
            {
                Logger.WriteLine("Failed to get update");
            }

        }


        public static void SetAutoModes()
        {

            if (Math.Abs(DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastAuto) < 1000) return;
            lastAuto = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            PowerLineStatus isPlugged = SystemInformation.PowerStatus.PowerLineStatus;

            Logger.WriteLine("AutoSetting for " + isPlugged.ToString());

            settingsForm.SetBatteryChargeLimit(config.getConfig("charge_limit"));

            settingsForm.AutoPerformance(isPlugged);
            settingsForm.AutoGPUMode(isPlugged);

            settingsForm.SetMatrix(isPlugged);
        }

        private static void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            Logger.WriteLine("Windows - Power Mode Changed");
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