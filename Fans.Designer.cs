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
            ((System.ComponentModel.ISupportInitialize)chartCPU).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chartGPU).BeginInit();
            SuspendLayout();
            // 
            // chartCPU
            // 
            chartArea1.Name = "ChartArea1";
            chartCPU.ChartAreas.Add(chartArea1);
            chartCPU.Location = new Point(16, 13);
            chartCPU.Name = "chartCPU";
            chartCPU.Size = new Size(900, 446);
            chartCPU.TabIndex = 0;
            chartCPU.Text = "chartCPU";
            // 
            // buttonApply
            // 
            buttonApply.Location = new Point(662, 944);
            buttonApply.Name = "buttonApply";
            buttonApply.Size = new Size(254, 46);
            buttonApply.TabIndex = 1;
            buttonApply.Text = "Apply Fan Curve";
            buttonApply.UseVisualStyleBackColor = true;
            // 
            // buttonReset
            // 
            buttonReset.Location = new Point(402, 944);
            buttonReset.Name = "buttonReset";
            buttonReset.Size = new Size(254, 46);
            buttonReset.TabIndex = 2;
            buttonReset.Text = "Factory Defaults";
            buttonReset.UseVisualStyleBackColor = true;
            // 
            // chartGPU
            // 
            chartArea2.Name = "ChartArea1";
            chartGPU.ChartAreas.Add(chartArea2);
            chartGPU.Location = new Point(16, 477);
            chartGPU.Name = "chartGPU";
            chartGPU.Size = new Size(900, 448);
            chartGPU.TabIndex = 3;
            chartGPU.Text = "chart1";
            // 
            // Fans
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(940, 1020);
            Controls.Add(chartGPU);
            Controls.Add(buttonReset);
            Controls.Add(buttonApply);
            Controls.Add(chartCPU);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MdiChildrenMinimizedAnchorBottom = false;
            MinimizeBox = false;
            Name = "Fans";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Fans";
            ((System.ComponentModel.ISupportInitialize)chartCPU).EndInit();
            ((System.ComponentModel.ISupportInitialize)chartGPU).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chartCPU;
        private Button buttonApply;
        private Button buttonReset;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartGPU;
    }
}