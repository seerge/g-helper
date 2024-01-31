using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GHelper.Helpers
{
    public static class OnScreenKeyboard
    {
        static OnScreenKeyboard()
        {
            var version = Environment.OSVersion.Version;
            switch (version.Major)
            {
                case 6:
                    switch (version.Minor)
                    {
                        case 2:
                            // Windows 10 (ok)
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        private static void StartTabTip()
        {
            var p = Process.Start(@"C:\Program Files\Common Files\Microsoft Shared\ink\TabTip.exe");
            int handle = 0;
            while ((handle = NativeMethods.FindWindow("IPTIP_Main_Window", "")) <= 0)
            {
                Thread.Sleep(100);
            }
        }

        public static void ToggleVisibility()
        {
            var type = Type.GetTypeFromCLSID(Guid.Parse("4ce576fa-83dc-4F88-951c-9d0782b4e376"));
            var instance = (ITipInvocation)Activator.CreateInstance(type);
            instance.Toggle(NativeMethods.GetDesktopWindow());
            Marshal.ReleaseComObject(instance);
        }

        public static void Show()
        {
            int handle = NativeMethods.FindWindow("IPTIP_Main_Window", "");
            if (handle <= 0) // nothing found
            {
                StartTabTip();
                Thread.Sleep(100);
            }
            // on some devices starting TabTip don't show keyboard, on some does  ¯\_(ツ)_/¯
            if (!IsOpen())
            {
                ToggleVisibility();
            }
        }

        public static void Hide()
        {
            if (IsOpen())
            {
                ToggleVisibility();
            }
        }


        public static bool Close()
        {
            // find it
            int handle = NativeMethods.FindWindow("IPTIP_Main_Window", "");
            bool active = handle > 0;
            if (active)
            {
                // don't check style - just close
                NativeMethods.SendMessage(handle, NativeMethods.WM_SYSCOMMAND, NativeMethods.SC_CLOSE, 0);
            }
            return active;
        }

        public static bool IsOpen()
        {
            return GetIsOpen1709() ?? GetIsOpenLegacy();
        }


        [DllImport("user32.dll", SetLastError = false)]
        private static extern IntPtr FindWindowEx(IntPtr parent, IntPtr after, string className, string title = null);

        [DllImport("user32.dll", SetLastError = false)]
        private static extern uint GetWindowLong(IntPtr wnd, int index);

        private static bool? GetIsOpen1709()
        {
            // if there is a top-level window - the keyboard is closed
            var wnd = FindWindowEx(IntPtr.Zero, IntPtr.Zero, WindowClass1709, WindowCaption1709);
            if (wnd != IntPtr.Zero)
                return false;

            var parent = IntPtr.Zero;
            for (; ; )
            {
                parent = FindWindowEx(IntPtr.Zero, parent, WindowParentClass1709);
                if (parent == IntPtr.Zero)
                    return null; // no more windows, keyboard state is unknown

                // if it's a child of a WindowParentClass1709 window - the keyboard is open
                wnd = FindWindowEx(parent, IntPtr.Zero, WindowClass1709, WindowCaption1709);
                if (wnd != IntPtr.Zero)
                    return true;
            }
        }

        private static bool GetIsOpenLegacy()
        {
            var wnd = FindWindowEx(IntPtr.Zero, IntPtr.Zero, WindowClass);
            if (wnd == IntPtr.Zero)
                return false;

            var style = GetWindowStyle(wnd);
            return style.HasFlag(WindowStyle.Visible)
                && !style.HasFlag(WindowStyle.Disabled);
        }

        private const string WindowClass = "IPTip_Main_Window";
        private const string WindowParentClass1709 = "ApplicationFrameWindow";
        private const string WindowClass1709 = "Windows.UI.Core.CoreWindow";
        private const string WindowCaption1709 = "Microsoft Text Input Application";

        private enum WindowStyle : uint
        {
            Disabled = 0x08000000,
            Visible = 0x10000000,
        }

        private static WindowStyle GetWindowStyle(IntPtr wnd)
        {
            return (WindowStyle)GetWindowLong(wnd, -16);
        }

    }


    [ComImport]
    [Guid("37c994e7-432b-4834-a2f7-dce1f13b834b")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface ITipInvocation
    {
        void Toggle(IntPtr hwnd);
    }

    internal static class NativeMethods
    {
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        internal static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        internal static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow", SetLastError = false)]
        internal static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        internal static extern int GetWindowLong(int hWnd, int nIndex);

        internal const int GWL_STYLE = -16;
        internal const int GWL_EXSTYLE = -20;
        internal const int WM_SYSCOMMAND = 0x0112;
        internal const int SC_CLOSE = 0xF060;

        internal const int WS_DISABLED = 0x08000000;

        internal const int WS_VISIBLE = 0x10000000;

    }
}