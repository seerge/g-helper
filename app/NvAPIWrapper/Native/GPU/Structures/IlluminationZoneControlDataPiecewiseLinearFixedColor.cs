using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Holds information regarding a piecewise linear fixed color control method
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct IlluminationZoneControlDataPiecewiseLinearFixedColor : IInitializable
    {
        private const int NumberColorEndPoints = 2;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NumberColorEndPoints)]
        internal IlluminationZoneControlDataFixedColorParameters[] _EndPoints;

        internal IlluminationZoneControlDataPiecewiseLinear _PiecewiseLinearData;

        /// <summary>
        ///     Creates a new instance of <see cref="IlluminationZoneControlDataPiecewiseLinearFixedColor" />.
        /// </summary>
        /// <param name="endPoints">The list of fixed color piecewise function endpoints.</param>
        /// <param name="piecewiseLinearData">The piecewise function settings.</param>
        public IlluminationZoneControlDataPiecewiseLinearFixedColor(
            IlluminationZoneControlDataFixedColorParameters[] endPoints,
            IlluminationZoneControlDataPiecewiseLinear piecewiseLinearData)
        {
            if (endPoints?.Length != NumberColorEndPoints)
            {
                throw new ArgumentOutOfRangeException(nameof(endPoints));
            }

            this = typeof(IlluminationZoneControlDataPiecewiseLinearFixedColor)
                .Instantiate<IlluminationZoneControlDataPiecewiseLinearFixedColor>();
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
        ///     Gets the list of fixed color piecewise function endpoints
        /// </summary>
        public IlluminationZoneControlDataFixedColorParameters[] EndPoints
        {
            get => _EndPoints;
        }
    }
}