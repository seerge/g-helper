using GHelper.Display;
using GHelper.Gpu.AMD;
using GHelper.Helpers;
using GHelper.Input;
using GHelper.Mode;
using GHelper.UI;
using System.Diagnostics;

namespace GHelper
{
    public partial class Extra : RForm
    {

        ScreenControl screenControl = new ScreenControl();
        ClamshellModeControl clamshellControl = new ClamshellModeControl();

        const string EMPTY = "--------------";

        Dictionary<string, string> customActions = new Dictionary<string, string>
        {
          {"", EMPTY},
          {"mute", Properties.Strings.VolumeMute},
          {"screenshot", Properties.Strings.PrintScreen},
          {"play", Properties.Strings.PlayPause},
          {"aura", Properties.Strings.ToggleAura},
          {"performance", Properties.Strings.PerformanceMode},
          {"screen", Properties.Strings.ToggleScreen},
          {"miniled", Properties.Strings.ToggleMiniled},
          {"fnlock", Properties.Strings.ToggleFnLock},
          {"brightness_down", Properties.Strings.BrightnessDown},
          {"brightness_up", Properties.Strings.BrightnessUp},
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
                case "fne":
                    customActions[""] = "Calculator";
                    customActions["ghelper"] = Properties.Strings.OpenGHelper;
                    break;
                case "paddle":
                    customActions[""] = EMPTY;
                    break;
                case "cc":
                    customActions[""] = EMPTY;
                    break;
            }

            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.DataSource = new BindingSource(customActions, null);
            combo.DisplayMember = "Value";
            combo.ValueMember = "Key";

            string action = AppConfig.GetString(name);

            combo.SelectedValue = (action is not null) ? action : "";
            if (combo.SelectedValue is null) combo.SelectedValue = "";

            combo.SelectedValueChanged += delegate
            {
                if (combo.SelectedValue is not null)
                    AppConfig.Set(name, combo.SelectedValue.ToString());

                if (name == "m1" || name == "m2")
                    Program.inputDispatcher.RegisterKeys();

            };

            txbox.Text = AppConfig.GetString(name + "_custom");
            txbox.TextChanged += delegate
            {
                AppConfig.Set(name + "_custom", txbox.Text);
            };
        }

