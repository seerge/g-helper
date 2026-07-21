using System.Drawing.Drawing2D;

namespace GHelper.UI
{
    // Themed HSV color picker replacing the native Win32 ColorDialog. The swatch grid is saved to
    // the aura_color_custom config key (0x00BBGGRR COLORREF, same format ColorDialog used).
    public class RColorPicker : RForm
    {
        public Color Color { get; set; } = Color.Red;

        // Fires when the color is committed (swatch/hex click or drag release) for the live device
        // apply. Dragging the square/bar only previews in-dialog until mouse-up. On Cancel the
        // original color is re-raised so the caller can revert.
        public event Action<Color>? ColorChanged;

        private const int Cols = 14;
        private const string CustomKey = "aura_color_custom";

        private static Color C(int rgb) => Color.FromArgb(255, (rgb >> 16) & 0xFF, (rgb >> 8) & 0xFF, rgb & 0xFF);

        private static readonly Color[] Defaults =
        {
            C(0xFF0000), C(0xFF4000), C(0xFF8000), C(0xFFBF00), C(0xFFFF00), C(0x80FF00), C(0x00FF00),
            C(0x00FF80), C(0x00FFFF), C(0x0080FF), C(0x0000FF), C(0x8000FF), C(0xFF00FF), C(0xFF0080),
            C(0xFFFFFF), C(0xE0E0E0), C(0xC0C0C0), C(0xA0A0A0), C(0x808080), C(0x606060), C(0x404040),
            C(0x000000), C(0xFFC0C0), C(0xFFE0C0), C(0xFFFFC0), C(0xC0FFC0), C(0xC0FFFF), C(0xC0C0FF),
        };

        private readonly Color originalColor;
        private readonly bool allowRandom;
        private float scale;
        private float hue, sat, val;

        private SVPanel svPanel;
        private HuePanel huePanel;
        private Panel preview;
        private RTextBox hexBox;
        private Label rgbLabel;
        private Swatch? active;

        private readonly List<Color> customColors = new();
        private Swatch[] customSwatches;
        private bool suppressHex;

        private int S(int v) => (int)Math.Round(v * scale);

        public RColorPicker(Color initial, bool allowRandom = false)
        {
            Color = initial;
            originalColor = initial;
            this.allowRandom = allowRandom;
            (hue, sat, val) = RgbToHsv(initial);

            Font = new Font("Segoe UI", 9F);
            Text = "Color";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            TopMost = true;
            StartPosition = FormStartPosition.CenterParent;
            AutoScaleMode = AutoScaleMode.None;

            using (var g = CreateGraphics()) scale = g.DpiX / 96f;

            LoadCustom();
            BuildLayout();
            InitTheme();
            RefreshCustom();
            UpdateAll(null);

            FormClosing += (s, e) =>
            {
                if (DialogResult != DialogResult.OK && ColorToColorRef(Color) != ColorToColorRef(originalColor)) ColorChanged?.Invoke(originalColor);
            };
        }

