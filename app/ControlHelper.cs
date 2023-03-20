using System;
using System.Drawing.Drawing2D;
using System.Windows.Forms.DataVisualization.Charting;
using CustomControls;

public static class ControlHelper
{

    static bool _invert = false;
    static bool _darkTheme = false;

    static float _scale = 1;

    static Color formBack;
    static Color backMain;
    static Color foreMain;
    static Color borderMain;
    static Color buttonMain;

    public static void Adjust(RForm container, float baseScale = 2)
    {
        _scale = GetDpiScale(container).Value / baseScale;
        

        if (container.DarkTheme)
        {
            formBack = Color.FromArgb(255, 35, 35, 35);
            backMain = Color.FromArgb(255, 50, 50, 50);
            foreMain = Color.White;
            borderMain = Color.FromArgb(255, 50, 50, 50);
            buttonMain = Color.FromArgb(255, 100, 100, 100);
        } else
        {
            formBack = SystemColors.Control;
            backMain = SystemColors.ControlLightLight;
            foreMain = SystemColors.ControlText;
            borderMain = Color.LightGray;
            buttonMain = SystemColors.ControlLight;
        }

        container.BackColor = formBack;
        container.ForeColor = foreMain;

        _invert = container.invert;
        AdjustControls(container.Controls);
        _invert = false;
    }


    private static void AdjustControls(Control.ControlCollection controls)
    {
        foreach (Control control in controls)
        {
            var button = control as Button;
            if (button != null)
            {
                button.BackColor = backMain;
                button.ForeColor = foreMain;

                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderColor = borderMain;

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
            if (combo != null)
            {
                combo.BackColor = backMain;
                combo.ForeColor = foreMain;
                combo.BorderColor = backMain;
                combo.ButtonColor = buttonMain;
            }

            var gb = control as GroupBox;
            if (gb != null)
            {
                gb.ForeColor = foreMain;
            }

            
            var chart = control as Chart;
            if (chart != null)
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
