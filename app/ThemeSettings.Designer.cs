using GHelper.UI;

namespace GHelper
{
    partial class ThemeSettings
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
            components = new System.ComponentModel.Container();
            panelSchedule = new Panel();
            checkSchedule = new CheckBox();
            tableLocation = new TableLayoutPanel();
            labelLatitude = new Label();
            textLatitude = new RTextBox();
            labelLongitude = new Label();
            textLongitude = new RTextBox();
            labelElevation = new Label();
            textElevation = new RTextBox();
            panelCrossings = new Panel();
            labelCrossings = new Label();
            panelButtons = new Panel();
            buttonSave = new RButton();
            buttonCancel = new RButton();
            toolTip = new ToolTip(components);
            panelSchedule.SuspendLayout();
            tableLocation.SuspendLayout();
            panelCrossings.SuspendLayout();
            panelButtons.SuspendLayout();
            SuspendLayout();
            //
            // checkSchedule
            //
            checkSchedule.AutoSize = true;
            checkSchedule.Location = new Point(16, 14);
            checkSchedule.Margin = new Padding(4);
            checkSchedule.Name = "checkSchedule";
            checkSchedule.Size = new Size(400, 24);
            checkSchedule.TabIndex = 0;
            checkSchedule.Text = "Switch Windows theme with the sun";
            checkSchedule.UseVisualStyleBackColor = true;
            //
            // panelSchedule
            //
            panelSchedule.AutoSize = true;
            panelSchedule.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelSchedule.Controls.Add(checkSchedule);
            panelSchedule.Dock = DockStyle.Top;
            panelSchedule.Location = new Point(0, 0);
            panelSchedule.Name = "panelSchedule";
            panelSchedule.Size = new Size(380, 52);
            panelSchedule.TabIndex = 0;
            //
            // labelLatitude
            //
            labelLatitude.Anchor = AnchorStyles.Left;
            labelLatitude.AutoSize = true;
            labelLatitude.Name = "labelLatitude";
            labelLatitude.Size = new Size(100, 20);
            labelLatitude.Text = "Latitude";
            //
            // textLatitude
            //
            textLatitude.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textLatitude.Margin = new Padding(4);
            textLatitude.Name = "textLatitude";
            textLatitude.Size = new Size(240, 27);
            textLatitude.TabIndex = 1;
            //
            // labelLongitude
            //
            labelLongitude.Anchor = AnchorStyles.Left;
            labelLongitude.AutoSize = true;
            labelLongitude.Name = "labelLongitude";
            labelLongitude.Size = new Size(100, 20);
            labelLongitude.Text = "Longitude";
            //
            // textLongitude
            //
            textLongitude.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textLongitude.Margin = new Padding(4);
            textLongitude.Name = "textLongitude";
            textLongitude.Size = new Size(240, 27);
            textLongitude.TabIndex = 2;
            //
            // labelElevation
            //
            labelElevation.Anchor = AnchorStyles.Left;
            labelElevation.AutoSize = true;
            labelElevation.Name = "labelElevation";
            labelElevation.Size = new Size(100, 20);
            labelElevation.Text = "Sun elevation";
            //
            // textElevation
            //
            textElevation.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textElevation.Margin = new Padding(4);
            textElevation.Name = "textElevation";
            textElevation.Size = new Size(240, 27);
            textElevation.TabIndex = 3;
            //
            // tableLocation
            //
            tableLocation.AutoSize = true;
            tableLocation.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLocation.ColumnCount = 2;
            tableLocation.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            tableLocation.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLocation.Controls.Add(labelLatitude, 0, 0);
            tableLocation.Controls.Add(textLatitude, 1, 0);
            tableLocation.Controls.Add(labelLongitude, 0, 1);
            tableLocation.Controls.Add(textLongitude, 1, 1);
            tableLocation.Controls.Add(labelElevation, 0, 2);
            tableLocation.Controls.Add(textElevation, 1, 2);
            tableLocation.Dock = DockStyle.Top;
            tableLocation.Location = new Point(0, 52);
            tableLocation.Name = "tableLocation";
            tableLocation.Padding = new Padding(12, 0, 12, 0);
            tableLocation.RowCount = 3;
            tableLocation.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLocation.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLocation.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLocation.TabIndex = 1;
            //
            // labelCrossings
            //
            labelCrossings.AutoSize = true;
            labelCrossings.Location = new Point(16, 8);
            labelCrossings.Margin = new Padding(4);
            labelCrossings.MaximumSize = new Size(348, 0);
            labelCrossings.MinimumSize = new Size(348, 20);
            labelCrossings.Name = "labelCrossings";
            labelCrossings.TabIndex = 0;
            //
            // panelCrossings
            //
            panelCrossings.AutoSize = true;
            panelCrossings.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelCrossings.Controls.Add(labelCrossings);
            panelCrossings.Dock = DockStyle.Top;
            panelCrossings.Name = "panelCrossings";
            panelCrossings.Size = new Size(380, 44);
            panelCrossings.TabIndex = 2;
            //
            // buttonSave
            //
            buttonSave.Activated = false;
            buttonSave.BorderRadius = 4;
            buttonSave.Location = new Point(160, 8);
            buttonSave.Margin = new Padding(4);
            buttonSave.Name = "buttonSave";
            buttonSave.Secondary = false;
            buttonSave.Size = new Size(100, 32);
            buttonSave.TabIndex = 4;
            buttonSave.Text = "Save";
            buttonSave.UseVisualStyleBackColor = true;
            //
            // buttonCancel
            //
            buttonCancel.Activated = false;
            buttonCancel.BorderRadius = 4;
            buttonCancel.Location = new Point(268, 8);
            buttonCancel.Margin = new Padding(4);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Secondary = true;
            buttonCancel.Size = new Size(100, 32);
            buttonCancel.TabIndex = 5;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            //
            // panelButtons
            //
            panelButtons.AutoSize = true;
            panelButtons.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelButtons.Controls.Add(buttonSave);
            panelButtons.Controls.Add(buttonCancel);
            panelButtons.Dock = DockStyle.Top;
            panelButtons.Padding = new Padding(0, 0, 0, 11);
            panelButtons.Name = "panelButtons";
            panelButtons.Size = new Size(380, 51);
            panelButtons.TabIndex = 3;
            //
            // ThemeSettings
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = false;
            ClientSize = new Size(380, 260);
            Controls.Add(panelButtons);
            Controls.Add(panelCrossings);
            Controls.Add(tableLocation);
            Controls.Add(panelSchedule);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ThemeSettings";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Text = "Theme Settings";
            panelSchedule.ResumeLayout(false);
            panelSchedule.PerformLayout();
            tableLocation.ResumeLayout(false);
            tableLocation.PerformLayout();
            panelCrossings.ResumeLayout(false);
            panelCrossings.PerformLayout();
            panelButtons.ResumeLayout(false);
            panelButtons.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panelSchedule;
        private CheckBox checkSchedule;
        private TableLayoutPanel tableLocation;
        private Label labelLatitude;
        private RTextBox textLatitude;
        private Label labelLongitude;
        private RTextBox textLongitude;
        private Label labelElevation;
        private RTextBox textElevation;
        private Panel panelCrossings;
        private Label labelCrossings;
        private Panel panelButtons;
        private RButton buttonSave;
        private RButton buttonCancel;
        private ToolTip toolTip;
    }
}
