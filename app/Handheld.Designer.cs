namespace GHelper
{
    partial class Handheld
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
            panelController = new Panel();
            buttonReset = new UI.RButton();
            panelVibra = new Panel();
            labelVibra = new Label();
            labelVibraTitle = new Label();
            trackVibra = new TrackBar();
            panelVibrationTitle = new Panel();
            pictureVibration = new PictureBox();
            labelVibraHeader = new Label();
            panelRT = new Panel();
            trackRTMax = new TrackBar();
            labelRT = new Label();
            trackRTMin = new TrackBar();
            labelRTTitle = new Label();
            panelLT = new Panel();
            trackLTMax = new TrackBar();
            labelLT = new Label();
            trackLTMin = new TrackBar();
            labelLTTitle = new Label();
            panelTDeadzone = new Panel();
            pictureTDeadzone = new PictureBox();
            labelTDeadzone = new Label();
            panelRS = new Panel();
            trackRSMax = new TrackBar();
            labelRS = new Label();
            trackRSMin = new TrackBar();
            labelRSTitle = new Label();
            panelLS = new Panel();
            trackLSMax = new TrackBar();
            labelLS = new Label();
            trackLSMin = new TrackBar();
            labelLSTitle = new Label();
            panelSDeadzone = new Panel();
            pictureSDeadzone = new PictureBox();
            labelSDeadzone = new Label();
            panelBindings = new Panel();
            tableBindings = new TableLayoutPanel();
            labelPrimary = new Label();
            labelSecondary = new Label();
            panelBindingsTitle = new Panel();
            pictureBindings = new PictureBox();
            labelBindings = new Label();
            panelController.SuspendLayout();
            panelVibra.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackVibra).BeginInit();
            panelVibrationTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureVibration).BeginInit();
            panelRT.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackRTMax).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackRTMin).BeginInit();
            panelLT.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackLTMax).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackLTMin).BeginInit();
            panelTDeadzone.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureTDeadzone).BeginInit();
            panelRS.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackRSMax).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackRSMin).BeginInit();
            panelLS.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackLSMax).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackLSMin).BeginInit();
            panelSDeadzone.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureSDeadzone).BeginInit();
            panelBindings.SuspendLayout();
            tableBindings.SuspendLayout();
            panelBindingsTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBindings).BeginInit();
            SuspendLayout();
            // 
            // panelController
            // 
            panelController.AutoSize = true;
            panelController.Controls.Add(buttonReset);
            panelController.Controls.Add(panelVibra);
            panelController.Controls.Add(panelVibrationTitle);
            panelController.Controls.Add(panelRT);
            panelController.Controls.Add(panelLT);
            panelController.Controls.Add(panelTDeadzone);
            panelController.Controls.Add(panelRS);
            panelController.Controls.Add(panelLS);
            panelController.Controls.Add(panelSDeadzone);
            panelController.Dock = DockStyle.Left;
            panelController.Location = new Point(10, 10);
            panelController.Margin = new Padding(4);
            panelController.MinimumSize = new Size(560, 800);
            panelController.Name = "panelController";
            panelController.Padding = new Padding(0, 0, 0, 18);
            panelController.Size = new Size(560, 912);
            panelController.TabIndex = 45;
            // 
            // buttonReset
            // 
            buttonReset.Activated = false;
            buttonReset.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonReset.BackColor = SystemColors.ControlLight;
            buttonReset.BorderColor = Color.Transparent;
            buttonReset.BorderRadius = 2;
            buttonReset.FlatStyle = FlatStyle.Flat;
            buttonReset.Location = new Point(20, 840);
            buttonReset.Margin = new Padding(4, 2, 4, 2);
            buttonReset.Name = "buttonReset";
            buttonReset.Secondary = true;
            buttonReset.Size = new Size(239, 50);
            buttonReset.TabIndex = 54;
            buttonReset.Text = "Reset";
            buttonReset.UseVisualStyleBackColor = false;
            // 
            // panelVibra
            // 
            panelVibra.AutoSize = true;
            panelVibra.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelVibra.Controls.Add(labelVibra);
            panelVibra.Controls.Add(labelVibraTitle);
            panelVibra.Controls.Add(trackVibra);
            panelVibra.Dock = DockStyle.Top;
            panelVibra.Location = new Point(0, 676);
            panelVibra.Margin = new Padding(4);
            panelVibra.MaximumSize = new Size(0, 124);
            panelVibra.Name = "panelVibra";
            panelVibra.Size = new Size(560, 124);
            panelVibra.TabIndex = 46;
            // 
            // labelVibra
            // 
            labelVibra.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelVibra.Location = new Point(408, 14);
            labelVibra.Margin = new Padding(4, 0, 4, 0);
            labelVibra.Name = "labelVibra";
            labelVibra.Size = new Size(124, 32);
            labelVibra.TabIndex = 44;
            labelVibra.Text = "100%";
            labelVibra.TextAlign = ContentAlignment.TopRight;
            // 
            // labelVibraTitle
            // 
            labelVibraTitle.AutoSize = true;
            labelVibraTitle.Location = new Point(10, 14);
            labelVibraTitle.Margin = new Padding(4, 0, 4, 0);
            labelVibraTitle.Name = "labelVibraTitle";
            labelVibraTitle.Size = new Size(209, 32);
            labelVibraTitle.TabIndex = 43;
            labelVibraTitle.Text = "Vibration Strength";
            // 
            // trackVibra
            // 
            trackVibra.Location = new Point(6, 48);
            trackVibra.Margin = new Padding(4, 2, 4, 2);
            trackVibra.Maximum = 100;
            trackVibra.Name = "trackVibra";
            trackVibra.Size = new Size(546, 90);
            trackVibra.TabIndex = 42;
            trackVibra.TickFrequency = 5;
            trackVibra.TickStyle = TickStyle.TopLeft;
            trackVibra.Value = 100;
            // 
            // panelVibrationTitle
            // 
            panelVibrationTitle.AutoSize = true;
            panelVibrationTitle.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelVibrationTitle.Controls.Add(pictureVibration);
            panelVibrationTitle.Controls.Add(labelVibraHeader);
            panelVibrationTitle.Dock = DockStyle.Top;
            panelVibrationTitle.Location = new Point(0, 616);
            panelVibrationTitle.Margin = new Padding(4);
            panelVibrationTitle.Name = "panelVibrationTitle";
            panelVibrationTitle.Size = new Size(560, 60);
            panelVibrationTitle.TabIndex = 53;
            // 
            // pictureVibration
            // 
            pictureVibration.BackgroundImage = Properties.Resources.icons8_soonvibes_32;
            pictureVibration.BackgroundImageLayout = ImageLayout.Zoom;
            pictureVibration.ErrorImage = null;
            pictureVibration.InitialImage = null;
            pictureVibration.Location = new Point(10, 18);
            pictureVibration.Margin = new Padding(4, 2, 4, 10);
            pictureVibration.Name = "pictureVibration";
            pictureVibration.Size = new Size(32, 32);
            pictureVibration.TabIndex = 41;
            pictureVibration.TabStop = false;
            // 
            // labelVibraHeader
            // 
            labelVibraHeader.AutoSize = true;
            labelVibraHeader.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelVibraHeader.Location = new Point(45, 17);
            labelVibraHeader.Margin = new Padding(4, 0, 4, 0);
            labelVibraHeader.Name = "labelVibraHeader";
            labelVibraHeader.Size = new Size(121, 32);
            labelVibraHeader.TabIndex = 40;
            labelVibraHeader.Text = "Vibration";
            // 
            // panelRT
            // 
            panelRT.AutoSize = true;
            panelRT.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelRT.Controls.Add(trackRTMax);
            panelRT.Controls.Add(labelRT);
            panelRT.Controls.Add(trackRTMin);
            panelRT.Controls.Add(labelRTTitle);
            panelRT.Dock = DockStyle.Top;
            panelRT.Location = new Point(0, 492);
            panelRT.Margin = new Padding(4);
            panelRT.MaximumSize = new Size(0, 124);
            panelRT.Name = "panelRT";
            panelRT.Size = new Size(560, 124);
            panelRT.TabIndex = 50;
            // 
            // trackRTMax
            // 
            trackRTMax.Location = new Point(272, 48);
            trackRTMax.Margin = new Padding(4, 2, 4, 2);
            trackRTMax.Maximum = 100;
            trackRTMax.Minimum = 50;
            trackRTMax.Name = "trackRTMax";
            trackRTMax.RightToLeft = RightToLeft.No;
            trackRTMax.Size = new Size(280, 90);
            trackRTMax.TabIndex = 30;
            trackRTMax.TickFrequency = 5;
            trackRTMax.TickStyle = TickStyle.TopLeft;
            trackRTMax.Value = 100;
            // 
            // labelRT
            // 
            labelRT.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelRT.Location = new Point(363, 13);
            labelRT.Margin = new Padding(4, 0, 4, 0);
            labelRT.Name = "labelRT";
            labelRT.Size = new Size(176, 32);
            labelRT.TabIndex = 29;
            labelRT.Text = "0 - 100%";
            labelRT.TextAlign = ContentAlignment.TopRight;
            // 
            // trackRTMin
            // 
            trackRTMin.LargeChange = 100;
            trackRTMin.Location = new Point(6, 48);
            trackRTMin.Margin = new Padding(4, 2, 4, 2);
            trackRTMin.Maximum = 50;
            trackRTMin.Name = "trackRTMin";
            trackRTMin.RightToLeft = RightToLeft.No;
            trackRTMin.Size = new Size(280, 90);
            trackRTMin.SmallChange = 10;
            trackRTMin.TabIndex = 18;
            trackRTMin.TickFrequency = 5;
            trackRTMin.TickStyle = TickStyle.TopLeft;
            // 
            // labelRTTitle
            // 
            labelRTTitle.AutoSize = true;
            labelRTTitle.Location = new Point(10, 16);
            labelRTTitle.Margin = new Padding(4, 0, 4, 0);
            labelRTTitle.Name = "labelRTTitle";
            labelRTTitle.Size = new Size(151, 32);
            labelRTTitle.TabIndex = 17;
            labelRTTitle.Text = "Right Trigger";
            // 
            // panelLT
            // 
            panelLT.AutoSize = true;
            panelLT.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelLT.Controls.Add(trackLTMax);
            panelLT.Controls.Add(labelLT);
            panelLT.Controls.Add(trackLTMin);
            panelLT.Controls.Add(labelLTTitle);
            panelLT.Dock = DockStyle.Top;
            panelLT.Location = new Point(0, 368);
            panelLT.Margin = new Padding(4);
            panelLT.MaximumSize = new Size(0, 124);
            panelLT.Name = "panelLT";
            panelLT.Size = new Size(560, 124);
            panelLT.TabIndex = 51;
            // 
            // trackLTMax
            // 
            trackLTMax.Location = new Point(272, 48);
            trackLTMax.Margin = new Padding(4, 2, 4, 2);
            trackLTMax.Maximum = 100;
            trackLTMax.Minimum = 50;
            trackLTMax.Name = "trackLTMax";
            trackLTMax.RightToLeft = RightToLeft.No;
            trackLTMax.Size = new Size(280, 90);
            trackLTMax.TabIndex = 30;
            trackLTMax.TickFrequency = 5;
            trackLTMax.TickStyle = TickStyle.TopLeft;
            trackLTMax.Value = 100;
            // 
            // labelLT
            // 
            labelLT.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelLT.Location = new Point(363, 13);
            labelLT.Margin = new Padding(4, 0, 4, 0);
            labelLT.Name = "labelLT";
            labelLT.Size = new Size(176, 32);
            labelLT.TabIndex = 29;
            labelLT.Text = "0 - 100%";
            labelLT.TextAlign = ContentAlignment.TopRight;
            // 
            // trackLTMin
            // 
            trackLTMin.LargeChange = 100;
            trackLTMin.Location = new Point(6, 48);
            trackLTMin.Margin = new Padding(4, 2, 4, 2);
            trackLTMin.Maximum = 50;
            trackLTMin.Name = "trackLTMin";
            trackLTMin.RightToLeft = RightToLeft.No;
            trackLTMin.Size = new Size(280, 90);
            trackLTMin.SmallChange = 10;
            trackLTMin.TabIndex = 18;
            trackLTMin.TickFrequency = 5;
            trackLTMin.TickStyle = TickStyle.TopLeft;
            // 
            // labelLTTitle
            // 
            labelLTTitle.AutoSize = true;
            labelLTTitle.Location = new Point(10, 16);
            labelLTTitle.Margin = new Padding(4, 0, 4, 0);
            labelLTTitle.Name = "labelLTTitle";
            labelLTTitle.Size = new Size(135, 32);
            labelLTTitle.TabIndex = 17;
            labelLTTitle.Text = "Left Trigger";
            // 
            // panelTDeadzone
            // 
            panelTDeadzone.AutoSize = true;
            panelTDeadzone.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelTDeadzone.Controls.Add(pictureTDeadzone);
            panelTDeadzone.Controls.Add(labelTDeadzone);
            panelTDeadzone.Dock = DockStyle.Top;
            panelTDeadzone.Location = new Point(0, 308);
            panelTDeadzone.Margin = new Padding(4);
            panelTDeadzone.Name = "panelTDeadzone";
            panelTDeadzone.Size = new Size(560, 60);
            panelTDeadzone.TabIndex = 52;
            // 
            // pictureTDeadzone
            // 
            pictureTDeadzone.BackgroundImage = Properties.Resources.icons8_xbox_lt_32;
            pictureTDeadzone.BackgroundImageLayout = ImageLayout.Zoom;
            pictureTDeadzone.ErrorImage = null;
            pictureTDeadzone.InitialImage = null;
            pictureTDeadzone.Location = new Point(10, 18);
            pictureTDeadzone.Margin = new Padding(4, 2, 4, 10);
            pictureTDeadzone.Name = "pictureTDeadzone";
            pictureTDeadzone.Size = new Size(32, 32);
            pictureTDeadzone.TabIndex = 41;
            pictureTDeadzone.TabStop = false;
            // 
            // labelTDeadzone
            // 
            labelTDeadzone.AutoSize = true;
            labelTDeadzone.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelTDeadzone.Location = new Point(45, 17);
            labelTDeadzone.Margin = new Padding(4, 0, 4, 0);
            labelTDeadzone.Name = "labelTDeadzone";
            labelTDeadzone.Size = new Size(228, 32);
            labelTDeadzone.TabIndex = 40;
            labelTDeadzone.Text = "Trigger Deadzones";
            // 
            // panelRS
            // 
            panelRS.AutoSize = true;
            panelRS.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelRS.Controls.Add(trackRSMax);
            panelRS.Controls.Add(labelRS);
            panelRS.Controls.Add(trackRSMin);
            panelRS.Controls.Add(labelRSTitle);
            panelRS.Dock = DockStyle.Top;
            panelRS.Location = new Point(0, 184);
            panelRS.Margin = new Padding(4);
            panelRS.MaximumSize = new Size(0, 124);
            panelRS.Name = "panelRS";
            panelRS.Size = new Size(560, 124);
            panelRS.TabIndex = 49;
            // 
            // trackRSMax
            // 
            trackRSMax.Location = new Point(272, 48);
            trackRSMax.Margin = new Padding(4, 2, 4, 2);
            trackRSMax.Maximum = 100;
            trackRSMax.Minimum = 50;
            trackRSMax.Name = "trackRSMax";
            trackRSMax.RightToLeft = RightToLeft.No;
            trackRSMax.Size = new Size(280, 90);
            trackRSMax.TabIndex = 30;
            trackRSMax.TickFrequency = 5;
            trackRSMax.TickStyle = TickStyle.TopLeft;
            trackRSMax.Value = 100;
            // 
            // labelRS
            // 
            labelRS.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelRS.Location = new Point(363, 13);
            labelRS.Margin = new Padding(4, 0, 4, 0);
            labelRS.Name = "labelRS";
            labelRS.Size = new Size(176, 32);
            labelRS.TabIndex = 29;
            labelRS.Text = "0 - 100%";
            labelRS.TextAlign = ContentAlignment.TopRight;
            // 
            // trackRSMin
            // 
            trackRSMin.LargeChange = 100;
            trackRSMin.Location = new Point(6, 48);
            trackRSMin.Margin = new Padding(4, 2, 4, 2);
            trackRSMin.Maximum = 50;
            trackRSMin.Name = "trackRSMin";
            trackRSMin.RightToLeft = RightToLeft.No;
            trackRSMin.Size = new Size(280, 90);
            trackRSMin.SmallChange = 10;
            trackRSMin.TabIndex = 18;
            trackRSMin.TickFrequency = 5;
            trackRSMin.TickStyle = TickStyle.TopLeft;
            // 
            // labelRSTitle
            // 
            labelRSTitle.AutoSize = true;
            labelRSTitle.Location = new Point(10, 16);
            labelRSTitle.Margin = new Padding(4, 0, 4, 0);
            labelRSTitle.Name = "labelRSTitle";
            labelRSTitle.Size = new Size(126, 32);
            labelRSTitle.TabIndex = 17;
            labelRSTitle.Text = "Right Stick";
            // 
            // panelLS
            // 
            panelLS.AutoSize = true;
            panelLS.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelLS.Controls.Add(trackLSMax);
            panelLS.Controls.Add(labelLS);
            panelLS.Controls.Add(trackLSMin);
            panelLS.Controls.Add(labelLSTitle);
            panelLS.Dock = DockStyle.Top;
            panelLS.Location = new Point(0, 60);
            panelLS.Margin = new Padding(4);
            panelLS.MaximumSize = new Size(0, 124);
            panelLS.Name = "panelLS";
            panelLS.Size = new Size(560, 124);
            panelLS.TabIndex = 48;
            // 
            // trackLSMax
            // 
            trackLSMax.Location = new Point(272, 48);
            trackLSMax.Margin = new Padding(4, 2, 4, 2);
            trackLSMax.Maximum = 100;
            trackLSMax.Minimum = 50;
            trackLSMax.Name = "trackLSMax";
            trackLSMax.RightToLeft = RightToLeft.No;
            trackLSMax.Size = new Size(280, 90);
            trackLSMax.TabIndex = 30;
            trackLSMax.TickFrequency = 5;
            trackLSMax.TickStyle = TickStyle.TopLeft;
            trackLSMax.Value = 100;
            // 
            // labelLS
            // 
            labelLS.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelLS.Location = new Point(363, 13);
            labelLS.Margin = new Padding(4, 0, 4, 0);
            labelLS.Name = "labelLS";
            labelLS.Size = new Size(176, 32);
            labelLS.TabIndex = 29;
            labelLS.Text = "0 - 100%";
            labelLS.TextAlign = ContentAlignment.TopRight;
            // 
            // trackLSMin
            // 
            trackLSMin.LargeChange = 100;
            trackLSMin.Location = new Point(6, 48);
            trackLSMin.Margin = new Padding(4, 2, 4, 2);
            trackLSMin.Maximum = 50;
            trackLSMin.Name = "trackLSMin";
            trackLSMin.RightToLeft = RightToLeft.No;
            trackLSMin.Size = new Size(280, 90);
            trackLSMin.SmallChange = 10;
            trackLSMin.TabIndex = 18;
            trackLSMin.TickFrequency = 5;
            trackLSMin.TickStyle = TickStyle.TopLeft;
            // 
            // labelLSTitle
            // 
            labelLSTitle.AutoSize = true;
            labelLSTitle.Location = new Point(10, 16);
            labelLSTitle.Margin = new Padding(4, 0, 4, 0);
            labelLSTitle.Name = "labelLSTitle";
            labelLSTitle.Size = new Size(110, 32);
            labelLSTitle.TabIndex = 17;
            labelLSTitle.Text = "Left Stick";
            // 
            // panelSDeadzone
            // 
            panelSDeadzone.AutoSize = true;
            panelSDeadzone.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelSDeadzone.Controls.Add(pictureSDeadzone);
            panelSDeadzone.Controls.Add(labelSDeadzone);
            panelSDeadzone.Dock = DockStyle.Top;
            panelSDeadzone.Location = new Point(0, 0);
            panelSDeadzone.Margin = new Padding(4);
            panelSDeadzone.Name = "panelSDeadzone";
            panelSDeadzone.Size = new Size(560, 60);
            panelSDeadzone.TabIndex = 43;
            // 
            // pictureSDeadzone
            // 
            pictureSDeadzone.BackgroundImage = Properties.Resources.icons8_joystick_32;
            pictureSDeadzone.BackgroundImageLayout = ImageLayout.Zoom;
            pictureSDeadzone.ErrorImage = null;
            pictureSDeadzone.InitialImage = null;
            pictureSDeadzone.Location = new Point(10, 18);
            pictureSDeadzone.Margin = new Padding(4, 2, 4, 10);
            pictureSDeadzone.Name = "pictureSDeadzone";
            pictureSDeadzone.Size = new Size(32, 32);
            pictureSDeadzone.TabIndex = 41;
            pictureSDeadzone.TabStop = false;
            // 
            // labelSDeadzone
            // 
            labelSDeadzone.AutoSize = true;
            labelSDeadzone.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelSDeadzone.Location = new Point(45, 17);
            labelSDeadzone.Margin = new Padding(4, 0, 4, 0);
            labelSDeadzone.Name = "labelSDeadzone";
            labelSDeadzone.Size = new Size(199, 32);
            labelSDeadzone.TabIndex = 40;
            labelSDeadzone.Text = "Stick Deadzones";
            // 
            // panelBindings
            // 
            panelBindings.Controls.Add(tableBindings);
            panelBindings.Controls.Add(panelBindingsTitle);
            panelBindings.Dock = DockStyle.Left;
            panelBindings.Location = new Point(570, 10);
            panelBindings.MinimumSize = new Size(700, 0);
            panelBindings.Name = "panelBindings";
            panelBindings.Size = new Size(700, 912);
            panelBindings.TabIndex = 46;
            // 
            // tableBindings
            // 
            tableBindings.AutoSize = true;
            tableBindings.ColumnCount = 3;
            tableBindings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableBindings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableBindings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableBindings.Controls.Add(labelPrimary, 1, 0);
            tableBindings.Controls.Add(labelSecondary, 2, 0);
            tableBindings.Dock = DockStyle.Top;
            tableBindings.Location = new Point(0, 60);
            tableBindings.Name = "tableBindings";
            tableBindings.Padding = new Padding(5);
            tableBindings.RowCount = 1;
            tableBindings.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableBindings.Size = new Size(700, 52);
            tableBindings.TabIndex = 49;
            // 
            // labelPrimary
            // 
            labelPrimary.AutoSize = true;
            labelPrimary.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelPrimary.Location = new Point(147, 5);
            labelPrimary.Margin = new Padding(4, 0, 4, 0);
            labelPrimary.Name = "labelPrimary";
            labelPrimary.Padding = new Padding(5);
            labelPrimary.Size = new Size(115, 42);
            labelPrimary.TabIndex = 41;
            labelPrimary.Text = "Primary";
            // 
            // labelSecondary
            // 
            labelSecondary.AutoSize = true;
            labelSecondary.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelSecondary.Location = new Point(423, 5);
            labelSecondary.Margin = new Padding(4, 0, 4, 0);
            labelSecondary.Name = "labelSecondary";
            labelSecondary.Padding = new Padding(5);
            labelSecondary.Size = new Size(144, 42);
            labelSecondary.TabIndex = 42;
            labelSecondary.Text = "Secondary";
            // 
            // panelBindingsTitle
            // 
            panelBindingsTitle.AutoSize = true;
            panelBindingsTitle.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelBindingsTitle.Controls.Add(pictureBindings);
            panelBindingsTitle.Controls.Add(labelBindings);
            panelBindingsTitle.Dock = DockStyle.Top;
            panelBindingsTitle.Location = new Point(0, 0);
            panelBindingsTitle.Margin = new Padding(4);
            panelBindingsTitle.Name = "panelBindingsTitle";
            panelBindingsTitle.Size = new Size(700, 60);
            panelBindingsTitle.TabIndex = 44;
            // 
            // pictureBindings
            // 
            pictureBindings.BackgroundImage = Properties.Resources.icons8_next_32;
            pictureBindings.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBindings.ErrorImage = null;
            pictureBindings.InitialImage = null;
            pictureBindings.Location = new Point(10, 18);
            pictureBindings.Margin = new Padding(4, 2, 4, 10);
            pictureBindings.Name = "pictureBindings";
            pictureBindings.Size = new Size(32, 32);
            pictureBindings.TabIndex = 41;
            pictureBindings.TabStop = false;
            // 
            // labelBindings
            // 
            labelBindings.AutoSize = true;
            labelBindings.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelBindings.Location = new Point(45, 17);
            labelBindings.Margin = new Padding(4, 0, 4, 0);
            labelBindings.Name = "labelBindings";
            labelBindings.Size = new Size(558, 32);
            labelBindings.TabIndex = 40;
            labelBindings.Text = "Bindings for Gamepad or Auto (in-game) Mode";
            // 
            // Handheld
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(1286, 932);
            Controls.Add(panelBindings);
            Controls.Add(panelController);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Handheld";
            Padding = new Padding(10);
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Controller";
            panelController.ResumeLayout(false);
            panelController.PerformLayout();
            panelVibra.ResumeLayout(false);
            panelVibra.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackVibra).EndInit();
            panelVibrationTitle.ResumeLayout(false);
            panelVibrationTitle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureVibration).EndInit();
            panelRT.ResumeLayout(false);
            panelRT.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackRTMax).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackRTMin).EndInit();
            panelLT.ResumeLayout(false);
            panelLT.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackLTMax).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackLTMin).EndInit();
            panelTDeadzone.ResumeLayout(false);
            panelTDeadzone.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureTDeadzone).EndInit();
            panelRS.ResumeLayout(false);
            panelRS.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackRSMax).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackRSMin).EndInit();
            panelLS.ResumeLayout(false);
            panelLS.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackLSMax).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackLSMin).EndInit();
            panelSDeadzone.ResumeLayout(false);
            panelSDeadzone.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureSDeadzone).EndInit();
            panelBindings.ResumeLayout(false);
            panelBindings.PerformLayout();
            tableBindings.ResumeLayout(false);
            tableBindings.PerformLayout();
            panelBindingsTitle.ResumeLayout(false);
            panelBindingsTitle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBindings).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panelController;
        private Panel panelVibra;
        private Label labelVibra;
        private Label labelVibraTitle;
        private TrackBar trackVibra;
        private Panel panelLS;
        private TrackBar trackLSMax;
        private Label labelLS;
        private TrackBar trackLSMin;
        private Label labelLSTitle;
        private Panel panelSDeadzone;
        private PictureBox pictureSDeadzone;
        private Label labelSDeadzone;
        private Panel panelRS;
        private TrackBar trackRSMax;
        private Label labelRS;
        private TrackBar trackRSMin;
        private Label labelRSTitle;
        private Panel panelRT;
        private TrackBar trackRTMax;
        private Label labelRT;
        private TrackBar trackRTMin;
        private Label labelRTTitle;
        private Panel panelLT;
        private TrackBar trackLTMax;
        private Label labelLT;
        private TrackBar trackLTMin;
        private Label labelLTTitle;
        private Panel panelTDeadzone;
        private PictureBox pictureTDeadzone;
        private Label labelTDeadzone;
        private Panel panelVibrationTitle;
        private PictureBox pictureVibration;
        private Label labelVibraHeader;
        private UI.RButton buttonReset;
        private Panel panelBindings;
        private Panel panelBindingsTitle;
        private PictureBox pictureBindings;
        private Label labelBindings;
        private TableLayoutPanel tableBindings;
        private Label labelPrimary;
        private Label labelSecondary;
    }
}