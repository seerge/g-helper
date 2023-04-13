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
            ChartArea chartArea1 = new ChartArea();
            Title title1 = new Title();
            ChartArea chartArea2 = new ChartArea();
            Title title2 = new Title();
            ChartArea chartArea3 = new ChartArea();
            Title title3 = new Title();
            panelFans = new Panel();
            labelTip = new Label();
            labelBoost = new Label();
            comboBoost = new RComboBox();
            picturePerf = new PictureBox();
            tableFanCharts = new TableLayoutPanel();
            chartGPU = new Chart();
            chartCPU = new Chart();
            chartMid = new Chart();
            labelFans = new Label();
            checkApplyFans = new RCheckBox();
            buttonReset = new RButton();
            panelPower = new Panel();
            pictureBox1 = new PictureBox();
            labelPowerLimits = new Label();
            checkApplyPower = new RCheckBox();
            panelCPU = new Panel();
            labelCPU = new Label();
            label2 = new Label();
            trackCPU = new TrackBar();
            panelTotal = new Panel();
            labelTotal = new Label();
            label1 = new Label();
            trackTotal = new TrackBar();
            pictureFine = new PictureBox();
            labelInfo = new Label();
            panelFans.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picturePerf).BeginInit();
            tableFanCharts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)chartGPU).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chartCPU).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chartMid).BeginInit();
            panelPower.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panelCPU.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackCPU).BeginInit();
            panelTotal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackTotal).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureFine).BeginInit();
            SuspendLayout();
            // 
            // panelFans
            // 
            panelFans.Controls.Add(labelTip);
            panelFans.Controls.Add(labelBoost);
            panelFans.Controls.Add(comboBoost);
            panelFans.Controls.Add(picturePerf);
            panelFans.Controls.Add(tableFanCharts);
            panelFans.Controls.Add(labelFans);
            panelFans.Controls.Add(checkApplyFans);
            panelFans.Controls.Add(buttonReset);
            panelFans.Dock = DockStyle.Left;
            panelFans.Location = new Point(364, 0);
            panelFans.Margin = new Padding(0);
            panelFans.Name = "panelFans";
            panelFans.Padding = new Padding(20);
            panelFans.Size = new Size(824, 1159);
            panelFans.TabIndex = 12;
            // 
            // labelTip
            // 
            labelTip.AutoSize = true;
            labelTip.BackColor = SystemColors.ControlLightLight;
            labelTip.Location = new Point(155, 9);
            labelTip.Name = "labelTip";
            labelTip.Padding = new Padding(5);
            labelTip.Size = new Size(107, 42);
            labelTip.TabIndex = 40;
            labelTip.Text = "500,300";
            // 
            // labelBoost
            // 
            labelBoost.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelBoost.AutoSize = true;
            labelBoost.Location = new Point(375, 17);
            labelBoost.Name = "labelBoost";
            labelBoost.Size = new Size(125, 32);
            labelBoost.TabIndex = 39;
            labelBoost.Text = Properties.Strings.CPUBoost;
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
            comboBoost.Location = new Point(526, 15);
            comboBoost.Name = "comboBoost";
            comboBoost.Size = new Size(266, 40);
            comboBoost.TabIndex = 38;
            // 
            // picturePerf
            // 
            picturePerf.BackgroundImage = Properties.Resources.icons8_fan_head_96;
            picturePerf.BackgroundImageLayout = ImageLayout.Zoom;
            picturePerf.InitialImage = null;
            picturePerf.Location = new Point(30, 18);
            picturePerf.Margin = new Padding(4, 2, 4, 2);
            picturePerf.Name = "picturePerf";
            picturePerf.Size = new Size(36, 38);
            picturePerf.TabIndex = 37;
            picturePerf.TabStop = false;
            // 
            // tableFanCharts
            // 
            tableFanCharts.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableFanCharts.ColumnCount = 1;
            tableFanCharts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableFanCharts.Controls.Add(chartGPU, 0, 1);
            tableFanCharts.Controls.Add(chartCPU, 0, 0);
            tableFanCharts.Controls.Add(chartMid, 0, 2);
            tableFanCharts.Location = new Point(28, 64);
            tableFanCharts.Margin = new Padding(6);
            tableFanCharts.Name = "tableFanCharts";
            tableFanCharts.RowCount = 2;
            tableFanCharts.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));
            tableFanCharts.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));
            tableFanCharts.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));
            tableFanCharts.Size = new Size(764, 992);
            tableFanCharts.TabIndex = 36;
            // 
            // chartGPU
            // 
            chartArea1.Name = "ChartArea1";
            chartGPU.ChartAreas.Add(chartArea1);
            chartGPU.Dock = DockStyle.Fill;
            chartGPU.Location = new Point(2, 340);
            chartGPU.Margin = new Padding(2, 10, 2, 10);
            chartGPU.Name = "chartGPU";
            chartGPU.Size = new Size(760, 310);
            chartGPU.TabIndex = 17;
            chartGPU.Text = "chartGPU";
            title1.Name = "Title1";
            chartGPU.Titles.Add(title1);
            // 
            // chartCPU
            // 
            chartArea2.Name = "ChartArea1";
            chartCPU.ChartAreas.Add(chartArea2);
            chartCPU.Dock = DockStyle.Fill;
            chartCPU.Location = new Point(2, 10);
            chartCPU.Margin = new Padding(2, 10, 2, 10);
            chartCPU.Name = "chartCPU";
            chartCPU.Size = new Size(760, 310);
            chartCPU.TabIndex = 14;
            chartCPU.Text = "chartCPU";
            title2.Name = "Title1";
            chartCPU.Titles.Add(title2);
            // 
            // chartMid
            // 
            chartArea3.Name = "ChartArea3";
            chartMid.ChartAreas.Add(chartArea3);
            chartMid.Dock = DockStyle.Fill;
            chartMid.Location = new Point(2, 670);
            chartMid.Margin = new Padding(2, 10, 2, 10);
            chartMid.Name = "chartMid";
            chartMid.Size = new Size(760, 312);
            chartMid.TabIndex = 14;
            chartMid.Text = "chartMid";
            title3.Name = "Title3";
            chartMid.Titles.Add(title3);
            chartMid.Visible = false;
            // 
            // labelFans
            // 
            labelFans.AutoSize = true;
            labelFans.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelFans.Location = new Point(66, 20);
            labelFans.Margin = new Padding(4, 0, 4, 0);
            labelFans.Name = "labelFans";
            labelFans.Size = new Size(138, 32);
            labelFans.TabIndex = 28;
            labelFans.Text = "Fan Curves";
            // 
            // checkApplyFans
            // 
            checkApplyFans.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            checkApplyFans.AutoSize = true;
            checkApplyFans.BackColor = SystemColors.ControlLight;
            checkApplyFans.Location = new Point(449, 1084);
            checkApplyFans.Margin = new Padding(4, 2, 4, 2);
            checkApplyFans.Name = "checkApplyFans";
            checkApplyFans.Padding = new Padding(15, 5, 15, 5);
            checkApplyFans.Size = new Size(339, 50);
            checkApplyFans.TabIndex = 17;
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
            buttonReset.Location = new Point(30, 1082);
            buttonReset.Margin = new Padding(4, 2, 4, 2);
            buttonReset.Name = "buttonReset";
            buttonReset.Secondary = true;
            buttonReset.Size = new Size(232, 54);
            buttonReset.TabIndex = 15;
            buttonReset.Text = Properties.Strings.FactoryDefaults;
            buttonReset.UseVisualStyleBackColor = false;
            // 
            // panelPower
            // 
            panelPower.Controls.Add(pictureBox1);
            panelPower.Controls.Add(labelPowerLimits);
            panelPower.Controls.Add(checkApplyPower);
            panelPower.Controls.Add(panelCPU);
            panelPower.Controls.Add(panelTotal);
            panelPower.Controls.Add(pictureFine);
            panelPower.Controls.Add(labelInfo);
            panelPower.Dock = DockStyle.Left;
            panelPower.Location = new Point(0, 0);
            panelPower.Margin = new Padding(10);
            panelPower.Name = "panelPower";
            panelPower.Padding = new Padding(10);
            panelPower.Size = new Size(364, 1159);
            panelPower.TabIndex = 13;
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = Properties.Resources.icons8_processor_96;
            pictureBox1.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox1.InitialImage = null;
            pictureBox1.Location = new Point(20, 18);
            pictureBox1.Margin = new Padding(4, 2, 4, 2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(36, 38);
            pictureBox1.TabIndex = 38;
            pictureBox1.TabStop = false;
            // 
            // labelPowerLimits
            // 
            labelPowerLimits.AutoSize = true;
            labelPowerLimits.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelPowerLimits.Location = new Point(54, 20);
            labelPowerLimits.Margin = new Padding(4, 0, 4, 0);
            labelPowerLimits.Name = "labelPowerLimits";
            labelPowerLimits.Size = new Size(229, 32);
            labelPowerLimits.TabIndex = 26;
            labelPowerLimits.Text = Properties.Strings.PowerLimits;
            // 
            // checkApplyPower
            // 
            checkApplyPower.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            checkApplyPower.AutoSize = true;
            checkApplyPower.BackColor = SystemColors.ControlLight;
            checkApplyPower.Location = new Point(20, 1084);
            checkApplyPower.Margin = new Padding(4, 2, 4, 2);
            checkApplyPower.Name = "checkApplyPower";
            checkApplyPower.Padding = new Padding(15, 5, 15, 5);
            checkApplyPower.Size = new Size(277, 50);
            checkApplyPower.TabIndex = 25;
            checkApplyPower.Text = Properties.Strings.ApplyPowerLimits;
            checkApplyPower.UseVisualStyleBackColor = false;
            // 
            // panelCPU
            // 
            panelCPU.Controls.Add(labelCPU);
            panelCPU.Controls.Add(label2);
            panelCPU.Controls.Add(trackCPU);
            panelCPU.Location = new Point(184, 72);
            panelCPU.Margin = new Padding(4);
            panelCPU.Name = "panelCPU";
            panelCPU.Size = new Size(160, 510);
            panelCPU.TabIndex = 23;
            // 
            // labelCPU
            // 
            labelCPU.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelCPU.Location = new Point(13, 40);
            labelCPU.Margin = new Padding(4, 0, 4, 0);
            labelCPU.Name = "labelCPU";
            labelCPU.Size = new Size(120, 32);
            labelCPU.TabIndex = 13;
            labelCPU.Text = "CPU";
            labelCPU.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(44, 8);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(58, 32);
            label2.TabIndex = 12;
            label2.Text = "CPU";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // trackCPU
            // 
            trackCPU.Location = new Point(48, 88);
            trackCPU.Margin = new Padding(4, 2, 4, 2);
            trackCPU.Maximum = 85;
            trackCPU.Minimum = 5;
            trackCPU.Name = "trackCPU";
            trackCPU.Orientation = Orientation.Vertical;
            trackCPU.Size = new Size(90, 416);
            trackCPU.TabIndex = 11;
            trackCPU.TickFrequency = 5;
            trackCPU.Value = 80;
            // 
            // panelTotal
            // 
            panelTotal.Controls.Add(labelTotal);
            panelTotal.Controls.Add(label1);
            panelTotal.Controls.Add(trackTotal);
            panelTotal.Location = new Point(16, 72);
            panelTotal.Margin = new Padding(4);
            panelTotal.Name = "panelTotal";
            panelTotal.Size = new Size(160, 512);
            panelTotal.TabIndex = 22;
            // 
            // labelTotal
            // 
            labelTotal.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelTotal.Location = new Point(16, 40);
            labelTotal.Margin = new Padding(4, 0, 4, 0);
            labelTotal.Name = "labelTotal";
            labelTotal.Size = new Size(122, 32);
            labelTotal.TabIndex = 12;
            labelTotal.Text = "Platform";
            labelTotal.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(26, 8);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(104, 32);
            label1.TabIndex = 11;
            label1.Text = "Platform";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // trackTotal
            // 
            trackTotal.Location = new Point(44, 88);
            trackTotal.Margin = new Padding(4, 2, 4, 2);
            trackTotal.Maximum = 180;
            trackTotal.Minimum = 10;
            trackTotal.Name = "trackTotal";
            trackTotal.Orientation = Orientation.Vertical;
            trackTotal.Size = new Size(90, 416);
            trackTotal.TabIndex = 10;
            trackTotal.TickFrequency = 5;
            trackTotal.TickStyle = TickStyle.TopLeft;
            trackTotal.Value = 125;
            // 
            // pictureFine
            // 
            pictureFine.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pictureFine.BackgroundImageLayout = ImageLayout.Zoom;
            pictureFine.Image = Properties.Resources.everything_is_fine_itsfine;
            pictureFine.Location = new Point(20, 682);
            pictureFine.Margin = new Padding(4, 2, 4, 2);
            pictureFine.Name = "pictureFine";
            pictureFine.Size = new Size(324, 268);
            pictureFine.SizeMode = PictureBoxSizeMode.Zoom;
            pictureFine.TabIndex = 20;
            pictureFine.TabStop = false;
            pictureFine.Visible = false;
            // 
            // labelInfo
            // 
            labelInfo.Location = new Point(24, 618);
            labelInfo.Margin = new Padding(4, 0, 4, 0);
            labelInfo.Name = "labelInfo";
            labelInfo.Size = new Size(320, 330);
            labelInfo.TabIndex = 19;
            labelInfo.Text = "label";
            // 
            // Fans
            // 
            AutoScaleDimensions = new SizeF(192F, 192F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(1178, 1159);
            Controls.Add(panelFans);
            Controls.Add(panelPower);
            Margin = new Padding(4, 2, 4, 2);
            MaximizeBox = false;
            MdiChildrenMinimizedAnchorBottom = false;
            MinimizeBox = false;
            MinimumSize = new Size(26, 1230);
            Name = "Fans";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = Properties.Strings.FansAndPower;
            panelFans.ResumeLayout(false);
            panelFans.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picturePerf).EndInit();
            tableFanCharts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)chartGPU).EndInit();
            ((System.ComponentModel.ISupportInitialize)chartCPU).EndInit();
            ((System.ComponentModel.ISupportInitialize)chartMid).EndInit();
            panelPower.ResumeLayout(false);
            panelPower.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panelCPU.ResumeLayout(false);
            panelCPU.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackCPU).EndInit();
            panelTotal.ResumeLayout(false);
            panelTotal.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackTotal).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureFine).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Panel panelFans;
        private RCheckBox checkApplyFans;
        private RButton buttonReset;
        private Panel panelPower;
        private RCheckBox checkApplyPower;
        private Panel panelCPU;
        private Label labelCPU;
        private Label label2;
        private TrackBar trackCPU;
        private Panel panelTotal;
        private Label labelTotal;
        private Label label1;
        private TrackBar trackTotal;
        private PictureBox pictureFine;
        private Label labelInfo;
        private Label labelPowerLimits;
        private TableLayoutPanel tableFanCharts;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartGPU;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartCPU;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartMid;
        private Label labelFans;
        private PictureBox picturePerf;
        private PictureBox pictureBox1;
        private RComboBox comboBoost;
        private Label labelBoost;
        private Label labelTip;
    }
}