using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GHelper.UI
{
    public class IconHelper
    {

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        private const uint WM_SETICON = 0x80u;
        private const int ICON_SMALL = 0;
        private const int ICON_BIG = 1;


        public static void SetIcon(Form form, Bitmap icon)
        {
            try
            {
                SendMessage(form.Handle, WM_SETICON, ICON_BIG, Icon.ExtractAssociatedIcon(Application.ExecutablePath)!.Handle);
                IntPtr hIcon = icon.GetHicon();
                SendMessage(form.Handle, WM_SETICON, ICON_SMALL, hIcon);
                DestroyIcon(hIcon);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting icon {ex.Message}");
            }
        }

    }
}
