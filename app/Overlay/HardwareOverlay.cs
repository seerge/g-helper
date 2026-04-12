using GHelper.Helpers;
using GHelper.Helpers;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace GHelper.Overlay
{
    public class HardwareOverlay : OSDNativeForm
    {
        // ?? DPI ??????????????????????????????????????????????????????????????
        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromPoint(POINT pt, uint dwFlags);
        [DllImport("shcore.dll")]
        private static extern int GetDpiForMonitor(IntPtr hMonitor, int dpiType, out uint dpiX, out uint dpiY);
        private const uint MONITOR_DEFAULTTOPRIMARY = 1;
        private const int MDT_EFFECTIVE_DPI = 0;
        private const int BaseDpi = 96;

        // ?? Layout constants (base = 96 dpi) ?????????????????????????????????
        private const float BaseFontSize   = 13f;
        private const int   BaseLineHeight = 18;
        private const int   BasePadX       = 8;
        private const int   BasePadY       = 6;
        private const int   BaseLineSpacing = 4;
        private const int   BaseWidth      = 240;
        private const int   BaseChartHeight = 44;   // total stacked chart area
        private const int   BaseChartGap   = 6;
        private const int   CornerRadius   = 3;
        private const int   MarginFromEdge = 10;

        // ?? Colours (all fully opaque — background gets 50% alpha painted directly) ??
        // Background: 50% transparent black so only it is semi-transparent
        private static readonly SolidBrush _bgBrush       = new(Color.FromArgb(128, 0, 0, 0));
        // Text / chart: 100% opaque
        private static readonly SolidBrush _textBrush     = new(Color.FromArgb(255, 0,   255, 80));
        private static readonly SolidBrush _dimTextBrush  = new(Color.FromArgb(255, 0,   180, 56));
        private static readonly Pen        _cpuLinePen    = new(Color.FromArgb(255, 0,   255, 80),  1.5f);
        private static readonly Pen        _gpuLinePen    = new(Color.FromArgb(255, 60,  220, 255), 1.5f);
        private static readonly SolidBrush _cpuFillBrush  = new(Color.FromArgb(255, 0,   80,  30));
        private static readonly SolidBrush _gpuFillBrush  = new(Color.FromArgb(255, 20,  70,  90));
        private static readonly SolidBrush _totalFillBrush = new(Color.FromArgb(80,  255, 255, 255));

        // ?? State ?????????????????????????????????????????????????????????????
        private string _line1 = "";
        private string _line2 = "";
        private string _line3 = "";

        private const int HistoryLength = 60;
        private readonly float[] _cpuHistory = new float[HistoryLength];
        private readonly float[] _gpuHistory = new float[HistoryLength];
        private int _historyHead = 0;

        private readonly System.Timers.Timer _timer = new(1000) { AutoReset = true };
        private FpsCounter? _fps;
        private bool _active;  // true while overlay should be visible

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int w, int h, uint flags);
        private static readonly IntPtr HWND_TOPMOST = new(-1);
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOACTIVATE = 0x0010;

        public HardwareOverlay()
        {
            // Alpha=255: per-pixel alpha in the bitmap fully controls transparency.
            // Background is painted with Color.FromArgb(128,...) ? 50% transparent.
            // All text and chart pixels use alpha=255 ? fully opaque.
            Alpha = 255;
            _timer.Elapsed += (_, _) => Tick();
        }

        // When Cyberpunk (or any game) switches DWM mode, Windows sends WM_NCDESTROY to
        // layered WS_EX_TOPMOST windows. NativeWindow destroys the handle. We re-create it
        // only if _active=true (i.e. the user hasn’t toggled the overlay off).
        private const int WM_NCDESTROY = 0x0082;
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_NCDESTROY)
            {
                base.WndProc(ref m);   // let NativeWindow zero the handle
                if (_active)
                {
                    Program.settingsForm?.BeginInvoke(() =>
                    {
                        PositionAtTopLeft();
                        base.Show();
                    });
                }
                return;
            }
            base.WndProc(ref m);
        }

        // ?? DPI ??????????????????????????????????????????????????????????????
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

        // ?? Tick ?????????????????????????????????????????????????????????????
        private void Tick()
        {
            // Re-assert HWND_TOPMOST every second so the overlay stays above fullscreen games
            if (Handle != nint.Zero)
                SetWindowPos(Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);

            HardwareControl.ReadSensorsOverlay();

            string cpuTemp  = HardwareControl.cpuTemp > 0
                ? ": " + Math.Round((decimal)HardwareControl.cpuTemp) + "°C" : "";
            string cpuPower = HardwareControl.cpuPower > 0
                ? " " + Math.Round((decimal)HardwareControl.cpuPower, 1) + "W" : "";
            string gpuTemp  = HardwareControl.gpuTemp > 0
                ? ": " + HardwareControl.gpuTemp + "°C" : "";
            string gpuPower = HardwareControl.gpuPower > 0
                ? " " + Math.Round((decimal)HardwareControl.gpuPower, 1) + "W" : "";

            _line1 = "CPU" + cpuTemp + cpuPower + "  " + HardwareControl.cpuFan;
            _line2 = "GPU" + gpuTemp + gpuPower + "  " + HardwareControl.gpuFan;

            // FPS
            int fps = _fps?.Sample() ?? -1;
            string fpsStr = fps > 0 ? fps + " FPS" : "";

            // Battery
            string batStr = "";
            if (HardwareControl.batteryRate < 0)
                batStr = Math.Round(-(decimal)HardwareControl.batteryRate, 1) + "W ?";
            else if (HardwareControl.batteryRate > 0)
                batStr = Math.Round((decimal)HardwareControl.batteryRate, 1) + "W ?";

            _line3 = string.Join("   ", new[] { fpsStr, batStr }.Where(s => s.Length > 0));

            // Power history
            float cpuW = HardwareControl.cpuPower > 0 ? (float)HardwareControl.cpuPower : 0f;
            float gpuW = HardwareControl.gpuPower > 0 ? (float)HardwareControl.gpuPower : 0f;
            _cpuHistory[_historyHead] = cpuW;
            _gpuHistory[_historyHead] = gpuW;
            _historyHead = (_historyHead + 1) % HistoryLength;

            Invalidate();
        }

        // ?? Paint ?????????????????????????????????????????????????????????????
        protected override void PerformPaint(PaintEventArgs e)
        {
            float sc = GetDpiScale();

            int padX    = S(sc, BasePadX);
            int padY    = S(sc, BasePadY);
            int lineH   = S(sc, BaseLineHeight);
            int lineGap = S(sc, BaseLineSpacing);
            int chartH  = S(sc, BaseChartHeight);
            int chartGap = S(sc, BaseChartGap);
            int width   = S(sc, BaseWidth);
            int radius  = S(sc, CornerRadius);

            bool has3   = _line3.Length > 0;
            int tLines  = has3 ? 3 : 2;
            int totalH  = padY * 2 + lineH * tLines + lineGap * (tLines - 1) + chartGap + chartH;

            if (Size.Width != width || Size.Height != totalH)
                Size = new Size(width, totalH);

            var g = e.Graphics;
            g.SmoothingMode     = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Semi-transparent background, fully opaque everything else
            g.FillRoundedRectangle(_bgBrush, Bound, radius);

            using var font = new Font("Consolas", BaseFontSize * sc, FontStyle.Regular, GraphicsUnit.Pixel);

            int y = padY;
            g.DrawString(_line1, font, _textBrush, new PointF(padX, y));
            y += lineH + lineGap;
            g.DrawString(_line2, font, _textBrush, new PointF(padX, y));
            if (has3)
            {
                y += lineH + lineGap;
                g.DrawString(_line3, font, _dimTextBrush, new PointF(padX, y));
            }

            int chartTop = padY + lineH * tLines + lineGap * (tLines - 1) + chartGap;
            int chartW   = width - padX * 2;
            DrawStackedChart(g, padX, chartTop, chartW, chartH, sc);
        }

        // ?? Stacked power chart ???????????????????????????????????????????????
        // Bottom layer: GPU (cyan fill), stacked on top: CPU (green fill).
        // The top edge of each stacked area represents CPU+GPU total.
        private void DrawStackedChart(Graphics g, int x, int y, int w, int h, float sc)
        {
            // Peak = max total (CPU+GPU) seen, minimum 10 W
            float peak = 10f;
            for (int i = 0; i < HistoryLength; i++)
            {
                float total = _cpuHistory[i] + _gpuHistory[i];
                if (total > peak) peak = total;
            }

            float stepX = (float)w / (HistoryLength - 1);

            // Build chronological index helper
            int Idx(int i) => (_historyHead + i) % HistoryLength;

            // ?? GPU fill (bottom layer, from baseline up to gpuW/peak) ????????
            var gpuPts  = new PointF[HistoryLength];
            var cpuPts  = new PointF[HistoryLength];
            var basePts = new PointF[HistoryLength]; // bottom line (y+h)

            for (int i = 0; i < HistoryLength; i++)
            {
                int idx   = Idx(i);
                float px  = x + i * stepX;
                float gpuH = (_gpuHistory[idx] / peak) * h;
                float cpuH = (_cpuHistory[idx] / peak) * h;

                basePts[i] = new PointF(px, y + h);
                gpuPts[i]  = new PointF(px, y + h - gpuH);
                cpuPts[i]  = new PointF(px, y + h - gpuH - cpuH);  // stacked above GPU
            }

            // GPU polygon: gpuPts (top of GPU) + reversed basePts (bottom)
            FillArea(g, gpuPts, basePts, _gpuFillBrush);
            g.DrawLines(_gpuLinePen, gpuPts);

            // CPU polygon: cpuPts (top of CPU stack) + reversed gpuPts (bottom of CPU = top of GPU)
            FillArea(g, cpuPts, gpuPts, _cpuFillBrush);
            g.DrawLines(_cpuLinePen, cpuPts);

            // Total line (top of full stack) — subtle white tint
            using var totalPen = new Pen(Color.FromArgb(255, 200, 200, 200), sc * 0.75f)
            {
                DashStyle = DashStyle.Dot
            };
            g.DrawLines(totalPen, cpuPts);

            // Baseline
            using var axPen = new Pen(Color.FromArgb(255, 80, 80, 80), sc * 0.5f);
            g.DrawLine(axPen, x, y + h, x + w, y + h);
        }

        // Fill the area between topLine and bottomLine as a closed polygon
        private static void FillArea(Graphics g, PointF[] topLine, PointF[] bottomLine, SolidBrush brush)
        {
            int n     = topLine.Length;
            var poly  = new PointF[n * 2];
            topLine.CopyTo(poly, 0);
            // Bottom goes right-to-left to close the polygon correctly
            for (int i = 0; i < n; i++)
                poly[n + i] = bottomLine[n - 1 - i];
            g.FillPolygon(brush, poly);
        }

        // ?? Lifecycle ?????????????????????????????????????????????????????????
        private void PositionAtTopLeft()
        {
            Screen screen = Screen.PrimaryScreen ?? Screen.AllScreens[0];
            Location = new Point(screen.Bounds.X + MarginFromEdge, screen.Bounds.Y + MarginFromEdge);
        }

        public void StartOverlay()
        {
            _active = true;
            _fps?.Dispose();
            _fps = new FpsCounter();
            _fps.Start();

            float sc = GetDpiScale();
            Size = new Size(S(sc, BaseWidth),
                S(sc, BasePadY) * 2 + S(sc, BaseLineHeight) * 2 +
                S(sc, BaseLineSpacing) + S(sc, BaseChartGap) + S(sc, BaseChartHeight));

            PositionAtTopLeft();
            base.Show();
            Tick();
            _timer.Start();
        }

        public void StopOverlay()
        {
            _active = false;
            _timer.Stop();
            _fps?.Dispose();
            _fps = null;
            base.Hide();
        }
    }
}
