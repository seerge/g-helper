using GHelper.Peripherals.Mouse;
using GHelper.UI;

namespace GHelper
{
    public partial class AsusMouseSettings : RForm
    {
        private static Dictionary<LightingMode, string> lightingModeNames = new Dictionary<LightingMode, string>()
        {
            { LightingMode.Static,Properties.Strings.AuraStatic},
            { LightingMode.Breathing, Properties.Strings.AuraBreathe},
            { LightingMode.ColorCycle, Properties.Strings.AuraColorCycle},
            { LightingMode.Rainbow, Properties.Strings.AuraRainbow},
            { LightingMode.React, Properties.Strings.AuraReact},
            { LightingMode.Comet, Properties.Strings.AuraComet},
            { LightingMode.BatteryState, Properties.Strings.AuraBatteryState},
            { LightingMode.Off, Properties.Strings.MatrixOff},
        };
        private List<LightingMode> supportedLightingModes = new List<LightingMode>();

        private readonly AsusMouse mouse;
        private readonly RButton[] dpiButtons;
        private LightingZone visibleZone = LightingZone.All;

        private bool updateMouseDPI = true;

        public AsusMouseSettings(AsusMouse mouse)
        {
            this.mouse = mouse;
            InitializeComponent();

            dpiButtons = new RButton[] { buttonDPI1, buttonDPI2, buttonDPI3, buttonDPI4 };


            labelPollingRate.Text = Properties.Strings.PollingRate;
            labelLighting.Text = Properties.Strings.Lighting;
            labelLightingMode.Text = Properties.Strings.AuraLightingMode;
            labelEnergy.Text = Properties.Strings.EnergySettings;
            labelPerformance.Text = Properties.Strings.MousePerformance;
            checkBoxRandomColor.Text = Properties.Strings.AuraRandomColor;
            labelLowBatteryWarning.Text = Properties.Strings.MouseLowBatteryWarning;
            labelAutoPowerOff.Text = Properties.Strings.MouseAutoPowerOff;
            buttonSync.Text = Properties.Strings.MouseSynchronize;
            checkBoxAngleSnapping.Text = Properties.Strings.MouseAngleSnapping;
            labelLiftOffDistance.Text = Properties.Strings.MouseLiftOffDistance;
            labelChargingState.Text = "(" + Properties.Strings.Charging + ")";
            labelProfile.Text = Properties.Strings.Profile;
            labelButtonDebounce.Text = Properties.Strings.MouseButtonResponse;
            labelAcceleration.Text = Properties.Strings.Acceleration;
            labelDeceleration.Text = Properties.Strings.Deceleration;

            buttonLightingZoneLogo.Text = Properties.Strings.AuraZoneLogo;
            buttonLightingZoneScroll.Text = Properties.Strings.AuraZoneScroll;
            buttonLightingZoneUnderglow.Text = Properties.Strings.AuraZoneUnderglow;
            buttonLightingZoneAll.Text = Properties.Strings.AuraZoneAll;
            buttonLightingZoneDock.Text = Properties.Strings.AuraZoneDock;

            buttonExport.Text = Properties.Strings.Export;
            buttonImport.Text = Properties.Strings.Import;

            InitTheme();

            this.Text = mouse.GetDisplayName();

            Shown += AsusMouseSettings_Shown;
            FormClosing += AsusMouseSettings_FormClosing;

            comboProfile.DropDownClosed += ComboProfile_DropDownClosed;

            sliderDPI.ValueChanged += SliderDPI_ValueChanged;
            numericUpDownCurrentDPI.ValueChanged += NumericUpDownCurrentDPI_ValueChanged;
            sliderDPI.MouseUp += SliderDPI_MouseUp;
            sliderDPI.MouseDown += SliderDPI_MouseDown;
            buttonDPIColor.Click += ButtonDPIColor_Click;
            buttonDPI1.Click += ButtonDPI_Click;
            buttonDPI2.Click += ButtonDPI_Click;
            buttonDPI3.Click += ButtonDPI_Click;
            buttonDPI4.Click += ButtonDPI_Click;

            comboBoxPollingRate.DropDownClosed += ComboBoxPollingRate_DropDownClosed;
            checkBoxAngleSnapping.CheckedChanged += CheckAngleSnapping_CheckedChanged;
            sliderAngleAdjustment.ValueChanged += SliderAngleAdjustment_ValueChanged;
            sliderAngleAdjustment.MouseUp += SliderAngleAdjustment_MouseUp;
            comboBoxLiftOffDistance.DropDownClosed += ComboBoxLiftOffDistance_DropDownClosed;
            sliderButtonDebounce.ValueChanged += SliderButtonDebounce_ValueChanged;
            sliderButtonDebounce.MouseUp += SliderButtonDebounce_MouseUp;

            sliderAcceleration.MouseUp += SliderAcceleration_MouseUp;
            sliderAcceleration.ValueChanged += SliderAcceleration_ValueChanged;

            sliderDeceleration.MouseUp += SliderDeceleration_MouseUp;
            sliderDeceleration.ValueChanged += SliderDeceleration_ValueChanged;

            buttonLightingColor.Click += ButtonLightingColor_Click;
            comboBoxLightingMode.DropDownClosed += ComboBoxLightingMode_DropDownClosed;
            sliderBrightness.MouseUp += SliderBrightness_MouseUp;
            comboBoxAnimationSpeed.DropDownClosed += ComboBoxAnimationSpeed_DropDownClosed;
            comboBoxAnimationDirection.DropDownClosed += ComboBoxAnimationDirection_DropDownClosed;
            checkBoxRandomColor.CheckedChanged += CheckBoxRandomColor_CheckedChanged;

            sliderLowBatteryWarning.ValueChanged += SliderLowBatteryWarning_ValueChanged;
            sliderLowBatteryWarning.MouseUp += SliderLowBatteryWarning_MouseUp;
            comboBoxAutoPowerOff.DropDownClosed += ComboBoxAutoPowerOff_DropDownClosed;


            buttonLightingZoneAll.Click += ButtonLightingZoneAll_Click;
            buttonLightingZoneDock.Click += ButtonLightingZoneDock_Click;
            buttonLightingZoneLogo.Click += ButtonLightingZoneLogo_Click;
            buttonLightingZoneUnderglow.Click += ButtonLightingZoneUnderglow_Click;
            buttonLightingZoneScroll.Click += ButtonLightingZoneScroll_Click;

            InitMouseCapabilities();
            Logger.WriteLine(mouse.GetDisplayName() + " (GUI): Initialized capabilities. Synchronizing mouse data");
            RefreshMouseData();
        }

