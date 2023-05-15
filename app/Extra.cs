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
            groupLight.Text = " " + Properties.Strings.LaptopBacklight;
            groupOther.Text = Properties.Strings.Other;

            checkAwake.Text = Properties.Strings.Awake;
            checkSleep.Text = Properties.Strings.Sleep;
            checkBoot.Text = Properties.Strings.Boot;
            checkShutdown.Text = Properties.Strings.Shutdown;

            labelSpeed.Text = Properties.Strings.AnimationSpeed;
            labelBrightness.Text = Properties.Strings.Brightness;

            checkKeyboardAuto.Text = Properties.Strings.KeyboardAuto;
            checkNoOverdrive.Text = Properties.Strings.DisableOverdrive;
            checkAutoUpdate.Text = Properties.Strings.EnableAutoUpdate;
            checkTopmost.Text = Properties.Strings.WindowTop;
            checkUSBC.Text = Properties.Strings.OptimizedUSBC;

            labelBacklight.Text = Properties.Strings.Keyboard;
            labelBacklightBar.Text = Properties.Strings.Lightbar;
            labelBacklightLid.Text = Properties.Strings.Lid;
            labelBacklightLogo.Text = Properties.Strings.Logo;

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

            // Keyboard
            checkAwake.Checked = !(Program.config.getConfig("keyboard_awake") == 0);
            checkBoot.Checked = !(Program.config.getConfig("keyboard_boot") == 0);
            checkSleep.Checked = !(Program.config.getConfig("keyboard_sleep") == 0);
            checkShutdown.Checked = !(Program.config.getConfig("keyboard_shutdown") == 0);

            // Lightbar
            checkAwakeBar.Checked = !(Program.config.getConfig("keyboard_awake_bar") == 0);
            checkBootBar.Checked = !(Program.config.getConfig("keyboard_boot_bar") == 0);
            checkSleepBar.Checked = !(Program.config.getConfig("keyboard_sleep_bar") == 0);
            checkShutdownBar.Checked = !(Program.config.getConfig("keyboard_shutdown_bar") == 0);

            // Lid
            checkAwakeLid.Checked = !(Program.config.getConfig("keyboard_awake_lid") == 0);
            checkBootLid.Checked = !(Program.config.getConfig("keyboard_boot_lid") == 0);
            checkSleepLid.Checked = !(Program.config.getConfig("keyboard_sleep_lid") == 0);
            checkShutdownLid.Checked = !(Program.config.getConfig("keyboard_shutdown_lid") == 0);

            // Logo
            checkAwakeLogo.Checked = !(Program.config.getConfig("keyboard_awake_logo") == 0);
            checkBootLogo.Checked = !(Program.config.getConfig("keyboard_boot_logo") == 0);
            checkSleepLogo.Checked = !(Program.config.getConfig("keyboard_sleep_logo") == 0);
            checkShutdownLogo.Checked = !(Program.config.getConfig("keyboard_shutdown_logo") == 0);

            checkAwake.CheckedChanged += CheckPower_CheckedChanged;
            checkBoot.CheckedChanged += CheckPower_CheckedChanged;
            checkSleep.CheckedChanged += CheckPower_CheckedChanged;
            checkShutdown.CheckedChanged += CheckPower_CheckedChanged;

            checkAwakeBar.CheckedChanged += CheckPower_CheckedChanged;
            checkBootBar.CheckedChanged += CheckPower_CheckedChanged;
            checkSleepBar.CheckedChanged += CheckPower_CheckedChanged;
            checkShutdownBar.CheckedChanged += CheckPower_CheckedChanged;

            checkAwakeLid.CheckedChanged += CheckPower_CheckedChanged;
            checkBootLid.CheckedChanged += CheckPower_CheckedChanged;
            checkSleepLid.CheckedChanged += CheckPower_CheckedChanged;
            checkShutdownLid.CheckedChanged += CheckPower_CheckedChanged;

            checkAwakeLogo.CheckedChanged += CheckPower_CheckedChanged;
            checkBootLogo.CheckedChanged += CheckPower_CheckedChanged;
            checkSleepLogo.CheckedChanged += CheckPower_CheckedChanged;
            checkShutdownLogo.CheckedChanged += CheckPower_CheckedChanged;

            if (!Program.config.ContainsModel("Strix"))
            {
                labelBacklightBar.Visible = false;
                checkAwakeBar.Visible = false;
                checkBootBar.Visible = false;
                checkSleepBar.Visible = false;
                checkShutdownBar.Visible = false;

                if (!Program.config.ContainsModel("Z13"))
                {
                    labelBacklightLid.Visible = false;
                    checkAwakeLid.Visible = false;
                    checkBootLid.Visible = false;
                    checkSleepLid.Visible = false;
                    checkShutdownLid.Visible = false;

                    labelBacklightLogo.Visible = false;
                    checkAwakeLogo.Visible = false;
                    checkBootLogo.Visible = false;
                    checkSleepLogo.Visible = false;
                    checkShutdownLogo.Visible = false;
                }
            }

            checkTopmost.Checked = (Program.config.getConfig("topmost") == 1);
            checkTopmost.CheckedChanged += CheckTopmost_CheckedChanged; ;

            checkKeyboardAuto.Checked = (Program.config.getConfig("keyboard_auto") == 1);
            checkKeyboardAuto.CheckedChanged += CheckKeyboardAuto_CheckedChanged;

            checkNoOverdrive.Checked = (Program.config.getConfig("no_overdrive") == 1);
            checkNoOverdrive.CheckedChanged += CheckNoOverdrive_CheckedChanged;
            
            checkAutoUpdate.Checked = (Program.config.getConfig("auto_update") == 1);
            checkAutoUpdate.CheckedChanged += CheckAutoUpdate_CheckedChanged;

            checkUSBC.Checked = (Program.config.getConfig("optimized_usbc") == 1);
            checkUSBC.CheckedChanged += CheckUSBC_CheckedChanged;

            int kb_brightness = Program.config.getConfig("keyboard_brightness");
            trackBrightness.Value = (kb_brightness >= 0 && kb_brightness <= 3) ? kb_brightness : 3;

            pictureHelp.Click += PictureHelp_Click;
            trackBrightness.Scroll += TrackBrightness_Scroll;

            panelXMG.Visible = (Program.wmi.DeviceGet(ASUSWmi.GPUXGConnected) == 1);
            checkXMG.Checked = !(Program.config.getConfig("xmg_light") == 0);
            checkXMG.CheckedChanged += CheckXMG_CheckedChanged;

            int kb_timeout = Program.config.getConfig("keyboard_light_tiomeout");
            numericBacklightTime.Value = (kb_timeout >= 0) ? kb_timeout : 60;

            numericBacklightTime.ValueChanged += NumericBacklightTime_ValueChanged;

        }

        private void NumericBacklightTime_ValueChanged(object? sender, EventArgs e)
        {
            Program.RunAsAdmin("extra");
            Program.config.setConfig("keyboard_light_tiomeout", (int)numericBacklightTime.Value);
            Aura.SetBacklightOffDelay((int)numericBacklightTime.Value);
        }

        private void CheckXMG_CheckedChanged(object? sender, EventArgs e)
        {
            Program.config.setConfig("xmg_light", (checkXMG.Checked ? 1 : 0));
            Aura.ApplyXGMLight(checkXMG.Checked);
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
        
        private void CheckAutoUpdate_CheckedChanged(object? sender, EventArgs e)
        {
            Program.config.setConfig("auto_update", (checkAutoUpdate.Checked ? 1 : 0));
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

            Program.config.setConfig("keyboard_awake_bar", (checkAwakeBar.Checked ? 1 : 0));
            Program.config.setConfig("keyboard_boot_bar", (checkBootBar.Checked ? 1 : 0));
            Program.config.setConfig("keyboard_sleep_bar", (checkSleepBar.Checked ? 1 : 0));
            Program.config.setConfig("keyboard_shutdown_bar", (checkShutdownBar.Checked ? 1 : 0));

            Program.config.setConfig("keyboard_awake_lid", (checkAwakeLid.Checked ? 1 : 0));
            Program.config.setConfig("keyboard_boot_lid", (checkBootLid.Checked ? 1 : 0));
            Program.config.setConfig("keyboard_sleep_lid", (checkSleepLid.Checked ? 1 : 0));
            Program.config.setConfig("keyboard_shutdown_lid", (checkShutdownLid.Checked ? 1 : 0));

            Program.config.setConfig("keyboard_awake_logo", (checkAwakeLogo.Checked ? 1 : 0));
            Program.config.setConfig("keyboard_boot_logo", (checkBootLogo.Checked ? 1 : 0));
            Program.config.setConfig("keyboard_sleep_logo", (checkSleepLogo.Checked ? 1 : 0));
            Program.config.setConfig("keyboard_shutdown_logo", (checkShutdownLogo.Checked ? 1 : 0));

            List<AuraDev19b6> flags = new List<AuraDev19b6>();

            if (checkAwake.Checked) flags.Add(AuraDev19b6.AwakeKeyb);
            if (checkBoot.Checked) flags.Add(AuraDev19b6.BootKeyb);
            if (checkSleep.Checked) flags.Add(AuraDev19b6.SleepKeyb);
            if (checkShutdown.Checked) flags.Add(AuraDev19b6.ShutdownKeyb);

            if (checkAwakeBar.Checked) flags.Add(AuraDev19b6.AwakeBar);
            if (checkBootBar.Checked) flags.Add(AuraDev19b6.BootBar);
            if (checkSleepBar.Checked) flags.Add(AuraDev19b6.SleepBar);
            if (checkShutdownBar.Checked) flags.Add(AuraDev19b6.ShutdownBar);

            if (checkAwakeLid.Checked) flags.Add(AuraDev19b6.AwakeLid);
            if (checkBootLid.Checked) flags.Add(AuraDev19b6.BootLid);
            if (checkSleepLid.Checked) flags.Add(AuraDev19b6.SleepLid);
            if (checkShutdownLid.Checked) flags.Add(AuraDev19b6.ShutdownLid);

            if (checkAwakeLogo.Checked) flags.Add(AuraDev19b6.AwakeLogo);
            if (checkBootLogo.Checked) flags.Add(AuraDev19b6.BootLogo);
            if (checkSleepLogo.Checked) flags.Add(AuraDev19b6.SleepLogo);
            if (checkShutdownLogo.Checked) flags.Add(AuraDev19b6.ShutdownLogo);

            Aura.ApplyAuraPower(flags);

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
