namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Contains possible audio low frequency effects channel playback level
    /// </summary>
    public enum InfoFrameAudioLFEPlaybackLevel : uint
    {
        /// <summary>
        ///     Data not available
        /// </summary>
        NoData = 0,

        /// <summary>
        ///     No change to the source audio
        /// </summary>
        Plus0Decibel,

        /// <summary>
        ///     Adds 10 decibel
        /// </summary>
        Plus10Decibel,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 7
    }
}