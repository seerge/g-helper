using GHelper.UI;
namespace GHelper
{
    partial class Routines
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
            panelLockHeader = new Panel();
            labelLockTitle = new Label();
            panelLock = new Panel();
            comboLock = new RComboBox();
            labelLock = new Label();
            labelHint = new Label();
            panelAppsHeader = new Panel();
            buttonRemove = new RButton();
            buttonAdd = new RButton();
            labelApps = new Label();
            panelApps = new Panel();
            labelStatus = new Label();
            textTitle = new RTextBox();
            labelTitle = new Label();
            comboMode = new RComboBox();
            labelMode = new Label();
            comboApps = new RComboBox();
            panelLockHeader.SuspendLayout();
            panelLock.SuspendLayout();
            panelAppsHeader.SuspendLayout();
            panelApps.SuspendLayout();
            SuspendLayout();
            //
            // panelLockHeader
            //
            panelLockHeader.AutoSize = true;
            panelLockHeader.BackColor = SystemColors.ControlLight;
            panelLockHeader.Controls.Add(labelLockTitle);
            panelLockHeader.Dock = DockStyle.Top;
            panelLockHeader.Location = new Point(15, 15);
            panelLockHeader.Name = "panelLockHeader";
            panelLockHeader.Padding = new Padding(11, 5, 11, 5);
            panelLockHeader.Size = new Size(770, 51);
            panelLockHeader.TabIndex = 0;
            //
            // labelLockTitle
            //
            labelLockTitle.AutoSize = true;
            labelLockTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelLockTitle.Location = new Point(21, 9);
            labelLockTitle.Name = "labelLockTitle";
            labelLockTitle.Size = new Size(108, 32);
            labelLockTitle.TabIndex = 0;
            labelLockTitle.Text = "Routines";
            //
            // panelLock
            //
            panelLock.AutoSize = true;
            panelLock.Controls.Add(comboLock);
            panelLock.Controls.Add(labelLock);
            panelLock.Controls.Add(labelHint);
            panelLock.Dock = DockStyle.Top;
            panelLock.Location = new Point(15, 66);
            panelLock.Name = "panelLock";
            panelLock.Padding = new Padding(11, 5, 11, 10);
            panelLock.Size = new Size(770, 152);
            panelLock.TabIndex = 1;
            //
            // comboLock
            //
            comboLock.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            comboLock.BorderColor = Color.White;
            comboLock.ButtonColor = Color.FromArgb(255, 255, 255);
            comboLock.Font = new Font("Segoe UI", 9F);
            comboLock.FormattingEnabled = true;
            comboLock.Location = new Point(438, 99);
            comboLock.Name = "comboLock";
            comboLock.Size = new Size(320, 40);
            comboLock.TabIndex = 2;
            //
            // labelLock
            //
            labelLock.AutoSize = true;
            labelLock.Location = new Point(21, 102);
            labelLock.Name = "labelLock";
            labelLock.Size = new Size(147, 32);
            labelLock.TabIndex = 1;
            labelLock.Text = "When Locked";
            //
            // labelHint
            //
            labelHint.Dock = DockStyle.Top;
            labelHint.ForeColor = SystemColors.GrayText;
            labelHint.Location = new Point(11, 5);
            labelHint.Name = "labelHint";
            labelHint.Padding = new Padding(10, 5, 0, 0);
            labelHint.Size = new Size(748, 86);
            labelHint.TabIndex = 0;
            labelHint.Text = "Switch mode while app is running or PC is locked. Previous mode is restored after.";
            //
            // panelAppsHeader
            //
            panelAppsHeader.BackColor = SystemColors.ControlLight;
            panelAppsHeader.Controls.Add(buttonRemove);
            panelAppsHeader.Controls.Add(buttonAdd);
            panelAppsHeader.Controls.Add(labelApps);
            panelAppsHeader.Dock = DockStyle.Top;
            panelAppsHeader.Location = new Point(15, 218);
            panelAppsHeader.Name = "panelAppsHeader";
            panelAppsHeader.Padding = new Padding(11, 5, 11, 5);
            panelAppsHeader.Size = new Size(770, 62);
            panelAppsHeader.TabIndex = 2;
            //
            // buttonRemove
            //
            buttonRemove.Activated = false;
            buttonRemove.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonRemove.BackColor = SystemColors.ControlLight;
            buttonRemove.BorderColor = Color.Transparent;
            buttonRemove.BorderRadius = 2;
            buttonRemove.FlatStyle = FlatStyle.Flat;
            buttonRemove.Image = Properties.Resources.icons8_remove_64;
            buttonRemove.Location = new Point(644, 8);
            buttonRemove.Margin = new Padding(0, 0, 6, 0);
            buttonRemove.Name = "buttonRemove";
            buttonRemove.Secondary = true;
            buttonRemove.Size = new Size(54, 46);
            buttonRemove.TabIndex = 1;
            buttonRemove.UseVisualStyleBackColor = false;
            //
            // buttonAdd
            //
            buttonAdd.Activated = false;
            buttonAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonAdd.BackColor = SystemColors.ControlLight;
            buttonAdd.BorderColor = Color.Transparent;
            buttonAdd.BorderRadius = 2;
            buttonAdd.FlatStyle = FlatStyle.Flat;
            buttonAdd.Image = Properties.Resources.icons8_add_64;
            buttonAdd.Location = new Point(704, 8);
            buttonAdd.Margin = new Padding(0, 0, 6, 0);
            buttonAdd.Name = "buttonAdd";
            buttonAdd.Secondary = true;
            buttonAdd.Size = new Size(54, 46);
            buttonAdd.TabIndex = 2;
            buttonAdd.UseVisualStyleBackColor = false;
            //
            // labelApps
            //
            labelApps.AutoSize = true;
            labelApps.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelApps.Location = new Point(21, 15);
            labelApps.Name = "labelApps";
            labelApps.Size = new Size(190, 32);
            labelApps.TabIndex = 0;
            labelApps.Text = "Apps and Games";
            //
            // panelApps
            //
            panelApps.AutoSize = true;
            panelApps.Controls.Add(labelStatus);
            panelApps.Controls.Add(textTitle);
            panelApps.Controls.Add(labelTitle);
            panelApps.Controls.Add(comboMode);
            panelApps.Controls.Add(labelMode);
            panelApps.Controls.Add(comboApps);
            panelApps.Dock = DockStyle.Top;
            panelApps.Location = new Point(15, 280);
            panelApps.Name = "panelApps";
            panelApps.Padding = new Padding(11, 5, 11, 10);
            panelApps.Size = new Size(770, 248);
            panelApps.TabIndex = 3;
            //
            // labelStatus
            //
            labelStatus.AutoSize = true;
            labelStatus.ForeColor = SystemColors.GrayText;
            labelStatus.Location = new Point(21, 204);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(0, 32);
            labelStatus.TabIndex = 5;
            //
            // textTitle
            //
            textTitle.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            textTitle.Location = new Point(438, 136);
            textTitle.Name = "textTitle";
            textTitle.Size = new Size(320, 39);
            textTitle.TabIndex = 4;
            //
            // labelTitle
            //
            labelTitle.AutoSize = true;
            labelTitle.Location = new Point(21, 139);
            labelTitle.Name = "labelTitle";
            labelTitle.Size = new Size(147, 32);
            labelTitle.TabIndex = 3;
            labelTitle.Text = "Window Title";
            //
            // comboMode
            //
            comboMode.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            comboMode.BorderColor = Color.White;
            comboMode.ButtonColor = Color.FromArgb(255, 255, 255);
            comboMode.Font = new Font("Segoe UI", 9F);
            comboMode.FormattingEnabled = true;
            comboMode.Location = new Point(438, 80);
            comboMode.Name = "comboMode";
            comboMode.Size = new Size(320, 40);
            comboMode.TabIndex = 2;
            //
            // labelMode
            //
            labelMode.AutoSize = true;
            labelMode.Location = new Point(21, 83);
            labelMode.Name = "labelMode";
            labelMode.Size = new Size(68, 32);
            labelMode.TabIndex = 1;
            labelMode.Text = "Mode";
            //
            // comboApps
            //
            comboApps.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            comboApps.BorderColor = Color.White;
            comboApps.ButtonColor = Color.FromArgb(255, 255, 255);
            comboApps.Font = new Font("Segoe UI", 9F);
            comboApps.FormattingEnabled = true;
            comboApps.Location = new Point(21, 18);
            comboApps.Name = "comboApps";
            comboApps.Size = new Size(737, 40);
            comboApps.TabIndex = 0;
            //
            // Routines
            //
            AutoScaleDimensions = new SizeF(192F, 192F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new Size(800, 543);
            Controls.Add(panelApps);
            Controls.Add(panelAppsHeader);
            Controls.Add(panelLock);
            Controls.Add(panelLockHeader);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Routines";
            Padding = new Padding(15);
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Routines";
            panelLockHeader.ResumeLayout(false);
            panelLockHeader.PerformLayout();
            panelLock.ResumeLayout(false);
            panelLock.PerformLayout();
            panelAppsHeader.ResumeLayout(false);
            panelAppsHeader.PerformLayout();
            panelApps.ResumeLayout(false);
            panelApps.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panelLockHeader;
        private Label labelLockTitle;
        private Panel panelLock;
        private RComboBox comboLock;
        private Label labelLock;
        private Label labelHint;
        private Panel panelAppsHeader;
        private RButton buttonRemove;
        private RButton buttonAdd;
        private Label labelApps;
        private Panel panelApps;
        private Label labelStatus;
        private RTextBox textTitle;
        private Label labelTitle;
        private RComboBox comboMode;
        private Label labelMode;
        private RComboBox comboApps;
    }
}
