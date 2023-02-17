using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;

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

            buttonQuit.Click += ButtonQuit_Click;

            SetTimer();


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
            aTimer = new System.Timers.Timer(1000);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            var cpuFan = Math.Round(Program.wmi.DeviceGet(ASUSWmi.CPU_Fan) / 0.6);
            var gpuFan = Math.Round(Program.wmi.DeviceGet(ASUSWmi.GPU_Fan) / 0.6);

            Program.settingsForm.BeginInvoke(delegate
            {
                Program.settingsForm.labelCPUFan.Text = "CPU Fan: " + cpuFan.ToString() + "%";
                Program.settingsForm.labelGPUFan.Text = "GPU Fan: " + gpuFan.ToString() + "%";
            });

        }

        private void SettingsForm_VisibleChanged(object? sender, EventArgs e)
        {
            if (this.Visible)
            {
                this.Left = Screen.FromControl(this).Bounds.Width - 10 - this.Width;
                this.Top = Screen.FromControl(this).Bounds.Height - 100 - this.Height;
                this.Activate();
                aTimer.Enabled = true;
            }
            else
            {
                aTimer.Enabled = false;
            }
        }

        public void SetPerformanceMode(int PerformanceMode = ASUSWmi.PerformanceBalanced)
        {

            buttonSilent.FlatAppearance.BorderSize = buttonInactive;
            buttonBalanced.FlatAppearance.BorderSize = buttonInactive;
            buttonTurbo.FlatAppearance.BorderSize = buttonInactive;

            switch (PerformanceMode)
            {
                case ASUSWmi.PerformanceSilent:
                    buttonSilent.FlatAppearance.BorderSize = buttonActive;
                    labelPerf.Text = "Peformance Mode: Silent";
                    break;
                case ASUSWmi.PerformanceTurbo:
                    buttonTurbo.FlatAppearance.BorderSize = buttonActive;
                    labelPerf.Text = "Peformance Mode: Turbo";
                    break;
                default:
                    buttonBalanced.FlatAppearance.BorderSize = buttonActive;
                    labelPerf.Text = "Peformance Mode: Balanced";
                    PerformanceMode = ASUSWmi.PerformanceBalanced;
                    break;
            }


            Program.config.setConfig("performance_mode", PerformanceMode);
            Program.wmi.DeviceSet(ASUSWmi.PerformanceMode, PerformanceMode);

        }


        public void CyclePerformanceMode()
        {
            SetPerformanceMode(Program.config.getConfig("performance_mode") + 1);
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
                    GPUMode = ASUSWmi.GPUModeStandard;
                    VisualiseGPUMode(GPUMode);
                    Program.wmi.DeviceSet(ASUSWmi.GPUEco, 0);
                    Program.config.setConfig("gpu_mode", GPUMode);
                }
                else if (eco == 0 && Plugged == 0)  // Standard going Eco on plugged
                {
                    GPUMode = ASUSWmi.GPUModeEco;
                    VisualiseGPUMode(GPUMode);
                    Program.wmi.DeviceSet(ASUSWmi.GPUEco, 1);
                    Program.config.setConfig("gpu_mode", GPUMode);

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

        public void VisualiseGPUMode(int GPUMode)
        {

            buttonEco.FlatAppearance.BorderSize = buttonInactive;
            buttonStandard.FlatAppearance.BorderSize = buttonInactive;
            buttonUltimate.FlatAppearance.BorderSize = buttonInactive;

            switch (GPUMode)
            {
                case ASUSWmi.GPUModeEco:
                    buttonEco.FlatAppearance.BorderSize = buttonActive;
                    labelGPU.Text = "GPU Mode: Eco (iGPU only)";
                    Program.trayIcon.Icon = GHelper.Properties.Resources.eco;
                    break;
                case ASUSWmi.GPUModeUltimate:
                    buttonUltimate.FlatAppearance.BorderSize = buttonActive;
                    labelGPU.Text = "GPU Mode: Ultimate (dGPU exclusive)";
                    Program.trayIcon.Icon = GHelper.Properties.Resources.ultimate;
                    break;
                default:
                    buttonStandard.FlatAppearance.BorderSize = buttonActive;
                    labelGPU.Text = "GPU Mode: Eco (iGPU and dGPU)";
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

        public void SetBatteryChargeLimit (int limit = 100)
        {

            if (limit < 50 || limit > 100) limit = 100;

            labelBatteryLimit.Text = limit.ToString() + "%";
            trackBattery.Value = limit;
            Program.wmi.DeviceSet(ASUSWmi.BatteryLimit, limit);
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
            {
                Program.config.setConfig("gpu_auto", 1);
            }
            else
            {
                Program.config.setConfig("gpu_auto", 0);
            }
        }


    }


}
