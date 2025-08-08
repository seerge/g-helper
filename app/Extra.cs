using GHelper.Display;
using GHelper.Gpu.AMD;
using GHelper.Helpers;
using GHelper.Input;
using GHelper.Mode;
using GHelper.UI;
using GHelper.USB;
using System.Diagnostics;

namespace GHelper
{
    public partial class Extra : RForm
    {

        ClamshellModeControl clamshellControl = new ClamshellModeControl();

        const string EMPTY = "--------------";


        private void SetKeyCombo(ComboBox combo, TextBox txbox, string name)
        {

            Dictionary<string, string> customActions = new Dictionary<string, string>
            {
              {"", EMPTY},
              {"mute", Properties.Strings.VolumeMute},
              {"screenshot", Properties.Strings.PrintScreen},
              {"play", Properties.Strings.PlayPause},
              {"aura", Properties.Strings.ToggleAura},
              {"performance", Properties.Strings.PerformanceMode},
              {"screen", Properties.Strings.ToggleScreen},
              {"lock", Properties.Strings.LockScreen},
              {"miniled", Properties.Strings.ToggleMiniled},
              {"fnlock", Properties.Strings.ToggleFnLock},
              {"brightness_down", Properties.Strings.BrightnessDown},
              {"brightness_up", Properties.Strings.BrightnessUp},
              {"visual", Properties.Strings.VisualMode},
              {"touchscreen", Properties.Strings.ToggleTouchscreen },
              {"micmute", Properties.Strings.MuteMic},
              {"ghelper", Properties.Strings.OpenGHelper},
              {"custom", Properties.Strings.Custom}
            };

            if (AppConfig.IsDUO())
            {
                customActions.Add("screenpad_down", Properties.Strings.ScreenPadDown);
                customActions.Add("screenpad_up", Properties.Strings.ScreenPadUp);
            }

            if (AppConfig.IsAlly())
            {
                customActions.Add("controller", "Controller Mode");
            }

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
                    customActions.Remove("micmute");
                    break;
                case "m4":
                    customActions[""] = Properties.Strings.OpenGHelper;
                    customActions.Remove("ghelper");
                    break;
                case "fnf4":
                    customActions[""] = Properties.Strings.ToggleAura;
                    customActions.Remove("aura");
                    break;
                case "fnc":
                    customActions[""] = Properties.Strings.ToggleFnLock;
                    customActions.Remove("fnlock");
                    break;
                case "fnv":
                    customActions[""] = Properties.Strings.VisualMode;
                    customActions.Remove("visual");
                    break;
                case "fne":
                    customActions[""] = "Calculator";
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

            checkAwake.Text = Properties.Strings.Awake;
            checkSleep.Text = Properties.Strings.Sleep;
            checkBoot.Text = Properties.Strings.Boot;
            checkShutdown.Text = Properties.Strings.Shutdown;
            checkBootSound.Text = Properties.Strings.BootSound;
            checkStatusLed.Text = Properties.Strings.LEDStatusIndicators;

            labelSpeed.Text = Properties.Strings.AnimationSpeed;
            //labelBrightness.Text = Properties.Strings.Brightness;

            labelBacklightTimeout.Text = Properties.Strings.BacklightTimeout;
            //labelBacklightTimeoutPlugged.Text = Properties.Strings.BacklightTimeoutPlugged;

            checkGPUFix.Text = Properties.Strings.EnableGPUOnShutdown;
            checkNoOverdrive.Text = Properties.Strings.DisableOverdrive;
            checkTopmost.Text = Properties.Strings.WindowTop;
            checkUSBC.Text = Properties.Strings.OptimizedUSBC;
            checkAutoToggleClamshellMode.Text = Properties.Strings.ToggleClamshellMode;

            labelBacklightKeyboard.Text = Properties.Strings.Keyboard;
            labelBacklightBar.Text = Properties.Strings.Lightbar;
            labelBacklightLid.Text = Properties.Strings.Lid;
            labelBacklightLogo.Text = Properties.Strings.Logo;

            checkGpuApps.Text = Properties.Strings.KillGpuApps;
            checkBWIcon.Text = Properties.Strings.BWTrayIcon;
            labelHibernateAfter.Text = Properties.Strings.HibernateAfter;

            labelAPUMem.Text = Properties.Strings.APUMemory;

            Text = Properties.Strings.ExtraSettings;

            // Accessible Labels

            panelServices.AccessibleName = Properties.Strings.AsusServicesRunning;
            panelBindings.AccessibleName = Properties.Strings.KeyBindings;
            tableBindings.AccessibleName = Properties.Strings.KeyBindings;

            comboM1.AccessibleName = "M1 Action";
            comboM2.AccessibleName = "M2 Action";
            comboM3.AccessibleName = "M3 Action";
            comboM4.AccessibleName = "M4 Action";
            comboFNF4.AccessibleName = "Fn+F4 Action";
            comboFNC.AccessibleName = "Fn+C Action";
            comboFNV.AccessibleName = "Fn+V Action";
            comboFNE.AccessibleName = "Fn+Numpad Action";

            numericBacklightPluggedTime.AccessibleName = Properties.Strings.BacklightTimeoutPlugged;
            numericBacklightTime.AccessibleName = Properties.Strings.BacklightTimeoutBattery;

            comboKeyboardSpeed.AccessibleName = Properties.Strings.LaptopBacklight + " " + Properties.Strings.AnimationSpeed;
            comboAPU.AccessibleName = Properties.Strings.LaptopBacklight + " " + Properties.Strings.AnimationSpeed;

            checkBoot.AccessibleName = Properties.Strings.Boot + " " + Properties.Strings.LaptopBacklight;
            checkAwake.AccessibleName = Properties.Strings.Awake + " " + Properties.Strings.LaptopBacklight;
            checkSleep.AccessibleName = Properties.Strings.Sleep + " " + Properties.Strings.LaptopBacklight;
            checkShutdown.AccessibleName = Properties.Strings.Shutdown + " " + Properties.Strings.LaptopBacklight;

            panelSettings.AccessibleName = Properties.Strings.ExtraSettings;
            numericHibernateAfter.AccessibleName = Properties.Strings.HibernateAfter;

            if (AppConfig.IsARCNM())
            {
                labelM3.Text = "FN+F6";
                labelM1.Visible = comboM1.Visible = textM1.Visible = false;
                labelM2.Visible = comboM2.Visible = textM2.Visible = false;
                labelM4.Visible = comboM4.Visible = textM4.Visible = false;
                labelFNF4.Visible = comboFNF4.Visible = textFNF4.Visible = false;
            }

            if (AppConfig.NoMKeys())
            {
                labelM1.Text = "FN+F2";
                labelM2.Text = "FN+F3";
                labelM3.Text = "FN+F4";
                labelM4.Visible = comboM4.Visible = textM4.Visible = AppConfig.IsM4Button();
                labelFNF4.Visible = comboFNF4.Visible = textFNF4.Visible = false;
            }

            if (AppConfig.IsVivoZenPro())
            {
                labelM1.Visible = comboM1.Visible = textM1.Visible = false;
                labelM2.Visible = comboM2.Visible = textM2.Visible = false;
                labelM3.Visible = comboM3.Visible = textM3.Visible = false;
                labelFNF4.Visible = comboFNF4.Visible = textFNF4.Visible = false;
                labelM4.Text = "FN+F12";
            }

            if (AppConfig.MediaKeys())
            {
                labelFNF4.Visible = comboFNF4.Visible = textFNF4.Visible = false;
            }

            if (!AppConfig.IsTUF())
            {
                labelFNE.Visible = comboFNE.Visible = textFNE.Visible = false;
            }

            if (AppConfig.IsNoFNV())
            {
                labelFNV.Visible = comboFNV.Visible = textFNV.Visible = false;
            }

            if (Program.acpi.DeviceGet(AsusACPI.GPUEco) < 0)
            {
                checkGpuApps.Visible = false;
                checkUSBC.Visible = false;
            }

            checkNoOverdrive.Visible = Program.acpi.IsOverdriveSupported();

            // Change text and hide irrelevant options on the ROG Ally,
            // which is a bit of a special case piece of hardware.
            if (AppConfig.IsAlly())
            {
                labelM1.Visible = comboM1.Visible = textM1.Visible = false;
                labelM2.Visible = comboM2.Visible = textM2.Visible = false;

                // Re-label M3 and M4 and FNF4 to match the front labels.
                labelM3.Text = "Cmd Center";
                labelM4.Text = "ROG";
                labelFNF4.Text = "Back Paddles";

                // Hide all of the FN options, as the Ally has no special keyboard FN key.
                labelFNC.Visible = false;
                comboFNC.Visible = false;
                textFNC.Visible = false;

                labelFNV.Visible = false;
                comboFNV.Visible = false;
                textFNV.Visible = false;

                SetKeyCombo(comboM3, textM3, "cc");
                SetKeyCombo(comboM4, textM4, "m4");
                SetKeyCombo(comboFNF4, textFNF4, "paddle");

                checkGpuApps.Visible = false;
                checkUSBC.Visible = false;
                checkAutoToggleClamshellMode.Visible = false;

                int apuMem = Program.acpi.GetAPUMem();
                if (apuMem >= 0)
                {
                    panelAPU.Visible = true;
                    comboAPU.DropDownStyle = ComboBoxStyle.DropDownList;
                    comboAPU.SelectedIndex = apuMem;
                }

                comboAPU.SelectedIndexChanged += ComboAPU_SelectedIndexChanged;

            }
            else
            {
                SetKeyCombo(comboM1, textM1, "m1");
                SetKeyCombo(comboM2, textM2, "m2");

                SetKeyCombo(comboM3, textM3, "m3");
                SetKeyCombo(comboM4, textM4, "m4");
                SetKeyCombo(comboFNF4, textFNF4, "fnf4");

                SetKeyCombo(comboFNC, textFNC, "fnc");
                SetKeyCombo(comboFNV, textFNV, "fnv");
                SetKeyCombo(comboFNE, textFNE, "fne");
            }

            if (AppConfig.IsStrix())
            {
                labelM4.Text = "M5/ROG";
            }


            InitTheme();
            Shown += Keyboard_Shown;

            comboKeyboardSpeed.DropDownStyle = ComboBoxStyle.DropDownList;
            comboKeyboardSpeed.DataSource = new BindingSource(Aura.GetSpeeds(), null);
            comboKeyboardSpeed.DisplayMember = "Value";
            comboKeyboardSpeed.ValueMember = "Key";
            comboKeyboardSpeed.SelectedValue = Aura.Speed;
            comboKeyboardSpeed.SelectedValueChanged += ComboKeyboardSpeed_SelectedValueChanged;

            // Keyboard
            checkAwake.Checked = AppConfig.IsNotFalse("keyboard_awake");
            checkBattery.Checked = AppConfig.IsOnBattery("keyboard_awake");
            checkBoot.Checked = AppConfig.IsNotFalse("keyboard_boot");
            checkSleep.Checked = AppConfig.IsNotFalse("keyboard_sleep");
            checkShutdown.Checked = AppConfig.IsNotFalse("keyboard_shutdown");

            // Lightbar
            checkAwakeBar.Checked = AppConfig.IsNotFalse("keyboard_awake_bar");
            checkBatteryBar.Checked = AppConfig.IsOnBattery("keyboard_awake_bar");
            checkBootBar.Checked = AppConfig.IsNotFalse("keyboard_boot_bar");
            checkSleepBar.Checked = AppConfig.IsNotFalse("keyboard_sleep_bar");
            checkShutdownBar.Checked = AppConfig.IsNotFalse("keyboard_shutdown_bar");

            // Lid
            checkAwakeLid.Checked = AppConfig.IsNotFalse("keyboard_awake_lid");
            checkBatteryLid.Checked = AppConfig.IsOnBattery("keyboard_awake_lid");
            checkBootLid.Checked = AppConfig.IsNotFalse("keyboard_boot_lid");
            checkSleepLid.Checked = AppConfig.IsNotFalse("keyboard_sleep_lid");
            checkShutdownLid.Checked = AppConfig.IsNotFalse("keyboard_shutdown_lid");

            // Logo
            checkAwakeLogo.Checked = AppConfig.IsNotFalse("keyboard_awake_logo");
            checkBatteryLogo.Checked = AppConfig.IsOnBattery("keyboard_awake_logo");
            checkBootLogo.Checked = AppConfig.IsNotFalse("keyboard_boot_logo");
            checkSleepLogo.Checked = AppConfig.IsNotFalse("keyboard_sleep_logo");
            checkShutdownLogo.Checked = AppConfig.IsNotFalse("keyboard_shutdown_logo");

            checkAwake.CheckedChanged += CheckPower_CheckedChanged;
            checkBattery.CheckedChanged += CheckPower_CheckedChanged;
            checkBoot.CheckedChanged += CheckPower_CheckedChanged;
            checkSleep.CheckedChanged += CheckPower_CheckedChanged;
            checkShutdown.CheckedChanged += CheckPower_CheckedChanged;

            checkAwakeBar.CheckedChanged += CheckPower_CheckedChanged;
            checkBatteryBar.CheckedChanged += CheckPower_CheckedChanged;
            checkBootBar.CheckedChanged += CheckPower_CheckedChanged;
            checkSleepBar.CheckedChanged += CheckPower_CheckedChanged;
            checkShutdownBar.CheckedChanged += CheckPower_CheckedChanged;

            checkAwakeLid.CheckedChanged += CheckPower_CheckedChanged;
            checkBatteryLid.CheckedChanged += CheckPower_CheckedChanged;
            checkBootLid.CheckedChanged += CheckPower_CheckedChanged;
            checkSleepLid.CheckedChanged += CheckPower_CheckedChanged;
            checkShutdownLid.CheckedChanged += CheckPower_CheckedChanged;

            checkAwakeLogo.CheckedChanged += CheckPower_CheckedChanged;
            checkBatteryLogo.CheckedChanged += CheckPower_CheckedChanged;
            checkBootLogo.CheckedChanged += CheckPower_CheckedChanged;
            checkSleepLogo.CheckedChanged += CheckPower_CheckedChanged;
            checkShutdownLogo.CheckedChanged += CheckPower_CheckedChanged;

            if (!AppConfig.IsBacklightZones() || AppConfig.IsStrixLimitedRGB() || AppConfig.IsARCNM())
            {

                if (!AppConfig.IsStrixLimitedRGB())
                {
                    labelBacklightBar.Visible = false;
                    checkAwakeBar.Visible = false;
                    checkBatteryBar.Visible = false;
                    checkBootBar.Visible = false;
                    checkSleepBar.Visible = false;
                    checkShutdownBar.Visible = false;

                    labelBacklightKeyboard.Visible = false;
                    checkBattery.Visible = false;
                }

                labelBacklightLid.Visible = false;
                checkAwakeLid.Visible = false;
                checkBatteryLid.Visible = false;
                checkBootLid.Visible = false;
                checkSleepLid.Visible = false;
                checkShutdownLid.Visible = false;

                labelBacklightLogo.Visible = false;
                checkAwakeLogo.Visible = false;
                checkBatteryLogo.Visible = false;
                checkBootLogo.Visible = false;
                checkSleepLogo.Visible = false;
                checkShutdownLogo.Visible = false;
            }

            if (AppConfig.IsZ13())
            {
                labelBacklightBar.Visible = false;
                checkAwakeBar.Visible = false;
                checkBatteryBar.Visible = false;
                checkBootBar.Visible = false;
                checkSleepBar.Visible = false;
                checkShutdownBar.Visible = false;

                labelBacklightLid.Visible = false;
                checkAwakeLid.Visible = false;
                checkBatteryLid.Visible = false;
                checkBootLid.Visible = false;
                checkSleepLid.Visible = false;
                checkShutdownLid.Visible = false;
            }

            //checkAutoToggleClamshellMode.Visible = clamshellControl.IsExternalDisplayConnected();
            checkAutoToggleClamshellMode.Checked = AppConfig.Is("toggle_clamshell_mode");
            checkAutoToggleClamshellMode.CheckedChanged += checkAutoToggleClamshellMode_CheckedChanged;

            checkTopmost.Checked = AppConfig.Is("topmost");
            checkTopmost.CheckedChanged += CheckTopmost_CheckedChanged; ;

            checkNoOverdrive.Checked = AppConfig.IsNoOverdrive();
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

            int bootSound = Program.acpi.DeviceGet(AsusACPI.BootSound);
            if (bootSound < 0 || bootSound > UInt16.MaxValue) bootSound = AppConfig.Get("boot_sound", 0);

            checkBootSound.Checked = (bootSound == 1);
            checkBootSound.CheckedChanged += CheckBootSound_CheckedChanged;

            var statusLed = Program.acpi.DeviceGet(AsusACPI.StatusLed);
            checkStatusLed.Visible = statusLed >= 0;
            checkStatusLed.Checked = (statusLed > 0);
            checkStatusLed.CheckedChanged += CheckLEDStatus_CheckedChanged;

            var optimalBrightness = ScreenControl.GetOptimalBrightness();
            checkOptimalBrightness.Visible = optimalBrightness >= 0;
            checkOptimalBrightness.Checked = (optimalBrightness > 0);
            checkOptimalBrightness.CheckedChanged += CheckOptimalBrightness_CheckedChanged;


            checkBWIcon.Checked = AppConfig.IsBWIcon();
            checkBWIcon.CheckedChanged += CheckBWIcon_CheckedChanged;

            pictureHelp.Click += PictureHelp_Click;
            buttonServices.Click += ButtonServices_Click;

            pictureLog.Click += PictureLog_Click;

            checkGPUFix.Visible = Program.acpi.IsNVidiaGPU();
            checkGPUFix.Checked = AppConfig.IsGPUFix();
            checkGPUFix.CheckedChanged += CheckGPUFix_CheckedChanged;

            checkPerKeyRGB.Visible = AppConfig.IsPossible4ZoneRGB();
            checkPerKeyRGB.Checked = AppConfig.Is("per_key_rgb");
            checkPerKeyRGB.CheckedChanged += CheckPerKeyRGB_CheckedChanged;

            toolTip.SetToolTip(checkAutoToggleClamshellMode, "Disable sleep on lid close when plugged in and external monitor is connected");

            InitCores();
            InitVariBright();
            InitServices();
            InitHibernate();

            InitACPITesting();

        }

