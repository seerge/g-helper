using GHelper;
using GHelper.USB;
using System.Management;
using System.Runtime.InteropServices;

public enum AsusFan
{
    CPU = 0,
    GPU = 1,
    Mid = 2,
    XGM = 3
}

public enum AsusMode
{
    Balanced = 0,
    Turbo = 1,
    Silent = 2
}

public enum AsusGPU
{
    Eco = 0,
    Standard = 1,
    Ultimate = 2
}

public class AsusACPI
{

    const string FILE_NAME = @"\\.\\ATKACPI";
    const uint CONTROL_CODE = 0x0022240C;

    const uint DSTS = 0x53545344;
    const uint DEVS = 0x53564544;
    const uint INIT = 0x54494E49;

    public const uint UniversalControl = 0x00100021;

    public const int Airplane = 0x88;
    public const int KB_Light_Up = 0xc4;
    public const int KB_Light_Down = 0xc5;
    public const int Brightness_Down = 0x10;
    public const int Brightness_Up = 0x20;
    public const int KB_Sleep = 0x6c;
    public const int KB_DUO_PgUpDn = 0x4B;
    public const int KB_DUO_SecondDisplay = 0x6A;


    public const int Touchpad_Toggle = 0x6B;

    public const int ChargerMode = 0x0012006C;

    public const int ChargerUSB = 2;
    public const int ChargerBarrel = 1;

    public const uint CPU_Fan = 0x00110013;
    public const uint GPU_Fan = 0x00110014;
    public const uint Mid_Fan = 0x00110031;

    public const uint BatteryDischarge = 0x0012005A;

    public const uint PerformanceMode = 0x00120075; // Performance modes
    public const uint VivoBookMode = 0x00110019; // Vivobook performance modes

    public const uint GPUEco = 0x00090020;

    public const uint GPUXGConnected = 0x00090018;
    public const uint GPUXG = 0x00090019;

    public const uint GPUMux = 0x00090016;
    public const uint GPUMuxVivo = 0x00090026;

    public const uint BatteryLimit = 0x00120057;

    public const uint ScreenOverdrive = 0x00050019;
    public const uint ScreenMiniled1 = 0x0005001E;
    public const uint ScreenMiniled2 = 0x0005002E;

    public const uint DevsCPUFan = 0x00110022;
    public const uint DevsGPUFan = 0x00110023;

    public const uint DevsCPUFanCurve = 0x00110024;
    public const uint DevsGPUFanCurve = 0x00110025;
    public const uint DevsMidFanCurve = 0x00110032;

    public const int Temp_CPU = 0x00120094;
    public const int Temp_GPU = 0x00120097;

    public const int PPT_APUA0 = 0x001200A0;  // sPPT (slow boost limit) / PL2
    public const int PPT_EDCA1 = 0x001200A1;  // CPU EDC
    public const int PPT_TDCA2 = 0x001200A2;  // CPU TDC
    public const int PPT_APUA3 = 0x001200A3;  // SPL (sustained limit) / PL1

    public const int PPT_CPUB0 = 0x001200B0;  // CPU PPT on 2022 (PPT_LIMIT_APU)
    public const int PPT_CPUB1 = 0x001200B1;  // Total PPT on 2022 (PPT_LIMIT_SLOW)

    public const int PPT_GPUC0 = 0x001200C0;  // NVIDIA GPU Boost
    public const int PPT_APUC1 = 0x001200C1;  // fPPT (fast boost limit)
    public const int PPT_GPUC2 = 0x001200C2;  // NVIDIA GPU Temp Target (75.. 87 C) 

    public const int APU_MEM = 0x000600C1;

    public const int TUF_KB_BRIGHTNESS = 0x00050021;
    public const int TUF_KB = 0x00100056;
    public const int TUF_KB2 = 0x0010005a;
    public const int TUF_KB_STATE = 0x00100057;

    public const int MICMUTE_LED = 0x00040017;

    public const int TabletState = 0x00060077;
    public const int FnLock = 0x00100023;

    public const int ScreenPadToggle = 0x00050031;
    public const int ScreenPadBrightness = 0x00050032;

    public const int BootSound = 0x00130022;

    public const int Tablet_Notebook = 0;
    public const int Tablet_Tablet = 1;
    public const int Tablet_Tent = 2;
    public const int Tablet_Rotated = 3;

