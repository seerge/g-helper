using System.Diagnostics;
using System.Timers;
using System.Windows.Forms;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;

namespace GHelper
{

    public partial class SettingsForm : Form
    {

        static Color colorEco = Color.FromArgb(255, 6, 180, 138);
        static Color colorStandard = Color.FromArgb(255, 58, 174, 239);
        static Color colorTurbo = Color.FromArgb(255, 255, 32, 32);

        static int buttonInactive = 0;
        static int buttonActive = 5;

        static System.Timers.Timer aTimer = default!;

        public SettingsForm()
        {

            InitializeComponent();

            FormClosing += SettingsForm_FormClosing;

            buttonSilent.FlatAppearance.BorderColor = colorEco;
            buttonBalanced.FlatAppearance.BorderColor = colorStandard;
            buttonTurbo.FlatAppearance.BorderColor = colorTurbo;

            buttonEco.FlatAppearance.BorderColor = colorEco;
            buttonStandard.FlatAppearance.BorderColor = colorStandard;
            buttonUltimate.FlatAppearance.BorderColor = colorTurbo;

            buttonSilent.Click += ButtonSilent_Click;
            buttonBalanced.Click += ButtonBalanced_Click;
            buttonTurbo.Click += ButtonTurbo_Click;

            buttonEco.Click += ButtonEco_Click;
            buttonStandard.Click += ButtonStandard_Click;
            buttonUltimate.Click += ButtonUltimate_Click;

            VisibleChanged += SettingsForm_VisibleChanged;

            trackBattery.Scroll += trackBatteryChange;

            button60Hz.Click += Button60Hz_Click;
            button120Hz.Click += Button120Hz_Click;

            buttonQuit.Click += ButtonQuit_Click;

            checkBoost.Click += CheckBoost_Click;

            checkScreen.CheckedChanged += checkScreen_CheckedChanged;

            comboKeyboard.DropDownStyle = ComboBoxStyle.DropDownList;
            comboKeyboard.SelectedIndex = 0;

            comboKeyboard.SelectedValueChanged += ComboKeyboard_SelectedValueChanged;

            buttonKeyboardColor.Click += ButtonKeyboardColor_Click;

            SetTimer();

        }

        private void ButtonKeyboardColor_Click(object? sender, EventArgs e)
        {

            Button but = (Button)sender;

            ColorDialog colorDlg = new ColorDialog();
            colorDlg.AllowFullOpen = false;
            colorDlg.Color = but.FlatAppearance.BorderColor;

            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                SetAuraColor(colorDlg.Color);
            }
        }

        public void InitAura()
        {
            int mode = Program.config.getConfig("aura_mode");
            int colorCode = Program.config.getConfig("aura_color");
            int speed = Program.config.getConfig("aura_speed");

            Color color = Color.FromArgb(255, 255, 255);

            if (mode == -1)
                mode = 0;

            if (colorCode != -1)
                color = Color.FromArgb(colorCode);


            SetAuraColor(color, false);
            SetAuraMode(mode, false);

            Aura.Mode = mode;


        }

        public void SetAuraColor(Color color, bool apply = true)
        {
            Aura.Color1 = color;
            Program.config.setConfig("aura_color", color.ToArgb());

            if (apply)
                Aura.ApplyAura();

            buttonKeyboardColor.FlatAppearance.BorderColor = color;
        }

        public void SetAuraMode(int mode = 0, bool apply = true)
        {

            //Debug.WriteLine(mode);

            if (mode > 3) mode = 0;

            if (Aura.Mode == mode) return; // same mode

            Aura.Mode = mode;
            Program.config.setConfig("aura_mode", mode);

            comboKeyboard.SelectedValueChanged -= ComboKeyboard_SelectedValueChanged;
            comboKeyboard.SelectedIndex = mode;
            comboKeyboard.SelectedValueChanged += ComboKeyboard_SelectedValueChanged;

            if (apply)
                Aura.ApplyAura();

        }

