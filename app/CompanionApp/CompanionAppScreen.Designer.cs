namespace GHelper.CompanionApp
{
    partial class CompanionAppScreen
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
            labelIpAddress = new Label();
            buttonStart = new UI.RButton();
            rbWiFi = new RadioButton();
            rbBLE = new RadioButton();
            SuspendLayout();
            // 
            // labelIpAddress
            // 
            labelIpAddress.AutoSize = true;
            labelIpAddress.Location = new Point(23, 70);
            labelIpAddress.Name = "labelIpAddress";
            labelIpAddress.Size = new Size(114, 20);
            labelIpAddress.TabIndex = 0;
            labelIpAddress.Text = "Your IP Address:";
            // 
            // buttonStart
            // 
            buttonStart.Activated = false;
            buttonStart.BackColor = SystemColors.ControlLight;
            buttonStart.BorderColor = Color.Transparent;
            buttonStart.BorderRadius = 5;
            buttonStart.FlatAppearance.BorderSize = 0;
            buttonStart.FlatStyle = FlatStyle.Flat;
            buttonStart.Location = new Point(362, 22);
            buttonStart.Name = "buttonStart";
            buttonStart.Secondary = false;
            buttonStart.Size = new Size(94, 68);
            buttonStart.TabIndex = 2;
            buttonStart.Text = "Start";
            buttonStart.UseVisualStyleBackColor = false;
            buttonStart.Click += buttonStart_Click;
            // 
            // rbWiFi
            // 
            rbWiFi.AutoSize = true;
            rbWiFi.Location = new Point(23, 32);
            rbWiFi.Name = "rbWiFi";
            rbWiFi.Size = new Size(63, 24);
            rbWiFi.TabIndex = 4;
            rbWiFi.TabStop = true;
            rbWiFi.Text = "Wi Fi";
            rbWiFi.UseVisualStyleBackColor = true;
            // 
            // rbBLE
            // 
            rbBLE.AutoSize = true;
            rbBLE.Location = new Point(107, 32);
            rbBLE.Name = "rbBLE";
            rbBLE.Size = new Size(114, 24);
            rbBLE.TabIndex = 5;
            rbBLE.TabStop = true;
            rbBLE.Text = "Bluetooth LE";
            rbBLE.UseVisualStyleBackColor = true;
            // 
            // CompanionAppScreen
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(475, 119);
            Controls.Add(rbBLE);
            Controls.Add(rbWiFi);
            Controls.Add(buttonStart);
            Controls.Add(labelIpAddress);
            Name = "CompanionAppScreen";
            Text = "CompanionAppScreen";
            FormClosed += CompanionAppScreen_FormClosed;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelIpAddress;
        private UI.RButton buttonStart;
        private RadioButton rbWiFi;
        private RadioButton rbBLE;
    }
}