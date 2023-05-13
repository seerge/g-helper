namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Contains possible values for the color data desktop color depth
    /// </summary>
    public enum ColorDataDesktopDepth : uint
    {
        /// <summary>
        ///     Default color depth meaning that the current setting should be kept
        /// </summary>
        Default = 0x0,

        /// <summary>
        ///     8bit per integer color component
        /// </summary>
        BPC8 = 0x1,

        /// <summary>
        ///     10bit integer per color component
        /// </summary>
        BPC10 = 0x2,

        /// <summary>
        ///     16bit float per color component
        /// </summary>
        BPC16Float = 0x3,

        /// <summary>
        ///     16bit float per color component wide color gamut
        /// </summary>
        BPC16FloatWcg = 0x4,

        /// <summary>
        ///     16bit float per color component HDR
        /// </summary>
        BPC16FloatHDR = 0x5
    }
}