        public void CycleAuraMode()
        {
            SetAuraMode(Program.config.getConfig("aura_mode") + 1);
        }

        private void ComboKeyboard_SelectedValueChanged(object? sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            SetAuraMode(cmb.SelectedIndex);
        }


        private void CheckBoost_Click(object? sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            if (chk.Checked)
                NativeMethods.SetCPUBoost(3);
            else
                NativeMethods.SetCPUBoost(0);
        }

        private void Button120Hz_Click(object? sender, EventArgs e)
        {
            SetScreen(1000, 1);
        }

        private void Button60Hz_Click(object? sender, EventArgs e)
        {
            SetScreen(60, 0);
        }


        public void SetScreen(int frequency = -1, int overdrive = -1)
        {

            int currentFrequency = NativeMethods.GetRefreshRate();
            if (currentFrequency < 0)  // Laptop screen not detected or has unknown refresh rate
                return;

            if (frequency >= 1000)
            {
                frequency = Program.config.getConfig("max_frequency");
                if (frequency <= 60)
                    frequency = 120;
            }

            if (frequency > 0)
                NativeMethods.SetRefreshRate(frequency);

            try
            {
                if (overdrive > 0)
                    Program.wmi.DeviceSet(ASUSWmi.ScreenOverdrive, overdrive);
            }
            catch
            {
                Debug.WriteLine("Screen Overdrive not supported");
            }


            InitScreen();
        }


        public void InitBoost()
        {
            int boost = NativeMethods.GetCPUBoost();
            checkBoost.Checked = (boost > 0);
        }

        public void InitScreen()
        {

            int frequency = NativeMethods.GetRefreshRate();
            int maxFrequency = Program.config.getConfig("max_frequency");

            if (frequency < 0)
            {
                button60Hz.Enabled = false;
                button120Hz.Enabled = false;
                labelSreen.Text = "Laptop Screen: Turned off";
                button60Hz.BackColor = SystemColors.ControlLight;
                button120Hz.BackColor = SystemColors.ControlLight;
            }
            else
            {
                button60Hz.Enabled = true;
                button120Hz.Enabled = true;
                button60Hz.BackColor = SystemColors.ControlLightLight;
                button120Hz.BackColor = SystemColors.ControlLightLight;
                labelSreen.Text = "Laptop Screen";
            }

            int overdrive = 0;
            try
            {
                overdrive = Program.wmi.DeviceGet(ASUSWmi.ScreenOverdrive);
            }
            catch
            {
                Debug.WriteLine("Screen Overdrive not supported");
            }

            button60Hz.FlatAppearance.BorderSize = buttonInactive;
            button120Hz.FlatAppearance.BorderSize = buttonInactive;

            if (frequency == 60)
            {
                button60Hz.FlatAppearance.BorderSize = buttonActive;
            }
            else
            {
                if (frequency > 60)
                    maxFrequency = frequency;

                Program.config.setConfig("max_frequency", maxFrequency);
                button120Hz.FlatAppearance.BorderSize = buttonActive;
            }

            if (maxFrequency > 60)
            {
                button120Hz.Text = maxFrequency.ToString() + "Hz + OD";
            }

            Program.config.setConfig("frequency", frequency);
            Program.config.setConfig("overdrive", overdrive);
        }

        private void ButtonQuit_Click(object? sender, EventArgs e)
        {
            Close();
            Program.trayIcon.Visible = false;
            Application.Exit();
        }

        private void SettingsForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void ButtonUltimate_Click(object? sender, EventArgs e)
        {
            SetGPUMode(ASUSWmi.GPUModeUltimate);
        }

        private void ButtonStandard_Click(object? sender, EventArgs e)
        {
            SetGPUMode(ASUSWmi.GPUModeStandard);
        }

