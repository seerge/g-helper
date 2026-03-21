using GHelper.Helpers;
using GHelper.UI;
using System.Drawing.Drawing2D;

namespace GHelper.Battery
{
    public partial class BatteryReminderForm : Form
    {
        private static BatteryReminderForm? _instance;

        private System.Windows.Forms.Timer? _autoHideTimer;
        private bool _mouseHovering;

        public static void ShowReminder(List<string> issues)
        {
            _instance?.Close();
            _instance = new BatteryReminderForm(issues);
            _instance.Show();
        }

        public static void DismissIfShowing()
        {
            if (_instance != null && !_instance.IsDisposed)
            {
                _instance.Close();
                _instance = null;
            }
        }

        public BatteryReminderForm()
        {
            InitializeComponent();
        }

        private BatteryReminderForm(List<string> issues)
        {
            TopMost = true;
            Opacity = 0.95;

            BackColor = RForm.formBack;
            ForeColor = RForm.foreMain;

            buttonOptimize.BackColor = RForm.buttonMain;
            buttonOptimize.ForeColor = RForm.foreMain;
            buttonOptimize.FlatAppearance.BorderColor = RForm.borderMain;
            buttonOptimize.Activated = true;
            buttonOptimize.BorderColor = RForm.borderMain;

            buttonDismiss.BackColor = RForm.buttonSecond;
            buttonDismiss.ForeColor = RForm.foreMain;
            buttonDismiss.FlatAppearance.BorderColor = RForm.borderMain;
            buttonDismiss.Activated = true;
            buttonDismiss.BorderColor = RForm.borderMain;

            Region = CreateRoundedRegion(ClientRectangle, 20);

            labelTitle.Text = Properties.Strings.BatteryOptimization;
            labelSubtitle.Text = Properties.Strings.BatteryOptimizationSubtitle;
            checkAutoOptimize.Text = Properties.Strings.BatteryAutoSwitch;
            buttonOptimize.Text = Properties.Strings.BatteryOptimizeButton;
            buttonDismiss.Text = Properties.Strings.BatteryDontRemind;

            PopulateIssues(issues);
            PositionOnScreen();

            // Wire up events
            labelClose.Click += (s, e) => Close();
            buttonOptimize.Click += ButtonOptimize_Click;
            buttonDismiss.Click += ButtonDismiss_Click;

            // Auto-hide timer
            int timeout = AppConfig.Get("battery_remind_timeout", 30);
            timeout = Math.Clamp(timeout, 2, 120);

            _autoHideTimer = new System.Windows.Forms.Timer();
            _autoHideTimer.Interval = timeout * 1000;
            _autoHideTimer.Tick += (s, e) =>
            {
                if (!_mouseHovering) Close();
            };
            _autoHideTimer.Start();

            // Track mouse hover on form and all controls
            MouseEnter += (s, e) => _mouseHovering = true;
            MouseLeave += (s, e) => _mouseHovering = false;
            foreach (Control control in Controls)
                HookMouseHover(control);

            FormClosed += (s, e) =>
            {
                _autoHideTimer?.Stop();
                _autoHideTimer?.Dispose();
                if (_instance == this) _instance = null;
            };
        }

        private void PopulateIssues(List<string> issues)
        {
            Label[] labels = { labelIssue1, labelIssue2, labelIssue3 };

            for (int i = 0; i < labels.Length; i++)
            {
                if (i < issues.Count)
                {
                    labels[i].Text = (i == 2 && issues.Count > 3)
                        ? "..."
                        : "\u2022  " + issues[i];
                    labels[i].Visible = true;
                }
                else
                {
                    labels[i].Visible = false;
                }
            }
        }

        private void PositionOnScreen()
        {
            var screen = Screen.PrimaryScreen ?? Screen.FromControl(this);
            var workArea = screen.WorkingArea;
            Left = workArea.Right - Width - 20;
            Top = workArea.Bottom - Height - 20;
        }

        private void ButtonOptimize_Click(object? sender, EventArgs e)
        {
            if (checkAutoOptimize.Checked)
                AppConfig.Set("battery_auto_optimize", 1);

            Close();

            Task.Run(() =>
            {
                BatteryOptimizationService.ApplyBatteryOptimizations();
                Program.toast.RunToast(Properties.Strings.BatteryOptimizedToast);
            });
        }

        private void ButtonDismiss_Click(object? sender, EventArgs e)
        {
            AppConfig.Set("battery_remind", 0);
            Close();
        }

        private void HookMouseHover(Control control)
        {
            control.MouseEnter += (s, e) => _mouseHovering = true;
            control.MouseLeave += (s, e) => _mouseHovering = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using var pen = new Pen(RForm.borderMain, 1);
            using var path = CreateRoundedPath(new Rectangle(0, 0, Width - 1, Height - 1), 20);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.DrawPath(pen, path);
        }

        private static Region CreateRoundedRegion(Rectangle bounds, int radius)
        {
            using var path = CreateRoundedPath(bounds, radius);
            return new Region(path);
        }

        private static GraphicsPath CreateRoundedPath(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
