using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Holds information regarding a RGB control method
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct IlluminationZoneControlDataManualRGB : IInitializable
    {
        internal IlluminationZoneControlDataManualRGBParameters _Parameters;

        /// <summary>
        ///     Creates a new instance of <see cref="IlluminationZoneControlDataManualRGB" />.
        /// </summary>
        /// <param name="parameters">The RGB parameters.</param>
        public IlluminationZoneControlDataManualRGB(IlluminationZoneControlDataManualRGBParameters parameters)
        {
            _Parameters = parameters;
        }

        /// <summary>
        ///     Gets the RGB parameters
        /// </summary>
        public IlluminationZoneControlDataManualRGBParameters Parameters
        {
            get => _Parameters;
        }
    }
}