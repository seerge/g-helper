using GHelper.AnimeMatrix;
using GHelper.AutoUpdate;
using GHelper.Battery;
using GHelper.Display;
using GHelper.Gpu;
using GHelper.Helpers;
using GHelper.Input;
using GHelper.Mode;
using GHelper.UI;
using System.Diagnostics;
using System.Timers;
using System.Windows.Forms;

namespace GHelper
{

    public partial class SettingsForm : RForm
    {

        ContextMenuStrip contextMenuStrip = new CustomContextMenu();
        ToolStripMenuItem menuSilent, menuBalanced, menuTurbo, menuEco, menuStandard, menuUltimate, menuOptimized;

        GPUModeControl gpuControl;
        ScreenControl screenControl = new ScreenControl();
        AutoUpdateControl updateControl;

        public AniMatrixControl matrix;

        public static System.Timers.Timer sensorTimer = default!;

        public Fans? fans;
        public Extra? keyb;
        public Updates? updates;

        static long lastRefresh;
        static long lastBatteryRefresh;

        bool isGpuSection = true;
        bool batteryMouseOver = false;

        public SettingsForm()
        {

            InitializeComponent();
            InitTheme(true);

            gpuControl = new GPUModeControl(this);
            updateControl = new AutoUpdateControl(this);
            matrix = new AniMatrixControl(this);

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

            checkMatrix.Text = Properties.Strings.TurnOffOnBattery;
            checkStartup.Text = Properties.Strings.RunOnStartup;

            buttonMatrix.Text = Properties.Strings.PictureGif;
            buttonQuit.Text = Properties.Strings.Quit;
            buttonUpdates.Text = Properties.Strings.Updates;

            FormClosing += SettingsForm_FormClosing;

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

            labelBattery.MouseEnter += PanelBattery_MouseEnter;
            labelBattery.MouseLeave += PanelBattery_MouseLeave;

            labelModel.Text = AppConfig.GetModelShort() + (ProcessHelper.IsUserAdministrator() ? "." : "");
            TopMost = AppConfig.Is("topmost");

            SetContextMenu();

        }

        private void PanelBattery_MouseEnter(object? sender, EventArgs e)
        {
            batteryMouseOver = true;
            ShowBatteryWear();
        }

        private void PanelBattery_MouseLeave(object? sender, EventArgs e)
        {
            batteryMouseOver = false;
            RefreshSensors();
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
                labelBattery.Text = Properties.Strings.BatteryHealth + ": " + Math.Round(HardwareControl.batteryHealth, 1) + "%";
            }
        }

        private void SettingsForm_VisibleChanged(object? sender, EventArgs e)
        {
            sensorTimer.Enabled = this.Visible;
            if (this.Visible)
            {
                screenControl.InitScreen();
                gpuControl.InitXGM();
                updateControl.CheckForUpdates();
            }
        }

        private void ButtonUpdates_Click(object? sender, EventArgs e)
        {
            if (updates == null || updates.Text == "")
            {
                updates = new Updates();
                updates.Show();
            }
            else
            {
                updates.Close();
            }
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
                                AsusUSB.ApplyBrightness(0);
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

            if (CheckSystemDarkModeStatus())
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
            matrix.SetMatrix();
        }



        private void ButtonMatrix_Click(object? sender, EventArgs e)
        {
            matrix.OpenMatrixPicture();
        }

        public void SetMatrixRunning(int mode)
        {
            Invoke(delegate
            {
                comboMatrixRunning.SelectedIndex = mode;
            });
        }

