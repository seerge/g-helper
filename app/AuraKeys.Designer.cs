using GHelper.UI;

namespace GHelper
{
    partial class AuraKeys
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
            buttonPaintColor = new GHelper.UI.RColorButton();
            panelPalette = new Panel();
            buttonFillAll = new GHelper.UI.RButton();
            comboBoxLayout = new GHelper.UI.RComboBox();
            panelKeys = new Panel();
            SuspendLayout();
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
            buttonPaintColor.Location = new Point(24, 24);
            buttonPaintColor.Margin = new Padding(0);
            buttonPaintColor.Name = "buttonPaintColor";
            buttonPaintColor.Secondary = false;
            buttonPaintColor.Size = new Size(180, 50);
            buttonPaintColor.TabIndex = 0;
            buttonPaintColor.Text = "Color";
            buttonPaintColor.TextAlign = ContentAlignment.MiddleCenter;
            buttonPaintColor.UseVisualStyleBackColor = false;
            //
            // panelPalette
            //
            panelPalette.Location = new Point(224, 24);
            panelPalette.Margin = new Padding(4);
            panelPalette.Name = "panelPalette";
            panelPalette.Size = new Size(616, 50);
            panelPalette.TabIndex = 1;
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
            buttonFillAll.Location = new Point(706, 24);
            buttonFillAll.Margin = new Padding(4);
            buttonFillAll.Name = "buttonFillAll";
            buttonFillAll.Secondary = true;
            buttonFillAll.Size = new Size(180, 50);
            buttonFillAll.TabIndex = 2;
            buttonFillAll.Text = "  Fill All";
            buttonFillAll.TextAlign = ContentAlignment.MiddleLeft;
            buttonFillAll.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonFillAll.UseVisualStyleBackColor = false;
            //
            // comboBoxLayout
            //
            comboBoxLayout.BorderColor = Color.White;
            comboBoxLayout.ButtonColor = Color.FromArgb(255, 255, 255);
            comboBoxLayout.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxLayout.FlatStyle = FlatStyle.Flat;
            comboBoxLayout.FormattingEnabled = true;
            comboBoxLayout.Location = new Point(24, 98);
            comboBoxLayout.Margin = new Padding(4);
            comboBoxLayout.Name = "comboBoxLayout";
            comboBoxLayout.Size = new Size(378, 40);
            comboBoxLayout.TabIndex = 3;
            comboBoxLayout.Visible = false;
            //
            // panelKeys
            //
            panelKeys.Location = new Point(24, 98);
            panelKeys.Margin = new Padding(4);
            panelKeys.Name = "panelKeys";
            panelKeys.Size = new Size(832, 100);
            panelKeys.TabIndex = 4;
            //
            // AuraKeys
            //
            AutoScaleDimensions = new SizeF(192F, 192F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(900, 222);
            Controls.Add(buttonPaintColor);
            Controls.Add(panelPalette);
            Controls.Add(buttonFillAll);
            Controls.Add(comboBoxLayout);
            Controls.Add(panelKeys);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4);
            MaximizeBox = false;
            MdiChildrenMinimizedAnchorBottom = false;
            MinimizeBox = false;
            Name = "AuraKeys";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Laptop Keyboard";
            ResumeLayout(false);
        }

        #endregion

        private GHelper.UI.RColorButton buttonPaintColor;
        private Panel panelPalette;
        private GHelper.UI.RButton buttonFillAll;
        private GHelper.UI.RComboBox comboBoxLayout;
        private Panel panelKeys;
    }
}
