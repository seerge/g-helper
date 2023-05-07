using CustomControls;
using GHelper.Gpu;
using System.Diagnostics;
using System.Windows.Forms.DataVisualization.Charting;

namespace GHelper
{
    public partial class Fans : RForm
    {

        DataPoint curPoint = null;
        Series seriesCPU;
        Series seriesGPU;
        Series seriesMid;

        static int MinRPM, MaxRPM;

        NvidiaGpuControl? nvControl = null;

        public Fans()
        {

            InitializeComponent();

            Text = Properties.Strings.FansAndPower;
            labelPowerLimits.Text = Properties.Strings.PowerLimits;
            labelInfo.Text = Properties.Strings.PPTExperimental;
            checkApplyPower.Text = Properties.Strings.ApplyPowerLimits;

            labelFans.Text = Properties.Strings.FanCurves;
            labelBoost.Text = Properties.Strings.CPUBoost;
            buttonReset.Text = Properties.Strings.FactoryDefaults;
            checkApplyFans.Text = Properties.Strings.ApplyFanCurve;

            labelGPU.Text = Properties.Strings.GPUSettings;

            InitTheme();

            MinRPM = 18;
            MaxRPM = HardwareControl.GetFanMax();
            labelTip.Visible = false;
            labelTip.BackColor = Color.Transparent;

            FormClosing += Fans_FormClosing;

            seriesCPU = chartCPU.Series.Add("CPU");
            seriesGPU = chartGPU.Series.Add("GPU");
            seriesMid = chartMid.Series.Add("Mid");

            seriesCPU.Color = colorStandard;
            seriesGPU.Color = colorTurbo;
            seriesMid.Color = colorEco;

            chartCPU.MouseMove += ChartCPU_MouseMove;
            chartCPU.MouseUp += ChartCPU_MouseUp;

            chartGPU.MouseMove += ChartCPU_MouseMove;
            chartGPU.MouseUp += ChartCPU_MouseUp;

            chartMid.MouseMove += ChartCPU_MouseMove;
            chartMid.MouseUp += ChartCPU_MouseUp;

            buttonReset.Click += ButtonReset_Click;

            trackTotal.Maximum = ASUSWmi.MaxTotal;
            trackTotal.Minimum = ASUSWmi.MinTotal;

            trackCPU.Maximum = ASUSWmi.MaxCPU;
            trackCPU.Minimum = ASUSWmi.MinCPU;

            trackCPU.Scroll += TrackPower_Scroll;
            trackTotal.Scroll += TrackPower_Scroll;

            trackCPU.MouseUp += TrackPower_MouseUp;
            trackTotal.MouseUp += TrackPower_MouseUp;

            checkApplyFans.Click += CheckApplyFans_Click;
            checkApplyPower.Click += CheckApplyPower_Click;

            trackGPUCore.Minimum = NvidiaGpuControl.MinCoreOffset;
            trackGPUCore.Maximum = NvidiaGpuControl.MaxCoreOffset;

            trackGPUMemory.Minimum = NvidiaGpuControl.MinMemoryOffset;
            trackGPUMemory.Maximum = NvidiaGpuControl.MaxMemoryOffset;

            trackGPUBoost.Minimum = ASUSWmi.MinGPUBoost;
            trackGPUBoost.Maximum = ASUSWmi.MaxGPUBoost;

            trackGPUTemp.Minimum = ASUSWmi.MinGPUTemp;
            trackGPUTemp.Maximum = ASUSWmi.MaxGPUTemp;

            trackGPUCore.Scroll += trackGPU_Scroll;
            trackGPUMemory.Scroll += trackGPU_Scroll;

            trackGPUBoost.Scroll += trackGPUPower_Scroll;
            trackGPUTemp.Scroll += trackGPUPower_Scroll;

            trackGPUCore.MouseUp += TrackGPU_MouseUp;
            trackGPUMemory.MouseUp += TrackGPU_MouseUp;

            trackGPUBoost.MouseUp += TrackGPUBoost_MouseUp;
            trackGPUTemp.MouseUp += TrackGPUBoost_MouseUp;

            //labelInfo.MaximumSize = new Size(280, 0);
            labelInfo.Text = Properties.Strings.PPTExperimental;
            labelFansResult.Visible = false;

            InitFans();
            InitPower();
            InitBoost();

            comboBoost.SelectedValueChanged += ComboBoost_Changed;

            Shown += Fans_Shown;

            InitGPUControl();

        }

