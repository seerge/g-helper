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
    [StructureVersion(1)]
    public struct ColorDataV1 : IInitializable, IColorData
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

            public ColorDataBag(ColorDataFormat colorFormat, ColorDataColorimetry colorimetry)
            {
                ColorFormat = (byte)colorFormat;
                Colorimetry = (byte)colorimetry;
            }
        }

        /// <summary>
        ///     Creates an instance of <see cref="ColorDataV1" /> to retrieve color data information
        /// </summary>
        /// <param name="command">The command to be executed.</param>
        public ColorDataV1(ColorDataCommand command)
        {
            this = typeof(ColorDataV1).Instantiate<ColorDataV1>();
            _Size = (ushort)_Version.StructureSize;

            if (command != ColorDataCommand.Get && command != ColorDataCommand.GetDefault)
            {
                throw new ArgumentOutOfRangeException(nameof(command));
            }

            _Command = (byte)command;
        }

        /// <summary>
        ///     Creates an instance of <see cref="ColorDataV1" /> to modify the color data
        /// </summary>
        /// <param name="command">The command to be executed.</param>
        /// <param name="colorFormat">The color data color format.</param>
        /// <param name="colorimetry">The color data color space.</param>
        public ColorDataV1(
            ColorDataCommand command,
            ColorDataFormat colorFormat,
            ColorDataColorimetry colorimetry
        )
        {
            this = typeof(ColorDataV1).Instantiate<ColorDataV1>();
            _Size = (ushort)_Version.StructureSize;

            if (command != ColorDataCommand.Set && command != ColorDataCommand.IsSupportedColor)
            {
                throw new ArgumentOutOfRangeException(nameof(command));
            }

            _Command = (byte)command;
            _Data = new ColorDataBag(colorFormat, colorimetry);
        }

        /// <inheritdoc />
        public ColorDataFormat ColorFormat
        {
            get => (ColorDataFormat)_Data.ColorFormat;
        }

        /// <inheritdoc />
        public ColorDataColorimetry Colorimetry
        {
            get => (ColorDataColorimetry)_Data.Colorimetry;
        }

        /// <inheritdoc />
        public ColorDataDynamicRange? DynamicRange
        {
            get => null;
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