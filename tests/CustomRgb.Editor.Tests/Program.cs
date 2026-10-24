using GHelper;
using GHelper.USB;
using System.Reflection;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        CustomRgbScene? applied = null;
        string library = "";
        var scene = CustomRgbScene.CreateDefault()
            .Replace([0], new RgbSection { Style = RgbSectionStyle.Static, Palette = ["#FF0000", "#00FF00", "#0000FF"] })
            .Replace([1], new RgbSection { Style = RgbSectionStyle.Breathe, Palette = ["#0080FF"] })
            .Replace([2], new RgbSection { Style = RgbSectionStyle.Rainbow })
            .Replace([3], new RgbSection { Style = RgbSectionStyle.Strobe, Palette = ["#FF0080"] });
        using var editor = new CustomRgbEditor(scene, value => applied = value, "", json => library = json);
        editor.StartPosition = FormStartPosition.Manual;
        editor.Location = new Point(-30000, -30000);
        editor.Show();
        Application.DoEvents();
        Check(applied == null, "opening the editor does not apply to hardware");
        Check((RgbSectionStyle)Find<ComboBox>(editor, "style").SelectedItem! == RgbSectionStyle.Static, "initial controls preserve the selected section's saved style");
        foreach (string name in new[] { "period", "brightness", "phase", "duty" })
        {
            var field = Find<NumericUpDown>(editor, name);
            int x = editor.PointToClient(field.PointToScreen(Point.Empty)).X;
            Check(x >= 0 && x + field.Width < editor.ClientSize.Width, name + " control fits horizontally in the editor");
        }
        Find<ComboBox>(editor, "style").SelectedItem = RgbSectionStyle.ColorCycle;
        Find<NumericUpDown>(editor, "period").Value = 2;
        Find<NumericUpDown>(editor, "brightness").Value = 66;
        Find<NumericUpDown>(editor, "phase").Value = 25;
        Find<ListBox>(editor, "palette").SelectedIndex = 0;
        Click(editor, "colorDown");
        Click(editor, "apply");
        Check(applied != null && applied.Sections[0].Style == RgbSectionStyle.ColorCycle, "style selection reaches the applied scene");
        Check(applied!.Sections[0].PeriodSeconds == 2 && applied.Sections[0].Brightness == 66 && applied.Sections[0].PhasePercent == 25, "timing, brightness and phase controls update selected section");
        Check(applied.Sections[0].Palette[0] == "#00FF00" && applied.Sections[0].Palette[1] == "#FF0000", "palette order can be rearranged in the editor");
        Check(applied.Sections[1].Style == RgbSectionStyle.Breathe && applied.Sections[4].Style == RgbSectionStyle.Static, "unselected keyboard and lightbar sections are preserved");
        Click(editor, "selectKeyboard");
        Find<NumericUpDown>(editor, "brightness").Value = 33;
        Click(editor, "apply");
        Check(applied!.Sections.Take(4).All(s => s.Brightness == 33), "keyboard multi-selection edits all keyboard sections");
        Check(applied.Sections[1].Style == RgbSectionStyle.Breathe && applied.Sections[2].Style == RgbSectionStyle.Rainbow, "multi-selection preserves other independent settings");
        Check(applied.Sections.Skip(4).All(s => s.Brightness == 100), "keyboard editing does not change lightbar brightness");
        Click(editor, "selectLightbar");
        Find<ComboBox>(editor, "style").SelectedItem = RgbSectionStyle.ColorSteps;
        Click(editor, "apply");
        Check(applied!.Sections.Skip(4).All(s => s.Style == RgbSectionStyle.ColorSteps), "lightbar has its own independent effect style");
        Find<TextBox>(editor, "profileName").Text = "My custom lighting";
        Click(editor, "saveProfile");
        Check(library.Contains("My custom lighting"), "named profiles are passed to persistence callback");
        var saved = applied.ToJson();
        Find<NumericUpDown>(editor, "brightness").Value = 21;
        Check(applied.ToJson() == saved, "editing the preview does not silently apply changes");
        Click(editor, "loadProfile");
        Click(editor, "apply");
        Check(applied!.Sections.Skip(4).All(s => s.Brightness == 100), "loading a profile restores its independent section settings");
        Click(editor, "pausePreview");
        Check(Find<Button>(editor, "pausePreview").Text == "Play preview", "preview playback can be paused");
        Click(editor, "pausePreview");
        Check(Find<Button>(editor, "pausePreview").Text == "Pause preview", "preview playback can resume");

        string imagePath = args.Length > 0 ? Path.GetFullPath(args[0]) : Path.Combine(AppContext.BaseDirectory, "custom-editor.png");
        Directory.CreateDirectory(Path.GetDirectoryName(imagePath)!);
        using var bitmap = new Bitmap(editor.Width, editor.Height);
        editor.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
        bitmap.Save(imagePath);
        editor.Close();
        SimpleEditorTests.Run(Path.Combine(Path.GetDirectoryName(imagePath)!, "simple-rgb-preview.png"));
        ModeEditorTests.Run(Path.GetDirectoryName(imagePath)!);
        Console.WriteLine("Offscreen editor render: " + imagePath);
        Console.WriteLine("All editor checks passed. Theme/picker wrappers are inert; no GHelper application or hardware API was loaded.");
    }

    private static T Find<T>(Control parent, string name) where T : Control => (T)parent.Controls.Find(name, true).Single();
    private static void Click(Control parent, string name) => typeof(Button).GetMethod("OnClick", BindingFlags.NonPublic | BindingFlags.Instance)!
        .Invoke(Find<Button>(parent, name), [EventArgs.Empty]);
    private static void Check(bool ok, string name) { if (!ok) throw new Exception("FAIL: " + name); Console.WriteLine("PASS: " + name); }
}
