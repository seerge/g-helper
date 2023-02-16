using System;
using System.Windows.Forms;

using System.Text.Json;
using System.Management;
using Microsoft.Win32.TaskScheduler;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Reflection;

using System.Diagnostics;

using GHelper;
using System.Dynamic;

public class ASUSWmi
{
    private ManagementObject mo;
    private ManagementClass classInstance;

    public const int CPU_Fan = 0x00110013;
    public const int GPU_Fan = 0x00110014;

    public const int PerformanceMode = 0x00120075;

    public const int GPUEco = 0x00090020;
    public const int GPUMux = 0x00090016;


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


static class Program
{
    static NotifyIcon trayIcon;

    public static ASUSWmi wmi;
    public static dynamic config = new System.Dynamic.ExpandoObject();

    public static SettingsForm settingsForm;

    // The main entry point for the application
    public static void Main()
    {
        trayIcon = new NotifyIcon
        {
            Text = "G14 Helper",
            Icon = new System.Drawing.Icon("Resources/standard.ico"),
            Visible = true
        };

        trayIcon.MouseClick += TrayIcon_MouseClick; ;

        wmi = new ASUSWmi();
        wmi.SubscribeToEvents(WatcherEventArrived);

        settingsForm = new SettingsForm();
        //settingsForm.Show();

        int GpuMode = GetGPUMode();

        settingsForm.SetPerformanceMode();
        settingsForm.VisualiseGPUMode(GpuMode);

        settingsForm.FormClosed += SettingsForm_FormClosed;

        config.PerformanceMode = 0;

        Application.Run();

    }

    public static int GetGPUMode ()
    {
        int eco = wmi.DeviceGet(ASUSWmi.GPUEco);
        int mux = wmi.DeviceGet(ASUSWmi.GPUMux);

        int GpuMode;

        if (mux == 0)
        {
            GpuMode = ASUSWmi.GPUModeUltimate;
        }
        else
        {
            if (eco == 1)
            {
                GpuMode = ASUSWmi.GPUModeEco;
            }
            else
            {
                GpuMode = ASUSWmi.GPUModeStandard;
            }

            if (mux != 1)
            {
                settingsForm.Disable_Ultimate();
            }
        }

        config.gpu_mode = GpuMode;

        return GpuMode;

    }

    private static void SettingsForm_FormClosed(object? sender, FormClosedEventArgs e)
    {
        trayIcon.Visible = false;
        Application.Exit();
    }

    static void WatcherEventArrived(object sender, EventArrivedEventArgs e)
    {
        var collection = (ManagementEventWatcher)sender;
        int EventID = int.Parse(e.NewEvent["EventID"].ToString());

        Debug.WriteLine(EventID);

        switch (EventID)
        {
            case 56:
            case 174:    
                CyclePerformanceMode();
                return;
        }


    }

    static void TrayIcon_MouseClick(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            if (settingsForm.Visible)
            {
                settingsForm.Hide();
            } else
            {
                settingsForm.Show();
                settingsForm.Activate();
            }
        }
    }


    static void CyclePerformanceMode()
    {
        settingsForm.BeginInvoke(delegate
        {
            settingsForm.SetPerformanceMode(config.PerformanceMode + 1);
        });
    }

    static void OnExit(object sender, EventArgs e)
    {
        trayIcon.Visible = false;
        Application.Exit();
    }
}

