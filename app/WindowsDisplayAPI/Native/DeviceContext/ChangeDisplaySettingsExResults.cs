namespace WindowsDisplayAPI.Native.DeviceContext
{
    /// <summary>
    ///     Contains possible values for the result of mode change request
    /// </summary>
    public enum ChangeDisplaySettingsExResults
    {
        /// <summary>
        ///     Completed successfully
        /// </summary>
        Successful = 0,

        /// <summary>
        ///     Changes needs restart
        /// </summary>
        Restart = 1,

        /// <summary>
        ///     Failed to change and save setings
        /// </summary>
        Failed = -1,

        /// <summary>
        ///     Invalid data provide
        /// </summary>
        BadMode = -2,

        /// <summary>
        ///     Changes not updated
        /// </summary>
        NotUpdated = -3,

        /// <summary>
        ///     Invalid flags provided
        /// </summary>
        BadFlags = -4,

        /// <summary>
        ///     Bad parameters provided
        /// </summary>
        BadParam = -5,

        /// <summary>
        ///     Bad Dual View mode used with mode
        /// </summary>
        BadDualView = -6
    }
}