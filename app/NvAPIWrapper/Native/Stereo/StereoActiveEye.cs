namespace NvAPIWrapper.Native.Stereo
{
    /// <summary>
    ///     Holds a list of valid values for back buffer mode
    /// </summary>
    public enum StereoActiveEye
    {
        /// <summary>
        ///     No back buffer
        /// </summary>
        None = 0,

        /// <summary>
        ///     Right eye back buffer mode
        /// </summary>
        RightEye = 1,

        /// <summary>
        ///     Left eye back buffer mode
        /// </summary>
        LeftEye = 2,

        /// <summary>
        ///     Mono back buffer mode
        /// </summary>
        Mono = 3
    }
}