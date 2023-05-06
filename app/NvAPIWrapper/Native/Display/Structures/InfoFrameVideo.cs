using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Helpers;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <summary>
    ///     Contains info-frame video information
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 8)]
    public struct InfoFrameVideo
    {
        [FieldOffset(0)] private readonly uint _WordAt0;
        [FieldOffset(4)] private readonly uint _WordAt4;
        [FieldOffset(8)] private readonly uint _WordAt8;
        [FieldOffset(12)] private readonly uint _WordAt12;
        [FieldOffset(16)] private readonly uint _WordAt16;
        [FieldOffset(20)] private readonly uint _WordAt20;

        /// <summary>
        ///     Creates an instance of <see cref="InfoFrameVideo" />.
        /// </summary>
        /// <param name="videoIdentificationCode">The video identification code (VIC)</param>
        /// <param name="pixelRepetition">The video pixel repetition</param>
        /// <param name="colorFormat">The video color format</param>
        /// <param name="colorimetry">The video color space</param>
        /// <param name="extendedColorimetry">The extended video color space</param>
        /// <param name="rgbQuantization">The RGB quantization configuration</param>
        /// <param name="yccQuantization">The YCC quantization configuration</param>
        /// <param name="contentMode">The video content mode</param>
        /// <param name="contentType">The video content type</param>
        /// <param name="scanInfo">The video scan information</param>
        /// <param name="isActiveFormatInfoPresent">A value indicating if the active format information is present</param>
        /// <param name="activeFormatAspectRatio">The active format aspect ratio</param>
        /// <param name="pictureAspectRatio">The picture aspect ratio</param>
        /// <param name="nonUniformPictureScaling">The non uniform picture scaling direction</param>
        /// <param name="barInfo">The video bar information</param>
        /// <param name="topBar">The top bar value if not auto and present; otherwise null</param>
        /// <param name="bottomBar">The bottom bar value if not auto and present; otherwise null</param>
        /// <param name="leftBar">The left bar value if not auto and present; otherwise null</param>
        /// <param name="rightBar">The right bar value if not auto and present; otherwise null</param>
        public InfoFrameVideo(
            byte videoIdentificationCode,
            InfoFrameVideoPixelRepetition pixelRepetition,
            InfoFrameVideoColorFormat colorFormat,
            InfoFrameVideoColorimetry colorimetry,
            InfoFrameVideoExtendedColorimetry extendedColorimetry,
            InfoFrameVideoRGBQuantization rgbQuantization,
            InfoFrameVideoYCCQuantization yccQuantization,
            InfoFrameVideoITC contentMode,
            InfoFrameVideoContentType contentType,
            InfoFrameVideoScanInfo scanInfo,
            InfoFrameBoolean isActiveFormatInfoPresent,
            InfoFrameVideoAspectRatioActivePortion activeFormatAspectRatio,
            InfoFrameVideoAspectRatioCodedFrame pictureAspectRatio,
            InfoFrameVideoNonUniformPictureScaling nonUniformPictureScaling,
            InfoFrameVideoBarData barInfo,
            uint? topBar,
            uint? bottomBar,
            uint? leftBar,
            uint? rightBar
        )
        {
            _WordAt0 = 0u
                .SetBits(0, 8, videoIdentificationCode)
                .SetBits(8, 5, (uint) pixelRepetition)
                .SetBits(13, 3, (uint) colorFormat)
                .SetBits(16, 3, (uint) colorimetry)
                .SetBits(19, 4, (uint) extendedColorimetry)
                .SetBits(23, 3, (uint) rgbQuantization)
                .SetBits(26, 3, (uint) yccQuantization)
                .SetBits(29, 2, (uint) contentMode);

            _WordAt4 = 0u
                .SetBits(0, 3, (uint) contentType)
                .SetBits(3, 3, (uint) scanInfo)
                .SetBits(6, 2, (uint) isActiveFormatInfoPresent)
                .SetBits(8, 5, (uint) activeFormatAspectRatio)
                .SetBits(13, 3, (uint) pictureAspectRatio)
                .SetBits(16, 3, (uint) nonUniformPictureScaling)
                .SetBits(19, 3, (uint) barInfo);

            _WordAt8 = topBar == null ? 0x1FFFF : 0u.SetBits(0, 17, topBar.Value);
            _WordAt12 = bottomBar == null ? 0x1FFFF : 0u.SetBits(0, 17, bottomBar.Value);
            _WordAt16 = leftBar == null ? 0x1FFFF : 0u.SetBits(0, 17, leftBar.Value);
            _WordAt20 = rightBar == null ? 0x1FFFF : 0u.SetBits(0, 17, rightBar.Value);
        }

        /// <summary>
        ///     Gets the video identification code (VIC)
        /// </summary>
        // ReSharper disable once ConvertToAutoProperty
        public byte? VideoIdentificationCode
        {
            get
            {
                var value = (byte) _WordAt0.GetBits(0, 8);

                if (value == 0xFF)
                {
                    return null;
                }

                return value;
            }
        }

        /// <summary>
        ///     Gets the video pixel repetition
        /// </summary>
        public InfoFrameVideoPixelRepetition PixelRepetition
        {
            get => (InfoFrameVideoPixelRepetition) _WordAt0.GetBits(8, 5);
        }

        /// <summary>
        ///     Gets the video color format
        /// </summary>
        public InfoFrameVideoColorFormat ColorFormat
        {
            get => (InfoFrameVideoColorFormat) _WordAt0.GetBits(13, 3);
        }

        /// <summary>
        ///     Gets the video color space
        /// </summary>
        public InfoFrameVideoColorimetry Colorimetry
        {
            get => (InfoFrameVideoColorimetry) _WordAt0.GetBits(16, 3);
        }

        /// <summary>
        ///     Gets the extended video color space; only valid when <see cref="Colorimetry" /> ==
        ///     <see cref="InfoFrameVideoColorimetry.UseExtendedColorimetry" />
        /// </summary>
        public InfoFrameVideoExtendedColorimetry? ExtendedColorimetry
        {
            get
            {
                if (Colorimetry != InfoFrameVideoColorimetry.UseExtendedColorimetry)
                {
                    return null;
                }

                return (InfoFrameVideoExtendedColorimetry) _WordAt0.GetBits(19, 4);
            }
        }

        /// <summary>
        ///     Gets the RGB quantization configuration
        /// </summary>
        public InfoFrameVideoRGBQuantization RGBQuantization
        {
            get => (InfoFrameVideoRGBQuantization) _WordAt0.GetBits(23, 3);
        }

        /// <summary>
        ///     Gets the YCC quantization configuration
        /// </summary>
        public InfoFrameVideoYCCQuantization YCCQuantization
        {
            get => (InfoFrameVideoYCCQuantization) _WordAt0.GetBits(26, 3);
        }

        /// <summary>
        ///     Gets the video content mode
        /// </summary>
        public InfoFrameVideoITC ContentMode
        {
            get => (InfoFrameVideoITC) _WordAt0.GetBits(29, 2);
        }

        /// <summary>
        ///     Gets the video content type
        /// </summary>
        public InfoFrameVideoContentType ContentType
        {
            get => (InfoFrameVideoContentType) _WordAt4.GetBits(0, 3);
        }

        /// <summary>
        ///     Gets the video scan information
        /// </summary>
        public InfoFrameVideoScanInfo ScanInfo
        {
            get => (InfoFrameVideoScanInfo) _WordAt4.GetBits(3, 3);
        }

        /// <summary>
        ///     Gets a value indicating if the active format information is present
        /// </summary>
        public InfoFrameBoolean IsActiveFormatInfoPresent
        {
            get => (InfoFrameBoolean) _WordAt4.GetBits(6, 2);
        }

        /// <summary>
        ///     Gets the active format aspect ratio
        /// </summary>
        public InfoFrameVideoAspectRatioActivePortion ActiveFormatAspectRatio
        {
            get => (InfoFrameVideoAspectRatioActivePortion) _WordAt4.GetBits(8, 5);
        }

        /// <summary>
        ///     Gets the picture aspect ratio
        /// </summary>
        public InfoFrameVideoAspectRatioCodedFrame PictureAspectRatio
        {
            get => (InfoFrameVideoAspectRatioCodedFrame) _WordAt4.GetBits(13, 3);
        }

        /// <summary>
        ///     Gets the non uniform picture scaling direction
        /// </summary>
        public InfoFrameVideoNonUniformPictureScaling NonUniformPictureScaling
        {
            get => (InfoFrameVideoNonUniformPictureScaling) _WordAt4.GetBits(16, 3);
        }

        /// <summary>
        ///     Gets the video bar information
        /// </summary>
        public InfoFrameVideoBarData BarInfo
        {
            get => (InfoFrameVideoBarData) _WordAt4.GetBits(19, 3);
        }

        /// <summary>
        ///     Gets the top bar value if not auto and present; otherwise null
        /// </summary>
        public uint? TopBar
        {
            get
            {
                if (BarInfo == InfoFrameVideoBarData.NotPresent || BarInfo == InfoFrameVideoBarData.Horizontal)
                {
                    return null;
                }

                var val = _WordAt8.GetBits(0, 17);

                if (val == 0x1FFFF)
                {
                    return null;
                }

                return (uint) val;
            }
        }

        /// <summary>
        ///     Gets the bottom bar value if not auto and present; otherwise null
        /// </summary>
        public uint? BottomBar
        {
            get
            {
                if (BarInfo == InfoFrameVideoBarData.NotPresent || BarInfo == InfoFrameVideoBarData.Horizontal)
                {
                    return null;
                }

                var val = _WordAt12.GetBits(0, 17);

                if (val == 0x1FFFF)
                {
                    return null;
                }

                return (uint) val;
            }
        }

        /// <summary>
        ///     Gets the left bar value if not auto and present; otherwise null
        /// </summary>
        public uint? LeftBar
        {
            get
            {
                if (BarInfo == InfoFrameVideoBarData.NotPresent || BarInfo == InfoFrameVideoBarData.Vertical)
                {
                    return null;
                }

                var val = _WordAt16.GetBits(0, 17);

                if (val == 0x1FFFF)
                {
                    return null;
                }

                return (uint) val;
            }
        }

        /// <summary>
        ///     Gets the right bar value if not auto and present; otherwise null
        /// </summary>
        public uint? RightBar
        {
            get
            {
                if (BarInfo == InfoFrameVideoBarData.NotPresent || BarInfo == InfoFrameVideoBarData.Vertical)
                {
                    return null;
                }

                var val = _WordAt20.GetBits(0, 17);

                if (val == 0x1FFFF)
                {
                    return null;
                }

                return (uint) val;
            }
        }
    }
}