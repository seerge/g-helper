using GHelper;
using GHelper.Gpu;
using System.Diagnostics;
using System.Management;

public static class HardwareMonitor
{
    private static IGpuTemperatureProvider? GpuTemperatureProvider;

    public static float? cpuTemp = -1;
    public static float? batteryDischarge = -1;
    public static int? gpuTemp = null;

    public static string? cpuFan;
    public static string? gpuFan;
    public static string? midFan;

    //public static List<int> gpuUsage = new List<int>();
    public static int? gpuUse;

    public static int GetFanMax()
    {
        int max = 58;
        if (Program.config.ContainsModel("401")) max = 72;
        else if (Program.config.ContainsModel("503")) max = 68;
        return Math.Max(max, Program.config.getConfig("fan_max"));
    }

    public static void SetFanMax(int fan)
    {
        Program.config.setConfig("fan_max", fan);
    }
    private static string FormatFan(int fan)
    {
        // fix for old models 
        if (fan < 0)
        {
            fan += 65536;
            if (fan <= 0 || fan > 100) return null; //nothing reasonable
        }

        int fanMax = GetFanMax();
        if (fan > fanMax) SetFanMax(fan);

        if (Program.config.getConfig("fan_rpm") == 1)
            return " Fan: " + (fan * 100).ToString() + "RPM";
        else
            return " Fan: " + Math.Min(Math.Round((float)fan / fanMax * 100), 100).ToString() + "%"; // relatively to 6000 rpm
    }

    private static int GetGpuUse()
    {
        try
        {
            int? gpuUse = GpuTemperatureProvider?.GetGpuUse();
            if (gpuUse is not null) return (int)gpuUse;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }

        return 0;
    }


    public static void ReadSensors()
    {
        batteryDischarge = -1;
        gpuTemp = -1;
        gpuUse = -1;

        cpuFan = FormatFan(Program.wmi.DeviceGet(ASUSWmi.CPU_Fan));
        gpuFan = FormatFan(Program.wmi.DeviceGet(ASUSWmi.GPU_Fan));
        midFan = FormatFan(Program.wmi.DeviceGet(ASUSWmi.Mid_Fan));

        cpuTemp = Program.wmi.DeviceGet(ASUSWmi.Temp_CPU);

        if (cpuTemp < 0) try
        {
            var ct = new PerformanceCounter("Thermal Zone Information", "Temperature", @"\_TZ.THRM", true);
            cpuTemp = ct.NextValue() - 273;
            ct.Dispose();
        }
        catch
        {
            Debug.WriteLine("Failed reading CPU temp");
        }

        try
        {
            gpuTemp = GpuTemperatureProvider?.GetCurrentTemperature();

        }
        catch (Exception ex)
        {
            gpuTemp = -1;
            Debug.WriteLine("Failed reading GPU temp");
            Debug.WriteLine(ex.ToString());
        }

        if (gpuTemp is null || gpuTemp < 0)
            gpuTemp = Program.wmi.DeviceGet(ASUSWmi.Temp_GPU);

        /*
        gpuUsage.Add(GetGpuUse());
        if (gpuUsage.Count > 3) gpuUsage.RemoveAt(0);
        */

        try
        {
            var cb = new PerformanceCounter("Power Meter", "Power", "Power Meter (0)", true);
            batteryDischarge = cb.NextValue() / 1000;
            cb.Dispose();

        }
        catch
        {
            Debug.WriteLine("Failed reading Battery discharge");
        }
    }

    public static bool IsUsedGPU(int threshold = 20)
    {
        if (GetGpuUse() > threshold)
        {
            Thread.Sleep(1000);
            return (GetGpuUse() > threshold);
        } 
        return false;
    }

    public static void RecreateGpuTemperatureProviderWithDelay()
    {

        // Re-enabling the discrete GPU takes a bit of time,
        // so a simple workaround is to refresh again after that happens
        Task.Run(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
            RecreateGpuTemperatureProvider();
        });



    }

    public static void RecreateGpuTemperatureProvider()
    {
        try
        {
            GpuTemperatureProvider?.Dispose();

            // Detect valid GPU temperature provider.
            // We start with NVIDIA because there's always at least an integrated AMD GPU
            IGpuTemperatureProvider gpuTemperatureProvider = new NvidiaGpuTemperatureProvider();
            if (gpuTemperatureProvider.IsValid)
            {
                GpuTemperatureProvider = gpuTemperatureProvider;
                return;
            }

            gpuTemperatureProvider.Dispose();
            gpuTemperatureProvider = new AmdGpuTemperatureProvider();
            if (gpuTemperatureProvider.IsValid)
            {
                GpuTemperatureProvider = gpuTemperatureProvider;
                return;
            }

            gpuTemperatureProvider.Dispose();

            GpuTemperatureProvider = null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }
    }
}
