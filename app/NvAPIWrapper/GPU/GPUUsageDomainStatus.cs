using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.Interfaces.GPU;

namespace NvAPIWrapper.GPU
{
    /// <summary>
    ///     Holds information about a utilization domain
    /// </summary>
    public class GPUUsageDomainStatus
    {
        internal GPUUsageDomainStatus(UtilizationDomain domain, IUtilizationDomainInfo utilizationDomainInfo)
        {
            Domain = domain;
            Percentage = (int) utilizationDomainInfo.Percentage;
        }

        /// <summary>
        ///     Gets the utilization domain that this instance describes
        /// </summary>
        public UtilizationDomain Domain { get; }

        /// <summary>
        ///     Gets the percentage of time where the domain is considered busy in the last 1 second interval.
        /// </summary>
        public int Percentage { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"[{Domain}] {Percentage}%";
        }
    }
}