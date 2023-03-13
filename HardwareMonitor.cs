using System.Diagnostics;
using GHelper.Gpu;

public class HardwareMonitor
{
    private static IGpuTemperatureProvider? GpuTemperatureProvider;

    public static float? cpuTemp = -1;
    public static float? batteryDischarge = -1;
    public static int? gpuTemp = null;

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
            batteryDischarge = cb.NextValue() / 1000;
            cb.Dispose();

        }
        catch
        {
            //Logger.WriteLine("Failed reading sensors");
        }

        try
        {
            gpuTemp = GpuTemperatureProvider?.GetCurrentTemperature();
        } catch
        {
            //Logger.WriteLine("Failed reading GPU temps");
        }

    }

    public static void RecreateGpuTemperatureProvider() {
        try {
            if (GpuTemperatureProvider != null) {
                GpuTemperatureProvider.Dispose();
            }
            
            // Detect valid GPU temperature provider.
            // We start with NVIDIA because there's always at least an integrated AMD GPU
            IGpuTemperatureProvider gpuTemperatureProvider = new NvidiaGpuTemperatureProvider();
            if (gpuTemperatureProvider.IsValid) {
                GpuTemperatureProvider = gpuTemperatureProvider;
                return;
            }
        
            gpuTemperatureProvider.Dispose();
            gpuTemperatureProvider = new AmdGpuTemperatureProvider();
            if (gpuTemperatureProvider.IsValid) {
                GpuTemperatureProvider = gpuTemperatureProvider;
                return;
            }
        
            gpuTemperatureProvider.Dispose();
        
            GpuTemperatureProvider = null;
        } finally {
            Debug.WriteLine($"GpuTemperatureProvider: {GpuTemperatureProvider?.GetType().Name}");
        }
    }
}
