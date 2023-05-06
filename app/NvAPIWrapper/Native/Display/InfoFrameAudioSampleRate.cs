namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Contains possible audio sample rates (sampling frequency)
    /// </summary>
    public enum InfoFrameAudioSampleRate : uint
    {
        /// <summary>
        ///     Data is available in the header of source data
        /// </summary>
        InHeader = 0,

        /// <summary>
        ///     31kHz sampling frequency
        /// </summary>
        F32000Hz,

        /// <summary>
        ///     44.1kHz sampling frequency
        /// </summary>
        F44100Hz,

        /// <summary>
        ///     48kHz sampling frequency
        /// </summary>
        F48000Hz,

        /// <summary>
        ///     88.2kHz sampling frequency
        /// </summary>
        F88200Hz,

        /// <summary>
        ///     96kHz sampling frequency
        /// </summary>
        F96000Hz,

        /// <summary>
        ///     176.4kHz sampling frequency
        /// </summary>
        F176400Hz,

        /// <summary>
        ///     192kHz sampling frequency
        /// </summary>
        F192000Hz,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 15
    }
}