namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Gets the possible values for AVI source aspect ratio
    /// </summary>
    public enum InfoFrameVideoAspectRatioCodedFrame : uint
    {
        /// <summary>
        ///     No data available
        /// </summary>
        NoData = 0,

        /// <summary>
        ///     The 4x3 aspect ratio
        /// </summary>
        Aspect4X3,

        /// <summary>
        ///     The 16x9 aspect ratio
        /// </summary>
        Aspect16X9,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 7
    }
}