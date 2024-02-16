using System;

namespace WindowsDisplayAPI.Native.DisplayConfig
{
    /// <summary>
    ///     Possible topology identifications
    ///     https://msdn.microsoft.com/en-us/library/windows/hardware/ff554001(v=vs.85).aspx
    /// </summary>
    [Flags]
    public enum DisplayConfigTopologyId : uint
    {
        /// <summary>
        ///     Invalid topology identification
        /// </summary>
        None = 0,

        /// <summary>
        ///     Indicates that the display topology is an internal configuration.
        /// </summary>
        Internal = 0x00000001,

        /// <summary>
        ///     Indicates that the display topology is clone-view configuration.
        /// </summary>
        Clone = 0x00000002,

        /// <summary>
        ///     Indicates that the display topology is an extended configuration.
        /// </summary>
        Extend = 0x00000004,

        /// <summary>
        ///     Indicates that the display topology is an external configuration.
        /// </summary>
        External = 0x00000008
    }
}