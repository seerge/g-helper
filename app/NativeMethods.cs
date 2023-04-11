using GHelper;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static Tools.ScreenInterrogatory;

namespace Tools
{

    public class KeyHandler
    {

        public const int NOMOD = 0x0000;
        public const int ALT = 0x0001;
        public const int CTRL = 0x0002;
        public const int SHIFT = 0x0004;
        public const int WIN = 0x0008;

        public const int WM_HOTKEY_MSG_ID = 0x0312;


        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private int modifier;
        private int key;
        private IntPtr hWnd;
        private int id;
        public KeyHandler(int modifier, Keys key, nint handle)
        {
            this.modifier = modifier;
            this.key = (int)key;
            this.hWnd = handle;
            id = this.GetHashCode();
        }
        public override int GetHashCode()
        {
            return modifier ^ key ^ hWnd.ToInt32();
        }
        public bool Register()
        {
            return RegisterHotKey(hWnd, id, modifier, key);
        }
        public bool Unregiser()
        {
            return UnregisterHotKey(hWnd, id);
        }
    }

    public static class ScreenInterrogatory
    {
        public const int ERROR_SUCCESS = 0;

        #region enums

        public enum QUERY_DEVICE_CONFIG_FLAGS : uint
        {
            QDC_ALL_PATHS = 0x00000001,
            QDC_ONLY_ACTIVE_PATHS = 0x00000002,
            QDC_DATABASE_CURRENT = 0x00000004
        }

        public enum DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY : uint
        {
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_OTHER = 0xFFFFFFFF,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HD15 = 0,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SVIDEO = 1,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPOSITE_VIDEO = 2,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPONENT_VIDEO = 3,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DVI = 4,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HDMI = 5,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_LVDS = 6,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_D_JPN = 8,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDI = 9,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EXTERNAL = 10,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EMBEDDED = 11,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EXTERNAL = 12,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EMBEDDED = 13,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDTVDONGLE = 14,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_MIRACAST = 15,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INTERNAL = 0x80000000,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_FORCE_UINT32 = 0xFFFFFFFF
        }

        public enum DISPLAYCONFIG_SCANLINE_ORDERING : uint
        {
            DISPLAYCONFIG_SCANLINE_ORDERING_UNSPECIFIED = 0,
            DISPLAYCONFIG_SCANLINE_ORDERING_PROGRESSIVE = 1,
            DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED = 2,
            DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_UPPERFIELDFIRST = DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED,
            DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_LOWERFIELDFIRST = 3,
            DISPLAYCONFIG_SCANLINE_ORDERING_FORCE_UINT32 = 0xFFFFFFFF
        }

        public enum DISPLAYCONFIG_ROTATION : uint
        {
            DISPLAYCONFIG_ROTATION_IDENTITY = 1,
            DISPLAYCONFIG_ROTATION_ROTATE90 = 2,
            DISPLAYCONFIG_ROTATION_ROTATE180 = 3,
            DISPLAYCONFIG_ROTATION_ROTATE270 = 4,
            DISPLAYCONFIG_ROTATION_FORCE_UINT32 = 0xFFFFFFFF
        }

        public enum DISPLAYCONFIG_SCALING : uint
        {
            DISPLAYCONFIG_SCALING_IDENTITY = 1,
            DISPLAYCONFIG_SCALING_CENTERED = 2,
            DISPLAYCONFIG_SCALING_STRETCHED = 3,
            DISPLAYCONFIG_SCALING_ASPECTRATIOCENTEREDMAX = 4,
            DISPLAYCONFIG_SCALING_CUSTOM = 5,
            DISPLAYCONFIG_SCALING_PREFERRED = 128,
            DISPLAYCONFIG_SCALING_FORCE_UINT32 = 0xFFFFFFFF
        }

        public enum DISPLAYCONFIG_PIXELFORMAT : uint
        {
            DISPLAYCONFIG_PIXELFORMAT_8BPP = 1,
            DISPLAYCONFIG_PIXELFORMAT_16BPP = 2,
            DISPLAYCONFIG_PIXELFORMAT_24BPP = 3,
            DISPLAYCONFIG_PIXELFORMAT_32BPP = 4,
            DISPLAYCONFIG_PIXELFORMAT_NONGDI = 5,
            DISPLAYCONFIG_PIXELFORMAT_FORCE_UINT32 = 0xffffffff
        }

