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

        private const uint MONITOR_DEFAULTTOPRIMARY = 1;
        private const int MDT_EFFECTIVE_DPI = 0;
        private const int BaseDpi = 96;

        // ── Layout constants (base = 96 dpi) ─────────────────────────────────
        //
        // ┌─padX─┬─fpsCol─┬─gap─┬─── leftCol ────┬─gap─┬─chartCol─┬─pwrGap─┬─powCol─┬─padX─┐
        // │      │  194   │     │ GPU: 82° 5300RPM│     │  chart   │        │ 111.3W │      │
        // │      │  fps   │     │ CPU: 78° 4500RPM│     │          │        │  16.9W │      │
        // └──────┴────────┴─────┴────────────────┴─────┴──────────┴────────┴────────┴──────┘
        //
        // BaseWidth = 8 + 52 + 8 + 128 + 8 + 120 + 4 + 50 + 8 = 386
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

        private static readonly SolidBrush _bgBrush = new(Color.FromArgb(128, 0, 0, 0));
        private static readonly SolidBrush _gpuBrush = new(Color.FromArgb(255, 0, 255, 80));
        private static readonly SolidBrush _cpuBrush = new(Color.FromArgb(255, 60, 220, 255));
        private static readonly Pen _gpuLinePen = new(Color.FromArgb(255, 0, 255, 80), 1.5f);
        private static readonly Pen _cpuLinePen = new(Color.FromArgb(255, 60, 220, 255), 1.5f);
        private static readonly SolidBrush _gpuFillBrush = new(Color.FromArgb(128, 0, 85, 27));
        private static readonly SolidBrush _cpuFillBrush = new(Color.FromArgb(128, 20, 73, 85));

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
            int width = S(sc, BaseWidth);
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

            using var font = new Font("Consolas", BaseFontSize * sc, FontStyle.Regular, GraphicsUnit.Pixel);
            using var rpmFont = new Font("Consolas", BaseRpmFontSize * sc, FontStyle.Regular, GraphicsUnit.Pixel);
            using var fpsBold = new Font("Consolas", innerH / 1.15f, FontStyle.Bold, GraphicsUnit.Pixel);

            // Differential trick: MeasureString("XX") - MeasureString("X") cancels the
            // fixed GDI+ padding, giving the true per-character advance width for Consolas.
            float charW = g.MeasureString("XX", font).Width - g.MeasureString("X", font).Width;

            int leftX = padX + fpsColW + colGap;
            int chartX = leftX + S(sc, BaseLeftColWidth);
            int powX = chartX + chartColW + powGap;
            int topY = padY;

            // FPS
            string fpsStr = _currentFps > 0 ? _currentFps.ToString() : "--";
            float fpsW = g.MeasureString(fpsStr, fpsBold).Width;
            g.DrawString(fpsStr, fpsBold, _gpuBrush,
            new PointF(padX + (fpsColW - fpsW) / 2f, topY));

            // Left column: "GPU: 82° " then "5300" then "RPM" superscript
            DrawTempFan(g, font, rpmFont, charW, sc, leftX, topY, _gpuTempStr, _gpuFanNum, _gpuBrush);
            DrawTempFan(g, font, rpmFont, charW, sc, leftX, topY + lineH + lineGap, _cpuTempStr, _cpuFanNum, _cpuBrush);

            // Chart
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

        private void PositionAtTopLeft()
        {
            Screen screen = Screen.PrimaryScreen ?? Screen.AllScreens[0];
            Location = new Point(screen.Bounds.X + MarginFromEdge, screen.Bounds.Y + MarginFromEdge);
        }

        public void StartOverlay()
        {
            _active = true;
            _lastFgPid = 0;

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
            base.Hide();
        }
    }
}
