using GHelper.Peripherals;
using GHelper.Peripherals.Keyboard;
using GHelper.Peripherals.Keyboard.Models;
using GHelper.Peripherals.Mouse;
using GHelper.UI;
using GHelper.USB;

namespace GHelper
{
    public partial class AsusKeyboardSettings : RForm
    {
        private static Dictionary<KeyboardLightingMode, string> lightingModeNames = new Dictionary<KeyboardLightingMode, string>()
        {
            { KeyboardLightingMode.Static, Properties.Strings.AuraStatic },
            { KeyboardLightingMode.Breathing, Properties.Strings.AuraBreathe },
            { KeyboardLightingMode.ColorCycle, Properties.Strings.AuraColorCycle },
            { KeyboardLightingMode.Reactive, Properties.Strings.AuraReact },
            { KeyboardLightingMode.Wave, Properties.Strings.AuraRainbow },
            { KeyboardLightingMode.Ripple, "Ripple" },
            { KeyboardLightingMode.StarryNight, "Starry Night" },
            { KeyboardLightingMode.Quicksand, "Quicksand" },
            { KeyboardLightingMode.Current, "Current" },
            { KeyboardLightingMode.RainDrop, "Rain Drop" },
            { KeyboardLightingMode.Direct, "Custom" },
        };

        private static readonly Color[] paletteColors =
        {
            Color.FromArgb(0, 0, 0), Color.FromArgb(255, 255, 255),
            Color.FromArgb(255, 0, 0), Color.FromArgb(255, 128, 0), Color.FromArgb(255, 255, 0),
            Color.FromArgb(128, 255, 0), Color.FromArgb(0, 255, 0), Color.FromArgb(0, 255, 255),
            Color.FromArgb(0, 128, 255), Color.FromArgb(0, 0, 255), Color.FromArgb(128, 0, 255),
            Color.FromArgb(255, 0, 255), Color.FromArgb(255, 0, 128), Color.FromArgb(128, 128, 128),
        };

        private readonly AsusKeyboard keyboard;
        private Azoth? oled => keyboard as Azoth;
        private readonly List<KeyboardLightingMode> supportedModes;

        private Color[] keyColors = Array.Empty<Color>();
        private readonly List<Button> keyButtons = new();
        private readonly Dictionary<Button, int> keyLedSpan = new();
        private readonly Dictionary<Button, ushort> keyUsage = new();
        private int keyBorderSize = 2;

        private Color paintColor;

        private Button? selectedKey;
        private bool updatingBindings;

        private readonly ToolTip keyToolTip = new();

        // test-only layout previewer (keyboard_test mode): reopen the form for any layout
        public static Action<AsusKeyboard>? RequestReopen;
        private bool testLayoutSelector;

        private readonly System.Windows.Forms.Timer previewTimer = new() { Interval = 120 };
        private float previewPhase;
        private float[] keyGlow = Array.Empty<float>();
        private Color[] keyGlowColor = Array.Empty<Color>();
        private float rippleX, rippleY, rippleR;
        private readonly Random previewRandom = new();

        private bool loadingSettings = true;
        private bool settingsChanged = false;

