namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Contains possible values for the AVI color space
    /// </summary>
    public enum InfoFrameVideoColorimetry : uint
    {
        /// <summary>
        ///     No data available
        /// </summary>
        NoData = 0,

        /// <summary>
        ///     The SMPTE170M color space
        /// </summary>
        SMPTE170M,

        /// <summary>
        ///     The ITURBT709 color space
        /// </summary>
        ITURBT709,

        /// <summary>
        ///     Extended colorimetry value should be used to get information regarding AVI color space
        /// </summary>
        UseExtendedColorimetry,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 7
    }
}