namespace WindowsDisplayAPI.Native.DeviceContext
{
    /// <summary>
    ///     Contains possible values for the display orientation
    /// </summary>
    public enum DisplayOrientation : uint
    {
        /// <summary>
        ///     No rotation
        /// </summary>
        Identity = 0,

        /// <summary>
        ///     90 degree rotation
        /// </summary>
        Rotate90Degree = 1,

        /// <summary>
        ///     180 degree rotation
        /// </summary>
        Rotate180Degree = 2,

        /// <summary>
        ///     270 degree rotation
        /// </summary>
        Rotate270Degree = 3
    }
}