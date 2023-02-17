using Microsoft.Win32.TaskScheduler;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Reflection.Metadata.Ecma335;

public class ASUSWmi
{
    private ManagementObject mo;
    private ManagementClass classInstance;

    public const int CPU_Fan = 0x00110013;
    public const int GPU_Fan = 0x00110014;

    public const int PerformanceMode = 0x00120075;

    public const int GPUEco = 0x00090020;
    public const int GPUMux = 0x00090016;

    public const int BatteryLimit = 0x00120057;

    public const int PerformanceBalanced = 0;
    public const int PerformanceTurbo = 1;
    public const int PerformanceSilent = 2;

    public const int GPUModeEco = 0;
    public const int GPUModeStandard = 1;
    public const int GPUModeUltimate = 2;

    public ASUSWmi()
    {
        this.classInstance = new ManagementClass(new ManagementScope("root\\wmi"), new ManagementPath("AsusAtkWmi_WMNB"), null);
        foreach (ManagementObject mo in this.classInstance.GetInstances())
        {
            this.mo = mo;
        }
    }

    private int WMICall(string MethodName, int Device_Id, int Control_status = -1)
    {
        ManagementBaseObject inParams = this.classInstance.Methods[MethodName].InParameters;
        inParams["Device_ID"] = Device_Id;
        if (Control_status != -1)
        {
            inParams["Control_status"] = Control_status;
        }

        ManagementBaseObject outParams = this.mo.InvokeMethod(MethodName, inParams, null);
        foreach (PropertyData property in outParams.Properties)
        {
            if (property.Name == "device_status") return int.Parse(property.Value.ToString()) - 65536;
            if (property.Name == "result") return int.Parse(property.Value.ToString());
        }

        return -1;

    }

    public int DeviceGet(int Device_Id)
    {
        return this.WMICall("DSTS", Device_Id);
    }
    public int DeviceSet(int Device_Id, int Control_status)
    {
        return this.WMICall("DEVS", Device_Id, Control_status);
    }

    public void SubscribeToEvents(Action<object, EventArrivedEventArgs> EventHandler)
    {

        ManagementEventWatcher watcher = new ManagementEventWatcher();

        watcher.EventArrived += new EventArrivedEventHandler(EventHandler);
        watcher.Scope = new ManagementScope("root\\wmi");
        watcher.Query = new WqlEventQuery("SELECT * FROM AsusAtkWmiEvent");

        watcher.Start();

    }

}

public class Startup
{

    static string taskName = "GSharpHelper";

    public Startup()
    {

    }

    public bool IsScheduled()
    {
        TaskService taskService = new TaskService();
        return (taskService.RootFolder.AllTasks.Any(t => t.Name == taskName));
    }

    public void Schedule()
    {
        TaskService taskService = new TaskService();

        string strExeFilePath = Application.ExecutablePath;

        if (strExeFilePath is null) return;

        Debug.WriteLine(strExeFilePath);
        TaskDefinition td = TaskService.Instance.NewTask();
        td.RegistrationInfo.Description = "GSharpHelper Auto Start";

        LogonTrigger lt = new LogonTrigger();
        td.Triggers.Add(lt);
        td.Actions.Add(strExeFilePath);
        td.Principal.RunLevel = TaskRunLevel.Highest;
        td.Settings.StopIfGoingOnBatteries = false;
        td.Settings.DisallowStartIfOnBatteries = false;

        TaskService.Instance.RootFolder.RegisterTaskDefinition(taskName, td);
    }

    public void UnSchedule()
    {
        TaskService taskService = new TaskService();
        taskService.RootFolder.DeleteTask(taskName);
    }
}


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
            config = JsonConvert.DeserializeObject<Dictionary<string, object>>(text);
            if (config is null)
                initConfig();
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
        string jsonString = JsonConvert.SerializeObject(config);
        File.WriteAllText(configFile, jsonString);
    }

    public int getConfig(string name)
    {
        if (config.ContainsKey(name)) 
            return int.Parse(config[name].ToString());
        else return -1;
    }

    public void setConfig(string name, int value)
    {
        config[name] = value;
        string jsonString = JsonConvert.SerializeObject(config);
        File.WriteAllText(configFile, jsonString);
    }



}

namespace GHelper
{
    static class Program
    {
        public static NotifyIcon trayIcon;

        public static ASUSWmi wmi;
        public static AppConfig config;

        public static SettingsForm settingsForm;
        public static Startup scheduler;

        // The main entry point for the application
        public static void Main()
        {
            trayIcon = new NotifyIcon
            {
                Text = "G14 Helper",
                Icon = GHelper.Properties.Resources.standard,
                Visible = true
            };

            trayIcon.MouseClick += TrayIcon_MouseClick; ;

            config = new AppConfig();

            wmi = new ASUSWmi();
            wmi.SubscribeToEvents(WatcherEventArrived);

            scheduler = new Startup();

            settingsForm = new SettingsForm();

            settingsForm.InitGPUMode();

            settingsForm.SetPerformanceMode(config.getConfig("performance_mode"));
            settingsForm.SetBatteryChargeLimit(config.getConfig("charge_limit"));

            settingsForm.VisualiseGPUAuto(config.getConfig("gpu_auto"));
            settingsForm.SetStartupCheck(scheduler.IsScheduled());

            Application.Run();

        }


        static void WatcherEventArrived(object sender, EventArrivedEventArgs e)
        {
            var collection = (ManagementEventWatcher)sender;
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
                case 87:  // Battery
                    settingsForm.BeginInvoke(delegate
                    {
                        settingsForm.AutoGPUMode(0);
                    });
                    return;
                case 88:  // Plugged
                    settingsForm.SetBatteryChargeLimit(config.getConfig("charge_limit"));
                    settingsForm.BeginInvoke(delegate
                    {
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
            }
        }



        static void OnExit(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            Application.Exit();
        }
    }

}