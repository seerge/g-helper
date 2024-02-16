using System;

namespace WindowsDisplayAPI
{
    /// <summary>
    ///     Contains possible text drawing capabilities of a display device
    /// </summary>
    [Flags]
    public enum DisplayTextCapabilities
    {
        /// <summary>
        ///     Device is capable of character output precision.
        /// </summary>
        CharacterOutputPrecision = 1,

        /// <summary>
        ///     Device is capable of stroke output precision.
        /// </summary>
        StrokeOutputPrecision = 2,

        /// <summary>
        ///     Device is capable of stroke clip precision.
        /// </summary>
        StrokeClipPrecision = 4,

        /// <summary>
        ///     Device is capable of 90-degree character rotation.
        /// </summary>
        CharacterRotation90 = 8,

        /// <summary>
        ///     Device is capable of any character rotation.
        /// </summary>
        CharacterRotationAny = 16,

        /// <summary>
        ///     Device can scale independently in the x-direction and y-direction.
        /// </summary>
        IndependentXYScaling = 32,

        /// <summary>
        ///     Device is capable of doubled character for scaling.
        /// </summary>
        DoubleCharacterScaling = 64,

        /// <summary>
        ///     Device uses integer multiples only for character scaling.
        /// </summary>
        IntegerCharacterScaling = 128,

        /// <summary>
        ///     Device uses any multiples for exact character scaling.
        /// </summary>
        ExactCharacterScaling = 256,

        /// <summary>
        ///     Device can draw double-weight characters.
        /// </summary>
        DoubleWeightCharacter = 512,

        /// <summary>
        ///     Device can italicize.
        /// </summary>
        CanItalicize = 1024,

        /// <summary>
        ///     Device can underline.
        /// </summary>
        CanUnderline = 2048,

        /// <summary>
        ///     Device can draw strikeouts.
        /// </summary>
        CanStrikeout = 4096,

        /// <summary>
        ///     Device can draw raster fonts.
        /// </summary>
        RasterFonts = 8192,

        /// <summary>
        ///     Device can draw vector fonts.
        /// </summary>
        VectorFonts = 16384,

        /// <summary>
        ///     Device cannot scroll using a bit-block transfer. Note that this meaning may be the opposite of what you expect.
        /// </summary>
        BitBlockTransferScrollInAbility = 65536
    }
}