using System;
using System.Diagnostics;
using System.Windows.Forms.DataVisualization.Charting;

namespace GHelper
{
    public partial class Fans : Form
    {

        DataPoint curPoint = null;
        Series seriesCPU;
        Series seriesGPU;

        const int MaxTotal = 150;
        const int MinTotal = 15;
        const int DefaultTotal = 125;

        const int MaxCPU = 90;
        const int MinCPU = 15;
        const int DefaultCPU = 80;

        void SetChart(Chart chart, int device)
        {

            string title;

            if (device == 1)
                title = "GPU Fan Profile";
            else
                title = "CPU Fan Profile";

            if (Program.settingsForm.perfName.Length > 0)
                title += ": " + Program.settingsForm.perfName;

            if (chart.Titles.Count > 0)
                chart.Titles[0].Text = title;
            else
                chart.Titles.Add(title);

            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
            chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;

            chart.ChartAreas[0].AxisX.Minimum = 10;
            chart.ChartAreas[0].AxisX.Maximum = 100;
            chart.ChartAreas[0].AxisX.Interval = 10;
            chart.ChartAreas[0].AxisY.Minimum = 0;
            chart.ChartAreas[0].AxisY.Maximum = 100;

            chart.ChartAreas[0].AxisX.Interval = 10;
            chart.ChartAreas[0].AxisY.Interval = 10;

            if (chart.Legends.Count > 0)
                chart.Legends[0].Enabled = false;

        }

        private void Fans_Shown(object? sender, EventArgs e)
        {
            Top = Program.settingsForm.Top;
            Left = Program.settingsForm.Left - Width - 5;
        }

        public Fans()
        {

            InitializeComponent();

            seriesCPU = chartCPU.Series.Add("CPU");
            seriesGPU = chartGPU.Series.Add("GPU");

            seriesCPU.Color = Color.Blue;
            seriesGPU.Color = Color.Red;

            LoadFans();

            chartCPU.MouseMove += ChartCPU_MouseMove;
            chartCPU.MouseUp += ChartCPU_MouseUp;

            chartGPU.MouseMove += ChartCPU_MouseMove;
            chartGPU.MouseUp += ChartCPU_MouseUp;

            buttonReset.Click += ButtonReset_Click;
            buttonApply.Click += ButtonApply_Click;

            trackTotal.Maximum = MaxTotal;
            trackTotal.Minimum = MinTotal;

            trackCPU.Maximum = MaxCPU;
            trackCPU.Minimum = MinCPU;

            trackCPU.Scroll += TrackCPU_Scroll;
            trackTotal.Scroll += TrackTotal_Scroll;

            buttonApplyPower.Click += ButtonApplyPower_Click;

            labelInfo.MaximumSize = new Size(300, 0);
            labelInfo.Text = "Power Limits (PPT) is experimental feature.\n\nValues will be applied only after you click 'Apply' and reset after performance mode change.\n\nUse carefully and on your own risk!";

            VisualisePower(true);

            Shown += Fans_Shown;

        }

        private void ButtonApplyPower_Click(object? sender, EventArgs e)
        {
            int limit_total = trackTotal.Value;
            int limit_cpu = trackCPU.Value;

            Program.config.setConfig("limit_total", limit_total);
            Program.config.setConfig("limit_cpu", limit_cpu);

            Program.wmi.DeviceSet(ASUSWmi.PPT_Total, limit_total);
            Program.wmi.DeviceSet(ASUSWmi.PPT_CPU, limit_cpu);

        }

        public void VisualisePower(bool init = false)
        {

            int limit_total;
            int limit_cpu;

            if (init)
            {
                limit_total = Program.config.getConfig("limit_total");
                limit_cpu = Program.config.getConfig("limit_cpu");
            }
            else
            {
                limit_total = trackTotal.Value;
                limit_cpu = trackCPU.Value;
            }

            if (limit_total < 0) limit_total = DefaultTotal;
            if (limit_total > MaxTotal) limit_total = MaxTotal;
            if (limit_total < MinTotal) limit_total = MinTotal;

            if (limit_cpu < 0) limit_cpu = DefaultCPU;
            if (limit_cpu > MaxCPU) limit_cpu = MaxCPU;
            if (limit_cpu < MinCPU) limit_cpu = MinCPU;

            if (limit_cpu > limit_total) limit_cpu = limit_total;

            trackTotal.Value = limit_total;
            trackCPU.Value = limit_cpu;

            labelTotal.Text = trackTotal.Value.ToString() + "W";
            labelCPU.Text = trackCPU.Value.ToString() + "W";

            pictureFine.Visible = (limit_cpu > 85 || limit_total > 145);
        }

