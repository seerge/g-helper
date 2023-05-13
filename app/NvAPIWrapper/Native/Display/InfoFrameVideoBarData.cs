namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Contains possible AVI bar data that are available and should be used
    /// </summary>
    public enum InfoFrameVideoBarData : uint
    {
        /// <summary>
        ///     No bar data present
        /// </summary>
        NotPresent = 0,

        /// <summary>
        ///     Vertical bar
        /// </summary>
        Vertical,

        /// <summary>
        ///     Horizontal bar
        /// </summary>
        Horizontal,

        /// <summary>
        ///     Both sides have bars
        /// </summary>
        Both,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 7
    }
}