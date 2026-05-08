using System.Runtime.InteropServices;

namespace GHelper.UI
{
    public class RTextBox : TextBox
    {
        private const int WM_PAINT = 0xF;

        [DllImport("uxtheme.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string? pszSubIdList);

        public RTextBox()
        {
            BorderStyle = BorderStyle.FixedSingle;
        }

        public void ApplyTheme(bool dark)
        {
            BackColor = dark ? RForm.buttonMain : SystemColors.Window;
            ForeColor = dark ? RForm.foreMain : SystemColors.WindowText;
            SetWindowTheme(Handle, dark ? "DarkMode_Explorer" : "Explorer", null);
        }

        // Framework's PlaceholderText is hardcoded to SystemColors.GrayText (~109,109,109),
        // which is unreadable on a dark BG. We shadow it and paint our own using a BackColor/ForeColor blend.
        private string _placeholder = "";
        public new string PlaceholderText
        {
            get => _placeholder;
            set
            {
                _placeholder = value ?? "";
                Invalidate();
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_PAINT
                && !Focused
                && string.IsNullOrEmpty(Text)
                && !string.IsNullOrEmpty(_placeholder))
            {
                using var g = Graphics.FromHwnd(Handle);
                Color faded = Color.FromArgb(
                    (BackColor.R + ForeColor.R) / 2,
                    (BackColor.G + ForeColor.G) / 2,
                    (BackColor.B + ForeColor.B) / 2);
                var rect = ClientRectangle;
                rect.X += 1;
                TextRenderer.DrawText(g, _placeholder, Font, rect, faded,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }
        }
    }
}
