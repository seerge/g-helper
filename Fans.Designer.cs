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
            chartCPU = new System.Windows.Forms.DataVisualization.Charting.Chart();
            buttonApply = new Button();
            buttonReset = new Button();
            chartGPU = new System.Windows.Forms.DataVisualization.Charting.Chart();
            groupBox1 = new GroupBox();
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
            buttonApplyPower = new Button();
            checkAuto = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)chartCPU).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chartGPU).BeginInit();
            groupBox1.SuspendLayout();
            panelCPU.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackCPU).BeginInit();
            panelTotal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackTotal).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureFine).BeginInit();
            SuspendLayout();
            // 
            // chartCPU
            // 
            chartArea3.Name = "ChartArea1";
            chartCPU.ChartAreas.Add(chartArea3);
            chartCPU.Location = new Point(390, 28);
            chartCPU.Margin = new Padding(4, 2, 4, 2);
            chartCPU.Name = "chartCPU";
            chartCPU.Size = new Size(832, 436);
            chartCPU.TabIndex = 0;
            chartCPU.Text = "chartCPU";
            // 
            // buttonApply
            // 
            buttonApply.Location = new Point(946, 952);
            buttonApply.Margin = new Padding(4, 2, 4, 2);
            buttonApply.Name = "buttonApply";
            buttonApply.Size = new Size(274, 44);
            buttonApply.TabIndex = 1;
            buttonApply.Text = "Apply Fan Curve";
            buttonApply.UseVisualStyleBackColor = true;
            // 
            // buttonReset
            // 
            buttonReset.Location = new Point(390, 952);
            buttonReset.Margin = new Padding(4, 2, 4, 2);
            buttonReset.Name = "buttonReset";
            buttonReset.Size = new Size(274, 44);
            buttonReset.TabIndex = 2;
            buttonReset.Text = "Factory Defaults";
            buttonReset.UseVisualStyleBackColor = true;
            // 
            // chartGPU
            // 
            chartArea4.Name = "ChartArea1";
            chartGPU.ChartAreas.Add(chartArea4);
            chartGPU.Location = new Point(390, 480);
            chartGPU.Margin = new Padding(4, 2, 4, 2);
            chartGPU.Name = "chartGPU";
            chartGPU.Size = new Size(832, 450);
            chartGPU.TabIndex = 3;
            chartGPU.Text = "chart1";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(panelCPU);
            groupBox1.Controls.Add(panelTotal);
            groupBox1.Controls.Add(labelApplied);
            groupBox1.Controls.Add(pictureFine);
            groupBox1.Controls.Add(labelInfo);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Margin = new Padding(4, 2, 4, 2);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(6, 4, 6, 4);
            groupBox1.Size = new Size(356, 918);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "Power Limits (PPT)";
            // 
            // panelCPU
            // 
            panelCPU.Controls.Add(labelCPU);
            panelCPU.Controls.Add(label2);
            panelCPU.Controls.Add(trackCPU);
            panelCPU.Location = new Point(186, 72);
            panelCPU.Name = "panelCPU";
            panelCPU.Size = new Size(160, 510);
            panelCPU.TabIndex = 15;
            // 
            // labelCPU
            // 
            labelCPU.AutoSize = true;
            labelCPU.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelCPU.Location = new Point(43, 41);
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
            label2.Location = new Point(45, 7);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(58, 32);
            label2.TabIndex = 12;
            label2.Text = "CPU";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // trackCPU
            // 
            trackCPU.Location = new Point(47, 89);
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
            panelTotal.Location = new Point(8, 72);
            panelTotal.Name = "panelTotal";
            panelTotal.Size = new Size(160, 511);
            panelTotal.TabIndex = 14;
            // 
            // labelTotal
            // 
            labelTotal.AutoSize = true;
            labelTotal.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelTotal.Location = new Point(46, 41);
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
            label1.Location = new Point(48, 7);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(65, 32);
            label1.TabIndex = 11;
            label1.Text = "Total";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // trackTotal
            // 
            trackTotal.Location = new Point(44, 89);
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
            labelApplied.Location = new Point(13, 36);
            labelApplied.Margin = new Padding(4, 0, 4, 0);
            labelApplied.Name = "labelApplied";
            labelApplied.Size = new Size(143, 32);
            labelApplied.TabIndex = 13;
            labelApplied.Text = "Not Applied";
            // 
            // pictureFine
            // 
            pictureFine.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureFine.BackgroundImageLayout = ImageLayout.Zoom;
            pictureFine.Image = Properties.Resources.everything_is_fine_itsfine;
            pictureFine.Location = new Point(10, 682);
            pictureFine.Margin = new Padding(4, 2, 4, 2);
            pictureFine.Name = "pictureFine";
            pictureFine.Size = new Size(336, 226);
            pictureFine.SizeMode = PictureBoxSizeMode.Zoom;
            pictureFine.TabIndex = 12;
            pictureFine.TabStop = false;
            pictureFine.Visible = false;
            // 
            // labelInfo
            // 
            labelInfo.AutoSize = true;
            labelInfo.Dock = DockStyle.Bottom;
            labelInfo.Location = new Point(6, 882);
            labelInfo.Margin = new Padding(4, 0, 4, 0);
            labelInfo.Name = "labelInfo";
            labelInfo.Size = new Size(65, 32);
            labelInfo.TabIndex = 11;
            labelInfo.Text = "label";
            // 
            // buttonApplyPower
            // 
            buttonApplyPower.Location = new Point(16, 952);
            buttonApplyPower.Margin = new Padding(4, 2, 4, 2);
            buttonApplyPower.Name = "buttonApplyPower";
            buttonApplyPower.Size = new Size(352, 44);
            buttonApplyPower.TabIndex = 11;
            buttonApplyPower.Text = "Apply Power Limits";
            buttonApplyPower.UseVisualStyleBackColor = true;
            // 
            // checkAuto
            // 
            checkAuto.AutoSize = true;
            checkAuto.Location = new Point(772, 958);
            checkAuto.Margin = new Padding(4, 2, 4, 2);
            checkAuto.Name = "checkAuto";
            checkAuto.Size = new Size(165, 36);
            checkAuto.TabIndex = 12;
            checkAuto.Text = "Auto Apply";
            checkAuto.UseVisualStyleBackColor = true;
            // 
            // Fans
            // 
            AutoScaleDimensions = new SizeF(192F, 192F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new Size(1242, 1020);
            Controls.Add(checkAuto);
            Controls.Add(buttonApplyPower);
            Controls.Add(groupBox1);
            Controls.Add(chartGPU);
            Controls.Add(buttonReset);
            Controls.Add(buttonApply);
            Controls.Add(chartCPU);
            Margin = new Padding(4, 2, 4, 2);
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
            panelCPU.ResumeLayout(false);
            panelCPU.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackCPU).EndInit();
            panelTotal.ResumeLayout(false);
            panelTotal.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackTotal).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureFine).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chartCPU;
        private Button buttonApply;
        private Button buttonReset;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartGPU;
        private GroupBox groupBox1;
        private Button buttonApplyPower;
        private Label labelInfo;
        private PictureBox pictureFine;
        private Label labelApplied;
        private CheckBox checkAuto;
        private Panel panelTotal;
        private Label labelTotal;
        private Label label1;
        private TrackBar trackTotal;
        private Panel panelCPU;
        private Label labelCPU;
        private Label label2;
        private TrackBar trackCPU;
    }
}