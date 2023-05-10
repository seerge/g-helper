using CustomControls;
using GHelper.Properties;

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
            groupBindings = new GroupBox();
            pictureHelp = new PictureBox();
            textFNF4 = new TextBox();
            comboFNF4 = new RComboBox();
            labelFNF4 = new Label();
            textM4 = new TextBox();
            textM3 = new TextBox();
            comboM4 = new RComboBox();
            labelM4 = new Label();
            comboM3 = new RComboBox();
            labelM3 = new Label();
            groupLight = new GroupBox();
            panelBacklightExtra = new Panel();
            labelBrightness = new Label();
            trackBrightness = new TrackBar();
            labelSpeed = new Label();
            comboKeyboardSpeed = new RComboBox();
            panelXMG = new Panel();
            checkXMG = new CheckBox();
            tableBacklight = new TableLayoutPanel();
            labelBacklight = new Label();
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
            groupOther = new GroupBox();
            checkUSBC = new CheckBox();
            checkNoOverdrive = new CheckBox();
            checkKeyboardAuto = new CheckBox();
            checkTopmost = new CheckBox();
            groupBindings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureHelp).BeginInit();
            groupLight.SuspendLayout();
            panelBacklightExtra.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBrightness).BeginInit();
            panelXMG.SuspendLayout();
            tableBacklight.SuspendLayout();
            groupOther.SuspendLayout();
            SuspendLayout();
            // 
            // groupBindings
            // 
            groupBindings.Controls.Add(pictureHelp);
            groupBindings.Controls.Add(textFNF4);
            groupBindings.Controls.Add(comboFNF4);
            groupBindings.Controls.Add(labelFNF4);
            groupBindings.Controls.Add(textM4);
            groupBindings.Controls.Add(textM3);
            groupBindings.Controls.Add(comboM4);
            groupBindings.Controls.Add(labelM4);
            groupBindings.Controls.Add(comboM3);
            groupBindings.Controls.Add(labelM3);
            groupBindings.Dock = DockStyle.Top;
            groupBindings.Location = new Point(10, 10);
            groupBindings.Name = "groupBindings";
            groupBindings.Size = new Size(934, 242);
            groupBindings.TabIndex = 0;
            groupBindings.TabStop = false;
            groupBindings.Text = "Key Bindings";
            // 
            // pictureHelp
            // 
            pictureHelp.BackgroundImage = Resources.icons8_help_64;
            pictureHelp.BackgroundImageLayout = ImageLayout.Zoom;
            pictureHelp.Cursor = Cursors.Hand;
            pictureHelp.Location = new Point(884, 58);
            pictureHelp.Name = "pictureHelp";
            pictureHelp.Size = new Size(32, 32);
            pictureHelp.TabIndex = 9;
            pictureHelp.TabStop = false;
            // 
            // textFNF4
            // 
            textFNF4.Location = new Point(415, 176);
            textFNF4.Name = "textFNF4";
            textFNF4.PlaceholderText = "action";
            textFNF4.Size = new Size(448, 39);
            textFNF4.TabIndex = 8;
            // 
            // comboFNF4
            // 
            comboFNF4.BorderColor = Color.White;
            comboFNF4.ButtonColor = Color.FromArgb(255, 255, 255);
            comboFNF4.FormattingEnabled = true;
            comboFNF4.Location = new Point(93, 175);
            comboFNF4.Name = "comboFNF4";
            comboFNF4.Size = new Size(312, 40);
            comboFNF4.TabIndex = 7;
            // 
            // labelFNF4
            // 
            labelFNF4.AutoSize = true;
            labelFNF4.Location = new Point(2, 178);
            labelFNF4.Name = "labelFNF4";
            labelFNF4.Size = new Size(90, 32);
            labelFNF4.TabIndex = 6;
            labelFNF4.Text = "FN+F4:";
            // 
            // textM4
            // 
            textM4.Location = new Point(415, 113);
            textM4.Name = "textM4";
            textM4.PlaceholderText = "action";
            textM4.Size = new Size(448, 39);
            textM4.TabIndex = 5;
            // 
            // textM3
            // 
            textM3.Location = new Point(415, 54);
            textM3.Name = "textM3";
            textM3.PlaceholderText = "notepad /p \"file.txt\"";
            textM3.Size = new Size(448, 39);
            textM3.TabIndex = 4;
            // 
            // comboM4
            // 
            comboM4.BorderColor = Color.White;
            comboM4.ButtonColor = Color.FromArgb(255, 255, 255);
            comboM4.FormattingEnabled = true;
            comboM4.Items.AddRange(new object[] { Strings.PerformanceMode, Strings.OpenGHelper, Strings.Custom });
            comboM4.Location = new Point(93, 112);
            comboM4.Name = "comboM4";
            comboM4.Size = new Size(312, 40);
            comboM4.TabIndex = 3;
            // 
            // labelM4
            // 
            labelM4.AutoSize = true;
            labelM4.Location = new Point(25, 116);
            labelM4.Name = "labelM4";
            labelM4.Size = new Size(54, 32);
            labelM4.TabIndex = 2;
            labelM4.Text = "M4:";
            // 
            // comboM3
            // 
            comboM3.BorderColor = Color.White;
            comboM3.ButtonColor = Color.FromArgb(255, 255, 255);
            comboM3.FormattingEnabled = true;
            comboM3.Items.AddRange(new object[] { Strings.Default, Strings.VolumeMute, Strings.PlayPause, Strings.PrintScreen, Strings.ToggleAura, Strings.Custom });
            comboM3.Location = new Point(93, 54);
            comboM3.Name = "comboM3";
            comboM3.Size = new Size(312, 40);
            comboM3.TabIndex = 1;
            // 
            // labelM3
            // 
            labelM3.AutoSize = true;
            labelM3.Location = new Point(25, 58);
            labelM3.Name = "labelM3";
            labelM3.Size = new Size(54, 32);
            labelM3.TabIndex = 0;
            labelM3.Text = "M3:";
            // 
            // groupLight
            // 
            groupLight.AutoSize = true;
            groupLight.Controls.Add(panelBacklightExtra);
            groupLight.Controls.Add(panelXMG);
            groupLight.Controls.Add(tableBacklight);
            groupLight.Dock = DockStyle.Top;
            groupLight.Location = new Point(10, 252);
            groupLight.Name = "groupLight";
            groupLight.Size = new Size(934, 475);
            groupLight.TabIndex = 1;
            groupLight.TabStop = false;
            groupLight.Text = "Keyboard Backlight";
            // 
            // panelBacklightExtra
            // 
            panelBacklightExtra.AutoSize = true;
            panelBacklightExtra.Controls.Add(labelBrightness);
            panelBacklightExtra.Controls.Add(trackBrightness);
            panelBacklightExtra.Controls.Add(labelSpeed);
            panelBacklightExtra.Controls.Add(comboKeyboardSpeed);
            panelBacklightExtra.Dock = DockStyle.Top;
            panelBacklightExtra.Location = new Point(3, 319);
            panelBacklightExtra.Name = "panelBacklightExtra";
            panelBacklightExtra.Size = new Size(928, 153);
            panelBacklightExtra.TabIndex = 43;
            // 
            // labelBrightness
            // 
            labelBrightness.Location = new Point(13, 76);
            labelBrightness.Name = "labelBrightness";
            labelBrightness.Size = new Size(197, 49);
            labelBrightness.TabIndex = 41;
            labelBrightness.Text = "Brightness";
            // 
            // trackBrightness
            // 
            trackBrightness.Location = new Point(216, 60);
            trackBrightness.Maximum = 3;
            trackBrightness.Name = "trackBrightness";
            trackBrightness.Size = new Size(600, 90);
            trackBrightness.TabIndex = 42;
            trackBrightness.TickStyle = TickStyle.TopLeft;
            // 
            // labelSpeed
            // 
            labelSpeed.AutoSize = true;
            labelSpeed.Location = new Point(13, 15);
            labelSpeed.MaximumSize = new Size(200, 0);
            labelSpeed.Name = "labelSpeed";
            labelSpeed.Size = new Size(198, 32);
            labelSpeed.TabIndex = 44;
            labelSpeed.Text = "Animation Speed";
            // 
            // comboKeyboardSpeed
            // 
            comboKeyboardSpeed.BorderColor = Color.White;
            comboKeyboardSpeed.ButtonColor = SystemColors.ControlLight;
            comboKeyboardSpeed.FlatStyle = FlatStyle.Flat;
            comboKeyboardSpeed.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            comboKeyboardSpeed.FormattingEnabled = true;
            comboKeyboardSpeed.ItemHeight = 32;
            comboKeyboardSpeed.Items.AddRange(new object[] { "Slow", "Normal", "Fast" });
            comboKeyboardSpeed.Location = new Point(220, 12);
            comboKeyboardSpeed.Margin = new Padding(4, 10, 4, 8);
            comboKeyboardSpeed.Name = "comboKeyboardSpeed";
            comboKeyboardSpeed.Size = new Size(291, 40);
            comboKeyboardSpeed.TabIndex = 43;
            comboKeyboardSpeed.TabStop = false;
            // 
            // panelXMG
            // 
            panelXMG.Controls.Add(checkXMG);
            panelXMG.Dock = DockStyle.Top;
            panelXMG.Location = new Point(3, 261);
            panelXMG.Name = "panelXMG";
            panelXMG.Size = new Size(928, 58);
            panelXMG.TabIndex = 42;
            // 
            // checkXMG
            // 
            checkXMG.AutoSize = true;
            checkXMG.Location = new Point(3, 10);
            checkXMG.Name = "checkXMG";
            checkXMG.Padding = new Padding(15, 2, 5, 2);
            checkXMG.Size = new Size(178, 40);
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
            tableBacklight.Controls.Add(labelBacklight, 0, 0);
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
            tableBacklight.Location = new Point(3, 35);
            tableBacklight.Margin = new Padding(0);
            tableBacklight.Name = "tableBacklight";
            tableBacklight.RowCount = 5;
            tableBacklight.RowStyles.Add(new RowStyle());
            tableBacklight.RowStyles.Add(new RowStyle());
            tableBacklight.RowStyles.Add(new RowStyle());
            tableBacklight.RowStyles.Add(new RowStyle());
            tableBacklight.RowStyles.Add(new RowStyle());
            tableBacklight.Size = new Size(928, 226);
            tableBacklight.TabIndex = 41;
            // 
            // labelBacklight
            // 
            labelBacklight.Dock = DockStyle.Fill;
            labelBacklight.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelBacklight.Location = new Point(3, 0);
            labelBacklight.Name = "labelBacklight";
            labelBacklight.Padding = new Padding(10, 5, 5, 5);
            labelBacklight.Size = new Size(226, 42);
            labelBacklight.TabIndex = 6;
            labelBacklight.Text = "Keyboard";
            // 
            // checkAwake
            // 
            checkAwake.Dock = DockStyle.Fill;
            checkAwake.Location = new Point(3, 45);
            checkAwake.Name = "checkAwake";
            checkAwake.Padding = new Padding(15, 2, 5, 2);
            checkAwake.Size = new Size(226, 40);
            checkAwake.TabIndex = 1;
            checkAwake.Text = Strings.Awake;
            checkAwake.UseVisualStyleBackColor = true;
            // 
            // checkBoot
            // 
            checkBoot.Dock = DockStyle.Fill;
            checkBoot.Location = new Point(3, 91);
            checkBoot.Name = "checkBoot";
            checkBoot.Padding = new Padding(15, 2, 5, 2);
            checkBoot.Size = new Size(226, 40);
            checkBoot.TabIndex = 2;
            checkBoot.Text = Strings.Boot;
            checkBoot.UseVisualStyleBackColor = true;
            // 
            // checkSleep
            // 
            checkSleep.Dock = DockStyle.Fill;
            checkSleep.Location = new Point(3, 137);
            checkSleep.Name = "checkSleep";
            checkSleep.Padding = new Padding(15, 2, 5, 2);
            checkSleep.Size = new Size(226, 40);
            checkSleep.TabIndex = 3;
            checkSleep.Text = "Sleep";
            checkSleep.UseVisualStyleBackColor = true;
            // 
            // checkShutdown
            // 
            checkShutdown.Dock = DockStyle.Fill;
            checkShutdown.Location = new Point(3, 183);
            checkShutdown.Name = "checkShutdown";
            checkShutdown.Padding = new Padding(15, 2, 5, 2);
            checkShutdown.Size = new Size(226, 40);
            checkShutdown.TabIndex = 4;
            checkShutdown.Text = Strings.Shutdown;
            checkShutdown.UseVisualStyleBackColor = true;
            // 
            // labelBacklightLogo
            // 
            labelBacklightLogo.Dock = DockStyle.Fill;
            labelBacklightLogo.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelBacklightLogo.Location = new Point(235, 0);
            labelBacklightLogo.Name = "labelBacklightLogo";
            labelBacklightLogo.Padding = new Padding(10, 5, 5, 5);
            labelBacklightLogo.Size = new Size(226, 42);
            labelBacklightLogo.TabIndex = 21;
            labelBacklightLogo.Text = "Logo";
            // 
            // checkAwakeLogo
            // 
            checkAwakeLogo.Dock = DockStyle.Fill;
            checkAwakeLogo.Location = new Point(235, 45);
            checkAwakeLogo.Name = "checkAwakeLogo";
            checkAwakeLogo.Padding = new Padding(15, 2, 5, 2);
            checkAwakeLogo.Size = new Size(226, 40);
            checkAwakeLogo.TabIndex = 17;
            checkAwakeLogo.Text = Strings.Awake;
            checkAwakeLogo.UseVisualStyleBackColor = true;
            // 
            // checkBootLogo
            // 
            checkBootLogo.Dock = DockStyle.Fill;
            checkBootLogo.Location = new Point(235, 91);
            checkBootLogo.Name = "checkBootLogo";
            checkBootLogo.Padding = new Padding(15, 2, 5, 2);
            checkBootLogo.Size = new Size(226, 40);
            checkBootLogo.TabIndex = 18;
            checkBootLogo.Text = Strings.Boot;
            checkBootLogo.UseVisualStyleBackColor = true;
            // 
            // checkSleepLogo
            // 
            checkSleepLogo.Dock = DockStyle.Fill;
            checkSleepLogo.Location = new Point(235, 137);
            checkSleepLogo.Name = "checkSleepLogo";
            checkSleepLogo.Padding = new Padding(15, 2, 5, 2);
            checkSleepLogo.Size = new Size(226, 40);
            checkSleepLogo.TabIndex = 19;
            checkSleepLogo.Text = Strings.Sleep;
            checkSleepLogo.UseVisualStyleBackColor = true;
            // 
            // checkShutdownLogo
            // 
            checkShutdownLogo.Dock = DockStyle.Fill;
            checkShutdownLogo.Location = new Point(235, 183);
            checkShutdownLogo.Name = "checkShutdownLogo";
            checkShutdownLogo.Padding = new Padding(15, 2, 5, 2);
            checkShutdownLogo.Size = new Size(226, 40);
            checkShutdownLogo.TabIndex = 20;
            checkShutdownLogo.Text = Strings.Shutdown;
            checkShutdownLogo.UseVisualStyleBackColor = true;
            // 
            // labelBacklightBar
            // 
            labelBacklightBar.Dock = DockStyle.Fill;
            labelBacklightBar.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelBacklightBar.Location = new Point(467, 0);
            labelBacklightBar.Name = "labelBacklightBar";
            labelBacklightBar.Padding = new Padding(10, 5, 5, 5);
            labelBacklightBar.Size = new Size(226, 42);
            labelBacklightBar.TabIndex = 11;
            labelBacklightBar.Text = "Lightbar";
            // 
            // checkAwakeBar
            // 
            checkAwakeBar.Dock = DockStyle.Fill;
            checkAwakeBar.Location = new Point(467, 45);
            checkAwakeBar.Name = "checkAwakeBar";
            checkAwakeBar.Padding = new Padding(15, 2, 5, 2);
            checkAwakeBar.Size = new Size(226, 40);
            checkAwakeBar.TabIndex = 7;
            checkAwakeBar.Text = Strings.Awake;
            checkAwakeBar.UseVisualStyleBackColor = true;
            // 
            // checkBootBar
            // 
            checkBootBar.Dock = DockStyle.Fill;
            checkBootBar.Location = new Point(467, 91);
            checkBootBar.Name = "checkBootBar";
            checkBootBar.Padding = new Padding(15, 2, 5, 2);
            checkBootBar.Size = new Size(226, 40);
            checkBootBar.TabIndex = 8;
            checkBootBar.Text = Strings.Boot;
            checkBootBar.UseVisualStyleBackColor = true;
            // 
            // checkSleepBar
            // 
            checkSleepBar.Dock = DockStyle.Fill;
            checkSleepBar.Location = new Point(467, 137);
            checkSleepBar.Name = "checkSleepBar";
            checkSleepBar.Padding = new Padding(15, 2, 5, 2);
            checkSleepBar.Size = new Size(226, 40);
            checkSleepBar.TabIndex = 9;
            checkSleepBar.Text = Strings.Sleep;
            checkSleepBar.UseVisualStyleBackColor = true;
            // 
            // checkShutdownBar
            // 
            checkShutdownBar.Dock = DockStyle.Fill;
            checkShutdownBar.Location = new Point(467, 183);
            checkShutdownBar.Name = "checkShutdownBar";
            checkShutdownBar.Padding = new Padding(15, 2, 5, 2);
            checkShutdownBar.Size = new Size(226, 40);
            checkShutdownBar.TabIndex = 10;
            checkShutdownBar.Text = Strings.Shutdown;
            checkShutdownBar.UseVisualStyleBackColor = true;
            // 
            // labelBacklightLid
            // 
            labelBacklightLid.Dock = DockStyle.Fill;
            labelBacklightLid.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelBacklightLid.Location = new Point(699, 0);
            labelBacklightLid.Name = "labelBacklightLid";
            labelBacklightLid.Padding = new Padding(10, 5, 5, 5);
            labelBacklightLid.Size = new Size(226, 42);
            labelBacklightLid.TabIndex = 16;
            labelBacklightLid.Text = "Lid";
            // 
            // checkAwakeLid
            // 
            checkAwakeLid.Dock = DockStyle.Fill;
            checkAwakeLid.Location = new Point(699, 45);
            checkAwakeLid.Name = "checkAwakeLid";
            checkAwakeLid.Padding = new Padding(15, 2, 5, 2);
            checkAwakeLid.Size = new Size(226, 40);
            checkAwakeLid.TabIndex = 12;
            checkAwakeLid.Text = Strings.Awake;
            checkAwakeLid.UseVisualStyleBackColor = true;
            // 
            // checkBootLid
            // 
            checkBootLid.Dock = DockStyle.Fill;
            checkBootLid.Location = new Point(699, 91);
            checkBootLid.Name = "checkBootLid";
            checkBootLid.Padding = new Padding(15, 2, 5, 2);
            checkBootLid.Size = new Size(226, 40);
            checkBootLid.TabIndex = 13;
            checkBootLid.Text = Strings.Boot;
            checkBootLid.UseVisualStyleBackColor = true;
            // 
            // checkSleepLid
            // 
            checkSleepLid.Dock = DockStyle.Fill;
            checkSleepLid.Location = new Point(699, 137);
            checkSleepLid.Name = "checkSleepLid";
            checkSleepLid.Padding = new Padding(15, 2, 5, 2);
            checkSleepLid.Size = new Size(226, 40);
            checkSleepLid.TabIndex = 14;
            checkSleepLid.Text = Strings.Sleep;
            checkSleepLid.UseVisualStyleBackColor = true;
            // 
            // checkShutdownLid
            // 
            checkShutdownLid.Dock = DockStyle.Fill;
            checkShutdownLid.Location = new Point(699, 183);
            checkShutdownLid.Name = "checkShutdownLid";
            checkShutdownLid.Padding = new Padding(15, 2, 5, 2);
            checkShutdownLid.Size = new Size(226, 40);
            checkShutdownLid.TabIndex = 15;
            checkShutdownLid.Text = Strings.Shutdown;
            checkShutdownLid.UseVisualStyleBackColor = true;
            // 
            // groupOther
            // 
            groupOther.Controls.Add(checkUSBC);
            groupOther.Controls.Add(checkNoOverdrive);
            groupOther.Controls.Add(checkKeyboardAuto);
            groupOther.Controls.Add(checkTopmost);
            groupOther.Dock = DockStyle.Top;
            groupOther.Location = new Point(10, 727);
            groupOther.Name = "groupOther";
            groupOther.Size = new Size(934, 276);
            groupOther.TabIndex = 2;
            groupOther.TabStop = false;
            groupOther.Text = "Other";
            // 
            // checkUSBC
            // 
            checkUSBC.AutoSize = true;
            checkUSBC.Location = new Point(25, 210);
            checkUSBC.Name = "checkUSBC";
            checkUSBC.Size = new Size(659, 36);
            checkUSBC.TabIndex = 4;
            checkUSBC.Text = "Keep GPU disabled on USB-C charger in Optimized mode";
            checkUSBC.UseVisualStyleBackColor = true;
            // 
            // checkNoOverdrive
            // 
            checkNoOverdrive.AutoSize = true;
            checkNoOverdrive.Location = new Point(25, 156);
            checkNoOverdrive.Name = "checkNoOverdrive";
            checkNoOverdrive.Size = new Size(307, 36);
            checkNoOverdrive.TabIndex = 3;
            checkNoOverdrive.Text = Strings.DisableOverdrive;
            checkNoOverdrive.UseVisualStyleBackColor = true;
            // 
            // checkKeyboardAuto
            // 
            checkKeyboardAuto.AutoSize = true;
            checkKeyboardAuto.Location = new Point(25, 52);
            checkKeyboardAuto.MaximumSize = new Size(780, 0);
            checkKeyboardAuto.Name = "checkKeyboardAuto";
            checkKeyboardAuto.Size = new Size(712, 36);
            checkKeyboardAuto.TabIndex = 2;
            checkKeyboardAuto.Text = Strings.KeyboardAuto;
            checkKeyboardAuto.UseVisualStyleBackColor = true;
            // 
            // checkTopmost
            // 
            checkTopmost.AutoSize = true;
            checkTopmost.Location = new Point(25, 104);
            checkTopmost.Name = "checkTopmost";
            checkTopmost.Size = new Size(390, 36);
            checkTopmost.TabIndex = 1;
            checkTopmost.Text = Strings.WindowTop;
            checkTopmost.UseVisualStyleBackColor = true;
            // 
            // Extra
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(954, 1013);
            Controls.Add(groupOther);
            Controls.Add(groupLight);
            Controls.Add(groupBindings);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MdiChildrenMinimizedAnchorBottom = false;
            MinimizeBox = false;
            Name = "Extra";
            Padding = new Padding(10);
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Extra Settings";
            groupBindings.ResumeLayout(false);
            groupBindings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureHelp).EndInit();
            groupLight.ResumeLayout(false);
            groupLight.PerformLayout();
            panelBacklightExtra.ResumeLayout(false);
            panelBacklightExtra.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBrightness).EndInit();
            panelXMG.ResumeLayout(false);
            panelXMG.PerformLayout();
            tableBacklight.ResumeLayout(false);
            groupOther.ResumeLayout(false);
            groupOther.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox groupBindings;
        private Label labelM3;
        private RComboBox comboM3;
        private RComboBox comboM4;
        private Label labelM4;
        private TextBox textM4;
        private TextBox textM3;
        private TextBox textFNF4;
        private RComboBox comboFNF4;
        private Label labelFNF4;
        private GroupBox groupLight;
        private GroupBox groupOther;
        private CheckBox checkTopmost;
        private CheckBox checkKeyboardAuto;
        private CheckBox checkNoOverdrive;
        private PictureBox pictureHelp;
        private CheckBox checkUSBC;
        private TableLayoutPanel tableBacklight;
        private CheckBox checkShutdown;
        private CheckBox checkAwake;
        private CheckBox checkBoot;
        private CheckBox checkSleep;
        private CheckBox checkBootLid;
        private Label labelBacklight;
        private CheckBox checkSleepBar;
        private CheckBox checkShutdownBar;
        private Label labelBacklightBar;
        private CheckBox checkAwakeBar;
        private CheckBox checkBootBar;
        private CheckBox checkSleepLid;
        private CheckBox checkShutdownLid;
        private Label labelBacklightLid;
        private CheckBox checkAwakeLid;
        private Label labelBacklightLogo;
        private CheckBox checkAwakeLogo;
        private CheckBox checkBootLogo;
        private CheckBox checkSleepLogo;
        private CheckBox checkShutdownLogo;
        private Panel panelBacklightExtra;
        private Label labelBrightness;
        private TrackBar trackBrightness;
        private Label labelSpeed;
        private RComboBox comboKeyboardSpeed;
        private Panel panelXMG;
        private CheckBox checkXMG;
    }
}