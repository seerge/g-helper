using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace GHelper.UI
{
    public class RForm : Form
    {

        public static Color colorEco = Color.FromArgb(255, 6, 180, 138);
        public static Color colorStandard = Color.FromArgb(255, 58, 174, 239);
        public static Color colorTurbo = Color.FromArgb(255, 255, 32, 32);
        public static Color colorCustom = Color.FromArgb(255, 255, 128, 0);
        public static Color colorGray = Color.FromArgb(255, 168, 168, 168);


        public static Color buttonMain;
        public static Color buttonSecond;

        public static Color formBack;
        public static Color foreMain;
        public static Color borderMain;
        public static Color borderSecond;
        public static Color chartMain;
        public static Color chartGrid;

        public static bool flatTheme = false;

        // Liquid glass theme: translucent acrylic window, frosted cards, dark palette.
        // On by default (Windows 10 1803+). Set "theme" to "classic" or "flat" in config.json to turn it off.
        public static bool glassTheme = false;
        public static Color glassCard;
        public static Color glassHighlight;

        public static bool GlassSupported => Environment.OSVersion.Version.Build >= 17134;

        public static bool IsGlassTheme()
        {
            if (!GlassSupported) return false;
            string? theme = AppConfig.GetString("theme")?.ToLower();
            return string.IsNullOrEmpty(theme) || theme == "glass";
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct AccentPolicy
        {
            public int AccentState;
            public int AccentFlags;
            public uint GradientColor;
            public int AnimationId;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WindowCompositionAttributeData
        {
            public int Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        [DllImport("user32.dll")]
        private static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        private const int WCA_ACCENT_POLICY = 19;
        private const int ACCENT_ENABLE_ACRYLICBLURBEHIND = 4;
        private const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
        private const int DWMWA_BORDER_COLOR = 34;
        private const int DWMWCP_ROUND = 2;

        // Tint laid over the blurred desktop, AABBGGRR
        private const uint GlassTint = 0x8C1A1410;

        /// <summary>
        /// Turns the window into frosted glass. GDI paints with alpha 0, so DWM composites
        /// the client area additively: black is fully clear, dark greys become a light frost.
        /// </summary>
        protected void ApplyGlass()
        {
            if (!glassTheme || !IsHandleCreated) return;

            var accent = new AccentPolicy
            {
                AccentState = ACCENT_ENABLE_ACRYLICBLURBEHIND,
                AccentFlags = 2,
                GradientColor = GlassTint
            };

            int size = Marshal.SizeOf(accent);
            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(accent, ptr, false);
                var data = new WindowCompositionAttributeData
                {
                    Attribute = WCA_ACCENT_POLICY,
                    Data = ptr,
                    SizeOfData = size
                };
                SetWindowCompositionAttribute(Handle, ref data);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            // Windows 11: rounded corners and a hairline glass edge (ignored on Windows 10)
            DwmSetWindowAttribute(Handle, DWMWA_WINDOW_CORNER_PREFERENCE, new[] { DWMWCP_ROUND }, 4);
            DwmSetWindowAttribute(Handle, DWMWA_BORDER_COLOR, new[] { ColorTranslator.ToWin32(Color.FromArgb(78, 78, 90)) }, 4);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (themeInitialized) ApplyGlass();
        }

        [DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#138")]
        public static extern bool CheckSystemDarkModeStatus();

        [DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#135")]
        private static extern int SetPreferredAppMode(int preferredAppMode);

        [DllImport("UXTheme.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(nint hWnd, string pszSubAppName, string? pszSubIdList);

        [DllImport("DwmApi")] //System.Runtime.InteropServices
        private static extern int DwmSetWindowAttribute(nint hwnd, int attr, int[] attrValue, int attrSize);

        public bool darkTheme = false;
        private bool themeInitialized = false;
        protected override CreateParams CreateParams
        {
            get
            {
                var parms = base.CreateParams;
                parms.Style &= ~0x02000000;  // Turn off WS_CLIPCHILDREN
                parms.ClassStyle &= ~0x00020000;
                return parms;
            }
        }
        public static void InitColors(bool darkTheme)
        {
            flatTheme = AppConfig.GetString("theme")?.ToLower() == "flat";
            glassTheme = IsGlassTheme();

            if (glassTheme)
            {
                // Pure black = fully see-through glass; dark greys = layered frost
                formBack = Color.Black;
                glassCard = Color.FromArgb(255, 30, 31, 40);
                glassHighlight = Color.FromArgb(255, 255, 255, 255);

                buttonMain = Color.FromArgb(255, 50, 52, 64);
                buttonSecond = Color.FromArgb(255, 42, 44, 56);

                foreMain = Color.FromArgb(255, 246, 247, 252);
                borderMain = Color.FromArgb(255, 72, 74, 88);
                borderSecond = Color.FromArgb(255, 58, 60, 72);

                chartMain = Color.FromArgb(255, 18, 18, 22);
                chartGrid = Color.FromArgb(255, 62, 62, 72);
            }
            else if (darkTheme)
            {
                buttonMain = Color.FromArgb(255, 46, 46, 46);
                buttonSecond = Color.FromArgb(255, 36, 36, 36);

                formBack = Color.FromArgb(255, 28, 28, 28);
                foreMain = Color.FromArgb(255, 240, 240, 240);
                borderMain = Color.FromArgb(255, 55, 55, 55);
                borderSecond = Color.FromArgb(255, 42, 42, 42);

                chartMain = Color.FromArgb(255, 35, 35, 35);
                chartGrid = Color.FromArgb(255, 70, 70, 70);
            }
            else
            {
                buttonMain = SystemColors.ControlLightLight;
                buttonSecond = SystemColors.ControlLight;

                formBack = SystemColors.Control;
                foreMain = SystemColors.ControlText;
                borderMain = Color.FromArgb(255, 220, 220, 220);
                borderSecond = Color.FromArgb(255, 215, 215, 215);

                chartMain = SystemColors.ControlLightLight;
                chartGrid = Color.LightGray;
            }
        }

        private static bool IsDarkTheme()
        {
            // Glass relies on additive compositing, which only reads well on a dark palette
            if (IsGlassTheme()) return true;

            string? uiMode = AppConfig.GetString("ui_mode");

            if (uiMode is not null && uiMode.ToLower() == "dark")
            {
                return true;
            }

            if (uiMode is not null && uiMode.ToLower() == "light")
            {
                return false;
            }

            if (uiMode is not null && uiMode.ToLower() == "windows")
            {
                return CheckSystemDarkModeStatus();
            }

            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            var registryValueObject = key?.GetValue("AppsUseLightTheme");

            if (registryValueObject == null) return false;
            return (int)registryValueObject <= 0;
        }

        public bool InitTheme(bool setDPI = false)
        {
            bool newDarkTheme = IsDarkTheme();
            bool changed = darkTheme != newDarkTheme;
            bool firstInit = !themeInitialized;
            darkTheme = newDarkTheme;
            themeInitialized = true;

            InitColors(darkTheme);

            if (setDPI)
                ControlHelper.Resize(this);

            if (changed || firstInit)
            {
                DwmSetWindowAttribute(Handle, 20, new[] { darkTheme ? 1 : 0 }, 4);
                SetPreferredAppMode(darkTheme ? 1 : 0); 
                SetWindowTheme(Handle, darkTheme ? "DarkMode_Explorer" : "Explorer", null);
                ControlHelper.Adjust(this, changed);
                ApplyGlass();
                this.Invalidate();
            }


            return changed;

        }

    }
}
