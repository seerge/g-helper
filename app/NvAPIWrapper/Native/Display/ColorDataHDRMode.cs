using System;

namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Contains possible color data HDR modes
    /// </summary>
    public enum ColorDataHDRMode : uint
    {
        /// <summary>
        ///     Turn off HDR.
        /// </summary>
        Off = 0,

        /// <summary>
        ///     Source: CCCS [a.k.a FP16 scRGB, linear, sRGB primaries, [-65504,0, 65504] range, RGB(1,1,1) = 80nits]
        ///     Output: UHDA HDR [a.k.a HDR10, RGB/YCC 10/12bpc ST2084(PQ) EOTF RGB(1,1,1) = 10000 nits, Rec2020 color primaries,
        ///     ST2086 static HDR metadata].
        ///     This is the only supported production HDR mode.
        /// </summary>
        UHDA = 2,

        /// <summary>
        ///     Source: CCCS (a.k.a FP16 scRGB)
        ///     Output: EDR (Extended Dynamic Range) - HDR content is tone-mapped and gamut mapped to output on regular SDR display
        ///     set to max luminance ( ~300 nits ).
        /// </summary>
        [Obsolete("Do not use! Internal test mode only, to be removed.", false)]
        EDR = 3,

        /// <summary>
        ///     Source: any
        ///     Output: SDR (Standard Dynamic Range), we continuously send SDR EOTF InfoFrame signaling, HDMI compliance testing.
        /// </summary>
        [Obsolete("Do not use! Internal test mode only, to be removed.", false)]
        SDR = 4,

        /// <summary>
        ///     Source: HDR10 RGB 10bpc
        ///     Output: HDR10 RGB 10 colorDepth - signal UHDA HDR mode (PQ + Rec2020) to the sink but send source pixel values
        ///     unmodified (no PQ or Rec2020 conversions) - assumes source is already in HDR10 format.
        /// </summary>
        [Obsolete("Experimental mode only, not for production!", false)]
        UHDAPassthrough = 5,

        /// <summary>
        ///     Source: CCCS (a.k.a FP16 scRGB)
        ///     Output: notebook HDR
        /// </summary>
        [Obsolete("Do not use! Internal test mode only, to be removed.", false)]
        UHDANB = 6,

        /// <summary>
        ///     Source: RGB8 Dolby Vision encoded (12 colorDepth YCbCr422 packed into RGB8)
        ///     Output: Dolby Vision encoded : Application is to encoded frames in DV format and embed DV dynamic metadata as
        ///     described in Dolby Vision specification.
        /// </summary>
        [Obsolete("Experimental mode only, not for production!", false)]
        DolbyVision = 7
    }
}