namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Contains possible values for the AVI RGB quantization
    /// </summary>
    public enum InfoFrameVideoRGBQuantization : uint
    {
        /// <summary>
        ///     Default setting
        /// </summary>
        Default = 0,

        /// <summary>
        ///     Limited RGB range [16-235] (86%)
        /// </summary>
        LimitedRange,

        /// <summary>
        ///     Full RGB range [0-255] (100%)
        /// </summary>
        FullRange,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 7
    }
}