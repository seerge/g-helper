using System.Collections;
using System.Runtime.InteropServices;
using static GHelper.Display.ScreenInterrogatory;

namespace GHelper.Display
{

    class DeviceComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            uint displayX = ((DISPLAYCONFIG_TARGET_DEVICE_NAME)x).connectorInstance;
            uint displayY = ((DISPLAYCONFIG_TARGET_DEVICE_NAME)y).connectorInstance;

            if (displayX > displayY)
                return 1;
            if (displayX < displayY)
                return -1;
            else
                return 0;
        }
    }

    class ScreenComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            int displayX = Int32.Parse(((Screen)x).DeviceName.Replace(@"\\.\DISPLAY", ""));
            int displayY = Int32.Parse(((Screen)y).DeviceName.Replace(@"\\.\DISPLAY", ""));
            return (new CaseInsensitiveComparer()).Compare(displayX, displayY);
        }
    }
    internal class ScreenNative
    {

        [DllImport("gdi32", CharSet = CharSet.Unicode)]
        internal static extern IntPtr CreateDC(string driver, string device, string port, IntPtr deviceMode);

        [DllImport("gdi32")]
        internal static extern bool SetDeviceGammaRamp(IntPtr dcHandle, ref GammaRamp ramp);

        [DllImport("gdi32")]
        internal static extern bool GetDeviceGammaRamp(IntPtr dcHandle, ref GammaRamp ramp);

        [DllImport("gdi32", CharSet = CharSet.Unicode)]
        internal static extern bool SetICMProfileW(IntPtr dcHandle, string lpFileName);

        [DllImport("gdi32", CharSet = CharSet.Unicode)]
        internal static extern bool SetICMMode(IntPtr dcHandle, int mode);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;

            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public int dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;

            public short dmLogPixels;
            public short dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        };

        [Flags()]
        public enum DisplaySettingsFlags : int
        {
            CDS_UPDATEREGISTRY = 1,
            CDS_TEST = 2,
            CDS_FULLSCREEN = 4,
            CDS_GLOBAL = 8,
            CDS_SET_PRIMARY = 0x10,
            CDS_RESET = 0x40000000,
            CDS_NORESET = 0x10000000
        }

        // PInvoke declaration for EnumDisplaySettings Win32 API
        [DllImport("user32.dll")]
        public static extern int EnumDisplaySettingsEx(
             string lpszDeviceName,
             int iModeNum,
             ref DEVMODE lpDevMode);

        // PInvoke declaration for ChangeDisplaySettings Win32 API
        [DllImport("user32.dll")]
        public static extern int ChangeDisplaySettingsEx(
                string lpszDeviceName, ref DEVMODE lpDevMode, IntPtr hwnd,
                DisplaySettingsFlags dwflags, IntPtr lParam);

        public static DEVMODE CreateDevmode()
        {
            DEVMODE dm = new DEVMODE();
            dm.dmDeviceName = new String(new char[32]);
            dm.dmFormName = new String(new char[32]);
            dm.dmSize = (short)Marshal.SizeOf(dm);
            return dm;
        }

        public enum COLORPROFILETYPE
        {
            CPT_ICC,
            CPT_DMP,
            CPT_CAMP,
            CPT_GMMP
        }
        public enum COLORPROFILESUBTYPE
        {
            CPST_PERCEPTUAL,
            CPST_RELATIVE_COLORIMETRIC,
            CPST_SATURATION,
            CPST_ABSOLUTE_COLORIMETRIC,
            CPST_NONE,
            CPST_RGB_WORKING_SPACE,
            CPST_CUSTOM_WORKING_SPACE,
            CPST_STANDARD_DISPLAY_COLOR_MODE,
            CPST_EXTENDED_DISPLAY_COLOR_MODE
        }
        public enum WCS_PROFILE_MANAGEMENT_SCOPE
        {
            WCS_PROFILE_MANAGEMENT_SCOPE_SYSTEM_WIDE,
            WCS_PROFILE_MANAGEMENT_SCOPE_CURRENT_USER
        }

        [DllImport("mscms.dll", CharSet = CharSet.Unicode)]
        public static extern bool WcsSetDefaultColorProfile(
            WCS_PROFILE_MANAGEMENT_SCOPE scope,
            string pDeviceName,
            COLORPROFILETYPE cptColorProfileType,
            COLORPROFILESUBTYPE cpstColorProfileSubType,
            uint dwProfileID,
            string pProfileName
        );


        public const int ENUM_CURRENT_SETTINGS = -1;
        public const string defaultDevice = @"\\.\DISPLAY1";


        public static string? FindInternalName(bool log = false)
        {
            try
            {
                var devicesList = GetAllDevices();
                var devices = devicesList.ToArray();
                string internalName = AppConfig.GetString("internal_display");

                foreach (var device in devices)
                {
                    if (device.outputTechnology == DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY.DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INTERNAL ||
                        device.outputTechnology == DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY.DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EMBEDDED ||
                        device.monitorFriendlyDeviceName == internalName)
                    {
                        if (log) Logger.WriteLine(device.monitorDevicePath + " " + device.outputTechnology);

                        AppConfig.Set("internal_display", device.monitorFriendlyDeviceName);
                        var names = device.monitorDevicePath.Split("#");
                        
                        if (names.Length > 1) return names[1];
                        else return "";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.ToString());
            }

            return null;
        }

        static string ExtractDisplay(string input)
        {
            int index = input.IndexOf('\\', 4); // Start searching from index 4 to skip ""

            if (index != -1)
            {
                string extracted = input.Substring(0, index);
                return extracted;
            }

            return input;
        }

        public static string? FindLaptopScreen(bool log = false)
        {
            string? laptopScreen = null;
            string? internalName = FindInternalName(log);

            if (internalName == null)
            {
                Logger.WriteLine("Internal screen off");
                return null;
            }

            try
            {
                var displays = GetDisplayDevices().ToArray();
                foreach (var display in displays)
                {
                    if (log) Logger.WriteLine(display.DeviceID + " " + display.DeviceName);
                    if (display.DeviceID.Contains(internalName))
                    {
                        laptopScreen = ExtractDisplay(display.DeviceName);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.ToString());
            }

            if (laptopScreen is null)
            {
                Logger.WriteLine("Default internal screen");
                laptopScreen = Screen.PrimaryScreen.DeviceName;
            }

            return laptopScreen;
        }


        public static int GetMaxRefreshRate(string? laptopScreen)
        {

            if (laptopScreen is null) return -1;

            DEVMODE dm = CreateDevmode();
            int frequency = -1;

            int i = 0;
            while (0 != EnumDisplaySettingsEx(laptopScreen, i, ref dm))
            {
                if (dm.dmDisplayFrequency > frequency) frequency = dm.dmDisplayFrequency;
                i++;
            }

            if (frequency > 0) AppConfig.Set("screen_max", frequency);
            else frequency = AppConfig.Get("screen_max");

            return frequency;

        }

        public static int GetRefreshRate(string? laptopScreen)
        {

            if (laptopScreen is null) return -1;

            DEVMODE dm = CreateDevmode();
            int frequency = -1;

            if (0 != EnumDisplaySettingsEx(laptopScreen, ENUM_CURRENT_SETTINGS, ref dm))
            {
                frequency = dm.dmDisplayFrequency;
            }

            return frequency;
        }

        public static int SetRefreshRate(string laptopScreen, int frequency = 120)
        {
            DEVMODE dm = CreateDevmode();

            if (0 != EnumDisplaySettingsEx(laptopScreen, ENUM_CURRENT_SETTINGS, ref dm))
            {
                dm.dmDisplayFrequency = frequency;
                int iRet = ChangeDisplaySettingsEx(laptopScreen, ref dm, IntPtr.Zero, DisplaySettingsFlags.CDS_UPDATEREGISTRY, IntPtr.Zero);
                Logger.WriteLine("Screen = " + frequency.ToString() + "Hz : " + (iRet == 0 ? "OK" : iRet));
                return iRet;
            }

            return 0;

        }
    }
}
