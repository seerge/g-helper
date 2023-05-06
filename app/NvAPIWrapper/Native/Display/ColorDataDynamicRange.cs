namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Contains possible values for color data dynamic range
    /// </summary>
    public enum ColorDataDynamicRange : uint
    {
        /// <summary>
        ///     VESA standard progress signal
        /// </summary>
        VESA = 0,

        /// <summary>
        ///     CEA interlaced signal
        /// </summary>
        CEA,

        /// <summary>
        ///     Automatically select the best value
        /// </summary>
        Auto
    }
}