        public Extra()
        {
            InitializeComponent();

            labelBindings.Text = Properties.Strings.KeyBindings;
            labelBacklightTitle.Text = Properties.Strings.LaptopBacklight;
            labelSettings.Text = Properties.Strings.Other;

            checkAwakeKeyboard.Text = Properties.Strings.Awake;
            checkSleepKeyboard.Text = Properties.Strings.Sleep;
            checkBootKeyboard.Text = Properties.Strings.Boot;
            checkShutdownKeyboard.Text = Properties.Strings.Shutdown;

            labelSpeed.Text = Properties.Strings.AnimationSpeed;
            //labelBrightness.Text = Properties.Strings.Brightness;

            labelBacklightTimeout.Text = Properties.Strings.BacklightTimeout;
            //labelBacklightTimeoutPlugged.Text = Properties.Strings.BacklightTimeoutPlugged;

            checkNoOverdrive.Text = Properties.Strings.DisableOverdrive;
            checkTopmost.Text = Properties.Strings.WindowTop;
            checkUSBC.Text = Properties.Strings.OptimizedUSBC;
            checkAutoToggleClamshellMode.Text = Properties.Strings.ToggleClamshellMode;

            groupBoxBacklightKeyboard.Text = Properties.Strings.Keyboard;
            groupBoxBacklightBar.Text = Properties.Strings.Lightbar;
            groupBoxBacklightLid.Text = Properties.Strings.Lid;
            groupBoxBacklightLogo.Text = Properties.Strings.Logo;

            checkGpuApps.Text = Properties.Strings.KillGpuApps;
            labelHibernateAfter.Text = Properties.Strings.HibernateAfter;

            Text = Properties.Strings.ExtraSettings;

            if (AppConfig.IsDUO())
            {
                customActions.Add("screenpad_down", Properties.Strings.ScreenPadDown);
                customActions.Add("screenpad_up", Properties.Strings.ScreenPadUp);
            }

            if (AppConfig.NoMKeys())
            {
                labelM1.Text = "FN+F2";
                labelM2.Text = "FN+F3";
                labelM3.Text = "FN+F4";
                labelM4.Visible = comboM4.Visible = textM4.Visible = false;
                labelFNF4.Visible = comboFNF4.Visible = textFNF4.Visible = false;
            }

            if (AppConfig.NoAura())
            {
                labelFNF4.Visible = comboFNF4.Visible = textFNF4.Visible = false;
            }

            if (!AppConfig.IsTUF())
            {
                labelFNE.Visible = comboFNE.Visible = textFNE.Visible = false;
            }

            if (Program.acpi.DeviceGet(AsusACPI.GPUEco) < 0)
            {
                checkGpuApps.Visible = false;
                checkUSBC.Visible = false;
            }

            // Change text and hide irrelevant options on the ROG Ally,
            // which is a bit of a special case piece of hardware.
            if (AppConfig.IsAlly())
            {
                labelM1.Visible = comboM1.Visible = textM1.Visible = false;
                labelM2.Visible = comboM2.Visible = textM2.Visible = false;

                // Re-label M3 and M4 and FNF4 to match the front labels.
                labelM3.Text = "Ctrl Center";
                labelM4.Text = "ROG";
                labelFNF4.Text = "Back Paddles";

                // Hide all of the FN options, as the Ally has no special keyboard FN key.
                labelFNC.Visible = false;
                comboFNC.Visible = false;
                textFNC.Visible = false;

                SetKeyCombo(comboM3, textM3, "cc");
                SetKeyCombo(comboM4, textM4, "m4");
                SetKeyCombo(comboFNF4, textFNF4, "paddle");

            }
            else
            {
                SetKeyCombo(comboM1, textM1, "m1");
                SetKeyCombo(comboM2, textM2, "m2");

                SetKeyCombo(comboM3, textM3, "m3");
                SetKeyCombo(comboM4, textM4, "m4");
                SetKeyCombo(comboFNF4, textFNF4, "fnf4");

                SetKeyCombo(comboFNC, textFNC, "fnc");
                SetKeyCombo(comboFNE, textFNE, "fne");
            }

            if (AppConfig.IsStrix())
            {
                labelM4.Text = "M5/ROG";
            }


            InitTheme();
            Shown += Keyboard_Shown;

            comboKeyboardSpeed.DropDownStyle = ComboBoxStyle.DropDownList;
            comboKeyboardSpeed.DataSource = new BindingSource(AsusUSB.GetSpeeds(), null);
            comboKeyboardSpeed.DisplayMember = "Value";
            comboKeyboardSpeed.ValueMember = "Key";
            comboKeyboardSpeed.SelectedValue = AsusUSB.Speed;
            comboKeyboardSpeed.SelectedValueChanged += ComboKeyboardSpeed_SelectedValueChanged;

            // Keyboard
            checkAwakeKeyboard.Checked = AppConfig.IsNotFalse("keyboard_awake");
            checkBootKeyboard.Checked = AppConfig.IsNotFalse("keyboard_boot");
            checkSleepKeyboard.Checked = AppConfig.IsNotFalse("keyboard_sleep");
            checkShutdownKeyboard.Checked = AppConfig.IsNotFalse("keyboard_shutdown");

            // Lightbar
            checkAwakeBar.Checked = AppConfig.IsNotFalse("keyboard_awake_bar");
            checkBootBar.Checked = AppConfig.IsNotFalse("keyboard_boot_bar");
            checkSleepBar.Checked = AppConfig.IsNotFalse("keyboard_sleep_bar");
            checkShutdownBar.Checked = AppConfig.IsNotFalse("keyboard_shutdown_bar");

            // Lid
            checkAwakeLid.Checked = AppConfig.IsNotFalse("keyboard_awake_lid");
            checkBootLid.Checked = AppConfig.IsNotFalse("keyboard_boot_lid");
            checkSleepLid.Checked = AppConfig.IsNotFalse("keyboard_sleep_lid");
            checkShutdownLid.Checked = AppConfig.IsNotFalse("keyboard_shutdown_lid");

            // Logo
            checkAwakeLogo.Checked = AppConfig.IsNotFalse("keyboard_awake_logo");
            checkBootLogo.Checked = AppConfig.IsNotFalse("keyboard_boot_logo");
            checkSleepLogo.Checked = AppConfig.IsNotFalse("keyboard_sleep_logo");
            checkShutdownLogo.Checked = AppConfig.IsNotFalse("keyboard_shutdown_logo");

            checkAwakeKeyboard.CheckedChanged += CheckPower_CheckedChanged;
            checkBootKeyboard.CheckedChanged += CheckPower_CheckedChanged;
            checkSleepKeyboard.CheckedChanged += CheckPower_CheckedChanged;
            checkShutdownKeyboard.CheckedChanged += CheckPower_CheckedChanged;

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

            if (!AppConfig.IsStrix())
            {
                groupBoxBacklightBar.Visible = false;
                //labelBacklightBar.Visible = false;
                //checkAwakeBar.Visible = false;
                //checkBootBar.Visible = false;
                //checkSleepBar.Visible = false;
                //checkShutdownBar.Visible = false;

                if (!AppConfig.IsZ13())
                {
                    groupBoxBacklightLid.Visible = false;
                    //labelBacklightLid.Visible = false;
                    //checkAwakeLid.Visible = false;
                    //checkBootLid.Visible = false;
                    //checkSleepLid.Visible = false;
                    //checkShutdownLid.Visible = false;

                    groupBoxBacklightLogo.Visible = false;
                    //labelBacklightLogo.Visible = false;
                    //checkAwakeLogo.Visible = false;
                    //checkBootLogo.Visible = false;
                    //checkSleepLogo.Visible = false;
                    //checkShutdownLogo.Visible = false;

                    //labelBacklightKeyboard.Visible = false;
                }
            }

            //checkAutoToggleClamshellMode.Visible = clamshellControl.IsExternalDisplayConnected();
            checkAutoToggleClamshellMode.Checked = AppConfig.Is("toggle_clamshell_mode");
            checkAutoToggleClamshellMode.CheckedChanged += checkAutoToggleClamshellMode_CheckedChanged;

            checkTopmost.Checked = AppConfig.Is("topmost");
            checkTopmost.CheckedChanged += CheckTopmost_CheckedChanged; ;

            checkNoOverdrive.Checked = AppConfig.Is("no_overdrive");
            checkNoOverdrive.CheckedChanged += CheckNoOverdrive_CheckedChanged;

            checkUSBC.Checked = AppConfig.Is("optimized_usbc");
            checkUSBC.CheckedChanged += CheckUSBC_CheckedChanged;

            sliderBrightness.Value = InputDispatcher.GetBacklight();
            sliderBrightness.ValueChanged += SliderBrightness_ValueChanged;

            panelXMG.Visible = (Program.acpi.DeviceGet(AsusACPI.GPUXGConnected) == 1);
            checkXMG.Checked = !(AppConfig.Get("xmg_light") == 0);
            checkXMG.CheckedChanged += CheckXMG_CheckedChanged;

            numericBacklightTime.Value = AppConfig.Get("keyboard_timeout", 60);
            numericBacklightPluggedTime.Value = AppConfig.Get("keyboard_ac_timeout", 0);

            numericBacklightTime.ValueChanged += NumericBacklightTime_ValueChanged;
            numericBacklightPluggedTime.ValueChanged += NumericBacklightTime_ValueChanged;

            checkGpuApps.Checked = AppConfig.Is("kill_gpu_apps");
            checkGpuApps.CheckedChanged += CheckGpuApps_CheckedChanged;

            pictureHelp.Click += PictureHelp_Click;
            buttonServices.Click += ButtonServices_Click;

            pictureLog.Click += PictureLog_Click;

            checkGPUFix.Visible = AppConfig.IsGPUFixNeeded();
            checkGPUFix.Checked = AppConfig.IsGPUFix();
            checkGPUFix.CheckedChanged += CheckGPUFix_CheckedChanged;

            toolTip.SetToolTip(checkAutoToggleClamshellMode, "Disable sleep on lid close when plugged in and external monitor is connected");

            InitVariBright();
            InitServices();
            InitHibernate();
        }

