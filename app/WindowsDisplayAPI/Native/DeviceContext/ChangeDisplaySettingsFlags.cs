using System;

namespace WindowsDisplayAPI.Native.DeviceContext
{
    [Flags]
    internal enum ChangeDisplaySettingsFlags : uint
    {
        UpdateRegistry = 0x00000001,

        Global = 0x00000008,

        SetPrimary = 0x00000010,

        Reset = 0x40000000,

        NoReset = 0x10000000
    }
}