using GHelper.Helpers;
using NvAPIWrapper.GPU;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.GPU.Structures;
using NvAPIWrapper.Native.Interfaces.GPU;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using static NvAPIWrapper.Native.GPU.Structures.PerformanceStates20InfoV1;

namespace GHelper.Gpu.NVidia;

public class NvidiaGpuControl : IGpuControl
{
    public static int MaxCoreOffset = AppConfig.Get("max_gpu_core", 250);
    public static int MaxMemoryOffset = AppConfig.Get("max_gpu_memory", 500);

    public static int MinCoreOffset = AppConfig.Get("min_gpu_core", -250);
    public static int MinMemoryOffset = AppConfig.Get("min_gpu_memory", -500);

    public static int MinClockLimit = AppConfig.Get("min_gpu_clock", 400);
    public const int MaxClockLimit = 3000;

    private static PhysicalGPU? _internalGpu;

    public NvidiaGpuControl()
    {
        _internalGpu = GetInternalDiscreteGpu();
        if (IsValid)
        {
            if (FullName.Contains("5080") || FullName.Contains("5090"))
            {
                MaxCoreOffset = AppConfig.Get("max_gpu_core", 400);
                MaxMemoryOffset = AppConfig.Get("max_gpu_memory", 1000);
                Logger.WriteLine($"NVIDIA GPU: {FullName} ({MaxCoreOffset},{MaxMemoryOffset})");
            }
            if (FullName.Contains("5070 Ti") || FullName.Contains("4080") || FullName.Contains("4090"))
            {
                MaxCoreOffset = AppConfig.Get("max_gpu_core", 300);
                Logger.WriteLine($"NVIDIA GPU: {FullName} ({MaxCoreOffset},{MaxMemoryOffset})");
            }
        }
    }

    public bool IsValid => _internalGpu != null;

    public bool IsNvidia => IsValid;

    public string FullName => _internalGpu!.FullName;

    public int? GetCurrentTemperatureForced()
    {
        if (!IsValid) return null;
        PhysicalGPU internalGpu = _internalGpu!;

        try
        {
            var perfState = GPUApi.GetCurrentPerformanceState(internalGpu.Handle);
            Logger.WriteLine($"GPU Power state {perfState}");
        }
        catch (Exception ex)
        {
            Logger.WriteLine($"GPU Power state: {ex.Message}");
        }

        IThermalSensor? gpuSensor =
            GPUApi.GetThermalSettings(internalGpu.Handle).Sensors
            .FirstOrDefault(s => s.Target == ThermalSettingsTarget.GPU);

        Logger.WriteLine($"GPU Temp: {gpuSensor?.CurrentTemperature}");
        return gpuSensor?.CurrentTemperature;
    }

    public int? GetCurrentTemperature()
    {
        return D3DKMTHelper.GetGpuTemperature();
    }

    public void Dispose()
    {
    }

