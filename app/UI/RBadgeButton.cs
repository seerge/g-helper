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

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            if (badge <= 0) return;

            float ratio = pevent.Graphics.DpiX / 192.0f;
            var rectSurface = ClientRectangle;

            using (Brush brush = new SolidBrush(BorderColor))
            {
                var radius = ratio * 14;
                var badgeRect = new RectangleF(
                    rectSurface.Width - rectSurface.Height / 2f - radius,
                    rectSurface.Height / 2f - radius,
                    radius + radius,
                    radius + radius
                );

                pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                pevent.Graphics.FillEllipse(brush, badgeRect);

                using (Font badgeFont = new Font("Arial", (float)(0.8 * Font.Size), FontStyle.Bold))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    badgeRect.Width = (float)Math.Ceiling(badgeRect.Width*ratio)/ratio;
                    badgeRect.Height = (float)Math.Ceiling(badgeRect.Height*ratio)/ratio;
                    pevent.Graphics.DrawString(badge.ToString(), badgeFont, Brushes.White, badgeRect, sf);
                }
            }
        }
    }
}