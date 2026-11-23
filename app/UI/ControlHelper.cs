using GHelper.UI;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms.DataVisualization.Charting;

public static class ControlHelper
{

    [DllImport("uxtheme.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
    private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string? pszSubIdList);

    static bool _invert = false;
    static bool _darkMode = false;
    static float _scale = 1;

    public static float Scale => _scale;
    public static bool DarkMode => _darkMode;

    public static void Adjust(RForm container, bool invert = false)
    {

        container.BackColor = RForm.formBack;
        container.ForeColor = RForm.foreMain;

        _invert = invert;
        _darkMode = container.darkTheme;
        AdjustControls(container.Controls);
        _invert = false;

    }

    public static void Resize(RForm container, float baseScale = 2)
    {
        _scale = GetDpiScale(container).Value / baseScale;
        if (Math.Abs(_scale - 1) > 0.2) ResizeControls(container.Controls);

    }

    private static void ResizeControls(Control.ControlCollection controls)
    {
        foreach (Control control in controls)
        {
            var button = control as RButton;
            if (button != null && button.Image is not null)
                button.Image = ResizeImage(button.Image);

            /*
            var pictureBox = control as PictureBox;
            if (pictureBox != null && pictureBox.BackgroundImage is not null)
                pictureBox.BackgroundImage = ResizeImage(pictureBox.BackgroundImage);
            */

            ResizeControls(control.Controls);
        }
    }


    private static void AdjustControls(Control.ControlCollection controls)
    {
        foreach (Control control in controls)
        {

            AdjustControls(control.Controls);

            if (RForm.glassTheme && GlassFontFamily is not null && control.Font.Name == "Segoe UI")
                control.Font = new Font(GlassFontFamily, control.Font.Size, control.Font.Style);

            var button = control as RButton;
            if (button != null)
            {
                button.BackColor = button.Secondary ? RForm.buttonSecond : RForm.buttonMain;
                button.ForeColor = RForm.foreMain;

                button.FlatStyle = FlatStyle.Flat;
                if (RForm.glassTheme)
                {
                    // Liquid glass: soft pill-ish shapes, no resting outline (only the active state gets a rim)
                    button.BorderRadius = Math.Max(button.BorderRadius, 18);
                    button.FlatAppearance.BorderColor = button.BackColor;
                }
                else if (!button.Borderless)
                    button.FlatAppearance.BorderColor = button.Secondary ? RForm.borderSecond : RForm.borderMain;

                if (button.Image is not null && _invert)
                    button.Image = AdjustImage(button.Image);
            }

            if (button != null && RForm.glassTheme && button.Image is not null)
                button.Image = GlassIcon(button.Image);

            var pictureBox = control as PictureBox;
            if (pictureBox != null && RForm.glassTheme)
            {
                pictureBox.BackgroundImageChanged -= GlassPicture_Changed;
                pictureBox.BackgroundImageChanged += GlassPicture_Changed;
                if (pictureBox.BackgroundImage is not null)
                    pictureBox.BackgroundImage = GlassIcon(pictureBox.BackgroundImage);
            }
            else if (pictureBox != null && pictureBox.BackgroundImage is not null && _invert)
                pictureBox.BackgroundImage = AdjustImage(pictureBox.BackgroundImage);


            var combo = control as RComboBox;
            if (combo != null)
            {
                combo.BackColor = RForm.buttonMain;
                combo.ForeColor = RForm.foreMain;
                combo.BorderColor = RForm.borderMain;
                combo.ButtonColor = RForm.buttonMain;
                combo.ArrowColor = RForm.foreMain;
            }
            var rNumeric = control as RNumericUpDown;
            if (rNumeric is not null)
            {
                rNumeric.ApplyTheme(_darkMode);
            }
            else if (control is NumericUpDown numbericUpDown)
            {
                numbericUpDown.ForeColor = RForm.foreMain;
                numbericUpDown.BackColor = RForm.buttonMain;
            }

            var rText = control as RTextBox;
            if (rText is not null)
            {
                rText.ApplyTheme(_darkMode);
            }

            var gb = control as GroupBox;
            if (gb != null)
            {
                gb.ForeColor = RForm.foreMain;
            }

            var pn = control as Panel;
            if (pn != null && pn.Name.Contains("Header"))
            {
                pn.BackColor = RForm.buttonSecond;
            }

            var sl = control as Slider;
            if (sl != null)
            {
                sl.borderColor = RForm.buttonMain;
            }

            var chk = control as CheckBox;
            if (chk != null)
            {
                bool inheritsCard = RForm.glassTheme && chk.Parent is not null && chk.BackColor == chk.Parent.BackColor;
                if (chk.BackColor != RForm.formBack && !inheritsCard)
                {
                    chk.BackColor = RForm.buttonSecond;
                    if (chk is RCheckBox)
                        chk.FlatAppearance.BorderColor = RForm.borderSecond;
                }
                SetWindowTheme(chk.Handle, _darkMode ? "DarkMode_Explorer" : "Explorer", null);
            }

            var chart = control as Chart;
            if (chart != null)
            {
                chart.BackColor = RForm.chartMain;
                chart.ChartAreas[0].BackColor = RForm.chartMain;

                chart.ChartAreas[0].AxisX.TitleForeColor = RForm.foreMain;
                chart.ChartAreas[0].AxisY.TitleForeColor = RForm.foreMain;

                chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = RForm.foreMain;
                chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = RForm.foreMain;

                chart.ChartAreas[0].AxisX.MajorTickMark.LineColor = RForm.foreMain;
                chart.ChartAreas[0].AxisY.MajorTickMark.LineColor = RForm.foreMain;

                chart.ChartAreas[0].AxisX.MajorGrid.LineColor = RForm.chartGrid;
                chart.ChartAreas[0].AxisY.MajorGrid.LineColor = RForm.chartGrid;
                chart.ChartAreas[0].AxisX.LineColor = RForm.chartGrid;
                chart.ChartAreas[0].AxisY.LineColor = RForm.chartGrid;

                chart.Titles[0].ForeColor = RForm.foreMain;

            }

        }
    }

    public static Lazy<float> GetDpiScale(Control control)
    {
        return new Lazy<float>(() =>
        {
            using (var graphics = control.CreateGraphics())
                return graphics.DpiX / 96.0f;
        });
    }

    private static Image ResizeImage(Image image)
    {
        return ResizeImage(image, _scale);
    }

    public static Image ResizeImage(Image image, float scale)
    {
        if (Math.Abs(scale - 1) < 0.1) return image;

        var newSize = new Size((int)(image.Width * scale), (int)(image.Height * scale));
        var pic = new Bitmap(newSize.Width, newSize.Height);

        using (var g = Graphics.FromImage(pic))
        {
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(image, new Rectangle(new Point(), newSize));
        }
        return pic;
    }

    // Design tokens
    private const float GradientHeightFraction = 0.3f;
    private const float LightGradientHeightFraction = 0.9f;
    private const int TopFadeAlpha = 64;

    public static void DrawGradientBorder(Graphics g, Rectangle bounds, Color sideColor, int radius, float strokeWidth = 1f, PenAlignment alignment = PenAlignment.Center, float topLighten = 0.1f)
    {
        Color topColor = !_darkMode && strokeWidth <= 1f
            ? Color.FromArgb(TopFadeAlpha, sideColor)
            : Color.FromArgb(sideColor.A,
                (int)(sideColor.R + (255 - sideColor.R) * topLighten),
                (int)(sideColor.G + (255 - sideColor.G) * topLighten),
                (int)(sideColor.B + (255 - sideColor.B) * topLighten));

        float flatHeight = Math.Max(1f, strokeWidth);
        float gradHeight = (float)Math.Round(bounds.Height * (_darkMode ? GradientHeightFraction : LightGradientHeightFraction));
        float pad = strokeWidth;
        float axisStart = bounds.Y - pad;
        float axisEnd = bounds.Y + bounds.Height + pad;
        float axisLen = axisEnd - axisStart;
        float p1 = Math.Max(0f, Math.Min(0.98f, (pad + flatHeight) / axisLen));
        float p2 = Math.Max(p1 + 0.01f, Math.Min(1f, (pad + flatHeight + gradHeight) / axisLen));

        using (GraphicsPath path = RComboBox.RoundedRect(bounds, radius, radius))
        using (LinearGradientBrush brush = new LinearGradientBrush(
            new PointF(0, axisStart), new PointF(0, axisEnd),
            topColor, sideColor))
        {
            brush.InterpolationColors = new ColorBlend(4)
            {
                Colors = new[] { topColor, topColor, sideColor, sideColor },
                Positions = new[] { 0f, p1, p2, 1f }
            };
            using (Pen pen = new Pen(brush, strokeWidth) { Alignment = alignment })
            {
                SmoothingMode prev = g.SmoothingMode;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawPath(pen, path);
                g.SmoothingMode = prev;
            }
        }
    }

    // Windows 11's modern UI font, when available
    private static readonly Lazy<FontFamily?> _glassFont = new(() =>
        FontFamily.Families.FirstOrDefault(f => f.Name == "Segoe UI Variable Text"));
    public static FontFamily? GlassFontFamily => _glassFont.Value;

    // Glass card tokens (logical pixels, scaled by DPI)
    private const int CardRadius = 16;
    private const int CardGap = 3;
    private const int CardBottomPad = 6;

    /// <summary>
    /// Turns a docked section panel into a frosted glass card: rounded, with a soft top sheen
    /// and a light-catching rim. Outside the card is painted pure black, which reads as clear glass.
    /// </summary>
    public static void MakeGlassCard(Panel panel)
    {
        if (!RForm.glassTheme || panel.Tag as string == "glass-card") return;
        panel.Tag = "glass-card";

        float s = panel.DeviceDpi / 96f;
        var p = panel.Padding;
        int minTop = (int)Math.Round((CardGap + 6) * s);
        int minBottom = (int)Math.Round(CardBottomPad * s);
        panel.Padding = new Padding(p.Left, Math.Max(p.Top, minTop), p.Right, Math.Max(p.Bottom, minBottom));

        panel.BackColor = RForm.glassCard;
        panel.Resize += (_, _) => panel.Invalidate();
        panel.Paint += GlassCard_Paint;
    }

    private static void GlassCard_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Panel panel || panel.Width < 4 || panel.Height < 4) return;