        private void TrackGPUBoost_MouseUp(object? sender, MouseEventArgs e)
        {
            Program.config.setConfig("gpu_boost", trackGPUBoost.Value);
            Program.config.setConfig("gpu_temp", trackGPUTemp.Value);
            Program.settingsForm.SetGPUPower();
        }

        private void TrackGPU_MouseUp(object? sender, MouseEventArgs e)
        {
            try
            {
                Program.config.setConfig("gpu_core", trackGPUCore.Value);
                Program.config.setConfig("gpu_memory", trackGPUMemory.Value);

                int status = nvControl.SetClocks(trackGPUCore.Value, trackGPUMemory.Value);
                if (status == -1) Program.RunAsAdmin("gpu");
            }
            catch (Exception ex)
            {
                Logger.WriteLine("F:" + ex.ToString());
            }

            InitGPUControl();
        }

        private void InitGPUControl()
        {
            if (HardwareControl.GpuControl is not null && HardwareControl.GpuControl.IsNvidia)
            {
                nvControl = (NvidiaGpuControl)HardwareControl.GpuControl;
            }
            else
            {
                panelGPU.Visible = false;
                return;
            }

            try
            {
                panelGPU.Visible = true;

                nvControl.GetClocks(out int core, out int memory, out string gpuTitle);

                trackGPUCore.Value = Math.Max(Math.Min(core, NvidiaGpuControl.MaxCoreOffset), NvidiaGpuControl.MinCoreOffset);
                trackGPUMemory.Value = Math.Max(Math.Min(memory, NvidiaGpuControl.MaxMemoryOffset), NvidiaGpuControl.MinMemoryOffset);
                labelGPU.Text = gpuTitle;

                int gpu_boost = Program.config.getConfig("gpu_boost");
                int gpu_temp = Program.config.getConfig("gpu_temp");

                if (gpu_boost < 0) gpu_boost = ASUSWmi.MaxGPUBoost;
                if (gpu_temp < 0) gpu_temp = ASUSWmi.MaxGPUTemp;

                trackGPUBoost.Value = Math.Max(Math.Min(gpu_boost, ASUSWmi.MaxGPUBoost), ASUSWmi.MinGPUBoost);
                trackGPUTemp.Value = Math.Max(Math.Min(gpu_temp, ASUSWmi.MaxGPUTemp), ASUSWmi.MinGPUTemp);

                panelGPUBoost.Visible = (Program.wmi.DeviceGet(ASUSWmi.PPT_GPUC0) >= 0);
                panelGPUTemp.Visible = (Program.wmi.DeviceGet(ASUSWmi.PPT_GPUC2) >= 0);

                VisualiseGPUSettings();

            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.ToString());
                panelGPU.Visible = false;
            }

        }

        private void VisualiseGPUSettings()
        {
            labelGPUCore.Text = $"{trackGPUCore.Value} MHz";
            labelGPUMemory.Text = $"{trackGPUMemory.Value} MHz";
            labelGPUBoost.Text = $"{trackGPUBoost.Value}W";
            labelGPUTemp.Text = $"{trackGPUTemp.Value}°C";
        }

        private void trackGPU_Scroll(object? sender, EventArgs e)
        {
            if (sender is null) return;

            TrackBar track = (TrackBar)sender;
            track.Value = (int)Math.Round((float)track.Value / 5) * 5;
            VisualiseGPUSettings();

        }

        private void trackGPUPower_Scroll(object? sender, EventArgs e)
        {
            VisualiseGPUSettings();
        }

        static string ChartPercToRPM(int percentage, string unit = "")
        {
            if (percentage == 0) return "OFF";

            return (200 * Math.Round((float)(MinRPM * 100 + (MaxRPM - MinRPM) * percentage) / 200)).ToString() + unit;
        }

