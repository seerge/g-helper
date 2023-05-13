namespace NvAPIWrapper.Native.Interfaces.DRS
{
    /// <summary>
    ///     Represents an application rule registered in a profile
    /// </summary>
    public interface IDRSApplication
    {
        /// <summary>
        ///     Gets the application name
        /// </summary>
        string ApplicationName { get; }

        /// <summary>
        ///     Gets the application friendly name
        /// </summary>
        string FriendlyName { get; }

        /// <summary>
        ///     Gets a boolean value indicating if this application is predefined as part of NVIDIA driver
        /// </summary>
        bool IsPredefined { get; }

        /// <summary>
        ///     Gets the application launcher name.
        /// </summary>
        string LauncherName { get; }
    }
}