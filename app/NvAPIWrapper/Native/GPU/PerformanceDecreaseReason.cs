namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     Holds a list possible reasons for performance decrease
    /// </summary>
    public enum PerformanceDecreaseReason : uint
    {
        /// <summary>
        ///     No performance decrease
        /// </summary>
        None = 0,

        /// <summary>
        ///     Thermal protection performance decrease
        /// </summary>
        ThermalProtection = 0x00000001,

        /// <summary>
        ///     Power control performance decrease
        /// </summary>
        PowerControl = 0x00000002,

        /// <summary>
        ///     AC-BATT event performance decrease
        /// </summary>
        // ReSharper disable once InconsistentNaming
        AC_BATT = 0x00000004,

        /// <summary>
        ///     API triggered performance decrease
        /// </summary>
        ApiTriggered = 0x00000008,

        /// <summary>
        ///     Insufficient performance decrease (Power Connector Missing)
        /// </summary>
        InsufficientPower = 0x00000010,

        /// <summary>
        ///     Unknown
        /// </summary>
        Unknown = 0x80000000
    }
}