namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     Holds the possible clock lock modes
    /// </summary>
    public enum ClockLockMode : uint
    {
        /// <summary>
        ///     No clock lock
        /// </summary>
        None = 0,

        /// <summary>
        ///     Manual clock lock
        /// </summary>
        Manual = 3
    }
}