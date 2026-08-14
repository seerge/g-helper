using GHelper.UI;
namespace GHelper
{
    partial class Slash
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
            panelPowerHeader = new Panel();
            labelPower = new Label();
            panelPower = new Panel();
            checkLidOff = new CheckBox();
            checkAutoOff = new CheckBox();
            panelIntervalHeader = new Panel();
            sliderInterval = new Slider();
            labelInterval = new Label();
            panelSpacer = new Panel();
            panelAnimationsHeader = new Panel();
            labelAnimations = new Label();
            panelAnimations = new Panel();
            panelDim = new Panel();
            comboDim = new RComboBox();
            checkPowerSaving = new CheckBox();
            checkBatteryLevel = new CheckBox();
            checkLowBattery = new CheckBox();
            panelSleep = new Panel();
            comboSleepPattern = new RComboBox();
            checkSleepAnimation = new CheckBox();
            checkBoot = new CheckBox();
            panelPowerHeader.SuspendLayout();
            panelPower.SuspendLayout();
            panelIntervalHeader.SuspendLayout();
            panelAnimationsHeader.SuspendLayout();
            panelAnimations.SuspendLayout();
            panelDim.SuspendLayout();
            panelSleep.SuspendLayout();
            SuspendLayout();
            //
            // panelPowerHeader
            //
            panelPowerHeader.AutoSize = true;
            panelPowerHeader.BackColor = SystemColors.ControlLight;
            panelPowerHeader.Controls.Add(labelPower);
            panelPowerHeader.Dock = DockStyle.Top;
            panelPowerHeader.Location = new Point(15, 15);
            panelPowerHeader.Name = "panelPowerHeader";
            panelPowerHeader.Padding = new Padding(11, 5, 11, 5);
            panelPowerHeader.Size = new Size(770, 51);
            panelPowerHeader.TabIndex = 0;
            //
            // labelPower
            //
            labelPower.AutoSize = true;
            labelPower.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelPower.Location = new Point(21, 9);
            labelPower.Name = "labelPower";
            labelPower.Size = new Size(85, 32);
            labelPower.TabIndex = 0;
            labelPower.Text = "Power";
            //
            // panelPower
            //
            panelPower.AutoSize = true;
            panelPower.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelPower.Controls.Add(checkLidOff);
            panelPower.Controls.Add(checkAutoOff);
            panelPower.Dock = DockStyle.Top;
            panelPower.Location = new Point(15, 66);
            panelPower.Name = "panelPower";
            panelPower.Padding = new Padding(21, 5, 11, 5);
            panelPower.Size = new Size(770, 100);
            panelPower.TabIndex = 1;
            //
            // checkLidOff
            //
            checkLidOff.AutoSize = true;
            checkLidOff.Dock = DockStyle.Top;
            checkLidOff.Location = new Point(21, 47);
            checkLidOff.Margin = new Padding(5, 3, 5, 3);
            checkLidOff.Name = "checkLidOff";
            checkLidOff.Padding = new Padding(3);
            checkLidOff.Size = new Size(738, 42);
            checkLidOff.TabIndex = 1;
            checkLidOff.Text = "Disable on lid close";
            checkLidOff.UseVisualStyleBackColor = true;
            //
            // checkAutoOff
            //
            checkAutoOff.AutoSize = true;
            checkAutoOff.Dock = DockStyle.Top;
            checkAutoOff.Location = new Point(21, 5);
            checkAutoOff.Margin = new Padding(5, 3, 5, 3);
            checkAutoOff.Name = "checkAutoOff";
            checkAutoOff.Padding = new Padding(3);
            checkAutoOff.Size = new Size(738, 42);
            checkAutoOff.TabIndex = 0;
            checkAutoOff.Text = "Disable on battery";
            checkAutoOff.UseVisualStyleBackColor = true;
            //
            // panelIntervalHeader
            //
            panelIntervalHeader.AutoSize = true;
            panelIntervalHeader.BackColor = SystemColors.ControlLight;
            panelIntervalHeader.Controls.Add(sliderInterval);
            panelIntervalHeader.Controls.Add(labelInterval);
            panelIntervalHeader.Dock = DockStyle.Top;
            panelIntervalHeader.Location = new Point(15, 166);
            panelIntervalHeader.Name = "panelIntervalHeader";
            panelIntervalHeader.Padding = new Padding(11, 5, 11, 5);
            panelIntervalHeader.Size = new Size(770, 51);
            panelIntervalHeader.TabIndex = 2;
            //
            // sliderInterval
            //
            sliderInterval.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            sliderInterval.Location = new Point(360, 5);
            sliderInterval.Margin = new Padding(0);
            sliderInterval.Max = 5;
            sliderInterval.Min = 0;
            sliderInterval.Name = "sliderInterval";
            sliderInterval.Size = new Size(395, 40);
            sliderInterval.Step = 1;
            sliderInterval.TabIndex = 1;
            sliderInterval.TabStop = true;
            sliderInterval.Text = "sliderInterval";
            sliderInterval.Value = 0;
            //
            // labelInterval
            //
            labelInterval.AutoSize = true;
            labelInterval.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelInterval.Location = new Point(21, 9);
            labelInterval.Name = "labelInterval";
            labelInterval.Size = new Size(101, 32);
            labelInterval.TabIndex = 0;
            labelInterval.Text = "Interval";
            //
            // panelSpacer
            //
            panelSpacer.Dock = DockStyle.Top;
            panelSpacer.Location = new Point(15, 217);
            panelSpacer.Name = "panelSpacer";
            panelSpacer.Size = new Size(770, 16);
            panelSpacer.TabIndex = 5;
            //
            // panelAnimationsHeader
            //
            panelAnimationsHeader.AutoSize = true;
            panelAnimationsHeader.BackColor = SystemColors.ControlLight;
            panelAnimationsHeader.Controls.Add(labelAnimations);
            panelAnimationsHeader.Dock = DockStyle.Top;
            panelAnimationsHeader.Location = new Point(15, 217);
            panelAnimationsHeader.Name = "panelAnimationsHeader";
            panelAnimationsHeader.Padding = new Padding(11, 5, 11, 5);
            panelAnimationsHeader.Size = new Size(770, 51);
            panelAnimationsHeader.TabIndex = 3;
            //
            // labelAnimations
            //
            labelAnimations.AutoSize = true;
            labelAnimations.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelAnimations.Location = new Point(21, 9);
            labelAnimations.Name = "labelAnimations";
            labelAnimations.Size = new Size(150, 32);
            labelAnimations.TabIndex = 0;
            labelAnimations.Text = "Animations";
            //
            // panelAnimations
            //
            panelAnimations.AutoSize = true;
            panelAnimations.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelAnimations.Controls.Add(panelDim);
            panelAnimations.Controls.Add(checkBatteryLevel);
            panelAnimations.Controls.Add(checkLowBattery);
            panelAnimations.Controls.Add(panelSleep);
            panelAnimations.Controls.Add(checkBoot);
            panelAnimations.Dock = DockStyle.Top;
            panelAnimations.Location = new Point(15, 268);
            panelAnimations.Name = "panelAnimations";
            panelAnimations.Padding = new Padding(21, 5, 11, 5);
            panelAnimations.Size = new Size(770, 240);
            panelAnimations.TabIndex = 4;
            //
            // panelDim
            //
            panelDim.AutoSize = true;
            panelDim.Controls.Add(comboDim);
            panelDim.Controls.Add(checkPowerSaving);
            panelDim.Dock = DockStyle.Top;
            panelDim.Location = new Point(21, 191);
            panelDim.Name = "panelDim";
            panelDim.Size = new Size(738, 48);
            panelDim.TabIndex = 4;
            //
            // comboDim
            //
            comboDim.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            comboDim.BorderColor = Color.White;
            comboDim.ButtonColor = Color.FromArgb(255, 255, 255);
            comboDim.Font = new Font("Segoe UI", 9F);
            comboDim.FormattingEnabled = true;
            comboDim.Location = new Point(438, 3);
            comboDim.Name = "comboDim";
            comboDim.Size = new Size(300, 40);
            comboDim.TabIndex = 1;
            //
            // checkPowerSaving
            //
            checkPowerSaving.AutoSize = true;
            checkPowerSaving.Dock = DockStyle.Left;
            checkPowerSaving.Location = new Point(0, 0);
            checkPowerSaving.Margin = new Padding(5, 3, 5, 3);
            checkPowerSaving.Name = "checkPowerSaving";
            checkPowerSaving.Padding = new Padding(3);
            checkPowerSaving.Size = new Size(280, 48);
            checkPowerSaving.TabIndex = 0;
            checkPowerSaving.Text = "Dim on low battery";
            checkPowerSaving.UseVisualStyleBackColor = true;
            //
            // checkBatteryLevel
            //
            checkBatteryLevel.AutoSize = true;
            checkBatteryLevel.Dock = DockStyle.Top;
            checkBatteryLevel.Location = new Point(21, 149);
            checkBatteryLevel.Margin = new Padding(5, 3, 5, 3);
            checkBatteryLevel.Name = "checkBatteryLevel";
            checkBatteryLevel.Padding = new Padding(3);
            checkBatteryLevel.Size = new Size(738, 42);
            checkBatteryLevel.TabIndex = 3;
            checkBatteryLevel.Text = "Battery level indicator";
            checkBatteryLevel.UseVisualStyleBackColor = true;
            //
            // checkLowBattery
            //
            checkLowBattery.AutoSize = true;
            checkLowBattery.Dock = DockStyle.Top;
            checkLowBattery.Location = new Point(21, 107);
            checkLowBattery.Margin = new Padding(5, 3, 5, 3);
            checkLowBattery.Name = "checkLowBattery";
            checkLowBattery.Padding = new Padding(3);
            checkLowBattery.Size = new Size(738, 42);
            checkLowBattery.TabIndex = 2;
            checkLowBattery.Text = "Low battery alert";
            checkLowBattery.UseVisualStyleBackColor = true;
            //
            // panelSleep
            //
            panelSleep.AutoSize = true;
            panelSleep.Controls.Add(comboSleepPattern);
            panelSleep.Controls.Add(checkSleepAnimation);
            panelSleep.Dock = DockStyle.Top;
            panelSleep.Location = new Point(21, 47);
            panelSleep.Name = "panelSleep";
            panelSleep.Size = new Size(738, 48);
            panelSleep.TabIndex = 1;
            //
            // comboSleepPattern
            //
            comboSleepPattern.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            comboSleepPattern.BorderColor = Color.White;
            comboSleepPattern.ButtonColor = Color.FromArgb(255, 255, 255);
            comboSleepPattern.Font = new Font("Segoe UI", 9F);
            comboSleepPattern.FormattingEnabled = true;
            comboSleepPattern.Location = new Point(438, 3);
            comboSleepPattern.Name = "comboSleepPattern";
            comboSleepPattern.Size = new Size(300, 40);
            comboSleepPattern.TabIndex = 1;
            //
            // checkSleepAnimation
            //
            checkSleepAnimation.AutoSize = true;
            checkSleepAnimation.Dock = DockStyle.Left;
            checkSleepAnimation.Location = new Point(0, 0);
            checkSleepAnimation.Margin = new Padding(5, 3, 5, 3);
            checkSleepAnimation.Name = "checkSleepAnimation";
            checkSleepAnimation.Padding = new Padding(3);
            checkSleepAnimation.Size = new Size(280, 48);
            checkSleepAnimation.TabIndex = 0;
            checkSleepAnimation.Text = "Sleep animation";
            checkSleepAnimation.UseVisualStyleBackColor = true;
            //
            // checkBoot
            //
            checkBoot.AutoSize = true;
            checkBoot.Dock = DockStyle.Top;
            checkBoot.Location = new Point(21, 5);
            checkBoot.Margin = new Padding(5, 3, 5, 3);
            checkBoot.Name = "checkBoot";
            checkBoot.Padding = new Padding(3);
            checkBoot.Size = new Size(738, 42);
            checkBoot.TabIndex = 0;
            checkBoot.Text = "Startup and shutdown animation";
            checkBoot.UseVisualStyleBackColor = true;
            //
            // Slash
            //
            AutoScaleDimensions = new SizeF(192F, 192F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new Size(800, 523);
            Controls.Add(panelAnimations);
            Controls.Add(panelAnimationsHeader);
            Controls.Add(panelSpacer);
            Controls.Add(panelIntervalHeader);
            Controls.Add(panelPower);
            Controls.Add(panelPowerHeader);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Slash";
            Padding = new Padding(15);
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Slash Lighting";
            panelPowerHeader.ResumeLayout(false);
            panelPowerHeader.PerformLayout();
            panelPower.ResumeLayout(false);
            panelPower.PerformLayout();
            panelIntervalHeader.ResumeLayout(false);
            panelIntervalHeader.PerformLayout();
            panelAnimationsHeader.ResumeLayout(false);
            panelAnimationsHeader.PerformLayout();
            panelAnimations.ResumeLayout(false);
            panelAnimations.PerformLayout();
            panelDim.ResumeLayout(false);
            panelDim.PerformLayout();
            panelSleep.ResumeLayout(false);
            panelSleep.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panelPowerHeader;
        private Label labelPower;
        private Panel panelPower;
        private CheckBox checkAutoOff;
        private CheckBox checkLidOff;
        private Panel panelIntervalHeader;
        private Panel panelSpacer;
        private Slider sliderInterval;
        private Label labelInterval;
        private Panel panelAnimationsHeader;
        private Label labelAnimations;
        private Panel panelAnimations;
        private CheckBox checkBoot;
        private Panel panelSleep;
        private CheckBox checkSleepAnimation;
        private RComboBox comboSleepPattern;
        private CheckBox checkLowBattery;
        private CheckBox checkBatteryLevel;
        private Panel panelDim;
        private CheckBox checkPowerSaving;
        private RComboBox comboDim;
    }
}
