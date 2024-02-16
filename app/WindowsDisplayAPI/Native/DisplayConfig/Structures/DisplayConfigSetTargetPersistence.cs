using System.Runtime.InteropServices;
using WindowsDisplayAPI.Native.Structures;

namespace WindowsDisplayAPI.Native.DisplayConfig.Structures
{
    // https://msdn.microsoft.com/en-us/library/vs/alm/ff553981(v=vs.85).aspx
    [StructLayout(LayoutKind.Sequential)]
    internal struct DisplayConfigSetTargetPersistence
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        [MarshalAs(UnmanagedType.Struct)] private readonly DisplayConfigDeviceInfoHeader _Header;
        [MarshalAs(UnmanagedType.U4)] private readonly uint _BootPersistenceOn;

        public bool BootPersistence
        {
            get => _BootPersistenceOn > 0;
        }

        public DisplayConfigSetTargetPersistence(LUID adapter, uint targetId, bool bootPersistence) : this()
        {
            _Header = new DisplayConfigDeviceInfoHeader(adapter, targetId, GetType());
            _BootPersistenceOn = bootPersistence ? 1u : 0u;
        }
    }
}