namespace WindowsDisplayAPI.Native.DisplayConfig
{
    /// <summary>
    ///     Rotation modes
    ///     https://msdn.microsoft.com/en-us/library/windows/hardware/ff553970(v=vs.85).aspx
    /// </summary>
    public enum DisplayConfigRotation : uint
    {
        /// <summary>
        ///     Rotation mode is not specified
        /// </summary>
        NotSpecified = 0,

        /// <summary>
        ///     Indicates that rotation is 0 degrees—landscape mode.
        /// </summary>
        Identity = 1,

        /// <summary>
        ///     Indicates that rotation is 90 degrees clockwise—portrait mode.
        /// </summary>
        Rotate90 = 2,

        /// <summary>
        ///     Indicates that rotation is 180 degrees clockwise—inverted landscape mode.
        /// </summary>
        Rotate180 = 3,

        /// <summary>
        ///     Indicates that rotation is 270 degrees clockwise—inverted portrait mode.
        /// </summary>
        Rotate270 = 4
    }
}