namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Contains possible values for the AVI non uniform picture scaling
    /// </summary>
    public enum InfoFrameVideoNonUniformPictureScaling : uint
    {
        /// <summary>
        ///     No data available
        /// </summary>
        NoData = 0,

        /// <summary>
        ///     Horizontal scaling
        /// </summary>
        Horizontal,

        /// <summary>
        ///     Vertical scaling
        /// </summary>
        Vertical,

        /// <summary>
        ///     Scaling in both directions
        /// </summary>
        Both,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 7
    }
}