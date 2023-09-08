using GHelper.Gpu.NVidia;
using GHelper.Mode;
using GHelper.UI;
using Ryzen;
using System.Diagnostics;
using System.Windows.Forms.DataVisualization.Charting;

namespace GHelper
{
    public partial class Fans : RForm
    {

        int curIndex = -1;
        DataPoint? curPoint = null;

        Series seriesCPU;
        Series seriesGPU;
        Series seriesMid;
        Series seriesXGM;

        static bool gpuVisible = true;
        static bool fanRpm = true;

        const int fansMax = 100;

        NvidiaGpuControl? nvControl = null;
        ModeControl modeControl = Program.modeControl;

        public static System.Timers.Timer timer = default!;

        public Fans()
        {

            InitializeComponent();

            //float dpi = ControlHelper.GetDpiScale(this).Value;
            //comboModes.Size = new Size(comboModes.Width, (int)dpi * 18);
            comboModes.ClientSize = new Size(comboModes.Width, comboModes.Height - 4);

            Text = Properties.Strings.FansAndPower;
            labelPowerLimits.Text = Properties.Strings.PowerLimits;
            checkApplyPower.Text = Properties.Strings.ApplyPowerLimits;

            labelFans.Text = Properties.Strings.FanCurves;
            labelBoost.Text = Properties.Strings.CPUBoost;
            buttonReset.Text = Properties.Strings.FactoryDefaults;
            checkApplyFans.Text = Properties.Strings.ApplyFanCurve;

            labelGPU.Text = Properties.Strings.GPUSettings;

            labelGPUCoreTitle.Text = Properties.Strings.GPUCoreClockOffset;
            labelGPUMemoryTitle.Text = Properties.Strings.GPUMemoryClockOffset;
            labelGPUBoostTitle.Text = Properties.Strings.GPUBoost;
            labelGPUTempTitle.Text = Properties.Strings.GPUTempTarget;

            labelRisky.Text = Properties.Strings.UndervoltingRisky;
            buttonApplyAdvanced.Text = Properties.Strings.Apply;
            checkApplyUV.Text = Properties.Strings.AutoApply;

            buttonCalibrate.Text = Properties.Strings.Calibrate;

            InitTheme(true);

            labelTip.Visible = false;
            labelTip.BackColor = Color.Transparent;

            FormClosing += Fans_FormClosing;

            seriesCPU = chartCPU.Series.Add("CPU");
            seriesGPU = chartGPU.Series.Add("GPU");
            seriesMid = chartMid.Series.Add("Mid");
            seriesXGM = chartXGM.Series.Add("XGM");

            seriesCPU.Color = colorStandard;
            seriesGPU.Color = colorTurbo;
            seriesMid.Color = colorEco;
            seriesXGM.Color = Color.Orange;

            chartCPU.MouseMove += (sender, e) => ChartCPU_MouseMove(sender, e, AsusFan.CPU);
            chartCPU.MouseUp += ChartCPU_MouseUp;
            chartCPU.MouseLeave += ChartCPU_MouseLeave;

            chartGPU.MouseMove += (sender, e) => ChartCPU_MouseMove(sender, e, AsusFan.GPU);
            chartGPU.MouseUp += ChartCPU_MouseUp;
            chartGPU.MouseLeave += ChartCPU_MouseLeave;

            chartMid.MouseMove += (sender, e) => ChartCPU_MouseMove(sender, e, AsusFan.Mid);
            chartMid.MouseUp += ChartCPU_MouseUp;
            chartMid.MouseLeave += ChartCPU_MouseLeave;

            chartXGM.MouseMove += (sender, e) => ChartCPU_MouseMove(sender, e, AsusFan.XGM);
            chartXGM.MouseUp += ChartCPU_MouseUp;
            chartXGM.MouseLeave += ChartCPU_MouseLeave;

            chartCPU.MouseClick += ChartCPU_MouseClick;
            chartGPU.MouseClick += ChartCPU_MouseClick;
            chartMid.MouseClick += ChartCPU_MouseClick;
            chartXGM.MouseClick += ChartCPU_MouseClick;

            buttonReset.Click += ButtonReset_Click;

            trackA0.Maximum = AsusACPI.MaxTotal;
            trackA0.Minimum = AsusACPI.MinTotal;

            trackB0.Maximum = AsusACPI.MaxCPU;
            trackB0.Minimum = AsusACPI.MinCPU;

            trackC1.Maximum = AsusACPI.MaxTotal;
            trackC1.Minimum = AsusACPI.MinTotal;

            trackC1.Scroll += TrackPower_Scroll;
            trackB0.Scroll += TrackPower_Scroll;
            trackA0.Scroll += TrackPower_Scroll;

            trackC1.MouseUp += TrackPower_MouseUp;
            trackB0.MouseUp += TrackPower_MouseUp;
            trackA0.MouseUp += TrackPower_MouseUp;

            checkApplyFans.Click += CheckApplyFans_Click;
            checkApplyPower.Click += CheckApplyPower_Click;

            trackGPUClockLimit.Minimum = NvidiaGpuControl.MinClockLimit;
            trackGPUClockLimit.Maximum = NvidiaGpuControl.MaxClockLimit;

            trackGPUCore.Minimum = NvidiaGpuControl.MinCoreOffset;
            trackGPUCore.Maximum = NvidiaGpuControl.MaxCoreOffset;

            trackGPUMemory.Minimum = NvidiaGpuControl.MinMemoryOffset;
            trackGPUMemory.Maximum = NvidiaGpuControl.MaxMemoryOffset;

            trackGPUBoost.Minimum = AsusACPI.MinGPUBoost;
            trackGPUBoost.Maximum = AsusACPI.MaxGPUBoost;

            trackGPUTemp.Minimum = AsusACPI.MinGPUTemp;
            trackGPUTemp.Maximum = AsusACPI.MaxGPUTemp;

            trackGPUClockLimit.Scroll += trackGPUClockLimit_Scroll;
            trackGPUCore.Scroll += trackGPU_Scroll;
            trackGPUMemory.Scroll += trackGPU_Scroll;

            trackGPUBoost.Scroll += trackGPUPower_Scroll;
            trackGPUTemp.Scroll += trackGPUPower_Scroll;

            trackGPUCore.MouseUp += TrackGPU_MouseUp;
            trackGPUMemory.MouseUp += TrackGPU_MouseUp;
            trackGPUBoost.MouseUp += TrackGPU_MouseUp;
            trackGPUTemp.MouseUp += TrackGPU_MouseUp;

            trackGPUClockLimit.MouseUp += TrackGPU_MouseUp;

            //labelInfo.MaximumSize = new Size(280, 0);
            labelFansResult.Visible = false;


            trackUV.Minimum = RyzenControl.MinCPUUV;
            trackUV.Maximum = RyzenControl.MaxCPUUV;

            trackUViGPU.Minimum = RyzenControl.MinIGPUUV;
            trackUViGPU.Maximum = RyzenControl.MaxIGPUUV;

            trackTemp.Minimum = RyzenControl.MinTemp;
            trackTemp.Maximum = RyzenControl.MaxTemp;

            comboPowerMode.DropDownStyle = ComboBoxStyle.DropDownList;
            comboPowerMode.DataSource = new BindingSource(PowerNative.powerModes, null);
            comboPowerMode.DisplayMember = "Value";
            comboPowerMode.ValueMember = "Key";

            FillModes();
            InitAll();

            comboBoost.SelectedValueChanged += ComboBoost_Changed;
            comboPowerMode.SelectedValueChanged += ComboPowerMode_Changed;


            comboModes.SelectionChangeCommitted += ComboModes_SelectedValueChanged;
            comboModes.TextChanged += ComboModes_TextChanged;
            comboModes.KeyPress += ComboModes_KeyPress;

            Shown += Fans_Shown;

            buttonAdd.Click += ButtonAdd_Click;
            buttonRemove.Click += ButtonRemove_Click;
            buttonRename.Click += ButtonRename_Click;


            trackUV.Scroll += TrackUV_Scroll;
            trackUViGPU.Scroll += TrackUV_Scroll;
            trackTemp.Scroll += TrackUV_Scroll;

            buttonApplyAdvanced.Click += ButtonApplyAdvanced_Click;

            buttonCPU.BorderColor = colorStandard;
            buttonGPU.BorderColor = colorTurbo;
            buttonAdvanced.BorderColor = Color.Gray;

            buttonCPU.Click += ButtonCPU_Click;
            buttonGPU.Click += ButtonGPU_Click;
            buttonAdvanced.Click += ButtonAdvanced_Click;

            checkApplyUV.Click += CheckApplyUV_Click;

            timer = new System.Timers.Timer(1000);
            timer.Elapsed += Timer_Elapsed;
            buttonCalibrate.Click += ButtonCalibrate_Click;

            ToggleNavigation(0);

            FormClosed += Fans_FormClosed;

        }

