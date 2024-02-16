using System.Runtime.InteropServices;
using WindowsDisplayAPI.Native.Structures;

namespace WindowsDisplayAPI.Native.DisplayConfig.Structures
{
    // https://msdn.microsoft.com/en-us/library/vs/alm/ff553983(v=vs.85).aspx
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DisplayConfigSourceDeviceName
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        [MarshalAs(UnmanagedType.Struct)] private readonly DisplayConfigDeviceInfoHeader _Header;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public readonly string DeviceName;

        public DisplayConfigSourceDeviceName(LUID adapter, uint sourceId) : this()
        {
            _Header = new DisplayConfigDeviceInfoHeader(adapter, sourceId, GetType());
        }
    }
}