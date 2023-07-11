using GHelper.Properties;
using GHelper.UI;

namespace GHelper
{
    partial class Extra
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
            panelServices = new Panel();
            pictureService = new PictureBox();
            labelServices = new Label();
            buttonServices = new RButton();
            panelBindingsHeader = new Panel();
            pictureBindings = new PictureBox();
            pictureHelp = new PictureBox();
            labelBindings = new Label();
            panelBindings = new Panel();
            tableBindings = new TableLayoutPanel();
            label1 = new Label();
            textBox1 = new TextBox();
            textM1 = new TextBox();
            comboM1 = new RComboBox();
            labelM1 = new Label();
            comboM4 = new RComboBox();
            comboM3 = new RComboBox();
            textM4 = new TextBox();
            textM3 = new TextBox();
            labelM4 = new Label();
            labelM3 = new Label();
            labelM2 = new Label();
            comboM2 = new RComboBox();
            labelFNF4 = new Label();
            comboFNF4 = new RComboBox();
            textFNF4 = new TextBox();
            comboFNC = new RComboBox();
            textFNC = new TextBox();
            tableKeys = new TableLayoutPanel();
            labelFNC = new Label();
            textM2 = new TextBox();
            panelBacklightHeader = new Panel();
            sliderBrightness = new Slider();
            pictureBacklight = new PictureBox();
            labelBacklightTitle = new Label();
            panelBacklight = new Panel();
            panelBacklightExtra = new Panel();
            numericBacklightPluggedTime = new NumericUpDown();
            labelBacklightTimeoutPlugged = new Label();
            numericBacklightTime = new NumericUpDown();
            labelBacklightTimeout = new Label();
            labelSpeed = new Label();
            comboKeyboardSpeed = new RComboBox();
            panelXMG = new Panel();
            checkXMG = new CheckBox();
            tableBacklight = new TableLayoutPanel();
            labelBacklightKeyboard = new Label();
            checkAwake = new CheckBox();
            checkBoot = new CheckBox();
            checkSleep = new CheckBox();
            checkShutdown = new CheckBox();
            labelBacklightLogo = new Label();
            checkAwakeLogo = new CheckBox();
            checkBootLogo = new CheckBox();
            checkSleepLogo = new CheckBox();
            checkShutdownLogo = new CheckBox();
            labelBacklightBar = new Label();
            checkAwakeBar = new CheckBox();
            checkBootBar = new CheckBox();
            checkSleepBar = new CheckBox();
            checkShutdownBar = new CheckBox();
            labelBacklightLid = new Label();
            checkAwakeLid = new CheckBox();
            checkBootLid = new CheckBox();
            checkSleepLid = new CheckBox();
            checkShutdownLid = new CheckBox();
            panelSettingsHeader = new Panel();
            pictureLog = new PictureBox();
            pictureSettings = new PictureBox();
            labelSettings = new Label();
            panelSettings = new Panel();
            checkAutoApplyWindowsPowerMode = new CheckBox();
            checkTopmost = new CheckBox();
            checkNoOverdrive = new CheckBox();
            checkUSBC = new CheckBox();
            checkVariBright = new CheckBox();
            checkGpuApps = new CheckBox();
            checkFnLock = new CheckBox();
            panelServices.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureService).BeginInit();
            panelBindingsHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBindings).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureHelp).BeginInit();
            panelBindings.SuspendLayout();
            tableBindings.SuspendLayout();
            tableKeys.SuspendLayout();
            panelBacklightHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBacklight).BeginInit();
            panelBacklight.SuspendLayout();
            panelBacklightExtra.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericBacklightPluggedTime).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericBacklightTime).BeginInit();
            panelXMG.SuspendLayout();
            tableBacklight.SuspendLayout();
            panelSettingsHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureLog).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureSettings).BeginInit();
            panelSettings.SuspendLayout();
            SuspendLayout();
            // 
            // panelServices
            // 
            panelServices.Controls.Add(pictureService);
            panelServices.Controls.Add(labelServices);
            panelServices.Controls.Add(buttonServices);
            panelServices.Dock = DockStyle.Top;
            panelServices.Location = new Point(15, 1285);
            panelServices.Name = "panelServices";
            panelServices.Size = new Size(983, 75);
            panelServices.TabIndex = 3;
            // 
            // pictureService
            // 
            pictureService.BackgroundImage = Resources.icons8_automation_32;
            pictureService.BackgroundImageLayout = ImageLayout.Zoom;
            pictureService.Location = new Point(20, 19);
            pictureService.Name = "pictureService";
            pictureService.Size = new Size(32, 32);
            pictureService.TabIndex = 21;
            pictureService.TabStop = false;
            // 
            // labelServices
            // 
            labelServices.AutoSize = true;
            labelServices.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelServices.Location = new Point(57, 18);
            labelServices.Name = "labelServices";
            labelServices.Size = new Size(273, 32);
            labelServices.TabIndex = 20;
            labelServices.Text = "Asus Services Running";
            // 
            // buttonServices
            // 
            buttonServices.Activated = false;
            buttonServices.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonServices.BackColor = SystemColors.ButtonHighlight;
            buttonServices.BorderColor = Color.Transparent;
            buttonServices.BorderRadius = 2;
            buttonServices.FlatStyle = FlatStyle.Flat;
            buttonServices.Location = new Point(713, 12);
            buttonServices.Margin = new Padding(4, 3, 4, 3);
            buttonServices.Name = "buttonServices";
            buttonServices.Secondary = false;
            buttonServices.Size = new Size(256, 52);
            buttonServices.TabIndex = 19;
            buttonServices.Text = "Start Services";
            buttonServices.UseVisualStyleBackColor = false;
            // 
            // panelBindingsHeader
            // 
            panelBindingsHeader.AutoSize = true;
            panelBindingsHeader.BackColor = SystemColors.ControlLight;
            panelBindingsHeader.Controls.Add(pictureBindings);
            panelBindingsHeader.Controls.Add(pictureHelp);
            panelBindingsHeader.Controls.Add(labelBindings);
            panelBindingsHeader.Dock = DockStyle.Top;
            panelBindingsHeader.Location = new Point(15, 15);
            panelBindingsHeader.Name = "panelBindingsHeader";
            panelBindingsHeader.Padding = new Padding(10, 8, 10, 8);
            panelBindingsHeader.Size = new Size(983, 56);
            panelBindingsHeader.TabIndex = 4;
            // 
            // pictureBindings
            // 
            pictureBindings.BackgroundImage = Resources.icons8_keyboard_32;
            pictureBindings.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBindings.Location = new Point(20, 10);
            pictureBindings.Name = "pictureBindings";
            pictureBindings.Size = new Size(32, 32);
            pictureBindings.TabIndex = 1;
            pictureBindings.TabStop = false;
            // 
            // pictureHelp
            // 
            pictureHelp.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pictureHelp.BackgroundImage = Resources.icons8_help_32;
            pictureHelp.BackgroundImageLayout = ImageLayout.Zoom;
            pictureHelp.Cursor = Cursors.Hand;
            pictureHelp.Location = new Point(930, 13);
            pictureHelp.Margin = new Padding(4, 3, 4, 3);
            pictureHelp.Name = "pictureHelp";
            pictureHelp.Size = new Size(32, 32);
            pictureHelp.TabIndex = 11;
            pictureHelp.TabStop = false;
            // 
            // labelBindings
            // 
            labelBindings.AutoSize = true;
            labelBindings.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelBindings.Location = new Point(56, 9);
            labelBindings.Name = "labelBindings";
            labelBindings.Size = new Size(114, 32);
            labelBindings.TabIndex = 0;
            labelBindings.Text = "Bindings";
            // 
            // panelBindings
            // 
            panelBindings.AutoSize = true;
            panelBindings.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelBindings.Controls.Add(tableBindings);
            panelBindings.Dock = DockStyle.Top;
            panelBindings.Location = new Point(15, 71);
            panelBindings.Name = "panelBindings";
            panelBindings.Padding = new Padding(0, 0, 10, 5);
            panelBindings.Size = new Size(983, 325);
            panelBindings.TabIndex = 5;
            // 
            // tableBindings
            // 
            tableBindings.ColumnCount = 3;
            tableBindings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15.4649944F));
            tableBindings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40.97453F));
            tableBindings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 43.6323357F));
            tableBindings.Controls.Add(label1, 0, 5);
            tableBindings.Controls.Add(textBox1, 2, 1);
            tableBindings.Controls.Add(textM1, 2, 0);
            tableBindings.Controls.Add(comboM1, 1, 0);
            tableBindings.Controls.Add(labelM1, 0, 0);
            tableBindings.Controls.Add(comboM4, 1, 3);
            tableBindings.Controls.Add(comboM3, 1, 2);
            tableBindings.Controls.Add(textM4, 2, 3);
            tableBindings.Controls.Add(textM3, 2, 2);
            tableBindings.Controls.Add(labelM4, 0, 3);
            tableBindings.Controls.Add(labelM3, 0, 2);
            tableBindings.Controls.Add(labelM2, 0, 1);
            tableBindings.Controls.Add(comboM2, 1, 1);
            tableBindings.Controls.Add(labelFNF4, 0, 4);
            tableBindings.Controls.Add(comboFNF4, 1, 4);
            tableBindings.Controls.Add(textFNF4, 2, 4);
            tableBindings.Controls.Add(comboFNC, 1, 5);
            tableBindings.Controls.Add(textFNC, 2, 5);
            tableBindings.Dock = DockStyle.Top;
            tableBindings.Location = new Point(0, 0);
            tableBindings.Margin = new Padding(0, 3, 4, 3);
            tableBindings.Name = "tableBindings";
            tableBindings.Padding = new Padding(16, 12, 0, 12);
            tableBindings.RowCount = 6;
            tableBindings.RowStyles.Add(new RowStyle(SizeType.Absolute, 51F));
            tableBindings.RowStyles.Add(new RowStyle(SizeType.Absolute, 51F));
            tableBindings.RowStyles.Add(new RowStyle(SizeType.Absolute, 51F));
            tableBindings.RowStyles.Add(new RowStyle(SizeType.Absolute, 51F));
            tableBindings.RowStyles.Add(new RowStyle(SizeType.Absolute, 49F));
            tableBindings.RowStyles.Add(new RowStyle(SizeType.Absolute, 21F));
            tableBindings.Size = new Size(973, 320);
            tableBindings.TabIndex = 12;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(16, 265);
            label1.Margin = new Padding(0);
            label1.Name = "label1";
            label1.Padding = new Padding(5, 10, 0, 0);
            label1.Size = new Size(85, 42);
            label1.TabIndex = 15;
            label1.Text = "FN+C:";
            // 
            // textBox1
            // 
            textBox1.Dock = DockStyle.Top;
            textBox1.Location = new Point(558, 66);
            textBox1.Margin = new Padding(4, 3, 4, 3);
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = "action";
            textBox1.Size = new Size(411, 39);
            textBox1.TabIndex = 14;
            // 
            // textM1
            // 
            textM1.Dock = DockStyle.Top;
            textM1.Location = new Point(558, 15);
            textM1.Margin = new Padding(4, 3, 4, 3);
            textM1.Name = "textM1";
            textM1.PlaceholderText = "action";
            textM1.Size = new Size(411, 39);
            textM1.TabIndex = 13;
            // 
            // comboM1
            // 
            comboM1.BorderColor = Color.White;
            comboM1.ButtonColor = Color.FromArgb(255, 255, 255);
            comboM1.Dock = DockStyle.Top;
            comboM1.FormattingEnabled = true;
            comboM1.Items.AddRange(new object[] { Strings.Default, Strings.VolumeMute, Strings.PlayPause, Strings.PrintScreen, Strings.ToggleAura, Strings.Custom });
            comboM1.Location = new Point(167, 15);
            comboM1.Margin = new Padding(4, 3, 4, 3);
            comboM1.Name = "comboM1";
            comboM1.Size = new Size(383, 40);
            comboM1.TabIndex = 11;
            // 
            // labelM1
            // 
            labelM1.AutoSize = true;
            labelM1.Location = new Point(16, 12);
            labelM1.Margin = new Padding(0);
            labelM1.Name = "labelM1";
            labelM1.Padding = new Padding(5, 10, 0, 0);
            labelM1.Size = new Size(59, 42);
            labelM1.TabIndex = 9;
            labelM1.Text = "M1:";
            // 
            // comboM4
            // 
            comboM4.BorderColor = Color.White;
            comboM4.ButtonColor = Color.FromArgb(255, 255, 255);
            comboM4.Dock = DockStyle.Top;
            comboM4.FormattingEnabled = true;
            comboM4.Items.AddRange(new object[] { Strings.PerformanceMode, Strings.OpenGHelper, Strings.Custom });
            comboM4.Location = new Point(167, 168);
            comboM4.Margin = new Padding(4, 3, 4, 3);
            comboM4.Name = "comboM4";
            comboM4.Size = new Size(383, 40);
            comboM4.TabIndex = 3;
            // 
            // comboM3
            // 
            comboM3.BorderColor = Color.White;
            comboM3.ButtonColor = Color.FromArgb(255, 255, 255);
            comboM3.Dock = DockStyle.Top;
            comboM3.FormattingEnabled = true;
            comboM3.Items.AddRange(new object[] { Strings.Default, Strings.VolumeMute, Strings.PlayPause, Strings.PrintScreen, Strings.ToggleAura, Strings.Custom });
            comboM3.Location = new Point(167, 117);
            comboM3.Margin = new Padding(4, 3, 4, 3);
            comboM3.Name = "comboM3";
            comboM3.Size = new Size(383, 40);
            comboM3.TabIndex = 1;
            // 
            // textM4
            // 
            textM4.Dock = DockStyle.Top;
            textM4.Location = new Point(558, 168);
            textM4.Margin = new Padding(4, 3, 4, 3);
            textM4.Name = "textM4";
            textM4.PlaceholderText = "action";
            textM4.Size = new Size(411, 39);
            textM4.TabIndex = 5;
            // 
            // textM3
            // 
            textM3.Dock = DockStyle.Top;
            textM3.Location = new Point(558, 117);
            textM3.Margin = new Padding(4, 3, 4, 3);
            textM3.Name = "textM3";
            textM3.PlaceholderText = "action";
            textM3.Size = new Size(411, 39);
            textM3.TabIndex = 4;
            // 
            // labelM4
            // 
            labelM4.AutoSize = true;
            labelM4.Location = new Point(16, 165);
            labelM4.Margin = new Padding(0);
            labelM4.Name = "labelM4";
            labelM4.Padding = new Padding(5, 10, 0, 0);
            labelM4.Size = new Size(116, 42);
            labelM4.TabIndex = 2;
            labelM4.Text = "M4/ROG:";
            // 
            // labelM3
            // 
            labelM3.AutoSize = true;
            labelM3.Location = new Point(16, 114);
            labelM3.Margin = new Padding(0);
            labelM3.Name = "labelM3";
            labelM3.Padding = new Padding(5, 10, 0, 0);
            labelM3.Size = new Size(59, 42);
            labelM3.TabIndex = 0;
            labelM3.Text = "M3:";
            // 
            // labelM2
            // 
            labelM2.AutoSize = true;
            labelM2.Location = new Point(16, 63);
            labelM2.Margin = new Padding(0);
            labelM2.Name = "labelM2";
            labelM2.Padding = new Padding(5, 10, 0, 0);
            labelM2.Size = new Size(59, 42);
            labelM2.TabIndex = 10;
            labelM2.Text = "M2:";
            // 
            // comboM2
            // 
            comboM2.BorderColor = Color.White;
            comboM2.ButtonColor = Color.FromArgb(255, 255, 255);
            comboM2.Dock = DockStyle.Top;
            comboM2.FormattingEnabled = true;
            comboM2.Items.AddRange(new object[] { Strings.Default, Strings.VolumeMute, Strings.PlayPause, Strings.PrintScreen, Strings.ToggleAura, Strings.Custom });
            comboM2.Location = new Point(167, 66);
            comboM2.Margin = new Padding(4, 3, 4, 3);
            comboM2.Name = "comboM2";
            comboM2.Size = new Size(383, 40);
            comboM2.TabIndex = 12;
            // 
            // labelFNF4
            // 
            labelFNF4.AutoSize = true;
            labelFNF4.Location = new Point(16, 216);
            labelFNF4.Margin = new Padding(0);
            labelFNF4.Name = "labelFNF4";
            labelFNF4.Padding = new Padding(5, 10, 0, 0);
            labelFNF4.Size = new Size(95, 42);
            labelFNF4.TabIndex = 6;
            labelFNF4.Text = "FN+F4:";
            // 
            // comboFNF4
            // 
            comboFNF4.BorderColor = Color.White;
            comboFNF4.ButtonColor = Color.FromArgb(255, 255, 255);
            comboFNF4.Dock = DockStyle.Top;
            comboFNF4.FormattingEnabled = true;
            comboFNF4.Location = new Point(167, 219);
            comboFNF4.Margin = new Padding(4, 3, 4, 3);
            comboFNF4.Name = "comboFNF4";
            comboFNF4.Size = new Size(383, 40);
            comboFNF4.TabIndex = 7;
            // 
            // textFNF4
            // 
            textFNF4.Dock = DockStyle.Top;
            textFNF4.Location = new Point(558, 219);
            textFNF4.Margin = new Padding(4, 3, 4, 3);
            textFNF4.Name = "textFNF4";
            textFNF4.PlaceholderText = "action";
            textFNF4.Size = new Size(411, 39);
            textFNF4.TabIndex = 8;
            // 
            // comboFNC
            // 
            comboFNC.BorderColor = Color.White;
            comboFNC.ButtonColor = Color.FromArgb(255, 255, 255);
            comboFNC.Dock = DockStyle.Top;
            comboFNC.FormattingEnabled = true;
            comboFNC.Location = new Point(167, 268);
            comboFNC.Margin = new Padding(4, 3, 4, 3);
            comboFNC.Name = "comboFNC";
            comboFNC.Size = new Size(383, 40);
            comboFNC.TabIndex = 16;
            // 
            // textFNC
            // 
            textFNC.Dock = DockStyle.Top;
            textFNC.Location = new Point(558, 268);
            textFNC.Margin = new Padding(4, 3, 4, 3);
            textFNC.Name = "textFNC";
            textFNC.PlaceholderText = "action";
            textFNC.Size = new Size(411, 39);
            textFNC.TabIndex = 17;
            // 
            // tableKeys
            // 
            tableKeys.ColumnCount = 3;
            tableKeys.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableKeys.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableKeys.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableKeys.Controls.Add(labelFNC, 0, 5);
            tableKeys.Location = new Point(0, 0);
            tableKeys.Name = "tableKeys";
            tableKeys.RowCount = 6;
            tableKeys.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableKeys.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableKeys.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableKeys.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableKeys.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableKeys.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableKeys.Size = new Size(200, 100);
            tableKeys.TabIndex = 0;
            // 
            // labelFNC
            // 
            labelFNC.AutoSize = true;
            labelFNC.Location = new Point(4, 100);
            labelFNC.Margin = new Padding(4, 0, 4, 0);
            labelFNC.Name = "labelFNC";
            labelFNC.Size = new Size(32, 20);
            labelFNC.TabIndex = 15;
            labelFNC.Text = "FN+C:";
            // 
            // textM2
            // 
            textM2.Dock = DockStyle.Top;
            textM2.Location = new Point(124, 2);
            textM2.Margin = new Padding(4, 2, 4, 2);
            textM2.Name = "textM2";
            textM2.PlaceholderText = "action";
            textM2.Size = new Size(72, 39);
            textM2.TabIndex = 14;
            // 
            // panelBacklightHeader
            // 
            panelBacklightHeader.AutoSize = true;
            panelBacklightHeader.BackColor = SystemColors.ControlLight;
            panelBacklightHeader.Controls.Add(sliderBrightness);
            panelBacklightHeader.Controls.Add(pictureBacklight);
            panelBacklightHeader.Controls.Add(labelBacklightTitle);
            panelBacklightHeader.Dock = DockStyle.Top;
            panelBacklightHeader.Location = new Point(15, 396);
            panelBacklightHeader.Name = "panelBacklightHeader";
            panelBacklightHeader.Padding = new Padding(10, 8, 10, 8);
            panelBacklightHeader.Size = new Size(983, 56);
            panelBacklightHeader.TabIndex = 44;
            // 
            // sliderBrightness
            // 
            sliderBrightness.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            sliderBrightness.Location = new Point(546, 9);
            sliderBrightness.Max = 3;
            sliderBrightness.Min = 0;
            sliderBrightness.Name = "sliderBrightness";
            sliderBrightness.Size = new Size(419, 36);
            sliderBrightness.Step = 1;
            sliderBrightness.TabIndex = 50;
            sliderBrightness.Text = "sliderBrightness";
            sliderBrightness.Value = 3;
            // 
            // pictureBacklight
            // 
            pictureBacklight.BackgroundImage = Resources.backlight;
            pictureBacklight.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBacklight.Location = new Point(20, 10);
            pictureBacklight.Name = "pictureBacklight";
            pictureBacklight.Size = new Size(32, 32);
            pictureBacklight.TabIndex = 3;
            pictureBacklight.TabStop = false;
            // 
            // labelBacklightTitle
            // 
            labelBacklightTitle.AutoSize = true;
            labelBacklightTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelBacklightTitle.Location = new Point(56, 8);
            labelBacklightTitle.Name = "labelBacklightTitle";
            labelBacklightTitle.Size = new Size(119, 32);
            labelBacklightTitle.TabIndex = 2;
            labelBacklightTitle.Text = "Backlight";
            // 
            // panelBacklight
            // 
            panelBacklight.AutoSize = true;
            panelBacklight.Controls.Add(panelBacklightExtra);
            panelBacklight.Controls.Add(panelXMG);
            panelBacklight.Controls.Add(tableBacklight);
            panelBacklight.Dock = DockStyle.Top;
            panelBacklight.Location = new Point(15, 452);
            panelBacklight.Name = "panelBacklight";
            panelBacklight.Padding = new Padding(0, 11, 0, 11);
            panelBacklight.Size = new Size(983, 463);
            panelBacklight.TabIndex = 6;
            // 
            // panelBacklightExtra
            // 
            panelBacklightExtra.AutoSize = true;
            panelBacklightExtra.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelBacklightExtra.Controls.Add(numericBacklightPluggedTime);
            panelBacklightExtra.Controls.Add(labelBacklightTimeoutPlugged);
            panelBacklightExtra.Controls.Add(numericBacklightTime);
            panelBacklightExtra.Controls.Add(labelBacklightTimeout);
            panelBacklightExtra.Controls.Add(labelSpeed);
            panelBacklightExtra.Controls.Add(comboKeyboardSpeed);
            panelBacklightExtra.Dock = DockStyle.Top;
            panelBacklightExtra.Location = new Point(0, 288);
            panelBacklightExtra.Margin = new Padding(4, 3, 4, 3);
            panelBacklightExtra.Name = "panelBacklightExtra";
            panelBacklightExtra.Padding = new Padding(0, 0, 0, 5);
            panelBacklightExtra.Size = new Size(983, 164);
            panelBacklightExtra.TabIndex = 46;
            // 
            // numericBacklightPluggedTime
            // 
            numericBacklightPluggedTime.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            numericBacklightPluggedTime.Location = new Point(764, 117);
            numericBacklightPluggedTime.Margin = new Padding(4, 3, 4, 3);
            numericBacklightPluggedTime.Maximum = new decimal(new int[] { 3600, 0, 0, 0 });
            numericBacklightPluggedTime.Name = "numericBacklightPluggedTime";
            numericBacklightPluggedTime.Size = new Size(197, 39);
            numericBacklightPluggedTime.TabIndex = 49;
            // 
            // labelBacklightTimeoutPlugged
            // 
            labelBacklightTimeoutPlugged.Location = new Point(16, 112);
            labelBacklightTimeoutPlugged.Margin = new Padding(4, 0, 4, 0);
            labelBacklightTimeoutPlugged.Name = "labelBacklightTimeoutPlugged";
            labelBacklightTimeoutPlugged.Size = new Size(744, 47);
            labelBacklightTimeoutPlugged.TabIndex = 48;
            labelBacklightTimeoutPlugged.Text = "Seconds to turn off backlight when plugged";
            // 
            // numericBacklightTime
            // 
            numericBacklightTime.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            numericBacklightTime.Location = new Point(764, 65);
            numericBacklightTime.Margin = new Padding(4, 3, 4, 3);
            numericBacklightTime.Maximum = new decimal(new int[] { 3600, 0, 0, 0 });
            numericBacklightTime.Name = "numericBacklightTime";
            numericBacklightTime.Size = new Size(197, 39);
            numericBacklightTime.TabIndex = 47;
            // 
            // labelBacklightTimeout
            // 
            labelBacklightTimeout.Location = new Point(16, 63);
            labelBacklightTimeout.Margin = new Padding(4, 0, 4, 0);
            labelBacklightTimeout.Name = "labelBacklightTimeout";
            labelBacklightTimeout.Size = new Size(744, 47);
            labelBacklightTimeout.TabIndex = 46;
            labelBacklightTimeout.Text = "Seconds to turn off backlight on battery";
            // 
            // labelSpeed
            // 
            labelSpeed.Location = new Point(16, 16);
            labelSpeed.Margin = new Padding(4, 0, 4, 0);
            labelSpeed.Name = "labelSpeed";
            labelSpeed.Size = new Size(547, 43);
            labelSpeed.TabIndex = 44;
            labelSpeed.Text = "Animation Speed";
            // 
            // comboKeyboardSpeed
            // 
            comboKeyboardSpeed.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            comboKeyboardSpeed.BorderColor = Color.White;
            comboKeyboardSpeed.ButtonColor = SystemColors.ControlLight;
            comboKeyboardSpeed.FlatStyle = FlatStyle.Flat;
            comboKeyboardSpeed.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            comboKeyboardSpeed.FormattingEnabled = true;
            comboKeyboardSpeed.ItemHeight = 32;
            comboKeyboardSpeed.Items.AddRange(new object[] { "Slow", "Normal", "Fast" });
            comboKeyboardSpeed.Location = new Point(594, 13);
            comboKeyboardSpeed.Margin = new Padding(4, 12, 4, 9);
            comboKeyboardSpeed.Name = "comboKeyboardSpeed";
            comboKeyboardSpeed.Size = new Size(369, 40);
            comboKeyboardSpeed.TabIndex = 43;
            comboKeyboardSpeed.TabStop = false;
            // 
            // panelXMG
            // 
            panelXMG.Controls.Add(checkXMG);
            panelXMG.Dock = DockStyle.Top;
            panelXMG.Location = new Point(0, 228);
            panelXMG.Margin = new Padding(4, 3, 4, 3);
            panelXMG.Name = "panelXMG";
            panelXMG.Size = new Size(983, 60);
            panelXMG.TabIndex = 45;
            // 
            // checkXMG
            // 
            checkXMG.AutoSize = true;
            checkXMG.Location = new Point(4, 10);
            checkXMG.Margin = new Padding(4, 3, 4, 3);
            checkXMG.Name = "checkXMG";
            checkXMG.Padding = new Padding(16, 3, 7, 3);
            checkXMG.Size = new Size(181, 42);
            checkXMG.TabIndex = 2;
            checkXMG.Text = "XG Mobile";
            checkXMG.UseVisualStyleBackColor = true;
            // 
            // tableBacklight
            // 
            tableBacklight.AutoSize = true;
            tableBacklight.ColumnCount = 4;
            tableBacklight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableBacklight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableBacklight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableBacklight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableBacklight.Controls.Add(labelBacklightKeyboard, 0, 0);
            tableBacklight.Controls.Add(checkAwake, 0, 1);
            tableBacklight.Controls.Add(checkBoot, 0, 2);
            tableBacklight.Controls.Add(checkSleep, 0, 3);
            tableBacklight.Controls.Add(checkShutdown, 0, 4);
            tableBacklight.Controls.Add(labelBacklightLogo, 1, 0);
            tableBacklight.Controls.Add(checkAwakeLogo, 1, 1);
            tableBacklight.Controls.Add(checkBootLogo, 1, 2);
            tableBacklight.Controls.Add(checkSleepLogo, 1, 3);
            tableBacklight.Controls.Add(checkShutdownLogo, 1, 4);
            tableBacklight.Controls.Add(labelBacklightBar, 2, 0);
            tableBacklight.Controls.Add(checkAwakeBar, 2, 1);
            tableBacklight.Controls.Add(checkBootBar, 2, 2);
            tableBacklight.Controls.Add(checkSleepBar, 2, 3);
            tableBacklight.Controls.Add(checkShutdownBar, 2, 4);
            tableBacklight.Controls.Add(labelBacklightLid, 3, 0);
            tableBacklight.Controls.Add(checkAwakeLid, 3, 1);
            tableBacklight.Controls.Add(checkBootLid, 3, 2);
            tableBacklight.Controls.Add(checkSleepLid, 3, 3);
            tableBacklight.Controls.Add(checkShutdownLid, 3, 4);
            tableBacklight.Dock = DockStyle.Top;
            tableBacklight.Location = new Point(0, 11);
            tableBacklight.Margin = new Padding(0);
            tableBacklight.Name = "tableBacklight";
            tableBacklight.RowCount = 5;
            tableBacklight.RowStyles.Add(new RowStyle());
            tableBacklight.RowStyles.Add(new RowStyle());
            tableBacklight.RowStyles.Add(new RowStyle());
            tableBacklight.RowStyles.Add(new RowStyle());
            tableBacklight.RowStyles.Add(new RowStyle());
            tableBacklight.Size = new Size(983, 217);
            tableBacklight.TabIndex = 44;
            // 
            // labelBacklightKeyboard
            // 
            labelBacklightKeyboard.Dock = DockStyle.Fill;
            labelBacklightKeyboard.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelBacklightKeyboard.Location = new Point(4, 0);
            labelBacklightKeyboard.Margin = new Padding(4, 0, 4, 0);
            labelBacklightKeyboard.Name = "labelBacklightKeyboard";
            labelBacklightKeyboard.Padding = new Padding(9, 4, 7, 4);
            labelBacklightKeyboard.Size = new Size(237, 45);
            labelBacklightKeyboard.TabIndex = 6;
            labelBacklightKeyboard.Text = "Keyboard";
            // 
            // checkAwake
            // 
            checkAwake.Dock = DockStyle.Fill;
            checkAwake.Location = new Point(4, 45);
            checkAwake.Margin = new Padding(4, 0, 4, 0);
            checkAwake.Name = "checkAwake";
            checkAwake.Padding = new Padding(16, 3, 7, 3);
            checkAwake.Size = new Size(237, 43);
            checkAwake.TabIndex = 1;
            checkAwake.Text = Strings.Awake;
            checkAwake.UseVisualStyleBackColor = true;
            // 
            // checkBoot
            // 
            checkBoot.Dock = DockStyle.Fill;
            checkBoot.Location = new Point(4, 88);
            checkBoot.Margin = new Padding(4, 0, 4, 0);
            checkBoot.Name = "checkBoot";
            checkBoot.Padding = new Padding(16, 3, 7, 3);
            checkBoot.Size = new Size(237, 43);
            checkBoot.TabIndex = 2;
            checkBoot.Text = Strings.Boot;
            checkBoot.UseVisualStyleBackColor = true;
            // 
            // checkSleep
            // 
            checkSleep.Dock = DockStyle.Fill;
            checkSleep.Location = new Point(4, 131);
            checkSleep.Margin = new Padding(4, 0, 4, 0);
            checkSleep.Name = "checkSleep";
            checkSleep.Padding = new Padding(16, 3, 7, 3);
            checkSleep.Size = new Size(237, 43);
            checkSleep.TabIndex = 3;
            checkSleep.Text = "Sleep";
            checkSleep.UseVisualStyleBackColor = true;
            // 
            // checkShutdown
            // 
            checkShutdown.Dock = DockStyle.Fill;
            checkShutdown.Location = new Point(4, 174);
            checkShutdown.Margin = new Padding(4, 0, 4, 0);
            checkShutdown.Name = "checkShutdown";
            checkShutdown.Padding = new Padding(16, 3, 7, 3);
            checkShutdown.Size = new Size(237, 43);
            checkShutdown.TabIndex = 4;
            checkShutdown.Text = Strings.Shutdown;
            checkShutdown.UseVisualStyleBackColor = true;
            // 
            // labelBacklightLogo
            // 
            labelBacklightLogo.Dock = DockStyle.Fill;
            labelBacklightLogo.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelBacklightLogo.Location = new Point(249, 0);
            labelBacklightLogo.Margin = new Padding(4, 0, 4, 0);
            labelBacklightLogo.Name = "labelBacklightLogo";
            labelBacklightLogo.Padding = new Padding(9, 4, 7, 4);
            labelBacklightLogo.Size = new Size(237, 45);
            labelBacklightLogo.TabIndex = 21;
            labelBacklightLogo.Text = "Logo";
            // 
            // checkAwakeLogo
            // 
            checkAwakeLogo.Dock = DockStyle.Fill;
            checkAwakeLogo.Location = new Point(249, 45);
            checkAwakeLogo.Margin = new Padding(4, 0, 4, 0);
            checkAwakeLogo.Name = "checkAwakeLogo";
            checkAwakeLogo.Padding = new Padding(16, 3, 7, 3);
            checkAwakeLogo.Size = new Size(237, 43);
            checkAwakeLogo.TabIndex = 17;
            checkAwakeLogo.Text = Strings.Awake;
            checkAwakeLogo.UseVisualStyleBackColor = true;
            // 
            // checkBootLogo
            // 
            checkBootLogo.Dock = DockStyle.Fill;
            checkBootLogo.Location = new Point(249, 88);
            checkBootLogo.Margin = new Padding(4, 0, 4, 0);
            checkBootLogo.Name = "checkBootLogo";
            checkBootLogo.Padding = new Padding(16, 3, 7, 3);
            checkBootLogo.Size = new Size(237, 43);
            checkBootLogo.TabIndex = 18;
            checkBootLogo.Text = Strings.Boot;
            checkBootLogo.UseVisualStyleBackColor = true;
            // 
            // checkSleepLogo
            // 
            checkSleepLogo.Dock = DockStyle.Fill;
            checkSleepLogo.Location = new Point(249, 131);
            checkSleepLogo.Margin = new Padding(4, 0, 4, 0);
            checkSleepLogo.Name = "checkSleepLogo";
            checkSleepLogo.Padding = new Padding(16, 3, 7, 3);
            checkSleepLogo.Size = new Size(237, 43);
            checkSleepLogo.TabIndex = 19;
            checkSleepLogo.Text = Strings.Sleep;
            checkSleepLogo.UseVisualStyleBackColor = true;
            // 
            // checkShutdownLogo
            // 
            checkShutdownLogo.Dock = DockStyle.Fill;
            checkShutdownLogo.Location = new Point(249, 174);
            checkShutdownLogo.Margin = new Padding(4, 0, 4, 0);
            checkShutdownLogo.Name = "checkShutdownLogo";
            checkShutdownLogo.Padding = new Padding(16, 3, 7, 3);
            checkShutdownLogo.Size = new Size(237, 43);
            checkShutdownLogo.TabIndex = 20;
            checkShutdownLogo.Text = Strings.Shutdown;
            checkShutdownLogo.UseVisualStyleBackColor = true;
            // 
            // labelBacklightBar
            // 
            labelBacklightBar.Dock = DockStyle.Fill;
            labelBacklightBar.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelBacklightBar.Location = new Point(494, 0);
            labelBacklightBar.Margin = new Padding(4, 0, 4, 0);
            labelBacklightBar.Name = "labelBacklightBar";
            labelBacklightBar.Padding = new Padding(9, 4, 7, 4);
            labelBacklightBar.Size = new Size(237, 45);
            labelBacklightBar.TabIndex = 11;
            labelBacklightBar.Text = "Lightbar";
            // 
            // checkAwakeBar
            // 
            checkAwakeBar.Dock = DockStyle.Fill;
            checkAwakeBar.Location = new Point(494, 45);
            checkAwakeBar.Margin = new Padding(4, 0, 4, 0);
            checkAwakeBar.Name = "checkAwakeBar";
            checkAwakeBar.Padding = new Padding(16, 3, 7, 3);
            checkAwakeBar.Size = new Size(237, 43);
            checkAwakeBar.TabIndex = 7;
            checkAwakeBar.Text = Strings.Awake;
            checkAwakeBar.UseVisualStyleBackColor = true;
            // 
            // checkBootBar
            // 
            checkBootBar.Dock = DockStyle.Fill;
            checkBootBar.Location = new Point(494, 88);
            checkBootBar.Margin = new Padding(4, 0, 4, 0);
            checkBootBar.Name = "checkBootBar";
            checkBootBar.Padding = new Padding(16, 3, 7, 3);
            checkBootBar.Size = new Size(237, 43);
            checkBootBar.TabIndex = 8;
            checkBootBar.Text = Strings.Boot;
            checkBootBar.UseVisualStyleBackColor = true;
            // 
            // checkSleepBar
            // 
            checkSleepBar.Dock = DockStyle.Fill;
            checkSleepBar.Location = new Point(494, 131);
            checkSleepBar.Margin = new Padding(4, 0, 4, 0);
            checkSleepBar.Name = "checkSleepBar";
            checkSleepBar.Padding = new Padding(16, 3, 7, 3);
            checkSleepBar.Size = new Size(237, 43);
            checkSleepBar.TabIndex = 9;
            checkSleepBar.Text = Strings.Sleep;
            checkSleepBar.UseVisualStyleBackColor = true;
            // 
            // checkShutdownBar
            // 
            checkShutdownBar.Dock = DockStyle.Fill;
            checkShutdownBar.Location = new Point(494, 174);
            checkShutdownBar.Margin = new Padding(4, 0, 4, 0);
            checkShutdownBar.Name = "checkShutdownBar";
            checkShutdownBar.Padding = new Padding(16, 3, 7, 3);
            checkShutdownBar.Size = new Size(237, 43);
            checkShutdownBar.TabIndex = 10;
            checkShutdownBar.Text = Strings.Shutdown;
            checkShutdownBar.UseVisualStyleBackColor = true;
            // 
            // labelBacklightLid
            // 
            labelBacklightLid.Dock = DockStyle.Fill;
            labelBacklightLid.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelBacklightLid.Location = new Point(739, 0);
            labelBacklightLid.Margin = new Padding(4, 0, 4, 0);
            labelBacklightLid.Name = "labelBacklightLid";
            labelBacklightLid.Padding = new Padding(9, 4, 7, 4);
            labelBacklightLid.Size = new Size(240, 45);
            labelBacklightLid.TabIndex = 16;
            labelBacklightLid.Text = "Lid";
            // 
            // checkAwakeLid
            // 
            checkAwakeLid.Dock = DockStyle.Fill;
            checkAwakeLid.Location = new Point(739, 45);
            checkAwakeLid.Margin = new Padding(4, 0, 4, 0);
            checkAwakeLid.Name = "checkAwakeLid";
            checkAwakeLid.Padding = new Padding(16, 3, 7, 3);
            checkAwakeLid.Size = new Size(240, 43);
            checkAwakeLid.TabIndex = 12;
            checkAwakeLid.Text = Strings.Awake;
            checkAwakeLid.UseVisualStyleBackColor = true;
            // 
            // checkBootLid
            // 
            checkBootLid.Dock = DockStyle.Fill;
            checkBootLid.Location = new Point(739, 88);
            checkBootLid.Margin = new Padding(4, 0, 4, 0);
            checkBootLid.Name = "checkBootLid";
            checkBootLid.Padding = new Padding(16, 3, 7, 3);
            checkBootLid.Size = new Size(240, 43);
            checkBootLid.TabIndex = 13;
            checkBootLid.Text = Strings.Boot;
            checkBootLid.UseVisualStyleBackColor = true;
            // 
            // checkSleepLid
            // 
            checkSleepLid.Dock = DockStyle.Fill;
            checkSleepLid.Location = new Point(739, 131);
            checkSleepLid.Margin = new Padding(4, 0, 4, 0);
            checkSleepLid.Name = "checkSleepLid";
            checkSleepLid.Padding = new Padding(16, 3, 7, 3);
            checkSleepLid.Size = new Size(240, 43);
            checkSleepLid.TabIndex = 14;
            checkSleepLid.Text = Strings.Sleep;
            checkSleepLid.UseVisualStyleBackColor = true;
            // 
            // checkShutdownLid
            // 
            checkShutdownLid.Dock = DockStyle.Fill;
            checkShutdownLid.Location = new Point(739, 174);
            checkShutdownLid.Margin = new Padding(4, 0, 4, 0);
            checkShutdownLid.Name = "checkShutdownLid";
            checkShutdownLid.Padding = new Padding(16, 3, 7, 3);
            checkShutdownLid.Size = new Size(240, 43);
            checkShutdownLid.TabIndex = 15;
            checkShutdownLid.Text = Strings.Shutdown;
            checkShutdownLid.UseVisualStyleBackColor = true;
            // 
            // panelSettingsHeader
            // 
            panelSettingsHeader.AutoSize = true;
            panelSettingsHeader.BackColor = SystemColors.ControlLight;
            panelSettingsHeader.Controls.Add(pictureLog);
            panelSettingsHeader.Controls.Add(pictureSettings);
            panelSettingsHeader.Controls.Add(labelSettings);
            panelSettingsHeader.Dock = DockStyle.Top;
            panelSettingsHeader.Location = new Point(15, 915);
            panelSettingsHeader.Name = "panelSettingsHeader";
            panelSettingsHeader.Padding = new Padding(10, 8, 10, 8);
            panelSettingsHeader.Size = new Size(983, 56);
            panelSettingsHeader.TabIndex = 45;
            // 
            // pictureLog
            // 
            pictureLog.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pictureLog.BackgroundImage = Resources.icons8_log_32;
            pictureLog.BackgroundImageLayout = ImageLayout.Zoom;
            pictureLog.Cursor = Cursors.Hand;
            pictureLog.Location = new Point(930, 13);
            pictureLog.Margin = new Padding(4, 3, 4, 3);
            pictureLog.Name = "pictureLog";
            pictureLog.Size = new Size(32, 32);
            pictureLog.TabIndex = 12;
            pictureLog.TabStop = false;
            // 
            // pictureSettings
            // 
            pictureSettings.BackgroundImage = Resources.icons8_settings_32;
            pictureSettings.BackgroundImageLayout = ImageLayout.Zoom;
            pictureSettings.Location = new Point(20, 10);
            pictureSettings.Name = "pictureSettings";
            pictureSettings.Size = new Size(32, 32);
            pictureSettings.TabIndex = 1;
            pictureSettings.TabStop = false;
            // 
            // labelSettings
            // 
            labelSettings.AutoSize = true;
            labelSettings.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelSettings.Location = new Point(56, 8);
            labelSettings.Name = "labelSettings";
            labelSettings.Size = new Size(78, 32);
            labelSettings.TabIndex = 0;
            labelSettings.Text = "Other";
            // 
            // panelSettings
            // 
            panelSettings.AutoSize = true;
            panelSettings.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelSettings.Controls.Add(checkAutoApplyWindowsPowerMode);
            panelSettings.Controls.Add(checkTopmost);
            panelSettings.Controls.Add(checkNoOverdrive);
            panelSettings.Controls.Add(checkUSBC);
            panelSettings.Controls.Add(checkVariBright);
            panelSettings.Controls.Add(checkGpuApps);
            panelSettings.Controls.Add(checkFnLock);
            panelSettings.Dock = DockStyle.Top;
            panelSettings.Location = new Point(15, 971);
            panelSettings.Name = "panelSettings";
            panelSettings.Padding = new Padding(20, 10, 10, 10);
            panelSettings.Size = new Size(983, 314);
            panelSettings.TabIndex = 46;
            // 
            // checkAutoApplyWindowsPowerMode
            // 
            checkAutoApplyWindowsPowerMode.AutoSize = true;
            checkAutoApplyWindowsPowerMode.Dock = DockStyle.Top;
            checkAutoApplyWindowsPowerMode.Location = new Point(20, 262);
            checkAutoApplyWindowsPowerMode.Margin = new Padding(4, 3, 4, 3);
            checkAutoApplyWindowsPowerMode.Name = "checkAutoApplyWindowsPowerMode";
            checkAutoApplyWindowsPowerMode.Padding = new Padding(3);
            checkAutoApplyWindowsPowerMode.Size = new Size(953, 42);
            checkAutoApplyWindowsPowerMode.TabIndex = 54;
            checkAutoApplyWindowsPowerMode.Text = "Auto Adjust Windows Power Mode";
            checkAutoApplyWindowsPowerMode.UseVisualStyleBackColor = true;
            // 
            // checkTopmost
            // 
            checkTopmost.AutoSize = true;
            checkTopmost.Dock = DockStyle.Top;
            checkTopmost.Location = new Point(20, 220);
            checkTopmost.Margin = new Padding(4, 3, 4, 3);
            checkTopmost.Name = "checkTopmost";
            checkTopmost.Padding = new Padding(3);
            checkTopmost.Size = new Size(953, 42);
            checkTopmost.TabIndex = 51;
            checkTopmost.Text = Strings.WindowTop;
            checkTopmost.UseVisualStyleBackColor = true;
            // 
            // checkNoOverdrive
            // 
            checkNoOverdrive.AutoSize = true;
            checkNoOverdrive.Dock = DockStyle.Top;
            checkNoOverdrive.Location = new Point(20, 178);
            checkNoOverdrive.Margin = new Padding(4, 3, 4, 3);
            checkNoOverdrive.Name = "checkNoOverdrive";
            checkNoOverdrive.Padding = new Padding(3);
            checkNoOverdrive.Size = new Size(953, 42);
            checkNoOverdrive.TabIndex = 52;
            checkNoOverdrive.Text = Strings.DisableOverdrive;
            checkNoOverdrive.UseVisualStyleBackColor = true;
            // 
            // checkUSBC
            // 
            checkUSBC.AutoSize = true;
            checkUSBC.Dock = DockStyle.Top;
            checkUSBC.Location = new Point(20, 136);
            checkUSBC.Margin = new Padding(4, 3, 4, 3);
            checkUSBC.Name = "checkUSBC";
            checkUSBC.Padding = new Padding(3);
            checkUSBC.Size = new Size(953, 42);
            checkUSBC.TabIndex = 53;
            checkUSBC.Text = "Keep GPU disabled on USB-C charger in Optimized mode";
            checkUSBC.UseVisualStyleBackColor = true;
            // 
            // checkVariBright
            // 
            checkVariBright.AutoSize = true;
            checkVariBright.Dock = DockStyle.Top;
            checkVariBright.Location = new Point(20, 94);
            checkVariBright.Margin = new Padding(4, 3, 4, 3);
            checkVariBright.Name = "checkVariBright";
            checkVariBright.Padding = new Padding(3);
            checkVariBright.Size = new Size(953, 42);
            checkVariBright.TabIndex = 57;
            checkVariBright.Text = "AMD Display VariBright";
            checkVariBright.UseVisualStyleBackColor = true;
            // 
            // checkGpuApps
            // 
            checkGpuApps.AutoSize = true;
            checkGpuApps.Dock = DockStyle.Top;
            checkGpuApps.Location = new Point(20, 52);
            checkGpuApps.Margin = new Padding(4, 3, 4, 3);
            checkGpuApps.Name = "checkGpuApps";
            checkGpuApps.Padding = new Padding(3);
            checkGpuApps.Size = new Size(953, 42);
            checkGpuApps.TabIndex = 55;
            checkGpuApps.Text = "Stop all apps using GPU when switching to Eco";
            checkGpuApps.UseVisualStyleBackColor = true;
            // 
            // checkFnLock
            // 
            checkFnLock.AutoSize = true;
            checkFnLock.Dock = DockStyle.Top;
            checkFnLock.Location = new Point(20, 10);
            checkFnLock.Margin = new Padding(4, 3, 4, 3);
            checkFnLock.MaximumSize = new Size(800, 0);
            checkFnLock.Name = "checkFnLock";
            checkFnLock.Padding = new Padding(3);
            checkFnLock.Size = new Size(800, 42);
            checkFnLock.TabIndex = 56;
            checkFnLock.Text = "Process Fn+F hotkeys without Fn";
            checkFnLock.UseVisualStyleBackColor = true;
            // 
            // Extra
            // 
            AutoScaleDimensions = new SizeF(192F, 192F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(1013, 1355);
            Controls.Add(panelServices);
            Controls.Add(panelSettings);
            Controls.Add(panelSettingsHeader);
            Controls.Add(panelBacklight);
            Controls.Add(panelBacklightHeader);
            Controls.Add(panelBindings);
            Controls.Add(panelBindingsHeader);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MdiChildrenMinimizedAnchorBottom = false;
            MinimizeBox = false;
            MinimumSize = new Size(1034, 71);
            Name = "Extra";
            Padding = new Padding(15);
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Extra Settings";
            panelServices.ResumeLayout(false);
            panelServices.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureService).EndInit();
            panelBindingsHeader.ResumeLayout(false);
            panelBindingsHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBindings).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureHelp).EndInit();
            panelBindings.ResumeLayout(false);
            tableBindings.ResumeLayout(false);
            tableBindings.PerformLayout();
            tableKeys.ResumeLayout(false);
            tableKeys.PerformLayout();
            panelBacklightHeader.ResumeLayout(false);
            panelBacklightHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBacklight).EndInit();
            panelBacklight.ResumeLayout(false);
            panelBacklight.PerformLayout();
            panelBacklightExtra.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numericBacklightPluggedTime).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericBacklightTime).EndInit();
            panelXMG.ResumeLayout(false);
            panelXMG.PerformLayout();
            tableBacklight.ResumeLayout(false);
            panelSettingsHeader.ResumeLayout(false);
            panelSettingsHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureLog).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureSettings).EndInit();
            panelSettings.ResumeLayout(false);
            panelSettings.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Panel panelServices;
        private RButton buttonServices;
        private Label labelServices;
        private Panel panelBindingsHeader;
        private Panel panelBindings;
        private TableLayoutPanel tableBindings;
        private Label label1;
        private TextBox textBox1;
        private TextBox textM1;
        private RComboBox comboM1;
        private Label labelM1;
        private RComboBox comboM4;
        private RComboBox comboM3;
        private TextBox textM4;
        private TextBox textM3;
        private Label labelM4;
        private Label labelM3;
        private Label labelM2;
        private RComboBox comboM2;
        private Label labelFNF4;
        private RComboBox comboFNF4;
        private TextBox textFNF4;
        private RComboBox comboFNC;
        private TextBox textFNC;
        private PictureBox pictureHelp;
        private TableLayoutPanel tableKeys;
        private Label labelFNC;
        private TextBox textM2;
        private PictureBox pictureBindings;
        private Label labelBindings;
        private Panel panelBacklightHeader;
        private Panel panelBacklight;
        private Panel panelBacklightExtra;
        private NumericUpDown numericBacklightPluggedTime;
        private Label labelBacklightTimeoutPlugged;
        private NumericUpDown numericBacklightTime;
        private Label labelBacklightTimeout;
        private Label labelSpeed;
        private RComboBox comboKeyboardSpeed;
        private Panel panelXMG;
        private CheckBox checkXMG;
        private TableLayoutPanel tableBacklight;
        private Label labelBacklightKeyboard;
        private CheckBox checkAwake;
        private CheckBox checkBoot;
        private CheckBox checkSleep;
        private CheckBox checkShutdown;
        private Label labelBacklightLogo;
        private CheckBox checkAwakeLogo;
        private CheckBox checkBootLogo;
        private CheckBox checkSleepLogo;
        private CheckBox checkShutdownLogo;
        private Label labelBacklightBar;
        private CheckBox checkAwakeBar;
        private CheckBox checkBootBar;
        private CheckBox checkSleepBar;
        private CheckBox checkShutdownBar;
        private Label labelBacklightLid;
        private CheckBox checkAwakeLid;
        private CheckBox checkBootLid;
        private CheckBox checkSleepLid;
        private CheckBox checkShutdownLid;
        private Panel panelSettingsHeader;
        private PictureBox pictureSettings;
        private Label labelSettings;
        private Panel panelSettings;
        private CheckBox checkAutoApplyWindowsPowerMode;
        private CheckBox checkTopmost;
        private CheckBox checkNoOverdrive;
        private CheckBox checkUSBC;
        private CheckBox checkVariBright;
        private CheckBox checkGpuApps;
        private CheckBox checkFnLock;
        private PictureBox pictureBacklight;
        private Label labelBacklightTitle;
        private PictureBox pictureService;
        private Slider sliderBrightness;
        private PictureBox pictureLog;
    }
}