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
            labelFNE = new Label();
            comboFNE = new RComboBox();
            textFNE = new TextBox();
            labelFNC = new Label();
            textM2 = new TextBox();
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
            panelBacklightHeader = new Panel();
            sliderBrightness = new Slider();
            pictureBacklight = new PictureBox();
            labelBacklightTitle = new Label();
            panelBacklight = new Panel();
            panelBacklightExtra = new Panel();
            numericBacklightPluggedTime = new NumericUpDown();
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
            checkAutoToggleClamshellMode = new CheckBox();
            checkAutoApplyWindowsPowerMode = new CheckBox();
            checkTopmost = new CheckBox();
            checkNoOverdrive = new CheckBox();
            checkUSBC = new CheckBox();
            checkVariBright = new CheckBox();
            checkGpuApps = new CheckBox();
            checkGPUFix = new CheckBox();
            panelPower = new Panel();
            numericHibernateAfter = new NumericUpDown();
            labelHibernateAfter = new Label();
            pictureHibernate = new PictureBox();
            panelServices.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureService).BeginInit();
            panelBindingsHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBindings).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureHelp).BeginInit();
            panelBindings.SuspendLayout();
            tableBindings.SuspendLayout();
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
            panelPower.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericHibernateAfter).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureHibernate).BeginInit();
            SuspendLayout();
            // 
            // panelServices
            // 
            panelServices.AccessibleName = "Asus Services";
            panelServices.AccessibleRole = AccessibleRole.Grouping;
            panelServices.Controls.Add(pictureService);
            panelServices.Controls.Add(labelServices);
            panelServices.Controls.Add(buttonServices);
            panelServices.Dock = DockStyle.Top;
            panelServices.Location = new Point(15, 1321);
            panelServices.Name = "panelServices";
            panelServices.Size = new Size(983, 75);
            panelServices.TabIndex = 5;
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
            labelServices.Location = new Point(57, 19);
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
            panelBindingsHeader.Padding = new Padding(11, 5, 11, 5);
            panelBindingsHeader.Size = new Size(983, 51);
            panelBindingsHeader.TabIndex = 4;
            // 
            // pictureBindings
            // 
            pictureBindings.BackgroundImage = Resources.icons8_keyboard_32;
            pictureBindings.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBindings.Location = new Point(20, 11);
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
            pictureHelp.Location = new Point(931, 11);
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
            labelBindings.Location = new Point(56, 8);
            labelBindings.Name = "labelBindings";
            labelBindings.Size = new Size(114, 32);
            labelBindings.TabIndex = 0;
            labelBindings.Text = "Bindings";
            // 
            // panelBindings
            // 
            panelBindings.AccessibleName = "Key Bindings";
            panelBindings.AutoSize = true;
            panelBindings.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelBindings.Controls.Add(tableBindings);
            panelBindings.Dock = DockStyle.Top;
            panelBindings.Location = new Point(15, 66);
            panelBindings.Name = "panelBindings";
            panelBindings.Padding = new Padding(0, 0, 11, 5);
            panelBindings.Size = new Size(983, 351);
            panelBindings.TabIndex = 1;
            panelBindings.TabStop = true;
            // 
            // tableBindings
            // 
            tableBindings.AccessibleName = "Keyboard Bindings";
            tableBindings.AccessibleRole = AccessibleRole.Table;
            tableBindings.AutoSize = true;
            tableBindings.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableBindings.ColumnCount = 3;
            tableBindings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableBindings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableBindings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableBindings.Controls.Add(labelFNE, 0, 6);
            tableBindings.Controls.Add(comboFNE, 0, 6);
            tableBindings.Controls.Add(textFNE, 0, 6);
            tableBindings.Controls.Add(labelFNC, 0, 5);
            tableBindings.Controls.Add(textM2, 2, 1);
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
            tableBindings.RowCount = 7;
            tableBindings.RowStyles.Add(new RowStyle());
            tableBindings.RowStyles.Add(new RowStyle());
            tableBindings.RowStyles.Add(new RowStyle());
            tableBindings.RowStyles.Add(new RowStyle());
            tableBindings.RowStyles.Add(new RowStyle());
            tableBindings.RowStyles.Add(new RowStyle());
            tableBindings.RowStyles.Add(new RowStyle());
            tableBindings.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableBindings.Size = new Size(972, 346);
            tableBindings.TabIndex = 12;
            // 
            // labelFNE
            // 
            labelFNE.AutoSize = true;
            labelFNE.Location = new Point(16, 288);
            labelFNE.Margin = new Padding(0);
            labelFNE.Name = "labelFNE";
            labelFNE.Padding = new Padding(5, 11, 0, 0);
            labelFNE.Size = new Size(143, 43);
            labelFNE.TabIndex = 20;
            labelFNE.Text = "FN+NmEnt:";
            // 
            // comboFNE
            // 
            comboFNE.AccessibleName = "Fn+Numpad Action";
            comboFNE.BorderColor = Color.White;
            comboFNE.ButtonColor = Color.FromArgb(255, 255, 255);
            comboFNE.Dock = DockStyle.Top;
            comboFNE.FormattingEnabled = true;
            comboFNE.Location = new Point(211, 291);
            comboFNE.Margin = new Padding(4, 3, 4, 3);
            comboFNE.Name = "comboFNE";
            comboFNE.Size = new Size(374, 40);
            comboFNE.TabIndex = 7;
            // 
            // textFNE
            // 
            textFNE.Dock = DockStyle.Top;
            textFNE.Location = new Point(593, 291);
            textFNE.Margin = new Padding(4, 3, 4, 3);
            textFNE.Name = "textFNE";
            textFNE.PlaceholderText = "action";
            textFNE.Size = new Size(375, 39);
            textFNE.TabIndex = 18;
            textFNE.TabStop = false;
            // 
            // labelFNC
            // 
            labelFNC.AutoSize = true;
            labelFNC.Location = new Point(16, 242);
            labelFNC.Margin = new Padding(0);
            labelFNC.Name = "labelFNC";
            labelFNC.Padding = new Padding(5, 11, 0, 0);
            labelFNC.Size = new Size(85, 43);
            labelFNC.TabIndex = 15;
            labelFNC.Text = "FN+C:";
            // 
            // textM2
            // 
            textM2.Dock = DockStyle.Top;
            textM2.Location = new Point(594, 61);
            textM2.Margin = new Padding(5, 3, 5, 3);
            textM2.Name = "textM2";
            textM2.PlaceholderText = "action";
            textM2.Size = new Size(373, 39);
            textM2.TabIndex = 14;
            textM2.TabStop = false;
            // 
            // textM1
            // 
            textM1.Dock = DockStyle.Top;
            textM1.Location = new Point(593, 15);
            textM1.Margin = new Padding(4, 3, 4, 3);
            textM1.Name = "textM1";
            textM1.PlaceholderText = "action";
            textM1.Size = new Size(375, 39);
            textM1.TabIndex = 13;
            textM1.TabStop = false;
            // 
            // comboM1
            // 
            comboM1.AccessibleName = "M1 Action";
            comboM1.BorderColor = Color.White;
            comboM1.ButtonColor = Color.FromArgb(255, 255, 255);
            comboM1.Dock = DockStyle.Top;
            comboM1.FormattingEnabled = true;
            comboM1.Items.AddRange(new object[] { Strings.Default, Strings.VolumeMute, Strings.PlayPause, Strings.PrintScreen, Strings.ToggleAura, Strings.Custom });
            comboM1.Location = new Point(211, 15);
            comboM1.Margin = new Padding(4, 3, 4, 3);
            comboM1.Name = "comboM1";
            comboM1.Size = new Size(374, 40);
            comboM1.TabIndex = 1;
            // 
            // labelM1
            // 
            labelM1.AutoSize = true;
            labelM1.Location = new Point(16, 12);
            labelM1.Margin = new Padding(0);
            labelM1.Name = "labelM1";
            labelM1.Padding = new Padding(5, 11, 0, 0);
            labelM1.Size = new Size(59, 43);
            labelM1.TabIndex = 9;
            labelM1.Text = "M1:";
            // 
            // comboM4
            // 
            comboM4.AccessibleName = "M4 Action";
            comboM4.BorderColor = Color.White;
            comboM4.ButtonColor = Color.FromArgb(255, 255, 255);
            comboM4.Dock = DockStyle.Top;
            comboM4.FormattingEnabled = true;
            comboM4.Items.AddRange(new object[] { Strings.PerformanceMode, Strings.OpenGHelper, Strings.Custom });
            comboM4.Location = new Point(211, 153);
            comboM4.Margin = new Padding(4, 3, 4, 3);
            comboM4.Name = "comboM4";
            comboM4.Size = new Size(374, 40);
            comboM4.TabIndex = 4;
            // 
            // comboM3
            // 
            comboM3.AccessibleName = "M3 Action";
            comboM3.BorderColor = Color.White;
            comboM3.ButtonColor = Color.FromArgb(255, 255, 255);
            comboM3.Dock = DockStyle.Top;
            comboM3.FormattingEnabled = true;
            comboM3.Items.AddRange(new object[] { Strings.Default, Strings.VolumeMute, Strings.PlayPause, Strings.PrintScreen, Strings.ToggleAura, Strings.Custom });
            comboM3.Location = new Point(211, 107);
            comboM3.Margin = new Padding(4, 3, 4, 3);
            comboM3.Name = "comboM3";
            comboM3.Size = new Size(374, 40);
            comboM3.TabIndex = 3;
            // 
            // textM4
            // 
            textM4.Dock = DockStyle.Top;
            textM4.Location = new Point(593, 153);
            textM4.Margin = new Padding(4, 3, 4, 3);
            textM4.Name = "textM4";
            textM4.PlaceholderText = "action";
            textM4.Size = new Size(375, 39);
            textM4.TabIndex = 5;
            textM4.TabStop = false;
            // 
            // textM3
            // 
            textM3.Dock = DockStyle.Top;
            textM3.Location = new Point(593, 107);
            textM3.Margin = new Padding(4, 3, 4, 3);
            textM3.Name = "textM3";
            textM3.PlaceholderText = "action";
            textM3.Size = new Size(375, 39);
            textM3.TabIndex = 4;
            textM3.TabStop = false;
            // 
            // labelM4
            // 
            labelM4.AutoSize = true;
            labelM4.Location = new Point(16, 150);
            labelM4.Margin = new Padding(0);
            labelM4.Name = "labelM4";
            labelM4.Padding = new Padding(5, 11, 0, 0);
            labelM4.Size = new Size(116, 43);
            labelM4.TabIndex = 2;
            labelM4.Text = "M4/ROG:";
            // 
            // labelM3
            // 
            labelM3.AutoSize = true;
            labelM3.Location = new Point(16, 104);
            labelM3.Margin = new Padding(0);
            labelM3.Name = "labelM3";
            labelM3.Padding = new Padding(5, 11, 0, 0);
            labelM3.Size = new Size(59, 43);
            labelM3.TabIndex = 0;
            labelM3.Text = "M3:";
            // 
            // labelM2
            // 
            labelM2.AutoSize = true;
            labelM2.Location = new Point(16, 58);
            labelM2.Margin = new Padding(0);
            labelM2.Name = "labelM2";
            labelM2.Padding = new Padding(5, 11, 0, 0);
            labelM2.Size = new Size(59, 43);
            labelM2.TabIndex = 10;
            labelM2.Text = "M2:";
            // 
            // comboM2
            // 
            comboM2.AccessibleName = "M2 Action";
            comboM2.BorderColor = Color.White;
            comboM2.ButtonColor = Color.FromArgb(255, 255, 255);
            comboM2.Dock = DockStyle.Top;
            comboM2.FormattingEnabled = true;
            comboM2.Items.AddRange(new object[] { Strings.Default, Strings.VolumeMute, Strings.PlayPause, Strings.PrintScreen, Strings.ToggleAura, Strings.Custom });
            comboM2.Location = new Point(211, 61);
            comboM2.Margin = new Padding(4, 3, 4, 3);
            comboM2.Name = "comboM2";
            comboM2.Size = new Size(374, 40);
            comboM2.TabIndex = 2;
            // 
            // labelFNF4
            // 
            labelFNF4.AutoSize = true;
            labelFNF4.Location = new Point(16, 196);
            labelFNF4.Margin = new Padding(0);
            labelFNF4.Name = "labelFNF4";
            labelFNF4.Padding = new Padding(5, 11, 0, 0);
            labelFNF4.Size = new Size(95, 43);
            labelFNF4.TabIndex = 6;
            labelFNF4.Text = "FN+F4:";
            // 
            // comboFNF4
            // 
            comboFNF4.AccessibleName = "Fn+F4 Action";
            comboFNF4.BorderColor = Color.White;
            comboFNF4.ButtonColor = Color.FromArgb(255, 255, 255);
            comboFNF4.Dock = DockStyle.Top;
            comboFNF4.FormattingEnabled = true;
            comboFNF4.Location = new Point(211, 199);
            comboFNF4.Margin = new Padding(4, 3, 4, 3);
            comboFNF4.Name = "comboFNF4";
            comboFNF4.Size = new Size(374, 40);
            comboFNF4.TabIndex = 5;
            // 
            // textFNF4
            // 
            textFNF4.Dock = DockStyle.Top;
            textFNF4.Location = new Point(593, 199);
            textFNF4.Margin = new Padding(4, 3, 4, 3);
            textFNF4.Name = "textFNF4";
            textFNF4.PlaceholderText = "action";
            textFNF4.Size = new Size(375, 39);
            textFNF4.TabIndex = 8;
            textFNF4.TabStop = false;
            // 
            // comboFNC
            // 
            comboFNC.AccessibleName = "Fn+C Action";
            comboFNC.BorderColor = Color.White;
            comboFNC.ButtonColor = Color.FromArgb(255, 255, 255);
            comboFNC.Dock = DockStyle.Top;
            comboFNC.FormattingEnabled = true;
            comboFNC.Location = new Point(211, 245);
            comboFNC.Margin = new Padding(4, 3, 4, 3);
            comboFNC.Name = "comboFNC";
            comboFNC.Size = new Size(374, 40);
            comboFNC.TabIndex = 6;
            // 
            // textFNC
            // 
            textFNC.Dock = DockStyle.Top;
            textFNC.Location = new Point(593, 245);
            textFNC.Margin = new Padding(4, 3, 4, 3);
            textFNC.Name = "textFNC";
            textFNC.PlaceholderText = "action";
            textFNC.Size = new Size(375, 39);
            textFNC.TabIndex = 17;
            textFNC.TabStop = false;
            // 
            // tableKeys
            // 
            tableKeys.ColumnCount = 3;
            tableKeys.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableKeys.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableKeys.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
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
            // panelBacklightHeader
            // 
            panelBacklightHeader.AutoSize = true;
            panelBacklightHeader.BackColor = SystemColors.ControlLight;
            panelBacklightHeader.Controls.Add(sliderBrightness);
            panelBacklightHeader.Controls.Add(pictureBacklight);
            panelBacklightHeader.Controls.Add(labelBacklightTitle);
            panelBacklightHeader.Dock = DockStyle.Top;
            panelBacklightHeader.Location = new Point(15, 417);
            panelBacklightHeader.Name = "panelBacklightHeader";
            panelBacklightHeader.Padding = new Padding(11, 5, 11, 5);
            panelBacklightHeader.Size = new Size(983, 51);
            panelBacklightHeader.TabIndex = 44;
            // 
            // sliderBrightness
            // 
            sliderBrightness.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            sliderBrightness.Location = new Point(467, 5);
            sliderBrightness.Margin = new Padding(0);
            sliderBrightness.Max = 3;
            sliderBrightness.Min = 0;
            sliderBrightness.Name = "sliderBrightness";
            sliderBrightness.Size = new Size(500, 40);
            sliderBrightness.Step = 1;
            sliderBrightness.TabIndex = 50;
            sliderBrightness.TabStop = false;
            sliderBrightness.Text = "sliderBrightness";
            sliderBrightness.Value = 3;
            // 
            // pictureBacklight
            // 
            pictureBacklight.BackgroundImage = Resources.backlight;
            pictureBacklight.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBacklight.Location = new Point(20, 11);
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
            panelBacklight.Location = new Point(15, 468);
            panelBacklight.Name = "panelBacklight";
            panelBacklight.Padding = new Padding(0, 5, 0, 5);
            panelBacklight.Size = new Size(983, 402);
            panelBacklight.TabIndex = 2;
            // 
            // panelBacklightExtra
            // 
            panelBacklightExtra.AutoSize = true;
            panelBacklightExtra.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelBacklightExtra.Controls.Add(numericBacklightPluggedTime);
            panelBacklightExtra.Controls.Add(numericBacklightTime);
            panelBacklightExtra.Controls.Add(labelBacklightTimeout);
            panelBacklightExtra.Controls.Add(labelSpeed);
            panelBacklightExtra.Controls.Add(comboKeyboardSpeed);
            panelBacklightExtra.Dock = DockStyle.Top;
            panelBacklightExtra.Location = new Point(0, 282);
            panelBacklightExtra.Margin = new Padding(4, 3, 4, 3);
            panelBacklightExtra.Name = "panelBacklightExtra";
            panelBacklightExtra.Padding = new Padding(0, 0, 0, 5);
            panelBacklightExtra.Size = new Size(983, 115);
            panelBacklightExtra.TabIndex = 46;
            // 
            // numericBacklightPluggedTime
            // 
            numericBacklightPluggedTime.AccessibleName = "Backlight Timeout when plugged";
            numericBacklightPluggedTime.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            numericBacklightPluggedTime.Location = new Point(670, 63);
            numericBacklightPluggedTime.Margin = new Padding(4, 3, 4, 3);
            numericBacklightPluggedTime.Maximum = new decimal(new int[] { 3600, 0, 0, 0 });
            numericBacklightPluggedTime.Name = "numericBacklightPluggedTime";
            numericBacklightPluggedTime.Size = new Size(140, 39);
            numericBacklightPluggedTime.TabIndex = 1;
            // 
            // numericBacklightTime
            // 
            numericBacklightTime.AccessibleName = "Backlight Timeout when on battery";
            numericBacklightTime.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            numericBacklightTime.Location = new Point(822, 63);
            numericBacklightTime.Margin = new Padding(4, 3, 4, 3);
            numericBacklightTime.Maximum = new decimal(new int[] { 3600, 0, 0, 0 });
            numericBacklightTime.Name = "numericBacklightTime";
            numericBacklightTime.Size = new Size(140, 39);
            numericBacklightTime.TabIndex = 2;
            // 
            // labelBacklightTimeout
            // 
            labelBacklightTimeout.Location = new Point(16, 63);
            labelBacklightTimeout.Margin = new Padding(4, 0, 4, 0);
            labelBacklightTimeout.Name = "labelBacklightTimeout";
            labelBacklightTimeout.Size = new Size(612, 47);
            labelBacklightTimeout.TabIndex = 46;
            labelBacklightTimeout.Text = "Timeout when plugged / on battery";
            // 
            // labelSpeed
            // 
            labelSpeed.Location = new Point(16, 16);
            labelSpeed.Margin = new Padding(4, 0, 4, 0);
            labelSpeed.Name = "labelSpeed";
            labelSpeed.Size = new Size(612, 43);
            labelSpeed.TabIndex = 44;
            labelSpeed.Text = "Animation Speed";
            // 
            // comboKeyboardSpeed
            // 
            comboKeyboardSpeed.AccessibleName = "Keyboard Animation Speed";
            comboKeyboardSpeed.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            comboKeyboardSpeed.BorderColor = Color.White;
            comboKeyboardSpeed.ButtonColor = SystemColors.ControlLight;
            comboKeyboardSpeed.FlatStyle = FlatStyle.Flat;
            comboKeyboardSpeed.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            comboKeyboardSpeed.FormattingEnabled = true;
            comboKeyboardSpeed.ItemHeight = 32;
            comboKeyboardSpeed.Items.AddRange(new object[] { "Slow", "Normal", "Fast" });
            comboKeyboardSpeed.Location = new Point(670, 13);
            comboKeyboardSpeed.Margin = new Padding(4, 12, 4, 9);
            comboKeyboardSpeed.Name = "comboKeyboardSpeed";
            comboKeyboardSpeed.Size = new Size(293, 40);
            comboKeyboardSpeed.TabIndex = 0;
            comboKeyboardSpeed.TabStop = false;
            // 
            // panelXMG
            // 
            panelXMG.Controls.Add(checkXMG);
            panelXMG.Dock = DockStyle.Top;
            panelXMG.Location = new Point(0, 222);
            panelXMG.Margin = new Padding(4, 3, 4, 3);
            panelXMG.Name = "panelXMG";
            panelXMG.Size = new Size(983, 60);
            panelXMG.TabIndex = 45;
            // 
            // checkXMG
            // 
            checkXMG.AutoSize = true;
            checkXMG.Location = new Point(4, 11);
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
            tableBacklight.Location = new Point(0, 5);
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
            checkAwake.AccessibleName = "Awake Backlight";
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
            checkBoot.AccessibleName = "Boot Backlight";
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
            checkSleep.AccessibleName = "Sleep Backlight";
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
            checkShutdown.AccessibleName = "Shutdown Backlight";
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
            panelSettingsHeader.Location = new Point(15, 870);
            panelSettingsHeader.Name = "panelSettingsHeader";
            panelSettingsHeader.Padding = new Padding(11, 5, 11, 5);
            panelSettingsHeader.Size = new Size(983, 51);
            panelSettingsHeader.TabIndex = 45;
            // 
            // pictureLog
            // 
            pictureLog.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pictureLog.BackgroundImage = Resources.icons8_log_32;
            pictureLog.BackgroundImageLayout = ImageLayout.Zoom;
            pictureLog.Cursor = Cursors.Hand;
            pictureLog.Location = new Point(931, 11);
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
            pictureSettings.Location = new Point(20, 11);
            pictureSettings.Name = "pictureSettings";
            pictureSettings.Size = new Size(32, 32);
            pictureSettings.TabIndex = 1;
            pictureSettings.TabStop = false;
            // 
            // labelSettings
            // 
            labelSettings.AutoSize = true;
            labelSettings.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelSettings.Location = new Point(56, 9);
            labelSettings.Name = "labelSettings";
            labelSettings.Size = new Size(78, 32);
            labelSettings.TabIndex = 0;
            labelSettings.Text = "Other";
            // 
            // panelSettings
            // 
            panelSettings.AccessibleName = "Extra Settings";
            panelSettings.AccessibleRole = AccessibleRole.Grouping;
            panelSettings.AutoSize = true;
            panelSettings.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelSettings.Controls.Add(checkAutoToggleClamshellMode);
            panelSettings.Controls.Add(checkAutoApplyWindowsPowerMode);
            panelSettings.Controls.Add(checkTopmost);
            panelSettings.Controls.Add(checkNoOverdrive);
            panelSettings.Controls.Add(checkUSBC);
            panelSettings.Controls.Add(checkVariBright);
            panelSettings.Controls.Add(checkGpuApps);
            panelSettings.Controls.Add(checkGPUFix);
            panelSettings.Dock = DockStyle.Top;
            panelSettings.Location = new Point(15, 921);
            panelSettings.Name = "panelSettings";
            panelSettings.Padding = new Padding(20, 5, 11, 5);
            panelSettings.Size = new Size(983, 346);
            panelSettings.TabIndex = 3;
            // 
            // checkAutoToggleClamshellMode
            // 
            checkAutoToggleClamshellMode.AutoSize = true;
            checkAutoToggleClamshellMode.Dock = DockStyle.Top;
            checkAutoToggleClamshellMode.Location = new Point(20, 299);
            checkAutoToggleClamshellMode.Name = "checkAutoToggleClamshellMode";
            checkAutoToggleClamshellMode.Padding = new Padding(3);
            checkAutoToggleClamshellMode.Size = new Size(952, 42);
            checkAutoToggleClamshellMode.TabIndex = 8;
            checkAutoToggleClamshellMode.Text = "Auto Toggle Clamshell Mode";
            checkAutoToggleClamshellMode.UseVisualStyleBackColor = true;
            // 
            // checkAutoApplyWindowsPowerMode
            // 
            checkAutoApplyWindowsPowerMode.AutoSize = true;
            checkAutoApplyWindowsPowerMode.Dock = DockStyle.Top;
            checkAutoApplyWindowsPowerMode.Location = new Point(20, 257);
            checkAutoApplyWindowsPowerMode.Margin = new Padding(4, 3, 4, 3);
            checkAutoApplyWindowsPowerMode.Name = "checkAutoApplyWindowsPowerMode";
            checkAutoApplyWindowsPowerMode.Padding = new Padding(3);
            checkAutoApplyWindowsPowerMode.Size = new Size(952, 42);
            checkAutoApplyWindowsPowerMode.TabIndex = 7;
            checkAutoApplyWindowsPowerMode.Text = "Auto Adjust Windows Power Mode";
            checkAutoApplyWindowsPowerMode.UseVisualStyleBackColor = true;
            // 
            // checkTopmost
            // 
            checkTopmost.AutoSize = true;
            checkTopmost.Dock = DockStyle.Top;
            checkTopmost.Location = new Point(20, 215);
            checkTopmost.Margin = new Padding(4, 3, 4, 3);
            checkTopmost.Name = "checkTopmost";
            checkTopmost.Padding = new Padding(3);
            checkTopmost.Size = new Size(952, 42);
            checkTopmost.TabIndex = 6;
            checkTopmost.Text = Strings.WindowTop;
            checkTopmost.UseVisualStyleBackColor = true;
            // 
            // checkNoOverdrive
            // 
            checkNoOverdrive.AutoSize = true;
            checkNoOverdrive.Dock = DockStyle.Top;
            checkNoOverdrive.Location = new Point(20, 173);
            checkNoOverdrive.Margin = new Padding(4, 3, 4, 3);
            checkNoOverdrive.Name = "checkNoOverdrive";
            checkNoOverdrive.Padding = new Padding(3);
            checkNoOverdrive.Size = new Size(952, 42);
            checkNoOverdrive.TabIndex = 5;
            checkNoOverdrive.Text = Strings.DisableOverdrive;
            checkNoOverdrive.UseVisualStyleBackColor = true;
            // 
            // checkUSBC
            // 
            checkUSBC.AutoSize = true;
            checkUSBC.Dock = DockStyle.Top;
            checkUSBC.Location = new Point(20, 131);
            checkUSBC.Margin = new Padding(4, 3, 4, 3);
            checkUSBC.Name = "checkUSBC";
            checkUSBC.Padding = new Padding(3);
            checkUSBC.Size = new Size(952, 42);
            checkUSBC.TabIndex = 4;
            checkUSBC.Text = "Keep GPU disabled on USB-C charger in Optimized mode";
            checkUSBC.UseVisualStyleBackColor = true;
            // 
            // checkVariBright
            // 
            checkVariBright.AutoSize = true;
            checkVariBright.Dock = DockStyle.Top;
            checkVariBright.Location = new Point(20, 89);
            checkVariBright.Margin = new Padding(4, 3, 4, 3);
            checkVariBright.Name = "checkVariBright";
            checkVariBright.Padding = new Padding(3);
            checkVariBright.Size = new Size(952, 42);
            checkVariBright.TabIndex = 3;
            checkVariBright.Text = "AMD Display VariBright";
            checkVariBright.UseVisualStyleBackColor = true;
            // 
            // checkGpuApps
            // 
            checkGpuApps.AutoSize = true;
            checkGpuApps.Dock = DockStyle.Top;
            checkGpuApps.Location = new Point(20, 47);
            checkGpuApps.Margin = new Padding(4, 3, 4, 3);
            checkGpuApps.Name = "checkGpuApps";
            checkGpuApps.Padding = new Padding(3);
            checkGpuApps.Size = new Size(952, 42);
            checkGpuApps.TabIndex = 2;
            checkGpuApps.Text = "Stop all apps using GPU when switching to Eco";
            checkGpuApps.UseVisualStyleBackColor = true;
            // 
            // checkGPUFix
            // 
            checkGPUFix.AutoSize = true;
            checkGPUFix.Dock = DockStyle.Top;
            checkGPUFix.Location = new Point(20, 5);
            checkGPUFix.Margin = new Padding(4, 3, 4, 3);
            checkGPUFix.Name = "checkGPUFix";
            checkGPUFix.Padding = new Padding(3);
            checkGPUFix.Size = new Size(952, 42);
            checkGPUFix.TabIndex = 9;
            checkGPUFix.Text = "Enable GPU on shutdown (prevents issue with Eco mode)";
            checkGPUFix.UseVisualStyleBackColor = true;
            // 
            // panelPower
            // 
            panelPower.Controls.Add(numericHibernateAfter);
            panelPower.Controls.Add(labelHibernateAfter);
            panelPower.Controls.Add(pictureHibernate);
            panelPower.Dock = DockStyle.Top;
            panelPower.Location = new Point(15, 1267);
            panelPower.Name = "panelPower";
            panelPower.Size = new Size(983, 54);
            panelPower.TabIndex = 4;
            // 
            // numericHibernateAfter
            // 
            numericHibernateAfter.AccessibleName = "Minutes till Hibernation in sleep";
            numericHibernateAfter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            numericHibernateAfter.Increment = new decimal(new int[] { 10, 0, 0, 0 });
            numericHibernateAfter.Location = new Point(810, 7);
            numericHibernateAfter.Margin = new Padding(4, 3, 4, 3);
            numericHibernateAfter.Maximum = new decimal(new int[] { 3000000, 0, 0, 0 });
            numericHibernateAfter.Name = "numericHibernateAfter";
            numericHibernateAfter.Size = new Size(152, 39);
            numericHibernateAfter.TabIndex = 1;
            // 
            // labelHibernateAfter
            // 
            labelHibernateAfter.AutoSize = true;
            labelHibernateAfter.Location = new Point(59, 10);
            labelHibernateAfter.Margin = new Padding(4, 0, 4, 0);
            labelHibernateAfter.Name = "labelHibernateAfter";
            labelHibernateAfter.Size = new Size(457, 32);
            labelHibernateAfter.TabIndex = 45;
            labelHibernateAfter.Text = "Minutes till Hibernation in sleep (0 - OFF)";
            // 
            // pictureHibernate
            // 
            pictureHibernate.BackgroundImage = Resources.icons8_hibernate_32;
            pictureHibernate.BackgroundImageLayout = ImageLayout.Zoom;
            pictureHibernate.Location = new Point(20, 10);
            pictureHibernate.Name = "pictureHibernate";
            pictureHibernate.Size = new Size(32, 32);
            pictureHibernate.TabIndex = 22;
            pictureHibernate.TabStop = false;
            // 
            // Extra
            // 
            AutoScaleDimensions = new SizeF(192F, 192F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(1013, 1467);
            Controls.Add(panelServices);
            Controls.Add(panelPower);
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
            MinimumSize = new Size(1033, 0);
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
            panelBindings.PerformLayout();
            tableBindings.ResumeLayout(false);
            tableBindings.PerformLayout();
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
            panelPower.ResumeLayout(false);
            panelPower.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericHibernateAfter).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureHibernate).EndInit();
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
        private Label labelFNC;
        private TextBox textM2;
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
        private PictureBox pictureBindings;
        private Label labelBindings;
        private Panel panelBacklightHeader;
        private Panel panelBacklight;
        private Panel panelBacklightExtra;
        private NumericUpDown numericBacklightPluggedTime;
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
        private PictureBox pictureBacklight;
        private Label labelBacklightTitle;
        private PictureBox pictureService;
        private Slider sliderBrightness;
        private PictureBox pictureLog;
        private CheckBox checkAutoToggleClamshellMode;
        private Label labelFNE;
        private RComboBox comboFNE;
        private TextBox textFNE;
        private Slider slider1;
        private Panel panelPower;
        private PictureBox pictureHibernate;
        private Label labelHibernateAfter;
        private NumericUpDown numericHibernateAfter;
        private CheckBox checkGPUFix;
    }
}