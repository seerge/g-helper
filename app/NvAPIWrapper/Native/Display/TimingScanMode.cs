namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Timing scan modes
    /// </summary>
    public enum TimingScanMode : ushort
    {
        /// <summary>
        ///     Progressive scan mode
        /// </summary>
        Progressive = 0,

        /// <summary>
        ///     Interlaced scan mode
        /// </summary>
        Interlaced = 1,

        /// <summary>
        ///     Interlaced scan mode with extra vertical blank
        /// </summary>
        InterlacedWithExtraVerticalBlank = 1,

        /// <summary>
        ///     Interlaced scan mode without extra vertical blank
        /// </summary>
        InterlacedWithNoExtraVerticalBlank = 2
    }
}