        private void CheckOptimalBrightness_CheckedChanged(object? sender, EventArgs e)
        {
            ScreenControl.SetOptimalBrightness(checkOptimalBrightness.Checked ? 1 : 0);
        }

        private void CheckPerKeyRGB_CheckedChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("per_key_rgb", (checkPerKeyRGB.Checked ? 1 : 0));
        }

        private void CheckLEDStatus_CheckedChanged(object? sender, EventArgs e)
        {
            InputDispatcher.SetStatusLED(checkStatusLed.Checked);
        }

        private void CheckBWIcon_CheckedChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("bw_icon", (checkBWIcon.Checked ? 1 : 0));
            Program.settingsForm.VisualiseIcon();
        }

        private void InitACPITesting()
        {
            if (!AppConfig.Is("debug")) return;

            pictureScan.Visible = true;
            panelACPI.Visible = true;

            textACPICommand.Text = "120098";
            textACPIParam.Text = "25";

            buttonACPISend.Click += ButtonACPISend_Click;
            pictureScan.Click += PictureScan_Click;
        }

        private void ButtonACPISend_Click(object? sender, EventArgs e)
        {
            try
            {
                int deviceID = Convert.ToInt32(textACPICommand.Text, 16);
                int status = Convert.ToInt32(textACPIParam.Text, textACPIParam.Text.Contains("x") ? 16 : 10);
                int result = Program.acpi.DeviceSet((uint)deviceID, status, "TestACPI " + deviceID.ToString("X8") + " " + status.ToString("X4"));
                labelACPITitle.Text = "ACPI DEVS Test : " + result.ToString();
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
            }
        }

        private void InitCores()
        {
            (int eCores, int pCores) = Program.acpi.GetCores();
            (int eCoresMax, int pCoresMax) = Program.acpi.GetCores(true);

            if (eCores < 0 || pCores < 0 || eCoresMax < 0 || pCoresMax < 0)
            {
                panelCores.Visible = false;
                return;
            }

            if (eCoresMax == 0) eCoresMax = 8;
            if (pCoresMax == 0) pCoresMax = 6;

            if (AppConfig.Is8Ecores()) eCoresMax = Math.Max(8, eCoresMax);

            eCoresMax = Math.Max(4, eCoresMax);
            pCoresMax = Math.Max(6, pCoresMax);

            panelCores.Visible = true;

            comboCoresE.DropDownStyle = ComboBoxStyle.DropDownList;
            comboCoresP.DropDownStyle = ComboBoxStyle.DropDownList;

            for (int i = AsusACPI.PCoreMin; i <= pCoresMax; i++) comboCoresP.Items.Add(i.ToString() + " Pcores");
            for (int i = AsusACPI.ECoreMin; i <= eCoresMax; i++) comboCoresE.Items.Add(i.ToString() + " Ecores");

            comboCoresP.SelectedIndex = Math.Max(Math.Min(pCores - AsusACPI.PCoreMin, comboCoresP.Items.Count - 1), 0);
            comboCoresE.SelectedIndex = Math.Max(Math.Min(eCores - AsusACPI.ECoreMin, comboCoresE.Items.Count - 1), 0);

            buttonCores.Click += ButtonCores_Click;

        }

        private void ButtonCores_Click(object? sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show(Properties.Strings.AlertAPUMemoryRestart, Properties.Strings.AlertAPUMemoryRestartTitle, MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                Program.acpi.SetCores(AsusACPI.ECoreMin + comboCoresE.SelectedIndex, AsusACPI.PCoreMin + comboCoresP.SelectedIndex);
                Process.Start("shutdown", "/r /t 1");
            }
        }


        private void PictureScan_Click(object? sender, EventArgs e)
        {
            string logFile = Program.acpi.ScanRange();
            new Process
            {
                StartInfo = new ProcessStartInfo(logFile)
                {
                    UseShellExecute = true
                }
            }.Start();
        }

        private void ComboAPU_SelectedIndexChanged(object? sender, EventArgs e)
        {
            int mem = comboAPU.SelectedIndex;
            Program.acpi.SetAPUMem(mem);

            DialogResult dialogResult = MessageBox.Show(Properties.Strings.AlertAPUMemoryRestart, Properties.Strings.AlertAPUMemoryRestartTitle, MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Process.Start("shutdown", "/r /t 1");
            }

        }

        private void CheckBootSound_CheckedChanged(object? sender, EventArgs e)
        {
            int bootSound = checkBootSound.Checked ? 1 : 0;
            Program.acpi.DeviceSet(AsusACPI.BootSound, bootSound, "BootSound");
            AppConfig.Set("boot_sound", bootSound);
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

            Aura.ApplyBrightness(sliderBrightness.Value, "Slider");
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
                if (NvidiaSmi.GetDisplayActiveStatus()) return; // Skip if Nvidia GPU is active
                var status = checkVariBright.Checked ? 1 : 0;
                var result = amdControl.SetVariBright(status);
                Logger.WriteLine($"VariBright {status}: {result}");
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
            XGM.Light(checkXMG.Checked);
        }

        private void CheckUSBC_CheckedChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("optimized_usbc", (checkUSBC.Checked ? 1 : 0));
        }

        private void PictureHelp_Click(object? sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/seerge/g-helper/wiki/Power-user-settings#custom-hotkey-actions") { UseShellExecute = true });
        }

        private void CheckNoOverdrive_CheckedChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("no_overdrive", (checkNoOverdrive.Checked ? 1 : 0));
            ScreenControl.AutoScreen(true);
        }


        private void CheckTopmost_CheckedChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("topmost", (checkTopmost.Checked ? 1 : 0));
            Program.settingsForm.TopMost = checkTopmost.Checked;
        }

        private void CheckPower_CheckedChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("keyboard_awake", (checkAwake.Checked ? 1 : 0));
            AppConfig.Set("keyboard_boot", (checkBoot.Checked ? 1 : 0));
            AppConfig.Set("keyboard_sleep", (checkSleep.Checked ? 1 : 0));
            AppConfig.Set("keyboard_shutdown", (checkShutdown.Checked ? 1 : 0));

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

            if (AppConfig.IsBacklightZones())
            {
                AppConfig.Set("keyboard_awake_bat", (checkBattery.Checked ? 1 : 0));
                AppConfig.Set("keyboard_awake_bar_bat", (checkBatteryBar.Checked ? 1 : 0));
                AppConfig.Set("keyboard_awake_lid_bat", (checkBatteryLid.Checked ? 1 : 0));
                AppConfig.Set("keyboard_awake_logo_bat", (checkBatteryLogo.Checked ? 1 : 0));
            }

            Aura.ApplyPower();

        }

        private void ComboKeyboardSpeed_SelectedValueChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("aura_speed", (int)comboKeyboardSpeed.SelectedValue);
            Aura.ApplyAura();
        }


        private void Keyboard_Shown(object? sender, EventArgs e)
        {
            if (Height > Program.settingsForm.Height)
            {
                var top = Program.settingsForm.Top + Program.settingsForm.Height - Height;

                if (top < 0)
                {
                    MaximumSize = new Size(Width, Program.settingsForm.Height);
                    Top = Program.settingsForm.Top;
                }
                else
                {
                    Top = top;
                }

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

        private void panelAPU_Paint(object sender, PaintEventArgs e)
        {

        }

        private void checkEstimateBatteryTime_CheckedChanged(object sender, EventArgs e)
        {
            AppConfig.Set("estimate_battery_time", checkEstimateBatteryTime.Checked ? 1 : 0);
        }
    }
}
