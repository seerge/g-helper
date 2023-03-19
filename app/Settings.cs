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

            HighDpiHelper.AdjustControlImagesDpiScale(this, 2);

            FormClosing += SettingsForm_FormClosing;

            buttonSilent.BorderColor = colorEco;
            buttonBalanced.BorderColor = colorStandard;
            buttonTurbo.BorderColor = colorTurbo;

            buttonEco.BorderColor = colorEco;
            buttonStandard.BorderColor = colorStandard;
            buttonUltimate.BorderColor = colorTurbo;
            buttonOptimized.BorderColor = colorEco;

            button60Hz.BorderColor = SystemColors.ActiveBorder;
            button120Hz.BorderColor = SystemColors.ActiveBorder;
            buttonScreenAuto.BorderColor = SystemColors.ActiveBorder;
            buttonMiniled.BorderColor = colorTurbo;

            buttonOptimized.Click += ButtonOptimized_Click;
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
            buttonScreenAuto.Click += ButtonScreenAuto_Click;
            buttonMiniled.Click += ButtonMiniled_Click;

            buttonQuit.Click += ButtonQuit_Click;

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

            buttonOptimized.MouseMove += ButtonOptimized_MouseHover;
            buttonOptimized.MouseLeave += ButtonGPU_MouseLeave;

            buttonEco.MouseMove += ButtonEco_MouseHover;
            buttonEco.MouseLeave += ButtonGPU_MouseLeave;

            buttonStandard.MouseMove += ButtonStandard_MouseHover;
            buttonStandard.MouseLeave += ButtonGPU_MouseLeave;

            buttonUltimate.MouseMove += ButtonUltimate_MouseHover;
            buttonUltimate.MouseLeave += ButtonGPU_MouseLeave;

            buttonScreenAuto.MouseMove += ButtonScreenAuto_MouseHover;
            buttonScreenAuto.MouseLeave += ButtonScreen_MouseLeave;

            button60Hz.MouseMove += Button60Hz_MouseHover;
            button60Hz.MouseLeave += ButtonScreen_MouseLeave;

            button120Hz.MouseMove += Button120Hz_MouseHover;
            button120Hz.MouseLeave += ButtonScreen_MouseLeave;

            //buttonStandard.Image = (Image)(new Bitmap(buttonStandard.Image, new Size(16, 16)));

            SetTimer();

        }

        private void Button120Hz_MouseHover(object? sender, EventArgs e)
        {
            labelTipScreen.Text = "Max refresh rate + screen overdrive for lower latency";
        }

        private void Button60Hz_MouseHover(object? sender, EventArgs e)
        {
            labelTipScreen.Text = "60Hz refresh rate to save battery";
        }

        private void ButtonScreen_MouseLeave(object? sender, EventArgs e)
        {
            labelTipScreen.Text = "";
        }

        private void ButtonScreenAuto_MouseHover(object? sender, EventArgs e)
        {
            labelTipScreen.Text = "Sets 60Hz to save battery, and back when plugged";
        }

        private void ButtonUltimate_MouseHover(object? sender, EventArgs e)
        {
            labelTipGPU.Text = "Routes laptop screen to dGPU, maximizing FPS";
        }

        private void ButtonStandard_MouseHover(object? sender, EventArgs e)
        {
            labelTipGPU.Text = "Enables dGPU for standard use";
        }

        private void ButtonEco_MouseHover(object? sender, EventArgs e)
        {
            labelTipGPU.Text = "Disables dGPU for battery savings";
        }

        private void ButtonOptimized_MouseHover(object? sender, EventArgs e)
        {
            labelTipGPU.Text = "Switch to Eco on battery and to Standard when plugged";
        }

        private void ButtonGPU_MouseLeave(object? sender, EventArgs e)
        {
            labelTipGPU.Text = "";
        }


        private void ButtonOptimized_Click(object? sender, EventArgs e)
        {
            Program.config.setConfig("gpu_auto", (Program.config.getConfig("gpu_auto") == 1) ? 0 : 1);
            VisualiseGPUMode();
            AutoGPUMode(SystemInformation.PowerStatus.PowerLineStatus);
        }

        private void ButtonScreenAuto_Click(object? sender, EventArgs e)
        {
            Program.config.setConfig("screen_auto", 1);
            InitScreen();
            AutoScreen(SystemInformation.PowerStatus.PowerLineStatus);
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
            Program.config.setConfig("screen_auto", 0);
            SetScreen(1000, 1);
        }

        private void Button60Hz_Click(object? sender, EventArgs e)
        {
            Program.config.setConfig("screen_auto", 0);
            SetScreen(60, 0);
        }

        private void ButtonMiniled_Click(object? sender, EventArgs e)
        {
            int miniled = (Program.config.getConfig("miniled") == 1) ? 0 : 1;
            Program.config.setConfig("miniled", miniled);
            SetScreen(-1, -1, miniled);
        }

        public void SetScreen(int frequency = -1, int overdrive = -1, int miniled = -1)
        {

            if (NativeMethods.GetRefreshRate() < 0) // Laptop screen not detected or has unknown refresh rate
            {
                InitScreen();
                return;
            }

            if (frequency >= 1000)
            {
                frequency = Program.config.getConfig("max_frequency");
                if (frequency <= 60) frequency = 120;
            }

            if (frequency > 0)
            {
                NativeMethods.SetRefreshRate(frequency);
                Logger.WriteLine("Screen " + frequency.ToString() + "Hz");
            }

            if (overdrive >= 0)
                Program.wmi.DeviceSet(ASUSWmi.ScreenOverdrive, overdrive);

            if (miniled >= 0)
            {
                Program.wmi.DeviceSet(ASUSWmi.ScreenMiniled, miniled);
                Debug.WriteLine("Miniled " + miniled);
            }

            InitScreen();

        }

        public void InitScreen()
        {

            int frequency = NativeMethods.GetRefreshRate();
            int maxFrequency = Program.config.getConfig("max_frequency");

            bool screenAuto = (Program.config.getConfig("screen_auto") == 1);

            int overdrive = Program.wmi.DeviceGet(ASUSWmi.ScreenOverdrive);
            int miniled = Program.wmi.DeviceGet(ASUSWmi.ScreenMiniled);

            if (frequency < 0)
            {
                button60Hz.Enabled = false;
                button120Hz.Enabled = false;
                buttonScreenAuto.Enabled = false;
                labelSreen.Text = "Laptop Screen: Turned off";
                button60Hz.BackColor = SystemColors.ControlLight;
                button120Hz.BackColor = SystemColors.ControlLight;
                buttonScreenAuto.BackColor = SystemColors.ControlLight;
            }
            else
            {
                button60Hz.Enabled = true;
                button120Hz.Enabled = true;
                buttonScreenAuto.Enabled = true;
                button60Hz.BackColor = SystemColors.ControlLightLight;
                button120Hz.BackColor = SystemColors.ControlLightLight;
                buttonScreenAuto.BackColor = SystemColors.ControlLightLight;
                labelSreen.Text = "Laptop Screen: " + frequency + "Hz" + ((overdrive == 1) ? " + Overdrive" : "");
            }


            button60Hz.Activated = false;
            button120Hz.Activated = false;
            buttonScreenAuto.Activated = false;

            if (screenAuto)
            {
                buttonScreenAuto.Activated = true;
            }
            else if (frequency == 60)
            {
                button60Hz.Activated = true;
            }
            else
            {
                if (frequency > 60)
                    maxFrequency = frequency;

                Program.config.setConfig("max_frequency", maxFrequency);
                button120Hz.Activated = true;
            }

            if (maxFrequency > 60)
            {
                button120Hz.Text = maxFrequency.ToString() + "Hz + OD";
            }

            if (miniled >= 0)
            {
                tableScreen.Controls.Add(buttonMiniled, 3, 0);
                buttonMiniled.Activated = (miniled == 1);
                Program.config.setConfig("miniled", miniled);
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

            if (Program.wmi.DeviceGet(ASUSWmi.PPT_TotalA0) >= 0)
                Program.wmi.DeviceSet(ASUSWmi.PPT_TotalA0, limit_total);

            if (Program.wmi.DeviceGet(ASUSWmi.PPT_CPUB0) >= 0)
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


            buttonSilent.Activated = false;
            buttonBalanced.Activated = false;
            buttonTurbo.Activated = false;

            switch (PerformanceMode)
            {
                case ASUSWmi.PerformanceSilent:
                    buttonSilent.Activated = true;
                    perfName = "Silent";
                    break;
                case ASUSWmi.PerformanceTurbo:
                    buttonTurbo.Activated = true;
                    perfName = "Turbo";
                    break;
                default:
                    buttonBalanced.Activated = true;
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

            NativeMethods.SetPowerScheme(PerformanceMode);

            if (NativeMethods.PowerGetEffectiveOverlayScheme(out Guid activeScheme) == 0)
            {
                Debug.WriteLine("Effective :" + activeScheme);
            }

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
            if (Program.config.getConfig("screen_auto") != 1) return;

            if (Plugged == PowerLineStatus.Online)
                SetScreen(1000, 1);
            else
                SetScreen(60, 0);


        }

        public bool AutoGPUMode(PowerLineStatus Plugged = PowerLineStatus.Online)
        {

            bool GpuAuto = Program.config.getConfig("gpu_auto") == 1;
            if (!GpuAuto) return false;

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

        private void UltimateUI(bool ultimate)
        {
            if (!ultimate)
            {
                tableGPU.Controls.Remove(buttonUltimate);

                /*
                 * buttonFans.Image = null;
                buttonFans.Height = 44;
                */

                tablePerf.ColumnCount = 0;
                tableGPU.ColumnCount = 0;
                tableScreen.ColumnCount = 0;

            }

            tableLayoutKeyboard.ColumnCount = 0;
            tableLayoutMatrix.ColumnCount = 0;


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

                UltimateUI(mux == 1);

            }

            Program.config.setConfig("gpu_mode", GpuMode);

            ButtonEnabled(buttonOptimized, true);
            ButtonEnabled(buttonEco, true);
            ButtonEnabled(buttonStandard, true);
            ButtonEnabled(buttonUltimate, true);

            VisualiseGPUMode(GpuMode);

            return GpuMode;

        }


        public void SetEcoGPU(int eco)
        {

            ButtonEnabled(buttonOptimized, false);
            ButtonEnabled(buttonEco, false);
            ButtonEnabled(buttonStandard, false);
            ButtonEnabled(buttonUltimate, false);

            labelGPU.Text = "GPU Mode: Changing ...";

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                if (eco == 1)
                {
                    string[] tokill = { "EADesktop", "RadeonSoftware" };
                    foreach (string kill in tokill)
                        foreach (var process in Process.GetProcessesByName(kill)) process.Kill();
                }

                Program.wmi.DeviceSet(ASUSWmi.GPUEco, eco);
                Program.settingsForm.BeginInvoke(delegate
                {
                    InitGPUMode();
                    HardwareMonitor.RecreateGpuTemperatureProviderWithRetry();
                    Thread.Sleep(500);
                    AutoScreen(SystemInformation.PowerStatus.PowerLineStatus);
                });
            })
            {

            }.Start();

        }

        public void SetGPUMode(int GPUMode)
        {

            int CurrentGPU = Program.config.getConfig("gpu_mode");
            Program.config.setConfig("gpu_auto", 0);

            if (CurrentGPU == GPUMode)
            {
                VisualiseGPUMode();
                return;
            }

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
                DialogResult dialogResult = MessageBox.Show("Ultimate Mode requires restart", "Reboot now?", MessageBoxButtons.YesNo);
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
                VisualiseGPUMode();
                Process.Start("shutdown", "/r /t 1");
            }

        }


        public void VisualiseGPUMode(int GPUMode = -1)
        {

            if (GPUMode == -1)
                GPUMode = Program.config.getConfig("gpu_mode");

            bool GPUAuto = (Program.config.getConfig("gpu_auto") == 1);

            buttonEco.Activated = false;
            buttonStandard.Activated = false;
            buttonUltimate.Activated = false;
            buttonOptimized.Activated = false;

            switch (GPUMode)
            {
                case ASUSWmi.GPUModeEco:
                    buttonOptimized.BorderColor = colorEco;
                    buttonEco.Activated = !GPUAuto;
                    buttonOptimized.Activated = GPUAuto;
                    labelGPU.Text = "GPU Mode: iGPU only";
                    Program.trayIcon.Icon = Properties.Resources.eco;
                    break;
                case ASUSWmi.GPUModeUltimate:
                    buttonUltimate.Activated = true;
                    labelGPU.Text = "GPU Mode: dGPU exclusive";
                    Program.trayIcon.Icon = Properties.Resources.ultimate;
                    break;
                default:
                    buttonOptimized.BorderColor = colorStandard;
                    buttonStandard.Activated = !GPUAuto;
                    buttonOptimized.Activated = GPUAuto;
                    labelGPU.Text = "GPU Mode: iGPU + dGPU";
                    Program.trayIcon.Icon = Properties.Resources.standard;
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


    }


}
