using GHelper.Peripherals.Keyboard;
using GHelper.UI;
using GHelper.USB;

namespace GHelper
{
    public partial class AuraKeys : RForm
    {
        private readonly KeyGrid keyGrid = new();

        public AuraKeys()
        {
            InitializeComponent();

            Text = Properties.Strings.LaptopKeyboard;
            buttonPaintColor.Text = Properties.Strings.Color;
            buttonFillAll.Text = "  " + Properties.Strings.FillAll;

            var (rows, targets) = Aura.KeyLayout();
            keyGrid.Colors = Aura.KeyColors();
            keyGrid.PaintColor = Aura.Color1;
            keyGrid.BindPaint(buttonPaintColor, panelPalette, buttonFillAll);
            keyGrid.Build(panelKeys, rows, AuraKeyboardLayouts.CultureLayout(InputLanguage.CurrentInputLanguage.Culture), targets);
            keyGrid.Painted += KeyGrid_Painted;

            string? layout = AppConfig.GetString("aura_layout");
            if (layout is not null)
            {
                comboBoxLayout.Items.Add("(auto)");
                comboBoxLayout.Items.AddRange(AuraKeyboardLayouts.LaptopNames.ToArray<object>());
                comboBoxLayout.SelectedItem = layout;
                comboBoxLayout.SelectedIndexChanged += ComboBoxLayout_SelectedIndexChanged;
                comboBoxLayout.Visible = true;
                panelKeys.Top = comboBoxLayout.Bottom + comboBoxLayout.Top - buttonPaintColor.Bottom;
            }

            ClientSize = new Size(panelKeys.Right + panelKeys.Left, panelKeys.Bottom + panelKeys.Left);

            InitTheme(true);
            keyGrid.Refresh();

            Shown += AuraKeys_Shown;
        }

        private void AuraKeys_Shown(object? sender, EventArgs e)
        {
            if (Height > Program.settingsForm.Height)
            {
                Top = Program.settingsForm.Top + Program.settingsForm.Height - Height;
            }
            else
            {
                Top = Program.settingsForm.Top;
            }

            Left = Program.settingsForm.Left - Width - 5;
        }

        private void ComboBoxLayout_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (comboBoxLayout.SelectedIndex > 0) AppConfig.Set("aura_layout", (string)comboBoxLayout.SelectedItem!);
            else AppConfig.Remove("aura_layout");

            BeginInvoke(() => { Close(); Program.settingsForm.ToggleAuraKeys(); });
        }

        private void KeyGrid_Painted()
        {
            Aura.StoreKeyColors(keyGrid.Colors);
            Task.Run(() => Aura.ApplyKeyColors(keyGrid.Colors));
        }
    }
}
