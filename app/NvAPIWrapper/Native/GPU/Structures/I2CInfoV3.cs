using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Helpers.Structures;
using NvAPIWrapper.Native.Interfaces;
using NvAPIWrapper.Native.Interfaces.GPU;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <inheritdoc cref="II2CInfo" />
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(3)]
    public struct I2CInfoV3 : IInitializable, IDisposable, II2CInfo
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly StructureVersion _Version;
        private readonly OutputId _OutputMask;
        private readonly byte _UseDDCPort;
        private readonly byte _I2CDeviceAddress;
        private ValueTypeArray _I2CRegisterAddress;
        private readonly uint _I2CRegisterAddressLength;
        private ValueTypeArray _Data;
        private readonly uint _DataLength;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly uint _I2CSpeed;
        private readonly I2CSpeed _I2CSpeedInKHz;
        private readonly byte _PortId;
        private readonly uint _IsPortIdPresent;

        /// <inheritdoc />
        // ReSharper disable once ConvertToAutoProperty
        public OutputId OutputMask
        {
            get => _OutputMask;
        }

        /// <inheritdoc />
        public bool UseDDCPort
        {
            get => _UseDDCPort > 0;
        }

        /// <inheritdoc />
        // ReSharper disable once ConvertToAutoProperty
        public I2CSpeed Speed
        {
            get => _I2CSpeedInKHz;
        }

        /// <inheritdoc />
        public bool IsReadOperation
        {
            get => (_I2CDeviceAddress & 1) == 1;
        }

        /// <inheritdoc />
        public byte DeviceAddress
        {
            get => (byte) (_I2CDeviceAddress >> 1);
        }

        /// <inheritdoc />
        public byte? PortId
        {
            get
            {
                if (_IsPortIdPresent > 0)
                {
                    return _PortId;
                }

                return null;
            }
        }

        /// <inheritdoc />
        public byte[] Data
        {
            get
            {
                if (_Data.IsNull || _DataLength == 0)
                {
                    return new byte[0];
                }

                return _Data.ToArray<byte>((int) _DataLength);
            }
        }

        /// <inheritdoc />
        public byte[] RegisterAddress
        {
            get
            {
                if (_I2CRegisterAddress.IsNull || _I2CRegisterAddressLength == 0)
                {
                    return new byte[0];
                }

                return _I2CRegisterAddress.ToArray<byte>((int) _I2CRegisterAddressLength);
            }
        }

        /// <summary>
        ///     Creates an instance of <see cref="I2CInfoV3" /> for write operations.
        /// </summary>
        /// <param name="outputMask">The target display output mask</param>
        /// <param name="portId">The port id on which device is connected</param>
        /// <param name="useDDCPort">A boolean value indicating that the DDC port should be used instead of the communication port</param>
        /// <param name="deviceAddress">The device I2C slave address</param>
        /// <param name="registerAddress">The target I2C register address</param>
        /// <param name="data">The payload data</param>
        /// <param name="speed">The target speed of the transaction in kHz</param>
        public I2CInfoV3(
            OutputId outputMask,
            byte? portId,
            bool useDDCPort,
            byte deviceAddress,
            byte[] registerAddress,
            byte[] data,
            I2CSpeed speed = I2CSpeed.Default
        ) : this(outputMask, portId, useDDCPort, deviceAddress, false, registerAddress, data, speed)
        {
        }

        /// <summary>
        ///     Creates an instance of <see cref="I2CInfoV3" /> for read operations.
        /// </summary>
        /// <param name="outputMask">The target display output mask</param>
        /// <param name="portId">The port id on which device is connected</param>
        /// <param name="useDDCPort">A boolean value indicating that the DDC port should be used instead of the communication port</param>
        /// <param name="deviceAddress">The device I2C slave address</param>
        /// <param name="registerAddress">The target I2C register address</param>
        /// <param name="readDataLength">The length of the buffer to allocate for the read operation.</param>
        /// <param name="speed">The target speed of the transaction in kHz</param>
        public I2CInfoV3(
            OutputId outputMask,
            byte? portId,
            bool useDDCPort,
            byte deviceAddress,
            byte[] registerAddress,
            uint readDataLength,
            I2CSpeed speed = I2CSpeed.Default
        ) : this(outputMask, portId, useDDCPort, deviceAddress, true, registerAddress, new byte[readDataLength], speed)
        {
        }

        private I2CInfoV3(
            OutputId outputMask,
            byte? portId,
            bool useDDCPort,
            byte deviceAddress,
            bool isRead,
            byte[] registerAddress,
            byte[] data,
            I2CSpeed speed = I2CSpeed.Default
        )
        {
            this = typeof(I2CInfoV3).Instantiate<I2CInfoV3>();

            _UseDDCPort = useDDCPort ? (byte) 1 : (byte) 0;
            _OutputMask = outputMask;
            _I2CDeviceAddress = (byte) (deviceAddress << 1);
            _I2CSpeed = 0xFFFF; // Deprecated
            _I2CSpeedInKHz = speed;

            if (isRead)
            {
                _I2CDeviceAddress |= 1;
            }

            if (portId != null)
            {
                _PortId = portId.Value;
                _IsPortIdPresent = 1;
            }
            else
            {
                _IsPortIdPresent = 0;
            }

            if (registerAddress?.Length > 0)
            {
                _I2CRegisterAddress = ValueTypeArray.FromArray(registerAddress);
                _I2CRegisterAddressLength = (uint) registerAddress.Length;
            }
            else
            {
                _I2CRegisterAddress = ValueTypeArray.Null;
                _I2CRegisterAddressLength = 0;
            }

            if (data?.Length > 0)
            {
                _Data = ValueTypeArray.FromArray(data);
                _DataLength = (uint) data.Length;
            }
            else
            {
                _Data = ValueTypeArray.Null;
                _DataLength = 0;
            }
        }

        /// <summary>
        ///     Calculates and fills the last byte of data to the checksum value required by the DDCCI protocol
        /// </summary>
        /// <param name="deviceAddress">The target device address.</param>
        /// <param name="registerAddress">The target register address.</param>
        /// <param name="data">The data to be sent and store the checksum.</param>
        public static void FillDDCCIChecksum(byte deviceAddress, byte[] registerAddress, byte[] data)
        {
            I2CInfoV2.FillDDCCIChecksum(deviceAddress, registerAddress, data);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!_I2CRegisterAddress.IsNull)
            {
                _I2CRegisterAddress.Dispose();
            }

            if (!_Data.IsNull)
            {
                _Data.Dispose();
            }
        }
    }
}