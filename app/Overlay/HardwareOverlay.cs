using GHelper.Helpers;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace GHelper.Overlay
{
    public class HardwareOverlay : OSDNativeForm
    {
        // DPI
        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromPoint(POINT pt, uint dwFlags);
        [DllImport("shcore.dll")]
        private static extern int GetDpiForMonitor(IntPtr hMonitor, int dpiType, out uint dpiX, out uint dpiY);
        private const uint MONITOR_DEFAULTTOPRIMARY = 1;
        private const int MDT_EFFECTIVE_DPI = 0;
        private const int BaseDpi = 96;

        // ── Layout constants (base = 96 dpi) ─────────────────────────────────
        //
        //  ┌─padX─┬─fpsCol─┬─gap─┬─── leftCol ────┬─gap─┬─chartCol─┬─gap─┬─powCol─┬─padX─┐
        //  │      │  144   │     │ GPU:68° 2500RPM│     │  chart   │     │ 19.6W  │      │
        //  │      │  fps   │     │ CPU:53° 2100RPM│     │          │     │  6.7W  │      │
        //  └──────┴────────┴─────┴────────────────┴─────┴──────────┴─────┴────────┴──────┘
        //
        //  BaseWidth = 8 + 52 + 8 + 128 + 8 + 80 + 8 + 50 + 8 = 350
        //
        private const float BaseFontSize = 13f;
        private const int BaseLineHeight = 18;
        private const int BaseLineSpacing = 1;
        private const int BasePadX = 8;
        private const int BasePadY = 6;
        private const int BaseWidth = 350;
        private const int BaseFpsColWidth = 52;
        private const int BaseLeftColWidth = 128;  // temp + fan
        private const int BaseChartColWidth = 80;
        private const int BasePowerColWidth = 50;   // power draw (right of chart)
        private const int BaseColGap = 8;
        private const int CornerRadius = 3;
        private const int MarginFromEdge = 10;

        // ── Colours — GPU = green, CPU = cyan ────────────────────────────────
        private static readonly SolidBrush _bgBrush = new(Color.FromArgb(128, 0, 0, 0));
        private static readonly SolidBrush _gpuBrush = new(Color.FromArgb(255, 0, 255, 80));
        private static readonly SolidBrush _cpuBrush = new(Color.FromArgb(255, 60, 220, 255));
        private static readonly Pen _gpuLinePen = new(Color.FromArgb(255, 0, 255, 80), 1.5f);
        private static readonly Pen _cpuLinePen = new(Color.FromArgb(255, 60, 220, 255), 1.5f);
        private static readonly SolidBrush _gpuFillBrush = new(Color.FromArgb(128, 0, 255 / 3, 80 / 3));
        private static readonly SolidBrush _cpuFillBrush = new(Color.FromArgb(128, 60 / 3, 220 / 3, 255 / 3));

        // ── Data ─────────────────────────────────────────────────────────────
        private string _gpuLeft = "";   // "GPU: 68° 2500RPM"
        private string _cpuLeft = "";   // "CPU: 53° 2100RPM"
        private string _gpuPow = "";   // " 19.6W"
        private string _cpuPow = "";   // "  6.7W"

        private const int HistoryLength = 60;
        private readonly float[] _cpuHistory = new float[HistoryLength];
        private readonly float[] _gpuHistory = new float[HistoryLength];
        private int _historyHead = 0;

        // ── Timers / monitors ────────────────────────────────────────────────
        private readonly System.Timers.Timer _timer = new(1000) { AutoReset = true };
        private EtwFpsMonitor? _fps;
        private Task? _fpsTask;
        private volatile int _currentFps;
        private double _smoothFps;
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
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_NCDESTROY)
            {
                base.WndProc(ref m);
                if (_active)
                    Program.settingsForm?.BeginInvoke(() => { PositionAtTopLeft(); base.Show(); });
                return;
            }
            base.WndProc(ref m);
        }

        // ── DPI ──────────────────────────────────────────────────────────────
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

        // ── Safe value conversion ─────────────────────────────────────────────
        private static double D(object? v) { try { return v is null ? 0.0 : Convert.ToDouble(v); } catch { return 0.0; } }

        // ── Fixed-width formatters ────────────────────────────────────────────
        // Temp:  4 chars  →  " 68°"  / "100°"  / "    "
        // Power: 6 chars  →  "  3.1W" / "120.4W" / "      "
        // Fan:   7 chars  →  "   0RPM" / "6000RPM" / "       "
        private static string FmtTemp(double t) =>
            t > 0 ? ((int)Math.Round(t) + "°").PadLeft(4) : "    ";

        private static string FmtPow(double p) =>
            p > 0 ? (Math.Round(p, 1).ToString("F1") + "W").PadLeft(6) : "      ";

        private static string FmtFan(string? fan)
        {
            if (string.IsNullOrWhiteSpace(fan)) return "       ";
            var digits = new string(fan.Trim().TakeWhile(char.IsDigit).ToArray());
            if (int.TryParse(digits, out int rpm))
                return (rpm + "RPM").PadLeft(7);
            return fan.Trim().PadLeft(7);
        }

        // ── Tick ─────────────────────────────────────────────────────────────
        private void Tick()
        {
            if (Handle != nint.Zero)
                SetWindowPos(Handle, HWND_TOPMOST, 0, 0, 0, 0,
                    SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);

            HardwareControl.ReadSensorsOverlay();

            // Left column: label + temp + fan
            _gpuLeft = "GPU:" + FmtTemp(D(HardwareControl.gpuTemp))
                     + " " + FmtFan(HardwareControl.gpuFan);
            _cpuLeft = "CPU:" + FmtTemp(D(HardwareControl.cpuTemp))
                     + " " + FmtFan(HardwareControl.cpuFan);

            // Power column: right of chart
            _gpuPow = FmtPow(D(HardwareControl.gpuPower));
            _cpuPow = FmtPow(D(HardwareControl.cpuPower));

            _cpuHistory[_historyHead] = (float)Math.Max(0, D(HardwareControl.cpuPower));
            _gpuHistory[_historyHead] = (float)Math.Max(0, D(HardwareControl.gpuPower));
            _historyHead = (_historyHead + 1) % HistoryLength;

            Invalidate();
        }

        // ── Paint ────────────────────────────────────────────────────────────
        protected override void PerformPaint(PaintEventArgs e)
        {
            float sc = GetDpiScale();

            int padX = S(sc, BasePadX);
            int padY = S(sc, BasePadY);
            int lineH = S(sc, BaseLineHeight);
            int lineGap = S(sc, BaseLineSpacing);
            int width = S(sc, BaseWidth);
            int radius = S(sc, CornerRadius);
            int fpsColW = S(sc, BaseFpsColWidth);
            int leftColW = S(sc, BaseLeftColWidth);
            int chartColW = S(sc, BaseChartColWidth);
            int powColW = S(sc, BasePowerColWidth);
            int colGap = S(sc, BaseColGap);

            int innerH = lineH * 2 + lineGap;
            int totalH = padY * 2 + innerH;

            if (Size.Width != width || Size.Height != totalH)
                Size = new Size(width, totalH);

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            g.FillRoundedRectangle(_bgBrush, Bound, radius);

            using var font = new Font("Consolas", BaseFontSize * sc, FontStyle.Regular, GraphicsUnit.Pixel);
            using var fpsBold = new Font("Consolas", innerH / 1.15f, FontStyle.Bold, GraphicsUnit.Pixel);

            // Column X origins
            int leftX = padX + fpsColW + colGap;
            int chartX = leftX + leftColW + colGap;
            int powX = chartX + chartColW + colGap;
            int topY = padY;

            // ── FPS — horizontally centred in fpsCol ──────────────────────────
            string fpsStr = _currentFps > 0 ? _currentFps.ToString() : "--";
            float fpsW = g.MeasureString(fpsStr, fpsBold).Width;
            g.DrawString(fpsStr, fpsBold, _gpuBrush,
                new PointF(padX + (fpsColW - fpsW) / 2f, topY));

            // ── Left column: temp + fan ───────────────────────────────────────
            g.DrawString(_gpuLeft, font, _gpuBrush, new PointF(leftX, topY));
            g.DrawString(_cpuLeft, font, _cpuBrush, new PointF(leftX, topY + lineH + lineGap));

            // ── Chart ─────────────────────────────────────────────────────────
            DrawStackedChart(g, chartX, topY, chartColW, innerH, sc);

            // ── Power column: right-aligned in powCol ─────────────────────────
            float gpuPowW = g.MeasureString(_gpuPow, font).Width;
            float cpuPowW = g.MeasureString(_cpuPow, font).Width;
            g.DrawString(_gpuPow, font, _gpuBrush,
                new PointF(powX + powColW - gpuPowW, topY));
            g.DrawString(_cpuPow, font, _cpuBrush,
                new PointF(powX + powColW - cpuPowW, topY + lineH + lineGap));
        }

        // ── Stacked power chart ───────────────────────────────────────────────
        // CPU at bottom (cyan), GPU stacked on top (green)
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

            var basePts = new PointF[HistoryLength];
            var cpuPts = new PointF[HistoryLength];
            var gpuPts = new PointF[HistoryLength];

            for (int i = 0; i < HistoryLength; i++)
            {
                int idx = Idx(i);
                float px = x + i * stepX;
                float cpuH = (_cpuHistory[idx] / peak) * h;
                float gpuH = (_gpuHistory[idx] / peak) * h;

                basePts[i] = new PointF(px, y + h);
                cpuPts[i] = new PointF(px, y + h - cpuH);
                gpuPts[i] = new PointF(px, y + h - cpuH - gpuH);
            }

            FillArea(g, cpuPts, basePts, _cpuFillBrush);
            g.DrawLines(_cpuLinePen, cpuPts);

            FillArea(g, gpuPts, cpuPts, _gpuFillBrush);
            g.DrawLines(_gpuLinePen, gpuPts);

            using var totalPen = new Pen(Color.FromArgb(255, 200, 200, 200), sc * 0.75f)
            { DashStyle = DashStyle.Dot };
            g.DrawLines(totalPen, gpuPts);

            using var axPen = new Pen(Color.FromArgb(255, 80, 80, 80), sc * 0.5f);
            g.DrawLine(axPen, x, y + h, x + w, y + h);
        }

        private static void FillArea(Graphics g, PointF[] topLine, PointF[] bottomLine, SolidBrush brush)
        {
            int n = topLine.Length;
            var poly = new PointF[n * 2];
            topLine.CopyTo(poly, 0);
            for (int i = 0; i < n; i++)
                poly[n + i] = bottomLine[n - 1 - i];
            g.FillPolygon(brush, poly);
        }

        // ── Lifecycle ────────────────────────────────────────────────────────
        private void PositionAtTopLeft()
        {
            Screen screen = Screen.PrimaryScreen ?? Screen.AllScreens[0];
            Location = new Point(screen.Bounds.X + MarginFromEdge, screen.Bounds.Y + MarginFromEdge);
        }

        public void StartOverlay()
        {
            _active = true;

            _fps?.Dispose();
            _currentFps = 0;
            _fps = new EtwFpsMonitor();
            _fps.FpsUpdated += d =>
            {
                _smoothFps = _smoothFps < 1.0 ? d : _smoothFps * 0.7 + d * 0.3;
                _currentFps = (int)Math.Round(_smoothFps);
            };
            _fpsTask = Task.Run(() => _fps.Start());

            float sc = GetDpiScale();
            int innerH = S(sc, BaseLineHeight) * 2 + S(sc, BaseLineSpacing);
            Size = new Size(S(sc, BaseWidth), S(sc, BasePadY) * 2 + innerH);

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
            _currentFps = 0;
            _smoothFps = 0;
            base.Hide();
        }
    }
}
