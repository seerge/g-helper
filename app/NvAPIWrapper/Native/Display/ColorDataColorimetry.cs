namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Contains possible values for color data color space
    /// </summary>
    public enum ColorDataColorimetry : uint
    {
        /// <summary>
        ///     RGB color space
        /// </summary>
        RGB = 0,

        /// <summary>
        ///     YCC601 color space
        /// </summary>
        YCC601,

        /// <summary>
        ///     YCC709 color space
        /// </summary>
        YCC709,

        /// <summary>
        ///     XVYCC601 color space
        /// </summary>
        XVYCC601,

        /// <summary>
        ///     XVYCC709 color space
        /// </summary>
        XVYCC709,

        /// <summary>
        ///     SYCC601 color space
        /// </summary>
        SYCC601,

        /// <summary>
        ///     ADOBEYCC601 color space
        /// </summary>
        ADOBEYCC601,

        /// <summary>
        ///     ADOBERGB color space
        /// </summary>
        ADOBERGB,

        /// <summary>
        ///     BT2020RGB color space
        /// </summary>
        BT2020RGB,

        /// <summary>
        ///     BT2020YCC color space
        /// </summary>
        BT2020YCC,

        /// <summary>
        ///     BT2020cYCC color space
        /// </summary>
        // ReSharper disable once InconsistentNaming
        BT2020cYCC,

        /// <summary>
        ///     Default color space
        /// </summary>
        Default = 0xFE,

        /// <summary>
        ///     Automatically select color space
        /// </summary>
        Auto = 0xFF
    }
}