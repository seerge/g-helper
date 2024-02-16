using System.Runtime.InteropServices;
using WindowsDisplayAPI.Native.Structures;

namespace WindowsDisplayAPI.Native.DisplayConfig.Structures
{
    // https://msdn.microsoft.com/en-us/library/vs/alm/ff553996(v=vs.85).aspx
    [StructLayout(LayoutKind.Sequential)]
    internal struct DisplayConfigTargetPreferredMode
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        [MarshalAs(UnmanagedType.Struct)] private readonly DisplayConfigDeviceInfoHeader _Header;
        [MarshalAs(UnmanagedType.U4)] public readonly uint Width;
        [MarshalAs(UnmanagedType.U4)] public readonly uint Height;
        [MarshalAs(UnmanagedType.Struct)] public readonly DisplayConfigTargetMode TargetMode;

        public DisplayConfigTargetPreferredMode(LUID adapter, uint targetId) : this()
        {
            _Header = new DisplayConfigDeviceInfoHeader(adapter, targetId, GetType());
        }
    }
}