using NvAPIWrapper.GPU;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.GPU.Structures;
using NvAPIWrapper.Native.Interfaces.GPU;
using static NvAPIWrapper.Native.GPU.Structures.PerformanceStates20InfoV1;

namespace GHelper.Gpu;

public class NvidiaGpuControl : IGpuControl
{

    public const int MaxCoreOffset = 250;
    public const int MaxMemoryOffset = 250;

    public const int MinCoreOffset = -250;
    public const int MinMemoryOffset = -250;

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
        Logger.WriteLine(internalGpu.FullName);
        Logger.WriteLine(internalGpu.ArchitectInformation.ToString());

        IPerformanceStates20Info states = GPUApi.GetPerformanceStates20(internalGpu.Handle);

        //Logger.WriteLine("IPerformanceStates20Info type : " + states.GetType());

        core = states.Clocks[PerformanceStateId.P0_3DPerformance][0].FrequencyDeltaInkHz.DeltaValue / 1000;
        memory = states.Clocks[PerformanceStateId.P0_3DPerformance][1].FrequencyDeltaInkHz.DeltaValue / 1000;

        Logger.WriteLine($"GET GPU Clock offsets : {core}, {memory}");

    }

    public int SetClocksFromConfig()
    {
        int core = Program.config.getConfig("gpu_core");
        int memory = Program.config.getConfig("gpu_memory");
        int status = SetClocks(core, memory);
        return status;
    }

    public int SetClocks(int core, int memory)
    {

        if (core < MinCoreOffset || core > MaxCoreOffset) return 0;
        if (memory < MinMemoryOffset || memory > MaxMemoryOffset) return 0;

        PhysicalGPU internalGpu = _internalGpu!;

        var coreClock = new PerformanceStates20ClockEntryV1(PublicClockDomain.Graphics, new PerformanceStates20ParameterDelta(core * 1000));
        var memoryClock = new PerformanceStates20ClockEntryV1(PublicClockDomain.Memory, new PerformanceStates20ParameterDelta(memory * 1000));

        PerformanceStates20ClockEntryV1[] clocks = { coreClock , memoryClock};
        PerformanceStates20BaseVoltageEntryV1[] voltages = { };

        PerformanceState20[] performanceStates = { new PerformanceState20(PerformanceStateId.P0_3DPerformance, clocks, voltages) };

        var overclock = new PerformanceStates20InfoV1(performanceStates, 2, 0);

        try
        {
            GPUApi.SetPerformanceStates20(internalGpu.Handle, overclock);
            Logger.WriteLine($"SET GPU Clock offsets : {core}, {memory}");
        }
        catch (Exception ex)
        {
            Logger.WriteLine(ex.ToString());
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
        catch (Exception ex)
        {
            Logger.WriteLine(ex.ToString());
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
