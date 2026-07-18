using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace GHelperOverlay;

// Thin uiAccess renderer: GHelper composes overlay frames and writes them to a
// shared-memory buffer (see OverlayIpc); this process only blits them into a
// layered topmost window and forwards raw mouse input back. No sensors, no
// config, no logic - it exists solely because uiAccess (drawing above
// fullscreen games) requires a separate signed executable in Program Files.
internal static class Program
{
    [STAThread]
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

        using var view = mmf.CreateViewAccessor();
        if (view.ReadInt32(OverlayIpc.OffMagic) != OverlayIpc.Magic)
        {
            Logger.WriteLine("Frame buffer magic mismatch - exiting");
            return;
        }

        int appPid = view.ReadInt32(OverlayIpc.OffAppPid);
        try
        {
            var app = Process.GetProcessById(appPid);
            app.EnableRaisingEvents = true;
            app.Exited += (_, _) => Environment.Exit(0);
        }
        catch
        {
            Logger.WriteLine($"GHelper (pid {appPid}) is gone - exiting");
            return;
        }

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        using var frameEvent = new EventWaitHandle(false, EventResetMode.AutoReset, OverlayIpc.FrameEventName);

        var invoker = new Control();
        invoker.CreateControl();

        var window = new RenderWindow(view);
        Logger.WriteLine($"Renderer started (app pid {appPid})");

        var pump = new Thread(() =>
        {
            while (true)
            {
                frameEvent.WaitOne(500);
                try { invoker.BeginInvoke((MethodInvoker)window.Apply); }
                catch { return; }
            }
        })
        { IsBackground = true };
        pump.Start();

        window.Apply();
        Application.Run();
    }
}

