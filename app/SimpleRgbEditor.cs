using GHelper.UI;
using GHelper.USB;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace GHelper;

public sealed class SimpleRgbEditor : RForm
{
    private CustomRgbScene baseline;
    private CustomRgbScene working;
    private CustomRgbScene customBase;
    private readonly Action<CustomRgbScene> apply;
    private readonly Action<string> saveProfiles;
    private string profilesJson;
    private RgbPreset? selectedPreset;
    private readonly Dictionary<string, RgbPresetTile> tiles = new();
    private readonly ComboBox variants = new RComboBox { Name = "variants", Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox speed = new RComboBox { Name = "simpleSpeed", Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox where = new RComboBox { Name = "where", Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox direction = new RComboBox { Name = "simpleDirection", Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly FlowLayoutPanel colors = new() { Name = "simpleColors", Dock = DockStyle.Fill, WrapContents = false, AutoScroll = true };
    private string[]? paletteOverride;
    private readonly TrackBar brightness = new() { Name = "simpleBrightness", Dock = DockStyle.Fill, Minimum = 0, Maximum = 100, Value = 75, TickStyle = TickStyle.None, LargeChange = 10, SmallChange = 5 };
    private readonly Label title = new() { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 18, FontStyle.Bold), AutoEllipsis = true };
    private readonly Label description = new() { Dock = DockStyle.Fill, AutoEllipsis = true };
    private readonly Label brightnessTitle = new() { Text = "Brightness · 75%", AutoSize = true };
    private readonly Label status = new() { Text = "Preview only · Apply when you're ready", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, AutoEllipsis = true };
    private readonly RgbLookPreview preview = new() { Dock = DockStyle.Fill, Name = "lookPreview" };
    private readonly System.Windows.Forms.Timer timer = new() { Interval = 50 };
    private readonly Stopwatch elapsed = Stopwatch.StartNew();
    private bool loading;
    private readonly float scale;

    public SimpleRgbEditor(CustomRgbScene scene, Action<CustomRgbScene> apply, string profilesJson,
        Action<string> saveProfiles, bool hasLightbar = true)
    {
        baseline = scene.Normalize();
        working = customBase = baseline;
        this.apply = apply;
        this.profilesJson = profilesJson;
        this.saveProfiles = saveProfiles;
        Text = "Lighting · G-Helper";
        Font = new Font("Segoe UI", 9.5f);
        AutoScaleMode = AutoScaleMode.None;
        using (var graphics = CreateGraphics()) scale = graphics.DpiX / 96f;
        ClientSize = new Size(S(1000), S(740));
        MinimumSize = new Size(S(940), S(720));
        StartPosition = FormStartPosition.CenterParent;
        ShowInTaskbar = false;

        var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 3, Padding = new Padding(S(22)) };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 67));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, S(64)));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, S(54)));
        Controls.Add(root);
        var heading = new Label { Text = "1 · Choose a look", UseMnemonic = false, AutoEllipsis = true, Font = new Font("Segoe UI", 18, FontStyle.Bold), Dock = DockStyle.Fill };
        var instruction = new Label { Text = "Choose → Adjust → Apply", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight };
        root.Controls.Add(heading, 0, 0);
        root.Controls.Add(instruction, 1, 0);

        var gallery = new FlowLayoutPanel { Name = "presetGallery", Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown,
            WrapContents = false, AutoScroll = true, Padding = new Padding(0, 0, S(14), 0) };
        root.Controls.Add(gallery, 0, 1);
        foreach (var preset in RgbPresets.All)
        {
            var tile = new RgbPresetTile(preset) { Name = "preset_" + preset.Id, Height = S(59), Margin = new Padding(0, 0, 0, S(7)) };
            tile.Click += (_, _) => SelectPreset(preset);
            tiles[preset.Id] = tile;
            gallery.Controls.Add(tile);
        }
        void ResizeTiles()
        {
            int width = Math.Max(120, gallery.ClientSize.Width - gallery.Padding.Right - SystemInformation.VerticalScrollBarWidth);
            foreach (var tile in tiles.Values) tile.Width = width;
        }
        gallery.SizeChanged += (_, _) => ResizeTiles();
        Shown += (_, _) => ResizeTiles();
        ResizeTiles();

        var details = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 8, Padding = new Padding(S(12), 0, 0, 0) };
        details.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        foreach (int height in new[] { 37, 34, 23, 34 }) details.RowStyles.Add(new RowStyle(SizeType.Absolute, S(height)));
        details.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        details.RowStyles.Add(new RowStyle(SizeType.Absolute, S(80)));
        details.RowStyles.Add(new RowStyle(SizeType.Absolute, S(80)));
        details.RowStyles.Add(new RowStyle(SizeType.Absolute, S(55)));
        root.Controls.Add(details, 1, 1);
        details.Controls.Add(title, 0, 0);
        details.Controls.Add(description, 0, 1);
        details.Controls.Add(new Label { Text = "Variant", Dock = DockStyle.Fill }, 0, 2);
        details.Controls.Add(variants, 0, 3);
        preview.HasLightbar = hasLightbar;
        details.Controls.Add(preview, 0, 4);
        var palettePanel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
        palettePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, S(25)));
        palettePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        palettePanel.Controls.Add(new Label { Text = "2 · Adjust · click a color to change it", Dock = DockStyle.Fill }, 0, 0);
        palettePanel.Controls.Add(colors, 0, 1);
        details.Controls.Add(palettePanel, 0, 5);
        var adjustments = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 2, Padding = new Padding(0, S(10), 0, 0) };
        adjustments.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 27));
        adjustments.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
        adjustments.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        adjustments.RowStyles.Add(new RowStyle(SizeType.Absolute, S(23)));
        adjustments.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        adjustments.Controls.Add(new Label { Text = "Speed", AutoSize = true }, 0, 0);
        adjustments.Controls.Add(brightnessTitle, 1, 0);
        speed.Items.AddRange(["Slow", "Normal", "Fast"]);
        speed.SelectedIndex = 1;
        adjustments.Controls.Add(speed, 0, 1);
        adjustments.Controls.Add(brightness, 1, 1);
        adjustments.Controls.Add(new Label { Text = "Direction", AutoSize = true }, 2, 0);
        direction.Items.AddRange(["As preset", "Reverse"]);
        direction.SelectedIndex = 0;
        adjustments.Controls.Add(direction, 2, 1);
        details.Controls.Add(adjustments, 0, 6);
        var target = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(0, S(10), 0, 0) };
        target.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        target.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        target.Controls.Add(new Label { Text = "Where", AutoSize = true, Padding = new Padding(0, S(4), S(15), 0) }, 0, 0);
        where.Items.AddRange(["Keyboard + lightbar", "Keyboard only", "Lightbar only"]);
        where.SelectedIndex = hasLightbar ? 0 : 1;
        where.Enabled = hasLightbar;
        target.Controls.Add(where, 1, 0);
        details.Controls.Add(target, 0, 7);

        var footer = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, Padding = new Padding(0, S(12), 0, 0) };
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        footer.Controls.Add(ActionButton("Customize sections…", "advanced", OpenAdvanced), 0, 0);
        status.Margin = new Padding(S(16), 0, S(8), 0);
        footer.Controls.Add(status, 1, 0);
        footer.Controls.Add(ActionButton("3 · Apply look", "applySimple", () =>
        {
            try { apply(working.Normalize()); baseline = working.Normalize(); status.Text = "Applied · " + working.Name; }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Lighting", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        }), 2, 0);
        footer.Controls.Add(ActionButton("Close", "closeSimple", Close), 3, 0);
        root.Controls.Add(footer, 0, 2);
        root.SetColumnSpan(footer, 2);

        variants.SelectedIndexChanged += (_, _) => { if (!loading) { paletteOverride = null; UpdatePalette(); UpdateLook(); } };
        speed.SelectedIndexChanged += (_, _) => UpdateLook();
        direction.SelectedIndexChanged += (_, _) => UpdateLook();
        where.SelectedIndexChanged += (_, _) => UpdateLook();
        brightness.ValueChanged += (_, _) => UpdateLook();
        InitTheme();
        bool dark = BackColor.GetBrightness() < 0.5;
        Color muted = dark ? Color.FromArgb(165, 173, 184) : Color.FromArgb(83, 91, 103);
        description.ForeColor = instruction.ForeColor = status.ForeColor = muted;
        foreach (var tile in tiles.Values) { tile.BackColor = BackColor; tile.ForeColor = ForeColor; }
        var applyButton = (Button)footer.Controls.Find("applySimple", true)[0];
        applyButton.BackColor = Color.FromArgb(194, 38, 61);
        applyButton.ForeColor = Color.White;
        applyButton.FlatAppearance.BorderSize = 0;
        ShowCurrentLook(baseline, false);
        timer.Tick += (_, _) => { preview.Colors = working.Render(elapsed.Elapsed.TotalSeconds); preview.Invalidate(); };
        timer.Start();
    }

    private int S(int value) => (int)Math.Round(value * scale);
    private Button ActionButton(string text, string name, Action action)
    {
        var button = new Button { Name = name, Text = text, AutoSize = false, FlatStyle = FlatStyle.Flat,
            Width = Math.Max(S(90), (int)Math.Ceiling(TextRenderer.MeasureText(text, Font).Width * scale) + S(40)), Height = S(34),
            Padding = new Padding(S(14), S(5), S(14), S(5)), Margin = new Padding(S(4), 0, 0, 0), Cursor = Cursors.Hand };
        button.FlatAppearance.BorderColor = Color.FromArgb(90, 98, 110);
        button.Click += (_, _) => action();
        return button;
    }

    private void SelectPreset(RgbPreset preset)
    {
        loading = true;
        selectedPreset = preset;
        paletteOverride = null;
        direction.SelectedIndex = 0;
        title.Text = preset.Name;
        description.Text = preset.Description;
        variants.Enabled = true;
        variants.Items.Clear();
        variants.Items.AddRange(preset.Variants);
        variants.SelectedIndex = 0;
        foreach (var tile in tiles.Values) { tile.Chosen = tile.Preset.Id == preset.Id; tile.Invalidate(); }
        loading = false;
        UpdatePalette();
        UpdateLook();
    }

    private void UpdatePalette()
    {
        foreach (Control child in colors.Controls.Cast<Control>().ToArray()) child.Dispose();
        var variant = variants.SelectedItem as RgbPresetVariant;
        bool editable = variant != null && variant.Style is not (RgbSectionStyle.Rainbow or RgbSectionStyle.Off);
        string[] palette = paletteOverride ?? variant?.Colors ?? ["#FFFFFF"];
        for (int i = 0; i < palette.Length; i++)
        {
            int index = i;
            var color = RgbSection.ParseColor(palette[i]);
            var button = new Button { Name = "simpleColor_" + i, Text = editable ? palette[i] : variant?.Style == RgbSectionStyle.Rainbow ? "Auto rainbow" : "—",
                Width = S(editable ? 98 : 130), Height = S(35), FlatStyle = FlatStyle.Flat,
                BackColor = color, ForeColor = color.GetBrightness() > 0.55 ? Color.Black : Color.White,
                Enabled = editable, Cursor = Cursors.Hand, AccessibleName = "Edit color " + (i + 1) };
            button.Click += (_, _) =>
            {
                using var picker = new RColorPicker(RgbSection.ParseColor(palette[index]));
                if (picker.ShowDialog(this) != DialogResult.OK) return;
                paletteOverride = (string[])palette.Clone();
                paletteOverride[index] = RgbSection.ToHex(picker.Color);
                UpdatePalette();
                UpdateLook();
            };
            colors.Controls.Add(button);
        }
        speed.Enabled = variant == null || variant.Style is not (RgbSectionStyle.Static or RgbSectionStyle.Off);
        direction.Enabled = variant != null && speed.Enabled;
        brightness.Enabled = variant?.Style != RgbSectionStyle.Off;
    }

    private void UpdateLook()
    {
        if (loading) return;
        double multiplier = speed.SelectedIndex == 0 ? 2 : speed.SelectedIndex == 2 ? 0.5 : 1;
        var target = (RgbPresetTarget)Math.Max(0, where.SelectedIndex);
        if (selectedPreset != null && variants.SelectedItem is RgbPresetVariant variant)
            working = RgbPresets.Create(selectedPreset.Id, variant.Id, baseline, target, multiplier, brightness.Value, paletteOverride, direction.SelectedIndex == 1);
        else
        {
            working = customBase.Normalize();
            int start = target == RgbPresetTarget.Lightbar ? 4 : 0, end = target == RgbPresetTarget.Keyboard ? 4 : 8;
            for (int i = start; i < end; i++) working = working.Replace([i], working.Sections[i] with
                { PeriodSeconds = customBase.Sections[i].PeriodSeconds * multiplier, Brightness = brightness.Value });
        }
        brightnessTitle.Text = "Brightness · " + brightness.Value + "%";
        status.Text = "Preview only · Apply when you're ready";
        elapsed.Restart();
        preview.Colors = working.Render(0);
        preview.Invalidate();
    }

    private void OpenAdvanced()
    {
        using var advanced = new CustomRgbEditor(working, scene =>
        {
            apply(scene);
            baseline = scene.Normalize();
            ShowCurrentLook(scene, true);
        }, profilesJson, json => { profilesJson = json; saveProfiles(json); }, preview.HasLightbar);
        advanced.ShowDialog(this);
    }

    private void ShowCurrentLook(CustomRgbScene scene, bool applied)
    {
        working = customBase = scene.Normalize();
        selectedPreset = null;
        paletteOverride = null;
        loading = true;
        title.Text = "Your current look";
        description.Text = "Pick a ready-made look on the left, or keep your custom colors.";
        variants.Items.Clear();
        variants.Items.Add("Your custom colors & effects");
        variants.SelectedIndex = 0;
        variants.Enabled = false;
        foreach (var tile in tiles.Values) { tile.Chosen = false; tile.Invalidate(); }
        speed.SelectedIndex = 1;
        direction.SelectedIndex = 0;
        brightness.Value = (int)Math.Round(scene.Sections.Average(s => s.Brightness));
        brightnessTitle.Text = "Brightness · " + brightness.Value + "%";
        loading = false;
        UpdatePalette();
        status.Text = applied ? "Applied · " + scene.Name : "Your saved look · choose a preset to try something new";
        elapsed.Restart();
        preview.Colors = working.Render(0);
    }

    protected override void Dispose(bool disposing) { if (disposing) timer.Dispose(); base.Dispose(disposing); }
}

internal sealed class RgbPresetTile : Button
{
    public readonly RgbPreset Preset;
    public bool Chosen;
    private bool hover;
    public RgbPresetTile(RgbPreset preset)
    {
        Preset = preset;
        AccessibleName = preset.Name;
        AccessibleDescription = preset.Description;
        Cursor = Cursors.Hand;
        DoubleBuffered = true;
        FlatStyle = FlatStyle.Flat;
        Font = new Font("Segoe UI", 10, FontStyle.Bold);
    }
    protected override void OnMouseEnter(EventArgs e) { hover = true; Invalidate(); base.OnMouseEnter(e); }
    protected override void OnMouseLeave(EventArgs e) { hover = false; Invalidate(); base.OnMouseLeave(e); }
    protected override void OnPaint(PaintEventArgs e)
    {
        bool dark = BackColor.GetBrightness() < 0.5;
        Color surface = dark ? Color.FromArgb(Chosen ? 40 : hover ? 35 : 30, Chosen ? 49 : hover ? 38 : 33, Chosen ? 59 : hover ? 43 : 38)
            : Chosen ? Color.FromArgb(226, 237, 245) : hover ? Color.FromArgb(240, 244, 248) : Color.FromArgb(248, 249, 251);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var bounds = new Rectangle(1, 1, Width - 3, Height - 3);
        using var shape = RgbLookPreview.Round(bounds, 8);
        using var fill = new SolidBrush(surface);
        e.Graphics.FillPath(fill, shape);
        using var border = new Pen(Chosen ? Color.FromArgb(215, 55, 78) : dark ? Color.FromArgb(57, 62, 70) : Color.FromArgb(216, 222, 230));
        e.Graphics.DrawPath(border, shape);
        int swatch = Math.Max(7, (int)(DeviceDpi / 96f * 8)), gap = swatch / 2;
        int swatchesWidth = Preset.Swatches.Length * (swatch + gap);
        TextRenderer.DrawText(e.Graphics, Preset.Name, Font, new Rectangle(14, 10, Width - swatchesWidth - 30, 24), ForeColor, TextFormatFlags.EndEllipsis);
        using var small = new Font(Font.FontFamily, 8.5f);
        TextRenderer.DrawText(e.Graphics, Preset.Description, small, new Rectangle(14, Height - 26, Width - 25, 22),
            dark ? Color.FromArgb(157, 166, 178) : Color.FromArgb(88, 98, 111), TextFormatFlags.EndEllipsis);
        for (int i = 0; i < Preset.Swatches.Length; i++)
        {
            using var color = new SolidBrush(RgbSection.ParseColor(Preset.Swatches[i]));
            e.Graphics.FillEllipse(color, Width - swatchesWidth - 7 + i * (swatch + gap), 15, swatch, swatch);
        }
        if (Focused) ControlPaint.DrawFocusRectangle(e.Graphics, Rectangle.Inflate(bounds, -5, -5));
    }
}

internal sealed class RgbLookPreview : Control
{
    public Color[] Colors = Enumerable.Repeat(Color.Black, 8).ToArray();
    public bool HasLightbar = true;
    public RgbLookPreview() { DoubleBuffered = true; ResizeRedraw = true; }
    internal static GraphicsPath Round(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();
        int d = radius * 2;
        path.AddArc(rect.Left, rect.Top, d, d, 180, 90); path.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
        path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90); path.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        int pad = Math.Max(15, Width / 25), width = Math.Max(100, Width - pad * 2);
        int keyboardHeight = Math.Clamp(Height - 112, 76, 170);
        bool dark = BackColor.GetBrightness() < 0.5;
        var muted = dark ? Color.FromArgb(158, 169, 183) : Color.FromArgb(80, 91, 105);
        TextRenderer.DrawText(e.Graphics, "Keyboard", Font, new Point(pad, 15), muted);
        var keyboard = new Rectangle(pad, 41, width, keyboardHeight);
        using var shape = Round(keyboard, 10);
        using var chassis = new SolidBrush(dark ? Color.FromArgb(21, 24, 29) : Color.FromArgb(223, 228, 235));
        e.Graphics.FillPath(chassis, shape);
        int keyGap = 4, cols = 16, rows = 5;
        float kw = (width - 20 - keyGap * (cols - 1)) / (float)cols;
        float kh = (keyboardHeight - 18 - keyGap * (rows - 1)) / (float)rows;
        for (int row = 0; row < rows; row++) for (int col = 0; col < cols; col++)
        {
            using var color = new SolidBrush(Colors[col / 4]);
            e.Graphics.FillRectangle(color, pad + 10 + col * (kw + keyGap), 50 + row * (kh + keyGap), Math.Max(2, kw), Math.Max(2, kh));
        }
        if (!HasLightbar) return;
        int barTop = keyboard.Bottom + 34;
        TextRenderer.DrawText(e.Graphics, "Lightbar", Font, new Point(pad, keyboard.Bottom + 9), muted);
        for (int i = 0; i < 4; i++)
        {
            using var color = new SolidBrush(Colors[4 + i]);
            e.Graphics.FillRectangle(color, pad + i * (width / 4f), barTop, width / 4f - 2, 12);
        }
    }
}
