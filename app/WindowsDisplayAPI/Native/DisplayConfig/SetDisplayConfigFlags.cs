using System;

namespace WindowsDisplayAPI.Native.DisplayConfig
{
    [Flags]
    internal enum SetDisplayConfigFlags : uint
    {
        TopologyInternal = 0x00000001,
        TopologyClone = 0x00000002,
        TopologyExtend = 0x00000004,
        TopologyExternal = 0x00000008,
        UseDatabaseCurrent = TopologyInternal | TopologyClone | TopologyExtend | TopologyExternal,
        TopologySupplied = 0x00000010,
        UseSuppliedDisplayConfig = 0x00000020,
        Validate = 0x00000040,
        Apply = 0x00000080,
        NoOptimization = 0x00000100,
        SaveToDatabase = 0x00000200,
        AllowChanges = 0x00000400,
        PathPersistIfRequired = 0x00000800,
        ForceModeEnumeration = 0x00001000,
        AllowPathOrderChanges = 0x00002000,
        VirtualModeAware = 0x00008000
    }
}