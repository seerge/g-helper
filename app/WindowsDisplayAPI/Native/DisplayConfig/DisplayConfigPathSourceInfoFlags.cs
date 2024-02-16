using System;

namespace WindowsDisplayAPI.Native.DisplayConfig
{
    [Flags]
    internal enum DisplayConfigPathSourceInfoFlags : uint
    {
        None = 0,
        InUse = 1
    }
}