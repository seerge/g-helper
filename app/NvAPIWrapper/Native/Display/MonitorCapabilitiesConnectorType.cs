namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Possible values for the monitor capabilities connector type
    /// </summary>
    public enum MonitorCapabilitiesConnectorType : uint
    {
        /// <summary>
        ///     Unknown or invalid connector
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///     VGA connector
        /// </summary>
        VGA,

        /// <summary>
        ///     Composite connector (TV)
        /// </summary>
        TV,

        /// <summary>
        ///     DVI connector
        /// </summary>
        DVI,

        /// <summary>
        ///     HDMI connector
        /// </summary>
        HDMI,

        /// <summary>
        ///     Display Port connector
        /// </summary>
        DisplayPort
    }
}