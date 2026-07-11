using System.Runtime.InteropServices;

namespace GHelper.UI
{
    public class RNumericUpDown : NumericUpDown
    {
        [DllImport("uxtheme.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string? pszSubIdList);

        public RNumericUpDown()
        {
            BorderStyle = BorderStyle.None;
        }

        public void ApplyTheme(bool dark)
        {
            BackColor = dark ? RForm.buttonMain : SystemColors.Window;
            ForeColor = dark ? RForm.foreMain : SystemColors.WindowText;

            string theme = dark ? "DarkMode_Explorer" : "Explorer";
            SetWindowTheme(Handle, theme, null);
            foreach (Control child in Controls)
                SetWindowTheme(child.Handle, theme, null);
        }
    }
}
