﻿using GHelper;
using GHelper.Gpu;
using GHelper.Gpu.NVidia;
using GHelper.Gpu.AMD;

using GHelper.Helpers;
using System.Diagnostics;
using System.Management;

public static class HardwareControl
{

    const int DEFAULT_FAN_MAX = 58;
    const int INADEQUATE_MAX = 80;

    public static IGpuControl? GpuControl;

    public static float? cpuTemp = -1;
    public static decimal? batteryRate = 0;
    public static decimal batteryHealth = -1;
    public static decimal batteryCapacity = -1;

    public static decimal? designCapacity;
    public static decimal? fullCapacity;
    public static decimal? chargeCapacity;


    public static int? gpuTemp = null;

    public static string? cpuFan;
    public static string? gpuFan;
    public static string? midFan;

    public static int? gpuUse;

    static long lastUpdate;

    static int _fanMax = DEFAULT_FAN_MAX;
    static bool _fanRpm = false;

    public static int fanMax
    {
        get
        {
            return _fanMax;
        }
        set
        {
            AppConfig.Set("fan_max", value);
            _fanMax = value;
        }
    }

    public static bool fanRpm
    {
        get
        {
            return _fanRpm;
        }
        set
        {
            AppConfig.Set("fan_rpm", value ? 1 : 0);
            _fanRpm = value;
        }
    }

    static HardwareControl()
    {
        _fanMax = AppConfig.Get("fan_max");
        if (_fanMax > INADEQUATE_MAX) _fanMax = -1; // skipping inadvequate settings

        if (_fanMax < 0 && AppConfig.ContainsModel("401")) _fanMax = 72;
        if (_fanMax < 0 && AppConfig.ContainsModel("503")) _fanMax = 68;
        if (_fanMax < 0) _fanMax = DEFAULT_FAN_MAX;

        _fanRpm = AppConfig.IsNotFalse("fan_rpm");

    }

    public static string FormatFan(int fan)
    {
        // fix for old models 
        if (fan < 0)
        {
            fan += 65536;
            if (fan <= 0 || fan > 100) return null; //nothing reasonable
        }

        if (fan > fanMax && fan <= INADEQUATE_MAX) fanMax = fan;

        if (fanRpm)
            return GHelper.Properties.Strings.FanSpeed + ": " + (fan * 100).ToString() + "RPM";
        else
            return GHelper.Properties.Strings.FanSpeed + ": " + Math.Min(Math.Round((float)fan / fanMax * 100), 100).ToString() + "%"; // relatively to 6000 rpm
    }

    private static int GetGpuUse()
    {
        try
        {
            int? gpuUse = GpuControl?.GetGpuUse();
            Logger.WriteLine("GPU usage: " + GpuControl?.FullName + " " + gpuUse + "%");
            if (gpuUse is not null) return (int)gpuUse;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }

        return 0;
    }


    public static void GetBatteryStatus()
    {

        batteryRate = 0;
        chargeCapacity = 0;

        try
        {
            ManagementScope scope = new ManagementScope("root\\WMI");
            ObjectQuery query = new ObjectQuery("SELECT * FROM BatteryStatus");

            using ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            foreach (ManagementObject obj in searcher.Get().Cast<ManagementObject>())
            {

                chargeCapacity = Convert.ToDecimal(obj["RemainingCapacity"]);

                decimal chargeRate = Convert.ToDecimal(obj["ChargeRate"]);
                decimal dischargeRate = Convert.ToDecimal(obj["DischargeRate"]);
                
                if (chargeRate > 0)
                    batteryRate = chargeRate / 1000;
                else
                    batteryRate = -dischargeRate / 1000;

            }

        }
        catch (Exception ex)
        {
            Logger.WriteLine("Discharge Reading: " + ex.Message);
        }

    }
    public static void ReadFullChargeCapacity()
    {
        if (fullCapacity > 0) return;

        try
        {
            ManagementScope scope = new ManagementScope("root\\WMI");
            ObjectQuery query = new ObjectQuery("SELECT * FROM BatteryFullChargedCapacity");

            using ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            foreach (ManagementObject obj in searcher.Get().Cast<ManagementObject>())
            {
                fullCapacity = Convert.ToDecimal(obj["FullChargedCapacity"]);
            }

        }
        catch (Exception ex)
        {
            Logger.WriteLine("Full Charge Reading: " + ex.Message);
        }

    }

    public static void ReadDesignCapacity()
    {
        if (designCapacity > 0) return;

        try
        {
            ManagementScope scope = new ManagementScope("root\\WMI");
            ObjectQuery query = new ObjectQuery("SELECT * FROM BatteryStaticData");

            using ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            foreach (ManagementObject obj in searcher.Get().Cast<ManagementObject>())
            {
                designCapacity = Convert.ToDecimal(obj["DesignedCapacity"]);
            }

        }
        catch (Exception ex)
        {
            Logger.WriteLine("Design Capacity Reading: " + ex.Message);
        }
    }

