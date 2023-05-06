namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Contains possible AVI color formats
    /// </summary>
    public enum InfoFrameVideoColorFormat : uint
    {
        /// <summary>
        ///     The RGB color format
        /// </summary>
        RGB = 0,

        /// <summary>
        ///     The YCbCr422 color format
        /// </summary>
        YCbCr422,

        /// <summary>
        ///     The YCbCr444 color format
        /// </summary>
        YCbCr444,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 7
    }
}