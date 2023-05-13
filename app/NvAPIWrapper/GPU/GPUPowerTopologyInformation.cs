using System.Collections.Generic;
using System.Linq;
using NvAPIWrapper.Native;

namespace NvAPIWrapper.GPU
{
    /// <summary>
    ///     Holds information regarding current power topology and their current power usage
    /// </summary>
    public class GPUPowerTopologyInformation
    {
        internal GPUPowerTopologyInformation(PhysicalGPU physicalGPU)
        {
            PhysicalGPU = physicalGPU;
        }

        /// <summary>
        ///     Gets the physical GPU that this instance describes
        /// </summary>
        public PhysicalGPU PhysicalGPU { get; }

        /// <summary>
        ///     Gets the current power topology entries
        /// </summary>
        public IEnumerable<GPUPowerTopologyStatus> PowerTopologyEntries
        {
            get
            {
                return GPUApi.ClientPowerTopologyGetStatus(PhysicalGPU.Handle).PowerPolicyStatusEntries
                    .Select(entry => new GPUPowerTopologyStatus(entry));
            }
        }
    }
}