using GHelper;
using GHelper.UI;
using GHelper.USB;
using System.Reflection;

internal static class ModeEditorTests
{
    public static void Run(string directory)
    {
        foreach (int mode in new[] { 2, 3, 24, 22 })
        {
            ModeLightingSettings? applied = null;
            using var editor = new LightingModeEditor(mode, ModeLightingSettings.Default(mode), value => applied = value, true, TestCapture);
            editor.StartPosition = FormStartPosition.Manual; editor.Location = new Point(-30000, -30000);
            editor.Show(); Application.DoEvents();
            Check(applied == null, mode + " opens without applying");
            if (mode != 22)
            {
                Find<ComboBox>(editor, "modePreset").SelectedIndex = 1;
                if (mode == 3) Check(!Find<CheckBox>(editor, "fullRainbow").Checked, "choosing a palette replaces automatic rainbow");
                RColorPicker.NextColor = Color.FromArgb(12, 34, 56);
                Click(editor, "modeColor_0");
                Check(applied == null, mode + " palette edits stay local");
                if (mode == 24)
                {
                    if (Find<Button>(editor, "modeAddColor").Enabled) Click(editor, "modeAddColor");
                    Check(!Find<Button>(editor, "modeAddColor").Enabled, mode + " enforces two-color maximum");
                }
                if (mode == 24)
                {
                    Find<ComboBox>(editor, "gradientMotion").SelectedIndex = 1;
                    Check(Find<NumericUpDown>(editor, "modeSeconds").Enabled, "animated gradient enables timing controls");
                }
            }
            else
            {
                Find<NumericUpDown>(editor, "ambientBoost").Value = 200;
                Find<NumericUpDown>(editor, "ambientShadows").Value = 35;
                Find<ComboBox>(editor, "ambientAreaPreset").SelectedIndex = 1;
                var region = Find<AmbientRegionPreview>(editor, "ambientRegions");
                Check(region.Area.Width == .5, "ambient region preset updates the selected capture area");
                typeof(AmbientRegionPreview).GetMethod("OnMouseDown", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(region, [new MouseEventArgs(MouseButtons.Left, 1, region.Width / 3, region.Height / 3, 0)]);
                typeof(AmbientRegionPreview).GetMethod("OnMouseUp", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(region, [new MouseEventArgs(MouseButtons.Left, 1, region.Width * 2 / 3, region.Height * 2 / 3, 0)]);
                Check(region.Area.X > 0 && region.Area.Width < .5, "dragging a screen rectangle changes the sampling area");
            }
            Click(editor, "modeApply");
            Check(applied != null, mode + " Apply commits the preview");
            if (mode != 22) Check(applied!.Keyboard.Colors[0] == "#0C2238", mode + " uses the edited palette");
            else Check(applied!.Keyboard.Ambient.Boost == 200 && applied.Lightbar.Ambient.Boost == 100, "ambient adjustments are independent between keyboard and lightbar");
            var previous = applied!.ToJson(mode);
            Find<ComboBox>(editor, "modeSurface").SelectedIndex = 1;
            Find<NumericUpDown>(editor, "modeBrightness").Value = 40;
            Check(applied.ToJson(mode) == previous, mode + " switching and adjusting another surface does not apply");
            Click(editor, "modeApply");
            Check(applied!.Keyboard.Brightness == 100 && applied.Lightbar.Brightness == 40, mode + " keeps each surface's own settings");
            Find<ComboBox>(editor, "modeSurface").SelectedIndex = 0;
            using var bitmap = new Bitmap(editor.Width, editor.Height);
            editor.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size)); bitmap.Save(Path.Combine(directory, "mode-" + mode + ".png"));
            editor.Close();
        }
        Console.WriteLine("Mode editors tested with synthetic screen images; no device APIs loaded.");
    }
    private static Bitmap TestCapture(Rectangle bounds, int width, int height)
    {
        var bitmap = new Bitmap(width, height);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.FromArgb(65, 75, 100));
        if (height > 20) graphics.DrawString("TEST IMAGE · synthetic screen", SystemFonts.DefaultFont, Brushes.White, 15, 20);
        return bitmap;
    }
    private static T Find<T>(Control parent, string name) where T : Control => (T)parent.Controls.Find(name, true).Single();
    private static void Click(Control parent, string name) => typeof(Button).GetMethod("OnClick", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(Find<Button>(parent, name), [EventArgs.Empty]);
    private static void Check(bool ok, string name) { if (!ok) throw new Exception("FAIL: " + name); Console.WriteLine("PASS: " + name); }
}
