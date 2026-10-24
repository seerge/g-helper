using GHelper.UI;
using GHelper.USB;
using System.Diagnostics;

namespace GHelper;

public sealed class LightingModeEditor : RForm
{
    private readonly int mode;
    private ModeLightingSettings working;
    private readonly Action<ModeLightingSettings> apply;
    private readonly Func<Rectangle, int, int, Bitmap>? capture;
    private bool loading;
    private readonly ComboBox surface = Combo("modeSurface"), presets = Combo("modePreset"), motion = Combo("gradientMotion"), monitor = Combo("ambientMonitor");
    private readonly NumericUpDown seconds = Number("modeSeconds", .2m, 60, 4, 1), brightness = Number("modeBrightness", 0, 100, 100),
        boost = Number("ambientBoost", 50, 300, 100),
        shadows = Number("ambientShadows", 0, 100, 0), saturation = Number("ambientSaturation", 0, 200, 100), smoothing = Number("ambientSmoothing", 0, 95, 50);
    private readonly CheckBox reverse = new() { Text = "Reverse direction", AutoSize = true, Name = "modeReverse" },
        rainbow = new() { Text = "Use full rainbow (automatic colors)", AutoSize = true, Name = "fullRainbow" },
        black = new() { Text = "Keep black scenes off", AutoSize = true, Name = "ambientKeepBlack" };
    private readonly FlowLayoutPanel palette = new() { Name = "modeColors", AutoSize = true, WrapContents = true, MaximumSize = new Size(410, 0) };
    private readonly Label status = new() { Text = "Preview only · Apply to update lighting", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
    private readonly RgbLookPreview preview = new() { Dock = DockStyle.Fill };
    private readonly AmbientRegionPreview regions = new() { Dock = DockStyle.Fill, Name = "ambientRegions" };
    private readonly System.Windows.Forms.Timer timer = new() { Interval = 100 };
    private readonly Stopwatch elapsed = Stopwatch.StartNew();
    private Color[]? ambientPreviewPrevious;
    private readonly List<(string Name, string[] Colors)> choices = new();
    private readonly FlowLayoutPanel options = new() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true };
    private ModeSurface Current => surface.SelectedIndex == 1 ? working.Lightbar : working.Keyboard;
    private void SetCurrent(ModeSurface value) => working = surface.SelectedIndex == 1 ? working with { Lightbar = value.Normalize(mode) } : working with { Keyboard = value.Normalize(mode) };

