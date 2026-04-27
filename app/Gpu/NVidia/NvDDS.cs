using System.Runtime.InteropServices;

namespace GHelper.Gpu.NVidia;

public static class NvDDS
{
    private const int NVAPI_OK = 0;

    private const uint ID_NvAPI_Initialize        = 0x0150E828;
    private const uint ID_EnumPhysicalGPUs        = 0xE5AC921F;
    private const uint ID_GPU_GetAllDisplayIds    = 0x785210A2;
    private const uint ID_GetDDSState             = 0x6050BB05;
    private const uint ID_OpenDDSTray             = 0x5C3565D0;

    private const uint NV_GPU_DISPLAYIDS_VER3 = 0x30010;
    private const uint NV_DDS_STATE_VER1      = 0x10010;

    public enum DDSSwitchType { Invalid = 0, Auto = 1, Manual = 2 }
    public enum DDSMuxState   { Unknown = 0, IGpu = 1, DGpu = 2 }

    private static bool _initialized;
    private static IntPtr _enumGpusFn;
    private static IntPtr _getDisplayIdsFn;
    private static IntPtr _getDDSStateFn;
    private static IntPtr _openDDSTrayFn;

    private const uint OUTER_VERSION = 0x10038;
    private const uint INNER_VERSION = 0x1002C;

    [StructLayout(LayoutKind.Sequential, Size = 0x2C)]
    private struct DdsTrayInner
    {
        public uint version;
        public uint pad0;
        public uint flag;
    }

    [StructLayout(LayoutKind.Sequential, Size = 0x38)]
    private struct DdsTrayOuter
    {
        public uint version;
        public uint pad0;
        public IntPtr innerPtr;
        public uint innerSize;
        public uint flag;
    }