        float s = panel.DeviceDpi / 96f;
        int gap = (int)Math.Round(CardGap * s);
        int radius = (int)Math.Round(CardRadius * s);

        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        var card = new Rectangle(0, gap, panel.Width - 1, panel.Height - 1 - gap * 2);
        if (card.Height < radius * 2) return;

        using var cardPath = RComboBox.RoundedRect(card, radius, radius);

        // Clear everything outside the rounded card back to glass
        using (var outside = new GraphicsPath(FillMode.Alternate))
        {
            outside.AddRectangle(new Rectangle(-1, -1, panel.Width + 2, panel.Height + 2));
            outside.AddPath(cardPath, false);
            using var clear = new SolidBrush(RForm.formBack);
            g.FillPath(clear, outside);
        }

        // Liquid sheen: a soft light pool along the top edge
        var sheenRect = new Rectangle(card.X, card.Y, card.Width, Math.Max(1, (int)(card.Height * 0.45f)));
        using (var sheen = new LinearGradientBrush(sheenRect,
            Color.FromArgb(22, RForm.glassHighlight), Color.FromArgb(0, RForm.glassHighlight), LinearGradientMode.Vertical))
        {
            var state = g.Save();
            g.SetClip(cardPath);
            g.FillRectangle(sheen, sheenRect);
            g.Restore(state);
        }

