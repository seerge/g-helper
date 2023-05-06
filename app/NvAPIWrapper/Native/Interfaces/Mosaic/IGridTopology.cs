using System.Collections.Generic;
using NvAPIWrapper.Native.Mosaic.Structures;

namespace NvAPIWrapper.Native.Interfaces.Mosaic
{
    /// <summary>
    ///     Interface for all GridTopology structures
    /// </summary>
    public interface IGridTopology
    {
        /// <summary>
        ///     Enable SLI acceleration on the primary display while in single-wide mode (For Immersive Gaming only). Will not be
        ///     persisted. Value undefined on get.
        /// </summary>
        bool AcceleratePrimaryDisplay { get; }

        /// <summary>
        ///     When enabling and doing the mode-set, do we switch to the bezel-corrected resolution
        /// </summary>
        bool ApplyWithBezelCorrectedResolution { get; }

        /// <summary>
        ///     Enable as Base Mosaic (Panoramic) instead of Mosaic SLI (for NVS and Quadro-boards only)
        /// </summary>
        bool BaseMosaicPanoramic { get; }

        /// <summary>
        ///     Number of columns
        /// </summary>
        int Columns { get; }

        /// <summary>
        ///     Topology displays; Displays are done as [(row * columns) + column]
        /// </summary>
        IEnumerable<IGridTopologyDisplay> Displays { get; }

        /// <summary>
        ///     Display settings
        /// </summary>
        DisplaySettingsV1 DisplaySettings { get; }

        /// <summary>
        ///     If necessary, reloading the driver is permitted (for Vista and above only). Will not be persisted. Value undefined
        ///     on get.
        /// </summary>
        bool DriverReloadAllowed { get; }

        /// <summary>
        ///     Enable as immersive gaming instead of Mosaic SLI (for Quadro-boards only)
        /// </summary>
        bool ImmersiveGaming { get; }

        /// <summary>
        ///     Number of rows
        /// </summary>
        int Rows { get; }
    }
}