        public AsusKeyboardSettings(AsusKeyboard keyboard)
        {
            this.keyboard = keyboard;
            supportedModes = new List<KeyboardLightingMode>(keyboard.SupportedLightingModes());

            try { keyboard.ReadProfile(); } catch { }

            InitializeComponent();

            Text = keyboard.GetDisplayName();
            labelLighting.Text = Properties.Strings.Lighting;
            labelLightingMode.Text = Properties.Strings.AuraLightingMode;
            labelAnimationSpeed.Text = Properties.Strings.AnimationSpeed;
            checkBoxSyncAura.Text = Properties.Strings.MouseSyncWithAura;
            buttonLightingColor.Text = Properties.Strings.Color;
            buttonLightingColor2.Text = Properties.Strings.Color + " 2";
            buttonLightingColor3.Text = "Back";
            buttonPaintColor.Text = Properties.Strings.Color;
            labelKeyBinding.Text = Properties.Strings.KeyBindings;
            labelProfile.Text = Properties.Strings.Profile;
            buttonResetBindings.Text = "  " + Properties.Strings.Reset;

            foreach (var mode in supportedModes)
                comboBoxLightingMode.Items.Add(lightingModeNames.TryGetValue(mode, out var name) ? name : mode.ToString());

            foreach (var speed in Aura.GetSpeeds().Values)
                comboBoxAnimationSpeed.Items.Add(speed);

            sliderBrightness.Max = keyboard.MaxBrightness();

            paintColor = keyboard.StoredColor;

            if (keyboard.HasPerKeyRGB())
            {
                BuildPalette();
                BuildKeyGrid();
            }

            SetupEnergy();
            SetupOled();
            if (keyboard.TestMode && keyboard.HasPerKeyRGB()) BuildTestLayoutSelector();
            BuildTopRow();
            LayoutSections();

            InitTheme(true);
            VisualizeBatteryState();
            keyboard.BatteryUpdated += Keyboard_BatteryUpdated;
            keyboard.Disconnect += Keyboard_Disconnect;

            LoadSettings();
            loadingSettings = false;

            checkBoxSyncAura.CheckedChanged += CheckBoxSyncAura_CheckedChanged;
            comboBoxLightingMode.DropDownClosed += (_, _) => ApplySettings();
            comboBoxAnimationSpeed.DropDownClosed += (_, _) => ApplySettings();
            sliderBrightness.MouseUp += SliderBrightness_MouseUp;
            sliderBrightness.KeyUp += SliderBrightness_MouseUp;
            buttonLightingColor.Click += (_, _) => PickColor(buttonLightingColor);
            buttonLightingColor2.Click += (_, _) => PickColor(buttonLightingColor2);
            buttonLightingColor3.Click += (_, _) => PickColor(buttonLightingColor3);
            buttonPaintColor.Click += ButtonPaintColor_Click;
            buttonFillAll.Click += ButtonFillAll_Click;
            buttonResetBindings.Click += ButtonResetBindings_Click;

            comboBoxKeyBinding.UseCustomTextPadding = false;
            comboBoxKeyBinding.DrawMode = DrawMode.OwnerDrawFixed;
            comboBoxKeyBinding.ItemHeight = comboBoxProfile.ItemHeight;
            comboBoxKeyBinding.DrawItem += RComboBox.DrawBindingItem;
            comboBoxKeyBinding.Items.AddRange(BuildBindingComboItems());
            comboBoxKeyBinding.SelectedIndexChanged += BindingCombo_Changed;
            textBoxKeyPath.TextChanged += TextBoxKeyPath_Changed;

            if (keyboard.HasProfiles())
            {
                comboBoxProfile.Items.Add("Default");
                for (int i = 1; i < keyboard.ProfileCount(); i++)
                    comboBoxProfile.Items.Add(Properties.Strings.Profile + " " + i);
                comboBoxProfile.SelectedIndex = Math.Clamp(keyboard.Profile, 0, comboBoxProfile.Items.Count - 1);
                comboBoxProfile.DropDownClosed += ComboBoxProfile_DropDownClosed;
            }

            previewTimer.Tick += PreviewTimer_Tick;

            Shown += AsusKeyboardSettings_Shown;
            FormClosing += AsusKeyboardSettings_FormClosing;
        }

        private void UpdatePreviewTimer()
        {
            KeyboardLightingMode mode = SelectedMode();
            bool animated = keyboard.HasPerKeyRGB() && mode is not KeyboardLightingMode.Static and not KeyboardLightingMode.Direct;
            if (animated) previewTimer.Start();
            else previewTimer.Stop();
        }

        private float PreviewStep()
        {
            return (AuraSpeed)Math.Max(0, comboBoxAnimationSpeed.SelectedIndex) switch
            {
                AuraSpeed.Slow => 0.02f,
                AuraSpeed.Fast => 0.09f,
                _ => 0.045f,
            };
        }

        private static float Tri(float t) => t < 0.5f ? t * 2 : 2 - t * 2;

        private static Color Dim(Color c, float f) => Color.FromArgb((int)(c.R * f), (int)(c.G * f), (int)(c.B * f));

        private static Color Lerp(Color a, Color b, float t) =>
            Color.FromArgb((int)(a.R + (b.R - a.R) * t), (int)(a.G + (b.G - a.G) * t), (int)(a.B + (b.B - a.B) * t));

        private static Color Hsv(float hue)
        {
            float h = (hue % 1f + 1f) % 1f * 6f;
            int i = (int)h;
            float f = h - i;
            int q = (int)(255 * (1 - f)), t = (int)(255 * f);
            return i switch
            {
                0 => Color.FromArgb(255, t, 0),
                1 => Color.FromArgb(q, 255, 0),
                2 => Color.FromArgb(0, 255, t),
                3 => Color.FromArgb(0, q, 255),
                4 => Color.FromArgb(t, 0, 255),
                _ => Color.FromArgb(255, 0, q),
            };
        }

        private static void SetKeyBorder(Button key, Color c)
        {
            if (key.FlatAppearance.BorderColor != c) key.FlatAppearance.BorderColor = c;
        }

