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
            textM4 = new TextBox();
            textM3 = new TextBox();
            comboM4 = new ComboBox();
            labelM4 = new Label();
            comboM3 = new ComboBox();
            labelM3 = new Label();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(textM4);
            groupBox1.Controls.Add(textM3);
            groupBox1.Controls.Add(comboM4);
            groupBox1.Controls.Add(labelM4);
            groupBox1.Controls.Add(comboM3);
            groupBox1.Controls.Add(labelM3);
            groupBox1.Dock = DockStyle.Top;
            groupBox1.Location = new Point(10, 10);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(751, 196);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Key Bindings";
            // 
            // textM4
            // 
            textM4.Location = new Point(411, 113);
            textM4.Name = "textM4";
            textM4.PlaceholderText = "notepad /p \"file.txt\"";
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
            comboM4.Items.AddRange(new object[] { "Performance Mode", "Open G-Helper window", "Custom" });
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
            comboM3.Items.AddRange(new object[] { "Default", "Volume Mute", "Play / Pause", "PrintScreen", "Toggle Aura", "Custom" });
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
            // Keyboard
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(771, 858);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MdiChildrenMinimizedAnchorBottom = false;
            MinimizeBox = false;
            Name = "Keyboard";
            Padding = new Padding(10);
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Keyboard";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private Label labelM3;
        private ComboBox comboM3;
        private ComboBox comboM4;
        private Label labelM4;
        private TextBox textM4;
        private TextBox textM3;
    }
}