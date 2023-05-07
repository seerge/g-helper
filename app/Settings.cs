using CustomControls;
using Starlight.AnimeMatrix;
using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Timers;
using Tools;

namespace GHelper
{

    public partial class SettingsForm : RForm
    {

        private ContextMenuStrip contextMenuStrip = new CustomContextMenu();
        private ToolStripMenuItem menuSilent, menuBalanced, menuTurbo, menuEco, menuStandard, menuUltimate, menuOptimized;

        public static System.Timers.Timer aTimer = default!;
        public static Point trayPoint;

        static System.Timers.Timer matrixTimer = default!;

        public string versionUrl = "http://github.com/seerge/g-helper/releases";

        public string perfName = "Balanced";

        public Fans fans;
        public Extra keyb;

        static AnimeMatrixDevice mat;
        static long lastRefresh;

        private bool customFans = false;
        private int customPower = 0;

        public SettingsForm()
        {


            InitializeComponent();
            InitTheme(true);

            FormClosing += SettingsForm_FormClosing;

            buttonSilent.BorderColor = colorEco;
            buttonBalanced.BorderColor = colorStandard;
            buttonTurbo.BorderColor = colorTurbo;

            buttonEco.BorderColor = colorEco;
            buttonStandard.BorderColor = colorStandard;
            buttonUltimate.BorderColor = colorTurbo;
            buttonOptimized.BorderColor = colorEco;
            buttonXGM.BorderColor = colorTurbo;

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

            button60Hz.Click += Button60Hz_Click;
            button120Hz.Click += Button120Hz_Click;
            buttonScreenAuto.Click += ButtonScreenAuto_Click;
            buttonMiniled.Click += ButtonMiniled_Click;

            buttonQuit.Click += ButtonQuit_Click;

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
            labelVersion.ForeColor = Color.FromArgb(128, Color.Gray);

            buttonOptimized.MouseMove += ButtonOptimized_MouseHover;
            buttonOptimized.MouseLeave += ButtonGPU_MouseLeave;

            buttonEco.MouseMove += ButtonEco_MouseHover;
            buttonEco.MouseLeave += ButtonGPU_MouseLeave;

            buttonStandard.MouseMove += ButtonStandard_MouseHover;
            buttonStandard.MouseLeave += ButtonGPU_MouseLeave;

            buttonUltimate.MouseMove += ButtonUltimate_MouseHover;
            buttonUltimate.MouseLeave += ButtonGPU_MouseLeave;

            buttonXGM.Click += ButtonXGM_Click;

            buttonScreenAuto.MouseMove += ButtonScreenAuto_MouseHover;
            buttonScreenAuto.MouseLeave += ButtonScreen_MouseLeave;

            button60Hz.MouseMove += Button60Hz_MouseHover;
            button60Hz.MouseLeave += ButtonScreen_MouseLeave;

            button120Hz.MouseMove += Button120Hz_MouseHover;
            button120Hz.MouseLeave += ButtonScreen_MouseLeave;

            sliderBattery.ValueChanged += SliderBattery_ValueChanged;
            Program.trayIcon.MouseMove += TrayIcon_MouseMove;

            aTimer = new System.Timers.Timer(1000);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.Enabled = true;

            SetVersionLabel(Properties.Strings.VersionLabel + ": " + Assembly.GetExecutingAssembly().GetName().Version);

            string model = Program.config.GetModel();
            int trim = model.LastIndexOf("_");
            if (trim > 0) model = model.Substring(0, trim);

            labelModel.Text = model;

            TopMost = Program.config.getConfig("topmost") == 1;

            SetContextMenu();

            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                CheckForUpdatesAsync();
            });

        }

        public void SetContextMenu()
        {

            contextMenuStrip.Items.Clear();

            Padding padding = new Padding(5, 5, 5, 5);

            /*
            TableLayoutPanel[] tables = { tablePerf, tableGPU };
            string[] titles = { Properties.Strings.PerformanceMode, Properties.Strings.GPUMode};

            int index = 0;
            foreach (TableLayoutPanel table in tables)
            {

                var title = new ToolStripMenuItem(titles[index]);
                title.Margin = padding;
                title.Enabled = false;
                contextMenuStrip.Items.Add(title);

                foreach (Control control in table.Controls)
                {
                    var button = control as RButton;
                    if (button != null && !button.Secondary && button.Enabled)
                    {
                        var menu = new ToolStripMenuItem(button.Text);
                        menu.Margin = padding;
                        menu.Checked = button.Activated;
                        menu.Click += delegate { button.PerformClick(); };
                        contextMenuStrip.Items.Add(menu);
                    }
                }
                contextMenuStrip.Items.Add("-");
                index++;
            }*/


            var title = new ToolStripMenuItem(Properties.Strings.PerformanceMode);
            title.Margin = padding;
            title.Enabled = false;
            contextMenuStrip.Items.Add(title);

            menuSilent = new ToolStripMenuItem(Properties.Strings.Silent);
            menuSilent.Click += ButtonSilent_Click;
            menuSilent.Margin = padding;
            contextMenuStrip.Items.Add(menuSilent);

            menuBalanced = new ToolStripMenuItem(Properties.Strings.Balanced);
            menuBalanced.Click += ButtonBalanced_Click;
            menuBalanced.Margin = padding;
            contextMenuStrip.Items.Add(menuBalanced);

            menuTurbo = new ToolStripMenuItem(Properties.Strings.Turbo);
            menuTurbo.Click += ButtonTurbo_Click;
            menuTurbo.Checked = true;
            menuTurbo.Margin = padding;
            contextMenuStrip.Items.Add(menuTurbo);

            contextMenuStrip.Items.Add("-");

            var titleGPU = new ToolStripMenuItem(Properties.Strings.GPUMode);
            titleGPU.Margin = padding;
            titleGPU.Enabled = false;
            contextMenuStrip.Items.Add(titleGPU);

            menuEco = new ToolStripMenuItem(Properties.Strings.EcoMode);
            menuEco.Click += ButtonEco_Click;
            menuEco.Margin = padding;
            contextMenuStrip.Items.Add(menuEco);

            menuStandard = new ToolStripMenuItem(Properties.Strings.StandardMode);
            menuStandard.Click += ButtonStandard_Click;
            menuStandard.Margin = padding;
            contextMenuStrip.Items.Add(menuStandard);

            menuUltimate = new ToolStripMenuItem(Properties.Strings.UltimateMode);
            menuUltimate.Click += ButtonUltimate_Click;
            menuUltimate.Margin = padding;
            contextMenuStrip.Items.Add(menuUltimate);

            menuOptimized = new ToolStripMenuItem(Properties.Strings.Optimized);
            menuOptimized.Click += ButtonOptimized_Click;
            menuOptimized.Margin = padding;
            contextMenuStrip.Items.Add(menuOptimized);

            contextMenuStrip.Items.Add("-");

            var quit = new ToolStripMenuItem(Properties.Strings.Quit);
            quit.Click += ButtonQuit_Click;
            quit.Margin = padding;
            contextMenuStrip.Items.Add(quit);

            contextMenuStrip.ShowCheckMargin = true;
            contextMenuStrip.RenderMode = ToolStripRenderMode.System;

            if (CheckSystemDarkModeStatus())
            {
                contextMenuStrip.BackColor = this.BackColor;
                contextMenuStrip.ForeColor = this.ForeColor;
            }

            Program.trayIcon.ContextMenuStrip = contextMenuStrip;


        }

        private void ButtonXGM_Click(object? sender, EventArgs e)
        {
            if (Program.wmi.DeviceGet(ASUSWmi.GPUXG) == 1)
            {
                Program.wmi.DeviceSet(ASUSWmi.GPUXG, 0, "GPU XGM");
            }
            else
            {
                Program.wmi.DeviceSet(ASUSWmi.GPUXG, 1, "GPU XGM");
            }
            InitXGM();
        }

        private void SliderBattery_ValueChanged(object? sender, EventArgs e)
        {
            SetBatteryChargeLimit(sliderBattery.Value);
        }


        public async void CheckForUpdatesAsync()
        {

            try
            {

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "C# App");
                    var json = await httpClient.GetStringAsync("https://api.github.com/repos/seerge/g-helper/releases/latest");
                    var config = JsonSerializer.Deserialize<JsonElement>(json);
                    var tag = config.GetProperty("tag_name").ToString().Replace("v", "");
                    var url = config.GetProperty("assets")[0].GetProperty("browser_download_url").ToString();

                    var gitVersion = new Version(tag);
                    var appVersion = new Version(Assembly.GetExecutingAssembly().GetName().Version.ToString());

                    if (gitVersion.CompareTo(appVersion) > 0)
                    {
                        SetVersionLabel(Properties.Strings.DownloadUpdate + ": " + tag, url);
                    }
                    else
                    {
                        Debug.WriteLine("Latest version");
                    }

                }
            }
            catch (Exception ex)
            {
                //Logger.WriteLine("Failed to check for updates:" + ex.Message);

            }

        }

        private static void TrayIcon_MouseMove(object? sender, MouseEventArgs e)
        {
            Program.settingsForm.RefreshSensors();
        }


        private static void OnTimedEvent(Object? source, ElapsedEventArgs? e)
        {
            Program.settingsForm.RefreshSensors();
        }

        private void Button120Hz_MouseHover(object? sender, EventArgs e)
        {
            labelTipScreen.Text = Properties.Strings.MaxRefreshTooltip;
        }

        private void Button60Hz_MouseHover(object? sender, EventArgs e)
        {
            labelTipScreen.Text = Properties.Strings.MinRefreshTooltip;
        }

        private void ButtonScreen_MouseLeave(object? sender, EventArgs e)
        {
            labelTipScreen.Text = "";
        }

        private void ButtonScreenAuto_MouseHover(object? sender, EventArgs e)
        {
            labelTipScreen.Text = Properties.Strings.AutoRefreshTooltip;
        }

        private void ButtonUltimate_MouseHover(object? sender, EventArgs e)
        {
            labelTipGPU.Text = Properties.Strings.UltimateGPUTooltip;
        }

        private void ButtonStandard_MouseHover(object? sender, EventArgs e)
        {
            labelTipGPU.Text = Properties.Strings.StandardGPUTooltip;
        }

        private void ButtonEco_MouseHover(object? sender, EventArgs e)
        {
            labelTipGPU.Text = Properties.Strings.EcoGPUTooltip;
        }

        private void ButtonOptimized_MouseHover(object? sender, EventArgs e)
        {
            labelTipGPU.Text = Properties.Strings.OptimizedGPUTooltip;
        }

        private void ButtonGPU_MouseLeave(object? sender, EventArgs e)
        {
            labelTipGPU.Text = "";
        }


        private void ButtonOptimized_Click(object? sender, EventArgs e)
        {
            Program.config.setConfig("gpu_auto", (Program.config.getConfig("gpu_auto") == 1) ? 0 : 1);
            VisualiseGPUMode();
            AutoGPUMode();
        }

        private void ButtonScreenAuto_Click(object? sender, EventArgs e)
        {
            Program.config.setConfig("screen_auto", 1);
            InitScreen();
            AutoScreen();
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
                                SetBatteryChargeLimit(Program.config.getConfig("charge_limit"));
                                break;
                            case 1:
                                Logger.WriteLine("Monitor Power On");
                                Program.SetAutoModes();
                                break;
                            case 2:
                                Logger.WriteLine("Monitor Dimmed");
                                break;
                        }
                    }
                    m.Result = (IntPtr)1;
                    break;
                case KeyHandler.WM_HOTKEY_MSG_ID:
                    CyclePerformanceMode();
                    break;
            }
            base.WndProc(ref m);
        }


        void SetVersionLabel(string label, string url = null)
        {
            labelVersion.Text = label;
            if (url is not null)
            {
                this.versionUrl = url;
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

        private static void StartMatrixTimer(int interval = 100)
        {
            matrixTimer.Interval = interval;
            matrixTimer.Enabled = true;
        }

        private static void StopMatrixTimer()
        {
            matrixTimer.Enabled = false;
        }

        private static void MatrixTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (mat is null) return;

            switch (Program.config.getConfig("matrix_running"))
            {
                case 2:
                    mat.PresentNextFrame();
                    break;
                case 3:
                    mat.PresentClock();
                    break;
            }

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
                Logger.WriteLine("Matrix GIF " + fileName);
            }
            else
            {
                mat.GenerateFrame(image);
                mat.Present();
                Logger.WriteLine("Matrix " + fileName);
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
                Program.config.setConfig("matrix_running", 2);

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

        public void SetMatrix()
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

            if (brightness == 0 || (auto && SystemInformation.PowerStatus.PowerLineStatus != PowerLineStatus.Online))
            {
                mat.SetDisplayState(false);
                Logger.WriteLine("Matrix Off");
            }
            else
            {
                mat.SetDisplayState(true);
                mat.SetBrightness((BrightnessMode)brightness);

                switch (running)
                {
                    case 2:
                        SetMatrixPicture(Program.config.getConfigString("matrix_picture"));
                        break;
                    case 3:
                        mat.SetBuiltInAnimation(false);
                        StartMatrixTimer(1000);
                        Logger.WriteLine("Matrix Clock");
                        break;
                    default:
                        mat.SetBuiltInAnimation(true, animation);
                        Logger.WriteLine("Matrix builtin " + animation.AsByte);
                        break;

                }

                //mat.SetBrightness((BrightnessMode)brightness);
            }

        }



        private void LabelCPUFan_Click(object? sender, EventArgs e)
        {
            Program.config.setConfig("fan_rpm", (Program.config.getConfig("fan_rpm") == 1) ? 0 : 1);
            RefreshSensors(true);
        }

        private void PictureColor2_Click(object? sender, EventArgs e)
        {

            ColorDialog colorDlg = new ColorDialog();
            colorDlg.AllowFullOpen = true;
            colorDlg.Color = pictureColor2.BackColor;

            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                Program.config.setConfig("aura_color2", colorDlg.Color.ToArgb());
                SetAura();
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
                keyb = new Extra();
                keyb.Show();
            }
            else
            {
                keyb.Close();
            }
        }

        public void FansToggle()
        {
            if (fans == null || fans.Text == "")
            {
                fans = new Fans();
            }

            if (fans.Visible)
            {
                fans.Close();
            }
            else
            {
                fans.Show();
            }

        }

        private void ButtonFans_Click(object? sender, EventArgs e)
        {
            FansToggle();
        }

        private void ButtonKeyboardColor_Click(object? sender, EventArgs e)
        {

            ColorDialog colorDlg = new ColorDialog();
            colorDlg.AllowFullOpen = true;
            colorDlg.Color = pictureColor.BackColor;

            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                Program.config.setConfig("aura_color", colorDlg.Color.ToArgb());
                SetAura();
            }
        }

        public void InitAura()
        {
            Aura.Mode = Program.config.getConfig("aura_mode");
            Aura.Speed = Program.config.getConfig("aura_speed");
            Aura.SetColor(Program.config.getConfig("aura_color"));
            Aura.SetColor2(Program.config.getConfig("aura_color2"));

            comboKeyboard.DropDownStyle = ComboBoxStyle.DropDownList;
            comboKeyboard.DataSource = new BindingSource(Aura.GetModes(), null);
            comboKeyboard.DisplayMember = "Value";
            comboKeyboard.ValueMember = "Key";
            comboKeyboard.SelectedValue = Aura.Mode;
            comboKeyboard.SelectedValueChanged += ComboKeyboard_SelectedValueChanged;

            pictureColor.BackColor = Aura.Color1;
            pictureColor2.BackColor = Aura.Color2;
            pictureColor2.Visible = Aura.HasSecondColor();
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

            comboMatrix.SelectedIndex = (brightness != -1) ? Math.Min(brightness, comboMatrix.Items.Count - 1) : 0;
            comboMatrixRunning.SelectedIndex = (running != -1) ? Math.Min(running, comboMatrixRunning.Items.Count - 1) : 0;

            checkMatrix.Checked = (Program.config.getConfig("matrix_auto") == 1);


        }


        public void SetAura()
        {
            Aura.Mode = Program.config.getConfig("aura_mode");
            Aura.Speed = Program.config.getConfig("aura_speed");
            Aura.SetColor(Program.config.getConfig("aura_color"));
            Aura.SetColor2(Program.config.getConfig("aura_color2"));

            pictureColor.BackColor = Aura.Color1;
            pictureColor2.BackColor = Aura.Color2;
            pictureColor2.Visible = Aura.HasSecondColor();

            Aura.ApplyAura();

        }

        public void CycleAuraMode()
        {
            if (comboKeyboard.SelectedIndex < comboKeyboard.Items.Count - 1)
                comboKeyboard.SelectedIndex += 1;
            else
                comboKeyboard.SelectedIndex = 0;
        }

        private void ComboKeyboard_SelectedValueChanged(object? sender, EventArgs e)
        {
            Program.config.setConfig("aura_mode", (int)comboKeyboard.SelectedValue);
            SetAura();
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

        public void ToogleMiniled()
        {
            int miniled = (Program.config.getConfig("miniled") == 1) ? 0 : 1;
            Program.config.setConfig("miniled", miniled);
            SetScreen(-1, -1, miniled);
        }

        private void ButtonMiniled_Click(object? sender, EventArgs e)
        {
            ToogleMiniled();
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
                frequency = NativeMethods.GetRefreshRate(true);
            }

            if (frequency > 0)
            {
                NativeMethods.SetRefreshRate(frequency);
            }

            if (overdrive >= 0)
            {
                if (Program.config.getConfig("no_overdrive") == 1) overdrive = 0;
                Program.wmi.DeviceSet(ASUSWmi.ScreenOverdrive, overdrive, "ScreenOverdrive");

            }

            if (miniled >= 0)
            {
                Program.wmi.DeviceSet(ASUSWmi.ScreenMiniled, miniled, "Miniled");
                Debug.WriteLine("Miniled " + miniled);
            }

            InitScreen();
        }

        public void InitScreen()
        {

            int frequency = NativeMethods.GetRefreshRate();
            int maxFrequency = NativeMethods.GetRefreshRate(true);

            bool screenAuto = (Program.config.getConfig("screen_auto") == 1);
            bool overdriveSetting = (Program.config.getConfig("no_overdrive") != 1);

            int overdrive = Program.wmi.DeviceGet(ASUSWmi.ScreenOverdrive);
            int miniled = Program.wmi.DeviceGet(ASUSWmi.ScreenMiniled);

            bool screenEnabled = (frequency >= 0);

            ButtonEnabled(button60Hz, screenEnabled);
            ButtonEnabled(button120Hz, screenEnabled);
            ButtonEnabled(buttonScreenAuto, screenEnabled);
            ButtonEnabled(buttonMiniled, screenEnabled);

            labelSreen.Text = screenEnabled
                ? Properties.Strings.LaptopScreen + ": " + frequency + "Hz" + ((overdrive == 1) ? " + " + Properties.Strings.Overdrive : "")
                : Properties.Strings.LaptopScreen + ": " + Properties.Strings.TurnedOff;

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
            else if (frequency > 60)
            {
                button120Hz.Activated = true;
            }

            if (maxFrequency > 60)
            {
                button120Hz.Text = maxFrequency.ToString() + "Hz" + (overdriveSetting ? " + OD" : "");
                panelScreen.Visible = true;
            }
            else if (maxFrequency > 0)
            {
                panelScreen.Visible = false;
            }

            if (miniled >= 0)
            {
                buttonMiniled.Activated = (miniled == 1);
                Program.config.setConfig("miniled", miniled);
            }
            else
            {
                buttonMiniled.Visible = false;
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

        public void RefreshSensors(bool force = false)
        {

            if (!force && Math.Abs(DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastRefresh) < 2000) return;
            lastRefresh = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            string cpuTemp = "";
            string gpuTemp = "";
            string battery = "";

            HardwareControl.ReadSensors();

            if (HardwareControl.cpuTemp > 0)
                cpuTemp = ": " + Math.Round((decimal)HardwareControl.cpuTemp).ToString() + "°C ";

            if (HardwareControl.batteryDischarge > 0)
                battery = Properties.Strings.Discharging + ": " + Math.Round((decimal)HardwareControl.batteryDischarge, 1).ToString() + "W";

            if (HardwareControl.gpuTemp > 0)
            {
                gpuTemp = $": {HardwareControl.gpuTemp}°C ";
            }


            Program.settingsForm.BeginInvoke(delegate
            {
                labelCPUFan.Text = "CPU" + cpuTemp + HardwareControl.cpuFan;
                labelGPUFan.Text = "GPU" + gpuTemp + HardwareControl.gpuFan;
                if (HardwareControl.midFan is not null)
                    labelMidFan.Text = "Mid" + HardwareControl.midFan;

                labelBattery.Text = battery;
            });


            Program.trayIcon.Text = "CPU" + cpuTemp + HardwareControl.cpuFan + "\n"
                                    + "GPU" + gpuTemp + HardwareControl.gpuFan +
                                    ((battery.Length > 0) ? ("\n" + battery) : "");

        }


        private void SettingsForm_VisibleChanged(object? sender, EventArgs e)
        {
            if (this.Visible)
            {
                InitScreen();
                InitXGM();

                this.Left = Screen.FromControl(this).WorkingArea.Width - 10 - this.Width;
                this.Top = Screen.FromControl(this).WorkingArea.Height - 10 - this.Height;
                this.Activate();

                aTimer.Enabled = true;

            }
            else
            {
                aTimer.Enabled = false;
            }
        }


        private void SetPerformanceLabel()
        {
            labelPerf.Text = Properties.Strings.PerformanceMode + (customFans ? "+" : "") + ((customPower > 0) ? " " + customPower + "W" : "");
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
            {
                Program.wmi.DeviceSet(ASUSWmi.PPT_TotalA0, limit_total, "PowerLimit A0");
                Program.wmi.DeviceSet(ASUSWmi.PPT_APUA3, limit_total, "PowerLimit A3");
                customPower = limit_total;
            }

            if (Program.wmi.DeviceGet(ASUSWmi.PPT_CPUB0) >= 0)
            {
                Program.wmi.DeviceSet(ASUSWmi.PPT_CPUB0, limit_cpu, "PowerLimit B0");
                customPower = limit_cpu;
            }

            Program.settingsForm.BeginInvoke(SetPerformanceLabel);

        }

        public void SetGPUPower()
        {

            int gpu_boost = Program.config.getConfig("gpu_boost");
            int gpu_temp = Program.config.getConfig("gpu_temp");

            if (gpu_boost < ASUSWmi.MinGPUBoost || gpu_boost > ASUSWmi.MaxGPUBoost ) return;
            if (gpu_temp < ASUSWmi.MinGPUTemp || gpu_temp > ASUSWmi.MaxGPUTemp) return;

            if (Program.wmi.DeviceGet(ASUSWmi.PPT_GPUC0) >= 0)
            {
                Program.wmi.DeviceSet(ASUSWmi.PPT_GPUC0, gpu_boost, "PowerLimit C0");
            }

            if (Program.wmi.DeviceGet(ASUSWmi.PPT_GPUC2) >= 0)
            {
                Program.wmi.DeviceSet(ASUSWmi.PPT_GPUC2, gpu_temp, "PowerLimit C2");
            }

        }

        protected void LabelFansResult(string text)
        {
            if (fans != null && fans.Text != "")
                fans.LabelFansResult(text);
        }


        public void AutoFans(bool force = false)
        {
            customFans = false;

            if (Program.config.getConfigPerf("auto_apply") == 1 || force)
            {
                int cpuResult = Program.wmi.SetFanCurve(0, Program.config.getFanConfig(0));
                int gpuResult = Program.wmi.SetFanCurve(1, Program.config.getFanConfig(1));

                if (Program.config.getConfig("mid_fan") == 1)
                    Program.wmi.SetFanCurve(2, Program.config.getFanConfig(2));

                // something went wrong, resetting to default profile
                if (cpuResult != 1 || gpuResult != 1)
                {
                    int mode = Program.config.getConfig("performance_mode");
                    Logger.WriteLine("ASUS BIOS rejected fan curve, resetting mode to " + mode);
                    Program.wmi.DeviceSet(ASUSWmi.PerformanceMode, mode, "PerformanceMode");
                    LabelFansResult("ASUS BIOS rejected fan curve");
                }
                else
                {
                    LabelFansResult("");
                    customFans = true;
                }

                // fix for misbehaving bios on intell based TUF 2022
                if ((Program.config.ContainsModel("FX507") || Program.config.ContainsModel("FX517")) && Program.config.getConfigPerf("auto_apply_power") != 1)
                {
                    Task.Run(async () =>
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1));
                        Program.wmi.DeviceSet(ASUSWmi.PPT_TotalA0, 80, "PowerLimit Fix A0");
                        Program.wmi.DeviceSet(ASUSWmi.PPT_APUA3, 80, "PowerLimit Fix A3");
                    });
                }

            }

            Program.settingsForm.BeginInvoke(SetPerformanceLabel);

        }

        public void AutoPower(int delay = 0)
        {

            // fix for misbehaving bios PPTs on G513
            if (Program.config.ContainsModel("G513") && Program.config.getConfigPerf("auto_apply") != 1)
            {
                AutoFans(true);
                delay = 500;
            }

            customPower = 0;

            if (Program.config.getConfigPerf("auto_apply_power") == 1)
            {
                if (delay > 0)
                {
                    var timer = new System.Timers.Timer(1000);
                    timer.Elapsed += delegate
                    {
                        timer.Stop();
                        timer.Dispose();
                        SetPower();
                        SetGPUPower();
                    };
                    timer.Start();
                }
                else
                {
                    SetPower();
                    SetGPUPower();
                }

            }

        }


        public void SetPerformanceMode(int PerformanceMode = -1, bool notify = false)
        {

            if (PerformanceMode < 0)
                PerformanceMode = Program.config.getConfig("performance_mode");

            buttonSilent.Activated = false;
            buttonBalanced.Activated = false;
            buttonTurbo.Activated = false;

            switch (PerformanceMode)
            {
                case ASUSWmi.PerformanceSilent:
                    buttonSilent.Activated = true;
                    perfName = Properties.Strings.Silent;
                    break;
                case ASUSWmi.PerformanceTurbo:
                    buttonTurbo.Activated = true;
                    perfName = Properties.Strings.Turbo;
                    break;
                default:
                    buttonBalanced.Activated = true;
                    PerformanceMode = ASUSWmi.PerformanceBalanced;
                    perfName = Properties.Strings.Balanced;
                    break;
            }

            menuSilent.Checked = buttonSilent.Activated;
            menuBalanced.Checked = buttonBalanced.Activated;
            menuTurbo.Checked = buttonTurbo.Activated;

            int oldMode = Program.config.getConfig("performance_mode");
            Program.config.setConfig("performance_" + (int)SystemInformation.PowerStatus.PowerLineStatus, PerformanceMode);
            Program.config.setConfig("performance_mode", PerformanceMode);

            Program.wmi.DeviceSet(ASUSWmi.PerformanceMode, PerformanceMode, "PerformanceMode");

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

            AutoFans();
            AutoPower(1000);

            if (Program.config.getConfigPerfString("scheme") is not null)
                NativeMethods.SetPowerScheme(Program.config.getConfigPerfString("scheme"));
            else
                NativeMethods.SetPowerScheme(PerformanceMode);

            if (Program.config.getConfigPerf("auto_boost") != -1)
            {
                NativeMethods.SetCPUBoost(Program.config.getConfigPerf("auto_boost"));
            }

            if (NativeMethods.PowerGetEffectiveOverlayScheme(out Guid activeScheme) == 0)
            {
                Debug.WriteLine("Effective :" + activeScheme);
            }

            if (fans != null && fans.Text != "")
            {
                fans.InitFans();
                fans.InitPower();
                fans.InitBoost();
            }
        }


        public void CyclePerformanceMode()
        {
            int mode = Program.config.getConfig("performance_mode");

            if (Control.ModifierKeys == Keys.Shift)
                mode = (mode == 0) ? 2 : mode - 1;
            else
                mode++;

            SetPerformanceMode(mode, true);
        }


        public void AutoKeyboard()
        {
            if (Program.config.getConfig("keyboard_auto") != 1) return;

            if (SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online)
                Aura.ApplyBrightness(3);
            else
                Aura.ApplyBrightness(0);


        }

        public void AutoPerformance()
        {
            var Plugged = SystemInformation.PowerStatus.PowerLineStatus;

            int mode = Program.config.getConfig("performance_" + (int)Plugged);
            if (mode != -1)
                SetPerformanceMode(mode, true);
            else
                SetPerformanceMode(Program.config.getConfig("performance_mode"));
        }


        public void AutoScreen(bool force = false)
        {
            if (force || Program.config.getConfig("screen_auto") == 1)
            {
                if (SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online)
                    SetScreen(1000, 1);
                else
                    SetScreen(60, 0);
            } else
            {
                SetScreen(overdrive : Program.config.getConfig("overdrive")); 
            }



        }

        public static bool IsPlugged()
        {
            bool optimizedUSBC = Program.config.getConfig("optimized_usbc") != 1;

            return SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online &&
                   (optimizedUSBC || Program.wmi.DeviceGet(ASUSWmi.ChargerMode) != ASUSWmi.ChargerUSB);

        }

        public bool AutoGPUMode()
        {

            bool GpuAuto = Program.config.getConfig("gpu_auto") == 1;
            bool ForceGPU = Program.config.ContainsModel("503");

            int GpuMode = Program.config.getConfig("gpu_mode");

            if (!GpuAuto && !ForceGPU) return false;

            int eco = Program.wmi.DeviceGet(ASUSWmi.GPUEco);
            int mux = Program.wmi.DeviceGet(ASUSWmi.GPUMux);

            if (mux == 0) // GPU in Ultimate, ignore
                return false;
            else
            {

                if (ReEnableGPU()) return true;

                if (eco == 1)
                    if ((GpuAuto && IsPlugged()) || (ForceGPU && GpuMode == ASUSWmi.GPUModeStandard))
                    {
                        SetEcoGPU(0);
                        return true;
                    }
                if (eco == 0)
                    if ((GpuAuto && !IsPlugged()) || (ForceGPU && GpuMode == ASUSWmi.GPUModeEco))
                    {

                        if (HardwareControl.IsUsedGPU())
                        {
                            DialogResult dialogResult = MessageBox.Show(Properties.Strings.AlertDGPU, Properties.Strings.AlertDGPUTitle, MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.No) return false;
                        }

                        SetEcoGPU(1);
                        return true;
                    }
            }

            return false;

        }

        public bool ReEnableGPU()
        {
            if (Screen.AllScreens.Length <= 1) return false;
            if (Program.config.getConfig("gpu_reenable") != 1) return false;

            Logger.WriteLine("Re-enabling gpu for 503 model");

            Thread.Sleep(1000);
            SetEcoGPU(1);
            Thread.Sleep(1000);
            SetEcoGPU(0);
            return true;
        }

        private void UltimateUI(bool ultimate)
        {
            if (!ultimate)
            {
                tableGPU.Controls.Remove(buttonUltimate);
                tablePerf.ColumnCount = 0;
                tableGPU.ColumnCount = 0;
                tableScreen.ColumnCount = 0;
                menuUltimate.Visible = false;

            }
            //tableLayoutMatrix.ColumnCount = 0;
        }

        public void InitXGM()
        {
            int connected = Program.wmi.DeviceGet(ASUSWmi.GPUXGConnected);
            int enabled = Program.wmi.DeviceGet(ASUSWmi.GPUXG);

            buttonXGM.Enabled = buttonXGM.Visible = (connected == 1);
            buttonXGM.Activated = (enabled == 1);

        }


        public int InitGPUMode()
        {

            int eco = Program.wmi.DeviceGet(ASUSWmi.GPUEco);
            int mux = Program.wmi.DeviceGet(ASUSWmi.GPUMux);

            Logger.WriteLine("Eco flag : " + eco);
            Logger.WriteLine("Mux flag : " + mux);

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

            InitXGM();

            VisualiseGPUMode(GpuMode);

            return GpuMode;

        }


        public void SetEcoGPU(int eco)
        {

            ButtonEnabled(buttonOptimized, false);
            ButtonEnabled(buttonEco, false);
            ButtonEnabled(buttonStandard, false);
            ButtonEnabled(buttonUltimate, false);

            labelGPU.Text = Properties.Strings.GPUMode + ": " + Properties.Strings.GPUChanging + " ...";

            Thread t = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                if (eco == 1)
                {
                    string[] tokill = { "EADesktop", "RadeonSoftware" };
                    foreach (string kill in tokill)
                        foreach (var process in Process.GetProcessesByName(kill)) process.Kill();
                }

                Program.wmi.DeviceSet(ASUSWmi.GPUEco, eco, "GPUEco");

                if (eco == 0)
                    HardwareControl.RecreateGpuControlWithDelay();

                Program.settingsForm.BeginInvoke(delegate
                {
                    Thread.Sleep(500);
                    InitGPUMode();
                    AutoScreen();
                });
            });

            t.Start();

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
                DialogResult dialogResult = MessageBox.Show(Properties.Strings.AlertUltimateOff, Properties.Strings.AlertUltimateTitle, MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Program.wmi.DeviceSet(ASUSWmi.GPUMux, 1, "GPUMux");
                    restart = true;
                    changed = true;
                }
            }
            else if (GPUMode == ASUSWmi.GPUModeUltimate)
            {
                DialogResult dialogResult = MessageBox.Show(Properties.Strings.AlertUltimateOn, Properties.Strings.AlertUltimateTitle, MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Program.wmi.DeviceSet(ASUSWmi.GPUMux, 0, "GPUMux");
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
                    labelGPU.Text = Properties.Strings.GPUMode + ": " + Properties.Strings.GPUModeEco;
                    Program.trayIcon.Icon = Properties.Resources.eco;
                    break;
                case ASUSWmi.GPUModeUltimate:
                    buttonUltimate.Activated = true;
                    labelGPU.Text = Properties.Strings.GPUMode + ": " + Properties.Strings.GPUModeUltimate;
                    Program.trayIcon.Icon = Properties.Resources.ultimate;
                    break;
                default:
                    buttonOptimized.BorderColor = colorStandard;
                    buttonStandard.Activated = !GPUAuto;
                    buttonOptimized.Activated = GPUAuto;
                    labelGPU.Text = Properties.Strings.GPUMode + ": " + Properties.Strings.GPUModeStandard;
                    Program.trayIcon.Icon = Properties.Resources.standard;
                    break;
            }

            menuEco.Checked = buttonEco.Activated;
            menuStandard.Checked = buttonStandard.Activated;
            menuUltimate.Checked = buttonUltimate.Activated;
            menuOptimized.Checked = buttonOptimized.Activated;

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

        public void ButtonEnabled(RButton but, bool enabled)
        {
            but.Enabled = enabled;
            but.BackColor = but.Enabled ? Color.FromArgb(255, but.BackColor) : Color.FromArgb(100, but.BackColor);
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

            //Debug.WriteLine(limit);

            labelBatteryTitle.Text = Properties.Strings.BatteryChargeLimit + ": " + limit.ToString() + "%";
            sliderBattery.Value = limit;

            Program.wmi.DeviceSet(ASUSWmi.BatteryLimit, limit, "BatteryLimit");
            Program.config.setConfig("charge_limit", limit);

        }


    }


}
