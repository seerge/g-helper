using NAudio.Wave;
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

    static bool init = false;

    public static void Init()
    {
        if (!init)
        {
            try
            {
                var result = nvmlInit_v2();
                init = result == NVML_SUCCESS;
                Logger.WriteLine($"NVML Init: {result}");
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
            if (nvmlDeviceGetHandleByIndex_v2(gpuIndex, out IntPtr device) != NVML_SUCCESS)
            {
                Shutdown();
                return null;
            }
            if (nvmlDeviceGetTemperature(device, NVML_TEMPERATURE_GPU, out uint temp) != NVML_SUCCESS)
            {
                Shutdown();
                return null;
            }
            return (int)temp;
        }
        catch
        {
            Shutdown();
            return null;
        }
    }

    public static float? GetGpuPower(uint gpuIndex = 0)
    {
        Init();
        try
        {
            if (nvmlDeviceGetHandleByIndex_v2(gpuIndex, out IntPtr device) != NVML_SUCCESS)
            {
                Shutdown();
                return null;
            }
            if (nvmlDeviceGetPowerUsage(device, out uint mW) != NVML_SUCCESS)
            {
                Shutdown();
                return null;
            }

            if (mW > 200_000f) return null;
            return mW / 1000f;
        }
        catch
        {
            Logger.WriteLine("NVML power read failed");
            Shutdown();
            return null;
        }
    }

    public static void Shutdown()
    {
        if (init)
        {
            try
            {
                var result = nvmlShutdown();
                Logger.WriteLine($"NVML Shutdown: {result}");
                init = false;
            }
            catch
            {
            }
        }
    }

}