        const int FAN_COUNT = 3;
        static int[] fanMax;
        static int sameCount = 0;

        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            int fan;
            bool same = true;

            for (int i = 0; i < FAN_COUNT; i++)
            {
                fan = Program.acpi.GetFan((AsusFan)i);
                if (fan > fanMax[i])
                {
                    fanMax[i] = fan;
                    same = false;
                }
            }

            if (same) sameCount++;
            else sameCount = 0;

            LabelFansResult("Max Speed - CPU: " + fanMax[(int)AsusFan.CPU] * 100 + ", GPU: " + fanMax[(int)AsusFan.GPU] * 100 + " (" + sameCount + "s)");

            if (sameCount >= 15)
            {
                for (int i = 0; i < FAN_COUNT; i++)
                {
                    if (fanMax[i] > 30 && fanMax[i] < 80) AppConfig.Set("fan_max_" + i, fanMax[i]);
                }

                sameCount = 0;
                CalibrateNext();
            }

        }

        private void CalibrateNext()
        {

            timer.Enabled = false;
            modeControl.SetPerformanceMode();

            string label = "Measured - CPU: " + AppConfig.Get("fan_max_" + (int)AsusFan.CPU) * 100;

            if (AppConfig.Get("fan_max_" + (int)AsusFan.GPU) > 0)
                label = label + ", GPU: " + AppConfig.Get("fan_max_" + (int)AsusFan.GPU) * 100;

            if (AppConfig.Get("fan_max_" + (int)AsusFan.Mid) > 0)
                label = label + ", Mid: " + AppConfig.Get("fan_max_" + (int)AsusFan.Mid) * 100;

            LabelFansResult(label);

            Invoke(delegate
            {
                buttonCalibrate.Enabled = true;
                SetAxis(chartCPU, AsusFan.CPU);
                SetAxis(chartGPU, AsusFan.GPU);
                if (chartMid.Visible) SetAxis(chartMid, AsusFan.Mid);

            });

        }

