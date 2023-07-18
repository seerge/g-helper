using System.Runtime.InteropServices;
using static GHelper.Display.ScreenInterrogatory;

namespace GHelper.Display
{
    internal class ScreenNative
    {
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

        static bool? _ultimate = null;

        static bool isUltimate
        {
            get
            {
                if (_ultimate is null) _ultimate = (Program.acpi.DeviceGet(AsusACPI.GPUMux) == 0);
                return (bool)_ultimate;
            }
        }

        public static string? FindLaptopScreen(bool log = false)
        {
            string? laptopScreen = null;
            var screens = Screen.AllScreens;

            if (!isUltimate)
            {
                foreach (var screen in screens )
                {
                    if (log) Logger.WriteLine(screen.DeviceName);
                    if (screen.DeviceName == defaultDevice) return defaultDevice;
                }
            }

            try
            {
                var devices = GetAllDevices().ToArray();

                int count = 0, displayNum = -1;

                string internalName = AppConfig.GetString("internal_display");

                foreach (var device in devices)
                {
                    if (device.outputTechnology == DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY.DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INTERNAL ||
                        device.outputTechnology == DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY.DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EMBEDDED ||
                        device.monitorFriendlyDeviceName == internalName)
                    {
                        displayNum = count;
                        AppConfig.Set("internal_display", device.monitorFriendlyDeviceName);
                    }
                    if (log) Logger.WriteLine(device.monitorFriendlyDeviceName + ":" + device.outputTechnology.ToString() + ": " + ((count < screens.Length) ? screens[count].DeviceName : ""));
                    count++;
                }

                count = 0;
                foreach (var screen in screens)
                {
                    if (count == displayNum) laptopScreen = screen.DeviceName;
                    count++;
                }

                if (displayNum > 0 && count == 0) laptopScreen = defaultDevice;
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.ToString());
                Logger.WriteLine("Can't detect internal screen");
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

                //Fallback scenario
                if (iRet != 0)
                {
                    Thread.Sleep(1000);
                    iRet = ChangeDisplaySettingsEx(laptopScreen, ref dm, IntPtr.Zero, DisplaySettingsFlags.CDS_UPDATEREGISTRY, IntPtr.Zero);
                    Logger.WriteLine("Screen = " + frequency.ToString() + "Hz : " + (iRet == 0 ? "OK" : iRet));
                }

                return iRet;
            }

            return 0;

        }
    }
}
