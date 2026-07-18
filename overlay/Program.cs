using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace GHelperOverlay;

// Thin uiAccess renderer, pure Win32: GHelper composes overlay frames and writes
// them to a shared-memory buffer (see OverlayIpc); this process only blits them
// into a layered topmost window and forwards raw mouse input back. No WinForms,
// no GDI+, single thread - it exists solely because uiAccess (drawing above
// fullscreen games) requires a separate signed executable in Program Files.
internal static unsafe class Program
{
    private const uint WS_POPUP = 0x80000000;
    private const int WS_EX_TOPMOST = 0x8;
    private const int WS_EX_TOOLWINDOW = 0x80;
    private const int WS_EX_LAYERED = 0x80000;
    private const int WS_EX_TRANSPARENT = 0x20;
    private const int WS_EX_NOACTIVATE = 0x08000000;
    private const int GWL_EXSTYLE = -20;
    private const int SW_HIDE = 0;
    private const int SW_SHOWNOACTIVATE = 4;
    private const uint WM_MOUSEMOVE = 0x0200;
    private const uint WM_LBUTTONDOWN = 0x0201;
    private const uint WM_LBUTTONUP = 0x0202;
    private const uint WM_MBUTTONDOWN = 0x0207;
    private const uint WM_MOUSEWHEEL = 0x020A;
    private const uint WM_SETCURSOR = 0x0020;
    private const uint WM_NCDESTROY = 0x0082;
    private const uint WM_QUIT = 0x0012;
    private const uint SWP_NOSIZE = 0x1;
    private const uint SWP_NOMOVE = 0x2;
    private const uint SWP_NOZORDER = 0x4;
    private const uint SWP_NOACTIVATE = 0x10;
    private const uint GW_HWNDPREV = 3;
    private const int IDC_SIZEALL = 32646;
    private const int IDC_HAND = 32649;
    private const uint PM_REMOVE = 1;
    private const uint QS_ALLINPUT = 0x04FF;
    private const uint MWMO_INPUTAVAILABLE = 0x4;
    private const uint WAIT_FAILED = 0xFFFFFFFF;
    private const uint INFINITE = 0xFFFFFFFF;
    private const uint SYNCHRONIZE = 0x00100000;
    private const uint TOKEN_QUERY = 0x8;
    private const int TokenUIAccess = 26;
    private static readonly IntPtr HWND_TOPMOST = new(-1);

    private delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    private static readonly WndProcDelegate _wndProc = WndProc; // keeps the marshaled callback alive

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct WNDCLASSEX
    {
        public uint cbSize, style;
        public IntPtr lpfnWndProc;
        public int cbClsExtra, cbWndExtra;
        public IntPtr hInstance, hIcon, hCursor, hbrBackground;
        [MarshalAs(UnmanagedType.LPWStr)] public string? lpszMenuName;
        [MarshalAs(UnmanagedType.LPWStr)] public string lpszClassName;
        public IntPtr hIconSm;
    }

    [StructLayout(LayoutKind.Sequential)] private struct POINT { public int x, y; }
    [StructLayout(LayoutKind.Sequential)] private struct SIZE { public int cx, cy; }
    [StructLayout(LayoutKind.Sequential)]
    private struct MSG
    {
        public IntPtr hwnd;
        public uint message;
        public IntPtr wParam, lParam;
        public uint time;
        public POINT pt;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct BLENDFUNCTION { public byte BlendOp, BlendFlags, SourceConstantAlpha, AlphaFormat; }

