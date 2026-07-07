using System.Runtime.InteropServices;

namespace GHelperOverlay.Gpu;

// Minimal ADL2 wrapper — only what the overlay needs to read AMD GPU power and
// edge temperature. Full ADL surface lives in main g-helper's AmdAdl2.cs (~660
// lines); 95 % of it is unused exports we don't carry into the overlay.
internal static class AmdAdl
{
    private const string Dll = "atiadlxx.dll";
    private const int ADL_MAX_PATH = 256;
    private const int ADL_MAX_ADAPTERS = 40;
    private const int ADL_PMLOG_MAX_SENSORS = 256;
    private const int ADL_SUCCESS = 0;

    // ADLSensorType indices we read.
    private const int PMLOG_TEMPERATURE_EDGE = 8;
    private const int PMLOG_INFO_ACTIVITY_GFX = 19;
    private const int PMLOG_ASIC_POWER = 23;
    private const int PMLOG_GFX_POWER = 30;
    private const int PMLOG_CPU_POWER = 33;
    private const int PMLOG_BOARD_POWER = 73;

    [Flags]
    private enum AsicFamily { Discrete = 1 << 0, Integrated = 1 << 1 }

    [StructLayout(LayoutKind.Sequential)]
    private struct AdapterInfo
    {
        int Size;
        public int AdapterIndex;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ADL_MAX_PATH)] public string UDID;
        public int BusNumber, DriverNumber, FunctionNumber, VendorID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ADL_MAX_PATH)] public string AdapterName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ADL_MAX_PATH)] public string DisplayName;
        public int Present, Exist;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ADL_MAX_PATH)] public string DriverPath;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ADL_MAX_PATH)] public string DriverPathExt;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ADL_MAX_PATH)] public string PNPString;
        public int OSDisplayIndex;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct AdapterInfoArray
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ADL_MAX_ADAPTERS)]
        public AdapterInfo[] ADLAdapterInfo;
    }

    [StructLayout(LayoutKind.Sequential)] private struct Sensor { public int Supported, Value; }

    [StructLayout(LayoutKind.Sequential)]
    private struct PMLogDataOutput
    {
        int Size;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ADL_PMLOG_MAX_SENSORS)]
        public Sensor[] Sensors;
    }

    private delegate IntPtr AllocCallback(int size);
    private static readonly AllocCallback _alloc = Marshal.AllocCoTaskMem;

    [DllImport(Dll)] static extern int ADL2_Main_Control_Create(AllocCallback cb, int enumConnected, out IntPtr ctx);
    [DllImport(Dll)] static extern int ADL2_Adapter_NumberOfAdapters_Get(IntPtr ctx, out int n);
    [DllImport(Dll)] static extern int ADL2_Adapter_AdapterInfo_Get(IntPtr ctx, IntPtr info, int size);
    [DllImport(Dll)] static extern int ADL2_Adapter_ASICFamilyType_Get(IntPtr ctx, int adapter, out AsicFamily family, out int valids);
    [DllImport(Dll)] static extern int ADL2_New_QueryPMLogData_Get(IntPtr ctx, int adapter, out PMLogDataOutput log);
    [DllImport(Dll)] static extern int ADL2_Adapter_MemoryInfo2_Get(IntPtr ctx, int adapter, out MemoryInfo2 mem);
    [DllImport(Dll)] static extern int ADL2_Adapter_DedicatedVRAMUsage_Get(IntPtr ctx, int adapter, out int usedMB);

    [StructLayout(LayoutKind.Sequential)]
    private struct MemoryInfo2
    {
        public long iMemorySize;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)] public string strMemoryType;
        public long iMemoryBandwidth, iHyperMemorySize, iInvisibleMemorySize, iVisibleMemorySize;
    }

    private static long _totalVramMb; // static — cached on first successful query

    private static bool _tried, _ready;
    private static IntPtr _ctx;
    private static int _dGpuIndex = -1, _iGpuIndex = -1;

    private static void Init()
    {
        if (_tried) return;
        _tried = true;

        try { if (ADL2_Main_Control_Create(_alloc, 1, out _ctx) != ADL_SUCCESS) return; }
        catch (Exception ex) when (ex is DllNotFoundException or EntryPointNotFoundException)
        { Logger.WriteLine("ADL2 not present (no AMD driver)"); return; }

        if (ADL2_Adapter_NumberOfAdapters_Get(_ctx, out int count) != ADL_SUCCESS || count <= 0) return;

        var arr = new AdapterInfoArray();
        int sz = Marshal.SizeOf(arr);
        IntPtr buf = Marshal.AllocCoTaskMem(sz);
        try
        {
            Marshal.StructureToPtr(arr, buf, false);
            if (ADL2_Adapter_AdapterInfo_Get(_ctx, buf, sz) != ADL_SUCCESS) return;
            arr = (AdapterInfoArray)Marshal.PtrToStructure(buf, arr.GetType())!;
        }
        finally { Marshal.FreeCoTaskMem(buf); }

        foreach (var a in arr.ADLAdapterInfo)
        {
            if (a.Exist == 0 || a.Present == 0 || a.VendorID != 1002) continue;
            if (ADL2_Adapter_ASICFamilyType_Get(_ctx, a.AdapterIndex, out AsicFamily fam, out int valids) != ADL_SUCCESS) continue;
            fam = (AsicFamily)((int)fam & valids);
            if ((fam & AsicFamily.Discrete) != 0 && _dGpuIndex < 0) _dGpuIndex = a.AdapterIndex;
            else if ((fam & AsicFamily.Integrated) != 0 && _iGpuIndex < 0) _iGpuIndex = a.AdapterIndex;
        }

        _ready = _dGpuIndex >= 0 || _iGpuIndex >= 0;
        if (_ready) Logger.WriteLine($"ADL2 ready (dGPU={_dGpuIndex} iGPU={_iGpuIndex})");
    }

    private static int? ReadSensor(int adapter, int sensorIndex)
    {
        if (adapter < 0) return null;
        if (ADL2_New_QueryPMLogData_Get(_ctx, adapter, out PMLogDataOutput log) != ADL_SUCCESS) return null;
        var s = log.Sensors[sensorIndex];
        return (s.Supported != 0 && s.Value > 0) ? s.Value : null;
    }

    // CPU/APU power — Ally and other AMD iGPU-only configs where the "Apu Power"
    // PerfCounter instance is absent. Returns watts.
    public static float? ApuPower()
    {
        if (!PawnIO.CpuInfo.IsAMD) return null;
        Init(); if (!_ready) return null;
        return ReadSensor(_iGpuIndex, PMLOG_ASIC_POWER);
    }

    // Discrete AMD GPU power. Tries ASIC_POWER first, then GFX_POWER, then
    // BOARD_POWER — matches main g-helper's AmdGpuControl.GetGpuPower fallback
    // since not every AMD ASIC reports all three.
    public static float? DGpuPower()
    {
        if (!PawnIO.CpuInfo.IsAMD) return null;
        Init(); if (!_ready || _dGpuIndex < 0) return null;
        if (ADL2_New_QueryPMLogData_Get(_ctx, _dGpuIndex, out PMLogDataOutput log) != ADL_SUCCESS) return null;
        foreach (int idx in new[] { PMLOG_ASIC_POWER, PMLOG_GFX_POWER, PMLOG_BOARD_POWER })
        {
            var s = log.Sensors[idx];
            if (s.Supported != 0 && s.Value > 0) return s.Value;
        }
        return null;
    }

    // Discrete AMD GPU edge temperature in °C; null when no AMD dGPU is present.
    public static int? DGpuTemperature()
    {
        if (!PawnIO.CpuInfo.IsAMD) return null;
        Init(); if (!_ready) return null;
        return ReadSensor(_dGpuIndex, PMLOG_TEMPERATURE_EDGE);
    }

    // Discrete AMD GPU activity, percent.
    public static int? DGpuUsage()
    {
        if (!PawnIO.CpuInfo.IsAMD) return null;
        Init(); if (!_ready) return null;
        return ReadSensor(_dGpuIndex, PMLOG_INFO_ACTIVITY_GFX);
    }

    // Discrete AMD GPU VRAM (used, total) in MB; null when no AMD dGPU.
    public static (long usedMb, long totalMb)? DGpuMemoryInfo()
    {
        if (!PawnIO.CpuInfo.IsAMD) return null;
        Init(); if (!_ready || _dGpuIndex < 0) return null;

        if (_totalVramMb <= 0)
        {
            if (ADL2_Adapter_MemoryInfo2_Get(_ctx, _dGpuIndex, out MemoryInfo2 mem) != ADL_SUCCESS) return null;
            _totalVramMb = mem.iMemorySize / (1024 * 1024);
            if (_totalVramMb <= 0) return null;
        }

        if (ADL2_Adapter_DedicatedVRAMUsage_Get(_ctx, _dGpuIndex, out int usedMb) != ADL_SUCCESS) return null;
        return (usedMb, _totalVramMb);
    }

    // iGPU (APU) edge temperature — GPU temp on AMD iGPU-only machines.
    public static int? IGpuTemperature()
    {
        if (!PawnIO.CpuInfo.IsAMD) return null;
        Init(); if (!_ready) return null;
        return ReadSensor(_iGpuIndex, PMLOG_TEMPERATURE_EDGE);
    }

    // iGPU (APU) graphics activity, percent.
    public static int? IGpuUsage()
    {
        if (!PawnIO.CpuInfo.IsAMD) return null;
        Init(); if (!_ready) return null;
        return ReadSensor(_iGpuIndex, PMLOG_INFO_ACTIVITY_GFX);
    }

    // One PMLog query for the CPU/iGPU power split on AMD iGPU machines. Values are
    // raw (not >0 filtered) so the caller can mirror main g-helper's split arithmetic.
    public static (int? gfx, int? cpu, int? asic) IGpuPowerSplit()
    {
        if (!PawnIO.CpuInfo.IsAMD) return default;
        Init(); if (!_ready || _iGpuIndex < 0) return default;
        if (ADL2_New_QueryPMLogData_Get(_ctx, _iGpuIndex, out PMLogDataOutput log) != ADL_SUCCESS) return default;
        return (RawSensor(log, PMLOG_GFX_POWER), RawSensor(log, PMLOG_CPU_POWER), RawSensor(log, PMLOG_ASIC_POWER));
    }

    private static int? RawSensor(PMLogDataOutput log, int sensorIndex)
    {
        var s = log.Sensors[sensorIndex];
        return s.Supported != 0 ? s.Value : (int?)null;
    }

    private static long _totalIGpuVramMb;

    // iGPU dedicated VRAM (used, total) in MB — AMD iGPU-only machines without a dGPU.
    public static (long usedMb, long totalMb)? IGpuMemoryInfo()
    {
        if (!PawnIO.CpuInfo.IsAMD) return null;
        Init(); if (!_ready || _iGpuIndex < 0) return null;

        if (_totalIGpuVramMb <= 0)
        {
            if (ADL2_Adapter_MemoryInfo2_Get(_ctx, _iGpuIndex, out MemoryInfo2 mem) != ADL_SUCCESS) return null;
            _totalIGpuVramMb = mem.iMemorySize / (1024 * 1024);
            if (_totalIGpuVramMb <= 0) return null;
        }

        if (ADL2_Adapter_DedicatedVRAMUsage_Get(_ctx, _iGpuIndex, out int usedMb) != ADL_SUCCESS) return null;
        return (usedMb, _totalIGpuVramMb);
    }
}
