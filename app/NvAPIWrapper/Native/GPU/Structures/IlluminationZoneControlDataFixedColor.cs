using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Holds information regarding a fixed color control data
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct IlluminationZoneControlDataFixedColor : IInitializable
    {
        private const int MaximumNumberOfDataBytes = 64;
        private const int MaximumNumberOfReservedBytes = 64;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaximumNumberOfDataBytes)]
        internal byte[] _Data;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaximumNumberOfReservedBytes)]
        internal byte[] _Reserved;

        /// <summary>
        ///     Creates a new instance of <see cref="IlluminationZoneControlDataFixedColor" />.
        /// </summary>
        /// <param name="manualFixedColor">The zone manual control data.</param>
        public IlluminationZoneControlDataFixedColor(IlluminationZoneControlDataManualFixedColor manualFixedColor)
            : this(manualFixedColor.ToByteArray())
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="IlluminationZoneControlDataFixedColor" />.
        /// </summary>
        /// <param name="piecewiseLinearFixedColor">The zone piecewise linear control data.</param>
        public IlluminationZoneControlDataFixedColor(
            IlluminationZoneControlDataPiecewiseLinearFixedColor piecewiseLinearFixedColor)
            : this(piecewiseLinearFixedColor.ToByteArray())
        {
        }

        private IlluminationZoneControlDataFixedColor(byte[] data)
        {
            if (!(data?.Length > 0) || data.Length > MaximumNumberOfDataBytes)
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }

            this = typeof(IlluminationZoneControlDataFixedColor).Instantiate<IlluminationZoneControlDataFixedColor>();
            Array.Copy(data, 0, _Data, 0, data.Length);
        }

        /// <summary>
        ///     Gets the control data as a manual control structure.
        /// </summary>
        /// <returns>An instance of <see cref="IlluminationZoneControlDataManualFixedColor" /> containing manual settings.</returns>
        public IlluminationZoneControlDataManualFixedColor AsManual()
        {
            return _Data.ToStructure<IlluminationZoneControlDataManualFixedColor>();
        }

        /// <summary>
        ///     Gets the control data as a piecewise linear control structure.
        /// </summary>
        /// <returns>
        ///     An instance of <see cref="IlluminationZoneControlDataPiecewiseLinearFixedColor" /> containing piecewise
        ///     settings.
        /// </returns>
        public IlluminationZoneControlDataPiecewiseLinearFixedColor AsPiecewise()
        {
            return _Data.ToStructure<IlluminationZoneControlDataPiecewiseLinearFixedColor>();
        }
    }
}