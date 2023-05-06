namespace NvAPIWrapper.Native.DRS
{
    /// <summary>
    ///     Holds possible values for the setting location
    /// </summary>
    public enum DRSSettingLocation : uint
    {
        /// <summary>
        ///     Setting is part of the current profile
        /// </summary>
        CurrentProfile = 0,

        /// <summary>
        ///     Setting is part of the global profile
        /// </summary>
        GlobalProfile,

        /// <summary>
        ///     Setting is part of the base profile
        /// </summary>
        BaseProfile,

        /// <summary>
        ///     Setting is part of the default profile
        /// </summary>
        DefaultProfile
    }
}