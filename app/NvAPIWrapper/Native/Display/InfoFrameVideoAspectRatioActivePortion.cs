namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Contains possible values for AVI aspect ratio portions
    /// </summary>
    public enum InfoFrameVideoAspectRatioActivePortion : uint
    {
        /// <summary>
        ///     Disabled or not available
        /// </summary>
        Disabled = 0,

        /// <summary>
        ///     Letter box 16x9
        /// </summary>
        LetterboxGreaterThan16X9 = 4,

        /// <summary>
        ///     Equal to the source frame size
        /// </summary>
        EqualCodedFrame = 8,

        /// <summary>
        ///     Centered 4x3 ratio
        /// </summary>
        Center4X3 = 9,

        /// <summary>
        ///     Centered 16x9 ratio
        /// </summary>
        Center16X9 = 10,

        /// <summary>
        ///     Centered 14x9 ratio
        /// </summary>
        Center14X9 = 11,

        /// <summary>
        ///     Bordered 4x3 on 14x9
        /// </summary>
        Bordered4X3On14X9 = 13,

        /// <summary>
        ///     Bordered 16x9 on 14x9
        /// </summary>
        Bordered16X9On14X9 = 14,

        /// <summary>
        ///     Bordered 16x9 on 4x3
        /// </summary>
        Bordered16X9On4X3 = 15,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 31
    }
}