    [StructLayout(LayoutKind.Sequential)]
    private struct BITMAPINFOHEADER
    {
        public uint biSize;
        public int biWidth, biHeight;
        public ushort biPlanes, biBitCount;
        public uint biCompression, biSizeImage;
        public int biXPelsPerMeter, biYPelsPerMeter;
        public uint biClrUsed, biClrImportant;
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)] private static extern ushort RegisterClassEx(ref WNDCLASSEX wc);
    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)] private static extern IntPtr CreateWindowEx(int exStyle, string className, string windowName, uint style, int x, int y, int width, int height, IntPtr parent, IntPtr menu, IntPtr instance, IntPtr param);
    [DllImport("user32.dll")] private static extern IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    [DllImport("user32.dll")] private static extern bool PeekMessage(out MSG msg, IntPtr hWnd, uint filterMin, uint filterMax, uint remove);
    [DllImport("user32.dll")] private static extern bool TranslateMessage(ref MSG msg);
    [DllImport("user32.dll")] private static extern IntPtr DispatchMessage(ref MSG msg);
    [DllImport("user32.dll")] private static extern uint MsgWaitForMultipleObjectsEx(uint count, IntPtr[] handles, uint milliseconds, uint wakeMask, uint flags);
    [DllImport("user32.dll")] private static extern IntPtr LoadCursor(IntPtr instance, IntPtr cursorName);
    [DllImport("user32.dll")] private static extern IntPtr SetCursor(IntPtr cursor);
    [DllImport("user32.dll")] private static extern IntPtr GetDC(IntPtr hWnd);
    [DllImport("user32.dll")] private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
    [DllImport("user32.dll")] private static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref POINT pptDst, ref SIZE psize, IntPtr hdcSrc, ref POINT pprSrc, int crKey, ref BLENDFUNCTION pblend, int dwFlags);
    [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr hWnd, int cmdShow);
    [DllImport("user32.dll")] private static extern bool SetWindowPos(IntPtr hWnd, IntPtr insertAfter, int x, int y, int cx, int cy, uint flags);
    [DllImport("user32.dll")] private static extern IntPtr GetWindow(IntPtr hWnd, uint cmd);
    [DllImport("user32.dll")] private static extern int GetWindowLong(IntPtr hWnd, int index);
    [DllImport("user32.dll")] private static extern int SetWindowLong(IntPtr hWnd, int index, int newLong);
    [DllImport("user32.dll")] private static extern IntPtr SetCapture(IntPtr hWnd);
    [DllImport("user32.dll")] private static extern bool ReleaseCapture();
    [DllImport("user32.dll")] private static extern bool PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)] private static extern IntPtr GetModuleHandle(string? moduleName);
    [DllImport("kernel32.dll")] private static extern IntPtr OpenProcess(uint access, bool inherit, int processId);
    [DllImport("kernel32.dll")] private static extern bool CloseHandle(IntPtr handle);
    [DllImport("advapi32.dll")] private static extern bool OpenProcessToken(IntPtr process, uint access, out IntPtr token);
    [DllImport("advapi32.dll")] private static extern bool GetTokenInformation(IntPtr token, int infoClass, out uint info, uint length, out uint returnLength);
    [DllImport("gdi32.dll")] private static extern IntPtr CreateCompatibleDC(IntPtr hdc);
    [DllImport("gdi32.dll")] private static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFOHEADER pbmi, uint usage, out IntPtr ppvBits, IntPtr hSection, uint offset);
    [DllImport("gdi32.dll")] private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);
    [DllImport("gdi32.dll")] private static extern bool DeleteObject(IntPtr hObject);

    private static MemoryMappedViewAccessor _view = null!;
    private static int _lastSeq = -1;
    private static bool _visible;
    private static bool _clickThrough = true;
    private static bool _dragging;
    private static bool _needRecreate;
    private static bool _uiAccess;
    private static int _x, _y, _w, _h;
    private static IntPtr _hwnd, _memDc, _dib, _oldBmp, _bits;
    private static int _dibW, _dibH;

    private static void Main()
    {
        using var mutex = new Mutex(true, "GHelperOverlaySingleton", out bool created);
        if (!created) return;

        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            Logger.WriteLine("FATAL: " + (e.ExceptionObject as Exception));

        MemoryMappedFile? mmf = null;
        for (int i = 0; i < 40 && mmf is null; i++)
        {
            try { mmf = MemoryMappedFile.OpenExisting(OverlayIpc.MapName); }
            catch (FileNotFoundException) { Thread.Sleep(250); }
        }
        if (mmf is null)
        {
            Logger.WriteLine("Frame buffer not found (renderer is started by GHelper) - exiting");
            return;
        }

        using var mmfHolder = mmf;
        using var view = mmf.CreateViewAccessor();
        _view = view;

        if (view.ReadInt32(OverlayIpc.OffMagic) != OverlayIpc.Magic)
        {
            Logger.WriteLine("Frame buffer magic mismatch - exiting");
            return;
        }

        int appPid = view.ReadInt32(OverlayIpc.OffAppPid);
        IntPtr hApp = OpenProcess(SYNCHRONIZE, false, appPid);
        if (hApp == IntPtr.Zero)
        {
            Logger.WriteLine($"GHelper (pid {appPid}) is gone - exiting");
            return;
        }

        using var frameEvent = new EventWaitHandle(false, EventResetMode.AutoReset, OverlayIpc.FrameEventName);

        var wc = new WNDCLASSEX
        {
            cbSize = (uint)Marshal.SizeOf<WNDCLASSEX>(),
            lpfnWndProc = Marshal.GetFunctionPointerForDelegate(_wndProc),
            hInstance = GetModuleHandle(null),
            lpszClassName = "GHelperOverlayWnd",
        };
        if (RegisterClassEx(ref wc) == 0)
        {
            Logger.WriteLine("RegisterClassEx failed: " + Marshal.GetLastWin32Error());
            return;
        }

        _uiAccess = HasUiAccess();
        CreateOverlayWindow();
        Logger.WriteLine($"Renderer started (app pid {appPid}, uiAccess {_uiAccess})");

        // With uiAccess the window lives in the UIAccess z-band that normal topmost
        // windows can't reach, so no periodic topmost re-assert (or any wakeup) is
        // needed; without it, fall back to a 500 ms tick to fight the topmost war.
        var handles = new[] { frameEvent.SafeWaitHandle.DangerousGetHandle(), hApp };
        while (true)
        {
            uint r = MsgWaitForMultipleObjectsEx(2, handles, _uiAccess ? INFINITE : 500, QS_ALLINPUT, MWMO_INPUTAVAILABLE);
            if (r == 1 || r == WAIT_FAILED) break; // app gone
            if (r == 2 && !Pump()) break;          // window messages
            Apply();
        }
    }

    private static bool HasUiAccess()
    {
        if (!OpenProcessToken((IntPtr)(-1), TOKEN_QUERY, out IntPtr token)) return false;
        try
        {
            return GetTokenInformation(token, TokenUIAccess, out uint ui, 4, out _) && ui != 0;
        }
        finally
        {
            CloseHandle(token);
        }
    }

    private static bool Pump()
    {
        while (PeekMessage(out MSG msg, IntPtr.Zero, 0, 0, PM_REMOVE))
        {
            if (msg.message == WM_QUIT) return false;
            TranslateMessage(ref msg);
            DispatchMessage(ref msg);
        }
        return true;
    }

    private static void CreateOverlayWindow()
    {
        _hwnd = CreateWindowEx(
            WS_EX_TOPMOST | WS_EX_TOOLWINDOW | WS_EX_LAYERED | WS_EX_NOACTIVATE | (_clickThrough ? WS_EX_TRANSPARENT : 0),
            "GHelperOverlayWnd", "GHelperOverlay", WS_POPUP,
            _x, _y, Math.Max(_w, 1), Math.Max(_h, 1),
            IntPtr.Zero, IntPtr.Zero, GetModuleHandle(null), IntPtr.Zero);
        if (_hwnd == IntPtr.Zero)
            Logger.WriteLine("CreateWindowEx failed: " + Marshal.GetLastWin32Error());
    }

    private static void Apply()
    {
        if (_needRecreate)
        {
            _needRecreate = false;
            _visible = false;
            _lastSeq = -1;
            CreateOverlayWindow();
        }
        if (_hwnd == IntPtr.Zero) return;

        int seq = _view.ReadInt32(OverlayIpc.OffSeq);
        int x = _view.ReadInt32(OverlayIpc.OffX);
        int y = _view.ReadInt32(OverlayIpc.OffY);
        bool visible = _view.ReadInt32(OverlayIpc.OffVisible) != 0;
        bool clickThrough = _view.ReadInt32(OverlayIpc.OffClickThrough) != 0;

        if (clickThrough != _clickThrough)
        {
            _clickThrough = clickThrough;
            int style = GetWindowLong(_hwnd, GWL_EXSTYLE);
            SetWindowLong(_hwnd, GWL_EXSTYLE, clickThrough ? style | WS_EX_TRANSPARENT : style & ~WS_EX_TRANSPARENT);
        }

        if (seq != _lastSeq)
        {
            for (int retry = 0; retry < 3; retry++)
            {
                int w = _view.ReadInt32(OverlayIpc.OffWidth);
                int h = _view.ReadInt32(OverlayIpc.OffHeight);
                if (w <= 0 || h <= 0 || w > OverlayIpc.MaxWidth || h > OverlayIpc.MaxHeight) break;
                CopyPixels(w, h);
                int after = _view.ReadInt32(OverlayIpc.OffSeq);
                if (after == seq)
                {
                    _w = w;
                    _h = h;
                    _lastSeq = seq;
                    Blit(x, y);
                    break;
                }
                seq = after;
            }
        }
        else if ((x != _x || y != _y) && _lastSeq >= 0)
        {
            _x = x;
            _y = y;
            SetWindowPos(_hwnd, IntPtr.Zero, x, y, 0, 0, SWP_NOSIZE | SWP_NOACTIVATE | SWP_NOZORDER);
        }

        if (visible != _visible)
        {
            _visible = visible;
            ShowWindow(_hwnd, visible ? SW_SHOWNOACTIVATE : SW_HIDE);
        }

        if (!_uiAccess && _visible && GetWindow(_hwnd, GW_HWNDPREV) != IntPtr.Zero)
            SetWindowPos(_hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
    }

    private static void CopyPixels(int w, int h)
    {
        EnsureDib(w, h);
        if (_bits == IntPtr.Zero) return;
        byte* src = null;
        _view.SafeMemoryMappedViewHandle.AcquirePointer(ref src);
        try
        {
            Buffer.MemoryCopy(src + OverlayIpc.OffPixels, (void*)_bits, (long)_dibW * _dibH * 4, (long)w * h * 4);
        }
        finally
        {
            _view.SafeMemoryMappedViewHandle.ReleasePointer();
        }
    }

    private static void EnsureDib(int w, int h)
    {
        if (w == _dibW && h == _dibH && _dib != IntPtr.Zero) return;
        if (_memDc == IntPtr.Zero) _memDc = CreateCompatibleDC(IntPtr.Zero);
        if (_dib != IntPtr.Zero)
        {
            SelectObject(_memDc, _oldBmp);
            DeleteObject(_dib);
            _dib = IntPtr.Zero;
        }
        var bmi = new BITMAPINFOHEADER
        {
            biSize = (uint)Marshal.SizeOf<BITMAPINFOHEADER>(),
            biWidth = w,
            biHeight = -h,
            biPlanes = 1,
            biBitCount = 32,
        };
        _dib = CreateDIBSection(_memDc, ref bmi, 0, out _bits, IntPtr.Zero, 0);
        if (_dib == IntPtr.Zero) { _bits = IntPtr.Zero; _dibW = _dibH = 0; return; }
        _oldBmp = SelectObject(_memDc, _dib);
        _dibW = w;
        _dibH = h;
    }

    private static void Blit(int x, int y)
    {
        _x = x;
        _y = y;
        IntPtr screen = GetDC(IntPtr.Zero);
        var size = new SIZE { cx = _w, cy = _h };
        var dst = new POINT { x = x, y = y };
        var src = new POINT();
        var blend = new BLENDFUNCTION { SourceConstantAlpha = 255, AlphaFormat = 1 };
        UpdateLayeredWindow(_hwnd, screen, ref dst, ref size, _memDc, ref src, 0, ref blend, 2);
        ReleaseDC(IntPtr.Zero, screen);
    }

    private static IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        switch (msg)
        {
            case WM_LBUTTONDOWN:
                _dragging = true;
                SetCapture(hWnd);
                Forward(OverlayIpc.MsgLButtonDown, wParam, lParam);
                return IntPtr.Zero;
            case WM_MOUSEMOVE:
                Forward(OverlayIpc.MsgMouseMove, wParam, lParam);
                return IntPtr.Zero;
            case WM_LBUTTONUP:
                _dragging = false;
                ReleaseCapture();
                Forward(OverlayIpc.MsgLButtonUp, wParam, lParam);
                return IntPtr.Zero;
            case WM_MOUSEWHEEL:
                Forward(OverlayIpc.MsgMouseWheel, wParam, lParam);
                return IntPtr.Zero;
            case WM_MBUTTONDOWN:
                Forward(OverlayIpc.MsgMButtonDown, wParam, lParam);
                return IntPtr.Zero;
            case WM_SETCURSOR:
                SetCursor(LoadCursor(IntPtr.Zero, (IntPtr)(_dragging ? IDC_SIZEALL : IDC_HAND)));
                return (IntPtr)1;
            case WM_NCDESTROY:
                _needRecreate = true;
                break;
        }
        return DefWindowProc(hWnd, msg, wParam, lParam);
    }

    private static void Forward(int msg, IntPtr wParam, IntPtr lParam)
    {
        long hwnd = _view.ReadInt64(OverlayIpc.OffInputHwnd);
        if (hwnd != 0) PostMessage((IntPtr)hwnd, msg, wParam, lParam);
    }
}