        void SetChart(Chart chart, int device)
        {

            string title;

            if (device == 1)
                title = Properties.Strings.FanProfileGPU;
            else if (device == 2)
                title = Properties.Strings.FanProfileMid;
            else
                title = Properties.Strings.FanProfileCPU;

            if (Program.settingsForm.perfName.Length > 0)
                labelFans.Text = Properties.Strings.FanProfiles + ": " + Program.settingsForm.perfName;

            chart.Titles[0].Text = title;

            chart.ChartAreas[0].AxisX.Minimum = 10;
            chart.ChartAreas[0].AxisX.Maximum = 100;
            chart.ChartAreas[0].AxisX.Interval = 10;

            chart.ChartAreas[0].AxisY.Minimum = 0;
            chart.ChartAreas[0].AxisY.Maximum = 100;

            chart.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Arial", 7F);

            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = chartGrid;
            chart.ChartAreas[0].AxisY.MajorGrid.LineColor = chartGrid;
            chart.ChartAreas[0].AxisX.LineColor = chartGrid;
            chart.ChartAreas[0].AxisY.LineColor = chartGrid;

            for (int i = 0; i <= 90; i += 10)
                chart.ChartAreas[0].AxisY.CustomLabels.Add(i - 2, i + 2, ChartPercToRPM(i));

            chart.ChartAreas[0].AxisY.CustomLabels.Add(98, 102, Properties.Strings.RPM);

            chart.ChartAreas[0].AxisY.Interval = 10;

            if (chart.Legends.Count > 0)
                chart.Legends[0].Enabled = false;

        }

        private void Fans_Shown(object? sender, EventArgs e)
        {
            if (Height > Program.settingsForm.Height)
            {
                Top = Program.settingsForm.Top + Program.settingsForm.Height - Height;
            }
            else
            {
                MinimumSize = new Size(0, Program.settingsForm.Height);
                Height = Program.settingsForm.Height;
                Top = Program.settingsForm.Top;
            }


            Left = Program.settingsForm.Left - Width - 5;
        }


        private void TrackPower_MouseUp(object? sender, MouseEventArgs e)
        {
            Program.settingsForm.AutoPower();
        }


        public void InitBoost()
        {
            int boost = NativeMethods.GetCPUBoost();
            if (boost >= 0)
                comboBoost.SelectedIndex = Math.Min(boost, comboBoost.Items.Count - 1);
        }

        private void ComboBoost_Changed(object? sender, EventArgs e)
        {
            if (Program.config.getConfigPerf("auto_boost") != comboBoost.SelectedIndex)
            {
                NativeMethods.SetCPUBoost(comboBoost.SelectedIndex);
                Program.config.setConfigPerf("auto_boost", comboBoost.SelectedIndex);
            }
        }

        private void CheckApplyPower_Click(object? sender, EventArgs e)
        {
            if (sender is null) return;
            CheckBox chk = (CheckBox)sender;
            Program.config.setConfigPerf("auto_apply_power", chk.Checked ? 1 : 0);

            if (chk.Checked)
            {
                Program.settingsForm.AutoPower();
            }
            else
            {
                Program.wmi.DeviceSet(ASUSWmi.PerformanceMode, Program.config.getConfig("performance_mode"), "PerfMode");
                Program.settingsForm.AutoFans();
            }

        }

        private void CheckApplyFans_Click(object? sender, EventArgs e)
        {
            if (sender is null) return;
            CheckBox chk = (CheckBox)sender;

            Program.config.setConfigPerf("auto_apply", chk.Checked ? 1 : 0);

            if (chk.Checked)
            {
                Program.settingsForm.AutoFans();
            }
            else
            {
                Program.wmi.DeviceSet(ASUSWmi.PerformanceMode, Program.config.getConfig("performance_mode"), "PerfMode");
                Program.settingsForm.AutoPower();
            }
        }


        public void LabelFansResult(string text)
        {
            labelFansResult.Text = text;
            labelFansResult.Visible = (text.Length > 0);
        }

        private void Fans_FormClosing(object? sender, FormClosingEventArgs e)
        {

            /*
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }*/
        }


