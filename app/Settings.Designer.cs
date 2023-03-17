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
            picturePerf = new PictureBox();
            labelPerf = new Label();
            labelCPUFan = new Label();
            tablePerf = new TableLayoutPanel();
            buttonSilent = new RoundedButton();
            buttonBalanced = new RoundedButton();
            buttonTurbo = new RoundedButton();
            buttonFans = new Button();
            panelGPU = new Panel();
            pictureGPU = new PictureBox();
            labelGPU = new Label();
            labelGPUFan = new Label();
            tableGPU = new TableLayoutPanel();
            buttonOptimized = new RoundedButton();
            buttonEco = new RoundedButton();
            buttonStandard = new RoundedButton();
            buttonUltimate = new RoundedButton();
            panelScreen = new Panel();
            tableScreen = new TableLayoutPanel();
            buttonScreenAuto = new RoundedButton();
            button60Hz = new RoundedButton();
            button120Hz = new RoundedButton();
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
            panelMatrix.Location = new Point(16, 764);
            panelMatrix.Margin = new Padding(4);
            panelMatrix.Name = "panelMatrix";
            panelMatrix.Size = new Size(818, 180);
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
            tableLayoutMatrix.ColumnCount = 4;
            tableLayoutMatrix.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutMatrix.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutMatrix.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutMatrix.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutMatrix.Controls.Add(buttonMatrix, 0, 0);
            tableLayoutMatrix.Controls.Add(comboMatrixRunning, 0, 0);
            tableLayoutMatrix.Controls.Add(comboMatrix, 0, 0);
            tableLayoutMatrix.Location = new Point(15, 52);
            tableLayoutMatrix.Margin = new Padding(4);
            tableLayoutMatrix.Name = "tableLayoutMatrix";
            tableLayoutMatrix.RowCount = 1;
            tableLayoutMatrix.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutMatrix.Size = new Size(780, 62);
            tableLayoutMatrix.TabIndex = 43;
            // 
            // buttonMatrix
            // 
            buttonMatrix.BackColor = SystemColors.ButtonFace;
            buttonMatrix.Dock = DockStyle.Top;
            buttonMatrix.FlatAppearance.BorderSize = 0;
            buttonMatrix.Location = new Point(395, 10);
            buttonMatrix.Margin = new Padding(5, 10, 5, 10);
            buttonMatrix.Name = "buttonMatrix";
            buttonMatrix.Size = new Size(185, 42);
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
            comboMatrixRunning.Location = new Point(200, 10);
            comboMatrixRunning.Margin = new Padding(5, 10, 5, 10);
            comboMatrixRunning.Name = "comboMatrixRunning";
            comboMatrixRunning.Size = new Size(185, 40);
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
            comboMatrix.Location = new Point(5, 10);
            comboMatrix.Margin = new Padding(5, 10, 5, 10);
            comboMatrix.Name = "comboMatrix";
            comboMatrix.Size = new Size(185, 40);
            comboMatrix.TabIndex = 41;
            comboMatrix.TabStop = false;
            // 
            // pictureMatrix
            // 
            pictureMatrix.BackgroundImage = Properties.Resources.icons8_matrix_desktop_48;
            pictureMatrix.BackgroundImageLayout = ImageLayout.Zoom;
            pictureMatrix.Location = new Point(25, 10);
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
            labelMatrix.Location = new Point(64, 12);
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
            panelBattery.Location = new Point(16, 944);
            panelBattery.Margin = new Padding(4);
            panelBattery.Name = "panelBattery";
            panelBattery.Size = new Size(818, 148);
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
            labelBattery.Location = new Point(528, 12);
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
            pictureBattery.Location = new Point(25, 10);
            pictureBattery.Margin = new Padding(4, 2, 4, 2);
            pictureBattery.Name = "pictureBattery";
            pictureBattery.Size = new Size(36, 38);
            pictureBattery.TabIndex = 35;
            pictureBattery.TabStop = false;
            // 
            // labelBatteryTitle
            // 
            labelBatteryTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelBatteryTitle.Location = new Point(66, 12);
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
            trackBattery.Size = new Size(780, 90);
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
            panelFooter.Location = new Point(16, 1092);
            panelFooter.Margin = new Padding(4);
            panelFooter.Name = "panelFooter";
            panelFooter.Size = new Size(818, 64);
            panelFooter.TabIndex = 35;
            // 
            // buttonQuit
            // 
            buttonQuit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonQuit.BackColor = SystemColors.ButtonFace;
            buttonQuit.Location = new Point(684, 9);
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
            panelPerformance.Controls.Add(picturePerf);
            panelPerformance.Controls.Add(labelPerf);
            panelPerformance.Controls.Add(labelCPUFan);
            panelPerformance.Controls.Add(tablePerf);
            panelPerformance.Dock = DockStyle.Top;
            panelPerformance.Location = new Point(16, 16);
            panelPerformance.Margin = new Padding(0);
            panelPerformance.Name = "panelPerformance";
            panelPerformance.Size = new Size(818, 210);
            panelPerformance.TabIndex = 36;
            // 
            // picturePerf
            // 
            picturePerf.BackgroundImage = (Image)resources.GetObject("picturePerf.BackgroundImage");
            picturePerf.BackgroundImageLayout = ImageLayout.Zoom;
            picturePerf.InitialImage = null;
            picturePerf.Location = new Point(25, 10);
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
            labelPerf.Location = new Point(64, 12);
            labelPerf.Margin = new Padding(4, 0, 4, 0);
            labelPerf.Name = "labelPerf";
            labelPerf.Size = new Size(234, 32);
            labelPerf.TabIndex = 31;
            labelPerf.Text = "Performance Mode";
            // 
            // labelCPUFan
            // 
            labelCPUFan.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelCPUFan.Location = new Point(422, 10);
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
            tablePerf.ColumnCount = 4;
            tablePerf.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tablePerf.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tablePerf.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tablePerf.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tablePerf.Controls.Add(buttonSilent, 0, 0);
            tablePerf.Controls.Add(buttonBalanced, 1, 0);
            tablePerf.Controls.Add(buttonTurbo, 2, 0);
            tablePerf.Controls.Add(buttonFans, 3, 0);
            tablePerf.Location = new Point(15, 48);
            tablePerf.Margin = new Padding(4, 2, 4, 2);
            tablePerf.Name = "tablePerf";
            tablePerf.RowCount = 1;
            tablePerf.RowStyles.Add(new RowStyle(SizeType.Absolute, 108F));
            tablePerf.Size = new Size(780, 150);
            tablePerf.TabIndex = 29;
            // 
            // buttonSilent
            // 
            buttonSilent.Activated = false;
            buttonSilent.BackColor = SystemColors.ControlLightLight;
            buttonSilent.BackgroundImageLayout = ImageLayout.None;
            buttonSilent.BorderColor = Color.Transparent;
            buttonSilent.CausesValidation = false;
            buttonSilent.Dock = DockStyle.Fill;
            buttonSilent.FlatAppearance.BorderColor = Color.FromArgb(0, 192, 192);
            buttonSilent.FlatAppearance.BorderSize = 0;
            buttonSilent.FlatStyle = FlatStyle.Flat;
            buttonSilent.ForeColor = SystemColors.ControlText;
            buttonSilent.Image = Properties.Resources.icons8_bicycle_48__1_;
            buttonSilent.ImageAlign = ContentAlignment.BottomCenter;
            buttonSilent.Location = new Point(4, 12);
            buttonSilent.Margin = new Padding(4, 12, 4, 12);
            buttonSilent.Name = "buttonSilent";
            buttonSilent.Size = new Size(187, 126);
            buttonSilent.TabIndex = 0;
            buttonSilent.Text = "Silent";
            buttonSilent.TextImageRelation = TextImageRelation.ImageAboveText;
            buttonSilent.UseVisualStyleBackColor = false;
            // 
            // buttonBalanced
            // 
            buttonBalanced.Activated = false;
            buttonBalanced.BackColor = SystemColors.ControlLightLight;
            buttonBalanced.BorderColor = Color.Transparent;
            buttonBalanced.Dock = DockStyle.Fill;
            buttonBalanced.FlatAppearance.BorderColor = Color.FromArgb(0, 0, 192);
            buttonBalanced.FlatAppearance.BorderSize = 0;
            buttonBalanced.FlatStyle = FlatStyle.Flat;
            buttonBalanced.ForeColor = SystemColors.ControlText;
            buttonBalanced.Image = Properties.Resources.icons8_fiat_500_48;
            buttonBalanced.ImageAlign = ContentAlignment.BottomCenter;
            buttonBalanced.Location = new Point(199, 12);
            buttonBalanced.Margin = new Padding(4, 12, 4, 12);
            buttonBalanced.Name = "buttonBalanced";
            buttonBalanced.Size = new Size(187, 126);
            buttonBalanced.TabIndex = 1;
            buttonBalanced.Text = "Balanced";
            buttonBalanced.TextImageRelation = TextImageRelation.ImageAboveText;
            buttonBalanced.UseVisualStyleBackColor = false;
            // 
            // buttonTurbo
            // 
            buttonTurbo.Activated = false;
            buttonTurbo.BackColor = SystemColors.ControlLightLight;
            buttonTurbo.BorderColor = Color.Transparent;
            buttonTurbo.Dock = DockStyle.Fill;
            buttonTurbo.FlatAppearance.BorderColor = Color.FromArgb(192, 0, 0);
            buttonTurbo.FlatAppearance.BorderSize = 0;
            buttonTurbo.FlatStyle = FlatStyle.Flat;
            buttonTurbo.ForeColor = SystemColors.ControlText;
            buttonTurbo.Image = Properties.Resources.icons8_rocket_48;
            buttonTurbo.ImageAlign = ContentAlignment.BottomCenter;
            buttonTurbo.Location = new Point(394, 12);
            buttonTurbo.Margin = new Padding(4, 12, 4, 12);
            buttonTurbo.Name = "buttonTurbo";
            buttonTurbo.Size = new Size(187, 126);
            buttonTurbo.TabIndex = 2;
            buttonTurbo.Text = "Turbo";
            buttonTurbo.TextImageRelation = TextImageRelation.ImageAboveText;
            buttonTurbo.UseVisualStyleBackColor = false;
            // 
            // buttonFans
            // 
            buttonFans.BackColor = SystemColors.ButtonFace;
            buttonFans.Dock = DockStyle.Fill;
            buttonFans.FlatAppearance.BorderSize = 0;
            buttonFans.Image = Properties.Resources.icons8_fan_48;
            buttonFans.ImageAlign = ContentAlignment.BottomCenter;
            buttonFans.Location = new Point(589, 12);
            buttonFans.Margin = new Padding(4, 12, 4, 12);
            buttonFans.Name = "buttonFans";
            buttonFans.Size = new Size(187, 126);
            buttonFans.TabIndex = 35;
            buttonFans.Text = "Fans + Power";
            buttonFans.TextImageRelation = TextImageRelation.ImageAboveText;
            buttonFans.UseVisualStyleBackColor = false;
            // 
            // panelGPU
            // 
            panelGPU.Controls.Add(pictureGPU);
            panelGPU.Controls.Add(labelGPU);
            panelGPU.Controls.Add(labelGPUFan);
            panelGPU.Controls.Add(tableGPU);
            panelGPU.Dock = DockStyle.Top;
            panelGPU.Location = new Point(16, 226);
            panelGPU.Margin = new Padding(4);
            panelGPU.Name = "panelGPU";
            panelGPU.Size = new Size(818, 210);
            panelGPU.TabIndex = 37;
            // 
            // pictureGPU
            // 
            pictureGPU.BackgroundImage = (Image)resources.GetObject("pictureGPU.BackgroundImage");
            pictureGPU.BackgroundImageLayout = ImageLayout.Zoom;
            pictureGPU.Location = new Point(25, 10);
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
            labelGPU.Location = new Point(66, 12);
            labelGPU.Margin = new Padding(4, 0, 4, 0);
            labelGPU.Name = "labelGPU";
            labelGPU.Size = new Size(136, 32);
            labelGPU.TabIndex = 18;
            labelGPU.Text = "GPU Mode";
            // 
            // labelGPUFan
            // 
            labelGPUFan.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelGPUFan.Location = new Point(442, 10);
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
            tableGPU.AutoScroll = true;
            tableGPU.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableGPU.ColumnCount = 4;
            tableGPU.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableGPU.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableGPU.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableGPU.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableGPU.Controls.Add(buttonEco, 0, 0);
            tableGPU.Controls.Add(buttonStandard, 1, 0);
            tableGPU.Controls.Add(buttonUltimate, 2, 0);
            tableGPU.Controls.Add(buttonOptimized, 3, 0);
            tableGPU.Location = new Point(15, 48);
            tableGPU.Margin = new Padding(4, 2, 4, 2);
            tableGPU.Name = "tableGPU";
            tableGPU.RowCount = 1;
            tableGPU.RowStyles.Add(new RowStyle(SizeType.Absolute, 108F));
            tableGPU.Size = new Size(776, 150);
            tableGPU.TabIndex = 16;
            // 
            // buttonOptimized
            // 
            buttonOptimized.Activated = false;
            buttonOptimized.BackColor = SystemColors.ControlLightLight;
            buttonOptimized.BorderColor = Color.Transparent;
            buttonOptimized.Dock = DockStyle.Fill;
            buttonOptimized.FlatAppearance.BorderSize = 0;
            buttonOptimized.FlatStyle = FlatStyle.Flat;
            buttonOptimized.ForeColor = SystemColors.ControlText;
            buttonOptimized.Image = Properties.Resources.icons8_project_management_48__1_;
            buttonOptimized.ImageAlign = ContentAlignment.BottomCenter;
            buttonOptimized.Location = new Point(4, 12);
            buttonOptimized.Margin = new Padding(4, 12, 4, 12);
            buttonOptimized.Name = "buttonOptimized";
            buttonOptimized.Size = new Size(186, 126);
            buttonOptimized.TabIndex = 3;
            buttonOptimized.Text = "Optimized";
            buttonOptimized.TextImageRelation = TextImageRelation.ImageAboveText;
            buttonOptimized.UseVisualStyleBackColor = false;
            // 
            // buttonEco
            // 
            buttonEco.Activated = false;
            buttonEco.BackColor = SystemColors.ControlLightLight;
            buttonEco.BorderColor = Color.Transparent;
            buttonEco.CausesValidation = false;
            buttonEco.Dock = DockStyle.Fill;
            buttonEco.FlatAppearance.BorderSize = 0;
            buttonEco.FlatStyle = FlatStyle.Flat;
            buttonEco.ForeColor = SystemColors.ControlText;
            buttonEco.Image = Properties.Resources.icons8_leaf_48;
            buttonEco.ImageAlign = ContentAlignment.BottomCenter;
            buttonEco.Location = new Point(198, 12);
            buttonEco.Margin = new Padding(4, 12, 4, 12);
            buttonEco.Name = "buttonEco";
            buttonEco.Size = new Size(186, 126);
            buttonEco.TabIndex = 0;
            buttonEco.Text = "Eco";
            buttonEco.TextImageRelation = TextImageRelation.ImageAboveText;
            buttonEco.UseVisualStyleBackColor = false;
            // 
            // buttonStandard
            // 
            buttonStandard.Activated = false;
            buttonStandard.BackColor = SystemColors.ControlLightLight;
            buttonStandard.BorderColor = Color.Transparent;
            buttonStandard.Dock = DockStyle.Fill;
            buttonStandard.FlatAppearance.BorderSize = 0;
            buttonStandard.FlatStyle = FlatStyle.Flat;
            buttonStandard.ForeColor = SystemColors.ControlText;
            buttonStandard.Image = Properties.Resources.icons8_spa_flower_48;
            buttonStandard.ImageAlign = ContentAlignment.BottomCenter;
            buttonStandard.Location = new Point(392, 12);
            buttonStandard.Margin = new Padding(4, 12, 4, 12);
            buttonStandard.Name = "buttonStandard";
            buttonStandard.Size = new Size(186, 126);
            buttonStandard.TabIndex = 1;
            buttonStandard.Text = "Standard";
            buttonStandard.TextImageRelation = TextImageRelation.ImageAboveText;
            buttonStandard.UseVisualStyleBackColor = false;
            // 
            // buttonUltimate
            // 
            buttonUltimate.Activated = false;
            buttonUltimate.BackColor = SystemColors.ControlLightLight;
            buttonUltimate.BorderColor = Color.Transparent;
            buttonUltimate.Dock = DockStyle.Fill;
            buttonUltimate.FlatAppearance.BorderSize = 0;
            buttonUltimate.FlatStyle = FlatStyle.Flat;
            buttonUltimate.ForeColor = SystemColors.ControlText;
            buttonUltimate.Image = Properties.Resources.icons8_game_controller_48;
            buttonUltimate.ImageAlign = ContentAlignment.BottomCenter;
            buttonUltimate.Location = new Point(586, 12);
            buttonUltimate.Margin = new Padding(4, 12, 4, 12);
            buttonUltimate.Name = "buttonUltimate";
            buttonUltimate.Size = new Size(186, 126);
            buttonUltimate.TabIndex = 2;
            buttonUltimate.Text = "Ultimate";
            buttonUltimate.TextImageRelation = TextImageRelation.ImageAboveText;
            buttonUltimate.UseVisualStyleBackColor = false;
            // 
            // panelScreen
            // 
            panelScreen.Controls.Add(tableScreen);
            panelScreen.Controls.Add(pictureScreen);
            panelScreen.Controls.Add(labelSreen);
            panelScreen.Dock = DockStyle.Top;
            panelScreen.Location = new Point(16, 436);
            panelScreen.Margin = new Padding(4);
            panelScreen.Name = "panelScreen";
            panelScreen.Size = new Size(818, 182);
            panelScreen.TabIndex = 38;
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
            tableScreen.Location = new Point(15, 48);
            tableScreen.Margin = new Padding(4, 2, 4, 2);
            tableScreen.Name = "tableScreen";
            tableScreen.RowCount = 1;
            tableScreen.RowStyles.Add(new RowStyle(SizeType.Absolute, 108F));
            tableScreen.Size = new Size(776, 108);
            tableScreen.TabIndex = 23;
            // 
            // buttonScreenAuto
            // 
            buttonScreenAuto.Activated = false;
            buttonScreenAuto.BackColor = SystemColors.ControlLightLight;
            buttonScreenAuto.BorderColor = Color.Transparent;
            buttonScreenAuto.Dock = DockStyle.Fill;
            buttonScreenAuto.FlatAppearance.BorderSize = 0;
            buttonScreenAuto.FlatStyle = FlatStyle.Flat;
            buttonScreenAuto.ForeColor = SystemColors.ControlText;
            buttonScreenAuto.Location = new Point(4, 12);
            buttonScreenAuto.Margin = new Padding(4, 12, 4, 12);
            buttonScreenAuto.Name = "buttonScreenAuto";
            buttonScreenAuto.Size = new Size(186, 84);
            buttonScreenAuto.TabIndex = 0;
            buttonScreenAuto.Text = "Auto";
            buttonScreenAuto.UseVisualStyleBackColor = false;
            // 
            // button60Hz
            // 
            button60Hz.Activated = false;
            button60Hz.BackColor = SystemColors.ControlLightLight;
            button60Hz.BorderColor = Color.Transparent;
            button60Hz.CausesValidation = false;
            button60Hz.Dock = DockStyle.Fill;
            button60Hz.FlatAppearance.BorderSize = 0;
            button60Hz.FlatStyle = FlatStyle.Flat;
            button60Hz.ForeColor = SystemColors.ControlText;
            button60Hz.Location = new Point(198, 12);
            button60Hz.Margin = new Padding(4, 12, 4, 12);
            button60Hz.Name = "button60Hz";
            button60Hz.Size = new Size(186, 84);
            button60Hz.TabIndex = 1;
            button60Hz.Text = "60Hz";
            button60Hz.UseVisualStyleBackColor = false;
            // 
            // button120Hz
            // 
            button120Hz.Activated = false;
            button120Hz.BackColor = SystemColors.ControlLightLight;
            button120Hz.BorderColor = Color.Transparent;
            button120Hz.Dock = DockStyle.Fill;
            button120Hz.FlatAppearance.BorderSize = 0;
            button120Hz.FlatStyle = FlatStyle.Flat;
            button120Hz.ForeColor = SystemColors.ControlText;
            button120Hz.Location = new Point(392, 12);
            button120Hz.Margin = new Padding(4, 12, 4, 12);
            button120Hz.Name = "button120Hz";
            button120Hz.Size = new Size(186, 84);
            button120Hz.TabIndex = 2;
            button120Hz.Text = "120Hz + OD";
            button120Hz.UseVisualStyleBackColor = false;
            // 
            // pictureScreen
            // 
            pictureScreen.BackgroundImage = (Image)resources.GetObject("pictureScreen.BackgroundImage");
            pictureScreen.BackgroundImageLayout = ImageLayout.Zoom;
            pictureScreen.Location = new Point(25, 8);
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
            labelSreen.Location = new Point(68, 10);
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
            panelKeyboard.Location = new Point(16, 618);
            panelKeyboard.Margin = new Padding(4);
            panelKeyboard.Name = "panelKeyboard";
            panelKeyboard.Size = new Size(818, 146);
            panelKeyboard.TabIndex = 39;
            // 
            // tableLayoutKeyboard
            // 
            tableLayoutKeyboard.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutKeyboard.ColumnCount = 4;
            tableLayoutKeyboard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutKeyboard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutKeyboard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutKeyboard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutKeyboard.Controls.Add(buttonKeyboard, 2, 0);
            tableLayoutKeyboard.Controls.Add(comboKeyboard, 0, 0);
            tableLayoutKeyboard.Controls.Add(panelColor, 1, 0);
            tableLayoutKeyboard.Location = new Point(15, 56);
            tableLayoutKeyboard.Margin = new Padding(4);
            tableLayoutKeyboard.Name = "tableLayoutKeyboard";
            tableLayoutKeyboard.RowCount = 1;
            tableLayoutKeyboard.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutKeyboard.Size = new Size(780, 66);
            tableLayoutKeyboard.TabIndex = 39;
            // 
            // buttonKeyboard
            // 
            buttonKeyboard.BackColor = SystemColors.ButtonFace;
            buttonKeyboard.Dock = DockStyle.Top;
            buttonKeyboard.FlatAppearance.BorderSize = 0;
            buttonKeyboard.Location = new Point(395, 10);
            buttonKeyboard.Margin = new Padding(5, 10, 5, 10);
            buttonKeyboard.Name = "buttonKeyboard";
            buttonKeyboard.Size = new Size(185, 42);
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
            comboKeyboard.Location = new Point(5, 10);
            comboKeyboard.Margin = new Padding(5, 10, 5, 10);
            comboKeyboard.Name = "comboKeyboard";
            comboKeyboard.Size = new Size(185, 40);
            comboKeyboard.TabIndex = 35;
            comboKeyboard.TabStop = false;
            // 
            // panelColor
            // 
            panelColor.Controls.Add(pictureColor2);
            panelColor.Controls.Add(pictureColor);
            panelColor.Controls.Add(buttonKeyboardColor);
            panelColor.Dock = DockStyle.Fill;
            panelColor.Location = new Point(200, 10);
            panelColor.Margin = new Padding(5, 10, 5, 10);
            panelColor.Name = "panelColor";
            panelColor.Size = new Size(185, 46);
            panelColor.TabIndex = 36;
            // 
            // pictureColor2
            // 
            pictureColor2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pictureColor2.Location = new Point(125, 12);
            pictureColor2.Margin = new Padding(4);
            pictureColor2.Name = "pictureColor2";
            pictureColor2.Size = new Size(20, 20);
            pictureColor2.TabIndex = 41;
            pictureColor2.TabStop = false;
            // 
            // pictureColor
            // 
            pictureColor.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pictureColor.Location = new Point(153, 12);
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
            buttonKeyboardColor.Size = new Size(185, 42);
            buttonKeyboardColor.TabIndex = 39;
            buttonKeyboardColor.Text = "Color   ";
            buttonKeyboardColor.UseVisualStyleBackColor = false;
            // 
            // pictureKeyboard
            // 
            pictureKeyboard.BackgroundImage = Properties.Resources.icons8_keyboard_48;
            pictureKeyboard.BackgroundImageLayout = ImageLayout.Zoom;
            pictureKeyboard.Location = new Point(27, 16);
            pictureKeyboard.Margin = new Padding(4, 2, 4, 2);
            pictureKeyboard.Name = "pictureKeyboard";
            pictureKeyboard.Size = new Size(32, 36);
            pictureKeyboard.TabIndex = 33;
            pictureKeyboard.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(68, 16);
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
            ClientSize = new Size(850, 1217);
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
            MinimumSize = new Size(850, 0);
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
        private PictureBox picturePerf;
        private Label labelPerf;
        private Label labelCPUFan;
        private TableLayoutPanel tablePerf;
        private RoundedButton buttonTurbo;
        private RoundedButton buttonBalanced;
        private RoundedButton buttonSilent;
        private Panel panelGPU;
        private PictureBox pictureGPU;
        private Label labelGPU;
        private Label labelGPUFan;
        private TableLayoutPanel tableGPU;
        private RoundedButton buttonUltimate;
        private RoundedButton buttonStandard;
        private RoundedButton buttonEco;
        private Panel panelScreen;
        private TableLayoutPanel tableScreen;
        private RoundedButton buttonScreenAuto;
        private RoundedButton button60Hz;
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
        private RoundedButton button120Hz;
        private Button buttonFans;
        private RoundedButton buttonOptimized;
    }
}