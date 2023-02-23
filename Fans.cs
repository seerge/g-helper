using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms.Design;

namespace GHelper
{
    public partial class Fans : Form
    {

        static Color colorbackgroundDark = Color.FromArgb(255, 25, 25, 25);
        static Color colorDark = Color.FromArgb(255, 50, 50, 50);
        static Color colorGray = Color.FromArgb(255, 75, 75, 75);

        DataPoint curPoint = null;
        Series seriesCPU;
        Series seriesGPU;

        // Get dark mode status from Windows API
        [DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#138")]
        public static extern bool CheckSystemDarkModeStatus();

        bool ThemeMode = CheckSystemDarkModeStatus();

        // Setup to change taskbar to dark with Windows API (Windows Forms limitation workaround from 
        // https://lostintheframework.blogspot.com/2020/10/creating-dark-mode-for-windows-forms.html)
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr,
    ref int attrValue, int attrSize);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        internal static bool UseImmersiveDarkMode(IntPtr handle, bool enabled)
        {
            if (IsWindows10OrGreater(17763))
            {
                var attribute = DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;
                if (IsWindows10OrGreater(18985))
                {
                    attribute = DWMWA_USE_IMMERSIVE_DARK_MODE;
                }

                int useImmersiveDarkMode = enabled ? 1 : 0;
                return DwmSetWindowAttribute(handle, (int)attribute,
                    ref useImmersiveDarkMode, sizeof(int)) == 0;
            }

            return false;
        }

        private static bool IsWindows10OrGreater(int build = -1)
        {
            return Environment.OSVersion.Version.Major >= 10 &&
                Environment.OSVersion.Version.Build >= build;
        }

        void SetChart(Chart chart, string title)
        {
            chart.Titles.Add(title);
            chart.ChartAreas[0].AxisX.Minimum = 10;
            chart.ChartAreas[0].AxisX.Maximum = 100;
            chart.ChartAreas[0].AxisX.Interval = 10;
            chart.ChartAreas[0].AxisY.Minimum = 0;
            chart.ChartAreas[0].AxisY.Maximum = 100;
            chart.Legends[0].Enabled = false;

        }

        public Fans()
        {
            InitializeComponent();

            SetChart(chartCPU, "CPU Fan Profile");
            SetChart(chartGPU, "GPU Fan Profile");

            seriesCPU = chartCPU.Series.Add("CPU");
            seriesGPU = chartGPU.Series.Add("GPU");

            seriesCPU.Color = Color.Blue;
            seriesGPU.Color = Color.Red;

            LoadFans();


            if (ThemeMode == true)
            {
                UseImmersiveDarkMode(this.Handle, true);
                BackColor = colorbackgroundDark;
                ForeColor = Color.White;

                chartCPU.BackColor = colorDark;
                chartCPU.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.White;
                chartCPU.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.White;
                chartCPU.Titles[0].ForeColor = Color.White;

                chartGPU.BackColor = colorDark;
                chartGPU.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.White;
                chartGPU.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.White;
                chartGPU.Titles[0].ForeColor = Color.White;

                buttonReset.BackColor = colorDark;
                buttonReset.ForeColor = Color.White;

                buttonApply.BackColor = colorDark;
                buttonApply.ForeColor = Color.White;
            }

            chartCPU.MouseMove += ChartCPU_MouseMove;
            chartCPU.MouseUp += ChartCPU_MouseUp;

            chartGPU.MouseMove += ChartCPU_MouseMove;
            chartGPU.MouseUp += ChartCPU_MouseUp;

            buttonReset.Click += ButtonReset_Click;
            buttonApply.Click += ButtonApply_Click;

        }

        public void LoadFans()
        {
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
            byte[] curve = {};

            if (curveString is not null)
                curve = StringToBytes(curveString);

            if (def == 1 || curve.Length < 16)
                curve = Program.wmi.GetFanCurve(device, mode);

            
            Debug.WriteLine(BitConverter.ToString(curve));

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
                    double dx = ax.PixelPositionToValue(e.X);
                    double dy = ay.PixelPositionToValue(e.Y);

                    if (dx < 0) dx = 0;
                    if (dx > 100) dx = 100;

                    if (dy < 0) dy = 0;
                    if (dy > 100) dy = 100;

                    curPoint.XValue = dx;
                    curPoint.YValues[0] = dy;
                }
            }
        }
    }

}
