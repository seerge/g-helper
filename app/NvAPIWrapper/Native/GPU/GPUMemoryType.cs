namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     Holds a list of known memory types
    /// </summary>
    public enum GPUMemoryType : uint
    {
        /// <summary>
        ///     Unknown memory type
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///     Synchronous dynamic random-access memory
        /// </summary>
        SDRAM,

        /// <summary>
        ///     Double Data Rate Synchronous Dynamic Random-Access Memory
        /// </summary>
        DDR1,

        /// <summary>
        ///     Double Data Rate 2 Synchronous Dynamic Random-Access Memory
        /// </summary>
        DDR2,

        /// <summary>
        ///     Graphics Double Data Rate 2 Synchronous Dynamic Random-Access Memory
        /// </summary>
        GDDR2,

        /// <summary>
        ///     Graphics Double Data Rate 3 Synchronous Dynamic Random-Access Memory
        /// </summary>
        GDDR3,

        /// <summary>
        ///     Graphics Double Data Rate 4 Synchronous Dynamic Random-Access Memory
        /// </summary>
        GDDR4,

        /// <summary>
        ///     Double Data Rate 3 Synchronous Dynamic Random-Access Memory
        /// </summary>
        DDR3,

        /// <summary>
        ///     Graphics Double Data Rate 5 Synchronous Dynamic Random-Access Memory
        /// </summary>
        GDDR5,

        /// <summary>
        ///     Lowe Power Double Data Rate 2 Synchronous Dynamic Random-Access Memory
        /// </summary>
        LPDDR2,

        /// <summary>
        ///     Graphics Double Data Rate 5X Synchronous Dynamic Random-Access Memory
        /// </summary>
        GDDR5X
    }
}