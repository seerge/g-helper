namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Contains possible values for the color data depth
    /// </summary>
    public enum ColorDataDepth : uint
    {
        /// <summary>
        ///     Default color depth meaning that the current setting should be kept
        /// </summary>
        Default = 0,

        /// <summary>
        ///     6bit per color depth
        /// </summary>
        BPC6 = 1,

        /// <summary>
        ///     8bit per color depth
        /// </summary>
        BPC8 = 2,

        /// <summary>
        ///     10bit per color depth
        /// </summary>
        BPC10 = 3,

        /// <summary>
        ///     12bit per color depth
        /// </summary>
        BPC12 = 4,

        /// <summary>
        ///     16bit per color depth
        /// </summary>
        BPC16 = 5
    }
}