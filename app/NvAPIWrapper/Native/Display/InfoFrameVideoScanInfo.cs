namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Contains possible values for AVI scan information
    /// </summary>
    public enum InfoFrameVideoScanInfo : uint
    {
        /// <summary>
        ///     No data available
        /// </summary>
        NoData = 0,

        /// <summary>
        ///     Overscan
        /// </summary>
        OverScan,

        /// <summary>
        ///     Underscan
        /// </summary>
        UnderScan,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 7
    }
}