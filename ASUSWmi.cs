using System.Management;
using System.Runtime.InteropServices;

public class ASUSWmi
{

    const string FILE_NAME = @"\\.\\ATKACPI";
    const uint CONTROL_CODE = 0x0022240C;

    const uint DSTS = 0x53545344;
    const uint DEVS = 0x53564544;

    public const uint CPU_Fan = 0x00110013;
    public const uint GPU_Fan = 0x00110014;

    public const uint PerformanceMode = 0x00120075; // Thermal Control

    public const uint GPUEco = 0x00090020;
    public const uint GPUMux = 0x00090016;

    public const uint BatteryLimit = 0x00120057;
    public const uint ScreenOverdrive = 0x00050019;

    public const uint DevsCPUFanCurve = 0x00110024;
    public const uint DevsGPUFanCurve = 0x00110025;

    public const int PerformanceBalanced = 0;
    public const int PerformanceTurbo = 1;
    public const int PerformanceSilent = 2;

    public const int GPUModeEco = 0;
    public const int GPUModeStandard = 1;
    public const int GPUModeUltimate = 2;


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

    public ASUSWmi()
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
        if (handle == new IntPtr(-1))
        {
            throw new Exception("Can't connect to ACPI");
        }
    }

    public void Control(uint dwIoControlCode, byte[] lpInBuffer, byte[] lpOutBuffer)
    {
        uint lpBytesReturned = 0;
        bool result = DeviceIoControl(
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
        byte[] outBuffer = new byte[20];

        BitConverter.GetBytes((uint)MethodID).CopyTo(acpiBuf, 0);
        BitConverter.GetBytes((uint)args.Length).CopyTo(acpiBuf, 4);
        Array.Copy(args, 0, acpiBuf, 8, args.Length);

        Console.WriteLine(BitConverter.ToString(acpiBuf, 0, acpiBuf.Length));

        Control(CONTROL_CODE, acpiBuf, outBuffer);

        return outBuffer;

    }

    public void DeviceSet(uint DeviceID, int Status)
    {
        byte[] args = new byte[8];
        BitConverter.GetBytes((uint)DeviceID).CopyTo(args, 0);
        BitConverter.GetBytes((uint)Status).CopyTo(args, 4);
        CallMethod(DEVS, args);
    }


    public void DeviceSet(uint DeviceID, byte[] Params)
    {
        byte[] args = new byte[4 + Params.Length];
        BitConverter.GetBytes((uint)DeviceID).CopyTo(args, 0);
        Params.CopyTo(args, 4);
        CallMethod(DEVS, args);
    }


    public int DeviceGet(uint DeviceID)
    {
        byte[] args = new byte[8];
        BitConverter.GetBytes((uint)DeviceID).CopyTo(args, 0);
        byte[] status = CallMethod(DSTS, args);
        return BitConverter.ToInt32(status, 0)-65536;
    }

    public byte[] DeviceGetBuffer(uint DeviceID, int Status = 0)
    {
        byte[] args = new byte[8];
        BitConverter.GetBytes((uint)DeviceID).CopyTo(args, 0);
        BitConverter.GetBytes((uint)Status).CopyTo(args, 4);
        return CallMethod(DSTS, args);
    }


    public void SetFanCurve(byte[] curve)
    {
        DeviceSet(DevsCPUFanCurve, curve);
    }

    public byte[] GetFanCurve(int mode = 0)
    {
        if (mode == 1) mode = 2;
        if (mode == 2) mode = 1; // because it's asus, and modes are swapped here

        return DeviceGetBuffer(DevsCPUFanCurve, mode);
    }


    public void SubscribeToEvents(Action<object, EventArrivedEventArgs> EventHandler)
    {
        ManagementEventWatcher watcher = new ManagementEventWatcher();
        watcher.EventArrived += new EventArrivedEventHandler(EventHandler);
        watcher.Scope = new ManagementScope("root\\wmi");
        watcher.Query = new WqlEventQuery("SELECT * FROM AsusAtkWmiEvent");
        watcher.Start();
    }

}
