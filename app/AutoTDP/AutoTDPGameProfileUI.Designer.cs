namespace GHelper.AutoTDP
{
    partial class AutoTDPGameProfileUI
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
            panelPerformanceHeader = new Panel();
            pictureKeyboard = new PictureBox();
            labelSettings = new Label();
            checkBoxEnabled = new CheckBox();
            panelLightingContent = new Panel();
            labelMinTDP = new Label();
            labelMaxTDP = new Label();
            sliderMaxTDP = new UI.Slider();
            numericUpDownFPS = new NumericUpDown();
            sliderMinTDP = new UI.Slider();
            label2 = new Label();
            label3 = new Label();
            label1 = new Label();
            textBoxTitle = new TextBox();
            textBoxProcessName = new TextBox();
            labelFPSSource = new Label();
            labelLimiter = new Label();
            buttonSave = new UI.RButton();
            buttonDelete = new UI.RButton();
            panelPerformanceHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureKeyboard).BeginInit();
            panelLightingContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownFPS).BeginInit();
            SuspendLayout();
            // 
            // panelPerformanceHeader
            // 
            panelPerformanceHeader.BackColor = SystemColors.ControlLight;
            panelPerformanceHeader.Controls.Add(pictureKeyboard);
            panelPerformanceHeader.Controls.Add(labelSettings);
            panelPerformanceHeader.Controls.Add(checkBoxEnabled);
            panelPerformanceHeader.Dock = DockStyle.Top;
            panelPerformanceHeader.Location = new Point(0, 0);
            panelPerformanceHeader.Margin = new Padding(2);
            panelPerformanceHeader.Name = "panelPerformanceHeader";
            panelPerformanceHeader.Size = new Size(386, 30);
            panelPerformanceHeader.TabIndex = 53;
            // 
            // pictureKeyboard
            // 
            pictureKeyboard.BackgroundImage = Properties.Resources.icons8_automation_32;
            pictureKeyboard.BackgroundImageLayout = ImageLayout.Zoom;
            pictureKeyboard.Location = new Point(3, 8);
            pictureKeyboard.Margin = new Padding(2);
            pictureKeyboard.Name = "pictureKeyboard";
            pictureKeyboard.Size = new Size(16, 16);
            pictureKeyboard.TabIndex = 35;
            pictureKeyboard.TabStop = false;
            // 
            // labelSettings
            // 
            labelSettings.AutoSize = true;
            labelSettings.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelSettings.Location = new Point(22, 8);
            labelSettings.Margin = new Padding(4, 0, 4, 0);
            labelSettings.Name = "labelSettings";
            labelSettings.Size = new Size(89, 15);
            labelSettings.TabIndex = 34;
            labelSettings.Text = "Game Settings";
            // 
            // checkBoxEnabled
            // 
            checkBoxEnabled.Location = new Point(241, 2);
            checkBoxEnabled.Margin = new Padding(4, 0, 4, 0);
            checkBoxEnabled.Name = "checkBoxEnabled";
            checkBoxEnabled.Size = new Size(136, 25);
            checkBoxEnabled.TabIndex = 53;
            checkBoxEnabled.Text = "Enabled";
            checkBoxEnabled.UseVisualStyleBackColor = true;
            // 
            // panelLightingContent
            // 
            panelLightingContent.AutoSize = true;
            panelLightingContent.Controls.Add(labelMinTDP);
            panelLightingContent.Controls.Add(labelMaxTDP);
            panelLightingContent.Controls.Add(sliderMaxTDP);
            panelLightingContent.Controls.Add(numericUpDownFPS);
            panelLightingContent.Controls.Add(sliderMinTDP);
            panelLightingContent.Controls.Add(label2);
            panelLightingContent.Controls.Add(label3);
            panelLightingContent.Controls.Add(label1);
            panelLightingContent.Controls.Add(textBoxTitle);
            panelLightingContent.Controls.Add(textBoxProcessName);
            panelLightingContent.Controls.Add(labelFPSSource);
            panelLightingContent.Controls.Add(labelLimiter);
            panelLightingContent.Dock = DockStyle.Top;
            panelLightingContent.Location = new Point(0, 30);
            panelLightingContent.Margin = new Padding(2);
            panelLightingContent.Name = "panelLightingContent";
            panelLightingContent.Padding = new Padding(0, 0, 0, 7);
            panelLightingContent.Size = new Size(386, 146);
            panelLightingContent.TabIndex = 57;
            // 
            // labelMinTDP
            // 
            labelMinTDP.Location = new Point(342, 91);
            labelMinTDP.Name = "labelMinTDP";
            labelMinTDP.Size = new Size(39, 22);
            labelMinTDP.TabIndex = 67;
            labelMinTDP.Text = "30W";
            labelMinTDP.TextAlign = ContentAlignment.MiddleRight;
            // 
            // labelMaxTDP
            // 
            labelMaxTDP.Location = new Point(341, 117);
            labelMaxTDP.Name = "labelMaxTDP";
            labelMaxTDP.Size = new Size(40, 22);
            labelMaxTDP.TabIndex = 66;
            labelMaxTDP.Text = "150W";
            labelMaxTDP.TextAlign = ContentAlignment.MiddleRight;
            // 
            // sliderMaxTDP
            // 
            sliderMaxTDP.AccessibleName = "DPI Slider";
            sliderMaxTDP.Location = new Point(140, 117);
            sliderMaxTDP.Margin = new Padding(2);
            sliderMaxTDP.Max = 200;
            sliderMaxTDP.Min = 5;
            sliderMaxTDP.Name = "sliderMaxTDP";
            sliderMaxTDP.Size = new Size(204, 20);
            sliderMaxTDP.Step = 1;
            sliderMaxTDP.TabIndex = 65;
            sliderMaxTDP.TabStop = false;
            sliderMaxTDP.Text = "sliderBattery";
            sliderMaxTDP.Value = 0;
            // 
            // numericUpDownFPS
            // 
            numericUpDownFPS.BorderStyle = BorderStyle.None;
            numericUpDownFPS.Location = new Point(291, 66);
            numericUpDownFPS.Margin = new Padding(2);
            numericUpDownFPS.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numericUpDownFPS.Minimum = new decimal(new int[] { 20, 0, 0, 0 });
            numericUpDownFPS.Name = "numericUpDownFPS";
            numericUpDownFPS.Size = new Size(86, 19);
            numericUpDownFPS.TabIndex = 64;
            numericUpDownFPS.TextAlign = HorizontalAlignment.Center;
            numericUpDownFPS.Value = new decimal(new int[] { 60, 0, 0, 0 });
            // 
            // sliderMinTDP
            // 
            sliderMinTDP.AccessibleName = "DPI Slider";
            sliderMinTDP.Location = new Point(140, 93);
            sliderMinTDP.Margin = new Padding(2);
            sliderMinTDP.Max = 200;
            sliderMinTDP.Min = 5;
            sliderMinTDP.Name = "sliderMinTDP";
            sliderMinTDP.Size = new Size(204, 20);
            sliderMinTDP.Step = 1;
            sliderMinTDP.TabIndex = 63;
            sliderMinTDP.TabStop = false;
            sliderMinTDP.Text = "sliderBattery";
            sliderMinTDP.Value = 0;
            // 
            // label2
            // 
            label2.Location = new Point(5, 117);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(129, 22);
            label2.TabIndex = 61;
            label2.Text = "Max TDP:";
            label2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            label3.Location = new Point(5, 91);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(129, 22);
            label3.TabIndex = 62;
            label3.Text = "Min TDP:";
            label3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            label1.Location = new Point(5, 63);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(129, 22);
            label1.TabIndex = 60;
            label1.Text = "Target FPS:";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // textBoxTitle
            // 
            textBoxTitle.Location = new Point(140, 36);
            textBoxTitle.Name = "textBoxTitle";
            textBoxTitle.Size = new Size(237, 23);
            textBoxTitle.TabIndex = 59;
            // 
            // textBoxProcessName
            // 
            textBoxProcessName.Location = new Point(140, 6);
            textBoxProcessName.Name = "textBoxProcessName";
            textBoxProcessName.ReadOnly = true;
            textBoxProcessName.Size = new Size(237, 23);
            textBoxProcessName.TabIndex = 58;
            textBoxProcessName.WordWrap = false;
            // 
            // labelFPSSource
            // 
            labelFPSSource.Location = new Point(4, 37);
            labelFPSSource.Margin = new Padding(4, 0, 4, 0);
            labelFPSSource.Name = "labelFPSSource";
            labelFPSSource.Size = new Size(129, 22);
            labelFPSSource.TabIndex = 57;
            labelFPSSource.Text = "Name:";
            labelFPSSource.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // labelLimiter
            // 
            labelLimiter.Location = new Point(4, 7);
            labelLimiter.Margin = new Padding(4, 0, 4, 0);
            labelLimiter.Name = "labelLimiter";
            labelLimiter.Size = new Size(129, 22);
            labelLimiter.TabIndex = 47;
            labelLimiter.Text = "Process";
            labelLimiter.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // buttonSave
            // 
            buttonSave.AccessibleName = "Keyboard Color";
            buttonSave.Activated = false;
            buttonSave.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonSave.BackColor = SystemColors.ButtonHighlight;
            buttonSave.BorderColor = Color.Transparent;
            buttonSave.BorderRadius = 2;
            buttonSave.FlatStyle = FlatStyle.Flat;
            buttonSave.ForeColor = SystemColors.ControlText;
            buttonSave.Location = new Point(279, 182);
            buttonSave.Margin = new Padding(2, 4, 2, 4);
            buttonSave.Name = "buttonSave";
            buttonSave.Secondary = false;
            buttonSave.Size = new Size(103, 25);
            buttonSave.TabIndex = 61;
            buttonSave.Text = "Save";
            buttonSave.UseVisualStyleBackColor = false;
            // 
            // buttonDelete
            // 
            buttonDelete.AccessibleName = "Keyboard Color";
            buttonDelete.Activated = false;
            buttonDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonDelete.BackColor = SystemColors.ButtonHighlight;
            buttonDelete.BorderColor = Color.Transparent;
            buttonDelete.BorderRadius = 2;
            buttonDelete.FlatStyle = FlatStyle.Flat;
            buttonDelete.ForeColor = SystemColors.ControlText;
            buttonDelete.Location = new Point(4, 182);
            buttonDelete.Margin = new Padding(2, 4, 2, 4);
            buttonDelete.Name = "buttonDelete";
            buttonDelete.Secondary = false;
            buttonDelete.Size = new Size(103, 25);
            buttonDelete.TabIndex = 62;
            buttonDelete.Text = "Delete";
            buttonDelete.UseVisualStyleBackColor = false;
            // 
            // AutoTDPGameProfileUI
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(386, 217);
            Controls.Add(buttonDelete);
            Controls.Add(buttonSave);
            Controls.Add(panelLightingContent);
            Controls.Add(panelPerformanceHeader);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AutoTDPGameProfileUI";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Game Profile";
            panelPerformanceHeader.ResumeLayout(false);
            panelPerformanceHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureKeyboard).EndInit();
            panelLightingContent.ResumeLayout(false);
            panelLightingContent.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownFPS).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panelPerformanceHeader;
        private PictureBox pictureKeyboard;
        private Label labelSettings;
        private CheckBox checkBoxEnabled;
        private Panel panelLightingContent;
        private Label labelFPSSource;
        private Label labelLimiter;
        private TextBox textBoxTitle;
        private TextBox textBoxProcessName;
        private UI.RButton buttonSave;
        private Label label2;
        private Label label3;
        private Label label1;
        private UI.Slider sliderMinTDP;
        private UI.Slider sliderMaxTDP;
        private NumericUpDown numericUpDownFPS;
        private Label labelMinTDP;
        private Label labelMaxTDP;
        private UI.RButton buttonDelete;
    }
}