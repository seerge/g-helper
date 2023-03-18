using System.Drawing.Drawing2D;

public static class HighDpiHelper
{
    public static void AdjustControlImagesDpiScale(Control container, float baseScale = 2)
    {
        var dpiScale = GetDpiScale(container).Value;
        AdjustControlImagesDpiScale(container.Controls, dpiScale / baseScale);
    }

    public static void AdjustButtonDpiScale(ButtonBase button, float dpiScale)
    {
        var image = button.Image;
        
        if (image == null)
            return;

        button.Image = ScaleImage(image, dpiScale);
    }

    private static void AdjustControlImagesDpiScale(Control.ControlCollection controls, float dpiScale)
    {
        foreach (Control control in controls)
        {
            var button = control as ButtonBase;
            if (button != null)
                AdjustButtonDpiScale(button, dpiScale);

            AdjustControlImagesDpiScale(control.Controls, dpiScale);
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

    private static Image ScaleImage(Image image, float dpiScale)
    {
        var newSize = ScaleSize(image.Size, dpiScale);
        var newBitmap = new Bitmap(newSize.Width, newSize.Height);

        using (var g = Graphics.FromImage(newBitmap))
        {
            // According to this blog post http://blogs.msdn.com/b/visualstudio/archive/2014/03/19/improving-high-dpi-support-for-visual-studio-2013.aspx
            // NearestNeighbor is more adapted for 200% and 200%+ DPI

            var interpolationMode = InterpolationMode.HighQualityBicubic;
            if (dpiScale >= 2.0f)
                interpolationMode = InterpolationMode.NearestNeighbor;

            g.InterpolationMode = interpolationMode;
            g.DrawImage(image, new Rectangle(new Point(), newSize));
        }

        return newBitmap;
    }

    private static Size ScaleSize(Size size, float scale)
    {
        return new Size((int)(size.Width * scale), (int)(size.Height * scale));
    }
}
