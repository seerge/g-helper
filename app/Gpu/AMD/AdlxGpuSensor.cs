using System.Runtime.InteropServices;

namespace GHelper.Gpu.AMD;

internal static class AdlxGpuSensor
{
    private const ulong AdlxVersion = ((ulong)1 << 48) | ((ulong)5 << 32) | 124;
    private const int CacheMs = 500;
    // Function indexes from the backward-compatible ADLX 1.5 C interface vtables.
    private const int ReleaseIndex = 1;
    private const int SystemGetGpusIndex = 1;
    private const int SystemGetPerformanceIndex = 9;
    private const int ListBeginIndex = 5;
    private const int GpuListAtIndex = 11;
    private const int PerformanceGetCurrentGpuIndex = 18;
    private const int MetricsUsageIndex = 4;
    private const int MetricsTemperatureIndex = 7;
    private const int MetricsPowerIndex = 9;
    private const int MetricsTotalBoardPowerIndex = 10;
    private const int MetricsVramIndex = 12;

    private static readonly object Sync = new();
    private static bool _initialized;
    private static bool _initializationAttempted;
    private static nint _system;
    private static nint _gpu;
    private static nint _performance;
    private static long _lastRead = -CacheMs;
    private static (int? temp, int? use, int? vramUsedMb, float? gpuPower, float? totalBoardPower) _metrics;

    [DllImport("amdadlx64.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int ADLXInitializeWithIncompatibleDriver(ulong version, out nint system);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int GetInterface(nint self, out nint result);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate uint GetUInt(nint self);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int GetListItem(nint self, uint index, out nint result);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int GetCurrentGpuMetrics(nint self, nint gpu, out nint metrics);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int GetDouble(nint self, out double value);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int GetInt(nint self, out int value);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int Release(nint self);

    private static T Vtable<T>(nint instance, int index) where T : Delegate
    {
        nint vtable = Marshal.ReadIntPtr(instance);
        return Marshal.GetDelegateForFunctionPointer<T>(Marshal.ReadIntPtr(vtable, index * nint.Size));
    }

    private static bool Initialize()
    {
        if (_initializationAttempted) return _initialized;
        _initializationAttempted = true;

        nint gpuList = nint.Zero;
        try
        {
            if (ADLXInitializeWithIncompatibleDriver(AdlxVersion, out _system) != 0 || _system == nint.Zero)
                return false;

            if (Vtable<GetInterface>(_system, SystemGetGpusIndex)(_system, out gpuList) != 0 || gpuList == nint.Zero)
                return false;

            uint begin = Vtable<GetUInt>(gpuList, ListBeginIndex)(gpuList);
            if (Vtable<GetListItem>(gpuList, GpuListAtIndex)(gpuList, begin, out _gpu) != 0 || _gpu == nint.Zero)
                return false;

            if (Vtable<GetInterface>(_system, SystemGetPerformanceIndex)(_system, out _performance) != 0 || _performance == nint.Zero)
                return false;

            return _initialized = true;
        }
        catch
        {
            return false;
        }
        finally
        {
            if (gpuList != nint.Zero)
                try { Vtable<Release>(gpuList, ReleaseIndex)(gpuList); } catch { }
        }
    }

    public static (int? temp, int? use, int? vramUsedMb, float? gpuPower, float? totalBoardPower) GetMetrics()
    {
        lock (Sync)
        {
            long now = Environment.TickCount64;
            if (now - _lastRead < CacheMs) return _metrics;
            if (!Initialize()) return default;

            nint current = nint.Zero;
            try
            {
                if (Vtable<GetCurrentGpuMetrics>(_performance, PerformanceGetCurrentGpuIndex)(_performance, _gpu, out current) != 0 || current == nint.Zero)
                    return default;

                int? usage = Vtable<GetDouble>(current, MetricsUsageIndex)(current, out double usageValue) == 0
                    ? Math.Clamp((int)Math.Round(usageValue), 0, 100)
                    : null;
                int? temperature = Vtable<GetDouble>(current, MetricsTemperatureIndex)(current, out double temperatureValue) == 0
                    && temperatureValue is > 0 and < 125
                    ? (int)Math.Round(temperatureValue)
                    : null;
                int? vram = Vtable<GetInt>(current, MetricsVramIndex)(current, out int vramValue) == 0 && vramValue >= 0
                    ? vramValue
                    : null;
                float? gpuPower = Vtable<GetDouble>(current, MetricsPowerIndex)(current, out double gpuPowerValue) == 0
                    && gpuPowerValue is > 0 and < 1000
                    ? (float)gpuPowerValue
                    : null;
                float? totalBoardPower = Vtable<GetDouble>(current, MetricsTotalBoardPowerIndex)(current, out double totalBoardPowerValue) == 0
                    && totalBoardPowerValue is > 0 and < 1000
                    ? (float)totalBoardPowerValue
                    : null;

                _metrics = (temperature, usage, vram, gpuPower, totalBoardPower);
                _lastRead = now;
                return _metrics;
            }
            catch
            {
                return default;
            }
            finally
            {
                if (current != nint.Zero)
                    try { Vtable<Release>(current, ReleaseIndex)(current); } catch { }
            }
        }
    }
}
