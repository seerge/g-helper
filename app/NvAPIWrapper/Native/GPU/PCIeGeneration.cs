namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     Holds a list of known PCI-e generations and versions
    /// </summary>
    public enum PCIeGeneration : uint
    {
        /// <summary>
        ///     PCI-e 1.0
        /// </summary>
        PCIe1 = 0,

        /// <summary>
        ///     PCI-e 1.1
        /// </summary>
        PCIe1Minor1,

        /// <summary>
        ///     PCI-e 2.0
        /// </summary>
        PCIe2,

        /// <summary>
        ///     PCI-e 3.0
        /// </summary>
        PCIe3
    }
}