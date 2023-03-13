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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            panelMatrix = new Panel();
            checkMatrix = new CheckBox();
            tableLayoutMatrix = new TableLayoutPanel();
            buttonMatrix = new Button();
            comboMatrixRunning = new ComboBox();
            comboMatrix = new ComboBox();
            pictureMatrix = new PictureBox();
            labelMatrix = new Label();
            panelBattery = new Panel();
            labelVersion = new Label();
            labelBattery = new Label();
            pictureBattery = new PictureBox();
            labelBatteryTitle = new Label();
            trackBattery = new TrackBar();
            panelFooter = new Panel();
            buttonQuit = new Button();
            checkStartup = new CheckBox();
            panelPerformance = new Panel();
            buttonFans = new Button();
            picturePerf = new PictureBox();
            labelPerf = new Label();
            labelCPUFan = new Label();
            tablePerf = new TableLayoutPanel();
            buttonTurbo = new Button();
            buttonBalanced = new Button();
            buttonSilent = new Button();
            panelGPU = new Panel();
            checkGPU = new CheckBox();
            pictureGPU = new PictureBox();
            labelGPU = new Label();
            labelGPUFan = new Label();
            tableGPU = new TableLayoutPanel();
            buttonUltimate = new Button();
            buttonStandard = new Button();
            buttonEco = new Button();
            panelScreen = new Panel();
            checkScreen = new CheckBox();
            tableScreen = new TableLayoutPanel();
            button120Hz = new Button();
            button60Hz = new Button();
            pictureScreen = new PictureBox();
            labelSreen = new Label();
            panelKeyboard = new Panel();
            tableLayoutKeyboard = new TableLayoutPanel();
            buttonKeyboard = new Button();
            comboKeyboard = new ComboBox();
            panelColor = new Panel();
            pictureColor2 = new PictureBox();
            pictureColor = new PictureBox();
            buttonKeyboardColor = new Button();
            pictureKeyboard = new PictureBox();
            label1 = new Label();
            panelMatrix.SuspendLayout();
            tableLayoutMatrix.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureMatrix).BeginInit();
            panelBattery.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBattery).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBattery).BeginInit();
            panelFooter.SuspendLayout();
            panelPerformance.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picturePerf).BeginInit();
            tablePerf.SuspendLayout();
            panelGPU.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureGPU).BeginInit();
            tableGPU.SuspendLayout();
            panelScreen.SuspendLayout();
            tableScreen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureScreen).BeginInit();
            panelKeyboard.SuspendLayout();
            tableLayoutKeyboard.SuspendLayout();
            panelColor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureColor2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureColor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureKeyboard).BeginInit();
            SuspendLayout();
            // 
            // panelMatrix
            // 
            panelMatrix.Controls.Add(checkMatrix);
            panelMatrix.Controls.Add(tableLayoutMatrix);
            panelMatrix.Controls.Add(pictureMatrix);
            panelMatrix.Controls.Add(labelMatrix);
            panelMatrix.Dock = DockStyle.Top;
            panelMatrix.Location = new Point(16, 806);
            panelMatrix.Margin = new Padding(4);
            panelMatrix.Name = "panelMatrix";
            panelMatrix.Size = new Size(722, 180);
            panelMatrix.TabIndex = 33;
            // 
            // checkMatrix
            // 
            checkMatrix.AutoSize = true;
            checkMatrix.ForeColor = SystemColors.GrayText;
            checkMatrix.Location = new Point(28, 109);
            checkMatrix.Margin = new Padding(4, 2, 4, 2);
            checkMatrix.Name = "checkMatrix";
            checkMatrix.Size = new Size(249, 36);
            checkMatrix.TabIndex = 44;
            checkMatrix.Text = "Turn off on battery";
            checkMatrix.UseVisualStyleBackColor = true;
            // 
            // tableLayoutMatrix
            // 
            tableLayoutMatrix.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutMatrix.ColumnCount = 3;
            tableLayoutMatrix.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutMatrix.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutMatrix.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutMatrix.Controls.Add(buttonMatrix, 0, 0);
            tableLayoutMatrix.Controls.Add(comboMatrixRunning, 0, 0);
            tableLayoutMatrix.Controls.Add(comboMatrix, 0, 0);
            tableLayoutMatrix.Location = new Point(15, 52);
            tableLayoutMatrix.Margin = new Padding(4);
            tableLayoutMatrix.Name = "tableLayoutMatrix";
            tableLayoutMatrix.RowCount = 1;
            tableLayoutMatrix.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutMatrix.Size = new Size(684, 62);
            tableLayoutMatrix.TabIndex = 43;
            // 
            // buttonMatrix
            // 
            buttonMatrix.BackColor = SystemColors.ButtonFace;
            buttonMatrix.Dock = DockStyle.Top;
            buttonMatrix.FlatAppearance.BorderSize = 0;
            buttonMatrix.Location = new Point(466, 10);
            buttonMatrix.Margin = new Padding(10);
            buttonMatrix.Name = "buttonMatrix";
            buttonMatrix.Size = new Size(208, 42);
            buttonMatrix.TabIndex = 43;
            buttonMatrix.Text = "Picture / Gif";
            buttonMatrix.UseVisualStyleBackColor = false;
            // 
            // comboMatrixRunning
            // 
            comboMatrixRunning.Dock = DockStyle.Fill;
            comboMatrixRunning.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            comboMatrixRunning.FormattingEnabled = true;
            comboMatrixRunning.ItemHeight = 32;
            comboMatrixRunning.Items.AddRange(new object[] { "Binary Banner", "Rog Logo", "Picture" });
            comboMatrixRunning.Location = new Point(238, 10);
            comboMatrixRunning.Margin = new Padding(10);
            comboMatrixRunning.Name = "comboMatrixRunning";
            comboMatrixRunning.Size = new Size(208, 40);
            comboMatrixRunning.TabIndex = 42;
            comboMatrixRunning.TabStop = false;
            // 
            // comboMatrix
            // 
            comboMatrix.Dock = DockStyle.Fill;
            comboMatrix.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            comboMatrix.FormattingEnabled = true;
            comboMatrix.ItemHeight = 32;
            comboMatrix.Items.AddRange(new object[] { "Off", "Dim", "Medium", "Bright" });
            comboMatrix.Location = new Point(10, 10);
            comboMatrix.Margin = new Padding(10);
            comboMatrix.Name = "comboMatrix";
            comboMatrix.Size = new Size(208, 40);
            comboMatrix.TabIndex = 41;
            comboMatrix.TabStop = false;
            // 
            // pictureMatrix
            // 
            pictureMatrix.BackgroundImage = Properties.Resources.icons8_matrix_desktop_48;
            pictureMatrix.BackgroundImageLayout = ImageLayout.Zoom;
            pictureMatrix.Location = new Point(29, 10);
            pictureMatrix.Margin = new Padding(4, 2, 4, 2);
            pictureMatrix.Name = "pictureMatrix";
            pictureMatrix.Size = new Size(36, 36);
            pictureMatrix.TabIndex = 39;
            pictureMatrix.TabStop = false;
            // 
            // labelMatrix
            // 
            labelMatrix.AutoSize = true;
            labelMatrix.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelMatrix.Location = new Point(68, 12);
            labelMatrix.Margin = new Padding(4, 0, 4, 0);
            labelMatrix.Name = "labelMatrix";
            labelMatrix.Size = new Size(170, 32);
            labelMatrix.TabIndex = 38;
            labelMatrix.Text = "Anime Matrix";
            // 
            // panelBattery
            // 
            panelBattery.Controls.Add(labelVersion);
            panelBattery.Controls.Add(labelBattery);
            panelBattery.Controls.Add(pictureBattery);
            panelBattery.Controls.Add(labelBatteryTitle);
            panelBattery.Controls.Add(trackBattery);
            panelBattery.Dock = DockStyle.Top;
            panelBattery.Location = new Point(16, 986);
            panelBattery.Margin = new Padding(4);
            panelBattery.Name = "panelBattery";
            panelBattery.Size = new Size(722, 148);
            panelBattery.TabIndex = 34;
            // 
            // labelVersion
            // 
            labelVersion.AutoSize = true;
            labelVersion.Font = new Font("Segoe UI", 9F, FontStyle.Underline, GraphicsUnit.Point);
            labelVersion.ForeColor = SystemColors.ControlDark;
            labelVersion.Location = new Point(26, 102);
            labelVersion.Margin = new Padding(4, 0, 4, 0);
            labelVersion.Name = "labelVersion";
            labelVersion.Size = new Size(44, 32);
            labelVersion.TabIndex = 37;
            labelVersion.Text = "v.0";
            // 
            // labelBattery
            // 
            labelBattery.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelBattery.Location = new Point(432, 12);
            labelBattery.Margin = new Padding(4, 0, 4, 0);
            labelBattery.Name = "labelBattery";
            labelBattery.Size = new Size(258, 32);
            labelBattery.TabIndex = 36;
            labelBattery.Text = "                ";
            labelBattery.TextAlign = ContentAlignment.TopRight;
            // 
            // pictureBattery
            // 
            pictureBattery.BackgroundImage = (Image)resources.GetObject("pictureBattery.BackgroundImage");
            pictureBattery.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBattery.Location = new Point(29, 10);
            pictureBattery.Margin = new Padding(4, 2, 4, 2);
            pictureBattery.Name = "pictureBattery";
            pictureBattery.Size = new Size(36, 38);
            pictureBattery.TabIndex = 35;
            pictureBattery.TabStop = false;
            // 
            // labelBatteryTitle
            // 
            labelBatteryTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelBatteryTitle.Location = new Point(70, 12);
            labelBatteryTitle.Margin = new Padding(4, 0, 4, 0);
            labelBatteryTitle.Name = "labelBatteryTitle";
            labelBatteryTitle.Size = new Size(408, 36);
            labelBatteryTitle.TabIndex = 34;
            labelBatteryTitle.Text = "Battery Charge Limit";
            // 
            // trackBattery
            // 
            trackBattery.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBattery.LargeChange = 20;
            trackBattery.Location = new Point(15, 48);
            trackBattery.Margin = new Padding(4, 2, 4, 2);
            trackBattery.Maximum = 100;
            trackBattery.Minimum = 50;
            trackBattery.Name = "trackBattery";
            trackBattery.Size = new Size(684, 90);
            trackBattery.SmallChange = 10;
            trackBattery.TabIndex = 33;
            trackBattery.TickFrequency = 10;
            trackBattery.TickStyle = TickStyle.TopLeft;
            trackBattery.Value = 100;
            // 
            // panelFooter
            // 
            panelFooter.Controls.Add(buttonQuit);
            panelFooter.Controls.Add(checkStartup);
            panelFooter.Dock = DockStyle.Top;
            panelFooter.Location = new Point(16, 1134);
            panelFooter.Margin = new Padding(4);
            panelFooter.Name = "panelFooter";
            panelFooter.Size = new Size(722, 64);
            panelFooter.TabIndex = 35;
            // 
            // buttonQuit
            // 
            buttonQuit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonQuit.BackColor = SystemColors.ButtonFace;
            buttonQuit.Location = new Point(588, 9);
            buttonQuit.Margin = new Padding(4, 2, 4, 2);
            buttonQuit.Name = "buttonQuit";
            buttonQuit.Size = new Size(116, 40);
            buttonQuit.TabIndex = 18;
            buttonQuit.Text = "Quit";
            buttonQuit.UseVisualStyleBackColor = false;
            // 
            // checkStartup
            // 
            checkStartup.AutoSize = true;
            checkStartup.Location = new Point(27, 15);
            checkStartup.Margin = new Padding(4, 2, 4, 2);
            checkStartup.Name = "checkStartup";
            checkStartup.Size = new Size(206, 36);
            checkStartup.TabIndex = 17;
            checkStartup.Text = "Run on Startup";
            checkStartup.UseVisualStyleBackColor = true;
            // 
            // panelPerformance
            // 
            panelPerformance.Controls.Add(buttonFans);
            panelPerformance.Controls.Add(picturePerf);
            panelPerformance.Controls.Add(labelPerf);
            panelPerformance.Controls.Add(labelCPUFan);
            panelPerformance.Controls.Add(tablePerf);
            panelPerformance.Dock = DockStyle.Top;
            panelPerformance.Location = new Point(16, 16);
            panelPerformance.Margin = new Padding(0);
            panelPerformance.Name = "panelPerformance";
            panelPerformance.Size = new Size(722, 220);
            panelPerformance.TabIndex = 36;
            // 
            // buttonFans
            // 
            buttonFans.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonFans.BackColor = SystemColors.ButtonFace;
            buttonFans.FlatAppearance.BorderSize = 0;
            buttonFans.Location = new Point(479, 161);
            buttonFans.Margin = new Padding(4, 2, 4, 2);
            buttonFans.Name = "buttonFans";
            buttonFans.Size = new Size(213, 48);
            buttonFans.TabIndex = 34;
            buttonFans.Text = "Fans and Power";
            buttonFans.UseVisualStyleBackColor = false;
            // 
            // picturePerf
            // 
            picturePerf.BackgroundImage = (Image)resources.GetObject("picturePerf.BackgroundImage");
            picturePerf.BackgroundImageLayout = ImageLayout.Zoom;
            picturePerf.InitialImage = null;
            picturePerf.Location = new Point(29, 10);
            picturePerf.Margin = new Padding(4, 2, 4, 2);
            picturePerf.Name = "picturePerf";
            picturePerf.Size = new Size(36, 38);
            picturePerf.TabIndex = 32;
            picturePerf.TabStop = false;
            // 
            // labelPerf
            // 
            labelPerf.AutoSize = true;
            labelPerf.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelPerf.Location = new Point(68, 12);
            labelPerf.Margin = new Padding(4, 0, 4, 0);
            labelPerf.Name = "labelPerf";
            labelPerf.Size = new Size(234, 32);
            labelPerf.TabIndex = 31;
            labelPerf.Text = "Performance Mode";
            // 
            // labelCPUFan
            // 
            labelCPUFan.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelCPUFan.Location = new Point(326, 10);
            labelCPUFan.Margin = new Padding(4, 0, 4, 0);
            labelCPUFan.Name = "labelCPUFan";
            labelCPUFan.Size = new Size(366, 32);
            labelCPUFan.TabIndex = 30;
            labelCPUFan.Text = "      ";
            labelCPUFan.TextAlign = ContentAlignment.TopRight;
            // 
            // tablePerf
            // 
            tablePerf.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tablePerf.AutoScroll = true;
            tablePerf.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tablePerf.ColumnCount = 3;
            tablePerf.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            tablePerf.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            tablePerf.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            tablePerf.Controls.Add(buttonTurbo, 2, 0);
            tablePerf.Controls.Add(buttonBalanced, 1, 0);
            tablePerf.Controls.Add(buttonSilent, 0, 0);
            tablePerf.Location = new Point(15, 48);
            tablePerf.Margin = new Padding(4, 2, 4, 2);
            tablePerf.Name = "tablePerf";
            tablePerf.RowCount = 1;
            tablePerf.RowStyles.Add(new RowStyle(SizeType.Absolute, 108F));
            tablePerf.Size = new Size(684, 108);
            tablePerf.TabIndex = 29;
            // 
            // buttonTurbo
            // 
            buttonTurbo.BackColor = SystemColors.ControlLightLight;
            buttonTurbo.Dock = DockStyle.Fill;
            buttonTurbo.FlatAppearance.BorderColor = Color.FromArgb(192, 0, 0);
            buttonTurbo.FlatAppearance.BorderSize = 0;
            buttonTurbo.FlatStyle = FlatStyle.Flat;
            buttonTurbo.Location = new Point(464, 12);
            buttonTurbo.Margin = new Padding(8, 12, 8, 12);
            buttonTurbo.Name = "buttonTurbo";
            buttonTurbo.Size = new Size(212, 84);
            buttonTurbo.TabIndex = 2;
            buttonTurbo.Text = "Turbo";
            buttonTurbo.UseVisualStyleBackColor = false;
            // 
            // buttonBalanced
            // 
            buttonBalanced.BackColor = SystemColors.ControlLightLight;
            buttonBalanced.Dock = DockStyle.Fill;
            buttonBalanced.FlatAppearance.BorderColor = Color.FromArgb(0, 0, 192);
            buttonBalanced.FlatAppearance.BorderSize = 0;
            buttonBalanced.FlatStyle = FlatStyle.Flat;
            buttonBalanced.Location = new Point(236, 12);
            buttonBalanced.Margin = new Padding(8, 12, 8, 12);
            buttonBalanced.Name = "buttonBalanced";
            buttonBalanced.Size = new Size(212, 84);
            buttonBalanced.TabIndex = 1;
            buttonBalanced.Text = "Balanced";
            buttonBalanced.UseVisualStyleBackColor = false;
            // 
            // buttonSilent
            // 
            buttonSilent.BackColor = SystemColors.ControlLightLight;
            buttonSilent.CausesValidation = false;
            buttonSilent.Dock = DockStyle.Fill;
            buttonSilent.FlatAppearance.BorderColor = Color.FromArgb(0, 192, 192);
            buttonSilent.FlatAppearance.BorderSize = 0;
            buttonSilent.FlatStyle = FlatStyle.Flat;
            buttonSilent.Location = new Point(8, 12);
            buttonSilent.Margin = new Padding(8, 12, 8, 12);
            buttonSilent.Name = "buttonSilent";
            buttonSilent.Size = new Size(212, 84);
            buttonSilent.TabIndex = 0;
            buttonSilent.Text = "Silent";
            buttonSilent.UseVisualStyleBackColor = false;
            // 
            // panelGPU
            // 
            panelGPU.Controls.Add(checkGPU);
            panelGPU.Controls.Add(pictureGPU);
            panelGPU.Controls.Add(labelGPU);
            panelGPU.Controls.Add(labelGPUFan);
            panelGPU.Controls.Add(tableGPU);
            panelGPU.Dock = DockStyle.Top;
            panelGPU.Location = new Point(16, 236);
            panelGPU.Margin = new Padding(4);
            panelGPU.Name = "panelGPU";
            panelGPU.Size = new Size(722, 216);
            panelGPU.TabIndex = 37;
            // 
            // checkGPU
            // 
            checkGPU.AutoSize = true;
            checkGPU.ForeColor = SystemColors.GrayText;
            checkGPU.Location = new Point(27, 155);
            checkGPU.Margin = new Padding(4, 2, 4, 2);
            checkGPU.Name = "checkGPU";
            checkGPU.Size = new Size(550, 36);
            checkGPU.TabIndex = 20;
            checkGPU.Text = "Set Eco on battery and Standard when plugged";
            checkGPU.UseVisualStyleBackColor = true;
            // 
            // pictureGPU
            // 
            pictureGPU.BackgroundImage = (Image)resources.GetObject("pictureGPU.BackgroundImage");
            pictureGPU.BackgroundImageLayout = ImageLayout.Zoom;
            pictureGPU.Location = new Point(29, 10);
            pictureGPU.Margin = new Padding(4, 2, 4, 2);
            pictureGPU.Name = "pictureGPU";
            pictureGPU.Size = new Size(36, 38);
            pictureGPU.TabIndex = 19;
            pictureGPU.TabStop = false;
            // 
            // labelGPU
            // 
            labelGPU.AutoSize = true;
            labelGPU.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelGPU.Location = new Point(70, 12);
            labelGPU.Margin = new Padding(4, 0, 4, 0);
            labelGPU.Name = "labelGPU";
            labelGPU.Size = new Size(136, 32);
            labelGPU.TabIndex = 18;
            labelGPU.Text = "GPU Mode";
            // 
            // labelGPUFan
            // 
            labelGPUFan.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelGPUFan.Location = new Point(346, 10);
            labelGPUFan.Margin = new Padding(4, 0, 4, 0);
            labelGPUFan.Name = "labelGPUFan";
            labelGPUFan.Size = new Size(348, 32);
            labelGPUFan.TabIndex = 17;
            labelGPUFan.Text = "         ";
            labelGPUFan.TextAlign = ContentAlignment.TopRight;
            // 
            // tableGPU
            // 
            tableGPU.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableGPU.AutoSize = true;
            tableGPU.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableGPU.ColumnCount = 3;
            tableGPU.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            tableGPU.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            tableGPU.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            tableGPU.Controls.Add(buttonUltimate, 2, 0);
            tableGPU.Controls.Add(buttonStandard, 1, 0);
            tableGPU.Controls.Add(buttonEco, 0, 0);
            tableGPU.Location = new Point(15, 48);
            tableGPU.Margin = new Padding(4, 2, 4, 2);
            tableGPU.Name = "tableGPU";
            tableGPU.RowCount = 1;
            tableGPU.RowStyles.Add(new RowStyle(SizeType.Absolute, 108F));
            tableGPU.Size = new Size(684, 108);
            tableGPU.TabIndex = 16;
            // 
            // buttonUltimate
            // 
            buttonUltimate.BackColor = SystemColors.ControlLightLight;
            buttonUltimate.Dock = DockStyle.Fill;
            buttonUltimate.FlatAppearance.BorderSize = 0;
            buttonUltimate.FlatStyle = FlatStyle.Flat;
            buttonUltimate.Location = new Point(464, 12);
            buttonUltimate.Margin = new Padding(8, 12, 8, 12);
            buttonUltimate.Name = "buttonUltimate";
            buttonUltimate.Size = new Size(212, 84);
            buttonUltimate.TabIndex = 2;
            buttonUltimate.Text = "Ultimate";
            buttonUltimate.UseVisualStyleBackColor = false;
            // 
            // buttonStandard
            // 
            buttonStandard.BackColor = SystemColors.ControlLightLight;
            buttonStandard.Dock = DockStyle.Fill;
            buttonStandard.FlatAppearance.BorderSize = 0;
            buttonStandard.FlatStyle = FlatStyle.Flat;
            buttonStandard.Location = new Point(236, 12);
            buttonStandard.Margin = new Padding(8, 12, 8, 12);
            buttonStandard.Name = "buttonStandard";
            buttonStandard.Size = new Size(212, 84);
            buttonStandard.TabIndex = 1;
            buttonStandard.Text = "Standard";
            buttonStandard.UseVisualStyleBackColor = false;
            // 
            // buttonEco
            // 
            buttonEco.BackColor = SystemColors.ControlLightLight;
            buttonEco.CausesValidation = false;
            buttonEco.Dock = DockStyle.Fill;
            buttonEco.FlatAppearance.BorderSize = 0;
            buttonEco.FlatStyle = FlatStyle.Flat;
            buttonEco.Location = new Point(8, 12);
            buttonEco.Margin = new Padding(8, 12, 8, 12);
            buttonEco.Name = "buttonEco";
            buttonEco.Size = new Size(212, 84);
            buttonEco.TabIndex = 0;
            buttonEco.Text = "Eco";
            buttonEco.UseVisualStyleBackColor = false;
            // 
            // panelScreen
            // 
            panelScreen.Controls.Add(checkScreen);
            panelScreen.Controls.Add(tableScreen);
            panelScreen.Controls.Add(pictureScreen);
            panelScreen.Controls.Add(labelSreen);
            panelScreen.Dock = DockStyle.Top;
            panelScreen.Location = new Point(16, 452);
            panelScreen.Margin = new Padding(4);
            panelScreen.Name = "panelScreen";
            panelScreen.Size = new Size(722, 200);
            panelScreen.TabIndex = 38;
            // 
            // checkScreen
            // 
            checkScreen.AutoSize = true;
            checkScreen.ForeColor = SystemColors.GrayText;
            checkScreen.Location = new Point(27, 154);
            checkScreen.Margin = new Padding(4, 2, 4, 2);
            checkScreen.Name = "checkScreen";
            checkScreen.Size = new Size(527, 36);
            checkScreen.TabIndex = 24;
            checkScreen.Text = "Set 60Hz on battery, and back when plugged";
            checkScreen.UseVisualStyleBackColor = true;
            // 
            // tableScreen
            // 
            tableScreen.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableScreen.AutoSize = true;
            tableScreen.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableScreen.ColumnCount = 3;
            tableScreen.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            tableScreen.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            tableScreen.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            tableScreen.Controls.Add(button120Hz, 1, 0);
            tableScreen.Controls.Add(button60Hz, 0, 0);
            tableScreen.Location = new Point(15, 48);
            tableScreen.Margin = new Padding(4, 2, 4, 2);
            tableScreen.Name = "tableScreen";
            tableScreen.RowCount = 1;
            tableScreen.RowStyles.Add(new RowStyle(SizeType.Absolute, 108F));
            tableScreen.Size = new Size(684, 108);
            tableScreen.TabIndex = 23;
            // 
            // button120Hz
            // 
            button120Hz.BackColor = SystemColors.ControlLightLight;
            button120Hz.Dock = DockStyle.Fill;
            button120Hz.FlatAppearance.BorderColor = SystemColors.ActiveBorder;
            button120Hz.FlatAppearance.BorderSize = 0;
            button120Hz.FlatStyle = FlatStyle.Flat;
            button120Hz.Location = new Point(236, 12);
            button120Hz.Margin = new Padding(8, 12, 8, 12);
            button120Hz.Name = "button120Hz";
            button120Hz.Size = new Size(212, 84);
            button120Hz.TabIndex = 1;
            button120Hz.Text = "120Hz + OD";
            button120Hz.UseVisualStyleBackColor = false;
            // 
            // button60Hz
            // 
            button60Hz.BackColor = SystemColors.ControlLightLight;
            button60Hz.CausesValidation = false;
            button60Hz.Dock = DockStyle.Fill;
            button60Hz.FlatAppearance.BorderColor = SystemColors.ActiveBorder;
            button60Hz.FlatAppearance.BorderSize = 0;
            button60Hz.FlatStyle = FlatStyle.Flat;
            button60Hz.ForeColor = SystemColors.ControlText;
            button60Hz.Location = new Point(8, 12);
            button60Hz.Margin = new Padding(8, 12, 8, 12);
            button60Hz.Name = "button60Hz";
            button60Hz.Size = new Size(212, 84);
            button60Hz.TabIndex = 0;
            button60Hz.Text = "60Hz";
            button60Hz.UseVisualStyleBackColor = false;
            // 
            // pictureScreen
            // 
            pictureScreen.BackgroundImage = (Image)resources.GetObject("pictureScreen.BackgroundImage");
            pictureScreen.BackgroundImageLayout = ImageLayout.Zoom;
            pictureScreen.Location = new Point(29, 8);
            pictureScreen.Margin = new Padding(4, 2, 4, 2);
            pictureScreen.Name = "pictureScreen";
            pictureScreen.Size = new Size(36, 38);
            pictureScreen.TabIndex = 22;
            pictureScreen.TabStop = false;
            // 
            // labelSreen
            // 
            labelSreen.AutoSize = true;
            labelSreen.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelSreen.Location = new Point(72, 10);
            labelSreen.Margin = new Padding(4, 0, 4, 0);
            labelSreen.Name = "labelSreen";
            labelSreen.Size = new Size(176, 32);
            labelSreen.TabIndex = 21;
            labelSreen.Text = "Laptop Screen";
            // 
            // panelKeyboard
            // 
            panelKeyboard.Controls.Add(tableLayoutKeyboard);
            panelKeyboard.Controls.Add(pictureKeyboard);
            panelKeyboard.Controls.Add(label1);
            panelKeyboard.Dock = DockStyle.Top;
            panelKeyboard.Location = new Point(16, 652);
            panelKeyboard.Margin = new Padding(4);
            panelKeyboard.Name = "panelKeyboard";
            panelKeyboard.Size = new Size(722, 154);
            panelKeyboard.TabIndex = 39;
            // 
            // tableLayoutKeyboard
            // 
            tableLayoutKeyboard.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutKeyboard.ColumnCount = 3;
            tableLayoutKeyboard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            tableLayoutKeyboard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutKeyboard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutKeyboard.Controls.Add(buttonKeyboard, 2, 0);
            tableLayoutKeyboard.Controls.Add(comboKeyboard, 0, 0);
            tableLayoutKeyboard.Controls.Add(panelColor, 1, 0);
            tableLayoutKeyboard.Location = new Point(15, 56);
            tableLayoutKeyboard.Margin = new Padding(4);
            tableLayoutKeyboard.Name = "tableLayoutKeyboard";
            tableLayoutKeyboard.RowCount = 1;
            tableLayoutKeyboard.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutKeyboard.Size = new Size(684, 66);
            tableLayoutKeyboard.TabIndex = 39;
            // 
            // buttonKeyboard
            // 
            buttonKeyboard.BackColor = SystemColors.ButtonFace;
            buttonKeyboard.Dock = DockStyle.Top;
            buttonKeyboard.FlatAppearance.BorderSize = 0;
            buttonKeyboard.Location = new Point(466, 10);
            buttonKeyboard.Margin = new Padding(10);
            buttonKeyboard.Name = "buttonKeyboard";
            buttonKeyboard.Size = new Size(208, 42);
            buttonKeyboard.TabIndex = 37;
            buttonKeyboard.Text = "Extra";
            buttonKeyboard.UseVisualStyleBackColor = false;
            // 
            // comboKeyboard
            // 
            comboKeyboard.Dock = DockStyle.Fill;
            comboKeyboard.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            comboKeyboard.FormattingEnabled = true;
            comboKeyboard.ItemHeight = 32;
            comboKeyboard.Items.AddRange(new object[] { "Static", "Breathe", "Strobe", "Rainbow", "Dingding" });
            comboKeyboard.Location = new Point(10, 10);
            comboKeyboard.Margin = new Padding(10);
            comboKeyboard.Name = "comboKeyboard";
            comboKeyboard.Size = new Size(208, 40);
            comboKeyboard.TabIndex = 35;
            comboKeyboard.TabStop = false;
            // 
            // panelColor
            // 
            panelColor.Controls.Add(pictureColor2);
            panelColor.Controls.Add(pictureColor);
            panelColor.Controls.Add(buttonKeyboardColor);
            panelColor.Dock = DockStyle.Fill;
            panelColor.Location = new Point(238, 10);
            panelColor.Margin = new Padding(10);
            panelColor.Name = "panelColor";
            panelColor.Size = new Size(208, 46);
            panelColor.TabIndex = 36;
            // 
            // pictureColor2
            // 
            pictureColor2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pictureColor2.Location = new Point(148, 12);
            pictureColor2.Margin = new Padding(4);
            pictureColor2.Name = "pictureColor2";
            pictureColor2.Size = new Size(20, 20);
            pictureColor2.TabIndex = 41;
            pictureColor2.TabStop = false;
            // 
            // pictureColor
            // 
            pictureColor.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pictureColor.Location = new Point(176, 12);
            pictureColor.Margin = new Padding(4);
            pictureColor.Name = "pictureColor";
            pictureColor.Size = new Size(20, 20);
            pictureColor.TabIndex = 40;
            pictureColor.TabStop = false;
            // 
            // buttonKeyboardColor
            // 
            buttonKeyboardColor.BackColor = SystemColors.ButtonHighlight;
            buttonKeyboardColor.Dock = DockStyle.Top;
            buttonKeyboardColor.FlatAppearance.BorderColor = Color.Red;
            buttonKeyboardColor.FlatAppearance.BorderSize = 2;
            buttonKeyboardColor.ForeColor = SystemColors.ControlText;
            buttonKeyboardColor.Location = new Point(0, 0);
            buttonKeyboardColor.Margin = new Padding(0);
            buttonKeyboardColor.Name = "buttonKeyboardColor";
            buttonKeyboardColor.Size = new Size(208, 42);
            buttonKeyboardColor.TabIndex = 39;
            buttonKeyboardColor.Text = "Color   ";
            buttonKeyboardColor.UseVisualStyleBackColor = false;
            // 
            // pictureKeyboard
            // 
            pictureKeyboard.BackgroundImage = Properties.Resources.icons8_keyboard_48;
            pictureKeyboard.BackgroundImageLayout = ImageLayout.Zoom;
            pictureKeyboard.Location = new Point(29, 16);
            pictureKeyboard.Margin = new Padding(4, 2, 4, 2);
            pictureKeyboard.Name = "pictureKeyboard";
            pictureKeyboard.Size = new Size(36, 36);
            pictureKeyboard.TabIndex = 33;
            pictureKeyboard.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(72, 16);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(210, 32);
            label1.TabIndex = 32;
            label1.Text = "Laptop Keyboard";
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(192F, 192F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(754, 1217);
            Controls.Add(panelFooter);
            Controls.Add(panelBattery);
            Controls.Add(panelMatrix);
            Controls.Add(panelKeyboard);
            Controls.Add(panelScreen);
            Controls.Add(panelGPU);
            Controls.Add(panelPerformance);
            Margin = new Padding(4, 2, 4, 2);
            MaximizeBox = false;
            MdiChildrenMinimizedAnchorBottom = false;
            MinimizeBox = false;
            MinimumSize = new Size(780, 0);
            Name = "SettingsForm";
            Padding = new Padding(16);
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "G-Helper";
            Load += Settings_Load;
            panelMatrix.ResumeLayout(false);
            panelMatrix.PerformLayout();
            tableLayoutMatrix.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureMatrix).EndInit();
            panelBattery.ResumeLayout(false);
            panelBattery.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBattery).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBattery).EndInit();
            panelFooter.ResumeLayout(false);
            panelFooter.PerformLayout();
            panelPerformance.ResumeLayout(false);
            panelPerformance.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picturePerf).EndInit();
            tablePerf.ResumeLayout(false);
            panelGPU.ResumeLayout(false);
            panelGPU.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureGPU).EndInit();
            tableGPU.ResumeLayout(false);
            panelScreen.ResumeLayout(false);
            panelScreen.PerformLayout();
            tableScreen.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureScreen).EndInit();
            panelKeyboard.ResumeLayout(false);
            panelKeyboard.PerformLayout();
            tableLayoutKeyboard.ResumeLayout(false);
            panelColor.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureColor2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureColor).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureKeyboard).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Panel panelMatrix;
        private PictureBox pictureMatrix;
        private Label labelMatrix;
        private Panel panelBattery;
        private Label labelVersion;
        private Label labelBattery;
        private PictureBox pictureBattery;
        private Label labelBatteryTitle;
        private TrackBar trackBattery;
        private Panel panelFooter;
        private Button buttonQuit;
        private CheckBox checkStartup;
        private Panel panelPerformance;
        private Button buttonFans;
        private PictureBox picturePerf;
        private Label labelPerf;
        private Label labelCPUFan;
        private TableLayoutPanel tablePerf;
        private Button buttonTurbo;
        private Button buttonBalanced;
        private Button buttonSilent;
        private Panel panelGPU;
        private CheckBox checkGPU;
        private PictureBox pictureGPU;
        private Label labelGPU;
        private Label labelGPUFan;
        private TableLayoutPanel tableGPU;
        private Button buttonUltimate;
        private Button buttonStandard;
        private Button buttonEco;
        private Panel panelScreen;
        private CheckBox checkScreen;
        private TableLayoutPanel tableScreen;
        private Button button120Hz;
        private Button button60Hz;
        private PictureBox pictureScreen;
        private Label labelSreen;
        private Panel panelKeyboard;
        private PictureBox pictureKeyboard;
        private Label label1;
        private TableLayoutPanel tableLayoutMatrix;
        private Button buttonMatrix;
        private ComboBox comboMatrixRunning;
        private ComboBox comboMatrix;
        private TableLayoutPanel tableLayoutKeyboard;
        private Button buttonKeyboard;
        private ComboBox comboKeyboard;
        private Panel panelColor;
        private PictureBox pictureColor2;
        private PictureBox pictureColor;
        private Button buttonKeyboardColor;
        private CheckBox checkMatrix;
    }
}