    private static readonly HashSet<string> _systemProcessNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "dwm", "csrss", "winlogon", "services", "lsass", "smss", "wininit",
        "svchost", "fontdrvhost", "igfxem", "igfxhk", "igfxext",
        "nvcontainer", "nvdisplay.container", "nvsettings", "nvspcaps64",
        "nvsphelper64", "nvwmi64", "nvcplui", "atieclxx", "atiesrxx",
        "explorer", "taskhostw", "sihost", "runtimebroker", "shellexperiencehost",
        "searchhost", "startmenuexperiencehost", "textinputhost",
        "applicationframehost", "systemsettings", "dllhost", "conhost",
        "audiodg", "ctfloader", "spoolsv", "wlanext", "msdtc",
    };

    public void KillGPUApps()
    {
        if (!IsValid) return;
        PhysicalGPU internalGpu = _internalGpu!;

        int currentPid = Process.GetCurrentProcess().Id;

        try
        {
            Process[] processes = internalGpu.GetActiveApplications();
            foreach (Process process in processes)
                try
                {
                    if (process.Id == currentPid) continue;
                    if (process.SessionId == 0) continue;
                    if (_systemProcessNames.Contains(process.ProcessName)) continue;

                    Logger.WriteLine("Kill:" + process.ProcessName);
                    ProcessHelper.KillByProcess(process);
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(ex.Message);
                }
        }
        catch (Exception ex)
        {
            Logger.WriteLine(ex.Message);
        }

        //GeneralApi.RestartDisplayDriver();
    }


    public bool GetClocks(out int core, out int memory)
    {
        PhysicalGPU internalGpu = _internalGpu!;

        //Logger.WriteLine(internalGpu.FullName);
        //Logger.WriteLine(internalGpu.ArchitectInformation.ToString());

        try
        {
            var temp = GetCurrentTemperatureForced();

            IPerformanceStates20Info states = GPUApi.GetPerformanceStates20(internalGpu.Handle);
            core = states.Clocks[PerformanceStateId.P0_3DPerformance][0].FrequencyDeltaInkHz.DeltaValue / 1000;
            memory = states.Clocks[PerformanceStateId.P0_3DPerformance][1].FrequencyDeltaInkHz.DeltaValue / 1000;
            Logger.WriteLine($"GET GPU CLOCKS: {core}, {memory}");

            foreach (var delta in states.Voltages[PerformanceStateId.P0_3DPerformance])
            {
                Logger.WriteLine("GPU VOLT:" + delta.IsEditable + " - " + delta.ValueDeltaInMicroVolt.DeltaValue);
            }

            return true;

        }
        catch (Exception ex)
        {
            Logger.WriteLine("GET GPU CLOCKS:" + ex.Message);
            core = memory = 0;
            return false;
        }

    }


    private static bool RunPowershellCommand(string script)
    {
        try
        {
            ProcessHelper.RunCMD("powershell", script);
            return true;
        }
        catch (Exception ex)
        {
            Logger.WriteLine(ex.ToString());
            return false;
        }

    }

    public int GetMaxGPUCLock()
    {
        PhysicalGPU internalGpu = _internalGpu!;
        try
        {
            PrivateClockBoostLockV2 data = GPUApi.GetClockBoostLock(internalGpu.Handle);
            int limit = (int)data.ClockBoostLocks[0].VoltageInMicroV / 1000;
            Logger.WriteLine("GET CLOCK LIMIT: " + limit);
            return limit;
        }
        catch (Exception ex)
        {
            Logger.WriteLine("GET CLOCK LIMIT: " + ex.Message);
            return -1;

        }
    }


    public int SetMaxGPUClock(int clock)
    {

        if (clock < MinClockLimit || clock >= MaxClockLimit) clock = 0;

        int _clockLimit = GetMaxGPUCLock();

        if (_clockLimit < 0 && clock == 0) return 0;

        if (_clockLimit != clock)
        {
            if (clock > 0) RunPowershellCommand($"nvidia-smi -lgc 0,{clock}");
            else RunPowershellCommand($"nvidia-smi -rgc");
            return 1;
        }
        else
        {
            return 0;
        }


    }

    public static bool IsContainerRestartNeeded()
    {
        var logPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            @"NVIDIA Corporation\NVIDIA App\NvContainer\NvContainerLocalSystem.log"
        );

        using var fs = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        fs.Seek(-Math.Min(8192, fs.Length), SeekOrigin.End);
        var tail = new StreamReader(fs).ReadToEnd();

        var match = Regex.Match(tail,
            @"NvcPluginLoadStats for 'NvPluginWatchdog'.*?InitializeProcTime = (\d+) us",
            RegexOptions.Singleline);

        return match.Success && match.Groups[1].Value == "0";
    }

    public static void RestartNVService(bool light = false)
    {
        if (!ProcessHelper.IsUserAdministrator()) return;
        if (!light) RunPowershellCommand(@"Restart-Service -Name 'NVDisplay.ContainerLocalSystem' -Force");
        RunPowershellCommand(@"Restart-Service -Name 'NvContainerLocalSystem' -Force");
    }

    public static void StopNVService()
    {
        if (!ProcessHelper.IsUserAdministrator()) return;
        RunPowershellCommand(@"Stop-Service -Name 'NvContainerLocalSystem' -Force");
        RunPowershellCommand(@"Stop-Service -Name 'NVDisplay.ContainerLocalSystem' -Force");
    }

    public int SetClocks(int core, int memory)
    {

        if (core < MinCoreOffset || core > MaxCoreOffset) return 0;
        if (memory < MinMemoryOffset || memory > MaxMemoryOffset) return 0;

        GetClocks(out int currentCore, out int currentMemory);

        // Nothing to set
        if (Math.Abs(core - currentCore) < 5 && Math.Abs(memory - currentMemory) < 5) return 0;

        PhysicalGPU internalGpu = _internalGpu!;

        var coreClock = new PerformanceStates20ClockEntryV1(PublicClockDomain.Graphics, new PerformanceStates20ParameterDelta(core * 1000));
        var memoryClock = new PerformanceStates20ClockEntryV1(PublicClockDomain.Memory, new PerformanceStates20ParameterDelta(memory * 1000));
        //var voltageEntry = new PerformanceStates20BaseVoltageEntryV1(PerformanceVoltageDomain.Core, new PerformanceStates20ParameterDelta(voltage));

        PerformanceStates20ClockEntryV1[] clocks = { coreClock, memoryClock };
        PerformanceStates20BaseVoltageEntryV1[] voltages = { };

        PerformanceState20[] performanceStates = { new PerformanceState20(PerformanceStateId.P0_3DPerformance, clocks, voltages) };

        var overclock = new PerformanceStates20InfoV1(performanceStates, 2, 0);

        try
        {
            Logger.WriteLine($"SET GPU CLOCKS: {core}, {memory}");
            GPUApi.SetPerformanceStates20(internalGpu.Handle, overclock);
        }
        catch (Exception ex)
        {
            Logger.WriteLine("SET GPU CLOCKS: " + ex.Message);
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
            Debug.WriteLine(ex.Message);
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

internal static class D3DKMTHelper
{
    [StructLayout(LayoutKind.Sequential)]
    struct LUID { public uint LowPart; public int HighPart; }

    [StructLayout(LayoutKind.Sequential)]
    struct D3DKMT_ADAPTERINFO
    {
        public uint hAdapter;
        public LUID AdapterLuid;
        public uint NumOfSources;
        [MarshalAs(UnmanagedType.Bool)] public bool bPresentMoveRegionsPreferred;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct D3DKMT_ENUMADAPTERS2
    {
        public uint NumAdapters;
        public IntPtr pAdapters;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct D3DKMT_OPENADAPTERFROMLUID
    {
        public LUID AdapterLuid;
        public uint hAdapter;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct D3DKMT_QUERYADAPTERINFO
    {
        public uint hAdapter;
        public int Type;
        public IntPtr pPrivateDriverData;
        public uint PrivateDriverDataSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct D3DKMT_CLOSEADAPTER { public uint hAdapter; }

    // Matches d3dkmthk.h exactly: UINT32 + 4-byte explicit pad + 5×ULONGLONG + 3×ULONG + UCHAR + 3-byte pad = 72 bytes
    [StructLayout(LayoutKind.Sequential)]
    struct D3DKMT_ADAPTER_PERFDATA
    {
        public uint PhysicalAdapterIndex;
        public uint _pad;              // explicit padding to align next field to 8 bytes
        public ulong MemoryFrequency;
        public ulong MaxMemoryFrequency;
        public ulong MaxMemoryFrequencyOC;
        public ulong MemoryBandwidth;
        public ulong PCIEBandwidth;
        public uint FanRPM;
        public uint Power;           // tenths of a percentage
        public uint Temperature;     // deci-Celsius (divide by 10)
        public byte PowerStateOverride;
    }

    const int KMTQAITYPE_ADAPTERPERFDATA = 62;

    [DllImport("gdi32.dll")] static extern int D3DKMTEnumAdapters2(ref D3DKMT_ENUMADAPTERS2 pData);
    [DllImport("gdi32.dll")] static extern int D3DKMTOpenAdapterFromLuid(ref D3DKMT_OPENADAPTERFROMLUID pData);
    [DllImport("gdi32.dll")] static extern int D3DKMTQueryAdapterInfo(ref D3DKMT_QUERYADAPTERINFO pData);
    [DllImport("gdi32.dll")] static extern int D3DKMTCloseAdapter(ref D3DKMT_CLOSEADAPTER pData);

    private static LUID? _gpuLuid;
    private static bool _luidSearched;

    private static bool EnsureLuid()
    {
        if (_luidSearched) return _gpuLuid.HasValue;
        _luidSearched = true;

        var enumData = new D3DKMT_ENUMADAPTERS2 { NumAdapters = 0, pAdapters = IntPtr.Zero };
        if (D3DKMTEnumAdapters2(ref enumData) != 0 || enumData.NumAdapters == 0) return false;

        int infoSize = Marshal.SizeOf<D3DKMT_ADAPTERINFO>();
        int perfSize = Marshal.SizeOf<D3DKMT_ADAPTER_PERFDATA>();
        Logger.WriteLine($"D3DKMT: struct size={perfSize} numAdapters={enumData.NumAdapters}");

        IntPtr buf = Marshal.AllocHGlobal(infoSize * (int)enumData.NumAdapters);
        try
        {
            enumData.pAdapters = buf;
            if (D3DKMTEnumAdapters2(ref enumData) != 0) return false;

            for (int i = 0; i < enumData.NumAdapters; i++)
            {
                var info = Marshal.PtrToStructure<D3DKMT_ADAPTERINFO>(buf + i * infoSize);
                var openData = new D3DKMT_OPENADAPTERFROMLUID { AdapterLuid = info.AdapterLuid };
                if (D3DKMTOpenAdapterFromLuid(ref openData) != 0) continue;

                IntPtr perfPtr = Marshal.AllocHGlobal(perfSize);
                try
                {
                    var perfData = new D3DKMT_ADAPTER_PERFDATA { PhysicalAdapterIndex = 0 };
                    Marshal.StructureToPtr(perfData, perfPtr, false);
                    var q = new D3DKMT_QUERYADAPTERINFO
                    {
                        hAdapter = openData.hAdapter,
                        Type = KMTQAITYPE_ADAPTERPERFDATA,
                        pPrivateDriverData = perfPtr,
                        PrivateDriverDataSize = (uint)perfSize
                    };
                    int hr = D3DKMTQueryAdapterInfo(ref q);
                    perfData = Marshal.PtrToStructure<D3DKMT_ADAPTER_PERFDATA>(perfPtr);
                    Logger.WriteLine($"D3DKMT adapter[{i}] hr={hr} temp={perfData.Temperature} power={perfData.Power}");

                    if (hr == 0 && perfData.Temperature > 0)
                    {
                        _gpuLuid = info.AdapterLuid;
                        return true;
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(perfPtr);
                    var close = new D3DKMT_CLOSEADAPTER { hAdapter = openData.hAdapter };
                    D3DKMTCloseAdapter(ref close);
                }
            }
        }
        finally { Marshal.FreeHGlobal(buf); }

        return false;
    }

    public static void Reset() => _luidSearched = false;

    public static int? GetGpuTemperature()
    {
        if (!EnsureLuid()) return null;

        var openData = new D3DKMT_OPENADAPTERFROMLUID { AdapterLuid = _gpuLuid!.Value };
        if (D3DKMTOpenAdapterFromLuid(ref openData) != 0) return null;

        int perfSize = Marshal.SizeOf<D3DKMT_ADAPTER_PERFDATA>();
        IntPtr perfPtr = Marshal.AllocHGlobal(perfSize);
        try
        {
            var perfData = new D3DKMT_ADAPTER_PERFDATA { PhysicalAdapterIndex = 0 };
            Marshal.StructureToPtr(perfData, perfPtr, false);
            var q = new D3DKMT_QUERYADAPTERINFO
            {
                hAdapter = openData.hAdapter,
                Type = KMTQAITYPE_ADAPTERPERFDATA,
                pPrivateDriverData = perfPtr,
                PrivateDriverDataSize = (uint)perfSize
            };
            if (D3DKMTQueryAdapterInfo(ref q) != 0) return null;
            perfData = Marshal.PtrToStructure<D3DKMT_ADAPTER_PERFDATA>(perfPtr);
            return perfData.Temperature > 0 ? (int)(perfData.Temperature / 10) : null;
        }
        finally
        {
            Marshal.FreeHGlobal(perfPtr);
            var close = new D3DKMT_CLOSEADAPTER { hAdapter = openData.hAdapter };
            D3DKMTCloseAdapter(ref close);
        }
    }
}
