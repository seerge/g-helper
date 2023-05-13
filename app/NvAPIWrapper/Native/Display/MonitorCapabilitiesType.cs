namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Contains possible values for the monitor capabilities type
    /// </summary>
    public enum MonitorCapabilitiesType : uint
    {
        /// <summary>
        ///     The Vendor Specific Data Block
        /// </summary>
        VSDB = 0x1000,

        /// <summary>
        ///     The Video Capability Data Block
        /// </summary>
        VCDB = 0x1001
    }
}