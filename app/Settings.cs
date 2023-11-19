﻿using GHelper.AnimeMatrix;
using GHelper.AutoUpdate;
using GHelper.Battery;
using GHelper.Display;
using GHelper.Fan;
using GHelper.Gpu;
using GHelper.Helpers;
using GHelper.Input;
using GHelper.Mode;
using GHelper.Peripherals;
using GHelper.Peripherals.Mouse;
using GHelper.UI;
using GHelper.USB;
using System.Diagnostics;
using System.Timers;
using System.Runtime.InteropServices;

namespace GHelper
{
    public partial class SettingsForm : RForm
    {
        ContextMenuStrip contextMenuStrip = new CustomContextMenu();
        ToolStripMenuItem menuSilent, menuBalanced, menuTurbo, menuEco, menuStandard, menuUltimate, menuOptimized;

        GPUModeControl gpuControl;
        ScreenControl screenControl = new ScreenControl();
        AutoUpdateControl updateControl;

        AsusMouseSettings? mouseSettings;

        public AniMatrixControl matrixControl;

        public static System.Timers.Timer sensorTimer = default!;

        public Matrix? matrixForm;
        public Fans? fansForm;
        public Extra? extraForm;
        public Updates? updatesForm;

        static long lastRefresh;
        static long lastBatteryRefresh;
        static long lastLostFocus;

        bool isGpuSection = true;

        bool batteryMouseOver = false;
        bool batteryFullMouseOver = false;

        public SettingsForm()
        {

            InitializeComponent();
            InitTheme(true);

            gpuControl = new GPUModeControl(this);
            updateControl = new AutoUpdateControl(this);
            matrixControl = new AniMatrixControl(this);

            buttonSilent.Text = Properties.Strings.Silent;
            buttonBalanced.Text = Properties.Strings.Balanced;
            buttonTurbo.Text = Properties.Strings.Turbo;
            buttonFans.Text = Properties.Strings.FansPower;

            buttonEco.Text = Properties.Strings.EcoMode;
            buttonUltimate.Text = Properties.Strings.UltimateMode;
            buttonStandard.Text = Properties.Strings.StandardMode;
            buttonOptimized.Text = Properties.Strings.Optimized;
            buttonStopGPU.Text = Properties.Strings.StopGPUApps;

            buttonScreenAuto.Text = Properties.Strings.AutoMode;
            buttonMiniled.Text = Properties.Strings.Multizone;

            buttonKeyboardColor.Text = Properties.Strings.Color;
            buttonKeyboard.Text = Properties.Strings.Extra;

            labelPerf.Text = Properties.Strings.PerformanceMode;
            labelGPU.Text = Properties.Strings.GPUMode;
            labelSreen.Text = Properties.Strings.LaptopScreen;
            labelKeyboard.Text = Properties.Strings.LaptopKeyboard;
            labelMatrix.Text = Properties.Strings.AnimeMatrix;
            labelBatteryTitle.Text = Properties.Strings.BatteryChargeLimit;
            labelPeripherals.Text = Properties.Strings.Peripherals;

            checkMatrix.Text = Properties.Strings.TurnOffOnBattery;
            checkStartup.Text = Properties.Strings.RunOnStartup;

            buttonMatrix.Text = Properties.Strings.PictureGif;
            buttonQuit.Text = Properties.Strings.Quit;
            buttonUpdates.Text = Properties.Strings.Updates;

            FormClosing += SettingsForm_FormClosing;
            Deactivate += SettingsForm_LostFocus;

            buttonSilent.BorderColor = colorEco;
            buttonBalanced.BorderColor = colorStandard;
            buttonTurbo.BorderColor = colorTurbo;
            buttonFans.BorderColor = colorCustom;

            buttonEco.BorderColor = colorEco;
            buttonStandard.BorderColor = colorStandard;
            buttonUltimate.BorderColor = colorTurbo;
            buttonOptimized.BorderColor = colorEco;
            buttonXGM.BorderColor = colorTurbo;

            button60Hz.BorderColor = SystemColors.ActiveBorder;
            button120Hz.BorderColor = SystemColors.ActiveBorder;
            buttonScreenAuto.BorderColor = SystemColors.ActiveBorder;
            buttonMiniled.BorderColor = colorTurbo;

            buttonSilent.Click += ButtonSilent_Click;
            buttonBalanced.Click += ButtonBalanced_Click;
            buttonTurbo.Click += ButtonTurbo_Click;

            buttonEco.Click += ButtonEco_Click;
            buttonStandard.Click += ButtonStandard_Click;
            buttonUltimate.Click += ButtonUltimate_Click;
            buttonOptimized.Click += ButtonOptimized_Click;
            buttonStopGPU.Click += ButtonStopGPU_Click;

            VisibleChanged += SettingsForm_VisibleChanged;

            button60Hz.Click += Button60Hz_Click;
            button120Hz.Click += Button120Hz_Click;
            buttonScreenAuto.Click += ButtonScreenAuto_Click;
            buttonMiniled.Click += ButtonMiniled_Click;

            buttonQuit.Click += ButtonQuit_Click;

            buttonKeyboardColor.Click += ButtonKeyboardColor_Click;

            buttonFans.Click += ButtonFans_Click;
            buttonKeyboard.Click += ButtonKeyboard_Click;

            pictureColor.Click += PictureColor_Click;
            pictureColor2.Click += PictureColor2_Click;

            labelCPUFan.Click += LabelCPUFan_Click;
            labelGPUFan.Click += LabelCPUFan_Click;

            comboMatrix.DropDownStyle = ComboBoxStyle.DropDownList;
            comboMatrixRunning.DropDownStyle = ComboBoxStyle.DropDownList;

            comboMatrix.DropDownClosed += ComboMatrix_SelectedValueChanged;
            comboMatrixRunning.DropDownClosed += ComboMatrixRunning_SelectedValueChanged;

            buttonMatrix.Click += ButtonMatrix_Click;

            checkStartup.Checked = Startup.IsScheduled();
            checkStartup.CheckedChanged += CheckStartup_CheckedChanged;

            labelVersion.Click += LabelVersion_Click;
            labelVersion.ForeColor = Color.FromArgb(128, Color.Gray);

            buttonOptimized.MouseMove += ButtonOptimized_MouseHover;
            buttonOptimized.MouseLeave += ButtonGPU_MouseLeave;

            buttonEco.MouseMove += ButtonEco_MouseHover;
            buttonEco.MouseLeave += ButtonGPU_MouseLeave;

            buttonStandard.MouseMove += ButtonStandard_MouseHover;
            buttonStandard.MouseLeave += ButtonGPU_MouseLeave;

            buttonUltimate.MouseMove += ButtonUltimate_MouseHover;
            buttonUltimate.MouseLeave += ButtonGPU_MouseLeave;

            tableGPU.MouseMove += ButtonXGM_MouseMove;
            tableGPU.MouseLeave += ButtonGPU_MouseLeave;

            buttonXGM.Click += ButtonXGM_Click;

            buttonScreenAuto.MouseMove += ButtonScreenAuto_MouseHover;
            buttonScreenAuto.MouseLeave += ButtonScreen_MouseLeave;

            button60Hz.MouseMove += Button60Hz_MouseHover;
            button60Hz.MouseLeave += ButtonScreen_MouseLeave;

            button120Hz.MouseMove += Button120Hz_MouseHover;
            button120Hz.MouseLeave += ButtonScreen_MouseLeave;

            buttonUpdates.Click += ButtonUpdates_Click;

            sliderBattery.ValueChanged += SliderBattery_ValueChanged;
            Program.trayIcon.MouseMove += TrayIcon_MouseMove;

            sensorTimer = new System.Timers.Timer(1000);
            sensorTimer.Elapsed += OnTimedEvent;
            sensorTimer.Enabled = true;

            labelCharge.MouseEnter += PanelBattery_MouseEnter;
            labelCharge.MouseLeave += PanelBattery_MouseLeave;

            buttonPeripheral1.Click += ButtonPeripheral_Click;
            buttonPeripheral2.Click += ButtonPeripheral_Click;
            buttonPeripheral3.Click += ButtonPeripheral_Click;

            buttonPeripheral1.MouseEnter += ButtonPeripheral_MouseEnter;
            buttonPeripheral2.MouseEnter += ButtonPeripheral_MouseEnter;
            buttonPeripheral3.MouseEnter += ButtonPeripheral_MouseEnter;

            buttonBatteryFull.MouseEnter += ButtonBatteryFull_MouseEnter;
            buttonBatteryFull.MouseLeave += ButtonBatteryFull_MouseLeave;
            buttonBatteryFull.Click += ButtonBatteryFull_Click;

            Text = "G-Helper " + (ProcessHelper.IsUserAdministrator() ? "—" : "-") + " " + AppConfig.GetModelShort();
            TopMost = AppConfig.Is("topmost");

            //This will auto position the window again when it resizes. Might mess with position if people drag the window somewhere else.
            this.Resize += SettingsForm_Resize;
            SetContextMenu();

            VisualiseFnLock();
            buttonFnLock.Click += ButtonFnLock_Click;

            panelPerformance.Focus();
        }

