using System.Drawing;
using GHelper.Helpers;
using System.Runtime.InteropServices;

namespace GHelper.USB
{
    // -------------------------------------------------------------------------
    // Screen capture using the Windows Desktop Duplication API (DXGI 1.2+).
    // IDXGIOutputDuplication reads frames directly from the compositor's own
    // surface — it never stalls DWM and has near-zero CPU overhead between
    // frames because it only wakes up when something on screen actually changes.
    // -------------------------------------------------------------------------
    public sealed class ScreenCaptureSampler : IDisposable
    {
        #region DXGI / D3D11 COM interop

        // GUIDs (from Windows SDK headers)
        static readonly Guid IID_IDXGIOutput1    = new("00cddea8-939b-4b83-a340-a685226666cc"); // dxgi1_2.h
        static readonly Guid IID_ID3D11Texture2D = new("6f15aaf2-d208-4e89-9ab4-489535d34f9c"); // d3d11.h

        [DllImport("dxgi.dll")]
        static extern int CreateDXGIFactory1(in Guid riid, out IntPtr ppFactory);

        [DllImport("d3d11.dll")]
        static extern int D3D11CreateDevice(
            IntPtr pAdapter, int DriverType, IntPtr Software, uint Flags,
            IntPtr pFeatureLevels, uint FeatureLevels, uint SDKVersion,
            out IntPtr ppDevice, out int pFeatureLevel, out IntPtr ppImmediateContext);

        const int D3D_DRIVER_TYPE_UNKNOWN   = 0;
        const int D3D_DRIVER_TYPE_HARDWARE  = 1;
        const uint D3D11_SDK_VERSION        = 7;
        const int DXGI_ERROR_WAIT_TIMEOUT   = unchecked((int)0x887A0027);
        const int DXGI_ERROR_ACCESS_LOST    = unchecked((int)0x887A0026);

        // D3D11_TEXTURE2D_DESC
        [StructLayout(LayoutKind.Sequential)]
        struct D3D11_TEXTURE2D_DESC
        {
            public uint Width, Height, MipLevels, ArraySize;
            public int  Format;           // DXGI_FORMAT
            public uint SampleDescCount, SampleDescQuality;
            public int  Usage;            // D3D11_USAGE
            public uint BindFlags, CPUAccessFlags, MiscFlags;
        }

        // DXGI_OUTDUPL_FRAME_INFO
        [StructLayout(LayoutKind.Sequential)]
        struct DXGI_OUTDUPL_FRAME_INFO
        {
            public long LastPresentTime, LastMouseUpdateTime;
            public uint AccumulatedFrames;
            public int  RectsCoalesced, ProtectedContentMasked;
            public uint TotalMetadataBufferSize, PointerShapeBufferSize;
        }

        // D3D11_MAPPED_SUBRESOURCE
        [StructLayout(LayoutKind.Sequential)]
        struct D3D11_MAPPED_SUBRESOURCE
        {
            public IntPtr pData;
            public uint   RowPitch, DepthPitch;
        }

        // Minimal vtable offsets for the COM interfaces we use.
        // All counts are zero-based from IUnknown (QI=0, AddRef=1, Release=2).

        // IDXGIFactory1  vtable:  EnumAdapters=7
        // IDXGIAdapter   vtable:  EnumOutputs=7
        // IDXGIOutput    vtable:  GetDesc=7, QueryInterface inherited
        // IDXGIOutput1   vtable:  DuplicateOutput=22
        // IDXGIOutputDuplication vtable: GetDesc=7, AcquireNextFrame=8, GetFrameDirtyRects=9,
        //                                GetFrameMoveRects=10, GetFramePointerShape=11,
        //                                MapDesktopSurface=12, UnMapDesktopSurface=13, ReleaseFrame=14
        // ID3D11Device   vtable:  CreateTexture2D=5
        // ID3D11DeviceContext vtable: CopySubresourceRegion=46(?), Map=14, Unmap=15
        //   (exact offsets confirmed against SDK headers below)

