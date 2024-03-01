using GHelper.AutoTDP.FramerateSource;
using GHelper.UI;
using Ryzen;

namespace GHelper.AutoTDP
{
    public partial class AutoTDPUI : RForm
    {
        private AutoTDPGameProfileUI? profileUI;
        public AutoTDPUI()
        {
            InitializeComponent();

            InitTheme();

            checkBoxEnabled.CheckedChanged += CheckBoxEnabled_CheckedChanged;
            buttonAddGame.Click += ButtonAddGame_Click;

            comboBoxLimiter.DropDownClosed += ComboBoxLimiter_DropDownClosed;

            comboBoxFPSSource.DropDownClosed += ComboBoxFPSSource_DropDownClosed;

            Shown += AutoTDPUI_Shown;

            VisualizeGeneralSettings();
            VizualizeGameList();
        }

        private void ComboBoxFPSSource_DropDownClosed(object? sender, EventArgs e)
        {
            if ((comboBoxFPSSource.SelectedItem as string).StartsWith("Riva"))
            {
                AppConfig.Set("auto_tdp_fps_source", "rtss");
            }
        }

        private void ComboBoxLimiter_DropDownClosed(object? sender, EventArgs e)
        {
            if ((comboBoxLimiter.SelectedItem as string).StartsWith("Intel"))
            {
                AppConfig.Set("auto_tdp_limiter", "intel_msr");
            }


            if ((comboBoxLimiter.SelectedItem as string).StartsWith("ASUS ACPI"))
            {
                AppConfig.Set("auto_tdp_limiter", "asus_acpi");
            }
        }

        private void CheckBoxEnabled_CheckedChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("auto_tdp_enabled", checkBoxEnabled.Checked ? 1 : 0);

            if (Program.autoTDPService.IsEnabled())
            {
                Program.autoTDPService.Start();
            }
            else
            {
                Program.autoTDPService.Shutdown();
            }
        }

        private void ButtonAddGame_Click(object? sender, EventArgs e)
        {
            string? path = null;
            Thread t = new Thread(() =>
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Executables (*.exe)|*.exe";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    path = ofd.FileName;
                }
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            if (path is null)
            {
                //User did not select a file
                return;
            }

            GameProfile p = new GameProfile();
            p.ProcessName = Path.GetFileName(path);
            p.GameTitle = Path.GetFileName(path);
            p.Enabled = true;
            p.TargetFPS = 60;
            p.MaxTdp = 40;
            p.MinTdp = 20;

            profileUI = new AutoTDPGameProfileUI(p, this);
            profileUI.TopMost = true;
            profileUI.FormClosed += ProfileUI_FormClosed;
            profileUI.Show();
        }

        private void ProfileUI_FormClosed(object? sender, FormClosedEventArgs e)
        {
            profileUI = null;
        }

        private void AutoTDPUI_Shown(object? sender, EventArgs e)
        {
            if (Height > Program.settingsForm.Height)
            {
                Top = Program.settingsForm.Top + Program.settingsForm.Height - Height;
            }
            else
            {
                Top = Program.settingsForm.Top;
            }

            Left = Program.settingsForm.Left - Width - 5;
        }

        private void VisualizeGeneralSettings()
        {
            checkBoxEnabled.Checked = AppConfig.Get("auto_tdp_enabled", 0) == 1;

            if (!RyzenControl.IsAMD())
                comboBoxLimiter.Items.Add("Intel MSR Power Limiter");


            comboBoxLimiter.Items.Add("ASUS ACPI Power Limiter");

            string? limiter = AppConfig.GetString("auto_tdp_limiter");

            if (comboBoxLimiter.Items.Count > 0 && limiter is null)
                comboBoxLimiter.SelectedIndex = 0;

            if (!RyzenControl.IsAMD() && limiter is not null && limiter.Equals("intel_msr"))
            {
                comboBoxLimiter.SelectedIndex = 0;
            }

            if (limiter is not null && limiter.Equals("asus_acpi"))
            {
                comboBoxLimiter.SelectedIndex = !RyzenControl.IsAMD() ? 1 : 0;
            }



            if (RTSSFramerateSource.IsAvailable())
                comboBoxFPSSource.Items.Add("Riva Tuner Statistics Server");


            string? source = AppConfig.GetString("auto_tdp_fps_source", null);

            if (comboBoxFPSSource.Items.Count > 0 && source is null)
                comboBoxFPSSource.SelectedIndex = 0;

            if (source is not null && source.Equals("rtss"))
            {
                comboBoxFPSSource.SelectedIndex = 0;
            }
        }


        private void VizualizeGameList()
        {
            //Due to my lousy skills in UI design, the game table is limited to 7x4 games.
            buttonAddGame.Enabled = Program.autoTDPService.GameProfiles.Count < 7 * 4;

            tableLayoutGames.Controls.Clear();

            foreach (GameProfile gp in Program.autoTDPService.GameProfiles)
            {
                RButton bt = new RButton();
                bt.Text = gp.GameTitle + "\n" + gp.TargetFPS + " FPS";

                bt.Dock = DockStyle.Fill;
                bt.FlatStyle = FlatStyle.Flat;
                bt.FlatAppearance.BorderColor = RForm.borderMain;
                bt.UseVisualStyleBackColor = false;
                bt.AutoSize = true;
                bt.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                bt.BackColor = RForm.buttonMain;
                bt.ForeColor = RForm.foreMain;
                bt.Click += Bt_Click;
                bt.Tag = gp;

                tableLayoutGames.Controls.Add(bt);
            }
        }

        private void Bt_Click(object? sender, EventArgs e)
        {
            GameProfile gp = (GameProfile)((RButton)sender).Tag;
            profileUI = new AutoTDPGameProfileUI(gp, this);
            profileUI.TopMost = true;
            profileUI.FormClosed += ProfileUI_FormClosed;
            profileUI.Show();
        }

        public void DeleteGameProfile(GameProfile gp)
        {
            if (Program.autoTDPService.IsGameInList(gp.ProcessName))
            {
                Program.autoTDPService.GameProfiles.Remove(gp);
            }

            Program.autoTDPService.SaveGameProfiles();
            VizualizeGameList();
        }

        public void UpdateGameProfile(GameProfile gp)
        {
            if (!Program.autoTDPService.IsGameInList(gp.ProcessName))
            {
                Program.autoTDPService.GameProfiles.Add(gp);
            }
            Program.autoTDPService.SaveGameProfiles();
            VizualizeGameList();
        }
    }
}
