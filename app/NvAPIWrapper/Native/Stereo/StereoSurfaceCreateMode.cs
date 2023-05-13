namespace NvAPIWrapper.Native.Stereo
{
    /// <summary>
    ///     Holds a list of valid values for the stereo surface creation mode
    /// </summary>
    public enum StereoSurfaceCreateMode
    {
        /// <summary>
        ///     Automatic surface creation
        /// </summary>
        Auto = 0,

        /// <summary>
        ///     Force stereo surface creation
        /// </summary>
        ForceStereo,

        /// <summary>
        ///     Force mono surface creation
        /// </summary>
        ForceMono
    }
}