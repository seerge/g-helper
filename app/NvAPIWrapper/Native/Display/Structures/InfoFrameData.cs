using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <summary>
    ///     Contains info-frame requested information or information to be overriden
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 8)]
    [StructureVersion(1)]
    public struct InfoFrameData : IInitializable
    {
        [FieldOffset(0)] internal StructureVersion _Version;

        [FieldOffset(4)]
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly ushort _Size;

        [FieldOffset(6)]
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly byte _Command;

        [FieldOffset(7)] private readonly byte _Type;

        [FieldOffset(8)] private readonly InfoFrameProperty _Property;
        [FieldOffset(8)] private readonly InfoFrameAudio _Audio;
        [FieldOffset(8)] private readonly InfoFrameVideo _Video;

        /// <summary>
        ///     Creates a new instance of <see cref="InfoFrameData" />.
        /// </summary>
        /// <param name="command">
        ///     The operation to be done. Can be used for information retrieval or to reset configurations to
        ///     default.
        /// </param>
        /// <param name="dataType">The type of information.</param>
        public InfoFrameData(InfoFrameCommand command, InfoFrameDataType dataType)
        {
            this = typeof(InfoFrameData).Instantiate<InfoFrameData>();
            _Size = (ushort) _Version.StructureSize;

            if (command != InfoFrameCommand.Get &&
                command != InfoFrameCommand.GetDefault &&
                command != InfoFrameCommand.GetOverride &&
                command != InfoFrameCommand.GetProperty &&
                command != InfoFrameCommand.Reset)
            {
                throw new ArgumentOutOfRangeException(nameof(command));
            }

            _Command = (byte) command;
            _Type = (byte) dataType;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="InfoFrameData" />.
        /// </summary>
        /// <param name="command">The operation to be done. Can only be used to change property information.</param>
        /// <param name="dataType">The type of information.</param>
        /// <param name="propertyInformation">The new property information to be set.</param>
        public InfoFrameData(
            InfoFrameCommand command,
            InfoFrameDataType dataType,
            InfoFrameProperty propertyInformation)
        {
            this = typeof(InfoFrameData).Instantiate<InfoFrameData>();
            _Size = (ushort) _Version.StructureSize;

            if (command != InfoFrameCommand.SetProperty)
            {
                throw new ArgumentOutOfRangeException(nameof(command));
            }

            _Command = (byte) command;
            _Type = (byte) dataType;
            _Property = propertyInformation;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="InfoFrameData" />.
        /// </summary>
        /// <param name="command">The operation to be done. Can only be used to change current or default audio information.</param>
        /// <param name="audioInformation">The new audio information to be set.</param>
        public InfoFrameData(InfoFrameCommand command, InfoFrameAudio audioInformation)
        {
            this = typeof(InfoFrameData).Instantiate<InfoFrameData>();
            _Size = (ushort) _Version.StructureSize;

            if (command != InfoFrameCommand.Set &&
                command != InfoFrameCommand.SetOverride)
            {
                throw new ArgumentOutOfRangeException(nameof(command));
            }

            _Command = (byte) command;
            _Type = (byte) InfoFrameDataType.AudioInformation;
            _Audio = audioInformation;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="InfoFrameData" />.
        /// </summary>
        /// <param name="command">The operation to be done. Can only be used to change current or default video information.</param>
        /// <param name="videoInformation">The new video information to be set.</param>
        public InfoFrameData(InfoFrameCommand command, InfoFrameVideo videoInformation)
        {
            this = typeof(InfoFrameData).Instantiate<InfoFrameData>();
            _Size = (ushort) _Version.StructureSize;

            if (command != InfoFrameCommand.Set &&
                command != InfoFrameCommand.SetOverride)
            {
                throw new ArgumentOutOfRangeException(nameof(command));
            }

            _Command = (byte) command;
            _Type = (byte) InfoFrameDataType.AuxiliaryVideoInformation;
            _Video = videoInformation;
        }

        /// <summary>
        ///     Gets the type of data contained in this instance
        /// </summary>
        public InfoFrameDataType Type
        {
            get => (InfoFrameDataType) _Type;
        }

        /// <summary>
        ///     Gets the operation type
        /// </summary>
        public InfoFrameCommand Command
        {
            get => (InfoFrameCommand) _Command;
        }

        /// <summary>
        ///     Gets the info-frame audio information if available; otherwise null
        /// </summary>
        public InfoFrameAudio? AudioInformation
        {
            get
            {
                if (Command == InfoFrameCommand.GetProperty || Command == InfoFrameCommand.SetProperty)
                {
                    return null;
                }

                if (Type == InfoFrameDataType.AudioInformation)
                {
                    return _Audio;
                }

                return null;
            }
        }

        /// <summary>
        ///     Gets the info-frame auxiliary video information (AVI) if available; otherwise null
        /// </summary>
        public InfoFrameVideo? AuxiliaryVideoInformation
        {
            get
            {
                if (Command == InfoFrameCommand.GetProperty || Command == InfoFrameCommand.SetProperty)
                {
                    return null;
                }

                if (Type == InfoFrameDataType.AuxiliaryVideoInformation)
                {
                    return _Video;
                }

                return null;
            }
        }

        /// <summary>
        ///     Gets the info-frame property information if available; otherwise null
        /// </summary>
        public InfoFrameProperty? PropertyInformation
        {
            get
            {
                if (Command != InfoFrameCommand.GetProperty && Command != InfoFrameCommand.SetProperty)
                {
                    return null;
                }

                return _Property;
            }
        }
    }
}