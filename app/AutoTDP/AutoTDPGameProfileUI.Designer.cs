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
            panelGameSettings = new Panel();
            labelMinTDP = new Label();
            labelMaxTDP = new Label();
            sliderMaxTDP = new UI.Slider();
            numericUpDownFPS = new NumericUpDown();
            sliderMinTDP = new UI.Slider();
            labelMaxTDPText = new Label();
            labeMinTDPText = new Label();
            labelTargetFPS = new Label();
            textBoxTitle = new TextBox();
            textBoxProcessName = new TextBox();
            labelFPSSource = new Label();
            labelLimiter = new Label();
            buttonSave = new UI.RButton();
            buttonDelete = new UI.RButton();
            panelPerformanceHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureKeyboard).BeginInit();
            panelGameSettings.SuspendLayout();
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
            panelPerformanceHeader.Name = "panelPerformanceHeader";
            panelPerformanceHeader.Size = new Size(551, 50);
            panelPerformanceHeader.TabIndex = 53;
            // 
            // pictureKeyboard
            // 
            pictureKeyboard.BackgroundImage = Properties.Resources.icons8_automation_32;
            pictureKeyboard.BackgroundImageLayout = ImageLayout.Zoom;
            pictureKeyboard.Location = new Point(4, 13);
            pictureKeyboard.Name = "pictureKeyboard";
            pictureKeyboard.Size = new Size(23, 27);
            pictureKeyboard.TabIndex = 35;
            pictureKeyboard.TabStop = false;
            // 
            // labelSettings
            // 
            labelSettings.AutoSize = true;
            labelSettings.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelSettings.Location = new Point(31, 13);
            labelSettings.Margin = new Padding(6, 0, 6, 0);
            labelSettings.Name = "labelSettings";
            labelSettings.Size = new Size(135, 25);
            labelSettings.TabIndex = 34;
            labelSettings.Text = "Game Settings";
            // 
            // checkBoxEnabled
            // 
            checkBoxEnabled.Location = new Point(344, 3);
            checkBoxEnabled.Margin = new Padding(6, 0, 6, 0);
            checkBoxEnabled.Name = "checkBoxEnabled";
            checkBoxEnabled.Size = new Size(194, 42);
            checkBoxEnabled.TabIndex = 53;
            checkBoxEnabled.Text = "Enabled";
            checkBoxEnabled.UseVisualStyleBackColor = true;
            // 
            // panelGameSettings
            // 
            panelGameSettings.AutoSize = true;
            panelGameSettings.Controls.Add(labelMinTDP);
            panelGameSettings.Controls.Add(labelMaxTDP);
            panelGameSettings.Controls.Add(sliderMaxTDP);
            panelGameSettings.Controls.Add(numericUpDownFPS);
            panelGameSettings.Controls.Add(sliderMinTDP);
            panelGameSettings.Controls.Add(labelMaxTDPText);
            panelGameSettings.Controls.Add(labeMinTDPText);
            panelGameSettings.Controls.Add(labelTargetFPS);
            panelGameSettings.Controls.Add(textBoxTitle);
            panelGameSettings.Controls.Add(textBoxProcessName);
            panelGameSettings.Controls.Add(labelFPSSource);
            panelGameSettings.Controls.Add(labelLimiter);
            panelGameSettings.Dock = DockStyle.Top;
            panelGameSettings.Location = new Point(0, 50);
            panelGameSettings.Name = "panelGameSettings";
            panelGameSettings.Padding = new Padding(0, 0, 0, 12);
            panelGameSettings.Size = new Size(551, 244);
            panelGameSettings.TabIndex = 57;
            // 
            // labelMinTDP
            // 
            labelMinTDP.Location = new Point(489, 152);
            labelMinTDP.Margin = new Padding(4, 0, 4, 0);
            labelMinTDP.Name = "labelMinTDP";
            labelMinTDP.Size = new Size(56, 37);
            labelMinTDP.TabIndex = 67;
            labelMinTDP.Text = "30W";
            labelMinTDP.TextAlign = ContentAlignment.MiddleRight;
            // 
            // labelMaxTDP
            // 
            labelMaxTDP.Location = new Point(487, 195);
            labelMaxTDP.Margin = new Padding(4, 0, 4, 0);
            labelMaxTDP.Name = "labelMaxTDP";
            labelMaxTDP.Size = new Size(57, 37);
            labelMaxTDP.TabIndex = 66;
            labelMaxTDP.Text = "150W";
            labelMaxTDP.TextAlign = ContentAlignment.MiddleRight;
            // 
            // sliderMaxTDP
            // 
            sliderMaxTDP.AccessibleName = "DPI Slider";
            sliderMaxTDP.Location = new Point(200, 195);
            sliderMaxTDP.Max = 200;
            sliderMaxTDP.Min = 5;
            sliderMaxTDP.Name = "sliderMaxTDP";
            sliderMaxTDP.Size = new Size(291, 33);
            sliderMaxTDP.Step = 1;
            sliderMaxTDP.TabIndex = 65;
            sliderMaxTDP.TabStop = false;
            sliderMaxTDP.Text = "sliderBattery";
            sliderMaxTDP.Value = 0;
            // 
            // numericUpDownFPS
            // 
            numericUpDownFPS.BorderStyle = BorderStyle.None;
            numericUpDownFPS.Location = new Point(416, 110);
            numericUpDownFPS.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numericUpDownFPS.Minimum = new decimal(new int[] { 20, 0, 0, 0 });
            numericUpDownFPS.Name = "numericUpDownFPS";
            numericUpDownFPS.Size = new Size(123, 27);
            numericUpDownFPS.TabIndex = 64;
            numericUpDownFPS.TextAlign = HorizontalAlignment.Center;
            numericUpDownFPS.Value = new decimal(new int[] { 60, 0, 0, 0 });
            // 
            // sliderMinTDP
            // 
            sliderMinTDP.AccessibleName = "DPI Slider";
            sliderMinTDP.Location = new Point(200, 155);
            sliderMinTDP.Max = 200;
            sliderMinTDP.Min = 5;
            sliderMinTDP.Name = "sliderMinTDP";
            sliderMinTDP.Size = new Size(291, 33);
            sliderMinTDP.Step = 1;
            sliderMinTDP.TabIndex = 63;
            sliderMinTDP.TabStop = false;
            sliderMinTDP.Text = "sliderBattery";
            sliderMinTDP.Value = 0;
            // 
            // labelMaxTDPText
            // 
            labelMaxTDPText.Location = new Point(7, 195);
            labelMaxTDPText.Margin = new Padding(6, 0, 6, 0);
            labelMaxTDPText.Name = "labelMaxTDPText";
            labelMaxTDPText.Size = new Size(184, 37);
            labelMaxTDPText.TabIndex = 61;
            labelMaxTDPText.Text = "Max TDP:";
            labelMaxTDPText.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // labeMinTDPText
            // 
            labeMinTDPText.Location = new Point(7, 152);
            labeMinTDPText.Margin = new Padding(6, 0, 6, 0);
            labeMinTDPText.Name = "labeMinTDPText";
            labeMinTDPText.Size = new Size(184, 37);
            labeMinTDPText.TabIndex = 62;
            labeMinTDPText.Text = "Min TDP:";
            labeMinTDPText.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // labelTargetFPS
            // 
            labelTargetFPS.Location = new Point(7, 105);
            labelTargetFPS.Margin = new Padding(6, 0, 6, 0);
            labelTargetFPS.Name = "labelTargetFPS";
            labelTargetFPS.Size = new Size(184, 37);
            labelTargetFPS.TabIndex = 60;
            labelTargetFPS.Text = "Target FPS:";
            labelTargetFPS.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // textBoxTitle
            // 
            textBoxTitle.Location = new Point(200, 60);
            textBoxTitle.Margin = new Padding(4, 5, 4, 5);
            textBoxTitle.Name = "textBoxTitle";
            textBoxTitle.Size = new Size(337, 31);
            textBoxTitle.TabIndex = 59;
            // 
            // textBoxProcessName
            // 
            textBoxProcessName.Location = new Point(200, 10);
            textBoxProcessName.Margin = new Padding(4, 5, 4, 5);
            textBoxProcessName.Name = "textBoxProcessName";
            textBoxProcessName.ReadOnly = true;
            textBoxProcessName.Size = new Size(337, 31);
            textBoxProcessName.TabIndex = 58;
            textBoxProcessName.WordWrap = false;
            // 
            // labelFPSSource
            // 
            labelFPSSource.Location = new Point(6, 62);
            labelFPSSource.Margin = new Padding(6, 0, 6, 0);
            labelFPSSource.Name = "labelFPSSource";
            labelFPSSource.Size = new Size(184, 37);
            labelFPSSource.TabIndex = 57;
            labelFPSSource.Text = "Name:";
            labelFPSSource.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // labelLimiter
            // 
            labelLimiter.Location = new Point(6, 12);
            labelLimiter.Margin = new Padding(6, 0, 6, 0);
            labelLimiter.Name = "labelLimiter";
            labelLimiter.Size = new Size(184, 37);
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
            buttonSave.Location = new Point(399, 303);
            buttonSave.Margin = new Padding(3, 7, 3, 7);
            buttonSave.Name = "buttonSave";
            buttonSave.Secondary = false;
            buttonSave.Size = new Size(147, 42);
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
            buttonDelete.Location = new Point(6, 303);
            buttonDelete.Margin = new Padding(3, 7, 3, 7);
            buttonDelete.Name = "buttonDelete";
            buttonDelete.Secondary = false;
            buttonDelete.Size = new Size(147, 42);
            buttonDelete.TabIndex = 62;
            buttonDelete.Text = "Delete";
            buttonDelete.UseVisualStyleBackColor = false;
            // 
            // AutoTDPGameProfileUI
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(551, 362);
            Controls.Add(buttonDelete);
            Controls.Add(buttonSave);
            Controls.Add(panelGameSettings);
            Controls.Add(panelPerformanceHeader);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4, 5, 4, 5);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AutoTDPGameProfileUI";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Game Profile";
            panelPerformanceHeader.ResumeLayout(false);
            panelPerformanceHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureKeyboard).EndInit();
            panelGameSettings.ResumeLayout(false);
            panelGameSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownFPS).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panelPerformanceHeader;
        private PictureBox pictureKeyboard;
        private Label labelSettings;
        private CheckBox checkBoxEnabled;
        private Panel panelGameSettings;
        private Label labelFPSSource;
        private Label labelLimiter;
        private TextBox textBoxTitle;
        private TextBox textBoxProcessName;
        private UI.RButton buttonSave;
        private Label labelMaxTDPText;
        private Label labeMinTDPText;
        private Label labelTargetFPS;
        private UI.Slider sliderMinTDP;
        private UI.Slider sliderMaxTDP;
        private NumericUpDown numericUpDownFPS;
        private Label labelMinTDP;
        private Label labelMaxTDP;
        private UI.RButton buttonDelete;
    }
}