        private void ButtonEco_Click(object? sender, EventArgs e)
        {
            SetGPUMode(ASUSWmi.GPUModeEco);
        }

        private static void SetTimer()
        {
            aTimer = new System.Timers.Timer(500);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = false;
        }

        private static void RefreshSensors()
        {
            string cpuFan = " Fan: " + Math.Round(Program.wmi.DeviceGet(ASUSWmi.CPU_Fan) / 0.6).ToString() + "%";
            string gpuFan = " Fan: " + Math.Round(Program.wmi.DeviceGet(ASUSWmi.GPU_Fan) / 0.6) + "%";

            string cpuTemp = "";
            string gpuTemp = "";
            string battery = "";

            try
            {
                Program.hwmonitor.ReadSensors();
            }
            catch
            {
                Debug.WriteLine("Failed reading sensors");
            }

            if (Program.hwmonitor.cpuTemp > 0)
                cpuTemp = ": " + Math.Round((decimal)Program.hwmonitor.cpuTemp).ToString() + "°C - ";

            if (Program.hwmonitor.gpuTemp > 0)
                gpuTemp = ": " + Math.Round((decimal)Program.hwmonitor.gpuTemp).ToString() + "°C - ";

            if (Program.hwmonitor.batteryDischarge > 0)
                battery = "Discharging: " + Math.Round((decimal)Program.hwmonitor.batteryDischarge, 1).ToString() + "W";

            if (Program.hwmonitor.batteryCharge > 0)
                battery = "Charging: " + Math.Round((decimal)Program.hwmonitor.batteryCharge, 1).ToString() + "W";

            Program.settingsForm.BeginInvoke(delegate
            {
                Program.settingsForm.labelCPUFan.Text = "CPU" + cpuTemp + cpuFan;
                Program.settingsForm.labelGPUFan.Text = "GPU" + gpuTemp + gpuFan;
                Program.settingsForm.labelBattery.Text = battery;
            });
        }

        private static void OnTimedEvent(Object? source, ElapsedEventArgs? e)
        {
            RefreshSensors();
            aTimer.Interval = 2000;
        }

        private void SettingsForm_VisibleChanged(object? sender, EventArgs e)
        {
            if (this.Visible)
            {
                InitScreen();

                this.Left = Screen.FromControl(this).Bounds.Width - 10 - this.Width;
                this.Top = Screen.FromControl(this).WorkingArea.Height - 10 - this.Height;
                this.Activate();

                aTimer.Interval = 500;
                aTimer.Enabled = true;

            }
            else
            {
                aTimer.Enabled = false;
                Program.hwmonitor.StopReading();
            }
        }

        public void SetPerformanceMode(int PerformanceMode = ASUSWmi.PerformanceBalanced, bool notify = false)
        {

            string perfName;

            buttonSilent.FlatAppearance.BorderSize = buttonInactive;
            buttonBalanced.FlatAppearance.BorderSize = buttonInactive;
            buttonTurbo.FlatAppearance.BorderSize = buttonInactive;

            string[] mode = new string[]{
                "Balanced",
                "Turbo",
                "Silent"
            };

            switch (PerformanceMode)
            {
                case ASUSWmi.PerformanceSilent:
                    buttonSilent.FlatAppearance.BorderSize = buttonActive;
                    perfName = "Silent";
                    break;
                case ASUSWmi.PerformanceTurbo:
                    buttonTurbo.FlatAppearance.BorderSize = buttonActive;
                    perfName = "Turbo";
                    break;
                default:
                    buttonBalanced.FlatAppearance.BorderSize = buttonActive;
                    PerformanceMode = ASUSWmi.PerformanceBalanced;
                    perfName = "Balanced";
                    break;
            }

            string notifTitle = "Performance Mode Changed";
            string notifBody = "Switched to: " + mode[PerformanceMode];
            Program.config.setConfig("performance_mode", PerformanceMode);
            try
            {
                Program.wmi.DeviceSet(ASUSWmi.PerformanceMode, PerformanceMode);
            } catch
            {
                labelPerf.Text = "Performance Mode: not supported";
            }

            if(notify)
              sendNotification(notifTitle, notifBody);
        }


