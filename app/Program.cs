using GHelper.Battery;
using GHelper.Display;
using GHelper.Gpu;
using GHelper.Helpers;
using GHelper.Input;
using GHelper.Mode;
using GHelper.Peripherals;
using Microsoft.Win32;
using Ryzen;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using static NativeMethods;

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

        public static AsusACPI acpi;

        public static SettingsForm settingsForm = new SettingsForm();

        public static ModeControl modeControl = new ModeControl();
        public static GPUModeControl gpuControl = new GPUModeControl(settingsForm);
        public static ScreenControl screenControl = new ScreenControl();
        public static ClamshellModeControl clamshellControl = new ClamshellModeControl();

        public static ToastForm toast = new ToastForm();

        public static IntPtr unRegPowerNotify;

        private static long lastAuto;
        private static long lastTheme;

        public static InputDispatcher? inputDispatcher;

        private static PowerLineStatus isPlugged = SystemInformation.PowerStatus.PowerLineStatus;

        // The main entry point for the application
        public static void Main(string[] args)
        {

            string action = "";
            if (args.Length > 0) action = args[0];

            string language = AppConfig.GetString("language");

            if (language != null && language.Length > 0)
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(language);
            else
            {
                var culture = CultureInfo.CurrentUICulture;
                if (culture.ToString() == "kr") culture = CultureInfo.GetCultureInfo("ko");
                Thread.CurrentThread.CurrentUICulture = culture;
            }

            ProcessHelper.CheckAlreadyRunning();

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
            Logger.WriteLine("App launched: " + AppConfig.GetModel() + " :" + Assembly.GetExecutingAssembly().GetName().Version.ToString() + CultureInfo.CurrentUICulture + (ProcessHelper.IsUserAdministrator() ? "." : ""));

            Application.EnableVisualStyles();

            HardwareControl.RecreateGpuControl();
            RyzenControl.Init();

            trayIcon.MouseClick += TrayIcon_MouseClick;

            inputDispatcher = new InputDispatcher();

            settingsForm.InitAura();
            settingsForm.InitMatrix();

            gpuControl.InitXGM();

            SetAutoModes();

            // Subscribing for system power change events
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
            SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;

            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
            SystemEvents.SessionEnding += SystemEvents_SessionEnding;

            clamshellControl.RegisterDisplayEvents();
            clamshellControl.ToggleLidAction();

            // Subscribing for monitor power on events
            PowerSettingGuid settingGuid = new NativeMethods.PowerSettingGuid();
            unRegPowerNotify = NativeMethods.RegisterPowerSettingNotification(settingsForm.Handle, settingGuid.ConsoleDisplayState, NativeMethods.DEVICE_NOTIFY_WINDOW_HANDLE);


            Task task = Task.Run((Action)PeripheralsProvider.DetectAllAsusMice);
            PeripheralsProvider.RegisterForDeviceEvents();

            if (Environment.CurrentDirectory.Trim('\\') == Application.StartupPath.Trim('\\') || action.Length > 0)
            {
                SettingsToggle(action);
            }

            Application.Run();

        }

        private static void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            gpuControl.StandardModeFix();
            BatteryControl.SetBatteryChargeLimit();
        }

        private static void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLogon || e.Reason == SessionSwitchReason.SessionUnlock)
            {
                Logger.WriteLine("Session:" + e.Reason.ToString());
                screenControl.AutoScreen();
            }
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

                    if (settingsForm.updates is not null && settingsForm.updates.Text != "")
                        settingsForm.updates.InitTheme();

                    if (settingsForm.matrix is not null && settingsForm.matrix.Text != "")
                        settingsForm.matrix.InitTheme();
                    break;
            }
        }



        public static void SetAutoModes(bool powerChanged = false)
        {

            if (Math.Abs(DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastAuto) < 3000) return;
            lastAuto = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            isPlugged = SystemInformation.PowerStatus.PowerLineStatus;
            Logger.WriteLine("AutoSetting for " + isPlugged.ToString());

            inputDispatcher.Init();

            modeControl.AutoPerformance(powerChanged);

            bool switched = gpuControl.AutoGPUMode();

            if (!switched)
            {
                gpuControl.InitGPUMode();
                screenControl.AutoScreen();
            }

            BatteryControl.SetBatteryChargeLimit();

            settingsForm.AutoKeyboard();
            settingsForm.matrixControl.SetMatrix(true);
        }

        private static void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {

            if (e.Mode == PowerModes.Suspend)
            {
                Logger.WriteLine("Power Mode Changed:" + e.Mode.ToString());
                gpuControl.StandardModeFix();
            }

            if (SystemInformation.PowerStatus.PowerLineStatus == isPlugged) return;
            SetAutoModes(true);
        }

        public static void SettingsToggle(string action = "", bool checkForFocus = false)
        {
            if (settingsForm.Visible)
            {
                // If helper window is not on top, this just focuses on the app again
                // Pressing the ghelper button again will hide the app
                if (checkForFocus && !settingsForm.HasAnyFocus())
                {
                    settingsForm.ShowAll();
                }
                else
                {
                    settingsForm.HideAll();
                }
            }
            else
            {

                settingsForm.Left = Screen.FromControl(settingsForm).WorkingArea.Width - 10 - settingsForm.Width;
                settingsForm.Top = Screen.FromControl(settingsForm).WorkingArea.Height - 10 - settingsForm.Height;

                settingsForm.Show();
                settingsForm.Activate();

                settingsForm.Left = Screen.FromControl(settingsForm).WorkingArea.Width - 10 - settingsForm.Width;
                settingsForm.Top = Screen.FromControl(settingsForm).WorkingArea.Height - 10 - settingsForm.Height;

                settingsForm.VisualiseGPUMode();

                switch (action)
                {
                    case "cpu":
                        Startup.ReScheduleAdmin();
                        settingsForm.FansToggle();
                        break;
                    case "gpu":
                        Startup.ReScheduleAdmin();
                        settingsForm.FansToggle(1);
                        break;
                    case "gpurestart":
                        gpuControl.RestartGPU(false);
                        break;
                    case "services":
                        settingsForm.keyb = new Extra();
                        settingsForm.keyb.Show();
                        settingsForm.keyb.ServiesToggle();
                        break;
                    case "uv":
                        Startup.ReScheduleAdmin();
                        settingsForm.FansToggle(2);
                        modeControl.SetRyzen();
                        break;
                }
            }
        }

        static void TrayIcon_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                SettingsToggle();

        }



        static void OnExit(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            PeripheralsProvider.UnregisterForDeviceEvents();
            clamshellControl.UnregisterDisplayEvents();
            NativeMethods.UnregisterPowerSettingNotification(unRegPowerNotify);
            Application.Exit();
        }


    }
}