        // Helper: call a COM vtable slot with given args via function pointer.
        // Using unsafe + calli for zero-overhead COM dispatch.

        #endregion

        #region Thin COM wrappers

        static void SafeRelease(ref IntPtr p)
        {
            if (p == IntPtr.Zero) return;
            Marshal.Release(p);
            p = IntPtr.Zero;
        }

        #endregion

        #region Vtable dispatch helpers (unsafe)

        // IDXGIFactory1::EnumAdapters(UINT, IDXGIAdapter**)  slot 7
        static unsafe int Factory_EnumAdapters(IntPtr factory, uint idx, out IntPtr adapter)
        {
            adapter = IntPtr.Zero;
            IntPtr* vtbl = *(IntPtr**)factory;
            var fn = (delegate* unmanaged[Stdcall]<IntPtr, uint, IntPtr*, int>)vtbl[7];
            fixed (IntPtr* p = &adapter) return fn(factory, idx, p);
        }

        // IDXGIAdapter::EnumOutputs(UINT, IDXGIOutput**) slot 7
        static unsafe int Adapter_EnumOutputs(IntPtr adapter, uint idx, out IntPtr output)
        {
            output = IntPtr.Zero;
            IntPtr* vtbl = *(IntPtr**)adapter;
            var fn = (delegate* unmanaged[Stdcall]<IntPtr, uint, IntPtr*, int>)vtbl[7];
            fixed (IntPtr* p = &output) return fn(adapter, idx, p);
        }

        // IDXGIOutput1::DuplicateOutput(ID3D11Device*, IDXGIOutputDuplication**) slot 22
        static unsafe int Output1_DuplicateOutput(IntPtr output1, IntPtr device, out IntPtr dupl)
        {
            dupl = IntPtr.Zero;
            IntPtr* vtbl = *(IntPtr**)output1;
            var fn = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr*, int>)vtbl[22];
            fixed (IntPtr* p = &dupl) return fn(output1, device, p);
        }

        // IDXGIOutputDuplication::AcquireNextFrame(UINT timeout, DXGI_OUTDUPL_FRAME_INFO*, IDXGIResource**) slot 8
        static unsafe int Dupl_AcquireNextFrame(IntPtr dupl, uint timeout,
            out DXGI_OUTDUPL_FRAME_INFO info, out IntPtr resource)
        {
            info     = default;
            resource = IntPtr.Zero;
            IntPtr* vtbl = *(IntPtr**)dupl;
            var fn = (delegate* unmanaged[Stdcall]<IntPtr, uint, DXGI_OUTDUPL_FRAME_INFO*, IntPtr*, int>)vtbl[8];
            fixed (DXGI_OUTDUPL_FRAME_INFO* pi = &info)
            fixed (IntPtr* pr = &resource)
                return fn(dupl, timeout, pi, pr);
        }

        // IDXGIOutputDuplication::ReleaseFrame() slot 14
        static unsafe int Dupl_ReleaseFrame(IntPtr dupl)
        {
            IntPtr* vtbl = *(IntPtr**)dupl;
            var fn = (delegate* unmanaged[Stdcall]<IntPtr, int>)vtbl[14];
            return fn(dupl);
        }

        // ID3D11Device::CreateTexture2D(desc, null, ID3D11Texture2D**) slot 5
        static unsafe int Device_CreateTexture2D(IntPtr device,
            in D3D11_TEXTURE2D_DESC desc, out IntPtr tex)
        {
            tex = IntPtr.Zero;
            IntPtr* vtbl = *(IntPtr**)device;
            var fn = (delegate* unmanaged[Stdcall]<IntPtr, D3D11_TEXTURE2D_DESC*, IntPtr, IntPtr*, int>)vtbl[5];
            fixed (D3D11_TEXTURE2D_DESC* pd = &desc)
            fixed (IntPtr* pt = &tex)
                return fn(device, pd, IntPtr.Zero, pt);
        }

