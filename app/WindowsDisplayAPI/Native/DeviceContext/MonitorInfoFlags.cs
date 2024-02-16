using System;

namespace WindowsDisplayAPI.Native.DeviceContext
{
    [Flags]
    internal enum MonitorInfoFlags : uint
    {
        None = 0,
        Primary = 1
    }
}