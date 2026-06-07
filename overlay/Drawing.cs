using System.Drawing.Drawing2D;

namespace GHelperOverlay;

internal static class Drawing
{
    public static GraphicsPath RoundedRect(Rectangle bounds, int radius)
    {
        int d = radius * 2;
        Size sz = new(d, d);
        Rectangle arc = new(bounds.Location, sz);
        GraphicsPath path = new();

        if (radius == 0)
        {
            path.AddRectangle(bounds);
            return path;
        }

        path.AddArc(arc, 180, 90);
        arc.X = bounds.Right - d;
        path.AddArc(arc, 270, 90);
        arc.Y = bounds.Bottom - d;
        path.AddArc(arc, 0, 90);
        arc.X = bounds.Left;
        path.AddArc(arc, 90, 90);
        path.CloseFigure();
        return path;
    }

    public static void FillRoundedRectangle(this Graphics g, Brush brush, Rectangle bounds, int cornerRadius)
    {
        using GraphicsPath path = RoundedRect(bounds, cornerRadius);
        g.FillPath(brush, path);
    }
}