        // ID3D11DeviceContext::CopyResource(dst, src) slot 47
        static unsafe void Context_CopyResource(IntPtr ctx, IntPtr dst, IntPtr src)
        {
            IntPtr* vtbl = *(IntPtr**)ctx;
            var fn = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr, void>)vtbl[47];
            fn(ctx, dst, src);
        }

        // ID3D11DeviceContext::Map(resource, sub, mapType, flags, MappedSubresource*) slot 14
        static unsafe int Context_Map(IntPtr ctx, IntPtr resource, uint sub,
            int mapType, uint flags, out D3D11_MAPPED_SUBRESOURCE mapped)
        {
            mapped = default;
            IntPtr* vtbl = *(IntPtr**)ctx;
            var fn = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr, uint, int, uint, D3D11_MAPPED_SUBRESOURCE*, int>)vtbl[14];
            fixed (D3D11_MAPPED_SUBRESOURCE* pm = &mapped)
                return fn(ctx, resource, sub, mapType, flags, pm);
        }

        // ID3D11DeviceContext::Unmap(resource, sub) slot 15
        static unsafe void Context_Unmap(IntPtr ctx, IntPtr resource, uint sub)
        {
            IntPtr* vtbl = *(IntPtr**)ctx;
            var fn = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr, uint, void>)vtbl[15];
            fn(ctx, resource, sub);
        }

        #endregion

        // ---- Public surface ----

        public readonly int Width;
        public readonly int Height;

        // Sampled pixel grid: BGRA, [y * Width + x] * 4
        private readonly byte[] _pixels;

        // DXGI / D3D11 objects (all raw COM pointers)
        private IntPtr _device;
        private IntPtr _context;
        private IntPtr _dupl;           // IDXGIOutputDuplication
        private IntPtr _staging;        // CPU-readable ID3D11Texture2D (staging)

        // Screen dimensions at the time of initialisation (needed for sampling math)
        private int _screenW, _screenH;

        private bool _disposed;
        private bool _dxgiFailed;       // fallback flag: if DXGI init failed, do nothing

        public ScreenCaptureSampler(int width, int height)
        {
            Width   = width;
            Height  = height;
            _pixels = new byte[width * height * 4];

            TryInitDxgi();
        }

        private void TryInitDxgi()
        {
            try
            {
                // 1. Create D3D11 device on the default hardware adapter
                int hr = D3D11CreateDevice(
                    IntPtr.Zero, D3D_DRIVER_TYPE_HARDWARE, IntPtr.Zero, 0,
                    IntPtr.Zero, 0, D3D11_SDK_VERSION,
                    out _device, out _, out _context);
                if (hr < 0) { Logger.WriteLine($"DXGI: D3D11CreateDevice failed hr=0x{hr:X8}"); _dxgiFailed = true; return; }

                // 2. Get IDXGIDevice from the D3D device, walk up to IDXGIAdapter → IDXGIOutput
                var dxgiDeviceGuid = new Guid("54ec77fa-1377-44e6-8c32-88fd5f44c84c"); // IDXGIDevice
                hr = Marshal.QueryInterface(_device, ref dxgiDeviceGuid, out IntPtr dxgiDevice);
                if (hr < 0) { Logger.WriteLine($"DXGI: QI IDXGIDevice failed hr=0x{hr:X8}"); _dxgiFailed = true; return; }

                // IDXGIDevice::GetAdapter — slot 7
                // vtable: QI=0,AddRef=1,Release=2,SetPriv=3,SetPrivIface=4,GetPriv=5,GetParent=6,GetAdapter=7
                IntPtr adapter;
                unsafe
                {
                    IntPtr* vtbl = *(IntPtr**)dxgiDevice;
                    var fn = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr*, int>)vtbl[7];
                    IntPtr tmp = IntPtr.Zero;
                    hr = fn(dxgiDevice, &tmp);
                    adapter = tmp;
                }
                Marshal.Release(dxgiDevice);
                if (hr < 0) { Logger.WriteLine($"DXGI: GetAdapter failed hr=0x{hr:X8}"); _dxgiFailed = true; return; }

                // 3. Enumerate first output of this adapter
                hr = Adapter_EnumOutputs(adapter, 0, out IntPtr output);
                Marshal.Release(adapter);
                if (hr < 0) { Logger.WriteLine($"DXGI: EnumOutputs failed hr=0x{hr:X8}"); _dxgiFailed = true; return; }

                // 4. Get screen dimensions from DXGI_OUTPUT_DESC
                //    Layout: DeviceName[32 WCHARs=64 bytes] | DesktopCoordinates RECT(16 bytes) | ...
                unsafe
                {
                    byte* desc = stackalloc byte[256];
                    IntPtr* vtbl = *(IntPtr**)output;
                    var fn = (delegate* unmanaged[Stdcall]<IntPtr, byte*, int>)vtbl[7]; // GetDesc
                    fn(output, desc);
                    // DesktopCoordinates RECT starts at byte offset 64 (after 32 WCHARs)
                    int* rc = (int*)(desc + 64);
                    _screenW = rc[2] - rc[0]; // right - left
                    _screenH = rc[3] - rc[1]; // bottom - top
                }
                Logger.WriteLine($"DXGI: screen {_screenW}x{_screenH}");

                // 5. QI output for IDXGIOutput1 to get DuplicateOutput
                var output1Guid = new Guid("00cddea8-939b-4b83-a340-a685226666cc"); // IID_IDXGIOutput1 (dxgi1_2.h)
                hr = Marshal.QueryInterface(output, ref output1Guid, out IntPtr output1);
                Marshal.Release(output);
                if (hr < 0) { Logger.WriteLine($"DXGI: QI IDXGIOutput1 failed hr=0x{hr:X8}"); _dxgiFailed = true; return; }

                // 6. Create the duplication
                hr = Output1_DuplicateOutput(output1, _device, out _dupl);
                Marshal.Release(output1);
                if (hr < 0) { Logger.WriteLine($"DXGI: DuplicateOutput failed hr=0x{hr:X8}"); _dxgiFailed = true; return; }

                // 7. Create a CPU-readable staging texture matching the full screen
                var desc2d = new D3D11_TEXTURE2D_DESC
                {
                    Width             = (uint)_screenW,
                    Height            = (uint)_screenH,
                    MipLevels         = 1,
                    ArraySize         = 1,
                    Format            = 87,    // DXGI_FORMAT_B8G8R8A8_UNORM
                    SampleDescCount   = 1,
                    SampleDescQuality = 0,
                    Usage             = 3,     // D3D11_USAGE_STAGING
                    BindFlags         = 0,
                    CPUAccessFlags    = 131072, // D3D11_CPU_ACCESS_READ (0x20000)
                    MiscFlags         = 0,
                };
                hr = Device_CreateTexture2D(_device, in desc2d, out _staging);
                if (hr < 0) { Logger.WriteLine($"DXGI: CreateTexture2D staging failed hr=0x{hr:X8}"); _dxgiFailed = true; return; }

                Logger.WriteLine($"DXGI: ScreenCaptureSampler ready");
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"DXGI: TryInitDxgi exception: {ex.Message}");
                _dxgiFailed = true;
            }
        }

        // Capture and downsample the given screen rectangle into the Width×Height pixel grid.
        public unsafe void Capture(Rectangle source)
        {
            if (_dxgiFailed || _disposed) return;

            // Acquire the latest composed frame from DWM (0 ms timeout = non-blocking)
            int hr = Dupl_AcquireNextFrame(_dupl, 0, out var frameInfo, out IntPtr resource);

            if (hr == DXGI_ERROR_WAIT_TIMEOUT)
                return; // nothing changed on screen — keep previous pixels

            if (hr == DXGI_ERROR_ACCESS_LOST)
            {
                // Desktop switch / UAC / lock screen — recreate duplication
                ReinitDuplication();
                return;
            }

            if (hr < 0) return;

            // ReleaseFrame must always be called after a successful AcquireNextFrame,
            // regardless of AccumulatedFrames — failing to do so permanently breaks
            // subsequent AcquireNextFrame calls with DXGI_ERROR_INVALID_CALL.
            try
            {
                if (frameInfo.LastPresentTime == 0 || resource == IntPtr.Zero)
                    return; // no new visual content, but ReleaseFrame still runs in finally

                // QI IDXGIResource → ID3D11Texture2D
                var tex2dGuid = new Guid("6f15aaf2-d208-4e89-9ab4-489535d34f9c");
                hr = Marshal.QueryInterface(resource, ref tex2dGuid, out IntPtr tex);
                if (hr < 0) return;

                try
                {
                    // Copy GPU texture → staging (CPU-accessible)
                    Context_CopyResource(_context, _staging, tex);
                }
                finally { Marshal.Release(tex); }

                // Map staging to get a CPU pointer
                hr = Context_Map(_context, _staging, 0, 1 /*D3D11_MAP_READ*/, 0, out var mapped);
                if (hr < 0) return;

                try
                {
                    // Downsample source rectangle into Width×Height by sampling the center of each cell
                    byte* src = (byte*)mapped.pData;
                    int rowPitch = (int)mapped.RowPitch;

                    int srcX = Math.Max(0, source.X);
                    int srcY = Math.Max(0, source.Y);
                    int srcW = Math.Min(source.Width,  _screenW - srcX);
                    int srcH = Math.Min(source.Height, _screenH - srcY);

                    float cellW = (float)srcW / Width;
                    float cellH = (float)srcH / Height;

                    for (int py = 0; py < Height; py++)
                    {
                        int sy = srcY + (int)(cellH * (py + 0.5f));
                        sy = Math.Clamp(sy, 0, _screenH - 1);
                        byte* row = src + sy * rowPitch;

                        for (int px = 0; px < Width; px++)
                        {
                            int sx = srcX + (int)(cellW * (px + 0.5f));
                            sx = Math.Clamp(sx, 0, _screenW - 1);

                            byte* pixel = row + sx * 4;
                            int i = (py * Width + px) * 4;
                            _pixels[i]     = pixel[0]; // B
                            _pixels[i + 1] = pixel[1]; // G
                            _pixels[i + 2] = pixel[2]; // R
                            _pixels[i + 3] = pixel[3]; // A
                        }
                    }
                }
                finally { Context_Unmap(_context, _staging, 0); }
            }
            finally
            {
                Dupl_ReleaseFrame(_dupl);
                if (resource != IntPtr.Zero) Marshal.Release(resource);
            }
        }

        private void ReinitDuplication()
        {
            SafeRelease(ref _staging);
            SafeRelease(ref _dupl);
            SafeRelease(ref _context);
            SafeRelease(ref _device);
            _dxgiFailed = false;
            TryInitDxgi();
        }

        public Color GetPixel(int x, int y)
        {
            int i = (y * Width + x) * 4;
            return Color.FromArgb(_pixels[i + 2], _pixels[i + 1], _pixels[i]);
        }

        // Averages all sampled pixels directly from the raw buffer.
        public Color GetAverageColor()
        {
            int r = 0, g = 0, b = 0;
            int total = _pixels.Length / 4;
            for (int i = 0; i < _pixels.Length; i += 4)
            {
                b += _pixels[i];
                g += _pixels[i + 1];
                r += _pixels[i + 2];
            }
            return Color.FromArgb(r / total, g / total, b / total);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            SafeRelease(ref _staging);
            SafeRelease(ref _dupl);
            SafeRelease(ref _context);
            SafeRelease(ref _device);
        }
    }
}