    public const int PerformanceBalanced = 0;
    public const int PerformanceTurbo = 1;
    public const int PerformanceSilent = 2;
    public const int PerformanceManual = 4;

    public const int GPUModeEco = 0;
    public const int GPUModeStandard = 1;
    public const int GPUModeUltimate = 2;

    public const int MinTotal = 5;

    public static int MaxTotal = 150;
    public static int DefaultTotal = 80;

    public const int MinCPU = 5;
    public const int MaxCPU = 100;
    public const int DefaultCPU = 80;

    public const int MinGPUBoost = 5;
    public static int MaxGPUBoost = 25;

    public const int MinGPUTemp = 75;
    public const int MaxGPUTemp = 87;


    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr CreateFile(
        string lpFileName,
        uint dwDesiredAccess,
        uint dwShareMode,
        IntPtr lpSecurityAttributes,
        uint dwCreationDisposition,
        uint dwFlagsAndAttributes,
        IntPtr hTemplateFile
    );

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool DeviceIoControl(
        IntPtr hDevice,
        uint dwIoControlCode,
        byte[] lpInBuffer,
        uint nInBufferSize,
        byte[] lpOutBuffer,
        uint nOutBufferSize,
        ref uint lpBytesReturned,
        IntPtr lpOverlapped
    );

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);

    private const uint GENERIC_READ = 0x80000000;
    private const uint GENERIC_WRITE = 0x40000000;
    private const uint OPEN_EXISTING = 3;
    private const uint FILE_ATTRIBUTE_NORMAL = 0x80;
    private const uint FILE_SHARE_READ = 1;
    private const uint FILE_SHARE_WRITE = 2;

    private IntPtr handle;

    // Event handling attempt

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool WaitForSingleObject(IntPtr hHandle, int dwMilliseconds);

    private IntPtr eventHandle;
    private bool _connected = false;

    // still works only with asus optimization service on , if someone knows how to get ACPI events from asus without that - let me know
    public void RunListener()
    {

        eventHandle = CreateEvent(IntPtr.Zero, false, false, "ATK4001");

        byte[] outBuffer = new byte[16];
        byte[] data = new byte[8];
        bool result;

        data[0] = BitConverter.GetBytes(eventHandle.ToInt32())[0];
        data[1] = BitConverter.GetBytes(eventHandle.ToInt32())[1];

        Control(0x222400, data, outBuffer);
        Logger.WriteLine("ACPI :" + BitConverter.ToString(data) + "|" + BitConverter.ToString(outBuffer));

        while (true)
        {
            WaitForSingleObject(eventHandle, Timeout.Infinite);
            Control(0x222408, new byte[0], outBuffer);
            int code = BitConverter.ToInt32(outBuffer);
            Logger.WriteLine("ACPI Code: " + code);
        }
    }

    public bool IsConnected()
    {
        return _connected;
    }

    public AsusACPI()
    {
        try
        {
            handle = CreateFile(
                FILE_NAME,
                GENERIC_READ | GENERIC_WRITE,
                FILE_SHARE_READ | FILE_SHARE_WRITE,
                IntPtr.Zero,
                OPEN_EXISTING,
                FILE_ATTRIBUTE_NORMAL,
                IntPtr.Zero
            );

            //handle = new IntPtr(-1);
            //throw new Exception("ERROR");
            _connected = true;

        }
        catch (Exception ex)
        {
            Logger.WriteLine($"Can't connect to ACPI: {ex.Message}");
        }

        if (AppConfig.IsAdvantageEdition())
        {
            MaxTotal = 250;
        }

        if (AppConfig.IsG14AMD())
        {
            DefaultTotal = 125;
        }

        if (AppConfig.IsX13())
        {
            MaxTotal = 75;
            DefaultTotal = 50;
        }

        if (AppConfig.IsAlly())
        {
            MaxTotal = 50;
            DefaultTotal = 30;
        }

        if (AppConfig.IsIntelHX())
        {
            MaxTotal = 175;
        }

        if (AppConfig.DynamicBoost5())
        {
            MaxGPUBoost = 5;
        }

        if (AppConfig.DynamicBoost15())
        {
            MaxGPUBoost = 15;
        }
    }

    public void Control(uint dwIoControlCode, byte[] lpInBuffer, byte[] lpOutBuffer)
    {

        uint lpBytesReturned = 0;
        DeviceIoControl(
            handle,
            dwIoControlCode,
            lpInBuffer,
            (uint)lpInBuffer.Length,
            lpOutBuffer,
            (uint)lpOutBuffer.Length,
            ref lpBytesReturned,
            IntPtr.Zero
        );
    }

    public void Close()
    {
        CloseHandle(handle);
    }


    protected byte[] CallMethod(uint MethodID, byte[] args)
    {
        byte[] acpiBuf = new byte[8 + args.Length];
        byte[] outBuffer = new byte[16];

        BitConverter.GetBytes((uint)MethodID).CopyTo(acpiBuf, 0);
        BitConverter.GetBytes((uint)args.Length).CopyTo(acpiBuf, 4);
        Array.Copy(args, 0, acpiBuf, 8, args.Length);

        // if (MethodID == DEVS)  Debug.WriteLine(BitConverter.ToString(acpiBuf, 0, acpiBuf.Length));

        Control(CONTROL_CODE, acpiBuf, outBuffer);

        return outBuffer;

    }

    public byte[] DeviceInit()
    {
        byte[] args = new byte[8];
        return CallMethod(INIT, args);

    }

    public int DeviceSet(uint DeviceID, int Status, string? logName)
    {
        byte[] args = new byte[8];
        BitConverter.GetBytes((uint)DeviceID).CopyTo(args, 0);
        BitConverter.GetBytes((uint)Status).CopyTo(args, 4);

        byte[] status = CallMethod(DEVS, args);
        int result = BitConverter.ToInt32(status, 0);

        if (logName is not null)
            Logger.WriteLine(logName + " = " + Status + " : " + (result == 1 ? "OK" : result));

        return result;
    }


    public int DeviceSet(uint DeviceID, byte[] Params, string? logName)
    {
        byte[] args = new byte[4 + Params.Length];
        BitConverter.GetBytes((uint)DeviceID).CopyTo(args, 0);
        Params.CopyTo(args, 4);

        byte[] status = CallMethod(DEVS, args);
        int result = BitConverter.ToInt32(status, 0);

        if (logName is not null)
            Logger.WriteLine(logName + " = " + BitConverter.ToString(Params) + " : " + (result == 1 ? "OK" : result));

        return BitConverter.ToInt32(status, 0);
    }


    public int DeviceGet(uint DeviceID)
    {
        byte[] args = new byte[8];
        BitConverter.GetBytes((uint)DeviceID).CopyTo(args, 0);
        byte[] status = CallMethod(DSTS, args);

        return BitConverter.ToInt32(status, 0) - 65536;

    }

    public byte[] DeviceGetBuffer(uint DeviceID, uint Status = 0)
    {
        byte[] args = new byte[8];
        BitConverter.GetBytes((uint)DeviceID).CopyTo(args, 0);
        BitConverter.GetBytes((uint)Status).CopyTo(args, 4);

        return CallMethod(DSTS, args);
    }


    public decimal? GetBatteryDischarge()
    {
        var buffer = DeviceGetBuffer(BatteryDischarge);

        if (buffer[2] > 0)
        {
            buffer[2] = 0;
            return (decimal)BitConverter.ToInt16(buffer, 0) / 100;
        }
        else
        {
            return null;
        }


    }
    public int SetGPUEco(int eco)
    {
        int ecoFlag = DeviceGet(GPUEco);
        if (ecoFlag < 0) return -1;

        if (ecoFlag == 1 && eco == 0)
            return DeviceSet(GPUEco, eco, "GPUEco");

        if (ecoFlag == 0 && eco == 1)
            return DeviceSet(GPUEco, eco, "GPUEco");

        return -1;
    }

    public int GetFan(AsusFan device)
    {
        int fan = -1;

        switch (device)
        {
            case AsusFan.GPU:
                fan = Program.acpi.DeviceGet(GPU_Fan);
                break;
            case AsusFan.Mid:
                fan = Program.acpi.DeviceGet(Mid_Fan);
                break;
            default:
                fan = Program.acpi.DeviceGet(CPU_Fan);
                break;
        }

        if (fan < 0)
        {
            fan += 65536;
            if (fan <= 0 || fan > 100) fan = -1;
        }

        return fan;
    }


    public int SetFanRange(AsusFan device, byte[] curve)
    {

        if (curve.Length != 16) return -1;
        if (curve.All(singleByte => singleByte == 0)) return -1;

        byte min = (byte)(curve[8] * 255 / 100);
        byte max = (byte)(curve[15] * 255 / 100);
        byte[] range = { min, max };

        int result;
        switch (device)
        {
            case AsusFan.GPU:
                result = DeviceSet(DevsGPUFan, range, "FanRangeGPU");
                break;
            default:
                result = DeviceSet(DevsCPUFan, range, "FanRangeCPU");
                break;
        }

        return result;
    }


    public int SetFanCurve(AsusFan device, byte[] curve)
    {

        if (curve.Length != 16) return -1;
        if (curve.All(singleByte => singleByte == 0)) return -1;

        int result;

        int defaultScale = (AppConfig.IsFanScale() && (device == AsusFan.CPU || device == AsusFan.GPU)) ? 130 : 100;
        int fanScale = AppConfig.Get("fan_scale", defaultScale);

        if (fanScale != 100 && device == AsusFan.CPU) Logger.WriteLine("Custom fan scale: " + fanScale);

        // it seems to be a bug, when some old model's bios can go nuts if fan is set to 100% 

        for (int i = 8; i < curve.Length; i++) curve[i] = (byte)(Math.Max((byte)0, Math.Min((byte)100, curve[i])) * fanScale / 100);

        switch (device)
        {
            case AsusFan.GPU:
                result = DeviceSet(DevsGPUFanCurve, curve, "FanGPU");
                break;
            case AsusFan.Mid:
                result = DeviceSet(DevsMidFanCurve, curve, "FanMid");
                break;
            default:
                result = DeviceSet(DevsCPUFanCurve, curve, "FanCPU");
                break;
        }

        return result;
    }

    public byte[] GetFanCurve(AsusFan device, int mode = 0)
    {
        uint fan_mode;

        // because it's asus, and modes are swapped here
        switch (mode)
        {
            case 1: fan_mode = 2; break;
            case 2: fan_mode = 1; break;
            default: fan_mode = 0; break;
        }

        switch (device)
        {
            case AsusFan.GPU:
                return DeviceGetBuffer(DevsGPUFanCurve, fan_mode);
            case AsusFan.Mid:
                return DeviceGetBuffer(DevsMidFanCurve, fan_mode);
            default:
                return DeviceGetBuffer(DevsCPUFanCurve, fan_mode);
        }

    }

    public static bool IsInvalidCurve(byte[] curve)
    {
        return curve.Length != 16 || IsEmptyCurve(curve);
    }

    public static bool IsEmptyCurve(byte[] curve)
    {
        return curve.All(singleByte => singleByte == 0);
    }

    public static byte[] FixFanCurve(byte[] curve)
    {
        if (curve.Length != 16) throw new Exception("Incorrect curve");

        var points = new Dictionary<byte, byte>();
        byte old = 0;

        for (int i = 0; i < 8; i++)
        {
            if (curve[i] <= old) curve[i] = (byte)Math.Min(100, old + 6); // preventing 2 points in same spot from default asus profiles
            points[curve[i]] = curve[i + 8];
            old = curve[i];
        }

        var pointsFixed = new Dictionary<byte, byte>();
        bool fix = false;

        int count = 0;
        foreach (var pair in points.OrderBy(x => x.Key))
        {
            if (count == 0 && pair.Key >= 40)
            {
                fix = true;
                pointsFixed.Add(30, 0);
            }

            if (count != 3 || !fix)
                pointsFixed.Add(pair.Key, pair.Value);
            count++;
        }

        count = 0;
        foreach (var pair in pointsFixed.OrderBy(x => x.Key))
        {
            curve[count] = pair.Key;
            curve[count + 8] = pair.Value;
            count++;
        }

        return curve;

    }

    public bool IsXGConnected()
    {
        return DeviceGet(GPUXGConnected) == 1;
    }

    public bool IsAllAmdPPT()
    {
        //return false; 
        return DeviceGet(PPT_CPUB0) >= 0 && DeviceGet(PPT_GPUC0) < 0;
    }

    public bool IsNVidiaGPU()
    {
        return (!IsAllAmdPPT() && Program.acpi.DeviceGet(GPUEco) >= 0 && !AppConfig.IsAlly());
    }

    public void SetAPUMem(int memory = 4)
    {
        if (memory < 0 || memory > 8) return;

        int mem = 0;

        switch (memory)
        {
            case 0:
                mem = 0;
                break;
            case 1:
                mem = 258;
                break;
            case 2:
                mem = 259;
                break;
            case 3:
                mem = 260;
                break;
            case 4:
                mem = 261;
                break;
            case 5:
                mem = 263;
                break;
            case 6:
                mem = 264;
                break;
            case 7:
                mem = 265;
                break;
            case 8:
                mem = 262;
                break;
        }

        Program.acpi.DeviceSet(APU_MEM, mem, "APU Mem");
    }

    public int GetAPUMem()
    {
        int memory = Program.acpi.DeviceGet(APU_MEM);
        if (memory < 0) return -1;

        switch (memory)
        {
            case 256:
                return 0;
            case 258:
                return 1;
            case 259:
                return 2;
            case 260:
                return 3;
            case 261:
                return 4;
            case 262:
                return 8;
            case 263:
                return 5;
            case 264:
                return 6;
            case 265:
                return 7;
            default:
                return 4;
        }
    }

    public void ScanRange()
    {
        int value;
        string appPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\GHelper";
        string logFile = appPath + "\\scan.txt";
        for (uint i = 0x00000000; i <= 0x00160000; i++)
        {
            value = DeviceGet(i);
            if (value >= 0)
                using (StreamWriter w = File.AppendText(logFile))
                {
                    w.WriteLine(i.ToString("X8") + ": " + value.ToString("X4") + " (" + value + ")");
                    w.Close();
                }
        }

    }

    public void TUFKeyboardBrightness(int brightness)
    {
        int param = 0x80 | (brightness & 0x7F);
        DeviceSet(TUF_KB_BRIGHTNESS, param, "TUF Brightness");
    }

    public void TUFKeyboardRGB(AuraMode mode, Color color, int speed, string? log = "TUF RGB")
    {

        byte[] setting = new byte[6];

        setting[0] = (byte)0xb4;
        setting[1] = (byte)mode;
        setting[2] = color.R;
        setting[3] = color.G;
        setting[4] = color.B;
        setting[5] = (byte)speed;

        int result = DeviceSet(TUF_KB, setting, log);
        if (result != 1) DeviceSet(TUF_KB2, setting, log);

    }

    const int ASUS_WMI_KEYBOARD_POWER_BOOT = 0x03 << 16;
    const int ASUS_WMI_KEYBOARD_POWER_AWAKE = 0x0C << 16;
    const int ASUS_WMI_KEYBOARD_POWER_SLEEP = 0x30 << 16;
    const int ASUS_WMI_KEYBOARD_POWER_SHUTDOWN = 0xC0 << 16;
    public void TUFKeyboardPower(bool awake = true, bool boot = false, bool sleep = false, bool shutdown = false)
    {
        int state = 0xbd;

        if (boot) state = state | ASUS_WMI_KEYBOARD_POWER_BOOT;
        if (awake) state = state | ASUS_WMI_KEYBOARD_POWER_AWAKE;
        if (sleep) state = state | ASUS_WMI_KEYBOARD_POWER_SLEEP;
        if (shutdown) state = state | ASUS_WMI_KEYBOARD_POWER_SHUTDOWN;

        state = state | 0x01 << 8;

        DeviceSet(TUF_KB_STATE, state, "TUF_KB");
    }

    public void SubscribeToEvents(Action<object, EventArrivedEventArgs> EventHandler)
    {
        try
        {
            ManagementEventWatcher watcher = new ManagementEventWatcher();
            watcher.EventArrived += new EventArrivedEventHandler(EventHandler);
            watcher.Scope = new ManagementScope("root\\wmi");
            watcher.Query = new WqlEventQuery("SELECT * FROM AsusAtkWmiEvent");
            watcher.Start();
        }
        catch
        {
            Logger.WriteLine("Can't connect to ASUS WMI events");
        }
    }


}