        private void TrackTotal_Scroll(object? sender, EventArgs e)
        {
            VisualisePower();
        }

        private void TrackCPU_Scroll(object? sender, EventArgs e)
        {
            VisualisePower();
        }

        public void LoadFans()
        {

            SetChart(chartCPU, 0);
            SetChart(chartGPU, 1);

            LoadProfile(seriesCPU, 0);
            LoadProfile(seriesGPU, 1);

        }

        byte[] StringToBytes(string str)
        {
            String[] arr = str.Split('-');
            byte[] array = new byte[arr.Length];
            for (int i = 0; i < arr.Length; i++) array[i] = Convert.ToByte(arr[i], 16);
            return array;
        }

        string GetFanName(int device)
        {
            int mode = Program.config.getConfig("performance_mode");
            string name;

            if (device == 1)
                name = "gpu";
            else
                name = "cpu";

            return "fan_profile_" + name + "_" + mode;
        }

        void LoadProfile(Series series, int device, int def = 0)
        {

            series.ChartType = SeriesChartType.Line;
            series.MarkerSize = 10;
            series.MarkerStyle = MarkerStyle.Circle;

            series.Points.Clear();

            int mode = Program.config.getConfig("performance_mode");
            string curveString = Program.config.getConfigString(GetFanName(device));
            byte[] curve = { };

            if (curveString is not null)
                curve = StringToBytes(curveString);

            if (def == 1 || curve.Length != 16)
                curve = Program.wmi.GetFanCurve(device, mode);

            if (curve.All(singleByte => singleByte == 0))
            {
                switch (mode)
                {
                    case 1:
                        if (device == 1)
                            curve = StringToBytes("14-3F-44-48-4C-50-54-62-16-1F-26-2D-39-47-55-5F");
                        else
                            curve = StringToBytes("14-3F-44-48-4C-50-54-62-11-1A-22-29-34-43-51-5A");
                        break;
                    case 2:
                        if (device == 1)
                            curve = StringToBytes("3C-41-42-46-47-4B-4C-62-08-11-11-1D-1D-26-26-2D");
                        else
                            curve = StringToBytes("3C-41-42-46-47-4B-4C-62-03-0C-0C-16-16-22-22-29");
                        break;
                    default:
                        if (device == 1)
                            curve = StringToBytes("3A-3D-40-44-48-4D-51-62-0C-16-1D-1F-26-2D-34-4A");
                        else
                            curve = StringToBytes("3A-3D-40-44-48-4D-51-62-08-11-16-1A-22-29-30-45");
                        break;
                }

            }



            //Debug.WriteLine(BitConverter.ToString(curve));

            byte old = 0;
            for (int i = 0; i < 8; i++)
            {
                if (curve[i] == old) curve[i]++; // preventing 2 points in same spot from default asus profiles
                series.Points.AddXY(curve[i], curve[i + 8]);
                old = curve[i];
            }

        }

        void ApplyProfile(Series series, int device)
        {
            byte[] curve = new byte[16];
            int i = 0;
            foreach (DataPoint point in series.Points)
            {
                curve[i] = (byte)point.XValue;
                curve[i + 8] = (byte)point.YValues.First();
                i++;
            }

            string bitCurve = BitConverter.ToString(curve);
            Debug.WriteLine(bitCurve);
            Program.config.setConfig(GetFanName(device), bitCurve);

            Program.wmi.SetFanCurve(device, curve);

        }


        private void ButtonApply_Click(object? sender, EventArgs e)
        {
            ApplyProfile(seriesCPU, 0);
            ApplyProfile(seriesGPU, 1);
        }

        private void ButtonReset_Click(object? sender, EventArgs e)
        {
            LoadProfile(seriesCPU, 0, 1);
            LoadProfile(seriesGPU, 1, 1);
            Program.wmi.DeviceSet(ASUSWmi.PerformanceMode, Program.config.getConfig("performance_mode"));
        }

        private void ChartCPU_MouseUp(object? sender, MouseEventArgs e)
        {
            curPoint = null;
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

                        dymin = (dx - 60) * 1.2;

                        if (dy < dymin) dy = dymin;

                        curPoint.XValue = dx;
                        curPoint.YValues[0] = dy;

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
