namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Possible display port color formats
    /// </summary>
    public enum DisplayPortColorFormat : uint
    {
        /// <summary>
        ///     RGB color format
        /// </summary>
        RGB = 0,

        /// <summary>
        ///     YCbCr422 color format
        /// </summary>
        YCbCr422 = 1,

        /// <summary>
        ///     YCbCr444 color format
        /// </summary>
        YCbCr444 = 2
    }
}