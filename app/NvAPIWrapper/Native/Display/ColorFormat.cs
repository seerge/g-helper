namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Possible color formats
    /// </summary>
    public enum ColorFormat
    {
        /// <summary>
        ///     Unknown, driver will choose one automatically.
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///     8bpp mode
        /// </summary>
        P8 = 41,

        /// <summary>
        ///     16bpp mode
        /// </summary>
        R5G6B5 = 23,

        /// <summary>
        ///     32bpp mode
        /// </summary>
        A8R8G8B8 = 21,

        /// <summary>
        ///     64bpp (floating point)
        /// </summary>
        A16B16G16R16F = 113
    }
}