        private void PreviewTimer_Tick(object? sender, EventArgs e)
        {
            if (keyButtons.Count == 0) return;

            previewPhase = (previewPhase + PreviewStep() + 1f) % 1f;
            KeyboardLightingMode mode = SelectedMode();
            Color c1 = buttonLightingColor.SwatchColor ?? Color.Red;
            Color c2 = buttonLightingColor2.SwatchColor ?? Color.Black;
            Color c2eff = (c2.R != 0 || c2.G != 0 || c2.B != 0) ? c2 : c1;
            Color bg = keyboard.SupportsColor3Setting(mode) ? (buttonLightingColor3.SwatchColor ?? Color.Black) : Color.Black;
            float w = Math.Max(1, panelKeys.Width);
            float h = Math.Max(1, panelKeys.Height);

            if (mode is KeyboardLightingMode.Reactive or KeyboardLightingMode.StarryNight or KeyboardLightingMode.Current or KeyboardLightingMode.RainDrop)
            {
                for (int i = 0; i < keyGlow.Length; i++) keyGlow[i] = Math.Max(0, keyGlow[i] - 0.09f);
                int ignite = mode == KeyboardLightingMode.StarryNight ? 3 : 1;
                for (int i = 0; i < ignite; i++)
                    if (previewRandom.NextDouble() < 0.7)
                    {
                        int idx = previewRandom.Next(keyGlow.Length);
                        keyGlow[idx] = 1f;
                        keyGlowColor[idx] = previewRandom.Next(2) == 0 ? c1 : c2eff;
                    }
            }

            if (mode == KeyboardLightingMode.Ripple)
            {
                rippleR += 0.06f;
                if (rippleR > 1.3f)
                {
                    rippleR = 0;
                    rippleX = (float)previewRandom.NextDouble();
                    rippleY = (float)previewRandom.NextDouble();
                }
            }

            for (int i = 0; i < keyButtons.Count; i++)
            {
                Button key = keyButtons[i];
                float kx = (key.Left + key.Width / 2f) / w;
                float ky = (key.Top + key.Height / 2f) / h;

                Color c = mode switch
                {
                    KeyboardLightingMode.Breathing => Lerp(c2, c1, Tri(previewPhase)),
                    KeyboardLightingMode.ColorCycle => Hsv(previewPhase),
                    KeyboardLightingMode.Wave => Hsv(previewPhase + kx),
                    KeyboardLightingMode.Quicksand => Hsv(ky - previewPhase),
                    KeyboardLightingMode.Ripple => RippleColor(c1, kx, ky),
                    KeyboardLightingMode.Reactive or KeyboardLightingMode.StarryNight or KeyboardLightingMode.Current or KeyboardLightingMode.RainDrop
                        => Lerp(bg, keyGlowColor[i], keyGlow[i]),
                    _ => c1,
                };
                SetKeyBorder(key, c);
            }
        }

        private Color RippleColor(Color c1, float kx, float ky)
        {
            float dist = (float)Math.Sqrt((kx - rippleX) * (kx - rippleX) + (ky - rippleY) * (ky - rippleY));
            float band = Math.Abs(dist - rippleR);
            return Dim(c1, band < 0.12f ? 0.15f + 0.85f * (1 - band / 0.12f) : 0.15f);
        }

        private void SetupEnergy()
        {
            labelEnergy.Text = Properties.Strings.EnergySettings;
            labelAutoPowerOff.Text = Properties.Strings.MouseAutoPowerOff;
            labelLowBatteryWarning.Text = Properties.Strings.MouseLowBatteryWarning;

            if (keyboard.HasAutoPowerOff())
                comboBoxAutoPowerOff.Items.AddRange(new string[] {
                    " 1 " + Properties.Strings.Minute, " 2 " + Properties.Strings.Minutes, " 3 " + Properties.Strings.Minutes,
                    " 5 " + Properties.Strings.Minutes, "10 " + Properties.Strings.Minutes, Properties.Strings.Never,
                });

            sliderLowBatteryWarning.Max = keyboard.LowBatteryWarningMax();
            sliderLowBatteryWarning.Step = keyboard.LowBatteryWarningStep();

            comboBoxAutoPowerOff.DropDownClosed += ComboBoxAutoPowerOff_DropDownClosed;
            sliderLowBatteryWarning.ValueChanged += (_, _) => labelLowBatteryWarningValue.Text = sliderLowBatteryWarning.Value == 0 ? Properties.Strings.Never : sliderLowBatteryWarning.Value + "%";
            sliderLowBatteryWarning.MouseUp += (_, _) => ApplyEnergySettings(sliderLowBatteryWarning.Value, keyboard.PowerOffSetting);
            sliderLowBatteryWarning.KeyUp += (_, _) => ApplyEnergySettings(sliderLowBatteryWarning.Value, keyboard.PowerOffSetting);
        }

        // index 0 = OLED off, the rest are the built-in animations
        private void SetupOled()
        {
            if (oled is null) return;

            comboBoxOledMode.Items.Add(Properties.Strings.Off);
            for (int i = 1; i <= oled.OledAnimationCount(); i++)
                comboBoxOledMode.Items.Add("Animation " + i);
            comboBoxOledMode.Items.Add("Clock");

            sliderOledBrightness.MouseUp += (_, _) => oled.SetOledBrightness(sliderOledBrightness.Value);
            sliderOledBrightness.KeyUp += (_, _) => oled.SetOledBrightness(sliderOledBrightness.Value);
            comboBoxOledMode.DropDownClosed += (_, _) =>
            {
                if (loadingSettings || comboBoxOledMode.SelectedIndex < 0) return;

                int index = comboBoxOledMode.SelectedIndex;
                if (index == 0)
                {
                    oled.SetOledClock(false);
                    oled.SetOledEnabled(false);
                    return;
                }

                if (!oled.OledEnabled) oled.SetOledEnabled(true);

                if (index > oled.OledAnimationCount())
                {
                    oled.SetOledClock(true);
                }
                else
                {
                    oled.SetOledClock(false);
                    oled.SetOledAnimation(index - 1);
                }
            };
        }

