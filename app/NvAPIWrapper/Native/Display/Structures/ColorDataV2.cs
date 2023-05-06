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
    [StructureVersion(2)]
    public struct ColorDataV2 : IInitializable, IColorData
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

            public ColorDataBag(
                ColorDataFormat colorFormat,
                ColorDataColorimetry colorimetry,
                ColorDataDynamicRange colorDynamicRange
            )
            {
                ColorFormat = (byte) colorFormat;
                Colorimetry = (byte) colorimetry;
                ColorDynamicRange = (byte) colorDynamicRange;
            }
        }

        /// <summary>
        ///     Creates an instance of <see cref="ColorDataV2" /> to retrieve color data information
        /// </summary>
        /// <param name="command">The command to be executed.</param>
        public ColorDataV2(ColorDataCommand command)
        {
            this = typeof(ColorDataV2).Instantiate<ColorDataV2>();
            _Size = (ushort) _Version.StructureSize;

            if (command != ColorDataCommand.Get && command != ColorDataCommand.GetDefault)
            {
                throw new ArgumentOutOfRangeException(nameof(command));
            }

            _Command = (byte) command;
        }

        /// <summary>
        ///     Creates an instance of <see cref="ColorDataV2" /> to modify the color data
        /// </summary>
        /// <param name="command">The command to be executed.</param>
        /// <param name="colorFormat">The color data color format.</param>
        /// <param name="colorimetry">The color data color space.</param>
        /// <param name="colorDynamicRange">The color data dynamic range.</param>
        public ColorDataV2(
            ColorDataCommand command,
            ColorDataFormat colorFormat,
            ColorDataColorimetry colorimetry,
            ColorDataDynamicRange colorDynamicRange
        )
        {
            this = typeof(ColorDataV2).Instantiate<ColorDataV2>();
            _Size = (ushort) _Version.StructureSize;

            if (command != ColorDataCommand.Set && command != ColorDataCommand.IsSupportedColor)
            {
                throw new ArgumentOutOfRangeException(nameof(command));
            }

            _Command = (byte) command;
            _Data = new ColorDataBag(colorFormat, colorimetry, colorDynamicRange);
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
            get => null;
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