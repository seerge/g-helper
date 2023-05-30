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
            ChartArea chartArea9 = new ChartArea();
            Title title9 = new Title();
            ChartArea chartArea10 = new ChartArea();
            Title title10 = new Title();
            ChartArea chartArea11 = new ChartArea();
            Title title11 = new Title();
            ChartArea chartArea12 = new ChartArea();
            Title title12 = new Title();
            panelFans = new Panel();
            labelTip = new Label();
            tableFanCharts = new TableLayoutPanel();
            chartGPU = new Chart();
            chartCPU = new Chart();
            chartXGM = new Chart();
            chartMid = new Chart();
            panelTitleFans = new Panel();
            labelBoost = new Label();
            comboBoost = new RComboBox();
            picturePerf = new PictureBox();
            labelFans = new Label();
            panelApplyFans = new Panel();
            labelFansResult = new Label();
            checkApplyFans = new RCheckBox();
            buttonReset = new RButton();
            panelSliders = new Panel();
            panelPower = new Panel();
            panelApplyPower = new Panel();
            checkApplyPower = new RCheckBox();
            labelInfo = new Label();
            panelAPU = new Panel();
            labelAPU = new Label();
            labelLeftAPU = new Label();
            trackAPU = new TrackBar();
            panelCPU = new Panel();
            labelCPU = new Label();
            labelLeftCPU = new Label();
            trackCPU = new TrackBar();
            panelTotal = new Panel();
            labelTotal = new Label();
            labelLeftPlatform = new Label();
            trackTotal = new TrackBar();
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
            panelAPU.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackAPU).BeginInit();
            panelCPU.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackCPU).BeginInit();
            panelTotal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackTotal).BeginInit();
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
            chartArea9.Name = "ChartArea1";
            chartGPU.ChartAreas.Add(chartArea9);
            chartGPU.Dock = DockStyle.Fill;
            chartGPU.Location = new Point(12, 289);
            chartGPU.Margin = new Padding(2, 10, 2, 10);
            chartGPU.Name = "chartGPU";
            chartGPU.Size = new Size(781, 259);
            chartGPU.TabIndex = 17;
            chartGPU.Text = "chartGPU";
            title9.Name = "Title1";
            chartGPU.Titles.Add(title9);
            // 
            // chartCPU
            // 
            chartArea10.Name = "ChartArea1";
            chartCPU.ChartAreas.Add(chartArea10);
            chartCPU.Dock = DockStyle.Fill;
            chartCPU.Location = new Point(12, 10);
            chartCPU.Margin = new Padding(2, 10, 2, 10);
            chartCPU.Name = "chartCPU";
            chartCPU.Size = new Size(781, 259);
            chartCPU.TabIndex = 14;
            chartCPU.Text = "chartCPU";
            title10.Name = "Title1";
            chartCPU.Titles.Add(title10);
            // 
            // chartXGM
            // 
            chartArea11.Name = "ChartAreaXGM";
            chartXGM.ChartAreas.Add(chartArea11);
            chartXGM.Dock = DockStyle.Fill;
            chartXGM.Location = new Point(12, 847);
            chartXGM.Margin = new Padding(2, 10, 2, 10);
            chartXGM.Name = "chartXGM";
            chartXGM.Size = new Size(781, 261);
            chartXGM.TabIndex = 14;
            chartXGM.Text = "chartXGM";
            title11.Name = "Title4";
            chartXGM.Titles.Add(title11);
            chartXGM.Visible = false;
            // 
            // chartMid
            // 
            chartArea12.Name = "ChartArea3";
            chartMid.ChartAreas.Add(chartArea12);
            chartMid.Dock = DockStyle.Fill;
            chartMid.Location = new Point(12, 568);
            chartMid.Margin = new Padding(2, 10, 2, 10);
            chartMid.Name = "chartMid";
            chartMid.Size = new Size(781, 259);
            chartMid.TabIndex = 14;
            chartMid.Text = "chartMid";
            title12.Name = "Title3";
            chartMid.Titles.Add(title12);
            chartMid.Visible = false;
            // 
            // panelTitleFans
            // 
            panelTitleFans.Controls.Add(labelBoost);
            panelTitleFans.Controls.Add(comboBoost);
            panelTitleFans.Controls.Add(picturePerf);
            panelTitleFans.Controls.Add(labelFans);
            panelTitleFans.Dock = DockStyle.Top;
            panelTitleFans.Location = new Point(0, 0);
            panelTitleFans.Name = "panelTitleFans";
            panelTitleFans.Size = new Size(805, 66);
            panelTitleFans.TabIndex = 42;
            // 
            // labelBoost
            // 
            labelBoost.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelBoost.Location = new Point(356, 20);
            labelBoost.Name = "labelBoost";
            labelBoost.Size = new Size(140, 32);
            labelBoost.TabIndex = 43;
            labelBoost.Text = "CPU Boost";
            labelBoost.TextAlign = ContentAlignment.MiddleRight;
            // 
            // comboBoost
            // 
            comboBoost.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            comboBoost.BorderColor = Color.White;
            comboBoost.ButtonColor = Color.FromArgb(255, 255, 255);
            comboBoost.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoost.FormattingEnabled = true;
            comboBoost.Items.AddRange(new object[] { "Disabled", "Enabled", "Aggressive", "Efficient Enabled", "Efficient Aggressive" });
            comboBoost.Location = new Point(506, 16);
            comboBoost.Name = "comboBoost";
            comboBoost.Size = new Size(287, 40);
            comboBoost.TabIndex = 42;
            // 
            // picturePerf
            // 
            picturePerf.BackgroundImage = Properties.Resources.icons8_fan_head_96;
            picturePerf.BackgroundImageLayout = ImageLayout.Zoom;
            picturePerf.InitialImage = null;
            picturePerf.Location = new Point(20, 18);
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
            labelFans.Location = new Point(62, 20);
            labelFans.Margin = new Padding(4, 0, 4, 0);
            labelFans.Name = "labelFans";
            labelFans.Size = new Size(138, 32);
            labelFans.TabIndex = 40;
            labelFans.Text = "Fan Curves";
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
            panelPower.Controls.Add(panelCPU);
            panelPower.Controls.Add(panelAPU);
            panelPower.Controls.Add(panelTotal);
            panelPower.Controls.Add(panelTitleCPU);
            panelPower.Dock = DockStyle.Fill;
            panelPower.Location = new Point(10, 652);
            panelPower.Name = "panelPower";
            panelPower.Size = new Size(523, 658);
            panelPower.TabIndex = 43;
            // 
            // panelApplyPower
            // 
            panelApplyPower.Controls.Add(checkApplyPower);
            panelApplyPower.Dock = DockStyle.Bottom;
            panelApplyPower.Location = new Point(0, 568);
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
            labelInfo.Location = new Point(0, 482);
            labelInfo.Margin = new Padding(4, 0, 4, 0);
            labelInfo.Name = "labelInfo";
            labelInfo.Padding = new Padding(5);
            labelInfo.Size = new Size(523, 149);
            labelInfo.TabIndex = 43;
            labelInfo.Text = "Experimental Feature";
            // 
            // panelAPU
            // 
            panelAPU.AutoSize = true;
            panelAPU.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelAPU.Controls.Add(labelAPU);
            panelAPU.Controls.Add(labelLeftAPU);
            panelAPU.Controls.Add(trackAPU);
            panelAPU.Dock = DockStyle.Top;
            panelAPU.Location = new Point(0, 206);
            panelAPU.Margin = new Padding(4);
            panelAPU.Name = "panelAPU";
            panelAPU.Size = new Size(523, 140);
            panelAPU.TabIndex = 45;
            // 
            // labelAPU
            // 
            labelAPU.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelAPU.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelAPU.Location = new Point(396, 8);
            labelAPU.Margin = new Padding(4, 0, 4, 0);
            labelAPU.Name = "labelAPU";
            labelAPU.Size = new Size(119, 32);
            labelAPU.TabIndex = 13;
            labelAPU.Text = "APU";
            labelAPU.TextAlign = ContentAlignment.TopRight;
            // 
            // labelLeftAPU
            // 
            labelLeftAPU.AutoSize = true;
            labelLeftAPU.Location = new Point(10, 8);
            labelLeftAPU.Margin = new Padding(4, 0, 4, 0);
            labelLeftAPU.Name = "labelLeftAPU";
            labelLeftAPU.Size = new Size(58, 32);
            labelLeftAPU.TabIndex = 12;
            labelLeftAPU.Text = "APU";
            // 
            // trackAPU
            // 
            trackAPU.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackAPU.Location = new Point(6, 48);
            trackAPU.Margin = new Padding(4, 2, 4, 2);
            trackAPU.Maximum = 85;
            trackAPU.Minimum = 5;
            trackAPU.Name = "trackAPU";
            trackAPU.Size = new Size(513, 90);
            trackAPU.TabIndex = 11;
            trackAPU.TickFrequency = 5;
            trackAPU.TickStyle = TickStyle.TopLeft;
            trackAPU.Value = 80;
            // 
            // panelCPU
            // 
            panelCPU.AutoSize = true;
            panelCPU.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelCPU.Controls.Add(labelCPU);
            panelCPU.Controls.Add(labelLeftCPU);
            panelCPU.Controls.Add(trackCPU);
            panelCPU.Dock = DockStyle.Top;
            panelCPU.Location = new Point(0, 346);
            panelCPU.Margin = new Padding(4);
            panelCPU.Name = "panelCPU";
            panelCPU.Size = new Size(523, 136);
            panelCPU.TabIndex = 41;
            // 
            // labelCPU
            // 
            labelCPU.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelCPU.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelCPU.Location = new Point(398, 8);
            labelCPU.Margin = new Padding(4, 0, 4, 0);
            labelCPU.Name = "labelCPU";
            labelCPU.Size = new Size(120, 32);
            labelCPU.TabIndex = 13;
            labelCPU.Text = "CPU";
            labelCPU.TextAlign = ContentAlignment.TopRight;
            // 
            // labelLeftCPU
            // 
            labelLeftCPU.AutoSize = true;
            labelLeftCPU.Location = new Point(10, 8);
            labelLeftCPU.Margin = new Padding(4, 0, 4, 0);
            labelLeftCPU.Name = "labelLeftCPU";
            labelLeftCPU.Size = new Size(58, 32);
            labelLeftCPU.TabIndex = 12;
            labelLeftCPU.Text = "CPU";
            // 
            // trackCPU
            // 
            trackCPU.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackCPU.Location = new Point(6, 44);
            trackCPU.Margin = new Padding(4, 2, 4, 2);
            trackCPU.Maximum = 85;
            trackCPU.Minimum = 5;
            trackCPU.Name = "trackCPU";
            trackCPU.Size = new Size(513, 90);
            trackCPU.TabIndex = 11;
            trackCPU.TickFrequency = 5;
            trackCPU.TickStyle = TickStyle.TopLeft;
            trackCPU.Value = 80;
            // 
            // panelTotal
            // 
            panelTotal.AutoSize = true;
            panelTotal.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelTotal.Controls.Add(labelTotal);
            panelTotal.Controls.Add(labelLeftPlatform);
            panelTotal.Controls.Add(trackTotal);
            panelTotal.Dock = DockStyle.Top;
            panelTotal.Location = new Point(0, 66);
            panelTotal.Margin = new Padding(4);
            panelTotal.Name = "panelTotal";
            panelTotal.Size = new Size(523, 140);
            panelTotal.TabIndex = 40;
            // 
            // labelTotal
            // 
            labelTotal.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelTotal.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelTotal.Location = new Point(396, 10);
            labelTotal.Margin = new Padding(4, 0, 4, 0);
            labelTotal.Name = "labelTotal";
            labelTotal.Size = new Size(122, 32);
            labelTotal.TabIndex = 12;
            labelTotal.Text = "Platform";
            labelTotal.TextAlign = ContentAlignment.TopRight;
            // 
            // labelLeftPlatform
            // 
            labelLeftPlatform.AutoSize = true;
            labelLeftPlatform.Location = new Point(10, 10);
            labelLeftPlatform.Margin = new Padding(4, 0, 4, 0);
            labelLeftPlatform.Name = "labelLeftPlatform";
            labelLeftPlatform.Size = new Size(104, 32);
            labelLeftPlatform.TabIndex = 11;
            labelLeftPlatform.Text = "Platform";
            // 
            // trackTotal
            // 
            trackTotal.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackTotal.Location = new Point(6, 48);
            trackTotal.Margin = new Padding(4, 2, 4, 2);
            trackTotal.Maximum = 180;
            trackTotal.Minimum = 10;
            trackTotal.Name = "trackTotal";
            trackTotal.Size = new Size(513, 90);
            trackTotal.TabIndex = 10;
            trackTotal.TickFrequency = 5;
            trackTotal.TickStyle = TickStyle.TopLeft;
            trackTotal.Value = 125;
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
            pictureBox1.Location = new Point(18, 18);
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
            labelPowerLimits.Location = new Point(62, 20);
            labelPowerLimits.Margin = new Padding(4, 0, 4, 0);
            labelPowerLimits.Name = "labelPowerLimits";
            labelPowerLimits.Size = new Size(229, 32);
            labelPowerLimits.TabIndex = 39;
            labelPowerLimits.Text = "Power Limits (PPT)";
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
            panelGPU.Size = new Size(523, 652);
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
            panelGPUTemp.Location = new Point(0, 485);
            panelGPUTemp.Name = "panelGPUTemp";
            panelGPUTemp.Size = new Size(523, 149);
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
            panelGPUBoost.Location = new Point(0, 345);
            panelGPUBoost.Name = "panelGPUBoost";
            panelGPUBoost.Size = new Size(523, 140);
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
            panelGPUMemory.Location = new Point(0, 205);
            panelGPUMemory.Name = "panelGPUMemory";
            panelGPUMemory.Size = new Size(523, 140);
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
            panelGPUCore.Name = "panelGPUCore";
            panelGPUCore.Size = new Size(523, 139);
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
            pictureGPU.Location = new Point(18, 18);
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
            labelGPU.Location = new Point(62, 20);
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
            MdiChildrenMinimizedAnchorBottom = false;
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
            panelAPU.ResumeLayout(false);
            panelAPU.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackAPU).EndInit();
            panelCPU.ResumeLayout(false);
            panelCPU.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackCPU).EndInit();
            panelTotal.ResumeLayout(false);
            panelTotal.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackTotal).EndInit();
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
        private Panel panelCPU;
        private Label labelCPU;
        private Label labelLeftCPU;
        private TrackBar trackCPU;
        private Panel panelTotal;
        private Label labelTotal;
        private Label labelLeftPlatform;
        private TrackBar trackTotal;
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
        private Panel panelAPU;
        private Label labelAPU;
        private Label labelLeftAPU;
        private TrackBar trackAPU;
    }
}