internal sealed class RenderWindow : NativeWindow
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
    private const int WM_MOUSEMOVE = 0x0200;
    private const int WM_LBUTTONDOWN = 0x0201;
    private const int WM_LBUTTONUP = 0x0202;
    private const int WM_MBUTTONDOWN = 0x0207;
    private const int WM_MOUSEWHEEL = 0x020A;
    private const int WM_SETCURSOR = 0x0020;
    private const int WM_NCDESTROY = 0x0082;
    private const uint SWP_NOSIZE = 0x1;
    private const uint SWP_NOMOVE = 0x2;
    private const uint SWP_NOZORDER = 0x4;
    private const uint SWP_NOACTIVATE = 0x10;
    private const uint GW_HWNDPREV = 3;
    private static readonly IntPtr HWND_TOPMOST = new(-1);

    [DllImport("user32.dll")] private static extern IntPtr GetDC(IntPtr hWnd);
    [DllImport("user32.dll")] private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
    [DllImport("user32.dll")] private static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref POINT pptDst, ref SIZE psize, IntPtr hdcSrc, ref POINT pprSrc, int crKey, ref BLENDFUNCTION pblend, int dwFlags);
    [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    [DllImport("user32.dll")] private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint flags);
    [DllImport("user32.dll")] private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
    [DllImport("user32.dll")] private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    [DllImport("user32.dll")] private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    [DllImport("user32.dll")] private static extern bool SetCapture(IntPtr hWnd);
    [DllImport("user32.dll")] private static extern bool ReleaseCapture();
    [DllImport("user32.dll")] private static extern bool PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
    [DllImport("gdi32.dll")] private static extern IntPtr CreateCompatibleDC(IntPtr hdc);
    [DllImport("gdi32.dll")] private static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFOHEADER pbmi, uint usage, out IntPtr ppvBits, IntPtr hSection, uint offset);
    [DllImport("gdi32.dll")] private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);
    [DllImport("gdi32.dll")] private static extern bool DeleteObject(IntPtr hObject);

    [StructLayout(LayoutKind.Sequential)] private struct POINT { public int x, y; }
    [StructLayout(LayoutKind.Sequential)] private struct SIZE { public int cx, cy; }
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

    private readonly MemoryMappedViewAccessor _view;
    private int _lastSeq = -1;
    private bool _visible;
    private bool _clickThrough = true;
    private bool _dragging;
    private bool _needRecreate;
    private int _x, _y, _w, _h;
    private IntPtr _memDc, _dib, _oldBmp, _bits;
    private int _dibW, _dibH;

    public RenderWindow(MemoryMappedViewAccessor view)
    {
        _view = view;
        CreateOverlayWindow();
    }

    private void CreateOverlayWindow()
    {
        var cp = new CreateParams
        {
            Caption = "GHelperOverlay",
            X = _x,
            Y = _y,
            Width = Math.Max(_w, 1),
            Height = Math.Max(_h, 1),
            Style = unchecked((int)WS_POPUP),
            ExStyle = WS_EX_TOPMOST | WS_EX_TOOLWINDOW | WS_EX_LAYERED | WS_EX_NOACTIVATE
                    | (_clickThrough ? WS_EX_TRANSPARENT : 0),
        };
        CreateHandle(cp);
    }

    public void Apply()
    {
        if (_needRecreate)
        {
            _needRecreate = false;
            _visible = false;
            _lastSeq = -1;
            CreateOverlayWindow();
        }
        if (Handle == IntPtr.Zero) return;

        int seq = _view.ReadInt32(OverlayIpc.OffSeq);
        int x = _view.ReadInt32(OverlayIpc.OffX);
        int y = _view.ReadInt32(OverlayIpc.OffY);
        bool visible = _view.ReadInt32(OverlayIpc.OffVisible) != 0;
        bool clickThrough = _view.ReadInt32(OverlayIpc.OffClickThrough) != 0;

        if (clickThrough != _clickThrough)
        {
            _clickThrough = clickThrough;
            int style = GetWindowLong(Handle, GWL_EXSTYLE);
            SetWindowLong(Handle, GWL_EXSTYLE, clickThrough ? style | WS_EX_TRANSPARENT : style & ~WS_EX_TRANSPARENT);
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
            SetWindowPos(Handle, IntPtr.Zero, x, y, 0, 0, SWP_NOSIZE | SWP_NOACTIVATE | SWP_NOZORDER);
        }

        if (visible != _visible)
        {
            _visible = visible;
            ShowWindow(Handle, visible ? SW_SHOWNOACTIVATE : SW_HIDE);
        }

        if (_visible && GetWindow(Handle, GW_HWNDPREV) != IntPtr.Zero)
            SetWindowPos(Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
    }

    private unsafe void CopyPixels(int w, int h)
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

    private void EnsureDib(int w, int h)
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

    private void Blit(int x, int y)
    {
        _x = x;
        _y = y;
        IntPtr screen = GetDC(IntPtr.Zero);
        var size = new SIZE { cx = _w, cy = _h };
        var dst = new POINT { x = x, y = y };
        var src = new POINT();
        var blend = new BLENDFUNCTION { SourceConstantAlpha = 255, AlphaFormat = 1 };
        UpdateLayeredWindow(Handle, screen, ref dst, ref size, _memDc, ref src, 0, ref blend, 2);
        ReleaseDC(IntPtr.Zero, screen);
    }

    protected override void WndProc(ref Message m)
    {
        switch (m.Msg)
        {
            case WM_LBUTTONDOWN:
                _dragging = true;
                SetCapture(Handle);
                Forward(OverlayIpc.MsgLButtonDown, ref m);
                return;
            case WM_MOUSEMOVE:
                Forward(OverlayIpc.MsgMouseMove, ref m);
                return;
            case WM_LBUTTONUP:
                _dragging = false;
                ReleaseCapture();
                Forward(OverlayIpc.MsgLButtonUp, ref m);
                return;
            case WM_MOUSEWHEEL:
                Forward(OverlayIpc.MsgMouseWheel, ref m);
                return;
            case WM_MBUTTONDOWN:
                Forward(OverlayIpc.MsgMButtonDown, ref m);
                return;
            case WM_SETCURSOR:
                Cursor.Current = _dragging ? Cursors.SizeAll : Cursors.Hand;
                m.Result = (IntPtr)1;
                return;
            case WM_NCDESTROY:
                base.WndProc(ref m);
                _needRecreate = true;
                return;
        }
        base.WndProc(ref m);
    }

    private void Forward(int msg, ref Message m)
    {
        long hwnd = _view.ReadInt64(OverlayIpc.OffInputHwnd);
        if (hwnd != 0) PostMessage((IntPtr)hwnd, msg, m.WParam, m.LParam);
    }
}