        private void SettingsForm_LostFocus(object? sender, EventArgs e)
        {
            lastLostFocus = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        private void ButtonBatteryFull_Click(object? sender, EventArgs e)
        {
            BatteryControl.ToggleBatteryLimitFull();
        }

        private void ButtonBatteryFull_MouseLeave(object? sender, EventArgs e)
        {
            batteryFullMouseOver = false;
            RefreshSensors(true);
        }

        private void ButtonBatteryFull_MouseEnter(object? sender, EventArgs e)
        {
            batteryFullMouseOver = true;
            labelCharge.Text = Properties.Strings.BatteryLimitFull;
        }

        private void SettingsForm_Resize(object? sender, EventArgs e)
        {
            Left = Screen.FromControl(this).WorkingArea.Width - 10 - Width;
            Top = Screen.FromControl(this).WorkingArea.Height - 10 - Height;
        }

        private void PanelBattery_MouseEnter(object? sender, EventArgs e)
        {
            batteryMouseOver = true;
            ShowBatteryWear();
        }

        private void PanelBattery_MouseLeave(object? sender, EventArgs e)
        {
            batteryMouseOver = false;
            RefreshSensors(true);
        }

        private void ShowBatteryWear()
        {
            //Refresh again only after 15 Minutes since the last refresh
            if (lastBatteryRefresh == 0 || Math.Abs(DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastBatteryRefresh) > 15 * 60_000)
            {
                lastBatteryRefresh = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                HardwareControl.RefreshBatteryHealth();
            }

            if (HardwareControl.batteryHealth != -1)
            {
                labelCharge.Text = Properties.Strings.BatteryHealth + ": " + Math.Round(HardwareControl.batteryHealth, 1) + "%";
            }
        }

        private void SettingsForm_VisibleChanged(object? sender, EventArgs e)
        {
            sensorTimer.Enabled = this.Visible;
            if (this.Visible)
            {
                screenControl.InitScreen();
                VisualizeXGM();

                Task.Run((Action)RefreshPeripheralsBattery);
                updateControl.CheckForUpdates();
            }
        }

        private void RefreshPeripheralsBattery()
        {
            PeripheralsProvider.RefreshBatteryForAllDevices(true);
        }

        private void ButtonUpdates_Click(object? sender, EventArgs e)
        {
            if (updatesForm == null || updatesForm.Text == "")
            {
                updatesForm = new Updates();
                AddOwnedForm(updatesForm);
            }

            if (updatesForm.Visible)
            {
                updatesForm.Close();
            }
            else
            {
                updatesForm.Show();
            }
        }

        public void VisualiseMatrix(string image)
        {
            if (matrixForm == null || matrixForm.Text == "") return;
            matrixForm.VisualiseMatrix(image);
        }

        protected override void WndProc(ref Message m)
        {

            switch (m.Msg)
            {
                case NativeMethods.WM_POWERBROADCAST:
                    if (m.WParam == (IntPtr)NativeMethods.PBT_POWERSETTINGCHANGE)
                    {
                        var settings = (NativeMethods.POWERBROADCAST_SETTING)m.GetLParam(typeof(NativeMethods.POWERBROADCAST_SETTING));
                        switch (settings.Data)
                        {
                            case 0:
                                Logger.WriteLine("Monitor Power Off");
                                Aura.ApplyBrightness(0);
                                break;
                            case 1:
                                Logger.WriteLine("Monitor Power On");
                                Program.SetAutoModes();
                                break;
                            case 2:
                                Logger.WriteLine("Monitor Dimmed");
                                break;
                        }
                    }
                    m.Result = (IntPtr)1;
                    break;
            }

            try
            {
                base.WndProc(ref m);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public void SetContextMenu()
        {

            var mode = Modes.GetCurrent();

            contextMenuStrip.Items.Clear();
            Padding padding = new Padding(15, 5, 5, 5);

            var title = new ToolStripMenuItem(Properties.Strings.PerformanceMode);
            title.Margin = padding;
            title.Enabled = false;
            contextMenuStrip.Items.Add(title);

            menuSilent = new ToolStripMenuItem(Properties.Strings.Silent);
            menuSilent.Click += ButtonSilent_Click;
            menuSilent.Margin = padding;
            menuSilent.Checked = (mode == AsusACPI.PerformanceSilent);
            contextMenuStrip.Items.Add(menuSilent);

            menuBalanced = new ToolStripMenuItem(Properties.Strings.Balanced);
            menuBalanced.Click += ButtonBalanced_Click;
            menuBalanced.Margin = padding;
            menuBalanced.Checked = (mode == AsusACPI.PerformanceBalanced);
            contextMenuStrip.Items.Add(menuBalanced);

            menuTurbo = new ToolStripMenuItem(Properties.Strings.Turbo);
            menuTurbo.Click += ButtonTurbo_Click;
            menuTurbo.Margin = padding;
            menuTurbo.Checked = (mode == AsusACPI.PerformanceTurbo);
            contextMenuStrip.Items.Add(menuTurbo);

            contextMenuStrip.Items.Add("-");

            if (isGpuSection)
            {
                var titleGPU = new ToolStripMenuItem(Properties.Strings.GPUMode);
                titleGPU.Margin = padding;
                titleGPU.Enabled = false;
                contextMenuStrip.Items.Add(titleGPU);

                menuEco = new ToolStripMenuItem(Properties.Strings.EcoMode);
                menuEco.Click += ButtonEco_Click;
                menuEco.Margin = padding;
                contextMenuStrip.Items.Add(menuEco);

                menuStandard = new ToolStripMenuItem(Properties.Strings.StandardMode);
                menuStandard.Click += ButtonStandard_Click;
                menuStandard.Margin = padding;
                contextMenuStrip.Items.Add(menuStandard);

                menuUltimate = new ToolStripMenuItem(Properties.Strings.UltimateMode);
                menuUltimate.Click += ButtonUltimate_Click;
                menuUltimate.Margin = padding;
                contextMenuStrip.Items.Add(menuUltimate);

                menuOptimized = new ToolStripMenuItem(Properties.Strings.Optimized);
                menuOptimized.Click += ButtonOptimized_Click;
                menuOptimized.Margin = padding;
                contextMenuStrip.Items.Add(menuOptimized);

                contextMenuStrip.Items.Add("-");
            }


            var quit = new ToolStripMenuItem(Properties.Strings.Quit);
            quit.Click += ButtonQuit_Click;
            quit.Margin = padding;
            contextMenuStrip.Items.Add(quit);

            //contextMenuStrip.ShowCheckMargin = true;
            contextMenuStrip.RenderMode = ToolStripRenderMode.System;

            if (darkTheme)
            {
                contextMenuStrip.BackColor = this.BackColor;
                contextMenuStrip.ForeColor = this.ForeColor;
            }

            Program.trayIcon.ContextMenuStrip = contextMenuStrip;


        }

        private void ButtonXGM_Click(object? sender, EventArgs e)
        {
            gpuControl.ToggleXGM();
        }

        private void SliderBattery_ValueChanged(object? sender, EventArgs e)
        {
            BatteryControl.SetBatteryChargeLimit(sliderBattery.Value);
        }


        public void SetVersionLabel(string label, bool update = false)
        {
            Invoke(delegate
            {
                labelVersion.Text = label;
                if (update) labelVersion.ForeColor = colorTurbo;
            });
        }


        private void LabelVersion_Click(object? sender, EventArgs e)
        {
            updateControl.LoadReleases();
        }

        private static void TrayIcon_MouseMove(object? sender, MouseEventArgs e)
        {
            Program.settingsForm.RefreshSensors();
        }


        private static void OnTimedEvent(Object? source, ElapsedEventArgs? e)
        {
            Program.settingsForm.RefreshSensors();
        }

        private void Button120Hz_MouseHover(object? sender, EventArgs e)
        {
            labelTipScreen.Text = Properties.Strings.MaxRefreshTooltip;
        }

        private void Button60Hz_MouseHover(object? sender, EventArgs e)
        {
            labelTipScreen.Text = Properties.Strings.MinRefreshTooltip;
        }

        private void ButtonScreen_MouseLeave(object? sender, EventArgs e)
        {
            labelTipScreen.Text = "";
        }

        private void ButtonScreenAuto_MouseHover(object? sender, EventArgs e)
        {
            labelTipScreen.Text = Properties.Strings.AutoRefreshTooltip;
        }

        private void ButtonUltimate_MouseHover(object? sender, EventArgs e)
        {
            labelTipGPU.Text = Properties.Strings.UltimateGPUTooltip;
        }

        private void ButtonStandard_MouseHover(object? sender, EventArgs e)
        {
            labelTipGPU.Text = Properties.Strings.StandardGPUTooltip;
        }

        private void ButtonEco_MouseHover(object? sender, EventArgs e)
        {
            labelTipGPU.Text = Properties.Strings.EcoGPUTooltip;
        }

        private void ButtonOptimized_MouseHover(object? sender, EventArgs e)
        {
            labelTipGPU.Text = Properties.Strings.OptimizedGPUTooltip;
        }

        private void ButtonGPU_MouseLeave(object? sender, EventArgs e)
        {
            labelTipGPU.Text = "";
        }

        private void ButtonXGM_MouseMove(object? sender, MouseEventArgs e)
        {
            if (sender is null) return;
            TableLayoutPanel table = (TableLayoutPanel)sender;

            if (!buttonXGM.Visible) return;

            labelTipGPU.Text = buttonXGM.Bounds.Contains(table.PointToClient(Cursor.Position)) ?
                "XGMobile toggle works only in Standard mode" : "";

        }


        private void ButtonScreenAuto_Click(object? sender, EventArgs e)
        {
            AppConfig.Set("screen_auto", 1);
            screenControl.AutoScreen();
        }


        private void CheckStartup_CheckedChanged(object? sender, EventArgs e)
        {
            if (sender is null) return;
            CheckBox chk = (CheckBox)sender;

            if (chk.Checked)
                Startup.Schedule();
            else
                Startup.UnSchedule();
        }

        private void CheckMatrix_CheckedChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("matrix_auto", checkMatrix.Checked ? 1 : 0);
            matrixControl.SetMatrix();
        }



        private void ButtonMatrix_Click(object? sender, EventArgs e)
        {

            if (matrixForm == null || matrixForm.Text == "")
            {
                matrixForm = new Matrix();
                AddOwnedForm(matrixForm);
            }

            if (matrixForm.Visible)
            {
                matrixForm.Close();
            }
            else
            {
                matrixForm.FormPosition();
                matrixForm.Show();
            }

        }

        public void SetMatrixRunning(int mode)
        {
            Invoke(delegate
            {
                comboMatrixRunning.SelectedIndex = mode;
                if (comboMatrix.SelectedIndex == 0) comboMatrix.SelectedIndex = 3;
            });
        }

        private void ComboMatrixRunning_SelectedValueChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("matrix_running", comboMatrixRunning.SelectedIndex);
            matrixControl.SetMatrix();
        }


        private void ComboMatrix_SelectedValueChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("matrix_brightness", comboMatrix.SelectedIndex);
            matrixControl.SetMatrix();
        }


