using Starlight.AnimeMatrix;
using System.Diagnostics;
using System.Reflection;
using System.Timers;
using System.Drawing.Imaging;
using System.CodeDom.Compiler;
using System.Drawing;

namespace GHelper
{

    public partial class SettingsForm : Form
    {

        static Color colorEco = Color.FromArgb(255, 6, 180, 138);
        static Color colorStandard = Color.FromArgb(255, 58, 174, 239);
        static Color colorTurbo = Color.FromArgb(255, 255, 32, 32);

        static int buttonInactive = 0;
        static int buttonActive = 5;

        static System.Timers.Timer aTimer = default!;

        public string perfName = "Balanced";

        Fans fans;
        Keyboard keyb;

        public SettingsForm()
        {

            InitializeComponent();

            FormClosing += SettingsForm_FormClosing;

            buttonSilent.FlatAppearance.BorderColor = colorEco;
            buttonBalanced.FlatAppearance.BorderColor = colorStandard;
            buttonTurbo.FlatAppearance.BorderColor = colorTurbo;

            buttonEco.FlatAppearance.BorderColor = colorEco;
            buttonStandard.FlatAppearance.BorderColor = colorStandard;
            buttonUltimate.FlatAppearance.BorderColor = colorTurbo;

            buttonSilent.Click += ButtonSilent_Click;
            buttonBalanced.Click += ButtonBalanced_Click;
            buttonTurbo.Click += ButtonTurbo_Click;

            buttonEco.Click += ButtonEco_Click;
            buttonStandard.Click += ButtonStandard_Click;
            buttonUltimate.Click += ButtonUltimate_Click;

            VisibleChanged += SettingsForm_VisibleChanged;

            trackBattery.Scroll += trackBatteryChange;

            button60Hz.Click += Button60Hz_Click;
            button120Hz.Click += Button120Hz_Click;

            buttonQuit.Click += ButtonQuit_Click;

            checkBoost.Click += CheckBoost_Click;

            checkScreen.CheckedChanged += checkScreen_CheckedChanged;

            comboKeyboard.DropDownStyle = ComboBoxStyle.DropDownList;
            comboKeyboard.SelectedIndex = 0;
            comboKeyboard.SelectedValueChanged += ComboKeyboard_SelectedValueChanged;

            buttonKeyboardColor.Click += ButtonKeyboardColor_Click;

            buttonFans.Click += ButtonFans_Click;
            buttonKeyboard.Click += ButtonKeyboard_Click;

            pictureColor.Click += PictureColor_Click;
            pictureColor2.Click += PictureColor2_Click;

            labelVersion.Text = "Version " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            labelVersion.Click += LabelVersion_Click;

            labelCPUFan.Click += LabelCPUFan_Click;
            labelGPUFan.Click += LabelCPUFan_Click;


            InitMatrix();

            comboMatrix.DropDownStyle = ComboBoxStyle.DropDownList;
            comboMatrixRunning.DropDownStyle = ComboBoxStyle.DropDownList;
            comboMatrix.SelectedValueChanged += ComboMatrix_SelectedValueChanged;
            comboMatrixRunning.SelectedValueChanged += ComboMatrixRunning_SelectedValueChanged;

            buttonMatrix.Click += ButtonMatrix_Click;


            SetTimer();

        }

        void SetMatrixPicture(string fileName)
        {

            int width = 34 * 3;
            int height = 61;
            float scale;

            Bitmap image;

            try
            {
                using (var bmpTemp = (Bitmap)Image.FromFile(fileName))
                {
                    image = new Bitmap(bmpTemp);

                    Bitmap canvas = new Bitmap(width, height);

                    scale = Math.Min((float)width / (float)image.Width, (float)height / (float)image.Height);

                    var graph = Graphics.FromImage(canvas);
                    var scaleWidth = (int)(image.Width * scale);
                    var scaleHeight = (int)(image.Height * scale);

                    graph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                    graph.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    graph.DrawImage(image, ((int)width - scaleWidth), ((int)height - scaleHeight) / 2, scaleWidth, scaleHeight);

                    Bitmap bmp = new Bitmap(canvas, 34, 61);

                    var mat = new AnimeMatrixDevice();
                    mat.SetBuiltInAnimation(false);

                    for (int y = 0; y < bmp.Height; y++)
                    {
                        for (int x = 0; x < bmp.Width; x++)
                        {
                            var pixel = bmp.GetPixel(x, y);
                            byte color = (byte)((pixel.R + pixel.G + pixel.B) / 3);
                            mat.SetLedPlanar(x, y, color);
                        }
                    }

                    mat.Present();
                    mat.Dispose();
                }
            }
            catch
            {
                Debug.WriteLine("Error loading picture");
            }


        }


