using System;

namespace NvAPIWrapper.Native.General
{
    /// <summary>
    ///     Chipset information flags - obsolete
    /// </summary>
    [Flags]
    public enum ChipsetInfoFlag
    {
        /// <summary>
        ///     No flags
        /// </summary>
        None = 0,

        /// <summary>
        ///     Hybrid chipset configuration
        /// </summary>
        Hybrid = 1
    }
}