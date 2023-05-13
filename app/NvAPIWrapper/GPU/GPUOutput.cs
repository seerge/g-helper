using System;
using NvAPIWrapper.Display;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.Exceptions;
using NvAPIWrapper.Native.General;
using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.GPU.Structures;

namespace NvAPIWrapper.GPU
{
    /// <summary>
    ///     Represents a single GPU output
    /// </summary>
    public class GPUOutput : IEquatable<GPUOutput>
    {
        internal GPUOutput(OutputId outputId, PhysicalGPUHandle gpuHandle)
        {
            OutputId = outputId;
            OutputType = !gpuHandle.IsNull ? GPUApi.GetOutputType(gpuHandle, outputId) : OutputType.Unknown;
            PhysicalGPU = new PhysicalGPU(gpuHandle);
        }

        internal GPUOutput(OutputId outputId, PhysicalGPU gpu)
            : this(outputId, gpu?.Handle ?? PhysicalGPUHandle.DefaultHandle)
        {
            PhysicalGPU = gpu;
        }

        /// <summary>
        ///     Gets the corresponding Digital Vibrance Control information
        /// </summary>
        public DVCInformation DigitalVibranceControl
        {
            get => new DVCInformation(OutputId);
        }

        /// <summary>
        ///     Gets the corresponding HUE information
        /// </summary>
        public HUEInformation HUEControl
        {
            get => new HUEInformation(OutputId);
        }

        /// <summary>
        ///     Gets the output identification as a single bit unsigned integer
        /// </summary>
        public OutputId OutputId { get; }

        /// <summary>
        ///     Gets the output type
        /// </summary>
        public OutputType OutputType { get; }

        /// <summary>
        ///     Gets the corresponding physical GPU
        /// </summary>
        public PhysicalGPU PhysicalGPU { get; }

        /// <inheritdoc />
        public bool Equals(GPUOutput other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return PhysicalGPU.Equals(other.PhysicalGPU) && OutputId == other.OutputId;
        }

        /// <summary>
        ///     Checks for equality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are equal, otherwise false</returns>
        public static bool operator ==(GPUOutput left, GPUOutput right)
        {
            return right?.Equals(left) ?? ReferenceEquals(left, null);
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(GPUOutput left, GPUOutput right)
        {
            return !(left == right);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((GPUOutput) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return ((PhysicalGPU != null ? PhysicalGPU.GetHashCode() : 0) * 397) ^ (int) OutputId;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{OutputId} {OutputType} @ {PhysicalGPU}";
        }

        /// <summary>
        ///     Overrides the refresh rate on this output.
        ///     The new refresh rate can be applied right away or deferred to be applied with the next OS
        ///     mode-set.
        ///     The override is good for only one mode-set (regardless whether it's deferred or immediate).
        /// </summary>
        /// <param name="refreshRate">The refresh rate to be applied.</param>
        /// <param name="isDeferred">
        ///     A boolean value indicating if the refresh rate override should be deferred to the next OS
        ///     mode-set.
        /// </param>
        public void OverrideRefreshRate(float refreshRate, bool isDeferred = false)
        {
            DisplayApi.SetRefreshRateOverride(OutputId, refreshRate, isDeferred);
        }

        /// <summary>
        ///     Reads data from the I2C bus
        /// </summary>
        /// <param name="portId">The port id on which device is connected</param>
        /// <param name="useDDCPort">A boolean value indicating that the DDC port should be used instead of the communication port</param>
        /// <param name="deviceAddress">The device I2C slave address</param>
        /// <param name="registerAddress">The target I2C register address</param>
        /// <param name="readDataLength">The length of the buffer to allocate for the read operation.</param>
        /// <param name="speed">The target speed of the transaction in kHz</param>
        public byte[] ReadI2C(
            byte? portId,
            bool useDDCPort,
            byte deviceAddress,
            byte[] registerAddress,
            uint readDataLength,
            I2CSpeed speed = I2CSpeed.Default
        )
        {
            try
            {
                // ReSharper disable once InconsistentNaming
                var i2cInfoV3 = new I2CInfoV3(
                    OutputId,
                    portId,
                    useDDCPort,
                    deviceAddress,
                    registerAddress,
                    readDataLength,
                    speed
                );

                return PhysicalGPU.ReadI2C(i2cInfoV3);
            }
            catch (NVIDIAApiException e)
            {
                if (e.Status != Status.IncompatibleStructureVersion || portId != null)
                {
                    throw;
                }

                // ignore
            }

            // ReSharper disable once InconsistentNaming
            var i2cInfoV2 = new I2CInfoV2(
                OutputId,
                useDDCPort,
                deviceAddress,
                registerAddress,
                readDataLength,
                speed
            );

            return PhysicalGPU.ReadI2C(i2cInfoV2);
        }

        /// <summary>
        ///     Writes data to the I2C bus
        /// </summary>
        /// <param name="portId">The port id on which device is connected</param>
        /// <param name="useDDCPort">A boolean value indicating that the DDC port should be used instead of the communication port</param>
        /// <param name="deviceAddress">The device I2C slave address</param>
        /// <param name="registerAddress">The target I2C register address</param>
        /// <param name="data">The payload data</param>
        /// <param name="speed">The target speed of the transaction in kHz</param>
        public void WriteI2C(
            byte? portId,
            bool useDDCPort,
            byte deviceAddress,
            byte[] registerAddress,
            byte[] data,
            I2CSpeed speed = I2CSpeed.Default
        )
        {
            try
            {
                // ReSharper disable once InconsistentNaming
                var i2cInfoV3 = new I2CInfoV3(
                    OutputId,
                    portId,
                    useDDCPort,
                    deviceAddress,
                    registerAddress,
                    data,
                    speed
                );

                PhysicalGPU.WriteI2C(i2cInfoV3);

                return;
            }
            catch (NVIDIAApiException e)
            {
                if (e.Status != Status.IncompatibleStructureVersion || portId != null)
                {
                    throw;
                }

                // ignore
            }

            // ReSharper disable once InconsistentNaming
            var i2cInfoV2 = new I2CInfoV2(
                OutputId,
                useDDCPort,
                deviceAddress,
                registerAddress,
                data,
                speed
            );

            PhysicalGPU.WriteI2C(i2cInfoV2);
        }
    }
}