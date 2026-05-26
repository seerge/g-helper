using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace GHelperOverlay;

// GPU sensors via direct P/Invoke into nvapi64.dll, gated by a single
// availability check.
//
// NvAPI exposes a single entry point — nvapi_QueryInterface(id) — that returns
// a function pointer for the requested API. Each function has a stable ID
// baked into the NVIDIA driver, so resolving them by ID drops the
// NvAPIWrapper NuGet dependency without changing behaviour.
//
// Why a probe is needed: NVML powers the GPU up to service every query.
// Polling it on a 1 s timer means the dGPU never reaches D3, defeating Optimus.
// NvAPI's GetCurrentPstate succeeds when the GPU is powered and returns any
// error otherwise (NVAPI_GPU_NOT_POWERED = -220 when in low-power state),
// *without* waking it — so it's the cheap signal we use to decide whether to
// read any sensors at all.
public static class GpuSensors
{
    // NvAPI status codes
    private const int NVAPI_OK = 0;
    private const int NVAPI_GPU_NOT_POWERED = -220;

    public enum State { Active, Sleeping, Off }

    // NvAPI enums we care about
    private const int NV_SYSTEM_TYPE_LAPTOP = 1;
    private const int NVAPI_THERMAL_TARGET_GPU = 1;
    private const int NVAPI_THERMAL_TARGET_ALL = 15;

    // Function IDs — stable across driver versions (NvAPI ABI guarantee).
    private const uint ID_Initialize           = 0x0150E828;
    private const uint ID_EnumPhysicalGPUs     = 0xE5AC921F;
    private const uint ID_GetSystemType        = 0xBAAABFCC;
    private const uint ID_GetCurrentPstate     = 0x927DA4F6;
    private const uint ID_GetThermalSettings   = 0xE3640A56;
    private const uint ID_GetDynamicPstatesEx  = 0x60DED2ED;

