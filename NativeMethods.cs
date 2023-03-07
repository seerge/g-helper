using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;

public class NativeMethods
{

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

    // ENUM DISPLAYS
    [DllImport("user32.dll")]
    static extern bool EnumDisplayDevicesA(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

    [Flags()]
    public enum DisplayDeviceStateFlags : int
    {
        AttachedToDesktop = 0x1,
        MultiDriver = 0x2,
        PrimaryDevice = 0x4,
        MirroringDriver = 0x8,
        VGACompatible = 0x10,
        Removable = 0x20,
        ModesPruned = 0x8000000,
        Remote = 0x4000000,
        Disconnect = 0x2000000
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DISPLAY_DEVICE
    {
        [MarshalAs(UnmanagedType.U4)]
        public int cb;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DeviceName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceString;
        [MarshalAs(UnmanagedType.U4)]
        public DisplayDeviceStateFlags StateFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceKey;
    }

    // ----

    public static DEVMODE CreateDevmode()
    {
        DEVMODE dm = new DEVMODE();
        dm.dmDeviceName = new String(new char[32]);
        dm.dmFormName = new String(new char[32]);
        dm.dmSize = (short)Marshal.SizeOf(dm);
        return dm;
    }

    public const int ENUM_CURRENT_SETTINGS = -1;
    public const string laptopScreenName = "\\\\.\\DISPLAY1";

    public static string FindLaptopScreen()
    {
        string laptopScreen = null;

        DISPLAY_DEVICE d = new DISPLAY_DEVICE();
        d.cb = Marshal.SizeOf(d);

        List<string> activeScreens = new List<string>();
        int counter = 0;
        int deviceNum = -1;

        try
        {
            var searcherActive = new ManagementObjectSearcher(@"\\.\root\wmi", "SELECT * FROM WmiMonitorBasicDisplayParams");
            var resultsActive = searcherActive.Get();
            foreach (var result in resultsActive)
            {
                activeScreens.Add(result["InstanceName"].ToString());
            }

            var searcher = new ManagementObjectSearcher(@"\\.\root\wmi", "SELECT * FROM WmiMonitorConnectionParams");
            var results = searcher.Get();


            foreach (var result in results)
            {
                long technology;
                long.TryParse(result["VideoOutputTechnology"].ToString(), out technology);
                string instanceName = result["InstanceName"].ToString();

                if (technology == 0x80000000 && activeScreens.Contains(instanceName))
                {
                    deviceNum = counter;
                    //Debug.WriteLine(result["InstanceName"]);
                }
                counter++;
            }
        } catch
        {
            Debug.WriteLine("Failed to detect built in display");
            deviceNum = 0;
        }

        counter = 0;
        for (uint id = 0; EnumDisplayDevicesA(null, id, ref d, 0); id++)
        {
            if ((d.StateFlags & DisplayDeviceStateFlags.AttachedToDesktop) != 0)
            {
                if (counter == deviceNum)
                {
                    laptopScreen = d.DeviceName;
                    //Debug.WriteLine(d.DeviceID);
                    //Debug.WriteLine(d.DeviceName);
                }
                counter++;
            }

        }

        /*
        if (laptopScreen is null)
        {
            foreach (var screen in screens)
                Debug.WriteLine(screen.DeviceName);
        }
        */

        return laptopScreen;
    }

    public static int GetRefreshRate()
    {
        DEVMODE dm = CreateDevmode();

        string laptopScreen = FindLaptopScreen();
        int frequency = -1;

        if (laptopScreen is null)
            return -1;

        if (0 != NativeMethods.EnumDisplaySettingsEx(laptopScreen, NativeMethods.ENUM_CURRENT_SETTINGS, ref dm))
        {
            frequency = dm.dmDisplayFrequency;
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

        var hrDC = PowerWriteDCValueIndex(
             IntPtr.Zero,
             activeSchemeGuid,
             GUID_CPU,
             GUID_BOOST,
             boost);

        PowerSetActiveScheme(IntPtr.Zero, activeSchemeGuid);


    }
}
