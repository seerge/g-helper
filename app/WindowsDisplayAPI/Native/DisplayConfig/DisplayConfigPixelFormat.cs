namespace WindowsDisplayAPI.Native.DisplayConfig
{
    /// <summary>
    ///     Possible pixel formats
    ///     https://msdn.microsoft.com/en-us/library/windows/hardware/ff553963(v=vs.85).aspx
    /// </summary>
    public enum DisplayConfigPixelFormat : uint
    {
        /// <summary>
        ///     Pixel format is not specified
        /// </summary>
        NotSpecified = 0,

        /// <summary>
        ///     Indicates 8 bits per pixel format.
        /// </summary>
        PixelFormat8Bpp = 1,

        /// <summary>
        ///     Indicates 16 bits per pixel format.
        /// </summary>
        PixelFormat16Bpp = 2,

        /// <summary>
        ///     Indicates 24 bits per pixel format.
        /// </summary>
        PixelFormat24Bpp = 3,

        /// <summary>
        ///     Indicates 32 bits per pixel format.
        /// </summary>
        PixelFormat32Bpp = 4,

        /// <summary>
        ///     Indicates that the current display is not an 8, 16, 24, or 32 bits per pixel GDI desktop mode.
        /// </summary>
        PixelFormatNonGDI = 5
    }
}