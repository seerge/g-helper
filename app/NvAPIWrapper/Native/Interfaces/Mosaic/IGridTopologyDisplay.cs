using NvAPIWrapper.Native.Display;
using NvAPIWrapper.Native.Mosaic;

namespace NvAPIWrapper.Native.Interfaces.Mosaic
{
    /// <summary>
    ///     Interface for all GridTopologyDisplay structures
    /// </summary>
    public interface IGridTopologyDisplay
    {
        /// <summary>
        ///     Gets the clone group identification; Reserved, must be 0
        /// </summary>
        uint CloneGroup { get; }

        /// <summary>
        ///     Gets the display identification
        /// </summary>
        uint DisplayId { get; }

        /// <summary>
        ///     Gets the horizontal overlap (+overlap, -gap)
        /// </summary>
        int OverlapX { get; }

        /// <summary>
        ///     Gets the vertical overlap (+overlap, -gap)
        /// </summary>
        int OverlapY { get; }


        /// <summary>
        ///     Gets the type of display pixel shift
        /// </summary>
        PixelShiftType PixelShiftType { get; }

        /// <summary>
        ///     Gets the rotation of display
        /// </summary>
        Rotate Rotation { get; }
    }
}