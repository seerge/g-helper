using System.Diagnostics;
using GHelper.Gpu;

public static class HardwareMonitor
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
        } catch
        {
            Logger.WriteLine("Cpu sicakligi okunamadi.");
        }

        try
        {
            var cb = new PerformanceCounter("Power Meter", "Power", "Power Meter (0)", true);
            batteryDischarge = cb.NextValue() / 1000;
            cb.Dispose();

        } catch
        {
            Logger.WriteLine("Batarya tüketimi okunamadi.");
        }

        try
        {
            gpuTemp = GpuTemperatureProvider?.GetCurrentTemperature();
        } catch (Exception ex) {
            gpuTemp = null;
            Logger.WriteLine("GPU sicakligi okunamadi");
            Logger.WriteLine(ex.ToString());
        }

    }

    public static void RecreateGpuTemperatureProviderWithRetry() {
        RecreateGpuTemperatureProvider();

        // Re-enabling the discrete GPU takes a bit of time,
        // so a simple workaround is to refresh again after that happens
        Task.Run(async () => {
            await Task.Delay(TimeSpan.FromSeconds(3));
            RecreateGpuTemperatureProvider();
        });
    }

    public static void RecreateGpuTemperatureProvider() {
        try {
            GpuTemperatureProvider?.Dispose();

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
            Logger.WriteLine($"GpuTemperatureProvider: {GpuTemperatureProvider?.GetType().Name}");
        }
    }
}
