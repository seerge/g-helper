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

        private const uint MONITOR_DEFAULTTOPRIMARY = 1;
        private const int MDT_EFFECTIVE_DPI = 0;
        private const int BaseDpi = 96;

        private string _line1 = "";
        private string _line2 = "";

        private const int PadX = 8;
        private const int PadY = 6;
        private const int LineSpacing = 4;
        private const int MarginFromEdge = 10;
        private const int CornerRadius = 6;
        private const float BaseFontSize = 13f;
        private const int BaseLineHeight = 18;
        private const int BaseWidth = 220;

        private static readonly SolidBrush _bgBrush = new SolidBrush(Color.FromArgb(180, Color.Black));
        private static readonly SolidBrush _textBrush = new SolidBrush(Color.FromArgb(0, 255, 80));

        private readonly System.Timers.Timer _timer = new System.Timers.Timer(1000) { AutoReset = true };

        public HardwareOverlay()
        {
            Alpha = 204; // ~80% opacity (204/255)
            _timer.Elapsed += (_, _) => RefreshSensors();
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

        private void RefreshSensors()
        {
            HardwareControl.ReadSensors();

            string cpuTemp = HardwareControl.cpuTemp > 0
                ? ": " + Math.Round((decimal)HardwareControl.cpuTemp) + "°C" : "";
            string cpuPower = HardwareControl.cpuPower > 0
                ? " " + Math.Round((decimal)HardwareControl.cpuPower, 1) + "W" : "";
            string gpuTemp = HardwareControl.gpuTemp > 0
                ? ": " + HardwareControl.gpuTemp + "°C" : "";
            string gpuPower = HardwareControl.gpuPower > 0
                ? " " + Math.Round((decimal)HardwareControl.gpuPower, 1) + "W" : "";

            _line1 = "CPU" + cpuTemp + cpuPower + " " + HardwareControl.cpuFan;
            _line2 = "GPU" + gpuTemp + gpuPower + " " + HardwareControl.gpuFan;

            Invalidate();
        }

        protected override void PerformPaint(PaintEventArgs e)
        {
            float scale = GetDpiScale();
            Font font = new Font("Consolas", BaseFontSize * scale, FontStyle.Regular, GraphicsUnit.Pixel);

            int lineHeight = (int)(BaseLineHeight * scale);
            int padX = (int)(PadX * scale);
            int padY = (int)(PadY * scale);
            int width = (int)(BaseWidth * scale);
            int height = padY * 2 + lineHeight * 2 + (int)(LineSpacing * scale);
            int radius = (int)(CornerRadius * scale);

            if (Size.Width != width || Size.Height != height)
                Size = new Size(width, height);

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            g.FillRoundedRectangle(_bgBrush, Bound, radius);
            g.DrawString(_line1, font, _textBrush, new PointF(padX, padY));
            g.DrawString(_line2, font, _textBrush, new PointF(padX, padY + lineHeight + (int)(LineSpacing * scale)));

            font.Dispose();
        }

        private void PositionAtTopLeft()
        {
            Screen screen = Screen.PrimaryScreen ?? Screen.AllScreens[0];
            Location = new Point(screen.Bounds.X + MarginFromEdge, screen.Bounds.Y + MarginFromEdge);
        }

        public void StartOverlay()
        {
            float scale = GetDpiScale();
            int width = (int)(BaseWidth * scale);
            int lineHeight = (int)(BaseLineHeight * scale);
            int padY = (int)(PadY * scale);
            Size = new Size(width, padY * 2 + lineHeight * 2 + (int)(LineSpacing * scale));

            PositionAtTopLeft();
            base.Show();
            RefreshSensors();
            _timer.Start();
        }

        public void StopOverlay()
        {
            _timer.Stop();
            base.Hide();
        }
    }
}
