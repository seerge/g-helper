using System.Runtime.InteropServices;

namespace GHelper.Gpu.NVidia;

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
