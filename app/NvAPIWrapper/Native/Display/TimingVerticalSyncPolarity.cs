namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Vertical synchronized polarity modes
    /// </summary>
    public enum TimingVerticalSyncPolarity : byte
    {
        /// <summary>
        ///     Positive vertical synchronized polarity
        /// </summary>
        Positive = 0,

        /// <summary>
        ///     Negative vertical synchronized polarity
        /// </summary>
        Negative = 1,

        /// <summary>
        ///     Default vertical synchronized polarity
        /// </summary>
        Default = Positive
    }
}