    [DllImport("nvapi64.dll", EntryPoint = "nvapi_QueryInterface", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr NvAPI_QueryInterface(uint id);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int InitDelegate();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int OpenDDSTrayDelegate(IntPtr outer);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int EnumPhysicalGPUsDelegate([Out] IntPtr[] gpus, out uint count);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int GetAllDisplayIdsDelegate(IntPtr gpu, IntPtr buffer, ref uint count);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int GetDDSStateDelegate(uint displayId, IntPtr stateBuf);

    [StructLayout(LayoutKind.Sequential, Size = 16)]
    private struct NV_GPU_DISPLAYIDS
    {
        public uint version;
        public uint connectorType;
        public uint displayId;
        public uint flags;
    }

    private static bool EnsureInitialized()
    {
        if (_initialized) return _openDDSTrayFn != IntPtr.Zero;
        try
        {
            IntPtr initFp = NvAPI_QueryInterface(ID_NvAPI_Initialize);
            if (initFp != IntPtr.Zero)
            {
                int rc = Marshal.GetDelegateForFunctionPointer<InitDelegate>(initFp)();
                if (rc != NVAPI_OK)
                    Logger.WriteLine($"NvAPI_Initialize returned {rc}");
            }
            _enumGpusFn       = NvAPI_QueryInterface(ID_EnumPhysicalGPUs);
            _getDisplayIdsFn  = NvAPI_QueryInterface(ID_GPU_GetAllDisplayIds);
            _getDDSStateFn    = NvAPI_QueryInterface(ID_GetDDSState);
            _openDDSTrayFn    = NvAPI_QueryInterface(ID_OpenDDSTray);
        }
        catch (DllNotFoundException) { }
        catch (Exception ex) { Logger.WriteLine("NvDDS init exception: " + ex.Message); }
        _initialized = true;
        return _openDDSTrayFn != IntPtr.Zero;
    }

    public static bool TryGetState(out DDSSwitchType switchType, out DDSMuxState muxState)
    {
        switchType = DDSSwitchType.Invalid;
        muxState = DDSMuxState.Unknown;
        if (!EnsureInitialized()) return false;
        if (_enumGpusFn == IntPtr.Zero || _getDisplayIdsFn == IntPtr.Zero || _getDDSStateFn == IntPtr.Zero)
            return false;

        try
        {
            var enumGpus     = Marshal.GetDelegateForFunctionPointer<EnumPhysicalGPUsDelegate>(_enumGpusFn);
            var getDisplays  = Marshal.GetDelegateForFunctionPointer<GetAllDisplayIdsDelegate>(_getDisplayIdsFn);
            var getDDSState  = Marshal.GetDelegateForFunctionPointer<GetDDSStateDelegate>(_getDDSStateFn);

            var gpus = new IntPtr[64];
            if (enumGpus(gpus, out uint gpuCount) != NVAPI_OK || gpuCount == 0) return false;

            for (uint g = 0; g < gpuCount; g++)
            {
                uint count = 0;
                if (getDisplays(gpus[g], IntPtr.Zero, ref count) != NVAPI_OK || count == 0) continue;

                int dispSize = Marshal.SizeOf<NV_GPU_DISPLAYIDS>();
                IntPtr buf = Marshal.AllocHGlobal((int)count * dispSize);
                IntPtr stateBuf = Marshal.AllocHGlobal(16);
                try
                {
                    for (int i = 0; i < (int)count * dispSize; i++) Marshal.WriteByte(buf, i, 0);
                    Marshal.WriteInt32(buf, 0, (int)NV_GPU_DISPLAYIDS_VER3);

                    if (getDisplays(gpus[g], buf, ref count) != NVAPI_OK) continue;

                    for (uint d = 0; d < count; d++)
                    {
                        var entry = Marshal.PtrToStructure<NV_GPU_DISPLAYIDS>(buf + (int)(d * dispSize));

                        for (int i = 0; i < 16; i++) Marshal.WriteByte(stateBuf, i, 0);
                        Marshal.WriteInt32(stateBuf, 0, (int)NV_DDS_STATE_VER1);

                        if (getDDSState(entry.displayId, stateBuf) != NVAPI_OK) continue;

                        uint sup = (uint)Marshal.ReadInt32(stateBuf, 12);
                        if ((sup & 1) == 0) continue;

                        switchType = (DDSSwitchType)Marshal.ReadInt32(stateBuf, 4);
                        muxState   = (DDSMuxState)Marshal.ReadInt32(stateBuf, 8);

                        if (switchType == DDSSwitchType.Auto)
                        {
                            Thread.Sleep(300);
                            for (int i = 0; i < 16; i++) Marshal.WriteByte(stateBuf, i, 0);
                            Marshal.WriteInt32(stateBuf, 0, (int)NV_DDS_STATE_VER1);
                            if (getDDSState(entry.displayId, stateBuf) == NVAPI_OK)
                            {
                                switchType = (DDSSwitchType)Marshal.ReadInt32(stateBuf, 4);
                                muxState   = (DDSMuxState)Marshal.ReadInt32(stateBuf, 8);
                            }
                        }

                        Logger.WriteLine($"NvDDS state: switchType={switchType}, muxState={muxState}");
                        return true;
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(buf);
                    Marshal.FreeHGlobal(stateBuf);
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            Logger.WriteLine("NvDDS.TryGetState exception: " + ex.Message);
            return false;
        }
    }

    public static bool OpenTray()
    {
        if (!EnsureInitialized()) return false;

        IntPtr innerBuf = Marshal.AllocHGlobal(Marshal.SizeOf<DdsTrayInner>());
        IntPtr outerBuf = Marshal.AllocHGlobal(Marshal.SizeOf<DdsTrayOuter>());
        try
        {
            for (int i = 0; i < Marshal.SizeOf<DdsTrayInner>(); i++) Marshal.WriteByte(innerBuf, i, 0);
            for (int i = 0; i < Marshal.SizeOf<DdsTrayOuter>(); i++) Marshal.WriteByte(outerBuf, i, 0);

            Marshal.StructureToPtr(new DdsTrayInner
            {
                version = INNER_VERSION,
                flag = 1,
            }, innerBuf, false);

            Marshal.StructureToPtr(new DdsTrayOuter
            {
                version = OUTER_VERSION,
                innerPtr = innerBuf,
                innerSize = (uint)Marshal.SizeOf<DdsTrayInner>(),
                flag = 1,
            }, outerBuf, false);

            int rc = Marshal.GetDelegateForFunctionPointer<OpenDDSTrayDelegate>(_openDDSTrayFn)(outerBuf);
            if (rc != NVAPI_OK)
            {
                Logger.WriteLine($"OpenDDSTray: NVAPI returned {rc}");
                return false;
            }
            Logger.WriteLine("OpenDDSTray: opened");
            return true;
        }
        catch (Exception ex)
        {
            Logger.WriteLine("OpenDDSTray exception: " + ex.Message);
            return false;
        }
        finally
        {
            Marshal.FreeHGlobal(innerBuf);
            Marshal.FreeHGlobal(outerBuf);
        }
    }
}
