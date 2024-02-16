using System.Runtime.InteropServices;

namespace WindowsDisplayAPI.Native.DeviceContext.Structures
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DisplayDevice
    {
        [MarshalAs(UnmanagedType.U4)] internal uint Size;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public readonly string DeviceName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public readonly string DeviceString;

        [MarshalAs(UnmanagedType.U4)] public readonly DisplayDeviceStateFlags StateFlags;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public readonly string DeviceId;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public readonly string DeviceKey;

        public static DisplayDevice Initialize()
        {
            return new DisplayDevice
            {
                Size = (uint) Marshal.SizeOf(typeof(DisplayDevice))
            };
        }
    }
}