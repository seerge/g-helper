namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Contains possible values for the color data command
    /// </summary>
    public enum ColorDataCommand : uint
    {
        /// <summary>
        ///     Get the current color data
        /// </summary>
        Get = 1,

        /// <summary>
        ///     Set the current color data
        /// </summary>
        Set,

        /// <summary>
        ///     Check if the passed color data is supported
        /// </summary>
        IsSupportedColor,

        /// <summary>
        ///     Get the default color data
        /// </summary>
        GetDefault
    }
}