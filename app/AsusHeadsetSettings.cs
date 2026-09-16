using GHelper.Peripherals;
using GHelper.Peripherals.Headset;
using GHelper.UI;

namespace GHelper
{
    public partial class AsusHeadsetSettings : RForm
    {
        private static readonly int[] sleepTimers = { 2, 3, 5, 10, 15, 0 };
        private static readonly int[] voicePrompts = { 1, 2, 0 };

        private static readonly Dictionary<HeadsetLightingMode, string> lightingModeNames = new Dictionary<HeadsetLightingMode, string>()
        {
            { HeadsetLightingMode.Off, Properties.Strings.Off },
            { HeadsetLightingMode.Static, Properties.Strings.AuraStatic },
            { HeadsetLightingMode.Breathing, Properties.Strings.AuraBreathe },
            { HeadsetLightingMode.Strobing, Properties.Strings.AuraStrobe },
            { HeadsetLightingMode.ColorCycle, Properties.Strings.AuraColorCycle },
            { HeadsetLightingMode.Rainbow, Properties.Strings.AuraRainbow },
            { HeadsetLightingMode.MqaIndicator, "MQA Indicator" },
        };

        private readonly AsusHeadset headset;
        private readonly Slider[] bandSliders;
        private readonly (string Name, byte[] Gains)[] presets;
        private readonly List<HeadsetLightingMode> lightingModes = new List<HeadsetLightingMode>();

        private bool loadingSettings = true;