    public LightingModeEditor(int mode, ModeLightingSettings settings, Action<ModeLightingSettings> apply, bool hasLightbar = true,
        Func<Rectangle, int, int, Bitmap>? capture = null)
    {
        this.mode = mode;
        if (!ModeLightingSettings.Supports(mode)) throw new ArgumentOutOfRangeException(nameof(mode));
        working = settings.Normalize(mode);
        this.apply = apply;
        this.capture = capture;
        string name = mode switch { 2 => "Color Cycle", 3 => "Rainbow", 24 => "Gradient", _ => "Ambient" };
        Text = name + " · Modes";
        Name = "lightingModeEditor";
        Font = new Font("Segoe UI", 10);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(940, 680);
        MinimumSize = new Size(900, 650);
        StartPosition = FormStartPosition.CenterParent;
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 3, Padding = new Padding(20) };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 51)); root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 49));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 58)); root.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); root.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
        Controls.Add(root);
        root.Controls.Add(new Label { Text = name + " settings", Font = new Font(Font.FontFamily, 19, FontStyle.Bold), Dock = DockStyle.Fill }, 0, 0);
        surface.Items.Add("Keyboard"); if (hasLightbar) surface.Items.Add("Lightbar");
        surface.SelectedIndex = 0; surface.Enabled = hasLightbar;
        root.Controls.Add(surface, 1, 0);
        var visual = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Padding = new Padding(0, 0, 15, 0) };
        visual.RowStyles.Add(new RowStyle(SizeType.Percent, mode == 22 ? 50 : 0)); visual.RowStyles.Add(new RowStyle(SizeType.Percent, mode == 22 ? 50 : 100));
        regions.Visible = mode == 22;
        visual.Controls.Add(regions, 0, 0); visual.Controls.Add(preview, 0, 1);
        preview.HasLightbar = hasLightbar;
        root.Controls.Add(visual, 0, 1); root.Controls.Add(options, 1, 1);
        if (mode == 22) BuildAmbient(); else BuildEffects();
        var footer = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, Padding = new Padding(0, 12, 0, 0) };
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        for (int i = 0; i < 3; i++) footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        footer.Controls.Add(status, 0, 0);
        footer.Controls.Add(Button("Reset section", "modeReset", () => { SetCurrent(mode == 22 ? new ModeSurface() : ModeLightingSettings.Default(mode).Keyboard); LoadSurface(); }), 1, 0);
        footer.Controls.Add(Button("Apply", "modeApply", () =>
        {
            try { apply(working.Normalize(mode)); status.Text = "Applied · keyboard and lightbar settings saved"; }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Lighting", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        }), 2, 0);
        footer.Controls.Add(Button("Close", "modeClose", Close), 3, 0);
        root.Controls.Add(footer, 0, 2); root.SetColumnSpan(footer, 2);
        surface.SelectedIndexChanged += (_, _) => LoadSurface();
        foreach (var field in new[] { seconds, brightness, boost, shadows, saturation, smoothing }) field.ValueChanged += (_, _) => SaveControls();
        foreach (var field in new[] { reverse, rainbow, black }) field.CheckedChanged += (_, _) => { SaveControls(); if (!loading) DrawPalette(); };
        motion.SelectedIndexChanged += (_, _) => SaveControls();
        monitor.SelectedIndexChanged += (_, _) => { if (!loading) { SaveControls(); RefreshThumbnail(); } };
        presets.SelectedIndexChanged += (_, _) =>
        {
            if (loading || presets.SelectedIndex <= 0) return;
            SetCurrent(Current with { Colors = choices[presets.SelectedIndex].Colors, Rainbow = false }); LoadSurface();
        };
        regions.AreaChanged += area =>
        {
            SetCurrent(Current with { Ambient = Current.Ambient with { Area = area } }); regions.Area = Current.Ambient.Area;
            options.Controls.Find("ambientAreaPreset", true).OfType<ComboBox>().Single().SelectedIndex = 5;
            Changed();
        };
        InitTheme();
        var applyButton = (System.Windows.Forms.Button)footer.Controls.Find("modeApply", true)[0];
        applyButton.BackColor = Color.FromArgb(194, 38, 61); applyButton.ForeColor = Color.White;
        LoadSurface();
        timer.Tick += (_, _) => UpdatePreview(); timer.Start();
    }

    private static ComboBox Combo(string name) => new RComboBox { Name = name, Width = 375, DropDownStyle = ComboBoxStyle.DropDownList };
    private static NumericUpDown Number(string name, decimal min, decimal max, decimal value, int decimals = 0) =>
        new RNumericUpDown { Name = name, Minimum = min, Maximum = max, Value = value, DecimalPlaces = decimals, Increment = decimals > 0 ? .2m : 5, Width = 150 };
    private static System.Windows.Forms.Button Button(string text, string name, Action action)
    {
        var button = new System.Windows.Forms.Button { Text = text, Name = name, AutoSize = true, MinimumSize = new Size(85, 34), Padding = new Padding(8, 2, 8, 2), FlatStyle = FlatStyle.Flat };
        button.Click += (_, _) => action(); return button;
    }
    private void Add(string label, Control field)
    {
        var panel = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.TopDown, WrapContents = false, Margin = new Padding(0, 0, 0, 12) };
        if (label.Length > 0) panel.Controls.Add(new Label { Text = label, AutoSize = true, MaximumSize = new Size(400, 0) });
        panel.Controls.Add(field); options.Controls.Add(panel);
    }
    private void BuildEffects()
    {
        choices.Add(("Custom / current colors", []));
        foreach (var family in RgbPresets.All.Where(p => p.Id is not ("off" or "rainbow" or "spectrum")))
            foreach (var variant in family.Variants.Where(v => v.Style != RgbSectionStyle.Rainbow))
                choices.Add((family.Name + " · " + variant.Name, variant.Colors.Take(mode == 24 ? 2 : 8).ToArray()));
        presets.Items.AddRange(choices.Select(c => (object)c.Name).ToArray());
        Add("Start with a color preset", presets);
        if (mode == 3) Add("", rainbow);
        Add(mode == 24 ? "Your colors · maximum 2" : "Your colors · maximum 8", palette);
        if (mode == 24)
        {
            motion.Items.AddRange(["Still", "Flow · cycle animation", "Back and forth"]); Add("Gradient animation", motion);
        }
        Add("Cycle (seconds)", seconds);
        Add("Brightness (%)", brightness);
        Add("", reverse);
        Add("Keyboard and lightbar keep separate settings. Preview first, then Apply.", new Label { AutoSize = true });
    }
    private void BuildAmbient()
    {
        foreach (var screen in Screen.AllScreens) monitor.Items.Add(screen.DeviceName + $" · {screen.Bounds.Width} × {screen.Bounds.Height}");
        Add("Screen to sample", monitor);
        var areas = Combo("ambientAreaPreset");
        areas.Items.AddRange(["Whole screen", "Left half", "Right half", "Top half", "Bottom half", "Custom · drag on preview"]);
        areas.SelectedIndex = 5;
        areas.SelectedIndexChanged += (_, _) =>
        {
            CaptureArea? area = areas.SelectedIndex switch { 0 => new(), 1 => new() { Width = .5 }, 2 => new() { X = .5, Width = .5 },
                3 => new() { Height = .5 }, 4 => new() { Y = .5, Height = .5 }, _ => null };
            if (area == null || loading) return;
            SetCurrent(Current with { Ambient = Current.Ambient with { Area = area } }); regions.Area = area; Changed();
        };
        Add("Sampling area · drag a rectangle on the screen preview", areas);
        Add("", Button("Refresh screen preview", "ambientRefresh", RefreshThumbnail));
        Add("Brightness boost (%) · 100 is unchanged", boost);
        Add("Shadow lift (%) · brighten dim colors", shadows);
        Add("Color intensity (%) · gray stays gray", saturation);
        Add("Smoothing (%) · higher changes more slowly", smoothing);
        Add("", black);
        Add("Lighting brightness (%)", brightness);
        Add("Red rectangle: selected section. Blue: other section on this screen.", new Label { AutoSize = true });
    }
    private void LoadSurface()
    {
        loading = true;
        var current = Current;
        seconds.Value = (decimal)current.Seconds; brightness.Value = current.Brightness;
        reverse.Checked = current.Reverse; rainbow.Checked = current.Rainbow; motion.SelectedIndex = mode == 24 ? (int)current.Motion : -1;
        boost.Value = current.Ambient.Boost; shadows.Value = current.Ambient.ShadowLift; saturation.Value = current.Ambient.Saturation;
        smoothing.Value = current.Ambient.Smoothing; black.Checked = current.Ambient.KeepBlack;
        var screens = Screen.AllScreens;
        int monitorIndex = Array.FindIndex(screens, s => s.DeviceName == current.Ambient.Monitor);
        if (monitorIndex < 0) monitorIndex = Math.Max(0, Array.FindIndex(screens, s => s.Primary));
        monitor.SelectedIndex = mode == 22 ? monitorIndex : -1;
        if (mode == 22)
        {
            var areas = options.Controls.Find("ambientAreaPreset", true).OfType<ComboBox>().Single();
            areas.SelectedIndex = 5;
        }
        presets.SelectedIndex = mode == 22 ? -1 : Math.Max(0, choices.FindIndex(c => c.Colors.SequenceEqual(current.Colors)));
        loading = false;
        DrawPalette(); RefreshThumbnail(); Changed();
    }
    private void SaveControls()
    {
        if (loading) return;
        var current = Current;
        SetCurrent(current with { Seconds = (double)seconds.Value, Brightness = (int)brightness.Value, Reverse = reverse.Checked, Rainbow = rainbow.Checked,
            Motion = mode == 24 ? (GradientMotion)Math.Max(0, motion.SelectedIndex) : current.Motion,
            Ambient = current.Ambient with { Monitor = monitor.SelectedIndex >= 0 ? Screen.AllScreens[monitor.SelectedIndex].DeviceName : current.Ambient.Monitor,
                Boost = (int)boost.Value, ShadowLift = (int)shadows.Value, Saturation = (int)saturation.Value, Smoothing = (int)smoothing.Value, KeepBlack = black.Checked } });
        seconds.Enabled = mode != 24 || Current.Motion != GradientMotion.Still;
        reverse.Enabled = mode == 2 || mode == 3 || mode == 24 && Current.Motion != GradientMotion.Still;
        Changed();
    }
    private void DrawPalette()
    {
        foreach (Control child in palette.Controls.Cast<Control>().ToArray()) child.Dispose();
        if (mode == 22) return;
        for (int i = 0; i < Current.Colors.Length; i++)
        {
            int index = i; var color = RgbSection.ParseColor(Current.Colors[i]);
            var button = Button(Current.Colors[i], "modeColor_" + i, () =>
            {
                using var picker = new RColorPicker(RgbSection.ParseColor(Current.Colors[index]));
                if (picker.ShowDialog(this) != DialogResult.OK) return;
                var colors = (string[])Current.Colors.Clone(); colors[index] = RgbSection.ToHex(picker.Color);
                SetCurrent(Current with { Colors = colors }); DrawPalette(); Changed();
            });
            button.BackColor = color; button.ForeColor = color.GetBrightness() > .55 ? Color.Black : Color.White;
            button.Enabled = !Current.Rainbow; palette.Controls.Add(button);
        }
        var add = Button("+", "modeAddColor", () => { SetCurrent(Current with { Colors = [.. Current.Colors, "#FFFFFF"] }); DrawPalette(); Changed(); });
        add.Enabled = !Current.Rainbow && Current.Colors.Length < (mode == 24 ? 2 : 8); palette.Controls.Add(add);
        var remove = Button("−", "modeRemoveColor", () => { SetCurrent(Current with { Colors = Current.Colors[..^1] }); DrawPalette(); Changed(); });
        remove.Enabled = !Current.Rainbow && Current.Colors.Length > 1; palette.Controls.Add(remove);
    }
    private void Changed()
    {
        status.Text = "Preview only · Apply to update lighting";
        ambientPreviewPrevious = null;
        seconds.Enabled = mode != 24 || Current.Motion != GradientMotion.Still;
        reverse.Enabled = mode is 2 or 3 || mode == 24 && Current.Motion != GradientMotion.Still;
        elapsed.Restart(); UpdatePreview();
    }
    private Rectangle MonitorBounds(ModeSurface s) => (Screen.AllScreens.FirstOrDefault(screen => screen.DeviceName == s.Ambient.Monitor) ?? Screen.PrimaryScreen ?? Screen.AllScreens[0]).Bounds;
    private void RefreshThumbnail()
    {
        if (mode != 22) return;
        if (capture != null)
        {
            try { var bounds = MonitorBounds(Current); regions.SetImage(capture(bounds, 512, Math.Clamp((int)(512d * bounds.Height / bounds.Width), 64, 512))); }
            catch (Exception ex) { status.Text = "Screen preview unavailable: " + ex.Message; }
        }
        regions.Area = Current.Ambient.Area;
        var other = surface.SelectedIndex == 1 ? working.Keyboard : working.Lightbar;
        regions.OtherArea = MonitorBounds(Current) == MonitorBounds(other) ? other.Ambient.Area : null;
    }
    private void UpdatePreview()
    {
        try
        {
            if (mode != 22) preview.Colors = working.Render(mode, elapsed.Elapsed.TotalSeconds);
            else if (capture != null)
            {
                var colors = new Color[8];
                for (int group = 0; group < 2; group++)
                {
                    var s = group == 0 ? working.Keyboard : working.Lightbar;
                    using var bitmap = capture(s.Ambient.Area.ToPixels(MonitorBounds(s)), 4, 1);
                    for (int i = 0; i < 4; i++)
                    {
                        int index = group * 4 + i;
                        var color = ModeLightingSettings.Mix(Color.Black, s.Ambient.Transform(bitmap.GetPixel(i, 0)), s.Brightness / 100d);
                        colors[index] = ambientPreviewPrevious == null ? color : s.Ambient.Smooth(ambientPreviewPrevious[index], color);
                    }
                }
                ambientPreviewPrevious = colors;
                preview.Colors = colors;
            }
            preview.Invalidate();
        }
        catch (Exception ex) { status.Text = "Preview unavailable: " + ex.Message; }
    }
    protected override void Dispose(bool disposing) { if (disposing) timer.Dispose(); base.Dispose(disposing); }
}

