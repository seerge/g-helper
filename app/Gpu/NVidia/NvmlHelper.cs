using System.Runtime.InteropServices;

public static class NvmlHelper
{
    const string NvmlDll = "nvml.dll";

    [DllImport(NvmlDll)] static extern int nvmlInit_v2();
    [DllImport(NvmlDll)] static extern int nvmlShutdown();
    [DllImport(NvmlDll)] static extern int nvmlDeviceGetHandleByIndex_v2(uint index, out IntPtr device);
    [DllImport(NvmlDll)] static extern int nvmlDeviceGetTemperature(IntPtr device, uint sensorType, out uint temp);

    const uint NVML_TEMPERATURE_GPU = 0;
    const int NVML_SUCCESS = 0;

    static bool init = false;

    public static void Init()
    {
        if (!init)
        {
            try
            {
                var result = nvmlInit_v2();
                init = result == NVML_SUCCESS;
                Logger.WriteLine($"NVML Init result: {result}");
            }
            catch (Exception e)
            {
                Logger.WriteLine($"NVML Init exception: {e.Message}");
            }
        }
    }

    public static int? GetTemperature(uint gpuIndex = 0)
    {
        Init();
        try
        {
            if (nvmlDeviceGetHandleByIndex_v2(gpuIndex, out IntPtr device) != NVML_SUCCESS) return null;
            if (nvmlDeviceGetTemperature(device, NVML_TEMPERATURE_GPU, out uint temp) != NVML_SUCCESS) return null;
            return (int)temp;
        }
        catch
        {
            nvmlShutdown();
            init = false;
            return null;
        }
    }

    public static void Shutdown()
    {
        if (init)
        {
            try
            {
                nvmlShutdown();
                init = false;
            } catch
            {
            }
        }
    }

}