        public AsusHeadsetSettings(AsusHeadset headset)
        {
            this.headset = headset;
            InitializeComponent();

            labelLighting.Text = Properties.Strings.Lighting;
            labelLightingMode.Text = Properties.Strings.AuraLightingMode;
            checkBoxSyncAura.Text = Properties.Strings.MouseSyncWithAura;
            labelAudio.Text = Properties.Strings.MatrixAudio;
            checkBoxEqualizer.Text = Properties.Strings.HeadsetEqualizer;
            checkBoxSidetone.Text = Properties.Strings.HeadsetSidetone;
            checkBoxNoiseReduction.Text = Properties.Strings.HeadsetNoiseReduction;
            labelMicrophoneType.Text = Properties.Strings.HeadsetMicrophoneType;
            labelVoicePrompt.Text = Properties.Strings.HeadsetVoicePrompt;
            labelAnc.Text = "ANC";
            labelAncMode.Text = "ANC";
            checkBoxAdaptiveAnc.Text = Properties.Strings.HeadsetAdaptiveAnc;
            checkBoxDirac.Text = "Dirac";
            labelEnergy.Text = Properties.Strings.Power;
            labelAutoPowerOff.Text = Properties.Strings.MouseAutoPowerOff;
            labelLowBatteryWarning.Text = Properties.Strings.MouseLowBatteryWarning;
            labelChargingState.Text = "(" + Properties.Strings.Charging + ")";
            buttonReset.Text = Properties.Strings.Reset;

            lightingModes.Add(HeadsetLightingMode.Off);
            lightingModes.AddRange(headset.SupportedLightingModes());
            foreach (var mode in lightingModes) comboBoxLightingMode.Items.Add(lightingModeNames[mode]);

            presets = headset.EqualizerPresets;
            foreach (var preset in presets) comboBoxEqualizerPreset.Items.Add(preset.Name);
            for (int i = 1; i <= AsusHeadset.CustomPresets; i++) comboBoxEqualizerPreset.Items.Add(Properties.Strings.Custom + " " + i);

            bandSliders = new Slider[] { sliderBand0, sliderBand1, sliderBand2, sliderBand3, sliderBand4,
                sliderBand5, sliderBand6, sliderBand7, sliderBand8, sliderBand9 };

            for (int i = 0; i < bandSliders.Length; i++)
            {
                int band = i;
                bandSliders[i].Max = headset.MaxEqualizerGain();
                bandSliders[i].MouseUp += (s, e) => BandChanged(band);
                bandSliders[i].KeyUp += (s, e) => BandChanged(band);
            }

            comboBoxMicrophoneType.Items.AddRange(new string[] {
                Properties.Strings.HeadsetMicBoom,
                Properties.Strings.HeadsetMicInline,
                Properties.Strings.HeadsetMicExternal,
            });

            comboBoxNoiseReduction.Items.AddRange(new string[] {
                Properties.Strings.Low,
                Properties.Strings.Medium,
                Properties.Strings.High,
            });

            comboBoxVoicePrompt.Items.AddRange(new string[] {
                "English",
                "Chinese",
                Properties.Strings.HeadsetPromptSound,
            });

            comboBoxAnc.Items.AddRange(new string[] {
                Properties.Strings.Off,
                Properties.Strings.On,
                Properties.Strings.HeadsetAmbient,
            });

            comboBoxAncLevel.Items.AddRange(new string[] {
                Properties.Strings.Auto,
                Properties.Strings.Low,
                Properties.Strings.Medium,
                Properties.Strings.High,
            });

            comboBoxAutoPowerOff.Items.AddRange(new string[] {
                " 2 " + Properties.Strings.Minutes,
                " 3 " + Properties.Strings.Minutes,
                " 5 " + Properties.Strings.Minutes,
                "10 " + Properties.Strings.Minutes,
                "15 " + Properties.Strings.Minutes,
                Properties.Strings.Never,
            });

            InitTheme(true);

            Text = headset.GetDisplayName();

            Shown += AsusHeadsetSettings_Shown;
            FormClosing += AsusHeadsetSettings_FormClosing;

            comboBoxLightingMode.DropDownClosed += ComboBoxLightingMode_DropDownClosed;
            sliderBrightness.MouseUp += SliderBrightness_MouseUp;
            sliderBrightness.KeyUp += SliderBrightness_MouseUp;
            buttonLightingColor.Click += ButtonLightingColor_Click;
            checkBoxSyncAura.CheckedChanged += CheckBoxSyncAura_CheckedChanged;

            checkBoxEqualizer.CheckedChanged += CheckBoxEqualizer_CheckedChanged;
            comboBoxEqualizerPreset.DropDownClosed += ComboBoxEqualizerPreset_DropDownClosed;
            checkBoxSidetone.CheckedChanged += CheckBoxSidetone_CheckedChanged;
            sliderSidetone.ValueChanged += SliderSidetone_ValueChanged;
            sliderSidetone.MouseUp += SliderSidetone_MouseUp;
            sliderSidetone.KeyUp += SliderSidetone_MouseUp;
            checkBoxNoiseReduction.CheckedChanged += CheckBoxNoiseReduction_CheckedChanged;
            comboBoxNoiseReduction.DropDownClosed += ComboBoxNoiseReduction_DropDownClosed;
            comboBoxMicrophoneType.DropDownClosed += ComboBoxMicrophoneType_DropDownClosed;
            comboBoxVoicePrompt.DropDownClosed += ComboBoxVoicePrompt_DropDownClosed;
            comboBoxAnc.DropDownClosed += ComboBoxAnc_DropDownClosed;
            checkBoxAdaptiveAnc.CheckedChanged += CheckBoxAdaptiveAnc_CheckedChanged;
            comboBoxAncLevel.DropDownClosed += ComboBoxAncLevel_DropDownClosed;
            checkBoxDirac.CheckedChanged += CheckBoxDirac_CheckedChanged;

            comboBoxAutoPowerOff.DropDownClosed += ComboBoxAutoPowerOff_DropDownClosed;
            sliderLowBatteryWarning.ValueChanged += SliderLowBatteryWarning_ValueChanged;
            sliderLowBatteryWarning.MouseUp += SliderLowBatteryWarning_MouseUp;
            sliderLowBatteryWarning.KeyUp += SliderLowBatteryWarning_MouseUp;

            buttonReset.Click += ButtonReset_Click;

            if (!headset.HasBattery()) panelBatteryState.Visible = false;
            if (!headset.HasRGB()) panelLighting.Visible = false;
            if (!headset.HasBrightness()) sliderBrightness.Visible = false;
            sliderSidetone.Max = headset.MaxSidetone();

            panelEqualizer.Visible = headset.HasEqualizer();
            panelEqualizerBands.Visible = headset.HasEqualizer();
            panelSidetone.Visible = headset.HasSidetone();
            panelNoiseReduction.Visible = headset.HasNoiseReduction();
            panelMicrophoneType.Visible = headset.HasMicrophoneType();
            panelVoicePrompt.Visible = headset.HasVoicePrompt();
            comboBoxNoiseReduction.Visible = headset.HasNoiseReductionLevel();

            panelAudio.Visible = headset.HasEqualizer() || headset.HasSidetone() || headset.HasNoiseReduction() || headset.HasVoicePrompt() || headset.HasMicrophoneType();
            panelAnc.Visible = headset.HasAnc();
            panelDirac.Visible = headset.HasDirac();
            if (!headset.HasPowerSettings()) panelEnergy.Visible = false;

            RefreshHeadsetData();

            loadingSettings = false;
        }

