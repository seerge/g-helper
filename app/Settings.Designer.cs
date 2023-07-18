using GHelper.UI;

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
            panelMatrix = new Panel();
            tableLayoutMatrix = new TableLayoutPanel();
            comboMatrix = new RComboBox();
            comboMatrixRunning = new RComboBox();
            buttonMatrix = new RButton();
            panelMatrixTitle = new Panel();
            pictureMatrix = new PictureBox();
            labelMatrix = new Label();
            checkMatrix = new CheckBox();
            panelBattery = new Panel();
            sliderBattery = new Slider();
            panelBatteryTitle = new Panel();
            labelBattery = new Label();
            pictureBattery = new PictureBox();
            labelBatteryTitle = new Label();
            panelFooter = new Panel();
            buttonUpdates = new RButton();
            buttonQuit = new RButton();
            checkStartup = new CheckBox();
            panelPerformance = new Panel();
            tablePerf = new TableLayoutPanel();
            buttonSilent = new RButton();
            buttonBalanced = new RButton();
            buttonTurbo = new RButton();
            buttonFans = new RButton();
            panelCPUTitle = new Panel();
            picturePerf = new PictureBox();
            labelPerf = new Label();
            labelCPUFan = new Label();
            panelGPU = new Panel();
            labelTipGPU = new Label();
            tableGPU = new TableLayoutPanel();
            buttonStopGPU = new RButton();
            buttonEco = new RButton();
            buttonStandard = new RButton();
            buttonXGM = new RButton();
            buttonOptimized = new RButton();
            buttonUltimate = new RButton();
            panelGPUTitle = new Panel();
            pictureGPU = new PictureBox();
            labelGPU = new Label();
            labelGPUFan = new Label();
            panelScreen = new Panel();
            labelTipScreen = new Label();
            tableScreen = new TableLayoutPanel();
            buttonScreenAuto = new RButton();
            button60Hz = new RButton();
            button120Hz = new RButton();
            buttonMiniled = new RButton();
            panelScreenTitle = new Panel();
            labelMidFan = new Label();
            pictureScreen = new PictureBox();
            labelSreen = new Label();
            panelKeyboard = new Panel();
            tableLayoutKeyboard = new TableLayoutPanel();
            buttonKeyboard = new RButton();
            panelColor = new Panel();
            pictureColor2 = new PictureBox();
            pictureColor = new PictureBox();
            buttonKeyboardColor = new RButton();
            comboKeyboard = new RComboBox();
            panelKeyboardTitle = new Panel();
            pictureKeyboard = new PictureBox();
            labelKeyboard = new Label();
            labelVersion = new Label();
            labelModel = new Label();
            panelVersion = new Panel();
            panelMatrix.SuspendLayout();
            tableLayoutMatrix.SuspendLayout();
            panelMatrixTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureMatrix).BeginInit();
            panelBattery.SuspendLayout();
            panelBatteryTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBattery).BeginInit();
            panelFooter.SuspendLayout();
            panelPerformance.SuspendLayout();
            tablePerf.SuspendLayout();
            panelCPUTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picturePerf).BeginInit();
            panelGPU.SuspendLayout();
            tableGPU.SuspendLayout();
            panelGPUTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureGPU).BeginInit();
            panelScreen.SuspendLayout();
            tableScreen.SuspendLayout();
            panelScreenTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureScreen).BeginInit();
            panelKeyboard.SuspendLayout();
            tableLayoutKeyboard.SuspendLayout();
            panelColor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureColor2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureColor).BeginInit();
            panelKeyboardTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureKeyboard).BeginInit();
            panelVersion.SuspendLayout();
            SuspendLayout();
            // 
            // panelMatrix
            // 
            panelMatrix.AutoSize = true;
            panelMatrix.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelMatrix.Controls.Add(tableLayoutMatrix);
            panelMatrix.Controls.Add(panelMatrixTitle);
            panelMatrix.Controls.Add(checkMatrix);
            panelMatrix.Dock = DockStyle.Top;
            panelMatrix.Location = new Point(10, 891);
            panelMatrix.Margin = new Padding(0);
            panelMatrix.Name = "panelMatrix";
            panelMatrix.Padding = new Padding(20, 20, 20, 10);
            panelMatrix.Size = new Size(810, 171);
            panelMatrix.TabIndex = 4;
            // 
            // tableLayoutMatrix
            // 
            tableLayoutMatrix.AutoSize = true;
            tableLayoutMatrix.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutMatrix.ColumnCount = 3;
            tableLayoutMatrix.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutMatrix.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutMatrix.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutMatrix.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutMatrix.Controls.Add(comboMatrix, 0, 0);
            tableLayoutMatrix.Controls.Add(comboMatrixRunning, 1, 0);
            tableLayoutMatrix.Controls.Add(buttonMatrix, 2, 0);
            tableLayoutMatrix.Dock = DockStyle.Top;
            tableLayoutMatrix.Location = new Point(20, 59);
            tableLayoutMatrix.Margin = new Padding(8);
            tableLayoutMatrix.Name = "tableLayoutMatrix";
            tableLayoutMatrix.RowCount = 1;
            tableLayoutMatrix.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutMatrix.Size = new Size(770, 59);
            tableLayoutMatrix.TabIndex = 43;
            // 
            // comboMatrix
            // 
            comboMatrix.BorderColor = Color.White;
            comboMatrix.ButtonColor = Color.FromArgb(255, 255, 255);
            comboMatrix.Dock = DockStyle.Top;
            comboMatrix.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            comboMatrix.FormattingEnabled = true;
            comboMatrix.ItemHeight = 32;
            comboMatrix.Items.AddRange(new object[] { Properties.Strings.MatrixOff, Properties.Strings.MatrixDim, Properties.Strings.MatrixMedium, Properties.Strings.MatrixBright });
            comboMatrix.Location = new Point(4, 10);
            comboMatrix.Margin = new Padding(4, 10, 4, 8);
            comboMatrix.Name = "comboMatrix";
            comboMatrix.Size = new Size(248, 40);
            comboMatrix.TabIndex = 16;
            // 
            // comboMatrixRunning
            // 
            comboMatrixRunning.BorderColor = Color.White;
            comboMatrixRunning.ButtonColor = Color.FromArgb(255, 255, 255);
            comboMatrixRunning.Dock = DockStyle.Top;
            comboMatrixRunning.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            comboMatrixRunning.FormattingEnabled = true;
            comboMatrixRunning.ItemHeight = 32;
            comboMatrixRunning.Items.AddRange(new object[] { Properties.Strings.MatrixBanner, Properties.Strings.MatrixLogo, Properties.Strings.MatrixPicture, Properties.Strings.MatrixClock, Properties.Strings.MatrixAudio });
            comboMatrixRunning.Location = new Point(260, 10);
            comboMatrixRunning.Margin = new Padding(4, 10, 4, 8);
            comboMatrixRunning.Name = "comboMatrixRunning";
            comboMatrixRunning.Size = new Size(248, 40);
            comboMatrixRunning.TabIndex = 17;
            // 
            // buttonMatrix
            // 
            buttonMatrix.Activated = false;
            buttonMatrix.BackColor = SystemColors.ControlLight;
            buttonMatrix.BorderColor = Color.Transparent;
            buttonMatrix.BorderRadius = 2;
            buttonMatrix.Dock = DockStyle.Top;
            buttonMatrix.FlatAppearance.BorderSize = 0;
            buttonMatrix.FlatStyle = FlatStyle.Flat;
            buttonMatrix.Location = new Point(516, 7);
            buttonMatrix.Margin = new Padding(4, 7, 4, 7);
            buttonMatrix.Name = "buttonMatrix";
            buttonMatrix.Secondary = true;
            buttonMatrix.Size = new Size(250, 45);
            buttonMatrix.TabIndex = 18;
            buttonMatrix.Text = Properties.Strings.PictureGif;
            buttonMatrix.UseVisualStyleBackColor = false;
            // 
            // panelMatrixTitle
            // 
            panelMatrixTitle.Controls.Add(pictureMatrix);
            panelMatrixTitle.Controls.Add(labelMatrix);
            panelMatrixTitle.Dock = DockStyle.Top;
            panelMatrixTitle.Location = new Point(20, 20);
            panelMatrixTitle.Name = "panelMatrixTitle";
            panelMatrixTitle.Size = new Size(770, 39);
            panelMatrixTitle.TabIndex = 45;
            // 
            // pictureMatrix
            // 
            pictureMatrix.BackgroundImage = Properties.Resources.icons8_matrix_32;
            pictureMatrix.BackgroundImageLayout = ImageLayout.Zoom;
            pictureMatrix.Location = new Point(6, 0);
            pictureMatrix.Margin = new Padding(4);
            pictureMatrix.Name = "pictureMatrix";
            pictureMatrix.Size = new Size(32, 32);
            pictureMatrix.TabIndex = 41;
            pictureMatrix.TabStop = false;
            // 
            // labelMatrix
            // 
            labelMatrix.AutoSize = true;
            labelMatrix.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelMatrix.Location = new Point(44, 0);
            labelMatrix.Margin = new Padding(8, 0, 8, 0);
            labelMatrix.Name = "labelMatrix";
            labelMatrix.Size = new Size(170, 32);
            labelMatrix.TabIndex = 40;
            labelMatrix.Text = "Anime Matrix";
            // 
            // checkMatrix
            // 
            checkMatrix.AutoSize = true;
            checkMatrix.ForeColor = SystemColors.GrayText;
            checkMatrix.Location = new Point(26, 121);
            checkMatrix.Margin = new Padding(8, 4, 8, 4);
            checkMatrix.Name = "checkMatrix";
            checkMatrix.Size = new Size(249, 36);
            checkMatrix.TabIndex = 19;
            checkMatrix.Text = Properties.Strings.TurnOffOnBattery;
            checkMatrix.UseVisualStyleBackColor = true;
            // 
            // panelBattery
            // 
            panelBattery.AutoSize = true;
            panelBattery.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelBattery.Controls.Add(sliderBattery);
            panelBattery.Controls.Add(panelBatteryTitle);
            panelBattery.Dock = DockStyle.Top;
            panelBattery.Location = new Point(10, 1062);
            panelBattery.Margin = new Padding(0);
            panelBattery.Name = "panelBattery";
            panelBattery.Padding = new Padding(20);
            panelBattery.Size = new Size(810, 125);
            panelBattery.TabIndex = 5;
            // 
            // sliderBattery
            // 
            sliderBattery.AccessibleName = "Battery Charge Limit";
            sliderBattery.Dock = DockStyle.Top;
            sliderBattery.Location = new Point(20, 65);
            sliderBattery.Max = 100;
            sliderBattery.Min = 40;
            sliderBattery.Name = "sliderBattery";
            sliderBattery.Size = new Size(770, 40);
            sliderBattery.Step = 5;
            sliderBattery.TabIndex = 20;
            sliderBattery.TabStop = false;
            sliderBattery.Text = "sliderBattery";
            sliderBattery.Value = 80;
            // 
            // panelBatteryTitle
            // 
            panelBatteryTitle.Controls.Add(labelBattery);
            panelBatteryTitle.Controls.Add(pictureBattery);
            panelBatteryTitle.Controls.Add(labelBatteryTitle);
            panelBatteryTitle.Dock = DockStyle.Top;
            panelBatteryTitle.Location = new Point(20, 20);
            panelBatteryTitle.Name = "panelBatteryTitle";
            panelBatteryTitle.Padding = new Padding(0, 0, 0, 5);
            panelBatteryTitle.Size = new Size(770, 45);
            panelBatteryTitle.TabIndex = 40;
            // 
            // labelBattery
            // 
            labelBattery.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelBattery.Location = new Point(432, 0);
            labelBattery.Margin = new Padding(8, 0, 8, 0);
            labelBattery.Name = "labelBattery";
            labelBattery.Size = new Size(324, 36);
            labelBattery.TabIndex = 39;
            labelBattery.Text = "                ";
            labelBattery.TextAlign = ContentAlignment.TopRight;
            // 
            // pictureBattery
            // 
            pictureBattery.BackgroundImage = Properties.Resources.icons8_charging_battery_32;
            pictureBattery.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBattery.Location = new Point(4, 1);
            pictureBattery.Margin = new Padding(4);
            pictureBattery.Name = "pictureBattery";
            pictureBattery.Size = new Size(32, 32);
            pictureBattery.TabIndex = 38;
            pictureBattery.TabStop = false;
            // 
            // labelBatteryTitle
            // 
            labelBatteryTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelBatteryTitle.Location = new Point(42, 0);
            labelBatteryTitle.Margin = new Padding(8, 0, 8, 0);
            labelBatteryTitle.Name = "labelBatteryTitle";
            labelBatteryTitle.Size = new Size(466, 32);
            labelBatteryTitle.TabIndex = 37;
            labelBatteryTitle.Text = "Battery Charge Limit";
            // 
            // panelFooter
            // 
            panelFooter.AutoSize = true;
            panelFooter.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelFooter.Controls.Add(buttonUpdates);
            panelFooter.Controls.Add(buttonQuit);
            panelFooter.Controls.Add(checkStartup);
            panelFooter.Dock = DockStyle.Top;
            panelFooter.Location = new Point(10, 1244);
            panelFooter.Margin = new Padding(0);
            panelFooter.Name = "panelFooter";
            panelFooter.Padding = new Padding(20);
            panelFooter.Size = new Size(810, 92);
            panelFooter.TabIndex = 7;
            // 
            // buttonUpdates
            // 
            buttonUpdates.AccessibleName = "BIOS and Driver Updates";
            buttonUpdates.Activated = false;
            buttonUpdates.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonUpdates.BackColor = SystemColors.ControlLight;
            buttonUpdates.BorderColor = Color.Transparent;
            buttonUpdates.BorderRadius = 2;
            buttonUpdates.FlatStyle = FlatStyle.Flat;
            buttonUpdates.Location = new Point(412, 24);
            buttonUpdates.Margin = new Padding(8, 4, 8, 4);
            buttonUpdates.Name = "buttonUpdates";
            buttonUpdates.Secondary = true;
            buttonUpdates.Size = new Size(180, 44);
            buttonUpdates.TabIndex = 22;
            buttonUpdates.Text = "Updates";
            buttonUpdates.UseVisualStyleBackColor = false;
            // 
            // buttonQuit
            // 
            buttonQuit.AccessibleName = "Quit Application";
            buttonQuit.Activated = false;
            buttonQuit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonQuit.BackColor = SystemColors.ControlLight;
            buttonQuit.BorderColor = Color.Transparent;
            buttonQuit.BorderRadius = 2;
            buttonQuit.FlatStyle = FlatStyle.Flat;
            buttonQuit.Location = new Point(606, 24);
            buttonQuit.Margin = new Padding(8, 4, 8, 4);
            buttonQuit.Name = "buttonQuit";
            buttonQuit.Secondary = true;
            buttonQuit.Size = new Size(180, 44);
            buttonQuit.TabIndex = 23;
            buttonQuit.Text = Properties.Strings.Quit;
            buttonQuit.UseVisualStyleBackColor = false;
            // 
            // checkStartup
            // 
            checkStartup.AutoSize = true;
            checkStartup.Location = new Point(26, 29);
            checkStartup.Margin = new Padding(8, 4, 8, 4);
            checkStartup.Name = "checkStartup";
            checkStartup.Size = new Size(206, 36);
            checkStartup.TabIndex = 21;
            checkStartup.Text = Properties.Strings.RunOnStartup;
            checkStartup.UseVisualStyleBackColor = true;
            // 
            // panelPerformance
            // 
            panelPerformance.AutoSize = true;
            panelPerformance.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelPerformance.Controls.Add(tablePerf);
            panelPerformance.Controls.Add(panelCPUTitle);
            panelPerformance.Dock = DockStyle.Top;
            panelPerformance.Location = new Point(10, 10);
            panelPerformance.Margin = new Padding(0);
            panelPerformance.Name = "panelPerformance";
            panelPerformance.Padding = new Padding(20);
            panelPerformance.Size = new Size(810, 207);
            panelPerformance.TabIndex = 0;
            // 
            // tablePerf
            // 
            tablePerf.AutoSize = true;
            tablePerf.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tablePerf.ColumnCount = 4;
            tablePerf.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tablePerf.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tablePerf.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tablePerf.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tablePerf.Controls.Add(buttonSilent, 0, 0);
            tablePerf.Controls.Add(buttonBalanced, 1, 0);
            tablePerf.Controls.Add(buttonTurbo, 2, 0);
            tablePerf.Controls.Add(buttonFans, 3, 0);
            tablePerf.Dock = DockStyle.Top;
            tablePerf.Location = new Point(20, 59);
            tablePerf.Margin = new Padding(8, 4, 8, 4);
            tablePerf.Name = "tablePerf";
            tablePerf.RowCount = 1;
            tablePerf.RowStyles.Add(new RowStyle(SizeType.Absolute, 128F));
            tablePerf.Size = new Size(770, 128);
            tablePerf.TabIndex = 29;
            // 
            // buttonSilent
            // 
            buttonSilent.AccessibleName = "Silent Mode";
            buttonSilent.Activated = false;
            buttonSilent.BackColor = SystemColors.ControlLightLight;
            buttonSilent.BackgroundImageLayout = ImageLayout.None;
            buttonSilent.BorderColor = Color.Transparent;
            buttonSilent.BorderRadius = 5;
            buttonSilent.Dock = DockStyle.Fill;
            buttonSilent.FlatAppearance.BorderSize = 0;
            buttonSilent.FlatStyle = FlatStyle.Flat;
            buttonSilent.ForeColor = SystemColors.ControlText;
            buttonSilent.Image = Properties.Resources.icons8_bicycle_48__1_;
            buttonSilent.ImageAlign = ContentAlignment.BottomCenter;
            buttonSilent.Location = new Point(4, 4);
            buttonSilent.Margin = new Padding(4);
            buttonSilent.Name = "buttonSilent";
            buttonSilent.Secondary = false;
            buttonSilent.Size = new Size(184, 120);
            buttonSilent.TabIndex = 1;
            buttonSilent.Text = Properties.Strings.Silent;
            buttonSilent.TextImageRelation = TextImageRelation.ImageAboveText;
            buttonSilent.UseVisualStyleBackColor = false;
            // 
            // buttonBalanced
            // 
            buttonBalanced.AccessibleName = "Balanced Mode";
            buttonBalanced.Activated = false;
            buttonBalanced.BackColor = SystemColors.ControlLightLight;
            buttonBalanced.BorderColor = Color.Transparent;
            buttonBalanced.BorderRadius = 5;
            buttonBalanced.Dock = DockStyle.Fill;
            buttonBalanced.FlatAppearance.BorderSize = 0;
            buttonBalanced.FlatStyle = FlatStyle.Flat;
            buttonBalanced.ForeColor = SystemColors.ControlText;
            buttonBalanced.Image = Properties.Resources.icons8_fiat_500_48;
            buttonBalanced.ImageAlign = ContentAlignment.BottomCenter;
            buttonBalanced.Location = new Point(196, 4);
            buttonBalanced.Margin = new Padding(4);
            buttonBalanced.Name = "buttonBalanced";
            buttonBalanced.Secondary = false;
            buttonBalanced.Size = new Size(184, 120);
            buttonBalanced.TabIndex = 1;
            buttonBalanced.Text = Properties.Strings.Balanced;
            buttonBalanced.TextImageRelation = TextImageRelation.ImageAboveText;
            buttonBalanced.UseVisualStyleBackColor = false;
            // 
            // buttonTurbo
            // 
            buttonTurbo.AccessibleName = "Turbo Mode";
            buttonTurbo.Activated = false;
            buttonTurbo.BackColor = SystemColors.ControlLightLight;
            buttonTurbo.BorderColor = Color.Transparent;
            buttonTurbo.BorderRadius = 5;
            buttonTurbo.Dock = DockStyle.Fill;
            buttonTurbo.FlatAppearance.BorderSize = 0;
            buttonTurbo.FlatStyle = FlatStyle.Flat;
            buttonTurbo.ForeColor = SystemColors.ControlText;
            buttonTurbo.Image = Properties.Resources.icons8_rocket_48;
            buttonTurbo.ImageAlign = ContentAlignment.BottomCenter;
            buttonTurbo.Location = new Point(388, 4);
            buttonTurbo.Margin = new Padding(4);
            buttonTurbo.Name = "buttonTurbo";
            buttonTurbo.Secondary = false;
            buttonTurbo.Size = new Size(184, 120);
            buttonTurbo.TabIndex = 2;
            buttonTurbo.Text = Properties.Strings.Turbo;
            buttonTurbo.TextImageRelation = TextImageRelation.ImageAboveText;
            buttonTurbo.UseVisualStyleBackColor = false;
            // 
            // buttonFans
            // 
            buttonFans.AccessibleName = "Fans and Power Settings";
            buttonFans.Activated = false;
            buttonFans.BackColor = SystemColors.ControlLight;
            buttonFans.BorderColor = Color.Transparent;
            buttonFans.BorderRadius = 5;
            buttonFans.Dock = DockStyle.Fill;
            buttonFans.FlatAppearance.BorderSize = 0;
            buttonFans.FlatStyle = FlatStyle.Flat;
            buttonFans.Image = Properties.Resources.icons8_fan_48;
            buttonFans.ImageAlign = ContentAlignment.BottomCenter;
            buttonFans.Location = new Point(580, 4);
            buttonFans.Margin = new Padding(4);
            buttonFans.Name = "buttonFans";
            buttonFans.Secondary = true;
            buttonFans.Size = new Size(186, 120);
            buttonFans.TabIndex = 3;
            buttonFans.Text = Properties.Strings.FansPower;
            buttonFans.TextImageRelation = TextImageRelation.ImageAboveText;
            buttonFans.UseVisualStyleBackColor = false;
            // 
            // panelCPUTitle
            // 
            panelCPUTitle.Controls.Add(picturePerf);
            panelCPUTitle.Controls.Add(labelPerf);
            panelCPUTitle.Controls.Add(labelCPUFan);
            panelCPUTitle.Dock = DockStyle.Top;
            panelCPUTitle.Location = new Point(20, 20);
            panelCPUTitle.Name = "panelCPUTitle";
            panelCPUTitle.Size = new Size(770, 39);
            panelCPUTitle.TabIndex = 30;
            // 
            // picturePerf
            // 
            picturePerf.BackgroundImage = Properties.Resources.icons8_gauge_32;
            picturePerf.BackgroundImageLayout = ImageLayout.Zoom;
            picturePerf.InitialImage = null;
            picturePerf.Location = new Point(8, 0);
            picturePerf.Margin = new Padding(4);
            picturePerf.Name = "picturePerf";
            picturePerf.Size = new Size(32, 32);
            picturePerf.TabIndex = 35;
            picturePerf.TabStop = false;
            // 
            // labelPerf
            // 
            labelPerf.AutoSize = true;
            labelPerf.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelPerf.Location = new Point(40, 0);
            labelPerf.Margin = new Padding(8, 0, 8, 0);
            labelPerf.Name = "labelPerf";
            labelPerf.Size = new Size(234, 32);
            labelPerf.TabIndex = 0;
            labelPerf.Text = "Performance Mode";
            // 
            // labelCPUFan
            // 
            labelCPUFan.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelCPUFan.Cursor = Cursors.Hand;
            labelCPUFan.Location = new Point(370, 0);
            labelCPUFan.Margin = new Padding(8, 0, 8, 0);
            labelCPUFan.Name = "labelCPUFan";
            labelCPUFan.Size = new Size(400, 36);
            labelCPUFan.TabIndex = 33;
            labelCPUFan.Text = "      ";
            labelCPUFan.TextAlign = ContentAlignment.TopRight;
            // 
            // panelGPU
            // 
            panelGPU.AutoSize = true;
            panelGPU.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelGPU.Controls.Add(labelTipGPU);
            panelGPU.Controls.Add(tableGPU);
            panelGPU.Controls.Add(panelGPUTitle);
            panelGPU.Dock = DockStyle.Top;
            panelGPU.Location = new Point(10, 217);
            panelGPU.Margin = new Padding(0);
            panelGPU.Name = "panelGPU";
            panelGPU.Padding = new Padding(20, 20, 20, 0);
            panelGPU.Size = new Size(810, 351);
            panelGPU.TabIndex = 1;
            // 
            // labelTipGPU
            // 
            labelTipGPU.Dock = DockStyle.Top;
            labelTipGPU.ForeColor = SystemColors.GrayText;
            labelTipGPU.Location = new Point(20, 315);
            labelTipGPU.Margin = new Padding(4, 0, 4, 0);
            labelTipGPU.Name = "labelTipGPU";
            labelTipGPU.Size = new Size(770, 36);
            labelTipGPU.TabIndex = 20;
            // 
            // tableGPU
            // 
            tableGPU.AutoSize = true;
            tableGPU.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableGPU.ColumnCount = 4;
            tableGPU.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableGPU.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableGPU.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableGPU.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableGPU.Controls.Add(buttonStopGPU, 0, 0);
            tableGPU.Controls.Add(buttonEco, 0, 0);
            tableGPU.Controls.Add(buttonStandard, 1, 0);
            tableGPU.Controls.Add(buttonXGM, 2, 0);
            tableGPU.Controls.Add(buttonOptimized, 2, 0);
            tableGPU.Controls.Add(buttonUltimate, 2, 0);
            tableGPU.Dock = DockStyle.Top;
            tableGPU.Location = new Point(20, 59);
            tableGPU.Margin = new Padding(8, 4, 8, 4);
            tableGPU.Name = "tableGPU";
            tableGPU.RowCount = 1;
            tableGPU.RowStyles.Add(new RowStyle(SizeType.Absolute, 128F));
            tableGPU.RowStyles.Add(new RowStyle(SizeType.Absolute, 128F));
            tableGPU.Size = new Size(770, 256);
            tableGPU.TabIndex = 16;
            // 
            // buttonStopGPU
            // 
            buttonStopGPU.Activated = false;
            buttonStopGPU.BackColor = SystemColors.ControlLightLight;
            buttonStopGPU.BorderColor = Color.Transparent;
            buttonStopGPU.BorderRadius = 5;
            buttonStopGPU.CausesValidation = false;
            buttonStopGPU.Dock = DockStyle.Top;
            buttonStopGPU.FlatAppearance.BorderSize = 0;
            buttonStopGPU.FlatStyle = FlatStyle.Flat;
            buttonStopGPU.ForeColor = SystemColors.ControlText;
            buttonStopGPU.Image = Properties.Resources.icons8_leaf_48;
            buttonStopGPU.ImageAlign = ContentAlignment.BottomCenter;
            buttonStopGPU.Location = new Point(196, 4);
            buttonStopGPU.Margin = new Padding(4);
            buttonStopGPU.Name = "buttonStopGPU";
            buttonStopGPU.Secondary = false;
            buttonStopGPU.Size = new Size(184, 120);
            buttonStopGPU.TabIndex = 4;
            buttonStopGPU.Text = "Stop GPU applications";
            buttonStopGPU.TextImageRelation = TextImageRelation.ImageAboveText;
            buttonStopGPU.UseVisualStyleBackColor = false;
            buttonStopGPU.Visible = false;
            // 
            // buttonEco
            // 
            buttonEco.AccessibleName = "Eco GPU Mode";
            buttonEco.Activated = false;
            buttonEco.BackColor = SystemColors.ControlLightLight;
            buttonEco.BorderColor = Color.Transparent;
            buttonEco.BorderRadius = 5;
            buttonEco.CausesValidation = false;
            buttonEco.Dock = DockStyle.Top;
            buttonEco.FlatAppearance.BorderSize = 0;
            buttonEco.FlatStyle = FlatStyle.Flat;
            buttonEco.ForeColor = SystemColors.ControlText;
            buttonEco.Image = Properties.Resources.icons8_leaf_48;
            buttonEco.ImageAlign = ContentAlignment.BottomCenter;
            buttonEco.Location = new Point(4, 4);
            buttonEco.Margin = new Padding(4);
            buttonEco.Name = "buttonEco";
            buttonEco.Secondary = false;
            buttonEco.Size = new Size(184, 120);
            buttonEco.TabIndex = 4;
            buttonEco.Text = Properties.Strings.EcoMode;
            buttonEco.TextImageRelation = TextImageRelation.ImageAboveText;
            buttonEco.UseVisualStyleBackColor = false;
            // 
            // buttonStandard
            // 
            buttonStandard.AccessibleName = "Standard GPU Mode";
            buttonStandard.Activated = false;
            buttonStandard.BackColor = SystemColors.ControlLightLight;
            buttonStandard.BorderColor = Color.Transparent;
            buttonStandard.BorderRadius = 5;
            buttonStandard.Dock = DockStyle.Top;
            buttonStandard.FlatAppearance.BorderSize = 0;
            buttonStandard.FlatStyle = FlatStyle.Flat;
            buttonStandard.ForeColor = SystemColors.ControlText;
            buttonStandard.Image = Properties.Resources.icons8_spa_flower_48;
            buttonStandard.ImageAlign = ContentAlignment.BottomCenter;
            buttonStandard.Location = new Point(388, 4);
            buttonStandard.Margin = new Padding(4);
            buttonStandard.Name = "buttonStandard";
            buttonStandard.Secondary = false;
            buttonStandard.Size = new Size(184, 120);
            buttonStandard.TabIndex = 5;
            buttonStandard.Text = Properties.Strings.StandardMode;
            buttonStandard.TextImageRelation = TextImageRelation.ImageAboveText;
            buttonStandard.UseVisualStyleBackColor = false;
            // 
            // buttonXGM
            // 
            buttonXGM.Activated = false;
            buttonXGM.BackColor = SystemColors.ControlLightLight;
            buttonXGM.BorderColor = Color.Transparent;
            buttonXGM.BorderRadius = 5;
            buttonXGM.Dock = DockStyle.Top;
            buttonXGM.FlatAppearance.BorderSize = 0;
            buttonXGM.FlatStyle = FlatStyle.Flat;
            buttonXGM.ForeColor = SystemColors.ControlText;
            buttonXGM.Image = Properties.Resources.icons8_video_48;
            buttonXGM.ImageAlign = ContentAlignment.BottomCenter;
            buttonXGM.Location = new Point(196, 132);
            buttonXGM.Margin = new Padding(4);
            buttonXGM.Name = "buttonXGM";
            buttonXGM.Secondary = false;
            buttonXGM.Size = new Size(184, 120);
            buttonXGM.TabIndex = 8;
            buttonXGM.Text = "XG Mobile";
            buttonXGM.TextImageRelation = TextImageRelation.ImageAboveText;
            buttonXGM.UseVisualStyleBackColor = false;
            buttonXGM.Visible = false;
            // 
            // buttonOptimized
            // 
            buttonOptimized.Activated = false;
            buttonOptimized.BackColor = SystemColors.ControlLightLight;
            buttonOptimized.BorderColor = Color.Transparent;
            buttonOptimized.BorderRadius = 5;
            buttonOptimized.Dock = DockStyle.Top;
            buttonOptimized.FlatAppearance.BorderSize = 0;
            buttonOptimized.FlatStyle = FlatStyle.Flat;
            buttonOptimized.ForeColor = SystemColors.ControlText;
            buttonOptimized.Image = Properties.Resources.icons8_project_management_48__1_;
            buttonOptimized.ImageAlign = ContentAlignment.BottomCenter;
            buttonOptimized.Location = new Point(4, 132);
            buttonOptimized.Margin = new Padding(4);
            buttonOptimized.Name = "buttonOptimized";
            buttonOptimized.Secondary = false;
            buttonOptimized.Size = new Size(184, 120);
            buttonOptimized.TabIndex = 7;
            buttonOptimized.Text = Properties.Strings.Optimized;
            buttonOptimized.TextImageRelation = TextImageRelation.ImageAboveText;
            buttonOptimized.UseVisualStyleBackColor = false;
            // 
            // buttonUltimate
            // 
            buttonUltimate.AccessibleName = "Ultimate GPU Mode";
            buttonUltimate.Activated = false;
            buttonUltimate.BackColor = SystemColors.ControlLightLight;
            buttonUltimate.BorderColor = Color.Transparent;
            buttonUltimate.BorderRadius = 5;
            buttonUltimate.Dock = DockStyle.Top;
            buttonUltimate.FlatAppearance.BorderSize = 0;
            buttonUltimate.FlatStyle = FlatStyle.Flat;
            buttonUltimate.ForeColor = SystemColors.ControlText;
            buttonUltimate.Image = Properties.Resources.icons8_game_controller_48;
            buttonUltimate.ImageAlign = ContentAlignment.BottomCenter;
            buttonUltimate.Location = new Point(580, 4);
            buttonUltimate.Margin = new Padding(4);
            buttonUltimate.Name = "buttonUltimate";
            buttonUltimate.Secondary = false;
            buttonUltimate.Size = new Size(186, 120);
            buttonUltimate.TabIndex = 6;
            buttonUltimate.Text = Properties.Strings.UltimateMode;
            buttonUltimate.TextImageRelation = TextImageRelation.ImageAboveText;
            buttonUltimate.UseVisualStyleBackColor = false;
            // 
            // panelGPUTitle
            // 
            panelGPUTitle.Controls.Add(pictureGPU);
            panelGPUTitle.Controls.Add(labelGPU);
            panelGPUTitle.Controls.Add(labelGPUFan);
            panelGPUTitle.Dock = DockStyle.Top;
            panelGPUTitle.Location = new Point(20, 20);
            panelGPUTitle.Name = "panelGPUTitle";
            panelGPUTitle.Size = new Size(770, 39);
            panelGPUTitle.TabIndex = 21;
            // 
            // pictureGPU
            // 
            pictureGPU.BackgroundImage = Properties.Resources.icons8_video_card_32;
            pictureGPU.BackgroundImageLayout = ImageLayout.Zoom;
            pictureGPU.Location = new Point(8, 0);
            pictureGPU.Margin = new Padding(4);
            pictureGPU.Name = "pictureGPU";
            pictureGPU.Size = new Size(32, 32);
            pictureGPU.TabIndex = 22;
            pictureGPU.TabStop = false;
            // 
            // labelGPU
            // 
            labelGPU.AutoSize = true;
            labelGPU.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelGPU.Location = new Point(40, 0);
            labelGPU.Margin = new Padding(8, 0, 8, 0);
            labelGPU.Name = "labelGPU";
            labelGPU.Size = new Size(136, 32);
            labelGPU.TabIndex = 21;
            labelGPU.Text = "GPU Mode";
            // 
            // labelGPUFan
            // 
            labelGPUFan.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelGPUFan.Location = new Point(370, 0);
            labelGPUFan.Margin = new Padding(8, 0, 8, 0);
            labelGPUFan.Name = "labelGPUFan";
            labelGPUFan.Size = new Size(400, 34);
            labelGPUFan.TabIndex = 20;
            labelGPUFan.Text = "         ";
            labelGPUFan.TextAlign = ContentAlignment.TopRight;
            // 
            // panelScreen
            // 
            panelScreen.AutoSize = true;
            panelScreen.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelScreen.Controls.Add(labelTipScreen);
            panelScreen.Controls.Add(tableScreen);
            panelScreen.Controls.Add(panelScreenTitle);
            panelScreen.Dock = DockStyle.Top;
            panelScreen.Location = new Point(10, 568);
            panelScreen.Margin = new Padding(0);
            panelScreen.Name = "panelScreen";
            panelScreen.Padding = new Padding(20, 20, 20, 10);
            panelScreen.Size = new Size(810, 185);
            panelScreen.TabIndex = 2;
            // 
            // labelTipScreen
            // 
            labelTipScreen.Dock = DockStyle.Top;
            labelTipScreen.ForeColor = SystemColors.GrayText;
            labelTipScreen.Location = new Point(20, 139);
            labelTipScreen.Margin = new Padding(4, 0, 4, 0);
            labelTipScreen.Name = "labelTipScreen";
            labelTipScreen.Size = new Size(770, 36);
            labelTipScreen.TabIndex = 24;
            // 
            // tableScreen
            // 
            tableScreen.AutoSize = true;
            tableScreen.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableScreen.ColumnCount = 4;
            tableScreen.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableScreen.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableScreen.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableScreen.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableScreen.Controls.Add(buttonScreenAuto, 0, 0);
            tableScreen.Controls.Add(button60Hz, 1, 0);
            tableScreen.Controls.Add(button120Hz, 2, 0);
            tableScreen.Controls.Add(buttonMiniled, 3, 0);
            tableScreen.Dock = DockStyle.Top;
            tableScreen.Location = new Point(20, 59);
            tableScreen.Margin = new Padding(8, 4, 8, 4);
            tableScreen.Name = "tableScreen";
            tableScreen.RowCount = 1;
            tableScreen.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));
            tableScreen.Size = new Size(770, 80);
            tableScreen.TabIndex = 23;
            // 
            // buttonScreenAuto
            // 
            buttonScreenAuto.AccessibleName = "Auto Screen Refresh Rate";
            buttonScreenAuto.Activated = false;
            buttonScreenAuto.BackColor = SystemColors.ControlLightLight;
            buttonScreenAuto.BorderColor = Color.Transparent;
            buttonScreenAuto.BorderRadius = 5;
            buttonScreenAuto.Dock = DockStyle.Fill;
            buttonScreenAuto.FlatAppearance.BorderSize = 0;
            buttonScreenAuto.FlatStyle = FlatStyle.Flat;
            buttonScreenAuto.ForeColor = SystemColors.ControlText;
            buttonScreenAuto.Location = new Point(4, 4);
            buttonScreenAuto.Margin = new Padding(4);
            buttonScreenAuto.Name = "buttonScreenAuto";
            buttonScreenAuto.Secondary = false;
            buttonScreenAuto.Size = new Size(184, 72);
            buttonScreenAuto.TabIndex = 9;
            buttonScreenAuto.Text = Properties.Strings.AutoMode;
            buttonScreenAuto.UseVisualStyleBackColor = false;
            // 
            // button60Hz
            // 
            button60Hz.AccessibleName = "60Hz Refresh Rate";
            button60Hz.Activated = false;
            button60Hz.BackColor = SystemColors.ControlLightLight;
            button60Hz.BorderColor = Color.Transparent;
            button60Hz.BorderRadius = 5;
            button60Hz.CausesValidation = false;
            button60Hz.Dock = DockStyle.Fill;
            button60Hz.FlatAppearance.BorderSize = 0;
            button60Hz.FlatStyle = FlatStyle.Flat;
            button60Hz.ForeColor = SystemColors.ControlText;
            button60Hz.Location = new Point(196, 4);
            button60Hz.Margin = new Padding(4);
            button60Hz.Name = "button60Hz";
            button60Hz.Secondary = false;
            button60Hz.Size = new Size(184, 72);
            button60Hz.TabIndex = 10;
            button60Hz.Text = "60Hz";
            button60Hz.UseVisualStyleBackColor = false;
            // 
            // button120Hz
            // 
            button120Hz.AccessibleName = "Maximum Refresh Rate";
            button120Hz.Activated = false;
            button120Hz.BackColor = SystemColors.ControlLightLight;
            button120Hz.BorderColor = Color.Transparent;
            button120Hz.BorderRadius = 5;
            button120Hz.Dock = DockStyle.Fill;
            button120Hz.FlatAppearance.BorderSize = 0;
            button120Hz.FlatStyle = FlatStyle.Flat;
            button120Hz.ForeColor = SystemColors.ControlText;
            button120Hz.Location = new Point(388, 4);
            button120Hz.Margin = new Padding(4);
            button120Hz.Name = "button120Hz";
            button120Hz.Secondary = false;
            button120Hz.Size = new Size(184, 72);
            button120Hz.TabIndex = 11;
            button120Hz.Text = "120Hz + OD";
            button120Hz.UseVisualStyleBackColor = false;
            // 
            // buttonMiniled
            // 
            buttonMiniled.Activated = false;
            buttonMiniled.BackColor = SystemColors.ControlLightLight;
            buttonMiniled.BorderColor = Color.Transparent;
            buttonMiniled.BorderRadius = 5;
            buttonMiniled.CausesValidation = false;
            buttonMiniled.Dock = DockStyle.Fill;
            buttonMiniled.FlatAppearance.BorderSize = 0;
            buttonMiniled.FlatStyle = FlatStyle.Flat;
            buttonMiniled.ForeColor = SystemColors.ControlText;
            buttonMiniled.Location = new Point(580, 4);
            buttonMiniled.Margin = new Padding(4);
            buttonMiniled.Name = "buttonMiniled";
            buttonMiniled.Secondary = false;
            buttonMiniled.Size = new Size(186, 72);
            buttonMiniled.TabIndex = 12;
            buttonMiniled.Text = Properties.Strings.Multizone;
            buttonMiniled.UseVisualStyleBackColor = false;
            // 
            // panelScreenTitle
            // 
            panelScreenTitle.Controls.Add(labelMidFan);
            panelScreenTitle.Controls.Add(pictureScreen);
            panelScreenTitle.Controls.Add(labelSreen);
            panelScreenTitle.Dock = DockStyle.Top;
            panelScreenTitle.Location = new Point(20, 20);
            panelScreenTitle.Name = "panelScreenTitle";
            panelScreenTitle.Size = new Size(770, 39);
            panelScreenTitle.TabIndex = 25;
            // 
            // labelMidFan
            // 
            labelMidFan.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelMidFan.Location = new Point(516, -2);
            labelMidFan.Margin = new Padding(8, 0, 8, 0);
            labelMidFan.Name = "labelMidFan";
            labelMidFan.Size = new Size(254, 34);
            labelMidFan.TabIndex = 28;
            labelMidFan.Text = "         ";
            labelMidFan.TextAlign = ContentAlignment.TopRight;
            // 
            // pictureScreen
            // 
            pictureScreen.BackgroundImage = Properties.Resources.icons8_laptop_32;
            pictureScreen.BackgroundImageLayout = ImageLayout.Zoom;
            pictureScreen.Location = new Point(6, 0);
            pictureScreen.Margin = new Padding(4);
            pictureScreen.Name = "pictureScreen";
            pictureScreen.Size = new Size(32, 32);
            pictureScreen.TabIndex = 27;
            pictureScreen.TabStop = false;
            // 
            // labelSreen
            // 
            labelSreen.AutoSize = true;
            labelSreen.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelSreen.Location = new Point(40, -1);
            labelSreen.Margin = new Padding(8, 0, 8, 0);
            labelSreen.Name = "labelSreen";
            labelSreen.Size = new Size(176, 32);
            labelSreen.TabIndex = 26;
            labelSreen.Text = "Laptop Screen";
            // 
            // panelKeyboard
            // 
            panelKeyboard.AutoSize = true;
            panelKeyboard.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelKeyboard.Controls.Add(tableLayoutKeyboard);
            panelKeyboard.Controls.Add(panelKeyboardTitle);
            panelKeyboard.Dock = DockStyle.Top;
            panelKeyboard.Location = new Point(10, 753);
            panelKeyboard.Margin = new Padding(0);
            panelKeyboard.Name = "panelKeyboard";
            panelKeyboard.Padding = new Padding(20);
            panelKeyboard.Size = new Size(810, 138);
            panelKeyboard.TabIndex = 3;
            // 
            // tableLayoutKeyboard
            // 
            tableLayoutKeyboard.AutoSize = true;
            tableLayoutKeyboard.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutKeyboard.ColumnCount = 3;
            tableLayoutKeyboard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            tableLayoutKeyboard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            tableLayoutKeyboard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            tableLayoutKeyboard.Controls.Add(buttonKeyboard, 0, 0);
            tableLayoutKeyboard.Controls.Add(panelColor, 0, 0);
            tableLayoutKeyboard.Controls.Add(comboKeyboard, 0, 0);
            tableLayoutKeyboard.Dock = DockStyle.Top;
            tableLayoutKeyboard.Location = new Point(20, 59);
            tableLayoutKeyboard.Margin = new Padding(8);
            tableLayoutKeyboard.Name = "tableLayoutKeyboard";
            tableLayoutKeyboard.RowCount = 1;
            tableLayoutKeyboard.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutKeyboard.Size = new Size(770, 59);
            tableLayoutKeyboard.TabIndex = 39;
            // 
            // buttonKeyboard
            // 
            buttonKeyboard.AccessibleName = "Extra Settings";
            buttonKeyboard.Activated = false;
            buttonKeyboard.BackColor = SystemColors.ControlLight;
            buttonKeyboard.BorderColor = Color.Transparent;
            buttonKeyboard.BorderRadius = 2;
            buttonKeyboard.Dock = DockStyle.Top;
            buttonKeyboard.FlatAppearance.BorderSize = 0;
            buttonKeyboard.FlatStyle = FlatStyle.Flat;
            buttonKeyboard.Image = Properties.Resources.icons8_settings_32;
            buttonKeyboard.ImageAlign = ContentAlignment.MiddleRight;
            buttonKeyboard.Location = new Point(516, 7);
            buttonKeyboard.Margin = new Padding(4, 7, 4, 7);
            buttonKeyboard.Name = "buttonKeyboard";
            buttonKeyboard.Secondary = true;
            buttonKeyboard.Size = new Size(250, 45);
            buttonKeyboard.TabIndex = 15;
            buttonKeyboard.Text = Properties.Strings.Extra;
            buttonKeyboard.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonKeyboard.UseVisualStyleBackColor = false;
            // 
            // panelColor
            // 
            panelColor.AutoSize = true;
            panelColor.Controls.Add(pictureColor2);
            panelColor.Controls.Add(pictureColor);
            panelColor.Controls.Add(buttonKeyboardColor);
            panelColor.Dock = DockStyle.Fill;
            panelColor.Location = new Point(260, 7);
            panelColor.Margin = new Padding(4, 7, 4, 7);
            panelColor.Name = "panelColor";
            panelColor.Size = new Size(248, 45);
            panelColor.TabIndex = 36;
            // 
            // pictureColor2
            // 
            pictureColor2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pictureColor2.Location = new Point(187, 13);
            pictureColor2.Margin = new Padding(8);
            pictureColor2.Name = "pictureColor2";
            pictureColor2.Size = new Size(20, 20);
            pictureColor2.TabIndex = 41;
            pictureColor2.TabStop = false;
            // 
            // pictureColor
            // 
            pictureColor.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pictureColor.Location = new Point(214, 13);
            pictureColor.Margin = new Padding(8);
            pictureColor.Name = "pictureColor";
            pictureColor.Size = new Size(20, 20);
            pictureColor.TabIndex = 40;
            pictureColor.TabStop = false;
            // 
            // buttonKeyboardColor
            // 
            buttonKeyboardColor.AccessibleName = "Keyboard Color";
            buttonKeyboardColor.Activated = false;
            buttonKeyboardColor.BackColor = SystemColors.ButtonHighlight;
            buttonKeyboardColor.BorderColor = Color.Transparent;
            buttonKeyboardColor.BorderRadius = 2;
            buttonKeyboardColor.Dock = DockStyle.Top;
            buttonKeyboardColor.FlatStyle = FlatStyle.Flat;
            buttonKeyboardColor.ForeColor = SystemColors.ControlText;
            buttonKeyboardColor.Location = new Point(0, 0);
            buttonKeyboardColor.Margin = new Padding(4, 7, 4, 7);
            buttonKeyboardColor.Name = "buttonKeyboardColor";
            buttonKeyboardColor.Secondary = false;
            buttonKeyboardColor.Size = new Size(248, 45);
            buttonKeyboardColor.TabIndex = 14;
            buttonKeyboardColor.Text = Properties.Strings.Color;
            buttonKeyboardColor.UseVisualStyleBackColor = false;
            // 
            // comboKeyboard
            // 
            comboKeyboard.AccessibleName = "Keyboard Backlight Mode";
            comboKeyboard.BorderColor = Color.White;
            comboKeyboard.ButtonColor = Color.FromArgb(255, 255, 255);
            comboKeyboard.Dock = DockStyle.Top;
            comboKeyboard.FlatStyle = FlatStyle.Flat;
            comboKeyboard.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            comboKeyboard.FormattingEnabled = true;
            comboKeyboard.ItemHeight = 32;
            comboKeyboard.Items.AddRange(new object[] { "Static", "Breathe", "Rainbow", "Strobe" });
            comboKeyboard.Location = new Point(4, 10);
            comboKeyboard.Margin = new Padding(4, 10, 4, 8);
            comboKeyboard.Name = "comboKeyboard";
            comboKeyboard.Size = new Size(248, 40);
            comboKeyboard.TabIndex = 13;
            // 
            // panelKeyboardTitle
            // 
            panelKeyboardTitle.Controls.Add(pictureKeyboard);
            panelKeyboardTitle.Controls.Add(labelKeyboard);
            panelKeyboardTitle.Dock = DockStyle.Top;
            panelKeyboardTitle.Location = new Point(20, 20);
            panelKeyboardTitle.Name = "panelKeyboardTitle";
            panelKeyboardTitle.Size = new Size(770, 39);
            panelKeyboardTitle.TabIndex = 40;
            // 
            // pictureKeyboard
            // 
            pictureKeyboard.BackgroundImage = Properties.Resources.icons8_keyboard_32__1_;
            pictureKeyboard.BackgroundImageLayout = ImageLayout.Zoom;
            pictureKeyboard.Location = new Point(6, 0);
            pictureKeyboard.Margin = new Padding(4);
            pictureKeyboard.Name = "pictureKeyboard";
            pictureKeyboard.Size = new Size(32, 32);
            pictureKeyboard.TabIndex = 35;
            pictureKeyboard.TabStop = false;
            // 
            // labelKeyboard
            // 
            labelKeyboard.AutoSize = true;
            labelKeyboard.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelKeyboard.Location = new Point(44, 0);
            labelKeyboard.Margin = new Padding(8, 0, 8, 0);
            labelKeyboard.Name = "labelKeyboard";
            labelKeyboard.Size = new Size(210, 32);
            labelKeyboard.TabIndex = 34;
            labelKeyboard.Text = "Laptop Keyboard";
            // 
            // labelVersion
            // 
            labelVersion.Cursor = Cursors.Hand;
            labelVersion.Font = new Font("Segoe UI", 9F, FontStyle.Underline, GraphicsUnit.Point);
            labelVersion.ForeColor = SystemColors.ControlDark;
            labelVersion.Location = new Point(28, 11);
            labelVersion.Margin = new Padding(8, 0, 8, 0);
            labelVersion.Name = "labelVersion";
            labelVersion.Size = new Size(300, 32);
            labelVersion.TabIndex = 37;
            labelVersion.Text = "v.0";
            // 
            // labelModel
            // 
            labelModel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelModel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            labelModel.ForeColor = SystemColors.ControlDark;
            labelModel.Location = new Point(380, 11);
            labelModel.Margin = new Padding(8, 0, 8, 0);
            labelModel.Name = "labelModel";
            labelModel.Size = new Size(400, 32);
            labelModel.TabIndex = 38;
            labelModel.TextAlign = ContentAlignment.TopRight;
            // 
            // panelVersion
            // 
            panelVersion.Controls.Add(labelVersion);
            panelVersion.Controls.Add(labelModel);
            panelVersion.Dock = DockStyle.Top;
            panelVersion.Location = new Point(10, 1187);
            panelVersion.Name = "panelVersion";
            panelVersion.Size = new Size(810, 57);
            panelVersion.TabIndex = 6;
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(192F, 192F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(830, 1373);
            Controls.Add(panelFooter);
            Controls.Add(panelVersion);
            Controls.Add(panelBattery);
            Controls.Add(panelMatrix);
            Controls.Add(panelKeyboard);
            Controls.Add(panelScreen);
            Controls.Add(panelGPU);
            Controls.Add(panelPerformance);
            Margin = new Padding(8, 4, 8, 4);
            MaximizeBox = false;
            MdiChildrenMinimizedAnchorBottom = false;
            MinimizeBox = false;
            MinimumSize = new Size(830, 71);
            Name = "SettingsForm";
            Padding = new Padding(10);
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "G-Helper";
            panelMatrix.ResumeLayout(false);
            panelMatrix.PerformLayout();
            tableLayoutMatrix.ResumeLayout(false);
            panelMatrixTitle.ResumeLayout(false);
            panelMatrixTitle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureMatrix).EndInit();
            panelBattery.ResumeLayout(false);
            panelBatteryTitle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBattery).EndInit();
            panelFooter.ResumeLayout(false);
            panelFooter.PerformLayout();
            panelPerformance.ResumeLayout(false);
            panelPerformance.PerformLayout();
            tablePerf.ResumeLayout(false);
            panelCPUTitle.ResumeLayout(false);
            panelCPUTitle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picturePerf).EndInit();
            panelGPU.ResumeLayout(false);
            panelGPU.PerformLayout();
            tableGPU.ResumeLayout(false);
            panelGPUTitle.ResumeLayout(false);
            panelGPUTitle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureGPU).EndInit();
            panelScreen.ResumeLayout(false);
            panelScreen.PerformLayout();
            tableScreen.ResumeLayout(false);
            panelScreenTitle.ResumeLayout(false);
            panelScreenTitle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureScreen).EndInit();
            panelKeyboard.ResumeLayout(false);
            panelKeyboard.PerformLayout();
            tableLayoutKeyboard.ResumeLayout(false);
            tableLayoutKeyboard.PerformLayout();
            panelColor.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureColor2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureColor).EndInit();
            panelKeyboardTitle.ResumeLayout(false);
            panelKeyboardTitle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureKeyboard).EndInit();
            panelVersion.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Panel panelMatrix;
        private Panel panelBattery;
        private Panel panelFooter;
        private RButton buttonQuit;
        private CheckBox checkStartup;
        private Panel panelPerformance;
        private TableLayoutPanel tablePerf;
        private RButton buttonTurbo;
        private RButton buttonBalanced;
        private RButton buttonSilent;
        private Panel panelGPU;
        private TableLayoutPanel tableGPU;
        private RButton buttonXGM;
        private RButton buttonUltimate;
        private RButton buttonStandard;
        private RButton buttonEco;
        private Panel panelScreen;
        private TableLayoutPanel tableScreen;
        private RButton buttonScreenAuto;
        private RButton button60Hz;
        private Panel panelKeyboard;
        private TableLayoutPanel tableLayoutMatrix;
        private RComboBox comboMatrixRunning;
        private RComboBox comboMatrix;
        private TableLayoutPanel tableLayoutKeyboard;
        private RComboBox comboKeyboard;
        private Panel panelColor;
        private PictureBox pictureColor2;
        private PictureBox pictureColor;
        private CheckBox checkMatrix;
        private RButton button120Hz;
        private RButton buttonOptimized;
        private Label labelTipGPU;
        private Label labelTipScreen;
        private RButton buttonMiniled;
        private RButton buttonMatrix;
        private RButton buttonKeyboard;
        private RButton buttonKeyboardColor;
        private RButton buttonFans;
        private Slider sliderBattery;
        private RButton buttonUpdates;
        private Panel panelGPUTitle;
        private PictureBox pictureGPU;
        private Label labelGPU;
        private Label labelGPUFan;
        private Panel panelCPUTitle;
        private PictureBox picturePerf;
        private Label labelPerf;
        private Label labelCPUFan;
        private Panel panelScreenTitle;
        private Label labelMidFan;
        private PictureBox pictureScreen;
        private Label labelSreen;
        private Panel panelKeyboardTitle;
        private PictureBox pictureKeyboard;
        private Label labelKeyboard;
        private Panel panelMatrixTitle;
        private PictureBox pictureMatrix;
        private Label labelMatrix;
        private Panel panelBatteryTitle;
        private Label labelBattery;
        private PictureBox pictureBattery;
        private Label labelBatteryTitle;
        private Panel panelVersion;
        private Label labelVersion;
        private Label labelModel;
        private RButton buttonStopGPU;
    }
}