        public void InitPower(bool changed = false)
        {

            bool cpuBmode = (Program.wmi.DeviceGet(ASUSWmi.PPT_CPUB0) >= 0); // 2022 model +
            bool cpuAmode = (Program.wmi.DeviceGet(ASUSWmi.PPT_TotalA0) >= 0); // 2021 model +

            panelPower.Visible = cpuAmode;
            panelCPU.Visible = cpuBmode;

            // Yes, that's stupid, but Total slider on 2021 model actually adjusts CPU PPT
            if (!cpuBmode)
            {
                labelPlatform.Text = "CPU SPPT";
            }

            int limit_total;
            int limit_cpu;
            bool apply = Program.config.getConfigPerf("auto_apply_power") == 1;

            if (changed)
            {
                limit_total = trackTotal.Value;
                limit_cpu = trackCPU.Value;
            }
            else
            {
                limit_total = Program.config.getConfigPerf("limit_total");
                limit_cpu = Program.config.getConfigPerf("limit_cpu");
            }

            if (limit_total < 0) limit_total = ASUSWmi.DefaultTotal;
            if (limit_total > ASUSWmi.MaxTotal) limit_total = ASUSWmi.MaxTotal;
            if (limit_total < ASUSWmi.MinTotal) limit_total = ASUSWmi.MinTotal;

            if (limit_cpu < 0) limit_cpu = ASUSWmi.DefaultCPU;
            if (limit_cpu > ASUSWmi.MaxCPU) limit_cpu = ASUSWmi.MaxCPU;
            if (limit_cpu < ASUSWmi.MinCPU) limit_cpu = ASUSWmi.MinCPU;
            if (limit_cpu > limit_total) limit_cpu = limit_total;

            trackTotal.Value = limit_total;
            trackCPU.Value = limit_cpu;
            checkApplyPower.Checked = apply;

            labelTotal.Text = trackTotal.Value.ToString() + "W";
            labelCPU.Text = trackCPU.Value.ToString() + "W";

            Program.config.setConfigPerf("limit_total", limit_total);
            Program.config.setConfigPerf("limit_cpu", limit_cpu);


        }


        private void TrackPower_Scroll(object? sender, EventArgs e)
        {
            InitPower(true);
        }


        public void InitFans()
        {

            byte[] curve = Program.wmi.GetFanCurve(2);

            if (curve.All(singleByte => singleByte == 0))
            {
                Program.config.setConfig("mid_fan", 0);

            }
            else
            {
                Program.config.setConfig("mid_fan", 1);
                chartMid.Visible = true;
                SetChart(chartMid, 2);
                LoadProfile(seriesMid, 2);
            }


            SetChart(chartCPU, 0);
            SetChart(chartGPU, 1);

            LoadProfile(seriesCPU, 0);
            LoadProfile(seriesGPU, 1);

            int auto_apply = Program.config.getConfigPerf("auto_apply");

            checkApplyFans.Checked = (auto_apply == 1);

        }


        void LoadProfile(Series series, int device, int def = 0)
        {

            series.ChartType = SeriesChartType.Line;
            series.MarkerSize = 10;
            series.MarkerStyle = MarkerStyle.Circle;

            series.Points.Clear();

            int mode = Program.config.getConfig("performance_mode");
            byte[] curve = Program.config.getFanConfig(device);

            if (def == 1 || curve.Length != 16)
                curve = Program.wmi.GetFanCurve(device, mode);

            if (curve.Length != 16 || curve.All(singleByte => singleByte == 0))
                curve = Program.config.getDefaultCurve(device);

            //Debug.WriteLine(BitConverter.ToString(curve));

            byte old = 0;
            for (int i = 0; i < 8; i++)
            {
                if (curve[i] == old) curve[i]++; // preventing 2 points in same spot from default asus profiles
                series.Points.AddXY(curve[i], curve[i + 8]);
                old = curve[i];
            }

            SaveProfile(series, device);

        }

        void SaveProfile(Series series, int device)
        {
            byte[] curve = new byte[16];
            int i = 0;
            foreach (DataPoint point in series.Points)
            {
                curve[i] = (byte)point.XValue;
                curve[i + 8] = (byte)point.YValues.First();
                i++;
            }

            Program.config.setFanConfig(device, curve);
            //Program.wmi.SetFanCurve(device, curve);

        }


        private void ButtonReset_Click(object? sender, EventArgs e)
        {

            LoadProfile(seriesCPU, 0, 1);
            LoadProfile(seriesGPU, 1, 1);
            if (Program.config.getConfig("mid_fan") == 1)
                LoadProfile(seriesMid, 2, 1);

            checkApplyFans.Checked = false;
            checkApplyPower.Checked = false;

            trackGPUCore.Value = 0;
            trackGPUMemory.Value = 0;
            trackGPUBoost.Value = ASUSWmi.MaxGPUBoost;
            trackGPUTemp.Value = ASUSWmi.MaxGPUTemp;

            Program.config.setConfig("gpu_core", ASUSWmi.MaxGPUBoost);
            Program.config.setConfig("gpu_memory", ASUSWmi.MaxGPUTemp);

            Program.config.setConfigPerf("auto_apply", 0);
            Program.config.setConfigPerf("auto_apply_power", 0);

            Program.wmi.DeviceSet(ASUSWmi.PerformanceMode, Program.config.getConfig("performance_mode"), "PerfMode");
        }

