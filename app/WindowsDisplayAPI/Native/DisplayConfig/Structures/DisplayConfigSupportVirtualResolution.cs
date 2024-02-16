using System.Runtime.InteropServices;
using WindowsDisplayAPI.Native.Structures;

namespace WindowsDisplayAPI.Native.DisplayConfig.Structures
{
    // https://msdn.microsoft.com/en-us/library/vs/alm/mt622103(v=vs.85).aspx
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DisplayConfigSupportVirtualResolution
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        [MarshalAs(UnmanagedType.Struct)] private readonly DisplayConfigDeviceInfoHeader _Header;
        [MarshalAs(UnmanagedType.U4)] private readonly int _DisableMonitorVirtualResolution;

        public bool DisableMonitorVirtualResolution
        {
            get => _DisableMonitorVirtualResolution > 0;
        }

        public DisplayConfigSupportVirtualResolution(LUID adapter, uint targetId) : this()
        {
            _Header = new DisplayConfigDeviceInfoHeader(adapter, targetId, GetType(),
                DisplayConfigDeviceInfoType.GetSupportVirtualResolution);
        }

        public DisplayConfigSupportVirtualResolution(LUID adapter, uint targetId, bool disableMonitorVirtualResolution)
            : this()
        {
            _DisableMonitorVirtualResolution = disableMonitorVirtualResolution ? 1 : 0;
            _Header = new DisplayConfigDeviceInfoHeader(adapter, targetId, GetType(),
                DisplayConfigDeviceInfoType.SetSupportVirtualResolution);
        }
    }
}