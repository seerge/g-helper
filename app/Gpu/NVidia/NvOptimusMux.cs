using System.Runtime.InteropServices;

namespace GHelper.Gpu.NVidia;

public static class NvOptimusMux
{
    private const int NVAPI_OK = 0;

    private const uint ID_NvAPI_Initialize        = 0x0150E828;
    private const uint ID_EnumPhysicalGPUs        = 0xE5AC921F;
    private const uint ID_GPU_GetAllDisplayIds    = 0x785210A2;
    private const uint ID_QueryMuxState           = 0x6050BB05;
    private const uint ID_OpenMuxTray             = 0x5C3565D0;

    private const uint NV_GPU_DISPLAYIDS_VER3 = 0x30010;
    private const uint NV_MUX_STATE_VER1      = 0x10010;

    public enum OptimusSwitchType { Invalid = 0, Auto = 1, Manual = 2 }
    public enum OptimusMuxState   { Unknown = 0, IGpu = 1, DGpu = 2 }

    private static bool _initialized;
    private static IntPtr _enumGpusFn;
    private static IntPtr _getDisplayIdsFn;
    private static IntPtr _queryMuxStateFn;
    private static IntPtr _openMuxTrayFn;

    private const uint OUTER_VERSION = 0x10038;
    private const uint INNER_VERSION = 0x1002C;

    [StructLayout(LayoutKind.Sequential, Size = 0x2C)]
    private struct MuxTrayInner
    {
        public uint version;
        public uint pad0;
        public uint flag;
    }

    [StructLayout(LayoutKind.Sequential, Size = 0x38)]
    private struct MuxTrayOuter
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
    private delegate int OpenMuxTrayDelegate(IntPtr outer);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int EnumPhysicalGPUsDelegate([Out] IntPtr[] gpus, out uint count);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int GetAllDisplayIdsDelegate(IntPtr gpu, IntPtr buffer, ref uint count);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int QueryMuxStateDelegate(uint displayId, IntPtr stateBuf);

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
        if (_initialized) return _openMuxTrayFn != IntPtr.Zero;
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
            _queryMuxStateFn  = NvAPI_QueryInterface(ID_QueryMuxState);
            _openMuxTrayFn    = NvAPI_QueryInterface(ID_OpenMuxTray);
        }
        catch (DllNotFoundException) { }
        catch (Exception ex) { Logger.WriteLine("NvOptimusMux init exception: " + ex.Message); }
        _initialized = true;
        return _openMuxTrayFn != IntPtr.Zero;
    }

    public static bool TryGetState(out OptimusSwitchType switchType, out OptimusMuxState muxState)
    {
        switchType = OptimusSwitchType.Invalid;
        muxState = OptimusMuxState.Unknown;
        if (!EnsureInitialized()) return false;
        if (_enumGpusFn == IntPtr.Zero || _getDisplayIdsFn == IntPtr.Zero || _queryMuxStateFn == IntPtr.Zero)
            return false;

        try
        {
            var enumGpus      = Marshal.GetDelegateForFunctionPointer<EnumPhysicalGPUsDelegate>(_enumGpusFn);
            var getDisplays   = Marshal.GetDelegateForFunctionPointer<GetAllDisplayIdsDelegate>(_getDisplayIdsFn);
            var queryMuxState = Marshal.GetDelegateForFunctionPointer<QueryMuxStateDelegate>(_queryMuxStateFn);

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
                        Marshal.WriteInt32(stateBuf, 0, (int)NV_MUX_STATE_VER1);

                        if (queryMuxState(entry.displayId, stateBuf) != NVAPI_OK) continue;

                        uint sup = (uint)Marshal.ReadInt32(stateBuf, 12);
                        if ((sup & 1) == 0) continue;

                        switchType = (OptimusSwitchType)Marshal.ReadInt32(stateBuf, 4);
                        muxState   = (OptimusMuxState)Marshal.ReadInt32(stateBuf, 8);

                        if (switchType == OptimusSwitchType.Auto)
                        {
                            Thread.Sleep(300);
                            for (int i = 0; i < 16; i++) Marshal.WriteByte(stateBuf, i, 0);
                            Marshal.WriteInt32(stateBuf, 0, (int)NV_MUX_STATE_VER1);
                            if (queryMuxState(entry.displayId, stateBuf) == NVAPI_OK)
                            {
                                switchType = (OptimusSwitchType)Marshal.ReadInt32(stateBuf, 4);
                                muxState   = (OptimusMuxState)Marshal.ReadInt32(stateBuf, 8);
                            }
                        }

                        Logger.WriteLine($"NvOptimusMux state: switchType={switchType}, muxState={muxState}");
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
            Logger.WriteLine("NvOptimusMux.TryGetState exception: " + ex.Message);
            return false;
        }
    }

    public static bool OpenTray()
    {
        if (!EnsureInitialized()) return false;

        IntPtr innerBuf = Marshal.AllocHGlobal(Marshal.SizeOf<MuxTrayInner>());
        IntPtr outerBuf = Marshal.AllocHGlobal(Marshal.SizeOf<MuxTrayOuter>());
        try
        {
            for (int i = 0; i < Marshal.SizeOf<MuxTrayInner>(); i++) Marshal.WriteByte(innerBuf, i, 0);
            for (int i = 0; i < Marshal.SizeOf<MuxTrayOuter>(); i++) Marshal.WriteByte(outerBuf, i, 0);

            Marshal.StructureToPtr(new MuxTrayInner
            {
                version = INNER_VERSION,
                flag = 1,
            }, innerBuf, false);

            Marshal.StructureToPtr(new MuxTrayOuter
            {
                version = OUTER_VERSION,
                innerPtr = innerBuf,
                innerSize = (uint)Marshal.SizeOf<MuxTrayInner>(),
                flag = 1,
            }, outerBuf, false);

            int rc = Marshal.GetDelegateForFunctionPointer<OpenMuxTrayDelegate>(_openMuxTrayFn)(outerBuf);
            if (rc != NVAPI_OK)
            {
                Logger.WriteLine($"NvOptimusMux.OpenTray: NVAPI returned {rc}");
                return false;
            }
            Logger.WriteLine("NvOptimusMux.OpenTray: opened");
            return true;
        }
        catch (Exception ex)
        {
            Logger.WriteLine("NvOptimusMux.OpenTray exception: " + ex.Message);
            return false;
        }
        finally
        {
            Marshal.FreeHGlobal(innerBuf);
            Marshal.FreeHGlobal(outerBuf);
        }
    }
}