        private void ButtonCalibrate_Click(object? sender, EventArgs e)
        {
            fanMax = new int[] { 0, 0, 0 };

            buttonCalibrate.Enabled = false;
            timer.Enabled = true;

            for (int i = 0; i < FAN_COUNT; i++)
            {
                AppConfig.Remove("fan_max_" + i);
            }

            Program.acpi.DeviceSet(AsusACPI.PerformanceMode, AsusACPI.PerformanceTurbo, "ModeCalibration");
            
            for (int i = 0; i < FAN_COUNT; i++)
                Program.acpi.SetFanCurve((AsusFan)i, new byte[] { 20, 30, 40, 50, 60, 70, 80, 90, 100, 100, 100, 100, 100, 100, 100, 100 });
        }

        private void ChartCPU_MouseClick(object? sender, MouseEventArgs e)
        {
            if (sender is null) return;
            Chart chart = (Chart)sender;

            HitTestResult result = chart.HitTest(e.X, e.Y);

            if ((result.ChartElementType == ChartElementType.AxisLabels || result.ChartElementType == ChartElementType.Axis) && result.Axis == chart.ChartAreas[0].AxisY)
            {
                fanRpm = !fanRpm;
                SetAxis(chartCPU, AsusFan.CPU);
                SetAxis(chartGPU, AsusFan.GPU);
                if (chartMid.Visible) SetAxis(chartMid, AsusFan.Mid);
                if (chartXGM.Visible) SetAxis(chartXGM, AsusFan.XGM);
            }

        }

        private void Fans_FormClosed(object? sender, FormClosedEventArgs e)
        {
            //Because windows charts seem to eat a lot of memory :(
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
        }

        private void CheckApplyUV_Click(object? sender, EventArgs e)
        {
            AppConfig.SetMode("auto_uv", checkApplyUV.Checked ? 1 : 0);
            modeControl.AutoRyzen();
        }

        public void InitAll()
        {
            InitMode();
            InitFans();
            InitPower();
            InitPowerPlan();
            InitUV();
            InitGPU();
        }

        public void ToggleNavigation(int index = 0)
        {

            SuspendLayout();

            buttonCPU.Activated = false;
            buttonGPU.Activated = false;
            buttonAdvanced.Activated = false;

            panelPower.Visible = false;
            panelGPU.Visible = false;
            panelAdvanced.Visible = false;

            switch (index)
            {
                case 1:
                    buttonGPU.Activated = true;
                    panelGPU.Visible = true;
                    break;
                case 2:
                    buttonAdvanced.Activated = true;
                    panelAdvanced.Visible = true;
                    break;
                default:
                    buttonCPU.Activated = true;
                    panelPower.Visible = true;
                    break;
            }

            ResumeLayout(false);
            PerformLayout();
        }

        private void ButtonAdvanced_Click(object? sender, EventArgs e)
        {
            ToggleNavigation(2);
        }

        private void ButtonGPU_Click(object? sender, EventArgs e)
        {
            ToggleNavigation(1);
        }

        private void ButtonCPU_Click(object? sender, EventArgs e)
        {
            ToggleNavigation(0);
        }

        private void ButtonApplyAdvanced_Click(object? sender, EventArgs e)
        {
            modeControl.SetRyzen(true);
            checkApplyUV.Enabled = true;
        }

