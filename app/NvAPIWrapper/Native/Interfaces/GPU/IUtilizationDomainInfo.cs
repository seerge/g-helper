namespace NvAPIWrapper.Native.Interfaces.GPU
{

    /// <summary>
    ///     Holds information about a utilization domain
    /// </summary>
    public interface IUtilizationDomainInfo
    {
        /// <summary>
        ///     Gets a boolean value that indicates if this utilization domain is present on this GPU.
        /// </summary>
        bool IsPresent { get; }

        /// <summary>
        ///     Gets the percentage of time where the domain is considered busy in the last 1 second interval.
        /// </summary>
        uint Percentage { get; }
    }
}