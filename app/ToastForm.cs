using System.Diagnostics;
using System.Drawing.Drawing2D;
using OSD;


namespace GHelper
{

    static class Drawing
    {

        public static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle bounds, int cornerRadius)
        {
            using (GraphicsPath path = RoundedRect(bounds, cornerRadius))
            {
                graphics.FillPath(brush, path);
            }
        }
    }

    public class  ToastForm : OSDNativeForm
    {

        protected static string toastText = "Balanced";
        protected static System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        public ToastForm()
        {
            timer.Tick += timer_Tick;
            timer.Enabled = false;
            timer.Interval = 2000;
        }

        protected override void PerformPaint(PaintEventArgs e)
        {
            Brush brush = new SolidBrush(Color.FromArgb(150,Color.Black));
            Drawing.FillRoundedRectangle(e.Graphics, brush, this.Bound, 10);

            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Center;

            e.Graphics.DrawString(toastText, 
                new Font("Segoe UI", 16f, FontStyle.Bold), 
                new SolidBrush(Color.White), 
                new PointF(this.Bound.Width/2, this.Bound.Height / 2), 
                format);
        }

        public void RunToast(string text)
        {
            Hide();

            toastText = text;
            Screen screen1 = Screen.FromHandle(base.Handle);

            Width = 300;
            Height = 100;
            X = (screen1.Bounds.Width - this.Width)/2;
            Y = screen1.Bounds.Height - 300 - this.Height;

            Show();

            timer.Enabled = true;
        }

        private void timer_Tick(object? sender, EventArgs e)
        {
            Debug.WriteLine("Toast end");
            timer.Enabled = false;
            Hide();
        }
    }
}
