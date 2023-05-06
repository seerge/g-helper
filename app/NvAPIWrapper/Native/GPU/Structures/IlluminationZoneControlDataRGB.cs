using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Holds information regarding a RGB control data
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct IlluminationZoneControlDataRGB : IInitializable
    {
        private const int MaximumNumberOfDataBytes = 64;
        private const int MaximumNumberOfReservedBytes = 64;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaximumNumberOfDataBytes)]
        internal byte[] _Data;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaximumNumberOfReservedBytes)]
        internal byte[] _Reserved;

        /// <summary>
        ///     Creates a new instance of <see cref="IlluminationZoneControlDataRGB" />.
        /// </summary>
        /// <param name="manualRGB">The zone manual control data.</param>
        public IlluminationZoneControlDataRGB(IlluminationZoneControlDataManualRGB manualRGB)
            : this(manualRGB.ToByteArray())
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="IlluminationZoneControlDataRGB" />.
        /// </summary>
        /// <param name="piecewiseLinearRGB">The zone piecewise linear control data.</param>
        public IlluminationZoneControlDataRGB(IlluminationZoneControlDataPiecewiseLinearRGB piecewiseLinearRGB)
            : this(piecewiseLinearRGB.ToByteArray())
        {
        }

        private IlluminationZoneControlDataRGB(byte[] data)
        {
            if (!(data?.Length > 0) || data.Length > MaximumNumberOfDataBytes)
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }

            this = typeof(IlluminationZoneControlDataRGB).Instantiate<IlluminationZoneControlDataRGB>();
            Array.Copy(data, 0, _Data, 0, data.Length);
        }

        /// <summary>
        ///     Gets the control data as a manual control structure.
        /// </summary>
        /// <returns>An instance of <see cref="IlluminationZoneControlDataManualRGB" /> containing manual settings.</returns>
        public IlluminationZoneControlDataManualRGB AsManual()
        {
            return _Data.ToStructure<IlluminationZoneControlDataManualRGB>();
        }

        /// <summary>
        ///     Gets the control data as a piecewise linear control structure.
        /// </summary>
        /// <returns>
        ///     An instance of <see cref="IlluminationZoneControlDataPiecewiseLinearRGB" /> containing piecewise linear
        ///     settings.
        /// </returns>
        public IlluminationZoneControlDataPiecewiseLinearRGB AsPiecewise()
        {
            return _Data.ToStructure<IlluminationZoneControlDataPiecewiseLinearRGB>();
        }
    }
}