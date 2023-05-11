using NvAPIWrapper.GPU;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.Delegates;
using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.GPU.Structures;
using NvAPIWrapper.Native.Interfaces.GPU;
using System;
using System.Diagnostics;
using System.Management;
using static NvAPIWrapper.Native.GPU.Structures.PerformanceStates20InfoV1;

namespace GHelper.Gpu;

public class NvidiaGpuControl : IGpuControl
{

    public const int MaxCoreOffset = 250;
    public const int MaxMemoryOffset = 250;

    public const int MinCoreOffset = -250;
    public const int MinMemoryOffset = -250;

    private static PhysicalGPU? _internalGpu;

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


    public int GetClocks(out int core, out int memory, out string gpu)
    {
        PhysicalGPU internalGpu = _internalGpu!;

        gpu = internalGpu.FullName;

        //Logger.WriteLine(internalGpu.FullName);
        //Logger.WriteLine(internalGpu.ArchitectInformation.ToString());

        try
        {
            IPerformanceStates20Info states = GPUApi.GetPerformanceStates20(internalGpu.Handle);
            core = states.Clocks[PerformanceStateId.P0_3DPerformance][0].FrequencyDeltaInkHz.DeltaValue / 1000;
            memory = states.Clocks[PerformanceStateId.P0_3DPerformance][1].FrequencyDeltaInkHz.DeltaValue / 1000;
            Logger.WriteLine($"GET GPU Clock offsets : {core}, {memory}");
            return 0;

        } catch (Exception ex)
        {
            Logger.WriteLine(ex.Message);
            core = memory = 0; 
            return -1;
        }

    }

    private static void RunCMD(string name, string args)
    {
        var cmd = new Process();
        cmd.StartInfo.UseShellExecute = false;
        cmd.StartInfo.CreateNoWindow = true;
        cmd.StartInfo.RedirectStandardOutput = true;
        cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        cmd.StartInfo.FileName = name;
        cmd.StartInfo.Arguments = args;
        cmd.Start();
        Logger.WriteLine(cmd.StandardOutput.ReadToEnd());
        cmd.WaitForExit();
    }


    public bool RestartGPU()
    {

        if (!IsValid) return false;

        try
        {
            PhysicalGPU internalGpu = _internalGpu!;
            var pnpDeviceId = internalGpu.BusInformation.PCIIdentifiers.ToString();
            Logger.WriteLine("Device ID:"+ pnpDeviceId);
            RunCMD("pnputil", $"/disable-device /deviceid \"{pnpDeviceId}\"");
            Thread.Sleep(3000);
            RunCMD("pnputil", $"/enable-device /deviceid \"{pnpDeviceId}\"");
            Thread.Sleep(2000);
            return true;
        }
        catch (Exception ex )
        {
            Logger.WriteLine(ex.ToString());
            return false;
        }
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

        PerformanceStates20ClockEntryV1[] clocks = { coreClock, memoryClock };
        PerformanceStates20BaseVoltageEntryV1[] voltages = { };

        PerformanceState20[] performanceStates = { new PerformanceState20(PerformanceStateId.P0_3DPerformance, clocks, voltages) };

        var overclock = new PerformanceStates20InfoV1(performanceStates, 2, 0);

        try
        {
            Logger.WriteLine($"SET GPU Clock : {core}, {memory}");
            GPUApi.SetPerformanceStates20(internalGpu.Handle, overclock);
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
        catch (Exception ex)
        {
            Logger.WriteLine(ex.Message);
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
