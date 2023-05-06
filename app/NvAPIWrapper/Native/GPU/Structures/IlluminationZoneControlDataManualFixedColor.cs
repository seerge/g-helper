using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Holds information regarding a manual fixed color control method
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct IlluminationZoneControlDataManualFixedColor : IInitializable
    {
        internal IlluminationZoneControlDataFixedColorParameters _Parameters;

        /// <summary>
        ///     Creates a new instance of <see cref="IlluminationZoneControlDataManualFixedColor" />.
        /// </summary>
        /// <param name="parameters">The fixed color parameters.</param>
        public IlluminationZoneControlDataManualFixedColor(IlluminationZoneControlDataFixedColorParameters parameters)
        {
            _Parameters = parameters;
        }

        /// <summary>
        ///     Gets the fixed color parameters
        /// </summary>
        internal IlluminationZoneControlDataFixedColorParameters Parameters
        {
            get => _Parameters;
        }
    }
}