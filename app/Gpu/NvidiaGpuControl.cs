using NvAPIWrapper.GPU;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.GPU.Structures;
using NvAPIWrapper.Native.Interfaces.GPU;

namespace GHelper.Gpu;

public class NvidiaGpuControl : IGpuControl
{

    public const int MaxCoreOffset = 300;
    public const int MaxMemoryOffset = 300;

    public const int MinCoreOffset = -300;
    public const int MinMemoryOffset = -300;

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



    public void GetClocks(out int core, out int memory)
    {
        PhysicalGPU internalGpu = _internalGpu!;
        PerformanceStates20InfoV3 states = (PerformanceStates20InfoV3)GPUApi.GetPerformanceStates20(internalGpu.Handle);
        core = states.Clocks[PerformanceStateId.P0_3DPerformance][0].FrequencyDeltaInkHz.DeltaValue / 1000;
        memory = states.Clocks[PerformanceStateId.P0_3DPerformance][1].FrequencyDeltaInkHz.DeltaValue / 1000;
    }


    public int SetClocks(int core, int memory)
    {

        if (core < MinCoreOffset || core > MaxCoreOffset) return 0;
        if (memory < MinMemoryOffset || memory > MaxMemoryOffset) return 0;

        PhysicalGPU internalGpu = _internalGpu!;
        PerformanceStates20InfoV3 states = (PerformanceStates20InfoV3)GPUApi.GetPerformanceStates20(internalGpu.Handle);

        states._NumberOfPerformanceStates = 1;
        states._NumberOfClocks = 2;
        states.PerformanceStates[0]._Clocks[0]._FrequencyDeltaInkHz = new PerformanceStates20ParameterDelta(core * 1000);
        states.PerformanceStates[0]._Clocks[1]._FrequencyDeltaInkHz = new PerformanceStates20ParameterDelta(memory * 1000);

        try
        {
            GPUApi.SetPerformanceStates20(internalGpu.Handle, states);
        }
        catch (Exception ex)
        {
            Logger.WriteLine(ex.Message);
            return -1;
        }

        return 1;
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

        return (int?)gpuUsage?.Percentage;

    }

}
