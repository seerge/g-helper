using GHelper.UI;
namespace GHelper
{
    partial class Matrix
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
            picturePreview = new PictureBox();
            panelTabs = new TableLayoutPanel();
            buttonPictureMode = new UI.RButton();
            buttonClockMode = new UI.RButton();
            buttonAudioMode = new UI.RButton();
            buttonTextMode = new UI.RButton();
            panelPicture = new Panel();
            panelMain = new Panel();
            panelPictureSettings = new Panel();
            panelTextSettings = new Panel();
            panelClockSettings = new Panel();
            panelButtons = new Panel();
            buttonPicture = new UI.RButton();
            buttonReset = new UI.RButton();
            panelPower = new Panel();
            checkLidOff = new RCheckBox();
            panelPowerSpacer = new Panel();
            checkAutoOff = new RCheckBox();
            panelSliders = new TableLayoutPanel();
            panelGamma = new Panel();
            labelGamma = new Label();
            labelGammaTitle = new Label();
            trackGamma = new RTrackBar();
            panelContrast = new Panel();
            labelContrast = new Label();
            labelContrastTitle = new Label();
            trackContrast = new RTrackBar();
            panelRotation = new Panel();
            comboRotation = new UI.RComboBox();
            labelRotation = new Label();
            panelScaling = new Panel();
            comboScaling = new UI.RComboBox();
            labelScaling = new Label();
            panelZoom = new Panel();
            labelZoom = new Label();
            labelZoomTitle = new Label();
            trackZoom = new RTrackBar();
            panelText = new Panel();
            numTextSize = new RNumericUpDown();
            comboTextFont = new UI.RComboBox();
            textMatrix = new UI.RTextBox();
            panelText2 = new Panel();
            numTextSize2 = new RNumericUpDown();
            comboTextFont2 = new UI.RComboBox();
            textMatrix2 = new UI.RTextBox();
            panelTextRunning = new Panel();
            checkTextRunning = new CheckBox();
            panelClockTime = new Panel();
            textClockTime = new UI.RTextBox();
            labelClockTime = new Label();
            panelClockDate = new Panel();
            textClockDate = new UI.RTextBox();
            labelClockDate = new Label();
            panelClockBattery = new Panel();
            checkClockBattery = new CheckBox();
            panelAudioSettings = new Panel();
            comboAudioMode = new UI.RComboBox();
            labelAudioMode = new Label();
            ((System.ComponentModel.ISupportInitialize)picturePreview).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackZoom).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackGamma).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackContrast).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numTextSize).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numTextSize2).BeginInit();
            panelPicture.SuspendLayout();
            panelTabs.SuspendLayout();
            panelMain.SuspendLayout();
            panelPictureSettings.SuspendLayout();
            panelTextSettings.SuspendLayout();
            panelClockSettings.SuspendLayout();
            panelButtons.SuspendLayout();
            panelPower.SuspendLayout();
            panelSliders.SuspendLayout();
            panelGamma.SuspendLayout();
            panelContrast.SuspendLayout();
            panelRotation.SuspendLayout();
            panelScaling.SuspendLayout();
            panelZoom.SuspendLayout();
            panelText.SuspendLayout();
            panelText2.SuspendLayout();
            panelTextRunning.SuspendLayout();
            panelClockTime.SuspendLayout();
            panelClockDate.SuspendLayout();
            panelClockBattery.SuspendLayout();
            panelAudioSettings.SuspendLayout();
            SuspendLayout();
            //
            // picturePreview
            //
            picturePreview.BackColor = Color.Black;
            picturePreview.Cursor = Cursors.SizeAll;
            picturePreview.Dock = DockStyle.Fill;
            picturePreview.Location = new Point(0, 0);
            picturePreview.Name = "picturePreview";
            picturePreview.Size = new Size(834, 419);
            picturePreview.SizeMode = PictureBoxSizeMode.Zoom;
            picturePreview.TabIndex = 1;
            picturePreview.TabStop = false;
            //
            // panelTabs
            //
            panelTabs.ColumnCount = 4;
            panelTabs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            panelTabs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            panelTabs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            panelTabs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            panelTabs.Controls.Add(buttonPictureMode, 0, 0);
            panelTabs.Controls.Add(buttonClockMode, 1, 0);
            panelTabs.Controls.Add(buttonAudioMode, 2, 0);
            panelTabs.Controls.Add(buttonTextMode, 3, 0);
            panelTabs.Dock = DockStyle.Top;
            panelTabs.Location = new Point(0, 0);
            panelTabs.Name = "panelTabs";
            panelTabs.RowCount = 1;
            panelTabs.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            panelTabs.Size = new Size(834, 78);
            panelTabs.TabIndex = 12;
            //
            // buttonPictureMode
            //
            buttonPictureMode.Activated = false;
            buttonPictureMode.BackColor = SystemColors.ControlLight;
            buttonPictureMode.BorderColor = Color.Transparent;
            buttonPictureMode.BorderRadius = 2;
            buttonPictureMode.Dock = DockStyle.Fill;
            buttonPictureMode.FlatAppearance.BorderSize = 0;
            buttonPictureMode.FlatStyle = FlatStyle.Flat;
            buttonPictureMode.Location = new Point(4, 2);
            buttonPictureMode.Margin = new Padding(4, 2, 4, 14);
            buttonPictureMode.Name = "buttonPictureMode";
            buttonPictureMode.Secondary = true;
            buttonPictureMode.Size = new Size(200, 62);
            buttonPictureMode.TabIndex = 0;
            buttonPictureMode.Text = "Picture";
            buttonPictureMode.UseVisualStyleBackColor = false;
            //
            // buttonClockMode
            //
            buttonClockMode.Activated = false;
            buttonClockMode.BackColor = SystemColors.ControlLight;
            buttonClockMode.BorderColor = Color.Transparent;
            buttonClockMode.BorderRadius = 2;
            buttonClockMode.Dock = DockStyle.Fill;
            buttonClockMode.FlatAppearance.BorderSize = 0;
            buttonClockMode.FlatStyle = FlatStyle.Flat;
            buttonClockMode.Location = new Point(212, 2);
            buttonClockMode.Margin = new Padding(4, 2, 4, 14);
            buttonClockMode.Name = "buttonClockMode";
            buttonClockMode.Secondary = true;
            buttonClockMode.Size = new Size(200, 62);
            buttonClockMode.TabIndex = 1;
            buttonClockMode.Text = "Clock";
            buttonClockMode.UseVisualStyleBackColor = false;
            //
            // buttonAudioMode
            //
            buttonAudioMode.Activated = false;
            buttonAudioMode.BackColor = SystemColors.ControlLight;
            buttonAudioMode.BorderColor = Color.Transparent;
            buttonAudioMode.BorderRadius = 2;
            buttonAudioMode.Dock = DockStyle.Fill;
            buttonAudioMode.FlatAppearance.BorderSize = 0;
            buttonAudioMode.FlatStyle = FlatStyle.Flat;
            buttonAudioMode.Location = new Point(420, 2);
            buttonAudioMode.Margin = new Padding(4, 2, 4, 14);
            buttonAudioMode.Name = "buttonAudioMode";
            buttonAudioMode.Secondary = true;
            buttonAudioMode.Size = new Size(200, 62);
            buttonAudioMode.TabIndex = 2;
            buttonAudioMode.Text = "Audio";
            buttonAudioMode.UseVisualStyleBackColor = false;
            //
            // buttonTextMode
            //
            buttonTextMode.Activated = false;
            buttonTextMode.BackColor = SystemColors.ControlLight;
            buttonTextMode.BorderColor = Color.Transparent;
            buttonTextMode.BorderRadius = 2;
            buttonTextMode.Dock = DockStyle.Fill;
            buttonTextMode.FlatAppearance.BorderSize = 0;
            buttonTextMode.FlatStyle = FlatStyle.Flat;
            buttonTextMode.Location = new Point(628, 2);
            buttonTextMode.Margin = new Padding(4, 2, 4, 14);
            buttonTextMode.Name = "buttonTextMode";
            buttonTextMode.Secondary = true;
            buttonTextMode.Size = new Size(202, 62);
            buttonTextMode.TabIndex = 3;
            buttonTextMode.Text = "Text";
            buttonTextMode.UseVisualStyleBackColor = false;
            //
            // panelPicture
            //
            panelPicture.BackColor = Color.Black;
            panelPicture.Controls.Add(picturePreview);
            panelPicture.Dock = DockStyle.Top;
            panelPicture.Location = new Point(0, 78);
            panelPicture.Name = "panelPicture";
            panelPicture.Size = new Size(834, 419);
            panelPicture.TabIndex = 4;
            //
            // panelMain
            //
            panelMain.AutoSize = true;
            panelMain.Controls.Add(panelAudioSettings);
            panelMain.Controls.Add(panelClockSettings);
            panelMain.Controls.Add(panelTextSettings);
            panelMain.Controls.Add(panelPictureSettings);
            panelMain.Controls.Add(panelPicture);
            panelMain.Controls.Add(panelTabs);
            panelMain.Controls.Add(panelButtons);
            panelMain.Dock = DockStyle.Top;
            panelMain.Location = new Point(20, 20);
            panelMain.Name = "panelMain";
            panelMain.Size = new Size(834, 1300);
            panelMain.TabIndex = 5;
            //
            // panelPictureSettings
            //
            panelPictureSettings.AutoSize = true;
            panelPictureSettings.Controls.Add(panelSliders);
            panelPictureSettings.Controls.Add(panelRotation);
            panelPictureSettings.Controls.Add(panelScaling);
            panelPictureSettings.Controls.Add(panelZoom);
            panelPictureSettings.Dock = DockStyle.Top;
            panelPictureSettings.Location = new Point(0, 497);
            panelPictureSettings.Name = "panelPictureSettings";
            panelPictureSettings.Size = new Size(834, 591);
            panelPictureSettings.TabIndex = 6;
            //
            // panelTextSettings
            //
            panelTextSettings.AutoSize = true;
            panelTextSettings.Controls.Add(panelTextRunning);
            panelTextSettings.Controls.Add(panelText2);
            panelTextSettings.Controls.Add(panelText);
            panelTextSettings.Dock = DockStyle.Top;
            panelTextSettings.Location = new Point(0, 1088);
            panelTextSettings.Name = "panelTextSettings";
            panelTextSettings.Size = new Size(834, 216);
            panelTextSettings.TabIndex = 7;
            //
            // panelClockSettings
            //
            panelClockSettings.AutoSize = true;
            panelClockSettings.Controls.Add(panelClockBattery);
            panelClockSettings.Controls.Add(panelClockDate);
            panelClockSettings.Controls.Add(panelClockTime);
            panelClockSettings.Dock = DockStyle.Top;
            panelClockSettings.Location = new Point(0, 1304);
            panelClockSettings.Name = "panelClockSettings";
            panelClockSettings.Size = new Size(834, 216);
            panelClockSettings.TabIndex = 8;
            //
            // panelButtons
            //
            panelButtons.Controls.Add(buttonReset);
            panelButtons.Controls.Add(buttonPicture);
            panelButtons.Dock = DockStyle.Bottom;
            panelButtons.Location = new Point(0, 1460);
            panelButtons.Name = "panelButtons";
            panelButtons.Size = new Size(834, 94);
            panelButtons.TabIndex = 9;
            //
            // buttonPicture
            //
            buttonPicture.Activated = false;
            buttonPicture.BackColor = SystemColors.ControlLight;
            buttonPicture.BorderColor = Color.Transparent;
            buttonPicture.BorderRadius = 5;
            buttonPicture.FlatAppearance.BorderSize = 0;
            buttonPicture.FlatStyle = FlatStyle.Flat;
            buttonPicture.Image = Properties.Resources.icons8_matrix_32;
            buttonPicture.Location = new Point(16, 19);
            buttonPicture.Name = "buttonPicture";
            buttonPicture.Secondary = true;
            buttonPicture.Size = new Size(258, 56);
            buttonPicture.TabIndex = 3;
            buttonPicture.Text = "Picture / Gif";
            buttonPicture.TextAlign = ContentAlignment.MiddleRight;
            buttonPicture.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonPicture.UseVisualStyleBackColor = false;
            //
            // buttonReset
            //
            buttonReset.Activated = false;
            buttonReset.BackColor = SystemColors.ControlLight;
            buttonReset.BorderColor = Color.Transparent;
            buttonReset.BorderRadius = 5;
            buttonReset.FlatAppearance.BorderSize = 0;
            buttonReset.FlatStyle = FlatStyle.Flat;
            buttonReset.Image = Properties.Resources.icons8_refresh_32;
            buttonReset.Location = new Point(290, 19);
            buttonReset.Name = "buttonReset";
            buttonReset.Secondary = true;
            buttonReset.Size = new Size(258, 56);
            buttonReset.TabIndex = 4;
            buttonReset.Text = "Reset";
            buttonReset.TextAlign = ContentAlignment.MiddleRight;
            buttonReset.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonReset.UseVisualStyleBackColor = false;
            //
            // panelPower
            //
            panelPower.Controls.Add(checkLidOff);
            panelPower.Controls.Add(panelPowerSpacer);
            panelPower.Controls.Add(checkAutoOff);
            panelPower.Dock = DockStyle.Bottom;
            panelPower.Location = new Point(20, 1268);
            panelPower.Name = "panelPower";
            panelPower.Padding = new Padding(16, 5, 11, 5);
            panelPower.Size = new Size(834, 58);
            panelPower.TabIndex = 16;
            //
            // checkLidOff
            //
            checkLidOff.AutoSize = true;
            checkLidOff.BackColor = SystemColors.ControlLight;
            checkLidOff.Dock = DockStyle.Left;
            checkLidOff.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            checkLidOff.Location = new Point(332, 5);
            checkLidOff.Margin = new Padding(0);
            checkLidOff.Name = "checkLidOff";
            checkLidOff.Padding = new Padding(16, 6, 16, 6);
            checkLidOff.Size = new Size(300, 48);
            checkLidOff.TabIndex = 1;
            checkLidOff.Text = "Disable on lid close";
            checkLidOff.UseVisualStyleBackColor = false;
            //
            // panelPowerSpacer
            //
            panelPowerSpacer.Dock = DockStyle.Left;
            panelPowerSpacer.Location = new Point(316, 5);
            panelPowerSpacer.Name = "panelPowerSpacer";
            panelPowerSpacer.Size = new Size(16, 48);
            panelPowerSpacer.TabIndex = 2;
            //
            // checkAutoOff
            //
            checkAutoOff.AutoSize = true;
            checkAutoOff.BackColor = SystemColors.ControlLight;
            checkAutoOff.Dock = DockStyle.Left;
            checkAutoOff.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            checkAutoOff.Location = new Point(16, 5);
            checkAutoOff.Margin = new Padding(0);
            checkAutoOff.Name = "checkAutoOff";
            checkAutoOff.Padding = new Padding(16, 6, 16, 6);
            checkAutoOff.Size = new Size(300, 48);
            checkAutoOff.TabIndex = 0;
            checkAutoOff.Text = "Disable on battery";
            checkAutoOff.UseVisualStyleBackColor = false;
            //
            // panelText
            //
            panelText.Controls.Add(numTextSize);
            panelText.Controls.Add(comboTextFont);
            panelText.Controls.Add(textMatrix);
            panelText.Dock = DockStyle.Top;
            panelText.Location = new Point(0, 0);
            panelText.Name = "panelText";
            panelText.Size = new Size(834, 78);
            panelText.TabIndex = 9;
            //
            // numTextSize
            //
            numTextSize.BorderStyle = BorderStyle.FixedSingle;
            numTextSize.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            numTextSize.Location = new Point(688, 17);
            numTextSize.Margin = new Padding(4, 11, 4, 8);
            numTextSize.Maximum = 30;
            numTextSize.Minimum = 8;
            numTextSize.Name = "numTextSize";
            numTextSize.Size = new Size(110, 39);
            numTextSize.TabIndex = 19;
            numTextSize.Value = 15;
            //
            // comboTextFont
            //
            comboTextFont.BorderColor = Color.White;
            comboTextFont.ButtonColor = Color.FromArgb(255, 255, 255);
            comboTextFont.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            comboTextFont.FormattingEnabled = true;
            comboTextFont.Location = new Point(430, 17);
            comboTextFont.Margin = new Padding(4, 11, 4, 8);
            comboTextFont.Name = "comboTextFont";
            comboTextFont.Size = new Size(242, 40);
            comboTextFont.TabIndex = 18;
            //
            // textMatrix
            //
            textMatrix.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            textMatrix.Location = new Point(16, 17);
            textMatrix.Margin = new Padding(4, 11, 4, 8);
            textMatrix.MaxLength = 100;
            textMatrix.Name = "textMatrix";
            textMatrix.Size = new Size(398, 39);
            textMatrix.TabIndex = 17;
            //
            // panelText2
            //
            panelText2.Controls.Add(numTextSize2);
            panelText2.Controls.Add(comboTextFont2);
            panelText2.Controls.Add(textMatrix2);
            panelText2.Dock = DockStyle.Top;
            panelText2.Location = new Point(0, 78);
            panelText2.Name = "panelText2";
            panelText2.Size = new Size(834, 78);
            panelText2.TabIndex = 10;
            //
            // numTextSize2
            //
            numTextSize2.BorderStyle = BorderStyle.FixedSingle;
            numTextSize2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            numTextSize2.Location = new Point(688, 17);
            numTextSize2.Margin = new Padding(4, 11, 4, 8);
            numTextSize2.Maximum = 30;
            numTextSize2.Minimum = 8;
            numTextSize2.Name = "numTextSize2";
            numTextSize2.Size = new Size(110, 39);
            numTextSize2.TabIndex = 19;
            numTextSize2.Value = 15;
            //
            // comboTextFont2
            //
            comboTextFont2.BorderColor = Color.White;
            comboTextFont2.ButtonColor = Color.FromArgb(255, 255, 255);
            comboTextFont2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            comboTextFont2.FormattingEnabled = true;
            comboTextFont2.Location = new Point(430, 17);
            comboTextFont2.Margin = new Padding(4, 11, 4, 8);
            comboTextFont2.Name = "comboTextFont2";
            comboTextFont2.Size = new Size(242, 40);
            comboTextFont2.TabIndex = 18;
            //
            // textMatrix2
            //
            textMatrix2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            textMatrix2.Location = new Point(16, 17);
            textMatrix2.Margin = new Padding(4, 11, 4, 8);
            textMatrix2.MaxLength = 100;
            textMatrix2.Name = "textMatrix2";
            textMatrix2.Size = new Size(398, 39);
            textMatrix2.TabIndex = 17;
            //
            // panelTextRunning
            //
            panelTextRunning.Controls.Add(checkTextRunning);
            panelTextRunning.Dock = DockStyle.Top;
            panelTextRunning.Location = new Point(0, 156);
            panelTextRunning.Name = "panelTextRunning";
            panelTextRunning.Size = new Size(834, 60);
            panelTextRunning.TabIndex = 11;
            //
            // checkTextRunning
            //
            checkTextRunning.AutoSize = true;
            checkTextRunning.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            checkTextRunning.Location = new Point(16, 10);
            checkTextRunning.Name = "checkTextRunning";
            checkTextRunning.Size = new Size(180, 36);
            checkTextRunning.TabIndex = 18;
            checkTextRunning.Text = "Running Text";
            checkTextRunning.UseVisualStyleBackColor = true;
            //
            // panelClockTime
            //
            panelClockTime.Controls.Add(textClockTime);
            panelClockTime.Controls.Add(labelClockTime);
            panelClockTime.Dock = DockStyle.Top;
            panelClockTime.Location = new Point(0, 0);
            panelClockTime.Name = "panelClockTime";
            panelClockTime.Size = new Size(834, 78);
            panelClockTime.TabIndex = 13;
            //
            // textClockTime
            //
            textClockTime.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            textClockTime.Location = new Point(279, 17);
            textClockTime.Margin = new Padding(4, 11, 4, 8);
            textClockTime.MaxLength = 30;
            textClockTime.Name = "textClockTime";
            textClockTime.Size = new Size(322, 39);
            textClockTime.TabIndex = 17;
            //
            // labelClockTime
            //
            labelClockTime.AutoSize = true;
            labelClockTime.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelClockTime.Location = new Point(16, 20);
            labelClockTime.Name = "labelClockTime";
            labelClockTime.Size = new Size(140, 32);
            labelClockTime.TabIndex = 4;
            labelClockTime.Text = "Time Format";
            //
            // panelClockDate
            //
            panelClockDate.Controls.Add(textClockDate);
            panelClockDate.Controls.Add(labelClockDate);
            panelClockDate.Dock = DockStyle.Top;
            panelClockDate.Location = new Point(0, 78);
            panelClockDate.Name = "panelClockDate";
            panelClockDate.Size = new Size(834, 78);
            panelClockDate.TabIndex = 14;
            //
            // textClockDate
            //
            textClockDate.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            textClockDate.Location = new Point(279, 17);
            textClockDate.Margin = new Padding(4, 11, 4, 8);
            textClockDate.MaxLength = 30;
            textClockDate.Name = "textClockDate";
            textClockDate.Size = new Size(322, 39);
            textClockDate.TabIndex = 17;
            //
            // labelClockDate
            //
            labelClockDate.AutoSize = true;
            labelClockDate.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelClockDate.Location = new Point(16, 20);
            labelClockDate.Name = "labelClockDate";
            labelClockDate.Size = new Size(140, 32);
            labelClockDate.TabIndex = 4;
            labelClockDate.Text = "Date Format";
            //
            // panelClockBattery
            //
            panelClockBattery.Controls.Add(checkClockBattery);
            panelClockBattery.Dock = DockStyle.Top;
            panelClockBattery.Location = new Point(0, 156);
            panelClockBattery.Name = "panelClockBattery";
            panelClockBattery.Size = new Size(834, 60);
            panelClockBattery.TabIndex = 15;
            //
            // checkClockBattery
            //
            checkClockBattery.AutoSize = true;
            checkClockBattery.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            checkClockBattery.Location = new Point(16, 10);
            checkClockBattery.Name = "checkClockBattery";
            checkClockBattery.Size = new Size(180, 36);
            checkClockBattery.TabIndex = 18;
            checkClockBattery.Text = "Battery Level";
            checkClockBattery.UseVisualStyleBackColor = true;
            //
            // panelAudioSettings
            //
            panelAudioSettings.AutoSize = true;
            panelAudioSettings.Controls.Add(comboAudioMode);
            panelAudioSettings.Controls.Add(labelAudioMode);
            panelAudioSettings.Dock = DockStyle.Top;
            panelAudioSettings.Location = new Point(0, 1460);
            panelAudioSettings.Name = "panelAudioSettings";
            panelAudioSettings.Size = new Size(834, 78);
            panelAudioSettings.TabIndex = 15;
            //
            // comboAudioMode
            //
            comboAudioMode.BorderColor = Color.White;
            comboAudioMode.ButtonColor = Color.FromArgb(255, 255, 255);
            comboAudioMode.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            comboAudioMode.FormattingEnabled = true;
            comboAudioMode.Items.AddRange(new object[] { "Bars", "Spectrogram" });
            comboAudioMode.Location = new Point(279, 17);
            comboAudioMode.Margin = new Padding(4, 11, 4, 8);
            comboAudioMode.Name = "comboAudioMode";
            comboAudioMode.Size = new Size(322, 40);
            comboAudioMode.TabIndex = 17;
            //
            // labelAudioMode
            //
            labelAudioMode.AutoSize = true;
            labelAudioMode.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelAudioMode.Location = new Point(16, 20);
            labelAudioMode.Name = "labelAudioMode";
            labelAudioMode.Size = new Size(190, 32);
            labelAudioMode.TabIndex = 4;
            labelAudioMode.Text = "Visualizer Mode";
            //
            // panelSliders
            //
            panelSliders.ColumnCount = 2;
            panelSliders.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            panelSliders.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            panelSliders.Controls.Add(panelContrast, 0, 0);
            panelSliders.Controls.Add(panelGamma, 1, 0);
            panelSliders.Dock = DockStyle.Top;
            panelSliders.Location = new Point(0, 301);
            panelSliders.Name = "panelSliders";
            panelSliders.RowCount = 1;
            panelSliders.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            panelSliders.Size = new Size(834, 145);
            panelSliders.TabIndex = 9;
            //
            // panelGamma
            //
            panelGamma.Controls.Add(labelGamma);
            panelGamma.Controls.Add(labelGammaTitle);
            panelGamma.Controls.Add(trackGamma);
            panelGamma.Dock = DockStyle.Fill;
            panelGamma.Location = new Point(417, 0);
            panelGamma.Margin = new Padding(0);
            panelGamma.Name = "panelGamma";
            panelGamma.Size = new Size(417, 145);
            panelGamma.TabIndex = 7;
            //
            // labelGamma
            //
            labelGamma.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelGamma.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            labelGamma.Location = new Point(276, 17);
            labelGamma.Name = "labelGamma";
            labelGamma.Size = new Size(125, 32);
            labelGamma.TabIndex = 4;
            labelGamma.Text = "Brightness";
            labelGamma.TextAlign = ContentAlignment.TopRight;
            //
            // labelGammaTitle
            //
            labelGammaTitle.AutoSize = true;
            labelGammaTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelGammaTitle.Location = new Point(16, 17);
            labelGammaTitle.Name = "labelGammaTitle";
            labelGammaTitle.Size = new Size(134, 32);
            labelGammaTitle.TabIndex = 3;
            labelGammaTitle.Text = "Brightness";
            //
            // trackGamma
            //
            trackGamma.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackGamma.LargeChange = 50;
            trackGamma.Location = new Point(16, 52);
            trackGamma.Maximum = 100;
            trackGamma.Minimum = -100;
            trackGamma.Name = "trackGamma";
            trackGamma.Size = new Size(385, 90);
            trackGamma.SmallChange = 10;
            trackGamma.TabIndex = 2;
            trackGamma.TickFrequency = 20;
            trackGamma.TickStyle = TickStyle.TopLeft;
            //
            // panelContrast
            //
            panelContrast.Controls.Add(labelContrast);
            panelContrast.Controls.Add(labelContrastTitle);
            panelContrast.Controls.Add(trackContrast);
            panelContrast.Dock = DockStyle.Fill;
            panelContrast.Location = new Point(0, 0);
            panelContrast.Margin = new Padding(0);
            panelContrast.Name = "panelContrast";
            panelContrast.Size = new Size(417, 145);
            panelContrast.TabIndex = 6;
            //
            // labelContrast
            //
            labelContrast.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelContrast.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            labelContrast.Location = new Point(298, 17);
            labelContrast.Name = "labelContrast";
            labelContrast.Size = new Size(103, 32);
            labelContrast.TabIndex = 4;
            labelContrast.Text = "Contrast";
            labelContrast.TextAlign = ContentAlignment.TopRight;
            //
            // labelContrastTitle
            //
            labelContrastTitle.AutoSize = true;
            labelContrastTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelContrastTitle.Location = new Point(16, 17);
            labelContrastTitle.Name = "labelContrastTitle";
            labelContrastTitle.Size = new Size(111, 32);
            labelContrastTitle.TabIndex = 3;
            labelContrastTitle.Text = "Contrast";
            //
            // trackContrast
            //
            trackContrast.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackContrast.LargeChange = 50;
            trackContrast.Location = new Point(16, 52);
            trackContrast.Maximum = 200;
            trackContrast.Minimum = 10;
            trackContrast.Name = "trackContrast";
            trackContrast.Size = new Size(385, 90);
            trackContrast.SmallChange = 10;
            trackContrast.TabIndex = 2;
            trackContrast.TickFrequency = 20;
            trackContrast.TickStyle = TickStyle.TopLeft;
            trackContrast.Value = 100;
            //
            // panelRotation
            //
            panelRotation.Controls.Add(comboRotation);
            panelRotation.Controls.Add(labelRotation);
            panelRotation.Dock = DockStyle.Top;
            panelRotation.Location = new Point(0, 223);
            panelRotation.Name = "panelRotation";
            panelRotation.Size = new Size(834, 78);
            panelRotation.TabIndex = 8;
            //
            // comboRotation
            //
            comboRotation.BorderColor = Color.White;
            comboRotation.ButtonColor = Color.FromArgb(255, 255, 255);
            comboRotation.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            comboRotation.FormattingEnabled = true;
            comboRotation.Items.AddRange(new object[] { "Straight", "Diagonal" });
            comboRotation.Location = new Point(279, 17);
            comboRotation.Margin = new Padding(4, 11, 4, 8);
            comboRotation.Name = "comboRotation";
            comboRotation.Size = new Size(322, 40);
            comboRotation.TabIndex = 17;
            //
            // labelRotation
            //
            labelRotation.AutoSize = true;
            labelRotation.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelRotation.Location = new Point(16, 20);
            labelRotation.Name = "labelRotation";
            labelRotation.Size = new Size(190, 32);
            labelRotation.TabIndex = 4;
            labelRotation.Text = "Image Rotation";
            //
            // panelScaling
            //
            panelScaling.Controls.Add(comboScaling);
            panelScaling.Controls.Add(labelScaling);
            panelScaling.Dock = DockStyle.Top;
            panelScaling.Location = new Point(0, 145);
            panelScaling.Name = "panelScaling";
            panelScaling.Size = new Size(834, 78);
            panelScaling.TabIndex = 7;
            //
            // comboScaling
            //
            comboScaling.BorderColor = Color.White;
            comboScaling.ButtonColor = Color.FromArgb(255, 255, 255);
            comboScaling.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            comboScaling.FormattingEnabled = true;
            comboScaling.Items.AddRange(new object[] { "Default", "Low", "High", "Bilinear", "Bicubic", "NearestNeighbor", "HighQualityBilinear", "HighQualityBicubic" });
            comboScaling.Location = new Point(279, 17);
            comboScaling.Margin = new Padding(4, 11, 4, 8);
            comboScaling.Name = "comboScaling";
            comboScaling.Size = new Size(322, 40);
            comboScaling.TabIndex = 17;
            //
            // labelScaling
            //
            labelScaling.AutoSize = true;
            labelScaling.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelScaling.Location = new Point(16, 20);
            labelScaling.Name = "labelScaling";
            labelScaling.Size = new Size(185, 32);
            labelScaling.TabIndex = 4;
            labelScaling.Text = "Scaling Quality";
            //
            // panelZoom
            //
            panelZoom.AutoSize = true;
            panelZoom.Controls.Add(labelZoom);
            panelZoom.Controls.Add(labelZoomTitle);
            panelZoom.Controls.Add(trackZoom);
            panelZoom.Dock = DockStyle.Top;
            panelZoom.Location = new Point(0, 0);
            panelZoom.Name = "panelZoom";
            panelZoom.Size = new Size(834, 145);
            panelZoom.TabIndex = 5;
            //
            // labelZoom
            //
            labelZoom.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelZoom.AutoSize = true;
            labelZoom.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            labelZoom.Location = new Point(731, 17);
            labelZoom.Name = "labelZoom";
            labelZoom.Size = new Size(77, 32);
            labelZoom.TabIndex = 4;
            labelZoom.Text = "Zoom";
            //
            // labelZoomTitle
            //
            labelZoomTitle.AutoSize = true;
            labelZoomTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelZoomTitle.Location = new Point(16, 17);
            labelZoomTitle.Name = "labelZoomTitle";
            labelZoomTitle.Size = new Size(81, 32);
            labelZoomTitle.TabIndex = 3;
            labelZoomTitle.Text = "Zoom";
            //
            // trackZoom
            //
            trackZoom.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackZoom.LargeChange = 50;
            trackZoom.Location = new Point(16, 52);
            trackZoom.Maximum = 200;
            trackZoom.Minimum = 10;
            trackZoom.Name = "trackZoom";
            trackZoom.Size = new Size(782, 90);
            trackZoom.SmallChange = 10;
            trackZoom.TabIndex = 2;
            trackZoom.TickFrequency = 20;
            trackZoom.TickStyle = TickStyle.TopLeft;
            trackZoom.Value = 100;
            //
            // Matrix
            //
            AutoScaleDimensions = new SizeF(192F, 192F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new Size(874, 1340);
            Controls.Add(panelMain);
            Controls.Add(panelPower);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(900, 0);
            Name = "Matrix";
            Padding = new Padding(20);
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Matrix";
            ((System.ComponentModel.ISupportInitialize)picturePreview).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackZoom).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackGamma).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackContrast).EndInit();
            ((System.ComponentModel.ISupportInitialize)numTextSize).EndInit();
            ((System.ComponentModel.ISupportInitialize)numTextSize2).EndInit();
            panelPicture.ResumeLayout(false);
            panelTabs.ResumeLayout(false);
            panelMain.ResumeLayout(false);
            panelMain.PerformLayout();
            panelPictureSettings.ResumeLayout(false);
            panelPictureSettings.PerformLayout();
            panelTextSettings.ResumeLayout(false);
            panelTextSettings.PerformLayout();
            panelClockSettings.ResumeLayout(false);
            panelClockSettings.PerformLayout();
            panelButtons.ResumeLayout(false);
            panelPower.ResumeLayout(false);
            panelPower.PerformLayout();
            panelSliders.ResumeLayout(false);
            panelGamma.ResumeLayout(false);
            panelGamma.PerformLayout();
            panelContrast.ResumeLayout(false);
            panelContrast.PerformLayout();
            panelRotation.ResumeLayout(false);
            panelRotation.PerformLayout();
            panelScaling.ResumeLayout(false);
            panelScaling.PerformLayout();
            panelZoom.ResumeLayout(false);
            panelZoom.PerformLayout();
            panelText.ResumeLayout(false);
            panelText.PerformLayout();
            panelText2.ResumeLayout(false);
            panelText2.PerformLayout();
            panelTextRunning.ResumeLayout(false);
            panelTextRunning.PerformLayout();
            panelClockTime.ResumeLayout(false);
            panelClockTime.PerformLayout();
            panelClockDate.ResumeLayout(false);
            panelClockDate.PerformLayout();
            panelClockBattery.ResumeLayout(false);
            panelClockBattery.PerformLayout();
            panelAudioSettings.ResumeLayout(false);
            panelAudioSettings.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox picturePreview;
        private TableLayoutPanel panelTabs;
        private UI.RButton buttonPictureMode;
        private UI.RButton buttonClockMode;
        private UI.RButton buttonAudioMode;
        private UI.RButton buttonTextMode;
        private Panel panelPicture;
        private Panel panelMain;
        private Panel panelPictureSettings;
        private Panel panelTextSettings;
        private Panel panelClockSettings;
        private Panel panelZoom;
        private Label labelZoom;
        private Label labelZoomTitle;
        private RTrackBar trackZoom;
        private Panel panelButtons;
        private UI.RButton buttonPicture;
        private UI.RButton buttonReset;
        private Panel panelPower;
        private RCheckBox checkAutoOff;
        private RCheckBox checkLidOff;
        private Panel panelPowerSpacer;
        private TableLayoutPanel panelSliders;
        private Panel panelScaling;
        private Label labelScaling;
        private UI.RComboBox comboScaling;
        private Panel panelRotation;
        private UI.RComboBox comboRotation;
        private Label labelRotation;
        private Panel panelContrast;
        private Label labelContrast;
        private Label labelContrastTitle;
        private RTrackBar trackContrast;
        private Panel panelGamma;
        private Label labelGamma;
        private Label labelGammaTitle;
        private RTrackBar trackGamma;
        private Panel panelText;
        private UI.RTextBox textMatrix;
        private UI.RComboBox comboTextFont;
        private RNumericUpDown numTextSize;
        private Panel panelText2;
        private UI.RTextBox textMatrix2;
        private UI.RComboBox comboTextFont2;
        private RNumericUpDown numTextSize2;
        private Panel panelTextRunning;
        private CheckBox checkTextRunning;
        private Panel panelClockTime;
        private UI.RTextBox textClockTime;
        private Label labelClockTime;
        private Panel panelClockDate;
        private UI.RTextBox textClockDate;
        private Label labelClockDate;
        private Panel panelClockBattery;
        private CheckBox checkClockBattery;
        private Panel panelAudioSettings;
        private UI.RComboBox comboAudioMode;
        private Label labelAudioMode;
    }
}