        private void VisualizeOled()
        {
            if (oled is null) return;

            sliderOledBrightness.Value = oled.OledBrightness < 0
                ? sliderOledBrightness.Max
                : Math.Clamp(oled.OledBrightness, sliderOledBrightness.Min, sliderOledBrightness.Max);

            if (!oled.OledEnabled)
            {
                comboBoxOledMode.SelectedIndex = 0;
                return;
            }

            if (oled.OledClock)
            {
                comboBoxOledMode.SelectedIndex = comboBoxOledMode.Items.Count - 1;
                return;
            }

            int current = oled.ReadOledAnimation();
            if (current < 0) current = Math.Max(oled.OledAnimation, 0);
            comboBoxOledMode.SelectedIndex = current + 1;
        }

        private void ApplyEnergySettings(int lowBatteryWarning, PowerOffSetting powerOff)
        {
            Task.Run(() => { try { keyboard.SetEnergySettings(lowBatteryWarning, powerOff); } catch { } });
        }

        // never reads control.Visible, it returns false before the form is shown
        private void LayoutSections()
        {
            bool grid = keyboard.HasPerKeyRGB();
            bool custom = grid && SelectedMode() == KeyboardLightingMode.Direct;
            bool bindings = grid && keyboard.HasKeyBindings();
            bool sleep = keyboard.HasAutoPowerOff();
            bool warn = keyboard.HasLowBatteryWarning();

            int gap = comboBoxLightingMode.Height / 2;
            int bottom = comboBoxAnimationSpeed.Bottom + gap;

            if (testLayoutSelector)
            {
                labelTestLayout.Top = comboBoxTestLayout.Top = bottom;
                bottom = comboBoxTestLayout.Bottom + gap;
            }

            panelKeysHeader.Visible = panelKeys.Visible = grid;
            buttonPaintColor.Visible = panelPalette.Visible = buttonFillAll.Visible = custom;
            labelKeyBinding.Visible = comboBoxKeyBinding.Visible = buttonResetBindings.Visible = bindings;

            if (grid)
            {
                panelKeysHeader.Top = bottom;
                bottom = panelKeysHeader.Bottom + gap;

                if (custom)
                {
                    buttonPaintColor.Top = bottom;
                    panelPalette.Top = bottom;
                    buttonFillAll.Top = bottom;
                    bottom = buttonPaintColor.Bottom + gap;
                }

                panelKeys.Top = bottom;
                bottom = panelKeys.Bottom + gap;

                if (bindings)
                {
                    labelKeyBinding.Top = comboBoxKeyBinding.Top = textBoxKeyPath.Top = bottom;
                    buttonResetBindings.Top = bottom + (comboBoxKeyBinding.Height - buttonResetBindings.Height) / 2;
                    bottom = comboBoxKeyBinding.Bottom + gap;
                }
            }

            panelEnergyHeader.Visible = sleep || warn;
            labelAutoPowerOff.Visible = comboBoxAutoPowerOff.Visible = sleep;
            labelLowBatteryWarning.Visible = sliderLowBatteryWarning.Visible = labelLowBatteryWarningValue.Visible = warn;

            if (sleep || warn)
            {
                panelEnergyHeader.Top = bottom;
                bottom = panelEnergyHeader.Bottom + gap;

                if (sleep)
                {
                    labelAutoPowerOff.Top = comboBoxAutoPowerOff.Top = bottom;
                    bottom = comboBoxAutoPowerOff.Bottom + gap;
                }
                if (warn)
                {
                    labelLowBatteryWarning.Top = sliderLowBatteryWarning.Top = labelLowBatteryWarningValue.Top = bottom;
                    bottom = sliderLowBatteryWarning.Bottom + gap;
                }
            }

            bool hasOled = oled is not null;
            panelOled.Visible = hasOled;

            if (hasOled)
            {
                panelOled.Top = bottom;
                bottom = panelOled.Bottom + gap;
            }

            int margin = panelLightingHeader.Left;
            int width = grid ? Math.Max(ClientSize.Width, panelKeys.Right + panelKeys.Left) : ClientSize.Width;
            ClientSize = new Size(width, bottom + gap);

            panelLightingHeader.Width = panelKeysHeader.Width = panelEnergyHeader.Width = panelOled.Width = ClientSize.Width - 2 * margin;

            if (grid && bindings)
            {
                buttonResetBindings.Left = ClientSize.Width - margin - buttonResetBindings.Width;
                textBoxKeyPath.Width = Math.Max(0, buttonResetBindings.Left - gap - textBoxKeyPath.Left);
            }
        }

        private void ComboBoxAutoPowerOff_DropDownClosed(object? sender, EventArgs e)
        {
            var values = Enum.GetValues(typeof(PowerOffSetting));
            int idx = comboBoxAutoPowerOff.SelectedIndex;
            if (idx < 0 || idx >= values.Length) return;
            ApplyEnergySettings(keyboard.LowBatteryWarning, (PowerOffSetting)values.GetValue(idx)!);
        }

