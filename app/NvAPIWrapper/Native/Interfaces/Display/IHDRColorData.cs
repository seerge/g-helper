using NvAPIWrapper.Native.Display;
using NvAPIWrapper.Native.Display.Structures;

namespace NvAPIWrapper.Native.Interfaces.Display
{
    /// <summary>
    ///     Contains information regarding HDR color data
    /// </summary>
    public interface IHDRColorData
    {
        /// <summary>
        ///     Gets the HDR color depth if available; otherwise null
        ///     For Dolby Vision only, should and will be ignored if HDR is on
        /// </summary>
        ColorDataDepth? ColorDepth { get; }

        /// <summary>
        ///     Gets the HDR color format if available; otherwise null
        /// </summary>
        ColorDataFormat? ColorFormat { get; }

        /// <summary>
        ///     Gets the HDR dynamic range if available; otherwise null
        /// </summary>
        ColorDataDynamicRange? DynamicRange { get; }

        /// <summary>
        ///     Gets the HDR mode
        /// </summary>
        ColorDataHDRMode HDRMode { get; }

        /// <summary>
        ///     Gets the color space coordinates
        /// </summary>
        MasteringDisplayColorData MasteringDisplayData { get; }
    }
}