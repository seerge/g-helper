using NvAPIWrapper.Native.Display.Structures;

namespace NvAPIWrapper.Native.Interfaces.Display
{
    /// <summary>
    ///     Holds information regarding a display color space configurations
    /// </summary>
    public interface IDisplayColorData
    {
        /// <summary>
        ///     Gets the first primary color space coordinate (e.g. Red for RGB) [(0.0, 0.0)-(1.0, 1.0)]
        /// </summary>
        ColorDataColorCoordinate FirstColorCoordinate { get; }

        /// <summary>
        ///     Gets the second primary color space coordinate (e.g. Green for RGB) [(0.0, 0.0)-(1.0, 1.0)]
        /// </summary>
        ColorDataColorCoordinate SecondColorCoordinate { get; }

        /// <summary>
        ///     Gets the third primary color space coordinate (e.g. Blue for RGB) [(0.0, 0.0)-(1.0, 1.0)]
        /// </summary>
        ColorDataColorCoordinate ThirdColorCoordinate { get; }

        /// <summary>
        ///     Gets the white color space coordinate [(0.0, 0.0)-(1.0, 1.0)]
        /// </summary>
        ColorDataColorCoordinate WhiteColorCoordinate { get; }
    }
}