        private void AsusHeadsetSettings_Shown(object? sender, EventArgs e)
        {
            if (Height > Program.settingsForm.Height)
            {
                var top = Program.settingsForm.Top + Program.settingsForm.Height - Height;

                if (top < 0)
                {
                    MaximumSize = new Size(Width, Program.settingsForm.Height);
                    Top = Program.settingsForm.Top;
                }
                else
                {
                    Top = top;
                }
            }
            else
            {
                Top = Program.settingsForm.Top;
            }

            Left = Program.settingsForm.Left - Width - 5;

            headset.Disconnect += Headset_Disconnect;
            headset.BatteryUpdated += Headset_BatteryUpdated;
            headset.HeadsetReadyChanged += Headset_ReadyChanged;
        }

        private void AsusHeadsetSettings_FormClosing(object? sender, FormClosingEventArgs e)
        {
            headset.Disconnect -= Headset_Disconnect;
            headset.BatteryUpdated -= Headset_BatteryUpdated;
            headset.HeadsetReadyChanged -= Headset_ReadyChanged;
        }

        private void Headset_Disconnect(object? sender, EventArgs e)
        {
            if (Disposing || IsDisposed) return;
            Invoke(delegate { Close(); });
        }

        private void Headset_ReadyChanged(object? sender, EventArgs e)
        {
            if (Disposing || IsDisposed) return;
            if (!headset.IsDeviceReady) Invoke(delegate { Close(); });
        }

        private void Headset_BatteryUpdated(object? sender, EventArgs e)
        {
            if (Disposing || IsDisposed) return;
            Invoke(delegate { VisualizeBatteryState(); });
        }

        private void RefreshHeadsetData()
        {
            headset.SynchronizeDevice();

            if (!headset.IsDeviceReady)
            {
                Logger.WriteLine(headset.GetDisplayName() + " (GUI): Headset is not ready. Closing view.");
                Headset_Disconnect(this, EventArgs.Empty);
                return;
            }

            if (Disposing || IsDisposed) return;

            bool loading = loadingSettings;
            loadingSettings = true;
            VisualizeBatteryState();
            VisualizeLighting();
            VisualizeAudio();
            VisualizeEnergy();
            loadingSettings = loading;
        }

        private void VisualizeBatteryState()
        {
            if (!headset.HasBattery()) return;

            labelBatteryState.Text = headset.BatteryText();
            labelChargingState.Visible = headset.Charging;

            pictureBoxBatteryState.BackgroundImage = ControlHelper.TintImage(headset.Charging
                ? Properties.Resources.icons8_ladende_batterie_48
                : Properties.Resources.icons8_batterie_voll_geladen_48, foreMain);
        }

        private void VisualizeLighting()
        {
            if (!headset.HasRGB()) return;

            checkBoxSyncAura.Checked = PeripheralsProvider.IsHeadsetAuraSync;
            comboBoxLightingMode.SelectedIndex = Math.Max(0, lightingModes.IndexOf(headset.LightingMode));
            sliderBrightness.Value = headset.Brightness;
            buttonLightingColor.SwatchColor = headset.LightingColor;
            panelLightingColor.Visible = headset.LightingMode is HeadsetLightingMode.Static or HeadsetLightingMode.Breathing or HeadsetLightingMode.Strobing;
        }

