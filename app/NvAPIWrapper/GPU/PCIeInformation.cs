using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.GPU.Structures;

namespace NvAPIWrapper.GPU
{
    /// <summary>
    ///     Contains information about the PCI-e connection
    /// </summary>
    public class PCIeInformation
    {
        internal PCIeInformation(PrivatePCIeInfoV2.PCIePerformanceStateInfo stateInfo)
        {
            TransferRateInMTps = stateInfo.TransferRateInMTps;
            Generation = stateInfo.Generation;
            Lanes = stateInfo.Lanes;
            Version = stateInfo.Version;
        }

        /// <summary>
        ///     Gets the PCI-e generation
        /// </summary>
        public PCIeGeneration Generation { get; }

        /// <summary>
        ///     Gets the PCI-e down stream lanes
        /// </summary>
        public uint Lanes { get; }

        /// <summary>
        ///     Gets the PCIe transfer rate in Mega Transfers per Second
        /// </summary>
        public uint TransferRateInMTps { get; }

        /// <summary>
        ///     Gets the PCI-e version
        /// </summary>
        public PCIeGeneration Version { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            var v = "Unknown";

            switch (Version)
            {
                case PCIeGeneration.PCIe1:
                    v = "PCIe 1.0";

                    break;
                case PCIeGeneration.PCIe1Minor1:
                    v = "PCIe 1.1";

                    break;
                case PCIeGeneration.PCIe2:
                    v = "PCIe 2.0";

                    break;
                case PCIeGeneration.PCIe3:
                    v = "PCIe 3.0";

                    break;
            }

            return $"{v} x{Lanes} - {TransferRateInMTps} MTps";
        }
    }
}