        private void BuildLayout()
        {
            const int pad = 12, square = 200, hueW = 18, rightW = 150, sw = 22, gap = 6;

            svPanel = new SVPanel { Location = new Point(S(pad), S(pad)), Size = new Size(S(square), S(square)), UiScale = scale };
            svPanel.Picked += (s, v) => { sat = s; val = v; Color = HsvToRgb(hue, sat, val); Preview(svPanel); };
            svPanel.Committed += Commit;

            int hueX = pad + square + 8;
            huePanel = new HuePanel { Location = new Point(S(hueX), S(pad)), Size = new Size(S(hueW), S(square)), UiScale = scale };
            huePanel.Picked += h => { hue = h; Color = HsvToRgb(hue, sat, val); Preview(huePanel); };
            huePanel.Committed += Commit;

            int rightX = hueX + hueW + 16;

            preview = new Panel { Location = new Point(S(rightX), S(pad)), Size = new Size(S(rightW), S(110)), BorderStyle = BorderStyle.FixedSingle };

            var hexLabel = new Label { Text = "Hex", Location = new Point(S(rightX), S(pad + 118)), AutoSize = true };
            hexBox = new RTextBox { Location = new Point(S(rightX), S(pad + 145)), Size = new Size(S(rightW), S(23)), MaxLength = 7 };
            hexBox.TextChanged += (s, e) => { if (!suppressHex) ApplyHex(false); };
            hexBox.Leave += (s, e) => ApplyHex(true);

            rgbLabel = new Label { Location = new Point(S(rightX), S(pad + 174)), AutoSize = true, BackColor = Color.Transparent };

            int gridY = pad + square + 10;
            for (int i = 0; i < Defaults.Length; i++)
                AddSwatch(pad + (i % Cols) * (sw + gap), gridY + (i / Cols) * (sw + gap), sw, x => { SetActive(x); Apply(x.Color); }).Color = Defaults[i];

            int presetRows = (Defaults.Length + Cols - 1) / Cols;
            int customY = gridY + presetRows * (sw + gap) + 8;
            customSwatches = new Swatch[Cols];
            for (int i = 0; i < Cols; i++)
            {
                int idx = i;
                customSwatches[i] = AddSwatch(pad + i * (sw + gap), customY, sw, _ => OnCustomClick(idx));
            }

            int formW = rightX + rightW + pad;
            int buttonsY = customY + sw + 14;

            var ok = new RButton { Text = "OK", Size = new Size(S(84), S(28)), DialogResult = DialogResult.OK };
            var cancel = new RButton { Text = "Cancel", Size = new Size(S(84), S(28)), DialogResult = DialogResult.Cancel };
            cancel.Location = new Point(S(formW - pad - 84), S(buttonsY));
            ok.Location = new Point(S(formW - pad - 84 - 8 - 84), S(buttonsY));

            if (allowRandom)
            {
                var random = new RButton { Text = Properties.Strings.AuraRandomColor, Size = new Size(S(84), S(28)), Location = new Point(S(pad), S(buttonsY)) };
                random.Click += (s, e) => Apply(Color.Black);
                Controls.Add(random);
            }

            Controls.AddRange(new Control[] { svPanel, huePanel, preview, hexLabel, hexBox, rgbLabel, ok, cancel });

            AcceptButton = ok;
            CancelButton = cancel;
            ClientSize = new Size(S(formW), S(buttonsY + 28 + pad));
        }

        private Swatch AddSwatch(int x, int y, int size, Action<Swatch> onClick)
        {
            var swatch = new Swatch { Location = new Point(S(x), S(y)), Size = new Size(S(size), S(size)), UiScale = scale };
            swatch.Click += (s, e) => onClick(swatch);
            Controls.Add(swatch);
            return swatch;
        }

        private void SetActive(Swatch sw)
        {
            if (active != null && active != sw) { active.Selected = false; active.Invalidate(); }
            active = sw;
            sw.Selected = true;
            sw.Invalidate();
        }

        // Fixed Cols slots, white by default. Garbage config is tolerated: only valid integer tokens fill slots
        // (left to right), bad tokens are skipped and any beyond Cols are ignored — empty/short/long never breaks it.
        private void LoadCustom()
        {
            for (int i = 0; i < Cols; i++) customColors.Add(Color.White);

            int idx = 0;
            foreach (var s in (AppConfig.GetString(CustomKey, "") ?? "").Split('-'))
            {
                if (idx >= Cols) break;
                if (int.TryParse(s, out int v)) customColors[idx++] = ColorRefToColor(v);
            }
        }

        // Every slot is an editable preset: clicking selects and applies its color; editing then updates it in place.
        private void OnCustomClick(int idx)
        {
            SetActive(customSwatches[idx]);
            Apply(customColors[idx]);
        }

        private void RefreshCustom()
        {
            for (int i = 0; i < customSwatches.Length; i++)
                customSwatches[i].Color = customColors[i];
        }

        private static int ColorToColorRef(Color c) => c.R | (c.G << 8) | (c.B << 16);
        private static Color ColorRefToColor(int v) => Color.FromArgb(255, v & 0xFF, (v >> 8) & 0xFF, (v >> 16) & 0xFF);

        // Index of the selected editable custom slot, or -1 if the active swatch isn't one (fixed preset / none).
        private int ActiveCustomIndex
        {
            get { int i = active == null ? -1 : Array.IndexOf(customSwatches, active); return i < customColors.Count ? i : -1; }
        }

