namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     Monitor connection types. This is reserved for future use and clients should not rely on this information.
    /// </summary>
    public enum MonitorConnectionType
    {
        /// <summary>
        ///     Monitor not yet initialized
        /// </summary>
        Uninitialized = 0,

        /// <summary>
        ///     Connected through a VGA compatible connector
        /// </summary>
        VGA,

        /// <summary>
        ///     Connected through a Component compatible connector
        /// </summary>
        Component,

        /// <summary>
        ///     Connected through a SVideo compatible connector
        /// </summary>
        SVideo,

        /// <summary>
        ///     Connected through a HDMI compatible connector
        /// </summary>
        HDMI,

        /// <summary>
        ///     Connected through a LVDS compatible connector
        /// </summary>
        DVI,

        /// <summary>
        ///     Connected through a DisplayPort compatible connector
        /// </summary>
        LVDS,

        /// <summary>
        ///     Connected through a DisplayPort compatible connector
        /// </summary>
        DisplayPort,

        /// <summary>
        ///     Connected through a Composite compatible connector
        /// </summary>
        Composite,

        /// <summary>
        ///     Connection type unknown
        /// </summary>
        Unknown = -1
    }
}