using GHelper.Fan;
using GHelper.Gpu.NVidia;
using GHelper.Mode;
using GHelper.UI;
using GHelper.USB;
using Ryzen;
using System.Diagnostics;
using System.Management;
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

        const int tempMin = 20;
        const int tempMax = 110;

        const int fansMax = 100;

        NvidiaGpuControl? nvControl = null;
        ModeControl modeControl = Program.modeControl;

        FanSensorControl fanSensorControl;

        static int gpuPowerBase = 0;
        static bool isGPUPower => gpuPowerBase > 0;
        static bool clampFanDots = AppConfig.IsClampFanDots();

        FlowLayoutPanel panelFanTable;
        RButton buttonTable;

        // Helper to add accessible input fields next to sliders
        private void AddInput(TrackBar track, Label labelVal, Label labelTitle)
        {
            NumericUpDown num = new NumericUpDown();
            num.Top = labelVal.Top;
            num.Left = labelVal.Left - 20; // Shift slightly left to fit
            num.Width = labelVal.Width + 20;
            num.Height = 30;
            num.Font = labelVal.Font;
            num.ForeColor = labelVal.ForeColor;
            num.BackColor = SystemColors.ControlLightLight;
            num.BorderStyle = BorderStyle.FixedSingle;
            
            num.Minimum = track.Minimum;
            num.Maximum = track.Maximum;
            num.Increment = track.TickFrequency > 0 ? track.TickFrequency : 1;
            num.Value = Math.Max(num.Minimum, Math.Min(num.Maximum, track.Value));

            // Sync TrackBar -> Numeric
            track.ValueChanged += (s, e) =>
            {
                num.Value = Math.Max(num.Minimum, Math.Min(num.Maximum, track.Value));
            };

            // Sync Numeric -> TrackBar
            num.ValueChanged += (s, e) =>
            {
                track.Value = (int)num.Value;
                labelVal.Text = track.Value.ToString(); // Keep label updated for reference if needed, or hide it
            };

            // Accessibility
            num.AccessibleName = labelTitle.Text;
            track.AccessibleName = labelTitle.Text;

            // Replace the visual label with the interactive input
            labelVal.Visible = false; 
            track.Parent.Controls.Add(num);
            num.BringToFront();
        }

        private void InitFanTable()
        {
            if (panelFanTable != null) return;

            panelFanTable = new FlowLayoutPanel();
            panelFanTable.Dock = DockStyle.Fill;
            panelFanTable.AutoScroll = true;
            panelFanTable.WrapContents = false;
            panelFanTable.FlowDirection = FlowDirection.TopDown;
            panelFanTable.Padding = new Padding(10);
            panelFanTable.Visible = false;

            panelFans.Controls.Add(panelFanTable);
            panelFanTable.BringToFront();

            List<AsusFan> fans = new List<AsusFan> { AsusFan.CPU, AsusFan.GPU };
            if (chartMid.Visible) fans.Add(AsusFan.Mid);
            if (chartXGM.Visible) fans.Add(AsusFan.XGM);

            foreach (AsusFan fan in fans)
            {
                Label title = new Label();
                title.Text = fan.ToString() + " " + Properties.Strings.FanCurveWarning;
                title.Font = new Font(Font, FontStyle.Bold);
                title.AutoSize = true;
                title.Margin = new Padding(0, 10, 0, 5);
                panelFanTable.Controls.Add(title);

                TableLayoutPanel table = new TableLayoutPanel();
                table.RowCount = 9;
                table.ColumnCount = 3;
                table.AutoSize = true;
                table.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
                
                table.Controls.Add(new Label { Text = Properties.Strings.Point, AutoSize = true, Font = new Font(Font, FontStyle.Underline) }, 0, 0);
                table.Controls.Add(new Label { Text = Properties.Strings.Temperature, AutoSize = true, Font = new Font(Font, FontStyle.Underline) }, 1, 0);
                table.Controls.Add(new Label { Text = Properties.Strings.Speed, AutoSize = true, Font = new Font(Font, FontStyle.Underline) }, 2, 0);

                byte[] curve = AppConfig.GetFanConfig(fan);
                if (AsusACPI.IsInvalidCurve(curve)) curve = AppConfig.GetDefaultCurve(fan);
                curve = AsusACPI.FixFanCurve(curve);

                // Keep references to controls for validation logic
                List<NumericUpDown> tempControls = new List<NumericUpDown>();

                for (int i = 0; i < 8; i++)
                {
                    int index = i;
                    Label lbl = new Label { Text = (i + 1).ToString(), AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Top };
                    
                    NumericUpDown numTemp = new NumericUpDown { Minimum = 20, Maximum = 110, Value = Math.Max(20, Math.Min(110, (int)curve[i])), Width = 80 };
                    NumericUpDown numSpeed = new NumericUpDown { Minimum = 0, Maximum = 100, Value = curve[i + 8], Width = 80 };

                    numTemp.AccessibleName = $"{fan} Point {i + 1} Temperature";
                    numSpeed.AccessibleName = $"{fan} Point {i + 1} Speed %";
                    
                    tempControls.Add(numTemp);

                    // Smart Validation Logic (Cascading)
                    numTemp.ValueChanged += (s, e) => {
                        // Prevent infinite loop by checking if value actually needs logical adjustment
                        decimal currentVal = numTemp.Value;
                        
                        // 1. Push Upper Neighbors UP
                        // If I set Point 3 to 60, Point 4 must be at least 61
                        if (index < 7)
                        {
                            var nextControl = tempControls[index + 1];
                            if (nextControl.Value <= currentVal)
                            {
                                nextControl.Value = Math.Min(nextControl.Maximum, currentVal + 1);
                            }
                        }

                        // 2. Push Lower Neighbors DOWN
                        // If I set Point 3 to 40, Point 2 must be at most 39
                        if (index > 0)
                        {
                            var prevControl = tempControls[index - 1];
                            if (prevControl.Value >= currentVal)
                            {
                                prevControl.Value = Math.Max(prevControl.Minimum, currentVal - 1);
                            }
                        }

                        // Save config
                        byte[] c = AppConfig.GetFanConfig(fan);
                        c[index] = (byte)numTemp.Value;
                        AppConfig.SetFanConfig(fan, c);
                    };

                    numSpeed.ValueChanged += (s, e) => {
                        byte[] c = AppConfig.GetFanConfig(fan);
                        c[index + 8] = (byte)numSpeed.Value;
                        AppConfig.SetFanConfig(fan, c);
                    };

                    table.Controls.Add(lbl, 0, i + 1);
                    table.Controls.Add(numTemp, 1, i + 1);
                    table.Controls.Add(numSpeed, 2, i + 1);
                }

                panelFanTable.Controls.Add(table);
            }
        }

        public Fans()
        {

            InitializeComponent();

            // Create Table Editor Toggle
            buttonTable = new RButton();
            buttonTable.Text = Properties.Strings.TableEditor;
            buttonTable.AutoSize = true;
            buttonTable.BackColor = SystemColors.ControlLight;
            buttonTable.FlatStyle = FlatStyle.Flat;
            buttonTable.Top = 15;
            buttonTable.Left = 200; // Position near Profile label
            buttonTable.Click += (s, e) => {
                InitFanTable();
                bool tableMode = !panelFanTable.Visible;
                panelFanTable.Visible = tableMode;
                tableFanCharts.Visible = !tableMode;
                buttonTable.Text = tableMode ? Properties.Strings.ChartView : Properties.Strings.TableEditor;
                if (!tableMode) InitFans(); // Refresh charts from config when switching back
            };
            panelTitleFans.Controls.Add(buttonTable);
            buttonTable.BringToFront();

            fanSensorControl = new FanSensorControl(this);

            //float dpi = ControlHelper.GetDpiScale(this).Value;
            //comboModes.Size = new Size(comboModes.Width, (int)dpi * 18);
            comboModes.ClientSize = new Size(comboModes.Width, comboModes.Height - 4);

            Text = Properties.Strings.FansAndPower;
            labelPowerLimits.Text = Properties.Strings.PowerLimits;
            checkApplyPower.Text = Properties.Strings.ApplyPowerLimits;

            labelFans.Text = "BIOS " + Properties.Strings.FanCurves;
            labelBoost.Text = Properties.Strings.CPUBoost;
            buttonReset.Text = Properties.Strings.FactoryDefaults;
            checkApplyFans.Text = Properties.Strings.ApplyFanCurve;

            labelGPU.Text = Properties.Strings.GPUSettings;

            labelGPUCoreTitle.Text = Properties.Strings.GPUCoreClockOffset;
            labelGPUMemoryTitle.Text = Properties.Strings.GPUMemoryClockOffset;
            labelGPUBoostTitle.Text = Properties.Strings.GPUBoost;
            labelGPUTempTitle.Text = Properties.Strings.GPUTempTarget;
            labelGPUPowerTitle.Text = Properties.Strings.GPUPower;

            labelRisky.Text = Properties.Strings.UndervoltingRisky;
            buttonApplyAdvanced.Text = Properties.Strings.Apply;
            checkApplyUV.Text = Properties.Strings.AutoApply;

            buttonCalibrate.Text = Properties.Strings.Calibrate;

            InitTheme(true);

            labelTip.Visible = false;
            labelTip.BackColor = Color.Transparent;

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

            trackTotal.Maximum = AsusACPI.MaxTotal;
            trackTotal.Minimum = AsusACPI.MinTotal;

            trackSlow.Maximum = AsusACPI.MaxTotal;
            trackSlow.Minimum = AsusACPI.MinTotal;

            trackCPU.Maximum = AsusACPI.MaxCPU;
            trackCPU.Minimum = AsusACPI.MinCPU;

            trackFast.Maximum = AsusACPI.MaxTotal;
            trackFast.Minimum = AsusACPI.MinTotal;

            trackTotal.Scroll += TrackTotal_Scroll;
            trackSlow.Scroll += TrackSlow_Scroll;
            trackFast.Scroll += TrackFast_Scroll;
            trackCPU.Scroll += TrackCPU_Scroll;

            trackFast.MouseUp += TrackPower_MouseUp;
            trackCPU.MouseUp += TrackPower_MouseUp;
            trackTotal.MouseUp += TrackPower_MouseUp;
            trackSlow.MouseUp += TrackPower_MouseUp;

            trackFast.KeyUp += TrackPower_KeyUp;
            trackCPU.KeyUp += TrackPower_KeyUp;
            trackTotal.KeyUp += TrackPower_KeyUp;
            trackSlow.KeyUp += TrackPower_KeyUp;

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

            trackGPUPower.Minimum = AsusACPI.MinGPUPower;
            trackGPUPower.Maximum = AsusACPI.MaxGPUPower;

            trackGPUClockLimit.Scroll += trackGPUClockLimit_Scroll;
            trackGPUCore.Scroll += trackGPU_Scroll;
            trackGPUMemory.Scroll += trackGPU_Scroll;

            trackGPUBoost.Scroll += trackGPUPower_Scroll;
            trackGPUTemp.Scroll += trackGPUPower_Scroll;
            trackGPUPower.Scroll += trackGPUPower_Scroll;

            trackGPUCore.MouseUp += TrackGPUClocks_MouseUp;
            trackGPUMemory.MouseUp += TrackGPUClocks_MouseUp;
            trackGPUClockLimit.MouseUp += TrackGPUClocks_MouseUp;

            trackGPUBoost.MouseUp += TrackGPU_MouseUp;
            trackGPUTemp.MouseUp += TrackGPU_MouseUp;
            trackGPUPower.MouseUp += TrackGPU_MouseUp;


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

            FillModes(false);
            InitAll();
            InitCPU();

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

            // Accessibility: Treat buttons as Tabs
            buttonCPU.AccessibleRole = AccessibleRole.PageTab;
            buttonGPU.AccessibleRole = AccessibleRole.PageTab;
            buttonAdvanced.AccessibleRole = AccessibleRole.PageTab;
            tableNav.AccessibleRole = AccessibleRole.PageTabList;

            buttonCPU.Click += ButtonCPU_Click;
            buttonGPU.Click += ButtonGPU_Click;
            buttonAdvanced.Click += ButtonAdvanced_Click;

            checkApplyUV.Click += CheckApplyUV_Click;

            buttonCalibrate.Click += ButtonCalibrate_Click;

            buttonDownload.Click += ButtonDownload_Click;

            checkFanClamp.Checked = clampFanDots;
            checkFanClamp.Click += CheckFanClamp_Click;

            ToggleNavigation(0);

            if (Program.acpi.DeviceGet(AsusACPI.DevsCPUFanCurve) < 0) buttonCalibrate.Visible = false;

            // Accessible Mode Logic: The "Perfect" Tab System
            if (AppConfig.IsAccessible())
            {
                InitFanTable();
                tableFanCharts.Visible = false;
                buttonTable.Visible = false;

                TabControl tabControl = new TabControl();
                tabControl.Dock = DockStyle.Fill;
                tabControl.AccessibleRole = AccessibleRole.PageTabList;

                // 1. Fans Tab (Moved from the right side)
                TabPage tabFans = new TabPage(Properties.Strings.FanCurves);
                panelFanTable.AutoSize = false;
                panelFanTable.Dock = DockStyle.Fill;
                panelFanTable.Visible = true;
                panelFanTable.Parent = tabFans;
                tabFans.Controls.Add(panelFanTable);
                tabControl.TabPages.Add(tabFans);

                // 2. CPU / Power Tab
                TabPage tabCPU = new TabPage("CPU");
                panelPower.AutoSize = false;
                panelPower.Dock = DockStyle.Fill;
                panelPower.Visible = true;
                panelPower.Parent = tabCPU;
                tabCPU.Controls.Add(panelPower);
                tabCPU.AutoScroll = true;
                tabControl.TabPages.Add(tabCPU);

                // 3. GPU Tab (if available)
                if (gpuVisible)
                {
                    TabPage tabGPU = new TabPage("GPU");
                    panelGPU.AutoSize = false;
                    panelGPU.Dock = DockStyle.Fill;
                    panelGPU.Visible = true;
                    panelGPU.Parent = tabGPU;
                    tabGPU.Controls.Add(panelGPU);
                    tabGPU.AutoScroll = true;
                    tabControl.TabPages.Add(tabGPU);
                }

                // 4. Advanced Tab (if AMD)
                if (RyzenControl.IsAMD())
                {
                    TabPage tabAdvanced = new TabPage("Advanced");
                    panelAdvanced.AutoSize = false;
                    panelAdvanced.Dock = DockStyle.Fill;
                    panelAdvanced.Visible = true;
                    panelAdvanced.Parent = tabAdvanced;
                    tabAdvanced.Controls.Add(panelAdvanced);
                    tabAdvanced.AutoScroll = true;
                    tabControl.TabPages.Add(tabAdvanced);
                }

                // Layout transformation: Make tabs fill the whole window
                panelNav.Visible = false;
                panelFans.Visible = false; // Hide the right-side container
                panelSliders.Dock = DockStyle.Fill; // Left side becomes the whole window
                panelSliders.Padding = new Padding(0);
                panelSliders.Controls.Add(tabControl);
                tabControl.BringToFront();
            }

            FormClosed += Fans_FormClosed;

        }

        private void CheckFanClamp_Click(object? sender, EventArgs e)
        {
            clampFanDots = checkFanClamp.Checked;
            AppConfig.Set("fan_clamp", clampFanDots ? 1 : 0);
        }

        private void ButtonDownload_Click(object? sender, EventArgs e)
        {
            RyzenControl.DownloadRing();

            panelAdvancedAlways.Visible = true;
            panelAdvancedApply.Visible = true;
            labelRisky.Visible = true;
            panelUViGPU.Visible = true;
            panelUV.Visible = true;
            panelTitleAdvanced.Visible = true;
            panelTemperature.Visible = true;
            panelTitleTemp.Visible = true;

            VisualiseAdvanced();
        }

        private void ButtonCalibrate_Click(object? sender, EventArgs e)
        {
            buttonCalibrate.Enabled = false;
            fanSensorControl.StartCalibration();
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

        public void InitCPU()
        {
            Task.Run(async () =>
            {
                string CPUName;
                using (ManagementObjectSearcher myProcessorObject = new ManagementObjectSearcher("select * from Win32_Processor"))
                    foreach (ManagementObject obj in myProcessorObject.Get())
                    {
                        CPUName = obj["Name"].ToString();
                        Invoke(delegate
                        {
                            Text = Properties.Strings.FansAndPower + " - " + CPUName;
                        });
                    }
            });
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

            if (!RyzenControl.IsRingExsists())
            {
                panelTitleAdvanced.Visible = false;
                labelRisky.Visible = false;
                panelUV.Visible = false;
                panelUViGPU.Visible = false;
                panelTitleTemp.Visible = false;
                panelTemperature.Visible = false;
                panelAdvancedAlways.Visible = false;
                panelAdvancedApply.Visible = false;
                panelDownload.Visible = true;

            }
            else
            {
                panelDownload.Visible = false;
            }

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

        private void FillModes(bool contextMenu = true)
        {
            comboModes.DropDownStyle = ComboBoxStyle.DropDownList;
            comboModes.DataSource = new BindingSource(Modes.GetDictonary(), null);
            comboModes.DisplayMember = "Value";
            comboModes.ValueMember = "Key";
            if (contextMenu) Program.settingsForm.SetContextMenu();
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
        }

        private void TrackGPUClocks_MouseUp(object? sender, MouseEventArgs e)
        {
            modeControl.SetGPUClocks(true);
        }

        private void InitGPUPower()
        {
            gpuPowerBase = Program.acpi.DeviceGet(AsusACPI.GPU_BASE);
            if (gpuPowerBase >= 0) Logger.WriteLine($"ReadGPUPowerBase: {gpuPowerBase}");

            panelGPUPower.Visible = isGPUPower;
            if (!isGPUPower) return;

            int maxGPUPower = NvidiaSmi.GetMaxGPUPower();
            if (maxGPUPower > 0)
            {
                AsusACPI.MaxGPUPower = maxGPUPower - gpuPowerBase - AsusACPI.MaxGPUBoost;
                trackGPUPower.Minimum = AsusACPI.MinGPUPower;
                trackGPUPower.Maximum = AsusACPI.MaxGPUPower;
            }

            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(200));
                int gpuPowerVar = Program.acpi.DeviceGet(AsusACPI.GPU_POWER);
                Logger.WriteLine($"ReadGPUPower ({Modes.GetCurrentBase()}): {gpuPowerVar}");

                int gpu_power = AppConfig.GetMode("gpu_power");
                if (gpu_power < 0) gpu_power = (gpuPowerVar >= 0) ? gpuPowerVar : AsusACPI.MaxGPUPower;

                Invoke(delegate
                {
                    trackGPUPower.Value = Math.Max(Math.Min(gpu_power, AsusACPI.MaxGPUPower), AsusACPI.MinGPUPower);
                    AddInput(trackGPUPower, labelGPUPower, labelGPUPowerTitle);
                    VisualiseGPUSettings();
                });
            });
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

                if (nvControl is not null)
                {
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
                }

                trackGPUClockLimit.Value = Math.Max(Math.Min(clock_limit, NvidiaGpuControl.MaxClockLimit), NvidiaGpuControl.MinClockLimit);

                trackGPUCore.Value = Math.Max(Math.Min(core, NvidiaGpuControl.MaxCoreOffset), NvidiaGpuControl.MinCoreOffset);
                trackGPUMemory.Value = Math.Max(Math.Min(memory, NvidiaGpuControl.MaxMemoryOffset), NvidiaGpuControl.MinMemoryOffset);

                trackGPUBoost.Value = Math.Max(Math.Min(gpu_boost, AsusACPI.MaxGPUBoost), AsusACPI.MinGPUBoost);
                trackGPUTemp.Value = Math.Max(Math.Min(gpu_temp, AsusACPI.MaxGPUTemp), AsusACPI.MinGPUTemp);

                // Add accessible inputs for GPU
                AddInput(trackGPUClockLimit, labelGPUClockLimit, labelGPUClockLimitTitle);
                AddInput(trackGPUCore, labelGPUCore, labelGPUCoreTitle);
                AddInput(trackGPUMemory, labelGPUMemory, labelGPUMemoryTitle);
                AddInput(trackGPUBoost, labelGPUBoost, labelGPUBoostTitle);
                AddInput(trackGPUTemp, labelGPUTemp, labelGPUTempTitle);

                panelGPUBoost.Visible = (Program.acpi.DeviceGet(AsusACPI.PPT_GPUC0) >= 0);
                panelGPUTemp.Visible = (Program.acpi.DeviceGet(AsusACPI.PPT_GPUC2) >= 0);

                VisualiseGPUSettings();

                InitGPUPower();

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

            labelGPUPower.Text = (gpuPowerBase + trackGPUPower.Value) + "W";

        }

        private void trackGPUClockLimit_Scroll(object? sender, EventArgs e)
        {

            int maxClock = (int)Math.Round((float)trackGPUClockLimit.Value / 5) * 5;

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

            if (isGPUPower) AppConfig.SetMode("gpu_power", trackGPUPower.Value);

            VisualiseGPUSettings();
        }

        static string ChartYLabel(int percentage, AsusFan device, string unit = "")
        {
            if (percentage == 0) return "OFF";

            int Min = FanSensorControl.GetFanMin(device);
            int Max = FanSensorControl.GetFanMax(device);

            if (fanRpm)
                return (200 * Math.Floor((float)(Min * 100 + (Max - Min) * percentage) / 200)).ToString() + unit;
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

            chart.ChartAreas[0].AxisX.Minimum = tempMin;
            chart.ChartAreas[0].AxisX.Maximum = tempMax;
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
                Top = Math.Max(0, Program.settingsForm.Top + Program.settingsForm.Height - Height);
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
            Task.Run(() =>
            {
                modeControl.AutoPower(true);
            });
        }


        private void TrackPower_KeyUp(object? sender, KeyEventArgs e)
        {
            Task.Run(() =>
            {
                modeControl.AutoPower(true);
            });
        }

        public void InitPowerPlan()
        {
            int boost = PowerNative.GetCPUBoost();
            if (boost >= 0)
                comboBoost.SelectedIndex = Math.Min(boost, comboBoost.Items.Count - 1);

            string powerMode = PowerNative.GetPowerMode();
            bool batterySaver = PowerNative.GetBatterySaverStatus();

            comboPowerMode.Enabled = !batterySaver;

            if (batterySaver)
                comboPowerMode.SelectedIndex = 0;
            else
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

        public void InitAxis()
        {
            if (this == null || this.Text == "") return;

            Invoke(delegate
            {
                buttonCalibrate.Enabled = true;
                SetAxis(chartCPU, AsusFan.CPU);
                SetAxis(chartGPU, AsusFan.GPU);
                if (chartMid.Visible) SetAxis(chartMid, AsusFan.Mid);
            });
        }

        public void LabelFansResult(string text)
        {
            if (text.Length > 0) Logger.WriteLine(text);

            if (this == null || this.Text == "") return;

            Invoke(delegate
            {
                labelFansResult.Text = text;
                labelFansResult.Visible = (text.Length > 0);
            });
        }


        public void InitPower()
        {

            bool modeA = Program.acpi.DeviceGet(AsusACPI.PPT_APUA0) >= 0 || RyzenControl.IsAMD();
            bool modeB0 = Program.acpi.IsAllAmdPPT();
            bool modeC1 = Program.acpi.DeviceGet(AsusACPI.PPT_APUC1) >= 0;

            panelTotal.Visible = modeA;
            panelCPU.Visible = modeB0;

            panelApplyPower.Visible = panelTitleCPU.Visible = modeA || modeB0 || modeC1;


            // All AMD version has B0 but doesn't have C0 (Nvidia GPU) settings
            if (modeB0)
            {
                labelLeftTotal.Text = "Platform (CPU + GPU)";
                labelLeftCPU.Text = "CPU";
                panelFast.Visible = false;
                panelSlow.Visible = false;
            }
            else
            {
                panelSlow.Visible = modeA;

                if (RyzenControl.IsAMD())
                {
                    labelLeftTotal.Text = "SPL (CPU sustained)";
                    labelLeftSlow.Text = "sPPT (CPU 2 min boost)";
                    labelLeftFast.Text = "fPPT (CPU 2 sec boost)";
                    panelFast.Visible = modeC1;

                }
                else
                {
                    labelLeftTotal.Text = "PL1 (CPU sustained)";
                    labelLeftSlow.Text = "PL2 (CPU 2 min boost)";
                    panelFast.Visible = false;
                }

            }

            checkApplyPower.Checked = AppConfig.IsMode("auto_apply_power");

            int limit_total = AppConfig.GetMode("limit_total", AsusACPI.DefaultTotal);
            int limit_slow = AppConfig.GetMode("limit_slow", limit_total);
            int limit_fast = AppConfig.GetMode("limit_fast", limit_total);
            int limit_cpu = AppConfig.GetMode("limit_cpu", AsusACPI.DefaultCPU);

            if (limit_total > AsusACPI.MaxTotal) limit_total = AsusACPI.MaxTotal;
            if (limit_total < AsusACPI.MinTotal) limit_total = AsusACPI.MinTotal;

            if (limit_cpu > AsusACPI.MaxCPU) limit_cpu = AsusACPI.MaxCPU;
            if (limit_cpu < AsusACPI.MinCPU) limit_cpu = AsusACPI.MinCPU;

            if (limit_slow > AsusACPI.MaxTotal) limit_slow = AsusACPI.MaxTotal;
            if (limit_slow < AsusACPI.MinTotal) limit_slow = AsusACPI.MinTotal;

            if (limit_fast > AsusACPI.MaxTotal) limit_fast = AsusACPI.MaxTotal;
            if (limit_fast < AsusACPI.MinTotal) limit_fast = AsusACPI.MinTotal;

            trackTotal.Value = limit_total;
            trackSlow.Value = limit_slow;
            trackCPU.Value = limit_cpu;
            trackFast.Value = limit_fast;

            // Add accessible inputs
            AddInput(trackTotal, labelTotal, labelLeftTotal);
            AddInput(trackSlow, labelSlow, labelLeftSlow);
            AddInput(trackCPU, labelCPU, labelLeftCPU);
            AddInput(trackFast, labelFast, labelLeftFast);

            SavePower();

        }

        private void SavePower()
        {
            labelTotal.Text = trackTotal.Value.ToString() + "W";
            labelSlow.Text = trackSlow.Value.ToString() + "W";
            labelCPU.Text = trackCPU.Value.ToString() + "W";
            labelFast.Text = trackFast.Value.ToString() + "W";

            AppConfig.SetMode("limit_total", trackTotal.Value);
            AppConfig.SetMode("limit_slow", trackSlow.Value);
            AppConfig.SetMode("limit_cpu", trackCPU.Value);
            AppConfig.SetMode("limit_fast", trackFast.Value);
        }

        private void TrackTotal_Scroll(object? sender, EventArgs e)
        {
            if (trackTotal.Value > trackSlow.Value) trackSlow.Value = trackTotal.Value;
            if (trackTotal.Value > trackFast.Value) trackFast.Value = trackTotal.Value;
            if (trackTotal.Value < trackCPU.Value) trackCPU.Value = trackTotal.Value;
            SavePower();
        }

        private void TrackSlow_Scroll(object? sender, EventArgs e)
        {
            if (trackSlow.Value < trackTotal.Value) trackTotal.Value = trackSlow.Value;
            if (trackSlow.Value > trackFast.Value) trackFast.Value = trackSlow.Value;
            SavePower();
        }

        private void TrackFast_Scroll(object? sender, EventArgs e)
        {
            if (trackFast.Value < trackSlow.Value) trackSlow.Value = trackFast.Value;
            if (trackFast.Value < trackTotal.Value) trackTotal.Value = trackFast.Value;
            SavePower();
        }

        private void TrackCPU_Scroll(object? sender, EventArgs e)
        {
            if (trackCPU.Value > trackTotal.Value) trackTotal.Value = trackCPU.Value;
            SavePower();
        }

        public void InitFans()
        {

            int chartCount = 2;

            // Middle / system fan check
            if (!AsusACPI.IsEmptyCurve(Program.acpi.GetFanCurve(AsusFan.Mid)) || Program.acpi.GetFan(AsusFan.Mid) >= 0)
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
                    Size = MinimumSize = new Size(Size.Width, Math.Max(MinimumSize.Height, (int)(ControlHelper.GetDpiScale(this).Value * (chartCount * 200 + 100))));

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }


            SetChart(chartCPU, AsusFan.CPU);
            SetChart(chartGPU, AsusFan.GPU);

            LoadProfile(seriesCPU, AsusFan.CPU);
            LoadProfile(seriesGPU, AsusFan.GPU);

            bool autoFans = AppConfig.IsMode("auto_apply_power") && AppConfig.IsFanRequired();
            bool applyFans = AppConfig.IsMode("auto_apply");

            checkApplyFans.Checked = applyFans;

            if (autoFans || applyFans)
            {
                seriesCPU.Color = colorStandard;
                seriesGPU.Color = colorTurbo;
                seriesMid.Color = colorEco;
                seriesXGM.Color = Color.Orange;
            }
            else
            {
                seriesCPU.Color = Color.Gray;
                seriesGPU.Color = Color.Gray;
                seriesMid.Color = Color.Gray;
                seriesXGM.Color = Color.Gray;
            }

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
                Logger.WriteLine($"Default Curve: {device} - {BitConverter.ToString(curve)}");
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
            seriesCPU.Color = Color.Gray;
            seriesGPU.Color = Color.Gray;
            seriesMid.Color = Color.Gray;
            seriesXGM.Color = Color.Gray;

            AppConfig.SetMode("auto_apply", 0);
            AppConfig.SetMode("auto_apply_power", 0);

            trackUV.Value = RyzenControl.MaxCPUUV;
            trackUViGPU.Value = RyzenControl.MaxIGPUUV;
            trackTemp.Value = RyzenControl.MaxTemp;

            AdvancedScroll();
            AppConfig.RemoveMode("cpu_temp");

            modeControl.ResetPerformanceMode();

            InitPowerPlan();

            if (Program.acpi.IsXGConnected()) XGM.Reset();


            if (gpuVisible)
            {
                trackGPUClockLimit.Value = NvidiaGpuControl.MaxClockLimit;
                trackGPUCore.Value = 0;
                trackGPUMemory.Value = 0;

                trackGPUBoost.Value = AsusACPI.MaxGPUBoost;
                trackGPUTemp.Value = AsusACPI.MaxGPUTemp;

                //AppConfig.SetMode("gpu_boost", trackGPUBoost.Value);
                //AppConfig.SetMode("gpu_temp", trackGPUTemp.Value);

                AppConfig.RemoveMode("gpu_boost");
                AppConfig.RemoveMode("gpu_temp");

                AppConfig.RemoveMode("gpu_power");
                AppConfig.RemoveMode("gpu_clock_limit");
                AppConfig.RemoveMode("gpu_core");
                AppConfig.RemoveMode("gpu_memory");

                InitGPUPower();

                VisualiseGPUSettings();
                modeControl.SetGPUClocks(true, true);
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

                    if (dx < tempMin) dx = tempMin;
                    if (dx > tempMax) dx = tempMax;

                    if (dy < 0) dy = 0;
                    if (dy > fansMax) dy = fansMax;

                    dymin = (dx - 70) * 1.2;

                    if (dy < dymin) dy = dymin;

                    if (e.Button.HasFlag(MouseButtons.Left))
                    {
                        double deltaY = dy - curPoint.YValues[0];
                        double deltaX = dx - curPoint.XValue;

                        if (clampFanDots)
                        {
                            double minX = 30 + (curIndex * 10);
                            double maxX = minX + 9;
                            dx = Math.Max(minX, Math.Min(maxX, dx));
                        }

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

                    labelTip.Text = Math.Floor(curPoint.XValue) + "C, " + ChartYLabel((int)curPoint.YValues[0], device, " " + Properties.Strings.RPM);
                    labelTip.Top = e.Y + ((Control)sender).Top;
                    labelTip.Left = Math.Min(chart.Width - labelTip.Width - 20, e.X - 50);

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
                series.Points[i].XValue = Math.Max(tempMin, Math.Min(tempMax, series.Points[i].XValue + deltaX));
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
