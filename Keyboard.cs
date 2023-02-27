namespace GHelper
{
    public partial class Keyboard : Form
    {
        public Keyboard()
        {
            InitializeComponent();

            comboM3.DropDownStyle = ComboBoxStyle.DropDownList;
            comboM3.SelectedIndex = 0;

            comboM4.DropDownStyle = ComboBoxStyle.DropDownList;
            comboM4.SelectedIndex = 0;

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

        private void ComboM4_SelectedValueChanged(object? sender, EventArgs e)
        {
            if (sender is null) return;
            ComboBox cmb = (ComboBox)sender;
            Program.config.setConfig("m4", cmb.SelectedIndex);
        }

        private void ComboM3_SelectedValueChanged(object? sender, EventArgs e)
        {
            if (sender is null) return;
            ComboBox cmb = (ComboBox)sender;
            Program.config.setConfig("m3", cmb.SelectedIndex);
        }

        private void Keyboard_Shown(object? sender, EventArgs e)
        {

            Top = Program.settingsForm.Top;
            Left = Program.settingsForm.Left - Width - 5;

            int m3 = Program.config.getConfig("m3");
            int m4 = Program.config.getConfig("m4");

            if (m3 != -1)
                comboM3.SelectedIndex = m3;

            if (m4 != -1)
                comboM4.SelectedIndex = m4;

            textM3.Text = Program.config.getConfigString("m3_custom");
            textM4.Text = Program.config.getConfigString("m4_custom");

        }
    }
}