        // Live in-dialog preview while dragging; the device apply waits for mouse-up via Commit.
        // A selected custom slot is edited in place; editing away from a fixed preset clears its highlight.
        private void Preview(Control source)
        {
            UpdateAll(source);
            int idx = ActiveCustomIndex;
            if (idx >= 0) { customColors[idx] = Color; customSwatches[idx].Color = Color; }
            else if (active != null) { active.Selected = false; active.Invalidate(); active = null; }
        }

        // Persist an in-place custom-slot edit on mouse-up (not per drag-step) alongside the device apply.
        private void Commit()
        {
            if (ActiveCustomIndex >= 0) AppConfig.Set(CustomKey, string.Join("-", customColors.Select(ColorToColorRef)));
            ColorChanged?.Invoke(Color);
        }

        // Discrete pick (swatch/custom): set the color, sync the controls and apply to the device.
        private void Apply(Color c)
        {
            Color = c;
            (hue, sat, val) = RgbToHsv(c);
            UpdateAll(null);
            Commit();
        }

        private void ApplyHex(bool normalize)
        {
            string text = hexBox.Text.Trim().TrimStart('#');
            if (text.Length == 6 && int.TryParse(text, System.Globalization.NumberStyles.HexNumber, null, out int rgb))
            {
                Color = C(rgb);
                (hue, sat, val) = RgbToHsv(Color);
                Preview(hexBox);
                Commit();
            }
            else if (normalize) hexBox.Text = $"#{Color.R:X2}{Color.G:X2}{Color.B:X2}";
        }

        private void UpdateAll(Control? source)
        {
            svPanel.Hue = hue;
            svPanel.Sat = sat;
            svPanel.Val = val;
            huePanel.Hue = hue;

            if (source != svPanel) svPanel.Invalidate();
            if (source != huePanel) huePanel.Invalidate();

            preview.BackColor = Color;
            if (source != hexBox) { suppressHex = true; hexBox.Text = $"#{Color.R:X2}{Color.G:X2}{Color.B:X2}"; suppressHex = false; }
            rgbLabel.Text = $"R {Color.R}   G {Color.G}   B {Color.B}";
        }

        private static Color HsvToRgb(float h, float s, float v)
        {
            float c = v * s;
            float x = c * (1 - Math.Abs((h / 60f) % 2 - 1));
            float m = v - c;
            float r = 0, g = 0, b = 0;
            switch ((int)(h / 60) % 6)
            {
                case 0: r = c; g = x; break;
                case 1: r = x; g = c; break;
                case 2: g = c; b = x; break;
                case 3: g = x; b = c; break;
                case 4: r = x; b = c; break;
                case 5: r = c; b = x; break;
            }
            return Color.FromArgb(255, (int)Math.Round((r + m) * 255), (int)Math.Round((g + m) * 255), (int)Math.Round((b + m) * 255));
        }

        private static (float h, float s, float v) RgbToHsv(Color color)
        {
            float r = color.R / 255f, g = color.G / 255f, b = color.B / 255f;
            float max = Math.Max(r, Math.Max(g, b)), min = Math.Min(r, Math.Min(g, b));
            float delta = max - min;

            float h = 0;
            if (delta > 0)
            {
                if (max == r) h = 60 * (((g - b) / delta) % 6);
                else if (max == g) h = 60 * ((b - r) / delta + 2);
                else h = 60 * ((r - g) / delta + 4);
            }
            if (h < 0) h += 360;

            return (h, max == 0 ? 0 : delta / max, max);
        }

        private class Swatch : Panel
        {
            public bool Selected;
            public float UiScale = 1;

            private Color color;
            public Color Color
            {
                get => color;
                set { color = value; BackColor = value; Invalidate(); }
            }