        public void InitUV()
        {

            //if (!ProcessHelper.IsUserAdministrator()) return;

            int cpuUV = Math.Max(trackUV.Minimum, Math.Min(trackUV.Maximum, AppConfig.GetMode("cpu_uv", 0)));
            int igpuUV = Math.Max(trackUViGPU.Minimum, Math.Min(trackUViGPU.Maximum, AppConfig.GetMode("igpu_uv", 0)));

            int temp = AppConfig.GetMode("cpu_temp");
            if (temp < RyzenControl.MinTemp || temp > RyzenControl.MaxTemp) temp = RyzenControl.MaxTemp;

            checkApplyUV.Enabled = checkApplyUV.Checked = AppConfig.IsMode("auto_uv");

            trackUV.Value = cpuUV;
            trackUViGPU.Value = igpuUV;
            trackTemp.Value = temp;

            VisualiseAdvanced();

            buttonAdvanced.Visible = RyzenControl.IsAMD();

        }

        private void VisualiseAdvanced()
        {
            if (!RyzenControl.IsSupportedUV())
            {
                panelTitleAdvanced.Visible = false;
                labelRisky.Visible = false;
                panelUV.Visible = false;
                panelUViGPU.Visible = false;
            }

            if (!RyzenControl.IsSupportedUViGPU())
            {
                panelUViGPU.Visible = false;
            }

            labelUV.Text = trackUV.Value.ToString();
            labelUViGPU.Text = trackUViGPU.Value.ToString();
            labelTemp.Text = (trackTemp.Value < RyzenControl.MaxTemp) ? trackTemp.Value.ToString() + "°C" : "Default";
        }

        private void AdvancedScroll()
        {
            AppConfig.SetMode("auto_uv", 0);
            checkApplyUV.Enabled = checkApplyUV.Checked = false;

            VisualiseAdvanced();

            AppConfig.SetMode("cpu_temp", trackTemp.Value);
            AppConfig.SetMode("cpu_uv", trackUV.Value);
            AppConfig.SetMode("igpu_uv", trackUViGPU.Value);
        }


        private void TrackUV_Scroll(object? sender, EventArgs e)
        {
            AdvancedScroll();
        }

