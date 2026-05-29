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

        public bool Borderless { get; set; } = false;

        public RButton()
        {
            DoubleBuffered = true;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColorChanged += (s, e) => UpdateHoverColor();
            UpdateHoverColor();
        }

        private void UpdateHoverColor()
        {
            int lum = (BackColor.R * 30 + BackColor.G * 59 + BackColor.B * 11) / 100;
            Color target = lum > 128 ? Color.Black : Color.White;
            const float amount = 0.06f;
            FlatAppearance.MouseOverBackColor = Color.FromArgb(BackColor.A,
                (int)(BackColor.R + (target.R - BackColor.R) * amount),
                (int)(BackColor.G + (target.G - BackColor.G) * amount),
                (int)(BackColor.B + (target.B - BackColor.B) * amount));
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

            Color restBorderColor = (!activated && FlatAppearance.BorderColor.A > 0)
                ? FlatAppearance.BorderColor
                : Color.Transparent;

            using (GraphicsPath pathSurface = GetFigurePath(rectSurface, radius + border))
            using (Pen penSurface = new Pen(Parent.BackColor, border))
            {
                pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Region = new Region(pathSurface);
                pevent.Graphics.DrawPath(penSurface, pathSurface);

                bool drawActive = !Borderless && activated && borderColor.A > 0;
                bool drawRest = !Borderless && !activated && restBorderColor.A > 0;

                if (drawActive || drawRest)
                {
                    const float topLighten = 0.15f;

                    Color topColor, sideColor;
                    float strokeWidth;
                    if (drawActive)
                    {
                        sideColor = borderColor;
                        strokeWidth = border;
                    }
                    else
                    {
                        sideColor = restBorderColor;
                        strokeWidth = 1f;
                    }
                    topColor = Color.FromArgb(sideColor.A,
                        (int)(sideColor.R + (255 - sideColor.R) * topLighten),
                        (int)(sideColor.G + (255 - sideColor.G) * topLighten),
                        (int)(sideColor.B + (255 - sideColor.B) * topLighten));

                    float rectX, rectY, rectW, rectH, strokeRadiusF;
                    PenAlignment alignment;
                    if (drawActive)
                    {
                        rectX = rectSurface.X + border;
                        rectY = rectSurface.Y + border;
                        rectW = rectSurface.Width - 2 * border;
                        rectH = rectSurface.Height - 2 * border;
                        strokeRadiusF = radius;
                        alignment = PenAlignment.Outset;
                    }
                    else
                    {
                        float halfF = border / 2f;
                        rectX = rectSurface.X + halfF;
                        rectY = rectSurface.Y + halfF;
                        rectW = rectSurface.Width - border;
                        rectH = rectSurface.Height - border;
                        strokeRadiusF = radius + halfF;
                        alignment = PenAlignment.Center;
                    }
                    float curveSize = strokeRadiusF * 2f;

                    float flatHeight = drawActive ? 2f * ratio : 1f;
                    float gradHeight = drawActive ? 20f * ratio : 20f * ratio;
                    float h = rectSurface.Height;
                    float pad = strokeWidth;
                    float axisStart = -pad;
                    float axisEnd = h + pad;
                    float axisLen = axisEnd - axisStart;
                    float p1 = Math.Max(0f, Math.Min(0.98f, (pad + border + radius + flatHeight) / axisLen));
                    float p2 = Math.Max(p1 + 0.01f, Math.Min(1f, (pad + border + radius + flatHeight + gradHeight) / axisLen));

                    using (GraphicsPath pathStroke = new GraphicsPath())
                    using (LinearGradientBrush brush = new LinearGradientBrush(
                        new PointF(0, axisStart), new PointF(0, axisEnd),
                        topColor, sideColor))
                    {
                        pathStroke.AddArc(rectX, rectY, curveSize, curveSize, 180, 90);
                        pathStroke.AddArc(rectX + rectW - curveSize, rectY, curveSize, curveSize, 270, 90);
                        pathStroke.AddArc(rectX + rectW - curveSize, rectY + rectH - curveSize, curveSize, curveSize, 0, 90);
                        pathStroke.AddArc(rectX, rectY + rectH - curveSize, curveSize, curveSize, 90, 90);
                        pathStroke.CloseFigure();

                        brush.InterpolationColors = new ColorBlend(4)
                        {
                            Colors = new[] { topColor, topColor, sideColor, sideColor },
                            Positions = new[] { 0f, p1, p2, 1f }
                        };

                        using (Pen pen = new Pen(brush, strokeWidth) { Alignment = alignment })
                            pevent.Graphics.DrawPath(pen, pathStroke);
                    }
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
