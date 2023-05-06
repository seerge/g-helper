namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    /// Possible display port color depths
    /// </summary>
    public enum DisplayPortColorDepth : uint
    {
        /// <summary>
        /// Default color depth
        /// </summary>
        Default = 0,
        /// <summary>
        /// 6 bit per color color depth
        /// </summary>
        BPC6,
        /// <summary>
        /// 8 bit per color color depth
        /// </summary>
        BPC8,
        /// <summary>
        /// 10 bit per color color depth
        /// </summary>
        BPC10,
        /// <summary>
        /// 12 bit per color color depth
        /// </summary>
        BPC12,

        /// <summary>
        /// 16 bit per color color depth
        /// </summary>
        BPC16,
    }
}