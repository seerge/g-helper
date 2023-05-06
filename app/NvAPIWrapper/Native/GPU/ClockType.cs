namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     Clock types to request
    /// </summary>
    public enum ClockType : uint
    {
        /// <summary>
        ///     Current clock frequencies
        /// </summary>
        CurrentClock = 0,

        /// <summary>
        ///     Base clock frequencies
        /// </summary>
        BaseClock = 1,

        /// <summary>
        ///     Boost clock frequencies
        /// </summary>
        BoostClock = 2
    }
}