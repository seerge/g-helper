using System;

namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     Holds possible cooler policies
    /// </summary>
    [Flags]
    public enum CoolerPolicy : uint
    {
        /// <summary>
        ///     No cooler policy
        /// </summary>
        None = 0,

        /// <summary>
        ///     Manual cooler control
        /// </summary>
        Manual = 0b1,

        /// <summary>
        ///     Performance optimized cooler policy
        /// </summary>
        Performance = 0b10,

        /// <summary>
        ///     Discrete temperature based cooler policy
        /// </summary>
        TemperatureDiscrete = 0b100,

        /// <summary>
        ///     Continues temperature based cooler policy
        /// </summary>
        TemperatureContinuous = 0b1000,

        /// <summary>
        ///     Silent cooler policy
        /// </summary>
        Silent = 0b10000
    }
}