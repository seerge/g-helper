using System.Drawing.Drawing2D;

namespace GHelper.UI
{
    public class RBadgeButton : RButton
    {
        private int badge = 0;
        public int Badge
        {
            get => badge;
            set
            {
                if (badge != value)
                {
                    badge = value;
                    Invalidate();
                }
            }
        }

        private float pop = 1f;
        private int popStep;
        private static readonly float[] popTargets = { 1.25f, 1f, 1.2f, 1f };
        private readonly System.Windows.Forms.Timer popTimer = new() { Interval = 30 };

        public RBadgeButton()
        {
            popTimer.Tick += delegate
            {
                var target = popTargets[popStep];
                pop += (target - pop) * 0.3f;
                if (Math.Abs(target - pop) < 0.03f)
                {
                    pop = target;
                    if (++popStep == popTargets.Length) { popStep = 0; popTimer.Stop(); }
                }
                Invalidate();
            };
        }

        public void Pop(int value)
        {
            if (badge == 0) pop = 0f;
            Badge = value;
            popTimer.Start();
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            if (badge <= 0) return;

            float ratio = pevent.Graphics.DpiX / 192.0f;
            var rectSurface = ClientRectangle;

            using (Brush brush = new SolidBrush(BorderColor))
            {
                var radius = ratio * 14 * pop;
                var badgeRect = new RectangleF(
                    rectSurface.Width - rectSurface.Height / 2f - radius,
                    rectSurface.Height / 2f - radius,
                    radius + radius,
                    radius + radius
                );

                pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                pevent.Graphics.FillEllipse(brush, badgeRect);

                using (GraphicsPath path = new GraphicsPath())
                using (FontFamily family = new FontFamily("Segoe UI"))
                using (StringFormat sf = StringFormat.GenericTypographic)
                {
                    path.AddString(badge.ToString(), family, (int)FontStyle.Bold, 100f, PointF.Empty, sf);
                    path.Flatten();

                    RectangleF ink = path.GetBounds();
                    float scale = radius * 1.1f / ink.Height;
                    float anchorX = ink.X + ink.Width / 2f + (badge == 1 ? ink.Width * 0.10f : 0f);

                    using (System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix())
                    {
                        m.Translate(badgeRect.X + badgeRect.Width / 2f, badgeRect.Y + badgeRect.Height / 2f);
                        m.Scale(scale, scale);
                        m.Translate(-anchorX, -(ink.Y + ink.Height / 2f));
                        path.Transform(m);
                    }
                    pevent.Graphics.FillPath(Brushes.White, path);
                }
            }
        }
    }
}