        private void LabelCPUFan_Click(object? sender, EventArgs e)
        {
            FanSensorControl.fanRpm = !FanSensorControl.fanRpm;
            RefreshSensors(true);
        }

        private void PictureColor2_Click(object? sender, EventArgs e)
        {

            ColorDialog colorDlg = new ColorDialog();
            colorDlg.AllowFullOpen = true;
            colorDlg.Color = pictureColor2.BackColor;

            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                AppConfig.Set("aura_color2", colorDlg.Color.ToArgb());
                SetAura();
            }
        }

        private void PictureColor_Click(object? sender, EventArgs e)
        {
            buttonKeyboardColor.PerformClick();
        }

        private void ButtonKeyboard_Click(object? sender, EventArgs e)
        {
            if (extraForm == null || extraForm.Text == "")
            {
                extraForm = new Extra();
                AddOwnedForm(extraForm);
            }

            if (extraForm.Visible)
            {
                extraForm.Close();
            }
            else
            {
                extraForm.Show();
            }
        }

        public void FansInit()
        {
            Invoke(delegate
            {
                if (fansForm != null && fansForm.Text != "") fansForm.InitAll();
            });
        }

        public void GPUInit()
        {
            Invoke(delegate
            {
                if (fansForm != null && fansForm.Text != "") fansForm.InitGPU();
            });
        }

