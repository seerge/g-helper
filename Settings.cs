using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Windows.ApplicationModel.Store;

namespace GHelper
{
    public partial class SettingsForm : Form
    {

        static Color colorActive = Color.LightGray;

        static System.Timers.Timer aTimer;

        public SettingsForm()
        {

            InitializeComponent();

            buttonSilent.Click += ButtonSilent_Click;
            buttonBalanced.Click += ButtonBalanced_Click;
            buttonTurbo.Click += ButtonTurbo_Click;

            buttonEco.Click += ButtonEco_Click;
            buttonStandard.Click += ButtonStandard_Click;
            buttonUltimate.Click += ButtonUltimate_Click;

            VisibleChanged += SettingsForm_VisibleChanged;

            SetTimer();
            

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
            var cpuFan = Math.Round(Program.wmi.DeviceGet(ASUSWmi.CPU_Fan)/0.6);
            var gpuFan = Math.Round(Program.wmi.DeviceGet(ASUSWmi.GPU_Fan)/0.6);

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
            } else
            {
                aTimer.Enabled = false;
            }
        }

        public void SetPerformanceMode(int PerformanceMode = ASUSWmi.PerformanceBalanced)
        {

            buttonSilent.UseVisualStyleBackColor = true;
            buttonBalanced.UseVisualStyleBackColor = true;
            buttonTurbo.UseVisualStyleBackColor = true;


            switch (PerformanceMode)
            {
                case ASUSWmi.PerformanceSilent:
                    buttonSilent.BackColor = colorActive;
                    groupPerf.Text = "Peformance Mode: Silent";
                    break;
                case ASUSWmi.PerformanceTurbo:
                    buttonTurbo.BackColor = colorActive;
                    groupPerf.Text = "Peformance Mode: Turbo";  
                    break;
                default:
                    buttonBalanced.BackColor = colorActive;
                    groupPerf.Text = "Peformance Mode: Balanced";
                    PerformanceMode = ASUSWmi.PerformanceBalanced;
                    break;
            }

            Program.config.PerformanceMode = PerformanceMode;
            Program.wmi.DeviceSet(ASUSWmi.PerformanceMode, PerformanceMode);

        }


        public void SetGPUMode(int GPUMode = ASUSWmi.GPUModeStandard)
        { 

            int CurrentGPU = ASUSWmi.GPUModeStandard;

            if (((IDictionary<String, object>) Program.config).ContainsKey("gpu_mode")) {
                CurrentGPU = Program.config.gpu_mode;
            }

            if (CurrentGPU == GPUMode) { return; }

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
            } else if (GPUMode == ASUSWmi.GPUModeUltimate)
            {
                DialogResult dialogResult = MessageBox.Show(" Ultimate Mode requires restart", "Reboot now?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Program.wmi.DeviceSet(ASUSWmi.GPUMux, 0);
                    restart = true;
                    changed = true;
                }

            } else if (GPUMode == ASUSWmi.GPUModeEco)
            {
                VisualiseGPUMode(GPUMode);
                Program.wmi.DeviceSet(ASUSWmi.GPUEco, 1);
                changed = true;
            } else if (GPUMode == ASUSWmi.GPUModeStandard)
            {
                VisualiseGPUMode(GPUMode);
                Program.wmi.DeviceSet(ASUSWmi.GPUEco, 0);
                changed = true;
            }

            if (changed)
            {
                Program.config.gpu_mode = GPUMode;
            }

            if (restart)
            {
                VisualiseGPUMode(GPUMode);
                Process.Start("shutdown", "/r /t 1");
            }

        }

        public void VisualiseGPUMode (int GPUMode)
        {

            buttonEco.UseVisualStyleBackColor = true;
            buttonStandard.UseVisualStyleBackColor = true;
            buttonUltimate.UseVisualStyleBackColor = true;


            switch (GPUMode)
            {
                case ASUSWmi.GPUModeEco:
                    buttonEco.BackColor = colorActive;
                    groupGPU.Text = "GPU Mode: Eco (iGPU only)";
                    break;
                case ASUSWmi.GPUModeUltimate:
                    buttonUltimate.BackColor = colorActive;
                    groupGPU.Text = "GPU Mode: Ultimate (dGPU exclusive)";
                    break;
                default:
                    buttonStandard.BackColor = colorActive;
                    groupGPU.Text = "GPU Mode: Eco (iGPU and dGPU)";
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
            buttonUltimate.Enabled= false;
        }

   
    }


}
