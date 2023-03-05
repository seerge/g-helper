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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            chartCPU = new System.Windows.Forms.DataVisualization.Charting.Chart();
            buttonApply = new Button();
            buttonReset = new Button();
            chartGPU = new System.Windows.Forms.DataVisualization.Charting.Chart();
            groupBox1 = new GroupBox();
            labelApplied = new Label();
            pictureFine = new PictureBox();
            labelInfo = new Label();
            labelCPU = new Label();
            labelTotal = new Label();
            label2 = new Label();
            label1 = new Label();
            trackCPU = new TrackBar();
            trackTotal = new TrackBar();
            buttonApplyPower = new Button();
            checkAuto = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)chartCPU).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chartGPU).BeginInit();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureFine).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackCPU).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackTotal).BeginInit();
            SuspendLayout();
            // 
            // chartCPU
            // 
            chartArea1.Name = "ChartArea1";
            chartCPU.ChartAreas.Add(chartArea1);
            chartCPU.Location = new Point(195, 14);
            chartCPU.Margin = new Padding(2, 1, 2, 1);
            chartCPU.Name = "chartCPU";
            chartCPU.Size = new Size(416, 218);
            chartCPU.TabIndex = 0;
            chartCPU.Text = "chartCPU";
            // 
            // buttonApply
            // 
            buttonApply.Location = new Point(473, 476);
            buttonApply.Margin = new Padding(2, 1, 2, 1);
            buttonApply.Name = "buttonApply";
            buttonApply.Size = new Size(137, 22);
            buttonApply.TabIndex = 1;
            buttonApply.Text = "Apply Fan Curve";
            buttonApply.UseVisualStyleBackColor = true;
            // 
            // buttonReset
            // 
            buttonReset.Location = new Point(195, 476);
            buttonReset.Margin = new Padding(2, 1, 2, 1);
            buttonReset.Name = "buttonReset";
            buttonReset.Size = new Size(137, 22);
            buttonReset.TabIndex = 2;
            buttonReset.Text = "Factory Defaults";
            buttonReset.UseVisualStyleBackColor = true;
            // 
            // chartGPU
            // 
            chartArea2.Name = "ChartArea1";
            chartGPU.ChartAreas.Add(chartArea2);
            chartGPU.Location = new Point(195, 240);
            chartGPU.Margin = new Padding(2, 1, 2, 1);
            chartGPU.Name = "chartGPU";
            chartGPU.Size = new Size(416, 225);
            chartGPU.TabIndex = 3;
            chartGPU.Text = "chart1";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(labelApplied);
            groupBox1.Controls.Add(pictureFine);
            groupBox1.Controls.Add(labelInfo);
            groupBox1.Controls.Add(labelCPU);
            groupBox1.Controls.Add(labelTotal);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(trackCPU);
            groupBox1.Controls.Add(trackTotal);
            groupBox1.Location = new Point(6, 6);
            groupBox1.Margin = new Padding(2, 1, 2, 1);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(3, 2, 3, 2);
            groupBox1.Size = new Size(178, 459);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "Power Limits (PPT)";
            // 
            // labelApplied
            // 
            labelApplied.AutoSize = true;
            labelApplied.ForeColor = Color.Tomato;
            labelApplied.Location = new Point(8, 18);
            labelApplied.Margin = new Padding(2, 0, 2, 0);
            labelApplied.Name = "labelApplied";
            labelApplied.Size = new Size(71, 15);
            labelApplied.TabIndex = 13;
            labelApplied.Text = "Not Applied";
            // 
            // pictureFine
            // 
            pictureFine.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureFine.BackgroundImage = Properties.Resources.everything_is_fine_itsfine;
            pictureFine.BackgroundImageLayout = ImageLayout.Zoom;
            pictureFine.Location = new Point(5, 343);
            pictureFine.Margin = new Padding(2, 1, 2, 1);
            pictureFine.Name = "pictureFine";
            pictureFine.Size = new Size(167, 112);
            pictureFine.TabIndex = 12;
            pictureFine.TabStop = false;
            pictureFine.Visible = false;
            // 
            // labelInfo
            // 
            labelInfo.AutoSize = true;
            labelInfo.Dock = DockStyle.Bottom;
            labelInfo.Location = new Point(3, 442);
            labelInfo.Margin = new Padding(2, 0, 2, 0);
            labelInfo.Name = "labelInfo";
            labelInfo.Size = new Size(32, 15);
            labelInfo.TabIndex = 11;
            labelInfo.Text = "label";
            // 
            // labelCPU
            // 
            labelCPU.AutoSize = true;
            labelCPU.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelCPU.Location = new Point(106, 59);
            labelCPU.Margin = new Padding(2, 0, 2, 0);
            labelCPU.Name = "labelCPU";
            labelCPU.Size = new Size(30, 15);
            labelCPU.TabIndex = 10;
            labelCPU.Text = "CPU";
            labelCPU.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // labelTotal
            // 
            labelTotal.AutoSize = true;
            labelTotal.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelTotal.Location = new Point(21, 59);
            labelTotal.Margin = new Padding(2, 0, 2, 0);
            labelTotal.Name = "labelTotal";
            labelTotal.Size = new Size(34, 15);
            labelTotal.TabIndex = 9;
            labelTotal.Text = "Total";
            labelTotal.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(108, 43);
            label2.Margin = new Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new Size(30, 15);
            label2.TabIndex = 8;
            label2.Text = "CPU";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(22, 43);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(32, 15);
            label1.TabIndex = 7;
            label1.Text = "Total";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // trackCPU
            // 
            trackCPU.Location = new Point(109, 83);
            trackCPU.Margin = new Padding(2, 1, 2, 1);
            trackCPU.Maximum = 85;
            trackCPU.Minimum = 15;
            trackCPU.Name = "trackCPU";
            trackCPU.Orientation = Orientation.Vertical;
            trackCPU.Size = new Size(45, 208);
            trackCPU.TabIndex = 6;
            trackCPU.TickFrequency = 5;
            trackCPU.Value = 80;
            // 
            // trackTotal
            // 
            trackTotal.Location = new Point(23, 83);
            trackTotal.Margin = new Padding(2, 1, 2, 1);
            trackTotal.Maximum = 150;
            trackTotal.Minimum = 15;
            trackTotal.Name = "trackTotal";
            trackTotal.Orientation = Orientation.Vertical;
            trackTotal.Size = new Size(45, 208);
            trackTotal.TabIndex = 5;
            trackTotal.TickFrequency = 5;
            trackTotal.TickStyle = TickStyle.TopLeft;
            trackTotal.Value = 125;
            // 
            // buttonApplyPower
            // 
            buttonApplyPower.Location = new Point(8, 476);
            buttonApplyPower.Margin = new Padding(2, 1, 2, 1);
            buttonApplyPower.Name = "buttonApplyPower";
            buttonApplyPower.Size = new Size(176, 22);
            buttonApplyPower.TabIndex = 11;
            buttonApplyPower.Text = "Apply Power Limits";
            buttonApplyPower.UseVisualStyleBackColor = true;
            // 
            // checkAuto
            // 
            checkAuto.AutoSize = true;
            checkAuto.Location = new Point(381, 479);
            checkAuto.Margin = new Padding(2, 1, 2, 1);
            checkAuto.Name = "checkAuto";
            checkAuto.Size = new Size(86, 19);
            checkAuto.TabIndex = 12;
            checkAuto.Text = "Auto Apply";
            checkAuto.UseVisualStyleBackColor = true;
            // 
            // Fans
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(621, 510);
            Controls.Add(checkAuto);
            Controls.Add(buttonApplyPower);
            Controls.Add(groupBox1);
            Controls.Add(chartGPU);
            Controls.Add(buttonReset);
            Controls.Add(buttonApply);
            Controls.Add(chartCPU);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(2, 1, 2, 1);
            MaximizeBox = false;
            MdiChildrenMinimizedAnchorBottom = false;
            MinimizeBox = false;
            Name = "Fans";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Fans and Power";
            ((System.ComponentModel.ISupportInitialize)chartCPU).EndInit();
            ((System.ComponentModel.ISupportInitialize)chartGPU).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureFine).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackCPU).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackTotal).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chartCPU;
        private Button buttonApply;
        private Button buttonReset;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartGPU;
        private GroupBox groupBox1;
        private Label labelCPU;
        private Label labelTotal;
        private Label label2;
        private Label label1;
        private TrackBar trackCPU;
        private TrackBar trackTotal;
        private Button buttonApplyPower;
        private Label labelInfo;
        private PictureBox pictureFine;
        private Label labelApplied;
        private CheckBox checkAuto;
    }
}