        private void ButtonMatrix_Click(object? sender, EventArgs e)
        {
            Thread t = new Thread((ThreadStart)(() =>
            {
                OpenFileDialog of = new OpenFileDialog();
                of.Filter = "Image Files (*.bmp;*.jpg;*.jpeg,*.png)|*.BMP;*.JPG;*.JPEG;*.PNG";
                if (of.ShowDialog() == DialogResult.OK)
                {
                    Program.config.setConfig("matrix_picture", of.FileName);
                    SetMatrixPicture(of.FileName);
                    BeginInvoke(delegate
                    {
                        comboMatrixRunning.SelectedIndex = 2;
                    });
                }
                return;
            }));

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
        }

        private void ComboMatrixRunning_SelectedValueChanged(object? sender, EventArgs e)
        {
            SetAnimeMatrix();
        }


        private void ComboMatrix_SelectedValueChanged(object? sender, EventArgs e)
        {
            SetAnimeMatrix();
        }

        private void SetAnimeMatrix()
        {

            int brightness = comboMatrix.SelectedIndex;
            int running = comboMatrixRunning.SelectedIndex;

            var mat = new AnimeMatrixDevice();

            BuiltInAnimation animation = new BuiltInAnimation(
                (BuiltInAnimation.Running)running,
                BuiltInAnimation.Sleeping.Starfield,
                BuiltInAnimation.Shutdown.SeeYa,
                BuiltInAnimation.Startup.StaticEmergence
            );


            if (brightness == 0)
            {
                mat.SetDisplayState(false);
            }
            else
            {
                mat.SetDisplayState(true);
                mat.SetBrightness((BrightnessMode)brightness);

                if (running == 2)
                {
                    string fileName = Program.config.getConfigString("matrix_picture");
                    SetMatrixPicture(fileName);
                }
                else
                {
                    mat.SetBuiltInAnimation(true, animation);
                }
            }

            mat.Dispose();

            Program.config.setConfig("matrix_brightness", comboMatrix.SelectedIndex);
            Program.config.setConfig("matrix_running", comboMatrixRunning.SelectedIndex);
        }



        private void LabelCPUFan_Click(object? sender, EventArgs e)
        {
            Program.config.setConfig("fan_rpm", (Program.config.getConfig("fan_rpm") == 1) ? 0 : 1);
            RefreshSensors();
        }

        private void LabelVersion_Click(object? sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("http://github.com/seerge/g-helper/releases") { UseShellExecute = true });
        }

