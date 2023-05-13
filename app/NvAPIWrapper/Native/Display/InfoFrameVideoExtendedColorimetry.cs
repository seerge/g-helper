namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Contains possible values for the AVI extended color space
    /// </summary>
    public enum InfoFrameVideoExtendedColorimetry : uint
    {
        /// <summary>
        ///     The xvYCC601 color space
        /// </summary>
        // ReSharper disable once InconsistentNaming
        xvYCC601 = 0,

        /// <summary>
        ///     The xvYCC709 color space
        /// </summary>
        // ReSharper disable once InconsistentNaming
        xvYCC709,

        /// <summary>
        ///     The sYCC601 color space
        /// </summary>
        // ReSharper disable once InconsistentNaming
        sYCC601,

        /// <summary>
        ///     The AdobeYCC601 color space
        /// </summary>
        AdobeYCC601,

        /// <summary>
        ///     The AdobeRGB color space
        /// </summary>
        AdobeRGB,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 15
    }
}