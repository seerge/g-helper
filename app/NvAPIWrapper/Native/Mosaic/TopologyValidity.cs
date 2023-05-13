using System;

namespace NvAPIWrapper.Native.Mosaic
{
    /// <summary>
    ///     These bits are used to describe the validity of a topo
    /// </summary>
    [Flags]
    public enum TopologyValidity : uint
    {
        /// <summary>
        ///     The topology is valid
        /// </summary>
        Valid = 0,

        /// <summary>
        ///     Not enough SLI GPUs were found to fill the entire topology. PhysicalGPUHandle will be null for these.
        /// </summary>
        MissingGPU = 1,

        /// <summary>
        ///     Not enough displays were found to fill the entire topology. Output identification will be 0 for these.
        /// </summary>
        MissingDisplay = 2,

        /// <summary>
        ///     The topology is only possible with displays of the same output type. Check output identifications to make sure they
        ///     are all CRTs, or all DFPs.
        /// </summary>
        MixedDisplayTypes = 4
    }
}