using System.Drawing;
using GHelper.Helpers;
using System.Runtime.InteropServices;

namespace GHelper.USB
{
    // -------------------------------------------------------------------------
    // Screen capture via DXGI Desktop Duplication, driven by a dedicated thread
    // that BLOCKS on AcquireNextFrame — it only wakes when DWM delivers a new
    // composed frame. Between frames the thread is fully asleep: zero CPU, zero
    // GPU. Sampling happens on that thread; the public _pixels array is updated
    // under a lock and read by the ambient timer without any GPU involvement.
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

        // DXGI_MAPPED_RECT — returned by MapDesktopSurface
        [StructLayout(LayoutKind.Sequential)]
        struct DXGI_MAPPED_RECT
        {
            public int    Pitch;
            public IntPtr pBits;
        }

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

        // IDXGIOutputDuplication::AcquireNextFrame(UINT timeout, info*, IDXGIResource**) slot 8
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

        // IDXGIOutputDuplication::MapDesktopSurface(DXGI_MAPPED_RECT*) slot 12
        // Only works when the desktop surface is in system memory (iGPU / WARP).
        // Returns DXGI_ERROR_UNSUPPORTED on discrete GPU — we fall back to CopyResource.
        static unsafe int Dupl_MapDesktopSurface(IntPtr dupl, out DXGI_MAPPED_RECT rect)
        {
            rect = default;
            IntPtr* vtbl = *(IntPtr**)dupl;
            var fn = (delegate* unmanaged[Stdcall]<IntPtr, DXGI_MAPPED_RECT*, int>)vtbl[12];
            fixed (DXGI_MAPPED_RECT* p = &rect) return fn(dupl, p);
        }

        // IDXGIOutputDuplication::UnMapDesktopSurface() slot 13
        static unsafe int Dupl_UnMapDesktopSurface(IntPtr dupl)
        {
            IntPtr* vtbl = *(IntPtr**)dupl;
            var fn = (delegate* unmanaged[Stdcall]<IntPtr, int>)vtbl[13];
            return fn(dupl);
        }

        // IDXGIOutputDuplication::ReleaseFrame() slot 14
        static unsafe int Dupl_ReleaseFrame(IntPtr dupl)
        {
            IntPtr* vtbl = *(IntPtr**)dupl;
            var fn = (delegate* unmanaged[Stdcall]<IntPtr, int>)vtbl[14];
            return fn(dupl);
        }

        // ID3D11Device::CreateTexture2D slot 5
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