        private void VisualizeAudio()
        {
            checkBoxEqualizer.Checked = headset.EqualizerEnabled;
            comboBoxEqualizerPreset.SelectedIndex = FindPreset();

            for (int i = 0; i < bandSliders.Length; i++)
                bandSliders[i].Value = headset.EqualizerGains[i];

            checkBoxSidetone.Checked = headset.SidetoneEnabled;
            sliderSidetone.Value = headset.Sidetone;
            labelSidetoneValue.Text = headset.Sidetone.ToString();

            checkBoxNoiseReduction.Checked = headset.NoiseReductionEnabled;
            comboBoxNoiseReduction.SelectedIndex = headset.NoiseReduction;
            comboBoxMicrophoneType.SelectedIndex = headset.MicrophoneType;

            comboBoxVoicePrompt.SelectedIndex = Array.IndexOf(voicePrompts, headset.VoicePrompt);
            comboBoxAnc.SelectedIndex = headset.AncMode;
            checkBoxAdaptiveAnc.Checked = headset.AdaptiveAnc;
            comboBoxAncLevel.SelectedIndex = headset.AncLevel;
            panelAncLevel.Enabled = headset.AncMode == 1;
            checkBoxDirac.Checked = headset.Dirac;
        }

        private void VisualizeEnergy()
        {
            comboBoxAutoPowerOff.SelectedIndex = Math.Max(0, Array.IndexOf(sleepTimers, headset.SleepTimer));
            sliderLowBatteryWarning.Value = Math.Min(headset.LowBatteryWarning, sliderLowBatteryWarning.Max);
            labelLowBatteryWarningValue.Text = sliderLowBatteryWarning.Value + "%";
        }

        private void ComboBoxLightingMode_DropDownClosed(object? sender, EventArgs e)
        {
            if (comboBoxLightingMode.SelectedIndex < 0) return;
            HeadsetLightingMode mode = lightingModes[comboBoxLightingMode.SelectedIndex];
            if (mode == headset.LightingMode) return;

            headset.SetLighting(mode, headset.HasBrightness() ? sliderBrightness.Value : headset.Brightness, headset.LightingColor);
            VisualizeLighting();
        }

        private void CheckBoxSyncAura_CheckedChanged(object? sender, EventArgs e)
        {
            if (loadingSettings) return;

            PeripheralsProvider.SetHeadsetAuraSync(checkBoxSyncAura.Checked);
            if (checkBoxSyncAura.Checked) PeripheralsProvider.SyncHeadsetsWithAura();
        }

        private void SliderBrightness_MouseUp(object? sender, EventArgs e)
        {
            if (headset.LightingMode == HeadsetLightingMode.Off) return;
            headset.SetLighting(headset.LightingMode, sliderBrightness.Value, headset.LightingColor);
        }

        private void ButtonLightingColor_Click(object? sender, EventArgs e)
        {
            RColorPicker colorDlg = new RColorPicker(headset.LightingColor);
            colorDlg.ColorChanged += c =>
            {
                headset.SetLighting(headset.LightingMode, sliderBrightness.Value, c);
                buttonLightingColor.SwatchColor = c;
            };
            colorDlg.ShowDialog(this);
        }

        private void CheckBoxEqualizer_CheckedChanged(object? sender, EventArgs e)
        {
            if (loadingSettings) return;
            headset.SetEqualizer(checkBoxEqualizer.Checked);
        }

        private byte[] PresetGains(int index)
        {
            return index < presets.Length ? presets[index].Gains : headset.GetCustomPreset(index - presets.Length + 1);
        }

        private int FindPreset()
        {
            for (int i = 0; i < presets.Length + AsusHeadset.CustomPresets; i++)
                if (PresetGains(i).SequenceEqual(headset.EqualizerGains)) return i;
            return -1;
        }

        private void ComboBoxEqualizerPreset_DropDownClosed(object? sender, EventArgs e)
        {
            int index = comboBoxEqualizerPreset.SelectedIndex;
            if (index < 0) return;

            headset.SetEqualizerGains(PresetGains(index));

            loadingSettings = true;
            checkBoxEqualizer.Checked = true;
            for (int i = 0; i < bandSliders.Length; i++)
                bandSliders[i].Value = headset.EqualizerGains[i];
            loadingSettings = false;
        }

