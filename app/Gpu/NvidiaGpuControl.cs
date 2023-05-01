using NvAPIWrapper.GPU;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.Interfaces.GPU;

namespace GHelper.Gpu;

public class NvidiaGpuControl : IGpuControl
{
    private readonly PhysicalGPU? _internalGpu;

    public NvidiaGpuControl()
    {
        _internalGpu = GetInternalDiscreteGpu();
    }

    public bool IsValid => _internalGpu != null;

    public bool IsNvidia => IsValid;

    public int? GetCurrentTemperature()
    {
        if (!IsValid)
            return null;

        PhysicalGPU internalGpu = _internalGpu!;
        IThermalSensor? gpuSensor =
            GPUApi.GetThermalSettings(internalGpu.Handle).Sensors
            .FirstOrDefault(s => s.Target == ThermalSettingsTarget.GPU);

        return gpuSensor?.CurrentTemperature;
    }

    public void Dispose()
    {
    }

    private static PhysicalGPU? GetInternalDiscreteGpu()
    {
        try
        {
            return PhysicalGPU
                .GetPhysicalGPUs()
                .FirstOrDefault(gpu => gpu.SystemType == SystemType.Laptop);
        }
        catch
        {
            return null;
        }
    }


    public int? GetGpuUse()
    {
        if (!IsValid)
            return null;

        PhysicalGPU internalGpu = _internalGpu!;

        IUtilizationDomainInfo? gpuUsage = GPUApi.GetUsages(internalGpu.Handle).GPU;

        if (gpuUsage == null) 
            return null;

        return 
            (int)gpuUsage?.Percentage;

    }

}
