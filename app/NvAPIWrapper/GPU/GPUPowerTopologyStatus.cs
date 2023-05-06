using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.GPU.Structures;

namespace NvAPIWrapper.GPU
{
    /// <summary>
    ///     Contains information about a power domain usage
    /// </summary>
    public class GPUPowerTopologyStatus
    {
        internal GPUPowerTopologyStatus(
            PrivatePowerTopologiesStatusV1.PowerTopologiesStatusEntry powerTopologiesStatusEntry)
        {
            Domain = powerTopologiesStatusEntry.Domain;
            PowerUsageInPCM = powerTopologiesStatusEntry.PowerUsageInPCM;
        }

        /// <summary>
        ///     Gets the power usage domain
        /// </summary>
        public PowerTopologyDomain Domain { get; }

        /// <summary>
        ///     Gets the current power usage in per cent mille (PCM)
        /// </summary>
        public uint PowerUsageInPCM { get; }

        /// <summary>
        ///     Gets the current power usage in percentage
        /// </summary>
        public float PowerUsageInPercent
        {
            get => PowerUsageInPCM / 1000f;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"[{Domain}] {PowerUsageInPercent}%";
        }
    }
}