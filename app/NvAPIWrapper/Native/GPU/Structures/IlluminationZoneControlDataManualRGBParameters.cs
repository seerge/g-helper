using System.Runtime.InteropServices;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Holds information regarding a RGB color
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct IlluminationZoneControlDataManualRGBParameters
    {
        internal byte _Red;
        internal byte _Green;
        internal byte _Blue;
        internal byte _BrightnessInPercentage;

        /// <summary>
        ///     Creates a new instance of <see cref="IlluminationZoneControlDataManualRGBParameters" />.
        /// </summary>
        /// <param name="red">The red component of color applied to the zone.</param>
        /// <param name="green">The green component of color applied to the zone.</param>
        /// <param name="blue">The blue component of color applied to the zone.</param>
        /// <param name="brightnessInPercentage">The brightness percentage value of the zone.</param>
        // ReSharper disable once TooManyDependencies
        public IlluminationZoneControlDataManualRGBParameters(
            byte red,
            byte green,
            byte blue,
            byte brightnessInPercentage)
        {
            _Red = red;
            _Green = green;
            _Blue = blue;
            _BrightnessInPercentage = brightnessInPercentage;
        }

        /// <summary>
        ///     Gets the red component of color applied to the zone.
        /// </summary>
        public byte Red
        {
            get => _Red;
        }

        /// <summary>
        ///     Gets the green component of color applied to the zone.
        /// </summary>
        public byte Green
        {
            get => _Green;
        }

        /// <summary>
        ///     Gets the blue component of color applied to the zone.
        /// </summary>
        public byte Blue
        {
            get => _Blue;
        }

        /// <summary>
        ///     Gets the brightness percentage value of the zone.
        /// </summary>
        public byte BrightnessInPercentage
        {
            get => _BrightnessInPercentage;
        }
    }
}