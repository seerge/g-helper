using GHelper.UI;
using System.Drawing.Drawing2D;
using System.Windows.Forms.DataVisualization.Charting;

public static class ControlHelper
{

    static bool _invert = false;
    static float _scale = 1;

    public static float Scale => _scale;

    public static void Adjust(RForm container, bool invert = false)
    {

        container.BackColor = RForm.formBack;
        container.ForeColor = RForm.foreMain;

        _invert = invert;
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

            var button = control as RButton;
            if (button != null)
            {
                button.BackColor = button.Secondary ? RForm.buttonSecond : RForm.buttonMain;
                button.ForeColor = RForm.foreMain;

                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderColor = RForm.borderMain;

                if (button.Image is not null)
                    button.Image = AdjustImage(button.Image);
            }

            var pictureBox = control as PictureBox;
            if (pictureBox != null && pictureBox.BackgroundImage is not null)
                pictureBox.BackgroundImage = AdjustImage(pictureBox.BackgroundImage);


            var combo = control as RComboBox;
            if (combo != null)
            {
                combo.BackColor = RForm.buttonMain;
                combo.ForeColor = RForm.foreMain;
                combo.BorderColor = RForm.buttonMain;
                combo.ButtonColor = RForm.buttonMain;
                combo.ArrowColor = RForm.foreMain;
            }
            var numbericUpDown = control as NumericUpDown;
            if(numbericUpDown is not null)
            {
                numbericUpDown.ForeColor = RForm.foreMain;
                numbericUpDown.BackColor = RForm.buttonMain;
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
            if (chk != null && chk.BackColor != RForm.formBack)
            {
                chk.BackColor = RForm.buttonSecond;
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

    private static Image AdjustImage(Image image)
    {
        var pic = new Bitmap(image);

        if (_invert)
        {
            for (int y = 0; (y <= (pic.Height - 1)); y++)
            {
                for (int x = 0; (x <= (pic.Width - 1)); x++)
                {
                    Color col = pic.GetPixel(x, y);
                    pic.SetPixel(x, y, Color.FromArgb(col.A, (255 - col.R), (255 - col.G), (255 - col.B)));
                }
            }
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
