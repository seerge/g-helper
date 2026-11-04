using GHelper.UI;

namespace GHelper
{
    partial class AsusKeyboardSettings
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
            pictureBoxBatteryState = new PictureBox();
            labelBatteryState = new Label();
            panelLightingHeader = new Panel();
            sliderBrightness = new GHelper.UI.Slider();
            pictureBoxLighting = new PictureBox();
            labelLighting = new Label();
            labelLightingMode = new Label();
            comboBoxLightingMode = new GHelper.UI.RComboBox();
            checkBoxSyncAura = new CheckBox();
            buttonLightingColor = new GHelper.UI.RColorButton();
            buttonLightingColor2 = new GHelper.UI.RColorButton();
            buttonLightingColor3 = new GHelper.UI.RColorButton();
            labelAnimationSpeed = new Label();
            comboBoxAnimationSpeed = new GHelper.UI.RComboBox();
            labelProfile = new Label();
            comboBoxProfile = new GHelper.UI.RComboBox();
            labelTestLayout = new Label();
            comboBoxTestLayout = new GHelper.UI.RComboBox();
            panelKeysHeader = new Panel();
            buttonFillAll = new GHelper.UI.RButton();
            buttonResetBindings = new GHelper.UI.RButton();
            pictureBoxKeys = new PictureBox();
            labelKeys = new Label();
            buttonPaintColor = new GHelper.UI.RColorButton();
            panelPalette = new Panel();
            panelKeys = new Panel();
            labelKeyBinding = new Label();
            comboBoxKeyBinding = new GHelper.UI.RComboBox();
            textBoxKeyPath = new GHelper.UI.RTextBox();
            panelEnergyHeader = new Panel();
            pictureBoxEnergy = new PictureBox();
            labelEnergy = new Label();
            labelAutoPowerOff = new Label();
            comboBoxAutoPowerOff = new GHelper.UI.RComboBox();
            labelLowBatteryWarning = new Label();
            sliderLowBatteryWarning = new GHelper.UI.Slider();
            labelLowBatteryWarningValue = new Label();
            panelOled = new Panel();
            panelOledHeader = new Panel();
            sliderOledBrightness = new GHelper.UI.Slider();
            pictureBoxOled = new PictureBox();
            labelOled = new Label();
            labelOledMode = new Label();
            comboBoxOledMode = new GHelper.UI.RComboBox();
            ((System.ComponentModel.ISupportInitialize)pictureBoxBatteryState).BeginInit();
            panelLightingHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxLighting).BeginInit();
            panelKeysHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxKeys).BeginInit();
            panelEnergyHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxEnergy).BeginInit();
            panelOled.SuspendLayout();
            panelOledHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOled).BeginInit();
            SuspendLayout();
            //
            // pictureBoxBatteryState
            //
            pictureBoxBatteryState.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pictureBoxBatteryState.BackgroundImage = Properties.Resources.icons8_batterie_voll_geladen_48;
            pictureBoxBatteryState.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBoxBatteryState.Location = new Point(838, 14);
            pictureBoxBatteryState.Margin = new Padding(4);
            pictureBoxBatteryState.Name = "pictureBoxBatteryState";
            pictureBoxBatteryState.Size = new Size(48, 48);
            pictureBoxBatteryState.TabIndex = 60;
            pictureBoxBatteryState.TabStop = false;
            //
            // labelBatteryState
            //
            labelBatteryState.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelBatteryState.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            labelBatteryState.Location = new Point(730, 14);
            labelBatteryState.Margin = new Padding(4, 0, 4, 0);
            labelBatteryState.Name = "labelBatteryState";
            labelBatteryState.Size = new Size(100, 48);
            labelBatteryState.TabIndex = 61;
            labelBatteryState.Text = "100%";
            labelBatteryState.TextAlign = ContentAlignment.MiddleRight;
            //
            // panelLightingHeader
            //
            panelLightingHeader.BackColor = SystemColors.ControlLight;
            panelLightingHeader.Controls.Add(sliderBrightness);
            panelLightingHeader.Controls.Add(pictureBoxLighting);
            panelLightingHeader.Controls.Add(labelLighting);
            panelLightingHeader.Location = new Point(14, 78);
            panelLightingHeader.Margin = new Padding(4);
            panelLightingHeader.Name = "panelLightingHeader";
            panelLightingHeader.Size = new Size(872, 40);
            panelLightingHeader.TabIndex = 0;
            //
            // sliderBrightness
            //
            sliderBrightness.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            sliderBrightness.Location = new Point(488, 0);
            sliderBrightness.Margin = new Padding(2);
            sliderBrightness.Max = 100;
            sliderBrightness.Min = 0;
            sliderBrightness.Name = "sliderBrightness";
            sliderBrightness.Size = new Size(378, 40);
            sliderBrightness.Step = 1;
            sliderBrightness.TabIndex = 51;
            sliderBrightness.Text = "sliderBrightness";
            sliderBrightness.Value = 100;
            //
            // pictureBoxLighting
            //
            pictureBoxLighting.BackgroundImage = Properties.Resources.backlight;
            pictureBoxLighting.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBoxLighting.Location = new Point(6, 0);
            pictureBoxLighting.Margin = new Padding(4);
            pictureBoxLighting.Name = "pictureBoxLighting";
            pictureBoxLighting.Size = new Size(32, 32);
            pictureBoxLighting.TabIndex = 35;
            pictureBoxLighting.TabStop = false;
            //
            // labelLighting
            //
            labelLighting.AutoSize = true;
            labelLighting.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelLighting.Location = new Point(44, 0);
            labelLighting.Margin = new Padding(8, 0, 8, 0);
            labelLighting.Name = "labelLighting";
            labelLighting.Size = new Size(108, 32);
            labelLighting.TabIndex = 34;
            labelLighting.Text = "Lighting";
            //
            // labelLightingMode
            //
            labelLightingMode.Location = new Point(24, 138);
            labelLightingMode.Margin = new Padding(8, 0, 8, 0);
            labelLightingMode.Name = "labelLightingMode";
            labelLightingMode.Size = new Size(360, 44);
            labelLightingMode.TabIndex = 1;
            labelLightingMode.Text = "Lighting Mode";
            //
            // comboBoxLightingMode
            //
            comboBoxLightingMode.BorderColor = Color.White;
            comboBoxLightingMode.ButtonColor = Color.FromArgb(255, 255, 255);
            comboBoxLightingMode.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxLightingMode.FlatStyle = FlatStyle.Flat;
            comboBoxLightingMode.FormattingEnabled = true;
            comboBoxLightingMode.Location = new Point(440,138);
            comboBoxLightingMode.Margin = new Padding(22, 0, 22, 0);
            comboBoxLightingMode.Name = "comboBoxLightingMode";
            comboBoxLightingMode.Size = new Size(378, 40);
            comboBoxLightingMode.TabIndex = 2;
            //
            // checkBoxSyncAura
            //
            checkBoxSyncAura.AutoSize = true;
            checkBoxSyncAura.Location = new Point(24, 205);
            checkBoxSyncAura.Margin = new Padding(8, 0, 8, 0);
            checkBoxSyncAura.Name = "checkBoxSyncAura";
            checkBoxSyncAura.Size = new Size(420, 30);
            checkBoxSyncAura.TabIndex = 3;
            checkBoxSyncAura.Text = "Sync with Laptop Keyboard";
            checkBoxSyncAura.UseVisualStyleBackColor = true;
            //
            // buttonLightingColor
            //
            buttonLightingColor.AccessibleName = "Keyboard Color";
            buttonLightingColor.Activated = false;
            buttonLightingColor.BackColor = SystemColors.ButtonHighlight;
            buttonLightingColor.BorderColor = Color.Transparent;
            buttonLightingColor.BorderRadius = 2;
            buttonLightingColor.FlatStyle = FlatStyle.Flat;
            buttonLightingColor.ForeColor = SystemColors.ControlText;
            buttonLightingColor.Location = new Point(437, 195);
            buttonLightingColor.Margin = new Padding(0);
            buttonLightingColor.Name = "buttonLightingColor";
            buttonLightingColor.Secondary = false;
            buttonLightingColor.Size = new Size(185, 50);
            buttonLightingColor.TabIndex = 4;
            buttonLightingColor.Text = "Color";
            buttonLightingColor.TextAlign = ContentAlignment.MiddleCenter;
            buttonLightingColor.UseVisualStyleBackColor = false;
            //
            // buttonLightingColor2
            //
            buttonLightingColor2.AccessibleName = "Keyboard Color 2";
            buttonLightingColor2.Activated = false;
            buttonLightingColor2.BackColor = SystemColors.ButtonHighlight;
            buttonLightingColor2.BorderColor = Color.Transparent;
            buttonLightingColor2.BorderRadius = 2;
            buttonLightingColor2.FlatStyle = FlatStyle.Flat;
            buttonLightingColor2.ForeColor = SystemColors.ControlText;
            buttonLightingColor2.Location = new Point(636, 195);
            buttonLightingColor2.Margin = new Padding(0);
            buttonLightingColor2.Name = "buttonLightingColor2";
            buttonLightingColor2.Secondary = false;
            buttonLightingColor2.Size = new Size(185, 50);
            buttonLightingColor2.TabIndex = 5;
            buttonLightingColor2.Text = "Color 2";
            buttonLightingColor2.TextAlign = ContentAlignment.MiddleCenter;
            buttonLightingColor2.UseVisualStyleBackColor = false;
            //
            // buttonLightingColor3
            //
            buttonLightingColor3.AccessibleName = "Keyboard Background Color";
            buttonLightingColor3.Activated = false;
            buttonLightingColor3.BackColor = SystemColors.ButtonHighlight;
            buttonLightingColor3.BorderColor = Color.Transparent;
            buttonLightingColor3.BorderRadius = 2;
            buttonLightingColor3.FlatStyle = FlatStyle.Flat;
            buttonLightingColor3.ForeColor = SystemColors.ControlText;
            buttonLightingColor3.Location = new Point(835, 195);
            buttonLightingColor3.Margin = new Padding(0);
            buttonLightingColor3.Name = "buttonLightingColor3";
            buttonLightingColor3.Secondary = false;
            buttonLightingColor3.Size = new Size(185, 50);
            buttonLightingColor3.TabIndex = 6;
            buttonLightingColor3.Text = "Back";
            buttonLightingColor3.TextAlign = ContentAlignment.MiddleCenter;
            buttonLightingColor3.UseVisualStyleBackColor = false;
            //
            // labelAnimationSpeed
            //
            labelAnimationSpeed.Location = new Point(24, 262);
            labelAnimationSpeed.Margin = new Padding(8, 0, 8, 0);
            labelAnimationSpeed.Name = "labelAnimationSpeed";
            labelAnimationSpeed.Size = new Size(360, 44);
            labelAnimationSpeed.TabIndex = 6;
            labelAnimationSpeed.Text = "Animation Speed";
            //
            // comboBoxAnimationSpeed
            //
            comboBoxAnimationSpeed.BorderColor = Color.White;
            comboBoxAnimationSpeed.ButtonColor = Color.FromArgb(255, 255, 255);
            comboBoxAnimationSpeed.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxAnimationSpeed.FlatStyle = FlatStyle.Flat;
            comboBoxAnimationSpeed.FormattingEnabled = true;
            comboBoxAnimationSpeed.Location = new Point(440,262);
            comboBoxAnimationSpeed.Margin = new Padding(22, 0, 22, 0);
            comboBoxAnimationSpeed.Name = "comboBoxAnimationSpeed";
            comboBoxAnimationSpeed.Size = new Size(378, 40);
            comboBoxAnimationSpeed.TabIndex = 7;
            //
            // labelProfile
            //
            labelProfile.Location = new Point(24, 18);
            labelProfile.Margin = new Padding(8, 0, 8, 0);
            labelProfile.Name = "labelProfile";
            labelProfile.Size = new Size(360, 44);
            labelProfile.TabIndex = 40;
            labelProfile.Text = "Profile";
            //
            // comboBoxProfile
            //
            comboBoxProfile.BorderColor = Color.White;
            comboBoxProfile.ButtonColor = Color.FromArgb(255, 255, 255);
            comboBoxProfile.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxProfile.FlatStyle = FlatStyle.Flat;
            comboBoxProfile.FormattingEnabled = true;
            comboBoxProfile.Location = new Point(440,18);
            comboBoxProfile.Margin = new Padding(22, 0, 22, 0);
            comboBoxProfile.Name = "comboBoxProfile";
            comboBoxProfile.Size = new Size(378, 40);
            comboBoxProfile.TabIndex = 41;
            //
            // labelTestLayout
            //
            labelTestLayout.Location = new Point(24, 302);
            labelTestLayout.Margin = new Padding(8, 0, 8, 0);
            labelTestLayout.Name = "labelTestLayout";
            labelTestLayout.Size = new Size(360, 44);
            labelTestLayout.TabIndex = 45;
            labelTestLayout.Text = "Test Layout";
            labelTestLayout.Visible = false;
            //
            // comboBoxTestLayout
            //
            comboBoxTestLayout.BorderColor = Color.White;
            comboBoxTestLayout.ButtonColor = Color.FromArgb(255, 255, 255);
            comboBoxTestLayout.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxTestLayout.FlatStyle = FlatStyle.Flat;
            comboBoxTestLayout.FormattingEnabled = true;
            comboBoxTestLayout.Location = new Point(440,302);
            comboBoxTestLayout.Margin = new Padding(22, 0, 22, 0);
            comboBoxTestLayout.Name = "comboBoxTestLayout";
            comboBoxTestLayout.Size = new Size(378, 40);
            comboBoxTestLayout.TabIndex = 46;
            comboBoxTestLayout.Visible = false;
            //
            // panelKeysHeader
            //
            panelKeysHeader.BackColor = SystemColors.ControlLight;
            panelKeysHeader.Controls.Add(pictureBoxKeys);
            panelKeysHeader.Controls.Add(labelKeys);
            panelKeysHeader.Location = new Point(14, 304);
            panelKeysHeader.Margin = new Padding(4);
            panelKeysHeader.Name = "panelKeysHeader";
            panelKeysHeader.Size = new Size(872, 40);
            panelKeysHeader.TabIndex = 8;
            //
            // buttonFillAll
            //
            buttonFillAll.Activated = false;
            buttonFillAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonFillAll.BackColor = SystemColors.ControlLightLight;
            buttonFillAll.BorderColor = Color.Transparent;
            buttonFillAll.BorderRadius = 2;
            buttonFillAll.FlatAppearance.BorderSize = 0;
            buttonFillAll.FlatStyle = FlatStyle.Flat;
            buttonFillAll.ForeColor = SystemColors.ControlText;
            buttonFillAll.Image = Properties.Resources.icons8_fill_color_48;
            buttonFillAll.ImageAlign = ContentAlignment.MiddleLeft;
            buttonFillAll.Location = new Point(706, 356);
            buttonFillAll.Margin = new Padding(4);
            buttonFillAll.Name = "buttonFillAll";
            buttonFillAll.Secondary = true;
            buttonFillAll.Size = new Size(180, 50);
            buttonFillAll.TabIndex = 36;
            buttonFillAll.Text = "  Fill All";
            buttonFillAll.TextAlign = ContentAlignment.MiddleLeft;
            buttonFillAll.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonFillAll.UseVisualStyleBackColor = false;
            //
            // buttonResetBindings
            //
            buttonResetBindings.Activated = false;
            buttonResetBindings.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonResetBindings.BackColor = SystemColors.ControlLightLight;
            buttonResetBindings.BorderColor = Color.Transparent;
            buttonResetBindings.BorderRadius = 2;
            buttonResetBindings.FlatAppearance.BorderSize = 0;
            buttonResetBindings.FlatStyle = FlatStyle.Flat;
            buttonResetBindings.ForeColor = SystemColors.ControlText;
            buttonResetBindings.Image = Properties.Resources.icons8_refresh_48;
            buttonResetBindings.ImageAlign = ContentAlignment.MiddleLeft;
            buttonResetBindings.Location = new Point(706, 528);
            buttonResetBindings.Margin = new Padding(4);
            buttonResetBindings.Name = "buttonResetBindings";
            buttonResetBindings.Secondary = true;
            buttonResetBindings.Size = new Size(180, 50);
            buttonResetBindings.TabIndex = 37;
            buttonResetBindings.Text = "Reset";
            buttonResetBindings.TextAlign = ContentAlignment.MiddleLeft;
            buttonResetBindings.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonResetBindings.UseVisualStyleBackColor = false;
            //
            // pictureBoxKeys
            //
            pictureBoxKeys.BackgroundImage = Properties.Resources.icons8_keyboard_32;
            pictureBoxKeys.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBoxKeys.Location = new Point(6, 0);
            pictureBoxKeys.Margin = new Padding(4);
            pictureBoxKeys.Name = "pictureBoxKeys";
            pictureBoxKeys.Size = new Size(32, 32);
            pictureBoxKeys.TabIndex = 35;
            pictureBoxKeys.TabStop = false;
            //
            // labelKeys
            //
            labelKeys.AutoSize = true;
            labelKeys.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelKeys.Location = new Point(44, 0);
            labelKeys.Margin = new Padding(8, 0, 8, 0);
            labelKeys.Name = "labelKeys";
            labelKeys.Size = new Size(64, 32);
            labelKeys.TabIndex = 34;
            labelKeys.Text = "Keys";
            //
            // buttonPaintColor
            //
            buttonPaintColor.AccessibleName = "Paint Color";
            buttonPaintColor.Activated = false;
            buttonPaintColor.BackColor = SystemColors.ButtonHighlight;
            buttonPaintColor.BorderColor = Color.Transparent;
            buttonPaintColor.BorderRadius = 2;
            buttonPaintColor.FlatStyle = FlatStyle.Flat;
            buttonPaintColor.ForeColor = SystemColors.ControlText;
            buttonPaintColor.Location = new Point(24, 356);
            buttonPaintColor.Margin = new Padding(0);
            buttonPaintColor.Name = "buttonPaintColor";
            buttonPaintColor.Secondary = false;
            buttonPaintColor.Size = new Size(180, 50);
            buttonPaintColor.TabIndex = 9;
            buttonPaintColor.Text = "Color";
            buttonPaintColor.TextAlign = ContentAlignment.MiddleCenter;
            buttonPaintColor.UseVisualStyleBackColor = false;
            //
            // panelPalette
            //
            panelPalette.Location = new Point(224, 356);
            panelPalette.Margin = new Padding(4);
            panelPalette.Name = "panelPalette";
            panelPalette.Size = new Size(616, 50);
            panelPalette.TabIndex = 10;
            //
            // panelKeys
            //
            panelKeys.Location = new Point(24, 422);
            panelKeys.Margin = new Padding(4);
            panelKeys.Name = "panelKeys";
            panelKeys.Size = new Size(832, 100);
            panelKeys.TabIndex = 11;
            //
            // labelKeyBinding
            //
            labelKeyBinding.Location = new Point(24, 528);
            labelKeyBinding.Margin = new Padding(8, 0, 8, 0);
            labelKeyBinding.Name = "labelKeyBinding";
            labelKeyBinding.Size = new Size(360, 44);
            labelKeyBinding.TabIndex = 12;
            labelKeyBinding.Text = "Key Bindings";
            //
            // comboBoxKeyBinding
            //
            comboBoxKeyBinding.BorderColor = Color.White;
            comboBoxKeyBinding.ButtonColor = Color.FromArgb(255, 255, 255);
            comboBoxKeyBinding.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxKeyBinding.Enabled = false;
            comboBoxKeyBinding.FlatStyle = FlatStyle.Flat;
            comboBoxKeyBinding.FormattingEnabled = true;
            comboBoxKeyBinding.Location = new Point(440,528);
            comboBoxKeyBinding.Margin = new Padding(22, 0, 22, 0);
            comboBoxKeyBinding.Name = "comboBoxKeyBinding";
            comboBoxKeyBinding.Size = new Size(378, 40);
            comboBoxKeyBinding.TabIndex = 13;
            //
            // textBoxKeyPath
            //
            textBoxKeyPath.Location = new Point(830, 528);
            textBoxKeyPath.Margin = new Padding(5, 3, 5, 3);
            textBoxKeyPath.Name = "textBoxKeyPath";
            textBoxKeyPath.PlaceholderText = "path or url";
            textBoxKeyPath.Size = new Size(300, 40);
            textBoxKeyPath.TabIndex = 14;
            textBoxKeyPath.Visible = false;
            //
            // panelEnergyHeader
            //
            panelEnergyHeader.BackColor = SystemColors.ControlLight;
            panelEnergyHeader.Controls.Add(pictureBoxEnergy);
            panelEnergyHeader.Controls.Add(labelEnergy);
            panelEnergyHeader.Location = new Point(14, 588);
            panelEnergyHeader.Margin = new Padding(4);
            panelEnergyHeader.Name = "panelEnergyHeader";
            panelEnergyHeader.Size = new Size(872, 40);
            panelEnergyHeader.TabIndex = 14;
            //
            // pictureBoxEnergy
            //
            pictureBoxEnergy.BackgroundImage = Properties.Resources.icons8_charging_battery_32;
            pictureBoxEnergy.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBoxEnergy.Location = new Point(6, 0);
            pictureBoxEnergy.Margin = new Padding(4);
            pictureBoxEnergy.Name = "pictureBoxEnergy";
            pictureBoxEnergy.Size = new Size(32, 32);
            pictureBoxEnergy.TabIndex = 35;
            pictureBoxEnergy.TabStop = false;
            //
            // labelEnergy
            //
            labelEnergy.AutoSize = true;
            labelEnergy.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelEnergy.Location = new Point(44, 0);
            labelEnergy.Margin = new Padding(8, 0, 8, 0);
            labelEnergy.Name = "labelEnergy";
            labelEnergy.Size = new Size(108, 32);
            labelEnergy.TabIndex = 34;
            labelEnergy.Text = "Energy";
            //
            // labelAutoPowerOff
            //
            labelAutoPowerOff.Location = new Point(24, 644);
            labelAutoPowerOff.Margin = new Padding(8, 0, 8, 0);
            labelAutoPowerOff.Name = "labelAutoPowerOff";
            labelAutoPowerOff.Size = new Size(360, 44);
            labelAutoPowerOff.TabIndex = 15;
            labelAutoPowerOff.Text = "Auto Power Off";
            //
            // comboBoxAutoPowerOff
            //
            comboBoxAutoPowerOff.BorderColor = Color.White;
            comboBoxAutoPowerOff.ButtonColor = Color.FromArgb(255, 255, 255);
            comboBoxAutoPowerOff.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxAutoPowerOff.FlatStyle = FlatStyle.Flat;
            comboBoxAutoPowerOff.FormattingEnabled = true;
            comboBoxAutoPowerOff.Location = new Point(440,644);
            comboBoxAutoPowerOff.Margin = new Padding(22, 0, 22, 0);
            comboBoxAutoPowerOff.Name = "comboBoxAutoPowerOff";
            comboBoxAutoPowerOff.Size = new Size(378, 40);
            comboBoxAutoPowerOff.TabIndex = 16;
            //
            // labelLowBatteryWarning
            //
            labelLowBatteryWarning.Location = new Point(24, 696);
            labelLowBatteryWarning.Margin = new Padding(8, 0, 8, 0);
            labelLowBatteryWarning.Name = "labelLowBatteryWarning";
            labelLowBatteryWarning.Size = new Size(360, 44);
            labelLowBatteryWarning.TabIndex = 17;
            labelLowBatteryWarning.Text = "Low Battery Warning";
            //
            // sliderLowBatteryWarning
            //
            sliderLowBatteryWarning.Location = new Point(432, 700);
            sliderLowBatteryWarning.Margin = new Padding(4);
            sliderLowBatteryWarning.Max = 50;
            sliderLowBatteryWarning.Min = 0;
            sliderLowBatteryWarning.Name = "sliderLowBatteryWarning";
            sliderLowBatteryWarning.Size = new Size(274, 40);
            sliderLowBatteryWarning.Step = 25;
            sliderLowBatteryWarning.TabIndex = 18;
            sliderLowBatteryWarning.Value = 0;
            //
            // labelLowBatteryWarningValue
            //
            labelLowBatteryWarningValue.Location = new Point(714, 700);
            labelLowBatteryWarningValue.Margin = new Padding(8, 0, 8, 0);
            labelLowBatteryWarningValue.Name = "labelLowBatteryWarningValue";
            labelLowBatteryWarningValue.Size = new Size(104, 40);
            labelLowBatteryWarningValue.TabIndex = 19;
            labelLowBatteryWarningValue.Text = "0%";
            labelLowBatteryWarningValue.TextAlign = ContentAlignment.MiddleRight;
            //
            // panelOled
            //
            panelOled.Controls.Add(panelOledHeader);
            panelOled.Controls.Add(labelOledMode);
            panelOled.Controls.Add(comboBoxOledMode);
            panelOled.Location = new Point(14, 752);
            panelOled.Margin = new Padding(4);
            panelOled.Name = "panelOled";
            panelOled.Size = new Size(872, 104);
            panelOled.TabIndex = 20;
            //
            // panelOledHeader
            //
            panelOledHeader.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelOledHeader.BackColor = SystemColors.ControlLight;
            panelOledHeader.Controls.Add(sliderOledBrightness);
            panelOledHeader.Controls.Add(pictureBoxOled);
            panelOledHeader.Controls.Add(labelOled);
            panelOledHeader.Location = new Point(0, 0);
            panelOledHeader.Margin = new Padding(4);
            panelOledHeader.Name = "panelOledHeader";
            panelOledHeader.Size = new Size(872, 40);
            panelOledHeader.TabIndex = 20;
            //
            // sliderOledBrightness
            //
            sliderOledBrightness.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            sliderOledBrightness.Location = new Point(488, 0);
            sliderOledBrightness.Margin = new Padding(2);
            sliderOledBrightness.Max = 100;
            sliderOledBrightness.Min = 0;
            sliderOledBrightness.Name = "sliderOledBrightness";
            sliderOledBrightness.Size = new Size(378, 40);
            sliderOledBrightness.Step = 25;
            sliderOledBrightness.TabIndex = 21;
            sliderOledBrightness.Value = 100;
            //
            // pictureBoxOled
            //
            pictureBoxOled.BackgroundImage = Properties.Resources.icons8_matrix_32;
            pictureBoxOled.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBoxOled.Location = new Point(6, 0);
            pictureBoxOled.Margin = new Padding(4);
            pictureBoxOled.Name = "pictureBoxOled";
            pictureBoxOled.Size = new Size(32, 32);
            pictureBoxOled.TabIndex = 36;
            pictureBoxOled.TabStop = false;
            //
            // labelOled
            //
            labelOled.AutoSize = true;
            labelOled.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelOled.Location = new Point(44, 0);
            labelOled.Margin = new Padding(8, 0, 8, 0);
            labelOled.Name = "labelOled";
            labelOled.Size = new Size(108, 32);
            labelOled.TabIndex = 37;
            labelOled.Text = "OLED";
            //
            // labelOledMode
            //
            labelOledMode.Location = new Point(10, 60);
            labelOledMode.Margin = new Padding(8, 0, 8, 0);
            labelOledMode.Name = "labelOledMode";
            labelOledMode.Size = new Size(360, 44);
            labelOledMode.TabIndex = 24;
            labelOledMode.Text = "Animation";
            //
            // comboBoxOledMode
            //
            comboBoxOledMode.BorderColor = Color.White;
            comboBoxOledMode.ButtonColor = Color.FromArgb(255, 255, 255);
            comboBoxOledMode.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxOledMode.FlatStyle = FlatStyle.Flat;
            comboBoxOledMode.FormattingEnabled = true;
            comboBoxOledMode.Location = new Point(426, 60);
            comboBoxOledMode.Margin = new Padding(22, 0, 22, 0);
            comboBoxOledMode.Name = "comboBoxOledMode";
            comboBoxOledMode.Size = new Size(378, 40);
            comboBoxOledMode.TabIndex = 25;
            //
            // AsusKeyboardSettings
            //
            AutoScaleDimensions = new SizeF(192F, 192F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(900, 548);
            Controls.Add(pictureBoxBatteryState);
            Controls.Add(labelBatteryState);
            Controls.Add(panelLightingHeader);
            Controls.Add(labelLightingMode);
            Controls.Add(comboBoxLightingMode);
            Controls.Add(checkBoxSyncAura);
            Controls.Add(buttonLightingColor);
            Controls.Add(buttonLightingColor2);
            Controls.Add(buttonLightingColor3);
            Controls.Add(labelAnimationSpeed);
            Controls.Add(comboBoxAnimationSpeed);
            Controls.Add(labelProfile);
            Controls.Add(comboBoxProfile);
            Controls.Add(labelTestLayout);
            Controls.Add(comboBoxTestLayout);
            Controls.Add(panelKeysHeader);
            Controls.Add(buttonPaintColor);
            Controls.Add(panelPalette);
            Controls.Add(buttonFillAll);
            Controls.Add(panelKeys);
            Controls.Add(labelKeyBinding);
            Controls.Add(comboBoxKeyBinding);
            Controls.Add(textBoxKeyPath);
            Controls.Add(buttonResetBindings);
            Controls.Add(panelEnergyHeader);
            Controls.Add(labelAutoPowerOff);
            Controls.Add(comboBoxAutoPowerOff);
            Controls.Add(labelLowBatteryWarning);
            Controls.Add(sliderLowBatteryWarning);
            Controls.Add(labelLowBatteryWarningValue);
            Controls.Add(panelOled);
            Margin = new Padding(4);
            MaximizeBox = false;
            MdiChildrenMinimizedAnchorBottom = false;
            MinimizeBox = false;
            Name = "AsusKeyboardSettings";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Keyboard Settings";
            ((System.ComponentModel.ISupportInitialize)pictureBoxBatteryState).EndInit();
            panelLightingHeader.ResumeLayout(false);
            panelLightingHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxLighting).EndInit();
            panelKeysHeader.ResumeLayout(false);
            panelKeysHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxKeys).EndInit();
            panelEnergyHeader.ResumeLayout(false);
            panelEnergyHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxEnergy).EndInit();
            panelOled.ResumeLayout(false);
            panelOledHeader.ResumeLayout(false);
            panelOledHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOled).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBoxBatteryState;
        private Label labelBatteryState;
        private Panel panelLightingHeader;
        private GHelper.UI.Slider sliderBrightness;
        private PictureBox pictureBoxLighting;
        private Label labelLighting;
        private Label labelLightingMode;
        private GHelper.UI.RComboBox comboBoxLightingMode;
        private CheckBox checkBoxSyncAura;
        private GHelper.UI.RColorButton buttonLightingColor;
        private GHelper.UI.RColorButton buttonLightingColor2;
        private GHelper.UI.RColorButton buttonLightingColor3;
        private Label labelAnimationSpeed;
        private GHelper.UI.RComboBox comboBoxAnimationSpeed;
        private Label labelProfile;
        private GHelper.UI.RComboBox comboBoxProfile;
        private Label labelTestLayout;
        private GHelper.UI.RComboBox comboBoxTestLayout;
        private Panel panelKeysHeader;
        private GHelper.UI.RButton buttonFillAll;
        private GHelper.UI.RButton buttonResetBindings;
        private PictureBox pictureBoxKeys;
        private Label labelKeys;
        private GHelper.UI.RColorButton buttonPaintColor;
        private Panel panelPalette;
        private Panel panelKeys;
        private Label labelKeyBinding;
        private GHelper.UI.RComboBox comboBoxKeyBinding;
        private GHelper.UI.RTextBox textBoxKeyPath;
        private Panel panelEnergyHeader;
        private PictureBox pictureBoxEnergy;
        private Label labelEnergy;
        private Label labelAutoPowerOff;
        private GHelper.UI.RComboBox comboBoxAutoPowerOff;
        private Label labelLowBatteryWarning;
        private GHelper.UI.Slider sliderLowBatteryWarning;
        private Label labelLowBatteryWarningValue;
        private Panel panelOled;
        private Panel panelOledHeader;
        private GHelper.UI.Slider sliderOledBrightness;
        private PictureBox pictureBoxOled;
        private Label labelOled;
        private Label labelOledMode;
        private GHelper.UI.RComboBox comboBoxOledMode;
    }
}