internal sealed class AmbientRegionPreview : Control
{
    private Bitmap? image;
    private CaptureArea area = new();
    private PointF? start;
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public CaptureArea Area { get => area; set { area = value.Normalize(); Invalidate(); } }
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public CaptureArea? OtherArea { get; set; }
    public event Action<CaptureArea>? AreaChanged;
    public AmbientRegionPreview() { DoubleBuffered = true; ResizeRedraw = true; Cursor = Cursors.Cross; }
    public void SetImage(Bitmap value) { image?.Dispose(); image = value; Invalidate(); }
    private Rectangle ImageBounds
    {
        get
        {
            double aspect = image == null ? 16d / 9 : image.Width / (double)image.Height;
            int width = Math.Max(1, Math.Min(Width, (int)(Height * aspect))), height = Math.Max(1, (int)(width / aspect));
            return new Rectangle((Width - width) / 2, (Height - height) / 2, width, height);
        }
    }
    private PointF Relative(Point p) { var r = ImageBounds; return new((float)Math.Clamp((p.X - r.X) / (double)r.Width, 0, 1), (float)Math.Clamp((p.Y - r.Y) / (double)r.Height, 0, 1)); }
    protected override void OnMouseDown(MouseEventArgs e) { base.OnMouseDown(e); if (e.Button == MouseButtons.Left && ImageBounds.Contains(e.Location)) { start = Relative(e.Location); Capture = true; } }
    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e); if (start is not PointF origin) return;
        var end = Relative(e.Location);
        Area = new CaptureArea { X = Math.Min(origin.X, end.X), Y = Math.Min(origin.Y, end.Y), Width = Math.Max(.01, Math.Abs(end.X - origin.X)), Height = Math.Max(.01, Math.Abs(end.Y - origin.Y)) };
    }
    protected override void OnMouseUp(MouseEventArgs e) { base.OnMouseUp(e); if (start == null) return; OnMouseMove(e); start = null; Capture = false; AreaChanged?.Invoke(Area); }
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e); var r = ImageBounds;
        if (image != null) e.Graphics.DrawImage(image, r); else e.Graphics.FillRectangle(Brushes.Black, r);
        if (OtherArea != null) { using var pen = new Pen(Color.DeepSkyBlue, 2) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash }; e.Graphics.DrawRectangle(pen, OtherArea.ToPixels(r)); }
        using var selected = new Pen(Color.FromArgb(240, 55, 78), 3); e.Graphics.DrawRectangle(selected, Area.ToPixels(r));
    }
    protected override void Dispose(bool disposing) { if (disposing) image?.Dispose(); base.Dispose(disposing); }
}
