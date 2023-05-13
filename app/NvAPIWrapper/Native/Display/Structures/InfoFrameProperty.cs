using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Helpers;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <summary>
    ///     Contains info-frame property information
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 8)]
    public struct InfoFrameProperty
    {
        [FieldOffset(0)] private readonly uint _Word;

        /// <summary>
        ///     Creates an instance of <see cref="InfoFrameProperty" />.
        /// </summary>
        /// <param name="mode">The info-frame operation mode</param>
        /// <param name="isBlackListed">A value indicating if this display (monitor) is blacklisted</param>
        public InfoFrameProperty(InfoFramePropertyMode mode, InfoFrameBoolean isBlackListed)
        {
            _Word = 0u
                .SetBits(0, 4, (uint) mode)
                .SetBits(4, 2, (uint) isBlackListed);
        }

        /// <summary>
        ///     Gets the info-frame operation mode
        /// </summary>
        public InfoFramePropertyMode Mode
        {
            get => (InfoFramePropertyMode) _Word.GetBits(0, 4);
        }

        /// <summary>
        ///     Gets a value indicating if this display (monitor) is blacklisted
        /// </summary>
        public InfoFrameBoolean IsBlackListed
        {
            get => (InfoFrameBoolean) _Word.GetBits(4, 2);
        }

        /// <summary>
        ///     Gets the info-frame version
        /// </summary>
        public byte Version
        {
            get => (byte) _Word.GetBits(16, 8);
        }

        /// <summary>
        ///     Gets the info-frame length
        /// </summary>
        public byte Length
        {
            get => (byte) _Word.GetBits(24, 8);
        }
    }
}