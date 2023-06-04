using CustomControls;
using GHelper.Gpu;
using Microsoft.VisualBasic.Devices;
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
          {"fnlock", Properties.Strings.ToggleFnLock},
          {"custom", Properties.Strings.Custom}
        };

        private void SetKeyCombo(ComboBox combo, TextBox txbox, string name)
        {

            switch (name)
            {
                case "m1":
                    customActions[""] = Properties.Strings.VolumeDown;
                    break;
                case "m2":
                    customActions[""] = Properties.Strings.VolumeUp;
                    break;
                case "m3":
                    customActions[""] = Properties.Strings.MuteMic;
                    break;
                case "m4":
                    customActions[""] = Properties.Strings.OpenGHelper;
                    break;
                case "fnf4":
                    customActions[""] = Properties.Strings.ToggleAura;
                    customActions.Remove("aura");
                    break;
                case "fnc":
                    customActions[""] = Properties.Strings.ToggleFnLock;
                    customActions.Remove("fnlock");
                    break;
            }

            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.DataSource = new BindingSource(customActions, null);
            combo.DisplayMember = "Value";
            combo.ValueMember = "Key";

            string action = AppConfig.getConfigString(name);

            combo.SelectedValue = (action is not null) ? action : "";
            if (combo.SelectedValue is null) combo.SelectedValue = "";

            combo.SelectedValueChanged += delegate
            {
                if (combo.SelectedValue is not null)
                    AppConfig.setConfig(name, combo.SelectedValue.ToString());

                if (name == "m1" || name == "m2")
                    Program.inputDispatcher.RegisterKeys();

            };

            txbox.Text = AppConfig.getConfigString(name + "_custom");
            txbox.TextChanged += delegate
            {
                AppConfig.setConfig(name + "_custom", txbox.Text);
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

            labelBacklightTimeout.Text = Properties.Strings.BacklightTimeout;
            labelBacklightTimeoutPlugged.Text = Properties.Strings.BacklightTimeoutPlugged;

            checkNoOverdrive.Text = Properties.Strings.DisableOverdrive;
            checkTopmost.Text = Properties.Strings.WindowTop;
            checkUSBC.Text = Properties.Strings.OptimizedUSBC;
            checkAutoApplyWindowsPowerMode.Text = Properties.Strings.ApplyWindowsPowerPlan;
            checkFnLock.Text = Properties.Strings.FnLock;

            labelBacklight.Text = Properties.Strings.Keyboard;
            labelBacklightBar.Text = Properties.Strings.Lightbar;
            labelBacklightLid.Text = Properties.Strings.Lid;
            labelBacklightLogo.Text = Properties.Strings.Logo;

            checkGpuApps.Text = Properties.Strings.KillGpuApps;

            Text = Properties.Strings.ExtraSettings;

            InitTheme();

            SetKeyCombo(comboM1, textM1, "m1");
            SetKeyCombo(comboM2, textM2, "m2");
            SetKeyCombo(comboM3, textM3, "m3");
            SetKeyCombo(comboM4, textM4, "m4");
            SetKeyCombo(comboFNF4, textFNF4, "fnf4");
            SetKeyCombo(comboFNC, textFNC, "fnc");

            Shown += Keyboard_Shown;

            comboKeyboardSpeed.DropDownStyle = ComboBoxStyle.DropDownList;
            comboKeyboardSpeed.DataSource = new BindingSource(AsusUSB.GetSpeeds(), null);
            comboKeyboardSpeed.DisplayMember = "Value";
            comboKeyboardSpeed.ValueMember = "Key";
            comboKeyboardSpeed.SelectedValue = AsusUSB.Speed;
            comboKeyboardSpeed.SelectedValueChanged += ComboKeyboardSpeed_SelectedValueChanged;

            // Keyboard
            checkAwake.Checked = !(AppConfig.getConfig("keyboard_awake") == 0);
            checkBoot.Checked = !(AppConfig.getConfig("keyboard_boot") == 0);
            checkSleep.Checked = !(AppConfig.getConfig("keyboard_sleep") == 0);
            checkShutdown.Checked = !(AppConfig.getConfig("keyboard_shutdown") == 0);

            // Lightbar
            checkAwakeBar.Checked = !(AppConfig.getConfig("keyboard_awake_bar") == 0);
            checkBootBar.Checked = !(AppConfig.getConfig("keyboard_boot_bar") == 0);
            checkSleepBar.Checked = !(AppConfig.getConfig("keyboard_sleep_bar") == 0);
            checkShutdownBar.Checked = !(AppConfig.getConfig("keyboard_shutdown_bar") == 0);

            // Lid
            checkAwakeLid.Checked = !(AppConfig.getConfig("keyboard_awake_lid") == 0);
            checkBootLid.Checked = !(AppConfig.getConfig("keyboard_boot_lid") == 0);
            checkSleepLid.Checked = !(AppConfig.getConfig("keyboard_sleep_lid") == 0);
            checkShutdownLid.Checked = !(AppConfig.getConfig("keyboard_shutdown_lid") == 0);

            // Logo
            checkAwakeLogo.Checked = !(AppConfig.getConfig("keyboard_awake_logo") == 0);
            checkBootLogo.Checked = !(AppConfig.getConfig("keyboard_boot_logo") == 0);
            checkSleepLogo.Checked = !(AppConfig.getConfig("keyboard_sleep_logo") == 0);
            checkShutdownLogo.Checked = !(AppConfig.getConfig("keyboard_shutdown_logo") == 0);

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

            if (!AppConfig.ContainsModel("Strix"))
            {
                labelBacklightBar.Visible = false;
                checkAwakeBar.Visible = false;
                checkBootBar.Visible = false;
                checkSleepBar.Visible = false;
                checkShutdownBar.Visible = false;

                if (!AppConfig.ContainsModel("Z13"))
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

            checkTopmost.Checked = (AppConfig.getConfig("topmost") == 1);
            checkTopmost.CheckedChanged += CheckTopmost_CheckedChanged; ;

            checkNoOverdrive.Checked = (AppConfig.getConfig("no_overdrive") == 1);
            checkNoOverdrive.CheckedChanged += CheckNoOverdrive_CheckedChanged;

            checkUSBC.Checked = (AppConfig.getConfig("optimized_usbc") == 1);
            checkUSBC.CheckedChanged += CheckUSBC_CheckedChanged;

            checkAutoApplyWindowsPowerMode.Checked = (AppConfig.getConfig("auto_apply_power_plan") != 0);
            checkAutoApplyWindowsPowerMode.CheckedChanged += checkAutoApplyWindowsPowerMode_CheckedChanged;

            trackBrightness.Value = InputDispatcher.GetBacklight();
            trackBrightness.Scroll += TrackBrightness_Scroll;

            panelXMG.Visible = (Program.acpi.DeviceGet(AsusACPI.GPUXGConnected) == 1);
            checkXMG.Checked = !(AppConfig.getConfig("xmg_light") == 0);
            checkXMG.CheckedChanged += CheckXMG_CheckedChanged;

            numericBacklightTime.Value = AppConfig.getConfig("keyboard_timeout", 60);
            numericBacklightPluggedTime.Value = AppConfig.getConfig("keyboard_ac_timeout", 0);

            numericBacklightTime.ValueChanged += NumericBacklightTime_ValueChanged;
            numericBacklightPluggedTime.ValueChanged += NumericBacklightTime_ValueChanged;

            checkGpuApps.Checked = AppConfig.isConfig("kill_gpu_apps");
            checkGpuApps.CheckedChanged += CheckGpuApps_CheckedChanged;

            checkFnLock.Checked = AppConfig.isConfig("fn_lock");
            checkFnLock.CheckedChanged += CheckFnLock_CheckedChanged; ;

            pictureHelp.Click += PictureHelp_Click;

            InitVariBright();
        }

        private void InitVariBright()
        {
            try
            {
                using (var amdControl = new AmdGpuControl())
                {
                    int variBrightSupported = 0, VariBrightEnabled;
                    if (amdControl.GetVariBright(out variBrightSupported, out VariBrightEnabled))
                    {
                        Logger.WriteLine("Varibright: " + variBrightSupported + "," + VariBrightEnabled);
                        checkVariBright.Checked = (VariBrightEnabled == 3);
                    }

                    checkVariBright.Visible = (variBrightSupported > 0);
                    checkVariBright.CheckedChanged += CheckVariBright_CheckedChanged;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                checkVariBright.Visible = false;
            }


        }

        private void CheckVariBright_CheckedChanged(object? sender, EventArgs e)
        {
            using (var amdControl = new AmdGpuControl())
            {
                amdControl.SetVariBright(checkVariBright.Checked ? 1 : 0);
                ProcessHelper.KillByName("RadeonSoftware");
            }
        }

        private void CheckFnLock_CheckedChanged(object? sender, EventArgs e)
        {
            int fnLock = checkFnLock.Checked ? 1 : 0;
            AppConfig.setConfig("fn_lock", fnLock);
            Program.acpi.DeviceSet(AsusACPI.FnLock, (fnLock == 1) ? 0 : 1, "FnLock");

            Program.inputDispatcher.RegisterKeys();
        }

        private void CheckGpuApps_CheckedChanged(object? sender, EventArgs e)
        {
            AppConfig.setConfig("kill_gpu_apps", (checkGpuApps.Checked ? 1 : 0));
        }

        private void NumericBacklightTime_ValueChanged(object? sender, EventArgs e)
        {
            AppConfig.setConfig("keyboard_timeout", (int)numericBacklightTime.Value);
            AppConfig.setConfig("keyboard_ac_timeout", (int)numericBacklightPluggedTime.Value);
            Program.inputDispatcher.InitBacklightTimer();
        }

        private void CheckXMG_CheckedChanged(object? sender, EventArgs e)
        {
            AppConfig.setConfig("xmg_light", (checkXMG.Checked ? 1 : 0));
            AsusUSB.ApplyXGMLight(checkXMG.Checked);
        }

        private void CheckUSBC_CheckedChanged(object? sender, EventArgs e)
        {
            AppConfig.setConfig("optimized_usbc", (checkUSBC.Checked ? 1 : 0));
        }

        private void TrackBrightness_Scroll(object? sender, EventArgs e)
        {
            AppConfig.setConfig("keyboard_brightness", trackBrightness.Value);
            AppConfig.setConfig("keyboard_brightness_ac", trackBrightness.Value);
            AsusUSB.ApplyBrightness(trackBrightness.Value, "Slider");
        }

        private void PictureHelp_Click(object? sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/seerge/g-helper#custom-hotkey-actions") { UseShellExecute = true });
        }

        private void CheckNoOverdrive_CheckedChanged(object? sender, EventArgs e)
        {
            AppConfig.setConfig("no_overdrive", (checkNoOverdrive.Checked ? 1 : 0));
            Program.settingsForm.AutoScreen(true);
        }


        private void CheckTopmost_CheckedChanged(object? sender, EventArgs e)
        {
            AppConfig.setConfig("topmost", (checkTopmost.Checked ? 1 : 0));
            Program.settingsForm.TopMost = checkTopmost.Checked;
        }

        private void CheckPower_CheckedChanged(object? sender, EventArgs e)
        {
            AppConfig.setConfig("keyboard_awake", (checkAwake.Checked ? 1 : 0));
            AppConfig.setConfig("keyboard_boot", (checkBoot.Checked ? 1 : 0));
            AppConfig.setConfig("keyboard_sleep", (checkSleep.Checked ? 1 : 0));
            AppConfig.setConfig("keyboard_shutdown", (checkShutdown.Checked ? 1 : 0));

            AppConfig.setConfig("keyboard_awake_bar", (checkAwakeBar.Checked ? 1 : 0));
            AppConfig.setConfig("keyboard_boot_bar", (checkBootBar.Checked ? 1 : 0));
            AppConfig.setConfig("keyboard_sleep_bar", (checkSleepBar.Checked ? 1 : 0));
            AppConfig.setConfig("keyboard_shutdown_bar", (checkShutdownBar.Checked ? 1 : 0));

            AppConfig.setConfig("keyboard_awake_lid", (checkAwakeLid.Checked ? 1 : 0));
            AppConfig.setConfig("keyboard_boot_lid", (checkBootLid.Checked ? 1 : 0));
            AppConfig.setConfig("keyboard_sleep_lid", (checkSleepLid.Checked ? 1 : 0));
            AppConfig.setConfig("keyboard_shutdown_lid", (checkShutdownLid.Checked ? 1 : 0));

            AppConfig.setConfig("keyboard_awake_logo", (checkAwakeLogo.Checked ? 1 : 0));
            AppConfig.setConfig("keyboard_boot_logo", (checkBootLogo.Checked ? 1 : 0));
            AppConfig.setConfig("keyboard_sleep_logo", (checkSleepLogo.Checked ? 1 : 0));
            AppConfig.setConfig("keyboard_shutdown_logo", (checkShutdownLogo.Checked ? 1 : 0));

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

            AsusUSB.ApplyAuraPower(flags);

        }

        private void ComboKeyboardSpeed_SelectedValueChanged(object? sender, EventArgs e)
        {
            AppConfig.setConfig("aura_speed", (int)comboKeyboardSpeed.SelectedValue);
            Program.settingsForm.SetAura();
        }


        private void Keyboard_Shown(object? sender, EventArgs e)
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

        private void checkAutoApplyWindowsPowerMode_CheckedChanged(object? sender, EventArgs e)
        {
            AppConfig.setConfig("auto_apply_power_plan", checkAutoApplyWindowsPowerMode.Checked ? 1 : 0);
        }
    }
}
