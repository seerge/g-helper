using System.Runtime.InteropServices;
using WindowsDisplayAPI.Native.Structures;

namespace WindowsDisplayAPI.Native.DisplayConfig.Structures
{
    // https://msdn.microsoft.com/en-us/library/vs/alm/dn362043(v=vs.85).aspx
    [StructLayout(LayoutKind.Sequential)]
    internal struct DisplayConfigTargetBaseType
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        [MarshalAs(UnmanagedType.Struct)] private readonly DisplayConfigDeviceInfoHeader _Header;
        [MarshalAs(UnmanagedType.U4)] public readonly DisplayConfigVideoOutputTechnology BaseOutputTechnology;

        public DisplayConfigTargetBaseType(LUID adapter, uint targetId) : this()
        {
            _Header = new DisplayConfigDeviceInfoHeader(adapter, targetId, GetType());
        }
    }
}