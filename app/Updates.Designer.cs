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
            components = new System.ComponentModel.Container();
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
            tableLayoutLegend = new TableLayoutPanel();
            labelLegendGreen = new Label();
            labelLegendGray = new Label();
            labelLegendRed = new Label();
            labelLegend = new Label();
            toolTip = new ToolTip(components);
            ((System.ComponentModel.ISupportInitialize)pictureBios).BeginInit();
            panelBiosTitle.SuspendLayout();
            panelBios.SuspendLayout();
            panelDrivers.SuspendLayout();
            panelDriversTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureDrivers).BeginInit();
            tableLayoutLegend.SuspendLayout();
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
            labelBIOS.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
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
            panelBiosTitle.Size = new Size(1236, 60);
            panelBiosTitle.TabIndex = 3;
            // 
            // labelUpdates
            // 
            labelUpdates.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelUpdates.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelUpdates.Location = new Point(864, 19);
            labelUpdates.Name = "labelUpdates";
            labelUpdates.Size = new Size(302, 32);
            labelUpdates.TabIndex = 4;
            labelUpdates.Text = "Updates Available";
            // 
            // buttonRefresh
            // 
            buttonRefresh.Activated = false;
            buttonRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonRefresh.BackColor = SystemColors.ControlLight;
            buttonRefresh.Badge = false;
            buttonRefresh.BorderColor = Color.Transparent;
            buttonRefresh.BorderRadius = 5;
            buttonRefresh.FlatAppearance.BorderSize = 0;
            buttonRefresh.FlatStyle = FlatStyle.Flat;
            buttonRefresh.Image = Properties.Resources.icons8_refresh_32;
            buttonRefresh.Location = new Point(1172, 11);
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
            panelBios.Location = new Point(0, 60);
            panelBios.Margin = new Padding(4);
            panelBios.Name = "panelBios";
            panelBios.Padding = new Padding(20);
            panelBios.Size = new Size(1236, 40);
            panelBios.TabIndex = 4;
            // 
            // panelDrivers
            // 
            panelDrivers.AutoSize = true;
            panelDrivers.Controls.Add(tableDrivers);
            panelDrivers.Dock = DockStyle.Top;
            panelDrivers.Location = new Point(0, 144);
            panelDrivers.Margin = new Padding(4);
            panelDrivers.Name = "panelDrivers";
            panelDrivers.Padding = new Padding(20);
            panelDrivers.Size = new Size(1236, 40);
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
            panelDriversTitle.Location = new Point(0, 100);
            panelDriversTitle.Margin = new Padding(4);
            panelDriversTitle.Name = "panelDriversTitle";
            panelDriversTitle.Size = new Size(1236, 44);
            panelDriversTitle.TabIndex = 5;
            // 
            // labelDrivers
            // 
            labelDrivers.AutoSize = true;
            labelDrivers.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
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
            // tableLayoutLegend
            // 
            tableLayoutLegend.AutoSize = true;
            tableLayoutLegend.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutLegend.ColumnCount = 4;
            tableLayoutLegend.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15.151515F));
            tableLayoutLegend.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28.2828274F));
            tableLayoutLegend.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28.2828274F));
            tableLayoutLegend.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28.2828274F));
            tableLayoutLegend.Controls.Add(labelLegendGreen, 0, 0);
            tableLayoutLegend.Controls.Add(labelLegendGray, 0, 0);
            tableLayoutLegend.Controls.Add(labelLegendRed, 1, 0);
            tableLayoutLegend.Controls.Add(labelLegend, 0, 0);
            tableLayoutLegend.Dock = DockStyle.Bottom;
            tableLayoutLegend.Location = new Point(0, 608);
            tableLayoutLegend.Margin = new Padding(0);
            tableLayoutLegend.Name = "tableLayoutLegend";
            tableLayoutLegend.Padding = new Padding(10, 0, 10, 20);
            tableLayoutLegend.RowCount = 1;
            tableLayoutLegend.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutLegend.Size = new Size(1236, 82);
            tableLayoutLegend.TabIndex = 7;
            // 
            // labelLegendGreen
            // 
            labelLegendGreen.AutoSize = true;
            labelLegendGreen.BackColor = Color.Green;
            labelLegendGreen.Dock = DockStyle.Top;
            labelLegendGreen.ForeColor = Color.White;
            labelLegendGreen.Location = new Point(547, 10);
            labelLegendGreen.Margin = new Padding(10);
            labelLegendGreen.Name = "labelLegendGreen";
            labelLegendGreen.Padding = new Padding(5);
            labelLegendGreen.Size = new Size(323, 42);
            labelLegendGreen.TabIndex = 4;
            labelLegendGreen.Text = "Updated";
            // 
            // labelLegendGray
            // 
            labelLegendGray.AutoSize = true;
            labelLegendGray.BackColor = Color.Gray;
            labelLegendGray.Dock = DockStyle.Top;
            labelLegendGray.ForeColor = Color.White;
            labelLegendGray.Location = new Point(204, 10);
            labelLegendGray.Margin = new Padding(10);
            labelLegendGray.Name = "labelLegendGray";
            labelLegendGray.Padding = new Padding(5);
            labelLegendGray.Size = new Size(323, 42);
            labelLegendGray.TabIndex = 3;
            labelLegendGray.Text = "Can't check local version";
            // 
            // labelLegendRed
            // 
            labelLegendRed.AutoSize = true;
            labelLegendRed.BackColor = Color.Red;
            labelLegendRed.Dock = DockStyle.Top;
            labelLegendRed.ForeColor = Color.White;
            labelLegendRed.Location = new Point(890, 10);
            labelLegendRed.Margin = new Padding(10);
            labelLegendRed.Name = "labelLegendRed";
            labelLegendRed.Padding = new Padding(5);
            labelLegendRed.Size = new Size(326, 42);
            labelLegendRed.TabIndex = 1;
            labelLegendRed.Text = "Update Available";
            // 
            // labelLegend
            // 
            labelLegend.AutoSize = true;
            labelLegend.Dock = DockStyle.Top;
            labelLegend.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelLegend.Location = new Point(20, 10);
            labelLegend.Margin = new Padding(10);
            labelLegend.Name = "labelLegend";
            labelLegend.Padding = new Padding(5);
            labelLegend.Size = new Size(164, 42);
            labelLegend.TabIndex = 0;
            labelLegend.Text = "Legend";
            // 
            // Updates
            // 
            AutoScaleDimensions = new SizeF(192F, 192F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScroll = true;
            ClientSize = new Size(1236, 690);
            Controls.Add(tableLayoutLegend);
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
            tableLayoutLegend.ResumeLayout(false);
            tableLayoutLegend.PerformLayout();
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
        private TableLayoutPanel tableLayoutLegend;
        private Label labelLegend;
        private Label labelLegendRed;
        private Label labelLegendGray;
        private Label labelLegendGreen;
        private ToolTip toolTip;
    }
}