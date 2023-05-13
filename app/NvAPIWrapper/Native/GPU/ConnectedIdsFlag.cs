using System;

namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     Flags used for retrieving a list of display identifications
    /// </summary>
    [Flags]
    public enum ConnectedIdsFlag : uint
    {
        /// <summary>
        ///     No specific flag
        /// </summary>
        None = 0,

        /// <summary>
        ///     Get un-cached connected devices
        /// </summary>
        UnCached = 1,

        /// <summary>
        ///     Get devices such that those can be selected in an SLI configuration
        /// </summary>
        SLI = 2,

        /// <summary>
        ///     Get devices such that to reflect the Lid State
        /// </summary>
        LidState = 4,

        /// <summary>
        ///     Get devices that includes the fake connected monitors
        /// </summary>
        Fake = 8,

        /// <summary>
        ///     Excludes devices that are part of the multi stream topology
        /// </summary>
        ExcludeList = 16
    }
}