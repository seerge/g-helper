using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GHelper.Gpu.AMD;

internal static class WindowsGpuSensor
{
    private const int AdapterPerfData = 62;
    private const int CounterRefreshMs = 5000;

    private static readonly object CounterLock = new();
    private static readonly object MemoryLock = new();
    private static readonly Dictionary<string, PerformanceCounter> UsageCounters = new(StringComparer.OrdinalIgnoreCase);
    private static string? _luidToken;
    private static long _lastCounterRefresh = -CounterRefreshMs;
    private static long _temperatureTick = -500;
    private static int? _temperature;
    private static long _usageTick = -500;
    private static int? _usage;
    private static PerformanceCounter? _dedicatedMemoryCounter;
    private static long _vramBudgetMb;

    [StructLayout(LayoutKind.Sequential)]
    private struct Luid
    {
        public uint LowPart;
        public int HighPart;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct OpenAdapterFromGdiDisplayName
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DeviceName;
        public uint AdapterHandle;
        public Luid AdapterLuid;
        public uint VidPnSourceId;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct QueryAdapterInfo
    {
        public uint AdapterHandle;
        public int Type;
        public nint PrivateDriverData;
        public uint PrivateDriverDataSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct AdapterPerformanceData
    {
        public uint PhysicalAdapterIndex;
        public ulong MemoryFrequency;
        public ulong MaxMemoryFrequency;
        public ulong MaxMemoryFrequencyOc;
        public ulong MemoryBandwidth;
        public ulong PcieBandwidth;
        public uint FanRpm;
        public uint Power;
        public uint Temperature;
        public byte PowerStateOverride;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct CloseAdapter
    {
        public uint AdapterHandle;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct QueryVideoMemoryInfo
    {
        public nint ProcessHandle;
        public uint AdapterHandle;
        public int MemorySegmentGroup;
        public ulong Budget;
        public ulong CurrentUsage;
        public ulong CurrentReservation;
        public ulong AvailableForReservation;
        public uint PhysicalAdapterIndex;
    }

    [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
    private static extern int D3DKMTOpenAdapterFromGdiDisplayName(ref OpenAdapterFromGdiDisplayName data);

    [DllImport("gdi32.dll")]
    private static extern int D3DKMTQueryAdapterInfo(ref QueryAdapterInfo data);

    [DllImport("gdi32.dll")]
    private static extern int D3DKMTCloseAdapter(ref CloseAdapter data);

    [DllImport("gdi32.dll")]
    private static extern int D3DKMTQueryVideoMemoryInfo(ref QueryVideoMemoryInfo data);

    private static string LuidToken(Luid luid)
        => $"luid_0x{(uint)luid.HighPart:x8}_0x{luid.LowPart:x8}";

    private static bool TryQueryPerformanceData(out AdapterPerformanceData performanceData, out Luid adapterLuid)
    {
        performanceData = default;
        adapterLuid = default;

        string? deviceName = Screen.PrimaryScreen?.DeviceName;
        if (string.IsNullOrEmpty(deviceName)) return false;

        OpenAdapterFromGdiDisplayName open = new() { DeviceName = deviceName };
        if (D3DKMTOpenAdapterFromGdiDisplayName(ref open) != 0 || open.AdapterHandle == 0) return false;

        nint buffer = Marshal.AllocHGlobal(Marshal.SizeOf<AdapterPerformanceData>());
        try
        {
            Marshal.StructureToPtr(performanceData, buffer, false);
            QueryAdapterInfo query = new()
            {
                AdapterHandle = open.AdapterHandle,
                Type = AdapterPerfData,
                PrivateDriverData = buffer,
                PrivateDriverDataSize = (uint)Marshal.SizeOf<AdapterPerformanceData>()
            };

            if (D3DKMTQueryAdapterInfo(ref query) != 0) return false;
            performanceData = Marshal.PtrToStructure<AdapterPerformanceData>(buffer);
            adapterLuid = open.AdapterLuid;
            return true;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
            CloseAdapter close = new() { AdapterHandle = open.AdapterHandle };
            D3DKMTCloseAdapter(ref close);
        }
    }

    public static int? GetTemperature()
    {
        try
        {
            long now = Environment.TickCount64;
            if (now - _temperatureTick < 500) return _temperature;

            if (!TryQueryPerformanceData(out AdapterPerformanceData data, out _)) return null;
            _temperature = data.Temperature is > 0 and < 1250
                ? (int)Math.Round(data.Temperature / 10.0)
                : null;
            _temperatureTick = now;
            return _temperature;
        }
        catch
        {
            return null;
        }
    }

    public static int? GetUsage()
    {
        try
        {
            lock (CounterLock)
            {
                long now = Environment.TickCount64;
                if (now - _usageTick < 500) return _usage;

                if (_luidToken is null)
                {
                    if (!TryQueryPerformanceData(out _, out Luid luid)) return null;
                    _luidToken = LuidToken(luid);
                }

                RefreshUsageCounters();
                Dictionary<string, float> engineUsage = new(StringComparer.OrdinalIgnoreCase);

                foreach ((string instance, PerformanceCounter counter) in UsageCounters)
                {
                    int engineStart = instance.IndexOf("_phys_", StringComparison.OrdinalIgnoreCase);
                    int engineEnd = instance.IndexOf("_engtype_", StringComparison.OrdinalIgnoreCase);
                    if (engineStart < 0 || engineEnd <= engineStart) continue;

                    float value;
                    try { value = counter.NextValue(); }
                    catch { continue; }

                    string engine = instance[engineStart..engineEnd];
                    engineUsage[engine] = engineUsage.GetValueOrDefault(engine) + value;
                }

                _usage = engineUsage.Count == 0
                    ? 0
                    : Math.Clamp((int)Math.Round(engineUsage.Values.Max()), 0, 100);
                _usageTick = now;
                return _usage;
            }
        }
        catch
        {
            return null;
        }
    }

    public static (long usedMb, long totalMb)? GetVramInfo()
    {
        try
        {
            lock (MemoryLock)
            {
                if (_vramBudgetMb <= 0 || _luidToken is null)
                {
                    string? deviceName = Screen.PrimaryScreen?.DeviceName;
                    if (string.IsNullOrEmpty(deviceName)) return null;

                    OpenAdapterFromGdiDisplayName open = new() { DeviceName = deviceName };
                    if (D3DKMTOpenAdapterFromGdiDisplayName(ref open) != 0 || open.AdapterHandle == 0) return null;

                    try
                    {
                        QueryVideoMemoryInfo memory = new()
                        {
                            AdapterHandle = open.AdapterHandle,
                            MemorySegmentGroup = 0,
                            PhysicalAdapterIndex = 0
                        };
                        if (D3DKMTQueryVideoMemoryInfo(ref memory) != 0 || memory.Budget == 0) return null;

                        _vramBudgetMb = (long)(memory.Budget / (1024 * 1024));
                        _luidToken = LuidToken(open.AdapterLuid);
                    }
                    finally
                    {
                        CloseAdapter close = new() { AdapterHandle = open.AdapterHandle };
                        D3DKMTCloseAdapter(ref close);
                    }
                }

                int? adlxUsedMb = AdlxGpuSensor.GetMetrics().vramUsedMb;
                if (adlxUsedMb is null && _dedicatedMemoryCounter is null)
                {
                    PerformanceCounterCategory category = new("GPU Adapter Memory");
                    string? instance = category.GetInstanceNames()
                        .FirstOrDefault(name => name.Contains(_luidToken, StringComparison.OrdinalIgnoreCase));
                    if (instance is null) return null;

                    _dedicatedMemoryCounter = new PerformanceCounter("GPU Adapter Memory", "Dedicated Usage", instance, true);
                }

                long usedMb = adlxUsedMb ?? (long)Math.Round(_dedicatedMemoryCounter!.NextValue() / (1024 * 1024));
                return (usedMb, _vramBudgetMb);
            }
        }
        catch
        {
            _dedicatedMemoryCounter?.Dispose();
            _dedicatedMemoryCounter = null;
            return null;
        }
    }

    private static void RefreshUsageCounters()
    {
        long now = Environment.TickCount64;
        if (now - _lastCounterRefresh < CounterRefreshMs) return;
        _lastCounterRefresh = now;

        PerformanceCounterCategory category = new("GPU Engine");
        HashSet<string> activeInstances = category.GetInstanceNames()
            .Where(instance => instance.Contains(_luidToken!, StringComparison.OrdinalIgnoreCase))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (string instance in activeInstances)
        {
            if (!UsageCounters.ContainsKey(instance))
                UsageCounters[instance] = new PerformanceCounter("GPU Engine", "Utilization Percentage", instance, true);
        }

        foreach (string instance in UsageCounters.Keys.Except(activeInstances, StringComparer.OrdinalIgnoreCase).ToArray())
        {
            UsageCounters[instance].Dispose();
            UsageCounters.Remove(instance);
        }
    }
}