        private void SliderAcceleration_MouseUp(object? sender, MouseEventArgs e)
        {
            mouse.SetAcceleration(sliderAcceleration.Value);
        }

        private void SliderAcceleration_ValueChanged(object? sender, EventArgs e)
        {
            labelAccelerationValue.Text = sliderAcceleration.Value.ToString();
        }

        private void SliderDeceleration_MouseUp(object? sender, MouseEventArgs e)
        {
            mouse.SetDeceleration(sliderDeceleration.Value);
        }

        private void SliderDeceleration_ValueChanged(object? sender, EventArgs e)
        {
            labelDecelerationValue.Text = sliderDeceleration.Value.ToString();
        }

        private void SliderButtonDebounce_MouseUp(object? sender, MouseEventArgs e)
        {
            DebounceTime dbt = (DebounceTime)sliderButtonDebounce.Value;
            mouse.SetDebounce(dbt);
        }

        private void SliderButtonDebounce_ValueChanged(object? sender, EventArgs e)
        {
            DebounceTime dbt = (DebounceTime)sliderButtonDebounce.Value;
            int time = mouse.DebounceTimeInMS(dbt);

            labelButtonDebounceValue.Text = time + "ms";
        }

        private void SwitchLightingZone(LightingZone zone)
        {
            if (!mouse.HasRGB())
            {
                return;
            }
            visibleZone = zone;
            InitLightingModes();
            VisusalizeLightingSettings();
        }

        private void ButtonLightingZoneScroll_Click(object? sender, EventArgs e)
        {
            SwitchLightingZone(LightingZone.Scrollwheel);
        }

