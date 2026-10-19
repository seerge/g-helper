using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace GHelper.Theme
{
    /// <summary>
    /// Reads and writes the Windows light/dark theme. Port of the owner's
    /// theme_manager.ps1, including its hard-won lesson: if the broadcast does not
    /// go out, the registry write looks like it did nothing, so every apply is
    /// logged with the broadcast's result.
    /// </summary>
    public static class WindowsTheme
    {
        private const string PersonalizeKey = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

        private const uint WM_SETTINGCHANGE = 0x001A;
        private const uint SMTO_ABORTIFHUNG = 0x0002;
        private const uint BroadcastTimeoutMs = 5000;
        private static readonly IntPtr HWND_BROADCAST = (IntPtr)0xFFFF;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint msg, IntPtr wParam, string lParam, uint flags, uint timeout, out IntPtr result);

        /// <summary>
        /// True when the current Windows theme is light. A missing or non-int
        /// value reads as light, which is Windows' own default and matches
        /// RForm.IsDarkTheme().
        /// </summary>
        public static bool IsLight()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(PersonalizeKey);
                return key?.GetValue("AppsUseLightTheme") is not int value || value > 0;
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Theme: could not read the current theme, assuming light ({ex.Message})");
                return true;
            }
        }

        /// <summary>
        /// Sets both registry values and tells the shell and running apps to
        /// repaint. Logs and swallows any failure; never throws.
        /// </summary>
        public static void Apply(bool light)
        {
            try
            {
                using var key = Registry.CurrentUser.CreateSubKey(PersonalizeKey);
                key.SetValue("SystemUsesLightTheme", light ? 1 : 0, RegistryValueKind.DWord);
                key.SetValue("AppsUseLightTheme", light ? 1 : 0, RegistryValueKind.DWord);

                IntPtr broadcast = SendMessageTimeout(HWND_BROADCAST, WM_SETTINGCHANGE, IntPtr.Zero,
                    "ImmersiveColorSet", SMTO_ABORTIFHUNG, BroadcastTimeoutMs, out _);

                Logger.WriteLine($"Theme: applied {(light ? "light" : "dark")}, broadcast {(broadcast != IntPtr.Zero ? "acknowledged" : "NOT acknowledged - the shell may not repaint")}");
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Theme: failed to apply {(light ? "light" : "dark")} ({ex.Message})");
            }
        }

        /// <summary>
        /// Flips the theme. Returns the theme it set, true for light.
        /// </summary>
        public static bool Toggle()
        {
            bool light = !IsLight();
            Apply(light);
            return light;
        }
    }
}
