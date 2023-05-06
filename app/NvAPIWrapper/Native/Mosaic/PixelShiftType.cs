namespace NvAPIWrapper.Native.Mosaic
{
    /// <summary>
    ///     Possible pixel shift types for a display
    /// </summary>
    public enum PixelShiftType
    {
        /// <summary>
        ///     No pixel shift will be applied to this display.
        /// </summary>
        NoPixelShift = 0,

        /// <summary>
        ///     This display will be used to scan-out top left pixels in 2x2 PixelShift configuration
        /// </summary>
        TopLeft2X2Pixels = 1,

        /// <summary>
        ///     This display will be used to scan-out bottom right pixels in 2x2 PixelShift configuration
        /// </summary>
        BottomRight2X2Pixels = 2
    }
}