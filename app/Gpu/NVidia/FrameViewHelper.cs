using System.Runtime.InteropServices;

// NVIDIA FrameView SDK (installed with NVIDIA App) reports CPU power on Windows on ARM (RTX Spark N1X),
// where the Energy Meter counters have no channels. API: FrameViewSDK\SDK\Public_Release\FvSDK.h
public static unsafe class FrameViewHelper
{
    const string FvDll = @"NVIDIA Corporation\FrameViewSDK\bin\FvSDK_x64.dll";

    const int FV_SUCCESS = 0;
    const int FV_SDK_SESSION_IN_PROGRESS = 0x14;

    const uint eCPUPwr = 0xBB272534;

    [StructLayout(LayoutKind.Sequential)]
    struct Samples { public uint version; public uint type; public nuint mNumSamples; public void* mData; }

    // BaseMetric_v1 + CPUPwr_v1, E/P core power is only reported on Windows on ARM
    [StructLayout(LayoutKind.Sequential)]
    struct CPUPwr { public uint baseversion; public ulong mTimestamp; public uint version; public double mPkgPwr, mECorePwr, mPCorePwr; }

    static uint MakeVersion(int size, int ver) => (uint)size | ((uint)ver << 16);

    // The DLL only exports fv_QueryInterface. FvSDK.lib (C++ only) fills its function table with
    // fv_QueryInterface(slot) and calls through it, these are the slots it uses for each FvSDK_* function.
    enum Slot : uint
    {
        CreateSession = 0,
        DestroySession = 1,
        EnableMetrics = 2,
        SampleData = 3,
        ReadData = 4,
        StartSession = 5,
        StopSession = 6,
        Initialize = 7,  // FvSDK_Initialize passes the module handle and fv_QueryInterface
        Shutdown = 8,
    }

    private static readonly IntPtr[] _functions = new IntPtr[(int)Slot.Shutdown + 1];
    private static IntPtr Fn(Slot slot) => _functions[(int)slot];

    static int FvSDK_Initialize(IntPtr module, IntPtr queryInterface) => ((delegate* unmanaged[Cdecl]<IntPtr, IntPtr, int>)Fn(Slot.Initialize))(module, queryInterface);
    static int FvSDK_CreateSession(out IntPtr session) { IntPtr s; int rc = ((delegate* unmanaged[Cdecl]<IntPtr*, int>)Fn(Slot.CreateSession))(&s); session = s; return rc; }
    static int FvSDK_EnableMetrics(IntPtr session, uint* metrics, uint count) => ((delegate* unmanaged[Cdecl]<IntPtr, uint*, uint, int>)Fn(Slot.EnableMetrics))(session, metrics, count);
    static int FvSDK_StartSession(IntPtr session) => ((delegate* unmanaged[Cdecl]<IntPtr, int>)Fn(Slot.StartSession))(session);
    static int FvSDK_SampleData(IntPtr session) => ((delegate* unmanaged[Cdecl]<IntPtr, int, int>)Fn(Slot.SampleData))(session, 0);
    static int FvSDK_ReadData(IntPtr session, Samples* samples, uint count) => ((delegate* unmanaged[Cdecl]<IntPtr, Samples*, uint, int>)Fn(Slot.ReadData))(session, samples, count);
    static int FvSDK_StopSession(IntPtr session) => ((delegate* unmanaged[Cdecl]<IntPtr, int>)Fn(Slot.StopSession))(session);
    static int FvSDK_DestroySession(IntPtr session) => ((delegate* unmanaged[Cdecl]<IntPtr, int>)Fn(Slot.DestroySession))(session);
    static int FvSDK_Shutdown() => ((delegate* unmanaged[Cdecl]<int>)Fn(Slot.Shutdown))();

    private static readonly object _lock = new();
    private static bool _init, _failed;
    private static IntPtr _session;
    private static float? _lastPower;

    private static bool LoadLibrary()
    {
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), FvDll);
        if (!File.Exists(path)) return false;

        IntPtr module = NativeLibrary.Load(path);
        IntPtr queryInterface = NativeLibrary.GetExport(module, "fv_QueryInterface");
        var query = (delegate* unmanaged[Cdecl]<uint, IntPtr>)queryInterface;

        for (uint slot = 0; slot < _functions.Length; slot++)
            if ((_functions[slot] = query(slot)) == IntPtr.Zero) return false;

        return Check("Initialize", FvSDK_Initialize(module, queryInterface));
    }

    private static bool Check(string call, int rc)
    {
        if (rc != FV_SUCCESS) Logger.WriteLine($"FrameView {call}: {rc}");
        return rc == FV_SUCCESS;
    }

    private static bool Init()
    {
        if (_init) return true;
        if (_failed) return false;
        _failed = true;

        if (RuntimeInformation.OSArchitecture != Architecture.Arm64) return false;

        try
        {
            if (!LoadLibrary()) return false;
            if (!Check("CreateSession", FvSDK_CreateSession(out _session))) return false;

            uint metric = eCPUPwr;
            if (!Check("EnableMetrics", FvSDK_EnableMetrics(_session, &metric, 1))) return false;

            // Another client (e.g. NVIDIA Overlay) may already run a session, data is still delivered
            int rc = FvSDK_StartSession(_session);
            if (rc != FV_SDK_SESSION_IN_PROGRESS && !Check("StartSession", rc)) return false;

            Logger.WriteLine("CPU Power source: NVIDIA FrameView SDK");
            _init = true;
            _failed = false;
            return true;
        }
        catch (Exception ex)
        {
            Logger.WriteLine($"FrameView Init exception: {ex.Message}");
            return false;
        }
    }

    public static float? GetCPUPower()
    {
        lock (_lock)
        {
            if (!Init()) return null;
            try
            {
                FvSDK_SampleData(_session);

                // First call returns the number of samples since last SampleData, second call copies them
                var samples = new Samples { version = MakeVersion(sizeof(Samples), 1), type = eCPUPwr };
                FvSDK_ReadData(_session, &samples, 1);
                int count = (int)samples.mNumSamples;
                if (count <= 0 || count > 1000) return _lastPower;

                var data = new CPUPwr[count];
                for (int i = 0; i < count; i++)
                {
                    data[i].baseversion = MakeVersion(16, 1);
                    data[i].version = MakeVersion(sizeof(CPUPwr), 1);
                }

                fixed (CPUPwr* buffer = data)
                {
                    samples.mData = buffer;
                    if (FvSDK_ReadData(_session, &samples, 1) != FV_SUCCESS) return _lastPower;
                }

                _lastPower = (float)data.Average(sample => sample.mECorePwr + sample.mPCorePwr);
                return _lastPower;
            }
            catch
            {
                return _lastPower;
            }
        }
    }

    public static void Shutdown()
    {
        lock (_lock)
        {
            if (!_init) return;
            try
            {
                FvSDK_StopSession(_session);
                FvSDK_DestroySession(_session);
                FvSDK_Shutdown();
            }
            catch { }
            _init = false;
        }
    }
}
