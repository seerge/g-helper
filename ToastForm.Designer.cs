namespace GHelper
{
    partial class ToastForm
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
            this.pictureIcon = new System.Windows.Forms.PictureBox();
            this.labelMode = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureIcon
            // 
            this.pictureIcon.BackgroundImage = global::GHelper.Properties.Resources.icons8_speed_96;
            this.pictureIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureIcon.Location = new System.Drawing.Point(21, 21);
            this.pictureIcon.Name = "pictureIcon";
            this.pictureIcon.Size = new System.Drawing.Size(82, 80);
            this.pictureIcon.TabIndex = 0;
            this.pictureIcon.TabStop = false;
            // 
            // labelMode
            // 
            this.labelMode.AutoSize = true;
            this.labelMode.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelMode.Location = new System.Drawing.Point(127, 32);
            this.labelMode.Name = "labelMode";
            this.labelMode.Size = new System.Drawing.Size(195, 59);
            this.labelMode.TabIndex = 1;
            this.labelMode.Text = "Balanced";
            this.labelMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ToastForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(356, 122);
            this.Controls.Add(this.labelMode);
            this.Controls.Add(this.pictureIcon);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MdiChildrenMinimizedAnchorBottom = false;
            this.MinimizeBox = false;
            this.Name = "ToastForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "ToastForm";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PictureBox pictureIcon;
        private Label labelMode;
    }
}