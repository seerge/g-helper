using System.Collections.Generic;
using System.Linq;
using NvAPIWrapper.Native;

namespace NvAPIWrapper.GPU
{
    /// <summary>
    ///     Holds information regarding the available thermal sensors and current thermal level of a GPU
    /// </summary>
    public class GPUThermalInformation
    {
        internal GPUThermalInformation(PhysicalGPU physicalGPU)
        {
            PhysicalGPU = physicalGPU;
        }

        /// <summary>
        ///     Gets the current thermal level of the GPU
        /// </summary>
        public int CurrentThermalLevel
        {
            get => (int) GPUApi.GetCurrentThermalLevel(PhysicalGPU.Handle);
        }

        /// <summary>
        ///     Gets the physical GPU that this instance describes
        /// </summary>
        public PhysicalGPU PhysicalGPU { get; }

        /// <summary>
        ///     Gets the list of available thermal sensors
        /// </summary>
        public IEnumerable<GPUThermalSensor> ThermalSensors
        {
            get
            {
                return GPUApi.GetThermalSettings(PhysicalGPU.Handle).Sensors
                    .Select((sensor, i) => new GPUThermalSensor(i, sensor));
            }
        }
    }
}