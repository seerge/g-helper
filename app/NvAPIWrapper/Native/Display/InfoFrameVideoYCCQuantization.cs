namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Contains possible AVI YCC quantization
    /// </summary>
    public enum InfoFrameVideoYCCQuantization : uint
    {
        /// <summary>
        ///     Limited YCC range
        /// </summary>
        LimitedRange = 0,

        /// <summary>
        ///     Full YCC range
        /// </summary>
        FullRange,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 7
    }
}