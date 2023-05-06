namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Contains possible info-frame property modes
    /// </summary>
    public enum InfoFramePropertyMode : uint
    {
        /// <summary>
        ///     Driver determines whether to send info-frames.
        /// </summary>
        Auto = 0,

        /// <summary>
        ///     Driver always sends info-frame.
        /// </summary>
        Enable,

        /// <summary>
        ///     Driver never sends info-frame.
        /// </summary>
        Disable,

        /// <summary>
        ///     Driver only sends info-frame when client requests it via info-frame escape call.
        /// </summary>
        AllowOverride
    }
}