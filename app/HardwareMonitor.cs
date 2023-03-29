using System.Diagnostics;
using GHelper;
using GHelper.Gpu;

public static class HardwareMonitor
{
    private static IGpuTemperatureProvider? GpuTemperatureProvider;

    public static float? cpuTemp = -1;
    public static float? batteryDischarge = -1;
    public static int? gpuTemp = null;

    public static string? cpuFan;
    public static string? gpuFan;
    public static string? midFan;

    private static string FormatFan(int fan)
    {
        // fix for old models 
        if (fan < 0)
        {
            fan += 65536;
            if (fan <= 0 || fan > 100) return null; //nothing reasonable
        }

        if (Program.config.getConfig("fan_rpm") == 1)
            return " Fan: " + (fan * 100).ToString() + "RPM";
        else
            return " Fan: " + Math.Min(Math.Round(fan / 0.6), 100).ToString() + "%"; // relatively to 6000 rpm
    }

    public static void ReadSensors()
    {
        cpuTemp = -1;
        batteryDischarge = -1;

        cpuFan = FormatFan(Program.wmi.DeviceGet(ASUSWmi.CPU_Fan));
        gpuFan = FormatFan(Program.wmi.DeviceGet(ASUSWmi.GPU_Fan));
        midFan = FormatFan(Program.wmi.DeviceGet(ASUSWmi.Mid_Fan));

        try
        {
            cpuTemp = Program.wmi.DeviceGet(ASUSWmi.Temp_CPU);
            if (cpuTemp < 0)
            {
                var ct = new PerformanceCounter("Thermal Zone Information", "Temperature", @"\_TZ.THRM", true);
                cpuTemp = ct.NextValue() - 273;
                ct.Dispose();
            }
        } catch
        {
            Logger.WriteLine("Failed reading CPU temp");
        }

        try
        {
            var cb = new PerformanceCounter("Power Meter", "Power", "Power Meter (0)", true);
            batteryDischarge = cb.NextValue() / 1000;
            cb.Dispose();

        } catch
        {
            Logger.WriteLine("Failed reading Battery discharge");
        }

        try
        {
            gpuTemp = Program.wmi.DeviceGet(ASUSWmi.Temp_GPU);
            if (gpuTemp < 0)
                gpuTemp = GpuTemperatureProvider?.GetCurrentTemperature();

        }
        catch (Exception ex) {
            gpuTemp = null;
            Logger.WriteLine("Failed reading GPU temp");
            Logger.WriteLine(ex.ToString());
        }

    }

    public static void RecreateGpuTemperatureProviderWithDelay() {

        // Re-enabling the discrete GPU takes a bit of time,
        // so a simple workaround is to refresh again after that happens
        Task.Run(async () => {
            await Task.Delay(TimeSpan.FromSeconds(5));
            RecreateGpuTemperatureProvider();
        });



    }

    public static void RecreateGpuTemperatureProvider() {
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
