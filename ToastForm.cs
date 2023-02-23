using System.Runtime.InteropServices;


namespace GHelper
{
    public partial class ToastForm : Form
    {

        private System.Windows.Forms.Timer timer = default!;

        private const int SW_SHOWNOACTIVATE = 4;
        private const int HWND_TOP = 0;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;
        private const int SWP_NOZORDER = 0x0004;


        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        static extern bool SetWindowPos(
             int hWnd,             // Window handle
             int hWndInsertAfter,  // Placement-order handle
             int X,                // Horizontal position
             int Y,                // Vertical position
             int cx,               // Width
             int cy,               // Height
             uint uFlags);         // Window positioning flags

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        static void ShowInactiveTopmost(Form frm)
        {
            ShowWindow(frm.Handle, SW_SHOWNOACTIVATE);
            SetWindowPos(frm.Handle.ToInt32(), HWND_TOP,
            frm.Left, frm.Top, frm.Width, frm.Height,
            SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER);
            
        }

        public ToastForm()
        {
            InitializeComponent();
            this.FormBorderStyle= FormBorderStyle.None;
            this.TopMost = true;
            this.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 15, 15));

            timer = new System.Windows.Forms.Timer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 2500;
        }

        public void RunToast(string text)
        {

            Top = (Screen.FromControl(this).WorkingArea.Height - this.Height) / 2 ;
            Left = (Screen.FromControl(this).Bounds.Width - this.Width) / 2;
            
            labelMode.Text = text;

            ShowInactiveTopmost(this);

            timer.Enabled= true;
            timer.Stop();
            timer.Start();
        }

        private void ToastForm_Show(object? sender, EventArgs e)
        {
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            timer.Enabled = false;
            Hide();
        }
    }
}
