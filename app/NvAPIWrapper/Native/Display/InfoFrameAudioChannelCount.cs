namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Contains possible audio channels
    /// </summary>
    public enum InfoFrameAudioChannelCount : uint
    {
        /// <summary>
        ///     Data is available in the header of source data
        /// </summary>
        InHeader = 0,

        /// <summary>
        ///     Two channels
        /// </summary>
        Two,

        /// <summary>
        ///     Three channels
        /// </summary>
        Three,

        /// <summary>
        ///     Four channels
        /// </summary>
        Four,

        /// <summary>
        ///     Five channels
        /// </summary>
        Five,

        /// <summary>
        ///     Six channels
        /// </summary>
        Six,

        /// <summary>
        ///     Seven channels
        /// </summary>
        Seven,

        /// <summary>
        ///     Eight channels
        /// </summary>
        Eight,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 15
    }
}