using System;

namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     Holds a list of possible cooler targets
    /// </summary>
    [Flags]
    public enum CoolerTarget : uint
    {
        /// <summary>
        ///     No cooler target
        /// </summary>
        None = 0,

        /// <summary>
        ///     Cooler targets GPU
        /// </summary>
        GPU = 0b1,

        /// <summary>
        ///     Cooler targets memory
        /// </summary>
        Memory = 0b10,

        /// <summary>
        ///     Cooler targets power supply
        /// </summary>
        PowerSupply = 0b100,

        /// <summary>
        ///     Cooler targets GPU, memory and power supply
        /// </summary>
        All = GPU | Memory | PowerSupply
    }
}