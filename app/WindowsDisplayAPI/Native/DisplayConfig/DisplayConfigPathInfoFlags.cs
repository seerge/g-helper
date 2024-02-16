using System;

namespace WindowsDisplayAPI.Native.DisplayConfig
{
    [Flags]
    internal enum DisplayConfigPathInfoFlags : uint
    {
        None = 0,
        Active = 1,
        SupportVirtualMode = 8
    }
}