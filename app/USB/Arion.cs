using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace GHelper.USB;

internal static class Arion
{
    private const uint IOCTL_SCSI_PASS_THROUGH = 0x0004D004;
    private const int SenseLength = 32;
    private static readonly object syncLock = new();
    private static ArionRegisterWrite[]? pending;
    private static bool workerRunning;

    // A single worker serializes transactions and coalesces rapid settings changes.
    public static void Sync(byte mode, byte speed, byte red, byte green, byte blue)
    {
        if (!AppConfig.Is("arion_aura_sync")) return;

        lock (syncLock)
        {
            pending = ArionProtocol.BuildEffect(mode, speed, red, green, blue);
            if (workerRunning) return;
            workerRunning = true;
        }
        _ = Task.Run(ProcessPending);
    }

    private static void ProcessPending()
    {
        while (true)
        {
            ArionRegisterWrite[]? commands;
            lock (syncLock)
            {
                commands = pending;
                pending = null;
                if (commands is null)
                {
                    workerRunning = false;
                    return;
                }
            }

            try
            {
                if (!AppConfig.Is("arion_aura_sync")) continue;
                foreach (string path in FindDevices())
                {
                    try
                    {
                        using SafeFileHandle handle = CreateFile(path, 0xC0000000, 3, IntPtr.Zero, 3, 0, IntPtr.Zero);
                        if (handle.IsInvalid) throw new Win32Exception(Marshal.GetLastWin32Error());

                        foreach (var command in commands) WriteRegister(handle, command);
                        Logger.WriteLine("Arion: applied Aura effect");
                    }
                    catch (Exception ex)
                    {
                        // Includes access denied, removal during a transaction, and SCSI errors.
                        // Do not retry a failed transaction or write its remaining registers.
                        Logger.WriteLine($"Arion: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Arion discovery: {ex.Message}");
            }
        }
    }

    private static IEnumerable<string> FindDevices()
    {
        // Use the same SCSI device interface naming as OpenRGB's Windows detector.
        char[] names = new char[65536];
        uint count;
        while ((count = QueryDosDevice(null, names, (uint)names.Length)) == 0)
        {
            int error = Marshal.GetLastWin32Error();
            if (error != 122 || names.Length >= 1048576) throw new Win32Exception(error);
            names = new char[names.Length * 2];
        }

        return new string(names, 0, (int)count)
            .Split('\0', StringSplitOptions.RemoveEmptyEntries)
            .Select(ArionProtocol.GetDevicePath)
            .OfType<string>()
            .Distinct(StringComparer.OrdinalIgnoreCase);
    }

    private static void WriteRegister(SafeFileHandle handle, ArionRegisterWrite command)
    {
        byte[] cdb = ArionProtocol.BuildCdb(command.Register, command.Data.Length);
        int headerLength = Marshal.SizeOf<ScsiPassThrough>();
        int dataOffset = headerLength + SenseLength;
        int bufferLength = dataOffset + command.Data.Length;
        IntPtr buffer = Marshal.AllocHGlobal(bufferLength);
        try
        {
            Marshal.Copy(new byte[bufferLength], 0, buffer, bufferLength);
            var request = new ScsiPassThrough
            {
                Length = (ushort)headerLength,
                CdbLength = (byte)cdb.Length,
                SenseInfoLength = SenseLength,
                DataIn = 0, // SCSI_IOCTL_DATA_OUT
                DataTransferLength = (uint)command.Data.Length,
                TimeOutValue = 2,
                DataBufferOffset = (UIntPtr)dataOffset,
                SenseInfoOffset = (uint)headerLength,
                Cdb = cdb,
            };
            Marshal.StructureToPtr(request, buffer, false);
            Marshal.Copy(command.Data, 0, IntPtr.Add(buffer, dataOffset), command.Data.Length);

            if (!DeviceIoControl(handle, IOCTL_SCSI_PASS_THROUGH, buffer, (uint)bufferLength,
                buffer, (uint)bufferLength, out uint returned, IntPtr.Zero))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            var response = Marshal.PtrToStructure<ScsiPassThrough>(buffer);
            if (returned < headerLength || response.ScsiStatus != 0 || response.DataTransferLength != command.Data.Length)
            {
                byte[] sense = new byte[SenseLength];
                Marshal.Copy(IntPtr.Add(buffer, headerLength), sense, 0, sense.Length);
                throw new IOException($"SCSI register {command.Register:X4}: status={response.ScsiStatus:X2}, sense={BitConverter.ToString(sense)}");
            }
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    // Native SCSI_PASS_THROUGH, with pointer-sized DataBufferOffset (56 bytes on x64).
    // Buffered pass-through avoids the separate aligned data buffer required by DIRECT.
    [StructLayout(LayoutKind.Sequential)]
    private struct ScsiPassThrough
    {
        public ushort Length;
        public byte ScsiStatus;
        public byte PathId;
        public byte TargetId;
        public byte Lun;
        public byte CdbLength;
        public byte SenseInfoLength;
        public byte DataIn;
        public uint DataTransferLength;
        public uint TimeOutValue;
        public UIntPtr DataBufferOffset;
        public uint SenseInfoOffset;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Cdb;
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern uint QueryDosDevice(string? name, [Out] char[] target, uint maxLength);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern SafeFileHandle CreateFile(string name, uint access, uint share,
        IntPtr security, uint creation, uint flags, IntPtr template);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DeviceIoControl(SafeFileHandle device, uint code,
        IntPtr input, uint inputLength, IntPtr output, uint outputLength,
        out uint returned, IntPtr overlapped);
}
