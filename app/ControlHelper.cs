using CustomControls;
using WinFormsSliderBar;
using System.Drawing.Drawing2D;
using System.Windows.Forms.DataVisualization.Charting;

public static class ControlHelper
{

    static bool _invert = false;
    static bool _darkTheme = false;


    static float _scale = 1;

    static Color formBack;
    static Color backMain;
    static Color foreMain;
    static Color foreAccent;
    static Color borderMain;
    static Color buttonMain;

    public static void Adjust(RForm container, bool darkTheme = false, bool invert = false)
    {

        _darkTheme = darkTheme;

        if (darkTheme)
        {
            formBack = Color.FromArgb(255, 35, 35, 35);
            backMain = Color.FromArgb(255, 50, 50, 50);
            foreMain = Color.FromArgb(255, 240, 240, 240);
            foreAccent = Color.FromArgb(255, 100, 100, 100);
            borderMain = Color.FromArgb(255, 50, 50, 50);
            buttonMain = Color.FromArgb(255, 80, 80, 80);
        }
        else
        {
            formBack = SystemColors.Control;
            backMain = SystemColors.ControlLightLight;
            foreMain = SystemColors.ControlText;
            foreAccent = Color.LightGray;
            borderMain = Color.LightGray;
            buttonMain = Color.FromArgb(255, 230, 230, 230);
        }

        container.BackColor = formBack;
        container.ForeColor = foreMain;

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
                button.BackColor = button.Secondary ? buttonMain : backMain;
                button.ForeColor = foreMain;

                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderColor = borderMain;

                if (button.Image is not null)
                    button.Image = AdjustImage(button.Image);
            }

            var pictureBox = control as PictureBox;
            if (pictureBox != null && pictureBox.BackgroundImage is not null)
                pictureBox.BackgroundImage = AdjustImage(pictureBox.BackgroundImage);


            var combo = control as RComboBox;
            if (combo != null)
            {
                combo.BackColor = backMain;
                combo.ForeColor = foreMain;
                combo.BorderColor = backMain;
                combo.ButtonColor = backMain;
                combo.ArrowColor = foreMain;
            }

            var gb = control as GroupBox;
            if (gb != null)
            {
                gb.ForeColor = foreMain;
            }

            var sl = control as Slider;
            if (sl != null)
            {
                sl.borderColor = buttonMain;
            }

            var chk = control as CheckBox;
            if (chk != null)
            {
                chk.BackColor = buttonMain;
            }

            var chart = control as Chart;
            if (chart != null)
            {
                chart.BackColor = backMain;
                chart.ChartAreas[0].BackColor = backMain;

                chart.ChartAreas[0].AxisX.TitleForeColor = foreMain;
                chart.ChartAreas[0].AxisY.TitleForeColor = foreMain;

                chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = foreMain;
                chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = foreMain;

                chart.ChartAreas[0].AxisX.MajorTickMark.LineColor = foreMain;
                chart.ChartAreas[0].AxisY.MajorTickMark.LineColor = foreMain;

                chart.ChartAreas[0].AxisX.MajorGrid.LineColor = foreAccent;
                chart.ChartAreas[0].AxisY.MajorGrid.LineColor = foreAccent;
                chart.ChartAreas[0].AxisX.LineColor = foreAccent;
                chart.ChartAreas[0].AxisY.LineColor = foreAccent;

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

}
