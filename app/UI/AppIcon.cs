using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace GHelper.UI
{
    /// <summary>
    /// C-Helper's icon: a glossy cyan glass bubble with a white "C". Drawn in code so it stays crisp at any size.
    /// </summary>
    public static class AppIcon
    {
        [DllImport("user32.dll")] static extern bool DestroyIcon(IntPtr handle);

        static readonly Color Top = Color.FromArgb(255, 110, 228, 255);
        static readonly Color Bottom = Color.FromArgb(255, 0, 150, 214);

        static readonly Lazy<Bitmap?> logo = new(() =>
        {
            try
            {
                using var stream = typeof(AppIcon).Assembly.GetManifestResourceStream("GHelper.chelper-logo.png");
                return stream is null ? null : new Bitmap(stream);
            }
            catch { return null; }
        });

        /// <summary>The C-Helper logo at the given size (falls back to a plain white C).</summary>
        public static Bitmap Render(int size)
        {
            if (logo.Value is Bitmap art)
            {
                var scaled = new Bitmap(size, size);
                using var sg = Graphics.FromImage(scaled);
                sg.InterpolationMode = InterpolationMode.HighQualityBicubic;
                sg.PixelOffsetMode = PixelOffsetMode.HighQuality;
                sg.DrawImage(art, 0, 0, size, size);
                return scaled;
            }

            var bmp = new Bitmap(size, size);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // Just a bold white "C": a thick arc, open to the right, on a transparent background
            float stroke = Math.Max(2f, size * 0.2f);
            float r = size / 2f - stroke / 2f - size * 0.06f;
            var c = new RectangleF(size / 2f - r, size / 2f - r, r * 2, r * 2);
            using (var pen = new Pen(Color.White, stroke) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                g.DrawArc(pen, c, 50, 260);

            return bmp;
        }

        /// <summary>Tray-sized icon at the current DPI.</summary>
        public static Icon Tray()
        {
            int size = Math.Max(16, (int)Math.Round(SystemInformation.SmallIconSize.Width * (Program.settingsForm?.DeviceDpi ?? 96) / 96f));
            size = Math.Max(size, SystemInformation.SmallIconSize.Width);
            using var bmp = Render(Math.Max(32, size));
            IntPtr h = bmp.GetHicon();
            var icon = (Icon)Icon.FromHandle(h).Clone();
            DestroyIcon(h);
            return icon;
        }
    }
}