        private void ComboMatrixRunning_SelectedValueChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("matrix_running", comboMatrixRunning.SelectedIndex);
            matrix.SetMatrix();
        }


        private void ComboMatrix_SelectedValueChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("matrix_brightness", comboMatrix.SelectedIndex);
            matrix.SetMatrix();
        }


        private void LabelCPUFan_Click(object? sender, EventArgs e)
        {
            AppConfig.Set("fan_rpm", (AppConfig.Get("fan_rpm") == 1) ? 0 : 1);
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
                AsusUSB.ApplyAura();
                VisualiseAura();
            }
        }

        private void PictureColor_Click(object? sender, EventArgs e)
        {
            buttonKeyboardColor.PerformClick();
        }

        private void ButtonKeyboard_Click(object? sender, EventArgs e)
        {
            if (keyb == null || keyb.Text == "")
            {
                keyb = new Extra();
                keyb.Show();
            }
            else
            {
                keyb.Close();
            }
        }

        public void FansInit()
        {
            Invoke(delegate
            {
                if (fans != null && fans.Text != "") fans.InitAll();
            });
        }

        public void FansToggle(int index = 0)
        {
            if (fans == null || fans.Text == "")
            {
                fans = new Fans();
            }

            if (fans.Visible)
            {
                fans.Close();
            }
            else
            {
                fans.FormPosition();
                fans.Show();
                fans.ToggleNavigation(index);
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
                AsusUSB.ApplyAura();
                VisualiseAura();
            }
        }

        public void InitAura()
        {
            AsusUSB.Mode = AppConfig.Get("aura_mode");
            AsusUSB.Speed = AppConfig.Get("aura_speed");
            AsusUSB.SetColor(AppConfig.Get("aura_color"));
            AsusUSB.SetColor2(AppConfig.Get("aura_color2"));

            comboKeyboard.DropDownStyle = ComboBoxStyle.DropDownList;
            comboKeyboard.DataSource = new BindingSource(AsusUSB.GetModes(), null);
            comboKeyboard.DisplayMember = "Value";
            comboKeyboard.ValueMember = "Key";
            comboKeyboard.SelectedValue = AsusUSB.Mode;
            comboKeyboard.SelectedValueChanged += ComboKeyboard_SelectedValueChanged;


            if (AsusUSB.HasColor())
            {
                panelColor.Visible = false;
            }

            if (AppConfig.ContainsModel("GA401I"))
            {
                comboKeyboard.Visible = false;
            }

            VisualiseAura();

        }

        public void VisualiseAura()
        {
            pictureColor.BackColor = AsusUSB.Color1;
            pictureColor2.BackColor = AsusUSB.Color2;
            pictureColor2.Visible = AsusUSB.HasSecondColor();
        }

        public void InitMatrix()
        {

            if (!matrix.IsValid)
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
        }

        private void ComboKeyboard_SelectedValueChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("aura_mode", (int)comboKeyboard.SelectedValue);
            AsusUSB.ApplyAura();
            VisualiseAura();
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



        public void VisualiseScreen(bool screenEnabled, bool screenAuto, int frequency, int maxFrequency, int overdrive, bool overdriveSetting, int miniled)
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
                buttonMiniled.Activated = (miniled == 1);
            }
            else
            {
                buttonMiniled.Visible = false;
            }

        }

        private void ButtonQuit_Click(object? sender, EventArgs e)
        {
            matrix.Dispose();
            Close();
            Program.trayIcon.Visible = false;
            Application.Exit();
        }

        public void HideAll()
        {
            this.Hide();
            if (fans != null && fans.Text != "") fans.Close();
            if (keyb != null && keyb.Text != "") keyb.Close();
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

            HardwareControl.ReadSensors();

            if (HardwareControl.cpuTemp > 0)
                cpuTemp = ": " + Math.Round((decimal)HardwareControl.cpuTemp).ToString() + "°C";

            if (HardwareControl.batteryRate < 0)
                battery = Properties.Strings.Discharging + ": " + Math.Round(-(decimal)HardwareControl.batteryRate, 1).ToString() + "W";
            else if (HardwareControl.batteryRate > 0)
                battery = Properties.Strings.Charging + ": " + Math.Round((decimal)HardwareControl.batteryRate, 1).ToString() + "W";



            if (HardwareControl.gpuTemp > 0)
            {
                gpuTemp = $": {HardwareControl.gpuTemp}°C";
            }


            Program.settingsForm.BeginInvoke(delegate
            {
                labelCPUFan.Text = "CPU" + cpuTemp + " " + HardwareControl.cpuFan;
                labelGPUFan.Text = "GPU" + gpuTemp + " " + HardwareControl.gpuFan;
                if (HardwareControl.midFan is not null)
                    labelMidFan.Text = "SYS " + HardwareControl.midFan;

                if (!batteryMouseOver) labelBattery.Text = battery;
            });

            string trayTip = "CPU" + cpuTemp + " " + HardwareControl.cpuFan;
            if (gpuTemp.Length > 0) trayTip += "\nGPU" + gpuTemp + " " + HardwareControl.gpuFan;
            if (battery.Length > 0) trayTip += "\n" + battery;

            Program.trayIcon.Text = trayTip;

        }

        public void LabelFansResult(string text)
        {
            if (fans != null && fans.Text != "")
                fans.LabelFansResult(text);
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
            });
        }


        public void AutoKeyboard()
        {

            AsusUSB.ApplyAuraPower();
            AsusUSB.ApplyAura();

            InputDispatcher.SetBacklightAuto(true);

            if (Program.acpi.IsXGConnected())
                AsusUSB.ApplyXGMLight(AppConfig.Is("xmg_light"));

            if (AppConfig.ContainsModel("X16") || AppConfig.ContainsModel("X13")) InputDispatcher.TabletMode();

        }


        public void VisualizeXGM(bool connected, bool activated)
        {

            buttonXGM.Enabled = buttonXGM.Visible = connected;
            if (!connected) return;

            buttonXGM.Activated = activated;

            if (activated)
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

        public void HideUltimateMode()
        {
            tableGPU.Controls.Remove(buttonUltimate);
            tablePerf.ColumnCount = 0;
            tableGPU.ColumnCount = 0;
            tableScreen.ColumnCount = 0;
            menuUltimate.Visible = false;
        }

        public void HideGPUModes()
        {
            isGpuSection = false;

            buttonEco.Visible = false;
            buttonStandard.Visible = false;
            buttonUltimate.Visible = false;
            buttonOptimized.Visible = false;

            buttonStopGPU.Visible = true;

            SetContextMenu();
            if (HardwareControl.FormatFan(Program.acpi.DeviceGet(AsusACPI.GPU_Fan)) is null) panelGPU.Visible = false;

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


        public void VisualiseGPUMode(int GPUMode = -1)
        {
            ButtonEnabled(buttonOptimized, true);
            ButtonEnabled(buttonEco, true);
            ButtonEnabled(buttonStandard, true);
            ButtonEnabled(buttonUltimate, true);

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
                    ButtonEnabled(buttonXGM, false);
                    break;
                case AsusACPI.GPUModeUltimate:
                    buttonUltimate.Activated = true;
                    labelGPU.Text = Properties.Strings.GPUMode + ": " + Properties.Strings.GPUModeUltimate;
                    Program.trayIcon.Icon = Properties.Resources.ultimate;
                    break;
                default:
                    buttonOptimized.BorderColor = colorStandard;
                    buttonStandard.Activated = !GPUAuto;
                    buttonOptimized.Activated = GPUAuto;
                    labelGPU.Text = Properties.Strings.GPUMode + ": " + Properties.Strings.GPUModeStandard;
                    Program.trayIcon.Icon = Properties.Resources.standard;
                    ButtonEnabled(buttonXGM, true);
                    break;
            }

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
        }


    }


}
