using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Holds information regarding a piecewise linear RGB control method
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct IlluminationZoneControlDataPiecewiseLinearRGB : IInitializable
    {
        private const int NumberColorEndPoints = 2;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NumberColorEndPoints)]
        internal IlluminationZoneControlDataManualRGBParameters[] _EndPoints;

        internal IlluminationZoneControlDataPiecewiseLinear _PiecewiseLinearData;

        /// <summary>
        ///     Creates a new instance of <see cref="IlluminationZoneControlDataPiecewiseLinearRGB" />.
        /// </summary>
        /// <param name="endPoints">The list of RGB piecewise function endpoints.</param>
        /// <param name="piecewiseLinearData">The piecewise function settings.</param>
        public IlluminationZoneControlDataPiecewiseLinearRGB(
            IlluminationZoneControlDataManualRGBParameters[] endPoints,
            IlluminationZoneControlDataPiecewiseLinear piecewiseLinearData)
        {
            if (endPoints?.Length != NumberColorEndPoints)
            {
                throw new ArgumentOutOfRangeException(nameof(endPoints));
            }

            this = typeof(IlluminationZoneControlDataPiecewiseLinearRGB)
                .Instantiate<IlluminationZoneControlDataPiecewiseLinearRGB>();
            _PiecewiseLinearData = piecewiseLinearData;
            Array.Copy(endPoints, 0, _EndPoints, 0, endPoints.Length);
        }

        /// <summary>
        ///     Gets the piecewise function settings
        /// </summary>
        public IlluminationZoneControlDataPiecewiseLinear PiecewiseLinearData
        {
            get => _PiecewiseLinearData;
        }

        /// <summary>
        ///     Gets the list of RGB function endpoints
        /// </summary>
        public IlluminationZoneControlDataManualRGBParameters[] EndPoints
        {
            get => _EndPoints;
        }
    }
}