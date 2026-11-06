using GHelper.UI;
using GHelper.USB;
using System.Diagnostics;
using System.Text.Json;

namespace GHelper;

public sealed class CustomRgbEditor : RForm
{
    private CustomRgbScene working;
    private readonly Action<CustomRgbScene> apply;
    private readonly Action<string> saveProfiles;
    private readonly Dictionary<string, CustomRgbScene> profiles = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<int> selected = [0];
    private readonly RgbSectionsView preview;
    private readonly System.Windows.Forms.Timer previewTimer = new() { Interval = 50 };
    private readonly Stopwatch clock = Stopwatch.StartNew();
    private readonly ComboBox profile = new RComboBox { Name = "profiles", Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox profileName = new() { Name = "profileName", Width = 150 };
    private readonly ComboBox style = new RComboBox { Name = "style", Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ListBox palette = new() { Name = "palette", Dock = DockStyle.Fill, Height = 110, DrawMode = DrawMode.OwnerDrawFixed, ItemHeight = 23 };
    private readonly NumericUpDown period = Number("period", 0.2m, 60, 4, 2);
    private readonly NumericUpDown brightness = Number("brightness", 0, 100, 100);
    private readonly NumericUpDown phase = Number("phase", 0, 100, 0);
    private readonly NumericUpDown duty = Number("duty", 1, 99, 50);
    private readonly CheckBox reverse = new() { Name = "reverse", Text = "Reverse palette order", AutoSize = true };
    private readonly Label selectionTitle = new() { AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
    private readonly Label status = new() { Name = "status", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, AutoEllipsis = true };
    private readonly TableLayoutPanel properties = new() { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, Padding = new Padding(12) };
    private readonly float uiScale;
    private bool loading, playing = true;
    private double pausedAt;
    private RgbSection? copied;

    public CustomRgbEditor(CustomRgbScene scene, Action<CustomRgbScene> apply, string profilesJson,
        Action<string> saveProfiles, bool hasLightbar = true)
    {
        working = scene.Normalize();
        this.apply = apply;
        this.saveProfiles = saveProfiles;
        Text = "Custom RGB — keyboard & lightbar";
        Font = new Font("Segoe UI", 9);
        AutoScaleMode = AutoScaleMode.None;
        using (var graphics = CreateGraphics()) uiScale = graphics.DpiX / 96f;
        ClientSize = new Size(S(940), S(655));
        MinimumSize = new Size(S(880), S(650));
        StartPosition = FormStartPosition.CenterParent;
        ShowInTaskbar = false;

        var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 3, Padding = new Padding(12) };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, S(76)));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, S(46)));
        Controls.Add(root);

        var top = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = true, Padding = new Padding(0, 4, 0, 4) };
        top.Controls.AddRange([profile, MakeButton("Load", "loadProfile", LoadProfile), profileName,
            MakeButton("Save profile", "saveProfile", SaveProfile), MakeButton("Delete", "deleteProfile", DeleteProfile),
            MakeButton("Import…", "importProfile", ImportProfile), MakeButton("Export…", "exportProfile", ExportProfile)]);
        root.Controls.Add(top, 0, 0);
        root.SetColumnSpan(top, 2);
        profileName.Text = working.Name;

        var left = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
        left.RowStyles.Add(new RowStyle(SizeType.Absolute, S(42)));
        left.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        left.RowStyles.Add(new RowStyle(SizeType.Absolute, S(56)));
        var selection = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
        selection.Controls.AddRange([MakeButton("Keyboard", "selectKeyboard", () => SelectSections(Enumerable.Range(0, 4))),
            MakeButton("Lightbar", "selectLightbar", () => SelectSections(Enumerable.Range(4, 4))),
            MakeButton("All", "selectAll", () => SelectSections(Enumerable.Range(0, hasLightbar ? 8 : 4)))]);
        selection.Controls[1].Enabled = hasLightbar;
        left.Controls.Add(selection, 0, 0);
        preview = new RgbSectionsView { Dock = DockStyle.Fill, Name = "sections", HasLightbar = hasLightbar, Selected = selected };
        preview.SectionClicked += index =>
        {
            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
                if (!selected.Add(index)) selected.Remove(index);
            }
            else { selected.Clear(); selected.Add(index); }
            LoadSelection();
        };
        left.Controls.Add(preview, 0, 1);
        left.Controls.Add(new Label { Dock = DockStyle.Fill, Text = "Click a section. Ctrl-click to select several.\nPreview changes stay in this editor until you click Apply.", AutoSize = false }, 0, 2);
        root.Controls.Add(left, 0, 1);

        var right = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
        properties.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        right.SizeChanged += (_, _) =>
        {
            int width = Math.Max(1, right.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 2);
            if (properties.MaximumSize.Width != width)
            {
                properties.MaximumSize = new Size(width, 0);
                properties.Width = width;
            }
        };
        right.Controls.Add(properties);
        root.Controls.Add(right, 1, 1);
        AddProperty(selectionTitle);
        AddProperty(new Label { Text = "Style", AutoSize = true });
        style.Items.AddRange(Enum.GetValues<RgbSectionStyle>().Cast<object>().ToArray());
        style.FormattingEnabled = true;
        style.Format += (_, e) => { if (e.ListItem is RgbSectionStyle value) e.Value = StyleName(value); };
        AddProperty(style);
        AddProperty(new Label { Text = "Palette · top to bottom is color order", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
        AddProperty(palette);
        var paletteActions = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Fill };
        paletteActions.Controls.AddRange([MakeButton("+", "addColor", () => EditColor(true)), MakeButton("Edit", "editColor", () => EditColor(false)),
            MakeButton("−", "removeColor", RemoveColor), MakeButton("↑", "colorUp", () => MoveColor(-1)), MakeButton("↓", "colorDown", () => MoveColor(1))]);
        AddProperty(paletteActions);
        AddProperty(new Label { Text = "Static, Breathe and Strobe use the first color.\nPalette effects and Battery use the full palette.\nRainbow generates its own colors.", AutoSize = true, Padding = new Padding(0, 0, 0, 6) });
        AddProperty(Row("Cycle (seconds)", period));
        AddProperty(Row("Brightness (%)", brightness));
        AddProperty(Row("Phase offset (%)", phase));
        AddProperty(Row("Strobe on-time (%)", duty));
        AddProperty(reverse);
        var clipboard = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Fill, Padding = new Padding(0, 8, 0, 0) };
        clipboard.Controls.AddRange([MakeButton("Copy settings", "copySettings", () => { if (selected.Count > 0) copied = working.Sections[selected.Min()].Normalize(); }),
            MakeButton("Paste settings", "pasteSettings", () => { if (copied != null) Change(_ => copied); })]);
        AddProperty(clipboard);

        var bottom = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4 };
        bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        for (int i = 0; i < 3; i++) bottom.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        bottom.Controls.Add(status, 0, 0);
        bottom.Controls.Add(MakeButton("Pause preview", "pausePreview", () =>
        {
            if (playing) { pausedAt = clock.Elapsed.TotalSeconds; clock.Stop(); }
            else clock.Start();
            playing = !playing;
            ((Button)bottom.Controls.Find("pausePreview", false)[0]).Text = playing ? "Pause preview" : "Play preview";
        }), 1, 0);
        bottom.Controls.Add(MakeButton("Apply", "apply", () =>
        {
            try { working = (working with { Name = profileName.Text }).Normalize(); apply(working.Normalize()); status.Text = "Applied to keyboard and lightbar."; }
            catch (Exception ex) { Error(ex); }
        }), 2, 0);
        bottom.Controls.Add(MakeButton("Close", "close", Close), 3, 0);
        root.Controls.Add(bottom, 0, 2);
        root.SetColumnSpan(bottom, 2);

        style.SelectedIndexChanged += (_, _) => { if (style.SelectedItem is RgbSectionStyle value) Change(s => s with { Style = value }); };
        period.ValueChanged += (_, _) => Change(s => s with { PeriodSeconds = (double)period.Value });
        brightness.ValueChanged += (_, _) => Change(s => s with { Brightness = (int)brightness.Value });
        phase.ValueChanged += (_, _) => Change(s => s with { PhasePercent = (int)phase.Value });
        duty.ValueChanged += (_, _) => Change(s => s with { DutyPercent = (int)duty.Value });
        reverse.CheckedChanged += (_, _) => Change(s => s with { Reverse = reverse.Checked });
        palette.DoubleClick += (_, _) => EditColor(false);
        palette.DrawItem += DrawPalette;
        ReadProfiles(profilesJson);
        RefreshProfiles();
        InitTheme();
        ScaleControls(root);
        LoadSelection();
        previewTimer.Tick += (_, _) => RefreshPreview();
        previewTimer.Start();
        RefreshPreview();
    }

    private int S(int value) => (int)Math.Round(value * uiScale);

    private void ScaleControls(Control root)
    {
        foreach (Control control in root.Controls)
        {
            if (control is Button or ComboBox or TextBox or NumericUpDown or ListBox)
                control.Size = new Size(S(control.Width), S(control.Height));
            ScaleControls(control);
        }
    }

    private static NumericUpDown Number(string name, decimal min, decimal max, decimal value, int decimals = 0) =>
        new RNumericUpDown { Name = name, Minimum = min, Maximum = max, Value = value, DecimalPlaces = decimals,
            Increment = decimals == 0 ? 1 : 0.1m, Width = 110, AccessibleName = name };

    private static RButton MakeButton(string text, string name, Action action)
    {
        var button = new RButton { Text = text, Name = name, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink,
            MinimumSize = new Size(38, 28), Margin = new Padding(3), Padding = new Padding(7, 2, 7, 2) };
        button.Click += (_, _) => action();
        return button;
    }

    private void AddProperty(Control control)
    {
        control.Margin = new Padding(3, 3, 3, 4);
        properties.Controls.Add(control);
    }

    private static Control Row(string title, Control control)
    {
        var row = new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, ColumnCount = 2 };
        row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        row.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        row.Controls.Add(new Label { Text = title, AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
        row.Controls.Add(control, 1, 0);
        return row;
    }

    private void SelectSections(IEnumerable<int> sections)
    {
        selected.Clear();
        selected.UnionWith(sections);
        LoadSelection();
    }

    private void LoadSelection()
    {
        loading = true;
        properties.Enabled = selected.Count > 0;
        if (selected.Count > 0)
        {
            int first = selected.Min();
            RgbSection s = working.Sections[first];
            selectionTitle.Text = selected.Count == 1 ? SectionName(first) : $"{selected.Count} sections selected";
            style.SelectedItem = s.Style;
            period.Value = (decimal)s.PeriodSeconds;
            brightness.Value = s.Brightness;
            phase.Value = s.PhasePercent;
            duty.Value = s.DutyPercent;
            reverse.Checked = s.Reverse;
            palette.Items.Clear();
            palette.Items.AddRange(s.Palette);
            palette.SelectedIndex = 0;
            bool timed = s.Style is not (RgbSectionStyle.Off or RgbSectionStyle.Static or RgbSectionStyle.Battery);
            period.Enabled = phase.Enabled = timed;
            reverse.Enabled = s.Style is RgbSectionStyle.ColorCycle or RgbSectionStyle.ColorSteps or RgbSectionStyle.Rainbow;
            duty.Enabled = s.Style == RgbSectionStyle.Strobe;
            palette.Enabled = s.Style != RgbSectionStyle.Rainbow && s.Style != RgbSectionStyle.Off;
        }
        status.Text = selected.Count > 1 ? "Showing the first section. Edits change only that property." : "Preview only · click Apply to update the laptop.";
        loading = false;
        RefreshPreview();
    }

    private void Change(Func<RgbSection, RgbSection> update)
    {
        if (loading || selected.Count == 0) return;
        foreach (int index in selected) working = working.Replace([index], update(working.Sections[index]));
        LoadSelection();
    }

    private void EditColor(bool add)
    {
        if (selected.Count == 0) return;
        string[] colors = working.Sections[selected.Min()].Palette;
        int index = Math.Max(0, palette.SelectedIndex);
        if (add && colors.Length == 8) { status.Text = "A palette supports up to eight colors."; return; }
        using var picker = new RColorPicker(RgbSection.ParseColor(colors[index]));
        if (picker.ShowDialog(this) != DialogResult.OK) return;
        var next = colors.ToList();
        if (add) next.Add(RgbSection.ToHex(picker.Color));
        else next[index] = RgbSection.ToHex(picker.Color);
        Change(s => s with { Palette = next.ToArray() });
        palette.SelectedIndex = add ? next.Count - 1 : index;
    }

    private void RemoveColor()
    {
        if (selected.Count == 0 || palette.Items.Count <= 1) return;
        var next = working.Sections[selected.Min()].Palette.ToList();
        next.RemoveAt(Math.Max(0, palette.SelectedIndex));
        Change(s => s with { Palette = next.ToArray() });
    }

    private void MoveColor(int direction)
    {
        if (selected.Count == 0) return;
        int index = palette.SelectedIndex, target = index + direction;
        if (index < 0 || target < 0 || target >= palette.Items.Count) return;
        var next = working.Sections[selected.Min()].Palette.ToArray();
        (next[index], next[target]) = (next[target], next[index]);
        Change(s => s with { Palette = next.ToArray() });
        palette.SelectedIndex = target;
    }

    private void DrawPalette(object? sender, DrawItemEventArgs e)
    {
        if (e.Index < 0) return;
        e.DrawBackground();
        using var brush = new SolidBrush(RgbSection.ParseColor((string)palette.Items[e.Index]));
        e.Graphics.FillRectangle(brush, e.Bounds.X + 5, e.Bounds.Y + 4, 28, e.Bounds.Height - 8);
        TextRenderer.DrawText(e.Graphics, (string)palette.Items[e.Index], Font,
            new Point(e.Bounds.X + 42, e.Bounds.Y + 3), e.ForeColor);
        e.DrawFocusRectangle();
    }

    private void RefreshPreview()
    {
        preview.Scene = working;
        preview.Colors = working.Render(playing ? clock.Elapsed.TotalSeconds : pausedAt, SystemInformation.PowerStatus.BatteryLifePercent);
        preview.Invalidate();
    }

    private void ReadProfiles(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return;
        try
        {
            if (json.Length > 1048576) throw new InvalidDataException("The profile library is too large.");
            using var document = JsonDocument.Parse(json);
            foreach (var item in document.RootElement.EnumerateArray().Take(20))
            {
                var scene = CustomRgbScene.Parse(item.GetRawText());
                profiles[scene.Name] = scene;
            }
        }
        catch (Exception ex) { status.Text = "Saved profiles could not be loaded: " + ex.Message; }
    }

    private void RefreshProfiles()
    {
        profile.Items.Clear();
        profile.Items.AddRange(profiles.Keys.OrderBy(name => name).ToArray());
        if (profile.Items.Count > 0) profile.SelectedIndex = 0;
    }

    private void SaveProfile()
    {
        try
        {
            var scene = (working with { Name = profileName.Text }).Normalize();
            if (profiles.Count >= 20 && !profiles.ContainsKey(scene.Name)) throw new InvalidOperationException("A maximum of 20 profiles can be saved. Export or delete an existing profile first.");
            profiles[scene.Name] = scene;
            SaveLibrary();
            RefreshProfiles();
            profile.SelectedItem = scene.Name;
            status.Text = "Profile saved. Click Apply to use it on the laptop.";
        }
        catch (Exception ex) { Error(ex); }
    }

    private void SaveLibrary() => saveProfiles("[" + string.Join(",", profiles.Values.Select(p => p.ToJson())) + "]");

    private void LoadProfile()
    {
        if (profile.SelectedItem is string name && profiles.TryGetValue(name, out var saved))
        {
            working = saved.Normalize();
            profileName.Text = working.Name;
            clock.Restart();
            if (!playing) clock.Stop();
            pausedAt = 0;
            LoadSelection();
        }
    }

    private void DeleteProfile()
    {
        if (profile.SelectedItem is not string name) return;
        profiles.Remove(name);
        SaveLibrary();
        RefreshProfiles();
        status.Text = "Saved profile deleted; the current preview is unchanged.";
    }

    private void ImportProfile()
    {
        using var dialog = new OpenFileDialog { Filter = "RGB profile (*.json)|*.json", CheckFileExists = true };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            if (new FileInfo(dialog.FileName).Length > 65536) throw new InvalidDataException("The RGB profile is too large.");
            working = CustomRgbScene.Parse(File.ReadAllText(dialog.FileName));
            profileName.Text = working.Name;
            LoadSelection();
        }
        catch (Exception ex) { Error(ex); }
    }

    private void ExportProfile()
    {
        using var dialog = new SaveFileDialog { Filter = "RGB profile (*.json)|*.json", FileName = "custom-rgb.json" };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        try { File.WriteAllText(dialog.FileName, (working with { Name = profileName.Text }).ToJson()); status.Text = "Profile exported."; }
        catch (Exception ex) { Error(ex); }
    }

    private void Error(Exception ex) => MessageBox.Show(this, ex.Message, "Custom RGB", MessageBoxButtons.OK, MessageBoxIcon.Information);
    public static string SectionName(int index) => index < 4 ? $"Keyboard zone {index + 1}" : $"Lightbar section {index - 3}";
    public static string StyleName(RgbSectionStyle value) => value switch
    {
        RgbSectionStyle.ColorCycle => "Palette fade", RgbSectionStyle.ColorSteps => "Palette steps", _ => value.ToString()
    };

    protected override void Dispose(bool disposing)
    {
        if (disposing) previewTimer.Dispose();
        base.Dispose(disposing);
    }
}

