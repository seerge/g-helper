using NvAPIWrapper.Native;
using NvAPIWrapper.Native.GPU;

namespace NvAPIWrapper.GPU
{
    /// <summary>
    ///     Contains information about the GPU bus
    /// </summary>
    public class GPUBusInformation
    {
        internal GPUBusInformation(PhysicalGPU physicalGPU)
        {
            PhysicalGPU = physicalGPU;
        }

        /// <summary>
        ///     Gets accelerated graphics port information
        /// </summary>
        public AGPInformation AGPInformation
        {
            get
            {
                if (BusType != GPUBusType.AGP)
                {
                    return null;
                }

                return new AGPInformation(
                    GPUApi.GetAGPAperture(PhysicalGPU.Handle),
                    GPUApi.GetCurrentAGPRate(PhysicalGPU.Handle)
                );
            }
        }

        /// <summary>
        ///     Gets the bus identification
        /// </summary>
        public int BusId
        {
            get => GPUApi.GetBusId(PhysicalGPU.Handle);
        }

        /// <summary>
        ///     Gets the bus slot identification
        /// </summary>
        public int BusSlot
        {
            get => GPUApi.GetBusSlotId(PhysicalGPU.Handle);
        }

        /// <summary>
        ///     Gets the the bus type
        /// </summary>
        public GPUBusType BusType
        {
            get => GPUApi.GetBusType(PhysicalGPU.Handle);
        }

        /// <summary>
        ///     Gets number of PCIe lanes being used for the PCIe interface downstream
        /// </summary>
        public int CurrentPCIeLanes
        {
            get
            {
                if (BusType == GPUBusType.PCIExpress)
                {
                    return GPUApi.GetCurrentPCIEDownStreamWidth(PhysicalGPU.Handle);
                }

                return 0;
            }
        }

        /// <summary>
        ///     Gets GPU interrupt number
        /// </summary>
        public int IRQ
        {
            get => GPUApi.GetIRQ(PhysicalGPU.Handle);
        }

        /// <summary>
        ///     Gets the PCI identifiers
        /// </summary>
        public PCIIdentifiers PCIIdentifiers
        {
            get
            {
                if (BusType == GPUBusType.FPCI || BusType == GPUBusType.PCI || BusType == GPUBusType.PCIExpress)
                {
                    GPUApi.GetPCIIdentifiers(
                        PhysicalGPU.Handle,
                        out var deviceId,
                        out var subSystemId,
                        out var revisionId,
                        out var extDeviceId
                    );

                    return new PCIIdentifiers(deviceId, subSystemId, revisionId, (int) extDeviceId);
                }

                return null;
            }
        }

        /// <summary>
        ///     Gets the physical GPU that this instance describes
        /// </summary>
        public PhysicalGPU PhysicalGPU { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"[{BusType}] Bus #{BusId}, Slot #{BusSlot}";
        }
    }
}