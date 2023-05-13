namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Horizontal synchronized polarity modes
    /// </summary>
    public enum TimingHorizontalSyncPolarity : byte
    {
        /// <summary>
        ///     Positive horizontal synchronized polarity
        /// </summary>
        Positive = 0,

        /// <summary>
        ///     Negative horizontal synchronized polarity
        /// </summary>
        Negative = 1,

        /// <summary>
        ///     Default horizontal synchronized polarity
        /// </summary>
        Default = Negative
    }
}