namespace WindowsDisplayAPI.Native.DeviceContext
{
    /// <summary>
    ///     Contains possible values for the display fixed output
    /// </summary>
    public enum DisplayFixedOutput : uint
    {
        /// <summary>
        ///     Default behavior
        /// </summary>
        Default = 0,

        /// <summary>
        ///     Stretches the output to fit to the display
        /// </summary>
        Stretch = 1,

        /// <summary>
        ///     Centers the output in the middle of the display
        /// </summary>
        Center = 2
    }
}