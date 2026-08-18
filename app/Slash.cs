using GHelper.AnimeMatrix;
using GHelper.UI;

namespace GHelper
{
    public partial class Slash : RForm
    {
        SlashDevice? slash = Program.settingsForm.matrixControl.deviceSlash;

        static readonly int[] dimLevels = { 10, 20, 30, 40, 50, 100 };

        public Slash()
        {
            InitializeComponent();
            InitTheme(true);

            labelPower.Text = Properties.Strings.Power;
            labelAnimations.Text = Properties.Strings.Animations;
            checkAutoOff.Text = Properties.Strings.TurnOffOnBattery;
            checkLidOff.Text = Properties.Strings.DisableOnLidClose;
            checkBoot.Text = Properties.Strings.SlashBootAnimation;
            checkSleepAnimation.Text = Properties.Strings.SlashSleepAnimation;
            checkLowBattery.Text = Properties.Strings.SlashLowBatteryAlert;
            checkBatteryLevel.Text = Properties.Strings.SlashBatteryIndicator;
            checkPowerSaving.Text = Properties.Strings.SlashDimOnLowBattery;

            checkAutoOff.Checked = AppConfig.Is("matrix_auto");
            checkLidOff.Checked = AppConfig.Is("matrix_lid");

            sliderInterval.Value = Math.Min(AppConfig.Get("matrix_interval", 0), sliderInterval.Max);
            VisualiseInterval();

            comboSleepPattern.DropDownStyle = ComboBoxStyle.DropDownList;
            comboSleepPattern.Items.AddRange(new object[] { Properties.Strings.SystemDefault, Properties.Strings.AnimationPattern });

            comboDim.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (int level in dimLevels) comboDim.Items.Add(level == 100 ? Properties.Strings.Always : $"{level} %");
            comboDim.SelectedIndex = Math.Max(0, Array.IndexOf(dimLevels, AppConfig.Get("slash_dim", 20)));

            LoadFlags();

            checkAutoOff.CheckedChanged += CheckAutoOff_CheckedChanged;
            checkLidOff.CheckedChanged += CheckLidOff_CheckedChanged;

            sliderInterval.ValueChanged += (sender, e) => VisualiseInterval();
            sliderInterval.MouseUp += SliderInterval_Commit;
            sliderInterval.KeyUp += SliderInterval_Commit;

            checkBoot.CheckedChanged += (sender, e) => { SetFlag(0xA0, checkBoot.Checked); SetFlag(0xA4, checkBoot.Checked); };
            checkSleepAnimation.CheckedChanged += CheckSleepAnimation_CheckedChanged;
            comboSleepPattern.DropDownClosed += ComboSleepPattern_DropDownClosed;
            checkLowBattery.CheckedChanged += (sender, e) => SetFlag(0xA2, checkLowBattery.Checked);
            checkBatteryLevel.CheckedChanged += CheckBatteryLevel_CheckedChanged;
            checkPowerSaving.CheckedChanged += CheckPowerSaving_CheckedChanged;
            comboDim.DropDownClosed += ComboDim_DropDownClosed;
        }

        private void LoadFlags()
        {
            if (slash is null) return;
            try
            {
                checkBoot.Checked = slash.GetFlag(0xA0);
                var sleep = slash.GetRecord(0xA1);
                if (sleep is not null)
                {
                    checkSleepAnimation.Checked = sleep[8] == 0x01;
                    comboSleepPattern.SelectedIndex = sleep[6] == 0x00 ? 0 : 1;
                }
                checkLowBattery.Checked = slash.GetFlag(0xA2);
                checkBatteryLevel.Checked = slash.GetFlag(0xA3);
                checkPowerSaving.Checked = slash.GetFlag(0xA8);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
            }
        }

        private void SetFlag(byte region, bool status)
        {
            try
            {
                slash?.SetFlag(region, status);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
            }
        }

        private void CheckAutoOff_CheckedChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("matrix_auto", checkAutoOff.Checked ? 1 : 0);
            try
            {
                slash?.SetLightingOnBattery(!checkAutoOff.Checked);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
            }
        }

        private void CheckLidOff_CheckedChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("matrix_lid", checkLidOff.Checked ? 1 : 0);
            try
            {
                slash?.SetLightingOnLidClose(!checkLidOff.Checked);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
            }
        }

        private void VisualiseInterval()
        {
            labelInterval.Text = sliderInterval.Value == 0
                ? Properties.Strings.IntervalOff
                : string.Format(Properties.Strings.IntervalSeconds, sliderInterval.Value);
        }

        private void SliderInterval_Commit(object? sender, EventArgs e)
        {
            if (AppConfig.Get("matrix_interval", 0) == sliderInterval.Value) return;
            AppConfig.Set("matrix_interval", sliderInterval.Value);
            Program.settingsForm.matrixControl.SetDevice();
        }

        private void CheckSleepAnimation_CheckedChanged(object? sender, EventArgs e)
        {
            SetFlag(0xA1, checkSleepAnimation.Checked);
        }

        private void ComboSleepPattern_DropDownClosed(object? sender, EventArgs e)
        {
            try
            {
                byte pattern = comboSleepPattern.SelectedIndex == 0
                    ? (byte)0x00
                    : SlashDevice.GetModeCode((SlashMode)AppConfig.Get("matrix_running", 0));

                slash?.SetRecordByte(0xA1, 6, pattern);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
            }
        }

        private void CheckBatteryLevel_CheckedChanged(object? sender, EventArgs e)
        {
            try
            {
                slash?.SetBatteryAnimation(checkBatteryLevel.Checked);
                slash?.SetFlag(0xA5, checkBatteryLevel.Checked);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
            }
        }

        private void CheckPowerSaving_CheckedChanged(object? sender, EventArgs e)
        {
            try
            {
                slash?.SetPowerSaving(checkPowerSaving.Checked, AppConfig.Get("slash_dim", 20));
                slash?.SetFlag(0xA8, checkPowerSaving.Checked);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
            }
        }

        private void ComboDim_DropDownClosed(object? sender, EventArgs e)
        {
            int level = dimLevels[Math.Max(0, comboDim.SelectedIndex)];
            AppConfig.Set("slash_dim", level);
            try
            {
                slash?.SetPowerSaving(checkPowerSaving.Checked, level);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
            }
        }

        public void FormPosition()
        {
            Top = Program.settingsForm.Top;
            Left = Program.settingsForm.Left - Width - 5;
        }
    }
}
