using GHelper.UI;
namespace GHelper
{
    partial class OverlayConfig
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
            panelToggles = new Panel();
            checkEnable = new GHelper.UI.RCheckBox();
            checkGameOnly = new GHelper.UI.RCheckBox();
            tableModes = new TableLayoutPanel();
            buttonLight = new GHelper.UI.RButton();
            buttonDefault = new GHelper.UI.RButton();
            buttonFull = new GHelper.UI.RButton();
            buttonComplete = new GHelper.UI.RButton();
            tableBlocks = new TableLayoutPanel();
            checkFps = new GHelper.UI.RCheckBox();
            checkTemp = new GHelper.UI.RCheckBox();
            checkFans = new GHelper.UI.RCheckBox();
            checkChart = new GHelper.UI.RCheckBox();
            checkPower = new GHelper.UI.RCheckBox();
            checkUsage = new GHelper.UI.RCheckBox();
            checkRam = new GHelper.UI.RCheckBox();
            checkBattery = new GHelper.UI.RCheckBox();
            checkLabels = new GHelper.UI.RCheckBox();
            panelSliders = new Panel();
            labelSizeTitle = new Label();
            labelSize = new Label();
            trackScale = new RTrackBar();
            labelAlphaTitle = new Label();
            labelAlpha = new Label();
            trackAlpha = new RTrackBar();
            tableColors = new TableLayoutPanel();
            buttonCpuColor = new GHelper.UI.RColorButton();
            buttonGpuColor = new GHelper.UI.RColorButton();
            buttonReset = new GHelper.UI.RButton();
            panelHotkeys = new Panel();
            labelHotkeys = new Label();
            panelToggles.SuspendLayout();
            tableModes.SuspendLayout();
            tableBlocks.SuspendLayout();
            panelSliders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackScale).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackAlpha).BeginInit();
            tableColors.SuspendLayout();
            panelHotkeys.SuspendLayout();
            SuspendLayout();
            //
            // panelToggles
            //
            panelToggles.Controls.Add(checkEnable);
            panelToggles.Controls.Add(checkGameOnly);
            panelToggles.Dock = DockStyle.Top;
            panelToggles.Location = new Point(10, 425);
            panelToggles.Name = "panelToggles";
            panelToggles.Size = new Size(880, 70);
            panelToggles.TabIndex = 0;
            //
            // checkEnable
            //
            checkEnable.AutoSize = true;
            checkEnable.BackColor = SystemColors.ControlLight;
            checkEnable.Location = new Point(10, 10);
            checkEnable.Margin = new Padding(0);
            checkEnable.Name = "checkEnable";
            checkEnable.Padding = new Padding(16, 6, 16, 6);
            checkEnable.Size = new Size(160, 48);
            checkEnable.TabIndex = 0;
            checkEnable.Text = "Overlay";
            checkEnable.UseVisualStyleBackColor = false;
            //
            // checkGameOnly
            //
            checkGameOnly.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            checkGameOnly.AutoSize = true;
            checkGameOnly.BackColor = SystemColors.ControlLight;
            checkGameOnly.Location = new Point(600, 10);
            checkGameOnly.Margin = new Padding(0);
            checkGameOnly.Name = "checkGameOnly";
            checkGameOnly.Padding = new Padding(16, 6, 16, 6);
            checkGameOnly.Size = new Size(280, 48);
            checkGameOnly.TabIndex = 1;
            checkGameOnly.Text = "Overlay only in games";
            checkGameOnly.UseVisualStyleBackColor = false;
            //
            // tableModes
            //
            tableModes.AutoSize = true;
            tableModes.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableModes.ColumnCount = 4;
            tableModes.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableModes.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableModes.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableModes.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableModes.Controls.Add(buttonLight, 0, 0);
            tableModes.Controls.Add(buttonDefault, 1, 0);
            tableModes.Controls.Add(buttonFull, 2, 0);
            tableModes.Controls.Add(buttonComplete, 3, 0);
            tableModes.Dock = DockStyle.Top;
            tableModes.Location = new Point(10, 80);
            tableModes.Name = "tableModes";
            tableModes.RowCount = 1;
            tableModes.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F));
            tableModes.Size = new Size(880, 64);
            tableModes.TabIndex = 1;
            //
            // buttonLight
            //
            buttonLight.Activated = false;
            buttonLight.BackColor = SystemColors.ControlLight;
            buttonLight.BorderColor = Color.Transparent;
            buttonLight.BorderRadius = 2;
            buttonLight.Dock = DockStyle.Fill;
            buttonLight.FlatAppearance.BorderSize = 0;
            buttonLight.FlatStyle = FlatStyle.Flat;
            buttonLight.ForeColor = SystemColors.ControlText;
            buttonLight.Location = new Point(4, 4);
            buttonLight.Margin = new Padding(4, 2, 4, 2);
            buttonLight.Name = "buttonLight";
            buttonLight.Secondary = true;
            buttonLight.Size = new Size(212, 58);
            buttonLight.TabIndex = 0;
            buttonLight.Text = "Light";
            buttonLight.UseVisualStyleBackColor = false;
            //
            // buttonDefault
            //
            buttonDefault.Activated = false;
            buttonDefault.BackColor = SystemColors.ControlLight;
            buttonDefault.BorderColor = Color.Transparent;
            buttonDefault.BorderRadius = 2;
            buttonDefault.Dock = DockStyle.Fill;
            buttonDefault.FlatAppearance.BorderSize = 0;
            buttonDefault.FlatStyle = FlatStyle.Flat;
            buttonDefault.ForeColor = SystemColors.ControlText;
            buttonDefault.Location = new Point(224, 4);
            buttonDefault.Margin = new Padding(4, 2, 4, 2);
            buttonDefault.Name = "buttonDefault";
            buttonDefault.Secondary = true;
            buttonDefault.Size = new Size(212, 58);
            buttonDefault.TabIndex = 1;
            buttonDefault.Text = "Default";
            buttonDefault.UseVisualStyleBackColor = false;
            //
            // buttonFull
            //
            buttonFull.Activated = false;
            buttonFull.BackColor = SystemColors.ControlLight;
            buttonFull.BorderColor = Color.Transparent;
            buttonFull.BorderRadius = 2;
            buttonFull.Dock = DockStyle.Fill;
            buttonFull.FlatAppearance.BorderSize = 0;
            buttonFull.FlatStyle = FlatStyle.Flat;
            buttonFull.ForeColor = SystemColors.ControlText;
            buttonFull.Location = new Point(444, 4);
            buttonFull.Margin = new Padding(4, 2, 4, 2);
            buttonFull.Name = "buttonFull";
            buttonFull.Secondary = true;
            buttonFull.Size = new Size(212, 58);
            buttonFull.TabIndex = 2;
            buttonFull.Text = "Full";
            buttonFull.UseVisualStyleBackColor = false;
            //
            // buttonComplete
            //
            buttonComplete.Activated = false;
            buttonComplete.BackColor = SystemColors.ControlLight;
            buttonComplete.BorderColor = Color.Transparent;
            buttonComplete.BorderRadius = 2;
            buttonComplete.Dock = DockStyle.Fill;
            buttonComplete.FlatAppearance.BorderSize = 0;
            buttonComplete.FlatStyle = FlatStyle.Flat;
            buttonComplete.ForeColor = SystemColors.ControlText;
            buttonComplete.Location = new Point(664, 4);
            buttonComplete.Margin = new Padding(4, 2, 4, 2);
            buttonComplete.Name = "buttonComplete";
            buttonComplete.Secondary = true;
            buttonComplete.Size = new Size(212, 58);
            buttonComplete.TabIndex = 3;
            buttonComplete.Text = "Complete";
            buttonComplete.UseVisualStyleBackColor = false;
            //
            // tableBlocks
            //
            tableBlocks.AutoSize = true;
            tableBlocks.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableBlocks.ColumnCount = 3;
            tableBlocks.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tableBlocks.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tableBlocks.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));
            tableBlocks.Controls.Add(checkFps, 0, 0);
            tableBlocks.Controls.Add(checkTemp, 1, 0);
            tableBlocks.Controls.Add(checkFans, 2, 0);
            tableBlocks.Controls.Add(checkChart, 0, 1);
            tableBlocks.Controls.Add(checkPower, 1, 1);
            tableBlocks.Controls.Add(checkUsage, 2, 1);
            tableBlocks.Controls.Add(checkRam, 0, 2);
            tableBlocks.Controls.Add(checkBattery, 1, 2);
            tableBlocks.Controls.Add(checkLabels, 2, 2);
            tableBlocks.Dock = DockStyle.Top;
            tableBlocks.Location = new Point(10, 160);
            tableBlocks.Name = "tableBlocks";
            tableBlocks.RowCount = 3;
            tableBlocks.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tableBlocks.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tableBlocks.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tableBlocks.Size = new Size(880, 144);
            tableBlocks.TabIndex = 2;
            //
            // checkFps
            //
            checkFps.Anchor = AnchorStyles.Left;
            checkFps.AutoSize = true;
            checkFps.Location = new Point(10, 6);
            checkFps.Margin = new Padding(10, 2, 4, 2);
            checkFps.Name = "checkFps";
            checkFps.Size = new Size(120, 48);
            checkFps.TabIndex = 0;
            checkFps.Text = "FPS";
            //
            // checkTemp
            //
            checkTemp.Anchor = AnchorStyles.Left;
            checkTemp.AutoSize = true;
            checkTemp.Location = new Point(303, 6);
            checkTemp.Margin = new Padding(10, 2, 4, 2);
            checkTemp.Name = "checkTemp";
            checkTemp.Size = new Size(200, 48);
            checkTemp.TabIndex = 1;
            checkTemp.Text = "Temperatures";
            //
            // checkFans
            //
            checkFans.Anchor = AnchorStyles.Left;
            checkFans.AutoSize = true;
            checkFans.Location = new Point(596, 6);
            checkFans.Margin = new Padding(10, 2, 4, 2);
            checkFans.Name = "checkFans";
            checkFans.Size = new Size(120, 48);
            checkFans.TabIndex = 2;
            checkFans.Text = "Fan";
            //
            // checkChart
            //
            checkChart.Anchor = AnchorStyles.Left;
            checkChart.AutoSize = true;
            checkChart.Location = new Point(10, 66);
            checkChart.Margin = new Padding(10, 2, 4, 2);
            checkChart.Name = "checkChart";
            checkChart.Size = new Size(130, 48);
            checkChart.TabIndex = 3;
            checkChart.Text = "Chart";
            //
            // checkPower
            //
            checkPower.Anchor = AnchorStyles.Left;
            checkPower.AutoSize = true;
            checkPower.Location = new Point(303, 66);
            checkPower.Margin = new Padding(10, 2, 4, 2);
            checkPower.Name = "checkPower";
            checkPower.Size = new Size(130, 48);
            checkPower.TabIndex = 4;
            checkPower.Text = "Power";
            //
            // checkUsage
            //
            checkUsage.Anchor = AnchorStyles.Left;
            checkUsage.AutoSize = true;
            checkUsage.Location = new Point(596, 66);
            checkUsage.Margin = new Padding(10, 2, 4, 2);
            checkUsage.Name = "checkUsage";
            checkUsage.Size = new Size(120, 48);
            checkUsage.TabIndex = 5;
            checkUsage.Text = "Load";
            //
            // checkRam
            //
            checkRam.Anchor = AnchorStyles.Left;
            checkRam.AutoSize = true;
            checkRam.Location = new Point(10, 126);
            checkRam.Margin = new Padding(10, 2, 4, 2);
            checkRam.Name = "checkRam";
            checkRam.Size = new Size(120, 48);
            checkRam.TabIndex = 6;
            checkRam.Text = "RAM";
            //
            // checkBattery
            //
            checkBattery.Anchor = AnchorStyles.Left;
            checkBattery.AutoSize = true;
            checkBattery.Location = new Point(303, 126);
            checkBattery.Margin = new Padding(10, 2, 4, 2);
            checkBattery.Name = "checkBattery";
            checkBattery.Size = new Size(140, 48);
            checkBattery.TabIndex = 7;
            checkBattery.Text = "Battery";
            checkBattery.ThreeState = true;
            //
            // checkLabels
            //
            checkLabels.Anchor = AnchorStyles.Left;
            checkLabels.AutoSize = true;
            checkLabels.Location = new Point(596, 126);
            checkLabels.Margin = new Padding(10, 2, 4, 2);
            checkLabels.Name = "checkLabels";
            checkLabels.Size = new Size(130, 48);
            checkLabels.TabIndex = 8;
            checkLabels.Text = "Labels";
            //
            // panelSliders
            //
            panelSliders.Controls.Add(labelSizeTitle);
            panelSliders.Controls.Add(labelSize);
            panelSliders.Controls.Add(trackScale);
            panelSliders.Controls.Add(labelAlphaTitle);
            panelSliders.Controls.Add(labelAlpha);
            panelSliders.Controls.Add(trackAlpha);
            panelSliders.Dock = DockStyle.Top;
            panelSliders.Location = new Point(10, 340);
            panelSliders.Name = "panelSliders";
            panelSliders.Size = new Size(880, 120);
            panelSliders.TabIndex = 3;
            //
            // labelSizeTitle
            //
            labelSizeTitle.AutoSize = true;
            labelSizeTitle.Location = new Point(10, 8);
            labelSizeTitle.Margin = new Padding(4, 0, 4, 0);
            labelSizeTitle.Name = "labelSizeTitle";
            labelSizeTitle.Size = new Size(60, 32);
            labelSizeTitle.TabIndex = 0;
            labelSizeTitle.Text = "Size";
            //
            // labelSize
            //
            labelSize.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelSize.Location = new Point(330, 8);
            labelSize.Margin = new Padding(4, 0, 4, 0);
            labelSize.Name = "labelSize";
            labelSize.Size = new Size(100, 32);
            labelSize.TabIndex = 1;
            labelSize.Text = "100%";
            labelSize.TextAlign = ContentAlignment.TopRight;
            //
            // trackScale
            //
            trackScale.LargeChange = 25;
            trackScale.Location = new Point(6, 44);
            trackScale.Margin = new Padding(4, 2, 4, 2);
            trackScale.Maximum = 300;
            trackScale.Minimum = 35;
            trackScale.Name = "trackScale";
            trackScale.Size = new Size(424, 60);
            trackScale.SmallChange = 5;
            trackScale.TabIndex = 2;
            trackScale.TickFrequency = 25;
            trackScale.TickStyle = TickStyle.TopLeft;
            trackScale.Value = 100;
            //
            // labelAlphaTitle
            //
            labelAlphaTitle.AutoSize = true;
            labelAlphaTitle.Location = new Point(460, 8);
            labelAlphaTitle.Margin = new Padding(4, 0, 4, 0);
            labelAlphaTitle.Name = "labelAlphaTitle";
            labelAlphaTitle.Size = new Size(150, 32);
            labelAlphaTitle.TabIndex = 3;
            labelAlphaTitle.Text = "Transparency";
            //
            // labelAlpha
            //
            labelAlpha.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelAlpha.Location = new Point(780, 8);
            labelAlpha.Margin = new Padding(4, 0, 4, 0);
            labelAlpha.Name = "labelAlpha";
            labelAlpha.Size = new Size(100, 32);
            labelAlpha.TabIndex = 4;
            labelAlpha.Text = "50%";
            labelAlpha.TextAlign = ContentAlignment.TopRight;
            //
            // trackAlpha
            //
            trackAlpha.LargeChange = 32;
            trackAlpha.Location = new Point(456, 44);
            trackAlpha.Margin = new Padding(4, 2, 4, 2);
            trackAlpha.Maximum = 255;
            trackAlpha.Name = "trackAlpha";
            trackAlpha.Size = new Size(424, 60);
            trackAlpha.SmallChange = 8;
            trackAlpha.TabIndex = 5;
            trackAlpha.TickFrequency = 16;
            trackAlpha.TickStyle = TickStyle.TopLeft;
            trackAlpha.Value = 128;
            //
            // tableColors
            //
            tableColors.AutoSize = true;
            tableColors.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableColors.ColumnCount = 3;
            tableColors.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tableColors.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tableColors.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));
            tableColors.Controls.Add(buttonCpuColor, 0, 0);
            tableColors.Controls.Add(buttonGpuColor, 1, 0);
            tableColors.Controls.Add(buttonReset, 2, 0);
            tableColors.Dock = DockStyle.Top;
            tableColors.Location = new Point(10, 460);
            tableColors.Name = "tableColors";
            tableColors.RowCount = 1;
            tableColors.RowStyles.Add(new RowStyle(SizeType.Absolute, 63F));
            tableColors.Size = new Size(880, 63);
            tableColors.TabIndex = 4;
            //
            // buttonCpuColor
            //
            buttonCpuColor.Activated = false;
            buttonCpuColor.BackColor = SystemColors.ControlLight;
            buttonCpuColor.BorderColor = Color.Transparent;
            buttonCpuColor.BorderRadius = 2;
            buttonCpuColor.Dock = DockStyle.Fill;
            buttonCpuColor.FlatAppearance.BorderSize = 0;
            buttonCpuColor.FlatStyle = FlatStyle.Flat;
            buttonCpuColor.ForeColor = SystemColors.ControlText;
            buttonCpuColor.Location = new Point(4, 4);
            buttonCpuColor.Margin = new Padding(4);
            buttonCpuColor.Name = "buttonCpuColor";
            buttonCpuColor.Secondary = true;
            buttonCpuColor.Size = new Size(285, 55);
            buttonCpuColor.TabIndex = 0;
            buttonCpuColor.Text = "CPU Color";
            buttonCpuColor.UseVisualStyleBackColor = false;
            //
            // buttonGpuColor
            //
            buttonGpuColor.Activated = false;
            buttonGpuColor.BackColor = SystemColors.ControlLight;
            buttonGpuColor.BorderColor = Color.Transparent;
            buttonGpuColor.BorderRadius = 2;
            buttonGpuColor.Dock = DockStyle.Fill;
            buttonGpuColor.FlatAppearance.BorderSize = 0;
            buttonGpuColor.FlatStyle = FlatStyle.Flat;
            buttonGpuColor.ForeColor = SystemColors.ControlText;
            buttonGpuColor.Location = new Point(297, 4);
            buttonGpuColor.Margin = new Padding(4);
            buttonGpuColor.Name = "buttonGpuColor";
            buttonGpuColor.Secondary = true;
            buttonGpuColor.Size = new Size(285, 55);
            buttonGpuColor.TabIndex = 1;
            buttonGpuColor.Text = "GPU Color";
            buttonGpuColor.UseVisualStyleBackColor = false;
            //
            // buttonReset
            //
            buttonReset.Activated = false;
            buttonReset.BackColor = SystemColors.ControlLight;
            buttonReset.BorderColor = Color.Transparent;
            buttonReset.BorderRadius = 2;
            buttonReset.Dock = DockStyle.Fill;
            buttonReset.FlatAppearance.BorderSize = 0;
            buttonReset.FlatStyle = FlatStyle.Flat;
            buttonReset.ForeColor = SystemColors.ControlText;
            buttonReset.Location = new Point(590, 4);
            buttonReset.Margin = new Padding(4);
            buttonReset.Name = "buttonReset";
            buttonReset.Secondary = true;
            buttonReset.Size = new Size(286, 55);
            buttonReset.TabIndex = 2;
            buttonReset.Text = "Reset";
            buttonReset.UseVisualStyleBackColor = false;
            //
            // panelHotkeys
            //
            panelHotkeys.Controls.Add(labelHotkeys);
            panelHotkeys.Dock = DockStyle.Top;
            panelHotkeys.Location = new Point(10, 425);
            panelHotkeys.Name = "panelHotkeys";
            panelHotkeys.Size = new Size(880, 180);
            panelHotkeys.TabIndex = 5;
            //
            // labelHotkeys
            //
            labelHotkeys.AutoSize = true;
            labelHotkeys.Font = new Font("Consolas", 9F);
            labelHotkeys.ForeColor = SystemColors.ControlDarkDark;
            labelHotkeys.Location = new Point(10, 8);
            labelHotkeys.Margin = new Padding(4, 0, 4, 0);
            labelHotkeys.Name = "labelHotkeys";
            labelHotkeys.Size = new Size(400, 160);
            labelHotkeys.TabIndex = 0;
            //
            // OverlayConfig
            //
            AutoScaleDimensions = new SizeF(192F, 192F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(900, 675);
            Controls.Add(panelToggles);
            Controls.Add(panelHotkeys);
            Controls.Add(tableColors);
            Controls.Add(panelSliders);
            Controls.Add(tableBlocks);
            Controls.Add(tableModes);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "OverlayConfig";
            Padding = new Padding(10);
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Text = "Overlay";
            panelToggles.ResumeLayout(false);
            panelToggles.PerformLayout();
            tableModes.ResumeLayout(false);
            tableBlocks.ResumeLayout(false);
            tableBlocks.PerformLayout();
            panelSliders.ResumeLayout(false);
            panelSliders.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackScale).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackAlpha).EndInit();
            tableColors.ResumeLayout(false);
            panelHotkeys.ResumeLayout(false);
            panelHotkeys.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panelToggles;
        private UI.RCheckBox checkEnable;
        private UI.RCheckBox checkGameOnly;
        private TableLayoutPanel tableModes;
        private UI.RButton buttonLight;
        private UI.RButton buttonDefault;
        private UI.RButton buttonFull;
        private UI.RButton buttonComplete;
        private TableLayoutPanel tableBlocks;
        private UI.RCheckBox checkFps;
        private UI.RCheckBox checkTemp;
        private UI.RCheckBox checkFans;
        private UI.RCheckBox checkChart;
        private UI.RCheckBox checkPower;
        private UI.RCheckBox checkUsage;
        private UI.RCheckBox checkRam;
        private UI.RCheckBox checkBattery;
        private UI.RCheckBox checkLabels;
        private Panel panelSliders;
        private Label labelSizeTitle;
        private Label labelSize;
        private RTrackBar trackScale;
        private Label labelAlphaTitle;
        private Label labelAlpha;
        private RTrackBar trackAlpha;
        private TableLayoutPanel tableColors;
        private UI.RColorButton buttonCpuColor;
        private UI.RColorButton buttonGpuColor;
        private UI.RButton buttonReset;
        private Panel panelHotkeys;
        private Label labelHotkeys;
    }
}
