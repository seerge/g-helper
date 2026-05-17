using GHelper;
using GHelper.Battery;
using GHelper.Fan;
using GHelper.Gpu;
using GHelper.Gpu.AMD;
using GHelper.Gpu.NVidia;
using GHelper.Helpers;
using System.Diagnostics;
using System.Management;

public static class HardwareControl
{

    public static IGpuControl? GpuControl;

    public static float? cpuTemp = -1;
    public static float? gpuTemp = -1;

    public static string? cpuFan;
    public static string? gpuFan;
    public static string? midFan;

    public static int? gpuUse;

    static long lastUpdate;

    static bool isPZ13 = AppConfig.IsPZ13();

    static PerformanceCounter? _cpuTempCounter;

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

    public static float? GetCPUTemp()
    {
        var last = DateTimeOffset.Now.ToUnixTimeSeconds();
        if (Math.Abs(last - lastUpdate) < 2) return cpuTemp;
        lastUpdate = last;

        if (isPZ13) return (float)GetCPUTempWMI();
        cpuTemp = Program.acpi.DeviceGet(AsusACPI.Temp_CPU);

        if (cpuTemp < 0) try
        {
            if (_cpuTempCounter == null)
                _cpuTempCounter = new PerformanceCounter("Thermal Zone Information", "Temperature", @"\_TZ.THRM", true);

            cpuTemp = _cpuTempCounter.NextValue() - 273;
        }
        catch (Exception ex)
        {
            //Debug.WriteLine("Failed reading CPU temp :" + ex.Message);
        }


        return cpuTemp;
    }

    static double GetCPUTempWMI()
    {
        try
        {
            string wmiNamespace = @"root\WMI";
            string wmiQuery = @"SELECT CurrentTemperature FROM MSAcpi_ThermalZoneTemperature WHERE InstanceName = 'ACPI\\QCOM0C5A\\1_0'";  // ACPI\\ThermalZone\\THRM_0
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmiNamespace, wmiQuery))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    using (obj)
                    {
                        double tempKelvin = Convert.ToDouble(obj["CurrentTemperature"]);
                        return (tempKelvin / 10) - 273.15;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            //Logger.WriteLine("Error retrieving temperature: " + ex.Message);
        }
        return -1;
    }

    public static float? GetGPUTemp()
    {
        try
        {
            gpuTemp = GpuControl?.GetCurrentTemperature();

        }
        catch (Exception ex)
        {
            gpuTemp = -1;
            //Debug.WriteLine("Failed reading GPU temp :" + ex.Message);
        }

        if (gpuTemp is null || gpuTemp < 0)
        {
            int acpiTemp = Program.acpi.DeviceGet(AsusACPI.Temp_GPU);
            gpuTemp = (acpiTemp > 0 && acpiTemp < 125) ? acpiTemp : null;
        }

        return gpuTemp;
    }


    public static void ReadSensors(bool log = false)
    {
        gpuUse = -1;

        if (Program.acpi is null) return;

        cpuFan = FanSensorControl.FormatFan(AsusFan.CPU, Program.acpi.GetFan(AsusFan.CPU));
        gpuFan = FanSensorControl.FormatFan(AsusFan.GPU, Program.acpi.GetFan(AsusFan.GPU));
        midFan = FanSensorControl.FormatFan(AsusFan.Mid, Program.acpi.GetFan(AsusFan.Mid));

        cpuTemp = GetCPUTemp();
        gpuTemp = GetGPUTemp();

        if (log) Logger.WriteLine($"Temps: {cpuTemp} {gpuTemp} {cpuFan} {gpuFan} {midFan}");

        BatteryReader.ReadBatteryState();
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

    public static void DisposeGpuControl()
    {
        GpuControl?.Dispose();
        GpuControl = null;
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
        if (AppConfig.NoGpu()) return;
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
                if (GpuControl.FullName.Contains("6850M")) AppConfig.Set("xgm_special", 1);
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

        List<string> tokill = new() { "EADesktop", "epicgameslauncher", "ASUSSmartDisplayControl" };

        foreach (string kill in tokill) ProcessHelper.KillByName(kill);

        if (AppConfig.Is("kill_gpu_apps") && GpuControl is not null)
        {
            GpuControl.KillGPUApps();
        }
    }

    public static void Dispose()
    {
        _cpuTempCounter?.Dispose();
        _cpuTempCounter = null;
    }
}
