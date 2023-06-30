using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace GHelper.Helpers
{

    public class OSDNativeForm : NativeWindow, IDisposable
    {

        private bool _disposed = false;
        private byte _alpha = 250;
        private Size _size = new Size(350, 50);
        private Point _location = new Point(50, 50);


        protected virtual void PerformPaint(PaintEventArgs e)
        {
        }

        protected internal void Invalidate()
        {
            UpdateLayeredWindow();
        }
        private void UpdateLayeredWindow()
        {
            Bitmap bitmap1 = new Bitmap(Size.Width, Size.Height, PixelFormat.Format32bppArgb);
            using (Graphics graphics1 = Graphics.FromImage(bitmap1))
            {
                Rectangle rectangle1;
                SIZE size1;
                POINT point1;
                POINT point2;
                BLENDFUNCTION blendfunction1;
                rectangle1 = new Rectangle(0, 0, Size.Width, Size.Height);
                PerformPaint(new PaintEventArgs(graphics1, rectangle1));
                nint ptr1 = User32.GetDC(nint.Zero);
                nint ptr2 = Gdi32.CreateCompatibleDC(ptr1);
                nint ptr3 = bitmap1.GetHbitmap(Color.FromArgb(0));
                nint ptr4 = Gdi32.SelectObject(ptr2, ptr3);
                size1.cx = Size.Width;
                size1.cy = Size.Height;
                point1.x = Location.X;
                point1.x = Location.X;
                point1.y = Location.Y;
                point2.x = 0;
                point2.y = 0;
                blendfunction1 = new BLENDFUNCTION();
                blendfunction1.BlendOp = 0;
                blendfunction1.BlendFlags = 0;
                blendfunction1.SourceConstantAlpha = _alpha;
                blendfunction1.AlphaFormat = 1;
                User32.UpdateLayeredWindow(Handle, ptr1, ref point1, ref size1, ptr2, ref point2, 0, ref blendfunction1, 2); //2=ULW_ALPHA
                Gdi32.SelectObject(ptr2, ptr4);
                User32.ReleaseDC(nint.Zero, ptr1);
                Gdi32.DeleteObject(ptr3);
                Gdi32.DeleteDC(ptr2);
            }
        }

        public virtual void Show()
        {
            if (Handle == nint.Zero) //if handle don't equal to zero - window was created and just hided
                CreateWindowOnly();
            User32.ShowWindow(Handle, User32.SW_SHOWNOACTIVATE);
        }


        public virtual void Hide()
        {
            if (Handle == nint.Zero)
                return;
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

            CreateParams params1 = new CreateParams();
            params1.Caption = "FloatingNativeWindow";
            int nX = _location.X;
            int nY = _location.Y;
            Screen screen1 = Screen.FromHandle(Handle);
            if (nX + _size.Width > screen1.Bounds.Width)
            {
                nX = screen1.Bounds.Width - _size.Width;
            }
            if (nY + _size.Height > screen1.Bounds.Height)
            {
                nY = screen1.Bounds.Height - _size.Height;
            }
            _location = new Point(nX, nY);
            Size size1 = _size;
            Point point1 = _location;
            params1.X = nX;
            params1.Y = nY;
            params1.Height = size1.Height;
            params1.Width = size1.Width;
            params1.Parent = nint.Zero;
            uint ui = User32.WS_POPUP;
            params1.Style = (int)ui;
            params1.ExStyle = User32.WS_EX_TOPMOST | User32.WS_EX_TOOLWINDOW | User32.WS_EX_LAYERED | User32.WS_EX_NOACTIVATE | User32.WS_EX_TRANSPARENT;
            CreateHandle(params1);
            UpdateLayeredWindow();
        }



        protected virtual void SetBoundsCore(int x, int y, int width, int height)
        {
            if (X != x || Y != y || Width != width || Height != height)
            {
                if (Handle != nint.Zero)
                {
                    int num1 = 20;
                    if (X == x && Y == y)
                    {
                        num1 |= 2;
                    }
                    if (Width == width && Height == height)
                    {
                        num1 |= 1;
                    }
                    User32.SetWindowPos(Handle, nint.Zero, x, y, width, height, (uint)num1);
                }
                else
                {
                    Location = new Point(x, y);
                    Size = new Size(width, height);
                }
            }
        }




        #region #  Properties  #
        /// <summary>
        /// Get or set position of top-left corner of floating native window in screen coordinates
        /// </summary>
        public virtual Point Location
        {
            get { return _location; }
            set
            {
                if (Handle != nint.Zero)
                {
                    SetBoundsCore(value.X, value.Y, _size.Width, _size.Height);
                    RECT rect = new RECT();
                    User32.GetWindowRect(Handle, ref rect);
                    _location = new Point(rect.left, rect.top);
                    UpdateLayeredWindow();
                }
                else
                {
                    _location = value;
                }
            }
        }
        /// <summary>
        /// Get or set size of client area of floating native window
        /// </summary>
        public virtual Size Size
        {
            get { return _size; }
            set
            {
                if (Handle != nint.Zero)
                {
                    SetBoundsCore(_location.X, _location.Y, value.Width, value.Height);
                    RECT rect = new RECT();
                    User32.GetWindowRect(Handle, ref rect);
                    _size = new Size(rect.right - rect.left, rect.bottom - rect.top);
                    UpdateLayeredWindow();
                }
                else
                {
                    _size = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the height of the floating native window
        /// </summary>
		public int Height
        {
            get { return _size.Height; }
            set
            {
                _size = new Size(_size.Width, value);
            }
        }
        /// <summary>
        /// Gets or sets the width of the floating native window
        /// </summary>
        public int Width
        {
            get { return _size.Width; }
            set
            {
                _size = new Size(value, _size.Height);
            }
        }
        /// <summary>
        /// Get or set x-coordinate of top-left corner of floating native window in screen coordinates
        /// </summary>
		public int X
        {
            get { return _location.X; }
            set
            {
                Location = new Point(value, Location.Y);
            }
        }
        /// <summary>
        /// Get or set y-coordinate of top-left corner of floating native window in screen coordinates
        /// </summary>
        public int Y
        {
            get { return _location.Y; }
            set
            {
                Location = new Point(Location.X, value);
            }
        }
        /// <summary>
        /// Get rectangle represented client area of floating native window in client coordinates(top-left corner always has coord. 0,0)
        /// </summary>
		public Rectangle Bound
        {
            get
            {
                return new Rectangle(new Point(0, 0), _size);
            }
        }
        /// <summary>
        /// Get or set full opacity(255) or full transparency(0) or any intermediate state for floating native window transparency
        /// </summary>
		public byte Alpha
        {
            get { return _alpha; }
            set
            {
                if (_alpha == value) return;
                _alpha = value;
                UpdateLayeredWindow();
            }
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                DestroyHandle();
                _disposed = true;
            }
        }
        #endregion
    }

    #region #  Win32  #

    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        public int x;
        public int y;
    }
    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }
    [StructLayout(LayoutKind.Sequential)]
    internal struct SIZE
    {
        public int cx;
        public int cy;
    }
    [StructLayout(LayoutKind.Sequential)]
    internal struct TRACKMOUSEEVENTS
    {
        public uint cbSize;
        public uint dwFlags;
        public nint hWnd;
        public uint dwHoverTime;
    }
    [StructLayout(LayoutKind.Sequential)]
    internal struct MSG
    {
        public nint hwnd;
        public int message;
        public nint wParam;
        public nint lParam;
        public int time;
        public int pt_x;
        public int pt_y;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct BLENDFUNCTION
    {
        public byte BlendOp;
        public byte BlendFlags;
        public byte SourceConstantAlpha;
        public byte AlphaFormat;
    }
    internal class User32
    {
        public const uint WS_POPUP = 0x80000000;
        public const int WS_EX_TOPMOST = 0x8;
        public const int WS_EX_TOOLWINDOW = 0x80;
        public const int WS_EX_LAYERED = 0x80000;
        public const int WS_EX_TRANSPARENT = 0x20;
        public const int WS_EX_NOACTIVATE = 0x08000000;
        public const int SW_SHOWNOACTIVATE = 4;
        public const int SW_HIDE = 0;
        public const uint AW_HOR_POSITIVE = 0x1;
        public const uint AW_HOR_NEGATIVE = 0x2;
        public const uint AW_VER_POSITIVE = 0x4;
        public const uint AW_VER_NEGATIVE = 0x8;
        public const uint AW_CENTER = 0x10;
        public const uint AW_HIDE = 0x10000;
        public const uint AW_ACTIVATE = 0x20000;
        public const uint AW_SLIDE = 0x40000;
        public const uint AW_BLEND = 0x80000;
        // Methods
        private User32()
        {
        }
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool AnimateWindow(nint hWnd, uint dwTime, uint dwFlags);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool ClientToScreen(nint hWnd, ref POINT pt);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool DispatchMessage(ref MSG msg);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool DrawFocusRect(nint hWnd, ref RECT rect);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern nint GetDC(nint hWnd);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern nint GetFocus();
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern ushort GetKeyState(int virtKey);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool GetMessage(ref MSG msg, int hWnd, uint wFilterMin, uint wFilterMax);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern nint GetParent(nint hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool GetClientRect(nint hWnd, [In, Out] ref RECT rect);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern int GetWindowLong(nint hWnd, int nIndex);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern nint GetWindow(nint hWnd, int cmd);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool GetWindowRect(nint hWnd, ref RECT rect);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool HideCaret(nint hWnd);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool InvalidateRect(nint hWnd, ref RECT rect, bool erase);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern nint LoadCursor(nint hInstance, uint cursor);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int MapWindowPoints(nint hWndFrom, nint hWndTo, [In, Out] ref RECT rect, int cPoints);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool MoveWindow(nint hWnd, int x, int y, int width, int height, bool repaint);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool PeekMessage(ref MSG msg, int hWnd, uint wFilterMin, uint wFilterMax, uint wFlag);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool PostMessage(nint hWnd, int Msg, uint wParam, uint lParam);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool ReleaseCapture();
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern int ReleaseDC(nint hWnd, nint hDC);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool ScreenToClient(nint hWnd, ref POINT pt);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern uint SendMessage(nint hWnd, int Msg, uint wParam, uint lParam);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern nint SetCursor(nint hCursor);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern nint SetFocus(nint hWnd);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern int SetWindowLong(nint hWnd, int nIndex, int newLong);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern int SetWindowPos(nint hWnd, nint hWndAfter, int X, int Y, int Width, int Height, uint flags);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool SetWindowRgn(nint hWnd, nint hRgn, bool redraw);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool ShowCaret(nint hWnd);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool SetCapture(nint hWnd);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern int ShowWindow(nint hWnd, short cmdShow);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref int bRetValue, uint fWinINI);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool TrackMouseEvent(ref TRACKMOUSEEVENTS tme);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool TranslateMessage(ref MSG msg);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool UpdateLayeredWindow(nint hwnd, nint hdcDst, ref POINT pptDst, ref SIZE psize, nint hdcSrc, ref POINT pprSrc, int crKey, ref BLENDFUNCTION pblend, int dwFlags);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool UpdateWindow(nint hwnd);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool WaitMessage();
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool AdjustWindowRectEx(ref RECT lpRect, int dwStyle, bool bMenu, int dwExStyle);
    }

    internal class Gdi32
    {
        // Methods
        private Gdi32()
        {
        }
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        internal static extern int CombineRgn(nint dest, nint src1, nint src2, int flags);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        internal static extern nint CreateBrushIndirect(ref LOGBRUSH brush);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        internal static extern nint CreateCompatibleDC(nint hDC);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        internal static extern nint CreateRectRgnIndirect(ref RECT rect);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        internal static extern bool DeleteDC(nint hDC);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        internal static extern nint DeleteObject(nint hObject);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        internal static extern int GetClipBox(nint hDC, ref RECT rectBox);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        internal static extern bool PatBlt(nint hDC, int x, int y, int width, int height, uint flags);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        internal static extern int SelectClipRgn(nint hDC, nint hRgn);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        internal static extern nint SelectObject(nint hDC, nint hObject);
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct LOGBRUSH
    {
        public uint lbStyle;
        public uint lbColor;
        public uint lbHatch;
    }

    #endregion
}
