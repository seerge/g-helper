using Microsoft.Win32;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text.Json;


public class AppConfig
{

    string appPath;
    string configFile;

    public Dictionary<string, object> config = new Dictionary<string, object>();

    public AppConfig()
    {

        appPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\GHelper";
        configFile = appPath + "\\config.json";

        if (!System.IO.Directory.Exists(appPath))
            System.IO.Directory.CreateDirectory(appPath);

        if (File.Exists(configFile))
        {
            string text = File.ReadAllText(configFile);
            try
            {
                config = JsonSerializer.Deserialize<Dictionary<string, object>>(text);
            }
            catch
            {
                initConfig();
            }
        }
        else
        {
            initConfig();
        }

    }

    private void initConfig()
    {
        config = new Dictionary<string, object>();
        config["performance_mode"] = 0;
        string jsonString = JsonSerializer.Serialize(config);
        File.WriteAllText(configFile, jsonString);
    }

    public int getConfig(string name)
    {
        if (config.ContainsKey(name))
            return int.Parse(config[name].ToString());
        else return -1;
    }

    public string getConfigString(string name)
    {
        if (config.ContainsKey(name))
            return config[name].ToString();
        else return null;
    }

    public void setConfig(string name, int value)
    {
        config[name] = value;
        string jsonString = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(configFile, jsonString);
    }

    public void setConfig(string name, string value)
    {
        config[name] = value;
        string jsonString = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(configFile, jsonString);
    }


}

public class HardwareMonitor
{

    public static float? cpuTemp = -1;
    public static float? batteryDischarge = -1;


    public static void ReadSensors()
    {
        cpuTemp = -1;
        batteryDischarge = -1;

        try
        {
            var ct = new PerformanceCounter("Thermal Zone Information", "Temperature", @"\_TZ.THRM", true);
            cpuTemp = ct.NextValue() - 273;
            ct.Dispose();

            var cb = new PerformanceCounter("Power Meter", "Power", "Power Meter (0)", true);
            batteryDischarge = ct.NextValue() / 1000;
            cb.Dispose();
        }
        catch
        {
            Debug.WriteLine("Failed reading sensors");
        }
    }

}

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

        public static ASUSWmi wmi = new ASUSWmi();
        public static AppConfig config = new AppConfig();
        
        public static SettingsForm settingsForm = new SettingsForm();
        public static ToastForm toast = new ToastForm();

        // The main entry point for the application
        public static void Main()
        {

            trayIcon.MouseClick += TrayIcon_MouseClick; ;

            wmi.SubscribeToEvents(WatcherEventArrived);

            settingsForm.InitGPUMode();
            settingsForm.InitBoost();
            settingsForm.InitAura();

            settingsForm.SetPerformanceMode(config.getConfig("performance_mode"));
            settingsForm.SetBatteryChargeLimit(config.getConfig("charge_limit"));

            settingsForm.VisualiseGPUAuto(config.getConfig("gpu_auto"));
            settingsForm.VisualiseScreenAuto(config.getConfig("screen_auto"));

            settingsForm.SetStartupCheck(Startup.IsScheduled());

            bool isPlugged = (System.Windows.Forms.SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online);
            settingsForm.AutoGPUMode(isPlugged ? 1 : 0);
            settingsForm.AutoScreen(isPlugged ? 1 : 0);

            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;

            IntPtr ds = settingsForm.Handle;

            Application.Run();

        }

        private static void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            settingsForm.SetBatteryChargeLimit(config.getConfig("charge_limit"));
        }

        static void WatcherEventArrived(object sender, EventArrivedEventArgs e)
        {
            var collection = (ManagementEventWatcher)sender;

            if (e.NewEvent is null) return;

            int EventID = int.Parse(e.NewEvent["EventID"].ToString());

            Debug.WriteLine(EventID);

            switch (EventID)
            {
                case 56:    // Rog button
                case 174:   // FN+F5

                    settingsForm.BeginInvoke(delegate
                    {
                        settingsForm.CyclePerformanceMode();
                    });

                    return;
                case 179:   // FN+F4
                    settingsForm.BeginInvoke(delegate
                    {
                        settingsForm.CycleAuraMode();
                    });
                    return;
                case 87:  // Battery
                    settingsForm.BeginInvoke(delegate
                    {
                        settingsForm.AutoGPUMode(0);
                        settingsForm.AutoScreen(0);
                    });
                    return;
                case 88:  // Plugged
                    settingsForm.BeginInvoke(delegate
                    {
                        settingsForm.AutoScreen(1);
                        settingsForm.AutoGPUMode(1);
                    });
                    return;

            }


        }

        static void TrayIcon_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                if (settingsForm.Visible)
                    settingsForm.Hide();
                else
                {
                    settingsForm.Show();
                    settingsForm.Activate();
                }

                trayIcon.Icon = trayIcon.Icon; // refreshing icon as it get's blurred when screen resolution changes
            }
        }



        static void OnExit(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            Application.Exit();
        }
    }

}