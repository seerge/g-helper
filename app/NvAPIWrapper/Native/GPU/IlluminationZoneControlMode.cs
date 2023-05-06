using System;

namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     Contains a list of available illumination zone control modes
    /// </summary>
    [Flags]
    public enum IlluminationZoneControlMode : uint
    {
        /// <summary>
        ///     manual RGB control
        /// </summary>
        ManualRGB = 0,

        /// <summary>
        ///     Piecewise linear RGB control
        /// </summary>
        PiecewiseLinearRGB,

        /// <summary>
        ///     Invalid control mode
        /// </summary>
        Invalid = 0xFF
    }
}