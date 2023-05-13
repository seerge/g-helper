using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;
using NvAPIWrapper.Native.Interfaces.Display;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <inheritdoc cref="IHDRColorData" />
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(2)]
    public struct HDRColorDataV2 : IInitializable, IHDRColorData
    {
        internal StructureVersion _Version;
        private readonly ColorDataHDRCommand _Command;
        private readonly ColorDataHDRMode _HDRMode;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly StaticMetadataDescriptorId _StaticMetadataDescriptorId;
        private readonly MasteringDisplayColorData _MasteringDisplayData;
        private readonly ColorDataFormat _ColorFormat;
        private readonly ColorDataDynamicRange _DynamicRange;
        private readonly ColorDataDepth _ColorDepth;

        /// <summary>
        ///     Creates an instance of <see cref="HDRColorDataV2" />.
        /// </summary>
        /// <param name="command">The command to be executed.</param>
        /// <param name="hdrMode">The hdr mode.</param>
        /// <param name="masteringDisplayData">The display color space configurations.</param>
        /// <param name="colorFormat">The color data color format.</param>
        /// <param name="dynamicRange">The color data dynamic range.</param>
        /// <param name="colorDepth">The color data color depth.</param>
        public HDRColorDataV2(
            ColorDataHDRCommand command,
            ColorDataHDRMode hdrMode,
            MasteringDisplayColorData masteringDisplayData = default,
            ColorDataFormat colorFormat = ColorDataFormat.Default,
            ColorDataDynamicRange dynamicRange = ColorDataDynamicRange.Auto,
            ColorDataDepth colorDepth = ColorDataDepth.Default
        )
        {
            this = typeof(HDRColorDataV2).Instantiate<HDRColorDataV2>();

            if (command != ColorDataHDRCommand.Set)
            {
                throw new ArgumentOutOfRangeException(nameof(command));
            }

            _Command = command;
            _HDRMode = hdrMode;
            _MasteringDisplayData = masteringDisplayData;
            _ColorFormat = colorFormat;
            _DynamicRange = dynamicRange;
            _ColorDepth = colorDepth;
            _StaticMetadataDescriptorId = StaticMetadataDescriptorId.StaticMetadataType1;
        }

        /// <summary>
        ///     Creates an instance of <see cref="HDRColorDataV2" />.
        /// </summary>
        /// <param name="command">The command to be executed.</param>
        public HDRColorDataV2(ColorDataHDRCommand command)
        {
            this = typeof(HDRColorDataV2).Instantiate<HDRColorDataV2>();

            if (command != ColorDataHDRCommand.Get)
            {
                throw new ArgumentOutOfRangeException(nameof(command));
            }

            _Command = command;
        }

        /// <summary>
        ///     Gets the color data command
        /// </summary>
        // ReSharper disable once ConvertToAutoProperty
        public ColorDataHDRCommand Command
        {
            get => _Command;
        }

        /// <inheritdoc />
        // ReSharper disable once ConvertToAutoProperty
        public ColorDataHDRMode HDRMode
        {
            get => _HDRMode;
        }

        /// <inheritdoc />
        // ReSharper disable once ConvertToAutoProperty
        public MasteringDisplayColorData MasteringDisplayData
        {
            get => _MasteringDisplayData;
        }

        /// <inheritdoc />
        public ColorDataFormat? ColorFormat
        {
            get => _ColorFormat;
        }

        /// <inheritdoc />
        public ColorDataDynamicRange? DynamicRange
        {
            get => _DynamicRange;
        }

        /// <inheritdoc />
        public ColorDataDepth? ColorDepth
        {
            get
            {
                switch ((uint) _ColorDepth)
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
                        return _ColorDepth;
                }
            }
        }
    }
}