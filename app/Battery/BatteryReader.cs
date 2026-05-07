using GHelper.Helpers;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;

namespace GHelper.Battery
{
    public static class BatteryReader
    {
        public static decimal? batteryRate = 0;
        public static decimal batteryHealth = -1;
        public static decimal batteryCapacity = -1;

        public static decimal? designCapacity;
        public static decimal? fullCapacity;
        public static decimal? chargeCapacity;

        public static string? batteryCharge;

        static bool _chargeWatt = AppConfig.Is("charge_watt");
        public static bool chargeWatt
        {
            get => _chargeWatt;
            set
            {
                AppConfig.Set("charge_watt", value ? 1 : 0);
                _chargeWatt = value;
            }
        }

        #region Native API

        [DllImport("powrprof.dll", SetLastError = true)]
        private static extern uint CallNtPowerInformation(
            int InformationLevel, IntPtr InputBuffer, uint InputBufferLength,
            IntPtr OutputBuffer, uint OutputBufferLength);

        private const int SystemBatteryState = 5;

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEM_BATTERY_STATE
        {
            [MarshalAs(UnmanagedType.U1)] public bool AcOnLine;
            [MarshalAs(UnmanagedType.U1)] public bool BatteryPresent;
            [MarshalAs(UnmanagedType.U1)] public bool Charging;
            [MarshalAs(UnmanagedType.U1)] public bool Discharging;
            public byte Spare1;
            public byte Spare2;
            public byte Spare3;
            public byte Spare4;
            public uint MaxCapacity;
            public uint RemainingCapacity;
            public int Rate;
            public uint EstimatedTime;
            public uint DefaultAlert1;
            public uint DefaultAlert2;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetSystemPowerStatus(ref SYSTEM_POWER_STATUS lpSystemPowerStatus);

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEM_POWER_STATUS
        {
            public byte ACLineStatus;
            public byte BatteryFlag;
            public byte BatteryLifePercent;
            public byte SystemStatusFlag;
            public int BatteryLifeTime;
            public int BatteryFullLifeTime;
        }

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetupDiGetClassDevs(
            ref Guid classGuid, IntPtr enumerator, IntPtr hwndParent, uint flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiEnumDeviceInterfaces(
            IntPtr deviceInfoSet, IntPtr deviceInfoData, ref Guid interfaceClassGuid,
            uint memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetupDiGetDeviceInterfaceDetail(
            IntPtr deviceInfoSet, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
            IntPtr deviceInterfaceDetailData, uint deviceInterfaceDetailDataSize,
            out uint requiredSize, IntPtr deviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CreateFile(
            string lpFileName, uint dwDesiredAccess, uint dwShareMode,
            IntPtr lpSecurityAttributes, uint dwCreationDisposition,
            uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool DeviceIoControl(
            IntPtr hDevice, uint dwIoControlCode,
            ref uint lpInBuffer, uint nInBufferSize,
            ref uint lpOutBuffer, uint nOutBufferSize,
            out uint lpBytesReturned, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", EntryPoint = "DeviceIoControl", SetLastError = true)]
        private static extern bool DeviceIoControlStatus(
            IntPtr hDevice, uint dwIoControlCode,
            ref BATTERY_WAIT_STATUS lpInBuffer, uint nInBufferSize,
            ref BATTERY_STATUS lpOutBuffer, uint nOutBufferSize,
            out uint lpBytesReturned, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", EntryPoint = "DeviceIoControl", SetLastError = true)]
        private static extern bool DeviceIoControlInformation(
            IntPtr hDevice, uint dwIoControlCode,
            ref BATTERY_QUERY_INFORMATION lpInBuffer, uint nInBufferSize,
            ref BATTERY_INFORMATION lpOutBuffer, uint nOutBufferSize,
            out uint lpBytesReturned, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [StructLayout(LayoutKind.Sequential)]
        private struct SP_DEVICE_INTERFACE_DATA
        {
            public uint cbSize;
            public Guid InterfaceClassGuid;
            public uint Flags;
            public IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BATTERY_WAIT_STATUS
        {
            public uint BatteryTag;
            public uint Timeout;
            public uint PowerState;
            public uint LowCapacity;
            public uint HighCapacity;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BATTERY_STATUS
        {
            public uint PowerState;
            public uint Capacity;
            public int Voltage;
            public int Rate;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BATTERY_QUERY_INFORMATION
        {
            public uint BatteryTag;
            public int InformationLevel;
            public int AtRate;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BATTERY_INFORMATION
        {
            public uint Capabilities;
            public byte Technology;
            public byte Reserved0;
            public byte Reserved1;
            public byte Reserved2;
            public uint Chemistry;
            public uint DesignedCapacity;
            public uint FullChargedCapacity;
            public uint DefaultAlert1;
            public uint DefaultAlert2;
            public uint CriticalBias;
            public uint CycleCount;
        }

        private static readonly Guid GUID_DEVINTERFACE_BATTERY = new("72631E54-78A4-11D0-BCF7-00AA00B7B32A");
        private const uint DIGCF_PRESENT = 0x02;
        private const uint DIGCF_DEVICEINTERFACE = 0x10;
        private const uint GENERIC_READ = 0x80000000;
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint FILE_SHARE_READ = 0x01;
        private const uint FILE_SHARE_WRITE = 0x02;
        private const uint OPEN_EXISTING = 3;
        private const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        private static readonly IntPtr INVALID_HANDLE_VALUE = new(-1);

        private const uint IOCTL_BATTERY_QUERY_TAG = 0x294040;
        private const uint IOCTL_BATTERY_QUERY_INFORMATION = 0x294044;
        private const uint IOCTL_BATTERY_QUERY_STATUS = 0x29404C;

        private static SYSTEM_BATTERY_STATE? GetNativeBatteryState()
        {
            int size = Marshal.SizeOf<SYSTEM_BATTERY_STATE>();
            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                uint status = CallNtPowerInformation(SystemBatteryState, IntPtr.Zero, 0, ptr, (uint)size);
                if (status == 0) return Marshal.PtrToStructure<SYSTEM_BATTERY_STATE>(ptr);
                return null;
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        private static string? _batteryDevicePath;

        private static string? GetBatteryDevicePath()
        {
            if (_batteryDevicePath != null) return _batteryDevicePath;

            Guid batteryGuid = GUID_DEVINTERFACE_BATTERY;
            IntPtr deviceInfoSet = SetupDiGetClassDevs(ref batteryGuid, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);
            if (deviceInfoSet == INVALID_HANDLE_VALUE) return null;

            try
            {
                SP_DEVICE_INTERFACE_DATA did = new();
                did.cbSize = (uint)Marshal.SizeOf<SP_DEVICE_INTERFACE_DATA>();

                if (!SetupDiEnumDeviceInterfaces(deviceInfoSet, IntPtr.Zero, ref batteryGuid, 0, ref did))
                    return null;

                SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref did, IntPtr.Zero, 0, out uint requiredSize, IntPtr.Zero);
                if (requiredSize == 0) return null;

                IntPtr detailData = Marshal.AllocHGlobal((int)requiredSize);
                try
                {
                    Marshal.WriteInt32(detailData, IntPtr.Size == 8 ? 8 : 6);

                    if (!SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref did, detailData, requiredSize, out _, IntPtr.Zero))
                        return null;

                    _batteryDevicePath = Marshal.PtrToStringAuto(detailData + 4);
                    return _batteryDevicePath;
                }
                finally
                {
                    Marshal.FreeHGlobal(detailData);
                }
            }
            finally
            {
                SetupDiDestroyDeviceInfoList(deviceInfoSet);
            }
        }

        private static BATTERY_STATUS? QueryBatteryStatus()
        {
            string? devicePath = GetBatteryDevicePath();
            if (devicePath == null) return null;

            IntPtr handle = CreateFile(devicePath, GENERIC_READ | GENERIC_WRITE,
                FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);

            if (handle == INVALID_HANDLE_VALUE) return null;

            try
            {
                uint timeout = 0;
                uint batteryTag = 0;
                if (!DeviceIoControl(handle, IOCTL_BATTERY_QUERY_TAG,
                    ref timeout, 4, ref batteryTag, 4, out _, IntPtr.Zero) || batteryTag == 0)
                    return null;

                BATTERY_WAIT_STATUS waitStatus = new() { BatteryTag = batteryTag };
                BATTERY_STATUS status = new();

                if (!DeviceIoControlStatus(handle, IOCTL_BATTERY_QUERY_STATUS,
                    ref waitStatus, (uint)Marshal.SizeOf<BATTERY_WAIT_STATUS>(),
                    ref status, (uint)Marshal.SizeOf<BATTERY_STATUS>(),
                    out _, IntPtr.Zero))
                    return null;

                return status;
            }
            finally
            {
                CloseHandle(handle);
            }
        }

        private static BATTERY_INFORMATION? QueryBatteryInformation()
        {
            string? devicePath = GetBatteryDevicePath();
            if (devicePath == null) return null;

            IntPtr handle = CreateFile(devicePath, GENERIC_READ | GENERIC_WRITE,
                FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);

            if (handle == INVALID_HANDLE_VALUE) return null;

            try
            {
                uint timeout = 0;
                uint batteryTag = 0;
                if (!DeviceIoControl(handle, IOCTL_BATTERY_QUERY_TAG,
                    ref timeout, 4, ref batteryTag, 4, out _, IntPtr.Zero) || batteryTag == 0)
                    return null;

                BATTERY_QUERY_INFORMATION query = new() { BatteryTag = batteryTag, InformationLevel = 0 };
                BATTERY_INFORMATION info = new();

                if (!DeviceIoControlInformation(handle, IOCTL_BATTERY_QUERY_INFORMATION,
                    ref query, (uint)Marshal.SizeOf<BATTERY_QUERY_INFORMATION>(),
                    ref info, (uint)Marshal.SizeOf<BATTERY_INFORMATION>(),
                    out _, IntPtr.Zero))
                    return null;

                return info;
            }
            finally
            {
                CloseHandle(handle);
            }
        }

        #endregion

        static long _lastBatteryRead;

        public static void ReadBatteryState()
        {
            var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            if (Math.Abs(now - _lastBatteryRead) < 5000)
            {
                FormatBatteryCharge();
                return;
            }
            _lastBatteryRead = now;

            batteryRate = 0;
            chargeCapacity = 0;

            try
            {
                if (AppConfig.IsAlly())
                {
                    decimal? discharge = Program.acpi.GetBatteryDischarge();
                    if (discharge is not null)
                    {
                        batteryRate = discharge;

                        var batteryState = GetNativeBatteryState();
                        if (batteryState.HasValue) chargeCapacity = batteryState.Value.RemainingCapacity;

                        if (fullCapacity is null or 0) ReadDesignCapacity();
                        FormatBatteryCharge();
                        return;
                    }
                }

                var statusTask = Task.Run(QueryBatteryStatus);
                var directStatus = statusTask.Wait(1000) ? statusTask.Result : null;

                if (directStatus.HasValue)
                {
                    chargeCapacity = directStatus.Value.Capacity;
                    if (directStatus.Value.Rate != 0)
                        batteryRate = (decimal)directStatus.Value.Rate / 1000;
                }

                if (fullCapacity is null or 0) ReadDesignCapacity();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Battery Reading: " + ex.Message);
            }

            FormatBatteryCharge();
        }

        private static void FormatBatteryCharge()
        {
            if (fullCapacity > 0 && chargeCapacity > 0)
            {
                batteryCapacity = Math.Min(100, (decimal)chargeCapacity / (decimal)fullCapacity * 100);
                if (batteryCapacity > 99 && BatteryControl.chargeFull) BatteryControl.UnSetBatteryLimitFull();
                batteryCharge = chargeWatt
                    ? Math.Round((decimal)chargeCapacity / 1000, 1) + "Wh"
                    : Math.Round(batteryCapacity, 1) + "%";
            }
        }

        public static void ReadDesignCapacity()
        {
            try
            {
                var infoTask = Task.Run(QueryBatteryInformation);
                var info = infoTask.Wait(1000) ? infoTask.Result : null;
                if (info.HasValue)
                {
                    if (info.Value.DesignedCapacity > 0) designCapacity = info.Value.DesignedCapacity;
                    if (info.Value.FullChargedCapacity > 0) fullCapacity = info.Value.FullChargedCapacity;
                }

                if (designCapacity is null or 0)
                {
                    ManagementScope scope = new ManagementScope("root\\WMI");
                    ObjectQuery query = new ObjectQuery("SELECT DesignedCapacity FROM BatteryStaticData");

                    using ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
                    foreach (ManagementObject obj in searcher.Get().Cast<ManagementObject>())
                    {
                        using (obj)
                        {
                            designCapacity = Convert.ToDecimal(obj["DesignedCapacity"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Design Capacity Reading: " + ex.Message);
            }
        }

        public static void RefreshBatteryHealth()
        {
            ReadDesignCapacity();
            if (fullCapacity is null or 0) ReadBatteryState();

            if (designCapacity is null || fullCapacity is null || designCapacity == 0 || fullCapacity == 0)
            {
                batteryHealth = -1;
                return;
            }

            decimal health = (decimal)fullCapacity / (decimal)designCapacity;
            Logger.WriteLine("Design Capacity: " + designCapacity + "mWh, Full Charge Capacity: " + fullCapacity + "mWh, Health: " + health + "%");
            batteryHealth = health * 100;
        }

        public static double GetBatteryChargePercentage()
        {
            try
            {
                SYSTEM_POWER_STATUS status = default;
                if (GetSystemPowerStatus(ref status) && status.BatteryLifePercent != 255)
                {
                    return status.BatteryLifePercent;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Battery Percentage Reading: " + ex.Message);
            }
            return 0;
        }
    }
}
