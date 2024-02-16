using System.Runtime.InteropServices;
using WindowsDisplayAPI.Native.Structures;

namespace WindowsDisplayAPI.Native.DisplayConfig.Structures
{
    // Internal undocumented structure
    [StructLayout(LayoutKind.Sequential)]
    internal struct DisplayConfigGetSourceDPIScale
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        [MarshalAs(UnmanagedType.Struct)] private readonly DisplayConfigDeviceInfoHeader _Header;

        [field: MarshalAs(UnmanagedType.U4)]
        public int MinimumScaleSteps { get; }

        [field: MarshalAs(UnmanagedType.U4)]
        public int CurrentScaleSteps { get; }

        [field: MarshalAs(UnmanagedType.U4)]
        public int MaximumScaleSteps { get; }

        public DisplayConfigGetSourceDPIScale(LUID adapter, uint sourceId) : this()
        {
            _Header = new DisplayConfigDeviceInfoHeader(adapter, sourceId, GetType());
        }
    }
}