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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            panelFans = new Panel();
            tableFanCharts = new TableLayoutPanel();
            chartGPU = new System.Windows.Forms.DataVisualization.Charting.Chart();
            chartCPU = new System.Windows.Forms.DataVisualization.Charting.Chart();
            checkBoost = new CheckBox();
            labelFans = new Label();
            checkAuto = new CheckBox();
            buttonReset = new Button();
            buttonApply = new Button();
            panelPower = new Panel();
            labelPowerLimits = new Label();
            checkApplyPower = new CheckBox();
            buttonApplyPower = new Button();
            panelCPU = new Panel();
            labelCPU = new Label();
            label2 = new Label();
            trackCPU = new TrackBar();
            panelTotal = new Panel();
            labelTotal = new Label();
            label1 = new Label();
            trackTotal = new TrackBar();
            labelApplied = new Label();
            pictureFine = new PictureBox();
            labelInfo = new Label();
            panelFans.SuspendLayout();
            tableFanCharts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)chartGPU).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chartCPU).BeginInit();
            panelPower.SuspendLayout();
            panelCPU.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackCPU).BeginInit();
            panelTotal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackTotal).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureFine).BeginInit();
            SuspendLayout();
            // 
            // panelFans
            // 
            panelFans.Controls.Add(tableFanCharts);
            panelFans.Controls.Add(checkBoost);
            panelFans.Controls.Add(labelFans);
            panelFans.Controls.Add(checkAuto);
            panelFans.Controls.Add(buttonReset);
            panelFans.Controls.Add(buttonApply);
            panelFans.Dock = DockStyle.Left;
            panelFans.Location = new Point(364, 0);
            panelFans.Margin = new Padding(0);
            panelFans.MinimumSize = new Size(824, 1098);
            panelFans.Name = "panelFans";
            panelFans.Padding = new Padding(20);
            panelFans.Size = new Size(824, 1159);
            panelFans.TabIndex = 12;
            // 
            // tableFanCharts
            // 
            tableFanCharts.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableFanCharts.ColumnCount = 1;
            tableFanCharts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableFanCharts.Controls.Add(chartGPU, 0, 1);
            tableFanCharts.Controls.Add(chartCPU, 0, 0);
            tableFanCharts.Location = new Point(28, 68);
            tableFanCharts.Margin = new Padding(6);
            tableFanCharts.Name = "tableFanCharts";
            tableFanCharts.RowCount = 2;
            tableFanCharts.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableFanCharts.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableFanCharts.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableFanCharts.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableFanCharts.Size = new Size(764, 988);
            tableFanCharts.TabIndex = 36;
            // 
            // chartGPU
            // 
            chartArea3.Name = "ChartArea1";
            chartGPU.ChartAreas.Add(chartArea3);
            chartGPU.Dock = DockStyle.Fill;
            chartGPU.Location = new Point(2, 504);
            chartGPU.Margin = new Padding(2, 10, 2, 10);
            chartGPU.Name = "chartGPU";
            chartGPU.Size = new Size(760, 474);
            chartGPU.TabIndex = 17;
            chartGPU.Text = "chart1";
            // 
            // chartCPU
            // 
            chartArea4.Name = "ChartArea1";
            chartCPU.ChartAreas.Add(chartArea4);
            chartCPU.Dock = DockStyle.Fill;
            chartCPU.Location = new Point(2, 10);
            chartCPU.Margin = new Padding(2, 10, 2, 10);
            chartCPU.Name = "chartCPU";
            chartCPU.Size = new Size(760, 474);
            chartCPU.TabIndex = 14;
            chartCPU.Text = "chartCPU";
            // 
            // checkBoost
            // 
            checkBoost.AutoSize = true;
            checkBoost.ForeColor = SystemColors.ControlText;
            checkBoost.Location = new Point(475, 18);
            checkBoost.Margin = new Padding(4, 2, 4, 2);
            checkBoost.Name = "checkBoost";
            checkBoost.Size = new Size(320, 36);
            checkBoost.TabIndex = 35;
            checkBoost.Text = "CPU Turbo Boost enabled";
            checkBoost.UseVisualStyleBackColor = true;
            // 
            // labelFans
            // 
            labelFans.AutoSize = true;
            labelFans.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelFans.Location = new Point(28, 20);
            labelFans.Margin = new Padding(4, 0, 4, 0);
            labelFans.Name = "labelFans";
            labelFans.Size = new Size(138, 32);
            labelFans.TabIndex = 28;
            labelFans.Text = "Fan Curves";
            // 
            // checkAuto
            // 
            checkAuto.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            checkAuto.AutoSize = true;
            checkAuto.Location = new Point(377, 1086);
            checkAuto.Margin = new Padding(4, 2, 4, 2);
            checkAuto.Name = "checkAuto";
            checkAuto.Size = new Size(165, 36);
            checkAuto.TabIndex = 17;
            checkAuto.Text = "Auto Apply";
            checkAuto.UseVisualStyleBackColor = true;
            // 
            // buttonReset
            // 
            buttonReset.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonReset.Location = new Point(30, 1081);
            buttonReset.Margin = new Padding(4, 2, 4, 2);
            buttonReset.Name = "buttonReset";
            buttonReset.Size = new Size(232, 44);
            buttonReset.TabIndex = 15;
            buttonReset.Text = "Factory Defaults";
            buttonReset.UseVisualStyleBackColor = true;
            // 
            // buttonApply
            // 
            buttonApply.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonApply.Location = new Point(542, 1081);
            buttonApply.Margin = new Padding(4, 2, 4, 2);
            buttonApply.Name = "buttonApply";
            buttonApply.Size = new Size(248, 44);
            buttonApply.TabIndex = 14;
            buttonApply.Text = "Apply Fan Curve";
            buttonApply.UseVisualStyleBackColor = true;
            // 
            // panelPower
            // 
            panelPower.Controls.Add(labelPowerLimits);
            panelPower.Controls.Add(checkApplyPower);
            panelPower.Controls.Add(buttonApplyPower);
            panelPower.Controls.Add(panelCPU);
            panelPower.Controls.Add(panelTotal);
            panelPower.Controls.Add(labelApplied);
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
            // labelPowerLimits
            // 
            labelPowerLimits.AutoSize = true;
            labelPowerLimits.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelPowerLimits.Location = new Point(24, 20);
            labelPowerLimits.Margin = new Padding(4, 0, 4, 0);
            labelPowerLimits.Name = "labelPowerLimits";
            labelPowerLimits.Size = new Size(229, 32);
            labelPowerLimits.TabIndex = 26;
            labelPowerLimits.Text = "Power Limits (PPT)";
            // 
            // checkApplyPower
            // 
            checkApplyPower.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            checkApplyPower.AutoSize = true;
            checkApplyPower.Location = new Point(27, 1039);
            checkApplyPower.Margin = new Padding(4, 2, 4, 2);
            checkApplyPower.Name = "checkApplyPower";
            checkApplyPower.Size = new Size(165, 36);
            checkApplyPower.TabIndex = 25;
            checkApplyPower.Text = "Auto Apply";
            checkApplyPower.UseVisualStyleBackColor = true;
            // 
            // buttonApplyPower
            // 
            buttonApplyPower.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            buttonApplyPower.Location = new Point(20, 1081);
            buttonApplyPower.Margin = new Padding(4, 2, 4, 2);
            buttonApplyPower.Name = "buttonApplyPower";
            buttonApplyPower.Size = new Size(324, 44);
            buttonApplyPower.TabIndex = 24;
            buttonApplyPower.Text = "Apply Power Limits";
            buttonApplyPower.UseVisualStyleBackColor = true;
            // 
            // panelCPU
            // 
            panelCPU.Controls.Add(labelCPU);
            panelCPU.Controls.Add(label2);
            panelCPU.Controls.Add(trackCPU);
            panelCPU.Location = new Point(184, 90);
            panelCPU.Margin = new Padding(4);
            panelCPU.Name = "panelCPU";
            panelCPU.Size = new Size(160, 510);
            panelCPU.TabIndex = 23;
            // 
            // labelCPU
            // 
            labelCPU.AutoSize = true;
            labelCPU.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelCPU.Location = new Point(44, 40);
            labelCPU.Margin = new Padding(4, 0, 4, 0);
            labelCPU.Name = "labelCPU";
            labelCPU.Size = new Size(61, 32);
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
            trackCPU.Minimum = 15;
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
            panelTotal.Location = new Point(16, 90);
            panelTotal.Margin = new Padding(4);
            panelTotal.Name = "panelTotal";
            panelTotal.Size = new Size(160, 512);
            panelTotal.TabIndex = 22;
            // 
            // labelTotal
            // 
            labelTotal.AutoSize = true;
            labelTotal.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelTotal.Location = new Point(46, 40);
            labelTotal.Margin = new Padding(4, 0, 4, 0);
            labelTotal.Name = "labelTotal";
            labelTotal.Size = new Size(70, 32);
            labelTotal.TabIndex = 12;
            labelTotal.Text = "Total";
            labelTotal.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(48, 8);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(65, 32);
            label1.TabIndex = 11;
            label1.Text = "Total";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // trackTotal
            // 
            trackTotal.Location = new Point(44, 88);
            trackTotal.Margin = new Padding(4, 2, 4, 2);
            trackTotal.Maximum = 150;
            trackTotal.Minimum = 15;
            trackTotal.Name = "trackTotal";
            trackTotal.Orientation = Orientation.Vertical;
            trackTotal.Size = new Size(90, 416);
            trackTotal.TabIndex = 10;
            trackTotal.TickFrequency = 5;
            trackTotal.TickStyle = TickStyle.TopLeft;
            trackTotal.Value = 125;
            // 
            // labelApplied
            // 
            labelApplied.AutoSize = true;
            labelApplied.ForeColor = Color.Tomato;
            labelApplied.Location = new Point(24, 54);
            labelApplied.Margin = new Padding(4, 0, 4, 0);
            labelApplied.Name = "labelApplied";
            labelApplied.Size = new Size(143, 32);
            labelApplied.TabIndex = 21;
            labelApplied.Text = "Not Applied";
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
            ClientSize = new Size(1188, 1159);
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
            Text = "Fans and Power";
            panelFans.ResumeLayout(false);
            panelFans.PerformLayout();
            tableFanCharts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)chartGPU).EndInit();
            ((System.ComponentModel.ISupportInitialize)chartCPU).EndInit();
            panelPower.ResumeLayout(false);
            panelPower.PerformLayout();
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
        private CheckBox checkAuto;
        private Button buttonReset;
        private Button buttonApply;
        private Panel panelPower;
        private CheckBox checkApplyPower;
        private Button buttonApplyPower;
        private Panel panelCPU;
        private Label labelCPU;
        private Label label2;
        private TrackBar trackCPU;
        private Panel panelTotal;
        private Label labelTotal;
        private Label label1;
        private TrackBar trackTotal;
        private Label labelApplied;
        private PictureBox pictureFine;
        private Label labelInfo;
        private Label labelPowerLimits;
        private TableLayoutPanel tableFanCharts;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartGPU;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartCPU;
        private CheckBox checkBoost;
        private Label labelFans;
    }
}