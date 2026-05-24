using System.Runtime.InteropServices;

namespace GHelperOverlay;

// Read-only subset of g-helper's AsusACPI — just the endpoints the overlay needs:
// CPU/GPU temp and CPU/GPU fan RPM. Opens the same \\.\ATKACPI driver node with
// shared access, so coexistence with a running g-helper process is fine.
public sealed class AsusACPI : IDisposable
{
    const string FILE_NAME = @"\\.\ATKACPI";
    const uint CONTROL_CODE = 0x0022240C;
    const uint DSTS = 0x53545344;

    public const int Temp_CPU = 0x00120094;
    public const int Temp_GPU = 0x00120097;
    public const uint CPU_Fan = 0x00110013;
    public const uint GPU_Fan = 0x00110014;

    private const uint GENERIC_READ  = 0x80000000;
    private const uint GENERIC_WRITE = 0x40000000;
    private const uint OPEN_EXISTING = 3;
    private const uint FILE_ATTRIBUTE_NORMAL = 0x80;
    private const uint FILE_SHARE_READ  = 1;
    private const uint FILE_SHARE_WRITE = 2;
    private static readonly IntPtr INVALID = new(-1);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode,
        IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool DeviceIoControl(IntPtr hDevice, uint dwIoControlCode,
        byte[] lpInBuffer, uint nInBufferSize, byte[] lpOutBuffer, uint nOutBufferSize,
        ref uint lpBytesReturned, IntPtr lpOverlapped);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);

    private IntPtr handle = INVALID;
    public bool IsConnected => handle != INVALID;

    public AsusACPI()
    {
        try
        {
            handle = CreateFile(FILE_NAME,
                GENERIC_READ | GENERIC_WRITE,
                FILE_SHARE_READ | FILE_SHARE_WRITE,
                IntPtr.Zero, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);
            if (handle == INVALID)
                Logger.WriteLine("ATKACPI open failed (err=" + Marshal.GetLastWin32Error() + ")");
        }
        catch (Exception ex) { Logger.WriteLine("ATKACPI open exception: " + ex.Message); }
    }

    // Returns the raw signed value at DeviceID (matches g-helper's DeviceGet semantics:
    // the 65536 bias is subtracted so negative results indicate "no value / unsupported").
    public int DeviceGet(uint deviceId)
    {
        if (!IsConnected) return -1;
        byte[] acpiBuf = new byte[16];
        byte[] outBuffer = new byte[16];
        BitConverter.GetBytes(DSTS).CopyTo(acpiBuf, 0);
        BitConverter.GetBytes((uint)8).CopyTo(acpiBuf, 4);
        BitConverter.GetBytes(deviceId).CopyTo(acpiBuf, 8);

        uint returned = 0;
        if (!DeviceIoControl(handle, CONTROL_CODE, acpiBuf, (uint)acpiBuf.Length,
                outBuffer, (uint)outBuffer.Length, ref returned, IntPtr.Zero))
            return -1;
        return BitConverter.ToInt32(outBuffer, 0) - 65536;
    }

    public int GetCpuFanRpm() => NormaliseFan(DeviceGet(CPU_Fan));
    public int GetGpuFanRpm() => NormaliseFan(DeviceGet(GPU_Fan));

    private static int NormaliseFan(int fan)
    {
        if (fan < 0)
        {
            fan += 65536;
            if (fan <= 0 || fan > 100) fan = -1;
        }
        return fan;
    }

    public void Dispose()
    {
        if (handle != INVALID) CloseHandle(handle);
        handle = INVALID;
    }
}
