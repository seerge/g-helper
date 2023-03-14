using Starlight.AnimeMatrix;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Timers;


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
        static System.Timers.Timer matrixTimer = default!;

        public string versionUrl = "http://github.com/seerge/g-helper/releases";

        public string perfName = "Balanced";

        Fans fans;
        Keyboard keyb;

        static AnimeMatrixDevice mat;

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

            checkGPU.CheckedChanged += CheckGPU_CheckedChanged;

            checkScreen.CheckedChanged += checkScreen_CheckedChanged;

            comboKeyboard.DropDownStyle = ComboBoxStyle.DropDownList;
            comboKeyboard.SelectedIndex = 0;
            comboKeyboard.SelectedValueChanged += ComboKeyboard_SelectedValueChanged;

            buttonKeyboardColor.Click += ButtonKeyboardColor_Click;

            buttonFans.Click += ButtonFans_Click;
            buttonKeyboard.Click += ButtonKeyboard_Click;

            pictureColor.Click += PictureColor_Click;
            pictureColor2.Click += PictureColor2_Click;

            labelCPUFan.Click += LabelCPUFan_Click;
            labelGPUFan.Click += LabelCPUFan_Click;

            comboMatrix.DropDownStyle = ComboBoxStyle.DropDownList;
            comboMatrixRunning.DropDownStyle = ComboBoxStyle.DropDownList;

            comboMatrix.DropDownClosed += ComboMatrix_SelectedValueChanged;
            comboMatrixRunning.DropDownClosed += ComboMatrixRunning_SelectedValueChanged;

            checkMatrix.CheckedChanged += CheckMatrix_CheckedChanged; ;

            buttonMatrix.Click += ButtonMatrix_Click;

            checkStartup.CheckedChanged += CheckStartup_CheckedChanged;

            labelVersion.Click += LabelVersion_Click;

            SetTimer();

        }


        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case NativeMethods.WM_POWERBROADCAST:
                    if (m.WParam == (IntPtr)NativeMethods.PBT_POWERSETTINGCHANGE)
                    {
                        var settings = (NativeMethods.POWERBROADCAST_SETTING)m.GetLParam(typeof(NativeMethods.POWERBROADCAST_SETTING));
                        switch (settings.Data)
                        {
                            case 0:
                                Logger.WriteLine("Monitor Power Off");
                                break;
                            case 1:
                                Logger.WriteLine("Monitor Power On");
                                Program.settingsForm.BeginInvoke(delegate
                                {
                                    Program.SetAutoModes();
                                });
                                break;
                            case 2:
                                Logger.WriteLine("Monitor Dimmed");
                                break;
                        }
                    }
                    m.Result = (IntPtr)1;
                    break;
            }
            base.WndProc(ref m);
        }

        private void CheckGPU_CheckedChanged(object? sender, EventArgs e)
        {
            if (sender is null) return;
            CheckBox check = (CheckBox)sender;
            Program.config.setConfig("gpu_auto", check.Checked ? 1 : 0);
        }

        public void SetVersionLabel(string label, string url = null)
        {
            labelVersion.Text = label;
            if (url is not null)
            {
                versionUrl = url;
                labelVersion.ForeColor = Color.Red;
            }
        }


        private void LabelVersion_Click(object? sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo(versionUrl) { UseShellExecute = true });
        }


        private void CheckStartup_CheckedChanged(object? sender, EventArgs e)
        {
            if (sender is null) return;
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

        private void CheckMatrix_CheckedChanged(object? sender, EventArgs e)
        {
            if (sender is null) return;
            CheckBox check = (CheckBox)sender;
            Program.config.setConfig("matrix_auto", check.Checked ? 1 : 0);
        }

        private static void StartMatrixTimer()
        {
            matrixTimer.Enabled = true;
        }

        private static void StopMatrixTimer()
        {
            matrixTimer.Enabled = false;
        }

        private static void MatrixTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (mat is null) return;
            mat.PresentNextFrame();
        }

        void SetMatrixPicture(string fileName)
        {

            if (mat is null) return;

            StopMatrixTimer();

            Image image;

            try
            {
                using (var fs = new FileStream(fileName, FileMode.Open))
                {
                    var ms = new MemoryStream();
                    fs.CopyTo(ms);
                    ms.Position = 0;
                    image = Image.FromStream(ms);
                }
            }
            catch
            {
                Debug.WriteLine("Error loading picture");
                return;
            }

            mat.SetBuiltInAnimation(false);
            mat.ClearFrames();

            FrameDimension dimension = new FrameDimension(image.FrameDimensionsList[0]);
            int frameCount = image.GetFrameCount(dimension);

            if (frameCount > 1)
            {
                for (int i = 0; i < frameCount; i++)
                {
                    image.SelectActiveFrame(dimension, i);
                    mat.GenerateFrame(image);
                    mat.AddFrame();
                }

                StartMatrixTimer();
            }
            else
            {
                mat.GenerateFrame(image);
                mat.Present();
            }


        }


        private void ButtonMatrix_Click(object? sender, EventArgs e)
        {
            string fileName = null;

            Thread t = new Thread(() =>
            {
                OpenFileDialog of = new OpenFileDialog();
                of.Filter = "Image Files (*.bmp;*.jpg;*.jpeg,*.png,*.gif)|*.BMP;*.JPG;*.JPEG;*.PNG;*.GIF";
                if (of.ShowDialog() == DialogResult.OK)
                {
                    fileName = of.FileName;
                }
                return;
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            if (fileName is not null)
            {
                Program.config.setConfig("matrix_picture", fileName);
                SetMatrixPicture(fileName);
                BeginInvoke(delegate
                {
                    comboMatrixRunning.SelectedIndex = 2;
                });
            }


        }

        private void ComboMatrixRunning_SelectedValueChanged(object? sender, EventArgs e)
        {
            Program.config.setConfig("matrix_running", comboMatrixRunning.SelectedIndex);
            SetMatrix();
        }


        private void ComboMatrix_SelectedValueChanged(object? sender, EventArgs e)
        {
            Program.config.setConfig("matrix_brightness", comboMatrix.SelectedIndex);
            SetMatrix();
        }

        public void SetMatrix(PowerLineStatus Plugged = PowerLineStatus.Online)
        {

            if (mat is null) return;

            int brightness = Program.config.getConfig("matrix_brightness");
            int running = Program.config.getConfig("matrix_running");
            bool auto = Program.config.getConfig("matrix_auto") == 1;

            if (brightness < 0) brightness = 0;
            if (running < 0) running = 0;

            BuiltInAnimation animation = new BuiltInAnimation(
                (BuiltInAnimation.Running)running,
                BuiltInAnimation.Sleeping.Starfield,
                BuiltInAnimation.Shutdown.SeeYa,
                BuiltInAnimation.Startup.StaticEmergence
            );

            StopMatrixTimer();

            mat.SetProvider();

            if (brightness == 0 || (auto && Plugged != PowerLineStatus.Online))
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

        }



        private void LabelCPUFan_Click(object? sender, EventArgs e)
        {
            Program.config.setConfig("fan_rpm", (Program.config.getConfig("fan_rpm") == 1) ? 0 : 1);
            RefreshSensors();
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
                //Debug.WriteLine("Starting fans");
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

            try
            {
                mat = new AnimeMatrixDevice();
                matrixTimer = new System.Timers.Timer(100);
                matrixTimer.Elapsed += MatrixTimer_Elapsed;
            }
            catch
            {
                panelMatrix.Visible = false;
                return;
            }

            int brightness = Program.config.getConfig("matrix_brightness");
            int running = Program.config.getConfig("matrix_running");

            comboMatrix.SelectedIndex = (brightness != -1) ? brightness : 0;
            comboMatrixRunning.SelectedIndex = (running != -1) ? running : 0;

            checkMatrix.Checked = (Program.config.getConfig("matrix_auto") == 1);


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

            if (currentFrequency < 0) // Laptop screen not detected or has unknown refresh rate
            {
                InitScreen();
                return;
            }

            if (frequency >= 1000)
            {
                frequency = Program.config.getConfig("max_frequency");
                if (frequency <= 60)
                    frequency = 120;
            }

            if (frequency <= 0) return;

            NativeMethods.SetRefreshRate(frequency);
            if (overdrive > 0)
                Program.wmi.DeviceSet(ASUSWmi.ScreenOverdrive, overdrive);

            InitScreen();
            Logger.WriteLine("Screen " + frequency.ToString() + "Hz");

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
                Logger.WriteLine("Screen Overdrive not supported");
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

            if (HardwareMonitor.gpuTemp != null)
            {
                gpuTemp = $": {HardwareMonitor.gpuTemp}°C - ";
            }

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

                //RefreshSensors();
            }
            else
            {
                aTimer.Enabled = false;
            }
        }

        public void SetPower()
        {
            int limit_total = Program.config.getConfigPerf("limit_total");
            int limit_cpu = Program.config.getConfigPerf("limit_cpu");

            if (limit_total > ASUSWmi.MaxTotal) return;
            if (limit_total < ASUSWmi.MinTotal) return;

            if (limit_cpu > ASUSWmi.MaxCPU) return;
            if (limit_cpu < ASUSWmi.MinCPU) return;

            Program.wmi.DeviceSet(ASUSWmi.PPT_TotalA0, limit_total);
            Program.wmi.DeviceSet(ASUSWmi.PPT_TotalA1, limit_total);
            Program.wmi.DeviceSet(ASUSWmi.PPT_CPUB0, limit_cpu);

            Logger.WriteLine("PowerLimits " + limit_total.ToString() + ", " + limit_cpu.ToString());


        }


        public void AutoFansAndPower()
        {

            if (Program.config.getConfigPerf("auto_apply") == 1)
            {
                Program.wmi.SetFanCurve(0, Program.config.getFanConfig(0));
                Program.wmi.SetFanCurve(1, Program.config.getFanConfig(1));
            }

            if (Program.config.getConfigPerf("auto_apply_power") == 1)
            {
                var timer = new System.Timers.Timer(1000);
                timer.Elapsed += delegate
                {
                    timer.Stop();
                    timer.Dispose();
                    SetPower();
                };
                timer.Start();
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
            Logger.WriteLine("PerfMode " + perfName + " " + PerformanceMode);

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

            AutoFansAndPower();

            if (fans != null && fans.Text != "")
            {
                fans.InitFans();
                fans.InitPower();
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


        }

        public bool AutoGPUMode(PowerLineStatus Plugged = PowerLineStatus.Online)
        {

            int GpuAuto = Program.config.getConfig("gpu_auto");
            if (GpuAuto != 1) return false;

            int eco = Program.wmi.DeviceGet(ASUSWmi.GPUEco);
            int mux = Program.wmi.DeviceGet(ASUSWmi.GPUMux);

            if (mux == 0) // GPU in Ultimate, ignore
                return false;
            else
            {
                if (eco == 1 && Plugged == PowerLineStatus.Online)  // Eco going Standard on plugged
                {
                    SetEcoGPU(0);
                    return true;
                }
                else if (eco == 0 && Plugged != PowerLineStatus.Online)  // Standard going Eco on plugged
                {
                    SetEcoGPU(1);
                    return true;
                }
            }

            return false;

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

                buttonUltimate.Visible = (mux == 1);
            }

            ButtonEnabled(buttonEco, true);
            ButtonEnabled(buttonStandard, true);
            ButtonEnabled(buttonUltimate, true);

            Program.config.setConfig("gpu_mode", GpuMode);
            VisualiseGPUMode(GpuMode);

            return GpuMode;

        }


        public void SetEcoGPU(int eco)
        {

            ButtonEnabled(buttonEco, false);
            ButtonEnabled(buttonStandard, false);
            ButtonEnabled(buttonUltimate, false);

            labelGPU.Text = "GPU Mode: Changing ...";

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                Program.wmi.DeviceSet(ASUSWmi.GPUEco, eco);
                Program.settingsForm.BeginInvoke(delegate
                {
                    InitGPUMode();
                    HardwareMonitor.RecreateGpuTemperatureProviderWithRetry();
                    Thread.Sleep(500);
                    AutoScreen(SystemInformation.PowerStatus.PowerLineStatus);
                });
            }).Start();

        }

        public void SetGPUMode(int GPUMode)
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
                SetEcoGPU(1);
                changed = true;
            }
            else if (GPUMode == ASUSWmi.GPUModeStandard)
            {
                VisualiseGPUMode(GPUMode);
                SetEcoGPU(0);
                changed = true;
            }

            if (changed)
            {
                Program.config.setConfig("gpu_mode", GPUMode);

                HardwareMonitor.RecreateGpuTemperatureProviderWithRetry();
            }

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
                    labelGPU.Text = "GPU Mode: iGPU + dGPU";
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

        public void ButtonEnabled(Button but, bool enabled)
        {
            but.Enabled = enabled;
            but.BackColor = enabled ? SystemColors.ControlLightLight : SystemColors.ControlLight;
        }

        public void SetStartupCheck(bool status)
        {
            checkStartup.CheckedChanged -= CheckStartup_CheckedChanged;
            checkStartup.Checked = status;
            checkStartup.CheckedChanged += CheckStartup_CheckedChanged;
        }

        public void SetBatteryChargeLimit(int limit)
        {

            if (limit < 40 || limit > 100) return;

            labelBatteryTitle.Text = "Battery Charge Limit: " + limit.ToString() + "%";
            trackBattery.Value = limit;
            Program.wmi.DeviceSet(ASUSWmi.BatteryLimit, limit);

            Program.config.setConfig("charge_limit", limit);

        }

        private void trackBatteryChange(object? sender, EventArgs e)
        {
            if (sender is null) return;
            TrackBar bar = (TrackBar)sender;
            SetBatteryChargeLimit(bar.Value);
        }


        private void checkScreen_CheckedChanged(object? sender, EventArgs e)
        {
            if (sender is null) return;
            CheckBox check = (CheckBox)sender;
            Program.config.setConfig("screen_auto", check.Checked ? 1 : 0);
        }


    }


}