        public void FansToggle(int index = 0)
        {
            if (fansForm == null || fansForm.Text == "")
            {
                fansForm = new Fans();
                AddOwnedForm(fansForm);
            }

            if (fansForm.Visible)
            {
                fansForm.Close();
            }
            else
            {
                fansForm.FormPosition();
                fansForm.Show();
                fansForm.ToggleNavigation(index);
            }

        }

        private void ButtonFans_Click(object? sender, EventArgs e)
        {
            FansToggle();
        }

        private void ButtonKeyboardColor_Click(object? sender, EventArgs e)
        {

            ColorDialog colorDlg = new ColorDialog();
            colorDlg.AllowFullOpen = true;
            colorDlg.Color = pictureColor.BackColor;

            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                AppConfig.Set("aura_color", colorDlg.Color.ToArgb());
                SetAura();
            }
        }

        public void InitAura()
        {
            Aura.Mode = (AuraMode)AppConfig.Get("aura_mode");
            Aura.Speed = (AuraSpeed)AppConfig.Get("aura_speed");
            Aura.SetColor(AppConfig.Get("aura_color"));
            Aura.SetColor2(AppConfig.Get("aura_color2"));

            comboKeyboard.DropDownStyle = ComboBoxStyle.DropDownList;
            comboKeyboard.DataSource = new BindingSource(Aura.GetModes(), null);
            comboKeyboard.DisplayMember = "Value";
            comboKeyboard.ValueMember = "Key";
            comboKeyboard.SelectedValue = Aura.Mode;
            comboKeyboard.SelectedValueChanged += ComboKeyboard_SelectedValueChanged;


            if (AppConfig.IsSingleColor())
            {
                panelColor.Visible = false;
            }

            if (AppConfig.NoAura())
            {
                comboKeyboard.Visible = false;
            }

            VisualiseAura();

        }

        public void SetAura()
        {
            Task.Run(() =>
            {
                Aura.ApplyAura();
                VisualiseAura();
            });
        }

        public void VisualiseAura()
        {
            Invoke(delegate
            {
                pictureColor.BackColor = Aura.Color1;
                pictureColor2.BackColor = Aura.Color2;
                pictureColor2.Visible = Aura.HasSecondColor();
            });
        }

        public void InitMatrix()
        {

            if (!matrixControl.IsValid)
            {
                panelMatrix.Visible = false;
                return;
            }

            comboMatrix.SelectedIndex = Math.Min(AppConfig.Get("matrix_brightness", 0), comboMatrix.Items.Count - 1);
            comboMatrixRunning.SelectedIndex = Math.Min(AppConfig.Get("matrix_running", 0), comboMatrixRunning.Items.Count - 1);

            checkMatrix.Checked = AppConfig.Is("matrix_auto");
            checkMatrix.CheckedChanged += CheckMatrix_CheckedChanged;

        }


        public void CycleAuraMode()
        {
            if (comboKeyboard.SelectedIndex < comboKeyboard.Items.Count - 1)
                comboKeyboard.SelectedIndex += 1;
            else
                comboKeyboard.SelectedIndex = 0;

            Program.toast.RunToast(comboKeyboard.GetItemText(comboKeyboard.SelectedItem), ToastIcon.BacklightUp);
        }

        private void ComboKeyboard_SelectedValueChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("aura_mode", (int)comboKeyboard.SelectedValue);
            SetAura();
        }


        private void Button120Hz_Click(object? sender, EventArgs e)
        {
            AppConfig.Set("screen_auto", 0);
            screenControl.SetScreen(ScreenControl.MAX_REFRESH, 1);
        }

        private void Button60Hz_Click(object? sender, EventArgs e)
        {
            AppConfig.Set("screen_auto", 0);
            screenControl.SetScreen(60, 0);
        }


        private void ButtonMiniled_Click(object? sender, EventArgs e)
        {
            screenControl.ToogleMiniled();
        }



        public void VisualiseScreen(bool screenEnabled, bool screenAuto, int frequency, int maxFrequency, int overdrive, bool overdriveSetting, int miniled, bool hdr)
        {

            ButtonEnabled(button60Hz, screenEnabled);
            ButtonEnabled(button120Hz, screenEnabled);
            ButtonEnabled(buttonScreenAuto, screenEnabled);
            ButtonEnabled(buttonMiniled, screenEnabled);

            labelSreen.Text = screenEnabled
                ? Properties.Strings.LaptopScreen + ": " + frequency + "Hz" + ((overdrive == 1) ? " + " + Properties.Strings.Overdrive : "")
                : Properties.Strings.LaptopScreen + ": " + Properties.Strings.TurnedOff;

            button60Hz.Activated = false;
            button120Hz.Activated = false;
            buttonScreenAuto.Activated = false;

            if (screenAuto)
            {
                buttonScreenAuto.Activated = true;
            }
            else if (frequency == 60)
            {
                button60Hz.Activated = true;
            }
            else if (frequency > 60)
            {
                button120Hz.Activated = true;
            }

            if (maxFrequency > 60)
            {
                button120Hz.Text = maxFrequency.ToString() + "Hz" + (overdriveSetting ? " + OD" : "");
                panelScreen.Visible = true;
            }
            else if (maxFrequency > 0)
            {
                panelScreen.Visible = false;
            }

            if (miniled >= 0)
            {
                buttonMiniled.Activated = (miniled == 1) || hdr;
                buttonMiniled.Enabled = !hdr;
            }
            else
            {
                buttonMiniled.Visible = false;
            }

        }

        private void ButtonQuit_Click(object? sender, EventArgs e)
        {
            matrixControl.Dispose();
            Close();
            Program.trayIcon.Visible = false;
            Application.Exit();
        }

        /// <summary>
        /// Closes all forms except the settings. Hides the settings
        /// </summary>
        public void HideAll()
        {
            this.Hide();
            if (fansForm != null && fansForm.Text != "") fansForm.Close();
            if (extraForm != null && extraForm.Text != "") extraForm.Close();
            if (updatesForm != null && updatesForm.Text != "") updatesForm.Close();
            if (matrixForm != null && matrixForm.Text != "") matrixForm.Close();
        }

        /// <summary>
        /// Brings all visible windows to the top, with settings being the focus
        /// </summary>
        public void ShowAll()
        {
            this.Activate();
        }

        /// <summary>
        /// Check if any of fans, keyboard, update, or itself has focus
        /// </summary>
        /// <returns>Focus state</returns>
        public bool HasAnyFocus(bool lostFocusCheck = false)
        {
            return (fansForm != null && fansForm.ContainsFocus) ||
                   (extraForm != null && extraForm.ContainsFocus) ||
                   (updatesForm != null && updatesForm.ContainsFocus) ||
                   (matrixForm != null && matrixForm.ContainsFocus) ||
                   this.ContainsFocus ||
                   (lostFocusCheck && Math.Abs(DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastLostFocus) < 300);
        }

        private void SettingsForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                HideAll();
            }
        }

        private void ButtonUltimate_Click(object? sender, EventArgs e)
        {
            gpuControl.SetGPUMode(AsusACPI.GPUModeUltimate);
        }

        private void ButtonStandard_Click(object? sender, EventArgs e)
        {
            gpuControl.SetGPUMode(AsusACPI.GPUModeStandard);
        }

        private void ButtonEco_Click(object? sender, EventArgs e)
        {
            gpuControl.SetGPUMode(AsusACPI.GPUModeEco);
        }


        private void ButtonOptimized_Click(object? sender, EventArgs e)
        {
            AppConfig.Set("gpu_auto", (AppConfig.Get("gpu_auto") == 1) ? 0 : 1);
            VisualiseGPUMode();
            gpuControl.AutoGPUMode(true);
        }

        private void ButtonStopGPU_Click(object? sender, EventArgs e)
        {
            gpuControl.KillGPUApps();
        }

        public async void RefreshSensors(bool force = false)
        {

            if (!force && Math.Abs(DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastRefresh) < 2000) return;
            lastRefresh = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            string cpuTemp = "";
            string gpuTemp = "";
            string battery = "";
            string charge = "";

            HardwareControl.ReadSensors();
            Task.Run((Action)PeripheralsProvider.RefreshBatteryForAllDevices);

            if (HardwareControl.cpuTemp > 0)
                cpuTemp = ": " + Math.Round((decimal)HardwareControl.cpuTemp).ToString() + "°C";

            if (HardwareControl.batteryCapacity > 0)
                charge = Properties.Strings.BatteryCharge + ": " + Math.Round(HardwareControl.batteryCapacity, 1) + "% ";

            if (HardwareControl.batteryRate < 0)
                battery = Properties.Strings.Discharging + ": " + Math.Round(-(decimal)HardwareControl.batteryRate, 1).ToString() + "W";
            else if (HardwareControl.batteryRate > 0)
                battery = Properties.Strings.Charging + ": " + Math.Round((decimal)HardwareControl.batteryRate, 1).ToString() + "W";


            if (HardwareControl.gpuTemp > 0)
            {
                gpuTemp = $": {HardwareControl.gpuTemp}°C";
            }

            string trayTip = "CPU" + cpuTemp + " " + HardwareControl.cpuFan;
            if (gpuTemp.Length > 0) trayTip += "\nGPU" + gpuTemp + " " + HardwareControl.gpuFan;
            if (battery.Length > 0) trayTip += "\n" + battery;

            Program.settingsForm.BeginInvoke(delegate
            {
                labelCPUFan.Text = "CPU" + cpuTemp + " " + HardwareControl.cpuFan;
                labelGPUFan.Text = "GPU" + gpuTemp + " " + HardwareControl.gpuFan;
                if (HardwareControl.midFan is not null)
                    labelMidFan.Text = "Mid " + HardwareControl.midFan;

                labelBattery.Text = battery;
                if (!batteryMouseOver && !batteryFullMouseOver) labelCharge.Text = charge;

                //panelPerformance.AccessibleName = labelPerf.Text + " " + trayTip;
            });


            Program.trayIcon.Text = trayTip;

        }

        public void LabelFansResult(string text)
        {
            if (fansForm != null && fansForm.Text != "")
                fansForm.LabelFansResult(text);
        }

        public void ShowMode(int mode)
        {
            Invoke(delegate
            {
                buttonSilent.Activated = false;
                buttonBalanced.Activated = false;
                buttonTurbo.Activated = false;
                buttonFans.Activated = false;

                menuSilent.Checked = false;
                menuBalanced.Checked = false;
                menuTurbo.Checked = false;

                switch (mode)
                {
                    case AsusACPI.PerformanceSilent:
                        buttonSilent.Activated = true;
                        menuSilent.Checked = true;
                        break;
                    case AsusACPI.PerformanceTurbo:
                        buttonTurbo.Activated = true;
                        menuTurbo.Checked = true;
                        break;
                    case AsusACPI.PerformanceBalanced:
                        buttonBalanced.Activated = true;
                        menuBalanced.Checked = true;
                        break;
                    default:
                        buttonFans.Activated = true;
                        buttonFans.BorderColor = Modes.GetBase(mode) switch
                        {
                            AsusACPI.PerformanceSilent => colorEco,
                            AsusACPI.PerformanceTurbo => colorTurbo,
                            _ => colorStandard,
                        };
                        break;
                }
            });
        }


        public void SetModeLabel(string modeText)
        {
            Invoke(delegate
            {
                labelPerf.Text = modeText;
                panelPerformance.AccessibleName = labelPerf.Text; // + ". " + Program.trayIcon.Text;
            });
        }


        public void AutoKeyboard()
        {

            if (!AppConfig.Is("skip_aura"))
            {
                Aura.ApplyPower();
                Aura.ApplyAura();
            }

            InputDispatcher.SetBacklightAuto(true);

            if (Program.acpi.IsXGConnected())
                XGM.Light(AppConfig.Is("xmg_light"));

            if (AppConfig.HasTabletMode()) InputDispatcher.TabletMode();

        }


        public void VisualizeXGM(int GPUMode = -1)
        {

            bool connected = Program.acpi.IsXGConnected();
            buttonXGM.Enabled = buttonXGM.Visible = connected;

            if (!connected) return;

            if (GPUMode != -1)
                ButtonEnabled(buttonXGM, AppConfig.IsNoGPUModes() || GPUMode != AsusACPI.GPUModeEco);


            int activated = Program.acpi.DeviceGet(AsusACPI.GPUXG);
            Logger.WriteLine("XGM Activated flag: " + activated);

            buttonXGM.Activated = activated == 1;

            if (activated == 1)
            {
                ButtonEnabled(buttonOptimized, false);
                ButtonEnabled(buttonEco, false);
                ButtonEnabled(buttonStandard, false);
                ButtonEnabled(buttonUltimate, false);
            }
            else
            {
                ButtonEnabled(buttonOptimized, true);
                ButtonEnabled(buttonEco, true);
                ButtonEnabled(buttonStandard, true);
                ButtonEnabled(buttonUltimate, true);
            }

        }

        public void VisualiseGPUButtons(bool eco = true, bool ultimate = true)
        {
            if (!eco)
            {
                menuEco.Visible = buttonEco.Visible = false;
                menuOptimized.Visible = buttonOptimized.Visible = false;
                buttonStopGPU.Visible = true;
                tableGPU.ColumnCount = 3;
                tableScreen.ColumnCount = 3;
            }
            else
            {
                buttonStopGPU.Visible = false;
            }

            if (!ultimate)
            {
                menuUltimate.Visible = buttonUltimate.Visible = false;
                tableGPU.ColumnCount = 3;
                tableScreen.ColumnCount = 3;
            }
        }

        public void HideGPUModes(bool gpuExists)
        {
            isGpuSection = false;

            buttonEco.Visible = false;
            buttonStandard.Visible = false;
            buttonUltimate.Visible = false;
            buttonOptimized.Visible = false;
            buttonStopGPU.Visible = true;

            tableGPU.ColumnCount = 0;

            SetContextMenu();

            panelGPU.Visible = gpuExists;

        }


        public void LockGPUModes(string text = null)
        {
            Invoke(delegate
            {
                if (text is null) text = Properties.Strings.GPUMode + ": " + Properties.Strings.GPUChanging + " ...";

                ButtonEnabled(buttonOptimized, false);
                ButtonEnabled(buttonEco, false);
                ButtonEnabled(buttonStandard, false);
                ButtonEnabled(buttonUltimate, false);
                ButtonEnabled(buttonXGM, false);

                labelGPU.Text = text;
            });
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private const uint WM_SETICON = 0x80u;
        private const int ICON_SMALL = 0;
        private const int ICON_BIG = 1;

        public void VisualiseGPUMode(int GPUMode = -1)
        {
            ButtonEnabled(buttonOptimized, true);
            ButtonEnabled(buttonEco, true);
            ButtonEnabled(buttonStandard, true);
            ButtonEnabled(buttonUltimate, true);

            Bitmap? smallBmp = null;
            Bitmap? bigBmp = null;

            if (GPUMode == -1)
                GPUMode = AppConfig.Get("gpu_mode");

            bool GPUAuto = AppConfig.Is("gpu_auto");

            buttonEco.Activated = false;
            buttonStandard.Activated = false;
            buttonUltimate.Activated = false;
            buttonOptimized.Activated = false;

            switch (GPUMode)
            {
                case AsusACPI.GPUModeEco:
                    buttonOptimized.BorderColor = colorEco;
                    buttonEco.Activated = !GPUAuto;
                    buttonOptimized.Activated = GPUAuto;
                    labelGPU.Text = Properties.Strings.GPUMode + ": " + Properties.Strings.GPUModeEco;
                    Program.trayIcon.Icon = Properties.Resources.eco;

                    smallBmp = Properties.Resources.dot_eco.ToBitmap();
                    bigBmp = Properties.Resources.eco.ToBitmap();

                    SendMessage(this.Handle, WM_SETICON, ICON_SMALL, smallBmp.GetHicon());
                    SendMessage(this.Handle, WM_SETICON, ICON_BIG, bigBmp.GetHicon());
                    break;
                case AsusACPI.GPUModeUltimate:
                    buttonUltimate.Activated = true;
                    labelGPU.Text = Properties.Strings.GPUMode + ": " + Properties.Strings.GPUModeUltimate;
                    Program.trayIcon.Icon = Properties.Resources.ultimate;

                    smallBmp = Properties.Resources.dot_ultimate.ToBitmap();
                    bigBmp = Properties.Resources.ultimate.ToBitmap();

                    SendMessage(this.Handle, WM_SETICON, ICON_SMALL, smallBmp.GetHicon());
                    SendMessage(this.Handle, WM_SETICON, ICON_BIG, bigBmp.GetHicon());
                    break;
                default:
                    buttonOptimized.BorderColor = colorStandard;
                    buttonStandard.Activated = !GPUAuto;
                    buttonOptimized.Activated = GPUAuto;
                    labelGPU.Text = Properties.Strings.GPUMode + ": " + Properties.Strings.GPUModeStandard;
                    Program.trayIcon.Icon = Properties.Resources.standard;

                    smallBmp = Properties.Resources.dot_standard.ToBitmap();
                    bigBmp = Properties.Resources.standard.ToBitmap();

                    SendMessage(this.Handle, WM_SETICON, ICON_SMALL, smallBmp.GetHicon());
                    SendMessage(this.Handle, WM_SETICON, ICON_BIG, bigBmp.GetHicon());
                    break;
            }

            VisualizeXGM(GPUMode);


            if (isGpuSection)
            {
                menuEco.Checked = buttonEco.Activated;
                menuStandard.Checked = buttonStandard.Activated;
                menuUltimate.Checked = buttonUltimate.Activated;
                menuOptimized.Checked = buttonOptimized.Activated;
            }

        }


        private void ButtonSilent_Click(object? sender, EventArgs e)
        {
            Program.modeControl.SetPerformanceMode(AsusACPI.PerformanceSilent);
        }

        private void ButtonBalanced_Click(object? sender, EventArgs e)
        {
            Program.modeControl.SetPerformanceMode(AsusACPI.PerformanceBalanced);
        }

        private void ButtonTurbo_Click(object? sender, EventArgs e)
        {
            Program.modeControl.SetPerformanceMode(AsusACPI.PerformanceTurbo);
        }


        public void ButtonEnabled(RButton but, bool enabled)
        {
            but.Enabled = enabled;
            but.BackColor = but.Enabled ? Color.FromArgb(255, but.BackColor) : Color.FromArgb(100, but.BackColor);
        }

        public void VisualiseBattery(int limit)
        {
            labelBatteryTitle.Text = Properties.Strings.BatteryChargeLimit + ": " + limit.ToString() + "%";
            sliderBattery.Value = limit;
            VisualiseBatteryFull();
        }

        public void VisualiseBatteryFull()
        {
            if (AppConfig.Is("charge_full"))
            {
                buttonBatteryFull.BackColor = colorStandard;
                buttonBatteryFull.ForeColor = SystemColors.ControlLightLight;
            }
            else
            {
                buttonBatteryFull.BackColor = buttonSecond;
                buttonBatteryFull.ForeColor = SystemColors.ControlDark;
            }

        }


        public void VisualizePeripherals()
        {
            if (!PeripheralsProvider.IsAnyPeripheralConnect())
            {
                panelPeripherals.Visible = false;
                return;
            }

            Button[] buttons = new Button[] { buttonPeripheral1, buttonPeripheral2, buttonPeripheral3 };

            //we only support 4 devces for now. Who has more than 4 mice connected to the same PC anyways....
            List<IPeripheral> lp = PeripheralsProvider.AllPeripherals();

            for (int i = 0; i < lp.Count && i < buttons.Length; ++i)
            {
                IPeripheral m = lp.ElementAt(i);
                Button b = buttons[i];

                if (m.IsDeviceReady)
                {
                    if (m.HasBattery())
                    {
                        b.Text = m.GetDisplayName() + "\n" + m.Battery + "%"
                                            + (m.Charging ? "(" + Properties.Strings.Charging + ")" : "");
                    }
                    else
                    {
                        b.Text = m.GetDisplayName();
                    }

                }
                else
                {
                    //Mouse is either not connected or in standby
                    b.Text = m.GetDisplayName() + "\n(" + Properties.Strings.NotConnected + ")";
                }

                switch (m.DeviceType())
                {
                    case PeripheralType.Mouse:
                        b.Image = ControlHelper.TintImage(Properties.Resources.icons8_maus_32, b.ForeColor);
                        break;

                    case PeripheralType.Keyboard:
                        b.Image = ControlHelper.TintImage(Properties.Resources.icons8_keyboard_32, b.ForeColor);
                        break;
                }

                b.Visible = true;
            }

            for (int i = lp.Count; i < buttons.Length; ++i)
            {
                buttons[i].Visible = false;
            }

            panelPeripherals.Visible = true;
        }

        private void ButtonPeripheral_MouseEnter(object? sender, EventArgs e)
        {
            int index = 0;
            if (sender == buttonPeripheral2) index = 1;
            if (sender == buttonPeripheral3) index = 2;
            IPeripheral iph = PeripheralsProvider.AllPeripherals().ElementAt(index);


            if (iph is null)
            {
                return;
            }

            if (!iph.IsDeviceReady)
            {
                //Refresh battery on hover if the device is marked as "Not Ready"
                iph.ReadBattery();
            }
        }

        private void ButtonPeripheral_Click(object? sender, EventArgs e)
        {
            if (mouseSettings is not null)
            {
                mouseSettings.Close();
                return;
            }

            int index = 0;
            if (sender == buttonPeripheral2) index = 1;
            if (sender == buttonPeripheral3) index = 2;

            IPeripheral iph = PeripheralsProvider.AllPeripherals().ElementAt(index);

            if (iph is null)
            {
                //Can only happen when the user hits the button in the exact moment a device is disconnected.
                return;
            }

            if (iph.DeviceType() == PeripheralType.Mouse)
            {
                AsusMouse? am = iph as AsusMouse;
                if (am is null || !am.IsDeviceReady)
                {
                    //Should not happen if all device classes are implemented correctly. But better safe than sorry.
                    return;
                }
                mouseSettings = new AsusMouseSettings(am);
                mouseSettings.TopMost = true;
                mouseSettings.FormClosed += MouseSettings_FormClosed;
                mouseSettings.Disposed += MouseSettings_Disposed;
                if (!mouseSettings.IsDisposed)
                {
                    mouseSettings.Show();
                }
                else
                {
                    mouseSettings = null;
                }

            }
        }

        private void MouseSettings_Disposed(object? sender, EventArgs e)
        {
            mouseSettings = null;
        }

        private void MouseSettings_FormClosed(object? sender, FormClosedEventArgs e)
        {
            mouseSettings = null;
        }

        public void VisualiseFnLock()
        {

            if (AppConfig.Is("fn_lock"))
            {
                buttonFnLock.BackColor = colorStandard;
                buttonFnLock.ForeColor = SystemColors.ControlLightLight;
            }
            else
            {
                buttonFnLock.BackColor = buttonSecond;
                buttonFnLock.ForeColor = SystemColors.ControlDark;
            }
        }


        private void ButtonFnLock_Click(object? sender, EventArgs e)
        {
            InputDispatcher.ToggleFnLock();
        }

    }


}
