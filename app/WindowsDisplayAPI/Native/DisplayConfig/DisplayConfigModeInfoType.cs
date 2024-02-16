namespace WindowsDisplayAPI.Native.DisplayConfig
{
    /// <summary>
    ///     Possbile types of modes
    /// </summary>
    public enum DisplayConfigModeInfoType : uint
    {
        /// <summary>
        ///     Invalid value for mode type
        /// </summary>
        Invalid = 0,

        /// <summary>
        ///     Source mode type
        /// </summary>
        Source = 1,

        /// <summary>
        ///     Target mode type
        /// </summary>
        Target = 2,

        /// <summary>
        ///     Display image type
        /// </summary>
        DesktopImage = 3
    }
}