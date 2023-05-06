using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;
using NvAPIWrapper.Native.Interfaces.Display;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <inheritdoc cref="IColorData" />
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(3)]
    public struct ColorDataV3 : IInitializable, IColorData
    {
        internal StructureVersion _Version;
        internal ushort _Size;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly byte _Command;
        private readonly ColorDataBag _Data;

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        private struct ColorDataBag
        {
            public readonly byte ColorFormat;
            public readonly byte Colorimetry;
            public readonly byte ColorDynamicRange;
            public readonly ColorDataDepth ColorDepth;

            public ColorDataBag(
                ColorDataFormat colorFormat,
                ColorDataColorimetry colorimetry,
                ColorDataDynamicRange colorDynamicRange,
                ColorDataDepth colorDepth
            )
            {
                ColorFormat = (byte) colorFormat;
                Colorimetry = (byte) colorimetry;
                ColorDynamicRange = (byte) colorDynamicRange;
                ColorDepth = colorDepth;
            }
        }

        /// <summary>
        ///     Creates an instance of <see cref="ColorDataV3" /> to retrieve color data information
        /// </summary>
        /// <param name="command">The command to be executed.</param>
        public ColorDataV3(ColorDataCommand command)
        {
            this = typeof(ColorDataV3).Instantiate<ColorDataV3>();
            _Size = (ushort) _Version.StructureSize;

            if (command != ColorDataCommand.Get && command != ColorDataCommand.GetDefault)
            {
                throw new ArgumentOutOfRangeException(nameof(command));
            }

            _Command = (byte) command;
        }

        /// <summary>
        ///     Creates an instance of <see cref="ColorDataV3" /> to modify the color data
        /// </summary>
        /// <param name="command">The command to be executed.</param>
        /// <param name="colorFormat">The color data color format.</param>
        /// <param name="colorimetry">The color data color space.</param>
        /// <param name="colorDynamicRange">The color data dynamic range.</param>
        /// <param name="colorDepth">The color data color depth.</param>
        public ColorDataV3(
            ColorDataCommand command,
            ColorDataFormat colorFormat,
            ColorDataColorimetry colorimetry,
            ColorDataDynamicRange colorDynamicRange,
            ColorDataDepth colorDepth
        )
        {
            this = typeof(ColorDataV3).Instantiate<ColorDataV3>();
            _Size = (ushort) _Version.StructureSize;

            if (command != ColorDataCommand.Set && command != ColorDataCommand.IsSupportedColor)
            {
                throw new ArgumentOutOfRangeException(nameof(command));
            }

            _Command = (byte) command;
            _Data = new ColorDataBag(colorFormat, colorimetry, colorDynamicRange, colorDepth);
        }

        /// <inheritdoc />
        public ColorDataFormat ColorFormat
        {
            get => (ColorDataFormat) _Data.ColorFormat;
        }

        /// <inheritdoc />
        public ColorDataColorimetry Colorimetry
        {
            get => (ColorDataColorimetry) _Data.Colorimetry;
        }

        /// <inheritdoc />
        public ColorDataDynamicRange? DynamicRange
        {
            get => (ColorDataDynamicRange) _Data.ColorDynamicRange;
        }

        /// <inheritdoc />
        public ColorDataDepth? ColorDepth
        {
            get
            {
                switch ((int) _Data.ColorDepth)
                {
                    case 6:
                        return ColorDataDepth.BPC6;
                    case 8:
                        return ColorDataDepth.BPC8;
                    case 10:
                        return ColorDataDepth.BPC10;
                    case 12:
                        return ColorDataDepth.BPC12;
                    case 16:
                        return ColorDataDepth.BPC16;
                    default:
                        return _Data.ColorDepth;
                }
            }
        }

        /// <inheritdoc />
        public ColorDataSelectionPolicy? SelectionPolicy
        {
            get => null;
        }

        /// <inheritdoc />
        public ColorDataDesktopDepth? DesktopColorDepth
        {
            get => null;
        }
    }
}