using System.Runtime.InteropServices;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Holds information regarding a fixed color
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct IlluminationZoneControlDataFixedColorParameters
    {
        internal byte _BrightnessInPercentage;

        /// <summary>
        ///     Creates a new instance of <see cref="IlluminationZoneControlDataFixedColorParameters" />.
        /// </summary>
        /// <param name="brightnessInPercentage">The brightness percentage value of the zone.</param>
        public IlluminationZoneControlDataFixedColorParameters(byte brightnessInPercentage)
        {
            _BrightnessInPercentage = brightnessInPercentage;
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