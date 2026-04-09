using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace PawnIO
{
    public sealed class PawnIOWrapper : IDisposable
    {
        private const int FN_LEN = 32;
        private const uint DEV_TYPE = 41394u << 16;     // 0xA1B20000

        private enum Ctl : uint
        {
            Load    = DEV_TYPE | (0x821 << 2),          // 0xA1B22084
            Execute = DEV_TYPE | (0x841 << 2),          // 0xA1B22104
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr CreateFile(string n, uint acc, uint share,
            IntPtr sec, uint disp, uint fl, IntPtr tmpl);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CloseHandle(IntPtr h);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool DeviceIoControl(IntPtr dev, Ctl code,
            byte[] inB, uint inSz, byte[] outB, uint outSz, out uint ret, IntPtr ovl);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi)]
        static extern bool DeviceIoControl(SafeFileHandle dev, Ctl code,
            [In] byte[] inB, uint inSz, [Out] byte[] outB, uint outSz,
            out uint ret, IntPtr ovl);

        private IntPtr _raw = IntPtr.Zero;
        private SafeFileHandle? _safe;
        private bool _loaded, _disposed;

        public bool IsConnected    => _raw != IntPtr.Zero && _raw.ToInt64() != -1;
        public bool IsModuleLoaded => _loaded && _safe != null && !_safe.IsInvalid;

        public enum ConnectResult { OK, NotInstalled, AccessDenied, OtherError }

        public ConnectResult Connect()
        {
            if (IsConnected) return ConnectResult.OK;

            const string path = @"\\?\GLOBALROOT\Device\PawnIO";
            _raw = CreateFile(path, 0xC0000000u, 0x3, IntPtr.Zero, 3, 0, IntPtr.Zero);
            if (_raw == IntPtr.Zero || _raw.ToInt64() == -1)
            {
                int err = Marshal.GetLastWin32Error();
                _raw = IntPtr.Zero;
                return err switch
                {
                    2 or 3 => ConnectResult.NotInstalled,  // ERROR_FILE_NOT_FOUND / ERROR_PATH_NOT_FOUND
                    5      => ConnectResult.AccessDenied,  // ERROR_ACCESS_DENIED
                    _      => ConnectResult.OtherError,
                };
            }
            return ConnectResult.OK;
        }

        public bool LoadModule(byte[] data)
        {
            if (!IsConnected || data == null || data.Length == 0) return false;

            bool ok = DeviceIoControl(_raw, Ctl.Load, data, (uint)data.Length, null!, 0, out _, IntPtr.Zero);
            if (!ok) return false;

            _safe = new SafeFileHandle(_raw, ownsHandle: true);
            _raw = IntPtr.Zero; // now owned by _safe
            _loaded = true;
            return true;
        }

        public bool Execute(string functionName, ulong[]? input, ulong[]? output)
        {
            if (!IsModuleLoaded) return false;

            byte[] nameBytes = Encoding.ASCII.GetBytes(functionName);
            int inputCount = input?.Length ?? 0;
            byte[] buffer = new byte[FN_LEN + inputCount * 8];
            Buffer.BlockCopy(nameBytes, 0, buffer, 0, Math.Min(nameBytes.Length, FN_LEN - 1));

            if (input != null && inputCount > 0)
            {
                byte[] inputBytes = new byte[inputCount * 8];
                Buffer.BlockCopy(input, 0, inputBytes, 0, inputBytes.Length);
                Buffer.BlockCopy(inputBytes, 0, buffer, FN_LEN, inputBytes.Length);
            }

            int outputCount = output?.Length ?? 0;
            byte[] outputBuffer = outputCount > 0 ? new byte[outputCount * 8] : null!;
            bool ok = DeviceIoControl(_safe!, Ctl.Execute, buffer, (uint)buffer.Length,
                                      outputBuffer!, (uint)(outputBuffer?.Length ?? 0), out uint bytesReturned, IntPtr.Zero);

            if (ok && output != null && outputBuffer != null && bytesReturned > 0)
                Buffer.BlockCopy(outputBuffer, 0, output, 0, (int)Math.Min(bytesReturned, (uint)(outputCount * 8)));

            return ok;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _loaded = false;
            _safe?.Close();
            if (_raw != IntPtr.Zero && _raw.ToInt64() != -1) CloseHandle(_raw);
            _disposed = true;
        }
    }
}
