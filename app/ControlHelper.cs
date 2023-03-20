using System.Drawing.Drawing2D;
using System.Windows.Forms.DataVisualization.Charting;
using CustomControls;

public static class ControlHelper
{

    static bool _invert = false;
    static float _scale = 1;

    static Color formBack = Color.FromArgb(255, 35, 35, 35);
    static Color backMain = Color.FromArgb(255, 50, 50, 50);
    static Color foreMain = Color.White;
    static Color borderMain = Color.FromArgb(255, 50, 50, 50);
    static Color buttonMain = Color.FromArgb(255, 100,100,100);

    public static void Adjust(Control container, float baseScale = 2, bool invert = false)
    {
        _scale = GetDpiScale(container).Value / baseScale;
        _invert = invert;

        if (_invert)
        {
            container.BackColor = formBack;
            container.ForeColor = foreMain;
        }


        AdjustControls(container.Controls);
    }


    private static void AdjustControls(Control.ControlCollection controls)
    {
        foreach (Control control in controls)
        {
            var button = control as Button;
            if (button != null)
            {

                if (_invert)
                {
                    button.BackColor = backMain;
                    button.ForeColor = foreMain;

                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderColor = borderMain;
                }

                if (button.Image is not null)
                    button.Image = AdjustImage(button.Image);
            }
                
            var pictureBox = control as PictureBox;
            if (pictureBox != null)
            {
                if (pictureBox.BackgroundImage is not null)
                    pictureBox.BackgroundImage = AdjustImage(pictureBox.BackgroundImage);
            }

            var combo = control as RComboBox;
            if (combo != null && _invert)
            {
                combo.BackColor = borderMain;
                combo.ForeColor = foreMain;
                combo.BorderColor = borderMain;
                combo.ButtonColor = buttonMain;
            }
            
            var chart = control as Chart;
            if (chart != null && _invert)
            {
                chart.BackColor = backMain;
                chart.ChartAreas[0].BackColor = backMain;
                chart.ChartAreas[0].AxisX.MajorGrid.LineColor = foreMain;
                chart.ChartAreas[0].AxisY.MajorGrid.LineColor = foreMain;
                
                chart.ChartAreas[0].AxisX.TitleForeColor = foreMain;
                chart.ChartAreas[0].AxisY.TitleForeColor = foreMain;

                chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = foreMain;
                chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = foreMain;

                chart.ChartAreas[0].AxisX.MajorTickMark.LineColor = foreMain;
                chart.ChartAreas[0].AxisY.MajorTickMark.LineColor = foreMain;

                chart.ChartAreas[0].AxisX.LineColor = foreMain;
                chart.ChartAreas[0].AxisY.LineColor = foreMain;

                chart.Titles[0].ForeColor = foreMain;

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

    private static Image AdjustImage(Image image)
    {
        var newSize = new Size((int)(image.Width * _scale), (int)(image.Height * _scale));
        var pic = new Bitmap(newSize.Width, newSize.Height);

        using (var g = Graphics.FromImage(pic))
        {
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(image, new Rectangle(new Point(), newSize));
        }

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

}