    public static void RefreshBatteryHealth()
    {
        batteryHealth = GetBatteryHealth() * 100;
    }


    public static decimal GetBatteryHealth()
    {
        if (designCapacity is null)
        {
            ReadDesignCapacity();
        }
        ReadFullChargeCapacity();

        if (designCapacity is null || fullCapacity is null || designCapacity == 0 || fullCapacity == 0)
        {
            return -1;
        }

        decimal health = (decimal)fullCapacity / (decimal)designCapacity;
        Logger.WriteLine("Design Capacity: " + designCapacity + "mWh, Full Charge Capacity: " + fullCapacity + "mWh, Health: " + health + "%");

        return health;
    }

    public static float? GetCPUTemp() {

        var last = DateTimeOffset.Now.ToUnixTimeSeconds();
        if (Math.Abs(last - lastUpdate) < 2) return cpuTemp;
        lastUpdate = last;

        cpuTemp = Program.acpi.DeviceGet(AsusACPI.Temp_CPU);

        if (cpuTemp < 0) try
            {
                using (var ct = new PerformanceCounter("Thermal Zone Information", "Temperature", @"\_TZ.THRM", true))
                {
                    cpuTemp = ct.NextValue() - 273;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed reading CPU temp :" + ex.Message);
            }


        return cpuTemp;
    }


    public static void ReadSensors()
    {
        batteryRate = 0;
        gpuTemp = -1;
        gpuUse = -1;

        cpuFan = FormatFan(Program.acpi.DeviceGet(AsusACPI.CPU_Fan));
        gpuFan = FormatFan(Program.acpi.DeviceGet(AsusACPI.GPU_Fan));
        midFan = FormatFan(Program.acpi.DeviceGet(AsusACPI.Mid_Fan));

        cpuTemp = GetCPUTemp();

        try
        {
            gpuTemp = GpuControl?.GetCurrentTemperature();

        }
        catch (Exception ex)
        {
            gpuTemp = -1;
            Debug.WriteLine("Failed reading GPU temp :" + ex.Message);
        }

        if (gpuTemp is null || gpuTemp < 0)
            gpuTemp = Program.acpi.DeviceGet(AsusACPI.Temp_GPU);

        ReadFullChargeCapacity();
        GetBatteryStatus();

        if (fullCapacity > 0 && chargeCapacity > 0)
        {
            batteryCapacity = Math.Min(100, ((decimal)chargeCapacity / (decimal)fullCapacity) * 100);
        }


    }

    public static bool IsUsedGPU(int threshold = 10)
    {
        if (GetGpuUse() > threshold)
        {
            Thread.Sleep(1000);
            return (GetGpuUse() > threshold);
        }
        return false;
    }


    public static NvidiaGpuControl? GetNvidiaGpuControl()
    {
        if ((bool)GpuControl?.IsNvidia)
            return (NvidiaGpuControl)GpuControl;
        else
            return null;
    }

    public static void RecreateGpuControlWithDelay(int delay = 5)
    {
        // Re-enabling the discrete GPU takes a bit of time,
        // so a simple workaround is to refresh again after that happens
        Task.Run(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(delay));
            RecreateGpuControl();
        });
    }

    public static void RecreateGpuControl()
    {
        try
        {
            GpuControl?.Dispose();

            IGpuControl _gpuControl = new NvidiaGpuControl();
            
            if (_gpuControl.IsValid)
            {
                GpuControl = _gpuControl;
                Logger.WriteLine(GpuControl.FullName);
                return;
            }

            _gpuControl.Dispose();

            _gpuControl = new AmdGpuControl();
            if (_gpuControl.IsValid)
            {
                GpuControl = _gpuControl;
                Logger.WriteLine(GpuControl.FullName);
                return;
            }
            _gpuControl.Dispose();

            Logger.WriteLine("dGPU not found");
            GpuControl = null;


        }
        catch (Exception ex)
        {
            Debug.WriteLine("Can't connect to GPU " + ex.ToString());
        }
    }


    public static void KillGPUApps()
    {

        List<string> tokill = new() { "EADesktop", "RadeonSoftware", "epicgameslauncher", "ASUSSmartDisplayControl" };
        foreach (string kill in tokill) ProcessHelper.KillByName(kill);

        if (AppConfig.Is("kill_gpu_apps") && GpuControl is not null)
        {
            GpuControl.KillGPUApps();
        }
    }
}
