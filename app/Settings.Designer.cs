using CustomControls;

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
            comboMatrix = new RComboBox();
            comboMatrixRunning = new RComboBox();
            buttonMatrix = new RButton();
            pictureMatrix = new PictureBox();
            labelMatrix = new Label();
            panelBattery = new Panel();
            sliderBattery = new WinFormsSliderBar.Slider();
            labelModel = new Label();
            labelVersion = new Label();
            labelBattery = new Label();
            pictureBattery = new PictureBox();
            labelBatteryTitle = new Label();
            panelFooter = new Panel();
            buttonQuit = new RButton();
            checkStartup = new CheckBox();
            panelPerformance = new Panel();
            picturePerf = new PictureBox();
            labelPerf = new Label();
            labelCPUFan = new Label();
            tablePerf = new TableLayoutPanel();
            buttonSilent = new RButton();
            buttonBalanced = new RButton();
            buttonTurbo = new RButton();
            buttonFans = new RButton();
            panelGPU = new Panel();
            labelTipGPU = new Label();
            pictureGPU = new PictureBox();
            labelGPU = new Label();
            labelGPUFan = new Label();
            tableGPU = new TableLayoutPanel();
            buttonEco = new RButton();
            buttonStandard = new RButton();
            buttonOptimized = new RButton();
            buttonUltimate = new RButton();
            panelScreen = new Panel();
            labelMidFan = new Label();
            labelTipScreen = new Label();
            tableScreen = new TableLayoutPanel();
            buttonScreenAuto = new RButton();
            button60Hz = new RButton();
            button120Hz = new RButton();
            buttonMiniled = new RButton();
            pictureScreen = new PictureBox();
            labelSreen = new Label();
            panelKeyboard = new Panel();
            tableLayoutKeyboard = new TableLayoutPanel();
            comboKeyboard = new RComboBox();
            panelColor = new Panel();
            pictureColor2 = new PictureBox();
            pictureColor = new PictureBox();
            buttonKeyboardColor = new RButton();
            buttonKeyboard = new RButton();
            pictureKeyboard = new PictureBox();
            labelKeyboard = new Label();
            panelMatrix.SuspendLayout();
            tableLayoutMatrix.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureMatrix).BeginInit();
            panelBattery.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBattery).BeginInit();
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
            panelMatrix.AutoSize = true;
            panelMatrix.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelMatrix.Controls.Add(checkMatrix);
            panelMatrix.Controls.Add(tableLayoutMatrix);
            panelMatrix.Controls.Add(pictureMatrix);
            panelMatrix.Controls.Add(labelMatrix);
            panelMatrix.Dock = DockStyle.Top;
            panelMatrix.Location = new Point(10, 758);
            panelMatrix.Margin = new Padding(8);
            panelMatrix.Name = "panelMatrix";
            panelMatrix.Padding = new Padding(0, 0, 0, 12);
            panelMatrix.Size = new Size(810, 168);
            panelMatrix.TabIndex = 33;
            // 
            // checkMatrix
            // 
            checkMatrix.AutoSize = true;
            checkMatrix.ForeColor = SystemColors.GrayText;
            checkMatrix.Location = new Point(24, 116);
            checkMatrix.Margin = new Padding(8, 4, 8, 4);
            checkMatrix.Name = "checkMatrix";
            checkMatrix.Size = new Size(249, 36);
            checkMatrix.TabIndex = 44;
            checkMatrix.Text = "电池供电时关闭";
            checkMatrix.UseVisualStyleBackColor = true;
            // 
            // tableLayoutMatrix
            // 
            tableLayoutMatrix.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
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
            tableLayoutMatrix.Location = new Point(16, 52);
            tableLayoutMatrix.Margin = new Padding(8);
            tableLayoutMatrix.Name = "tableLayoutMatrix";
            tableLayoutMatrix.RowCount = 1;
            tableLayoutMatrix.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutMatrix.Size = new Size(771, 60);
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
            comboMatrix.Items.AddRange(new object[] { "Off", "Dim", "Medium", "Bright" });
            comboMatrix.Location = new Point(4, 10);
            comboMatrix.Margin = new Padding(4, 10, 4, 8);
            comboMatrix.Name = "comboMatrix";
            comboMatrix.Size = new Size(249, 40);
            comboMatrix.TabIndex = 41;
            comboMatrix.TabStop = false;
            // 
            // comboMatrixRunning
            // 
            comboMatrixRunning.BorderColor = Color.White;
            comboMatrixRunning.ButtonColor = Color.FromArgb(255, 255, 255);
            comboMatrixRunning.Dock = DockStyle.Top;
            comboMatrixRunning.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            comboMatrixRunning.FormattingEnabled = true;
            comboMatrixRunning.ItemHeight = 32;
            comboMatrixRunning.Items.AddRange(new object[] { "Binary Banner", "Rog Logo", "Picture", "Clock" });
            comboMatrixRunning.Location = new Point(261, 10);
            comboMatrixRunning.Margin = new Padding(4, 10, 4, 8);
            comboMatrixRunning.Name = "comboMatrixRunning";
            comboMatrixRunning.Size = new Size(249, 40);
            comboMatrixRunning.TabIndex = 42;
            comboMatrixRunning.TabStop = false;
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
            buttonMatrix.Location = new Point(518, 8);
            buttonMatrix.Margin = new Padding(4, 8, 4, 8);
            buttonMatrix.Name = "buttonMatrix";
            buttonMatrix.Secondary = true;
            buttonMatrix.Size = new Size(249, 44);
            buttonMatrix.TabIndex = 43;
            buttonMatrix.Text = "Picture / Gif";
            buttonMatrix.UseVisualStyleBackColor = false;
            // 
            // pictureMatrix
            // 
            pictureMatrix.BackgroundImage = Properties.Resources.icons8_matrix_desktop_48;
            pictureMatrix.BackgroundImageLayout = ImageLayout.Zoom;
            pictureMatrix.Location = new Point(24, 15);
            pictureMatrix.Margin = new Padding(4);
            pictureMatrix.Name = "pictureMatrix";
            pictureMatrix.Size = new Size(32, 32);
            pictureMatrix.TabIndex = 39;
            pictureMatrix.TabStop = false;
            // 
            // labelMatrix
            // 
            labelMatrix.AutoSize = true;
            labelMatrix.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelMatrix.Location = new Point(60, 14);
            labelMatrix.Margin = new Padding(8, 0, 8, 0);
            labelMatrix.Name = "labelMatrix";
            labelMatrix.Size = new Size(170, 32);
            labelMatrix.TabIndex = 38;
            labelMatrix.Text = "Anime Matrix";
            // 
            // panelBattery
            // 
            panelBattery.AutoSize = true;
            panelBattery.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelBattery.Controls.Add(sliderBattery);
            panelBattery.Controls.Add(labelModel);
            panelBattery.Controls.Add(labelVersion);
            panelBattery.Controls.Add(labelBattery);
            panelBattery.Controls.Add(pictureBattery);
            panelBattery.Controls.Add(labelBatteryTitle);
            panelBattery.Dock = DockStyle.Top;
            panelBattery.Location = new Point(10, 926);
            panelBattery.Margin = new Padding(8);
            panelBattery.Name = "panelBattery";
            panelBattery.Padding = new Padding(0, 0, 0, 12);
            panelBattery.Size = new Size(810, 163);
            panelBattery.TabIndex = 34;
            // 
            // sliderBattery
            // 
            sliderBattery.Location = new Point(16, 70);
            sliderBattery.Max = 100;
            sliderBattery.Min = 50;
            sliderBattery.Name = "sliderBattery";
            sliderBattery.Size = new Size(772, 40);
            sliderBattery.TabIndex = 39;
            sliderBattery.Text = "sliderBattery";
            sliderBattery.Value = 80;
            // 
            // labelModel
            // 
            labelModel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelModel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            labelModel.ForeColor = SystemColors.ControlDark;
            labelModel.Location = new Point(291, 119);
            labelModel.Margin = new Padding(8, 0, 8, 0);
            labelModel.Name = "labelModel";
            labelModel.Size = new Size(492, 32);
            labelModel.TabIndex = 38;
            labelModel.TextAlign = ContentAlignment.TopRight;
            // 
            // labelVersion
            // 
            labelVersion.AutoSize = true;
            labelVersion.Font = new Font("Segoe UI", 9F, FontStyle.Underline, GraphicsUnit.Point);
            labelVersion.ForeColor = SystemColors.ControlDark;
            labelVersion.Location = new Point(25, 119);
            labelVersion.Margin = new Padding(8, 0, 8, 0);
            labelVersion.Name = "labelVersion";
            labelVersion.Size = new Size(44, 32);
            labelVersion.TabIndex = 37;
            labelVersion.Text = "v.0";
            // 
            // labelBattery
            // 
            labelBattery.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelBattery.Location = new Point(422, 9);
            labelBattery.Margin = new Padding(8, 0, 8, 0);
            labelBattery.Name = "labelBattery";
            labelBattery.Size = new Size(364, 44);
            labelBattery.TabIndex = 36;
            labelBattery.Text = "                ";
            labelBattery.TextAlign = ContentAlignment.TopRight;
            // 
            // pictureBattery
            // 
            pictureBattery.BackgroundImage = (Image)resources.GetObject("pictureBattery.BackgroundImage");
            pictureBattery.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBattery.Location = new Point(24, 16);
            pictureBattery.Margin = new Padding(4);
            pictureBattery.Name = "pictureBattery";
            pictureBattery.Size = new Size(32, 32);
            pictureBattery.TabIndex = 35;
            pictureBattery.TabStop = false;
            // 
            // labelBatteryTitle
            // 
            labelBatteryTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelBatteryTitle.Location = new Point(60, 15);
            labelBatteryTitle.Margin = new Padding(8, 0, 8, 0);
            labelBatteryTitle.Name = "labelBatteryTitle";
            labelBatteryTitle.Size = new Size(393, 36);
            labelBatteryTitle.TabIndex = 34;
            labelBatteryTitle.Text = "最大充电限制：";
            // 
            // panelFooter
            // 
            panelFooter.AutoSize = true;
            panelFooter.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelFooter.Controls.Add(buttonQuit);
            panelFooter.Controls.Add(checkStartup);
            panelFooter.Dock = DockStyle.Top;
            panelFooter.Location = new Point(10, 1089);
            panelFooter.Margin = new Padding(8);
            panelFooter.Name = "panelFooter";
            panelFooter.Padding = new Padding(0, 0, 0, 10);
            panelFooter.Size = new Size(810, 74);
            panelFooter.TabIndex = 35;
            // 
            // buttonQuit
            // 
            buttonQuit.Activated = false;
            buttonQuit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonQuit.BackColor = SystemColors.ControlLight;
            buttonQuit.BorderColor = Color.Transparent;
            buttonQuit.BorderRadius = 2;
            buttonQuit.FlatStyle = FlatStyle.Flat;
            buttonQuit.Location = new Point(599, 16);
            buttonQuit.Margin = new Padding(8, 4, 8, 4);
            buttonQuit.Name = "buttonQuit";
            buttonQuit.Secondary = true;
            buttonQuit.Size = new Size(185, 44);
            buttonQuit.TabIndex = 18;
            buttonQuit.Text = "退出";
            buttonQuit.UseVisualStyleBackColor = false;
            // 
            // checkStartup
            // 
            checkStartup.AutoSize = true;
            checkStartup.Location = new Point(24, 21);
            checkStartup.Margin = new Padding(8, 4, 8, 4);
            checkStartup.Name = "checkStartup";
            checkStartup.Size = new Size(206, 36);
            checkStartup.TabIndex = 17;
            checkStartup.Text = "开机自启";
            checkStartup.UseVisualStyleBackColor = true;
            // 
            // panelPerformance
            // 
            panelPerformance.AutoSize = true;
            panelPerformance.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelPerformance.Controls.Add(picturePerf);
            panelPerformance.Controls.Add(labelPerf);
            panelPerformance.Controls.Add(labelCPUFan);
            panelPerformance.Controls.Add(tablePerf);
            panelPerformance.Dock = DockStyle.Top;
            panelPerformance.Location = new Point(10, 10);
            panelPerformance.Margin = new Padding(0);
            panelPerformance.Name = "panelPerformance";
            panelPerformance.Padding = new Padding(0, 0, 0, 12);
            panelPerformance.Size = new Size(810, 200);
            panelPerformance.TabIndex = 36;
            // 
            // picturePerf
            // 
            picturePerf.BackgroundImage = (Image)resources.GetObject("picturePerf.BackgroundImage");
            picturePerf.BackgroundImageLayout = ImageLayout.Zoom;
            picturePerf.InitialImage = null;
            picturePerf.Location = new Point(24, 20);
            picturePerf.Margin = new Padding(4);
            picturePerf.Name = "picturePerf";
            picturePerf.Size = new Size(32, 32);
            picturePerf.TabIndex = 32;
            picturePerf.TabStop = false;
            // 
            // labelPerf
            // 
            labelPerf.AutoSize = true;
            labelPerf.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelPerf.Location = new Point(60, 18);
            labelPerf.Margin = new Padding(8, 0, 8, 0);
            labelPerf.Name = "labelPerf";
            labelPerf.Size = new Size(234, 32);
            labelPerf.TabIndex = 31;
            labelPerf.Text = "性能模式";
            // 
            // labelCPUFan
            // 
            labelCPUFan.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelCPUFan.Cursor = Cursors.Hand;
            labelCPUFan.Location = new Point(384, 15);
            labelCPUFan.Margin = new Padding(8, 0, 8, 0);
            labelCPUFan.Name = "labelCPUFan";
            labelCPUFan.Size = new Size(400, 36);
            labelCPUFan.TabIndex = 30;
            labelCPUFan.Text = "      ";
            labelCPUFan.TextAlign = ContentAlignment.TopRight;
            // 
            // tablePerf
            // 
            tablePerf.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
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
            tablePerf.Location = new Point(16, 56);
            tablePerf.Margin = new Padding(8, 4, 8, 4);
            tablePerf.Name = "tablePerf";
            tablePerf.RowCount = 1;
            tablePerf.RowStyles.Add(new RowStyle(SizeType.Absolute, 128F));
            tablePerf.Size = new Size(772, 128);
            tablePerf.TabIndex = 29;
            // 
            // buttonSilent
            // 
            buttonSilent.Activated = false;
            buttonSilent.BackColor = SystemColors.ControlLightLight;
            buttonSilent.BackgroundImageLayout = ImageLayout.None;
            buttonSilent.BorderColor = Color.Transparent;
            buttonSilent.BorderRadius = 5;
            buttonSilent.CausesValidation = false;
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
            buttonSilent.Size = new Size(185, 120);
            buttonSilent.TabIndex = 0;
            buttonSilent.Text = "安静模式";
            buttonSilent.TextImageRelation = TextImageRelation.ImageAboveText;
            buttonSilent.UseVisualStyleBackColor = false;
            // 
            // buttonBalanced
            // 
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
            buttonBalanced.Location = new Point(197, 4);
            buttonBalanced.Margin = new Padding(4);
            buttonBalanced.Name = "buttonBalanced";
            buttonBalanced.Secondary = false;
            buttonBalanced.Size = new Size(185, 120);
            buttonBalanced.TabIndex = 1;
            buttonBalanced.Text = "平衡模式";
            buttonBalanced.TextImageRelation = TextImageRelation.ImageAboveText;
            buttonBalanced.UseVisualStyleBackColor = false;
            // 
            // buttonTurbo
            // 
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
            buttonTurbo.Location = new Point(390, 4);
            buttonTurbo.Margin = new Padding(4);
            buttonTurbo.Name = "buttonTurbo";
            buttonTurbo.Secondary = false;
            buttonTurbo.Size = new Size(185, 120);
            buttonTurbo.TabIndex = 2;
            buttonTurbo.Text = "极速模式";
            buttonTurbo.TextImageRelation = TextImageRelation.ImageAboveText;
            buttonTurbo.UseVisualStyleBackColor = false;
            // 
            // buttonFans
            // 
            buttonFans.Activated = false;
            buttonFans.BackColor = SystemColors.ControlLight;
            buttonFans.BorderColor = Color.Transparent;
            buttonFans.BorderRadius = 5;
            buttonFans.Dock = DockStyle.Fill;
            buttonFans.FlatAppearance.BorderSize = 0;
            buttonFans.FlatStyle = FlatStyle.Flat;
            buttonFans.Image = Properties.Resources.icons8_fan_48;
            buttonFans.ImageAlign = ContentAlignment.BottomCenter;
            buttonFans.Location = new Point(583, 4);
            buttonFans.Margin = new Padding(4);
            buttonFans.Name = "buttonFans";
            buttonFans.Secondary = true;
            buttonFans.Size = new Size(185, 120);
            buttonFans.TabIndex = 35;
            buttonFans.Text = "自定义设置";
            buttonFans.TextImageRelation = TextImageRelation.ImageAboveText;
            buttonFans.UseVisualStyleBackColor = false;
            // 
            // panelGPU
            // 
            panelGPU.AutoSize = true;
            panelGPU.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelGPU.Controls.Add(labelTipGPU);
            panelGPU.Controls.Add(pictureGPU);
            panelGPU.Controls.Add(labelGPU);
            panelGPU.Controls.Add(labelGPUFan);
            panelGPU.Controls.Add(tableGPU);
            panelGPU.Dock = DockStyle.Top;
            panelGPU.Location = new Point(10, 210);
            panelGPU.Margin = new Padding(8);
            panelGPU.Name = "panelGPU";
            panelGPU.Padding = new Padding(0, 0, 0, 10);
            panelGPU.Size = new Size(810, 237);
            panelGPU.TabIndex = 37;
            // 
            // labelTipGPU
            // 
            labelTipGPU.ForeColor = SystemColors.GrayText;
            labelTipGPU.Location = new Point(24, 191);
            labelTipGPU.Margin = new Padding(4, 0, 4, 0);
            labelTipGPU.Name = "labelTipGPU";
            labelTipGPU.Size = new Size(720, 36);
            labelTipGPU.TabIndex = 20;
            labelTipGPU.Text = "                                                                          ";
            // 
            // pictureGPU
            // 
            pictureGPU.BackgroundImage = (Image)resources.GetObject("pictureGPU.BackgroundImage");
            pictureGPU.BackgroundImageLayout = ImageLayout.Zoom;
            pictureGPU.Location = new Point(24, 21);
            pictureGPU.Margin = new Padding(4);
            pictureGPU.Name = "pictureGPU";
            pictureGPU.Size = new Size(32, 32);
            pictureGPU.TabIndex = 19;
            pictureGPU.TabStop = false;
            // 
            // labelGPU
            // 
            labelGPU.AutoSize = true;
            labelGPU.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelGPU.Location = new Point(60, 21);
            labelGPU.Margin = new Padding(8, 0, 8, 0);
            labelGPU.Name = "labelGPU";
            labelGPU.Size = new Size(136, 32);
            labelGPU.TabIndex = 18;
            labelGPU.Text = "GPU模式";
            // 
            // labelGPUFan
            // 
            labelGPUFan.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelGPUFan.Location = new Point(384, 21);
            labelGPUFan.Margin = new Padding(8, 0, 8, 0);
            labelGPUFan.Name = "labelGPUFan";
            labelGPUFan.Size = new Size(400, 34);
            labelGPUFan.TabIndex = 17;
            labelGPUFan.Text = "         ";
            labelGPUFan.TextAlign = ContentAlignment.TopRight;
            // 
            // tableGPU
            // 
            tableGPU.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableGPU.AutoSize = true;
            tableGPU.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableGPU.ColumnCount = 4;
            tableGPU.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableGPU.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableGPU.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableGPU.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableGPU.Controls.Add(buttonEco, 0, 0);
            tableGPU.Controls.Add(buttonStandard, 1, 0);
            tableGPU.Controls.Add(buttonOptimized, 2, 0);
            tableGPU.Controls.Add(buttonUltimate, 2, 0);
            tableGPU.Location = new Point(16, 60);
            tableGPU.Margin = new Padding(8, 4, 8, 4);
            tableGPU.Name = "tableGPU";
            tableGPU.RowCount = 1;
            tableGPU.RowStyles.Add(new RowStyle(SizeType.Absolute, 128F));
            tableGPU.Size = new Size(772, 128);
            tableGPU.TabIndex = 16;
            // 
            // buttonEco
            // 
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
            buttonEco.Size = new Size(185, 120);
            buttonEco.TabIndex = 0;
            buttonEco.Text = "节能模式";
            buttonEco.TextImageRelation = TextImageRelation.ImageAboveText;
            buttonEco.UseVisualStyleBackColor = false;
            // 
            // buttonStandard
            // 
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
            buttonStandard.Location = new Point(197, 4);
            buttonStandard.Margin = new Padding(4);
            buttonStandard.Name = "buttonStandard";
            buttonStandard.Secondary = false;
            buttonStandard.Size = new Size(185, 120);
            buttonStandard.TabIndex = 1;
            buttonStandard.Text = "标准模式";
            buttonStandard.TextImageRelation = TextImageRelation.ImageAboveText;
            buttonStandard.UseVisualStyleBackColor = false;
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
            buttonOptimized.Location = new Point(583, 4);
            buttonOptimized.Margin = new Padding(4);
            buttonOptimized.Name = "buttonOptimized";
            buttonOptimized.Secondary = false;
            buttonOptimized.Size = new Size(185, 120);
            buttonOptimized.TabIndex = 3;
            buttonOptimized.Text = "自动模式";
            buttonOptimized.TextImageRelation = TextImageRelation.ImageAboveText;
            buttonOptimized.UseVisualStyleBackColor = false;
            // 
            // buttonUltimate
            // 
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
            buttonUltimate.Location = new Point(390, 4);
            buttonUltimate.Margin = new Padding(4);
            buttonUltimate.Name = "buttonUltimate";
            buttonUltimate.Secondary = false;
            buttonUltimate.Size = new Size(185, 120);
            buttonUltimate.TabIndex = 2;
            buttonUltimate.Text = "无限制模式";
            buttonUltimate.TextImageRelation = TextImageRelation.ImageAboveText;
            buttonUltimate.UseVisualStyleBackColor = false;
            // 
            // panelScreen
            // 
            panelScreen.AutoSize = true;
            panelScreen.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelScreen.Controls.Add(labelMidFan);
            panelScreen.Controls.Add(labelTipScreen);
            panelScreen.Controls.Add(tableScreen);
            panelScreen.Controls.Add(pictureScreen);
            panelScreen.Controls.Add(labelSreen);
            panelScreen.Dock = DockStyle.Top;
            panelScreen.Location = new Point(10, 447);
            panelScreen.Margin = new Padding(8);
            panelScreen.Name = "panelScreen";
            panelScreen.Padding = new Padding(0, 0, 0, 10);
            panelScreen.Size = new Size(810, 181);
            panelScreen.TabIndex = 38;
            // 
            // labelMidFan
            // 
            labelMidFan.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelMidFan.Location = new Point(488, 13);
            labelMidFan.Margin = new Padding(8, 0, 8, 0);
            labelMidFan.Name = "labelMidFan";
            labelMidFan.Size = new Size(296, 34);
            labelMidFan.TabIndex = 25;
            labelMidFan.Text = "         ";
            labelMidFan.TextAlign = ContentAlignment.TopRight;
            // 
            // labelTipScreen
            // 
            labelTipScreen.ForeColor = SystemColors.GrayText;
            labelTipScreen.Location = new Point(20, 135);
            labelTipScreen.Margin = new Padding(4, 0, 4, 0);
            labelTipScreen.Name = "labelTipScreen";
            labelTipScreen.Size = new Size(724, 36);
            labelTipScreen.TabIndex = 24;
            // 
            // tableScreen
            // 
            tableScreen.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
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
            tableScreen.Location = new Point(16, 51);
            tableScreen.Margin = new Padding(8, 4, 8, 4);
            tableScreen.Name = "tableScreen";
            tableScreen.RowCount = 1;
            tableScreen.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));
            tableScreen.Size = new Size(772, 80);
            tableScreen.TabIndex = 23;
            // 
            // buttonScreenAuto
            // 
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
            buttonScreenAuto.Size = new Size(185, 72);
            buttonScreenAuto.TabIndex = 0;
            buttonScreenAuto.Text = "自动";
            buttonScreenAuto.UseVisualStyleBackColor = false;
            // 
            // button60Hz
            // 
            button60Hz.Activated = false;
            button60Hz.BackColor = SystemColors.ControlLightLight;
            button60Hz.BorderColor = Color.Transparent;
            button60Hz.BorderRadius = 5;
            button60Hz.CausesValidation = false;
            button60Hz.Dock = DockStyle.Fill;
            button60Hz.FlatAppearance.BorderSize = 0;
            button60Hz.FlatStyle = FlatStyle.Flat;
            button60Hz.ForeColor = SystemColors.ControlText;
            button60Hz.Location = new Point(197, 4);
            button60Hz.Margin = new Padding(4);
            button60Hz.Name = "button60Hz";
            button60Hz.Secondary = false;
            button60Hz.Size = new Size(185, 72);
            button60Hz.TabIndex = 1;
            button60Hz.Text = "60Hz";
            button60Hz.UseVisualStyleBackColor = false;
            // 
            // button120Hz
            // 
            button120Hz.Activated = false;
            button120Hz.BackColor = SystemColors.ControlLightLight;
            button120Hz.BorderColor = Color.Transparent;
            button120Hz.BorderRadius = 5;
            button120Hz.Dock = DockStyle.Fill;
            button120Hz.FlatAppearance.BorderSize = 0;
            button120Hz.FlatStyle = FlatStyle.Flat;
            button120Hz.ForeColor = SystemColors.ControlText;
            button120Hz.Location = new Point(390, 4);
            button120Hz.Margin = new Padding(4);
            button120Hz.Name = "button120Hz";
            button120Hz.Secondary = false;
            button120Hz.Size = new Size(185, 72);
            button120Hz.TabIndex = 2;
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
            buttonMiniled.Location = new Point(583, 4);
            buttonMiniled.Margin = new Padding(4);
            buttonMiniled.Name = "buttonMiniled";
            buttonMiniled.Secondary = false;
            buttonMiniled.Size = new Size(185, 72);
            buttonMiniled.TabIndex = 3;
            buttonMiniled.Text = "Miniled";
            buttonMiniled.UseVisualStyleBackColor = false;
            // 
            // pictureScreen
            // 
            pictureScreen.BackgroundImage = (Image)resources.GetObject("pictureScreen.BackgroundImage");
            pictureScreen.BackgroundImageLayout = ImageLayout.Zoom;
            pictureScreen.Location = new Point(24, 11);
            pictureScreen.Margin = new Padding(4);
            pictureScreen.Name = "pictureScreen";
            pictureScreen.Size = new Size(32, 32);
            pictureScreen.TabIndex = 22;
            pictureScreen.TabStop = false;
            // 
            // labelSreen
            // 
            labelSreen.AutoSize = true;
            labelSreen.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelSreen.Location = new Point(60, 9);
            labelSreen.Margin = new Padding(8, 0, 8, 0);
            labelSreen.Name = "labelSreen";
            labelSreen.Size = new Size(176, 32);
            labelSreen.TabIndex = 21;
            labelSreen.Text = "笔记本屏幕";
            // 
            // panelKeyboard
            // 
            panelKeyboard.AutoSize = true;
            panelKeyboard.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelKeyboard.Controls.Add(tableLayoutKeyboard);
            panelKeyboard.Controls.Add(pictureKeyboard);
            panelKeyboard.Controls.Add(labelKeyboard);
            panelKeyboard.Dock = DockStyle.Top;
            panelKeyboard.Location = new Point(10, 628);
            panelKeyboard.Margin = new Padding(8);
            panelKeyboard.Name = "panelKeyboard";
            panelKeyboard.Padding = new Padding(0, 0, 0, 12);
            panelKeyboard.Size = new Size(810, 130);
            panelKeyboard.TabIndex = 39;
            // 
            // tableLayoutKeyboard
            // 
            tableLayoutKeyboard.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutKeyboard.AutoSize = true;
            tableLayoutKeyboard.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutKeyboard.ColumnCount = 3;
            tableLayoutKeyboard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutKeyboard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutKeyboard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutKeyboard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutKeyboard.Controls.Add(comboKeyboard, 0, 0);
            tableLayoutKeyboard.Controls.Add(panelColor, 1, 0);
            tableLayoutKeyboard.Controls.Add(buttonKeyboard, 2, 0);
            tableLayoutKeyboard.Location = new Point(16, 50);
            tableLayoutKeyboard.Margin = new Padding(8);
            tableLayoutKeyboard.Name = "tableLayoutKeyboard";
            tableLayoutKeyboard.RowCount = 1;
            tableLayoutKeyboard.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutKeyboard.Size = new Size(771, 60);
            tableLayoutKeyboard.TabIndex = 39;
            // 
            // comboKeyboard
            // 
            comboKeyboard.BorderColor = Color.White;
            comboKeyboard.ButtonColor = Color.FromArgb(255, 255, 255);
            comboKeyboard.Dock = DockStyle.Top;
            comboKeyboard.FlatStyle = FlatStyle.Flat;
            comboKeyboard.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            comboKeyboard.FormattingEnabled = true;
            comboKeyboard.ItemHeight = 32;
            comboKeyboard.Items.AddRange(new object[] { "正常", "呼吸", "彩虹", "闪烁" });
            comboKeyboard.Location = new Point(4, 10);
            comboKeyboard.Margin = new Padding(4, 10, 4, 8);
            comboKeyboard.Name = "comboKeyboard";
            comboKeyboard.Size = new Size(249, 40);
            comboKeyboard.TabIndex = 35;
            comboKeyboard.TabStop = false;
            // 
            // panelColor
            // 
            panelColor.AutoSize = true;
            panelColor.Controls.Add(pictureColor2);
            panelColor.Controls.Add(pictureColor);
            panelColor.Controls.Add(buttonKeyboardColor);
            panelColor.Dock = DockStyle.Fill;
            panelColor.Location = new Point(261, 8);
            panelColor.Margin = new Padding(4, 8, 4, 8);
            panelColor.Name = "panelColor";
            panelColor.Size = new Size(249, 44);
            panelColor.TabIndex = 36;
            // 
            // pictureColor2
            // 
            pictureColor2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pictureColor2.Location = new Point(188, 12);
            pictureColor2.Margin = new Padding(8);
            pictureColor2.Name = "pictureColor2";
            pictureColor2.Size = new Size(20, 20);
            pictureColor2.TabIndex = 41;
            pictureColor2.TabStop = false;
            // 
            // pictureColor
            // 
            pictureColor.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pictureColor.Location = new Point(215, 12);
            pictureColor.Margin = new Padding(8);
            pictureColor.Name = "pictureColor";
            pictureColor.Size = new Size(20, 20);
            pictureColor.TabIndex = 40;
            pictureColor.TabStop = false;
            // 
            // buttonKeyboardColor
            // 
            buttonKeyboardColor.Activated = false;
            buttonKeyboardColor.BackColor = SystemColors.ButtonHighlight;
            buttonKeyboardColor.BorderColor = Color.Transparent;
            buttonKeyboardColor.BorderRadius = 2;
            buttonKeyboardColor.Dock = DockStyle.Top;
            buttonKeyboardColor.FlatStyle = FlatStyle.Flat;
            buttonKeyboardColor.ForeColor = SystemColors.ControlText;
            buttonKeyboardColor.Location = new Point(0, 0);
            buttonKeyboardColor.Margin = new Padding(4, 8, 4, 8);
            buttonKeyboardColor.Name = "buttonKeyboardColor";
            buttonKeyboardColor.Secondary = false;
            buttonKeyboardColor.Size = new Size(249, 44);
            buttonKeyboardColor.TabIndex = 39;
            buttonKeyboardColor.Text = "颜色";
            buttonKeyboardColor.UseVisualStyleBackColor = false;
            // 
            // buttonKeyboard
            // 
            buttonKeyboard.Activated = false;
            buttonKeyboard.BackColor = SystemColors.ControlLight;
            buttonKeyboard.BorderColor = Color.Transparent;
            buttonKeyboard.BorderRadius = 2;
            buttonKeyboard.Dock = DockStyle.Top;
            buttonKeyboard.FlatAppearance.BorderSize = 0;
            buttonKeyboard.FlatStyle = FlatStyle.Flat;
            buttonKeyboard.Location = new Point(518, 8);
            buttonKeyboard.Margin = new Padding(4, 8, 4, 8);
            buttonKeyboard.Name = "buttonKeyboard";
            buttonKeyboard.Secondary = true;
            buttonKeyboard.Size = new Size(249, 44);
            buttonKeyboard.TabIndex = 37;
            buttonKeyboard.Text = "更多";
            buttonKeyboard.UseVisualStyleBackColor = false;
            // 
            // pictureKeyboard
            // 
            pictureKeyboard.BackgroundImage = Properties.Resources.icons8_keyboard_48;
            pictureKeyboard.BackgroundImageLayout = ImageLayout.Zoom;
            pictureKeyboard.Location = new Point(24, 14);
            pictureKeyboard.Margin = new Padding(4);
            pictureKeyboard.Name = "pictureKeyboard";
            pictureKeyboard.Size = new Size(32, 32);
            pictureKeyboard.TabIndex = 33;
            pictureKeyboard.TabStop = false;
            // 
            // labelKeyboard
            // 
            labelKeyboard.AutoSize = true;
            labelKeyboard.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelKeyboard.Location = new Point(60, 13);
            labelKeyboard.Margin = new Padding(8, 0, 8, 0);
            labelKeyboard.Name = "labelKeyboard";
            labelKeyboard.Size = new Size(210, 32);
            labelKeyboard.TabIndex = 32;
            labelKeyboard.Text = "笔记本键盘";
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(192F, 192F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(830, 1173);
            Controls.Add(panelFooter);
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
            Load += Settings_Load;
            panelMatrix.ResumeLayout(false);
            panelMatrix.PerformLayout();
            tableLayoutMatrix.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureMatrix).EndInit();
            panelBattery.ResumeLayout(false);
            panelBattery.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBattery).EndInit();
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
            tableLayoutKeyboard.PerformLayout();
            panelColor.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureColor2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureColor).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureKeyboard).EndInit();
            ResumeLayout(false);
            PerformLayout();
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
        private Panel panelFooter;
        private RButton buttonQuit;
        private CheckBox checkStartup;
        private Panel panelPerformance;
        private PictureBox picturePerf;
        private Label labelPerf;
        private Label labelCPUFan;
        private TableLayoutPanel tablePerf;
        private RButton buttonTurbo;
        private RButton buttonBalanced;
        private RButton buttonSilent;
        private Panel panelGPU;
        private PictureBox pictureGPU;
        private Label labelGPU;
        private Label labelGPUFan;
        private TableLayoutPanel tableGPU;
        private RButton buttonUltimate;
        private RButton buttonStandard;
        private RButton buttonEco;
        private Panel panelScreen;
        private TableLayoutPanel tableScreen;
        private RButton buttonScreenAuto;
        private RButton button60Hz;
        private PictureBox pictureScreen;
        private Label labelSreen;
        private Panel panelKeyboard;
        private PictureBox pictureKeyboard;
        private Label labelKeyboard;
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
        private Label labelMidFan;
        private Label labelModel;
        private WinFormsSliderBar.Slider sliderBattery;
    }
}