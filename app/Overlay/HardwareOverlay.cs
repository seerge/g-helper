using GHelper.Helpers;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace GHelper.Overlay
{
    public class HardwareOverlay : OSDNativeForm
    {
        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromPoint(POINT pt, uint dwFlags);
        [DllImport("shcore.dll")]
        private static extern int GetDpiForMonitor(IntPtr hMonitor, int dpiType, out uint dpiX, out uint dpiY);

        // Foreground window — used to pin FPS measurement to the active process
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        // Drag support
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll")]
        private static extern bool SetCapture(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        private const uint MONITOR_DEFAULTTOPRIMARY = 1;
        private const int MDT_EFFECTIVE_DPI = 0;
        private const int BaseDpi = 96;

        private const int GWL_EXSTYLE        = -20;
        private const int WS_EX_TRANSPARENT_FLAG = 0x00000020;
        private const int WM_LBUTTONDOWN     = 0x0201;
        private const int WM_MOUSEMOVE       = 0x0200;
        private const int WM_LBUTTONUP       = 0x0202;
        private const int VK_CONTROL         = 0x11;
        private const int VK_SHIFT           = 0x10;
        private const int VK_MENU            = 0x12; // ALT

        private bool _dragging;
        private Point _dragCursorStart;
        private Point _dragWindowStart;
        private bool _dragModeActive;
        private bool _lightMode;

        // ── Layout constants (base = 96 dpi) ─────────────────────────────────
        //
        // Full mode:
        // ┌─padX─┬─fpsCol─┬─gap─┬─── leftCol ─────┬─gap─┬─chartCol─┬─pwrGap─┬─powCol─┬─padX─┐
        // │      │  fps   │     │ GPU: 82° 5300RPM│     │  chart   │        │ 111.3W │      │
        // │      │        │     │ CPU: 78° 4500RPM│     │          │        │  16.9W │      │
        // └──────┴────────┴─────┴─────────────────┴─────┴──────────┴────────┴────────┴──────┘
        // BaseWidth = 8 + 52 + 8 + 128 + 8 + 120 + 4 + 50 + 8 = 386
        //
        // Light mode (no chart, no fan RPM — narrower left col fits "GPU: 82° "):
        // ┌─padX─┬─fpsCol─┬─gap─┬─lightCol─┬─pwrGap─┬─powCol─┬─padX─┐
        // │      │  fps   │     │ GPU: 82° │        │ 111.3W │      │
        // │      │        │     │ CPU: 78° │        │  16.9W │      │
        // └──────┴────────┴─────┴──────────┴────────┴────────┴──────┘
        // BaseLightWidth = 8 + 52 + 8 + 76 + 4 + 50 + 8 = 206
        //
        private const float BaseFontSize = 13f;
        private const float BaseRpmFontSize = 8.5f;
        private const int BaseLineHeight = 18;
        private const int BaseLineSpacing = 1;
        private const int BasePadX = 8;
        private const int BasePadY = 4;
        private const int BaseWidth = 386;
        private const int BaseFpsColWidth = 52;
        private const int BaseLeftColWidth = 128;
        private const int BaseChartColWidth = 120;
        private const int BasePowerGap = 4;
        private const int BasePowerColWidth = 50;
        private const int BaseColGap = 8;
        private const int CornerRadius = 3;
        private const int MarginFromEdge = 10;
        private const int BaseLightLeftColWidth = 76; // fits "GPU: 82° " (9 Consolas chars) with a little breathing room
        private const int BaseLightWidth = BasePadX + BaseFpsColWidth + BaseColGap + BaseLightLeftColWidth + BasePowerGap + BasePowerColWidth + BasePadX; // 206

        private static readonly SolidBrush _bgBrush = new(Color.FromArgb(128, 0, 0, 0));
        private static readonly SolidBrush _gpuBrush = new(Color.FromArgb(255, 0, 255, 80));
        private static readonly SolidBrush _cpuBrush = new(Color.FromArgb(255, 60, 220, 255));
        private static readonly Pen _gpuLinePen = new(Color.FromArgb(255, 0, 255, 80), 1.5f);
        private static readonly Pen _cpuLinePen = new(Color.FromArgb(255, 60, 220, 255), 1.5f);
        private static readonly SolidBrush _gpuFillBrush = new(Color.FromArgb(128, 0, 85, 27));
        private static readonly SolidBrush _cpuFillBrush = new(Color.FromArgb(128, 20, 73, 85));

        // Cached per-DPI drawing resources — recreated only when DPI scale changes
        private float _lastDpiScale = 0f;
        private Font? _font;
        private Font? _rpmFont;
        private Font? _fpsBold;
        private Pen? _totalPen;
        private Pen? _axPen;

        // Pre-allocated chart point arrays — reused every repaint to avoid GC pressure
        private readonly PointF[] _basePts = new PointF[HistoryLength];
        private readonly PointF[] _cpuPts  = new PointF[HistoryLength];
        private readonly PointF[] _gpuPts  = new PointF[HistoryLength];
        private readonly PointF[] _polyPts = new PointF[HistoryLength * 2];

        // _gpuTempStr includes a trailing space so fan number has breathing room
        private string _gpuTempStr = "";
        private string _cpuTempStr = "";
        private string _gpuFanNum = "";
        private string _cpuFanNum = "";
        private string _gpuPow = "";
        private string _cpuPow = "";

        private const int HistoryLength = 60;
        private readonly float[] _cpuHistory = new float[HistoryLength];
        private readonly float[] _gpuHistory = new float[HistoryLength];
        private int _historyHead = 0;

        private readonly System.Timers.Timer _timer = new(1000) { AutoReset = true };
        private EtwFpsMonitor? _fps;
        private Task? _fpsTask;
        private volatile int _currentFps;
        private int _lastFgPid;
        private bool _active;

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
        int x, int y, int w, int h, uint flags);
        private static readonly IntPtr HWND_TOPMOST = new(-1);
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOACTIVATE = 0x0010;

        public HardwareOverlay()
        {
            Alpha = 255;
            _timer.Elapsed += (_, _) => Tick();
        }

        private const int WM_NCDESTROY = 0x0082;
        private const int WM_SETCURSOR  = 0x0020;
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_NCDESTROY)
            {
                base.WndProc(ref m);
                if (_active)
                    Program.settingsForm?.BeginInvoke(() => { RestorePosition(); base.Show(); });
                return;
            }

            if (m.Msg == WM_SETCURSOR)
            {
                Cursor.Current = _dragging ? Cursors.SizeAll : Cursors.Hand;
                m.Result = (IntPtr)1; // prevent DefWindowProc from overriding the cursor
                return;
            }

            if (m.Msg == WM_LBUTTONDOWN && _dragModeActive)
            {
                _dragging = true;
                _dragCursorStart = Cursor.Position;
                _dragWindowStart = Location;
                SetCapture(Handle);
                m.Result = IntPtr.Zero;
                return;
            }

            if (m.Msg == WM_MOUSEMOVE && _dragging)
            {
                Point cursor = Cursor.Position;
                int newX = _dragWindowStart.X + cursor.X - _dragCursorStart.X;
                int newY = _dragWindowStart.Y + cursor.Y - _dragCursorStart.Y;
                Screen screen = Screen.FromPoint(cursor);
                const int margin = 5;
                newX = Math.Clamp(newX, screen.Bounds.Left + margin, screen.Bounds.Right  - Width  - margin);
                newY = Math.Clamp(newY, screen.Bounds.Top  + margin, screen.Bounds.Bottom - Height - margin);
                Location = new Point(newX, newY);
                m.Result = IntPtr.Zero;
                return;
            }

            if (m.Msg == WM_LBUTTONUP && _dragging)
            {
                _dragging = false;
                ReleaseCapture();

                Point upCursor = Cursor.Position;
                bool wasClick = Math.Abs(upCursor.X - _dragCursorStart.X) <= 5 &&
                                Math.Abs(upCursor.Y - _dragCursorStart.Y) <= 5;
                if (wasClick)
                {
                    // Determine right-side anchor BEFORE the width changes
                    Point center = new Point(Location.X + Width / 2, Location.Y + Height / 2);
                    Screen screen = Screen.FromPoint(center);
                    bool isRight = center.X > screen.Bounds.X + screen.Bounds.Width / 2;
                    int rightEdge = Location.X + Width;

                    _lightMode = !_lightMode;
                    AppConfig.Set("overlay_light_mode", _lightMode ? 1 : 0);
                    Invalidate(); // resizes the window synchronously via PerformPaint → Size.set

                    if (isRight)
                    {
                        Location = new Point(rightEdge - Width, Location.Y);
                        SavePosition();
                    }
                }
                else
                {
                    SavePosition();
                }

                if (!AreDragKeysDown())
                {
                    _dragModeActive = false;
                    SetTransparentStyle(true);
                }
                m.Result = IntPtr.Zero;
                return;
            }

            base.WndProc(ref m);
        }

        private float GetDpiScale()
        {
            Screen screen = Screen.PrimaryScreen ?? Screen.AllScreens[0];
            POINT pt = new POINT { x = screen.Bounds.X + 1, y = screen.Bounds.Y + 1 };
            IntPtr monitor = MonitorFromPoint(pt, MONITOR_DEFAULTTOPRIMARY);
            if (GetDpiForMonitor(monitor, MDT_EFFECTIVE_DPI, out uint dpiX, out _) == 0)
                return dpiX / (float)BaseDpi;
            return 1f;
        }

        private static int S(float sc, int v) => (int)(v * sc);
        private static double D(object? v) { try { return v is null ? 0.0 : Convert.ToDouble(v); } catch { return 0.0; } }

        private static string FmtTemp(double t) =>
        t > 0 ? ((int)Math.Round(t) + "°").PadLeft(4) : " -- ";

        private static string FmtPow(double p) =>
        p > 0 ? Math.Round(p, 1).ToString("F1") + "W" : "";

        private static string FormatFan(int? fan)
        {
            if (fan is null || fan < 0) return "";
            return fan.ToString();
        }

        private void Tick()
        {
            // Drag-mode detection: only activate when the cursor is already over the
            // overlay, preventing CTRL+SHIFT+ALT used in games from toggling drag mode.
            bool mouseOver = new Rectangle(Location, Size).Contains(Cursor.Position);
            bool keysDown = mouseOver && AreDragKeysDown();
            if (keysDown != _dragModeActive && !_dragging)
            {
                _dragModeActive = keysDown;
                SetTransparentStyle(!keysDown);
                Cursor.Current = keysDown ? Cursors.Hand : Cursors.Default;
            }

            if (Handle != nint.Zero)
                SetWindowPos(Handle, HWND_TOPMOST, 0, 0, 0, 0,
                SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);

            // Pin FPS counter to foreground process — queried every second so
            // switching games is handled automatically without manual configuration.
            if (_fps != null)
            {
                GetWindowThreadProcessId(GetForegroundWindow(), out uint fgPid);
                int pid = (int)fgPid;
                if (pid != Environment.ProcessId && pid != _lastFgPid)
                {
                    _lastFgPid = pid;
                    _currentFps = 0;
                    _fps.TargetPid = pid;
                }
            }

            HardwareControl.ReadSensorsOverlay();

            // gpuActive gates power, fan and chart history — when the GPU is disabled
            // its sensors return stale non-zero values, so we zero them out explicitly.
            double gpuTemp = D(HardwareControl.gpuTemp);
            double cpuTemp = D(HardwareControl.cpuTemp);
            bool gpuActive = gpuTemp > 0;

            // Trailing space is the separator between temp and fan number
            _gpuTempStr = "GPU:" + FmtTemp(gpuTemp) + " ";
            _cpuTempStr = "CPU:" + FmtTemp(cpuTemp) + " ";
            _gpuFanNum = FormatFan(HardwareControl.gpuFanRPM);
            _cpuFanNum = FormatFan(HardwareControl.cpuFanRPM);
            _gpuPow = gpuActive ? FmtPow(D(HardwareControl.gpuPower)) : "";
            _cpuPow = FmtPow(D(HardwareControl.cpuPower));

            _cpuHistory[_historyHead] = (float)Math.Max(0, D(HardwareControl.cpuPower));
            _gpuHistory[_historyHead] = gpuActive ? (float)Math.Max(0, D(HardwareControl.gpuPower)) : 0f;
            _historyHead = (_historyHead + 1) % HistoryLength;

            Invalidate();
        }

        protected override void PerformPaint(PaintEventArgs e)
        {
            float sc = GetDpiScale();

            int padX = S(sc, BasePadX);
            int padY = S(sc, BasePadY);
            int lineH = S(sc, BaseLineHeight);
            int lineGap = S(sc, BaseLineSpacing);
            int width = S(sc, _lightMode ? BaseLightWidth : BaseWidth);
            int radius = S(sc, CornerRadius);
            int fpsColW = S(sc, BaseFpsColWidth);
            int chartColW = S(sc, BaseChartColWidth);
            int powColW = S(sc, BasePowerColWidth);
            int colGap = S(sc, BaseColGap);
            int powGap = S(sc, BasePowerGap);

            int innerH = lineH * 2 + lineGap;
            int totalH = padY * 2 + innerH;

            if (Size.Width != width || Size.Height != totalH)
                Size = new Size(width, totalH);

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            g.FillRoundedRectangle(_bgBrush, Bound, radius);

            if (sc != _lastDpiScale)
            {
                _lastDpiScale = sc;
                _font?.Dispose();    _font    = new Font("Consolas", BaseFontSize * sc, FontStyle.Regular, GraphicsUnit.Pixel);
                _rpmFont?.Dispose(); _rpmFont = new Font("Consolas", BaseRpmFontSize * sc, FontStyle.Regular, GraphicsUnit.Pixel);
                _fpsBold?.Dispose(); _fpsBold = new Font("Consolas", innerH / 1.15f, FontStyle.Bold, GraphicsUnit.Pixel);
                _totalPen?.Dispose(); _totalPen = new Pen(Color.FromArgb(255, 200, 200, 200), sc * 0.75f) { DashStyle = DashStyle.Dot };
                _axPen?.Dispose();    _axPen    = new Pen(Color.FromArgb(255, 80, 80, 80), sc * 0.5f);
            }

            var font    = _font!;
            var rpmFont = _rpmFont!;
            var fpsBold = _fpsBold!;

            // Differential trick: MeasureString("XX") - MeasureString("X") cancels the
            // fixed GDI+ padding, giving the true per-character advance width for Consolas.
            float charW = g.MeasureString("XX", font).Width - g.MeasureString("X", font).Width;

            int leftX = padX + fpsColW + colGap;
            int chartX = leftX + S(sc, _lightMode ? BaseLightLeftColWidth : BaseLeftColWidth);
            int powX = _lightMode ? chartX + powGap : chartX + chartColW + powGap;
            int topY = padY;

            // FPS
            string fpsStr = _currentFps > 0 ? _currentFps.ToString() : "--";
            float fpsW = g.MeasureString(fpsStr, fpsBold).Width;
            g.DrawString(fpsStr, fpsBold, _gpuBrush,
            new PointF(padX + (fpsColW - fpsW) / 2f, topY));

            // Left column: fan RPM only in full mode
            DrawTempFan(g, font, rpmFont, charW, sc, leftX, topY, _gpuTempStr, _lightMode ? "" : _gpuFanNum, _gpuBrush);
            DrawTempFan(g, font, rpmFont, charW, sc, leftX, topY + lineH + lineGap, _cpuTempStr, _lightMode ? "" : _cpuFanNum, _cpuBrush);

            // Chart — full mode only
            if (!_lightMode)
                DrawStackedChart(g, chartX, topY, chartColW, innerH, sc);

            // Power — right-aligned
            if (_gpuPow.Length > 0)
                g.DrawString(_gpuPow, font, _gpuBrush,
                new PointF(powX + powColW - g.MeasureString(_gpuPow, font).Width, topY));
            if (_cpuPow.Length > 0)
                g.DrawString(_cpuPow, font, _cpuBrush,
                new PointF(powX + powColW - g.MeasureString(_cpuPow, font).Width, topY + lineH + lineGap));
        }

        // tempStr already has a trailing space — natural separator from fan number.
        // 2px gap added before "RPM" superscript so it doesn't glue to the digits.
        private static void DrawTempFan(Graphics g, Font font, Font rpmFont, float charW, float sc,
        float x, float y, string tempStr, string fanNum, SolidBrush brush)
        {
            g.DrawString(tempStr, font, brush, new PointF(x, y));

            if (fanNum.Length == 0) return;

            float numX = x + charW * tempStr.Length;
            g.DrawString(fanNum, font, brush, new PointF(numX, y));

            // 2px gap between digits and "RPM" label
            float rpmX = numX + charW * fanNum.Length + 2f * sc;
            g.DrawString("RPM", rpmFont, brush, new PointF(rpmX, y));
        }

        private void DrawStackedChart(Graphics g, int x, int y, int w, int h, float sc)
        {
            float peak = 10f;
            for (int i = 0; i < HistoryLength; i++)
            {
                float t = _cpuHistory[i] + _gpuHistory[i];
                if (t > peak) peak = t;
            }

            float stepX = (float)w / (HistoryLength - 1);
            int Idx(int i) => (_historyHead + i) % HistoryLength;

            for (int i = 0; i < HistoryLength; i++)
            {
                int idx = Idx(i);
                float px = x + i * stepX;
                float cpuH = (_cpuHistory[idx] / peak) * h;
                float gpuH = (_gpuHistory[idx] / peak) * h;

                _basePts[i] = new PointF(px, y + h);
                _cpuPts[i]  = new PointF(px, y + h - cpuH);
                _gpuPts[i]  = new PointF(px, y + h - cpuH - gpuH);
            }

            FillArea(g, _cpuPts, _basePts, _cpuFillBrush);
            g.DrawLines(_cpuLinePen, _cpuPts);
            FillArea(g, _gpuPts, _cpuPts, _gpuFillBrush);
            g.DrawLines(_gpuLinePen, _gpuPts);

            g.DrawLines(_totalPen!, _gpuPts);
            g.DrawLine(_axPen!, x, y + h, x + w, y + h);
        }

        private void FillArea(Graphics g, PointF[] topLine, PointF[] bottomLine, SolidBrush brush)
        {
            int n = topLine.Length;
            topLine.CopyTo(_polyPts, 0);
            for (int i = 0; i < n; i++)
                _polyPts[n + i] = bottomLine[n - 1 - i];
            g.FillPolygon(brush, _polyPts);
        }

        private void PositionAtTopLeft()
        {
            Screen screen = Screen.PrimaryScreen ?? Screen.AllScreens[0];
            Location = new Point(screen.Bounds.X + MarginFromEdge, screen.Bounds.Y + MarginFromEdge);
        }

        private bool AreDragKeysDown() =>
            (GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0 &&
            (GetAsyncKeyState(VK_SHIFT)   & 0x8000) != 0 &&
            (GetAsyncKeyState(VK_MENU)    & 0x8000) != 0;

        private void SetTransparentStyle(bool transparent)
        {
            if (Handle == nint.Zero) return;
            int style = GetWindowLong(Handle, GWL_EXSTYLE);
            style = transparent ? (style | WS_EX_TRANSPARENT_FLAG) : (style & ~WS_EX_TRANSPARENT_FLAG);
            SetWindowLong(Handle, GWL_EXSTYLE, style);
        }

        private void SavePosition()
        {
            Point center = new Point(Location.X + Width / 2, Location.Y + Height / 2);
            Screen screen = Screen.FromPoint(center);
            bool isRight  = center.X > screen.Bounds.X + screen.Bounds.Width  / 2;
            bool isBottom = center.Y > screen.Bounds.Y + screen.Bounds.Height / 2;
            int anchor  = (isBottom ? 2 : 0) | (isRight ? 1 : 0);
            int offsetX = isRight  ? screen.Bounds.Right  - Location.X - Width  : Location.X - screen.Bounds.X;
            int offsetY = isBottom ? screen.Bounds.Bottom - Location.Y - Height : Location.Y - screen.Bounds.Y;
            AppConfig.Set("overlay_anchor",   anchor);
            AppConfig.Set("overlay_offset_x", offsetX);
            AppConfig.Set("overlay_offset_y", offsetY);
        }

        private void RestorePosition()
        {
            int anchor = AppConfig.Get("overlay_anchor", -1);
            if (anchor < 0) { PositionAtTopLeft(); return; }
            int offsetX = AppConfig.Get("overlay_offset_x", MarginFromEdge);
            int offsetY = AppConfig.Get("overlay_offset_y", MarginFromEdge);
            Screen screen = Screen.PrimaryScreen ?? Screen.AllScreens[0];
            bool isRight  = (anchor & 1) != 0;
            bool isBottom = (anchor & 2) != 0;
            int x = isRight  ? screen.Bounds.Right  - Width  - offsetX : screen.Bounds.X + offsetX;
            int y = isBottom ? screen.Bounds.Bottom - Height - offsetY : screen.Bounds.Y + offsetY;
            const int margin = 5;
            x = Math.Clamp(x, screen.Bounds.Left + margin, screen.Bounds.Right  - Width  - margin);
            y = Math.Clamp(y, screen.Bounds.Top  + margin, screen.Bounds.Bottom - Height - margin);
            Location = new Point(x, y);
        }

        public void StartOverlay()
        {
            _active = true;
            _lastFgPid = 0;
            _lightMode = AppConfig.Is("overlay_light_mode");

            _fps?.Dispose();
            _currentFps = 0;
            _fps = new EtwFpsMonitor();
            _fps.FpsUpdated += d =>
            {
                _currentFps = (int)Math.Round(d);
            };
            _fpsTask = Task.Run(() => _fps.Start());

            float sc = GetDpiScale();
            int innerH = S(sc, BaseLineHeight) * 2 + S(sc, BaseLineSpacing);
            Size = new Size(S(sc, BaseWidth), S(sc, BasePadY) * 2 + innerH);

            RestorePosition();
            base.Show();
            Tick();
            _timer.Start();
        }

        public void StopOverlay()
        {
            _active = false;
            _timer.Stop();
            _dragModeActive = false;
            _dragging = false;

            // Dispose triggers CloseTrace + StopTrace inside EtwFpsMonitor, which unblocks
            // ProcessTrace so the background task thread can exit.
            _fps?.Dispose();
            _fps = null;
            _currentFps = 0;

            // Wait for the ETW pump thread to actually finish before we trim memory,
            // otherwise its kernel ring buffers are still mapped into our working set.
            Task? task = _fpsTask;
            _fpsTask = null;
            MemoryHelper.TrimAfter(task);

            _font?.Dispose();     _font     = null;
            _rpmFont?.Dispose();  _rpmFont  = null;
            _fpsBold?.Dispose();  _fpsBold  = null;
            _totalPen?.Dispose(); _totalPen = null;
            _axPen?.Dispose();    _axPen    = null;
            _lastDpiScale = 0f;
            base.Hide();
        }
    }
}