    [DllImport("nvapi64.dll", EntryPoint = "nvapi_QueryInterface", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr nvapi_QueryInterface(uint id);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int Initialize_t();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int EnumPhysicalGPUs_t([Out] IntPtr[] handles, out uint count);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int GetSystemType_t(IntPtr gpu, out int systemType);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int GetCurrentPstate_t(IntPtr gpu, out int pstate);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int GetThermalSettings_t(IntPtr gpu, uint sensorIndex, ref NV_GPU_THERMAL_SETTINGS_V2 settings);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int GetDynamicPstates_t(IntPtr gpu, ref NV_GPU_DYNAMIC_PSTATES_INFO_EX info);

    [StructLayout(LayoutKind.Sequential)]
    private struct NV_GPU_THERMAL_SENSOR
    {
        public int controller;
        public int defaultMinTemp;
        public int defaultMaxTemp;
        public int currentTemp;
        public int target;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NV_GPU_THERMAL_SETTINGS_V2
    {
        public uint version;
        public uint count;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public NV_GPU_THERMAL_SENSOR[] sensor;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NV_GPU_UTILIZATION_DOMAIN_INFO
    {
        public uint bIsPresent;   // bit 0 = present
        public uint percentage;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NV_GPU_DYNAMIC_PSTATES_INFO_EX
    {
        public uint version;
        public uint flags;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public NV_GPU_UTILIZATION_DOMAIN_INFO[] utilization;
    }

    private static GetCurrentPstate_t? _getPstate;
    private static GetThermalSettings_t? _getThermal;
    private static GetDynamicPstates_t? _getDynPstates;
    private static IntPtr _gpu;

    private static int? _lastTemp;
    private static Task<int?>? _tempTask;

    static GpuSensors()
    {
        try { InitNvApi(); }
        catch (Exception ex) { Logger.WriteLine("NvAPI not available: " + ex.Message); }
    }

    private static void InitNvApi()
    {
        var init = Resolve<Initialize_t>(ID_Initialize);
        if (init is null || init() != NVAPI_OK)
        {
            Logger.WriteLine("NvAPI initialize failed");
            return;
        }

        var enumGpus = Resolve<EnumPhysicalGPUs_t>(ID_EnumPhysicalGPUs);
        if (enumGpus is null) return;

        IntPtr[] handles = new IntPtr[64];
        if (enumGpus(handles, out uint count) != NVAPI_OK || count == 0) return;

        var getSystemType = Resolve<GetSystemType_t>(ID_GetSystemType);
        for (int i = 0; i < count; i++)
        {
            if (getSystemType is not null
                && getSystemType(handles[i], out int sysType) == NVAPI_OK
                && sysType == NV_SYSTEM_TYPE_LAPTOP)
            {
                _gpu = handles[i];
                break;
            }
        }
        if (_gpu == IntPtr.Zero) return;

        _getPstate     = Resolve<GetCurrentPstate_t>(ID_GetCurrentPstate);
        _getThermal    = Resolve<GetThermalSettings_t>(ID_GetThermalSettings);
        _getDynPstates = Resolve<GetDynamicPstates_t>(ID_GetDynamicPstatesEx);
        Logger.WriteLine("NvAPI initialised, laptop dGPU handle=0x" + _gpu.ToInt64().ToString("X"));
    }

    private static T? Resolve<T>(uint id) where T : Delegate
    {
        try
        {
            IntPtr p = nvapi_QueryInterface(id);
            return p == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer<T>(p);
        }
        catch { return null; }
    }

    private static uint VersionOf<T>(int v) where T : struct =>
        (uint)(Marshal.SizeOf<T>() | (v << 16));

    // Active = GPU is on and responding (sensors readable).
    // Sleeping = GPU is in D3, handle is valid, will wake on demand (show 0W).
    // Off = GPU is firmware-disabled or absent (show "--").
    public static State GetState()
    {
        if (_getPstate is null || _gpu == IntPtr.Zero) return State.Off;
        int rc = _getPstate(_gpu, out _);
        if (rc == NVAPI_OK) return State.Active;
        if (rc == NVAPI_GPU_NOT_POWERED) return State.Sleeping;
        return State.Off;
    }

    public static int? GetTemperature()
    {
        var state = GetState();
        if (state == State.Off) return null;
        if (state == State.Sleeping) return _lastTemp;

        if (_tempTask?.IsCompleted ?? true)
        {
            _tempTask = Task.Run(() =>
            {
                int? t = ReadTemperatureSync();
                if (t.HasValue) _lastTemp = t;
                return t;
            });
        }
        _tempTask?.Wait(500);
        return _lastTemp;
    }

    private static int? ReadTemperatureSync()
    {
        if (_gpu == IntPtr.Zero || _getThermal is null) return null;
        try
        {
            var s = new NV_GPU_THERMAL_SETTINGS_V2
            {
                version = VersionOf<NV_GPU_THERMAL_SETTINGS_V2>(2),
                sensor = new NV_GPU_THERMAL_SENSOR[3],
            };
            if (_getThermal(_gpu, NVAPI_THERMAL_TARGET_ALL, ref s) != NVAPI_OK) return null;
            for (int i = 0; i < s.count && i < 3; i++)
                if (s.sensor[i].target == NVAPI_THERMAL_TARGET_GPU)
                    return s.sensor[i].currentTemp;
            return null;
        }
        catch { return null; }
    }

    public static float? GetPower()
    {
        var state = GetState();
        if (state == State.Off) { Nvml.Shutdown(); return null; }
        if (state == State.Sleeping) return 0f;
        return Nvml.GetPower() ?? 0f;
    }

    public static int? GetUsage()
    {
        if (_getDynPstates is null) return null;
        var state = GetState();
        if (state == State.Off) return null;
        if (state == State.Sleeping) return 0;
        try
        {
            var info = new NV_GPU_DYNAMIC_PSTATES_INFO_EX
            {
                version = VersionOf<NV_GPU_DYNAMIC_PSTATES_INFO_EX>(1),
                utilization = new NV_GPU_UTILIZATION_DOMAIN_INFO[8],
            };
            if (_getDynPstates(_gpu, ref info) != NVAPI_OK) return null;
            // utilization[0] is the GPU domain; [1]=FB, [2]=VID, [3]=BUS — unused.
            if ((info.utilization[0].bIsPresent & 1) != 0)
                return (int)info.utilization[0].percentage;
            return null;
        }
        catch { return null; }
    }

    public static void Shutdown() => Nvml.Shutdown();
}

// NVML — power only. The other sensors come from NvAPI because NvAPI exposes
// a non-waking state probe and NVML does not.
internal static class Nvml
{
    const string Dll = "nvml.dll";
    const int NVML_SUCCESS = 0;

    [DllImport(Dll)] static extern int nvmlInit_v2();
    [DllImport(Dll)] static extern int nvmlShutdown();
    [DllImport(Dll)] static extern int nvmlDeviceGetHandleByIndex_v2(uint index, out IntPtr device);
    [DllImport(Dll)] static extern int nvmlDeviceGetPowerUsage(IntPtr device, out uint powerMilliWatts);

    private static readonly object _lock = new();
    private static bool _init;

    private static bool Init()
    {
        if (_init) return true;
        try
        {
            if (nvmlInit_v2() != NVML_SUCCESS) return false;
            _init = true;
            Logger.WriteLine("NVML initialised");
            return true;
        }
        catch (Exception ex) { Logger.WriteLine("NVML init exception: " + ex.Message); return false; }
    }

    public static void Shutdown()
    {
        lock (_lock)
        {
            if (!_init) return;
            try { nvmlShutdown(); } catch { }
            _init = false;
            Logger.WriteLine("NVML shutdown");
        }
    }

    // Re-acquire device handle on every call. Caching it leaves a dangling
    // pointer after a firmware GPU toggle and faults inside nvmlDeviceGetPowerUsage.
    // Matches g-helper's NvmlHelper pattern, which isn't affected by toggles.
    [HandleProcessCorruptedStateExceptions, System.Security.SecurityCritical]
    public static float? GetPower()
    {
        lock (_lock)
        {
            if (!Init()) return null;
            try
            {
                if (nvmlDeviceGetHandleByIndex_v2(0, out IntPtr device) != NVML_SUCCESS) return null;
                if (nvmlDeviceGetPowerUsage(device, out uint mW) != NVML_SUCCESS) return null;
                if (mW > 200_000) return null;
                return mW / 1000f;
            }
            catch (Exception ex)
            {
                Logger.WriteLine("NVML GetPower exception: " + ex.GetType().Name);
                try { nvmlShutdown(); } catch { }
                _init = false;
                return null;
            }
        }
    }
}
