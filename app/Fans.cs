using System.Diagnostics;
using System.Windows.Forms.DataVisualization.Charting;
using CustomControls;

namespace GHelper
{
    public partial class Fans : RForm
    {

        DataPoint curPoint = null;
        Series seriesCPU;
        Series seriesGPU;
        Series seriesMid;

        static int MinRPM, MaxRPM;

        static string ChartPercToRPM(int percentage, string unit = "")
        {
            if (percentage == 0) return "OFF";

            return (200 * Math.Round((float)(MinRPM*100 + (MaxRPM - MinRPM) * percentage) / 200)).ToString() + unit;
        }

        void SetChart(Chart chart, int device)
        {

            string title;

            if (device == 1)
                title = "GPU Fan Profile";
            else if (device == 2)
                title = "Middle Fan Profile";
            else
                title = "CPU Fan Profile";

            if (Program.settingsForm.perfName.Length > 0)
                labelFans.Text = "Fan Profiles: " + Program.settingsForm.perfName;

            chart.Titles[0].Text = title;

            chart.ChartAreas[0].AxisX.Minimum = 10;
            chart.ChartAreas[0].AxisX.Maximum = 100;
            chart.ChartAreas[0].AxisX.Interval = 10;

            chart.ChartAreas[0].AxisY.Minimum = 0;
            chart.ChartAreas[0].AxisY.Maximum = 100;

            chart.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Arial", 7F);

            var foreAccent = Color.FromArgb(255, 180, 180, 180);

            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = foreAccent;
            chart.ChartAreas[0].AxisY.MajorGrid.LineColor = foreAccent;
            chart.ChartAreas[0].AxisX.LineColor = foreAccent;
            chart.ChartAreas[0].AxisY.LineColor = foreAccent;

            for (int i = 0; i <= 90; i += 10)
                chart.ChartAreas[0].AxisY.CustomLabels.Add(i - 2, i + 2, ChartPercToRPM(i));

            chart.ChartAreas[0].AxisY.CustomLabels.Add(98, 102, "RPM");

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

        public Fans()
        {

            InitializeComponent();
            InitTheme();

            MinRPM = 18;
            MaxRPM = HardwareMonitor.GetFanMax();



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
            buttonApply.Click += ButtonApply_Click;

            trackTotal.Maximum = ASUSWmi.MaxTotal;
            trackTotal.Minimum = ASUSWmi.MinTotal;

            trackCPU.Maximum = ASUSWmi.MaxCPU;
            trackCPU.Minimum = ASUSWmi.MinCPU;

            trackCPU.Scroll += TrackCPU_Scroll;
            trackTotal.Scroll += TrackTotal_Scroll;

            buttonApplyPower.Click += ButtonApplyPower_Click;

            checkAuto.Click += CheckAuto_Click;
            checkApplyPower.Click += CheckApplyPower_Click;

            //labelInfo.MaximumSize = new Size(280, 0);
            labelInfo.Text = "Power Limits (PPT) is\nexperimental feature.\n\nUse carefully and\non your own risk!";

            InitFans();
            InitPower();
            InitBoost();

            comboBoost.SelectedValueChanged += ComboBoost_Changed;

            Shown += Fans_Shown;

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
        }

        private void CheckAuto_Click(object? sender, EventArgs e)
        {
            if (sender is null) return;
            CheckBox chk = (CheckBox)sender;
            Program.config.setConfigPerf("auto_apply", chk.Checked ? 1 : 0);
        }

        private void Fans_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void ButtonApplyPower_Click(object? sender, EventArgs e)
        {
            Program.settingsForm.SetPower();
            ApplyLabel(true);
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
                label1.Text = "CPU SPPT";
            }

            int limit_total;
            int limit_cpu;
            bool apply = Program.config.getConfigPerf("auto_apply_power") == 1;

            if (changed)
            {
                limit_total = trackTotal.Value;
                limit_cpu = trackCPU.Value;
                ApplyLabel(false);
            }
            else
            {
                limit_total = Program.config.getConfigPerf("limit_total");
                limit_cpu = Program.config.getConfigPerf("limit_cpu");
                ApplyLabel(apply);
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
            pictureFine.Visible = (limit_cpu > 85 || limit_total > 145);

            Program.config.setConfigPerf("limit_total", limit_total);
            Program.config.setConfigPerf("limit_cpu", limit_cpu);
        }


        private void TrackTotal_Scroll(object? sender, EventArgs e)
        {
            InitPower(true);
        }

        private void TrackCPU_Scroll(object? sender, EventArgs e)
        {
            InitPower(true);
        }


        public void ApplyLabel(bool applied = false)
        {
            if (applied)
            {
                labelApplied.ForeColor = colorStandard;
                labelApplied.Text = "Applied";
            }
            else
            {
                labelApplied.ForeColor = colorTurbo;
                labelApplied.Text = "Not Applied";

            }
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

            checkAuto.Checked = (auto_apply == 1);

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


        private void ButtonApply_Click(object? sender, EventArgs e)
        {
            SaveProfile(seriesCPU, 0);
            SaveProfile(seriesGPU, 1);
            if (Program.config.getConfig("mid_fan") == 1)
                SaveProfile(seriesMid, 2);

            Program.settingsForm.AutoFans(true);
        }

        private void ButtonReset_Click(object? sender, EventArgs e)
        {

            LoadProfile(seriesCPU, 0, 1);
            LoadProfile(seriesGPU, 1, 1);
            if (Program.config.getConfig("mid_fan") == 1)
                LoadProfile(seriesMid, 2, 1);

            checkAuto.Checked = false;
            checkApplyPower.Checked = false;

            Program.config.setConfigPerf("auto_apply", 0);
            Program.config.setConfigPerf("auto_apply_power", 0);

            Program.wmi.DeviceSet(ASUSWmi.PerformanceMode, Program.config.getConfig("performance_mode"), "PerfMode");

            ApplyLabel(false);
        }

        private void ChartCPU_MouseUp(object? sender, MouseEventArgs e)
        {
            curPoint = null;
            labelTip.Visible = false;
        }

        private void ChartCPU_MouseMove(object? sender, MouseEventArgs e)
        {

            if (sender is null) return;

            Chart chart = (Chart)sender;

            if (e.Button.HasFlag(MouseButtons.Left))
            {
                ChartArea ca = chart.ChartAreas[0];
                Axis ax = ca.AxisX;
                Axis ay = ca.AxisY;

                HitTestResult hit = chart.HitTest(e.X, e.Y);
                if (hit.Series is not null && hit.PointIndex >= 0)
                    curPoint = hit.Series.Points[hit.PointIndex];


                if (curPoint != null)
                {

                    Series s = hit.Series;
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

                        curPoint.XValue = dx;
                        curPoint.YValues[0] = dy;

                        labelTip.Visible = true;
                        labelTip.Text = Math.Round(dx) + "C, " + ChartPercToRPM((int)dy, " RPM");
                        labelTip.Top = e.Y + ((Control)sender).Top;
                        labelTip.Left = e.X;

                    }
                    catch
                    {
                        Debug.WriteLine(e.Y);
                    }
                }
            }
        }
    }

}