        private void VisualizeEnergy()
        {
            if (!keyboard.HasAutoPowerOff() && !keyboard.HasLowBatteryWarning()) return;

            if (keyboard.HasAutoPowerOff())
                comboBoxAutoPowerOff.SelectedIndex = keyboard.PowerOffSetting == PowerOffSetting.Never
                    ? comboBoxAutoPowerOff.Items.Count - 1
                    : (int)keyboard.PowerOffSetting;

            if (keyboard.HasLowBatteryWarning())
            {
                sliderLowBatteryWarning.Value = Math.Clamp(keyboard.LowBatteryWarning, 0, sliderLowBatteryWarning.Max);
                labelLowBatteryWarningValue.Text = keyboard.LowBatteryWarning == 0 ? Properties.Strings.Never : keyboard.LowBatteryWarning + "%";
            }
        }

        private void AsusKeyboardSettings_Shown(object? sender, EventArgs e)
        {
            // combo heights settle once handles exist; relayout before positioning the window
            LayoutSections();

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

        private void ComboBoxProfile_DropDownClosed(object? sender, EventArgs e)
        {
            if (loadingSettings || comboBoxProfile.SelectedIndex < 0) return;
            if (comboBoxProfile.SelectedIndex == keyboard.Profile) return;

            // persist pending edits while the old profile is still active on the device
            if (settingsChanged)
            {
                if (keyColors.Length > 0) keyboard.StoreKeyColors(keyColors);
                keyboard.SaveLighting();
            }

            settingsChanged = false;
            keyboard.SetProfile(comboBoxProfile.SelectedIndex);

            loadingSettings = true;
            LoadSettings();
            loadingSettings = false;

            Task.Run(() => { try { if (keyboard.HasTransientLighting) keyboard.ApplyStoredLighting(); keyboard.ReadBattery(); BeginInvoke(VisualizeEnergy); } catch { } });
        }

        private object[] BuildBindingComboItems()
        {
            var list = new List<object>();
            var region = AuraKeyboardLayouts.RegionalLegends.GetValueOrDefault(keyboard.MultiLayout);
            list.Add(new BindingSeparator("Keyboard"));
            foreach (var (name, code) in AuraKeyboardLayouts.Keys)
                list.Add(new BindingItem(code, region?.GetValueOrDefault(name) ?? name));
            list.Add(new BindingSeparator("Multimedia"));
            foreach (var (name, code) in AuraKeyboardLayouts.MultimediaTargets)
                list.Add(new BindingItem(code, name));
            list.Add(new BindingSeparator("Mouse"));
            foreach (var (name, code) in AuraKeyboardLayouts.MouseTargets)
                list.Add(new BindingItem(code, name));
            list.Add(new BindingSeparator("Combos"));
            foreach (var combo in KeyCombos)
                list.Add(new BindingItem(combo.Code, combo.Name, combo.Keys));
            list.Add(new BindingSeparator("Launch"));
            for (int slot = 0; slot < AsusKeyboard.LaunchSlots; slot++)
                list.Add(new BindingItem((ushort)(AsusKeyboard.LaunchCode + slot), "Launch App " + (slot + 1),
                    new[] { K("Ctrl"), K("Shift"), K("Alt"), K("F" + (slot + 1)) }));
            return list.ToArray();
        }

        // Code is a config-only id, never sent to the device
        private sealed record ComboDef(ushort Code, string Name, ushort[] Keys);

        private static ushort K(string name) => AuraKeyboardLayouts.Keys[name];

        private static readonly ComboDef[] KeyCombos =
        {
            new(0xC101, "Copy",         new[] { K("Ctrl"), K("C") }),
            new(0xC102, "Paste",        new[] { K("Ctrl"), K("V") }),
            new(0xC103, "Cut",          new[] { K("Ctrl"), K("X") }),
            new(0xC104, "Undo",         new[] { K("Ctrl"), K("Z") }),
            new(0xC105, "Select All",   new[] { K("Ctrl"), K("A") }),
            new(0xC106, "Alt + Tab",    new[] { K("Alt"), K("Tab") }),
            new(0xC107, "Alt + F4",     new[] { K("Alt"), K("F4") }),
            new(0xC108, "Show Desktop", new[] { K("Win"), K("D") }),
            new(0xC109, "Lock PC",      new[] { K("Win"), K("L") }),
            new(0xC10A, "Task Manager", new[] { K("Ctrl"), K("Shift"), K("Esc") }),
            new(0xC10B, "Screenshot",   new[] { K("Win"), K("PrtSc") }),
            new(0xC10C, "Snipping Tool",new[] { K("Win"), K("Shift"), K("S") }),
        };

        private void BindingCombo_Changed(object? sender, EventArgs e)
        {
            if (updatingBindings || selectedKey is null) return;
            if (comboBoxKeyBinding.SelectedItem is BindingSeparator)
            {
                int next = comboBoxKeyBinding.SelectedIndex + 1;
                if (next < comboBoxKeyBinding.Items.Count && comboBoxKeyBinding.Items[next] is BindingItem)
                    comboBoxKeyBinding.SelectedIndex = next;
                return;
            }
            if (comboBoxKeyBinding.SelectedItem is BindingItem item
                && keyUsage.TryGetValue(selectedKey, out ushort sourceCode))
            {
                settingsChanged = true;
                Button? bound = selectedKey;
                VisualizeLaunchPath(item.Code);
                Task.Run(() =>
                {
                    try
                    {
                        if (item.ComboKeys is null) keyboard.SetKeyBinding(sourceCode, item.Code);
                        else keyboard.SetKeyCombo(sourceCode, item.Code, item.ComboKeys);
                        keyboard.SaveLighting();
                    }
                    catch { }
                    try { bound?.BeginInvoke(() => ApplyKeyUnderline(bound, sourceCode)); } catch { }
                });
            }
        }

        private void VisualizeLaunchPath(ushort code)
        {
            int slot = code - AsusKeyboard.LaunchCode;
            bool launch = slot >= 0 && slot < AsusKeyboard.LaunchSlots;
            textBoxKeyPath.Visible = launch;
            textBoxKeyPath.Tag = slot;
            if (launch) textBoxKeyPath.Text = AsusKeyboard.LaunchCommand(slot);
        }

        private void TextBoxKeyPath_Changed(object? sender, EventArgs e)
        {
            if (updatingBindings || textBoxKeyPath.Tag is not int slot) return;
            AsusKeyboard.SetLaunchCommand(slot, textBoxKeyPath.Text);
            Program.inputDispatcher?.RegisterKeys();
        }

        private void SelectKey(Button key)
        {
            if (selectedKey is not null) selectedKey.FlatAppearance.BorderSize = keyBorderSize;
            selectedKey = key;
            key.FlatAppearance.BorderSize = keyBorderSize * 2;

            bool known = keyUsage.TryGetValue(key, out ushort sourceCode);
            labelKeyBinding.Text = Properties.Strings.KeyBindings + ": " + key.Text;
            comboBoxKeyBinding.Enabled = known;

            updatingBindings = true;
            comboBoxKeyBinding.SelectedIndex = -1;
            if (known)
            {
                ushort current = keyboard.GetKeyBinding(sourceCode);
                if (current == 0) current = sourceCode;
                for (int i = 0; i < comboBoxKeyBinding.Items.Count; i++)
                    if (comboBoxKeyBinding.Items[i] is BindingItem item && item.Code == current)
                    { comboBoxKeyBinding.SelectedIndex = i; break; }
                VisualizeLaunchPath(current);
            }
            else textBoxKeyPath.Visible = false;
            updatingBindings = false;
        }

        private void BuildPalette()
        {
            float s = DeviceDpi / 192f;
            int size = (int)(40 * s);
            int gap = (int)(4 * s);

            for (int i = 0; i < paletteColors.Length; i++)
            {
                var swatch = new Button();
                swatch.SetBounds(i * (size + gap), (panelPalette.Height - size) / 2, size, size);
                swatch.FlatStyle = FlatStyle.Flat;
                swatch.FlatAppearance.BorderSize = 1;
                swatch.BackColor = paletteColors[i];
                swatch.Tag = paletteColors[i];
                swatch.Click += Swatch_Click;
                panelPalette.Controls.Add(swatch);
            }
        }

        private void Swatch_Click(object? sender, EventArgs e)
        {
            if (sender is not Button swatch || swatch.Tag is not Color color) return;
            paintColor = color;
            buttonPaintColor.SwatchColor = color;
        }

        private void ButtonPaintColor_Click(object? sender, EventArgs e)
        {
            RColorPicker colorDlg = new RColorPicker(paintColor);
            colorDlg.ColorChanged += c =>
            {
                paintColor = c;
                buttonPaintColor.SwatchColor = c;
            };
            colorDlg.ShowDialog(this);
        }

        private void ButtonFillAll_Click(object? sender, EventArgs e)
        {
            if (checkBoxSyncAura.Checked || SelectedMode() != KeyboardLightingMode.Direct) return;

            Array.Fill(keyColors, paintColor);
            settingsChanged = true;
            RefreshKeyGrid();
            Task.Run(() => { try { keyboard.SetLedColors(keyColors); } catch { } });
        }

        private void BuildKeyGrid()
        {
            float s = DeviceDpi / 192f;
            int unit = (int)(84 * s);
            int keyGap = (int)(4 * s);
            keyBorderSize = Math.Max(2, (int)(2 * s));

            var rows = new List<KeyDef[]>(keyboard.KeyLayout());

            if (keyboard.MediaKeyCount() > 0)
                rows.Add(Enumerable.Range(1, keyboard.MediaKeyCount()).Select(i => new KeyDef("M" + i)).ToArray());

            keyColors = keyboard.StoredKeyColors();

            int led = 0;
            float width = 0;
            for (int row = 0; row < rows.Count; row++)
            {
                float x = 0;
                Button? prev = null;
                foreach (var def in rows[row])
                {
                    x += def.Gap * unit;

                    if (prev is not null && def.Gap == 0 && AuraKeyboardLayouts.Legend(def.Name, keyboard.MultiLayout) == prev.Text)
                    {
                        prev.Width = (int)(x + def.Width * unit) - prev.Left - keyGap;
                        keyLedSpan[prev] = keyLedSpan.GetValueOrDefault(prev, 1) + 1;
                        x += def.Width * unit;
                        led++;
                        continue;
                    }

                    var key = new Button();
                    key.SetBounds((int)x, row * unit, (int)(def.Width * unit) - keyGap, (int)(def.Height * unit) - keyGap);
                    key.FlatStyle = FlatStyle.Flat;
                    key.FlatAppearance.BorderSize = keyBorderSize;
                    key.Font = new Font("Segoe UI", unit * 0.245f, GraphicsUnit.Pixel);
                    key.Text = AuraKeyboardLayouts.Legend(def.Name, keyboard.MultiLayout);
                    key.Tag = led;
                    key.Click += KeyTile_Click;
                    keyToolTip.SetToolTip(key, $"{def.Name} (LED {led})");
                    panelKeys.Controls.Add(key);
                    keyButtons.Add(key);

                    ushort code = AuraKeyboardLayouts.Keys.GetValueOrDefault(def.Name);
                    if (code == 0) code = AuraKeyboardLayouts.MediaKeys.GetValueOrDefault(def.Name);
                    if (code != 0) keyUsage[key] = code;

                    x += def.Width * unit;
                    led++;
                    prev = key;
                }
                width = Math.Max(width, x);
            }

            panelKeys.Size = new Size((int)width, rows.Count * unit);
            keyGlow = new float[keyButtons.Count];
            keyGlowColor = new Color[keyButtons.Count];
        }

        private void BuildTopRow()
        {
            labelProfile.Visible = comboBoxProfile.Visible = keyboard.HasProfiles();
            pictureBoxBatteryState.Visible = labelBatteryState.Visible = keyboard.HasBattery();
        }

        private void VisualizeBatteryState()
        {
            if (!keyboard.HasBattery()) return;
            labelBatteryState.Text = keyboard.Battery + "%";
            pictureBoxBatteryState.BackgroundImage = ControlHelper.TintImage(keyboard.Charging
                ? Properties.Resources.icons8_ladende_batterie_48
                : Properties.Resources.icons8_batterie_voll_geladen_48, foreMain);
        }

        private void Keyboard_BatteryUpdated(object? sender, EventArgs e)
        {
            if (Disposing || IsDisposed) return;
            try { BeginInvoke(VisualizeBatteryState); } catch { }
        }

        private void Keyboard_Disconnect(object? sender, EventArgs e)
        {
            if (Disposing || IsDisposed) return;
            try { BeginInvoke(Close); } catch { }
        }

        private void BuildTestLayoutSelector()
        {
            testLayoutSelector = true;
            labelTestLayout.Visible = comboBoxTestLayout.Visible = true;
            comboBoxTestLayout.Items.Add("(device)");
            foreach (var name in AuraKeyboardLayouts.Data.Keys.OrderBy(n => n))
                comboBoxTestLayout.Items.Add(name);
            comboBoxTestLayout.SelectedIndex = keyboard.ForcedLayoutName is null
                ? 0 : Math.Max(0, comboBoxTestLayout.Items.IndexOf(keyboard.ForcedLayoutName));
            comboBoxTestLayout.SelectedIndexChanged += TestLayout_Changed;
        }

        private void TestLayout_Changed(object? sender, EventArgs e)
        {
            keyboard.ForcedLayoutName = comboBoxTestLayout.SelectedIndex <= 0
                ? null : comboBoxTestLayout.SelectedItem as string;
            var reopen = RequestReopen;
            // defer past the combo's own message so we don't dispose the form re-entrantly
            BeginInvoke(new Action(() => { Close(); reopen?.Invoke(keyboard); }));
        }

        private Color DisplayColorForKey(int led)
        {
            KeyboardLightingMode mode = SelectedMode();
            if (mode == KeyboardLightingMode.Direct) return led < keyColors.Length ? keyColors[led] : Color.Gray;
            if (keyboard.SupportsColorSetting(mode)) return buttonLightingColor.SwatchColor ?? Color.Red;
            return Color.Gray;
        }

        private void RefreshKeyGrid()
        {
            foreach (var key in keyButtons)
                if (key.Tag is int led)
                    key.FlatAppearance.BorderColor = DisplayColorForKey(led);
        }

        private void ApplyKeyUnderline(Button key, ushort usage)
        {
            var style = keyboard.GetKeyBinding(usage) != 0 ? FontStyle.Underline : FontStyle.Regular;
            if (key.Font.Style != style) key.Font = new Font(key.Font, style);
        }

        private void ButtonResetBindings_Click(object? sender, EventArgs e)
        {
            settingsChanged = true;
            Task.Run(() =>
            {
                try { keyboard.ResetAllBindings(); } catch { }
                try { BeginInvoke(() => { RefreshBindingUnderlines(); if (selectedKey is not null) SelectKey(selectedKey); }); } catch { }
            });
        }

        private void RefreshBindingUnderlines()
        {
            foreach (var key in keyButtons)
                if (keyUsage.TryGetValue(key, out ushort u))
                    ApplyKeyUnderline(key, u);
        }

        private void KeyTile_Click(object? sender, EventArgs e)
        {
            if (sender is not Button key || key.Tag is not int led) return;

            SelectKey(key);

            if (checkBoxSyncAura.Checked) return;
            if (SelectedMode() != KeyboardLightingMode.Direct) return;

            int span = keyLedSpan.GetValueOrDefault(key, 1);
            for (int i = 0; i < span; i++)
                if (led + i < keyColors.Length) keyColors[led + i] = paintColor;
            key.FlatAppearance.BorderColor = paintColor;
            settingsChanged = true;
            Task.Run(() => { try { for (int i = 0; i < span; i++) keyboard.SetLedColor(led + i, paintColor); } catch { } });
        }

        private void LoadSettings()
        {
            checkBoxSyncAura.Checked = PeripheralsProvider.IsKeyboardAuraSync;

            if (keyColors.Length > 0) keyColors = keyboard.StoredKeyColors();

            int modeIndex = supportedModes.IndexOf(keyboard.StoredMode);
            comboBoxLightingMode.SelectedIndex = modeIndex >= 0 ? modeIndex : 0;
            comboBoxAnimationSpeed.SelectedIndex = Math.Clamp((int)keyboard.StoredSpeed, 0, comboBoxAnimationSpeed.Items.Count - 1);
            sliderBrightness.Value = Math.Clamp(keyboard.StoredBrightness, sliderBrightness.Min, sliderBrightness.Max);
            buttonLightingColor.SwatchColor = keyboard.StoredColor;
            buttonLightingColor2.SwatchColor = keyboard.StoredColor2;
            buttonLightingColor3.SwatchColor = keyboard.StoredColor3;
            buttonPaintColor.SwatchColor = paintColor;

            RefreshBindingUnderlines();
            VisualizeControls();
            VisualizeEnergy();
            VisualizeOled();
        }

        private KeyboardLightingMode SelectedMode()
        {
            int index = comboBoxLightingMode.SelectedIndex;
            return (index >= 0 && index < supportedModes.Count) ? supportedModes[index] : KeyboardLightingMode.Static;
        }

        private void VisualizeControls()
        {
            bool manual = !checkBoxSyncAura.Checked;
            KeyboardLightingMode mode = SelectedMode();

            comboBoxLightingMode.Enabled = manual;

            buttonLightingColor.Visible = keyboard.SupportsColorSetting(mode);
            buttonLightingColor.Enabled = manual;
            buttonLightingColor2.Visible = keyboard.SupportsColor2Setting(mode);
            buttonLightingColor2.Enabled = manual;
            buttonLightingColor3.Visible = keyboard.SupportsColor3Setting(mode);
            buttonLightingColor3.Enabled = manual;

            comboBoxAnimationSpeed.Enabled = manual;

            bool paint = manual && mode == KeyboardLightingMode.Direct;
            buttonPaintColor.Enabled = paint;
            panelPalette.Enabled = paint;
            buttonFillAll.Enabled = paint;

            comboBoxKeyBinding.Enabled = selectedKey is not null
                && keyUsage.ContainsKey(selectedKey);

            LayoutSections();
            RefreshKeyGrid();
            UpdatePreviewTimer();
        }

        private void PickColor(RColorButton button)
        {
            RColorPicker colorDlg = new RColorPicker(button.SwatchColor ?? Color.Red);
            colorDlg.ColorChanged += c =>
            {
                button.SwatchColor = c;
                ApplySettings();
            };
            colorDlg.ShowDialog(this);
        }

        private void CheckBoxSyncAura_CheckedChanged(object? sender, EventArgs e)
        {
            if (loadingSettings) return;

            PeripheralsProvider.SetKeyboardAuraSync(checkBoxSyncAura.Checked);
            VisualizeControls();

            if (checkBoxSyncAura.Checked) PeripheralsProvider.SyncKeyboardsWithAura();
            else ApplySettings();
        }

        private void StoreSettings()
        {
            keyboard.StoreLighting(
                SelectedMode(),
                buttonLightingColor.SwatchColor ?? Color.Red,
                buttonLightingColor2.SwatchColor ?? Color.Black,
                buttonLightingColor3.SwatchColor ?? Color.Black,
                (AuraSpeed)Math.Max(0, comboBoxAnimationSpeed.SelectedIndex),
                sliderBrightness.Value);
            if (keyColors.Length > 0) keyboard.StoreKeyColors(keyColors);
            settingsChanged = true;
        }

        private void ApplySettings()
        {
            if (loadingSettings || checkBoxSyncAura.Checked) return;

            VisualizeControls();
            StoreSettings();

            Task.Run(() => { try { keyboard.ApplyStoredLighting(); keyboard.SaveLighting(); } catch { } });
        }

        private void SliderBrightness_MouseUp(object? sender, EventArgs e)
        {
            if (loadingSettings) return;

            StoreSettings();

            if (checkBoxSyncAura.Checked) Task.Run(() => { try { keyboard.SyncFromLaptopAura(); } catch { } });
            else Task.Run(() => { try { keyboard.ApplyStoredLighting(); keyboard.SaveLighting(); } catch { } });
        }

        private void AsusKeyboardSettings_FormClosing(object? sender, FormClosingEventArgs e)
        {
            keyboard.BatteryUpdated -= Keyboard_BatteryUpdated;
            keyboard.Disconnect -= Keyboard_Disconnect;
            previewTimer.Stop();
            previewTimer.Dispose();
            keyToolTip.Dispose();

            if (!settingsChanged) return;
            // key colors are painted live over HID, persisted to config once here
            if (keyColors.Length > 0) keyboard.StoreKeyColors(keyColors);
            Task.Run(() => { try { keyboard.SaveLighting(); } catch { } });
        }
    }
}
