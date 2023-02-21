using System.Runtime.InteropServices;


namespace GHelper
{
    public partial class ToastForm : Form
    {

        private System.Windows.Forms.Timer timer = default!;

        private const int SW_SHOWNOACTIVATE = 4;
        private const int HWND_TOPMOST = -1;
        private const uint SWP_NOACTIVATE = 0x0010;

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        static extern bool SetWindowPos(
             int hWnd,             // Window handle
             int hWndInsertAfter,  // Placement-order handle
             int X,                // Horizontal position
             int Y,                // Vertical position
             int cx,               // Width
             int cy,               // Height
             uint uFlags);         // Window positioning flags

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        static void ShowInactiveTopmost(Form frm)
        {
            ShowWindow(frm.Handle, SW_SHOWNOACTIVATE);
            SetWindowPos(frm.Handle.ToInt32(), HWND_TOPMOST,
            frm.Left, frm.Top, frm.Width, frm.Height,
            SWP_NOACTIVATE);
        }

        public ToastForm()
        {
            InitializeComponent();
        }

        public void RunToast(string text)
        {

            Top = Screen.FromControl(this).WorkingArea.Height - this.Height - 100;
            Left = (Screen.FromControl(this).Bounds.Width - this.Width) / 2;

            ShowInactiveTopmost(this);

            labelMode.Text = text;

            timer = new System.Windows.Forms.Timer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Enabled = true;
            timer.Interval = 1000;
            timer.Start();
        }

        private void ToastForm_Show(object? sender, EventArgs e)
        {
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            Close();
        }
    }
}
