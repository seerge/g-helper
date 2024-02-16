using System;

namespace WindowsDisplayAPI
{
    /// <summary>
    ///     Contains possible color management capabilities of a display device
    /// </summary>
    [Flags]
    public enum DisplayColorManagementCapabilities
    {
        /// <summary>
        ///     Device does not support ICM.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Device can perform ICM on either the device driver or the device itself.
        /// </summary>
        DeviceICM = 1,

        /// <summary>
        ///     Device supports gamma ramp modification and retrieval
        /// </summary>
        GammaRamp = 2,

        /// <summary>
        ///     Device can accept CMYK color space ICC color profile.
        /// </summary>
        CMYKColor = 4
    }
}