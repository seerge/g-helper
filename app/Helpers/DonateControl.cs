using GHelper.UI;
using System.Diagnostics;

namespace GHelper.Helpers
{
    public class DonateControl
    {
        private readonly SettingsForm _settings;
        private readonly RBadgeButton _button;
        private CustomContextMenu? _contextMenu;

        public DonateControl(SettingsForm settings, RBadgeButton button)
        {
            _settings = settings;
            _button = button;
        }

        public void Init()
        {
            _button.Click += Button_Click;
            _button.MouseUp += Button_MouseUp;

            if (!AppConfig.Is("donate_dismissed"))
            {
                int click = AppConfig.Get("donate_click");
                int startCount = AppConfig.Get("start_count");
                if (startCount >= ((click < 20) ? 20 : click + 50))
                {
                    _button.BorderColor = RForm.colorTurbo;
                    _button.Badge = Math.Clamp((startCount - click) / 50, 1, 5);
                }
            }
        }

        public void ApplyTheme()
        {
            if (_contextMenu is not null)
            {
                _contextMenu.BackColor = _settings.BackColor;
                _contextMenu.ForeColor = _settings.ForeColor;
            }
        }

        private void Button_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            if (_contextMenu is null)
            {
                var padding = new Padding(5, 5, 5, 5);

                var menuAlreadyDonated = new ToolStripMenuItem("❤ Already donated") { Margin = padding };
                menuAlreadyDonated.Click += (s, ev) =>
                {
                    AppConfig.Set("donate_dismissed", 1);
                    SetThankYou();
                };

                var menuNotInterested = new ToolStripMenuItem("Not interested") { Margin = padding };
                menuNotInterested.Click += (s, ev) =>
                {
                    AppConfig.Set("donate_dismissed", 1);
                    _button.Badge = 0;
                };

                var menuCancel = new ToolStripMenuItem("Cancel") { Margin = padding };

                _contextMenu = new CustomContextMenu();
                _contextMenu.ShowImageMargin = false;
                _contextMenu.Items.Add(menuAlreadyDonated);
                _contextMenu.Items.Add(menuNotInterested);
                _contextMenu.Items.Add(new ToolStripSeparator());
                _contextMenu.Items.Add(menuCancel);
                _contextMenu.Renderer = new CustomMenuRenderer();
            }

            ApplyTheme();
            _contextMenu.Show(_button, new Point(e.X, e.Y));
        }

        private void Button_Click(object? sender, EventArgs e)
        {
            AppConfig.Set("donate_click", AppConfig.Get("start_count"));
            SetThankYou();
            Process.Start(new ProcessStartInfo("https://g-helper.com/support") { UseShellExecute = true });
        }

        private void SetThankYou()
        {
            _button.Badge = 0;
            _button.Text = Properties.Strings.ThankYou;
        }
    }
}
