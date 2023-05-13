namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Possible rotate modes
    /// </summary>
    public enum Rotate : uint
    {
        /// <summary>
        ///     No rotation
        /// </summary>
        Degree0 = 0,

        /// <summary>
        ///     90 degree rotation
        /// </summary>
        Degree90 = 1,

        /// <summary>
        ///     180 degree rotation
        /// </summary>
        Degree180 = 2,

        /// <summary>
        ///     270 degree rotation
        /// </summary>
        Degree270 = 3,

        /// <summary>
        ///     This value is ignored
        /// </summary>
        Ignored = 4
    }
}