        private void ComboModes_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) RenameToggle();
        }

        private void ComboModes_TextChanged(object? sender, EventArgs e)
        {
            if (comboModes.DropDownStyle == ComboBoxStyle.DropDownList) return;
            if (!Modes.IsCurrentCustom()) return;
            AppConfig.SetMode("mode_name", comboModes.Text);
        }

        private void RenameToggle()
        {
            if (comboModes.DropDownStyle == ComboBoxStyle.DropDownList)
                comboModes.DropDownStyle = ComboBoxStyle.Simple;
            else
            {
                var mode = Modes.GetCurrent();
                FillModes();
                comboModes.SelectedValue = mode;
            }
        }

        private void ButtonRename_Click(object? sender, EventArgs e)
        {
            RenameToggle();
        }

        private void ButtonRemove_Click(object? sender, EventArgs e)
        {
            int mode = Modes.GetCurrent();
            if (!Modes.IsCurrentCustom()) return;

            Modes.Remove(mode);
            FillModes();

            modeControl.SetPerformanceMode(AsusACPI.PerformanceBalanced);

        }

        private void FillModes()
        {
            comboModes.DropDownStyle = ComboBoxStyle.DropDownList;
            comboModes.DataSource = new BindingSource(Modes.GetDictonary(), null);
            comboModes.DisplayMember = "Value";
            comboModes.ValueMember = "Key";
        }

        private void ButtonAdd_Click(object? sender, EventArgs e)
        {
            int mode = Modes.Add();
            FillModes();
            modeControl.SetPerformanceMode(mode);
        }

        public void InitMode()
        {
            int mode = Modes.GetCurrent();
            comboModes.SelectedValue = mode;
            buttonRename.Visible = buttonRemove.Visible = Modes.IsCurrentCustom();
        }

        private void ComboModes_SelectedValueChanged(object? sender, EventArgs e)
        {
            var selectedMode = comboModes.SelectedValue;

            if (selectedMode == null) return;
            if ((int)selectedMode == Modes.GetCurrent()) return;

            Debug.WriteLine(selectedMode);

            modeControl.SetPerformanceMode((int)selectedMode);
        }

        private void TrackGPU_MouseUp(object? sender, MouseEventArgs e)
        {
            modeControl.SetGPUPower();
            modeControl.SetGPUClocks(true);
        }

        public void InitGPU()
        {

            if (Program.acpi.DeviceGet(AsusACPI.GPUEco) == 1)
            {
                gpuVisible = buttonGPU.Visible = false;
                return;
            }

            if (HardwareControl.GpuControl is null || !HardwareControl.GpuControl.IsValid) HardwareControl.RecreateGpuControl();

            if (HardwareControl.GpuControl is not null && HardwareControl.GpuControl.IsNvidia)
            {
                nvControl = (NvidiaGpuControl)HardwareControl.GpuControl;
            }
            else
            {
                gpuVisible = buttonGPU.Visible = false;
                return;
            }

            try
            {
                gpuVisible = buttonGPU.Visible = true;

                int gpu_boost = AppConfig.GetMode("gpu_boost");
                int gpu_temp = AppConfig.GetMode("gpu_temp");

                int core = AppConfig.GetMode("gpu_core");
                int memory = AppConfig.GetMode("gpu_memory");
                int clock_limit = AppConfig.GetMode("gpu_clock_limit");

                if (gpu_boost < 0) gpu_boost = AsusACPI.MaxGPUBoost;
                if (gpu_temp < 0) gpu_temp = AsusACPI.MaxGPUTemp;

                if (core == -1) core = 0;
                if (memory == -1) memory = 0;
                if (clock_limit == -1) clock_limit = NvidiaGpuControl.MaxClockLimit;

                if (nvControl.GetClocks(out int current_core, out int current_memory))
                {
                    core = current_core;
                    memory = current_memory;
                }

                int _clockLimit = nvControl.GetMaxGPUCLock();

                if (_clockLimit == 0) clock_limit = NvidiaGpuControl.MaxClockLimit;
                else if (_clockLimit > 0) clock_limit = _clockLimit;

                try
                {
                    labelGPU.Text = nvControl.FullName;
                }
                catch
                {

                }

                //}
                trackGPUClockLimit.Value = Math.Max(Math.Min(clock_limit, NvidiaGpuControl.MaxClockLimit), NvidiaGpuControl.MinClockLimit);

                trackGPUCore.Value = Math.Max(Math.Min(core, NvidiaGpuControl.MaxCoreOffset), NvidiaGpuControl.MinCoreOffset);
                trackGPUMemory.Value = Math.Max(Math.Min(memory, NvidiaGpuControl.MaxMemoryOffset), NvidiaGpuControl.MinMemoryOffset);

                trackGPUBoost.Value = Math.Max(Math.Min(gpu_boost, AsusACPI.MaxGPUBoost), AsusACPI.MinGPUBoost);
                trackGPUTemp.Value = Math.Max(Math.Min(gpu_temp, AsusACPI.MaxGPUTemp), AsusACPI.MinGPUTemp);

                panelGPUBoost.Visible = (Program.acpi.DeviceGet(AsusACPI.PPT_GPUC0) >= 0);
                panelGPUTemp.Visible = (Program.acpi.DeviceGet(AsusACPI.PPT_GPUC2) >= 0);

                VisualiseGPUSettings();

            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.ToString());
                gpuVisible = buttonGPU.Visible = false;
            }

        }

        private void VisualiseGPUSettings()
        {
            labelGPUCore.Text = $"{trackGPUCore.Value} MHz";
            labelGPUMemory.Text = $"{trackGPUMemory.Value} MHz";

            labelGPUBoost.Text = $"{trackGPUBoost.Value}W";
            labelGPUTemp.Text = $"{trackGPUTemp.Value}°C";

            if (trackGPUClockLimit.Value >= NvidiaGpuControl.MaxClockLimit)
                labelGPUClockLimit.Text = "Default";
            else
                labelGPUClockLimit.Text = $"{trackGPUClockLimit.Value} MHz";
        }

        private void trackGPUClockLimit_Scroll(object? sender, EventArgs e)
        {

            int maxClock = (int)Math.Round((float)trackGPUClockLimit.Value / 50) * 50;

            trackGPUClockLimit.Value = maxClock;
            AppConfig.SetMode("gpu_clock_limit", maxClock);
            VisualiseGPUSettings();
        }

        private void trackGPU_Scroll(object? sender, EventArgs e)
        {
            if (sender is null) return;
            TrackBar track = (TrackBar)sender;
            track.Value = (int)Math.Round((float)track.Value / 5) * 5;

            AppConfig.SetMode("gpu_core", trackGPUCore.Value);
            AppConfig.SetMode("gpu_memory", trackGPUMemory.Value);


            VisualiseGPUSettings();

        }

        private void trackGPUPower_Scroll(object? sender, EventArgs e)
        {
            AppConfig.SetMode("gpu_boost", trackGPUBoost.Value);
            AppConfig.SetMode("gpu_temp", trackGPUTemp.Value);

            VisualiseGPUSettings();
        }

        static string ChartYLabel(int percentage, AsusFan device, string unit = "")
        {
            if (percentage == 0) return "OFF";

            int Min = HardwareControl.DEFAULT_FAN_MIN;
            int Max = AppConfig.Get("fan_max_" + (int)device, HardwareControl.DEFAULT_FAN_MAX);

            if (device == AsusFan.XGM) Max = 72;

            if (fanRpm)
                return (200 * Math.Round((float)(Min * 100 + (Max - Min) * percentage) / 200)).ToString() + unit;
            else
                return percentage + "%";
        }

        void SetAxis(Chart chart, AsusFan device)
        {

            chart.ChartAreas[0].AxisY.CustomLabels.Clear();

            for (int i = 0; i <= fansMax; i += 10)
            {
                chart.ChartAreas[0].AxisY.CustomLabels.Add(i - 2, i + 2, ChartYLabel(i, device));
            }

            //chart.ChartAreas[0].AxisY.CustomLabels.Add(fansMax -2, fansMax + 2, Properties.Strings.RPM);
            chart.ChartAreas[0].AxisY.Interval = 10;
        }

        void SetChart(Chart chart, AsusFan device)
        {

            string title = "";
            string scale = ", RPM/°C";

            switch (device)
            {
                case AsusFan.CPU:
                    title = Properties.Strings.FanProfileCPU + scale;
                    break;
                case AsusFan.GPU:
                    title = Properties.Strings.FanProfileGPU + scale;
                    break;
                case AsusFan.Mid:
                    title = Properties.Strings.FanProfileMid + scale;
                    break;
                case AsusFan.XGM:
                    title = "XG Mobile" + scale;
                    break;
            }

            chart.Titles[0].Text = title;

            chart.ChartAreas[0].AxisX.Minimum = 10;
            chart.ChartAreas[0].AxisX.Maximum = 100;
            chart.ChartAreas[0].AxisX.Interval = 10;

            chart.ChartAreas[0].AxisY.Minimum = 0;
            chart.ChartAreas[0].AxisY.Maximum = fansMax;

            chart.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Arial", 7F);

            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = chartGrid;
            chart.ChartAreas[0].AxisY.MajorGrid.LineColor = chartGrid;
            chart.ChartAreas[0].AxisX.LineColor = chartGrid;
            chart.ChartAreas[0].AxisY.LineColor = chartGrid;

            SetAxis(chart, device);

            if (chart.Legends.Count > 0)
                chart.Legends[0].Enabled = false;

        }

        public void FormPosition()
        {
            if (Height > Program.settingsForm.Height)
            {
                Top = Program.settingsForm.Top + Program.settingsForm.Height - Height;
            }
            else
            {
                Size = MinimumSize = new Size(0, Program.settingsForm.Height);
                Height = Program.settingsForm.Height;
                Top = Program.settingsForm.Top;
            }

            Left = Program.settingsForm.Left - Width - 5;
        }

        private void Fans_Shown(object? sender, EventArgs e)
        {
            FormPosition();
        }


        private void TrackPower_MouseUp(object? sender, MouseEventArgs e)
        {
            modeControl.AutoPower();
        }


        public void InitPowerPlan()
        {
            int boost = PowerNative.GetCPUBoost();
            if (boost >= 0)
                comboBoost.SelectedIndex = Math.Min(boost, comboBoost.Items.Count - 1);

            string powerMode = PowerNative.GetPowerMode();
            comboPowerMode.SelectedValue = powerMode;

        }

        private void ComboPowerMode_Changed(object? sender, EventArgs e)
        {
            string powerMode = (string)comboPowerMode.SelectedValue;
            PowerNative.SetPowerMode(powerMode);

            if (PowerNative.GetDefaultPowerMode(Modes.GetCurrentBase()) != powerMode)
                AppConfig.SetMode("powermode", powerMode);
            else
                AppConfig.RemoveMode("powermode");
        }

        private void ComboBoost_Changed(object? sender, EventArgs e)
        {
            if (AppConfig.GetMode("auto_boost") != comboBoost.SelectedIndex)
            {
                PowerNative.SetCPUBoost(comboBoost.SelectedIndex);
            }
            AppConfig.SetMode("auto_boost", comboBoost.SelectedIndex);
        }

        private void CheckApplyPower_Click(object? sender, EventArgs e)
        {
            if (sender is null) return;
            CheckBox chk = (CheckBox)sender;

            AppConfig.SetMode("auto_apply_power", chk.Checked ? 1 : 0);
            modeControl.SetPerformanceMode();

        }

        private void CheckApplyFans_Click(object? sender, EventArgs e)
        {
            if (sender is null) return;
            CheckBox chk = (CheckBox)sender;

            AppConfig.SetMode("auto_apply", chk.Checked ? 1 : 0);
            modeControl.SetPerformanceMode();

        }


        public void LabelFansResult(string text)
        {
            Invoke(delegate
            {
                labelFansResult.Text = text;
                labelFansResult.Visible = (text.Length > 0);
            });
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

            bool modeA0 = (Program.acpi.DeviceGet(AsusACPI.PPT_TotalA0) >= 0 || RyzenControl.IsAMD());
            bool modeB0 = Program.acpi.IsAllAmdPPT();
            bool modeC1 = Program.acpi.DeviceGet(AsusACPI.PPT_APUC1) >= 0;

            panelA0.Visible = modeA0;
            panelB0.Visible = modeB0;

            panelApplyPower.Visible = panelTitleCPU.Visible = modeA0 || modeB0 || modeC1;


            // All AMD version has B0 but doesn't have C0 (Nvidia GPU) settings
            if (modeB0)
            {
                labelLeftA0.Text = "Platform (CPU + GPU)";
                labelLeftB0.Text = "CPU";
                panelC1.Visible = false;
            }
            else
            {
                if (RyzenControl.IsAMD())
                    labelLeftA0.Text = "CPU Slow (SPL + sPPT)";
                else
                    labelLeftA0.Text = "CPU (PL1 + PL2)";

                labelLeftC1.Text = "CPU Fast (fPPT)";
                panelC1.Visible = modeC1;
            }

            int limit_total;
            int limit_cpu;
            int limit_fast;

            bool apply = AppConfig.IsMode("auto_apply_power");

            if (changed)
            {
                limit_total = trackA0.Value;
                limit_cpu = trackB0.Value;
                limit_fast = trackC1.Value;
            }
            else
            {
                limit_total = AppConfig.GetMode("limit_total");
                limit_cpu = AppConfig.GetMode("limit_cpu");
                limit_fast = AppConfig.GetMode("limit_fast");
            }

            if (limit_total < 0) limit_total = AsusACPI.DefaultTotal;
            if (limit_total > AsusACPI.MaxTotal) limit_total = AsusACPI.MaxTotal;
            if (limit_total < AsusACPI.MinTotal) limit_total = AsusACPI.MinTotal;

            if (limit_cpu < 0) limit_cpu = AsusACPI.DefaultCPU;
            if (limit_cpu > AsusACPI.MaxCPU) limit_cpu = AsusACPI.MaxCPU;
            if (limit_cpu < AsusACPI.MinCPU) limit_cpu = AsusACPI.MinCPU;
            if (limit_cpu > limit_total) limit_cpu = limit_total;

            if (limit_fast < 0) limit_fast = AsusACPI.DefaultTotal;
            if (limit_fast > AsusACPI.MaxTotal) limit_fast = AsusACPI.MaxTotal;
            if (limit_fast < AsusACPI.MinTotal) limit_fast = AsusACPI.MinTotal;

            trackA0.Value = limit_total;
            trackB0.Value = limit_cpu;
            trackC1.Value = limit_fast;

            checkApplyPower.Checked = apply;

            labelA0.Text = trackA0.Value.ToString() + "W";
            labelB0.Text = trackB0.Value.ToString() + "W";
            labelC1.Text = trackC1.Value.ToString() + "W";

            AppConfig.SetMode("limit_total", limit_total);
            AppConfig.SetMode("limit_cpu", limit_cpu);
            AppConfig.SetMode("limit_fast", limit_fast);


        }


        private void TrackPower_Scroll(object? sender, EventArgs e)
        {
            InitPower(true);
        }


        public void InitFans()
        {

            int chartCount = 2;

            // Middle / system fan check
            if (!AsusACPI.IsEmptyCurve(Program.acpi.GetFanCurve(AsusFan.Mid)))
            {
                AppConfig.Set("mid_fan", 1);
                chartCount++;
                chartMid.Visible = true;
                SetChart(chartMid, AsusFan.Mid);
                LoadProfile(seriesMid, AsusFan.Mid);
            }
            else
            {
                AppConfig.Set("mid_fan", 0);
            }

            // XG Mobile Fan check
            if (Program.acpi.IsXGConnected())
            {
                AppConfig.Set("xgm_fan", 1);
                chartCount++;
                chartXGM.Visible = true;
                SetChart(chartXGM, AsusFan.XGM);
                LoadProfile(seriesXGM, AsusFan.XGM);
            }
            else
            {
                AppConfig.Set("xgm_fan", 0);
            }

            try
            {
                if (chartCount > 2)
                    Size = MinimumSize = new Size(Size.Width, (int)(ControlHelper.GetDpiScale(this).Value * (chartCount * 200 + 100)));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }


            SetChart(chartCPU, AsusFan.CPU);
            SetChart(chartGPU, AsusFan.GPU);

            LoadProfile(seriesCPU, AsusFan.CPU);
            LoadProfile(seriesGPU, AsusFan.GPU);

            checkApplyFans.Checked = AppConfig.IsMode("auto_apply");

        }


        void LoadProfile(Series series, AsusFan device, bool reset = false)
        {

            series.ChartType = SeriesChartType.Line;
            series.MarkerSize = 10;
            series.MarkerStyle = MarkerStyle.Circle;

            series.Points.Clear();

            byte[] curve = AppConfig.GetFanConfig(device);

            if (reset || AsusACPI.IsInvalidCurve(curve))
            {
                curve = Program.acpi.GetFanCurve(device, Modes.GetCurrentBase());

                if (AsusACPI.IsInvalidCurve(curve))
                    curve = AppConfig.GetDefaultCurve(device);

                curve = AsusACPI.FixFanCurve(curve);

            }

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

        void SaveProfile(Series series, AsusFan device)
        {
            byte[] curve = new byte[16];
            int i = 0;
            foreach (DataPoint point in series.Points)
            {
                curve[i] = (byte)point.XValue;
                curve[i + 8] = (byte)point.YValues.First();
                i++;
            }

            AppConfig.SetFanConfig(device, curve);
            //Program.wmi.SetFanCurve(device, curve);

        }


        private void ButtonReset_Click(object? sender, EventArgs e)
        {

            LoadProfile(seriesCPU, AsusFan.CPU, true);
            LoadProfile(seriesGPU, AsusFan.GPU, true);

            if (AppConfig.Is("mid_fan"))
                LoadProfile(seriesMid, AsusFan.Mid, true);

            if (AppConfig.Is("xgm_fan"))
                LoadProfile(seriesXGM, AsusFan.XGM, true);

            checkApplyFans.Checked = false;
            checkApplyPower.Checked = false;

            AppConfig.SetMode("auto_apply", 0);
            AppConfig.SetMode("auto_apply_power", 0);

            trackUV.Value = RyzenControl.MaxCPUUV;
            trackUViGPU.Value = RyzenControl.MaxIGPUUV;
            trackTemp.Value = RyzenControl.MaxTemp;

            AdvancedScroll();
            AppConfig.SetMode("cpu_temp", -1);

            modeControl.ResetPerformanceMode();

            InitPowerPlan();

            if (Program.acpi.IsXGConnected()) AsusUSB.ResetXGM();


            if (gpuVisible)
            {
                trackGPUClockLimit.Value = NvidiaGpuControl.MaxClockLimit;
                trackGPUCore.Value = 0;
                trackGPUMemory.Value = 0;
                trackGPUBoost.Value = AsusACPI.MaxGPUBoost;
                trackGPUTemp.Value = AsusACPI.MaxGPUTemp;

                AppConfig.SetMode("gpu_clock_limit", trackGPUClockLimit.Value);

                AppConfig.SetMode("gpu_boost", trackGPUBoost.Value);
                AppConfig.SetMode("gpu_temp", trackGPUTemp.Value);

                AppConfig.SetMode("gpu_core", trackGPUCore.Value);
                AppConfig.SetMode("gpu_memory", trackGPUMemory.Value);

                VisualiseGPUSettings();
                modeControl.SetGPUClocks(true);
                modeControl.SetGPUPower();
            }

        }

        private void Chart_Save()
        {
            curPoint = null;
            curIndex = -1;
            labelTip.Visible = false;

            SaveProfile(seriesCPU, AsusFan.CPU);
            SaveProfile(seriesGPU, AsusFan.GPU);

            if (AppConfig.Is("mid_fan"))
                SaveProfile(seriesMid, AsusFan.Mid);

            if (AppConfig.Is("xgm_fan"))
                SaveProfile(seriesXGM, AsusFan.XGM);

            modeControl.AutoFans();
        }

        private void ChartCPU_MouseUp(object? sender, MouseEventArgs e)
        {
            Chart_Save();
        }


        private void ChartCPU_MouseLeave(object? sender, EventArgs e)
        {
            curPoint = null;
            curIndex = -1;
            labelTip.Visible = false;
        }

        private void ChartCPU_MouseMove(object? sender, MouseEventArgs e, AsusFan device)
        {

            if (sender is null) return;
            Chart chart = (Chart)sender;

            ChartArea ca = chart.ChartAreas[0];
            Axis ax = ca.AxisX;
            Axis ay = ca.AxisY;

            bool tip = false;

            HitTestResult hit = chart.HitTest(e.X, e.Y);
            Series series = chart.Series[0];

            if (hit.Series is not null && hit.PointIndex >= 0)
            {
                curIndex = hit.PointIndex;
                curPoint = hit.Series.Points[curIndex];
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
                    if (dy > fansMax) dy = fansMax;

                    dymin = (dx - 70) * 1.2;

                    if (dy < dymin) dy = dymin;

                    if (e.Button.HasFlag(MouseButtons.Left))
                    {
                        double deltaY = dy - curPoint.YValues[0];
                        double deltaX = dx - curPoint.XValue;

                        curPoint.XValue = dx;

                        if (Control.ModifierKeys == Keys.Shift)
                            AdjustAll(0, deltaY, series);
                        else
                        {
                            curPoint.YValues[0] = dy;
                            AdjustAllLevels(curIndex, dx, dy, series);
                        }

                        tip = true;
                    }

                    labelTip.Text = Math.Round(curPoint.XValue) + "C, " + ChartYLabel((int)curPoint.YValues[0], device, " " + Properties.Strings.RPM);
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

        private void AdjustAll(double deltaX, double deltaY, Series series)
        {
            for (int i = 0; i < series.Points.Count; i++)
            {
                series.Points[i].XValue = Math.Max(20, Math.Min(100, series.Points[i].XValue + deltaX));
                series.Points[i].YValues[0] = Math.Max(0, Math.Min(100, series.Points[i].YValues[0] + deltaY));
            }
        }

        private void AdjustAllLevels(int index, double curXVal, double curYVal, Series series)
        {

            // Get the neighboring DataPoints of the hit point
            DataPoint? upperPoint = null;
            DataPoint? lowerPoint = null;

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
