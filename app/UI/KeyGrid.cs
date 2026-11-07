using GHelper.Peripherals.Keyboard;
using System.Drawing.Drawing2D;

namespace GHelper.UI
{
    public class KeyGrid
    {
        private static readonly Color[] paletteColors =
        {
            Color.FromArgb(0, 0, 0), Color.FromArgb(255, 255, 255),
            Color.FromArgb(255, 0, 0), Color.FromArgb(255, 128, 0), Color.FromArgb(255, 255, 0),
            Color.FromArgb(128, 255, 0), Color.FromArgb(0, 255, 0), Color.FromArgb(0, 255, 255),
            Color.FromArgb(0, 128, 255), Color.FromArgb(0, 0, 255), Color.FromArgb(128, 0, 255),
            Color.FromArgb(255, 0, 255), Color.FromArgb(255, 0, 128), Color.FromArgb(128, 128, 128),
        };

        public readonly List<Button> Keys = new();
        public Color[] Colors = Array.Empty<Color>();
        public Color PaintColor;
        public bool Paintable = true;
        public int BorderSize { get; private set; } = 2;

        public event Action<Button>? KeyClick;
        public event Action? Painted;

        private readonly Dictionary<Button, int> keySpan = new();
        private int[]? targets;
        private RColorButton? paintButton;

        public void Build(Panel panel, List<KeyDef[]> rows, int multiLayout = -1, int[]? targets = null, Action<Button, KeyDef, int>? keyAdded = null)
        {
            this.targets = targets;

            float s = panel.DeviceDpi / 192f;
            int unit = (int)(84 * s);
            int keyGap = (int)(4 * s);
            BorderSize = Math.Max(2, (int)(2 * s));

            int led = 0;
            float width = 0, y = 0;
            foreach (var row in rows)
            {
                float x = 0;
                Button? prev = null;
                foreach (var def in row)
                {
                    x += def.Gap * unit;

                    if (prev is not null && def.Gap == 0 && prev.Text != "" && def.Name != "Lightbar" && AuraKeyboardLayouts.Legend(def.Name, multiLayout) == prev.Text)
                    {
                        prev.Width = (int)(x + def.Width * unit) - prev.Left - keyGap;
                        keySpan[prev] = keySpan.GetValueOrDefault(prev, 1) + 1;
                        x += def.Width * unit;
                        led++;
                        continue;
                    }

                    var key = new Button();
                    key.SetBounds((int)x, (int)((y + def.Top) * unit), (int)(def.Width * unit) - keyGap, (int)(def.Height * unit) - keyGap);
                    key.FlatStyle = FlatStyle.Flat;
                    key.FlatAppearance.BorderSize = BorderSize;
                    key.Font = new Font("Segoe UI", unit * 0.245f, GraphicsUnit.Pixel);
                    key.Text = AuraKeyboardLayouts.Legend(def.Name, multiLayout);
                    key.Tag = led;
                    key.Click += Key_Click;
                    key.Paint += Key_Paint;
                    panel.Controls.Add(key);
                    Keys.Add(key);
                    keyAdded?.Invoke(key, def, led);

                    x += def.Width * unit;
                    led++;
                    prev = key;
                }
                width = Math.Max(width, x);
                y += Math.Min(1f, row.Max(def => def.Top + def.Height));
            }

            panel.Size = new Size((int)width, (int)(y * unit));
        }

        public void BindPaint(RColorButton paintButton, Panel palette, Button fillAll)
        {
            this.paintButton = paintButton;
            paintButton.SwatchColor = PaintColor;
            paintButton.Click += PaintButton_Click;
            fillAll.Click += (_, _) => Fill();

            float s = palette.DeviceDpi / 192f;
            int size = (int)(40 * s);
            int gap = (int)(4 * s);

            for (int i = 0; i < paletteColors.Length; i++)
            {
                Color color = paletteColors[i];
                var swatch = new Button();
                swatch.SetBounds(i * (size + gap), (palette.Height - size) / 2, size, size);
                swatch.FlatStyle = FlatStyle.Flat;
                swatch.FlatAppearance.BorderSize = 1;
                swatch.BackColor = color;
                swatch.Click += (_, _) => SetPaintColor(color);
                palette.Controls.Add(swatch);
            }
        }

        private int Target(int led) => targets?[led] ?? led;

        public Color KeyColor(int led) => Target(led) < Colors.Length ? Colors[Target(led)] : Color.Gray;

        public void Refresh()
        {
            foreach (var key in Keys)
                if (key.Tag is int led) SetBorder(key, KeyColor(led));
        }

        private static void Key_Paint(object? sender, PaintEventArgs e)
        {
            if (sender is not Button key) return;

            int border = key.FlatAppearance.BorderSize;
            int radius = Math.Max(1, (int)(6 * key.DeviceDpi / 192f));
            bool hover = key.ClientRectangle.Contains(key.PointToClient(Cursor.Position)) && !key.FlatAppearance.MouseOverBackColor.IsEmpty;

            var g = e.Graphics;
            g.Clear(key.Parent?.BackColor ?? key.BackColor);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using var path = RComboBox.RoundedRect(new Rectangle(border / 2, border / 2, key.Width - border - 1, key.Height - border - 1), radius, radius);
            using var fill = new SolidBrush(hover ? key.FlatAppearance.MouseOverBackColor : key.BackColor);
            using var pen = new Pen(key.FlatAppearance.BorderColor, border);
            g.FillPath(fill, path);
            g.DrawPath(pen, path);

            TextRenderer.DrawText(g, key.Text, key.Font, key.ClientRectangle, key.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
        }

        public static void SetBorder(Button key, Color c)
        {
            if (key.FlatAppearance.BorderColor != c) key.FlatAppearance.BorderColor = c;
        }

        private void SetPaintColor(Color c)
        {
            PaintColor = c;
            if (paintButton is not null) paintButton.SwatchColor = c;
        }

        private void PaintButton_Click(object? sender, EventArgs e)
        {
            RColorPicker colorDlg = new RColorPicker(PaintColor);
            colorDlg.ColorChanged += SetPaintColor;
            colorDlg.ShowDialog(paintButton?.FindForm());
        }

        private void Fill()
        {
            if (!Paintable) return;

            Array.Fill(Colors, PaintColor);
            Refresh();
            Painted?.Invoke();
        }

        private void Key_Click(object? sender, EventArgs e)
        {
            if (sender is not Button key || key.Tag is not int led) return;

            KeyClick?.Invoke(key);
            if (!Paintable) return;

            bool changed = false;
            for (int i = 0; i < keySpan.GetValueOrDefault(key, 1); i++)
            {
                int target = Target(led + i);
                if (target < Colors.Length && Colors[target].ToArgb() != PaintColor.ToArgb())
                {
                    Colors[target] = PaintColor;
                    changed = true;
                }
            }
            if (!changed) return;

            Refresh();
            Painted?.Invoke();
        }
    }
}
