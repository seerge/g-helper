namespace WindowsDisplayAPI.Native.DisplayConfig
{
    /// <summary>
    ///     Scaling modes
    ///     https://msdn.microsoft.com/en-us/library/windows/hardware/ff553974(v=vs.85).aspx
    /// </summary>
    public enum DisplayConfigScaling : uint
    {
        /// <summary>
        ///     Scaling mode is not specified
        /// </summary>
        NotSpecified = 0,

        /// <summary>
        ///     Indicates the identity transformation; the source content is presented with no change. This transformation is
        ///     available only if the path's source mode has the same spatial resolution as the path's target mode.
        /// </summary>
        Identity = 1,

        /// <summary>
        ///     Indicates the centering transformation; the source content is presented unscaled, centered with respect to the
        ///     spatial resolution of the target mode.
        /// </summary>
        Centered = 2,

        /// <summary>
        ///     Indicates the content is scaled to fit the path's target.
        /// </summary>
        Stretched = 3,

        /// <summary>
        ///     Indicates the aspect-ratio centering transformation.
        /// </summary>
        AspectRatioCenteredMax = 4,

        /// <summary>
        ///     Indicates that the caller requests a custom scaling that the caller cannot describe with any of the other values.
        ///     Only a hardware vendor's value-add application should use this value, because the value-add application might
        ///     require a private interface to the driver. The application can then use this value to indicate additional context
        ///     for the driver for the custom value on the specified path.
        /// </summary>
        Custom = 5,

        /// <summary>
        ///     Indicates that the caller does not have any preference for the scaling.
        /// </summary>
        Preferred = 128
    }
}