        public void CyclePerformanceMode()
        {
            SetPerformanceMode(Program.config.getConfig("performance_mode") + 1, true);
        }


        public void sendNotification(string title, string message)
        {
            var content = new ToastContentBuilder()
                .AddText(title)
                .AddText(message)
                .SetToastDuration(ToastDuration.Short)
                .GetToastContent();

            var notification = new ToastNotification(content.GetXml())
            {
                Priority = ToastNotificationPriority.High
            };

            ToastNotificationManagerCompat.CreateToastNotifier().Show(notification);
        }


        public void AutoScreen(int Plugged = 1)
        {
            int ScreenAuto = Program.config.getConfig("screen_auto");
            if (ScreenAuto != 1) return;

            if (Plugged == 1)
                SetScreen(1000, 1);
            else
                SetScreen(60, 0);

            InitScreen();

        }

        public void AutoGPUMode(int Plugged = 1)
        {

            int GpuAuto = Program.config.getConfig("gpu_auto");
            if (GpuAuto != 1) return;

            int eco = Program.wmi.DeviceGet(ASUSWmi.GPUEco);
            int mux = Program.wmi.DeviceGet(ASUSWmi.GPUMux);

            int GPUMode;

            if (mux == 0) // GPU in Ultimate, ignore
                return;
            else
            {
                if (eco == 1 && Plugged == 1)  // Eco going Standard on plugged
                {
                    Program.wmi.DeviceSet(ASUSWmi.GPUEco, 0);
                    InitGPUMode();
                }
                else if (eco == 0 && Plugged == 0)  // Standard going Eco on plugged
                {
                    Program.wmi.DeviceSet(ASUSWmi.GPUEco, 1);
                    InitGPUMode();
                }

            }
        }

        public int InitGPUMode()
        {

            int eco = Program.wmi.DeviceGet(ASUSWmi.GPUEco);
            int mux = Program.wmi.DeviceGet(ASUSWmi.GPUMux);

            int GpuMode;

            if (mux == 0)
                GpuMode = ASUSWmi.GPUModeUltimate;
            else
            {
                if (eco == 1)
                    GpuMode = ASUSWmi.GPUModeEco;
                else
                    GpuMode = ASUSWmi.GPUModeStandard;

                if (mux != 1)
                    Disable_Ultimate();
            }

            Program.config.setConfig("gpu_mode", GpuMode);
            VisualiseGPUMode(GpuMode);

            return GpuMode;

        }

        public void SetGPUMode(int GPUMode = ASUSWmi.GPUModeStandard)
        {

            int CurrentGPU = Program.config.getConfig("gpu_mode");

            if (CurrentGPU == GPUMode)
                return;

            var restart = false;
            var changed = false;

            if (CurrentGPU == ASUSWmi.GPUModeUltimate)
            {
                DialogResult dialogResult = MessageBox.Show("Switching off Ultimate Mode requires restart", "Reboot now?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Program.wmi.DeviceSet(ASUSWmi.GPUMux, 1);
                    restart = true;
                    changed = true;
                }
            }
            else if (GPUMode == ASUSWmi.GPUModeUltimate)
            {
                DialogResult dialogResult = MessageBox.Show(" Ultimate Mode requires restart", "Reboot now?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Program.wmi.DeviceSet(ASUSWmi.GPUMux, 0);
                    restart = true;
                    changed = true;
                }

            }
            else if (GPUMode == ASUSWmi.GPUModeEco)
            {
                VisualiseGPUMode(GPUMode);
                Program.wmi.DeviceSet(ASUSWmi.GPUEco, 1);
                changed = true;
            }
            else if (GPUMode == ASUSWmi.GPUModeStandard)
            {
                VisualiseGPUMode(GPUMode);
                Program.wmi.DeviceSet(ASUSWmi.GPUEco, 0);
                changed = true;
            }

            if (changed)
                Program.config.setConfig("gpu_mode", GPUMode);

            if (restart)
            {
                VisualiseGPUMode(GPUMode);
                Process.Start("shutdown", "/r /t 1");
            }

        }


