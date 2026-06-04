using GHelper.Helpers;
using Microsoft.Win32;
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

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
        private const uint GW_HWNDPREV = 3;

        private const uint MONITOR_DEFAULTTOPRIMARY = 1;
        private const int MDT_EFFECTIVE_DPI = 0;
        private const int BaseDpi = 96;

        private const int GWL_EXSTYLE        = -20;
        private const int WS_EX_TRANSPARENT_FLAG = 0x00000020;
        private const int WM_LBUTTONDOWN     = 0x0201;
        private const int WM_MOUSEMOVE       = 0x0200;
        private const int WM_LBUTTONUP       = 0x0202;
        private const int WM_MOUSEWHEEL      = 0x020A;
        private const int WM_MBUTTONDOWN     = 0x0207;
        private const int VK_CONTROL         = 0x11;
        private const int VK_SHIFT           = 0x10;
        private const int VK_MENU            = 0x12; // ALT

        private const int MinScalePercent  = 50;
        private const int MaxScalePercent  = 300;
        private const int ScaleStepPercent = 10;
        private int _scalePercent = 100;

        private bool _dragging;
        private Point _dragCursorStart;
        private Point _dragWindowStart;
        private bool _dragModeActive;
        // Values match the persisted "overlay_light_mode" key for backward compat
        // (legacy: 0 = default, 1 = light).
        private enum OverlayMode { Default = 0, Light = 1, Full = 2, Complete = 3 }
        private OverlayMode _mode;

        // ── Layout constants (base = 96 dpi) ─────────────────────────────────
        //
        // Light:    fps | temp                        | power
        // Default:  fps | temp + fan RPM              | chart | power
        // Full:     fps | temp + fan RPM              | chart | power | usage% | bar
        // Complete: fps | name | temp + fan RPM       | chart | power | usage% | bar | VRAM/RAM | mem% | bar
        //
        // Click on the overlay cycles Light → Default → Full → Complete → Light.
        // Complete adds a short GPU/CPU name column after FPS and a VRAM/RAM bar+% column at the right.
        //
        // Bar height is fixed per DPI (~BaseUsageBarHeight * sc) and cell pitch is
        // integer, so the number of cells varies with available pixels while every
        // cell stays a clean integer height.
        //
        private const float BaseFontSize = 13f;
        private const float BaseRpmFontSize = 8.5f;
        private const int BaseLineHeight = 18;
        private const int BaseLineSpacing = 1;
        private const int BasePadX = 8;
        private const int BasePadY = 4;
        private const int BaseFpsColWidth = 52;
        private const int BaseLeftColWidth = 128;
        private const int BaseChartColWidth = 120;
        private const int BasePowerGap = 4;
        private const int BasePowerColWidth = 46; // fits "120.9W" (6 chars, F1 + "W") right-aligned, no extra slack
        private const int BaseColGap = 8;
        private const int CornerRadius = 3;
        private const int MarginFromEdge = 10;
        private const int BaseLightLeftColWidth = 64; // fits "GPU: 82° " (9 Consolas chars); trailing space is the gap to the power column
        private const int BaseUsageBarGap = 11;       // gap between the power W and the usage % column (full mode)
        private const int BaseUsageBarWidth = 5;
        private const int BaseUsageNumGap = 4;        // gap between the usage % text and its bar
        private const int BaseUsageNumColWidth = 30;  // right-aligned column fitting "100%"
        private const int BaseFullPadRight = 4;       // tighter right margin in full mode (vs BasePadX)
        // Target bar height at base DPI; tuned so at 2x DPI numCells = 10.
        private const int BaseUsageBarHeight = 15;
        private const int BaseNameColWidth = 90;     // fits "AMD RX 6850M" / "NV RTX 4070" (~12 Consolas chars)
        private const int BaseMemBarGap = 8;          // gap between the usage bar and the VRAM/RAM label (complete mode)
        private const int BaseMemLabelColWidth = 34;  // fits "VRAM" + a small gap before the bar
        private const int BaseLightWidth = BasePadX + BaseFpsColWidth + BaseColGap + BaseLightLeftColWidth + BasePowerGap + BasePowerColWidth + BasePadX;
        private const int BaseWidth = BasePadX + BaseFpsColWidth + BaseColGap + BaseLeftColWidth + BaseColGap + BaseChartColWidth + BasePowerGap + BasePowerColWidth + BasePadX;
        private const int BaseFullWidth = BaseWidth - BasePadX + BaseUsageBarGap + BaseUsageBarWidth + BaseUsageNumGap + BaseUsageNumColWidth + BaseFullPadRight;
        private const int BaseCompleteWidth = BaseFullWidth + BaseNameColWidth + BaseColGap + BaseMemBarGap + BaseMemLabelColWidth + BaseUsageBarWidth + BaseUsageNumGap + BaseUsageNumColWidth;

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
        private int? _gpuUsage;
        private int? _cpuUsage;
        private int? _vramUsage;
        private int? _ramUsage;
        private string _gpuShortName = "";
        private string _cpuShortName = "";

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

            if (m.Msg == WM_MOUSEWHEEL && AreDragKeysDown())
            {
                int delta = (short)((m.WParam.ToInt64() >> 16) & 0xFFFF);
                ApplyScale(_scalePercent + (delta > 0 ? ScaleStepPercent : -ScaleStepPercent));
                m.Result = IntPtr.Zero;
                return;
            }

            if (m.Msg == WM_MBUTTONDOWN && AreDragKeysDown())
            {
                ApplyScale(100);
                m.Result = IntPtr.Zero;
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

                    _mode = _mode switch
                    {
                        OverlayMode.Light   => OverlayMode.Default,
                        OverlayMode.Default => OverlayMode.Full,
                        OverlayMode.Full    => OverlayMode.Complete,
                        _                   => OverlayMode.Light,
                    };
                    AppConfig.Set("overlay_mode", (int)_mode);
                    ApplyModeReadFlags();
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
            float dpi = 1f;
            if (GetDpiForMonitor(monitor, MDT_EFFECTIVE_DPI, out uint dpiX, out _) == 0)
                dpi = dpiX / (float)BaseDpi;
            return dpi * (_scalePercent / 100f);
        }

        private static int S(float sc, int v) => (int)(v * sc);
        private static double D(object? v) { try { return v is null ? 0.0 : Convert.ToDouble(v); } catch { return 0.0; } }

        private static string FmtTemp(double t) =>
        t > 0 ? ((int)Math.Round(t) + "°").PadLeft(4) : " -- ";

        private static string FmtPow(double p) =>
        p > 0 ? Math.Round(p, 1).ToString("F1") + "W" : "";

        // "NVIDIA GeForce RTX 4070 Laptop GPU" -> "NV RTX 4070", "AMD Radeon RX 6850M XT" -> "AMD RX 6850M"
        private static string ShortGpuName(string? full, bool isNvidia)
        {
            if (string.IsNullOrEmpty(full)) return "";
            string prefix = isNvidia ? "NV " : "AMD ";
            foreach (string tag in new[] { "RTX", "GTX", "RX", "Arc" })
            {
                int i = full.IndexOf(tag, StringComparison.OrdinalIgnoreCase);
                if (i < 0) continue;
                string[] p = full[i..].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return prefix + (p.Length >= 2 ? p[0] + " " + p[1] : p[0]);
            }
            return full;
        }

        // "AMD Ryzen 9 7945HX..." -> "Ryzen 9", "...Core(TM) i9-13980HX" -> "Core i9"
        private static string ShortCpuName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "";

            int r = name.IndexOf("Ryzen", StringComparison.OrdinalIgnoreCase);
            if (r >= 0)
            {
                string[] p = name[r..].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (p.Length >= 3 && p[1].Equals("AI", StringComparison.OrdinalIgnoreCase)) return "Ryzen AI " + p[2];
                return p.Length >= 2 ? "Ryzen " + p[1] : "Ryzen";
            }

            int c = name.IndexOf("Core", StringComparison.OrdinalIgnoreCase);
            if (c >= 0)
            {
                string[] p = name[c..].Split(new[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
                if (p.Length >= 3 && p[1].Equals("Ultra", StringComparison.OrdinalIgnoreCase)) return "Core Ultra " + p[2];
                return p.Length >= 2 ? "Core " + p[1] : "Core";
            }

            return name.Split(' ', StringSplitOptions.RemoveEmptyEntries) is { Length: > 0 } t ? t[0] : "";
        }

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

            if (Handle != nint.Zero && GetWindow(Handle, GW_HWNDPREV) != IntPtr.Zero)
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
                else
                {
                    _currentFps = (int)Math.Round(_fps.SampleFps());
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
            _gpuPow = HardwareControl.gpuPower is null ? "" : Math.Round(HardwareControl.gpuPower.Value, 1).ToString("F1") + "W";
            _cpuPow = FmtPow(D(HardwareControl.cpuPower));

            _cpuHistory[_historyHead] = (float)Math.Max(0, D(HardwareControl.cpuPower));
            _gpuHistory[_historyHead] = gpuActive ? (float)Math.Max(0, D(HardwareControl.gpuPower)) : 0f;
            _historyHead = (_historyHead + 1) % HistoryLength;

            _cpuUsage = HardwareControl.cpuUsage;
            _gpuUsage = gpuActive ? (HardwareControl.gpuUsage ?? 0) : null;
            _vramUsage = gpuActive ? (HardwareControl.vramUsage ?? 0) : null;
            _ramUsage = HardwareControl.ramUsage;

            // Names are static — resolve each once and cache; retry only while still empty
            // (GPU may not be ready yet when the overlay starts).
            if (_mode == OverlayMode.Complete)
            {
                if (_cpuShortName.Length == 0)
                    _cpuShortName = ShortCpuName(PawnIO.CpuInfo.Name);
                if (_gpuShortName.Length == 0)
                {
                    var gpu = HardwareControl.GpuControl;
                    _gpuShortName = ShortGpuName(gpu?.FullName, gpu?.IsNvidia ?? false);
                }
            }

            Invalidate();
        }

        protected override void PerformPaint(PaintEventArgs e)
        {
            float sc = GetDpiScale();

            int padX = S(sc, BasePadX);
            int padY = S(sc, BasePadY);
            int lineH = S(sc, BaseLineHeight);
            int lineGap = S(sc, BaseLineSpacing);
            int width = S(sc, _mode switch
            {
                OverlayMode.Light    => BaseLightWidth,
                OverlayMode.Full     => BaseFullWidth,
                OverlayMode.Complete => BaseCompleteWidth,
                _                    => BaseWidth,
            });
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
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
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

            bool isLight = _mode == OverlayMode.Light;
            bool isFull  = _mode == OverlayMode.Full;
            bool isComplete = _mode == OverlayMode.Complete;
            bool showUsage = isFull || isComplete;

            // Name column sits between FPS and the temp column, complete mode only.
            int nameX = padX + fpsColW + colGap;
            int nameColW = isComplete ? S(sc, BaseNameColWidth) : 0;
            int leftX = nameX + nameColW + (isComplete ? colGap : 0);
            int chartX = leftX + S(sc, isLight ? BaseLightLeftColWidth : BaseLeftColWidth);
            int powX = isLight ? chartX + powGap : chartX + chartColW + powGap;
            int usageNumX = powX + powColW + S(sc, BaseUsageBarGap);
            int usageNumColW = S(sc, BaseUsageNumColWidth);
            int barW = S(sc, BaseUsageBarWidth);
            int barX = usageNumX + usageNumColW + S(sc, BaseUsageNumGap);
            int memLabelX = barX + barW + S(sc, BaseMemBarGap);
            int memNumX = memLabelX + S(sc, BaseMemLabelColWidth);
            int memBarX = memNumX + usageNumColW + S(sc, BaseUsageNumGap);
            int topY = padY;
            // Nudge per-row text down so it lines up with the vertically centered usage bars.
            int textY = topY + (int)Math.Round(sc);

            // FPS
            string fpsStr = _currentFps > 0 ? _currentFps.ToString() : "--";
            float fpsW = g.MeasureString(fpsStr, fpsBold).Width;
            g.DrawString(fpsStr, fpsBold, _gpuBrush,
            new PointF(padX + (fpsColW - fpsW) / 2f, topY));

            // Short GPU/CPU names — complete mode only; clipped to the column so a long
            // name (e.g. "Core Ultra 9") can't bleed into the temp column.
            if (isComplete)
            {
                var savedClip = g.Save();
                g.SetClip(new RectangleF(nameX, topY, nameColW, innerH));
                g.DrawString(_gpuShortName, font, _gpuBrush, new PointF(nameX, textY));
                g.DrawString(_cpuShortName, font, _cpuBrush, new PointF(nameX, textY + lineH + lineGap));
                g.Restore(savedClip);
            }

            // Left column: fan RPM hidden in Light mode
            DrawTempFan(g, font, rpmFont, charW, sc, leftX, textY, _gpuTempStr, isLight ? "" : _gpuFanNum, _gpuBrush);
            DrawTempFan(g, font, rpmFont, charW, sc, leftX, textY + lineH + lineGap, _cpuTempStr, isLight ? "" : _cpuFanNum, _cpuBrush);

            // Chart — hidden in Light mode
            if (!isLight)
                DrawStackedChart(g, chartX, topY, chartColW, innerH, sc);

            // Power — right-aligned, drawn in all modes
            if (_gpuPow.Length > 0)
                g.DrawString(_gpuPow, font, _gpuBrush,
                new PointF(powX + powColW - g.MeasureString(_gpuPow, font).Width, textY));
            if (_cpuPow.Length > 0)
                g.DrawString(_cpuPow, font, _cpuBrush,
                new PointF(powX + powColW - g.MeasureString(_cpuPow, font).Width, textY + lineH + lineGap));

            if (showUsage)
            {
                // Bar sizing: fixed bar height per DPI; cellH grows with DPI but sepH stays 1.
                int cellH = Math.Max(1, (int)Math.Floor(sc));
                int sepH = 1;
                int targetBarH = S(sc, BaseUsageBarHeight);
                int pitch = cellH + sepH;
                int numCells = Math.Max(2, (targetBarH + sepH) / pitch);
                int barH = numCells * cellH + (numCells - 1) * sepH;
                int barYOff = (lineH - barH) / 2;
                int row2Y = lineH + lineGap;

                DrawUsageBar(g, barX, topY + barYOff, barW, cellH, sepH, numCells, _gpuUsage ?? 0, _gpuBrush, _gpuFillBrush);
                DrawUsageBar(g, barX, topY + row2Y + barYOff, barW, cellH, sepH, numCells, _cpuUsage ?? 0, _cpuBrush, _cpuFillBrush);

                DrawUsagePercent(g, font, usageNumX, usageNumColW, textY,           _gpuUsage, _gpuBrush);
                DrawUsagePercent(g, font, usageNumX, usageNumColW, textY + row2Y,   _cpuUsage, _cpuBrush);

                // VRAM (GPU row) / RAM (CPU row) — complete mode only
                if (isComplete)
                {
                    g.DrawString("VRAM", font, _gpuBrush, new PointF(memLabelX, textY));
                    g.DrawString("DRAM", font, _cpuBrush, new PointF(memLabelX, textY + row2Y));

                    DrawUsageBar(g, memBarX, topY + barYOff, barW, cellH, sepH, numCells, _vramUsage ?? 0, _gpuBrush, _gpuFillBrush);
                    DrawUsageBar(g, memBarX, topY + row2Y + barYOff, barW, cellH, sepH, numCells, _ramUsage ?? 0, _cpuBrush, _cpuFillBrush);

                    DrawUsagePercent(g, font, memNumX, usageNumColW, textY,         _vramUsage, _gpuBrush);
                    DrawUsagePercent(g, font, memNumX, usageNumColW, textY + row2Y, _ramUsage, _cpuBrush);
                }
            }
        }

        private static void DrawUsagePercent(Graphics g, Font font, int x, int colW, int y, int? usage, SolidBrush brush)
        {
            if (!usage.HasValue) return; // mirror the power column — empty when unavailable
            string s = usage.Value + "%";
            g.DrawString(s, font, brush, new PointF(x + colW - g.MeasureString(s, font).Width, y));
        }

        private static void DrawUsageBar(Graphics g, int x, int y, int w, int cellH, int sepH, int numCells, int usage, SolidBrush litBrush, SolidBrush dimBrush)
        {
            var prevSmoothing = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.None;

            int lit = Math.Clamp((int)Math.Round(usage * numCells / 100f), 0, numCells);
            int pitch = cellH + sepH;

            for (int i = 0; i < numCells; i++)
            {
                bool isLit = i >= (numCells - lit);
                g.FillRectangle(isLit ? litBrush : dimBrush, x, y + i * pitch, w, cellH);
            }

            g.SmoothingMode = prevSmoothing;
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

            var saved = g.Save();
            g.SetClip(new RectangleF(x, y, w, h));

            FillArea(g, _cpuPts, _basePts, _cpuFillBrush);
            g.DrawLines(_cpuLinePen, _cpuPts);
            FillArea(g, _gpuPts, _cpuPts, _gpuFillBrush);
            g.DrawLines(_gpuLinePen, _gpuPts);

            g.DrawLines(_totalPen!, _gpuPts);

            g.Restore(saved);
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

        private void ApplyScale(int next)
        {
            next = Math.Clamp(next, MinScalePercent, MaxScalePercent);
            if (next == _scalePercent) return;

            Point center = new Point(Location.X + Width / 2, Location.Y + Height / 2);
            Screen screen = Screen.FromPoint(center);
            bool isRight  = center.X > screen.Bounds.X + screen.Bounds.Width  / 2;
            bool isBottom = center.Y > screen.Bounds.Y + screen.Bounds.Height / 2;
            int rightEdge  = Location.X + Width;
            int bottomEdge = Location.Y + Height;

            _scalePercent = next;
            AppConfig.Set("overlay_scale_percent", _scalePercent);
            Invalidate(); // resizes synchronously via PerformPaint → Size.set

            int newX = isRight  ? rightEdge  - Width  : Location.X;
            int newY = isBottom ? bottomEdge - Height : Location.Y;
            if (newX != Location.X || newY != Location.Y)
                Location = new Point(newX, newY);
            SavePosition();
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

        private void ApplyModeReadFlags()
        {
            HardwareControl.readFans   = _mode != OverlayMode.Light;
            HardwareControl.readUsage  = _mode == OverlayMode.Full || _mode == OverlayMode.Complete;
            HardwareControl.readMemory = _mode == OverlayMode.Complete;
        }

        // Re-anchor the overlay after the user changes resolution or swaps the primary
        // display — without this the absolute Location can end up off-screen or far
        // from the corner the user originally pinned it to.
        private void OnDisplaySettingsChanged(object? sender, EventArgs e)
        {
            if (!_active) return;
            Program.settingsForm?.BeginInvoke(() => { if (_active) RestorePosition(); });
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
            // overlay_mode is the new key. Migrate from legacy overlay_light_mode (0/1)
            // when the new one isn't set yet so existing users keep their preference.
            int storedMode = AppConfig.Exists("overlay_mode")
                ? AppConfig.Get("overlay_mode", 0)
                : AppConfig.Get("overlay_light_mode", 0);
            _mode = storedMode == (int)OverlayMode.Light    ? OverlayMode.Light
                  : storedMode == (int)OverlayMode.Full     ? OverlayMode.Full
                  : storedMode == (int)OverlayMode.Complete ? OverlayMode.Complete
                  : OverlayMode.Default;
            _scalePercent = Math.Clamp(AppConfig.Get("overlay_scale_percent", 100), MinScalePercent, MaxScalePercent);
            ApplyModeReadFlags();
            SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;
            HardwareControl.ResetCPUPowerCounter();

            _fps?.Dispose();
            _currentFps = 0;
            _fps = new EtwFpsMonitor();
            _fpsTask = Task.Run(() => _fps.Start());

            float sc = GetDpiScale();
            int innerH = S(sc, BaseLineHeight) * 2 + S(sc, BaseLineSpacing);
            int initialBaseW = _mode switch
            {
                OverlayMode.Light    => BaseLightWidth,
                OverlayMode.Full     => BaseFullWidth,
                OverlayMode.Complete => BaseCompleteWidth,
                _                    => BaseWidth,
            };
            Size = new Size(S(sc, initialBaseW), S(sc, BasePadY) * 2 + innerH);

            RestorePosition();
            base.Show();
            Tick();
            _timer.Start();
        }

        public void StopOverlay()
        {
            _active = false;
            HardwareControl.readUsage = false;
            HardwareControl.readFans = false;
            HardwareControl.readMemory = false;
            SystemEvents.DisplaySettingsChanged -= OnDisplaySettingsChanged;
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
