namespace GHelper
{
    partial class Matrix
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
            pictureMatrix = new PictureBox();
            trackZoom = new TrackBar();
            buttonPicture = new UI.RButton();
            panelPicture = new Panel();
            panelMain = new Panel();
            panelButtons = new Panel();
            buttonReset = new UI.RButton();
            panelContrast = new Panel();
            labelContrast = new Label();
            labelContrastTitle = new Label();
            trackContrast = new TrackBar();
            panelRotation = new Panel();
            comboRotation = new UI.RComboBox();
            labelRotation = new Label();
            panelScaling = new Panel();
            comboScaling = new UI.RComboBox();
            labelScaling = new Label();
            panelZoom = new Panel();
            labelZoom = new Label();
            labelZoomTitle = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureMatrix).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackZoom).BeginInit();
            panelPicture.SuspendLayout();
            panelMain.SuspendLayout();
            panelButtons.SuspendLayout();
            panelContrast.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackContrast).BeginInit();
            panelRotation.SuspendLayout();
            panelScaling.SuspendLayout();
            panelZoom.SuspendLayout();
            SuspendLayout();
            // 
            // pictureMatrix
            // 
            pictureMatrix.BackColor = Color.Black;
            pictureMatrix.Cursor = Cursors.SizeAll;
            pictureMatrix.Location = new Point(731, 27);
            pictureMatrix.Name = "pictureMatrix";
            pictureMatrix.Size = new Size(81, 73);
            pictureMatrix.TabIndex = 0;
            pictureMatrix.TabStop = false;
            // 
            // trackZoom
            // 
            trackZoom.LargeChange = 50;
            trackZoom.Location = new Point(16, 52);
            trackZoom.Maximum = 200;
            trackZoom.Minimum = 10;
            trackZoom.Name = "trackZoom";
            trackZoom.Size = new Size(782, 90);
            trackZoom.SmallChange = 10;
            trackZoom.TabIndex = 2;
            trackZoom.TickFrequency = 20;
            trackZoom.TickStyle = TickStyle.TopLeft;
            trackZoom.Value = 100;
            // 
            // buttonPicture
            // 
            buttonPicture.Activated = false;
            buttonPicture.BackColor = SystemColors.ControlLight;
            buttonPicture.BorderColor = Color.Transparent;
            buttonPicture.BorderRadius = 5;
            buttonPicture.FlatAppearance.BorderSize = 0;
            buttonPicture.FlatStyle = FlatStyle.Flat;
            buttonPicture.Image = Properties.Resources.icons8_matrix_32;
            buttonPicture.Location = new Point(16, 19);
            buttonPicture.Name = "buttonPicture";
            buttonPicture.Secondary = true;
            buttonPicture.Size = new Size(258, 56);
            buttonPicture.TabIndex = 3;
            buttonPicture.Text = "Picture / Gif";
            buttonPicture.TextAlign = ContentAlignment.MiddleRight;
            buttonPicture.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonPicture.UseVisualStyleBackColor = false;
            // 
            // panelPicture
            // 
            panelPicture.BackColor = Color.Black;
            panelPicture.Controls.Add(pictureMatrix);
            panelPicture.Dock = DockStyle.Top;
            panelPicture.Location = new Point(0, 0);
            panelPicture.Name = "panelPicture";
            panelPicture.Size = new Size(834, 419);
            panelPicture.TabIndex = 4;
            // 
            // panelMain
            // 
            panelMain.AutoSize = true;
            panelMain.Controls.Add(panelButtons);
            panelMain.Controls.Add(panelContrast);
            panelMain.Controls.Add(panelRotation);
            panelMain.Controls.Add(panelScaling);
            panelMain.Controls.Add(panelZoom);
            panelMain.Controls.Add(panelPicture);
            panelMain.Dock = DockStyle.Top;
            panelMain.Location = new Point(20, 20);
            panelMain.Name = "panelMain";
            panelMain.Size = new Size(834, 959);
            panelMain.TabIndex = 5;
            // 
            // panelButtons
            // 
            panelButtons.Controls.Add(buttonReset);
            panelButtons.Controls.Add(buttonPicture);
            panelButtons.Dock = DockStyle.Top;
            panelButtons.Location = new Point(0, 865);
            panelButtons.Name = "panelButtons";
            panelButtons.Size = new Size(834, 94);
            panelButtons.TabIndex = 6;
            // 
            // buttonReset
            // 
            buttonReset.Activated = false;
            buttonReset.BackColor = SystemColors.ControlLight;
            buttonReset.BorderColor = Color.Transparent;
            buttonReset.BorderRadius = 5;
            buttonReset.FlatAppearance.BorderSize = 0;
            buttonReset.FlatStyle = FlatStyle.Flat;
            buttonReset.Image = Properties.Resources.icons8_refresh_32;
            buttonReset.Location = new Point(290, 19);
            buttonReset.Name = "buttonReset";
            buttonReset.Secondary = true;
            buttonReset.Size = new Size(258, 56);
            buttonReset.TabIndex = 4;
            buttonReset.Text = "Reset";
            buttonReset.TextAlign = ContentAlignment.MiddleRight;
            buttonReset.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonReset.UseVisualStyleBackColor = false;
            // 
            // panelContrast
            // 
            panelContrast.AutoSize = true;
            panelContrast.Controls.Add(labelContrast);
            panelContrast.Controls.Add(labelContrastTitle);
            panelContrast.Controls.Add(trackContrast);
            panelContrast.Dock = DockStyle.Top;
            panelContrast.Location = new Point(0, 720);
            panelContrast.Name = "panelContrast";
            panelContrast.Size = new Size(834, 145);
            panelContrast.TabIndex = 6;
            // 
            // labelContrast
            // 
            labelContrast.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelContrast.AutoSize = true;
            labelContrast.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            labelContrast.Location = new Point(705, 17);
            labelContrast.Name = "labelContrast";
            labelContrast.Size = new Size(103, 32);
            labelContrast.TabIndex = 4;
            labelContrast.Text = "Contrast";
            // 
            // labelContrastTitle
            // 
            labelContrastTitle.AutoSize = true;
            labelContrastTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelContrastTitle.Location = new Point(16, 17);
            labelContrastTitle.Name = "labelContrastTitle";
            labelContrastTitle.Size = new Size(111, 32);
            labelContrastTitle.TabIndex = 3;
            labelContrastTitle.Text = "Contrast";
            // 
            // trackContrast
            // 
            trackContrast.LargeChange = 50;
            trackContrast.Location = new Point(16, 52);
            trackContrast.Maximum = 200;
            trackContrast.Minimum = 10;
            trackContrast.Name = "trackContrast";
            trackContrast.Size = new Size(782, 90);
            trackContrast.SmallChange = 10;
            trackContrast.TabIndex = 2;
            trackContrast.TickFrequency = 20;
            trackContrast.TickStyle = TickStyle.TopLeft;
            trackContrast.Value = 100;
            // 
            // panelRotation
            // 
            panelRotation.Controls.Add(comboRotation);
            panelRotation.Controls.Add(labelRotation);
            panelRotation.Dock = DockStyle.Top;
            panelRotation.Location = new Point(0, 642);
            panelRotation.Name = "panelRotation";
            panelRotation.Size = new Size(834, 78);
            panelRotation.TabIndex = 8;
            // 
            // comboRotation
            // 
            comboRotation.BorderColor = Color.White;
            comboRotation.ButtonColor = Color.FromArgb(255, 255, 255);
            comboRotation.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            comboRotation.FormattingEnabled = true;
            comboRotation.ItemHeight = 32;
            comboRotation.Items.AddRange(new object[] { "Straight", "Diagonal" });
            comboRotation.Location = new Point(279, 17);
            comboRotation.Margin = new Padding(4, 11, 4, 8);
            comboRotation.Name = "comboRotation";
            comboRotation.Size = new Size(322, 40);
            comboRotation.TabIndex = 17;
            // 
            // labelRotation
            // 
            labelRotation.AutoSize = true;
            labelRotation.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelRotation.Location = new Point(16, 20);
            labelRotation.Name = "labelRotation";
            labelRotation.Size = new Size(190, 32);
            labelRotation.TabIndex = 4;
            labelRotation.Text = "Image Rotation";
            // 
            // panelScaling
            // 
            panelScaling.Controls.Add(comboScaling);
            panelScaling.Controls.Add(labelScaling);
            panelScaling.Dock = DockStyle.Top;
            panelScaling.Location = new Point(0, 564);
            panelScaling.Name = "panelScaling";
            panelScaling.Size = new Size(834, 78);
            panelScaling.TabIndex = 7;
            // 
            // comboScaling
            // 
            comboScaling.BorderColor = Color.White;
            comboScaling.ButtonColor = Color.FromArgb(255, 255, 255);
            comboScaling.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            comboScaling.FormattingEnabled = true;
            comboScaling.ItemHeight = 32;
            comboScaling.Items.AddRange(new object[] { "Default", "Low", "High", "Bilinear", "Bicubic", "NearestNeighbor", "HighQualityBilinear", "HighQualityBicubic" });
            comboScaling.Location = new Point(279, 17);
            comboScaling.Margin = new Padding(4, 11, 4, 8);
            comboScaling.Name = "comboScaling";
            comboScaling.Size = new Size(322, 40);
            comboScaling.TabIndex = 17;
            // 
            // labelScaling
            // 
            labelScaling.AutoSize = true;
            labelScaling.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelScaling.Location = new Point(16, 20);
            labelScaling.Name = "labelScaling";
            labelScaling.Size = new Size(185, 32);
            labelScaling.TabIndex = 4;
            labelScaling.Text = "Scaling Quality";
            // 
            // panelZoom
            // 
            panelZoom.AutoSize = true;
            panelZoom.Controls.Add(labelZoom);
            panelZoom.Controls.Add(labelZoomTitle);
            panelZoom.Controls.Add(trackZoom);
            panelZoom.Dock = DockStyle.Top;
            panelZoom.Location = new Point(0, 419);
            panelZoom.Name = "panelZoom";
            panelZoom.Size = new Size(834, 145);
            panelZoom.TabIndex = 5;
            // 
            // labelZoom
            // 
            labelZoom.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelZoom.AutoSize = true;
            labelZoom.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            labelZoom.Location = new Point(731, 17);
            labelZoom.Name = "labelZoom";
            labelZoom.Size = new Size(77, 32);
            labelZoom.TabIndex = 4;
            labelZoom.Text = "Zoom";
            // 
            // labelZoomTitle
            // 
            labelZoomTitle.AutoSize = true;
            labelZoomTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelZoomTitle.Location = new Point(16, 17);
            labelZoomTitle.Name = "labelZoomTitle";
            labelZoomTitle.Size = new Size(81, 32);
            labelZoomTitle.TabIndex = 3;
            labelZoomTitle.Text = "Zoom";
            // 
            // Matrix
            // 
            AutoScaleDimensions = new SizeF(192F, 192F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new Size(874, 1006);
            Controls.Add(panelMain);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(900, 0);
            Name = "Matrix";
            Padding = new Padding(20);
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Matrix";
            ((System.ComponentModel.ISupportInitialize)pictureMatrix).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackZoom).EndInit();
            panelPicture.ResumeLayout(false);
            panelMain.ResumeLayout(false);
            panelMain.PerformLayout();
            panelButtons.ResumeLayout(false);
            panelContrast.ResumeLayout(false);
            panelContrast.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackContrast).EndInit();
            panelRotation.ResumeLayout(false);
            panelRotation.PerformLayout();
            panelScaling.ResumeLayout(false);
            panelScaling.PerformLayout();
            panelZoom.ResumeLayout(false);
            panelZoom.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureMatrix;
        private TrackBar trackZoom;
        private UI.RButton buttonPicture;
        private Panel panelPicture;
        private Panel panelMain;
        private Panel panelZoom;
        private Label labelZoom;
        private Label labelZoomTitle;
        private Panel panelButtons;
        private UI.RButton buttonReset;
        private Panel panelScaling;
        private Label labelScaling;
        private UI.RComboBox comboScaling;
        private Panel panelRotation;
        private UI.RComboBox comboRotation;
        private Label labelRotation;
        private Panel panelContrast;
        private Label labelContrast;
        private Label labelContrastTitle;
        private TrackBar trackContrast;
    }
}