using System;

namespace WindowsDisplayAPI.Native.DisplayConfig
{
    [Flags]
    internal enum DisplayConfigTargetDeviceNameFlags : uint
    {
        None = 0,
        FriendlyNameFromEDID = 1,
        FriendlyNameForced = 2,
        EDIDIdsValid = 4
    }
}