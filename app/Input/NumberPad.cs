using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace GHelper.Input
{
    public static class NumberPad
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, byte[] lpInBuffer, uint nInBufferSize, byte[] lpOutBuffer, uint nOutBufferSize, ref uint lpBytesReturned, IntPtr lpOverlapped);

        private static bool Ioctl(uint code, int function, ref int value)
        {
            using SafeFileHandle device = CreateFile("\\\\.\\AsusTPPC", 0xC0000000, 3, IntPtr.Zero, 3, 0, IntPtr.Zero);
            if (device.IsInvalid) return false;

            byte[] buffer = new byte[0x800];
            BitConverter.GetBytes(0x104).CopyTo(buffer, 0);
            BitConverter.GetBytes(buffer.Length).CopyTo(buffer, 4);
            BitConverter.GetBytes(0xFF0000FF).CopyTo(buffer, 8);
            BitConverter.GetBytes(function).CopyTo(buffer, 12);
            BitConverter.GetBytes(0xC).CopyTo(buffer, 16);
            BitConverter.GetBytes(value).CopyTo(buffer, 20);

            uint returned = 0;
            bool status = DeviceIoControl(device, code, buffer, (uint)buffer.Length, buffer, (uint)buffer.Length, ref returned, IntPtr.Zero);
            value = BitConverter.ToInt32(buffer, 20);
            return status;
        }

        public static void Init()
        {
            if (AppConfig.Get("numberpad") == 0) Set(false);
        }

        public static int Get()
        {
            int value = 0;
            bool status = Ioctl(0x221444, 6, ref value);
            Logger.WriteLine($"NumberPad Get = {value} : " + (status ? "OK" : "Error"));
            if (!status) return -1;
            return value == 1 ? 1 : value == 2 ? 0 : -1;
        }

        public static void Set(bool enabled)
        {
            int value = enabled ? 1 : 2;
            bool status = Ioctl(0x2215EC, 7, ref value);
            if (status) AppConfig.Set("numberpad", enabled ? 1 : 0);
            Logger.WriteLine($"NumberPad Set = {(enabled ? 1 : 2)} : " + (status ? "OK" : "Error"));
        }
    }
}
