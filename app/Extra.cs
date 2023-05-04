using CustomControls;
using System.Diagnostics;

namespace GHelper
{
    public partial class Extra : RForm
    {

        Dictionary<string, string> customActions = new Dictionary<string, string>
        {
          {"","--------------" },
          {"mute", Properties.Strings.VolumeMute},
          {"screenshot", Properties.Strings.PrintScreen},
          {"play", Properties.Strings.PlayPause},
          {"aura", Properties.Strings.ToggleAura},
          {"performance", Properties.Strings.PerformanceMode},
          {"screen", Properties.Strings.ToggleScreen},
          {"miniled", Properties.Strings.ToggleMiniled},
          {"custom", Properties.Strings.Custom}
        };

        private void SetKeyCombo(ComboBox combo, TextBox txbox, string name)
        {
            if (name == "m4")
                customActions[""] = Properties.Strings.OpenGHelper;

            if (name == "fnf4")
            {
                customActions[""] = Properties.Strings.ToggleAura;
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

        public Extra()
        {
            InitializeComponent();

            groupBindings.Text = Properties.Strings.KeyBindings;
            groupLight.Text = Properties.Strings.KeyboardBacklight;
            groupOther.Text = Properties.Strings.Other;

            checkAwake.Text = Properties.Strings.Awake;
            checkSleep.Text = Properties.Strings.Sleep;
            checkBoot.Text = Properties.Strings.Boot;
            checkShutdown.Text = Properties.Strings.Shutdown;

            labelSpeed.Text = Properties.Strings.AnimationSpeed;
            labelBrightness.Text = Properties.Strings.Brightness;

            checkKeyboardAuto.Text = Properties.Strings.KeyboardAuto;
            checkNoOverdrive.Text = Properties.Strings.DisableOverdrive;
            checkTopmost.Text = Properties.Strings.WindowTop;
            checkUSBC.Text = Properties.Strings.OptimizedUSBC;

            Text = Properties.Strings.ExtraSettings;

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

            checkUSBC.Checked = (Program.config.getConfig("optimized_usbc") == 1);
            checkUSBC.CheckedChanged += CheckUSBC_CheckedChanged;

            int kb_brightness = Program.config.getConfig("keyboard_brightness");
            trackBrightness.Value = (kb_brightness >= 0 && kb_brightness <= 3) ? kb_brightness : 3;

            pictureHelp.Click += PictureHelp_Click;
            trackBrightness.Scroll += TrackBrightness_Scroll;

        }

        private void CheckUSBC_CheckedChanged(object? sender, EventArgs e)
        {
            Program.config.setConfig("optimized_usbc", (checkUSBC.Checked ? 1 : 0));
        }

        private void TrackBrightness_Scroll(object? sender, EventArgs e)
        {
            Program.config.setConfig("keyboard_brightness", trackBrightness.Value);
            Aura.ApplyBrightness(trackBrightness.Value);
        }

        private void PictureHelp_Click(object? sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/seerge/g-helper#custom-hotkey-actions") { UseShellExecute = true });
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
