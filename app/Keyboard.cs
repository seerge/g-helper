using CustomControls;

namespace GHelper
{
    public partial class Keyboard : RForm
    {

        Dictionary<string, string> customActions = new Dictionary<string, string>
        {
          {"","--------------" },
          {"mute", "Volume Mute"},
          {"screenshot", "Screenshot"},
          {"play", "Play/Pause"},
          {"aura", "Aura"},
          {"ghelper", "Open GHelper"},
          {"custom", "Custom"}
        };

        private void SetKeyCombo(ComboBox combo, TextBox txbox, string name)
        {
            if (name == "m4")
                customActions[""] = "Performance";

            if (name == "fnf4")
            {
                customActions[""] = "Aura";
                customActions.Remove("aura");
            }

            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.DataSource = new BindingSource(customActions, null);
            combo.DisplayMember = "Value";
            combo.ValueMember = "Key";

            string action = Program.config.getConfigString(name);

            combo.SelectedValue = (action is not null) ? action : "";
            if (combo.SelectedValue is null) combo.SelectedValue = "";

            combo.SelectedValueChanged += delegate
            {
                if (combo.SelectedValue is not null)
                    Program.config.setConfig(name, combo.SelectedValue.ToString());
            };

            txbox.Text = Program.config.getConfigString(name + "_custom");
            txbox.TextChanged += delegate
            {
                Program.config.setConfig(name + "_custom", txbox.Text);
            };
        }

        public Keyboard()
        {
            InitializeComponent();
            InitTheme();

            SetKeyCombo(comboM3, textM3, "m3");
            SetKeyCombo(comboM4, textM4, "m4");
            SetKeyCombo(comboFNF4, textFNF4, "fnf4");

            Shown += Keyboard_Shown;
        }


        private void Keyboard_Shown(object? sender, EventArgs e)
        {
            Top = Program.settingsForm.Top;
            Left = Program.settingsForm.Left - Width - 5;
        }
    }
}
