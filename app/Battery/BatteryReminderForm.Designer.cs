using GHelper.UI;

namespace GHelper.Battery
{
    partial class BatteryReminderForm
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
            labelClose = new Label();
            labelTitle = new Label();
            labelSubtitle = new Label();
            labelIssue1 = new Label();
            labelIssue2 = new Label();
            labelIssue3 = new Label();
            checkAutoOptimize = new CheckBox();
            buttonOptimize = new RButton();
            buttonDismiss = new RButton();
            SuspendLayout();
            // 
            // labelClose
            // 
            labelClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelClose.AutoSize = true;
            labelClose.Cursor = Cursors.Hand;
            labelClose.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            labelClose.Location = new Point(613, 6);
            labelClose.Name = "labelClose";
            labelClose.Size = new Size(40, 37);
            labelClose.TabIndex = 0;
            labelClose.Text = "✕";
            // 
            // labelTitle
            // 
            labelTitle.AutoSize = true;
            labelTitle.Font = new Font("Segoe UI Semibold", 12F);
            labelTitle.Location = new Point(28, 20);
            labelTitle.Name = "labelTitle";
            labelTitle.Size = new Size(472, 45);
            labelTitle.TabIndex = 1;
            labelTitle.Text = "Battery Optimization Reminder";
            // 
            // labelSubtitle
            // 
            labelSubtitle.AutoSize = true;
            labelSubtitle.Font = new Font("Segoe UI", 9.5F);
            labelSubtitle.Location = new Point(28, 71);
            labelSubtitle.Name = "labelSubtitle";
            labelSubtitle.Size = new Size(539, 36);
            labelSubtitle.TabIndex = 2;
            labelSubtitle.Text = "Your current settings may drain battery faster:";
            // 
            // labelIssue1
            // 
            labelIssue1.AutoSize = true;
            labelIssue1.Font = new Font("Segoe UI", 9.5F);
            labelIssue1.Location = new Point(42, 110);
            labelIssue1.Name = "labelIssue1";
            labelIssue1.Size = new Size(0, 36);
            labelIssue1.TabIndex = 3;
            // 
            // labelIssue2
            // 
            labelIssue2.AutoSize = true;
            labelIssue2.Font = new Font("Segoe UI", 9.5F);
            labelIssue2.Location = new Point(42, 141);
            labelIssue2.Name = "labelIssue2";
            labelIssue2.Size = new Size(0, 36);
            labelIssue2.TabIndex = 4;
            // 
            // labelIssue3
            // 
            labelIssue3.AutoSize = true;
            labelIssue3.Font = new Font("Segoe UI", 9.5F);
            labelIssue3.Location = new Point(42, 172);
            labelIssue3.Name = "labelIssue3";
            labelIssue3.Size = new Size(0, 36);
            labelIssue3.TabIndex = 5;
            // 
            // checkAutoOptimize
            // 
            checkAutoOptimize.AutoSize = true;
            checkAutoOptimize.FlatStyle = FlatStyle.Flat;
            checkAutoOptimize.Font = new Font("Segoe UI", 9.5F);
            checkAutoOptimize.Location = new Point(42, 210);
            checkAutoOptimize.Name = "checkAutoOptimize";
            checkAutoOptimize.Padding = new Padding(4, 2, 4, 2);
            checkAutoOptimize.Size = new Size(367, 44);
            checkAutoOptimize.TabIndex = 6;
            checkAutoOptimize.Text = "Always switch automatically";
            // 
            // buttonOptimize
            // 
            buttonOptimize.Activated = false;
            buttonOptimize.BorderColor = Color.Transparent;
            buttonOptimize.BorderRadius = 20;
            buttonOptimize.FlatStyle = FlatStyle.Flat;
            buttonOptimize.Font = new Font("Segoe UI Semibold", 10F);
            buttonOptimize.Location = new Point(11, 258);
            buttonOptimize.Name = "buttonOptimize";
            buttonOptimize.Secondary = false;
            buttonOptimize.Size = new Size(332, 50);
            buttonOptimize.TabIndex = 7;
            buttonOptimize.Text = "Optimize for Battery";
            // 
            // buttonDismiss
            // 
            buttonDismiss.Activated = false;
            buttonDismiss.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonDismiss.BorderColor = Color.Transparent;
            buttonDismiss.BorderRadius = 20;
            buttonDismiss.FlatStyle = FlatStyle.Flat;
            buttonDismiss.Font = new Font("Segoe UI", 10F);
            buttonDismiss.Location = new Point(376, 258);
            buttonDismiss.Name = "buttonDismiss";
            buttonDismiss.Secondary = true;
            buttonDismiss.Size = new Size(274, 50);
            buttonDismiss.TabIndex = 8;
            buttonDismiss.Text = "Don't remind me";
            // 
            // BatteryReminderForm
            // 
            AutoScaleDimensions = new SizeF(192F, 192F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(661, 340);
            Controls.Add(labelClose);
            Controls.Add(labelTitle);
            Controls.Add(labelSubtitle);
            Controls.Add(labelIssue1);
            Controls.Add(labelIssue2);
            Controls.Add(labelIssue3);
            Controls.Add(checkAutoOptimize);
            Controls.Add(buttonOptimize);
            Controls.Add(buttonDismiss);
            FormBorderStyle = FormBorderStyle.None;
            Name = "BatteryReminderForm";
            Padding = new Padding(8);
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelClose;
        private Label labelTitle;
        private Label labelSubtitle;
        private Label labelIssue1;
        private Label labelIssue2;
        private Label labelIssue3;
        private CheckBox checkAutoOptimize;
        private RButton buttonOptimize;
        private RButton buttonDismiss;
    }
}
