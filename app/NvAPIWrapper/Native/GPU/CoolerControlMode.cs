namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     Holds possible cooler control modes
    /// </summary>
    public enum CoolerControlMode : uint
    {
        /// <summary>
        ///     No cooler control
        /// </summary>
        None = 0,

        /// <summary>
        ///     Toggle based cooler control mode
        /// </summary>
        Toggle,

        /// <summary>
        ///     Variable cooler control mode
        /// </summary>
        Variable
    }
}