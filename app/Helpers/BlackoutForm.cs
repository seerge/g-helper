using GHelper.Display;
using System.Runtime.InteropServices;

namespace GHelper.Helpers
{
    public class BlackoutForm : Form
    {
        private const int SW_SHOWNA = 8;
        private const int SW_HIDE = 0;
        private const int WS_EX_TOOLWINDOW = 0x80;
        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int WS_EX_TOPMOST = 0x8;

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private bool _active;

        public BlackoutForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.Black;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            Cursor = Cursors.Default;
            DoubleBuffered = true;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE | WS_EX_TOPMOST;
                return cp;
            }
        }

        protected override bool ShowWithoutActivation => true;

        public bool IsActive => _active;

        private static Screen ResolveTargetScreen()
        {
            var laptopDevice = ScreenNative.FindLaptopScreen();
            if (laptopDevice is not null)
            {
                foreach (var screen in Screen.AllScreens)
                    if (string.Equals(screen.DeviceName, laptopDevice, StringComparison.OrdinalIgnoreCase))
                        return screen;
            }
            return Screen.PrimaryScreen ?? Screen.AllScreens[0];
        }

        public void SetActive(bool active)
        {
            if (Program.settingsForm is null) return;

            Program.settingsForm.Invoke(delegate
            {
                if (active)
                {
                    if (!IsHandleCreated) CreateControl();
                    Bounds = ResolveTargetScreen().Bounds;
                    ShowWindow(Handle, SW_SHOWNA);
                    _active = true;
                }
                else
                {
                    if (IsHandleCreated) ShowWindow(Handle, SW_HIDE);
                    _active = false;
                }
            });
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            SetActive(false);
        }
    }
}
