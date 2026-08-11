using GHelper.Helpers;
using GHelper.UI;
using GHelper.USB;

namespace GHelper
{
    /// <summary>
    /// 音频律动（Audio Spectrum / Audio Pulse）参数设置面板。
    /// 全部参数实时写入 AppConfig 并刷新 Aura 的音频处理参数。
    /// </summary>
    public class AudioSettingsForm : RForm
    {
        private sealed record SliderDef(string Key, string Label, string Help, int Default);

        private static readonly SliderDef[] Defs =
        {
            new("audio_sensitivity", "Sensitivity", "整体响应增益。调低可抑制乱闪，调高更活泼", 50),
            new("audio_attack",    "Attack",    "对声音上升的平滑程度。越小越稳定，越大越跟手", 40),
            new("audio_decay",     "Decay",     "声音消失后灯光渐灭的速度。越大余辉越久", 70),
            new("audio_reference", "Reference", "音量基准的跟随快慢。越小越不容易整体全亮", 30),
            new("audio_threshold", "Threshold", "低于此电平不响应。可压制环境噪音与轻微旋律乱闪", 15),
            new("audio_curve",     "Curve",     "亮度响应曲线指数。越大层次越分明、暗部越暗", 60),
            new("audio_min",       "Min Brightness", "灯光最低亮度，防止完全熄灭", 0),
            new("audio_max",       "Max Brightness", "灯光最高亮度，防止长时间全亮没有层次", 100),
            new("audio_color_speed", "Color Speed",  "颜色切换速度。越大音乐高潮时切换越急促", 30),
        };

        public AudioSettingsForm()
        {
            Text = "Audio";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            AutoScroll = true;

            InitTheme(true);

            var title = new Label
            {
                Text = "Audio 律动设置",
                AutoSize = true,
                Font = new Font(Font.FontFamily, 10f, FontStyle.Bold),
                ForeColor = foreMain,
                Location = new Point(20, 12)
            };
            Controls.Add(title);

            var tip = new Label
            {
                Text = "拖动滑块实时生效，可边听歌边调节",
                AutoSize = true,
                Font = new Font(Font.FontFamily, 8.5f),
                ForeColor = Color.FromArgb(140, 140, 140),
                Location = new Point(20, 34)
            };
            Controls.Add(tip);

            int y = 60;
            foreach (var def in Defs)
            {
                int value = AppConfig.Get(def.Key, def.Default);
                value = Math.Clamp(value, 0, 100);

                var panel = new Panel
                {
                    Size = new Size(430, 64),
                    Location = new Point(10, y),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                };
                y += 68;

                var info = new Label
                {
                    Text = def.Label,
                    AutoSize = true,
                    Font = new Font(Font.FontFamily, 9f),
                    ForeColor = foreMain,
                    Location = new Point(10, 2)
                };
                panel.Controls.Add(info);

                var help = new Label
                {
                    Text = def.Help,
                    AutoSize = true,
                    Font = new Font(Font.FontFamily, 8f),
                    ForeColor = Color.FromArgb(140, 140, 140),
                    Location = new Point(10, 22)
                };
                panel.Controls.Add(help);

                var slider = new Slider
                {
                    Min = 0,
                    Max = 100,
                    Step = 1,
                    Value = value,
                    Size = new Size(340, 30),
                    Location = new Point(10, 38),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    AccessibleName = def.Label
                };
                panel.Controls.Add(slider);

                var valueLabel = new Label
                {
                    Text = value.ToString(),
                    Size = new Size(50, 30),
                    Location = new Point(370, 38),
                    TextAlign = ContentAlignment.MiddleLeft,
                    ForeColor = foreMain,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };
                panel.Controls.Add(valueLabel);

                slider.ValueChanged += (_, _) =>
                {
                    AppConfig.Set(def.Key, slider.Value);
                    valueLabel.Text = slider.Value.ToString();
                    Aura.RefreshAudioParams();
                };

                Controls.Add(panel);
            }

            ClientSize = new Size(450, Math.Min(y + 16, 660));
        }
    }
}
