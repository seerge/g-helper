namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Contains possible audio sample size (bit depth)
    /// </summary>
    public enum InfoFrameAudioSampleSize : uint
    {
        /// <summary>
        ///     Data is available in the header of source data
        /// </summary>
        InHeader = 0,

        /// <summary>
        ///     16bit audio sample size
        /// </summary>
        B16,

        /// <summary>
        ///     20bit audio sample size
        /// </summary>
        B20,

        /// <summary>
        ///     24bit audio sample size
        /// </summary>
        B24,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 7
    }
}