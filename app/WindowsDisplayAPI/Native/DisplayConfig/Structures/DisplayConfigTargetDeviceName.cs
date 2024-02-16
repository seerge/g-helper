using System.Runtime.InteropServices;
using WindowsDisplayAPI.Native.Structures;

namespace WindowsDisplayAPI.Native.DisplayConfig.Structures
{
    // https://msdn.microsoft.com/en-us/library/vs/alm/ff553989(v=vs.85).aspx
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DisplayConfigTargetDeviceName
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        [MarshalAs(UnmanagedType.Struct)] private readonly DisplayConfigDeviceInfoHeader _Header;
        [MarshalAs(UnmanagedType.U4)] public readonly DisplayConfigTargetDeviceNameFlags Flags;
        [MarshalAs(UnmanagedType.U4)] public readonly DisplayConfigVideoOutputTechnology OutputTechnology;
        [MarshalAs(UnmanagedType.U2)] public readonly ushort EDIDManufactureId;
        [MarshalAs(UnmanagedType.U2)] public readonly ushort EDIDProductCodeId;
        [MarshalAs(UnmanagedType.U4)] public readonly uint ConnectorInstance;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public readonly string MonitorFriendlyDeviceName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public readonly string MonitorDevicePath;


        public DisplayConfigTargetDeviceName(LUID adapter, uint targetId) : this()
        {
            _Header = new DisplayConfigDeviceInfoHeader(adapter, targetId, GetType());
        }
    }
}