        private void PictureColor2_Click(object? sender, EventArgs e)
        {

            ColorDialog colorDlg = new ColorDialog();
            colorDlg.AllowFullOpen = false;
            colorDlg.Color = pictureColor2.BackColor;

            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                SetAuraColor(color2: colorDlg.Color);
            }
        }

        private void PictureColor_Click(object? sender, EventArgs e)
        {
            buttonKeyboardColor.PerformClick();
        }

        private void ButtonKeyboard_Click(object? sender, EventArgs e)
        {
            if (keyb == null || keyb.Text == "")
            {
                keyb = new Keyboard();
                keyb.Show();
            }
            else
            {
                keyb.Close();
            }
        }

        private void ButtonFans_Click(object? sender, EventArgs e)
        {
            if (fans == null || fans.Text == "")
            {
                fans = new Fans();
                Debug.WriteLine("Starting fans");
            }

            if (fans.Visible)
            {
                fans.Hide();
            }
            else
            {
                fans.Show();
            }


        }

        private void ButtonKeyboardColor_Click(object? sender, EventArgs e)
        {

            if (sender is null)
                return;

            Button but = (Button)sender;

            ColorDialog colorDlg = new ColorDialog();
            colorDlg.AllowFullOpen = false;
            colorDlg.Color = pictureColor.BackColor;

            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                SetAuraColor(color1: colorDlg.Color);
            }
        }

        public void InitAura()
        {
            int mode = Program.config.getConfig("aura_mode");
            int colorCode = Program.config.getConfig("aura_color");
            int colorCode2 = Program.config.getConfig("aura_color2");

            int speed = Program.config.getConfig("aura_speed");

            Color color = Color.FromArgb(255, 255, 255);
            Color color2 = Color.FromArgb(0, 0, 0);

            if (mode == -1)
                mode = 0;

            if (colorCode != -1)
                color = Color.FromArgb(colorCode);

            if (colorCode2 != -1)
                color2 = Color.FromArgb(colorCode2);

            SetAuraColor(color, color2, false);
            SetAuraMode(mode, false);

            Aura.Mode = mode;
        }

        public void InitMatrix()
        {
            int brightness = Program.config.getConfig("matrix_brightness");
            int running = Program.config.getConfig("matrix_running");

            comboMatrix.SelectedIndex = (brightness != -1) ? brightness : 0;
            comboMatrixRunning.SelectedIndex = (running != -1) ? running : 0;
        }


        public void SetAuraColor(Color? color1 = null, Color? color2 = null, bool apply = true)
        {

            if (color1 is not null)
            {
                Aura.Color1 = (Color)color1;
                Program.config.setConfig("aura_color", Aura.Color1.ToArgb());

            }

            if (color2 is not null)
            {
                Aura.Color2 = (Color)color2;
                Program.config.setConfig("aura_color2", Aura.Color2.ToArgb());
            }

            if (apply)
                Aura.ApplyAura();

            pictureColor.BackColor = Aura.Color1;
            pictureColor2.BackColor = Aura.Color2;
        }

        public void SetAuraMode(int mode = 0, bool apply = true)
        {

            //Debug.WriteLine(mode);

            if (mode > 4) mode = 0;

            pictureColor2.Visible = (mode == Aura.Breathe);

            if (Aura.Mode == mode) return; // same mode

            Aura.Mode = mode;

            Program.config.setConfig("aura_mode", mode);

            comboKeyboard.SelectedValueChanged -= ComboKeyboard_SelectedValueChanged;
            comboKeyboard.SelectedIndex = mode;
            comboKeyboard.SelectedValueChanged += ComboKeyboard_SelectedValueChanged;

            if (apply)
                Aura.ApplyAura();

        }

        public void CycleAuraMode()
        {
            SetAuraMode(Program.config.getConfig("aura_mode") + 1);
        }

        private void ComboKeyboard_SelectedValueChanged(object? sender, EventArgs e)
        {
            if (sender is null)
                return;

            ComboBox cmb = (ComboBox)sender;
            SetAuraMode(cmb.SelectedIndex);
        }


        private void CheckBoost_Click(object? sender, EventArgs e)
        {
            if (sender is null)
                return;

            CheckBox chk = (CheckBox)sender;
            if (chk.Checked)
                NativeMethods.SetCPUBoost(2);
            else
                NativeMethods.SetCPUBoost(0);
        }

        private void Button120Hz_Click(object? sender, EventArgs e)
        {
            SetScreen(1000, 1);
        }

        private void Button60Hz_Click(object? sender, EventArgs e)
        {
            SetScreen(60, 0);
        }


        public void SetScreen(int frequency = -1, int overdrive = -1)
        {

            int currentFrequency = NativeMethods.GetRefreshRate();
            if (currentFrequency < 0)  // Laptop screen not detected or has unknown refresh rate
                return;

            if (frequency >= 1000)
            {
                frequency = Program.config.getConfig("max_frequency");
                if (frequency <= 60)
                    frequency = 120;
            }

            if (frequency > 0)
                NativeMethods.SetRefreshRate(frequency);

            try
            {
                if (overdrive > 0)
                    Program.wmi.DeviceSet(ASUSWmi.ScreenOverdrive, overdrive);
            }
            catch
            {
                Debug.WriteLine("Screen Overdrive not supported");
            }


            InitScreen();
        }


        public void InitBoost()
        {
            int boost = NativeMethods.GetCPUBoost();
            checkBoost.Checked = (boost > 0);
        }

        public void InitScreen()
        {

            int frequency = NativeMethods.GetRefreshRate();
            int maxFrequency = Program.config.getConfig("max_frequency");

            if (frequency < 0)
            {
                button60Hz.Enabled = false;
                button120Hz.Enabled = false;
                labelSreen.Text = "Laptop Screen: Turned off";
                button60Hz.BackColor = SystemColors.ControlLight;
                button120Hz.BackColor = SystemColors.ControlLight;
            }
            else
            {
                button60Hz.Enabled = true;
                button120Hz.Enabled = true;
                button60Hz.BackColor = SystemColors.ControlLightLight;
                button120Hz.BackColor = SystemColors.ControlLightLight;
                labelSreen.Text = "Laptop Screen";
            }

            int overdrive = 0;
            try
            {
                overdrive = Program.wmi.DeviceGet(ASUSWmi.ScreenOverdrive);
            }
            catch
            {
                Debug.WriteLine("Screen Overdrive not supported");
            }

            button60Hz.FlatAppearance.BorderSize = buttonInactive;
            button120Hz.FlatAppearance.BorderSize = buttonInactive;

            if (frequency == 60)
            {
                button60Hz.FlatAppearance.BorderSize = buttonActive;
            }
            else
            {
                if (frequency > 60)
                    maxFrequency = frequency;

                Program.config.setConfig("max_frequency", maxFrequency);
                button120Hz.FlatAppearance.BorderSize = buttonActive;
            }

            if (maxFrequency > 60)
            {
                button120Hz.Text = maxFrequency.ToString() + "Hz + OD";
            }

            Program.config.setConfig("frequency", frequency);
            Program.config.setConfig("overdrive", overdrive);
        }

        private void ButtonQuit_Click(object? sender, EventArgs e)
        {
            Close();
            Program.trayIcon.Visible = false;
            Application.Exit();
        }

        private void SettingsForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void ButtonUltimate_Click(object? sender, EventArgs e)
        {
            SetGPUMode(ASUSWmi.GPUModeUltimate);
        }

        private void ButtonStandard_Click(object? sender, EventArgs e)
        {
            SetGPUMode(ASUSWmi.GPUModeStandard);
        }

        private void ButtonEco_Click(object? sender, EventArgs e)
        {
            SetGPUMode(ASUSWmi.GPUModeEco);
        }

        private static void SetTimer()
        {
            aTimer = new System.Timers.Timer(500);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = false;
        }


        private static string FormatFan(int fan)
        {
            if (Program.config.getConfig("fan_rpm") == 1)
                return " Fan: " + (fan * 100).ToString() + "RPM";
            else
                return " Fan: " + Math.Min(Math.Round(fan / 0.6), 100).ToString() + "%"; // relatively to 6000 rpm
        }

        private static void RefreshSensors()
        {

            string cpuFan = FormatFan(Program.wmi.DeviceGet(ASUSWmi.CPU_Fan));
            string gpuFan = FormatFan(Program.wmi.DeviceGet(ASUSWmi.GPU_Fan));

            string cpuTemp = "";
            string gpuTemp = "";
            string battery = "";

            HardwareMonitor.ReadSensors();

            if (HardwareMonitor.cpuTemp > 0)
                cpuTemp = ": " + Math.Round((decimal)HardwareMonitor.cpuTemp).ToString() + "°C - ";

            if (HardwareMonitor.batteryDischarge > 0)
                battery = "Discharging: " + Math.Round((decimal)HardwareMonitor.batteryDischarge, 1).ToString() + "W";

            Program.settingsForm.BeginInvoke(delegate
            {
                Program.settingsForm.labelCPUFan.Text = "CPU" + cpuTemp + cpuFan;
                Program.settingsForm.labelGPUFan.Text = "GPU" + gpuTemp + gpuFan;
                Program.settingsForm.labelBattery.Text = battery;
            });
        }

        private static void OnTimedEvent(Object? source, ElapsedEventArgs? e)
        {
            RefreshSensors();
            aTimer.Interval = 2000;
        }

        private void SettingsForm_VisibleChanged(object? sender, EventArgs e)
        {
            if (this.Visible)
            {
                InitScreen();

                this.Left = Screen.FromControl(this).WorkingArea.Width - 10 - this.Width;
                this.Top = Screen.FromControl(this).WorkingArea.Height - 10 - this.Height;
                this.Activate();

                aTimer.Interval = 300;
                aTimer.Enabled = true;

            }
            else
            {
                aTimer.Enabled = false;
            }
        }

        public void SetPerformanceMode(int PerformanceMode = ASUSWmi.PerformanceBalanced, bool notify = false)
        {

            buttonSilent.FlatAppearance.BorderSize = buttonInactive;
            buttonBalanced.FlatAppearance.BorderSize = buttonInactive;
            buttonTurbo.FlatAppearance.BorderSize = buttonInactive;

            switch (PerformanceMode)
            {
                case ASUSWmi.PerformanceSilent:
                    buttonSilent.FlatAppearance.BorderSize = buttonActive;
                    perfName = "Silent";
                    break;
                case ASUSWmi.PerformanceTurbo:
                    buttonTurbo.FlatAppearance.BorderSize = buttonActive;
                    perfName = "Turbo";
                    break;
                default:
                    buttonBalanced.FlatAppearance.BorderSize = buttonActive;
                    PerformanceMode = ASUSWmi.PerformanceBalanced;
                    perfName = "Balanced";
                    break;
            }

            int oldMode = Program.config.getConfig("performance_mode");
            Program.config.setConfig("performance_" + (int)SystemInformation.PowerStatus.PowerLineStatus, PerformanceMode);
            Program.config.setConfig("performance_mode", PerformanceMode);

            Program.wmi.DeviceSet(ASUSWmi.PerformanceMode, PerformanceMode);

            if (Program.config.getConfig("auto_apply_" + PerformanceMode) == 1)
            {
                Program.wmi.SetFanCurve(0, Program.config.getFanConfig(0));
                Program.wmi.SetFanCurve(1, Program.config.getFanConfig(1));
            }

            if (fans != null && fans.Text != "")
            {
                fans.LoadFans();
                fans.ResetApplyLabel();
            }

            if (notify && (oldMode != PerformanceMode))
            {
                try
                {
                    Program.toast.RunToast(perfName);
                }
                catch
                {
                    Debug.WriteLine("Toast error");
                }
            }

        }


        public void CyclePerformanceMode()
        {
            SetPerformanceMode(Program.config.getConfig("performance_mode") + 1, true);
        }

        public void AutoPerformance(PowerLineStatus Plugged = PowerLineStatus.Online)
        {
            int mode = Program.config.getConfig("performance_" + (int)Plugged);
            if (mode != -1)
                SetPerformanceMode(mode, true);
            else
                SetPerformanceMode(Program.config.getConfig("performance_mode"));
        }


        public void AutoScreen(PowerLineStatus Plugged = PowerLineStatus.Online)
        {
            int ScreenAuto = Program.config.getConfig("screen_auto");
            if (ScreenAuto != 1) return;

            if (Plugged == PowerLineStatus.Online)
                SetScreen(1000, 1);
            else
                SetScreen(60, 0);

            InitScreen();

        }

        public void AutoGPUMode(PowerLineStatus Plugged = PowerLineStatus.Online)
        {

            int GpuAuto = Program.config.getConfig("gpu_auto");
            if (GpuAuto != 1) return;

            int eco = Program.wmi.DeviceGet(ASUSWmi.GPUEco);
            int mux = Program.wmi.DeviceGet(ASUSWmi.GPUMux);


            if (mux == 0) // GPU in Ultimate, ignore
                return;
            else
            {
                if (eco == 1 && Plugged == PowerLineStatus.Online)  // Eco going Standard on plugged
                {
                    Program.wmi.DeviceSet(ASUSWmi.GPUEco, 0);
                    InitGPUMode();
                }
                else if (eco == 0 && Plugged != PowerLineStatus.Online)  // Standard going Eco on plugged
                {
                    Program.wmi.DeviceSet(ASUSWmi.GPUEco, 1);
                    InitGPUMode();
                }

            }
        }

        public int InitGPUMode()
        {

            int eco = Program.wmi.DeviceGet(ASUSWmi.GPUEco);
            int mux = Program.wmi.DeviceGet(ASUSWmi.GPUMux);

            int GpuMode;

            if (mux == 0)
                GpuMode = ASUSWmi.GPUModeUltimate;
            else
            {
                if (eco == 1)
                    GpuMode = ASUSWmi.GPUModeEco;
                else
                    GpuMode = ASUSWmi.GPUModeStandard;

                if (mux != 1)
                    Disable_Ultimate();
            }

            Program.config.setConfig("gpu_mode", GpuMode);
            VisualiseGPUMode(GpuMode);

            return GpuMode;

        }

        public void SetGPUMode(int GPUMode = ASUSWmi.GPUModeStandard)
        {

            int CurrentGPU = Program.config.getConfig("gpu_mode");

            if (CurrentGPU == GPUMode)
                return;

            var restart = false;
            var changed = false;

            if (CurrentGPU == ASUSWmi.GPUModeUltimate)
            {
                DialogResult dialogResult = MessageBox.Show("Switching off Ultimate Mode requires restart", "Reboot now?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Program.wmi.DeviceSet(ASUSWmi.GPUMux, 1);
                    restart = true;
                    changed = true;
                }
            }
            else if (GPUMode == ASUSWmi.GPUModeUltimate)
            {
                DialogResult dialogResult = MessageBox.Show(" Ultimate Mode requires restart", "Reboot now?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Program.wmi.DeviceSet(ASUSWmi.GPUMux, 0);
                    restart = true;
                    changed = true;
                }

            }
            else if (GPUMode == ASUSWmi.GPUModeEco)
            {
                VisualiseGPUMode(GPUMode);
                Program.wmi.DeviceSet(ASUSWmi.GPUEco, 1);
                changed = true;
            }
            else if (GPUMode == ASUSWmi.GPUModeStandard)
            {
                VisualiseGPUMode(GPUMode);
                Program.wmi.DeviceSet(ASUSWmi.GPUEco, 0);
                changed = true;
            }

            if (changed)
                Program.config.setConfig("gpu_mode", GPUMode);

            if (restart)
            {
                VisualiseGPUMode(GPUMode);
                Process.Start("shutdown", "/r /t 1");
            }

        }


        public void VisualiseGPUAuto(int GPUAuto)
        {
            checkGPU.Checked = (GPUAuto == 1);
        }

        public void VisualiseScreenAuto(int ScreenAuto)
        {
            checkScreen.Checked = (ScreenAuto == 1);
        }

        public void VisualiseGPUMode(int GPUMode = -1)
        {

            if (GPUMode == -1)
            {
                GPUMode = Program.config.getConfig("gpu_mode");
            }

            buttonEco.FlatAppearance.BorderSize = buttonInactive;
            buttonStandard.FlatAppearance.BorderSize = buttonInactive;
            buttonUltimate.FlatAppearance.BorderSize = buttonInactive;

            switch (GPUMode)
            {
                case ASUSWmi.GPUModeEco:
                    buttonEco.FlatAppearance.BorderSize = buttonActive;
                    labelGPU.Text = "GPU Mode: iGPU only";
                    Program.trayIcon.Icon = GHelper.Properties.Resources.eco;
                    break;
                case ASUSWmi.GPUModeUltimate:
                    buttonUltimate.FlatAppearance.BorderSize = buttonActive;
                    labelGPU.Text = "GPU Mode: dGPU exclusive";
                    Program.trayIcon.Icon = GHelper.Properties.Resources.ultimate;
                    break;
                default:
                    buttonStandard.FlatAppearance.BorderSize = buttonActive;
                    labelGPU.Text = "GPU Mode: iGPU and dGPU";
                    Program.trayIcon.Icon = GHelper.Properties.Resources.standard;
                    break;
            }
        }


        private void ButtonSilent_Click(object? sender, EventArgs e)
        {
            SetPerformanceMode(ASUSWmi.PerformanceSilent);
        }

        private void ButtonBalanced_Click(object? sender, EventArgs e)
        {
            SetPerformanceMode(ASUSWmi.PerformanceBalanced);
        }

        private void ButtonTurbo_Click(object? sender, EventArgs e)
        {
            SetPerformanceMode(ASUSWmi.PerformanceTurbo);
        }

        private void Settings_Load(object sender, EventArgs e)
        {

        }

        public void Disable_Ultimate()
        {
            buttonUltimate.Enabled = false;
            buttonUltimate.BackColor = SystemColors.ControlLight;
        }

        public void SetStartupCheck(bool status)
        {
            checkStartup.Checked = status;
        }
        private void checkStartup_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            if (chk.Checked)
            {
                Startup.Schedule();
            }
            else
            {
                Startup.UnSchedule();
            }
        }

        public void SetBatteryChargeLimit(int limit = 100)
        {

            if (limit < 40 || limit > 100) return;

            labelBatteryTitle.Text = "Battery Charge Limit: " + limit.ToString() + "%";
            trackBattery.Value = limit;
            Program.wmi.DeviceSet(ASUSWmi.BatteryLimit, limit);

            Program.config.setConfig("charge_limit", limit);

        }

        private void trackBatteryChange(object? sender, EventArgs e)
        {
            if (sender is null)
                return;

            TrackBar bar = (TrackBar)sender;
            SetBatteryChargeLimit(bar.Value);
        }

        private void checkGPU_CheckedChanged(object? sender, EventArgs e)
        {
            if (sender is null)
                return;

            CheckBox chk = (CheckBox)sender;
            if (chk.Checked)
                Program.config.setConfig("gpu_auto", 1);
            else
                Program.config.setConfig("gpu_auto", 0);
        }


        private void checkScreen_CheckedChanged(object? sender, EventArgs e)
        {

            if (sender is null)
                return;

            CheckBox chk = (CheckBox)sender;
            if (chk.Checked)
                Program.config.setConfig("screen_auto", 1);
            else
                Program.config.setConfig("screen_auto", 0);
        }


    }


}
