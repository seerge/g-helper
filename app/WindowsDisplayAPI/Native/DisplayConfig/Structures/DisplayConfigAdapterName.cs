using System.Runtime.InteropServices;
using WindowsDisplayAPI.Native.Structures;

namespace WindowsDisplayAPI.Native.DisplayConfig.Structures
{
    // https://msdn.microsoft.com/en-us/library/vs/alm/ff553915(v=vs.85).aspx
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DisplayConfigAdapterName
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        [MarshalAs(UnmanagedType.Struct)] private readonly DisplayConfigDeviceInfoHeader _Header;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public readonly string AdapterDevicePath;

        public DisplayConfigAdapterName(LUID adapter) : this()
        {
            _Header = new DisplayConfigDeviceInfoHeader(adapter, GetType());
        }
    }
}