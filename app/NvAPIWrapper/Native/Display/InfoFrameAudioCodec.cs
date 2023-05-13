namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Contains possible audio codecs
    /// </summary>
    public enum InfoFrameAudioCodec : uint
    {
        /// <summary>
        ///     Data is available in the header of source data
        /// </summary>
        InHeader = 0,

        /// <summary>
        ///     Pulse-code modulation
        /// </summary>
        PCM,

        /// <summary>
        ///     Dolby AC-3
        /// </summary>
        AC3,

        /// <summary>
        ///     MPEG1
        /// </summary>
        MPEG1,

        /// <summary>
        ///     MP3 (MPEG-2 Audio Layer III)
        /// </summary>
        MP3,

        /// <summary>
        ///     MPEG2
        /// </summary>
        MPEG2,

        /// <summary>
        ///     Advanced Audio Coding
        /// </summary>
        AACLC,

        /// <summary>
        ///     DTS
        /// </summary>
        DTS,

        /// <summary>
        ///     Adaptive Transform Acoustic Coding
        /// </summary>
        ATRAC,

        /// <summary>
        ///     Direct Stream Digital
        /// </summary>
        DSD,

        /// <summary>
        ///     Dolby Digital Plus
        /// </summary>
        EAC3,

        /// <summary>
        ///     DTS High Definition
        /// </summary>
        DTSHD,

        /// <summary>
        ///     Meridian Lossless Packing
        /// </summary>
        MLP,

        /// <summary>
        ///     DST
        /// </summary>
        DST,

        /// <summary>
        ///     Windows Media Audio Pro
        /// </summary>
        WMAPRO,

        /// <summary>
        ///     Extended audio codec value should be used to get information regarding audio codec
        /// </summary>
        UseExtendedCodecType,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 31
    }
}