        // ID3D11DeviceContext::CopyResource slot 47
        static unsafe void Context_CopyResource(IntPtr ctx, IntPtr dst, IntPtr src)
        {
            IntPtr* vtbl = *(IntPtr**)ctx;
            var fn = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr, void>)vtbl[47];
            fn(ctx, dst, src);
        }

        // ID3D11DeviceContext::GenerateMips(ID3D11ShaderResourceView*) slot 54
        static unsafe void Context_GenerateMips(IntPtr ctx, IntPtr srv)
        {
            IntPtr* vtbl = *(IntPtr**)ctx;
            var fn = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr, void>)vtbl[54];
            fn(ctx, srv);
        }

        // ID3D11DeviceContext::CopySubresourceRegion slot 46
        // Used to copy a single mip level from the mip-chain texture to the tiny staging.
        static unsafe void Context_CopySubresourceRegion(IntPtr ctx,
            IntPtr dst, uint dstSub, uint dstX, uint dstY, uint dstZ,
            IntPtr src, uint srcSub, IntPtr pSrcBox)
        {
            IntPtr* vtbl = *(IntPtr**)ctx;
            var fn = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr, uint, uint, uint, uint, IntPtr, uint, IntPtr, void>)vtbl[46];
            fn(ctx, dst, dstSub, dstX, dstY, dstZ, src, srcSub, pSrcBox);
        }

        // ID3D11Device::CreateShaderResourceView(resource, desc*, SRV**) slot 7
        static unsafe int Device_CreateSRV(IntPtr device, IntPtr resource, IntPtr pDesc, out IntPtr srv)
        {
            srv = IntPtr.Zero;
            IntPtr* vtbl = *(IntPtr**)device;
            var fn = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr, IntPtr*, int>)vtbl[7];
            fixed (IntPtr* p = &srv) return fn(device, resource, pDesc, p);
        }

        // ID3D11DeviceContext::Map slot 14
        static unsafe int Context_Map(IntPtr ctx, IntPtr resource, uint sub,
            int mapType, uint flags, out D3D11_MAPPED_SUBRESOURCE mapped)
        {
            mapped = default;
            IntPtr* vtbl = *(IntPtr**)ctx;
            var fn = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr, uint, int, uint, D3D11_MAPPED_SUBRESOURCE*, int>)vtbl[14];
            fixed (D3D11_MAPPED_SUBRESOURCE* pm = &mapped)
                return fn(ctx, resource, sub, mapType, flags, pm);
        }

        // ID3D11DeviceContext::Unmap slot 15
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

        // Latest sampled pixels — written by the capture thread, read by the ambient timer.
        private readonly byte[] _pixels;
        private readonly object _pixelsLock = new();

        // DXGI / D3D11 COM objects
        private IntPtr _device;
        private IntPtr _context;
        private IntPtr _dupl;
        private IntPtr _mipTex;     // D3D11_USAGE_DEFAULT + MipLevels=0 + GenerateMips flag
        private IntPtr _mipSRV;     // shader resource view for GenerateMips
        private IntPtr _mipStaging; // tiny CPU-readable texture for the last mip level
        private uint   _mipLevel;   // index of the mip that is closest to Width×Height
        private uint   _mipW, _mipH;

        private int _screenW, _screenH;

        // The source rectangle to sample — set by the caller, read by the capture thread.
        private Rectangle _source;
        private readonly object _sourceLock = new();

        private bool _disposed;
        private Thread? _thread;
        private volatile bool _stopThread;

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
                int hr = D3D11CreateDevice(
                    IntPtr.Zero, D3D_DRIVER_TYPE_HARDWARE, IntPtr.Zero, 0,
                    IntPtr.Zero, 0, D3D11_SDK_VERSION,
                    out _device, out _, out _context);
                if (hr < 0) { Logger.WriteLine($"DXGI: D3D11CreateDevice hr=0x{hr:X8}"); return; }

                var dxgiDeviceGuid = new Guid("54ec77fa-1377-44e6-8c32-88fd5f44c84c");
                hr = Marshal.QueryInterface(_device, ref dxgiDeviceGuid, out IntPtr dxgiDevice);
                if (hr < 0) { Logger.WriteLine($"DXGI: QI IDXGIDevice hr=0x{hr:X8}"); return; }

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
                if (hr < 0) { Logger.WriteLine($"DXGI: GetAdapter hr=0x{hr:X8}"); return; }

                hr = Adapter_EnumOutputs(adapter, 0, out IntPtr output);
                Marshal.Release(adapter);
                if (hr < 0) { Logger.WriteLine($"DXGI: EnumOutputs hr=0x{hr:X8}"); return; }

                unsafe
                {
                    byte* desc = stackalloc byte[256];
                    IntPtr* vtbl = *(IntPtr**)output;
                    var fn = (delegate* unmanaged[Stdcall]<IntPtr, byte*, int>)vtbl[7];
                    fn(output, desc);
                    int* rc = (int*)(desc + 64);
                    _screenW = rc[2] - rc[0];
                    _screenH = rc[3] - rc[1];
                }
                Logger.WriteLine($"DXGI: screen {_screenW}x{_screenH}");

                var output1Guid = new Guid("00cddea8-939b-4b83-a340-a685226666cc");
                hr = Marshal.QueryInterface(output, ref output1Guid, out IntPtr output1);
                Marshal.Release(output);
                if (hr < 0) { Logger.WriteLine($"DXGI: QI IDXGIOutput1 hr=0x{hr:X8}"); return; }

                hr = Output1_DuplicateOutput(output1, _device, out _dupl);
                Marshal.Release(output1);
                if (hr < 0) { Logger.WriteLine($"DXGI: DuplicateOutput hr=0x{hr:X8}"); return; }

                // Choose the mip level whose dimensions are closest to Width×Height.
                // Each mip halves both dimensions: mip N = (screenW>>N) x (screenH>>N).
                // We want the last mip where both dimensions are >= Width and >= Height.
                _mipLevel = 0;
                while ((_screenW >> (int)(_mipLevel + 1)) >= Width &&
                       (_screenH >> (int)(_mipLevel + 1)) >= Height)
                    _mipLevel++;
                _mipW = (uint)(_screenW >> (int)_mipLevel);
                _mipH = (uint)(_screenH >> (int)_mipLevel);
                Logger.WriteLine($"DXGI: using mip {_mipLevel} = {_mipW}x{_mipH}");

                // Full mip-chain texture: D3D11_USAGE_DEFAULT, BIND_SHADER_RESOURCE,
                // D3D11_RESOURCE_MISC_GENERATE_MIPS, MipLevels=0 (full chain).
                var mipDesc = new D3D11_TEXTURE2D_DESC
                {
                    Width = (uint)_screenW, Height = (uint)_screenH,
                    MipLevels = 0,           // 0 = full chain
                    ArraySize = 1,
                    Format = 87,             // DXGI_FORMAT_B8G8R8A8_UNORM
                    SampleDescCount = 1,
                    Usage = 0,               // D3D11_USAGE_DEFAULT
                    BindFlags = 0x8 | 0x20,  // BIND_SHADER_RESOURCE | BIND_RENDER_TARGET
                    MiscFlags = 1,           // D3D11_RESOURCE_MISC_GENERATE_MIPS
                };
                hr = Device_CreateTexture2D(_device, in mipDesc, out _mipTex);
                if (hr < 0) { Logger.WriteLine($"DXGI: CreateTexture2D mip hr=0x{hr:X8}"); return; }

                hr = Device_CreateSRV(_device, _mipTex, IntPtr.Zero, out _mipSRV);
                if (hr < 0) { Logger.WriteLine($"DXGI: CreateSRV hr=0x{hr:X8}"); return; }

                // Tiny staging texture sized to the chosen mip level.
                var stagDesc = new D3D11_TEXTURE2D_DESC
                {
                    Width = _mipW, Height = _mipH,
                    MipLevels = 1, ArraySize = 1,
                    Format = 87,
                    SampleDescCount = 1,
                    Usage = 3,               // D3D11_USAGE_STAGING
                    CPUAccessFlags = 131072, // D3D11_CPU_ACCESS_READ
                };
                hr = Device_CreateTexture2D(_device, in stagDesc, out _mipStaging);
                if (hr < 0) { Logger.WriteLine($"DXGI: CreateTexture2D staging hr=0x{hr:X8}"); return; }

                // Start the dedicated blocking capture thread
                _stopThread = false;
                _thread = new Thread(CaptureThreadProc)
                {
                    IsBackground = true,
                    Priority = ThreadPriority.BelowNormal,
                    Name = "AmbientCapture"
                };
                _thread.Start();
                Logger.WriteLine("DXGI: capture thread started");
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"DXGI: TryInitDxgi exception: {ex.Message}");
            }
        }

        private unsafe void CaptureThreadProc()
        {
            const int TIMEOUT_MS   = 500;  // blocking wait for next frame
            const int THROTTLE_MS  = 100;  // ~10 fps max for ambient lighting

            while (!_stopThread && !_disposed)
            {
                Rectangle source;
                lock (_sourceLock) { source = _source; }
                if (source.Width <= 0 || source.Height <= 0)
                {
                    Thread.Sleep(50);
                    continue;
                }

                int hr = Dupl_AcquireNextFrame(_dupl, (uint)TIMEOUT_MS,
                    out var frameInfo, out IntPtr resource);

                if (hr == DXGI_ERROR_WAIT_TIMEOUT) continue;

                if (hr == DXGI_ERROR_ACCESS_LOST)
                {
                    SafeRelease(ref _mipStaging);
                    SafeRelease(ref _mipSRV);
                    SafeRelease(ref _mipTex);
                    SafeRelease(ref _dupl);
                    SafeRelease(ref _context);
                    SafeRelease(ref _device);
                    TryInitDxgi();
                    return;
                }

                if (hr < 0) { Thread.Sleep(50); continue; }

                try
                {
                    if (frameInfo.LastPresentTime == 0 || resource == IntPtr.Zero) continue;

                    // QI IDXGIResource → ID3D11Texture2D
                    var tex2dGuid = new Guid("6f15aaf2-d208-4e89-9ab4-489535d34f9c");
                    hr = Marshal.QueryInterface(resource, ref tex2dGuid, out IntPtr tex);
                    if (hr < 0) continue;

                    try
                    {
                        Context_CopySubresourceRegion(_context,
                            _mipTex, 0, 0, 0, 0,
                            tex,     0, IntPtr.Zero);
                    }
                    finally { Marshal.Release(tex); }

                    Context_GenerateMips(_context, _mipSRV);

                    Context_CopySubresourceRegion(_context,
                        _mipStaging, 0, 0, 0, 0,
                        _mipTex, _mipLevel, IntPtr.Zero);

                    hr = Context_Map(_context, _mipStaging, 0, 1, 0, out var mapped);
                    if (hr < 0) continue;
                    try   { SamplePixels((byte*)mapped.pData, (int)mapped.RowPitch, source); }
                    finally { Context_Unmap(_context, _mipStaging, 0); }
                }
                finally
                {
                    Dupl_ReleaseFrame(_dupl);
                    if (resource != IntPtr.Zero) Marshal.Release(resource);
                }

                // Throttle to ~10 fps. Sleep here — DWM keeps compositing normally,
                // and the next AcquireNextFrame will just return the latest frame.
                Thread.Sleep(THROTTLE_MS);
            }
        }

        // Sample the tiny mip-level texture into _pixels.
        // The mip already represents the entire screen downsampled by the GPU.
        // We just evenly pick Width*Height cells from it.
        private unsafe void SamplePixels(byte* data, int rowPitch, Rectangle source)
        {
            // Map the mip coords to the source sub-region (lower portion of screen).
            // source is in screen-space; convert to mip-space.
            float scaleX = (float)_mipW / _screenW;
            float scaleY = (float)_mipH / _screenH;

            int srcX = (int)(Math.Max(0, source.X)               * scaleX);
            int srcY = (int)(Math.Max(0, source.Y)               * scaleY);
            int srcW = (int)(Math.Min(source.Width,  _screenW)   * scaleX);
            int srcH = (int)(Math.Min(source.Height, _screenH)   * scaleY);
            srcW = Math.Max(1, srcW);
            srcH = Math.Max(1, srcH);

            float cellW = (float)srcW / Width;
            float cellH = (float)srcH / Height;

            Span<byte> tmp = stackalloc byte[Width * Height * 4];
            for (int py = 0; py < Height; py++)
            {
                int sy = Math.Clamp(srcY + (int)(cellH * (py + 0.5f)), 0, (int)_mipH - 1);
                byte* row = data + sy * rowPitch;
                for (int px = 0; px < Width; px++)
                {
                    int sx = Math.Clamp(srcX + (int)(cellW * (px + 0.5f)), 0, (int)_mipW - 1);
                    byte* pixel = row + sx * 4;
                    int i = (py * Width + px) * 4;
                    tmp[i]     = pixel[0]; // B
                    tmp[i + 1] = pixel[1]; // G
                    tmp[i + 2] = pixel[2]; // R
                    tmp[i + 3] = pixel[3];
                }
            }
            lock (_pixelsLock) tmp.CopyTo(_pixels);
        }

        // Called by the ambient timer to set the region of interest.
        // The actual capture happens on the background thread.
        public void Capture(Rectangle source)
        {
            lock (_sourceLock) { _source = source; }
        }

        private void ReinitDuplication()
        {
            SafeRelease(ref _mipStaging);
            SafeRelease(ref _mipSRV);
            SafeRelease(ref _mipTex);
            SafeRelease(ref _dupl);
            SafeRelease(ref _context);
            SafeRelease(ref _device);
            TryInitDxgi();
        }

        public Color GetPixel(int x, int y)
        {
            lock (_pixelsLock)
            {
                int i = (y * Width + x) * 4;
                return Color.FromArgb(_pixels[i + 2], _pixels[i + 1], _pixels[i]);
            }
        }

        public Color GetAverageColor()
        {
            lock (_pixelsLock)
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
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _stopThread = true;
            _thread?.Join(2000);
            SafeRelease(ref _mipStaging);
            SafeRelease(ref _mipSRV);
            SafeRelease(ref _mipTex);
            SafeRelease(ref _dupl);
            SafeRelease(ref _context);
            SafeRelease(ref _device);
        }
    }
}
