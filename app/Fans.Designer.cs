using CustomControls;
using System.Windows.Forms.DataVisualization.Charting;

namespace GHelper
{
    partial class Fans
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ChartArea chartArea5 = new ChartArea();
            Title title5 = new Title();
            ChartArea chartArea6 = new ChartArea();
            Title title6 = new Title();
            ChartArea chartArea7 = new ChartArea();
            Title title7 = new Title();
            ChartArea chartArea8 = new ChartArea();
            Title title8 = new Title();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Fans));
            panelFans = new Panel();
            labelTip = new Label();
            tableFanCharts = new TableLayoutPanel();
            chartGPU = new Chart();
            chartCPU = new Chart();
            chartXGM = new Chart();
            chartMid = new Chart();
            panelTitleFans = new Panel();
            buttonRename = new RButton();
            buttonRemove = new RButton();
            buttonAdd = new RButton();
            comboModes = new RComboBox();
            picturePerf = new PictureBox();
            labelFans = new Label();
            panelApplyFans = new Panel();
            labelFansResult = new Label();
            checkApplyFans = new RCheckBox();
            buttonReset = new RButton();
            labelBoost = new Label();
            comboBoost = new RComboBox();
            panelSliders = new Panel();
            panelPower = new Panel();
            panelApplyPower = new Panel();
            checkApplyPower = new RCheckBox();
            labelInfo = new Label();
            panelB0 = new Panel();
            labelB0 = new Label();
            labelLeftB0 = new Label();
            trackB0 = new TrackBar();
            panelC1 = new Panel();
            labelC1 = new Label();
            labelLeftC1 = new Label();
            trackC1 = new TrackBar();
            panelA0 = new Panel();
            labelA0 = new Label();
            labelLeftA0 = new Label();
            trackA0 = new TrackBar();
            panelBoost = new Panel();
            panelTitleCPU = new Panel();
            pictureBox1 = new PictureBox();
            labelPowerLimits = new Label();
            panelGPU = new Panel();
            panelGPUTemp = new Panel();
            labelGPUTemp = new Label();
            labelGPUTempTitle = new Label();
            trackGPUTemp = new TrackBar();
            panelGPUBoost = new Panel();
            labelGPUBoost = new Label();
            labelGPUBoostTitle = new Label();
            trackGPUBoost = new TrackBar();
            panelGPUMemory = new Panel();
            labelGPUMemory = new Label();
            labelGPUMemoryTitle = new Label();
            trackGPUMemory = new TrackBar();
            panelGPUCore = new Panel();
            labelGPUCore = new Label();
            trackGPUCore = new TrackBar();
            labelGPUCoreTitle = new Label();
            panelTitleGPU = new Panel();
            pictureGPU = new PictureBox();
            labelGPU = new Label();
            panelFans.SuspendLayout();
            tableFanCharts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)chartGPU).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chartCPU).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chartXGM).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chartMid).BeginInit();
            panelTitleFans.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picturePerf).BeginInit();
            panelApplyFans.SuspendLayout();
            panelSliders.SuspendLayout();
            panelPower.SuspendLayout();
            panelApplyPower.SuspendLayout();
            panelB0.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackB0).BeginInit();
            panelC1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackC1).BeginInit();
            panelA0.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackA0).BeginInit();
            panelBoost.SuspendLayout();
            panelTitleCPU.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panelGPU.SuspendLayout();
            panelGPUTemp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackGPUTemp).BeginInit();
            panelGPUBoost.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackGPUBoost).BeginInit();
            panelGPUMemory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackGPUMemory).BeginInit();
            panelGPUCore.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackGPUCore).BeginInit();
            panelTitleGPU.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureGPU).BeginInit();
            SuspendLayout();
            // 
            // panelFans
            // 
            panelFans.AutoSize = true;
            panelFans.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelFans.Controls.Add(labelTip);
            panelFans.Controls.Add(tableFanCharts);
            panelFans.Controls.Add(panelTitleFans);
            panelFans.Controls.Add(panelApplyFans);
            panelFans.Dock = DockStyle.Left;
            panelFans.Location = new Point(533, 0);
            panelFans.Margin = new Padding(0);
            panelFans.MaximumSize = new Size(815, 0);
            panelFans.MinimumSize = new Size(815, 0);
            panelFans.Name = "panelFans";
            panelFans.Padding = new Padding(0, 0, 10, 0);
            panelFans.Size = new Size(815, 1310);
            panelFans.TabIndex = 12;
            // 
            // labelTip
            // 
            labelTip.AutoSize = true;
            labelTip.BackColor = SystemColors.ControlLightLight;
            labelTip.Location = new Point(684, 91);
            labelTip.Name = "labelTip";
            labelTip.Padding = new Padding(5);
            labelTip.Size = new Size(107, 42);
            labelTip.TabIndex = 40;
            labelTip.Text = "500,300";
            // 
            // tableFanCharts
            // 
            tableFanCharts.AutoSize = true;
            tableFanCharts.ColumnCount = 1;
            tableFanCharts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableFanCharts.Controls.Add(chartGPU, 0, 1);
            tableFanCharts.Controls.Add(chartCPU, 0, 0);
            tableFanCharts.Controls.Add(chartXGM, 0, 2);
            tableFanCharts.Controls.Add(chartMid, 0, 2);
            tableFanCharts.Dock = DockStyle.Fill;
            tableFanCharts.Location = new Point(0, 66);
            tableFanCharts.Margin = new Padding(4);
            tableFanCharts.Name = "tableFanCharts";
            tableFanCharts.Padding = new Padding(10, 0, 10, 10);
            tableFanCharts.RowCount = 2;
            tableFanCharts.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableFanCharts.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableFanCharts.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableFanCharts.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableFanCharts.Size = new Size(805, 1128);
            tableFanCharts.TabIndex = 36;
            // 
            // chartGPU
            // 
            chartArea5.Name = "ChartArea1";
            chartGPU.ChartAreas.Add(chartArea5);
            chartGPU.Dock = DockStyle.Fill;
            chartGPU.Location = new Point(12, 289);
            chartGPU.Margin = new Padding(2, 10, 2, 10);
            chartGPU.Name = "chartGPU";
            chartGPU.Size = new Size(781, 259);
            chartGPU.TabIndex = 17;
            chartGPU.Text = "chartGPU";
            title5.Name = "Title1";
            chartGPU.Titles.Add(title5);
            // 
            // chartCPU
            // 
            chartArea6.Name = "ChartArea1";
            chartCPU.ChartAreas.Add(chartArea6);
            chartCPU.Dock = DockStyle.Fill;
            chartCPU.Location = new Point(12, 10);
            chartCPU.Margin = new Padding(2, 10, 2, 10);
            chartCPU.Name = "chartCPU";
            chartCPU.Size = new Size(781, 259);
            chartCPU.TabIndex = 14;
            chartCPU.Text = "chartCPU";
            title6.Name = "Title1";
            chartCPU.Titles.Add(title6);
            // 
            // chartXGM
            // 
            chartArea7.Name = "ChartAreaXGM";
            chartXGM.ChartAreas.Add(chartArea7);
            chartXGM.Dock = DockStyle.Fill;
            chartXGM.Location = new Point(12, 847);
            chartXGM.Margin = new Padding(2, 10, 2, 10);
            chartXGM.Name = "chartXGM";
            chartXGM.Size = new Size(781, 261);
            chartXGM.TabIndex = 14;
            chartXGM.Text = "chartXGM";
            title7.Name = "Title4";
            chartXGM.Titles.Add(title7);
            chartXGM.Visible = false;
            // 
            // chartMid
            // 
            chartArea8.Name = "ChartArea3";
            chartMid.ChartAreas.Add(chartArea8);
            chartMid.Dock = DockStyle.Fill;
            chartMid.Location = new Point(12, 568);
            chartMid.Margin = new Padding(2, 10, 2, 10);
            chartMid.Name = "chartMid";
            chartMid.Size = new Size(781, 259);
            chartMid.TabIndex = 14;
            chartMid.Text = "chartMid";
            title8.Name = "Title3";
            chartMid.Titles.Add(title8);
            chartMid.Visible = false;
            // 
            // panelTitleFans
            // 
            panelTitleFans.Controls.Add(buttonRename);
            panelTitleFans.Controls.Add(buttonRemove);
            panelTitleFans.Controls.Add(buttonAdd);
            panelTitleFans.Controls.Add(comboModes);
            panelTitleFans.Controls.Add(picturePerf);
            panelTitleFans.Controls.Add(labelFans);
            panelTitleFans.Dock = DockStyle.Top;
            panelTitleFans.Location = new Point(0, 0);
            panelTitleFans.Name = "panelTitleFans";
            panelTitleFans.Size = new Size(805, 66);
            panelTitleFans.TabIndex = 42;
            // 
            // buttonRename
            // 
            buttonRename.Activated = false;
            buttonRename.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonRename.BackColor = SystemColors.ControlLight;
            buttonRename.BorderColor = Color.Transparent;
            buttonRename.BorderRadius = 2;
            buttonRename.FlatStyle = FlatStyle.Flat;
            buttonRename.Image = (Image)resources.GetObject("buttonRename.Image");
            buttonRename.Location = new Point(369, 12);
            buttonRename.Margin = new Padding(4, 2, 4, 2);
            buttonRename.Name = "buttonRename";
            buttonRename.Secondary = true;
            buttonRename.Size = new Size(52, 46);
            buttonRename.TabIndex = 45;
            buttonRename.UseVisualStyleBackColor = false;
            // 
            // buttonRemove
            // 
            buttonRemove.Activated = false;
            buttonRemove.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonRemove.BackColor = SystemColors.ControlLight;
            buttonRemove.BorderColor = Color.Transparent;
            buttonRemove.BorderRadius = 2;
            buttonRemove.FlatStyle = FlatStyle.Flat;
            buttonRemove.Image = (Image)resources.GetObject("buttonRemove.Image");
            buttonRemove.Location = new Point(314, 12);
            buttonRemove.Margin = new Padding(4, 2, 4, 2);
            buttonRemove.Name = "buttonRemove";
            buttonRemove.Secondary = true;
            buttonRemove.Size = new Size(52, 46);
            buttonRemove.TabIndex = 44;
            buttonRemove.UseVisualStyleBackColor = false;
            // 
            // buttonAdd
            // 
            buttonAdd.Activated = false;
            buttonAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonAdd.BackColor = SystemColors.ControlLight;
            buttonAdd.BorderColor = Color.Transparent;
            buttonAdd.BorderRadius = 2;
            buttonAdd.FlatStyle = FlatStyle.Flat;
            buttonAdd.Image = (Image)resources.GetObject("buttonAdd.Image");
            buttonAdd.Location = new Point(737, 12);
            buttonAdd.Margin = new Padding(4, 2, 4, 2);
            buttonAdd.Name = "buttonAdd";
            buttonAdd.Secondary = true;
            buttonAdd.Size = new Size(52, 46);
            buttonAdd.TabIndex = 43;
            buttonAdd.UseVisualStyleBackColor = false;
            // 
            // comboModes
            // 
            comboModes.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            comboModes.BorderColor = Color.White;
            comboModes.ButtonColor = Color.FromArgb(255, 255, 255);
            comboModes.FlatStyle = FlatStyle.Flat;
            comboModes.FormattingEnabled = true;
            comboModes.Location = new Point(429, 16);
            comboModes.Name = "comboModes";
            comboModes.Size = new Size(302, 40);
            comboModes.TabIndex = 42;
            // 
            // picturePerf
            // 
            picturePerf.BackgroundImage = Properties.Resources.icons8_fan_head_96;
            picturePerf.BackgroundImageLayout = ImageLayout.Zoom;
            picturePerf.InitialImage = null;
            picturePerf.Location = new Point(18, 18);
            picturePerf.Margin = new Padding(4, 2, 4, 2);
            picturePerf.Name = "picturePerf";
            picturePerf.Size = new Size(36, 38);
            picturePerf.TabIndex = 41;
            picturePerf.TabStop = false;
            // 
            // labelFans
            // 
            labelFans.AutoSize = true;
            labelFans.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelFans.Location = new Point(53, 20);
            labelFans.Margin = new Padding(4, 0, 4, 0);
            labelFans.Name = "labelFans";
            labelFans.Size = new Size(90, 32);
            labelFans.TabIndex = 40;
            labelFans.Text = "Profile";
            // 
            // panelApplyFans
            // 
            panelApplyFans.Controls.Add(labelFansResult);
            panelApplyFans.Controls.Add(checkApplyFans);
            panelApplyFans.Controls.Add(buttonReset);
            panelApplyFans.Dock = DockStyle.Bottom;
            panelApplyFans.Location = new Point(0, 1194);
            panelApplyFans.Name = "panelApplyFans";
            panelApplyFans.Size = new Size(805, 116);
            panelApplyFans.TabIndex = 43;
            // 
            // labelFansResult
            // 
            labelFansResult.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            labelFansResult.ForeColor = Color.Red;
            labelFansResult.Location = new Point(25, 3);
            labelFansResult.Name = "labelFansResult";
            labelFansResult.Size = new Size(760, 32);
            labelFansResult.TabIndex = 42;
            labelFansResult.TextAlign = ContentAlignment.TopRight;
            labelFansResult.Visible = false;
            // 
            // checkApplyFans
            // 
            checkApplyFans.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            checkApplyFans.AutoSize = true;
            checkApplyFans.BackColor = SystemColors.ControlLight;
            checkApplyFans.Location = new Point(453, 45);
            checkApplyFans.Margin = new Padding(4, 2, 4, 2);
            checkApplyFans.Name = "checkApplyFans";
            checkApplyFans.Padding = new Padding(15, 5, 15, 5);
            checkApplyFans.Size = new Size(339, 46);
            checkApplyFans.TabIndex = 19;
            checkApplyFans.Text = Properties.Strings.ApplyFanCurve;
            checkApplyFans.UseVisualStyleBackColor = false;
            // 
            // buttonReset
            // 
            buttonReset.Activated = false;
            buttonReset.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonReset.BackColor = SystemColors.ControlLight;
            buttonReset.BorderColor = Color.Transparent;
            buttonReset.BorderRadius = 2;
            buttonReset.FlatStyle = FlatStyle.Flat;
            buttonReset.Location = new Point(12, 38);
            buttonReset.Margin = new Padding(4, 2, 4, 2);
            buttonReset.Name = "buttonReset";
            buttonReset.Secondary = true;
            buttonReset.Size = new Size(274, 54);
            buttonReset.TabIndex = 18;
            buttonReset.Text = Properties.Strings.FactoryDefaults;
            buttonReset.UseVisualStyleBackColor = false;
            // 
            // labelBoost
            // 
            labelBoost.Location = new Point(10, 12);
            labelBoost.Name = "labelBoost";
            labelBoost.Size = new Size(201, 40);
            labelBoost.TabIndex = 43;
            labelBoost.Text = "CPU Boost";
            labelBoost.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // comboBoost
            // 
            comboBoost.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            comboBoost.BorderColor = Color.White;
            comboBoost.ButtonColor = Color.FromArgb(255, 255, 255);
            comboBoost.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoost.FormattingEnabled = true;
            comboBoost.Items.AddRange(new object[] { "Disabled", "Enabled", "Aggressive", "Efficient Enabled", "Efficient Aggressive" });
            comboBoost.Location = new Point(226, 12);
            comboBoost.Name = "comboBoost";
            comboBoost.Size = new Size(287, 40);
            comboBoost.TabIndex = 42;
            // 
            // panelSliders
            // 
            panelSliders.Controls.Add(panelPower);
            panelSliders.Controls.Add(panelGPU);
            panelSliders.Dock = DockStyle.Left;
            panelSliders.Location = new Point(0, 0);
            panelSliders.Margin = new Padding(0);
            panelSliders.Name = "panelSliders";
            panelSliders.Padding = new Padding(10, 0, 0, 0);
            panelSliders.Size = new Size(533, 1310);
            panelSliders.TabIndex = 13;
            // 
            // panelPower
            // 
            panelPower.AutoSize = true;
            panelPower.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelPower.Controls.Add(panelApplyPower);
            panelPower.Controls.Add(labelInfo);
            panelPower.Controls.Add(panelB0);
            panelPower.Controls.Add(panelC1);
            panelPower.Controls.Add(panelA0);
            panelPower.Controls.Add(panelBoost);
            panelPower.Controls.Add(panelTitleCPU);
            panelPower.Dock = DockStyle.Fill;
            panelPower.Location = new Point(10, 584);
            panelPower.Name = "panelPower";
            panelPower.Size = new Size(523, 726);
            panelPower.TabIndex = 43;
            // 
            // panelApplyPower
            // 
            panelApplyPower.Controls.Add(checkApplyPower);
            panelApplyPower.Dock = DockStyle.Bottom;
            panelApplyPower.Location = new Point(0, 636);
            panelApplyPower.Name = "panelApplyPower";
            panelApplyPower.Padding = new Padding(10);
            panelApplyPower.Size = new Size(523, 90);
            panelApplyPower.TabIndex = 44;
            // 
            // checkApplyPower
            // 
            checkApplyPower.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            checkApplyPower.AutoSize = true;
            checkApplyPower.BackColor = SystemColors.ControlLight;
            checkApplyPower.Location = new Point(18, 20);
            checkApplyPower.Margin = new Padding(10);
            checkApplyPower.Name = "checkApplyPower";
            checkApplyPower.Padding = new Padding(15, 5, 15, 5);
            checkApplyPower.Size = new Size(277, 46);
            checkApplyPower.TabIndex = 45;
            checkApplyPower.Text = Properties.Strings.ApplyPowerLimits;
            checkApplyPower.UseVisualStyleBackColor = false;
            // 
            // labelInfo
            // 
            labelInfo.Dock = DockStyle.Top;
            labelInfo.Location = new Point(0, 506);
            labelInfo.Margin = new Padding(4, 0, 4, 0);
            labelInfo.Name = "labelInfo";
            labelInfo.Padding = new Padding(5);
            labelInfo.Size = new Size(523, 100);
            labelInfo.TabIndex = 43;
            labelInfo.Text = "Experimental Feature";
            // 
            // panelB0
            // 
            panelB0.AutoSize = true;
            panelB0.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelB0.Controls.Add(labelB0);
            panelB0.Controls.Add(labelLeftB0);
            panelB0.Controls.Add(trackB0);
            panelB0.Dock = DockStyle.Top;
            panelB0.Location = new Point(0, 381);
            panelB0.Margin = new Padding(4);
            panelB0.MaximumSize = new Size(0, 125);
            panelB0.Name = "panelB0";
            panelB0.Size = new Size(523, 125);
            panelB0.TabIndex = 41;
            // 
            // labelB0
            // 
            labelB0.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelB0.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelB0.Location = new Point(398, 8);
            labelB0.Margin = new Padding(4, 0, 4, 0);
            labelB0.Name = "labelB0";
            labelB0.Size = new Size(120, 32);
            labelB0.TabIndex = 13;
            labelB0.Text = "CPU";
            labelB0.TextAlign = ContentAlignment.TopRight;
            // 
            // labelLeftB0
            // 
            labelLeftB0.AutoSize = true;
            labelLeftB0.Location = new Point(10, 8);
            labelLeftB0.Margin = new Padding(4, 0, 4, 0);
            labelLeftB0.Name = "labelLeftB0";
            labelLeftB0.Size = new Size(58, 32);
            labelLeftB0.TabIndex = 12;
            labelLeftB0.Text = "CPU";
            // 
            // trackB0
            // 
            trackB0.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackB0.Location = new Point(6, 44);
            trackB0.Margin = new Padding(4, 2, 4, 2);
            trackB0.Maximum = 85;
            trackB0.Minimum = 5;
            trackB0.Name = "trackB0";
            trackB0.Size = new Size(513, 90);
            trackB0.TabIndex = 11;
            trackB0.TickFrequency = 5;
            trackB0.TickStyle = TickStyle.TopLeft;
            trackB0.Value = 80;
            // 
            // panelC1
            // 
            panelC1.AutoSize = true;
            panelC1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelC1.Controls.Add(labelC1);
            panelC1.Controls.Add(labelLeftC1);
            panelC1.Controls.Add(trackC1);
            panelC1.Dock = DockStyle.Top;
            panelC1.Location = new Point(0, 256);
            panelC1.Margin = new Padding(4);
            panelC1.MaximumSize = new Size(0, 125);
            panelC1.Name = "panelC1";
            panelC1.Size = new Size(523, 125);
            panelC1.TabIndex = 45;
            // 
            // labelC1
            // 
            labelC1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelC1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelC1.Location = new Point(396, 8);
            labelC1.Margin = new Padding(4, 0, 4, 0);
            labelC1.Name = "labelC1";
            labelC1.Size = new Size(119, 32);
            labelC1.TabIndex = 13;
            labelC1.Text = "C1";
            labelC1.TextAlign = ContentAlignment.TopRight;
            // 
            // labelLeftC1
            // 
            labelLeftC1.AutoSize = true;
            labelLeftC1.Location = new Point(10, 8);
            labelLeftC1.Margin = new Padding(4, 0, 4, 0);
            labelLeftC1.Name = "labelLeftC1";
            labelLeftC1.Size = new Size(42, 32);
            labelLeftC1.TabIndex = 12;
            labelLeftC1.Text = "C1";
            // 
            // trackC1
            // 
            trackC1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackC1.Location = new Point(6, 48);
            trackC1.Margin = new Padding(4, 2, 4, 2);
            trackC1.Maximum = 85;
            trackC1.Minimum = 5;
            trackC1.Name = "trackC1";
            trackC1.Size = new Size(513, 90);
            trackC1.TabIndex = 11;
            trackC1.TickFrequency = 5;
            trackC1.TickStyle = TickStyle.TopLeft;
            trackC1.Value = 80;
            // 
            // panelA0
            // 
            panelA0.AutoSize = true;
            panelA0.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelA0.Controls.Add(labelA0);
            panelA0.Controls.Add(labelLeftA0);
            panelA0.Controls.Add(trackA0);
            panelA0.Dock = DockStyle.Top;
            panelA0.Location = new Point(0, 131);
            panelA0.Margin = new Padding(4);
            panelA0.MaximumSize = new Size(0, 125);
            panelA0.Name = "panelA0";
            panelA0.Size = new Size(523, 125);
            panelA0.TabIndex = 40;
            // 
            // labelA0
            // 
            labelA0.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelA0.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelA0.Location = new Point(396, 10);
            labelA0.Margin = new Padding(4, 0, 4, 0);
            labelA0.Name = "labelA0";
            labelA0.Size = new Size(122, 32);
            labelA0.TabIndex = 12;
            labelA0.Text = "Platform";
            labelA0.TextAlign = ContentAlignment.TopRight;
            // 
            // labelLeftA0
            // 
            labelLeftA0.AutoSize = true;
            labelLeftA0.Location = new Point(10, 10);
            labelLeftA0.Margin = new Padding(4, 0, 4, 0);
            labelLeftA0.Name = "labelLeftA0";
            labelLeftA0.Size = new Size(104, 32);
            labelLeftA0.TabIndex = 11;
            labelLeftA0.Text = "Platform";
            // 
            // trackA0
            // 
            trackA0.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackA0.Location = new Point(6, 48);
            trackA0.Margin = new Padding(4, 2, 4, 2);
            trackA0.Maximum = 180;
            trackA0.Minimum = 10;
            trackA0.Name = "trackA0";
            trackA0.Size = new Size(513, 90);
            trackA0.TabIndex = 10;
            trackA0.TickFrequency = 5;
            trackA0.TickStyle = TickStyle.TopLeft;
            trackA0.Value = 125;
            // 
            // panelBoost
            // 
            panelBoost.Controls.Add(comboBoost);
            panelBoost.Controls.Add(labelBoost);
            panelBoost.Dock = DockStyle.Top;
            panelBoost.Location = new Point(0, 66);
            panelBoost.Name = "panelBoost";
            panelBoost.Size = new Size(523, 65);
            panelBoost.TabIndex = 13;
            // 
            // panelTitleCPU
            // 
            panelTitleCPU.AutoSize = true;
            panelTitleCPU.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelTitleCPU.Controls.Add(pictureBox1);
            panelTitleCPU.Controls.Add(labelPowerLimits);
            panelTitleCPU.Dock = DockStyle.Top;
            panelTitleCPU.Location = new Point(0, 0);
            panelTitleCPU.Name = "panelTitleCPU";
            panelTitleCPU.Size = new Size(523, 66);
            panelTitleCPU.TabIndex = 42;
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = Properties.Resources.icons8_processor_96;
            pictureBox1.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox1.InitialImage = null;
            pictureBox1.Location = new Point(16, 18);
            pictureBox1.Margin = new Padding(4, 2, 4, 10);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(36, 38);
            pictureBox1.TabIndex = 40;
            pictureBox1.TabStop = false;
            // 
            // labelPowerLimits
            // 
            labelPowerLimits.AutoSize = true;
            labelPowerLimits.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelPowerLimits.Location = new Point(53, 20);
            labelPowerLimits.Margin = new Padding(4, 0, 4, 0);
            labelPowerLimits.Name = "labelPowerLimits";
            labelPowerLimits.Size = new Size(160, 32);
            labelPowerLimits.TabIndex = 39;
            labelPowerLimits.Text = "Power Limits";
            // 
            // panelGPU
            // 
            panelGPU.AutoSize = true;
            panelGPU.Controls.Add(panelGPUTemp);
            panelGPU.Controls.Add(panelGPUBoost);
            panelGPU.Controls.Add(panelGPUMemory);
            panelGPU.Controls.Add(panelGPUCore);
            panelGPU.Controls.Add(panelTitleGPU);
            panelGPU.Dock = DockStyle.Top;
            panelGPU.Location = new Point(10, 0);
            panelGPU.Name = "panelGPU";
            panelGPU.Padding = new Padding(0, 0, 0, 18);
            panelGPU.Size = new Size(523, 584);
            panelGPU.TabIndex = 44;
            // 
            // panelGPUTemp
            // 
            panelGPUTemp.AutoSize = true;
            panelGPUTemp.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelGPUTemp.Controls.Add(labelGPUTemp);
            panelGPUTemp.Controls.Add(labelGPUTempTitle);
            panelGPUTemp.Controls.Add(trackGPUTemp);
            panelGPUTemp.Dock = DockStyle.Top;
            panelGPUTemp.Location = new Point(0, 441);
            panelGPUTemp.MaximumSize = new Size(0, 125);
            panelGPUTemp.Name = "panelGPUTemp";
            panelGPUTemp.Size = new Size(523, 125);
            panelGPUTemp.TabIndex = 47;
            // 
            // labelGPUTemp
            // 
            labelGPUTemp.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelGPUTemp.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelGPUTemp.Location = new Point(378, 14);
            labelGPUTemp.Name = "labelGPUTemp";
            labelGPUTemp.Size = new Size(130, 32);
            labelGPUTemp.TabIndex = 44;
            labelGPUTemp.Text = "87C";
            labelGPUTemp.TextAlign = ContentAlignment.TopRight;
            // 
            // labelGPUTempTitle
            // 
            labelGPUTempTitle.AutoSize = true;
            labelGPUTempTitle.Location = new Point(10, 14);
            labelGPUTempTitle.Name = "labelGPUTempTitle";
            labelGPUTempTitle.Size = new Size(173, 32);
            labelGPUTempTitle.TabIndex = 43;
            labelGPUTempTitle.Text = "Thermal Target";
            // 
            // trackGPUTemp
            // 
            trackGPUTemp.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackGPUTemp.Location = new Point(6, 57);
            trackGPUTemp.Margin = new Padding(4, 2, 4, 2);
            trackGPUTemp.Maximum = 87;
            trackGPUTemp.Minimum = 75;
            trackGPUTemp.Name = "trackGPUTemp";
            trackGPUTemp.Size = new Size(502, 90);
            trackGPUTemp.TabIndex = 42;
            trackGPUTemp.TickFrequency = 5;
            trackGPUTemp.TickStyle = TickStyle.TopLeft;
            trackGPUTemp.Value = 87;
            // 
            // panelGPUBoost
            // 
            panelGPUBoost.AutoSize = true;
            panelGPUBoost.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelGPUBoost.Controls.Add(labelGPUBoost);
            panelGPUBoost.Controls.Add(labelGPUBoostTitle);
            panelGPUBoost.Controls.Add(trackGPUBoost);
            panelGPUBoost.Dock = DockStyle.Top;
            panelGPUBoost.Location = new Point(0, 316);
            panelGPUBoost.MaximumSize = new Size(0, 125);
            panelGPUBoost.Name = "panelGPUBoost";
            panelGPUBoost.Size = new Size(523, 125);
            panelGPUBoost.TabIndex = 46;
            // 
            // labelGPUBoost
            // 
            labelGPUBoost.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelGPUBoost.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelGPUBoost.Location = new Point(374, 14);
            labelGPUBoost.Name = "labelGPUBoost";
            labelGPUBoost.Size = new Size(130, 32);
            labelGPUBoost.TabIndex = 44;
            labelGPUBoost.Text = "25W";
            labelGPUBoost.TextAlign = ContentAlignment.TopRight;
            // 
            // labelGPUBoostTitle
            // 
            labelGPUBoostTitle.AutoSize = true;
            labelGPUBoostTitle.Location = new Point(10, 14);
            labelGPUBoostTitle.Name = "labelGPUBoostTitle";
            labelGPUBoostTitle.Size = new Size(174, 32);
            labelGPUBoostTitle.TabIndex = 43;
            labelGPUBoostTitle.Text = "Dynamic Boost";
            // 
            // trackGPUBoost
            // 
            trackGPUBoost.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackGPUBoost.Location = new Point(6, 48);
            trackGPUBoost.Margin = new Padding(4, 2, 4, 2);
            trackGPUBoost.Maximum = 25;
            trackGPUBoost.Minimum = 5;
            trackGPUBoost.Name = "trackGPUBoost";
            trackGPUBoost.Size = new Size(502, 90);
            trackGPUBoost.TabIndex = 42;
            trackGPUBoost.TickFrequency = 5;
            trackGPUBoost.TickStyle = TickStyle.TopLeft;
            trackGPUBoost.Value = 25;
            // 
            // panelGPUMemory
            // 
            panelGPUMemory.AutoSize = true;
            panelGPUMemory.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelGPUMemory.Controls.Add(labelGPUMemory);
            panelGPUMemory.Controls.Add(labelGPUMemoryTitle);
            panelGPUMemory.Controls.Add(trackGPUMemory);
            panelGPUMemory.Dock = DockStyle.Top;
            panelGPUMemory.Location = new Point(0, 191);
            panelGPUMemory.MaximumSize = new Size(0, 125);
            panelGPUMemory.Name = "panelGPUMemory";
            panelGPUMemory.Size = new Size(523, 125);
            panelGPUMemory.TabIndex = 45;
            // 
            // labelGPUMemory
            // 
            labelGPUMemory.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelGPUMemory.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelGPUMemory.Location = new Point(378, 14);
            labelGPUMemory.Name = "labelGPUMemory";
            labelGPUMemory.Size = new Size(130, 32);
            labelGPUMemory.TabIndex = 44;
            labelGPUMemory.Text = "2000 MHz";
            labelGPUMemory.TextAlign = ContentAlignment.TopRight;
            // 
            // labelGPUMemoryTitle
            // 
            labelGPUMemoryTitle.AutoSize = true;
            labelGPUMemoryTitle.Location = new Point(10, 14);
            labelGPUMemoryTitle.Name = "labelGPUMemoryTitle";
            labelGPUMemoryTitle.Size = new Size(241, 32);
            labelGPUMemoryTitle.TabIndex = 43;
            labelGPUMemoryTitle.Text = "Memory Clock Offset";
            // 
            // trackGPUMemory
            // 
            trackGPUMemory.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackGPUMemory.LargeChange = 100;
            trackGPUMemory.Location = new Point(6, 48);
            trackGPUMemory.Margin = new Padding(4, 2, 4, 2);
            trackGPUMemory.Maximum = 300;
            trackGPUMemory.Name = "trackGPUMemory";
            trackGPUMemory.Size = new Size(502, 90);
            trackGPUMemory.SmallChange = 10;
            trackGPUMemory.TabIndex = 42;
            trackGPUMemory.TickFrequency = 50;
            trackGPUMemory.TickStyle = TickStyle.TopLeft;
            // 
            // panelGPUCore
            // 
            panelGPUCore.AutoSize = true;
            panelGPUCore.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelGPUCore.Controls.Add(labelGPUCore);
            panelGPUCore.Controls.Add(trackGPUCore);
            panelGPUCore.Controls.Add(labelGPUCoreTitle);
            panelGPUCore.Dock = DockStyle.Top;
            panelGPUCore.Location = new Point(0, 66);
            panelGPUCore.MaximumSize = new Size(0, 125);
            panelGPUCore.Name = "panelGPUCore";
            panelGPUCore.Size = new Size(523, 125);
            panelGPUCore.TabIndex = 44;
            // 
            // labelGPUCore
            // 
            labelGPUCore.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelGPUCore.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelGPUCore.Location = new Point(378, 15);
            labelGPUCore.Name = "labelGPUCore";
            labelGPUCore.Size = new Size(130, 32);
            labelGPUCore.TabIndex = 29;
            labelGPUCore.Text = "1500 MHz";
            labelGPUCore.TextAlign = ContentAlignment.TopRight;
            // 
            // trackGPUCore
            // 
            trackGPUCore.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackGPUCore.LargeChange = 100;
            trackGPUCore.Location = new Point(6, 47);
            trackGPUCore.Margin = new Padding(4, 2, 4, 2);
            trackGPUCore.Maximum = 300;
            trackGPUCore.Name = "trackGPUCore";
            trackGPUCore.RightToLeft = RightToLeft.No;
            trackGPUCore.Size = new Size(502, 90);
            trackGPUCore.SmallChange = 10;
            trackGPUCore.TabIndex = 18;
            trackGPUCore.TickFrequency = 50;
            trackGPUCore.TickStyle = TickStyle.TopLeft;
            // 
            // labelGPUCoreTitle
            // 
            labelGPUCoreTitle.AutoSize = true;
            labelGPUCoreTitle.Location = new Point(10, 15);
            labelGPUCoreTitle.Name = "labelGPUCoreTitle";
            labelGPUCoreTitle.Size = new Size(201, 32);
            labelGPUCoreTitle.TabIndex = 17;
            labelGPUCoreTitle.Text = "Core Clock Offset";
            // 
            // panelTitleGPU
            // 
            panelTitleGPU.AutoSize = true;
            panelTitleGPU.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelTitleGPU.Controls.Add(pictureGPU);
            panelTitleGPU.Controls.Add(labelGPU);
            panelTitleGPU.Dock = DockStyle.Top;
            panelTitleGPU.Location = new Point(0, 0);
            panelTitleGPU.Name = "panelTitleGPU";
            panelTitleGPU.Size = new Size(523, 66);
            panelTitleGPU.TabIndex = 43;
            // 
            // pictureGPU
            // 
            pictureGPU.BackgroundImage = Properties.Resources.icons8_video_card_48;
            pictureGPU.BackgroundImageLayout = ImageLayout.Zoom;
            pictureGPU.ErrorImage = null;
            pictureGPU.InitialImage = null;
            pictureGPU.Location = new Point(16, 18);
            pictureGPU.Margin = new Padding(4, 2, 4, 10);
            pictureGPU.Name = "pictureGPU";
            pictureGPU.Size = new Size(36, 38);
            pictureGPU.TabIndex = 41;
            pictureGPU.TabStop = false;
            // 
            // labelGPU
            // 
            labelGPU.AutoSize = true;
            labelGPU.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelGPU.Location = new Point(52, 20);
            labelGPU.Margin = new Padding(4, 0, 4, 0);
            labelGPU.Name = "labelGPU";
            labelGPU.Size = new Size(162, 32);
            labelGPU.TabIndex = 40;
            labelGPU.Text = "GPU Settings";
            // 
            // Fans
            // 
            AutoScaleDimensions = new SizeF(192F, 192F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(1340, 1310);
            Controls.Add(panelFans);
            Controls.Add(panelSliders);
            Margin = new Padding(4, 2, 4, 2);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(0, 1200);
            Name = "Fans";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Text = "Fans and Power";
            panelFans.ResumeLayout(false);
            panelFans.PerformLayout();
            tableFanCharts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)chartGPU).EndInit();
            ((System.ComponentModel.ISupportInitialize)chartCPU).EndInit();
            ((System.ComponentModel.ISupportInitialize)chartXGM).EndInit();
            ((System.ComponentModel.ISupportInitialize)chartMid).EndInit();
            panelTitleFans.ResumeLayout(false);
            panelTitleFans.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picturePerf).EndInit();
            panelApplyFans.ResumeLayout(false);
            panelApplyFans.PerformLayout();
            panelSliders.ResumeLayout(false);
            panelSliders.PerformLayout();
            panelPower.ResumeLayout(false);
            panelPower.PerformLayout();
            panelApplyPower.ResumeLayout(false);
            panelApplyPower.PerformLayout();
            panelB0.ResumeLayout(false);
            panelB0.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackB0).EndInit();
            panelC1.ResumeLayout(false);
            panelC1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackC1).EndInit();
            panelA0.ResumeLayout(false);
            panelA0.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackA0).EndInit();
            panelBoost.ResumeLayout(false);
            panelTitleCPU.ResumeLayout(false);
            panelTitleCPU.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panelGPU.ResumeLayout(false);
            panelGPU.PerformLayout();
            panelGPUTemp.ResumeLayout(false);
            panelGPUTemp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackGPUTemp).EndInit();
            panelGPUBoost.ResumeLayout(false);
            panelGPUBoost.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackGPUBoost).EndInit();
            panelGPUMemory.ResumeLayout(false);
            panelGPUMemory.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackGPUMemory).EndInit();
            panelGPUCore.ResumeLayout(false);
            panelGPUCore.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackGPUCore).EndInit();
            panelTitleGPU.ResumeLayout(false);
            panelTitleGPU.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureGPU).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Panel panelFans;
        private Panel panelSliders;
        private TableLayoutPanel tableFanCharts;
        private Chart chartGPU;
        private Chart chartCPU;
        private Chart chartMid;
        private Chart chartXGM;
        private Label labelTip;
        private Panel panelPower;
        private Label labelInfo;
        private Panel panelB0;
        private Label labelB0;
        private Label labelLeftB0;
        private TrackBar trackB0;
        private Panel panelA0;
        private Label labelA0;
        private Label labelLeftA0;
        private TrackBar trackA0;
        private Panel panelTitleCPU;
        private PictureBox pictureBox1;
        private Label labelPowerLimits;
        private Panel panelGPU;
        private Panel panelGPUMemory;
        private Label labelGPUMemory;
        private Label labelGPUMemoryTitle;
        private TrackBar trackGPUMemory;
        private Panel panelGPUCore;
        private Label labelGPUCore;
        private TrackBar trackGPUCore;
        private Label labelGPUCoreTitle;
        private Panel panelTitleGPU;
        private PictureBox pictureGPU;
        private Label labelGPU;
        private Panel panelApplyPower;
        private RCheckBox checkApplyPower;
        private Panel panelGPUBoost;
        private Label labelGPUBoost;
        private Label labelGPUBoostTitle;
        private TrackBar trackGPUBoost;
        private Panel panelGPUTemp;
        private Label labelGPUTemp;
        private Label labelGPUTempTitle;
        private TrackBar trackGPUTemp;
        private Panel panelTitleFans;
        private Panel panelApplyFans;
        private Label labelFansResult;
        private RCheckBox checkApplyFans;
        private RButton buttonReset;
        private Label labelBoost;
        private RComboBox comboBoost;
        private PictureBox picturePerf;
        private Label labelFans;
        private Panel panelC1;
        private Label labelC1;
        private Label labelLeftC1;
        private TrackBar trackC1;
        private Panel panelBoost;
        private RComboBox comboModes;
        private RButton buttonAdd;
        private RButton buttonRemove;
        private RButton buttonRename;
    }
}