        // Rim: bright where light hits the top, fading down the sides
        using (var rim = new LinearGradientBrush(new Rectangle(card.X, card.Y - 1, card.Width, card.Height + 2),
            Color.FromArgb(255, 92, 92, 106), Color.FromArgb(255, 38, 38, 46), LinearGradientMode.Vertical))
        {
            rim.InterpolationColors = new ColorBlend
            {
                Colors = new[] { Color.FromArgb(255, 120, 124, 142), Color.FromArgb(255, 56, 58, 70), Color.FromArgb(255, 40, 41, 52) },
                Positions = new[] { 0f, 0.25f, 1f }
            };
            using var pen = new Pen(rim, Math.Max(1f, s)) { Alignment = PenAlignment.Inset };
            g.DrawPath(pen, cardPath);
        }
    }

    private static readonly ImageAttributes _invertAttributes = CreateInvertAttributes();

    private static ImageAttributes CreateInvertAttributes()
    {
        var matrix = new ColorMatrix(new[]
        {
            new float[] { -1,  0,  0, 0, 0 },
            new float[] {  0, -1,  0, 0, 0 },
            new float[] {  0,  0, -1, 0, 0 },
            new float[] {  0,  0,  0, 1, 0 },
            new float[] {  1,  1,  1, 0, 1 }
        });
        var attr = new ImageAttributes();
        attr.SetColorMatrix(matrix);
        return attr;
    }

    // Icon accent for the glass theme (#00C8FF)
    public static readonly Color GlassIconColor = Color.FromArgb(255, 0x00, 0xC8, 0xFF);

    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<Image, Image> _glassIcons = new();

    /// <summary>
    /// Recolors an icon to the glass accent, keeping its alpha shape. Cached, and safe to call repeatedly.
    /// </summary>
    public static Image? GlassIcon(Image? image)
    {
        if (image is null || !RForm.glassTheme) return image;
        lock (_glassIcons)
        {
            if (_glassIcons.TryGetValue(image, out var tinted)) return tinted;
            tinted = TintImage(image, GlassIconColor);
            _glassIcons.AddOrUpdate(image, tinted);
            _glassIcons.AddOrUpdate(tinted, tinted);
            return tinted;
        }
    }

    private static void GlassPicture_Changed(object? sender, EventArgs e)
    {
        if (sender is PictureBox pb && pb.BackgroundImage is not null)
        {
            var tinted = GlassIcon(pb.BackgroundImage);
            if (!ReferenceEquals(tinted, pb.BackgroundImage)) pb.BackgroundImage = tinted;
        }
    }

    private static Image AdjustImage(Image image)
    {
        if (RForm.glassTheme) return GlassIcon(image)!;

        var pic = new Bitmap(image.Width, image.Height);
        using (var g = Graphics.FromImage(pic))
        {
            g.DrawImage(image,
                new Rectangle(0, 0, image.Width, image.Height),
                0, 0, image.Width, image.Height,
                GraphicsUnit.Pixel, _invertAttributes);
        }
        return pic;
    }

    public static Image TintImage(Image image, Color tintColor)
    {
        var pic = new Bitmap(image);

        for (int y = 0; (y <= (pic.Height - 1)); y++)
        {
            for (int x = 0; (x <= (pic.Width - 1)); x++)
            {
                Color col = pic.GetPixel(x, y);
                pic.SetPixel(x, y, Color.FromArgb(col.A, tintColor.R, tintColor.G, tintColor.B));
            }
        }

        return pic;
    }

    public static Image RecolorDarkPixels(Image image, Color targetColor, byte luminanceThreshold = 128)
    {
        var pic = new Bitmap(image);
        for (int y = 0; y < pic.Height; y++)
        {
            for (int x = 0; x < pic.Width; x++)
            {
                Color col = pic.GetPixel(x, y);
                if (col.A == 0) continue;
                int lum = (col.R + col.G + col.B) / 3;
                if (lum < luminanceThreshold)
                    pic.SetPixel(x, y, Color.FromArgb(col.A, targetColor));
            }
        }
        return pic;
    }

    public static Image OverlayBadge(Image baseImage, Image badge, Color circleColor,
        float badgeScale = 0.5f, float shiftFraction = 0.18f,
        int? iconWidth = null, int? iconHeight = null)
    {
        int iw = iconWidth ?? baseImage.Width;
        int ih = iconHeight ?? baseImage.Height;

        int badgeSize = (int)(iw * badgeScale);
        int shift = (int)(badgeSize * shiftFraction);

        int newW = Math.Max(baseImage.Width, iw + shift);
        int newH = Math.Max(baseImage.Height, ih + shift);

        var pic = new Bitmap(newW, newH);
        using (var g = Graphics.FromImage(pic))
        using (var coloredBadge = (Bitmap)RecolorDarkPixels(badge, circleColor))
        {
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawImage(baseImage, 0, 0, baseImage.Width, baseImage.Height);

            int badgeX = iw - badgeSize + shift;
            int badgeY = ih - badgeSize + shift;
            g.DrawImage(coloredBadge, badgeX, badgeY, badgeSize, badgeSize);
        }
        return pic;
    }

    public static Image OverlayChargeBars(Image baseImage, int level, int max, Color color,
        int? iconWidth = null, int? iconHeight = null)
    {
        if (max <= 0) return baseImage;

        int iw = iconWidth ?? baseImage.Width;
        int ih = iconHeight ?? baseImage.Height;

        float s = iw / 48f;
        int barHeight = Math.Max(2, (int)Math.Round(10 * s));
        int barWidth = Math.Max(1, (int)Math.Round(4 * s));
        int barGap = Math.Max(1, (int)Math.Round(2 * s));
        int totalGap = barGap * (max - 1);
        int usedW = barWidth * max + totalGap;
        int xStart = (iw - usedW) / 2;

        // If a previous overlay extended the canvas below the icon (e.g. a corner badge),
        // start the bars below that extension; otherwise sit them right under the icon.
        int yStart = baseImage.Height > ih
            ? baseImage.Height + Math.Max(2, (int)Math.Round(2 * s))
            : ih + Math.Max(2, (int)Math.Round(3 * s));

        int newH = Math.Max(baseImage.Height, yStart + barHeight);
        int newW = Math.Max(baseImage.Width, iw);

        var pic = new Bitmap(newW, newH);
        using (var g = Graphics.FromImage(pic))
        using (var filled = new SolidBrush(color))
        using (var empty = new SolidBrush(Color.FromArgb(72, color)))
        {
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(baseImage, 0, 0, baseImage.Width, baseImage.Height);

            for (int i = 0; i < max; i++)
            {
                var rect = new Rectangle(xStart + i * (barWidth + barGap), yStart, barWidth, barHeight);
                g.FillRectangle(i < level ? filled : empty, rect);
            }
        }
        return pic;
    }

}
