namespace WindowsDisplayAPI.Native.DisplayConfig
{
    /// <summary>
    ///     Possible values for display scan line ordering
    ///     https://msdn.microsoft.com/en-us/library/windows/hardware/ff553977(v=vs.85).aspx
    /// </summary>
    public enum DisplayConfigScanLineOrdering : uint
    {
        /// <summary>
        ///     Indicates that scan-line ordering of the output is unspecified.
        /// </summary>
        NotSpecified = 0,

        /// <summary>
        ///     Indicates that the output is a progressive image.
        /// </summary>
        Progressive = 1,

        /// <summary>
        ///     Indicates that the output is an interlaced image that is created beginning with the upper field.
        /// </summary>
        InterlacedWithUpperFieldFirst = 2,

        /// <summary>
        ///     Indicates that the output is an interlaced image that is created beginning with the lower field.
        /// </summary>
        InterlacedWithLowerFieldFirst = 3
    }
}