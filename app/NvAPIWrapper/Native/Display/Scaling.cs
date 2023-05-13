namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Possible scaling modes
    /// </summary>
    public enum Scaling
    {
        /// <summary>
        ///     No change
        /// </summary>
        Default = 0,

        /// <summary>
        ///     Balanced  - Full Screen
        /// </summary>
        ToClosest = 1,

        /// <summary>
        ///     Force GPU - Full Screen
        /// </summary>
        ToNative = 2,

        /// <summary>
        ///     Force GPU - Centered\No Scaling
        /// </summary>
        GPUScanOutToNative = 3,

        /// <summary>
        ///     Force GPU - Aspect Ratio
        /// </summary>
        ToAspectScanOutToNative = 5,

        /// <summary>
        ///     Balanced  - Aspect Ratio
        /// </summary>
        ToAspectScanOutToClosest = 6,

        /// <summary>
        ///     Balanced  - Centered\No Scaling
        /// </summary>
        GPUScanOutToClosest = 7,

        /// <summary>
        ///     Customized scaling - For future use
        /// </summary>
        Customized = 255
    }
}