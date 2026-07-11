using System.Drawing.Drawing2D;

namespace GHelper.UI
{
    public class RColorButton : RButton
    {
        private Color? swatchColor;
        public Color? SwatchColor
        {
            get => swatchColor;
            set { if (swatchColor != value) { swatchColor = value; Invalidate(); } }
        }

        private Color? swatchColor2;
        public Color? SwatchColor2
        {
            get => swatchColor2;
            set { if (swatchColor2 != value) { swatchColor2 = value; Invalidate(); } }
        }

        public event EventHandler? Swatch2Click;

        private Rectangle swatch2Rect = Rectangle.Empty;
        private bool pressedSwatch2;

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            swatch2Rect = Rectangle.Empty;
            if (!swatchColor.HasValue && !swatchColor2.HasValue) return;

            Rectangle rect = ClientRectangle;
            int size = (int)Math.Round(rect.Height * 0.45f);
            int margin = (rect.Height - size) / 2;
            if (size <= 0) return;
            int radius = (int)Math.Round(pevent.Graphics.DpiX / 192.0f * BorderRadius, MidpointRounding.AwayFromZero);

            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int gap = (int)Math.Round(margin * 0.6f);
            int x = rect.Width - margin - size;
            if (swatchColor.HasValue && swatchColor.Value.A > 0)
            {
                DrawSwatch(g, new Rectangle(x, margin, size, size), radius, swatchColor.Value);
                x -= gap + size;
            }
            if (swatchColor2.HasValue && swatchColor2.Value.A > 0)
            {
                swatch2Rect = new Rectangle(x, margin, size, size);
                DrawSwatch(g, swatch2Rect, radius, swatchColor2.Value);
            }
        }

        private static void DrawSwatch(Graphics g, Rectangle rect, int radius, Color color)
        {
            int d = radius * 2;
            using var path = new GraphicsPath();
            if (d > 0)
            {
                path.AddArc(rect.X, rect.Y, d, d, 180, 90);
                path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
                path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
                path.CloseFigure();
            }
            else path.AddRectangle(rect);

            using var brush = new SolidBrush(color);
            using var pen = new Pen(RForm.borderMain);
            g.FillPath(brush, path);
            g.DrawPath(pen, path);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            pressedSwatch2 = swatchColor2.HasValue && swatch2Rect.Contains(e.Location);
            base.OnMouseDown(e);
        }

        // Suppress the normal Click (color1) when the second swatch was pressed.
        protected override void OnClick(EventArgs e)
        {
            if (pressedSwatch2)
            {
                pressedSwatch2 = false;
                Swatch2Click?.Invoke(this, e);
                return;
            }
            base.OnClick(e);
        }
    }
}
