using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Holds information regarding a illumination device
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct IlluminationDeviceInfoV1 : IInitializable
    {
        private const int MaximumNumberOfReserved = 64;
        private const int MaximumNumberOfDeviceData = 64;
        internal IlluminationDeviceType _DeviceType;
        internal IlluminationZoneControlMode _ControlModes;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaximumNumberOfDeviceData)]
        internal byte[] _DeviceData;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaximumNumberOfReserved)]
        internal byte[] _Reserved;

        /// <summary>
        ///     Gets the illumination device type
        /// </summary>
        public IlluminationDeviceType DeviceType
        {
            get => _DeviceType;
        }

        /// <summary>
        ///     Gets the illumination device control mode
        /// </summary>
        public IlluminationZoneControlMode ControlMode
        {
            get => _ControlModes;
        }

        /// <summary>
        ///     Gets the I2C index for a MCUV10 device
        /// </summary>
        /// <exception cref="InvalidOperationException" accessor="get">Device type is not MCUV10.</exception>
        public byte MCUV10DeviceI2CIndex
        {
            get
            {
                if (DeviceType != IlluminationDeviceType.MCUV10)
                {
                    throw new InvalidOperationException("Device type is not MCUV10.");
                }

                return _DeviceData[0];
            }
        }
    }
}