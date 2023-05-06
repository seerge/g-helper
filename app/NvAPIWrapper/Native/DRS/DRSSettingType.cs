namespace NvAPIWrapper.Native.DRS
{
    /// <summary>
    ///     Holds a list of possible setting value types
    /// </summary>
    public enum DRSSettingType : uint
    {
        /// <summary>
        ///     Integer value type
        /// </summary>
        Integer = 0,

        /// <summary>
        ///     Binary value type
        /// </summary>
        Binary,

        /// <summary>
        ///     ASCII string value type
        /// </summary>
        String,

        /// <summary>
        ///     Unicode string value type
        /// </summary>
        UnicodeString
    }
}