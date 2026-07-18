using System.Runtime.InteropServices;

namespace GHelperOverlay;

// Ported from main g-helper (app/UI/CustomContextMenu.cs) so the standalone
// tray menu matches the dark-themed look users see on the main app.
public class CustomMenuRenderer : ToolStripProfessionalRenderer
{
    public CustomMenuRenderer() : base(new ProfessionalColorTable()) { }

    protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
    {
        var bounds = new Rectangle(0, 0, e.Item.Width, e.Item.Height);
        Color bg = e.Item.Owner?.BackColor ?? SystemColors.Menu;

        if (e.Item.Selected && e.Item.Enabled)
            using (var b = new SolidBrush(SystemColors.Highlight))
                e.Graphics.FillRectangle(b, bounds);
        else
            using (var b = new SolidBrush(bg))
                e.Graphics.FillRectangle(b, bounds);
    }

    protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
    {
        if (e.Item is not ToolStripMenuItem { Checked: true }) return;

        Color color = e.Item.Selected
            ? SystemColors.HighlightText
            : (e.Item.Owner?.ForeColor ?? SystemColors.MenuText);

        var rect = new Rectangle(0, 0, e.ImageRectangle.Right + e.ImageRectangle.X, e.Item.Height);
        TextRenderer.DrawText(e.Graphics, "✓", e.Item.Font, rect, color,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
    }

    protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
    {
        if (!e.Item.Enabled)
            e.TextColor = Color.FromArgb(130, e.Item.Owner?.ForeColor ?? SystemColors.MenuText);
        else if (e.Item.Selected)
            e.TextColor = SystemColors.HighlightText;
        else
            e.TextColor = e.Item.Owner?.ForeColor ?? SystemColors.MenuText;

        base.OnRenderItemText(e);
    }

    protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
    {
        using var b = new SolidBrush(e.ToolStrip.BackColor);
        e.Graphics.FillRectangle(b, e.AffectedBounds);
    }

    protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
    {
        // suppress the default white/light stripe drawn on the left margin
        using var b = new SolidBrush(e.ToolStrip.BackColor);
        e.Graphics.FillRectangle(b, e.AffectedBounds);
    }

    protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) { }
}

public class CustomContextMenu : ContextMenuStrip
{
    [DllImport("dwmapi.dll", SetLastError = true)]
    private static extern long DwmSetWindowAttribute(IntPtr hwnd, int attribute,
        ref int pvAttribute, uint cbAttribute);

    private const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
    private const int DWMWCP_ROUNDSMALL = 3;

    public CustomContextMenu()
    {
        try
        {
            int pref = DWMWCP_ROUNDSMALL;
            DwmSetWindowAttribute(Handle, DWMWA_WINDOW_CORNER_PREFERENCE, ref pref, sizeof(int));
        }
        catch { }
    }
}
