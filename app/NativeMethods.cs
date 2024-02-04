using System.Runtime.InteropServices;


public class NativeMethods
{

    internal struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    }

    [DllImport("User32.dll")]
    private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

    public static TimeSpan GetIdleTime()
    {
        LASTINPUTINFO lastInPut = new LASTINPUTINFO();
        lastInPut.cbSize = (uint)Marshal.SizeOf(lastInPut);
        GetLastInputInfo(ref lastInPut);
        return TimeSpan.FromMilliseconds((uint)Environment.TickCount - lastInPut.dwTime);

    }

    private const int WM_SYSCOMMAND = 0x0112;
    private const int SC_MONITORPOWER = 0xF170;
    private const int MONITOR_OFF = 2;

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern uint FormatMessage(uint dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId, out string lpBuffer, uint nSize, IntPtr Arguments);

    public static void TurnOffScreen()
    {
        IntPtr result = SendMessage(-1, WM_SYSCOMMAND, (IntPtr)SC_MONITORPOWER, (IntPtr)MONITOR_OFF);
        if (result == IntPtr.Zero)
        {
            int error = Marshal.GetLastWin32Error();
            string message = "";
            uint formatFlags = 0x00001000 | 0x00000200 | 0x00000100 | 0x00000080;
            uint formatResult = FormatMessage(formatFlags, IntPtr.Zero, (uint)error, 0, out message, 0, IntPtr.Zero);
            if (formatResult == 0)
            {
                message = "Unknown error.";
            }
            Logger.WriteLine($"Failed to turn off screen. Error code: {error}. {message}");
        }
    }

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
        public static Guid AcdcPowerSource { get; } = new Guid("5d3e9a59-e9D5-4b00-a6bd-ff34ff516548");
        // POWERBROADCAST_SETTING.Data = 1-100
        public static Guid BatteryPercentageRemaining { get; } = new Guid("a7ad8041-b45a-4cae-87a3-eecbb468a9e1");
        // Windows 8+: 0=Monitor Off, 1=Monitor On, 2=Monitor Dimmed
        public static Guid ConsoleDisplayState { get; } = new Guid("6fe69556-704a-47a0-8f24-c28d936fda47");
        // Windows 8+, Session 0 enabled: 0=User providing Input, 2=User Idle
        public static Guid GlobalUserPresence { get; } = new Guid("786E8A1D-B427-4344-9207-09E70BDCBEA9");
        // 0=Monitor Off, 1=Monitor On.
        public static Guid MonitorPowerGuid { get; } = new Guid("02731015-4510-4526-99e6-e5a17ebd1aea");
        // 0=Battery Saver Off, 1=Battery Saver On.
        public static Guid PowerSavingStatus { get; } = new Guid("E00958C0-C213-4ACE-AC77-FECCED2EEEA5");

        // Windows 8+: 0=Off, 1=On, 2=Dimmed
        public static Guid SessionDisplayStatus { get; } = new Guid("2B84C20E-AD23-4ddf-93DB-05FFBD7EFCA5");

        // Windows 8+, no Session 0: 0=User providing Input, 2=User Idle
        public static Guid SessionUserPresence { get; } = new Guid("3C0F4548-C03F-4c4d-B9F2-237EDE686376");
        // 0=Exiting away mode 1=Entering away mode
        public static Guid SystemAwaymode { get; } = new Guid("98a7f580-01f7-48aa-9c0f-44352c29e5C0");

        /* Windows 8+ */
        // POWERBROADCAST_SETTING.Data not used
        public static Guid IdleBackgroundTask { get; } = new Guid(0x515C31D8, 0xF734, 0x163D, 0xA0, 0xFD, 0x11, 0xA0, 0x8C, 0x91, 0xE8, 0xF1);

        public static Guid PowerSchemePersonality { get; } = new Guid(0x245D8541, 0x3943, 0x4422, 0xB0, 0x25, 0x13, 0xA7, 0x84, 0xF6, 0x79, 0xB7);

        // The Following 3 Guids are the POWERBROADCAST_SETTING.Data result of PowerSchemePersonality
        public static Guid MinPowerSavings { get; } = new Guid("8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c");
        public static Guid MaxPowerSavings { get; } = new Guid("a1841308-3541-4fab-bc81-f71556f20b4a");
        public static Guid TypicalPowerSavings { get; } = new Guid("381b4222-f694-41f0-9685-ff5bb260df2e");

        public static Guid LIDSWITCH_STATE_CHANGE = new Guid("ba3e0f4d-b817-4094-a2d1-d56379e6a0f3");
    }



}
