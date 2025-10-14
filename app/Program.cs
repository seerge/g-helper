using GHelper.Ally;
using GHelper.Battery;
using GHelper.Display;
using GHelper.Gpu;
using GHelper.Helpers;
using GHelper.Input;
using GHelper.Mode;
using GHelper.Peripherals;
using GHelper.USB;
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
        public static NotifyIcon trayIcon;
        public static AsusACPI acpi;

        public static SettingsForm settingsForm = new SettingsForm();

        public static ModeControl modeControl = new ModeControl();
        public static GPUModeControl gpuControl = new GPUModeControl(settingsForm);
        public static AllyControl allyControl = new AllyControl(settingsForm);
        public static ClamshellModeControl clamshellControl = new ClamshellModeControl();

        public static ToastForm toast = new ToastForm();

        public static IntPtr unRegPowerNotify, unRegPowerNotifyLid;
        public static int WM_TASKBARCREATED = 0;

        private static long lastAuto;
        private static long lastTheme;

        public static InputDispatcher? inputDispatcher;

        private static PowerLineStatus isPlugged = SystemInformation.PowerStatus.PowerLineStatus;

        // The main entry point for the application
        public static void Main(string[] args)
        {

            string action = "";
            if (args.Length > 0) action = args[0];

            if (action == "charge")
            {
                if (AppConfig.IsZ13()) Aura.Init();
                BatteryLimit();
                InputDispatcher.StartupBacklight();
                Application.Exit();
                return;
            }

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
            ProcessHelper.SetPriority();

            Logger.WriteLine("------------");
            Logger.WriteLine("App launched: " + AppConfig.GetModel() + " :" + Assembly.GetExecutingAssembly().GetName().Version.ToString() + CultureInfo.CurrentUICulture + (ProcessHelper.IsUserAdministrator() ? "." : ""));

            var startCount = AppConfig.Get("start_count") + 1;
            AppConfig.Set("start_count", startCount);
            Logger.WriteLine("Start Count: " + startCount);

            acpi = new AsusACPI();

            if (!acpi.IsConnected() && AppConfig.IsASUS())
            {
                DialogResult dialogResult = MessageBox.Show(Properties.Strings.ACPIError, Properties.Strings.StartupError, MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Process.Start(new ProcessStartInfo("https://www.asus.com/support/FAQ/1047338/") { UseShellExecute = true });
                }

                Application.Exit();
                return;
            }

            ProcessHelper.KillByName("ASUSSmartDisplayControl");

            Application.EnableVisualStyles();

            HardwareControl.RecreateGpuControl();
            RyzenControl.Init();

            trayIcon = new NotifyIcon
            {
                Text = "G-Helper",
                Icon = Properties.Resources.standard,
                Visible = true
            };

            WM_TASKBARCREATED = RegisterWindowMessage("TaskbarCreated");
            Logger.WriteLine($"Tray Icon: {trayIcon.Visible} | {WM_TASKBARCREATED}");

            settingsForm.SetContextMenu();
            trayIcon.MouseClick += TrayIcon_MouseClick;
            trayIcon.MouseMove += TrayIcon_MouseMove;


            inputDispatcher = new InputDispatcher();

            settingsForm.InitAura();
            settingsForm.InitMatrix();

            gpuControl.InitXGM();

            SetAutoModes(init: true);

            // Subscribing for system power change events
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
            SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;

            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
            SystemEvents.SessionEnding += SystemEvents_SessionEnding;

            clamshellControl.RegisterDisplayEvents();
            clamshellControl.ToggleLidAction();

            // Subscribing for monitor power on events
            unRegPowerNotify = NativeMethods.RegisterPowerSettingNotification(settingsForm.Handle, PowerSettingGuid.ConsoleDisplayState, NativeMethods.DEVICE_NOTIFY_WINDOW_HANDLE);
            unRegPowerNotifyLid = NativeMethods.RegisterPowerSettingNotification(settingsForm.Handle, PowerSettingGuid.LIDSWITCH_STATE_CHANGE, NativeMethods.DEVICE_NOTIFY_WINDOW_HANDLE);


            Task task = Task.Run((Action)PeripheralsProvider.DetectAllAsusMice);
            PeripheralsProvider.RegisterForDeviceEvents();

            if (Environment.CurrentDirectory.Trim('\\') == Application.StartupPath.Trim('\\') || action.Length > 0)
            {
                SettingsToggle(false);
            }

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
                case "services":
                    settingsForm.extraForm = new Extra();
                    settingsForm.extraForm.Show();
                    settingsForm.extraForm.ServiesToggle();
                    break;
                case "uv":
                    Startup.ReScheduleAdmin();
                    settingsForm.FansToggle(2);
                    modeControl.SetRyzen();
                    break;
                case "colors":
                    Task.Run(async () =>
                    {
                        await ColorProfileHelper.InstallProfile();
                        settingsForm.Invoke(delegate
                        {
                            settingsForm.InitVisual();
                        });
                    });
                    break;
                default:
                    Task.Run(Startup.StartupCheck);
                    break;
            }

            Application.Run();

        }


        private static void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            gpuControl.StandardModeFix();
            modeControl.ShutdownReset();
            BatteryControl.AutoBattery();
            InputDispatcher.ShutdownStatusLed();
        }

        private static void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLogon || e.Reason == SessionSwitchReason.SessionUnlock)
            {
                Logger.WriteLine("Session:" + e.Reason.ToString());
                ScreenControl.AutoScreen();
            }
        }

        static void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {

            if (Math.Abs(DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastTheme) < 2000) return;

            switch (e.Category)
            {
                case UserPreferenceCategory.General:
                    bool changed = settingsForm.InitTheme();
                    settingsForm.VisualiseIcon();

                    if (changed)
                    {
                        Debug.WriteLine("Theme Changed");
                        lastTheme = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    }

                    if (settingsForm.fansForm is not null && settingsForm.fansForm.Text != "")
                        settingsForm.fansForm.InitTheme();

                    if (settingsForm.extraForm is not null && settingsForm.extraForm.Text != "")
                        settingsForm.extraForm.InitTheme();

                    if (settingsForm.updatesForm is not null && settingsForm.updatesForm.Text != "")
                        settingsForm.updatesForm.InitTheme();

                    if (settingsForm.matrixForm is not null && settingsForm.matrixForm.Text != "")
                        settingsForm.matrixForm.InitTheme();

                    if (settingsForm.handheldForm is not null && settingsForm.handheldForm.Text != "")
                        settingsForm.handheldForm.InitTheme();

                    break;
            }
        }



        public static bool SetAutoModes(bool powerChanged = false, bool init = false, bool wakeup = false)
        {
            int skipDelay = wakeup ? 10000 : 3000;

            if (Math.Abs(DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastAuto) < skipDelay) return false;
            lastAuto = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            isPlugged = SystemInformation.PowerStatus.PowerLineStatus;
            Logger.WriteLine("AutoSetting for " + isPlugged.ToString());

            BatteryControl.AutoBattery(init);
            if (init) InputDispatcher.InitScreenpad();
            DynamicLightingHelper.Init();
            ScreenControl.InitOptimalBrightness();

            inputDispatcher.Init();
            //HardwareControl.ReadSensors(true);

            modeControl.AutoPerformance(powerChanged);

            settingsForm.matrixControl.SetDevice(true);
            InputDispatcher.InitStatusLed();
            XGM.InitLight();

            if (AppConfig.IsAlly())
            {
                allyControl.Init();
            }
            else
            {
                InputDispatcher.AutoKeyboard();
            }

            bool switched = gpuControl.AutoGPUMode(delay: 1000);
            if (!switched)
            {
                gpuControl.InitGPUMode();
                ScreenControl.AutoScreen();
            }

            ScreenControl.InitMiniled();
            VisualControl.InitBrightness();

            return true;
        }

        private static void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            Logger.WriteLine($"Power Mode {e.Mode}: {SystemInformation.PowerStatus.PowerLineStatus}");
            if (e.Mode == PowerModes.Suspend)
            {
                Logger.WriteLine("Power Mode Changed:" + e.Mode.ToString());
                gpuControl.StandardModeFix(true);
                modeControl.ShutdownReset();
                InputDispatcher.ShutdownStatusLed();
            }

            int delay = AppConfig.Get("charger_delay");
            if (delay > 0)
            {
                Logger.WriteLine($"Charger Delay: {delay}");
                Thread.Sleep(delay);
            }

            if (SystemInformation.PowerStatus.PowerLineStatus == isPlugged) return;
            if (AppConfig.Is("disable_power_event")) return;
            SetAutoModes(powerChanged: true);
        }

        public static void SettingsToggle(bool checkForFocus = true, bool trayClick = false)
        {
            if (settingsForm.Visible)
            {
                // If helper window is not on top, this just focuses on the app again
                // Pressing the ghelper button again will hide the app
                if (checkForFocus && !settingsForm.HasAnyFocus(trayClick) && !AppConfig.Is("topmost"))
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
                var screen = Screen.PrimaryScreen;
                if (screen is null) screen = Screen.FromControl(settingsForm);

                settingsForm.Location = screen.WorkingArea.Location;
                settingsForm.Left = screen.WorkingArea.Width - 10 - settingsForm.Width;
                settingsForm.Top = screen.WorkingArea.Height - 10 - settingsForm.Height;

                settingsForm.Show();
                settingsForm.ShowAll();

                settingsForm.Left = screen.WorkingArea.Width - 10 - settingsForm.Width;

                if (AppConfig.IsAlly())
                    settingsForm.Top = Math.Max(10, screen.Bounds.Height - 110 - settingsForm.Height);
                else
                    settingsForm.Top = screen.WorkingArea.Height - 10 - settingsForm.Height;

                settingsForm.VisualiseGPUMode();
            }
        }

        static void TrayIcon_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                SettingsToggle(trayClick: true);

        }

        static void TrayIcon_MouseMove(object? sender, MouseEventArgs e)
        {
            settingsForm.RefreshSensors();
        }

        static void OnExit(object sender, EventArgs e)
        {
            if (trayIcon is not null)
            {
                trayIcon.Visible = false;
                trayIcon.Dispose();
            }

            PeripheralsProvider.UnregisterForDeviceEvents();
            clamshellControl.UnregisterDisplayEvents();
            NativeMethods.UnregisterPowerSettingNotification(unRegPowerNotify);
            NativeMethods.UnregisterPowerSettingNotification(unRegPowerNotifyLid);
            Application.Exit();
        }

        static void BatteryLimit()
        {
            try
            {
                int limit = AppConfig.Get("charge_limit");
                if (limit > 0 && limit < 100)
                {
                    Logger.WriteLine($"------- Startup Battery Limit {limit} -------");
                    ProcessHelper.StartEnableService("ATKWMIACPIIO", false);
                    Logger.WriteLine($"Connecting to ACPI");
                    acpi = new AsusACPI();
                    Logger.WriteLine($"Setting Limit");
                    acpi.DeviceSet(AsusACPI.BatteryLimit, limit, "Limit");
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Startup Battery Limit Error: " + ex.Message);
            }
        }

    }
}