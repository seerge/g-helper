using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CustomControls
{
    public class RoundedButton : Button
    {
        //Fields
        private int borderSize = 5;
        private int borderRadius = 5;
        private bool activated = false;
        private Color borderColor = Color.Transparent;

        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
            }
        }


        public bool Activated
        {
            get { return activated; }
            set
            {
                if (activated != value)
                    this.Invalidate();
                activated = value;

            }
        }


        public RoundedButton()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
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

            Rectangle rectSurface = this.ClientRectangle;
            Rectangle rectBorder = Rectangle.Inflate(rectSurface, -border, -border);

            Color borderDrawColor = activated ? borderColor : Color.Transparent;

            using (GraphicsPath pathSurface = GetFigurePath(rectSurface, borderRadius+ border))
            using (GraphicsPath pathBorder = GetFigurePath(rectBorder, borderRadius))
            using (Pen penSurface = new Pen(this.Parent.BackColor, border))
            using (Pen penBorder = new Pen(borderDrawColor, border))
            {
                penBorder.Alignment = PenAlignment.Outset;
                pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                this.Region = new Region(pathSurface);
                pevent.Graphics.DrawPath(penSurface, pathSurface);
                pevent.Graphics.DrawPath(penBorder, pathBorder);
            }
        }

    }
}
