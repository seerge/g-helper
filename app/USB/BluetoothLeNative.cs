using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace GHelper.USB
{
    internal static class BluetoothLeNative
    {
        public static readonly Guid VendorGattService = new("0000fff0-0000-1000-8000-00805f9b34fb");

        const uint DIGCF_PRESENT = 0x02;
        const uint DIGCF_DEVICEINTERFACE = 0x10;
        const uint ERROR_NO_MORE_ITEMS = 259;
        const uint ERROR_MORE_DATA = 234;
        const int BLUETOOTH_GATT_FLAG_NONE = 0x00000000;
        const int BLUETOOTH_GATT_FLAG_FORCE_READ_FROM_DEVICE = 0x00000004;

        [StructLayout(LayoutKind.Sequential)]
        private struct SP_DEVICE_INTERFACE_DATA
        {
            public int cbSize;
            public Guid InterfaceClassGuid;
            public uint Flags;
            public IntPtr Reserved;
        }

        [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr SetupDiGetClassDevsW(ref Guid classGuid, IntPtr enumerator, IntPtr hwndParent, uint flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiEnumDeviceInterfaces(IntPtr deviceInfoSet, IntPtr deviceInfoData,
            ref Guid interfaceClassGuid, uint memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

        [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool SetupDiGetDeviceInterfaceDetailW(IntPtr deviceInfoSet,
            ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr deviceInterfaceDetailData,
            uint deviceInterfaceDetailDataSize, out uint requiredSize, IntPtr deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool SetupDiGetDeviceInterfaceDetailW(IntPtr deviceInfoSet,
            ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr deviceInterfaceDetailData,
            uint deviceInterfaceDetailDataSize, IntPtr requiredSize, IntPtr deviceInfoData);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern SafeFileHandle CreateFileW(string fileName, uint desiredAccess, uint shareMode,
            IntPtr securityAttributes, uint creationDisposition, uint flagsAndAttributes, IntPtr templateFile);

        const uint GENERIC_READ = 0x80000000;
        const uint GENERIC_WRITE = 0x40000000;
        const uint FILE_SHARE_READ = 0x00000001;
        const uint FILE_SHARE_WRITE = 0x00000002;
        const uint OPEN_EXISTING = 3;

        [StructLayout(LayoutKind.Explicit, Size = 20)]
        public struct BTH_LE_UUID
        {
            [FieldOffset(0)] public byte IsShortUuid;
            [FieldOffset(4)] public ushort ShortUuid;
            [FieldOffset(4)] public Guid LongUuid;
        }

        [StructLayout(LayoutKind.Sequential, Size = 24)]
        public struct BTH_LE_GATT_SERVICE
        {
            public BTH_LE_UUID ServiceUuid;
            public ushort AttributeHandle;
        }

        [StructLayout(LayoutKind.Sequential, Size = 36)]
        public struct BTH_LE_GATT_CHARACTERISTIC
        {
            public ushort ServiceHandle;
            public BTH_LE_UUID CharacteristicUuid;
            public ushort AttributeHandle;
            public ushort CharacteristicValueHandle;
            public byte IsBroadcastable;
            public byte IsReadable;
            public byte IsWritable;
            public byte IsWritableWithoutResponse;
            public byte IsSignedWritable;
            public byte IsNotifiable;
            public byte IsIndicatable;
            public byte HasExtendedProperties;
        }

        [DllImport("BluetoothApis.dll", SetLastError = true)]
        private static extern int BluetoothGATTGetServices(SafeFileHandle hDevice, ushort serviceBufferCount,
            IntPtr serviceBuffer, out ushort actualBufferCount, int flags);

        [DllImport("BluetoothApis.dll", SetLastError = true)]
        private static extern int BluetoothGATTGetCharacteristics(SafeFileHandle hDevice, ref BTH_LE_GATT_SERVICE service,
            ushort charBufferCount, IntPtr charBuffer, out ushort actualBufferCount, int flags);

        [DllImport("BluetoothApis.dll", SetLastError = true)]
        private static extern int BluetoothGATTGetCharacteristicValue(SafeFileHandle hDevice,
            ref BTH_LE_GATT_CHARACTERISTIC characteristic, uint charValueDataSize, IntPtr charValueData,
            out ushort charValueSizeRequired, int flags);

        [DllImport("BluetoothApis.dll", SetLastError = true)]
        private static extern int BluetoothGATTSetCharacteristicValue(SafeFileHandle hDevice,
            ref BTH_LE_GATT_CHARACTERISTIC characteristic, IntPtr charValue, ulong reliableWriteContext, int flags);

        public sealed class Device : IDisposable
        {
            public SafeFileHandle Handle { get; }
            public string DevicePath { get; }
            public BTH_LE_GATT_CHARACTERISTIC[] Characteristics { get; }

            public Device(SafeFileHandle handle, string devicePath, BTH_LE_GATT_CHARACTERISTIC[] characteristics)
            {
                Handle = handle;
                DevicePath = devicePath;
                Characteristics = characteristics;
            }

            public BTH_LE_GATT_CHARACTERISTIC? FirstWritable() =>
                Characteristics.Cast<BTH_LE_GATT_CHARACTERISTIC?>()
                    .FirstOrDefault(c => c!.Value.IsWritable != 0 || c.Value.IsWritableWithoutResponse != 0);

            public BTH_LE_GATT_CHARACTERISTIC? FirstReadable() =>
                Characteristics.Cast<BTH_LE_GATT_CHARACTERISTIC?>()
                    .FirstOrDefault(c => c!.Value.IsReadable != 0);

            public void Dispose() => Handle.Dispose();
        }

        public static IEnumerable<string> EnumerateVendorBleInterfaces()
        {
            Guid guid = VendorGattService;
            IntPtr hDevInfo = SetupDiGetClassDevsW(ref guid, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);
            if (hDevInfo == new IntPtr(-1)) yield break;

            try
            {
                uint index = 0;
                while (true)
                {
                    var ifd = new SP_DEVICE_INTERFACE_DATA();
                    ifd.cbSize = Marshal.SizeOf<SP_DEVICE_INTERFACE_DATA>();
                    if (!SetupDiEnumDeviceInterfaces(hDevInfo, IntPtr.Zero, ref guid, index, ref ifd)) break;
                    index++;

                    SetupDiGetDeviceInterfaceDetailW(hDevInfo, ref ifd, IntPtr.Zero, 0, out uint required, IntPtr.Zero);
                    if (required == 0) continue;

                    IntPtr buf = Marshal.AllocHGlobal((int)required);
                    try
                    {
                        Marshal.WriteInt32(buf, IntPtr.Size == 8 ? 8 : 6);
                        if (!SetupDiGetDeviceInterfaceDetailW(hDevInfo, ref ifd, buf, required, IntPtr.Zero, IntPtr.Zero))
                            continue;

                        string path = Marshal.PtrToStringUni(buf + 4)!;
                        yield return path;
                    }
                    finally { Marshal.FreeHGlobal(buf); }
                }
            }
            finally { SetupDiDestroyDeviceInfoList(hDevInfo); }
        }

        static readonly Regex VidPidRx = new(
            @"vid[_&](?<vid>[0-9a-fA-F]{4,6})[_&]?pid[_&](?<pid>[0-9a-fA-F]{4})",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static bool TryParseVidPid(string devicePath, out ushort vid, out ushort pid)
        {
            vid = pid = 0;
            var m = VidPidRx.Match(devicePath);
            if (!m.Success) return false;
            string vidStr = m.Groups["vid"].Value;
            if (vidStr.Length > 4) vidStr = vidStr.Substring(vidStr.Length - 4);
            vid = ushort.Parse(vidStr, System.Globalization.NumberStyles.HexNumber);
            pid = ushort.Parse(m.Groups["pid"].Value, System.Globalization.NumberStyles.HexNumber);
            return true;
        }

        public static Device? Open(string devicePath)
        {
            var handle = CreateFileW(devicePath, GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE,
                IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
            if (handle.IsInvalid)
            {
                Logger.WriteLine($"BLE Open: CreateFile failed for {devicePath} err={Marshal.GetLastWin32Error()}");
                return null;
            }

            try
            {
                int hr = BluetoothGATTGetServices(handle, 0, IntPtr.Zero, out ushort svcCount, BLUETOOTH_GATT_FLAG_NONE);
                if (svcCount == 0) { Logger.WriteLine($"BLE Open: 0 services hr={hr:X8}"); handle.Dispose(); return null; }

                int svcStride = Marshal.SizeOf<BTH_LE_GATT_SERVICE>();
                IntPtr svcBuf = Marshal.AllocHGlobal(svcStride * svcCount);
                try
                {
                    hr = BluetoothGATTGetServices(handle, svcCount, svcBuf, out svcCount, BLUETOOTH_GATT_FLAG_NONE);
                    if (hr != 0) { handle.Dispose(); return null; }

                    var allChars = new List<BTH_LE_GATT_CHARACTERISTIC>();
                    int chStride = Marshal.SizeOf<BTH_LE_GATT_CHARACTERISTIC>();

                    for (int i = 0; i < svcCount; i++)
                    {
                        var svc = Marshal.PtrToStructure<BTH_LE_GATT_SERVICE>(svcBuf + i * svcStride);
                        if (svc.ServiceUuid.IsShortUuid == 0 || svc.ServiceUuid.ShortUuid != 0xFFF0) continue;

                        BluetoothGATTGetCharacteristics(handle, ref svc, 0, IntPtr.Zero, out ushort cCount, BLUETOOTH_GATT_FLAG_NONE);
                        if (cCount == 0) continue;

                        IntPtr cBuf = Marshal.AllocHGlobal(chStride * cCount);
                        try
                        {
                            if (BluetoothGATTGetCharacteristics(handle, ref svc, cCount, cBuf, out cCount, BLUETOOTH_GATT_FLAG_NONE) != 0)
                                continue;

                            for (int j = 0; j < cCount; j++)
                                allChars.Add(Marshal.PtrToStructure<BTH_LE_GATT_CHARACTERISTIC>(cBuf + j * chStride));
                        }
                        finally { Marshal.FreeHGlobal(cBuf); }
                    }

                    if (allChars.Count == 0) { Logger.WriteLine("BLE Open: 0 characteristics on FFF0"); handle.Dispose(); return null; }
                    return new Device(handle, devicePath, allChars.ToArray());
                }
                finally { Marshal.FreeHGlobal(svcBuf); }
            }
            catch
            {
                handle.Dispose();
                return null;
            }
        }

        public static bool Write(Device device, byte[] payload)
        {
            var ch = device.FirstWritable();
            if (ch is null) { Logger.WriteLine("BLE Write: no writable characteristic"); return false; }
            var c = ch.Value;

            int total = 4 + payload.Length;
            IntPtr buf = Marshal.AllocHGlobal(total);
            try
            {
                Marshal.WriteInt32(buf, payload.Length);
                Marshal.Copy(payload, 0, buf + 4, payload.Length);
                int hr = BluetoothGATTSetCharacteristicValue(device.Handle, ref c, buf, 0, BLUETOOTH_GATT_FLAG_NONE);
                if (hr != 0) Logger.WriteLine($"BLE Write: hr={hr:X8} char_uuid=0x{c.CharacteristicUuid.ShortUuid:X4} len={payload.Length}");
                return hr == 0;
            }
            finally { Marshal.FreeHGlobal(buf); }
        }

        public static byte[]? Read(Device device, int maxLen = 64)
        {
            var ch = device.FirstReadable();
            if (ch is null) return null;
            var c = ch.Value;

            BluetoothGATTGetCharacteristicValue(device.Handle, ref c, 0, IntPtr.Zero, out ushort needed, BLUETOOTH_GATT_FLAG_FORCE_READ_FROM_DEVICE);
            if (needed == 0) return null;

            IntPtr buf = Marshal.AllocHGlobal(needed);
            try
            {
                int hr = BluetoothGATTGetCharacteristicValue(device.Handle, ref c, needed, buf, out _, BLUETOOTH_GATT_FLAG_FORCE_READ_FROM_DEVICE);
                if (hr != 0) return null;
                int len = Marshal.ReadInt32(buf);
                if (len <= 0 || len > maxLen) len = Math.Min(maxLen, needed - 4);
                byte[] result = new byte[len];
                Marshal.Copy(buf + 4, result, 0, len);
                return result;
            }
            finally { Marshal.FreeHGlobal(buf); }
        }
    }
}
