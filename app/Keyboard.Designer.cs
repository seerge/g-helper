using CustomControls;

namespace GHelper
{
    partial class Keyboard
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
            groupBox1 = new GroupBox();
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
            labelSpeed = new Label();
            comboKeyboardSpeed = new RComboBox();
            checkShutdown = new CheckBox();
            checkSleep = new CheckBox();
            checkBoot = new CheckBox();
            checkAwake = new CheckBox();
            groupOther = new GroupBox();
            checkKeyboardAuto = new CheckBox();
            checkTopmost = new CheckBox();
            checkNoOverdrive = new CheckBox();
            groupBox1.SuspendLayout();
            groupLight.SuspendLayout();
            groupOther.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(textFNF4);
            groupBox1.Controls.Add(comboFNF4);
            groupBox1.Controls.Add(labelFNF4);
            groupBox1.Controls.Add(textM4);
            groupBox1.Controls.Add(textM3);
            groupBox1.Controls.Add(comboM4);
            groupBox1.Controls.Add(labelM4);
            groupBox1.Controls.Add(comboM3);
            groupBox1.Controls.Add(labelM3);
            groupBox1.Dock = DockStyle.Top;
            groupBox1.Location = new Point(10, 10);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(756, 242);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = Properties.Strings.KeyBindings;
            // 
            // textFNF4
            // 
            textFNF4.Location = new Point(411, 176);
            textFNF4.Name = "textFNF4";
            textFNF4.PlaceholderText = "action";
            textFNF4.Size = new Size(320, 39);
            textFNF4.TabIndex = 8;
            // 
            // comboFNF4
            // 
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
            textM4.Location = new Point(411, 113);
            textM4.Name = "textM4";
            textM4.PlaceholderText = "action";
            textM4.Size = new Size(320, 39);
            textM4.TabIndex = 5;
            // 
            // textM3
            // 
            textM3.Location = new Point(411, 54);
            textM3.Name = "textM3";
            textM3.PlaceholderText = "notepad /p \"file.txt\"";
            textM3.Size = new Size(320, 39);
            textM3.TabIndex = 4;
            // 
            // comboM4
            // 
            comboM4.FormattingEnabled = true;
            comboM4.Items.AddRange(new object[] { Properties.Strings.PerformanceMode, Properties.Strings.OpenGHelper, Properties.Strings.Custom });
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
            comboM3.FormattingEnabled = true;
            comboM3.Items.AddRange(new object[] { Properties.Strings.Default, Properties.Strings.VolumeMute, Properties.Strings.PlayPause, Properties.Strings.PrintScreen, Properties.Strings.ToggleAura, Properties.Strings.Custom });
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
            groupLight.Controls.Add(labelSpeed);
            groupLight.Controls.Add(comboKeyboardSpeed);
            groupLight.Controls.Add(checkShutdown);
            groupLight.Controls.Add(checkSleep);
            groupLight.Controls.Add(checkBoot);
            groupLight.Controls.Add(checkAwake);
            groupLight.Dock = DockStyle.Top;
            groupLight.Location = new Point(10, 252);
            groupLight.Name = "groupLight";
            groupLight.Size = new Size(756, 320);
            groupLight.TabIndex = 1;
            groupLight.TabStop = false;
            groupLight.Text = Properties.Strings.KeyboardBacklight;
            // 
            // labelSpeed
            // 
            labelSpeed.AutoSize = true;
            labelSpeed.Location = new Point(25, 237);
            labelSpeed.Name = "labelSpeed";
            labelSpeed.Size = new Size(198, 32);
            labelSpeed.MaximumSize = new Size(200, 0);
            labelSpeed.TabIndex = 40;
            labelSpeed.Text = Properties.Strings.AnimationSpeed;
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
            comboKeyboardSpeed.Location = new Point(230, 234);
            comboKeyboardSpeed.Margin = new Padding(4, 10, 4, 8);
            comboKeyboardSpeed.Name = "comboKeyboardSpeed";
            comboKeyboardSpeed.Size = new Size(291, 40);
            comboKeyboardSpeed.TabIndex = 39;
            comboKeyboardSpeed.TabStop = false;
            // 
            // checkShutdown
            // 
            checkShutdown.AutoSize = true;
            checkShutdown.Location = new Point(25, 185);
            checkShutdown.Name = "checkShutdown";
            checkShutdown.Size = new Size(154, 36);
            checkShutdown.TabIndex = 3;
            checkShutdown.Text = Properties.Strings.Shutdown;
            checkShutdown.UseVisualStyleBackColor = true;
            // 
            // checkSleep
            // 
            checkSleep.AutoSize = true;
            checkSleep.Location = new Point(25, 143);
            checkSleep.Name = "checkSleep";
            checkSleep.Size = new Size(105, 36);
            checkSleep.TabIndex = 2;
            checkSleep.Text = Properties.Strings.Sleep;
            checkSleep.UseVisualStyleBackColor = true;
            // 
            // checkBoot
            // 
            checkBoot.AutoSize = true;
            checkBoot.Location = new Point(25, 101);
            checkBoot.Name = "checkBoot";
            checkBoot.Size = new Size(96, 36);
            checkBoot.TabIndex = 1;
            checkBoot.Text = Properties.Strings.Boot;
            checkBoot.UseVisualStyleBackColor = true;
            // 
            // checkAwake
            // 
            checkAwake.AutoSize = true;
            checkAwake.Location = new Point(25, 59);
            checkAwake.Name = "checkAwake";
            checkAwake.Size = new Size(115, 36);
            checkAwake.TabIndex = 0;
            checkAwake.Text = Properties.Strings.Awake;
            checkAwake.UseVisualStyleBackColor = true;
            // 
            // groupOther
            // 
            groupOther.Controls.Add(checkNoOverdrive);
            groupOther.Controls.Add(checkKeyboardAuto);
            groupOther.Controls.Add(checkTopmost);
            groupOther.Dock = DockStyle.Top;
            groupOther.Location = new Point(10, 556);
            groupOther.Name = "groupOther";
            groupOther.Size = new Size(756, 225);
            groupOther.TabIndex = 2;
            groupOther.TabStop = false;
            groupOther.Text = Properties.Strings.Other;
            // 
            // checkKeyboardAuto
            // 
            checkKeyboardAuto.AutoSize = true;
            checkKeyboardAuto.Location = new Point(25, 51);
            checkKeyboardAuto.Name = "checkKeyboardAuto";
            checkKeyboardAuto.Size = new Size(712, 36);
            checkKeyboardAuto.MaximumSize = new Size(712, 0);
            checkKeyboardAuto.AutoEllipsis = true;
            checkKeyboardAuto.TabIndex = 2;
            checkKeyboardAuto.Text = Properties.Strings.KeyboardAuto;
            checkKeyboardAuto.UseVisualStyleBackColor = true;
            // 
            // checkTopmost
            // 
            checkTopmost.AutoSize = true;
            checkTopmost.Location = new Point(25, 104);
            checkTopmost.Name = "checkTopmost";
            checkTopmost.Size = new Size(390, 36);
            checkTopmost.TabIndex = 1;
            checkTopmost.Text = Properties.Strings.WindowTop;
            checkTopmost.UseVisualStyleBackColor = true;
            // 
            // checkNoOverdrive
            // 
            checkNoOverdrive.AutoSize = true;
            checkNoOverdrive.Location = new Point(25, 156);
            checkNoOverdrive.Name = "checkNoOverdrive";
            checkNoOverdrive.Size = new Size(307, 36);
            checkNoOverdrive.TabIndex = 3;
            checkNoOverdrive.Text = Properties.Strings.DisableOverdrive;
            checkNoOverdrive.UseVisualStyleBackColor = true;
            // 
            // Keyboard
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(830, 858);
            Controls.Add(groupOther);
            Controls.Add(groupLight);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MdiChildrenMinimizedAnchorBottom = false;
            MinimizeBox = false;
            Name = Properties.Strings.Keyboard;
            Padding = new Padding(10);
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = Properties.Strings.ExtraSettings;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupLight.ResumeLayout(false);
            groupLight.PerformLayout();
            groupOther.ResumeLayout(false);
            groupOther.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
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
        private CheckBox checkSleep;
        private CheckBox checkBoot;
        private CheckBox checkAwake;
        private CheckBox checkShutdown;
        private Label labelSpeed;
        private RComboBox comboKeyboardSpeed;
        private GroupBox groupOther;
        private CheckBox checkTopmost;
        private CheckBox checkKeyboardAuto;
        private CheckBox checkNoOverdrive;
    }
}