        private void CheckGPUFix_CheckedChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("gpu_fix", (checkGPUFix.Checked ? 1 : 0));
        }

        private void InitHibernate()
        {
            try
            {
                int hibernate = PowerNative.GetHibernateAfter();
                if (hibernate < 0 || hibernate > numericHibernateAfter.Maximum) hibernate = 0;
                numericHibernateAfter.Value = hibernate;
                numericHibernateAfter.ValueChanged += NumericHibernateAfter_ValueChanged;

            }
            catch (Exception ex)
            {
                panelPower.Visible = false;
                Logger.WriteLine(ex.ToString());
            }

        }

        private void NumericHibernateAfter_ValueChanged(object? sender, EventArgs e)
        {
            PowerNative.SetHibernateAfter((int)numericHibernateAfter.Value);
        }

        private void PictureLog_Click(object? sender, EventArgs e)
        {
            new Process
            {
                StartInfo = new ProcessStartInfo(Logger.logFile)
                {
                    UseShellExecute = true
                }
            }.Start();
        }

        private void SliderBrightness_ValueChanged(object? sender, EventArgs e)
        {
            bool onBattery = SystemInformation.PowerStatus.PowerLineStatus != PowerLineStatus.Online;

            if (onBattery)
                AppConfig.Set("keyboard_brightness_ac", sliderBrightness.Value);
            else
                AppConfig.Set("keyboard_brightness", sliderBrightness.Value);

            AsusUSB.ApplyBrightness(sliderBrightness.Value, "Slider");
        }

        private void InitServices()
        {

            int servicesCount = OptimizationService.GetRunningCount();

            if (servicesCount > 0)
            {
                buttonServices.Text = Properties.Strings.Stop;
                labelServices.ForeColor = colorTurbo;
            }
            else
            {
                buttonServices.Text = Properties.Strings.Start;
                labelServices.ForeColor = colorStandard;
            }

            labelServices.Text = Properties.Strings.AsusServicesRunning + ":  " + servicesCount;
            buttonServices.Enabled = true;

        }

        public void ServiesToggle()
        {
            buttonServices.Enabled = false;

            if (OptimizationService.GetRunningCount() > 0)
            {
                labelServices.Text = Properties.Strings.StoppingServices + " ...";
                Task.Run(() =>
                {
                    OptimizationService.StopAsusServices();
                    BeginInvoke(delegate
                    {
                        InitServices();
                    });
                    Program.inputDispatcher.Init();
                });
            }
            else
            {
                labelServices.Text = Properties.Strings.StartingServices + " ...";
                Task.Run(() =>
                {
                    OptimizationService.StartAsusServices();
                    BeginInvoke(delegate
                    {
                        InitServices();
                    });
                });
            }
        }

        private void ButtonServices_Click(object? sender, EventArgs e)
        {
            if (ProcessHelper.IsUserAdministrator())
                ServiesToggle();
            else
                ProcessHelper.RunAsAdmin("services");
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

        private void CheckGpuApps_CheckedChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("kill_gpu_apps", (checkGpuApps.Checked ? 1 : 0));
        }

        private void NumericBacklightTime_ValueChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("keyboard_timeout", (int)numericBacklightTime.Value);
            AppConfig.Set("keyboard_ac_timeout", (int)numericBacklightPluggedTime.Value);
            Program.inputDispatcher.InitBacklightTimer();
        }

        private void CheckXMG_CheckedChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("xmg_light", (checkXMG.Checked ? 1 : 0));
            AsusUSB.ApplyXGMLight(checkXMG.Checked);
        }

        private void CheckUSBC_CheckedChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("optimized_usbc", (checkUSBC.Checked ? 1 : 0));
        }

        private void PictureHelp_Click(object? sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/seerge/g-helper#custom-hotkey-actions") { UseShellExecute = true });
        }

        private void CheckNoOverdrive_CheckedChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("no_overdrive", (checkNoOverdrive.Checked ? 1 : 0));
            screenControl.AutoScreen(true);
        }


        private void CheckTopmost_CheckedChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("topmost", (checkTopmost.Checked ? 1 : 0));
            Program.settingsForm.TopMost = checkTopmost.Checked;
        }

        private void CheckPower_CheckedChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("keyboard_awake", (checkAwakeKeyboard.Checked ? 1 : 0));
            AppConfig.Set("keyboard_boot", (checkBootKeyboard.Checked ? 1 : 0));
            AppConfig.Set("keyboard_sleep", (checkSleepKeyboard.Checked ? 1 : 0));
            AppConfig.Set("keyboard_shutdown", (checkShutdownKeyboard.Checked ? 1 : 0));

            AppConfig.Set("keyboard_awake_bar", (checkAwakeBar.Checked ? 1 : 0));
            AppConfig.Set("keyboard_boot_bar", (checkBootBar.Checked ? 1 : 0));
            AppConfig.Set("keyboard_sleep_bar", (checkSleepBar.Checked ? 1 : 0));
            AppConfig.Set("keyboard_shutdown_bar", (checkShutdownBar.Checked ? 1 : 0));

            AppConfig.Set("keyboard_awake_lid", (checkAwakeLid.Checked ? 1 : 0));
            AppConfig.Set("keyboard_boot_lid", (checkBootLid.Checked ? 1 : 0));
            AppConfig.Set("keyboard_sleep_lid", (checkSleepLid.Checked ? 1 : 0));
            AppConfig.Set("keyboard_shutdown_lid", (checkShutdownLid.Checked ? 1 : 0));

            AppConfig.Set("keyboard_awake_logo", (checkAwakeLogo.Checked ? 1 : 0));
            AppConfig.Set("keyboard_boot_logo", (checkBootLogo.Checked ? 1 : 0));
            AppConfig.Set("keyboard_sleep_logo", (checkSleepLogo.Checked ? 1 : 0));
            AppConfig.Set("keyboard_shutdown_logo", (checkShutdownLogo.Checked ? 1 : 0));

            AsusUSB.ApplyAuraPower();

        }

        private void ComboKeyboardSpeed_SelectedValueChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("aura_speed", (int)comboKeyboardSpeed.SelectedValue);
            AsusUSB.ApplyAura();
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


        private void checkAutoToggleClamshellMode_CheckedChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("toggle_clamshell_mode", checkAutoToggleClamshellMode.Checked ? 1 : 0);

            if (checkAutoToggleClamshellMode.Checked)
            {
                clamshellControl.ToggleLidAction();
            }
            else
            {
                ClamshellModeControl.DisableClamshellMode();
            }

        }
    }
}
