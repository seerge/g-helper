namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Possible values for the color data selection policy
    /// </summary>
    public enum ColorDataSelectionPolicy : uint
    {
        /// <summary>
        ///     Application or the Nvidia Control Panel user configuration are used to decide the best color format
        /// </summary>
        User = 0,

        /// <summary>
        ///     Driver or the Operating System decides the best color format
        /// </summary>
        BestQuality = 1,

        /// <summary>
        ///     Default value, <see cref="BestQuality" />
        /// </summary>
        Default = BestQuality,

        /// <summary>
        ///     Unknown policy
        /// </summary>
        Unknown = 0xFF
    }
}