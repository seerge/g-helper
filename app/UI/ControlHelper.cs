using GHelper.UI;
using System.Drawing.Drawing2D;
using System.Windows.Forms.DataVisualization.Charting;

public static class ControlHelper
{

    static bool _invert = false;
    static float _scale = 1;

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

            AdjustControls(control.Controls);
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
        var newSize = new Size((int)(image.Width * _scale), (int)(image.Height * _scale));
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

}
