using LibreHardwareMonitor.Hardware;
using System.Diagnostics;
using System.Management;
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


public class UpdateVisitor : IVisitor
{
    public void VisitComputer(IComputer computer)
    {
        computer.Traverse(this);
    }
    public void VisitHardware(IHardware hardware)
    {
        hardware.Update();
        foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
    }
    public void VisitSensor(ISensor sensor) { }
    public void VisitParameter(IParameter parameter) { }
}


public class HardwareMonitor
{

    Computer computer;

    public float? cpuTemp = -1;
    public float? gpuTemp = -1;
    public float? batteryDischarge = -1;
    public float? batteryCharge = -1;

    public static bool IsAdministrator()
    {
        return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                  .IsInRole(WindowsBuiltInRole.Administrator);
    }

    public HardwareMonitor()
    {

    }

    public void ReadSensors()
    {

        try
        {
            if (computer is not Computer)
            {
                computer = new Computer
                {
                    IsGpuEnabled = true,
                    IsBatteryEnabled = true,
                };

                if (IsAdministrator()) computer.IsCpuEnabled = true;
            }

            computer.Open();
            computer.Accept(new UpdateVisitor());
        } catch
        {
            Debug.WriteLine("Failed to read sensors");
        }


        cpuTemp = -1;
        gpuTemp = -1;
        batteryDischarge = -1;
        batteryCharge = -1;

        foreach (IHardware hardware in computer.Hardware)
        {
            //Debug.WriteLine("Hardware: {0}", hardware.Name);
            //Debug.WriteLine("Hardware: {0}", hardware.HardwareType);

            foreach (ISensor sensor in hardware.Sensors)
            {
                if (sensor.SensorType == SensorType.Temperature)
                {
                    if (hardware.HardwareType.ToString().Contains("Cpu") && sensor.Name.Contains("Core"))
                    {
                        cpuTemp = sensor.Value;
                        //Debug.WriteLine("\tSensor: {0}, value: {1}", sensor.Name, sensor.Value);
                    }

                    if (hardware.HardwareType.ToString().Contains("Gpu") && sensor.Name.Contains("Core"))
                    {
                        gpuTemp = sensor.Value;
                    }

                    //Debug.WriteLine("\tSensor: {0}, value: {1}", sensor.Name, sensor.Value);

                }
                else if (sensor.SensorType == SensorType.Power)
                {
                    if (sensor.Name.Contains("Discharge"))
                    {
                        batteryDischarge = sensor.Value;
                    }

                    if (sensor.Name.Contains("Charge"))
                    {
                        batteryCharge = sensor.Value;
                    }
                }



            }
        }

    }

    public void StopReading()
    {
        //computer.Close();
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
        public static HardwareMonitor hwmonitor = new HardwareMonitor();

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


            IntPtr dummy = settingsForm.Handle;

            Application.Run();

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
                    settingsForm.SetBatteryChargeLimit(config.getConfig("charge_limit"));
                    settingsForm.BeginInvoke(delegate
                    {
                        settingsForm.AutoGPUMode(1);
                        settingsForm.AutoScreen(1);
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