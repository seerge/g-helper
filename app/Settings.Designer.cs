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
            buttonBatteryFull = new RButton();
            sliderBattery = new Slider();
            panelBatteryTitle = new Panel();
            labelBattery = new Label();
            pictureBattery = new PictureBox();
            labelBatteryTitle = new Label();
            panelFooter = new Panel();
            tableButtons = new TableLayoutPanel();
            labelVersion = new Label();
            buttonQuit = new RButton();
            buttonUpdates = new RButton();
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
            pictureColor4 = new PictureBox();
            pictureColor3 = new PictureBox();
            pictureColor2 = new PictureBox();
            pictureColor = new PictureBox();
            buttonKeyboardColor = new RButton();
            comboKeyboard = new RComboBox();
            panelKeyboardTitle = new Panel();
            buttonFnLock = new RButton();
            pictureKeyboard = new PictureBox();
            labelKeyboard = new Label();
            panelVersion = new Panel();
            labelCharge = new Label();
            panelPeripherals = new Panel();
            tableLayoutPeripherals = new TableLayoutPanel();
            buttonPeripheral3 = new RButton();
            buttonPeripheral2 = new RButton();
            buttonPeripheral1 = new RButton();
            panelPeripheralsTile = new Panel();
            picturePeripherals = new PictureBox();
            labelPeripherals = new Label();
            panelMatrix.SuspendLayout();
            tableLayoutMatrix.SuspendLayout();
            panelMatrixTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureMatrix).BeginInit();
            panelBattery.SuspendLayout();
            panelBatteryTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBattery).BeginInit();
            panelFooter.SuspendLayout();
            tableButtons.SuspendLayout();
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
            ((System.ComponentModel.ISupportInitialize)pictureColor4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureColor3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureColor2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureColor).BeginInit();
            panelKeyboardTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureKeyboard).BeginInit();
            panelVersion.SuspendLayout();
            panelPeripherals.SuspendLayout();
            tableLayoutPeripherals.SuspendLayout();
            panelPeripheralsTile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picturePeripherals).BeginInit();
            SuspendLayout();
            // 
            // panelMatrix
            // 
            panelMatrix.AccessibleName = "Anime Matrix";
            panelMatrix.AccessibleRole = AccessibleRole.Grouping;
            panelMatrix.AutoSize = true;
            panelMatrix.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelMatrix.Controls.Add(tableLayoutMatrix);
            panelMatrix.Controls.Add(panelMatrixTitle);
            panelMatrix.Controls.Add(checkMatrix);
            panelMatrix.Dock = DockStyle.Top;
            panelMatrix.Location = new Point(7, 464);
            panelMatrix.Margin = new Padding(0);
            panelMatrix.Name = "panelMatrix";
            panelMatrix.Padding = new Padding(12, 12, 12, 6);
            panelMatrix.Size = new Size(517, 111);
            panelMatrix.TabIndex = 3;
            panelMatrix.TabStop = true;
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
            tableLayoutMatrix.Location = new Point(12, 37);
            tableLayoutMatrix.Margin = new Padding(5);
            tableLayoutMatrix.Name = "tableLayoutMatrix";
            tableLayoutMatrix.RowCount = 1;
            tableLayoutMatrix.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutMatrix.Size = new Size(493, 40);
            tableLayoutMatrix.TabIndex = 43;
            // 
            // comboMatrix
            // 
            comboMatrix.BorderColor = Color.White;
            comboMatrix.ButtonColor = Color.FromArgb(255, 255, 255);
            comboMatrix.Dock = DockStyle.Top;
            comboMatrix.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            comboMatrix.FormattingEnabled = true;
            comboMatrix.ItemHeight = 20;
            comboMatrix.Items.AddRange(new object[] { Properties.Strings.MatrixOff, Properties.Strings.MatrixDim, Properties.Strings.MatrixMedium, Properties.Strings.MatrixBright });
            comboMatrix.Location = new Point(2, 7);
            comboMatrix.Margin = new Padding(2, 7, 2, 5);
            comboMatrix.Name = "comboMatrix";
            comboMatrix.Size = new Size(160, 28);
            comboMatrix.TabIndex = 16;
            // 
            // comboMatrixRunning
            // 
            comboMatrixRunning.BorderColor = Color.White;
            comboMatrixRunning.ButtonColor = Color.FromArgb(255, 255, 255);
            comboMatrixRunning.Dock = DockStyle.Top;
            comboMatrixRunning.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            comboMatrixRunning.FormattingEnabled = true;
            comboMatrixRunning.ItemHeight = 20;
            comboMatrixRunning.Items.AddRange(new object[] { Properties.Strings.MatrixBanner, Properties.Strings.MatrixLogo, Properties.Strings.MatrixPicture, Properties.Strings.MatrixClock, Properties.Strings.MatrixAudio });
            comboMatrixRunning.Location = new Point(166, 7);
            comboMatrixRunning.Margin = new Padding(2, 7, 2, 5);
            comboMatrixRunning.Name = "comboMatrixRunning";
            comboMatrixRunning.Size = new Size(160, 28);
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
            buttonMatrix.Location = new Point(330, 5);
            buttonMatrix.Margin = new Padding(2, 5, 2, 5);
            buttonMatrix.Name = "buttonMatrix";
            buttonMatrix.Secondary = true;
            buttonMatrix.Size = new Size(161, 30);
            buttonMatrix.TabIndex = 18;
            buttonMatrix.Text = Properties.Strings.PictureGif;
            buttonMatrix.UseVisualStyleBackColor = false;
            // 
            // panelMatrixTitle
            // 
            panelMatrixTitle.Controls.Add(pictureMatrix);
            panelMatrixTitle.Controls.Add(labelMatrix);
            panelMatrixTitle.Dock = DockStyle.Top;
            panelMatrixTitle.Location = new Point(12, 12);
            panelMatrixTitle.Margin = new Padding(2);
            panelMatrixTitle.Name = "panelMatrixTitle";
            panelMatrixTitle.Size = new Size(493, 25);
            panelMatrixTitle.TabIndex = 45;
            // 
            // pictureMatrix
            // 
            pictureMatrix.BackgroundImage = Properties.Resources.icons8_matrix_32;
            pictureMatrix.BackgroundImageLayout = ImageLayout.Zoom;
            pictureMatrix.Location = new Point(3, 0);
            pictureMatrix.Margin = new Padding(2);
            pictureMatrix.Name = "pictureMatrix";
            pictureMatrix.Size = new Size(20, 20);
            pictureMatrix.TabIndex = 41;
            pictureMatrix.TabStop = false;
            // 
            // labelMatrix
            // 
            labelMatrix.AutoSize = true;
            labelMatrix.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelMatrix.Location = new Point(28, 0);
            labelMatrix.Margin = new Padding(5, 0, 5, 0);
            labelMatrix.Name = "labelMatrix";
            labelMatrix.Size = new Size(105, 20);
            labelMatrix.TabIndex = 40;
            labelMatrix.Text = "Anime Matrix";
            // 
            // checkMatrix
            // 
            checkMatrix.AutoSize = true;
            checkMatrix.ForeColor = SystemColors.GrayText;
            checkMatrix.Location = new Point(17, 79);
            checkMatrix.Margin = new Padding(5, 2, 5, 2);
            checkMatrix.Name = "checkMatrix";
            checkMatrix.Size = new Size(155, 24);
            checkMatrix.TabIndex = 19;
            checkMatrix.Text = Properties.Strings.TurnOffOnBattery;
            checkMatrix.UseVisualStyleBackColor = true;
            // 
            // panelBattery
            // 
            panelBattery.AutoSize = true;
            panelBattery.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelBattery.Controls.Add(buttonBatteryFull);
            panelBattery.Controls.Add(sliderBattery);
            panelBattery.Controls.Add(panelBatteryTitle);
            panelBattery.Dock = DockStyle.Top;
            panelBattery.Location = new Point(7, 786);
            panelBattery.Margin = new Padding(0);
            panelBattery.Name = "panelBattery";
            panelBattery.Padding = new Padding(12, 12, 12, 6);
            panelBattery.Size = new Size(517, 74);
            panelBattery.TabIndex = 5;
            // 
            // buttonBatteryFull
            // 
            buttonBatteryFull.Activated = false;
            buttonBatteryFull.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonBatteryFull.BackColor = SystemColors.ControlLight;
            buttonBatteryFull.BorderColor = Color.Transparent;
            buttonBatteryFull.BorderRadius = 2;
            buttonBatteryFull.FlatAppearance.BorderSize = 0;
            buttonBatteryFull.FlatStyle = FlatStyle.Flat;
            buttonBatteryFull.Font = new Font("Segoe UI", 7.125F, FontStyle.Bold, GraphicsUnit.Point);
            buttonBatteryFull.ForeColor = SystemColors.ControlDark;
            buttonBatteryFull.Location = new Point(456, 41);
            buttonBatteryFull.Margin = new Padding(0);
            buttonBatteryFull.Name = "buttonBatteryFull";
            buttonBatteryFull.Secondary = true;
            buttonBatteryFull.Size = new Size(46, 22);
            buttonBatteryFull.TabIndex = 41;
            buttonBatteryFull.Text = "100%";
            buttonBatteryFull.UseVisualStyleBackColor = false;
            // 
            // sliderBattery
            // 
            sliderBattery.AccessibleName = "Battery Charge Limit";
            sliderBattery.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            sliderBattery.Location = new Point(12, 41);
            sliderBattery.Margin = new Padding(2);
            sliderBattery.Max = 100;
            sliderBattery.Min = 40;
            sliderBattery.Name = "sliderBattery";
            sliderBattery.Size = new Size(441, 25);
            sliderBattery.Step = 5;
            sliderBattery.TabIndex = 20;
            sliderBattery.Text = "sliderBattery";
            sliderBattery.Value = 80;
            // 
            // panelBatteryTitle
            // 
            panelBatteryTitle.Controls.Add(labelBattery);
            panelBatteryTitle.Controls.Add(pictureBattery);
            panelBatteryTitle.Controls.Add(labelBatteryTitle);
            panelBatteryTitle.Dock = DockStyle.Top;
            panelBatteryTitle.Location = new Point(12, 12);
            panelBatteryTitle.Margin = new Padding(2);
            panelBatteryTitle.Name = "panelBatteryTitle";
            panelBatteryTitle.Padding = new Padding(0, 0, 0, 2);
            panelBatteryTitle.Size = new Size(493, 28);
            panelBatteryTitle.TabIndex = 40;
            // 
            // labelBattery
            // 
            labelBattery.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelBattery.Location = new Point(281, 0);
            labelBattery.Margin = new Padding(5, 0, 5, 0);
            labelBattery.Name = "labelBattery";
            labelBattery.Size = new Size(202, 22);
            labelBattery.TabIndex = 39;
            labelBattery.Text = "                ";
            labelBattery.TextAlign = ContentAlignment.TopRight;
            // 
            // pictureBattery
            // 
            pictureBattery.BackgroundImage = Properties.Resources.icons8_charging_battery_32;
            pictureBattery.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBattery.Location = new Point(2, 1);
            pictureBattery.Margin = new Padding(2);
            pictureBattery.Name = "pictureBattery";
            pictureBattery.Size = new Size(20, 20);
            pictureBattery.TabIndex = 38;
            pictureBattery.TabStop = false;
            // 
            // labelBatteryTitle
            // 
            labelBatteryTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelBatteryTitle.Location = new Point(27, 0);
            labelBatteryTitle.Margin = new Padding(5, 0, 5, 0);
            labelBatteryTitle.Name = "labelBatteryTitle";
            labelBatteryTitle.Size = new Size(292, 20);
            labelBatteryTitle.TabIndex = 37;
            labelBatteryTitle.Text = "Battery Charge Limit";
            // 
            // panelFooter
            // 
            panelFooter.AutoSize = true;
            panelFooter.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelFooter.Controls.Add(tableButtons);
            panelFooter.Dock = DockStyle.Top;
            panelFooter.Location = new Point(7, 895);
            panelFooter.Margin = new Padding(0);
            panelFooter.Name = "panelFooter";
            panelFooter.Padding = new Padding(12);
            panelFooter.Size = new Size(517, 62);
            panelFooter.TabIndex = 7;
            // 
            // tableButtons
            // 
            tableButtons.AutoSize = true;
            tableButtons.ColumnCount = 3;
            tableButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 17F));
            tableButtons.Controls.Add(labelVersion, 0, 0);
            tableButtons.Controls.Add(buttonQuit, 2, 0);
            tableButtons.Controls.Add(buttonUpdates, 1, 0);
            tableButtons.Dock = DockStyle.Top;
            tableButtons.Location = new Point(12, 12);
            tableButtons.Margin = new Padding(2);
            tableButtons.Name = "tableButtons";
            tableButtons.RowCount = 1;
            tableButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableButtons.Size = new Size(493, 38);
            tableButtons.TabIndex = 25;
            // 
            // labelVersion
            // 
            labelVersion.Cursor = Cursors.Hand;
            labelVersion.Dock = DockStyle.Fill;
            labelVersion.Font = new Font("Segoe UI", 9F, FontStyle.Underline, GraphicsUnit.Point);
            labelVersion.ForeColor = SystemColors.ControlDark;
            labelVersion.Location = new Point(0, 0);
            labelVersion.Margin = new Padding(0);
            labelVersion.Name = "labelVersion";
            labelVersion.Padding = new Padding(3);
            labelVersion.Size = new Size(164, 38);
            labelVersion.TabIndex = 37;
            labelVersion.Text = "v.0";
            labelVersion.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // buttonQuit
            // 
            buttonQuit.AccessibleName = "Quit Application";
            buttonQuit.Activated = false;
            buttonQuit.BackColor = SystemColors.ControlLight;
            buttonQuit.BorderColor = Color.Transparent;
            buttonQuit.BorderRadius = 2;
            buttonQuit.Dock = DockStyle.Top;
            buttonQuit.FlatStyle = FlatStyle.Flat;
            buttonQuit.Image = Properties.Resources.icons8_quit_32;
            buttonQuit.Location = new Point(330, 4);
            buttonQuit.Margin = new Padding(2, 4, 2, 4);
            buttonQuit.Name = "buttonQuit";
            buttonQuit.Secondary = true;
            buttonQuit.Size = new Size(161, 30);
            buttonQuit.TabIndex = 2;
            buttonQuit.Text = "&Quit";
            buttonQuit.TextAlign = ContentAlignment.MiddleRight;
            buttonQuit.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonQuit.UseVisualStyleBackColor = false;
            // 
            // buttonUpdates
            // 
            buttonUpdates.AccessibleName = "BIOS and Driver Updates";
            buttonUpdates.Activated = false;
            buttonUpdates.BackColor = SystemColors.ControlLight;
            buttonUpdates.BorderColor = Color.Transparent;
            buttonUpdates.BorderRadius = 2;
            buttonUpdates.Dock = DockStyle.Top;
            buttonUpdates.FlatStyle = FlatStyle.Flat;
            buttonUpdates.Image = Properties.Resources.icons8_software_32_white;
            buttonUpdates.ImageAlign = ContentAlignment.MiddleRight;
            buttonUpdates.Location = new Point(166, 4);
            buttonUpdates.Margin = new Padding(2, 4, 2, 4);
            buttonUpdates.Name = "buttonUpdates";
            buttonUpdates.Secondary = true;
            buttonUpdates.Size = new Size(160, 30);
            buttonUpdates.TabIndex = 1;
            buttonUpdates.Text = "&Updates";
            buttonUpdates.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonUpdates.UseVisualStyleBackColor = false;
            // 
            // checkStartup
            // 
            checkStartup.AutoSize = true;
            checkStartup.Location = new Point(15, 7);
            checkStartup.Margin = new Padding(7, 3, 7, 3);
            checkStartup.Name = "checkStartup";
            checkStartup.Size = new Size(129, 24);
            checkStartup.TabIndex = 21;
            checkStartup.Text = Properties.Strings.RunOnStartup;
            checkStartup.UseVisualStyleBackColor = true;
            // 
            // panelPerformance
            // 
            panelPerformance.AccessibleName = "Performance";
            panelPerformance.AccessibleRole = AccessibleRole.Grouping;
            panelPerformance.AutoSize = true;
            panelPerformance.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelPerformance.Controls.Add(tablePerf);
            panelPerformance.Controls.Add(panelCPUTitle);
            panelPerformance.Dock = DockStyle.Top;
            panelPerformance.Location = new Point(7, 7);
            panelPerformance.Margin = new Padding(0);
            panelPerformance.Name = "panelPerformance";
            panelPerformance.Padding = new Padding(12);
            panelPerformance.Size = new Size(517, 129);
            panelPerformance.TabIndex = 0;
            panelPerformance.TabStop = true;
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
            tablePerf.Location = new Point(12, 37);
            tablePerf.Margin = new Padding(5, 2, 5, 2);
            tablePerf.Name = "tablePerf";
            tablePerf.RowCount = 1;
            tablePerf.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));
            tablePerf.Size = new Size(493, 80);
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
            buttonSilent.Location = new Point(2, 2);
            buttonSilent.Margin = new Padding(2);
            buttonSilent.Name = "buttonSilent";
            buttonSilent.Secondary = false;
            buttonSilent.Size = new Size(119, 76);
            buttonSilent.TabIndex = 1;
            buttonSilent.Text = "&Silent";
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
            buttonBalanced.Location = new Point(125, 2);
            buttonBalanced.Margin = new Padding(2);
            buttonBalanced.Name = "buttonBalanced";
            buttonBalanced.Secondary = false;
            buttonBalanced.Size = new Size(119, 76);
            buttonBalanced.TabIndex = 1;
            buttonBalanced.Text = "&Balanced";
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
            buttonTurbo.Location = new Point(248, 2);
            buttonTurbo.Margin = new Padding(2);
            buttonTurbo.Name = "buttonTurbo";
            buttonTurbo.Secondary = false;
            buttonTurbo.Size = new Size(119, 76);
            buttonTurbo.TabIndex = 2;
            buttonTurbo.Text = "&Turbo";
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
            buttonFans.Location = new Point(371, 2);
            buttonFans.Margin = new Padding(2);
            buttonFans.Name = "buttonFans";
            buttonFans.Secondary = true;
            buttonFans.Size = new Size(120, 76);
            buttonFans.TabIndex = 3;
            buttonFans.Text = "&Fans + Power";
            buttonFans.TextImageRelation = TextImageRelation.ImageAboveText;
            buttonFans.UseVisualStyleBackColor = false;
            // 
            // panelCPUTitle
            // 
            panelCPUTitle.Controls.Add(picturePerf);
            panelCPUTitle.Controls.Add(labelPerf);
            panelCPUTitle.Controls.Add(labelCPUFan);
            panelCPUTitle.Dock = DockStyle.Top;
            panelCPUTitle.Location = new Point(12, 12);
            panelCPUTitle.Margin = new Padding(2);
            panelCPUTitle.Name = "panelCPUTitle";
            panelCPUTitle.Size = new Size(493, 25);
            panelCPUTitle.TabIndex = 30;
            // 
            // picturePerf
            // 
            picturePerf.BackgroundImage = Properties.Resources.icons8_gauge_32;
            picturePerf.BackgroundImageLayout = ImageLayout.Zoom;
            picturePerf.InitialImage = null;
            picturePerf.Location = new Point(5, 0);
            picturePerf.Margin = new Padding(2);
            picturePerf.Name = "picturePerf";
            picturePerf.Size = new Size(20, 20);
            picturePerf.TabIndex = 35;
            picturePerf.TabStop = false;
            // 
            // labelPerf
            // 
            labelPerf.AccessibleRole = AccessibleRole.Caret;
            labelPerf.AutoSize = true;
            labelPerf.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelPerf.LiveSetting = System.Windows.Forms.Automation.AutomationLiveSetting.Polite;
            labelPerf.Location = new Point(25, 0);
            labelPerf.Margin = new Padding(5, 0, 5, 0);
            labelPerf.Name = "labelPerf";
            labelPerf.Size = new Size(143, 20);
            labelPerf.TabIndex = 0;
            labelPerf.Text = "Performance Mode";
            // 
            // labelCPUFan
            // 
            labelCPUFan.AccessibleRole = AccessibleRole.TitleBar;
            labelCPUFan.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelCPUFan.Cursor = Cursors.Hand;
            labelCPUFan.Location = new Point(243, 0);
            labelCPUFan.Margin = new Padding(5, 0, 5, 0);
            labelCPUFan.Name = "labelCPUFan";
            labelCPUFan.Size = new Size(250, 22);
            labelCPUFan.TabIndex = 33;
            labelCPUFan.Text = "      ";
            labelCPUFan.TextAlign = ContentAlignment.TopRight;
            // 
            // panelGPU
            // 
            panelGPU.AccessibleName = "GPU";
            panelGPU.AccessibleRole = AccessibleRole.Grouping;
            panelGPU.AutoSize = true;
            panelGPU.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelGPU.Controls.Add(labelTipGPU);
            panelGPU.Controls.Add(tableGPU);
            panelGPU.Controls.Add(panelGPUTitle);
            panelGPU.Dock = DockStyle.Top;
            panelGPU.Location = new Point(7, 136);
            panelGPU.Margin = new Padding(0);
            panelGPU.Name = "panelGPU";
            panelGPU.Padding = new Padding(12, 12, 12, 0);
            panelGPU.Size = new Size(517, 219);
            panelGPU.TabIndex = 1;
            panelGPU.TabStop = true;
            // 
            // labelTipGPU
            // 
            labelTipGPU.Dock = DockStyle.Top;
            labelTipGPU.ForeColor = SystemColors.GrayText;
            labelTipGPU.Location = new Point(12, 197);
            labelTipGPU.Margin = new Padding(2, 0, 2, 0);
            labelTipGPU.Name = "labelTipGPU";
            labelTipGPU.Size = new Size(493, 22);
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
            tableGPU.Location = new Point(12, 37);
            tableGPU.Margin = new Padding(5, 2, 5, 2);
            tableGPU.Name = "tableGPU";
            tableGPU.RowCount = 1;
            tableGPU.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));
            tableGPU.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));
            tableGPU.Size = new Size(493, 160);
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
            buttonStopGPU.Location = new Point(125, 2);
            buttonStopGPU.Margin = new Padding(2);
            buttonStopGPU.Name = "buttonStopGPU";
            buttonStopGPU.Secondary = false;
            buttonStopGPU.Size = new Size(119, 75);
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
            buttonEco.Location = new Point(2, 2);
            buttonEco.Margin = new Padding(2);
            buttonEco.Name = "buttonEco";
            buttonEco.Secondary = false;
            buttonEco.Size = new Size(119, 75);
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
            buttonStandard.Location = new Point(248, 2);
            buttonStandard.Margin = new Padding(2);
            buttonStandard.Name = "buttonStandard";
            buttonStandard.Secondary = false;
            buttonStandard.Size = new Size(119, 75);
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
            buttonXGM.Location = new Point(125, 82);
            buttonXGM.Margin = new Padding(2);
            buttonXGM.Name = "buttonXGM";
            buttonXGM.Secondary = false;
            buttonXGM.Size = new Size(119, 75);
            buttonXGM.TabIndex = 8;
            buttonXGM.Text = "XG Mobile";
            buttonXGM.TextImageRelation = TextImageRelation.ImageAboveText;
            buttonXGM.UseVisualStyleBackColor = false;
            buttonXGM.Visible = false;
            // 
            // buttonOptimized
            // 
            buttonOptimized.AccessibleName = "Optimized GPU Mode";
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
            buttonOptimized.Location = new Point(2, 82);
            buttonOptimized.Margin = new Padding(2);
            buttonOptimized.Name = "buttonOptimized";
            buttonOptimized.Secondary = false;
            buttonOptimized.Size = new Size(119, 75);
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
            buttonUltimate.Location = new Point(371, 2);
            buttonUltimate.Margin = new Padding(2);
            buttonUltimate.Name = "buttonUltimate";
            buttonUltimate.Secondary = false;
            buttonUltimate.Size = new Size(120, 75);
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
            panelGPUTitle.Location = new Point(12, 12);
            panelGPUTitle.Margin = new Padding(2);
            panelGPUTitle.Name = "panelGPUTitle";
            panelGPUTitle.Size = new Size(493, 25);
            panelGPUTitle.TabIndex = 21;
            // 
            // pictureGPU
            // 
            pictureGPU.BackgroundImage = Properties.Resources.icons8_video_card_32;
            pictureGPU.BackgroundImageLayout = ImageLayout.Zoom;
            pictureGPU.Location = new Point(5, 0);
            pictureGPU.Margin = new Padding(2);
            pictureGPU.Name = "pictureGPU";
            pictureGPU.Size = new Size(20, 20);
            pictureGPU.TabIndex = 22;
            pictureGPU.TabStop = false;
            // 
            // labelGPU
            // 
            labelGPU.AutoSize = true;
            labelGPU.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelGPU.Location = new Point(25, 0);
            labelGPU.Margin = new Padding(5, 0, 5, 0);
            labelGPU.Name = "labelGPU";
            labelGPU.Size = new Size(84, 20);
            labelGPU.TabIndex = 21;
            labelGPU.Text = "GPU Mode";
            // 
            // labelGPUFan
            // 
            labelGPUFan.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelGPUFan.Location = new Point(243, 0);
            labelGPUFan.Margin = new Padding(5, 0, 5, 0);
            labelGPUFan.Name = "labelGPUFan";
            labelGPUFan.Size = new Size(250, 22);
            labelGPUFan.TabIndex = 20;
            labelGPUFan.Text = "         ";
            labelGPUFan.TextAlign = ContentAlignment.TopRight;
            // 
            // panelScreen
            // 
            panelScreen.AccessibleName = "Screen";
            panelScreen.AccessibleRole = AccessibleRole.Grouping;
            panelScreen.AutoSize = true;
            panelScreen.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelScreen.Controls.Add(labelTipScreen);
            panelScreen.Controls.Add(tableScreen);
            panelScreen.Controls.Add(panelScreenTitle);
            panelScreen.Dock = DockStyle.Top;
            panelScreen.Location = new Point(7, 355);
            panelScreen.Margin = new Padding(0);
            panelScreen.Name = "panelScreen";
            panelScreen.Padding = new Padding(12, 12, 12, 0);
            panelScreen.Size = new Size(517, 109);
            panelScreen.TabIndex = 2;
            panelScreen.TabStop = true;
            // 
            // labelTipScreen
            // 
            labelTipScreen.Dock = DockStyle.Top;
            labelTipScreen.ForeColor = SystemColors.GrayText;
            labelTipScreen.Location = new Point(12, 87);
            labelTipScreen.Margin = new Padding(2, 0, 2, 0);
            labelTipScreen.Name = "labelTipScreen";
            labelTipScreen.Size = new Size(493, 22);
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
            tableScreen.Location = new Point(12, 37);
            tableScreen.Margin = new Padding(5, 2, 5, 2);
            tableScreen.Name = "tableScreen";
            tableScreen.RowCount = 1;
            tableScreen.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableScreen.Size = new Size(493, 50);
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
            buttonScreenAuto.Location = new Point(2, 2);
            buttonScreenAuto.Margin = new Padding(2);
            buttonScreenAuto.Name = "buttonScreenAuto";
            buttonScreenAuto.Secondary = false;
            buttonScreenAuto.Size = new Size(119, 46);
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
            button60Hz.Location = new Point(125, 2);
            button60Hz.Margin = new Padding(2);
            button60Hz.Name = "button60Hz";
            button60Hz.Secondary = false;
            button60Hz.Size = new Size(119, 46);
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
            button120Hz.Location = new Point(248, 2);
            button120Hz.Margin = new Padding(2);
            button120Hz.Name = "button120Hz";
            button120Hz.Secondary = false;
            button120Hz.Size = new Size(119, 46);
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
            buttonMiniled.Location = new Point(371, 2);
            buttonMiniled.Margin = new Padding(2);
            buttonMiniled.Name = "buttonMiniled";
            buttonMiniled.Secondary = false;
            buttonMiniled.Size = new Size(120, 46);
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
            panelScreenTitle.Location = new Point(12, 12);
            panelScreenTitle.Margin = new Padding(2);
            panelScreenTitle.Name = "panelScreenTitle";
            panelScreenTitle.Size = new Size(493, 25);
            panelScreenTitle.TabIndex = 25;
            // 
            // labelMidFan
            // 
            labelMidFan.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelMidFan.Location = new Point(333, -2);
            labelMidFan.Margin = new Padding(5, 0, 5, 0);
            labelMidFan.Name = "labelMidFan";
            labelMidFan.Size = new Size(158, 22);
            labelMidFan.TabIndex = 28;
            labelMidFan.Text = "         ";
            labelMidFan.TextAlign = ContentAlignment.TopRight;
            // 
            // pictureScreen
            // 
            pictureScreen.BackgroundImage = Properties.Resources.icons8_laptop_32;
            pictureScreen.BackgroundImageLayout = ImageLayout.Zoom;
            pictureScreen.Location = new Point(3, 0);
            pictureScreen.Margin = new Padding(2);
            pictureScreen.Name = "pictureScreen";
            pictureScreen.Size = new Size(20, 20);
            pictureScreen.TabIndex = 27;
            pictureScreen.TabStop = false;
            // 
            // labelSreen
            // 
            labelSreen.AutoSize = true;
            labelSreen.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelSreen.Location = new Point(25, 0);
            labelSreen.Margin = new Padding(5, 0, 5, 0);
            labelSreen.Name = "labelSreen";
            labelSreen.Size = new Size(108, 20);
            labelSreen.TabIndex = 26;
            labelSreen.Text = "Laptop Screen";
            // 
            // panelKeyboard
            // 
            panelKeyboard.AccessibleName = "Keyboard";
            panelKeyboard.AccessibleRole = AccessibleRole.Grouping;
            panelKeyboard.AutoSize = true;
            panelKeyboard.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelKeyboard.Controls.Add(tableLayoutKeyboard);
            panelKeyboard.Controls.Add(panelKeyboardTitle);
            panelKeyboard.Dock = DockStyle.Top;
            panelKeyboard.Location = new Point(7, 575);
            panelKeyboard.Margin = new Padding(0);
            panelKeyboard.Name = "panelKeyboard";
            panelKeyboard.Padding = new Padding(12);
            panelKeyboard.Size = new Size(517, 89);
            panelKeyboard.TabIndex = 4;
            panelKeyboard.TabStop = true;
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
            tableLayoutKeyboard.Location = new Point(12, 37);
            tableLayoutKeyboard.Margin = new Padding(5);
            tableLayoutKeyboard.Name = "tableLayoutKeyboard";
            tableLayoutKeyboard.RowCount = 1;
            tableLayoutKeyboard.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutKeyboard.Size = new Size(493, 40);
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
            buttonKeyboard.Location = new Point(330, 5);
            buttonKeyboard.Margin = new Padding(2, 5, 2, 5);
            buttonKeyboard.Name = "buttonKeyboard";
            buttonKeyboard.Secondary = true;
            buttonKeyboard.Size = new Size(161, 30);
            buttonKeyboard.TabIndex = 37;
            buttonKeyboard.Text = "&Extra";
            buttonKeyboard.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonKeyboard.UseVisualStyleBackColor = false;
            // 
            // panelColor
            // 
            panelColor.AutoSize = true;
            panelColor.Controls.Add(pictureColor4);
            panelColor.Controls.Add(pictureColor3);
            panelColor.Controls.Add(pictureColor2);
            panelColor.Controls.Add(pictureColor);
            panelColor.Controls.Add(buttonKeyboardColor);
            panelColor.Dock = DockStyle.Fill;
            panelColor.Location = new Point(166, 5);
            panelColor.Margin = new Padding(2, 5, 2, 5);
            panelColor.Name = "panelColor";
            panelColor.Size = new Size(160, 30);
            panelColor.TabIndex = 36;
            // 
            // pictureColor4
            // 
            pictureColor4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pictureColor4.Location = new Point(22, 9);
            pictureColor4.Margin = new Padding(5);
            pictureColor4.Name = "pictureColor4";
            pictureColor4.Size = new Size(12, 12);
            pictureColor4.TabIndex = 43;
            pictureColor4.TabStop = false;
            pictureColor4.Visible = false;
            // 
            // pictureColor3
            // 
            pictureColor3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pictureColor3.Location = new Point(42, 9);
            pictureColor3.Margin = new Padding(5);
            pictureColor3.Name = "pictureColor3";
            pictureColor3.Size = new Size(12, 12);
            pictureColor3.TabIndex = 42;
            pictureColor3.TabStop = false;
            pictureColor3.Visible = false;
            // 
            // pictureColor2
            // 
            pictureColor2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pictureColor2.Location = new Point(117, 9);
            pictureColor2.Margin = new Padding(5);
            pictureColor2.Name = "pictureColor2";
            pictureColor2.Size = new Size(12, 12);
            pictureColor2.TabIndex = 41;
            pictureColor2.TabStop = false;
            // 
            // pictureColor
            // 
            pictureColor.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pictureColor.Location = new Point(137, 9);
            pictureColor.Margin = new Padding(5);
            pictureColor.Name = "pictureColor";
            pictureColor.Size = new Size(12, 12);
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
            buttonKeyboardColor.Margin = new Padding(2, 5, 2, 5);
            buttonKeyboardColor.Name = "buttonKeyboardColor";
            buttonKeyboardColor.Secondary = false;
            buttonKeyboardColor.Size = new Size(160, 30);
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
            comboKeyboard.ItemHeight = 20;
            comboKeyboard.Items.AddRange(new object[] { "Static", "Breathe", "Rainbow", "Strobe" });
            comboKeyboard.Location = new Point(2, 7);
            comboKeyboard.Margin = new Padding(2, 7, 2, 5);
            comboKeyboard.Name = "comboKeyboard";
            comboKeyboard.Size = new Size(160, 28);
            comboKeyboard.TabIndex = 13;
            // 
            // panelKeyboardTitle
            // 
            panelKeyboardTitle.Controls.Add(buttonFnLock);
            panelKeyboardTitle.Controls.Add(pictureKeyboard);
            panelKeyboardTitle.Controls.Add(labelKeyboard);
            panelKeyboardTitle.Dock = DockStyle.Top;
            panelKeyboardTitle.Location = new Point(12, 12);
            panelKeyboardTitle.Margin = new Padding(0);
            panelKeyboardTitle.Name = "panelKeyboardTitle";
            panelKeyboardTitle.Padding = new Padding(0, 1, 2, 1);
            panelKeyboardTitle.Size = new Size(493, 25);
            panelKeyboardTitle.TabIndex = 40;
            // 
            // buttonFnLock
            // 
            buttonFnLock.Activated = false;
            buttonFnLock.BackColor = SystemColors.ControlLight;
            buttonFnLock.BorderColor = Color.Transparent;
            buttonFnLock.BorderRadius = 2;
            buttonFnLock.Dock = DockStyle.Right;
            buttonFnLock.FlatAppearance.BorderSize = 0;
            buttonFnLock.FlatStyle = FlatStyle.Flat;
            buttonFnLock.Font = new Font("Segoe UI", 7.125F, FontStyle.Bold, GraphicsUnit.Point);
            buttonFnLock.ForeColor = SystemColors.ControlDark;
            buttonFnLock.Location = new Point(424, 1);
            buttonFnLock.Margin = new Padding(0);
            buttonFnLock.Name = "buttonFnLock";
            buttonFnLock.Secondary = true;
            buttonFnLock.Size = new Size(67, 23);
            buttonFnLock.TabIndex = 4;
            buttonFnLock.Text = "FN-Lock";
            buttonFnLock.UseVisualStyleBackColor = false;
            // 
            // pictureKeyboard
            // 
            pictureKeyboard.BackgroundImage = Properties.Resources.icons8_keyboard_32__1_;
            pictureKeyboard.BackgroundImageLayout = ImageLayout.Zoom;
            pictureKeyboard.Location = new Point(3, 0);
            pictureKeyboard.Margin = new Padding(2);
            pictureKeyboard.Name = "pictureKeyboard";
            pictureKeyboard.Size = new Size(20, 20);
            pictureKeyboard.TabIndex = 35;
            pictureKeyboard.TabStop = false;
            // 
            // labelKeyboard
            // 
            labelKeyboard.AutoSize = true;
            labelKeyboard.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelKeyboard.Location = new Point(25, -1);
            labelKeyboard.Margin = new Padding(5, 0, 5, 0);
            labelKeyboard.Name = "labelKeyboard";
            labelKeyboard.Size = new Size(129, 20);
            labelKeyboard.TabIndex = 34;
            labelKeyboard.Text = "Laptop Keyboard";
            // 
            // panelVersion
            // 
            panelVersion.Controls.Add(labelCharge);
            panelVersion.Controls.Add(checkStartup);
            panelVersion.Dock = DockStyle.Top;
            panelVersion.Location = new Point(7, 860);
            panelVersion.Margin = new Padding(2);
            panelVersion.Name = "panelVersion";
            panelVersion.Size = new Size(517, 35);
            panelVersion.TabIndex = 6;
            // 
            // labelCharge
            // 
            labelCharge.Dock = DockStyle.Right;
            labelCharge.ForeColor = SystemColors.ControlDark;
            labelCharge.Location = new Point(228, 0);
            labelCharge.Margin = new Padding(0);
            labelCharge.Name = "labelCharge";
            labelCharge.Padding = new Padding(0, 0, 12, 0);
            labelCharge.Size = new Size(289, 35);
            labelCharge.TabIndex = 40;
            labelCharge.TextAlign = ContentAlignment.MiddleRight;
            // 
            // panelPeripherals
            // 
            panelPeripherals.AutoSize = true;
            panelPeripherals.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelPeripherals.Controls.Add(tableLayoutPeripherals);
            panelPeripherals.Controls.Add(panelPeripheralsTile);
            panelPeripherals.Dock = DockStyle.Top;
            panelPeripherals.Location = new Point(7, 664);
            panelPeripherals.Margin = new Padding(0);
            panelPeripherals.Name = "panelPeripherals";
            panelPeripherals.Padding = new Padding(12, 12, 12, 6);
            panelPeripherals.Size = new Size(517, 122);
            panelPeripherals.TabIndex = 4;
            panelPeripherals.Visible = false;
            // 
            // tableLayoutPeripherals
            // 
            tableLayoutPeripherals.AutoSize = true;
            tableLayoutPeripherals.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPeripherals.ColumnCount = 3;
            tableLayoutPeripherals.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPeripherals.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPeripherals.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPeripherals.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPeripherals.Controls.Add(buttonPeripheral3, 0, 0);
            tableLayoutPeripherals.Controls.Add(buttonPeripheral2, 0, 0);
            tableLayoutPeripherals.Controls.Add(buttonPeripheral1, 0, 0);
            tableLayoutPeripherals.Dock = DockStyle.Top;
            tableLayoutPeripherals.Location = new Point(12, 37);
            tableLayoutPeripherals.Margin = new Padding(5);
            tableLayoutPeripherals.Name = "tableLayoutPeripherals";
            tableLayoutPeripherals.RowCount = 1;
            tableLayoutPeripherals.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPeripherals.Size = new Size(493, 79);
            tableLayoutPeripherals.TabIndex = 43;
            // 
            // buttonPeripheral3
            // 
            buttonPeripheral3.AccessibleName = "Peripheral 3";
            buttonPeripheral3.Activated = false;
            buttonPeripheral3.BackColor = SystemColors.ControlLightLight;
            buttonPeripheral3.BorderColor = Color.Transparent;
            buttonPeripheral3.BorderRadius = 5;
            buttonPeripheral3.CausesValidation = false;
            buttonPeripheral3.Dock = DockStyle.Top;
            buttonPeripheral3.FlatAppearance.BorderSize = 0;
            buttonPeripheral3.FlatStyle = FlatStyle.Flat;
            buttonPeripheral3.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            buttonPeripheral3.ForeColor = SystemColors.ControlText;
            buttonPeripheral3.Image = Properties.Resources.icons8_maus_48;
            buttonPeripheral3.ImageAlign = ContentAlignment.MiddleLeft;
            buttonPeripheral3.Location = new Point(330, 2);
            buttonPeripheral3.Margin = new Padding(2);
            buttonPeripheral3.Name = "buttonPeripheral3";
            buttonPeripheral3.Secondary = false;
            buttonPeripheral3.Size = new Size(161, 75);
            buttonPeripheral3.TabIndex = 22;
            buttonPeripheral3.Text = "Mouse 3";
            buttonPeripheral3.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonPeripheral3.UseVisualStyleBackColor = false;
            // 
            // buttonPeripheral2
            // 
            buttonPeripheral2.AccessibleName = "Peripheral 2";
            buttonPeripheral2.Activated = false;
            buttonPeripheral2.BackColor = SystemColors.ControlLightLight;
            buttonPeripheral2.BorderColor = Color.Transparent;
            buttonPeripheral2.BorderRadius = 5;
            buttonPeripheral2.CausesValidation = false;
            buttonPeripheral2.Dock = DockStyle.Top;
            buttonPeripheral2.FlatAppearance.BorderSize = 0;
            buttonPeripheral2.FlatStyle = FlatStyle.Flat;
            buttonPeripheral2.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            buttonPeripheral2.ForeColor = SystemColors.ControlText;
            buttonPeripheral2.Image = Properties.Resources.icons8_maus_48;
            buttonPeripheral2.ImageAlign = ContentAlignment.MiddleLeft;
            buttonPeripheral2.Location = new Point(166, 2);
            buttonPeripheral2.Margin = new Padding(2);
            buttonPeripheral2.Name = "buttonPeripheral2";
            buttonPeripheral2.Secondary = false;
            buttonPeripheral2.Size = new Size(160, 75);
            buttonPeripheral2.TabIndex = 19;
            buttonPeripheral2.Text = "Mouse 2";
            buttonPeripheral2.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonPeripheral2.UseVisualStyleBackColor = false;
            // 
            // buttonPeripheral1
            // 
            buttonPeripheral1.AccessibleName = "Peripheral 1";
            buttonPeripheral1.Activated = false;
            buttonPeripheral1.BackColor = SystemColors.ControlLightLight;
            buttonPeripheral1.BorderColor = Color.Transparent;
            buttonPeripheral1.BorderRadius = 5;
            buttonPeripheral1.CausesValidation = false;
            buttonPeripheral1.Dock = DockStyle.Top;
            buttonPeripheral1.FlatAppearance.BorderSize = 0;
            buttonPeripheral1.FlatStyle = FlatStyle.Flat;
            buttonPeripheral1.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            buttonPeripheral1.ForeColor = SystemColors.ControlText;
            buttonPeripheral1.Image = Properties.Resources.icons8_maus_48;
            buttonPeripheral1.ImageAlign = ContentAlignment.MiddleLeft;
            buttonPeripheral1.Location = new Point(2, 2);
            buttonPeripheral1.Margin = new Padding(2);
            buttonPeripheral1.Name = "buttonPeripheral1";
            buttonPeripheral1.Secondary = false;
            buttonPeripheral1.Size = new Size(160, 75);
            buttonPeripheral1.TabIndex = 20;
            buttonPeripheral1.Text = "Mouse 1";
            buttonPeripheral1.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonPeripheral1.UseVisualStyleBackColor = false;
            // 
            // panelPeripheralsTile
            // 
            panelPeripheralsTile.Controls.Add(picturePeripherals);
            panelPeripheralsTile.Controls.Add(labelPeripherals);
            panelPeripheralsTile.Dock = DockStyle.Top;
            panelPeripheralsTile.Location = new Point(12, 12);
            panelPeripheralsTile.Margin = new Padding(2);
            panelPeripheralsTile.Name = "panelPeripheralsTile";
            panelPeripheralsTile.Size = new Size(493, 25);
            panelPeripheralsTile.TabIndex = 45;
            // 
            // picturePeripherals
            // 
            picturePeripherals.BackgroundImage = Properties.Resources.icons8_maus_48;
            picturePeripherals.BackgroundImageLayout = ImageLayout.Zoom;
            picturePeripherals.Location = new Point(3, 0);
            picturePeripherals.Margin = new Padding(2);
            picturePeripherals.Name = "picturePeripherals";
            picturePeripherals.Size = new Size(20, 20);
            picturePeripherals.TabIndex = 41;
            picturePeripherals.TabStop = false;
            // 
            // labelPeripherals
            // 
            labelPeripherals.AutoSize = true;
            labelPeripherals.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelPeripherals.Location = new Point(28, 0);
            labelPeripherals.Margin = new Padding(5, 0, 5, 0);
            labelPeripherals.Name = "labelPeripherals";
            labelPeripherals.Size = new Size(87, 20);
            labelPeripherals.TabIndex = 40;
            labelPeripherals.Text = "Peripherals";
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(531, 884);
            Controls.Add(panelFooter);
            Controls.Add(panelVersion);
            Controls.Add(panelBattery);
            Controls.Add(panelPeripherals);
            Controls.Add(panelKeyboard);
            Controls.Add(panelMatrix);
            Controls.Add(panelScreen);
            Controls.Add(panelGPU);
            Controls.Add(panelPerformance);
            Margin = new Padding(5, 2, 5, 2);
            MaximizeBox = false;
            MdiChildrenMinimizedAnchorBottom = false;
            MinimizeBox = false;
            MinimumSize = new Size(520, 62);
            Name = "SettingsForm";
            Padding = new Padding(7);
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
            tableButtons.ResumeLayout(false);
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
            ((System.ComponentModel.ISupportInitialize)pictureColor4).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureColor3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureColor2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureColor).EndInit();
            panelKeyboardTitle.ResumeLayout(false);
            panelKeyboardTitle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureKeyboard).EndInit();
            panelVersion.ResumeLayout(false);
            panelVersion.PerformLayout();
            panelPeripherals.ResumeLayout(false);
            panelPeripherals.PerformLayout();
            tableLayoutPeripherals.ResumeLayout(false);
            panelPeripheralsTile.ResumeLayout(false);
            panelPeripheralsTile.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picturePeripherals).EndInit();
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
        private RButton buttonKeyboardColor;
        private RButton buttonFans;
        private Slider sliderBattery;
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
        private RButton buttonStopGPU;
        private TableLayoutPanel tableButtons;
        private Panel panelPeripherals;
        private TableLayoutPanel tableLayoutPeripherals;
        private Panel panelPeripheralsTile;
        private PictureBox picturePeripherals;
        private Label labelPeripherals;
        private RButton buttonPeripheral2;
        private RButton buttonPeripheral3;
        private RButton buttonPeripheral1;
        private RButton buttonKeyboard;
        private RButton buttonUpdates;
        private Label labelCharge;
        private RButton buttonFnLock;
        private RButton buttonBatteryFull;
        private PictureBox pictureColor4;
        private PictureBox pictureColor3;
    }
}