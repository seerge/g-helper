namespace NvAPIWrapper.Native.Interfaces.Display
{
    /// <summary>
    ///     Holds the Digital Vibrance Control information regarding the saturation level.
    /// </summary>
    public interface IDisplayDVCInfo
    {
        /// <summary>
        ///     Gets the current saturation level
        /// </summary>
        int CurrentLevel { get; }

        /// <summary>
        ///     Gets the default saturation level
        /// </summary>
        int DefaultLevel { get; }

        /// <summary>
        ///     Gets the maximum valid saturation level
        /// </summary>
        int MaximumLevel { get; }

        /// <summary>
        ///     Gets the minimum valid saturation level
        /// </summary>
        int MinimumLevel { get; }
    }
}