        private void ChartCPU_MouseUp(object? sender, MouseEventArgs e)
        {
            curPoint = null;
            labelTip.Visible = false;

            SaveProfile(seriesCPU, 0);
            SaveProfile(seriesGPU, 1);
            if (Program.config.getConfig("mid_fan") == 1)
                SaveProfile(seriesMid, 2);

            Program.settingsForm.AutoFans();


        }

        private void ChartCPU_MouseMove(object? sender, MouseEventArgs e)
        {

            if (sender is null) return;
            Chart chart = (Chart)sender;

            ChartArea ca = chart.ChartAreas[0];
            Axis ax = ca.AxisX;
            Axis ay = ca.AxisY;

            bool tip = false;

            HitTestResult hit = chart.HitTest(e.X, e.Y);
            if (hit.Series is not null && hit.PointIndex >= 0)
            {
                curPoint = hit.Series.Points[hit.PointIndex];
                tip = true;
            }


            if (curPoint != null)
            {

                double dx, dy, dymin;

                try
                {
                    dx = ax.PixelPositionToValue(e.X);
                    dy = ay.PixelPositionToValue(e.Y);

                    if (dx < 20) dx = 20;
                    if (dx > 100) dx = 100;

                    if (dy < 0) dy = 0;
                    if (dy > 100) dy = 100;

                    dymin = (dx - 65) * 1.2;

                    if (dy < dymin) dy = dymin;

                    if (e.Button.HasFlag(MouseButtons.Left))
                    {
                        curPoint.XValue = dx;
                        curPoint.YValues[0] = dy;

                        if (hit.Series is not null)
                            AdjustAllLevels(hit.PointIndex, dx, dy, hit.Series);

                        tip = true;
                    }

                    labelTip.Text = Math.Round(curPoint.XValue) + "C, " + ChartPercToRPM((int)curPoint.YValues[0], " " + Properties.Strings.RPM);
                    labelTip.Top = e.Y + ((Control)sender).Top;
                    labelTip.Left = e.X - 50;

                }
                catch
                {
                    Debug.WriteLine(e.Y);
                    tip = false;
                }

            }

            labelTip.Visible = tip;


        }

        private void AdjustAllLevels(int index, double curXVal, double curYVal, Series series)
        {

            // Get the neighboring DataPoints of the hit point
            DataPoint upperPoint = null;
            DataPoint lowerPoint = null;

            if (index > 0)
            {
                lowerPoint = series.Points[index - 1];
            }

            if (index < series.Points.Count - 1)
            {
                upperPoint = series.Points[index + 1];
            }

            // Adjust the values according to the comparison between the value and its neighbors
            if (upperPoint != null)
            {
                if (curYVal > upperPoint.YValues[0])
                {

                    for (int i = index + 1; i < series.Points.Count; i++)
                    {
                        DataPoint curUpper = series.Points[i];
                        if (curUpper.YValues[0] >= curYVal) break;

                        curUpper.YValues[0] = curYVal;
                    }
                }
                if (curXVal > upperPoint.XValue)
                {

                    for (int i = index + 1; i < series.Points.Count; i++)
                    {
                        DataPoint curUpper = series.Points[i];
                        if (curUpper.XValue >= curXVal) break;

                        curUpper.XValue = curXVal;
                    }
                }
            }

            if (lowerPoint != null)
            {
                //Debug.WriteLine(curYVal + " <? " + Math.Floor(lowerPoint.YValues[0]));
                if (curYVal <= Math.Floor(lowerPoint.YValues[0]))
                {
                    for (int i = index - 1; i >= 0; i--)
                    {
                        DataPoint curLower = series.Points[i];
                        if (curLower.YValues[0] < curYVal) break;
                        curLower.YValues[0] = Math.Floor(curYVal);
                    }
                }
                if (curXVal < lowerPoint.XValue)
                {

                    for (int i = index - 1; i >= 0; i--)
                    {
                        DataPoint curLower = series.Points[i];
                        if (curLower.XValue <= curXVal) break;

                        curLower.XValue = curXVal;
                    }
                }
            }
        }

    }

}
