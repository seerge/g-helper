using GHelper.Helpers;
using GHelper.Properties;
using GHelper.UI;
using GHelper.USB;

namespace GHelper
{
    /// <summary>
    /// 音频律动（Audio Spectrum / Audio Pulse）参数设置面板。
    /// TableLayoutPanel 自动布局避免重叠；滑块与数值输入框双向同步，实时生效。
    /// </summary>
    public class AudioSettingsForm : RForm
    {
        private sealed record SliderDef(string Key, string Label, string Help, int Default);

        private static readonly SliderDef[] Defs =
        {
            new("audio_sensitivity", Strings.AudioParamSensitivity, Strings.AudioParamSensitivityHelp, 50),
            new("audio_attack",    Strings.AudioParamAttack,    Strings.AudioParamAttackHelp,    40),
            new("audio_decay",     Strings.AudioParamDecay,     Strings.AudioParamDecayHelp,     70),
            new("audio_reference", Strings.AudioParamReference, Strings.AudioParamReferenceHelp, 30),
            new("audio_threshold", Strings.AudioParamThreshold, Strings.AudioParamThresholdHelp, 15),
            new("audio_curve",     Strings.AudioParamCurve,     Strings.AudioParamCurveHelp,     60),
            new("audio_min",       Strings.AudioParamMinBrightness, Strings.AudioParamMinBrightnessHelp, 0),
            new("audio_max",       Strings.AudioParamMaxBrightness, Strings.AudioParamMaxBrightnessHelp, 100),
            new("audio_color_speed", Strings.AudioParamColorSpeed, Strings.AudioParamColorSpeedHelp, 30),
        };

        private readonly List<(SliderDef Def, Slider Slider, NumericUpDown Numeric)> rows = new();

        public AudioSettingsForm()
        {
            Text = "Audio";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            AutoScroll = true;
            ClientSize = new Size(500, 680);

            InitTheme(true);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                Margin = new Padding(14, 10, 14, 10)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            // 标题区
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
            var header = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0) };
            var title = new Label
            {
                Text = Strings.AudioSettingsTitle,
                AutoSize = true,
                Font = new Font(Font.FontFamily, 10f, FontStyle.Bold),
                ForeColor = foreMain,
                Location = new Point(2, 4)
            };
            var tip = new Label
            {
                Text = Strings.AudioSettingsTip,
                AutoSize = true,
                Font = new Font(Font.FontFamily, 8.5f),
                ForeColor = Color.FromArgb(140, 140, 140),
                Location = new Point(2, 28)
            };
            var resetButton = new RButton
            {
                Text = Strings.AudioReset,
                Dock = DockStyle.Right,
                Size = new Size(88, 30),
                Margin = new Padding(8, 12, 0, 0),
                Secondary = true
            };
            resetButton.Click += (_, _) => ResetAll();
            header.Controls.Add(resetButton);
            header.Controls.Add(title);
            header.Controls.Add(tip);
            layout.Controls.Add(header, 0, 0);

            // 每个参数一行：名称+说明 | 滑块 | 数值输入
            int rowIndex = 1;
            foreach (var def in Defs)
            {
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F));

                int value = Math.Clamp(AppConfig.Get(def.Key, def.Default), 0, 100);

                var row = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 3,
                    Margin = new Padding(0, 4, 0, 4)
                };
                row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190F));
                row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 72F));

                var infoPanel = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0) };
                var info = new Label
                {
                    Text = def.Label,
                    Dock = DockStyle.Top,
                    Height = 22,
                    Font = new Font(Font.FontFamily, 9f),
                    ForeColor = foreMain,
                    Margin = new Padding(0)
                };
                var help = new Label
                {
                    Text = def.Help,
                    Dock = DockStyle.Top,
                    Height = 34,
                    Font = new Font(Font.FontFamily, 7.5f),
                    ForeColor = Color.FromArgb(140, 140, 140),
                    Margin = new Padding(0)
                };
                infoPanel.Controls.Add(info);
                infoPanel.Controls.Add(help);

                var sliderHost = new Panel { Dock = DockStyle.Fill, Margin = new Padding(6, 0, 6, 0) };
                var slider = new Slider
                {
                    Min = 0,
                    Max = 100,
                    Step = 1,
                    Value = value,
                    Dock = DockStyle.None,
                    Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                    Height = 30,
                    AccessibleName = def.Label
                };
                sliderHost.Controls.Add(slider);
                sliderHost.Resize += (_, _) => slider.Top = Math.Max(0, (sliderHost.Height - slider.Height) / 2);

                var numeric = new NumericUpDown
                {
                    Minimum = 0,
                    Maximum = 100,
                    Value = value,
                    Dock = DockStyle.Fill,
                    TextAlign = HorizontalAlignment.Center,
                    Margin = new Padding(0, 14, 0, 14)
                };

                bool syncing = false;
                void ApplyValue(int v)
                {
                    AppConfig.Set(def.Key, v);
                    Aura.RefreshAudioParams();
                }
                slider.ValueChanged += (_, _) =>
                {
                    if (syncing) return;
                    syncing = true;
                    numeric.Value = slider.Value;
                    syncing = false;
                    ApplyValue(slider.Value);
                };
                numeric.ValueChanged += (_, _) =>
                {
                    if (syncing) return;
                    syncing = true;
                    slider.Value = (int)numeric.Value;
                    syncing = false;
                    ApplyValue((int)numeric.Value);
                };

                row.Controls.Add(infoPanel, 0, 0);
                row.Controls.Add(sliderHost, 1, 0);
                row.Controls.Add(numeric, 2, 0);
                layout.Controls.Add(row, 0, rowIndex);

                rows.Add((def, slider, numeric));

                rowIndex++;
            }

            Controls.Add(layout);
        }

        private void ResetAll()
        {
            foreach (var (def, slider, numeric) in rows)
            {
                AppConfig.Set(def.Key, def.Default);
                slider.Value = def.Default;
                numeric.Value = def.Default;
            }
            Aura.RefreshAudioParams();
        }
    }
}
