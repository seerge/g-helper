using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace OSD
{

    class OSDNativeForm : NativeWindow, IDisposable
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
            this.UpdateLayeredWindow();
        }
        private void UpdateLayeredWindow()
        {
            Bitmap bitmap1 = new Bitmap(this.Size.Width, this.Size.Height, PixelFormat.Format32bppArgb);
            using (Graphics graphics1 = Graphics.FromImage(bitmap1))
            {
                Rectangle rectangle1;
                SIZE size1;
                POINT point1;
                POINT point2;
                BLENDFUNCTION blendfunction1;
                rectangle1 = new Rectangle(0, 0, this.Size.Width, this.Size.Height);
                PerformPaint(new PaintEventArgs(graphics1, rectangle1));
                IntPtr ptr1 = User32.GetDC(IntPtr.Zero);
                IntPtr ptr2 = Gdi32.CreateCompatibleDC(ptr1);
                IntPtr ptr3 = bitmap1.GetHbitmap(Color.FromArgb(0));
                IntPtr ptr4 = Gdi32.SelectObject(ptr2, ptr3);
                size1.cx = this.Size.Width;
                size1.cy = this.Size.Height;
                point1.x = this.Location.X;
                point1.x = this.Location.X;
                point1.y = this.Location.Y;
                point2.x = 0;
                point2.y = 0;
                blendfunction1 = new BLENDFUNCTION();
                blendfunction1.BlendOp = 0;
                blendfunction1.BlendFlags = 0;
                blendfunction1.SourceConstantAlpha = this._alpha;
                blendfunction1.AlphaFormat = 1;
                User32.UpdateLayeredWindow(base.Handle, ptr1, ref point1, ref size1, ptr2, ref point2, 0, ref blendfunction1, 2); //2=ULW_ALPHA
                Gdi32.SelectObject(ptr2, ptr4);
                User32.ReleaseDC(IntPtr.Zero, ptr1);
                Gdi32.DeleteObject(ptr3);
                Gdi32.DeleteDC(ptr2);
            }
        }

        public virtual void Show()
        {
            if (base.Handle == IntPtr.Zero) //if handle don't equal to zero - window was created and just hided
                this.CreateWindowOnly();
            User32.ShowWindow(base.Handle, User32.SW_SHOWNOACTIVATE);
        }


        public virtual void Hide()
        {
            if (base.Handle == IntPtr.Zero)
                return;
            User32.ShowWindow(base.Handle, User32.SW_HIDE);
            this.DestroyHandle();
        }


        public virtual void Close()
        {
            this.Hide();
            this.Dispose();
        }

        private void CreateWindowOnly()
        {

            CreateParams params1 = new CreateParams();
            params1.Caption = "FloatingNativeWindow";
            int nX = this._location.X;
            int nY = this._location.Y;
            Screen screen1 = Screen.FromHandle(base.Handle);
            if ((nX + this._size.Width) > screen1.Bounds.Width)
            {
                nX = screen1.Bounds.Width - this._size.Width;
            }
            if ((nY + this._size.Height) > screen1.Bounds.Height)
            {
                nY = screen1.Bounds.Height - this._size.Height;
            }
            this._location = new Point(nX, nY);
            Size size1 = this._size;
            Point point1 = this._location;
            params1.X = nX;
            params1.Y = nY;
            params1.Height = size1.Height;
            params1.Width = size1.Width;
            params1.Parent = IntPtr.Zero;
            uint ui = User32.WS_POPUP;
            params1.Style = (int)ui;
            params1.ExStyle = User32.WS_EX_TOPMOST | User32.WS_EX_TOOLWINDOW | User32.WS_EX_LAYERED | User32.WS_EX_NOACTIVATE | User32.WS_EX_TRANSPARENT;
            this.CreateHandle(params1);
            this.UpdateLayeredWindow();
        }



        protected virtual void SetBoundsCore(int x, int y, int width, int height)
        {
            if (((this.X != x) || (this.Y != y)) || ((this.Width != width) || (this.Height != height)))
            {
                if (base.Handle != IntPtr.Zero)
                {
                    int num1 = 20;
                    if ((this.X == x) && (this.Y == y))
                    {
                        num1 |= 2;
                    }
                    if ((this.Width == width) && (this.Height == height))
                    {
                        num1 |= 1;
                    }
                    User32.SetWindowPos(base.Handle, IntPtr.Zero, x, y, width, height, (uint)num1);
                }
                else
                {
                    this.Location = new Point(x, y);
                    this.Size = new Size(width, height);
                }
            }
        }




        #region #  Properties  #
        /// <summary>
        /// Get or set position of top-left corner of floating native window in screen coordinates
        /// </summary>
        public virtual Point Location
        {
            get { return this._location; }
            set
            {
                if (base.Handle != IntPtr.Zero)
                {
                    this.SetBoundsCore(value.X, value.Y, this._size.Width, this._size.Height);
                    RECT rect = new RECT();
                    User32.GetWindowRect(base.Handle, ref rect);
                    this._location = new Point(rect.left, rect.top);
                    this.UpdateLayeredWindow();
                }
                else
                {
                    this._location = value;
                }
            }
        }
        /// <summary>
        /// Get or set size of client area of floating native window
        /// </summary>
        public virtual Size Size
        {
            get { return this._size; }
            set
            {
                if (base.Handle != IntPtr.Zero)
                {
                    this.SetBoundsCore(this._location.X, this._location.Y, value.Width, value.Height);
                    RECT rect = new RECT();
                    User32.GetWindowRect(base.Handle, ref rect);
                    this._size = new Size(rect.right - rect.left, rect.bottom - rect.top);
                    this.UpdateLayeredWindow();
                }
                else
                {
                    this._size = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the height of the floating native window
        /// </summary>
		public int Height
        {
            get { return this._size.Height; }
            set
            {
                this._size = new Size(this._size.Width, value);
            }
        }
        /// <summary>
        /// Gets or sets the width of the floating native window
        /// </summary>
        public int Width
        {
            get { return this._size.Width; }
            set
            {
                this._size = new Size(value, this._size.Height);
            }
        }
        /// <summary>
        /// Get or set x-coordinate of top-left corner of floating native window in screen coordinates
        /// </summary>
		public int X
        {
            get { return this._location.X; }
            set
            {
                this.Location = new Point(value, this.Location.Y);
            }
        }
        /// <summary>
        /// Get or set y-coordinate of top-left corner of floating native window in screen coordinates
        /// </summary>
        public int Y
        {
            get { return this._location.Y; }
            set
            {
                this.Location = new Point(this.Location.X, value);
            }
        }
        /// <summary>
        /// Get rectangle represented client area of floating native window in client coordinates(top-left corner always has coord. 0,0)
        /// </summary>
		public Rectangle Bound
        {
            get
            {
                return new Rectangle(new Point(0, 0), this._size);
            }
        }
        /// <summary>
        /// Get or set full opacity(255) or full transparency(0) or any intermediate state for floating native window transparency
        /// </summary>
		public byte Alpha
        {
            get { return this._alpha; }
            set
            {
                if (this._alpha == value) return;
                this._alpha = value;
                this.UpdateLayeredWindow();
            }
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                this.DestroyHandle();
                this._disposed = true;
            }
        }
        #endregion
    }

    #region #  Win32  #
    internal struct PAINTSTRUCT
    {
        public IntPtr hdc;
        public int fErase;
        public Rectangle rcPaint;
        public int fRestore;
        public int fIncUpdate;
        public int Reserved1;
        public int Reserved2;
        public int Reserved3;
        public int Reserved4;
        public int Reserved5;
        public int Reserved6;
        public int Reserved7;
        public int Reserved8;
    }
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
        public IntPtr hWnd;
        public uint dwHoverTime;
    }
    [StructLayout(LayoutKind.Sequential)]
    internal struct MSG
    {
        public IntPtr hwnd;
        public int message;
        public IntPtr wParam;
        public IntPtr lParam;
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
        internal static extern bool AnimateWindow(IntPtr hWnd, uint dwTime, uint dwFlags);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr BeginPaint(IntPtr hWnd, ref PAINTSTRUCT ps);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool ClientToScreen(IntPtr hWnd, ref POINT pt);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool DispatchMessage(ref MSG msg);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool DrawFocusRect(IntPtr hWnd, ref RECT rect);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT ps);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetFocus();
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern ushort GetKeyState(int virtKey);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool GetMessage(ref MSG msg, int hWnd, uint wFilterMin, uint wFilterMax);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetParent(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool GetClientRect(IntPtr hWnd, [In, Out] ref RECT rect);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetWindow(IntPtr hWnd, int cmd);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool GetWindowRect(IntPtr hWnd, ref RECT rect);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool HideCaret(IntPtr hWnd);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool InvalidateRect(IntPtr hWnd, ref RECT rect, bool erase);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr LoadCursor(IntPtr hInstance, uint cursor);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, [In, Out] ref RECT rect, int cPoints);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool MoveWindow(IntPtr hWnd, int x, int y, int width, int height, bool repaint);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool PeekMessage(ref MSG msg, int hWnd, uint wFilterMin, uint wFilterMax, uint wFlag);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool PostMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool ReleaseCapture();
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool ScreenToClient(IntPtr hWnd, ref POINT pt);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern uint SendMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SetCursor(IntPtr hCursor);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SetFocus(IntPtr hWnd);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int newLong);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndAfter, int X, int Y, int Width, int Height, uint flags);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool redraw);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool ShowCaret(IntPtr hWnd);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool SetCapture(IntPtr hWnd);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern int ShowWindow(IntPtr hWnd, short cmdShow);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref int bRetValue, uint fWinINI);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool TrackMouseEvent(ref TRACKMOUSEEVENTS tme);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool TranslateMessage(ref MSG msg);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref POINT pptDst, ref SIZE psize, IntPtr hdcSrc, ref POINT pprSrc, int crKey, ref BLENDFUNCTION pblend, int dwFlags);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool UpdateWindow(IntPtr hwnd);
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
        internal static extern int CombineRgn(IntPtr dest, IntPtr src1, IntPtr src2, int flags);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr CreateBrushIndirect(ref LOGBRUSH brush);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr CreateCompatibleDC(IntPtr hDC);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr CreateRectRgnIndirect(ref RECT rect);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        internal static extern bool DeleteDC(IntPtr hDC);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr DeleteObject(IntPtr hObject);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        internal static extern int GetClipBox(IntPtr hDC, ref RECT rectBox);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        internal static extern bool PatBlt(IntPtr hDC, int x, int y, int width, int height, uint flags);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        internal static extern int SelectClipRgn(IntPtr hDC, IntPtr hRgn);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
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