        public void VisualiseGPUAuto(int GPUAuto)
        {
            checkGPU.Checked = (GPUAuto == 1);
        }

        public void VisualiseScreenAuto(int ScreenAuto)
        {
            checkScreen.Checked = (ScreenAuto == 1);
        }

        public void VisualiseGPUMode(int GPUMode)
        {

            buttonEco.FlatAppearance.BorderSize = buttonInactive;
            buttonStandard.FlatAppearance.BorderSize = buttonInactive;
            buttonUltimate.FlatAppearance.BorderSize = buttonInactive;

            switch (GPUMode)
            {
                case ASUSWmi.GPUModeEco:
                    buttonEco.FlatAppearance.BorderSize = buttonActive;
                    labelGPU.Text = "GPU Mode: iGPU only";
                    Program.trayIcon.Icon = GHelper.Properties.Resources.eco;
                    break;
                case ASUSWmi.GPUModeUltimate:
                    buttonUltimate.FlatAppearance.BorderSize = buttonActive;
                    labelGPU.Text = "GPU Mode: dGPU exclusive";
                    Program.trayIcon.Icon = GHelper.Properties.Resources.ultimate;
                    break;
                default:
                    buttonStandard.FlatAppearance.BorderSize = buttonActive;
                    labelGPU.Text = "GPU Mode: iGPU and dGPU";
                    Program.trayIcon.Icon = GHelper.Properties.Resources.standard;
                    break;
            }
        }


        private void ButtonSilent_Click(object? sender, EventArgs e)
        {
            SetPerformanceMode(ASUSWmi.PerformanceSilent);
        }

        private void ButtonBalanced_Click(object? sender, EventArgs e)
        {
            SetPerformanceMode(ASUSWmi.PerformanceBalanced);
        }

        private void ButtonTurbo_Click(object? sender, EventArgs e)
        {
            SetPerformanceMode(ASUSWmi.PerformanceTurbo);
        }

        private void Settings_Load(object sender, EventArgs e)
        {

        }

        public void Disable_Ultimate()
        {
            buttonUltimate.Enabled = false;
            buttonUltimate.BackColor = SystemColors.ControlLight;
        }

        public void SetStartupCheck(bool status)
        {
            checkStartup.Checked = status;
        }
        private void checkStartup_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            if (chk.Checked)
            {
                Program.scheduler.Schedule();
            }
            else
            {
                Program.scheduler.UnSchedule();
            }
        }

        public void SetBatteryChargeLimit(int limit = 100)
        {

            if (limit < 50 || limit > 100) limit = 100;

            labelBatteryTitle.Text = "Battery Charge Limit: " + limit.ToString() + "%";
            trackBattery.Value = limit;
            try
            {
                Program.wmi.DeviceSet(ASUSWmi.BatteryLimit, limit);
            }
            catch
            {
                Debug.WriteLine("Can't set battery charge limit");
            }
            Program.config.setConfig("charge_limit", limit);

        }

        private void trackBatteryChange(object sender, EventArgs e)
        {
            TrackBar bar = (TrackBar)sender;
            SetBatteryChargeLimit(bar.Value);
        }

        private void checkGPU_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            if (chk.Checked)
                Program.config.setConfig("gpu_auto", 1);
            else
                Program.config.setConfig("gpu_auto", 0);
        }


        private void checkScreen_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            if (chk.Checked)
                Program.config.setConfig("screen_auto", 1);
            else
                Program.config.setConfig("screen_auto", 0);
        }


    }


}
