using GHelper.AnimeMatrix;
using GHelper.UI;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace GHelper
{
    public partial class Matrix : RForm
    {

        public AniMatrixControl matrixControl = Program.settingsForm.matrixControl;

        private bool dragging;
        private int xPos;
        private int yPos;
        private int dragX;
        private int dragY;

        private int baseX;
        private int baseY;
        private int dragLine;

        private int resetLeft;

        private float cellWidth;
        private float cellHeight;

        private bool previewPending;

        Image picture;
        string loadedPicture;
        bool animated;
        FrameDimension dimension;
        int frameCount;
        int dragFrame;
        MemoryStream ms = new MemoryStream();

        System.Windows.Forms.Timer textTimer;
        System.Windows.Forms.Timer clockTimer;
        System.Windows.Forms.Timer dragTimer = new System.Windows.Forms.Timer { Interval = 100 };

        public Matrix()
        {
            InitializeComponent();
            InitTheme(true);

            Text = Properties.Strings.AnimeMatrix;
            labelZoomTitle.Text = Properties.Strings.Zoom;
            labelScaling.Text = Properties.Strings.ScalingQuality;
            labelRotation.Text = Properties.Strings.ImageRotation;
            labelContrastTitle.Text = Properties.Strings.Contrast;
            labelGammaTitle.Text = Properties.Strings.Brightness;
            labelClockTime.Text = Properties.Strings.MatrixTimeFormat;
            labelClockDate.Text = Properties.Strings.MatrixDateFormat;
            labelAudioMode.Text = Properties.Strings.MatrixAudioMode;
            buttonPicture.Text = Properties.Strings.PictureGif;
            buttonReset.Text = Properties.Strings.Reset;
            checkTextRunning.Text = Properties.Strings.MatrixRunningText;
            checkClockBattery.Text = Properties.Strings.SlashBatteryLevel;
            checkAutoOff.Text = Properties.Strings.TurnOffOnBattery;
            checkLidOff.Text = Properties.Strings.DisableOnLidClose;

            buttonPictureMode.Text = Properties.Strings.MatrixPicture;
            buttonClockMode.Text = Properties.Strings.MatrixClock;
            buttonAudioMode.Text = Properties.Strings.MatrixAudio;
            buttonTextMode.Text = Properties.Strings.MatrixText;
            buttonPictureMode.BorderColor = buttonClockMode.BorderColor = buttonAudioMode.BorderColor = buttonTextMode.BorderColor = colorStandard;

            Shown += Matrix_Shown;
            FormClosing += Matrix_FormClosed;

            buttonPicture.Click += ButtonPicture_Click;
            buttonReset.Click += ButtonReset_Click;

            buttonPictureMode.Click += ButtonPictureMode_Click;
            buttonClockMode.Click += ButtonClockMode_Click;
            buttonAudioMode.Click += ButtonAudioMode_Click;
            buttonTextMode.Click += ButtonTextMode_Click;

            matrixControl.deviceMatrix.OnPresent = VisualisePicture;

            picturePreview.MouseDown += PicturePreview_MouseDown;
            picturePreview.MouseMove += PicturePreview_MouseMove;
            picturePreview.MouseUp += PicturePreview_MouseUp;

            trackZoom.MouseUp += TrackZoom_MouseUp;
            trackZoom.ValueChanged += TrackZoom_Changed;
            trackZoom.Value = Math.Min(trackZoom.Maximum, AppConfig.Get("matrix_zoom", 100));

            trackContrast.MouseUp += TrackMatrix_MouseUp;
            trackContrast.ValueChanged += TrackMatrix_ValueChanged;
            trackContrast.Value = Math.Min(trackContrast.Maximum, AppConfig.Get("matrix_contrast", 100));

            trackGamma.MouseUp += TrackMatrix_MouseUp;
            trackGamma.ValueChanged += TrackMatrix_ValueChanged;
            trackGamma.Value = Math.Min(trackGamma.Maximum, AppConfig.Get("matrix_gamma", 0));

            VisualiseMatrix();

            comboScaling.DropDownStyle = ComboBoxStyle.DropDownList;
            comboScaling.SelectedIndex = AppConfig.Get("matrix_quality", 0);
            comboScaling.SelectedValueChanged += ComboScaling_SelectedValueChanged;

            comboRotation.DropDownStyle = ComboBoxStyle.DropDownList;
            comboRotation.SelectedIndex = AppConfig.Get("matrix_rotation", 0);
            comboRotation.SelectedValueChanged += ComboRotation_SelectedValueChanged;

            comboAudioMode.DropDownStyle = ComboBoxStyle.DropDownList;
            comboAudioMode.SelectedIndex = Math.Clamp(AppConfig.Get("matrix_audio_mode", 0), 0, comboAudioMode.Items.Count - 1);
            comboAudioMode.SelectedValueChanged += ComboAudioMode_SelectedValueChanged;

            textMatrix.Text = AppConfig.GetString("matrix_text", "Hello!");
            textMatrix2.Text = AppConfig.GetString("matrix_text2", "");
            textMatrix.TextChanged += TextMatrix_TextChanged;
            textMatrix2.TextChanged += TextMatrix_TextChanged;

            textTimer = DebounceTimer(ApplyText);
            clockTimer = DebounceTimer(ApplyClock);
            dragTimer.Tick += DragTimer_Tick;

            InitFontCombo(comboTextFont, "matrix_text_font");
            InitFontCombo(comboTextFont2, "matrix_text2_font");

            numTextSize.Value = Math.Clamp(AppConfig.Get("matrix_text_size", 15), numTextSize.Minimum, numTextSize.Maximum);
            numTextSize2.Value = Math.Clamp(AppConfig.Get("matrix_text2_size", 15), numTextSize2.Minimum, numTextSize2.Maximum);
            numTextSize.ValueChanged += TextSettings_Changed;
            numTextSize2.ValueChanged += TextSettings_Changed;

            checkTextRunning.Checked = AppConfig.Is("matrix_text_running");
            checkTextRunning.CheckedChanged += TextSettings_Changed;

            textClockTime.Text = AppConfig.GetString("matrix_time", "HH:mm");
            textClockDate.Text = AppConfig.GetString("matrix_date", "yy.MM.dd");
            textClockTime.TextChanged += ClockFormat_TextChanged;
            textClockDate.TextChanged += ClockFormat_TextChanged;

            checkClockBattery.Checked = AppConfig.Is("matrix_clock_battery");
            checkClockBattery.CheckedChanged += CheckClockBattery_CheckedChanged;
            textClockDate.Enabled = !checkClockBattery.Checked;

            checkAutoOff.Checked = AppConfig.Is("matrix_auto");
            checkAutoOff.CheckedChanged += CheckAutoOff_CheckedChanged;

            checkLidOff.Checked = AppConfig.Is("matrix_lid");
            checkLidOff.CheckedChanged += CheckLidOff_CheckedChanged;

            int columns = matrixControl.deviceMatrix.MaxColumns + 1;
            int rows = matrixControl.deviceMatrix.MaxRows + 1;

            panelPicture.Height = panelPicture.Width * rows / columns / 3;
            cellWidth = (float)panelPicture.Width / columns;
            cellHeight = (float)panelPicture.Height / rows;

            resetLeft = buttonReset.Left;

            VisualiseMode();

        }

        private void InitFontCombo(RComboBox combo, string param)
        {
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.Items.Add("Default");
            foreach (string font in AniMatrixControl.TextFonts) if (font.Length > 0) combo.Items.Add(font);
            combo.SelectedIndex = Math.Clamp(AppConfig.Get(param, 0), 0, combo.Items.Count - 1);
            combo.SelectedValueChanged += TextSettings_Changed;
        }

        public void VisualiseMode()
        {
            if (InvokeRequired) { Invoke(VisualiseMode); return; }

            MatrixMode running = AniMatrixControl.Mode;
            bool text = running == MatrixMode.Text;
            bool clock = running == MatrixMode.Clock;
            bool audio = running == MatrixMode.Audio;
            bool pic = !text && !clock && !audio;

            buttonPictureMode.Activated = running == MatrixMode.Picture;
            buttonClockMode.Activated = clock;
            buttonAudioMode.Activated = audio;
            buttonTextMode.Activated = text;

            panelPictureSettings.Visible = pic;
            panelTextSettings.Visible = text;
            panelClockSettings.Visible = clock;
            panelAudioSettings.Visible = audio;

            panelButtons.Visible = !audio;
            buttonPicture.Visible = pic;
            buttonReset.Left = pic ? resetLeft : buttonPicture.Left;

            panelMain.PerformLayout();
            ClientSize = new Size(ClientSize.Width, panelMain.Height + panelPower.Height + Padding.Vertical);
            FormPosition();

            VisualisePicture();
        }

        private void ButtonPictureMode_Click(object? sender, EventArgs e)
        {
            if (AppConfig.GetString("matrix_picture") is null)
            {
                ButtonPicture_Click(sender, e);
                return;
            }

            SetRunningMode(MatrixMode.Picture);
        }

        private void ButtonTextMode_Click(object? sender, EventArgs e) => SetRunningMode(MatrixMode.Text);

        private void ButtonClockMode_Click(object? sender, EventArgs e) => SetRunningMode(MatrixMode.Clock);

        private void ButtonAudioMode_Click(object? sender, EventArgs e) => SetRunningMode(MatrixMode.Audio);

        private void SetRunningMode(MatrixMode mode) => Program.settingsForm.SetMatrixRunning((int)mode);

        public void VisualisePicture()
        {
            if (IsDisposed || Disposing) return;
            if (InvokeRequired)
            {
                if (!previewPending)
                {
                    previewPending = true;
                    try { BeginInvoke(VisualisePicture); }
                    catch { previewPending = false; }
                }
                return;
            }

            previewPending = false;
            picturePreview.Image = RenderPreview();
            picturePreview.Invalidate();
        }

        private Bitmap? preview;

        private Bitmap RenderPreview()
        {
            var device = matrixControl.deviceMatrix;
            byte[,] led = device.LedSnapshot();
            int cell = 12, row = 4;

            preview ??= new Bitmap(device.MaxColumns * cell + cell, device.MaxRows * row + row);
            using (Graphics g = Graphics.FromImage(preview))
            using (var dim = new SolidBrush(Color.FromArgb(35, 35, 35)))
            using (var brush = new SolidBrush(Color.Black))
            {
                g.Clear(Color.Black);
                g.SmoothingMode = SmoothingMode.AntiAlias;

                for (int y = 0; y < device.MaxRows; y++)
                    for (int x = device.FirstX(y); x < Math.Min(device.Width(y), device.MaxColumns); x++)
                    {
                        byte v = led[x, y];
                        if (v > 0)
                        {
                            brush.Color = Color.FromArgb(v, v, v);
                            g.FillEllipse(brush, x * cell + (y % 2) * cell / 2, y * row, cell / 2 + 1, row - 1);
                        }
                        else
                            g.FillEllipse(dim, x * cell + (y % 2) * cell / 2, y * row, cell / 2 + 1, row - 1);
                    }
            }

            return preview;
        }

        public void VisualiseMatrix(string fileName)
        {
            if (InvokeRequired) { Invoke(() => VisualiseMatrix(fileName)); return; }

            // same file already loaded for dragging, skip the re-read
            if (fileName == loadedPicture && picture is not null) { VisualisePicture(); return; }

            if (picture is not null) picture.Dispose();

            try
            {
                using (var fs = new FileStream(fileName, FileMode.Open))
                {
                    ms.SetLength(0);
                    fs.CopyTo(ms);
                    ms.Position = 0;
                    picture = Image.FromStream(ms);

                    dimension = new FrameDimension(picture.FrameDimensionsList[0]);
                    frameCount = picture.GetFrameCount(dimension);
                    animated = frameCount > 1;
                    dragFrame = 0;
                    loadedPicture = fileName;

                    if (animated) dragTimer.Interval = AniMatrixControl.PictureFrameDelay(picture);
                }
            }
            catch (Exception ex)
            {
                picture = null;
                Logger.WriteLine(ex.Message);
            }

            VisualisePicture();
        }

        private void GeneratePicture(int matrixX, int matrixY)
        {
            if (picture is null) return;

            matrixControl.GeneratePictureFrame(picture, matrixX, matrixY);
            VisualisePicture();
        }

        private void DragTimer_Tick(object? sender, EventArgs e)
        {
            if (!dragging || picture is null) return;
            picture.SelectActiveFrame(dimension, dragFrame++ % frameCount);
            GeneratePicture(baseX + dragX, baseY + dragY);
        }

        private void PicturePreview_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            MatrixMode running = AniMatrixControl.Mode;
            if (running != MatrixMode.Picture && running != MatrixMode.Clock && running != MatrixMode.Text) return;

            if (running == MatrixMode.Text)
            {
                matrixControl.StopMatrixTimer();
                dragLine = matrixControl.deviceMatrix.HitTestText((int)(e.X / cellWidth), (int)(e.Y / cellHeight));
                string prefix = AnimeMatrixDevice.TextPrefix(dragLine);
                baseX = AppConfig.Get(prefix + "_x", 0);
                baseY = AppConfig.Get(prefix + "_y", 0) / 2 * 2;
                matrixControl.deviceMatrix.PresentText(dragLine, baseX, baseY);
            }
            else if (running == MatrixMode.Clock)
            {
                matrixControl.StopMatrixTimer();
                baseX = AppConfig.Get("matrix_clock_x", 0);
                baseY = AppConfig.Get("matrix_clock_y", 0) / 2 * 2;
            }
            else
            {
                if (picture is null) return;
                matrixControl.StopMatrixTimer();
                matrixControl.deviceMatrix.ClearFrames();
                baseX = AppConfig.Get("matrix_x", 0);
                baseY = AppConfig.Get("matrix_y", 0);
                // freeze long gifs while dragging
                if (animated && frameCount < 60) dragTimer.Start();
            }

            dragging = true;
            xPos = e.X;
            yPos = e.Y;
            dragX = dragY = 0;
        }

        private void PicturePreview_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!dragging) return;

            MatrixMode running = AniMatrixControl.Mode;

            float cols = (e.X - xPos) / cellWidth;
            float rows = (e.Y - yPos) / cellHeight;

            int dX, dY;

            if (running == MatrixMode.Text || running == MatrixMode.Clock)
            {
                // odd row shifts look jagged on the staggered panel, move by 2
                dX = (int)Math.Round(cols);
                dY = 2 * (int)Math.Round(rows / 2);
            }
            else if ((MatrixRotation)AppConfig.Get("matrix_rotation", 0) == MatrixRotation.Planar)
            {
                dX = (int)Math.Round(-cols * 3);
                dY = (int)Math.Round(-rows);
            }
            else
            {
                // diagonal pan axes: convert cell shift to pan delta
                dX = (int)Math.Round(-cols - rows / 2);
                dY = -dX - (int)Math.Round(rows);
            }

            if (dX == dragX && dY == dragY) return;

            dragX = dX;
            dragY = dY;

            if (running == MatrixMode.Text)
            {
                matrixControl.deviceMatrix.PresentText(dragLine, baseX + dragX, baseY + dragY);
            }
            else if (running == MatrixMode.Clock)
            {
                matrixControl.deviceMatrix.PresentClock(baseX + dragX, baseY + dragY);
            }
            else
            {
                GeneratePicture(baseX + dragX, baseY + dragY);
            }
        }

        private void PicturePreview_MouseUp(object? sender, MouseEventArgs e)
        {
            if (!dragging) return;
            dragging = false;
            dragTimer.Stop();

            MatrixMode running = AniMatrixControl.Mode;

            if (running == MatrixMode.Text)
            {
                string prefix = AnimeMatrixDevice.TextPrefix(dragLine);
                AppConfig.Set(prefix + "_x", baseX + dragX);
                AppConfig.Set(prefix + "_y", baseY + dragY);
                dragX = dragY = 0;

                matrixControl.SetMatrixText();
            }
            else if (running == MatrixMode.Clock)
            {
                AppConfig.Set("matrix_clock_x", baseX + dragX);
                AppConfig.Set("matrix_clock_y", baseY + dragY);
                dragX = dragY = 0;

                matrixControl.SetMatrixClock();
            }
            else
            {
                AppConfig.Set("matrix_x", baseX + dragX);
                AppConfig.Set("matrix_y", baseY + dragY);
                dragX = dragY = 0;

                if (animated) ApplyPicture();
                else matrixControl.deviceMatrix.Present();
            }
        }

        private void TrackMatrix_ValueChanged(object? sender, EventArgs e)
        {
            VisualiseMatrix();
        }

        private void TrackMatrix_MouseUp(object? sender, MouseEventArgs e)
        {
            AppConfig.Set("matrix_contrast", trackContrast.Value);
            AppConfig.Set("matrix_gamma", trackGamma.Value);
            ApplyPicture();
        }


        private void ComboRotation_SelectedValueChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("matrix_rotation", comboRotation.SelectedIndex);
            ApplyPicture();
        }

        private void ComboScaling_SelectedValueChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("matrix_quality", comboScaling.SelectedIndex);
            ApplyPicture();
        }

        private void ComboAudioMode_SelectedValueChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("matrix_audio_mode", comboAudioMode.SelectedIndex);
            SetRunningMode(MatrixMode.Audio);
        }

        private static System.Windows.Forms.Timer DebounceTimer(Action apply)
        {
            var timer = new System.Windows.Forms.Timer { Interval = 500 };
            timer.Tick += (s, e) => { timer.Stop(); apply(); };
            return timer;
        }

        private void TextMatrix_TextChanged(object? sender, EventArgs e)
        {
            textTimer.Stop();
            textTimer.Start();
        }

        private void ClockFormat_TextChanged(object? sender, EventArgs e)
        {
            clockTimer.Stop();
            clockTimer.Start();
        }

        private void TextSettings_Changed(object? sender, EventArgs e)
        {
            ApplyText();
        }

        private void ApplyText()
        {
            AppConfig.Set("matrix_text", textMatrix.Text);
            AppConfig.Set("matrix_text2", textMatrix2.Text);
            AppConfig.Set("matrix_text_font", comboTextFont.SelectedIndex);
            AppConfig.Set("matrix_text2_font", comboTextFont2.SelectedIndex);
            AppConfig.Set("matrix_text_size", (int)numTextSize.Value);
            AppConfig.Set("matrix_text2_size", (int)numTextSize2.Value);
            AppConfig.Set("matrix_text_running", checkTextRunning.Checked ? 1 : 0);

            if (AniMatrixControl.Mode != MatrixMode.Text) SetRunningMode(MatrixMode.Text);
            else matrixControl.SetMatrixText();
        }

        private void CheckAutoOff_CheckedChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("matrix_auto", checkAutoOff.Checked ? 1 : 0);
            matrixControl.SetBatteryAuto();
        }

        private void CheckLidOff_CheckedChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("matrix_lid", checkLidOff.Checked ? 1 : 0);
            matrixControl.SetLidMode(true);
        }

        private void CheckClockBattery_CheckedChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("matrix_clock_battery", checkClockBattery.Checked ? 1 : 0);
            textClockDate.Enabled = !checkClockBattery.Checked;
            matrixControl.deviceMatrix.PresentClock();
        }

        private void ApplyClock()
        {
            // only save formats that actually parse
            try { DateTime.Now.ToString(textClockTime.Text); if (textClockTime.Text.Length > 0) AppConfig.Set("matrix_time", textClockTime.Text); } catch { }
            try { DateTime.Now.ToString(textClockDate.Text); if (textClockDate.Text.Length > 0) AppConfig.Set("matrix_date", textClockDate.Text); } catch { }

            matrixControl.deviceMatrix.PresentClock();
        }

        private void Matrix_FormClosed(object? sender, FormClosingEventArgs e)
        {
            matrixControl.deviceMatrix.OnPresent = null;
            textTimer.Dispose();
            clockTimer.Dispose();
            dragTimer.Dispose();

            picturePreview.Image = null;
            preview?.Dispose();
            preview = null;

            picture?.Dispose();
            picture = null;

            // dispose keeps the buffer
            ms.Dispose();
            ms = new MemoryStream();

            // re-apply gating
            if (matrixControl.IsGated) matrixControl.SetDevice();

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
        }

        private void VisualiseMatrix()
        {
            labelZoom.Text = trackZoom.Value + "%";
            labelContrast.Text = trackContrast.Value + "%";
            labelGamma.Text = trackGamma.Value + "%";
        }

        private void ButtonReset_Click(object? sender, EventArgs e)
        {
            MatrixMode running = AniMatrixControl.Mode;

            if (running == MatrixMode.Text)
            {
                AppConfig.Set("matrix_text_x", 0);
                AppConfig.Set("matrix_text_y", 0);
                AppConfig.Set("matrix_text2_x", 0);
                AppConfig.Set("matrix_text2_y", 0);
                numTextSize.Value = 15;
                numTextSize2.Value = 15;
                ApplyText();
                return;
            }

            if (running == MatrixMode.Clock)
            {
                AppConfig.Set("matrix_clock_x", 0);
                AppConfig.Set("matrix_clock_y", 0);
                AppConfig.Set("matrix_time", "HH:mm");
                AppConfig.Set("matrix_date", "yy.MM.dd");
                textClockTime.Text = "HH:mm";
                textClockDate.Text = "yy.MM.dd";
                checkClockBattery.Checked = false;
                matrixControl.deviceMatrix.PresentClock();
                return;
            }

            AppConfig.Set("matrix_gamma", 0);
            AppConfig.Set("matrix_contrast", 100);
            AppConfig.Set("matrix_zoom", 100);
            AppConfig.Set("matrix_x", 0);
            AppConfig.Set("matrix_y", 0);

            trackZoom.Value = 100;
            trackContrast.Value = 100;
            trackGamma.Value = 0;

            ApplyPicture();

        }

        private void TrackZoom_MouseUp(object? sender, EventArgs e)
        {
            AppConfig.Set("matrix_zoom", trackZoom.Value);
            ApplyPicture();
        }

        private void TrackZoom_Changed(object? sender, EventArgs e)
        {
            VisualiseMatrix();
        }

        private async void Matrix_Shown(object? sender, EventArgs e)
        {
            VisualiseMode();

            // gated: render for preview, otherwise just load picture for dragging
            if (matrixControl.IsGated) matrixControl.SetDevice();
            else if (AniMatrixControl.Mode == MatrixMode.Picture) VisualiseMatrix(AppConfig.GetString("matrix_picture"));

            if (!AnimeMatrixDevice.HasDefaultFont && await MatrixFont.Download() && !IsDisposed) matrixControl.SetDevice();
        }

        private void ApplyPicture()
        {
            string path = AppConfig.GetString("matrix_picture");
            if (path is null) return;

            if (AniMatrixControl.Mode != MatrixMode.Picture) SetRunningMode(MatrixMode.Picture);
            else matrixControl.SetMatrixPicture(path);
        }

        private void ButtonPicture_Click(object? sender, EventArgs e)
        {
            matrixControl.OpenMatrixPicture();
            VisualiseMode();
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

    }
}
