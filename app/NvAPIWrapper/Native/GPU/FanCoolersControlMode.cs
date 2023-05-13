using System;

namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     Holds possible fan cooler control modes
    /// </summary>
    [Flags]
    public enum FanCoolersControlMode : uint
    {
        /// <summary>
        ///     Automatic fan cooler control
        /// </summary>
        Auto = 0,

        /// <summary>
        ///     Manual fan cooler control
        /// </summary>
        Manual = 0b1,
    }
}