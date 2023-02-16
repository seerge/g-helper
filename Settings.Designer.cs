namespace GHelper
{
    partial class SettingsForm
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
            this.groupPerf = new System.Windows.Forms.GroupBox();
            this.labelCPUFan = new System.Windows.Forms.Label();
            this.tablePerf = new System.Windows.Forms.TableLayoutPanel();
            this.buttonTurbo = new System.Windows.Forms.Button();
            this.buttonBalanced = new System.Windows.Forms.Button();
            this.buttonSilent = new System.Windows.Forms.Button();
            this.groupGPU = new System.Windows.Forms.GroupBox();
            this.labelGPUFan = new System.Windows.Forms.Label();
            this.tableGPU = new System.Windows.Forms.TableLayoutPanel();
            this.buttonUltimate = new System.Windows.Forms.Button();
            this.buttonStandard = new System.Windows.Forms.Button();
            this.buttonEco = new System.Windows.Forms.Button();
            this.groupPerf.SuspendLayout();
            this.tablePerf.SuspendLayout();
            this.groupGPU.SuspendLayout();
            this.tableGPU.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupPerf
            // 
            this.groupPerf.Controls.Add(this.labelCPUFan);
            this.groupPerf.Controls.Add(this.tablePerf);
            this.groupPerf.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupPerf.Location = new System.Drawing.Point(10, 10);
            this.groupPerf.Margin = new System.Windows.Forms.Padding(10);
            this.groupPerf.Name = "groupPerf";
            this.groupPerf.Padding = new System.Windows.Forms.Padding(10);
            this.groupPerf.Size = new System.Drawing.Size(666, 188);
            this.groupPerf.TabIndex = 0;
            this.groupPerf.TabStop = false;
            this.groupPerf.Text = "Performance Mode";
            // 
            // labelCPUFan
            // 
            this.labelCPUFan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCPUFan.AutoSize = true;
            this.labelCPUFan.Location = new System.Drawing.Point(491, 28);
            this.labelCPUFan.Name = "labelCPUFan";
            this.labelCPUFan.Size = new System.Drawing.Size(154, 32);
            this.labelCPUFan.TabIndex = 2;
            this.labelCPUFan.Text = "CPU Fan : 0%";
            // 
            // tablePerf
            // 
            this.tablePerf.ColumnCount = 3;
            this.tablePerf.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tablePerf.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tablePerf.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tablePerf.Controls.Add(this.buttonTurbo, 2, 0);
            this.tablePerf.Controls.Add(this.buttonBalanced, 1, 0);
            this.tablePerf.Controls.Add(this.buttonSilent, 0, 0);
            this.tablePerf.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tablePerf.Location = new System.Drawing.Point(10, 72);
            this.tablePerf.Name = "tablePerf";
            this.tablePerf.RowCount = 1;
            this.tablePerf.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 106F));
            this.tablePerf.Size = new System.Drawing.Size(646, 106);
            this.tablePerf.TabIndex = 0;
            // 
            // buttonTurbo
            // 
            this.buttonTurbo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonTurbo.FlatAppearance.BorderSize = 0;
            this.buttonTurbo.Location = new System.Drawing.Point(440, 10);
            this.buttonTurbo.Margin = new System.Windows.Forms.Padding(10);
            this.buttonTurbo.Name = "buttonTurbo";
            this.buttonTurbo.Size = new System.Drawing.Size(196, 86);
            this.buttonTurbo.TabIndex = 2;
            this.buttonTurbo.Text = "Turbo";
            this.buttonTurbo.UseVisualStyleBackColor = true;
            // 
            // buttonBalanced
            // 
            this.buttonBalanced.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.buttonBalanced.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonBalanced.FlatAppearance.BorderSize = 0;
            this.buttonBalanced.Location = new System.Drawing.Point(225, 10);
            this.buttonBalanced.Margin = new System.Windows.Forms.Padding(10);
            this.buttonBalanced.Name = "buttonBalanced";
            this.buttonBalanced.Size = new System.Drawing.Size(195, 86);
            this.buttonBalanced.TabIndex = 1;
            this.buttonBalanced.Text = "Balanced";
            this.buttonBalanced.UseVisualStyleBackColor = false;
            // 
            // buttonSilent
            // 
            this.buttonSilent.CausesValidation = false;
            this.buttonSilent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonSilent.FlatAppearance.BorderSize = 0;
            this.buttonSilent.Location = new System.Drawing.Point(10, 10);
            this.buttonSilent.Margin = new System.Windows.Forms.Padding(10);
            this.buttonSilent.Name = "buttonSilent";
            this.buttonSilent.Size = new System.Drawing.Size(195, 86);
            this.buttonSilent.TabIndex = 0;
            this.buttonSilent.Text = "Silent";
            this.buttonSilent.UseVisualStyleBackColor = true;
            // 
            // groupGPU
            // 
            this.groupGPU.Controls.Add(this.labelGPUFan);
            this.groupGPU.Controls.Add(this.tableGPU);
            this.groupGPU.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupGPU.Location = new System.Drawing.Point(10, 198);
            this.groupGPU.Margin = new System.Windows.Forms.Padding(10);
            this.groupGPU.Name = "groupGPU";
            this.groupGPU.Padding = new System.Windows.Forms.Padding(10);
            this.groupGPU.Size = new System.Drawing.Size(666, 188);
            this.groupGPU.TabIndex = 1;
            this.groupGPU.TabStop = false;
            this.groupGPU.Text = "GPU Mode";
            // 
            // labelGPUFan
            // 
            this.labelGPUFan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelGPUFan.AutoSize = true;
            this.labelGPUFan.Location = new System.Drawing.Point(491, 33);
            this.labelGPUFan.Name = "labelGPUFan";
            this.labelGPUFan.Size = new System.Drawing.Size(155, 32);
            this.labelGPUFan.TabIndex = 3;
            this.labelGPUFan.Text = "GPU Fan : 0%";
            // 
            // tableGPU
            // 
            this.tableGPU.ColumnCount = 3;
            this.tableGPU.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableGPU.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableGPU.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableGPU.Controls.Add(this.buttonUltimate, 2, 0);
            this.tableGPU.Controls.Add(this.buttonStandard, 1, 0);
            this.tableGPU.Controls.Add(this.buttonEco, 0, 0);
            this.tableGPU.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableGPU.Location = new System.Drawing.Point(10, 72);
            this.tableGPU.Name = "tableGPU";
            this.tableGPU.RowCount = 1;
            this.tableGPU.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 106F));
            this.tableGPU.Size = new System.Drawing.Size(646, 106);
            this.tableGPU.TabIndex = 0;
            // 
            // buttonUltimate
            // 
            this.buttonUltimate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonUltimate.FlatAppearance.BorderSize = 0;
            this.buttonUltimate.Location = new System.Drawing.Point(440, 10);
            this.buttonUltimate.Margin = new System.Windows.Forms.Padding(10);
            this.buttonUltimate.Name = "buttonUltimate";
            this.buttonUltimate.Size = new System.Drawing.Size(196, 86);
            this.buttonUltimate.TabIndex = 2;
            this.buttonUltimate.Text = "Ultimate";
            this.buttonUltimate.UseVisualStyleBackColor = true;
            // 
            // buttonStandard
            // 
            this.buttonStandard.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.buttonStandard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonStandard.FlatAppearance.BorderSize = 0;
            this.buttonStandard.Location = new System.Drawing.Point(225, 10);
            this.buttonStandard.Margin = new System.Windows.Forms.Padding(10);
            this.buttonStandard.Name = "buttonStandard";
            this.buttonStandard.Size = new System.Drawing.Size(195, 86);
            this.buttonStandard.TabIndex = 1;
            this.buttonStandard.Text = "Standard";
            this.buttonStandard.UseVisualStyleBackColor = false;
            // 
            // buttonEco
            // 
            this.buttonEco.CausesValidation = false;
            this.buttonEco.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonEco.FlatAppearance.BorderSize = 0;
            this.buttonEco.Location = new System.Drawing.Point(10, 10);
            this.buttonEco.Margin = new System.Windows.Forms.Padding(10);
            this.buttonEco.Name = "buttonEco";
            this.buttonEco.Size = new System.Drawing.Size(195, 86);
            this.buttonEco.TabIndex = 0;
            this.buttonEco.Text = "Eco";
            this.buttonEco.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 636);
            this.Controls.Add(this.groupGPU);
            this.Controls.Add(this.groupPerf);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MdiChildrenMinimizedAnchorBottom = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "G14 Helper";
            this.Load += new System.EventHandler(this.Settings_Load);
            this.groupPerf.ResumeLayout(false);
            this.groupPerf.PerformLayout();
            this.tablePerf.ResumeLayout(false);
            this.groupGPU.ResumeLayout(false);
            this.groupGPU.PerformLayout();
            this.tableGPU.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private GroupBox groupPerf;
        private TableLayoutPanel tablePerf;
        private Button buttonSilent;
        private Button buttonTurbo;
        private Button buttonBalanced;
        private GroupBox groupGPU;
        private TableLayoutPanel tableGPU;
        private Button buttonUltimate;
        private Button buttonStandard;
        private Button buttonEco;
        private Label labelCPUFan;
        private Label labelGPUFan;
    }
}