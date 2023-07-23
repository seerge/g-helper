using GHelper.Battery;
using GHelper.Display;
using GHelper.Gpu;
using GHelper.Helpers;
using GHelper.Input;
using GHelper.Mode;
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
        static GPUModeControl gpuControl = new GPUModeControl(settingsForm);
        static ScreenControl screenControl = new ScreenControl();
        static ClamshellModeControl clamshellControl = new ClamshellModeControl();
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


            SetAutoModes();

            //Check boot from eco
            //Check the shutdown state
            if (AppConfig.Get("eco_shutdown") == 1) //shutdown from eco
            {
                gpuControl.SetGPUMode(AsusACPI.GPUModeEco);
                AppConfig.Set("eco_shutdown", 0);
            }
            if (AppConfig.Get("eco_shutdown") == 2) //shutdown from optimised
            {
                AppConfig.Set("gpu_auto", (AppConfig.Get("gpu_auto") == 1) ? 0 : 1);
                gpuControl.AutoGPUMode(true);
                AppConfig.Set("eco_shutdown", 0);
            }

            // Subscribing for system power change events
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
            SystemEvents.SessionEnding += c_SessionEndedEvent; //add handler for session end event
            SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;
            clamshellControl.RegisterDisplayEvents();
            clamshellControl.ToggleLidAction();

            // Subscribing for monitor power on events
            PowerSettingGuid settingGuid = new NativeMethods.PowerSettingGuid();
            unRegPowerNotify = NativeMethods.RegisterPowerSettingNotification(settingsForm.Handle, settingGuid.ConsoleDisplayState, NativeMethods.DEVICE_NOTIFY_WINDOW_HANDLE);


            if (Environment.CurrentDirectory.Trim('\\') == Application.StartupPath.Trim('\\') || action.Length > 0)
            {
                SettingsToggle(action);
            }

            Application.Run();

        }

        //Session end event handler to ensure Normal mode for safe boot
        private static void c_SessionEndedEvent(object sender, SessionEndingEventArgs e)
        {
            //Check for ultimate or standard mode
            int CurrentGPU = AppConfig.Get("gpu_mode");
            if (CurrentGPU != AsusACPI.GPUModeUltimate && CurrentGPU != AsusACPI.GPUModeStandard)
            {
                if (AppConfig.Is("gpu_auto"))
                {
                    AppConfig.Set("eco_shutdown", 2);
                }
                else
                {
                    AppConfig.Set("eco_shutdown", 1);
                }
                gpuControl.SetGPUMode(AsusACPI.GPUModeStandard); //set to standard for safe boot
                //This will ensure that eco and optimised unplugged mode will reboot correctly
                //If in optimised with power state the request will be ignored, dont need to handle.
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
            settingsForm.matrix.SetMatrix();
        }

        private static void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            //Eco mode safe boot when system shuts down
            if (e.Mode.ToString() == "Suspend")
            {
                int CurrentGPU = AppConfig.Get("gpu_mode");
                //ignore for standard and ultimate mode
                if (CurrentGPU != AsusACPI.GPUModeUltimate && CurrentGPU != AsusACPI.GPUModeStandard)
                {
                    if (AppConfig.Is("gpu_auto"))
                    {
                        AppConfig.Set("eco_shutdown", 2);
                    }
                    else
                    {
                        AppConfig.Set("eco_shutdown", 1);
                    }
                    gpuControl.SetGPUMode(AsusACPI.GPUModeStandard); //set standard mode to ensure safe boot
                    //Again, this will set standard mode for safe boot from eco mode and optimised unplugged state
                    //This will also trigger for optimised plugged in but does not need to be handled, on resume or
                    //restart the flag will trigger a reset to optimised, ultimatly no effect.
                }
            }
            if (e.Mode.ToString() == "Resume")
            {
                //Ensure correct modes are honored during system resume
                //Conditions only triggered if safe boot flags have been raised
                if (AppConfig.Get("eco_shutdown") == 1)
                {
                    gpuControl.SetGPUMode(AsusACPI.GPUModeEco);
                    AppConfig.Set("eco_shutdown", 0);
                }
                if (AppConfig.Get("eco_shutdown") == 2)
                {
                    AppConfig.Set("gpu_auto", (AppConfig.Get("gpu_auto") == 1) ? 0 : 1);
                    gpuControl.AutoGPUMode(true);
                    AppConfig.Set("eco_shutdown", 0);
                }
            }
            if (SystemInformation.PowerStatus.PowerLineStatus == isPlugged) return;
            Logger.WriteLine("Power Mode Changed");
            SetAutoModes(true);
        }



        public static void SettingsToggle(string action = "")
        {
            if (settingsForm.Visible) settingsForm.HideAll();
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
            clamshellControl.UnregisterDisplayEvents();
            NativeMethods.UnregisterPowerSettingNotification(unRegPowerNotify);
            Application.Exit();
        }


    }

}