        public enum DISPLAYCONFIG_MODE_INFO_TYPE : uint
        {
            DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE = 1,
            DISPLAYCONFIG_MODE_INFO_TYPE_TARGET = 2,
            DISPLAYCONFIG_MODE_INFO_TYPE_FORCE_UINT32 = 0xFFFFFFFF
        }

        public enum DISPLAYCONFIG_DEVICE_INFO_TYPE : uint
        {
            DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME = 1,
            DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME = 2,
            DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_PREFERRED_MODE = 3,
            DISPLAYCONFIG_DEVICE_INFO_GET_ADAPTER_NAME = 4,
            DISPLAYCONFIG_DEVICE_INFO_SET_TARGET_PERSISTENCE = 5,
            DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_BASE_TYPE = 6,
            DISPLAYCONFIG_DEVICE_INFO_FORCE_UINT32 = 0xFFFFFFFF
        }

        #endregion

        #region structs

        [StructLayout(LayoutKind.Sequential)]
        public struct LUID
        {
            public uint LowPart;
            public int HighPart;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_PATH_SOURCE_INFO
        {
            public LUID adapterId;
            public uint id;
            public uint modeInfoIdx;
            public uint statusFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_PATH_TARGET_INFO
        {
            public LUID adapterId;
            public uint id;
            public uint modeInfoIdx;
            private DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY outputTechnology;
            private DISPLAYCONFIG_ROTATION rotation;
            private DISPLAYCONFIG_SCALING scaling;
            private DISPLAYCONFIG_RATIONAL refreshRate;
            private DISPLAYCONFIG_SCANLINE_ORDERING scanLineOrdering;
            public bool targetAvailable;
            public uint statusFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_RATIONAL
        {
            public uint Numerator;
            public uint Denominator;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_PATH_INFO
        {
            public DISPLAYCONFIG_PATH_SOURCE_INFO sourceInfo;
            public DISPLAYCONFIG_PATH_TARGET_INFO targetInfo;
            public uint flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_2DREGION
        {
            public uint cx;
            public uint cy;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_VIDEO_SIGNAL_INFO
        {
            public ulong pixelRate;
            public DISPLAYCONFIG_RATIONAL hSyncFreq;
            public DISPLAYCONFIG_RATIONAL vSyncFreq;
            public DISPLAYCONFIG_2DREGION activeSize;
            public DISPLAYCONFIG_2DREGION totalSize;
            public uint videoStandard;
            public DISPLAYCONFIG_SCANLINE_ORDERING scanLineOrdering;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_TARGET_MODE
        {
            public DISPLAYCONFIG_VIDEO_SIGNAL_INFO targetVideoSignalInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINTL
        {
            private int x;
            private int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_SOURCE_MODE
        {
            public uint width;
            public uint height;
            public DISPLAYCONFIG_PIXELFORMAT pixelFormat;
            public POINTL position;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct DISPLAYCONFIG_MODE_INFO_UNION
        {
            [FieldOffset(0)]
            public DISPLAYCONFIG_TARGET_MODE targetMode;

            [FieldOffset(0)]
            public DISPLAYCONFIG_SOURCE_MODE sourceMode;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_MODE_INFO
        {
            public DISPLAYCONFIG_MODE_INFO_TYPE infoType;
            public uint id;
            public LUID adapterId;
            public DISPLAYCONFIG_MODE_INFO_UNION modeInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS
        {
            public uint value;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_DEVICE_INFO_HEADER
        {
            public DISPLAYCONFIG_DEVICE_INFO_TYPE type;
            public uint size;
            public LUID adapterId;
            public uint id;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct DISPLAYCONFIG_TARGET_DEVICE_NAME
        {
            public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
            public DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS flags;
            public DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY outputTechnology;
            public ushort edidManufactureId;
            public ushort edidProductCodeId;
            public uint connectorInstance;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string monitorFriendlyDeviceName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string monitorDevicePath;
        }

        #endregion

        #region DLL-Imports

        [DllImport("user32.dll")]
        public static extern int GetDisplayConfigBufferSizes(
            QUERY_DEVICE_CONFIG_FLAGS flags, out uint numPathArrayElements, out uint numModeInfoArrayElements);

        [DllImport("user32.dll")]
        public static extern int QueryDisplayConfig(
            QUERY_DEVICE_CONFIG_FLAGS flags,
            ref uint numPathArrayElements, [Out] DISPLAYCONFIG_PATH_INFO[] PathInfoArray,
            ref uint numModeInfoArrayElements, [Out] DISPLAYCONFIG_MODE_INFO[] ModeInfoArray,
            IntPtr currentTopologyId
            );

        [DllImport("user32.dll")]
        public static extern int DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_TARGET_DEVICE_NAME deviceName);

        #endregion

        private static DISPLAYCONFIG_TARGET_DEVICE_NAME DeviceName(LUID adapterId, uint targetId)
        {
            var deviceName = new DISPLAYCONFIG_TARGET_DEVICE_NAME
            {
                header =
                {
                    size = (uint)Marshal.SizeOf(typeof (DISPLAYCONFIG_TARGET_DEVICE_NAME)),
                    adapterId = adapterId,
                    id = targetId,
                    type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME
                }
            };
            var error = DisplayConfigGetDeviceInfo(ref deviceName);
            if (error != ERROR_SUCCESS)
                throw new Win32Exception(error);

            return deviceName;
        }

        public static IEnumerable<DISPLAYCONFIG_TARGET_DEVICE_NAME> GetAllDevices()
        {
            uint pathCount, modeCount;
            var error = GetDisplayConfigBufferSizes(QUERY_DEVICE_CONFIG_FLAGS.QDC_ONLY_ACTIVE_PATHS, out pathCount, out modeCount);
            if (error != ERROR_SUCCESS)
                throw new Win32Exception(error);

            var displayPaths = new DISPLAYCONFIG_PATH_INFO[pathCount];
            var displayModes = new DISPLAYCONFIG_MODE_INFO[modeCount];
            error = QueryDisplayConfig(QUERY_DEVICE_CONFIG_FLAGS.QDC_ONLY_ACTIVE_PATHS,
                ref pathCount, displayPaths, ref modeCount, displayModes, IntPtr.Zero);
            if (error != ERROR_SUCCESS)
                throw new Win32Exception(error);

            for (var i = 0; i < modeCount; i++)
                if (displayModes[i].infoType == DISPLAYCONFIG_MODE_INFO_TYPE.DISPLAYCONFIG_MODE_INFO_TYPE_TARGET)
                    yield return DeviceName(displayModes[i].adapterId, displayModes[i].id);
        }


    }

}

public class NativeMethods
{

    // Monitor Power detection

    internal const uint DEVICE_NOTIFY_WINDOW_HANDLE = 0x0;
    internal const uint DEVICE_NOTIFY_SERVICE_HANDLE = 0x1;
    internal const int WM_POWERBROADCAST = 0x0218;
    internal const int PBT_POWERSETTINGCHANGE = 0x8013;

    [DllImport("User32.dll", SetLastError = true)]
    internal static extern IntPtr RegisterPowerSettingNotification(IntPtr hWnd, [In] Guid PowerSettingGuid, uint Flags);

    [DllImport("User32.dll", SetLastError = true)]
    internal static extern bool UnregisterPowerSettingNotification(IntPtr hWnd);

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct POWERBROADCAST_SETTING
    {
        public Guid PowerSetting;
        public uint DataLength;
        public byte Data;
    }

    public class PowerSettingGuid
    {
        // 0=Powered by AC, 1=Powered by Battery, 2=Powered by short-term source (UPC)
        public Guid AcdcPowerSource { get; } = new Guid("5d3e9a59-e9D5-4b00-a6bd-ff34ff516548");
        // POWERBROADCAST_SETTING.Data = 1-100
        public Guid BatteryPercentageRemaining { get; } = new Guid("a7ad8041-b45a-4cae-87a3-eecbb468a9e1");
        // Windows 8+: 0=Monitor Off, 1=Monitor On, 2=Monitor Dimmed
        public Guid ConsoleDisplayState { get; } = new Guid("6fe69556-704a-47a0-8f24-c28d936fda47");
        // Windows 8+, Session 0 enabled: 0=User providing Input, 2=User Idle
        public Guid GlobalUserPresence { get; } = new Guid("786E8A1D-B427-4344-9207-09E70BDCBEA9");
        // 0=Monitor Off, 1=Monitor On.
        public Guid MonitorPowerGuid { get; } = new Guid("02731015-4510-4526-99e6-e5a17ebd1aea");
        // 0=Battery Saver Off, 1=Battery Saver On.
        public Guid PowerSavingStatus { get; } = new Guid("E00958C0-C213-4ACE-AC77-FECCED2EEEA5");

        // Windows 8+: 0=Off, 1=On, 2=Dimmed
        public Guid SessionDisplayStatus { get; } = new Guid("2B84C20E-AD23-4ddf-93DB-05FFBD7EFCA5");

        // Windows 8+, no Session 0: 0=User providing Input, 2=User Idle
        public Guid SessionUserPresence { get; } = new Guid("3C0F4548-C03F-4c4d-B9F2-237EDE686376");
        // 0=Exiting away mode 1=Entering away mode
        public Guid SystemAwaymode { get; } = new Guid("98a7f580-01f7-48aa-9c0f-44352c29e5C0");

        /* Windows 8+ */
        // POWERBROADCAST_SETTING.Data not used
        public Guid IdleBackgroundTask { get; } = new Guid(0x515C31D8, 0xF734, 0x163D, 0xA0, 0xFD, 0x11, 0xA0, 0x8C, 0x91, 0xE8, 0xF1);

        public Guid PowerSchemePersonality { get; } = new Guid(0x245D8541, 0x3943, 0x4422, 0xB0, 0x25, 0x13, 0xA7, 0x84, 0xF6, 0x79, 0xB7);

        // The Following 3 Guids are the POWERBROADCAST_SETTING.Data result of PowerSchemePersonality
        public Guid MinPowerSavings { get; } = new Guid("8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c");
        public Guid MaxPowerSavings { get; } = new Guid("a1841308-3541-4fab-bc81-f71556f20b4a");
        public Guid TypicalPowerSavings { get; } = new Guid("381b4222-f694-41f0-9685-ff5bb260df2e");
    }


    public const int KEYEVENTF_EXTENDEDKEY = 1;
    public const int KEYEVENTF_KEYUP = 2;

    public const int VK_MEDIA_NEXT_TRACK = 0xB0;
    public const int VK_MEDIA_PLAY_PAUSE = 0xB3;
    public const int VK_MEDIA_PREV_TRACK = 0xB1;
    public const int VK_VOLUME_MUTE = 0xAD;
    public const int VK_SNAPSHOT = 0x2C;

    [DllImport("user32.dll", SetLastError = true)]
    public static extern void keybd_event(byte virtualKey, byte scanCode, uint flags, IntPtr extraInfo);

    public static void KeyPress(int key = VK_MEDIA_PLAY_PAUSE)
    {
        keybd_event((byte)key, 0, KEYEVENTF_EXTENDEDKEY, IntPtr.Zero);
    }


    [DllImport("user32.dll")]
    public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
    public const int SW_RESTORE = 9;

    public static bool SwitchToCurrent()
    {
        IntPtr hWnd = IntPtr.Zero;
        Process process = Process.GetCurrentProcess();
        Process[] processes = Process.GetProcessesByName(process.ProcessName);
        foreach (Process _process in processes)
        {
            if (_process.Id != process.Id)
            {

                if (_process.MainWindowHandle != IntPtr.Zero)
                {
                    Debug.WriteLine(_process.Id);
                    Debug.WriteLine(process.Id);

                    hWnd = _process.MainWindowHandle;
                    ShowWindowAsync(hWnd, SW_RESTORE);
                }

                return true;
                break;
            }
        }

        return false;
    }

    [DllImport("PowrProf.dll", CharSet = CharSet.Unicode)]
    static extern UInt32 PowerWriteDCValueIndex(IntPtr RootPowerKey,
        [MarshalAs(UnmanagedType.LPStruct)] Guid SchemeGuid,
        [MarshalAs(UnmanagedType.LPStruct)] Guid SubGroupOfPowerSettingsGuid,
        [MarshalAs(UnmanagedType.LPStruct)] Guid PowerSettingGuid,
        int AcValueIndex);

    [DllImport("PowrProf.dll", CharSet = CharSet.Unicode)]
    static extern UInt32 PowerWriteACValueIndex(IntPtr RootPowerKey,
        [MarshalAs(UnmanagedType.LPStruct)] Guid SchemeGuid,
        [MarshalAs(UnmanagedType.LPStruct)] Guid SubGroupOfPowerSettingsGuid,
        [MarshalAs(UnmanagedType.LPStruct)] Guid PowerSettingGuid,
        int AcValueIndex);

    [DllImport("PowrProf.dll", CharSet = CharSet.Unicode)]
    static extern UInt32 PowerReadACValueIndex(IntPtr RootPowerKey,
        [MarshalAs(UnmanagedType.LPStruct)] Guid SchemeGuid,
        [MarshalAs(UnmanagedType.LPStruct)] Guid SubGroupOfPowerSettingsGuid,
        [MarshalAs(UnmanagedType.LPStruct)] Guid PowerSettingGuid,
        out IntPtr AcValueIndex
        );

    [DllImport("PowrProf.dll", CharSet = CharSet.Unicode)]
    static extern UInt32 PowerReadDCValueIndex(IntPtr RootPowerKey,
        [MarshalAs(UnmanagedType.LPStruct)] Guid SchemeGuid,
        [MarshalAs(UnmanagedType.LPStruct)] Guid SubGroupOfPowerSettingsGuid,
        [MarshalAs(UnmanagedType.LPStruct)] Guid PowerSettingGuid,
        out IntPtr AcValueIndex
        );



    [DllImport("PowrProf.dll", CharSet = CharSet.Unicode)]
    static extern UInt32 PowerSetActiveScheme(IntPtr RootPowerKey,
        [MarshalAs(UnmanagedType.LPStruct)] Guid SchemeGuid);

    [DllImport("PowrProf.dll", CharSet = CharSet.Unicode)]
    static extern UInt32 PowerGetActiveScheme(IntPtr UserPowerKey, out IntPtr ActivePolicyGuid);

    static readonly Guid GUID_CPU = new Guid("54533251-82be-4824-96c1-47b60b740d00");
    static readonly Guid GUID_BOOST = new Guid("be337238-0d82-4146-a960-4f3749d470c7");

    [DllImportAttribute("powrprof.dll", EntryPoint = "PowerGetActualOverlayScheme")]
    public static extern uint PowerGetActualOverlayScheme(out Guid ActualOverlayGuid);

    [DllImportAttribute("powrprof.dll", EntryPoint = "PowerGetEffectiveOverlayScheme")]
    public static extern uint PowerGetEffectiveOverlayScheme(out Guid EffectiveOverlayGuid);

    [DllImportAttribute("powrprof.dll", EntryPoint = "PowerSetActiveOverlayScheme")]
    public static extern uint PowerSetActiveOverlayScheme(Guid OverlaySchemeGuid);

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

    public const int ENUM_CURRENT_SETTINGS = -1;
    public const string defaultDevice = "\\\\.\\DISPLAY1";

    public static string FindLaptopScreen()
    {
        string laptopScreen = null;

        try
        {
            var devices = GetAllDevices().ToArray();
            int count = 0, displayNum = -1;

            foreach (var device in devices)
            {
                if (device.outputTechnology == DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY.DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INTERNAL ||
                    device.outputTechnology == DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY.DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EMBEDDED)
                {
                    displayNum = count;
                }
                count++;
                //Logger.WriteLine(device.outputTechnology.ToString());
                //Logger.WriteLine(device.monitorFriendlyDeviceName);
            }

            var screens = Screen.AllScreens;

            if (screens.Length != count) return null;

            count = 0;
            foreach (var screen in screens)
            {
                if (count == displayNum)
                {
                    laptopScreen = screen.DeviceName;
                }
                //Logger.WriteLine(screen.DeviceName);
                count++;
            }
        }
        catch (Exception ex)
        {
            Logger.WriteLine(ex.ToString());
            Logger.WriteLine("Can't detect internal screen");
            //laptopScreen = Screen.PrimaryScreen.DeviceName;
        }


        return laptopScreen;
    }

    public static int GetRefreshRate(bool max = false)
    {
        DEVMODE dm = CreateDevmode();

        string laptopScreen = FindLaptopScreen();
        int frequency = -1;

        if (laptopScreen is null)
            return -1;

        if (max)
        {
            int i = 0;
            while (0 != NativeMethods.EnumDisplaySettingsEx(laptopScreen, i, ref dm))
            {
                if (dm.dmDisplayFrequency > frequency) frequency = dm.dmDisplayFrequency;
                i++;
            }
        }
        else
        {
            if (0 != NativeMethods.EnumDisplaySettingsEx(laptopScreen, NativeMethods.ENUM_CURRENT_SETTINGS, ref dm))
            {
                frequency = dm.dmDisplayFrequency;
            }
        }


        return frequency;
    }

    public static int SetRefreshRate(int frequency = 120)
    {
        DEVMODE dm = CreateDevmode();
        string laptopScreen = FindLaptopScreen();

        if (laptopScreen is null)
            return -1;

        if (0 != NativeMethods.EnumDisplaySettingsEx(laptopScreen, NativeMethods.ENUM_CURRENT_SETTINGS, ref dm))
        {
            dm.dmDisplayFrequency = frequency;
            int iRet = NativeMethods.ChangeDisplaySettingsEx(laptopScreen, ref dm, IntPtr.Zero, DisplaySettingsFlags.CDS_UPDATEREGISTRY, IntPtr.Zero);
            Logger.WriteLine("Screen = " + frequency.ToString() + "Hz : " + (iRet == 0 ? "OK" : iRet));
            return iRet;
        }

        return 0;

    }

    static Guid GetActiveScheme()
    {
        IntPtr pActiveSchemeGuid;
        var hr = PowerGetActiveScheme(IntPtr.Zero, out pActiveSchemeGuid);
        Guid activeSchemeGuid = (Guid)Marshal.PtrToStructure(pActiveSchemeGuid, typeof(Guid));
        return activeSchemeGuid;
    }

    public static int GetCPUBoost()
    {
        IntPtr AcValueIndex;
        Guid activeSchemeGuid = GetActiveScheme();

        UInt32 value = PowerReadACValueIndex(IntPtr.Zero,
             activeSchemeGuid,
             GUID_CPU,
             GUID_BOOST, out AcValueIndex);

        return AcValueIndex.ToInt32();

    }

    public static void SetCPUBoost(int boost = 0)
    {
        Guid activeSchemeGuid = GetActiveScheme();

        var hrAC = PowerWriteACValueIndex(
             IntPtr.Zero,
             activeSchemeGuid,
             GUID_CPU,
             GUID_BOOST,
             boost);

        PowerSetActiveScheme(IntPtr.Zero, activeSchemeGuid);

        /*
        var hrDC = PowerWriteDCValueIndex(
             IntPtr.Zero,
             activeSchemeGuid,
             GUID_CPU,
             GUID_BOOST,
             boost);

        PowerSetActiveScheme(IntPtr.Zero, activeSchemeGuid);
        */

        Logger.WriteLine("Boost " + boost);
    }

    public static void SetPowerScheme(string scheme)
    {
        PowerSetActiveScheme(IntPtr.Zero, new Guid(scheme));
        PowerSetActiveOverlayScheme(new Guid(scheme));
    }

    public static void SetPowerScheme(int mode)
    {
        switch (mode)
        {
            case 0: // balanced
                PowerSetActiveOverlayScheme(new Guid("00000000-0000-0000-0000-000000000000"));
                break;
            case 1: // turbo
                PowerSetActiveOverlayScheme(new Guid("ded574b5-45a0-4f42-8737-46345c09c238"));
                break;
            case 2: //silent
                PowerSetActiveOverlayScheme(new Guid("961cc777-2547-4f9d-8174-7d86181b8a7a"));
                break;
        }
    }
}