        private void BandChanged(int band)
        {
            headset.SetEqualizerBand(band, bandSliders[band].Value);

            int index = comboBoxEqualizerPreset.SelectedIndex;
            loadingSettings = true;
            checkBoxEqualizer.Checked = true;
            if (index >= presets.Length) headset.SaveCustomPreset(index - presets.Length + 1, headset.EqualizerGains);
            else comboBoxEqualizerPreset.SelectedIndex = FindPreset();
            loadingSettings = false;
        }

        private void CheckBoxSidetone_CheckedChanged(object? sender, EventArgs e)
        {
            if (loadingSettings) return;
            headset.SetSidetone(checkBoxSidetone.Checked, sliderSidetone.Value);
        }

        private void SliderSidetone_ValueChanged(object? sender, EventArgs e)
        {
            labelSidetoneValue.Text = sliderSidetone.Value.ToString();
        }

        private void SliderSidetone_MouseUp(object? sender, EventArgs e)
        {
            headset.SetSidetone(checkBoxSidetone.Checked, sliderSidetone.Value);
        }

        private void CheckBoxNoiseReduction_CheckedChanged(object? sender, EventArgs e)
        {
            if (loadingSettings) return;
            headset.SetNoiseReduction(checkBoxNoiseReduction.Checked, Math.Max(0, comboBoxNoiseReduction.SelectedIndex));
        }

        private void ComboBoxNoiseReduction_DropDownClosed(object? sender, EventArgs e)
        {
            if (comboBoxNoiseReduction.SelectedIndex < 0) return;
            headset.SetNoiseReduction(checkBoxNoiseReduction.Checked, comboBoxNoiseReduction.SelectedIndex);
        }

        private void ComboBoxMicrophoneType_DropDownClosed(object? sender, EventArgs e)
        {
            if (comboBoxMicrophoneType.SelectedIndex < 0) return;
            headset.SetMicrophoneType(comboBoxMicrophoneType.SelectedIndex);
        }

        private void ComboBoxVoicePrompt_DropDownClosed(object? sender, EventArgs e)
        {
            if (comboBoxVoicePrompt.SelectedIndex < 0) return;
            headset.SetVoicePrompt(voicePrompts[comboBoxVoicePrompt.SelectedIndex]);
        }

        private void ComboBoxAnc_DropDownClosed(object? sender, EventArgs e)
        {
            if (comboBoxAnc.SelectedIndex < 0) return;
            headset.SetAnc(comboBoxAnc.SelectedIndex);
            panelAncLevel.Enabled = headset.AncMode == 1;
        }

        private void CheckBoxAdaptiveAnc_CheckedChanged(object? sender, EventArgs e)
        {
            if (loadingSettings) return;
            headset.SetAncLevel(checkBoxAdaptiveAnc.Checked, Math.Max(0, comboBoxAncLevel.SelectedIndex));
        }

        private void ComboBoxAncLevel_DropDownClosed(object? sender, EventArgs e)
        {
            if (comboBoxAncLevel.SelectedIndex < 0) return;
            headset.SetAncLevel(checkBoxAdaptiveAnc.Checked, comboBoxAncLevel.SelectedIndex);
        }

        private void CheckBoxDirac_CheckedChanged(object? sender, EventArgs e)
        {
            if (loadingSettings) return;
            headset.SetDirac(checkBoxDirac.Checked);
        }

        private void ComboBoxAutoPowerOff_DropDownClosed(object? sender, EventArgs e)
        {
            if (comboBoxAutoPowerOff.SelectedIndex < 0) return;
            headset.SetEnergySettings(sliderLowBatteryWarning.Value, sleepTimers[comboBoxAutoPowerOff.SelectedIndex]);
        }

        private void SliderLowBatteryWarning_ValueChanged(object? sender, EventArgs e)
        {
            labelLowBatteryWarningValue.Text = sliderLowBatteryWarning.Value + "%";
        }

        private void SliderLowBatteryWarning_MouseUp(object? sender, EventArgs e)
        {
            headset.SetEnergySettings(sliderLowBatteryWarning.Value, headset.SleepTimer);
        }

        private void ButtonReset_Click(object? sender, EventArgs e)
        {
            headset.ResetToDefaults();
            RefreshHeadsetData();
        }
    }
}
