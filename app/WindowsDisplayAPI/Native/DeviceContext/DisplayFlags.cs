using System;

namespace WindowsDisplayAPI.Native.DeviceContext
{
    [Flags]
    internal enum DisplayFlags : uint
    {
        None = 0,
        Grayscale = 1,
        Interlaced = 2
    }
}