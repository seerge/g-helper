namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Contains possible color data color format values
    /// </summary>
    public enum ColorDataFormat : uint
    {
        /// <summary>
        ///     RGB color format
        /// </summary>
        RGB = 0,

        /// <summary>
        ///     YUV422 color format
        /// </summary>
        YUV422,

        /// <summary>
        ///     YUV444 color format
        /// </summary>
        YUV444,

        /// <summary>
        ///     YUV420 color format
        /// </summary>
        YUV420,

        /// <summary>
        ///     Default color format
        /// </summary>
        Default = 0xFE,

        /// <summary>
        ///     Automatically select the best color format
        /// </summary>
        Auto = 0xFF
    }
}