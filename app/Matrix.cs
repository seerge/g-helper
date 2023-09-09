﻿using GHelper.AnimeMatrix;
using GHelper.UI;

namespace GHelper
{
    public partial class Matrix : RForm
    {

        public AniMatrixControl matrixControl = Program.settingsForm.matrixControl;

        private bool Dragging;
        private int xPos;
        private int yPos;

        private int baseX;
        private int baseY;

        private float uiScale;

        Image picture;
        MemoryStream ms = new MemoryStream();

        public Matrix()
        {
            InitializeComponent();
            InitTheme(true);

            Shown += Matrix_Shown;
            FormClosing += Matrix_FormClosed;

            buttonPicture.Click += ButtonPicture_Click;
            buttonReset.Click += ButtonReset_Click;

            pictureMatrix.MouseUp += PictureMatrix_MouseUp;
            pictureMatrix.MouseMove += PictureMatrix_MouseMove;
            pictureMatrix.MouseDown += PictureMatrix_MouseDown;

            trackZoom.MouseUp += TrackZoom_MouseUp;
            trackZoom.ValueChanged += TrackZoom_Changed;

            trackZoom.Value = Math.Min(trackZoom.Maximum, AppConfig.Get("matrix_zoom", 100));
            VisualiseZoom();

            comboScaling.DropDownStyle = ComboBoxStyle.DropDownList;
            comboScaling.SelectedIndex = AppConfig.Get("matrix_quality", 0);
            comboScaling.SelectedValueChanged += ComboScaling_SelectedValueChanged;

            uiScale = panelPicture.Width / matrixControl.device.MaxColumns / 3;
            panelPicture.Height = (int)(matrixControl.device.MaxRows * uiScale);

        }

        private void ComboScaling_SelectedValueChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("matrix_quality", comboScaling.SelectedIndex);
            SetMatrixPicture(false);
        }

        private void Matrix_FormClosed(object? sender, FormClosingEventArgs e)
        {
            if (picture is not null) picture.Dispose();
            if (ms is not null) ms.Dispose();

            pictureMatrix.Dispose();

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
        }

        private void VisualiseZoom()
        {
            labelZoom.Text = trackZoom.Value + "%";
        }

        private void ButtonReset_Click(object? sender, EventArgs e)
        {
            AppConfig.Set("matrix_zoom", 100);
            AppConfig.Set("matrix_x", 0);
            AppConfig.Set("matrix_y", 0);

            trackZoom.Value = 100;

            SetMatrixPicture();

        }

        private void TrackZoom_MouseUp(object? sender, EventArgs e)
        {
            AppConfig.Set("matrix_zoom", trackZoom.Value);
            SetMatrixPicture();
        }

        private void TrackZoom_Changed(object? sender, EventArgs e)
        {
            VisualiseZoom();
        }


        private void PictureMatrix_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Dragging = true;
                xPos = e.X;
                yPos = e.Y;
            }
        }

        private void PictureMatrix_MouseMove(object? sender, MouseEventArgs e)
        {
            Control c = sender as Control;
            if (Dragging && c != null)
            {
                c.Top = e.Y + c.Top - yPos;
                c.Left = e.X + c.Left - xPos;
            }
        }

        private void PictureMatrix_MouseUp(object? sender, MouseEventArgs e)
        {

            Dragging = false;

            Control c = sender as Control;

            int matrixX = (int)((baseX - c.Left) / uiScale);
            int matrixY = (int)((baseY - c.Top) / uiScale);

            AppConfig.Set("matrix_x", matrixX);
            AppConfig.Set("matrix_y", matrixY);

            SetMatrixPicture(false);
        }

        private void Matrix_Shown(object? sender, EventArgs e)
        {
            FormPosition();
            SetMatrixPicture();
        }

        private void SetMatrixPicture(bool visualise = true)
        {
            matrixControl.SetMatrixPicture(AppConfig.GetString("matrix_picture"), visualise);
        }

        private void ButtonPicture_Click(object? sender, EventArgs e)
        {
            matrixControl.OpenMatrixPicture();

        }
        public void FormPosition()
        {
            if (Height > Program.settingsForm.Height)
            {
                Top = Program.settingsForm.Top + Program.settingsForm.Height - Height;
            }
            else
            {
                Height = Program.settingsForm.Height;
                Top = Program.settingsForm.Top;
            }

            Left = Program.settingsForm.Left - Width - 5;
        }

        public void VisualiseMatrix(string fileName)
        {

            if (picture is not null) picture.Dispose();

            using (var fs = new FileStream(fileName, FileMode.Open))
            {

                ms.SetLength(0);
                fs.CopyTo(ms);
                ms.Position = 0;
                fs.Close();

                picture = Image.FromStream(ms);

                int width = picture.Width;
                int height = picture.Height;

                int matrixX = AppConfig.Get("matrix_x", 0);
                int matrixY = AppConfig.Get("matrix_y", 0);
                int matrixZoom = AppConfig.Get("matrix_zoom", 100);


                float scale = Math.Min((float)panelPicture.Width / (float)width, (float)panelPicture.Height / (float)height) * matrixZoom / 100;

                pictureMatrix.Width = (int)(width * scale);
                pictureMatrix.Height = (int)(height * scale);

                baseX = panelPicture.Width - pictureMatrix.Width;
                baseY = 0;

                pictureMatrix.Left = baseX - (int)(matrixX * uiScale);
                pictureMatrix.Top = baseY - (int)(matrixY * uiScale);

                pictureMatrix.SizeMode = PictureBoxSizeMode.Zoom;
                pictureMatrix.Image = picture;


            }




        }

    }
}
