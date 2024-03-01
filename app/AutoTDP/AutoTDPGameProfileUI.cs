using GHelper.UI;

namespace GHelper.AutoTDP
{
    public partial class AutoTDPGameProfileUI : RForm
    {
        public GameProfile GameProfile;
        private AutoTDPUI AutoTDPUI;

        public AutoTDPGameProfileUI(GameProfile profile, AutoTDPUI parent)
        {
            AutoTDPUI = parent;
            GameProfile = profile;
            InitializeComponent();

            sliderMinTDP.ValueChanged += SliderMinTDP_ValueChanged;
            sliderMaxTDP.ValueChanged += SliderMaxTDP_ValueChanged;
            buttonSave.Click += ButtonSave_Click;
            buttonDelete.Click += ButtonDelete_Click;

            InitTheme();


            Shown += AutoTDPGameProfileUI_Shown;

            VisualizeGameProfile();
        }

        private void ButtonDelete_Click(object? sender, EventArgs e)
        {
            AutoTDPUI.DeleteGameProfile(GameProfile);
            Close();
        }

        private void ButtonSave_Click(object? sender, EventArgs e)
        {
            GameProfile.Enabled = checkBoxEnabled.Checked;
            GameProfile.GameTitle = textBoxTitle.Text;
            GameProfile.TargetFPS = ((int)numericUpDownFPS.Value);
            GameProfile.MinTdp = sliderMinTDP.Value;
            GameProfile.MaxTdp = sliderMaxTDP.Value;

            AutoTDPUI.UpdateGameProfile(GameProfile);

            Close();
        }

        private void SliderMaxTDP_ValueChanged(object? sender, EventArgs e)
        {
            labelMaxTDP.Text = sliderMaxTDP.Value + "W";
            if (sliderMaxTDP.Value < sliderMinTDP.Value)
            {
                sliderMinTDP.Value = sliderMaxTDP.Value;
            }
        }

        private void SliderMinTDP_ValueChanged(object? sender, EventArgs e)
        {
            labelMinTDP.Text = sliderMinTDP.Value + "W";
            if (sliderMaxTDP.Value > sliderMinTDP.Value)
            {
                sliderMaxTDP.Value = sliderMinTDP.Value;
            }
        }

        private void AutoTDPGameProfileUI_Shown(object? sender, EventArgs e)
        {
            if (Height > Program.settingsForm.Height)
            {
                Top = Program.settingsForm.Top + Program.settingsForm.Height - Height;
            }
            else
            {
                Top = Program.settingsForm.Top + 60;
            }

            Left = Program.settingsForm.Left - Width - ((AutoTDPUI.Width - Width) / 2);
        }

        private void VisualizeGameProfile()
        {
            sliderMinTDP.Value = GameProfile.MinTdp;
            sliderMaxTDP.Value = GameProfile.MaxTdp;
            numericUpDownFPS.Value = GameProfile.TargetFPS;
            textBoxProcessName.Text = GameProfile.ProcessName;
            textBoxTitle.Text = GameProfile.GameTitle;
            checkBoxEnabled.Checked = GameProfile.Enabled;
        }
    }
}