            public Swatch() { Cursor = Cursors.Hand; }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e); // BackColor fills the body
                var g = e.Graphics;
                int t = Selected ? Math.Max(2, (int)Math.Round(2 * UiScale)) : 1;
                var r = ClientRectangle;
                using var b = new SolidBrush(Selected ? RForm.colorStandard : RForm.borderMain);
                g.FillRectangle(b, r.X, r.Y, r.Width, t);
                g.FillRectangle(b, r.X, r.Bottom - t, r.Width, t);
                g.FillRectangle(b, r.X, r.Y, t, r.Height);
                g.FillRectangle(b, r.Right - t, r.Y, t, r.Height);
            }
        }

        // Shared drag plumbing + cached cursor pens for the SV square and hue bar.
        private abstract class PickPanel : Panel
        {
            public float UiScale = 1;
            public event Action? Committed;

            private Pen? penWhite;
            protected Pen PenWhite => penWhite ??= new Pen(Color.White, 2 * UiScale);

            protected PickPanel() { DoubleBuffered = true; }

            protected abstract void Pick(MouseEventArgs e);

            protected override void OnMouseDown(MouseEventArgs e) { base.OnMouseDown(e); Pick(e); }
            protected override void OnMouseMove(MouseEventArgs e) { base.OnMouseMove(e); if (e.Button == MouseButtons.Left) Pick(e); }
            protected override void OnMouseUp(MouseEventArgs e) { base.OnMouseUp(e); Committed?.Invoke(); }

            protected override void Dispose(bool disposing)
            {
                if (disposing) penWhite?.Dispose();
                base.Dispose(disposing);
            }
        }

        // Saturation/Value square: white->hue horizontal gradient with a transparent->black vertical overlay.
        private class SVPanel : PickPanel
        {
            public float Hue, Sat, Val;
            public event Action<float, float>? Picked;

            private LinearGradientBrush? satBrush, valBrush;
            private float satHue = -1;

            public SVPanel() { Cursor = Cursors.Cross; }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                var rect = ClientRectangle;

                if (satBrush == null || satHue != Hue)
                {
                    satBrush?.Dispose();
                    satBrush = new LinearGradientBrush(rect, Color.White, HsvToRgb(Hue, 1, 1), LinearGradientMode.Horizontal);
                    satHue = Hue;
                }
                valBrush ??= new LinearGradientBrush(rect, Color.FromArgb(0, 0, 0, 0), Color.Black, LinearGradientMode.Vertical);

                g.FillRectangle(satBrush, rect);
                g.FillRectangle(valBrush, rect);

                int cx = (int)(Sat * rect.Width), cy = (int)((1 - Val) * rect.Height), r = (int)(9 * UiScale);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawEllipse(PenWhite, cx - r, cy - r, r * 2, r * 2);
                g.DrawEllipse(Pens.Black, cx - r - 1, cy - r - 1, r * 2 + 2, r * 2 + 2);
            }

            protected override void Pick(MouseEventArgs e)
            {
                Sat = Math.Clamp((float)e.X / Width, 0, 1);
                Val = Math.Clamp(1 - (float)e.Y / Height, 0, 1);
                Invalidate();
                Picked?.Invoke(Sat, Val);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing) { satBrush?.Dispose(); valBrush?.Dispose(); }
                base.Dispose(disposing);
            }
        }

        // Vertical hue strip (red -> ... -> red).
        private class HuePanel : PickPanel
        {
            public float Hue;
            public event Action<float>? Picked;

            private static readonly Color[] Stops = { Color.Red, Color.Yellow, Color.Lime, Color.Cyan, Color.Blue, Color.Magenta, Color.Red };
            private LinearGradientBrush? hueBrush;

            public HuePanel() { Cursor = Cursors.Hand; }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                var rect = ClientRectangle;

                if (hueBrush == null)
                {
                    var fill = rect; fill.Inflate(0, 1); // avoid GDI+ first-row wrap artifact
                    hueBrush = new LinearGradientBrush(fill, Color.Black, Color.Black, LinearGradientMode.Vertical);
                    var pos = new float[Stops.Length];
                    for (int i = 0; i < Stops.Length; i++) pos[i] = i / (float)(Stops.Length - 1);
                    hueBrush.InterpolationColors = new ColorBlend { Colors = Stops, Positions = pos };
                }
                g.FillRectangle(hueBrush, rect);

                int y = (int)(Hue / 360f * rect.Height), h = (int)(4 * UiScale);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawRectangle(PenWhite, 1, y - h, rect.Width - 3, h * 2);
                g.DrawRectangle(Pens.Black, 0, y - h - 1, rect.Width - 1, h * 2 + 2);
            }

            protected override void Pick(MouseEventArgs e)
            {
                Hue = Math.Clamp((float)e.Y / Height, 0, 1) * 360;
                Invalidate();
                Picked?.Invoke(Hue);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing) hueBrush?.Dispose();
                base.Dispose(disposing);
            }
        }
    }
}
