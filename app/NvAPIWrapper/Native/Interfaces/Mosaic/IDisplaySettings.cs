namespace NvAPIWrapper.Native.Interfaces.Mosaic
{
    /// <summary>
    ///     Interface for all DisplaySettings structures
    /// </summary>
    public interface IDisplaySettings
    {
        /// <summary>
        ///     Bits per pixel
        /// </summary>
        int BitsPerPixel { get; }

        /// <summary>
        ///     Display frequency
        /// </summary>
        int Frequency { get; }

        /// <summary>
        ///     Display frequency in x1k
        /// </summary>
        uint FrequencyInMillihertz { get; }

        /// <summary>
        ///     Per-display height
        /// </summary>
        int Height { get; }

        /// <summary>
        ///     Per-display width
        /// </summary>
        int Width { get; }
    }
}