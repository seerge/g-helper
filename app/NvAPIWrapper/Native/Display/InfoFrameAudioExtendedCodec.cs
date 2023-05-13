namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Contains possible extended audio codecs
    /// </summary>
    public enum InfoFrameAudioExtendedCodec : uint
    {
        /// <summary>
        ///     Use the primary audio codec type, data not available
        /// </summary>
        UseCodecType = 0,

        /// <summary>
        ///     High-Efficiency Advanced Audio Coding
        /// </summary>
        HEAAC,

        /// <summary>
        ///     High-Efficiency Advanced Audio Coding 2
        /// </summary>
        HEAACVersion2,

        /// <summary>
        ///     MPEG Surround
        /// </summary>
        MPEGSurround,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 63
    }
}