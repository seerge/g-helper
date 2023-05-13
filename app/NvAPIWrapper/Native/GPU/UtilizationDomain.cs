namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     Valid utilization domain
    /// </summary>
    public enum UtilizationDomain
    {
        /// <summary>
        ///     GPU utilization domain
        /// </summary>
        GPU,

        /// <summary>
        ///     Frame buffer utilization domain
        /// </summary>
        FrameBuffer,

        /// <summary>
        ///     Video engine utilization domain
        /// </summary>
        VideoEngine,

        /// <summary>
        ///     Bus interface utilization domain
        /// </summary>
        BusInterface
    }
}