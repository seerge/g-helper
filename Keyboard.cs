using System.Diagnostics.Metrics;

namespace GHelper
{
    public partial class Keyboard : Form
    {


        Dictionary<string, string> customActions = new Dictionary<string, string>
        {
          {"","--------------" },
          {"mute", "Volume Mute"},
          {"screenshot", "Screenshot"},
          {"play", "Play/Pause"},
          {"aura", "Aura"},
          {"custom", "Custom"}
        };


        public Keyboard()
        {
            InitializeComponent();

            comboM3.DropDownStyle = ComboBoxStyle.DropDownList;
            comboM3.DataSource = new BindingSource(customActions, null);
            comboM3.DisplayMember = "Value";
            comboM3.ValueMember = "Key";

            customActions[""] = "Performance";
            comboM4.DropDownStyle = ComboBoxStyle.DropDownList;
            comboM4.DataSource = new BindingSource(customActions, null);
            comboM4.DisplayMember = "Value";
            comboM4.ValueMember = "Key";

            comboM3.SelectedValueChanged += ComboM3_SelectedValueChanged;
            comboM4.SelectedValueChanged += ComboM4_SelectedValueChanged;

            textM3.TextChanged += TextM3_TextChanged;
            textM4.TextChanged += TextM4_TextChanged;

            Shown += Keyboard_Shown;
        }

        private void TextM3_TextChanged(object? sender, EventArgs e)
        {
            if (sender is null) return;
            TextBox tb = (TextBox)sender;
            Program.config.setConfig("m3_custom", tb.Text);
        }

        private void TextM4_TextChanged(object? sender, EventArgs e)
        {
            if (sender is null) return;
            TextBox tb = (TextBox)sender;
            Program.config.setConfig("m4_custom", tb.Text);
        }

        private void ComboKeyChanged(object? sender, string name = "m3")
        {
            if (sender is null) return;
            ComboBox cmb = (ComboBox)sender;

            if (cmb.SelectedValue is not null)
                Program.config.setConfig(name, cmb.SelectedValue.ToString());
        }

        private void ComboM4_SelectedValueChanged(object? sender, EventArgs e)
        {
            ComboKeyChanged(sender, "m4");
        }

        private void ComboM3_SelectedValueChanged(object? sender, EventArgs e)
        {
            ComboKeyChanged(sender, "m3");
        }

        private void Keyboard_Shown(object? sender, EventArgs e)
        {

            Top = Program.settingsForm.Top;
            Left = Program.settingsForm.Left - Width - 5;

            string m3 = Program.config.getConfigString("m3");
            string m4 = Program.config.getConfigString("m4");

            comboM3.SelectedValue = (m3 is not null) ? m3 : "";
            comboM4.SelectedValue = (m4 is not null) ? m4 : "";

            if (comboM3.SelectedValue is null) comboM3.SelectedValue = "";
            if (comboM4.SelectedValue is null) comboM4.SelectedValue = "";

            textM3.Text = Program.config.getConfigString("m3_custom");
            textM4.Text = Program.config.getConfigString("m4_custom");

        }
    }
}
