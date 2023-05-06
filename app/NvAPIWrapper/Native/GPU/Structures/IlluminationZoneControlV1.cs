using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Holds information regarding a zone control status
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct IlluminationZoneControlV1 : IInitializable
    {
        private const int MaximumNumberOfDataBytes = 128;
        private const int MaximumNumberOfReservedBytes = 64;

        internal IlluminationZoneType _ZoneType;

        internal IlluminationZoneControlMode _ControlMode;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaximumNumberOfDataBytes)]
        internal byte[] _Data;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaximumNumberOfReservedBytes)]
        internal byte[] _Reserved;

        /// <summary>
        ///     Creates a new instance of <see cref="IlluminationZoneControlV1" />.
        /// </summary>
        /// <param name="controlMode">The zone control mode.</param>
        /// <param name="rgbData">The zone control RGB data.</param>
        public IlluminationZoneControlV1(
            IlluminationZoneControlMode controlMode,
            IlluminationZoneControlDataRGB rgbData)
            : this(controlMode, IlluminationZoneType.RGB, rgbData.ToByteArray())
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="IlluminationZoneControlV1" />.
        /// </summary>
        /// <param name="controlMode">The zone control mode.</param>
        /// <param name="fixedColorData">The zone control fixed color data.</param>
        public IlluminationZoneControlV1(
            IlluminationZoneControlMode controlMode,
            IlluminationZoneControlDataFixedColor fixedColorData)
            : this(controlMode, IlluminationZoneType.FixedColor, fixedColorData.ToByteArray())
        {
        }

        private IlluminationZoneControlV1(
            IlluminationZoneControlMode controlMode,
            IlluminationZoneType zoneType,
            byte[] data)
        {
            if (!(data?.Length > 0) || data.Length > MaximumNumberOfDataBytes)
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }

            this = typeof(IlluminationZoneControlV1).Instantiate<IlluminationZoneControlV1>();
            _ControlMode = controlMode;
            _ZoneType = zoneType;
            Array.Copy(data, 0, _Data, 0, data.Length);
        }

        /// <summary>
        ///     Gets the type of zone and the type of data needed to control this zone
        /// </summary>
        internal IlluminationZoneType ZoneType
        {
            get => _ZoneType;
        }

        /// <summary>
        ///     Gets the zone control mode
        /// </summary>
        internal IlluminationZoneControlMode ControlMode
        {
            get => _ControlMode;
        }

        /// <summary>
        ///     Gets the control data as a RGB data structure.
        /// </summary>
        /// <returns>An instance of <see cref="IlluminationZoneControlDataRGB" /> containing RGB settings.</returns>
        public IlluminationZoneControlDataRGB AsRGBData()
        {
            return _Data.ToStructure<IlluminationZoneControlDataRGB>();
        }

        /// <summary>
        ///     Gets the control data as a fixed color data structure.
        /// </summary>
        /// <returns>An instance of <see cref="IlluminationZoneControlDataFixedColor" /> containing fixed color settings.</returns>
        public IlluminationZoneControlDataFixedColor AsFixedColorData()
        {
            return _Data.ToStructure<IlluminationZoneControlDataFixedColor>();
        }
    }
}