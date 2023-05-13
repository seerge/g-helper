using NvAPIWrapper.Native.Display;

namespace NvAPIWrapper.Native.Interfaces.Display
{
    /// <summary>
    ///     Contains data corresponding to color information
    /// </summary>
    public interface IColorData
    {
        /// <summary>
        ///     Gets the color data color depth
        /// </summary>
        ColorDataDepth? ColorDepth { get; }

        /// <summary>
        ///     Gets the color data dynamic range
        /// </summary>
        ColorDataDynamicRange? DynamicRange { get; }

        /// <summary>
        ///     Gets the color data color format
        /// </summary>
        ColorDataFormat ColorFormat { get; }

        /// <summary>
        ///     Gets the color data color space
        /// </summary>
        ColorDataColorimetry Colorimetry { get; }

        /// <summary>
        ///     Gets the color data selection policy
        /// </summary>
        ColorDataSelectionPolicy? SelectionPolicy { get; }

        /// <summary>
        ///     Gets the color data desktop color depth
        /// </summary>
        ColorDataDesktopDepth? DesktopColorDepth { get; }
    }
}