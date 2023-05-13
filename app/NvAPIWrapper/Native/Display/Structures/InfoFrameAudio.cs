using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Helpers;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <summary>
    ///     Contains info-frame audio information
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 8)]
    public struct InfoFrameAudio
    {
        [FieldOffset(0)] private readonly uint _WordAt0;

        [FieldOffset(4)] private readonly uint _WordAt4;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        [FieldOffset(8)] private readonly uint _WordAt8;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        [FieldOffset(12)] private readonly byte _ByteAt12;

        /// <summary>
        ///     Creates an instance of <see cref="InfoFrameAudio" />.
        /// </summary>
        /// <param name="codec">The audio coding type (codec)</param>
        /// <param name="codecExtension">The audio codec from codec extension</param>
        /// <param name="sampleSize">The audio sample size (depth)</param>
        /// <param name="sampleRate">The audio sample rate (sampling frequency)</param>
        /// <param name="channelCount">The number of audio channels</param>
        /// <param name="channelAllocation">The audio channel allocation (speaker placements)</param>
        /// <param name="isDownMixProhibited">A value indicating if down-mix is prohibited</param>
        /// <param name="lfePlaybackLevel">The Low Frequency Effects playback level value</param>
        /// <param name="levelShift">The audio level shift value</param>
        public InfoFrameAudio(
            InfoFrameAudioCodec codec,
            InfoFrameAudioExtendedCodec codecExtension,
            InfoFrameAudioSampleSize sampleSize,
            InfoFrameAudioSampleRate sampleRate,
            InfoFrameAudioChannelCount channelCount,
            InfoFrameAudioChannelAllocation channelAllocation,
            InfoFrameBoolean isDownMixProhibited,
            InfoFrameAudioLFEPlaybackLevel lfePlaybackLevel,
            InfoFrameAudioLevelShift levelShift
        )
        {
            _WordAt0 = 0u
                .SetBits(0, 5, (uint) codec)
                .SetBits(5, 6, (uint) codecExtension)
                .SetBits(11, 3, (uint) sampleSize)
                .SetBits(14, 4, (uint) sampleRate)
                .SetBits(18, 4, (uint) channelCount)
                .SetBits(22, 9, (uint) channelAllocation);
            _WordAt4 = 0u
                .SetBits(0, 2, (uint) isDownMixProhibited)
                .SetBits(2, 3, (uint) lfePlaybackLevel)
                .SetBits(5, 5, (uint) levelShift);
            _WordAt8 = 0;
            _ByteAt12 = 0;
        }

        /// <summary>
        ///     Gets the audio coding type (codec)
        /// </summary>
        public InfoFrameAudioCodec Codec
        {
            get => (InfoFrameAudioCodec) _WordAt0.GetBits(0, 5);
        }

        /// <summary>
        ///     Gets the audio codec from codec extension; only valid when
        ///     <see cref="Codec" /> == <see cref="InfoFrameAudioCodec.UseExtendedCodecType" />
        /// </summary>
        public InfoFrameAudioExtendedCodec? ExtendedCodec
        {
            get
            {
                if (Codec != InfoFrameAudioCodec.UseExtendedCodecType)
                {
                    return null;
                }

                return (InfoFrameAudioExtendedCodec) _WordAt0.GetBits(5, 6);
            }
        }

        /// <summary>
        ///     Gets the audio sample size (depth)
        /// </summary>
        public InfoFrameAudioSampleSize SampleSize
        {
            get => (InfoFrameAudioSampleSize) _WordAt0.GetBits(11, 3);
        }

        /// <summary>
        ///     Gets the audio sample rate (sampling frequency)
        /// </summary>
        public InfoFrameAudioSampleRate SampleRate
        {
            get => (InfoFrameAudioSampleRate) _WordAt0.GetBits(14, 4);
        }

        /// <summary>
        ///     Gets the number of audio channels
        /// </summary>
        public InfoFrameAudioChannelCount ChannelCount
        {
            get => (InfoFrameAudioChannelCount) _WordAt0.GetBits(18, 4);
        }

        /// <summary>
        ///     Gets the audio channel allocation (speaker placements)
        /// </summary>
        public InfoFrameAudioChannelAllocation ChannelAllocation
        {
            get => (InfoFrameAudioChannelAllocation) _WordAt0.GetBits(22, 9);
        }

        /// <summary>
        ///     Gets a value indicating if down-mix is prohibited
        /// </summary>
        public InfoFrameBoolean IsDownMixProhibited
        {
            get => (InfoFrameBoolean) _WordAt4.GetBits(0, 2);
        }

        /// <summary>
        ///     Gets the Low Frequency Effects playback level value
        /// </summary>
        public InfoFrameAudioLFEPlaybackLevel LFEPlaybackLevel
        {
            get => (InfoFrameAudioLFEPlaybackLevel) _WordAt4.GetBits(2, 3);
        }

        /// <summary>
        ///     Gets the audio level shift value
        /// </summary>
        public InfoFrameAudioLevelShift LevelShift
        {
            get => (InfoFrameAudioLevelShift) _WordAt4.GetBits(5, 5);
        }
    }
}