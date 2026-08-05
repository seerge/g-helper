namespace GHelper
{
    partial class ProxySettingsForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            labelProxyType = new Label();
            comboProxyType = new UI.RComboBox();
            labelHost = new Label();
            textHost = new UI.RTextBox();
            labelPort = new Label();
            numericPort = new UI.RNumericUpDown();
            labelUsername = new Label();
            textUsername = new UI.RTextBox();
            labelPassword = new Label();
            textPassword = new UI.RTextBox();
            buttonSave = new UI.RButton();
            buttonCancel = new UI.RButton();
            tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // labelProxyType
            // 
            labelProxyType.AutoSize = true;
            labelProxyType.Dock = DockStyle.Fill;
            labelProxyType.Location = new Point(3, 0);
            labelProxyType.Name = "labelProxyType";
            labelProxyType.Size = new Size(64, 30);
            labelProxyType.TabIndex = 0;
            labelProxyType.Text = "代理类型";
            labelProxyType.TextAlign = ContentAlignment.MiddleRight;
            // 
            // comboProxyType
            // 
            comboProxyType.Dock = DockStyle.Fill;
            comboProxyType.DropDownStyle = ComboBoxStyle.DropDownList;
            comboProxyType.FormattingEnabled = true;
            comboProxyType.Items.AddRange(new object[] { "不使用代理", "HTTP 代理", "SOCKS5 代理" });
            comboProxyType.Location = new Point(73, 3);
            comboProxyType.Name = "comboProxyType";
            comboProxyType.Size = new Size(214, 23);
            comboProxyType.TabIndex = 1;
            comboProxyType.SelectedIndexChanged += comboProxyType_SelectedIndexChanged;
            // 
            // labelHost
            // 
            labelHost.AutoSize = true;
            labelHost.Dock = DockStyle.Fill;
            labelHost.Location = new Point(3, 35);
            labelHost.Name = "labelHost";
            labelHost.Size = new Size(64, 28);
            labelHost.TabIndex = 2;
            labelHost.Text = "代理地址";
            labelHost.TextAlign = ContentAlignment.MiddleRight;
            // 
            // textHost
            // 
            textHost.Dock = DockStyle.Fill;
            textHost.Location = new Point(73, 38);
            textHost.Name = "textHost";
            textHost.PlaceholderText = "例如: 127.0.0.1";
            textHost.Size = new Size(133, 23);
            textHost.TabIndex = 3;
            // 
            // labelPort
            // 
            labelPort.AutoSize = true;
            labelPort.Dock = DockStyle.Fill;
            labelPort.Location = new Point(212, 35);
            labelPort.Name = "labelPort";
            labelPort.Size = new Size(37, 28);
            labelPort.TabIndex = 4;
            labelPort.Text = "端口";
            labelPort.TextAlign = ContentAlignment.MiddleRight;
            // 
            // numericPort
            // 
            numericPort.Dock = DockStyle.Fill;
            numericPort.Location = new Point(255, 38);
            numericPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            numericPort.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericPort.Name = "numericPort";
            numericPort.Size = new Size(32, 23);
            numericPort.TabIndex = 5;
            numericPort.Value = new decimal(new int[] { 7890, 0, 0, 0 });
            // 
            // labelUsername
            // 
            labelUsername.AutoSize = true;
            labelUsername.Dock = DockStyle.Fill;
            labelUsername.Location = new Point(3, 68);
            labelUsername.Name = "labelUsername";
            labelUsername.Size = new Size(64, 28);
            labelUsername.TabIndex = 6;
            labelUsername.Text = "用户名";
            labelUsername.TextAlign = ContentAlignment.MiddleRight;
            // 
            // textUsername
            // 
            textUsername.Dock = DockStyle.Fill;
            textUsername.Location = new Point(73, 71);
            textUsername.Name = "textUsername";
            textUsername.PlaceholderText = "可选";
            textUsername.Size = new Size(133, 23);
            textUsername.TabIndex = 7;
            // 
            // labelPassword
            // 
            labelPassword.AutoSize = true;
            labelPassword.Dock = DockStyle.Fill;
            labelPassword.Location = new Point(212, 68);
            labelPassword.Name = "labelPassword";
            labelPassword.Size = new Size(37, 28);
            labelPassword.TabIndex = 8;
            labelPassword.Text = "密码";
            labelPassword.TextAlign = ContentAlignment.MiddleRight;
            // 
            // textPassword
            // 
            textPassword.Dock = DockStyle.Fill;
            textPassword.Location = new Point(255, 71);
            textPassword.Name = "textPassword";
            textPassword.PlaceholderText = "可选";
            textPassword.Size = new Size(32, 23);
            textPassword.TabIndex = 9;
            textPassword.UseSystemPasswordChar = true;
            // 
            // buttonSave
            // 
            buttonSave.Activated = false;
            buttonSave.BackColor = SystemColors.ControlLight;
            buttonSave.BorderColor = Color.Transparent;
            buttonSave.BorderRadius = 2;
            buttonSave.Dock = DockStyle.Top;
            buttonSave.FlatStyle = FlatStyle.Flat;
            buttonSave.Location = new Point(4, 1);
            buttonSave.Margin = new Padding(4, 1, 4, 1);
            buttonSave.Name = "buttonSave";
            buttonSave.Secondary = false;
            buttonSave.Size = new Size(82, 30);
            buttonSave.TabIndex = 10;
            buttonSave.Text = "保存";
            buttonSave.UseVisualStyleBackColor = false;
            buttonSave.Click += buttonSave_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Activated = false;
            buttonCancel.BackColor = SystemColors.ControlLight;
            buttonCancel.BorderColor = Color.Transparent;
            buttonCancel.BorderRadius = 2;
            buttonCancel.Dock = DockStyle.Top;
            buttonCancel.FlatStyle = FlatStyle.Flat;
            buttonCancel.Location = new Point(4, 1);
            buttonCancel.Margin = new Padding(4, 1, 4, 1);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Secondary = true;
            buttonCancel.Size = new Size(58, 30);
            buttonCancel.TabIndex = 11;
            buttonCancel.Text = "取消";
            buttonCancel.UseVisualStyleBackColor = false;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 4;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel.Controls.Add(labelProxyType, 0, 0);
            tableLayoutPanel.Controls.Add(comboProxyType, 1, 0);
            tableLayoutPanel.Controls.Add(labelHost, 0, 1);
            tableLayoutPanel.Controls.Add(textHost, 1, 1);
            tableLayoutPanel.Controls.Add(labelPort, 2, 1);
            tableLayoutPanel.Controls.Add(numericPort, 3, 1);
            tableLayoutPanel.Controls.Add(labelUsername, 0, 2);
            tableLayoutPanel.Controls.Add(textUsername, 1, 2);
            tableLayoutPanel.Controls.Add(labelPassword, 2, 2);
            tableLayoutPanel.Controls.Add(textPassword, 3, 2);
            tableLayoutPanel.Controls.Add(buttonSave, 1, 3);
            tableLayoutPanel.Controls.Add(buttonCancel, 3, 3);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(12, 12);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.Padding = new Padding(0, 0, 0, 8);
            tableLayoutPanel.RowCount = 4;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableLayoutPanel.Size = new Size(290, 136);
            tableLayoutPanel.TabIndex = 12;
            // 
            // ProxySettingsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(314, 145);
            Controls.Add(tableLayoutPanel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ProxySettingsForm";
            StartPosition = FormStartPosition.Manual;
            Text = "代理设置";
            Load += ProxySettingsForm_Load;
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label labelProxyType;
        private UI.RComboBox comboProxyType;
        private Label labelHost;
        private UI.RTextBox textHost;
        private Label labelPort;
        private UI.RNumericUpDown numericPort;
        private Label labelUsername;
        private UI.RTextBox textUsername;
        private Label labelPassword;
        private UI.RTextBox textPassword;
        private UI.RButton buttonSave;
        private UI.RButton buttonCancel;
        private TableLayoutPanel tableLayoutPanel;
    }
}