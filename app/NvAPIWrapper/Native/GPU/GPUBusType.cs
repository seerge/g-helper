namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     Associated GPU bus types
    /// </summary>
    public enum GPUBusType
    {
        /// <summary>
        ///     Bus type is undefined
        /// </summary>
        Undefined = 0,

        /// <summary>
        ///     PCI Bus
        /// </summary>
        PCI = 1,

        /// <summary>
        ///     AGP Bus
        /// </summary>
        AGP = 2,

        /// <summary>
        ///     PCIExpress Bus
        /// </summary>
        PCIExpress = 3,

        /// <summary>
        ///     FPCI Bus
        /// </summary>
        FPCI = 4,

        /// <summary>
        ///     AXI Bus
        /// </summary>
        AXI = 5
    }
}