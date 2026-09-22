using System.Runtime.InteropServices;

public static class NvmlHelper
{
    const string NvmlDll = "nvml.dll";

    [DllImport(NvmlDll)] static extern int nvmlInit_v2();
    [DllImport(NvmlDll)] static extern int nvmlShutdown();
    [DllImport(NvmlDll)] static extern int nvmlDeviceGetHandleByIndex_v2(uint index, out IntPtr device);
    [DllImport(NvmlDll)] static extern int nvmlDeviceGetTemperature(IntPtr device, uint sensorType, out uint temp);
    [DllImport(NvmlDll)] static extern int nvmlDeviceGetPowerUsage(IntPtr device, out uint powerMilliWatts);
    [DllImport(NvmlDll)] static extern int nvmlDeviceGetMemoryInfo(IntPtr device, out nvmlMemory_t memory);
    [DllImport(NvmlDll)] static extern int nvmlDeviceGetFieldValues(IntPtr device, int valuesCount, nvmlFieldValue_t[] values);

    [StructLayout(LayoutKind.Sequential)]
    struct nvmlMemory_t { public ulong total; public ulong free; public ulong used; }

    [StructLayout(LayoutKind.Sequential)]
    struct nvmlFieldValue_t
    {
        public uint fieldId;
        public uint scopeId;
        public long timestamp;
        public long latencyUsec;
        public uint valueType;
        public int nvmlReturn;
        public ulong value; // union; power fields put milliwatts in the low uint
    }

    const uint NVML_TEMPERATURE_GPU = 0;
    const uint NVML_FI_DEV_POWER_INSTANT = 186;
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

                if (nvmlDeviceGetPowerUsage(device, out uint mW) == NVML_SUCCESS && mW > 0 && mW <= 200_000)
                    return mW / 1000f;

                // Mobile Ada (RTX 4090 Laptop, driver 2026-07): nvmlDeviceGetPowerUsage
                // reports a bogus scale (~590 W at idle). nvidia-smi itself reads the
                // POWER_INSTANT field, which is sane on the same driver - use it as a
                // fallback whenever the legacy call fails the sanity check.
                var fields = new nvmlFieldValue_t[] { new() { fieldId = NVML_FI_DEV_POWER_INSTANT } };
                if (nvmlDeviceGetFieldValues(device, 1, fields) == NVML_SUCCESS
                    && fields[0].nvmlReturn == NVML_SUCCESS)
                {
                    uint instMw = (uint)fields[0].value;
                    if (instMw > 0 && instMw <= 200_000) return instMw / 1000f;
                }

                return null;
            }
            catch { return null; }
        }
    }

    public static (long usedMb, long totalMb)? GetMemoryInfo(uint gpuIndex = 0)
    {
        lock (_lock)
        {
            Init();
            if (!_init) return null;
            try
            {
                if (nvmlDeviceGetHandleByIndex_v2(gpuIndex, out IntPtr device) != NVML_SUCCESS) return null;
                if (nvmlDeviceGetMemoryInfo(device, out nvmlMemory_t mem) != NVML_SUCCESS) return null;
                if (mem.total == 0) return null;
                const ulong MB = 1024 * 1024;
                return ((long)(mem.used / MB), (long)(mem.total / MB));
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
