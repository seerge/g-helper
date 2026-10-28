using System.Reflection;
using GHelper.USB;
#if SIMPLE_IMPLEMENTED
using GHelper;
#endif

internal static class SimpleEditorTests
{
    public static void Run(string imagePath)
    {
        Check(Assembly.GetExecutingAssembly().GetType("GHelper.SimpleRgbEditor") != null, "simple preset gallery exists");
#if SIMPLE_IMPLEMENTED
        CustomRgbScene? applied = null;
        using var editor = new SimpleRgbEditor(CustomRgbScene.CreateDefault(), value => applied = value, "", _ => { });
        editor.StartPosition = FormStartPosition.Manual;
        editor.Location = new Point(-30000, -30000);
        editor.Show();
        Application.DoEvents();
        Check(applied == null, "preset preview does not silently apply lighting");
        Check(editor.Controls.Find("phase", true).Length == 0 && editor.Controls.Find("palette", true).Length == 0, "technical section controls are hidden from the simple view");
        Check(editor.Controls.Find("advanced", true).Length == 1, "advanced editing remains available");
        Check(editor.Controls.Find("simpleDirection", true).Length == 1 && editor.Controls.Find("simpleColors", true).Length == 1, "simple adjustments include direction and clickable colors");
        Click(editor, "applySimple");
        Check(applied!.Sections.All(s => s.Brightness == 100), "opening the simple view preserves the existing custom look");
        Check(editor.Controls.Find("preset_rainbow", true).Single().Width > 200, "preset cards are wide enough to read their names");
        Click(editor, "preset_ocean");
        var variants = (ComboBox)editor.Controls.Find("variants", true).Single();
        Check(variants.Items.Count == 3, "choosing a look offers its three named variants");
        variants.SelectedIndex = 1;
        Check(editor.Controls.Find("simpleColor_0", true).Length == 1, "a selected look exposes its editable palette");
        Click(editor, "applySimple");
        Check(applied!.Name.Contains("Tropical"), "selected variant reaches Apply");
        var before = applied.ToJson();
        variants.SelectedIndex = 2;
        Check(applied.ToJson() == before, "variant changes remain a preview until Apply");
        ((ComboBox)editor.Controls.Find("simpleSpeed", true).Single()).SelectedIndex = 2;
        ((TrackBar)editor.Controls.Find("simpleBrightness", true).Single()).Value = 40;
        Click(editor, "applySimple");
        Check(applied!.Sections.All(s => s.Brightness == 40), "brightness slider applies to the whole selected look");
        Check(applied.Sections[0].PeriodSeconds == 4, "Fast makes Calm Blue's eight-second cycle twice as fast");
        ((ComboBox)editor.Controls.Find("where", true).Single()).SelectedIndex = 1;
        Click(editor, "applySimple");
        Check(applied!.Sections.Skip(4).All(s => s.Brightness == 40), "keyboard-only selection preserves the most recently applied lightbar");
        GHelper.UI.RColorPicker.NextColor = Color.FromArgb(17, 34, 51);
        var unchanged = applied.ToJson();
        Click(editor, "simpleColor_0");
        Check(applied.ToJson() == unchanged, "color picking remains a preview until Apply");
        Click(editor, "applySimple");
        Check(applied!.Sections.Take(4).All(s => s.Palette[0] == "#112233") && applied.Sections[4].Palette[0] != "#112233", "color adjustment applies to the chosen surface only");
        Click(editor, "preset_rainbow");
        Check(!editor.Controls.Find("simpleColor_0", true).Single().Enabled, "generated rainbow colors cannot be edited with ineffective swatches");
        ((ComboBox)editor.Controls.Find("simpleDirection", true).Single()).SelectedIndex = 1;
        Click(editor, "applySimple");
        Check(applied!.Sections[0].Reverse && !applied.Sections[4].Reverse, "direction adjustment preserves the other surface");
        Click(editor, "preset_solid");
        Check(!editor.Controls.Find("simpleSpeed", true).Single().Enabled && !editor.Controls.Find("simpleDirection", true).Single().Enabled, "static looks hide ineffective motion adjustments");
        Click(editor, "preset_cyberpunk");
        variants.SelectedIndex = 0;
        ((ComboBox)editor.Controls.Find("where", true).Single()).SelectedIndex = 0;
        ((TrackBar)editor.Controls.Find("simpleBrightness", true).Single()).Value = 75;
        Application.DoEvents();
        using var bitmap = new Bitmap(editor.Width, editor.Height);
        editor.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
        bitmap.Save(imagePath);
        editor.Close();
        Console.WriteLine("Simple screen offscreen render: " + imagePath);
#endif
    }
    private static void Click(Control form, string name) => typeof(Button).GetMethod("OnClick", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(form.Controls.Find(name, true).Single(), [EventArgs.Empty]);
    private static void Check(bool ok, string name) { if (!ok) throw new Exception("FAIL: " + name); Console.WriteLine("PASS: " + name); }
}
