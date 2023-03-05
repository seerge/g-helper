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
            checkStartup = new CheckBox();
            trackBattery = new TrackBar();
            labelBatteryTitle = new Label();
            pictureBattery = new PictureBox();
            labelGPUFan = new Label();
            tableGPU = new TableLayoutPanel();
            buttonUltimate = new Button();
            buttonStandard = new Button();
            buttonEco = new Button();
            labelGPU = new Label();
            pictureGPU = new PictureBox();
            labelCPUFan = new Label();
            tablePerf = new TableLayoutPanel();
            buttonTurbo = new Button();
            buttonBalanced = new Button();
            buttonSilent = new Button();
            picturePerf = new PictureBox();
            labelPerf = new Label();
            checkGPU = new CheckBox();
            buttonQuit = new Button();
            pictureScreen = new PictureBox();
            labelSreen = new Label();
            tableScreen = new TableLayoutPanel();
            button120Hz = new Button();
            button60Hz = new Button();
            checkScreen = new CheckBox();
            checkBoost = new CheckBox();
            pictureKeyboard = new PictureBox();
            label1 = new Label();
            comboKeyboard = new ComboBox();
            buttonKeyboardColor = new Button();
            labelBattery = new Label();
            buttonFans = new Button();
            buttonKeyboard = new Button();
            pictureColor = new PictureBox();
            pictureColor2 = new PictureBox();
            labelVersion = new Label();
            pictureMatrix = new PictureBox();
            labelMatrix = new Label();
            comboMatrix = new ComboBox();
            comboMatrixRunning = new ComboBox();
            buttonMatrix = new Button();
            ((System.ComponentModel.ISupportInitialize)trackBattery).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBattery).BeginInit();
            tableGPU.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureGPU).BeginInit();
            tablePerf.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picturePerf).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureScreen).BeginInit();
            tableScreen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureKeyboard).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureColor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureColor2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureMatrix).BeginInit();
            SuspendLayout();
            // 
            // checkStartup
            // 
            checkStartup.AutoSize = true;
            checkStartup.Location = new Point(18, 569);
            checkStartup.Margin = new Padding(2, 1, 2, 1);
            checkStartup.Name = "checkStartup";
            checkStartup.Size = new Size(105, 19);
            checkStartup.TabIndex = 2;
            checkStartup.Text = "Run on Startup";
            checkStartup.UseVisualStyleBackColor = true;
            checkStartup.CheckedChanged += checkStartup_CheckedChanged;
            // 
            // trackBattery
            // 
            trackBattery.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBattery.LargeChange = 20;
            trackBattery.Location = new Point(11, 515);
            trackBattery.Margin = new Padding(2, 1, 2, 1);
            trackBattery.Maximum = 100;
            trackBattery.Minimum = 50;
            trackBattery.Name = "trackBattery";
            trackBattery.Size = new Size(341, 45);
            trackBattery.SmallChange = 10;
            trackBattery.TabIndex = 3;
            trackBattery.TickFrequency = 10;
            trackBattery.TickStyle = TickStyle.TopLeft;
            trackBattery.Value = 100;
            // 
            // labelBatteryTitle
            // 
            labelBatteryTitle.AutoSize = true;
            labelBatteryTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelBatteryTitle.Location = new Point(40, 496);
            labelBatteryTitle.Margin = new Padding(2, 0, 2, 0);
            labelBatteryTitle.Name = "labelBatteryTitle";
            labelBatteryTitle.Size = new Size(122, 15);
            labelBatteryTitle.TabIndex = 4;
            labelBatteryTitle.Text = "Battery Charge Limit";
            // 
            // pictureBattery
            // 
            pictureBattery.BackgroundImage = (Image)resources.GetObject("pictureBattery.BackgroundImage");
            pictureBattery.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBattery.Location = new Point(19, 495);
            pictureBattery.Margin = new Padding(2, 1, 2, 1);
            pictureBattery.Name = "pictureBattery";
            pictureBattery.Size = new Size(18, 19);
            pictureBattery.TabIndex = 6;
            pictureBattery.TabStop = false;
            // 
            // labelGPUFan
            // 
            labelGPUFan.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelGPUFan.Location = new Point(172, 131);
            labelGPUFan.Margin = new Padding(2, 0, 2, 0);
            labelGPUFan.Name = "labelGPUFan";
            labelGPUFan.Size = new Size(174, 16);
            labelGPUFan.TabIndex = 8;
            labelGPUFan.Text = "         ";
            labelGPUFan.TextAlign = ContentAlignment.TopRight;
            // 
            // tableGPU
            // 
            tableGPU.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableGPU.ColumnCount = 3;
            tableGPU.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            tableGPU.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            tableGPU.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            tableGPU.Controls.Add(buttonUltimate, 2, 0);
            tableGPU.Controls.Add(buttonStandard, 1, 0);
            tableGPU.Controls.Add(buttonEco, 0, 0);
            tableGPU.Location = new Point(11, 152);
            tableGPU.Margin = new Padding(2, 1, 2, 1);
            tableGPU.Name = "tableGPU";
            tableGPU.RowCount = 1;
            tableGPU.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
            tableGPU.Size = new Size(341, 54);
            tableGPU.TabIndex = 7;
            // 
            // buttonUltimate
            // 
            buttonUltimate.BackColor = SystemColors.ControlLightLight;
            buttonUltimate.Dock = DockStyle.Fill;
            buttonUltimate.FlatAppearance.BorderSize = 0;
            buttonUltimate.FlatStyle = FlatStyle.Flat;
            buttonUltimate.Location = new Point(230, 6);
            buttonUltimate.Margin = new Padding(4, 6, 4, 6);
            buttonUltimate.Name = "buttonUltimate";
            buttonUltimate.Size = new Size(107, 42);
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
            buttonStandard.Location = new Point(117, 6);
            buttonStandard.Margin = new Padding(4, 6, 4, 6);
            buttonStandard.Name = "buttonStandard";
            buttonStandard.Size = new Size(105, 42);
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
            buttonEco.Location = new Point(4, 6);
            buttonEco.Margin = new Padding(4, 6, 4, 6);
            buttonEco.Name = "buttonEco";
            buttonEco.Size = new Size(105, 42);
            buttonEco.TabIndex = 0;
            buttonEco.Text = "Eco";
            buttonEco.UseVisualStyleBackColor = false;
            // 
            // labelGPU
            // 
            labelGPU.AutoSize = true;
            labelGPU.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelGPU.Location = new Point(39, 132);
            labelGPU.Margin = new Padding(2, 0, 2, 0);
            labelGPU.Name = "labelGPU";
            labelGPU.Size = new Size(67, 15);
            labelGPU.TabIndex = 9;
            labelGPU.Text = "GPU Mode";
            // 
            // pictureGPU
            // 
            pictureGPU.BackgroundImage = (Image)resources.GetObject("pictureGPU.BackgroundImage");
            pictureGPU.BackgroundImageLayout = ImageLayout.Zoom;
            pictureGPU.Location = new Point(18, 131);
            pictureGPU.Margin = new Padding(2, 1, 2, 1);
            pictureGPU.Name = "pictureGPU";
            pictureGPU.Size = new Size(18, 19);
            pictureGPU.TabIndex = 10;
            pictureGPU.TabStop = false;
            // 
            // labelCPUFan
            // 
            labelCPUFan.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelCPUFan.Location = new Point(163, 19);
            labelCPUFan.Margin = new Padding(2, 0, 2, 0);
            labelCPUFan.Name = "labelCPUFan";
            labelCPUFan.Size = new Size(183, 16);
            labelCPUFan.TabIndex = 12;
            labelCPUFan.Text = "      ";
            labelCPUFan.TextAlign = ContentAlignment.TopRight;
            // 
            // tablePerf
            // 
            tablePerf.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tablePerf.ColumnCount = 3;
            tablePerf.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            tablePerf.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            tablePerf.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            tablePerf.Controls.Add(buttonTurbo, 2, 0);
            tablePerf.Controls.Add(buttonBalanced, 1, 0);
            tablePerf.Controls.Add(buttonSilent, 0, 0);
            tablePerf.Location = new Point(11, 38);
            tablePerf.Margin = new Padding(2, 1, 2, 1);
            tablePerf.Name = "tablePerf";
            tablePerf.RowCount = 1;
            tablePerf.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
            tablePerf.Size = new Size(341, 54);
            tablePerf.TabIndex = 11;
            // 
            // buttonTurbo
            // 
            buttonTurbo.BackColor = SystemColors.ControlLightLight;
            buttonTurbo.Dock = DockStyle.Fill;
            buttonTurbo.FlatAppearance.BorderColor = Color.FromArgb(192, 0, 0);
            buttonTurbo.FlatAppearance.BorderSize = 0;
            buttonTurbo.FlatStyle = FlatStyle.Flat;
            buttonTurbo.Location = new Point(230, 6);
            buttonTurbo.Margin = new Padding(4, 6, 4, 6);
            buttonTurbo.Name = "buttonTurbo";
            buttonTurbo.Size = new Size(107, 42);
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
            buttonBalanced.Location = new Point(117, 6);
            buttonBalanced.Margin = new Padding(4, 6, 4, 6);
            buttonBalanced.Name = "buttonBalanced";
            buttonBalanced.Size = new Size(105, 42);
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
            buttonSilent.Location = new Point(4, 6);
            buttonSilent.Margin = new Padding(4, 6, 4, 6);
            buttonSilent.Name = "buttonSilent";
            buttonSilent.Size = new Size(105, 42);
            buttonSilent.TabIndex = 0;
            buttonSilent.Text = "Silent";
            buttonSilent.UseVisualStyleBackColor = false;
            // 
            // picturePerf
            // 
            picturePerf.BackgroundImage = (Image)resources.GetObject("picturePerf.BackgroundImage");
            picturePerf.BackgroundImageLayout = ImageLayout.Zoom;
            picturePerf.InitialImage = null;
            picturePerf.Location = new Point(18, 18);
            picturePerf.Margin = new Padding(2, 1, 2, 1);
            picturePerf.Name = "picturePerf";
            picturePerf.Size = new Size(18, 19);
            picturePerf.TabIndex = 14;
            picturePerf.TabStop = false;
            // 
            // labelPerf
            // 
            labelPerf.AutoSize = true;
            labelPerf.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelPerf.Location = new Point(39, 19);
            labelPerf.Margin = new Padding(2, 0, 2, 0);
            labelPerf.Name = "labelPerf";
            labelPerf.Size = new Size(115, 15);
            labelPerf.TabIndex = 13;
            labelPerf.Text = "Performance Mode";
            // 
            // checkGPU
            // 
            checkGPU.AutoSize = true;
            checkGPU.ForeColor = SystemColors.GrayText;
            checkGPU.Location = new Point(16, 206);
            checkGPU.Margin = new Padding(2, 1, 2, 1);
            checkGPU.Name = "checkGPU";
            checkGPU.Size = new Size(273, 19);
            checkGPU.TabIndex = 15;
            checkGPU.Text = "Set Eco on battery and Standard when plugged";
            checkGPU.UseVisualStyleBackColor = true;
            checkGPU.CheckedChanged += checkGPU_CheckedChanged;
            // 
            // buttonQuit
            // 
            buttonQuit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonQuit.BackColor = SystemColors.ButtonFace;
            buttonQuit.Location = new Point(292, 565);
            buttonQuit.Margin = new Padding(2, 1, 2, 1);
            buttonQuit.Name = "buttonQuit";
            buttonQuit.Size = new Size(60, 24);
            buttonQuit.TabIndex = 16;
            buttonQuit.Text = "Quit";
            buttonQuit.UseVisualStyleBackColor = false;
            // 
            // pictureScreen
            // 
            pictureScreen.BackgroundImage = (Image)resources.GetObject("pictureScreen.BackgroundImage");
            pictureScreen.BackgroundImageLayout = ImageLayout.Zoom;
            pictureScreen.Location = new Point(18, 248);
            pictureScreen.Margin = new Padding(2, 1, 2, 1);
            pictureScreen.Name = "pictureScreen";
            pictureScreen.Size = new Size(18, 19);
            pictureScreen.TabIndex = 18;
            pictureScreen.TabStop = false;
            // 
            // labelSreen
            // 
            labelSreen.AutoSize = true;
            labelSreen.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelSreen.Location = new Point(39, 248);
            labelSreen.Margin = new Padding(2, 0, 2, 0);
            labelSreen.Name = "labelSreen";
            labelSreen.Size = new Size(87, 15);
            labelSreen.TabIndex = 17;
            labelSreen.Text = "Laptop Screen";
            // 
            // tableScreen
            // 
            tableScreen.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableScreen.ColumnCount = 3;
            tableScreen.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            tableScreen.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            tableScreen.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            tableScreen.Controls.Add(button120Hz, 1, 0);
            tableScreen.Controls.Add(button60Hz, 0, 0);
            tableScreen.Location = new Point(11, 268);
            tableScreen.Margin = new Padding(2, 1, 2, 1);
            tableScreen.Name = "tableScreen";
            tableScreen.RowCount = 1;
            tableScreen.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
            tableScreen.Size = new Size(341, 54);
            tableScreen.TabIndex = 19;
            // 
            // button120Hz
            // 
            button120Hz.BackColor = SystemColors.ControlLightLight;
            button120Hz.Dock = DockStyle.Fill;
            button120Hz.FlatAppearance.BorderColor = SystemColors.ActiveBorder;
            button120Hz.FlatAppearance.BorderSize = 0;
            button120Hz.FlatStyle = FlatStyle.Flat;
            button120Hz.Location = new Point(117, 6);
            button120Hz.Margin = new Padding(4, 6, 4, 6);
            button120Hz.Name = "button120Hz";
            button120Hz.Size = new Size(105, 42);
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
            button60Hz.Location = new Point(4, 6);
            button60Hz.Margin = new Padding(4, 6, 4, 6);
            button60Hz.Name = "button60Hz";
            button60Hz.Size = new Size(105, 42);
            button60Hz.TabIndex = 0;
            button60Hz.Text = "60Hz";
            button60Hz.UseVisualStyleBackColor = false;
            // 
            // checkScreen
            // 
            checkScreen.AutoSize = true;
            checkScreen.ForeColor = SystemColors.GrayText;
            checkScreen.Location = new Point(16, 322);
            checkScreen.Margin = new Padding(2, 1, 2, 1);
            checkScreen.Name = "checkScreen";
            checkScreen.Size = new Size(261, 19);
            checkScreen.TabIndex = 20;
            checkScreen.Text = "Set 60Hz on battery, and back when plugged";
            checkScreen.UseVisualStyleBackColor = true;
            // 
            // checkBoost
            // 
            checkBoost.AutoSize = true;
            checkBoost.ForeColor = SystemColors.GrayText;
            checkBoost.Location = new Point(16, 92);
            checkBoost.Margin = new Padding(2, 1, 2, 1);
            checkBoost.Name = "checkBoost";
            checkBoost.Size = new Size(161, 19);
            checkBoost.TabIndex = 21;
            checkBoost.Text = "CPU Turbo Boost enabled";
            checkBoost.UseVisualStyleBackColor = true;
            // 
            // pictureKeyboard
            // 
            pictureKeyboard.BackgroundImage = Properties.Resources.icons8_keyboard_48;
            pictureKeyboard.BackgroundImageLayout = ImageLayout.Zoom;
            pictureKeyboard.Location = new Point(18, 362);
            pictureKeyboard.Margin = new Padding(2, 1, 2, 1);
            pictureKeyboard.Name = "pictureKeyboard";
            pictureKeyboard.Size = new Size(18, 18);
            pictureKeyboard.TabIndex = 23;
            pictureKeyboard.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(39, 362);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(101, 15);
            label1.TabIndex = 22;
            label1.Text = "Laptop Keyboard";
            // 
            // comboKeyboard
            // 
            comboKeyboard.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            comboKeyboard.FormattingEnabled = true;
            comboKeyboard.ItemHeight = 15;
            comboKeyboard.Items.AddRange(new object[] { "Static", "Breathe", "Strobe", "Rainbow", "Dingding" });
            comboKeyboard.Location = new Point(15, 385);
            comboKeyboard.Margin = new Padding(0);
            comboKeyboard.Name = "comboKeyboard";
            comboKeyboard.Size = new Size(108, 23);
            comboKeyboard.TabIndex = 24;
            comboKeyboard.TabStop = false;
            // 
            // buttonKeyboardColor
            // 
            buttonKeyboardColor.AutoSize = true;
            buttonKeyboardColor.BackColor = SystemColors.ButtonHighlight;
            buttonKeyboardColor.FlatAppearance.BorderColor = Color.Red;
            buttonKeyboardColor.FlatAppearance.BorderSize = 2;
            buttonKeyboardColor.ForeColor = SystemColors.ControlText;
            buttonKeyboardColor.Location = new Point(128, 383);
            buttonKeyboardColor.Margin = new Padding(0);
            buttonKeyboardColor.Name = "buttonKeyboardColor";
            buttonKeyboardColor.Size = new Size(106, 25);
            buttonKeyboardColor.TabIndex = 25;
            buttonKeyboardColor.Text = "Color  ";
            buttonKeyboardColor.UseVisualStyleBackColor = false;
            // 
            // labelBattery
            // 
            labelBattery.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelBattery.Location = new Point(210, 496);
            labelBattery.Margin = new Padding(2, 0, 2, 0);
            labelBattery.Name = "labelBattery";
            labelBattery.Size = new Size(138, 16);
            labelBattery.TabIndex = 27;
            labelBattery.Text = "                ";
            labelBattery.TextAlign = ContentAlignment.TopRight;
            // 
            // buttonFans
            // 
            buttonFans.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonFans.BackColor = SystemColors.ButtonFace;
            buttonFans.FlatAppearance.BorderSize = 0;
            buttonFans.Location = new Point(243, 93);
            buttonFans.Margin = new Padding(2, 1, 2, 1);
            buttonFans.Name = "buttonFans";
            buttonFans.Size = new Size(105, 24);
            buttonFans.TabIndex = 28;
            buttonFans.Text = "Fans and Power";
            buttonFans.UseVisualStyleBackColor = false;
            // 
            // buttonKeyboard
            // 
            buttonKeyboard.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonKeyboard.BackColor = SystemColors.ButtonFace;
            buttonKeyboard.FlatAppearance.BorderSize = 0;
            buttonKeyboard.Location = new Point(243, 382);
            buttonKeyboard.Margin = new Padding(2, 1, 2, 1);
            buttonKeyboard.Name = "buttonKeyboard";
            buttonKeyboard.Size = new Size(104, 24);
            buttonKeyboard.TabIndex = 29;
            buttonKeyboard.Text = "Extra";
            buttonKeyboard.UseVisualStyleBackColor = false;
            // 
            // pictureColor
            // 
            pictureColor.Location = new Point(216, 390);
            pictureColor.Margin = new Padding(2, 2, 2, 2);
            pictureColor.Name = "pictureColor";
            pictureColor.Size = new Size(10, 10);
            pictureColor.TabIndex = 30;
            pictureColor.TabStop = false;
            // 
            // pictureColor2
            // 
            pictureColor2.Location = new Point(202, 390);
            pictureColor2.Margin = new Padding(2, 2, 2, 2);
            pictureColor2.Name = "pictureColor2";
            pictureColor2.Size = new Size(10, 10);
            pictureColor2.TabIndex = 31;
            pictureColor2.TabStop = false;
            // 
            // labelVersion
            // 
            labelVersion.AutoSize = true;
            labelVersion.Font = new Font("Segoe UI", 9F, FontStyle.Underline, GraphicsUnit.Point);
            labelVersion.ForeColor = SystemColors.ControlDark;
            labelVersion.Location = new Point(18, 544);
            labelVersion.Margin = new Padding(2, 0, 2, 0);
            labelVersion.Name = "labelVersion";
            labelVersion.Size = new Size(22, 15);
            labelVersion.TabIndex = 32;
            labelVersion.Text = "v.0";
            // 
            // pictureMatrix
            // 
            pictureMatrix.BackgroundImage = Properties.Resources.icons8_matrix_desktop_48;
            pictureMatrix.BackgroundImageLayout = ImageLayout.Zoom;
            pictureMatrix.Location = new Point(18, 429);
            pictureMatrix.Margin = new Padding(2, 1, 2, 1);
            pictureMatrix.Name = "pictureMatrix";
            pictureMatrix.Size = new Size(18, 18);
            pictureMatrix.TabIndex = 34;
            pictureMatrix.TabStop = false;
            // 
            // labelMatrix
            // 
            labelMatrix.AutoSize = true;
            labelMatrix.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelMatrix.Location = new Point(39, 429);
            labelMatrix.Margin = new Padding(2, 0, 2, 0);
            labelMatrix.Name = "labelMatrix";
            labelMatrix.Size = new Size(83, 15);
            labelMatrix.TabIndex = 33;
            labelMatrix.Text = "Anime Matrix";
            // 
            // comboMatrix
            // 
            comboMatrix.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            comboMatrix.FormattingEnabled = true;
            comboMatrix.ItemHeight = 15;
            comboMatrix.Items.AddRange(new object[] { "Off", "Dim", "Medium", "Bright" });
            comboMatrix.Location = new Point(15, 455);
            comboMatrix.Margin = new Padding(0);
            comboMatrix.Name = "comboMatrix";
            comboMatrix.Size = new Size(108, 23);
            comboMatrix.TabIndex = 35;
            comboMatrix.TabStop = false;
            // 
            // comboMatrixRunning
            // 
            comboMatrixRunning.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            comboMatrixRunning.FormattingEnabled = true;
            comboMatrixRunning.ItemHeight = 15;
            comboMatrixRunning.Items.AddRange(new object[] { "Binary Banner", "Rog Logo", "Picture" });
            comboMatrixRunning.Location = new Point(128, 455);
            comboMatrixRunning.Margin = new Padding(0);
            comboMatrixRunning.Name = "comboMatrixRunning";
            comboMatrixRunning.Size = new Size(108, 23);
            comboMatrixRunning.TabIndex = 36;
            comboMatrixRunning.TabStop = false;
            // 
            // buttonMatrix
            // 
            buttonMatrix.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonMatrix.BackColor = SystemColors.ButtonFace;
            buttonMatrix.FlatAppearance.BorderSize = 0;
            buttonMatrix.Location = new Point(243, 452);
            buttonMatrix.Margin = new Padding(2, 1, 2, 1);
            buttonMatrix.Name = "buttonMatrix";
            buttonMatrix.Size = new Size(104, 24);
            buttonMatrix.TabIndex = 37;
            buttonMatrix.Text = "Picture";
            buttonMatrix.UseVisualStyleBackColor = false;
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(368, 602);
            Controls.Add(buttonMatrix);
            Controls.Add(comboMatrixRunning);
            Controls.Add(comboMatrix);
            Controls.Add(pictureMatrix);
            Controls.Add(labelMatrix);
            Controls.Add(labelVersion);
            Controls.Add(pictureColor2);
            Controls.Add(pictureColor);
            Controls.Add(buttonKeyboard);
            Controls.Add(buttonFans);
            Controls.Add(labelBattery);
            Controls.Add(buttonKeyboardColor);
            Controls.Add(comboKeyboard);
            Controls.Add(pictureKeyboard);
            Controls.Add(label1);
            Controls.Add(checkBoost);
            Controls.Add(checkScreen);
            Controls.Add(tableScreen);
            Controls.Add(pictureScreen);
            Controls.Add(labelSreen);
            Controls.Add(buttonQuit);
            Controls.Add(checkGPU);
            Controls.Add(picturePerf);
            Controls.Add(labelPerf);
            Controls.Add(labelCPUFan);
            Controls.Add(tablePerf);
            Controls.Add(pictureGPU);
            Controls.Add(labelGPU);
            Controls.Add(labelGPUFan);
            Controls.Add(tableGPU);
            Controls.Add(pictureBattery);
            Controls.Add(labelBatteryTitle);
            Controls.Add(trackBattery);
            Controls.Add(checkStartup);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(2, 1, 2, 1);
            MaximizeBox = false;
            MdiChildrenMinimizedAnchorBottom = false;
            MinimizeBox = false;
            Name = "SettingsForm";
            Padding = new Padding(4, 6, 4, 6);
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "G-Helper";
            Load += Settings_Load;
            ((System.ComponentModel.ISupportInitialize)trackBattery).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBattery).EndInit();
            tableGPU.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureGPU).EndInit();
            tablePerf.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picturePerf).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureScreen).EndInit();
            tableScreen.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureKeyboard).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureColor).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureColor2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureMatrix).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private CheckBox checkStartup;
        private TrackBar trackBattery;
        private Label labelBatteryTitle;
        private PictureBox pictureBattery;
        private Label labelGPUFan;
        private TableLayoutPanel tableGPU;
        private Button buttonUltimate;
        private Button buttonStandard;
        private Button buttonEco;
        private Label labelGPU;
        private PictureBox pictureGPU;
        private Label labelCPUFan;
        private TableLayoutPanel tablePerf;
        private Button buttonTurbo;
        private Button buttonBalanced;
        private Button buttonSilent;
        private PictureBox picturePerf;
        private Label labelPerf;
        private CheckBox checkGPU;
        private Button buttonQuit;
        private PictureBox pictureScreen;
        private Label labelSreen;
        private TableLayoutPanel tableScreen;
        private Button button120Hz;
        private Button button60Hz;
        private CheckBox checkScreen;
        private CheckBox checkBoost;
        private PictureBox pictureKeyboard;
        private Label label1;
        private ComboBox comboKeyboard;
        private Button buttonKeyboardColor;
        private Label labelBattery;
        private Button buttonFans;
        private Button buttonKeyboard;
        private PictureBox pictureColor;
        private PictureBox pictureColor2;
        private Label labelVersion;
        private PictureBox pictureMatrix;
        private Label labelMatrix;
        private ComboBox comboMatrix;
        private ComboBox comboMatrixRunning;
        private Button buttonMatrix;
    }
}