        private void ButtonLightingZoneUnderglow_Click(object? sender, EventArgs e)
        {
            SwitchLightingZone(LightingZone.Underglow);
        }

        private void ButtonLightingZoneLogo_Click(object? sender, EventArgs e)
        {
            SwitchLightingZone(LightingZone.Logo);
        }

        private void ButtonLightingZoneDock_Click(object? sender, EventArgs e)
        {
            SwitchLightingZone(LightingZone.Dock);
        }

        private void ButtonLightingZoneAll_Click(object? sender, EventArgs e)
        {
            SwitchLightingZone(LightingZone.All);
        }

        private void AsusMouseSettings_FormClosing(object? sender, FormClosingEventArgs e)
        {
            mouse.BatteryUpdated -= Mouse_BatteryUpdated;
            mouse.Disconnect -= Mouse_Disconnect;
            mouse.MouseReadyChanged -= Mouse_MouseReadyChanged;
        }

        private void Mouse_MouseReadyChanged(object? sender, EventArgs e)
        {
            if (Disposing || IsDisposed)
            {
                return;
            }
            if (!mouse.IsDeviceReady)
            {
                this.Invoke(delegate
                {
                    Close();
                });
            }
        }

        private void Mouse_BatteryUpdated(object? sender, EventArgs e)
        {
            if (Disposing || IsDisposed)
            {
                return;
            }
            this.Invoke(delegate
            {
                VisualizeBatteryState();
            });

        }

        private void ComboProfile_DropDownClosed(object? sender, EventArgs e)
        {
            if (mouse.Profile == comboProfile.SelectedIndex)
            {
                return;
            }

            mouse.SetProfile(comboProfile.SelectedIndex);
            RefreshMouseData();
        }

        private void ComboBoxPollingRate_DropDownClosed(object? sender, EventArgs e)
        {
            mouse.SetPollingRate(mouse.SupportedPollingrates()[comboBoxPollingRate.SelectedIndex]);
        }

        private void ButtonDPIColor_Click(object? sender, EventArgs e)
        {
            ColorDialog colorDlg = new ColorDialog
            {
                AllowFullOpen = true,
                Color = pictureDPIColor.BackColor
            };

            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                AsusMouseDPI dpi = mouse.DpiSettings[mouse.DpiProfile - 1];
                dpi.Color = colorDlg.Color;

                mouse.SetDPIForProfile(dpi, mouse.DpiProfile);

                VisualizeDPIButtons();
                VisualizeCurrentDPIProfile();
            }
        }

        private void ButtonDPI_Click(object? sender, EventArgs e)
        {
            int index = -1;

            for (int i = 0; i < dpiButtons.Length; ++i)
            {
                if (sender == dpiButtons[i])
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                //huh?
                return;
            }

            mouse.SetDPIProfile(index + 1);
            VisualizeDPIButtons();
            VisualizeCurrentDPIProfile();
        }

        private void UpdateLightingSettings(LightingSetting settings, LightingZone zone)
        {
            mouse.SetLightingSetting(settings, visibleZone);
            VisusalizeLightingSettings();
        }

        private void CheckBoxRandomColor_CheckedChanged(object? sender, EventArgs e)
        {
            LightingSetting? ls = mouse.LightingSettingForZone(visibleZone);
            ls.RandomColor = checkBoxRandomColor.Checked;

            UpdateLightingSettings(ls, visibleZone);
        }

        private void ComboBoxAnimationDirection_DropDownClosed(object? sender, EventArgs e)
        {
            LightingSetting? ls = mouse.LightingSettingForZone(visibleZone);
            ls.AnimationDirection = (AnimationDirection)comboBoxAnimationDirection.SelectedIndex;

            UpdateLightingSettings(ls, visibleZone);
        }

        private void ComboBoxAnimationSpeed_DropDownClosed(object? sender, EventArgs e)
        {
            LightingSetting? ls = mouse.LightingSettingForZone(visibleZone);
            // 0 => 0x9
            // 1 => 0x7
            // 2 => 0x5
            ls.AnimationSpeed = (AnimationSpeed)(0x9 - (comboBoxAnimationSpeed.SelectedIndex * 0x2));
            UpdateLightingSettings(ls, visibleZone);
        }

