namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     Connected output device types
    /// </summary>
    public enum OutputType : uint
    {
        /// <summary>
        ///     Unknown display device
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///     CRT display device
        /// </summary>
        CRT = 1,

        /// <summary>
        ///     Digital Flat Panel display device
        /// </summary>
        DFP = 2,

        /// <summary>
        ///     TV display device
        /// </summary>
        TV = 3
    }
}