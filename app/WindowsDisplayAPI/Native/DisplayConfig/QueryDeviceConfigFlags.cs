using System;

namespace WindowsDisplayAPI.Native.DisplayConfig
{
    /// <summary>
    ///     Possible values for QueryDisplayConfig() flags property
    ///     https://msdn.microsoft.com/en-us/library/windows/hardware/ff569215(v=vs.85).aspx
    /// </summary>
    [Flags]
    public enum QueryDeviceConfigFlags : uint
    {
        /// <summary>
        ///     All the possible path combinations of sources to targets.
        /// </summary>
        AllPaths = 0x00000001,

        /// <summary>
        ///     Currently active paths only.
        /// </summary>
        OnlyActivePaths = 0x00000002,

        /// <summary>
        ///     Active path as defined in the CCD database for the currently connected displays.
        /// </summary>
        DatabaseCurrent = 0x00000004,

        /// <summary>
        ///     Virtual Mode Aware
        /// </summary>
        VirtualModeAware = 0x0000010
    }
}