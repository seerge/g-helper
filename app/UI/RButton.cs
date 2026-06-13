using System.Drawing.Drawing2D;

namespace GHelper.UI
{
    public class RButton : Button
    {

        // Design tokens
        private const float HoverShiftAmount = 0.04f;
        private const float ActiveTopLighten = 0.25f;
        private const float RestTopLighten = 0.1f;
        private const int ActiveBgTopAlpha = 32;
        private const float ActiveBgEndFraction = 0.20f;

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
                AccessibleDescription = activated ? "Active" : null;
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

        protected override bool ShowFocusCues => false;

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
            FlatAppearance.MouseOverBackColor = Shift(BackColor, target, HoverShiftAmount);
            FlatAppearance.MouseDownBackColor = Shift(BackColor, target, HoverShiftAmount * 2f);
        }

        private static Color Shift(Color from, Color target, float amount)
        {
            return Color.FromArgb(from.A,
                (int)(from.R + (target.R - from.R) * amount),
                (int)(from.G + (target.G - from.G) * amount),
                (int)(from.B + (target.B - from.B) * amount));
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
            int border = (int)(ratio * borderSize);
            int radius = (int)(ratio * borderRadius);

            Rectangle rectSurface = ClientRectangle;

            using (GraphicsPath pathSurface = GetFigurePath(rectSurface, radius + border))
            using (Pen penSurface = new Pen(Parent.BackColor, border))
            {
                pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Region = new Region(pathSurface);
                pevent.Graphics.DrawPath(penSurface, pathSurface);

                bool drawActive = Enabled && !Borderless && activated && borderColor.A > 0;
                bool drawRest = Enabled && !Borderless && !activated && FlatAppearance.BorderColor.A > 0 && !RForm.flatTheme;

                if (drawActive)
                {
                    Rectangle borderRect = new Rectangle(border, border, rectSurface.Width - 2 * border, rectSurface.Height - 2 * border);

                    Color bgTop = Color.FromArgb(ActiveBgTopAlpha, borderColor);
                    Color bgTransparent = Color.FromArgb(0, borderColor);
                    float bgEndPos = ActiveBgEndFraction;

                    using (GraphicsPath bgPath = GetFigurePath(borderRect, radius))
                    using (LinearGradientBrush bgBrush = new LinearGradientBrush(
                        new PointF(0, borderRect.Y), new PointF(0, borderRect.Bottom),
                        bgTop, bgTransparent))
                    {
                        bgBrush.InterpolationColors = new ColorBlend
                        {
                            Colors = new[] { bgTop, bgTransparent, bgTransparent },
                            Positions = new[] { 0f, bgEndPos, 1f }
                        };
                        pevent.Graphics.FillPath(bgBrush, bgPath);
                    }

                    ControlHelper.DrawGradientBorder(pevent.Graphics, borderRect, borderColor, radius, border, PenAlignment.Outset, ActiveTopLighten);
                }
                else if (drawRest)
                {
                    int inset = border / 2 + 1;
                    Rectangle borderRect = new Rectangle(inset, inset, rectSurface.Width - 2 * inset, rectSurface.Height - 2 * inset);
                    ControlHelper.DrawGradientBorder(pevent.Graphics, borderRect, FlatAppearance.BorderColor, radius + border - inset, 1f, PenAlignment.Inset, RestTopLighten);
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
