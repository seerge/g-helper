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

            comboKeyboardSpeed.DropDownStyle = ComboBoxStyle.DropDownList;
            comboKeyboardSpeed.DataSource = new BindingSource(Aura.GetSpeeds(), null);
            comboKeyboardSpeed.DisplayMember = "Value";
            comboKeyboardSpeed.ValueMember = "Key";
            comboKeyboardSpeed.SelectedValue = Aura.Speed;
            comboKeyboardSpeed.SelectedValueChanged += ComboKeyboardSpeed_SelectedValueChanged;

            checkAwake.Checked = !(Program.config.getConfig("keyboard_awake") == 0);
            checkBoot.Checked = !(Program.config.getConfig("keyboard_boot") == 0);
            checkSleep.Checked = !(Program.config.getConfig("keyboard_sleep") == 0);
            checkShutdown.Checked = !(Program.config.getConfig("keyboard_shutdown") == 0);

            checkAwake.CheckedChanged += CheckPower_CheckedChanged;
            checkBoot.CheckedChanged += CheckPower_CheckedChanged;
            checkSleep.CheckedChanged += CheckPower_CheckedChanged;
            checkShutdown.CheckedChanged += CheckPower_CheckedChanged;

            checkTopmost.Checked = (Program.config.getConfig("topmost") == 1);
            checkTopmost.CheckedChanged += CheckTopmost_CheckedChanged; ;

            checkKeyboardAuto.Checked = (Program.config.getConfig("keyboard_auto") == 1);
            checkKeyboardAuto.CheckedChanged += CheckKeyboardAuto_CheckedChanged;

            checkNoOverdrive.Checked = (Program.config.getConfig("no_overdrive") == 1);
            checkNoOverdrive.CheckedChanged += CheckNoOverdrive_CheckedChanged;

        }

        private void CheckNoOverdrive_CheckedChanged(object? sender, EventArgs e)
        {
            Program.config.setConfig("no_overdrive", (checkNoOverdrive.Checked ? 1 : 0));
            Program.settingsForm.AutoScreen(true);
        }

        private void CheckKeyboardAuto_CheckedChanged(object? sender, EventArgs e)
        {
            Program.config.setConfig("keyboard_auto", (checkKeyboardAuto.Checked ? 1 : 0));
        }

        private void CheckTopmost_CheckedChanged(object? sender, EventArgs e)
        {
            Program.config.setConfig("topmost", (checkTopmost.Checked ? 1 : 0));
            Program.settingsForm.TopMost = checkTopmost.Checked;
        }

        private void CheckPower_CheckedChanged(object? sender, EventArgs e)
        {
            Program.config.setConfig("keyboard_awake", (checkAwake.Checked ? 1 : 0));
            Program.config.setConfig("keyboard_boot", (checkBoot.Checked ? 1 : 0));
            Program.config.setConfig("keyboard_sleep", (checkSleep.Checked ? 1 : 0));
            Program.config.setConfig("keyboard_shutdown", (checkShutdown.Checked ? 1 : 0));

            Aura.ApplyAuraPower(checkAwake.Checked, checkBoot.Checked, checkSleep.Checked, checkShutdown.Checked);
        }

        private void ComboKeyboardSpeed_SelectedValueChanged(object? sender, EventArgs e)
        {
            Program.config.setConfig("aura_speed", (int)comboKeyboardSpeed.SelectedValue);
            Program.settingsForm.SetAura();
        }


        private void Keyboard_Shown(object? sender, EventArgs e)
        {
            Top = Program.settingsForm.Top;
            Left = Program.settingsForm.Left - Width - 5;
        }
    }
}
