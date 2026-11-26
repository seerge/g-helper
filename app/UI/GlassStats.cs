using System.Drawing.Drawing2D;

namespace GHelper.UI
{
    /// <summary>
    /// Minimal live readout: CPU, GPU, RAM and battery as four compact tiles,
    /// each with a big value, a small caption and a thin usage bar.
    /// </summary>
    public class GlassStats : Control
    {
        public record struct Tile(string Name, string Value, string Detail, float? Level);

        private Tile[] tiles =
        {
            new("CPU", "--", "", null),
            new("GPU", "--", "", null),
            new("RAM", "--", "", null),
            new("BAT", "--", "", null),
        };

        public GlassStats()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            TabStop = false;
            AccessibleRole = AccessibleRole.StaticText;
            AccessibleName = "System usage";
        }

        public void SetTiles(params Tile[] values)
        {
            tiles = values;
            AccessibleDescription = string.Join(", ", values.Select(t => $"{t.Name} {t.Value} {t.Detail}"));
            Invalidate();
        }

        private static Color LevelColor(float level)
        {
            // Accent cyan normally, warming up as the load climbs
            if (level >= 0.9f) return Color.FromArgb(255, 255, 84, 84);
            if (level >= 0.7f) return Color.FromArgb(255, 255, 170, 40);
            return ControlHelper.GlassIconColor;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(BackColor);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            float s = DeviceDpi / 96f;
            int count = tiles.Length;
            if (count == 0) return;

            int gap = (int)Math.Round(8 * s);
            int tileW = (Width - gap * (count - 1)) / count;
            int barH = Math.Max(2, (int)Math.Round(3 * s));

            using var nameFont = new Font(Font.FontFamily, 7.5f, FontStyle.Bold);
            using var valueFont = new Font(Font.FontFamily, 13f, FontStyle.Regular);
            using var detailFont = new Font(Font.FontFamily, 7.5f, FontStyle.Regular);

            Color dim = Color.FromArgb(255, 150, 150, 165);
            Color track = Color.FromArgb(255, 44, 44, 54);

            for (int i = 0; i < count; i++)
            {
                var t = tiles[i];
                int x = i * (tileW + gap);
                var tile = new Rectangle(x, 0, tileW, Height);

                // Hairline divider between tiles
                if (i > 0)
                {
                    using var div = new Pen(Color.FromArgb(255, 46, 46, 56), Math.Max(1f, s));
                    int dx = x - gap / 2;
                    g.DrawLine(div, dx, Height * 0.18f, dx, Height * 0.82f);
                }

                int nameH = TextRenderer.MeasureText(g, t.Name, nameFont).Height;
                int valueH = TextRenderer.MeasureText(g, "0", valueFont).Height;

                var nameRect = new Rectangle(tile.X, tile.Y, tile.Width, nameH);
                TextRenderer.DrawText(g, t.Name, nameFont, nameRect, dim, BackColor,
                    TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.NoPadding);

                if (!string.IsNullOrEmpty(t.Detail))
                    TextRenderer.DrawText(g, t.Detail, detailFont, nameRect, dim, BackColor,
                        TextFormatFlags.Right | TextFormatFlags.Top | TextFormatFlags.NoPadding);

                var valueRect = new Rectangle(tile.X, nameRect.Bottom, tile.Width, valueH);
                TextRenderer.DrawText(g, t.Value, valueFont, valueRect, ForeColor, BackColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis);

                // Usage bar
                int barY = Height - barH - 1;
                var trackRect = new RectangleF(tile.X, barY, tile.Width, barH);
                using (var trackPath = Pill(trackRect))
                using (var trackBrush = new SolidBrush(track))
                    g.FillPath(trackBrush, trackPath);

                if (t.Level is float level && level > 0)
                {
                    level = Math.Clamp(level, 0f, 1f);
                    var fill = new RectangleF(tile.X, barY, Math.Max(barH, tile.Width * level), barH);
                    using var fillPath = Pill(fill);
                    // Battery is the other way round: a full battery is good, a low one is the warning
                    Color barColor = t.Name == "BAT" ? LevelColor(level <= 0.15f ? 1f : level <= 0.3f ? 0.75f : 0f) : LevelColor(level);
                    using var fillBrush = new SolidBrush(barColor);
                    g.FillPath(fillBrush, fillPath);
                }
            }
        }

        private static GraphicsPath Pill(RectangleF r)
        {
            var path = new GraphicsPath();
            float d = r.Height;
            if (r.Width <= d)
            {
                path.AddEllipse(r);
                return path;
            }
            path.AddArc(r.X, r.Y, d, d, 90, 180);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 180);
            path.CloseFigure();
            return path;
        }
    }
}
