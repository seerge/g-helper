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
    [StructureVersion(1)]
    public struct HDRColorDataV1 : IInitializable, IHDRColorData
    {
        internal StructureVersion _Version;
        private readonly ColorDataHDRCommand _Command;
        private readonly ColorDataHDRMode _HDRMode;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly StaticMetadataDescriptorId _StaticMetadataDescriptorId;
        private readonly MasteringDisplayColorData _MasteringDisplayData;

        /// <summary>
        ///     Creates an instance of <see cref="HDRColorDataV1" />.
        /// </summary>
        /// <param name="command">The command to be executed.</param>
        /// <param name="hdrMode">The hdr mode.</param>
        /// <param name="masteringDisplayData">The display color space configurations.</param>
        public HDRColorDataV1(
            ColorDataHDRCommand command,
            ColorDataHDRMode hdrMode,
            MasteringDisplayColorData masteringDisplayData = default
        )
        {
            this = typeof(HDRColorDataV1).Instantiate<HDRColorDataV1>();

            if (command != ColorDataHDRCommand.Set)
            {
                throw new ArgumentOutOfRangeException(nameof(command));
            }

            _Command = command;
            _HDRMode = hdrMode;
            _MasteringDisplayData = masteringDisplayData;
            _StaticMetadataDescriptorId = StaticMetadataDescriptorId.StaticMetadataType1;
        }


        /// <summary>
        ///     Creates an instance of <see cref="HDRColorDataV1" />.
        /// </summary>
        /// <param name="command">The command to be executed.</param>
        public HDRColorDataV1(ColorDataHDRCommand command)
        {
            this = typeof(HDRColorDataV1).Instantiate<HDRColorDataV1>();

            if (command != ColorDataHDRCommand.Get)
            {
                throw new ArgumentOutOfRangeException(nameof(command));
            }

            _Command = command;
        }

        /// <inheritdoc />
        public ColorDataDepth? ColorDepth
        {
            get => null;
        }

        /// <inheritdoc />
        public ColorDataFormat? ColorFormat
        {
            get => null;
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
        public ColorDataDynamicRange? DynamicRange
        {
            get => null;
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
    }
}