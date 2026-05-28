using System.Drawing.Drawing2D;

namespace GHelper.UI
{
    public class RButton : Button
    {

        //Fields
        private int borderSize = 5;

        private int borderRadius = 5;
        public int BorderRadius
        {
            get { return borderRadius; }
            set
            {
                borderRadius = value;
            }
        }

        private Color borderColor = Color.Transparent;
        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
            }
        }


        private bool activated = false;
        public bool Activated
        {
            get { return activated; }
            set
            {
                if (activated != value)
                    Invalidate();
                activated = value;

            }
        }

        private bool secondary = false;
        public bool Secondary
        {
            get { return secondary; }
            set
            {
                secondary = value;
            }
        }

        public RButton()
        {
            DoubleBuffered = true;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
        }

        private GraphicsPath GetFigurePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            float curveSize = radius * 2F;

            path.StartFigure();
            path.AddArc(rect.X, rect.Y, curveSize, curveSize, 180, 90);
            path.AddArc(rect.Right - curveSize, rect.Y, curveSize, curveSize, 270, 90);
            path.AddArc(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize, 0, 90);
            path.AddArc(rect.X, rect.Bottom - curveSize, curveSize, curveSize, 90, 90);
            path.CloseFigure();
            return path;
        }


        protected override void OnPaint(PaintEventArgs pevent)
        {

            base.OnPaint(pevent);

            float ratio = pevent.Graphics.DpiX / 192.0f;
            int border = (int)Math.Round(ratio * borderSize, MidpointRounding.AwayFromZero);
            int radius = (int)Math.Round(ratio * borderRadius, MidpointRounding.AwayFromZero);

            Rectangle rectSurface = ClientRectangle;
            Rectangle rectBorder = Rectangle.Inflate(rectSurface, -border, -border);

            Color borderDrawColor = activated ? borderColor : Color.Transparent;
            Color restBorderColor = (!activated && FlatAppearance.BorderColor.A > 0)
                ? FlatAppearance.BorderColor
                : Color.Transparent;

            using (GraphicsPath pathSurface = GetFigurePath(rectSurface, radius + border))
            using (GraphicsPath pathBorder = GetFigurePath(rectBorder, radius))
            using (Pen penSurface = new Pen(Parent.BackColor, border))
            using (Pen penBorder = new Pen(borderDrawColor, border))
            {
                penBorder.Alignment = PenAlignment.Outset;
                pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Region = new Region(pathSurface);
                pevent.Graphics.DrawPath(penSurface, pathSurface);
                pevent.Graphics.DrawPath(penBorder, pathBorder);

                if (restBorderColor.A > 0)
                {
                    float halfF = border / 2f;
                    float edgeRadius = radius + halfF;
                    float edgeCurve = edgeRadius * 2f;
                    float ex = rectSurface.X + halfF;
                    float ey = rectSurface.Y + halfF;
                    float ew = rectSurface.Width - border;
                    float eh = rectSurface.Height - border;

                    Color halfAlpha = Color.FromArgb(restBorderColor.A / 2, restBorderColor);

                    using (GraphicsPath pathRest = new GraphicsPath())
                    using (Pen penHalf = new Pen(halfAlpha, 1f))
                    using (Pen penTop = new Pen(restBorderColor, 1f))
                    {
                        pathRest.AddLine(ex + ew, ey + edgeRadius, ex + ew, ey + eh - edgeRadius);
                        pathRest.AddArc(ex + ew - edgeCurve, ey + eh - edgeCurve, edgeCurve, edgeCurve, 0, 90);
                        pathRest.AddArc(ex, ey + eh - edgeCurve, edgeCurve, edgeCurve, 90, 90);
                        pathRest.AddLine(ex, ey + eh - edgeRadius, ex, ey + edgeRadius);

                        pevent.Graphics.DrawPath(penHalf, pathRest);
                        pevent.Graphics.DrawLine(penTop, ex + edgeRadius, ey, ex + ew - edgeRadius, ey);
                    }

                    using (LinearGradientBrush brushTL = new LinearGradientBrush(
                        new PointF(ex + edgeRadius, ey), new PointF(ex, ey + edgeRadius),
                        restBorderColor, halfAlpha))
                    using (Pen penTL = new Pen(brushTL, 1f))
                        pevent.Graphics.DrawArc(penTL, ex, ey, edgeCurve, edgeCurve, 180, 90);

                    using (LinearGradientBrush brushTR = new LinearGradientBrush(
                        new PointF(ex + ew - edgeRadius, ey), new PointF(ex + ew, ey + edgeRadius),
                        restBorderColor, halfAlpha))
                    using (Pen penTR = new Pen(brushTR, 1f))
                        pevent.Graphics.DrawArc(penTR, ex + ew - edgeCurve, ey, edgeCurve, edgeCurve, 270, 90);
                }
            }

            if (!Enabled && ForeColor != SystemColors.ControlText)
            {
                var rect = pevent.ClipRectangle;
                if (Image is not null)
                {
                    rect.Y += Image.Height;
                    rect.Height -= Image.Height;
                }
                TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak;
                TextRenderer.DrawText(pevent.Graphics, Text, Font, rect, Color.Gray, flags);
            }


        }

    }
}
