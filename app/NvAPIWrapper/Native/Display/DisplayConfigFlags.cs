using System;

namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Flags for applying settings, used by NvAPI_DISP_SetDisplayConfig()
    /// </summary>
    [Flags]
    public enum DisplayConfigFlags
    {
        /// <summary>
        ///     None
        /// </summary>
        None = 0,

        /// <summary>
        ///     Do not apply
        /// </summary>
        ValidateOnly = 0x00000001,

        /// <summary>
        ///     Save to the persistence storage
        /// </summary>
        SaveToPersistence = 0x00000002,

        /// <summary>
        ///     Driver reload is permitted if necessary
        /// </summary>
        DriverReloadAllowed = 0x00000004,

        /// <summary>
        ///     Refresh OS mode list.
        /// </summary>
        ForceModeEnumeration = 0x00000008,

        /// <summary>
        ///     Tell OS to avoid optimizing CommitVidPn call during a modeset
        /// </summary>
        ForceCommitVideoPresentNetwork = 0x00000010
    }
}