using GHelper.UI;

namespace GHelper
{
    partial class Updates
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
            tableBios = new TableLayoutPanel();
            labelBIOS = new Label();
            pictureBios = new PictureBox();
            panelBiosTitle = new Panel();
            labelUpdates = new Label();
            buttonRefresh = new RButton();
            panelBios = new Panel();
            panelDrivers = new Panel();
            tableDrivers = new TableLayoutPanel();
            panelDriversTitle = new Panel();
            labelDrivers = new Label();
            pictureDrivers = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBios).BeginInit();
            panelBiosTitle.SuspendLayout();
            panelBios.SuspendLayout();
            panelDrivers.SuspendLayout();
            panelDriversTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureDrivers).BeginInit();
            SuspendLayout();
            // 
            // tableBios
            // 
            tableBios.AutoSize = true;
            tableBios.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableBios.ColumnCount = 4;
            tableBios.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 23F));
            tableBios.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableBios.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
            tableBios.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22F));
            tableBios.Dock = DockStyle.Top;
            tableBios.Location = new Point(20, 20);
            tableBios.Margin = new Padding(4);
            tableBios.MinimumSize = new Size(1300, 0);
            tableBios.Name = "tableBios";
            tableBios.Size = new Size(1300, 0);
            tableBios.TabIndex = 0;
            // 
            // labelBIOS
            // 
            labelBIOS.AutoSize = true;
            labelBIOS.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelBIOS.Location = new Point(68, 23);
            labelBIOS.Margin = new Padding(4, 0, 4, 0);
            labelBIOS.Name = "labelBIOS";
            labelBIOS.Size = new Size(68, 32);
            labelBIOS.TabIndex = 1;
            labelBIOS.Text = "BIOS";
            // 
            // pictureBios
            // 
            pictureBios.BackgroundImage = Properties.Resources.icons8_processor_32;
            pictureBios.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBios.Location = new Point(28, 23);
            pictureBios.Margin = new Padding(4);
            pictureBios.Name = "pictureBios";
            pictureBios.Size = new Size(32, 32);
            pictureBios.TabIndex = 2;
            pictureBios.TabStop = false;
            // 
            // panelBiosTitle
            // 
            panelBiosTitle.Controls.Add(labelUpdates);
            panelBiosTitle.Controls.Add(buttonRefresh);
            panelBiosTitle.Controls.Add(labelBIOS);
            panelBiosTitle.Controls.Add(pictureBios);
            panelBiosTitle.Dock = DockStyle.Top;
            panelBiosTitle.Location = new Point(0, 0);
            panelBiosTitle.Margin = new Padding(4);
            panelBiosTitle.Name = "panelBiosTitle";
            panelBiosTitle.Size = new Size(1294, 62);
            panelBiosTitle.TabIndex = 3;
            // 
            // labelUpdates
            // 
            labelUpdates.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelUpdates.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelUpdates.Location = new Point(848, 23);
            labelUpdates.Name = "labelUpdates";
            labelUpdates.Size = new Size(245, 32);
            labelUpdates.TabIndex = 4;
            labelUpdates.Text = "Updates Available";
            // 
            // buttonRefresh
            // 
            buttonRefresh.Activated = false;
            buttonRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonRefresh.BackColor = SystemColors.ControlLight;
            buttonRefresh.BorderColor = Color.Transparent;
            buttonRefresh.BorderRadius = 5;
            buttonRefresh.FlatAppearance.BorderSize = 0;
            buttonRefresh.FlatStyle = FlatStyle.Flat;
            buttonRefresh.Image = Properties.Resources.icons8_refresh_32;
            buttonRefresh.Location = new Point(1221, 14);
            buttonRefresh.Name = "buttonRefresh";
            buttonRefresh.Secondary = true;
            buttonRefresh.Size = new Size(52, 46);
            buttonRefresh.TabIndex = 1;
            buttonRefresh.UseVisualStyleBackColor = false;
            // 
            // panelBios
            // 
            panelBios.AutoSize = true;
            panelBios.Controls.Add(tableBios);
            panelBios.Dock = DockStyle.Top;
            panelBios.Location = new Point(0, 62);
            panelBios.Margin = new Padding(4);
            panelBios.Name = "panelBios";
            panelBios.Padding = new Padding(20);
            panelBios.Size = new Size(1294, 40);
            panelBios.TabIndex = 4;
            // 
            // panelDrivers
            // 
            panelDrivers.AutoSize = true;
            panelDrivers.Controls.Add(tableDrivers);
            panelDrivers.Dock = DockStyle.Top;
            panelDrivers.Location = new Point(0, 146);
            panelDrivers.Margin = new Padding(4);
            panelDrivers.Name = "panelDrivers";
            panelDrivers.Padding = new Padding(20);
            panelDrivers.Size = new Size(1294, 40);
            panelDrivers.TabIndex = 6;
            // 
            // tableDrivers
            // 
            tableDrivers.AutoSize = true;
            tableDrivers.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableDrivers.ColumnCount = 4;
            tableDrivers.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 23F));
            tableDrivers.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableDrivers.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
            tableDrivers.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22F));
            tableDrivers.Dock = DockStyle.Top;
            tableDrivers.Location = new Point(20, 20);
            tableDrivers.Margin = new Padding(4);
            tableDrivers.MinimumSize = new Size(1300, 0);
            tableDrivers.Name = "tableDrivers";
            tableDrivers.Size = new Size(1300, 0);
            tableDrivers.TabIndex = 0;
            // 
            // panelDriversTitle
            // 
            panelDriversTitle.Controls.Add(labelDrivers);
            panelDriversTitle.Controls.Add(pictureDrivers);
            panelDriversTitle.Dock = DockStyle.Top;
            panelDriversTitle.Location = new Point(0, 102);
            panelDriversTitle.Margin = new Padding(4);
            panelDriversTitle.Name = "panelDriversTitle";
            panelDriversTitle.Size = new Size(1294, 44);
            panelDriversTitle.TabIndex = 5;
            // 
            // labelDrivers
            // 
            labelDrivers.AutoSize = true;
            labelDrivers.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelDrivers.Location = new Point(68, 6);
            labelDrivers.Margin = new Padding(4, 0, 4, 0);
            labelDrivers.Name = "labelDrivers";
            labelDrivers.Size = new Size(254, 32);
            labelDrivers.TabIndex = 1;
            labelDrivers.Text = "Drivers and Software";
            // 
            // pictureDrivers
            // 
            pictureDrivers.BackgroundImage = Properties.Resources.icons8_software_32;
            pictureDrivers.BackgroundImageLayout = ImageLayout.Zoom;
            pictureDrivers.Location = new Point(28, 6);
            pictureDrivers.Margin = new Padding(4);
            pictureDrivers.Name = "pictureDrivers";
            pictureDrivers.Size = new Size(32, 32);
            pictureDrivers.TabIndex = 2;
            pictureDrivers.TabStop = false;
            // 
            // Updates
            // 
            AutoScaleDimensions = new SizeF(192F, 192F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScroll = true;
            ClientSize = new Size(1294, 690);
            Controls.Add(panelDrivers);
            Controls.Add(panelDriversTitle);
            Controls.Add(panelBios);
            Controls.Add(panelBiosTitle);
            Margin = new Padding(4);
            MinimizeBox = false;
            Name = "Updates";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "BIOS and Driver Updates";
            ((System.ComponentModel.ISupportInitialize)pictureBios).EndInit();
            panelBiosTitle.ResumeLayout(false);
            panelBiosTitle.PerformLayout();
            panelBios.ResumeLayout(false);
            panelBios.PerformLayout();
            panelDrivers.ResumeLayout(false);
            panelDrivers.PerformLayout();
            panelDriversTitle.ResumeLayout(false);
            panelDriversTitle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureDrivers).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel tableBios;
        private Label labelBIOS;
        private PictureBox pictureBios;
        private Panel panelBiosTitle;
        private Panel panelBios;
        private Panel panelDrivers;
        private TableLayoutPanel tableDrivers;
        private Panel panelDriversTitle;
        private Label labelDrivers;
        private PictureBox pictureDrivers;
        private RButton buttonRefresh;
        private Label labelUpdates;
    }
}