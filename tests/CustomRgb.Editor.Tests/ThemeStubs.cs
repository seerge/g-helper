// The actual editor is tested with inert theme/picker wrappers, without referencing
// the GHelper application, AppConfig, Program, or any HID/device-control code.
namespace GHelper.UI;

public class RForm : Form
{
    public bool InitTheme(bool setDPI = false)
    {
        PaintTheme(this);
        return true;
    }
    private static void PaintTheme(Control parent)
    {
        parent.BackColor = Color.FromArgb(28, 28, 28);
        parent.ForeColor = Color.WhiteSmoke;
        foreach (Control child in parent.Controls) PaintTheme(child);
    }
}
public class RButton : Button { }
public class RComboBox : ComboBox { }
public class RNumericUpDown : NumericUpDown { }
public class RColorPicker : Form
{
    public static Color? NextColor;
    public Color Color;
    public RColorPicker(Color color, bool allowRandom = false) { Color = color; }
    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        if (NextColor is not Color next) return;
        Color = next;
        NextColor = null;
        DialogResult = DialogResult.OK;
    }
}
