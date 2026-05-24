using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace GHelperOverlay;

public class OSDNativeForm : NativeWindow, IDisposable
{
    private bool _disposed;
    private byte _alpha = 250;
    private Size _size = new(350, 50);
    private Point _location = new(50, 50);
    private readonly object _paintLock = new();

    protected virtual void PerformPaint(PaintEventArgs e) { }

    protected internal void Invalidate() => UpdateLayeredWindow();

    private void UpdateLayeredWindow()
    {
        if (!Monitor.TryEnter(_paintLock)) return;
        try
        {
            using Bitmap bitmap = new(Size.Width, Size.Height, PixelFormat.Format32bppArgb);
            using Graphics graphics = Graphics.FromImage(bitmap);

            PerformPaint(new PaintEventArgs(graphics, new Rectangle(0, 0, Size.Width, Size.Height)));

            nint screenDc = User32.GetDC(nint.Zero);
            nint memDc = Gdi32.CreateCompatibleDC(screenDc);
            nint hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
            nint oldBitmap = Gdi32.SelectObject(memDc, hBitmap);

            SIZE size = new() { cx = Size.Width, cy = Size.Height };
            POINT topLeft = new() { x = Location.X, y = Location.Y };
            POINT srcOrigin = new() { x = 0, y = 0 };
            BLENDFUNCTION blend = new()
            {
                BlendOp = 0,
                BlendFlags = 0,
                SourceConstantAlpha = _alpha,
                AlphaFormat = 1,
            };

            User32.UpdateLayeredWindow(Handle, screenDc, ref topLeft, ref size, memDc, ref srcOrigin, 0, ref blend, 2);

            Gdi32.SelectObject(memDc, oldBitmap);
            User32.ReleaseDC(nint.Zero, screenDc);
            Gdi32.DeleteObject(hBitmap);
            Gdi32.DeleteDC(memDc);
        }
        finally { Monitor.Exit(_paintLock); }
    }

    public virtual void Show()
    {
        if (Handle == nint.Zero) CreateWindowOnly();
        User32.ShowWindow(Handle, User32.SW_SHOWNOACTIVATE);
    }

    public virtual void Hide()
    {
        if (Handle == nint.Zero) return;
        User32.ShowWindow(Handle, User32.SW_HIDE);
        DestroyHandle();
    }

    public virtual void Close()
    {
        Hide();
        Dispose();
    }

    private void CreateWindowOnly()
    {
        CreateParams p = new()
        {
            Caption = "FloatingNativeWindow",
            X = _location.X,
            Y = _location.Y,
            Width = _size.Width,
            Height = _size.Height,
            Parent = nint.Zero,
            Style = unchecked((int)User32.WS_POPUP),
            ExStyle = User32.WS_EX_TOPMOST | User32.WS_EX_TOOLWINDOW | User32.WS_EX_LAYERED | User32.WS_EX_NOACTIVATE | User32.WS_EX_TRANSPARENT,
        };
        CreateHandle(p);
        UpdateLayeredWindow();
    }

    protected virtual void SetBoundsCore(int x, int y, int width, int height)
    {
        if (X == x && Y == y && Width == width && Height == height) return;
        if (Handle == nint.Zero)
        {
            Location = new Point(x, y);
            Size = new Size(width, height);
            return;
        }
        int flags = 20;
        if (X == x && Y == y) flags |= 2;
        if (Width == width && Height == height) flags |= 1;
        User32.SetWindowPos(Handle, nint.Zero, x, y, width, height, (uint)flags);
    }

    public virtual Point Location
    {
        get => _location;
        set
        {
            if (Handle != nint.Zero)
            {
                SetBoundsCore(value.X, value.Y, _size.Width, _size.Height);
                RECT r = new();
                User32.GetWindowRect(Handle, ref r);
                _location = new Point(r.left, r.top);
                UpdateLayeredWindow();
            }
            else _location = value;
        }
    }

    public virtual Size Size
    {
        get => _size;
        set
        {
            if (Handle != nint.Zero)
            {
                SetBoundsCore(_location.X, _location.Y, value.Width, value.Height);
                RECT r = new();
                User32.GetWindowRect(Handle, ref r);
                _size = new Size(r.right - r.left, r.bottom - r.top);
                UpdateLayeredWindow();
            }
            else _size = value;
        }
    }

    public int Height
    {
        get => _size.Height;
        set => _size = new Size(_size.Width, value);
    }

    public int Width
    {
        get => _size.Width;
        set => _size = new Size(value, _size.Height);
    }

    public int X
    {
        get => _location.X;
        set => Location = new Point(value, Location.Y);
    }

    public int Y
    {
        get => _location.Y;
        set => Location = new Point(Location.X, value);
    }

    public Rectangle Bound => new(new Point(0, 0), _size);

    public byte Alpha
    {
        get => _alpha;
        set
        {
            if (_alpha == value) return;
            _alpha = value;
            UpdateLayeredWindow();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool _)
    {
        if (_disposed) return;
        DestroyHandle();
        _disposed = true;
    }
}

[StructLayout(LayoutKind.Sequential)]
internal struct POINT { public int x; public int y; }
[StructLayout(LayoutKind.Sequential)]
internal struct RECT { public int left; public int top; public int right; public int bottom; }
[StructLayout(LayoutKind.Sequential)]
internal struct SIZE { public int cx; public int cy; }
[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct BLENDFUNCTION { public byte BlendOp; public byte BlendFlags; public byte SourceConstantAlpha; public byte AlphaFormat; }

internal static class User32
{
    public const uint WS_POPUP = 0x80000000;
    public const int WS_EX_TOPMOST = 0x8;
    public const int WS_EX_TOOLWINDOW = 0x80;
    public const int WS_EX_LAYERED = 0x80000;
    public const int WS_EX_TRANSPARENT = 0x20;
    public const int WS_EX_NOACTIVATE = 0x08000000;
    public const short SW_SHOWNOACTIVATE = 4;
    public const short SW_HIDE = 0;

    [DllImport("user32.dll")] public static extern nint GetDC(nint hWnd);
    [DllImport("user32.dll")] public static extern int ReleaseDC(nint hWnd, nint hDC);
    [DllImport("user32.dll")] public static extern int ShowWindow(nint hWnd, short cmdShow);
    [DllImport("user32.dll")] public static extern int SetWindowPos(nint hWnd, nint hWndAfter, int X, int Y, int Width, int Height, uint flags);
    [DllImport("user32.dll")] public static extern bool GetWindowRect(nint hWnd, ref RECT rect);
    [DllImport("user32.dll")] public static extern bool UpdateLayeredWindow(nint hwnd, nint hdcDst, ref POINT pptDst, ref SIZE psize, nint hdcSrc, ref POINT pprSrc, int crKey, ref BLENDFUNCTION pblend, int dwFlags);
}

internal static class Gdi32
{
    [DllImport("gdi32.dll")] public static extern nint CreateCompatibleDC(nint hDC);
    [DllImport("gdi32.dll")] public static extern bool DeleteDC(nint hDC);
    [DllImport("gdi32.dll")] public static extern nint DeleteObject(nint hObject);
    [DllImport("gdi32.dll")] public static extern nint SelectObject(nint hDC, nint hObject);
}
