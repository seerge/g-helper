using NvAPIWrapper.Native.GPU;

namespace NvAPIWrapper.Native.Interfaces.GPU
{
    /// <summary>
    ///     Contains an I2C packet transmitted or to be transmitted
    /// </summary>
    public interface II2CInfo
    {
        /// <summary>
        ///     Gets the payload data
        /// </summary>
        byte[] Data { get; }

        /// <summary>
        ///     Gets the device I2C slave address
        /// </summary>
        byte DeviceAddress { get; }

        /// <summary>
        ///     Gets a boolean value indicating that this instance contents information about a read operation
        /// </summary>
        bool IsReadOperation { get; }

        /// <summary>
        ///     Gets the target display output mask
        /// </summary>
        OutputId OutputMask { get; }

        /// <summary>
        ///     Gets the port id on which device is connected
        /// </summary>
        byte? PortId { get; }

        /// <summary>
        ///     Gets the target I2C register address
        /// </summary>
        byte[] RegisterAddress { get; }

        /// <summary>
        ///     Gets the target speed of the transaction in kHz
        /// </summary>
        I2CSpeed Speed { get; }

        /// <summary>
        ///     Gets a boolean value indicating that the DDC port should be used instead of the communication port
        /// </summary>
        bool UseDDCPort { get; }
    }
}