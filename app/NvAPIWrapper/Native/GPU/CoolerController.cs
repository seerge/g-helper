namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     Holds the list of possible cooler controllers
    /// </summary>
    public enum CoolerController : uint
    {
        /// <summary>
        ///     No cooler controller
        /// </summary>
        None = 0,

        /// <summary>
        ///     ADI cooler controller
        /// </summary>
        ADI,

        /// <summary>
        ///     Internal cooler controller
        /// </summary>
        Internal
    }
}