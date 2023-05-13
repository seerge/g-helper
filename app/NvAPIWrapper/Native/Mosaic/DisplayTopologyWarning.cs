using System;

namespace NvAPIWrapper.Native.Mosaic
{
    /// <summary>
    ///     Possible display problems in a topology validation process
    /// </summary>
    [Flags]
    public enum DisplayTopologyWarning : uint
    {
        /// <summary>
        ///     No warning
        /// </summary>
        NoWarning = 0,

        /// <summary>
        ///     Display position is problematic
        /// </summary>
        DisplayPosition = 1,

        /// <summary>
        ///     Driver reload is required for this changes
        /// </summary>
        DriverReloadRequired = 2
    }
}