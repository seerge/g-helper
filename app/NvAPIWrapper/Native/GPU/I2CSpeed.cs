namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     Contains possible I2C bus speed values in kHz
    /// </summary>
    public enum I2CSpeed : uint
    {
        /// <summary>
        ///     Current / Default frequency setting
        /// </summary>
        Default,

        /// <summary>
        ///     3kHz
        /// </summary>
        I2C3KHz,

        /// <summary>
        ///     10kHz
        /// </summary>
        I2C10KHz,

        /// <summary>
        ///     33kHz
        /// </summary>
        I2C33KHz,

        /// <summary>
        ///     100kHz
        /// </summary>
        I2C100KHz,

        /// <summary>
        ///     200kHz
        /// </summary>
        I2C200KHz,

        /// <summary>
        ///     400kHz
        /// </summary>
        I2C400KHz
    }
}