using System.Runtime.InteropServices;

public static class NvmlHelper
{
    const string NvmlDll = "nvml.dll";

    [DllImport(NvmlDll)] static extern int nvmlInit_v2();
    [DllImport(NvmlDll)] static extern int nvmlShutdown();
    [DllImport(NvmlDll)] static extern int nvmlDeviceGetHandleByIndex_v2(uint index, out IntPtr device);
    [DllImport(NvmlDll)] static extern int nvmlDeviceGetTemperature(IntPtr device, uint sensorType, out uint temp);
    [DllImport(NvmlDll)] static extern int nvmlDeviceGetPowerUsage(IntPtr device, out uint powerMilliWatts);

    const uint NVML_TEMPERATURE_GPU = 0;
    const int NVML_SUCCESS = 0;

    private static readonly object _lock = new();
    private static bool _init = false;

    public static void Init()
    {
        if (_init) return;
        try
        {
            int rc = nvmlInit_v2();
            _init = rc == NVML_SUCCESS;
            Logger.WriteLine($"NVML Init: {rc}");
        }
        catch (Exception e)
        {
            Logger.WriteLine($"NVML Init exception: {e.Message}");
        }
    }

    public static int? GetTemperature(uint gpuIndex = 0)
    {
        lock (_lock)
        {
            Init();
            if (!_init) return null;
            try
            {
                if (nvmlDeviceGetHandleByIndex_v2(gpuIndex, out IntPtr device) != NVML_SUCCESS) return null;
                if (nvmlDeviceGetTemperature(device, NVML_TEMPERATURE_GPU, out uint temp) != NVML_SUCCESS) return null;
                return (int)temp;
            }
            catch { return null; }
        }
    }

    public static float? GetGpuPower(uint gpuIndex = 0)
    {
        lock (_lock)
        {
            Init();
            if (!_init) return null;
            try
            {
                if (nvmlDeviceGetHandleByIndex_v2(gpuIndex, out IntPtr device) != NVML_SUCCESS) return null;
                if (nvmlDeviceGetPowerUsage(device, out uint mW) != NVML_SUCCESS) return null;
                if (mW > 200_000f) return null;
                return mW / 1000f;
            }
            catch { return null; }
        }
    }

    public static void Shutdown()
    {
        lock (_lock)
        {
            if (!_init) return;
            try
            {
                int rc = nvmlShutdown();
                Logger.WriteLine($"NVML Shutdown: {rc}");
            }
            catch { }
            _init = false;
        }
    }
}