internal sealed class RgbSectionsView : Control
{
    public CustomRgbScene Scene = CustomRgbScene.CreateDefault();
    public Color[] Colors = Enumerable.Repeat(Color.Black, 8).ToArray();
    public HashSet<int> Selected = [0];
    public bool HasLightbar = true;
    public event Action<int>? SectionClicked;

    public RgbSectionsView() { DoubleBuffered = true; ResizeRedraw = true; Cursor = Cursors.Hand; }

    private Rectangle Area(int index)
    {
        int gap = 8, width = Math.Max(1, (ClientSize.Width - 5 * gap) / 4);
        int keyboardHeight = Math.Max(95, ClientSize.Height * 48 / 100);
        return new Rectangle(gap + (index % 4) * (width + gap), index < 4 ? 34 : keyboardHeight + 104,
            width, index < 4 ? keyboardHeight : 54);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        for (int i = 0; i < (HasLightbar ? 8 : 4); i++)
            if (Area(i).Contains(e.Location)) { SectionClicked?.Invoke(i); break; }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        TextRenderer.DrawText(e.Graphics, "KEYBOARD · FOUR ZONES", Font, new Point(8, 8), ForeColor);
        TextRenderer.DrawText(e.Graphics, HasLightbar ? "LIGHTBAR · FOUR LOGICAL SECTIONS" : "NO LIGHTBAR DETECTED", Font,
            new Point(8, Area(4).Top - 28), ForeColor);
        for (int i = 0; i < (HasLightbar ? 8 : 4); i++)
        {
            Rectangle area = Area(i);
            using var brush = new SolidBrush(Colors[i]);
            e.Graphics.FillRectangle(brush, area);
            using var border = new Pen(Selected.Contains(i) ? Color.DodgerBlue : Color.Gray, Selected.Contains(i) ? 3 : 1);
            e.Graphics.DrawRectangle(border, area);
            Color text = Colors[i].R * 0.299 + Colors[i].G * 0.587 + Colors[i].B * 0.114 > 145 ? Color.Black : Color.White;
            TextRenderer.DrawText(e.Graphics, (i < 4 ? "Zone " : "Section ") + (i % 4 + 1), Font,
                new Rectangle(area.X + 3, area.Y + 8, area.Width - 6, 24), text, TextFormatFlags.HorizontalCenter);
            if (i < 4) TextRenderer.DrawText(e.Graphics, CustomRgbEditor.StyleName(Scene.Sections[i].Style), Font,
                new Rectangle(area.X + 3, area.Bottom - 32, area.Width - 6, 24), text, TextFormatFlags.HorizontalCenter);
        }
    }
}