        private void SliderBrightness_MouseUp(object? sender, MouseEventArgs e)
        {
            LightingSetting? ls = mouse.LightingSettingForZone(visibleZone);
            ls.Brightness = sliderBrightness.Value;

            UpdateLightingSettings(ls, visibleZone);
        }

        private void ComboBoxLightingMode_DropDownClosed(object? sender, EventArgs e)
        {
            if (!mouse.HasRGB())
            {
                return;
            }

            var index = comboBoxLightingMode.SelectedIndex;
            LightingMode lm = supportedLightingModes[(index >= 0 && index < supportedLightingModes.Count) ? index : 0 ];

            LightingSetting? ls = mouse.LightingSettingForZone(visibleZone);
            if (ls.LightingMode == lm)
            {
                //Nothing to do here.
                return;
            }

            ls.LightingMode = lm;

            UpdateLightingSettings(ls, visibleZone);
        }

        private void ButtonLightingColor_Click(object? sender, EventArgs e)
        {
            ColorDialog colorDlg = new ColorDialog
            {
                AllowFullOpen = true,
                Color = pictureBoxLightingColor.BackColor
            };

            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                LightingSetting? ls = mouse.LightingSettingForZone(visibleZone);
                ls.RGBColor = colorDlg.Color;

                UpdateLightingSettings(ls, visibleZone);
            }
        }

        private void SliderLowBatteryWarning_ValueChanged(object? sender, EventArgs e)
        {
            labelLowBatteryWarningValue.Text = sliderLowBatteryWarning.Value.ToString() + "%";
        }

        private void SliderLowBatteryWarning_MouseUp(object? sender, MouseEventArgs e)
        {
            mouse.SetEnergySettings(sliderLowBatteryWarning.Value, mouse.PowerOffSetting);
        }


        private void ComboBoxAutoPowerOff_DropDownClosed(object? sender, EventArgs e)
        {
            object? obj = Enum.GetValues(typeof(PowerOffSetting)).GetValue(comboBoxAutoPowerOff.SelectedIndex);
            if (obj is null)
            {
                return;
            }
            PowerOffSetting pos = (PowerOffSetting)obj;


            mouse.SetEnergySettings(mouse.LowBatteryWarning, pos);
        }

        private void SliderAngleAdjustment_ValueChanged(object? sender, EventArgs e)
        {
            labelAngleAdjustmentValue.Text = sliderAngleAdjustment.Value.ToString() + "°";
        }

        private void SliderAngleAdjustment_MouseUp(object? sender, MouseEventArgs e)
        {
            mouse.SetAngleAdjustment((short)sliderAngleAdjustment.Value);
        }

        private void ComboBoxLiftOffDistance_DropDownClosed(object? sender, EventArgs e)
        {
            mouse.SetLiftOffDistance((LiftOffDistance)comboBoxLiftOffDistance.SelectedIndex);
        }

        private void CheckAngleSnapping_CheckedChanged(object? sender, EventArgs e)
        {
            mouse.SetAngleSnapping(checkBoxAngleSnapping.Checked);
            mouse.SetAngleAdjustment((short)sliderAngleAdjustment.Value);
        }

        private void SliderDPI_ValueChanged(object? sender, EventArgs e)
        {
            numericUpDownCurrentDPI.Value = sliderDPI.Value;
            UpdateMouseDPISettings();
        }

        private void NumericUpDownCurrentDPI_ValueChanged(object? sender, EventArgs e)
        {
            sliderDPI.Value = (int)numericUpDownCurrentDPI.Value;
        }

        private void SliderDPI_MouseDown(object? sender, MouseEventArgs e)
        {
            updateMouseDPI = false;
        }

        private void SliderDPI_MouseUp(object? sender, MouseEventArgs e)
        {
            updateMouseDPI = true;
            UpdateMouseDPISettings();
        }

        private void UpdateMouseDPISettings()
        {
            if (!updateMouseDPI)
            {
                return;
            }
            AsusMouseDPI dpi = mouse.DpiSettings[mouse.DpiProfile - 1];
            dpi.DPI = (uint)sliderDPI.Value;

            mouse.SetDPIForProfile(dpi, mouse.DpiProfile);

            VisualizeDPIButtons();
            VisualizeCurrentDPIProfile();
        }

        private void Mouse_Disconnect(object? sender, EventArgs e)
        {
            if (Disposing || IsDisposed)
            {
                return;
            }
            //Mouse disconnected. Bye bye.
            this.Invoke(delegate
            {
                this.Close();
            });

        }

        private void RefreshMouseData()
        {
            mouse.SynchronizeDevice();

            Logger.WriteLine(mouse.GetDisplayName() + " (GUI): Mouse data synchronized");
            if (!mouse.IsDeviceReady)
            {
                Logger.WriteLine(mouse.GetDisplayName() + " (GUI): Mouse is not ready. Closing view.");
                Mouse_Disconnect(this, EventArgs.Empty);
                return;
            }

            if (Disposing || IsDisposed)
            {
                return;
            }

            VisualizeMouseSettings();
            VisualizeBatteryState();
        }

        private void InitMouseCapabilities()
        {
            for (int i = 0; i < mouse.ProfileCount(); ++i)
            {
                String prf = Properties.Strings.Profile + " " + (i + 1);
                comboProfile.Items.Add(prf);
            }

            labelMinDPI.Text = mouse.MinDPI().ToString();
            labelMaxDPI.Text = mouse.MaxDPI().ToString();

            sliderDPI.Max = mouse.MaxDPI();
            sliderDPI.Min = mouse.MinDPI();
            sliderDPI.Step = mouse.DPIIncrements();

            numericUpDownCurrentDPI.Minimum = mouse.MinDPI();
            numericUpDownCurrentDPI.Maximum = mouse.MaxDPI();
            numericUpDownCurrentDPI.Increment = mouse.DPIIncrements();

            if (!mouse.HasDebounceSetting())
            {
                panelDebounce.Visible = false;
            }


            if (!mouse.HasDPIColors())
            {
                buttonDPIColor.Visible = false;
                pictureDPIColor.Visible = false;
                buttonDPI1.Image = ControlHelper.TintImage(Properties.Resources.lighting_dot_24, Color.Red);
                buttonDPI2.Image = ControlHelper.TintImage(Properties.Resources.lighting_dot_24, Color.Purple);
                buttonDPI3.Image = ControlHelper.TintImage(Properties.Resources.lighting_dot_24, Color.Blue);
                buttonDPI4.Image = ControlHelper.TintImage(Properties.Resources.lighting_dot_24, Color.Green);

                buttonDPI1.BorderColor = Color.Red;
                buttonDPI2.BorderColor = Color.Purple;
                buttonDPI3.BorderColor = Color.Blue;
                buttonDPI4.BorderColor = Color.Green;
            }

            if (mouse.CanSetPollingRate())
            {
                foreach (PollingRate pr in mouse.SupportedPollingrates())
                {
                    comboBoxPollingRate.Items.Add(mouse.PollingRateDisplayString(pr));
                }

            }
            else
            {
                panelPollingRate.Visible = false;
            }

            if (!mouse.HasAngleSnapping())
            {
                checkBoxAngleSnapping.Visible = false;
            }

            if (!mouse.HasAngleTuning())
            {
                labelAngleAdjustmentValue.Visible = false;
                sliderAngleAdjustment.Visible = false;
                sliderAngleAdjustment.Max = mouse.AngleTuningMax();
                sliderAngleAdjustment.Min = mouse.AngleTuningMin();
                sliderAngleAdjustment.Step = mouse.AngleTuningStep();
            }

            if (!mouse.HasAngleTuning() && !mouse.HasAngleSnapping())
            {
                panelAngleSnapping.Visible = false;
            }

            if (mouse.HasAcceleration())
            {
                sliderAcceleration.Max = mouse.MaxAcceleration();
            }
            else
            {
                panelAcceleration.Visible = false;
            }

            if (mouse.HasDeceleration())
            {
                sliderDeceleration.Max = mouse.MaxDeceleration();
            }
            else
            {
                panelDeceleration.Visible = false;
            }

            if (mouse.HasLiftOffSetting())
            {
                comboBoxLiftOffDistance.Items.AddRange(new string[] {
                    Properties.Strings.Low,
                    Properties.Strings.High,
                });
            }
            else
            {
                panelLiftOffDistance.Visible = false;
            }

            if (mouse.DPIProfileCount() < 4)
            {
                for (int i = 3; i > mouse.DPIProfileCount() - 1; --i)
                {
                    dpiButtons[i].Visible = false;
                }
            }

            if (!mouse.HasBattery())
            {
                panelBatteryState.Visible = false;
            }

            if (mouse.HasAutoPowerOff())
            {
                comboBoxAutoPowerOff.Items.AddRange(new string[]{
                    " 1 "+ Properties.Strings.Minute,
                    " 2 "+ Properties.Strings.Minutes,
                    " 3 "+ Properties.Strings.Minutes,
                    " 5 "+ Properties.Strings.Minutes,
                    "10 "+ Properties.Strings.Minutes,
                     Properties.Strings.Never,
                });
            }

            if (!mouse.HasLowBatteryWarning())
            {
                labelLowBatteryWarning.Visible = false;
                labelLowBatteryWarningValue.Visible = false;
                sliderLowBatteryWarning.Visible = false;
            }
            else
            {
                sliderLowBatteryWarning.Min = 0;
                sliderLowBatteryWarning.Step = mouse.LowBatteryWarningStep();
                sliderLowBatteryWarning.Max = mouse.LowBatteryWarningMax();
            }

            if (!mouse.HasAutoPowerOff() && !mouse.HasLowBatteryWarning())
            {
                panelEnergy.Visible = false;
            }

            if (mouse.HasRGB())
            {
                if (mouse.SupportedLightingZones().Length > 1)
                {
                    buttonLightingZoneLogo.Visible = mouse.SupportedLightingZones().Contains(LightingZone.Logo);
                    buttonLightingZoneScroll.Visible = mouse.SupportedLightingZones().Contains(LightingZone.Scrollwheel);
                    buttonLightingZoneUnderglow.Visible = mouse.SupportedLightingZones().Contains(LightingZone.Underglow);
                    buttonLightingZoneDock.Visible = mouse.SupportedLightingZones().Contains(LightingZone.Dock);
                }
                else
                {
                    buttonLightingZoneLogo.Visible = false;
                    buttonLightingZoneScroll.Visible = false;
                    buttonLightingZoneUnderglow.Visible = false;
                    buttonLightingZoneDock.Visible = false;
                }

                sliderBrightness.Max = mouse.MaxBrightness();

                InitLightingModes();

                comboBoxAnimationDirection.Items.AddRange(new string[] {
                    Properties.Strings.AuraClockwise,
                    Properties.Strings.AuraCounterClockwise,
                });

                comboBoxAnimationSpeed.Items.AddRange(new string[] {
                    Properties.Strings.AuraSlow,
                    Properties.Strings.AuraNormal,
                    Properties.Strings.AuraFast
                });
            }
            else
            {
                panelLighting.Visible = false;
            }
        }

        private void InitLightingModes()
        {
            comboBoxLightingMode.Items.Clear();
            supportedLightingModes.Clear();
            foreach (LightingMode lm in Enum.GetValues(typeof(LightingMode)))
            {
                if (mouse.IsLightingModeSupported(lm) && mouse.IsLightingModeSupportedForZone(lm, visibleZone))
                {
                    comboBoxLightingMode.Items.Add(lightingModeNames.GetValueOrDefault(lm));
                    supportedLightingModes.Add(lm);
                }
            }
        }


        private void VisualizeMouseSettings()
        {
            if (mouse.Profile < comboProfile.Items.Count)
                comboProfile.SelectedIndex = mouse.Profile;

            if (mouse.HasRGB())
            {
                //If current lighting mode is zoned, pre-select the first zone and not "All".
                bool zoned = mouse.IsLightingZoned();
                if (zoned)
                {
                    visibleZone = mouse.SupportedLightingZones()[0];
                    InitLightingModes();
                }
            }


            VisualizeDPIButtons();
            VisualizeCurrentDPIProfile();
            VisusalizeLightingSettings();

            if (mouse.CanSetPollingRate())
            {
                int idx = mouse.PollingRateIndex(mouse.PollingRate);
                if (idx == -1)
                {
                    return;
                }
                comboBoxPollingRate.SelectedIndex = idx;
            }

            if (mouse.HasAngleSnapping())
            {
                checkBoxAngleSnapping.Checked = mouse.AngleSnapping;
            }

            if (mouse.HasAngleTuning())
            {
                sliderAngleAdjustment.Value = mouse.AngleAdjustmentDegrees;
            }

            if (mouse.HasAutoPowerOff())
            {
                if (mouse.PowerOffSetting == PowerOffSetting.Never)
                {
                    comboBoxAutoPowerOff.SelectedIndex = comboBoxAutoPowerOff.Items.Count - 1;
                }
                else
                {
                    comboBoxAutoPowerOff.SelectedIndex = (int)mouse.PowerOffSetting;
                }
            }

            if (mouse.HasLowBatteryWarning())
            {
                sliderLowBatteryWarning.Value = mouse.LowBatteryWarning;
            }

            if (mouse.HasLiftOffSetting())
            {
                comboBoxLiftOffDistance.SelectedIndex = (int)mouse.LiftOffDistance;
            }

            if (mouse.HasDebounceSetting())
            {
                sliderButtonDebounce.Value = (int)mouse.Debounce;
            }

            if (mouse.HasAcceleration())
            {
                sliderAcceleration.Value = mouse.Acceleration;
            }

            if (mouse.HasDeceleration())
            {
                sliderDeceleration.Value = mouse.Deceleration;
            }
        }

        private void VisualizeBatteryState()
        {
            if (!mouse.HasBattery())
            {
                return;
            }

            labelBatteryState.Text = mouse.Battery + "%";
            labelChargingState.Visible = mouse.Charging;

            if (mouse.Charging)
            {
                pictureBoxBatteryState.BackgroundImage = ControlHelper.TintImage(Properties.Resources.icons8_ladende_batterie_48, foreMain);
            }
            else
            {
                pictureBoxBatteryState.BackgroundImage = ControlHelper.TintImage(Properties.Resources.icons8_batterie_voll_geladen_48, foreMain);
            }
        }

        public void VisusalizeLightingZones()
        {
            bool zoned = mouse.IsLightingZoned();

            buttonLightingZoneAll.Activated = visibleZone == LightingZone.All;
            buttonLightingZoneLogo.Activated = visibleZone == LightingZone.Logo;
            buttonLightingZoneScroll.Activated = visibleZone == LightingZone.Scrollwheel;
            buttonLightingZoneUnderglow.Activated = visibleZone == LightingZone.Underglow;
            buttonLightingZoneDock.Activated = visibleZone == LightingZone.Dock;

            buttonLightingZoneAll.Secondary = zoned;
            buttonLightingZoneLogo.Secondary = !zoned;
            buttonLightingZoneScroll.Secondary = !zoned;
            buttonLightingZoneUnderglow.Secondary = !zoned;
            buttonLightingZoneDock.Secondary = !zoned;

            buttonLightingZoneAll.BackColor = buttonLightingZoneAll.Secondary ? RForm.buttonSecond : RForm.buttonMain;
            buttonLightingZoneLogo.BackColor = buttonLightingZoneLogo.Secondary ? RForm.buttonSecond : RForm.buttonMain;
            buttonLightingZoneScroll.BackColor = buttonLightingZoneScroll.Secondary ? RForm.buttonSecond : RForm.buttonMain;
            buttonLightingZoneUnderglow.BackColor = buttonLightingZoneUnderglow.Secondary ? RForm.buttonSecond : RForm.buttonMain;
            buttonLightingZoneDock.BackColor = buttonLightingZoneDock.Secondary ? RForm.buttonSecond : RForm.buttonMain;
        }

        private void VisusalizeLightingSettings()
        {
            if (!mouse.HasRGB())
            {
                return;
            }

            VisusalizeLightingZones();

            LightingSetting? ls = mouse.LightingSettingForZone(visibleZone);

            if (ls is null)
            {
                //Lighting settings not loaded?
                return;
            }

            sliderBrightness.Value = ls.Brightness;

            checkBoxRandomColor.Visible = mouse.SupportsRandomColor(ls.LightingMode);

            pictureBoxLightingColor.Visible = mouse.SupportsColorSetting(ls.LightingMode);
            buttonLightingColor.Visible = mouse.SupportsColorSetting(ls.LightingMode);

            comboBoxAnimationSpeed.Visible = mouse.SupportsAnimationSpeed(ls.LightingMode);
            labelAnimationSpeed.Visible = mouse.SupportsAnimationSpeed(ls.LightingMode);
            comboBoxAnimationDirection.Visible = mouse.SupportsAnimationDirection(ls.LightingMode);
            labelAnimationDirection.Visible = mouse.SupportsAnimationDirection(ls.LightingMode);

            comboBoxLightingMode.SelectedIndex = supportedLightingModes.IndexOf(ls.LightingMode);

            if (mouse.SupportsRandomColor(ls.LightingMode))
            {
                checkBoxRandomColor.Checked = ls.RandomColor;
                buttonLightingColor.Visible = !ls.RandomColor;
            }

            if (ls.RandomColor && mouse.SupportsRandomColor(ls.LightingMode))
                pictureBoxLightingColor.BackColor = Color.Transparent;
            else
                pictureBoxLightingColor.BackColor = ls.RGBColor;

            //0x09 => 0
            //0x07 => 1
            //0x05 => 2
            comboBoxAnimationSpeed.SelectedIndex = 2 - ((((int)ls.AnimationSpeed) - 5) / 2);
            comboBoxAnimationDirection.SelectedIndex = (int)ls.AnimationDirection;
        }


        private void VisualizeDPIButtons()
        {
            for (int i = 0; i < mouse.DPIProfileCount() && i < 4; ++i)
            {
                AsusMouseDPI dpi = mouse.DpiSettings[i];
                if (dpi is null)
                {
                    continue;
                }
                if (mouse.HasDPIColors())
                {
                    dpiButtons[i].Image = ControlHelper.TintImage(Properties.Resources.lighting_dot_24, dpi.Color);
                    dpiButtons[i].BorderColor = dpi.Color;
                }
                dpiButtons[i].Activated = (mouse.DpiProfile - 1) == i;
                dpiButtons[i].Text = "DPI " + (i + 1) + "\n" + dpi.DPI;
            }
        }


        private void VisualizeCurrentDPIProfile()
        {
            if (mouse.DpiProfile > mouse.DpiSettings.Length)
            {
                Logger.WriteLine($"Wrong mouse DPI: {mouse.DpiProfile}");
                return;
            }

            AsusMouseDPI dpi;

            try
            {
                dpi = mouse.DpiSettings[mouse.DpiProfile - 1];
            } catch (Exception ex)
            {
                Logger.WriteLine($"Wrong mouse DPI: {mouse.DpiProfile} {mouse.DpiSettings.Length} {ex.Message}");
                return;
            }

            if (dpi is null)
            {
                return;
            }
            sliderDPI.Value = (int)dpi.DPI;
            pictureDPIColor.BackColor = dpi.Color;
        }

        private void AsusMouseSettings_Shown(object? sender, EventArgs e)
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


            mouse.Disconnect += Mouse_Disconnect;
            mouse.BatteryUpdated += Mouse_BatteryUpdated;
            mouse.MouseReadyChanged += Mouse_MouseReadyChanged;
        }

        private void ButtonSync_Click(object sender, EventArgs e)
        {
            RefreshMouseData();
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            byte[] data = null;

            Thread t = new Thread(() =>
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "G-Helper Mouse Profile V1 (*.gmp1)|*.gmp1";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    data = File.ReadAllBytes(ofd.FileName);
                }
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            if (data == null)
            {
                //User aborted loading
                return;
            }

            if (!mouse.Import(data))
            {
                Logger.WriteLine("Failed to import mouse profile");
                MessageBox.Show(Properties.Strings.MouseImportFailed);
            }
            else
            {
                if (!mouse.IsLightingZoned())
                {
                    visibleZone = LightingZone.All;
                }

                RefreshMouseData();
            }
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(() =>
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "G-Helper Mouse Profile V1 (*.gmp1)|*.gmp1";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(sfd.FileName, mouse.Export());
                }
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
        }
    }
}
