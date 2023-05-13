using System;

namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     Holds a list of known performance limitations
    /// </summary>
    [Flags]
    public enum PerformanceLimit : uint
    {
        /// <summary>
        ///     No performance limitation
        /// </summary>
        None = 0,

        /// <summary>
        ///     Limited by power usage
        /// </summary>
        PowerLimit = 0b1,

        /// <summary>
        ///     Limited by temperature
        /// </summary>
        TemperatureLimit = 0b10,

        /// <summary>
        ///     Limited by voltage
        /// </summary>
        VoltageLimit = 0b100,

        /// <summary>
        ///     Unknown limitation
        /// </summary>
        Unknown8 = 0b1000,

        /// <summary>
        ///     Limited due to no load
        /// </summary>
        NoLoadLimit = 0b10000
    }
}