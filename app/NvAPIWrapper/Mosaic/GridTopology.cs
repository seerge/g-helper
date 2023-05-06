using System;
using System.Linq;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.Display.Structures;
using NvAPIWrapper.Native.Exceptions;
using NvAPIWrapper.Native.General;
using NvAPIWrapper.Native.Interfaces.Mosaic;
using NvAPIWrapper.Native.Mosaic;
using NvAPIWrapper.Native.Mosaic.Structures;

namespace NvAPIWrapper.Mosaic
{
    /// <summary>
    ///     Represents a mosaic grid topology
    /// </summary>
    public class GridTopology : IEquatable<GridTopology>
    {
        /// <summary>
        ///     Creates a new GridTopology
        /// </summary>
        /// <param name="rows">Mosaic rows</param>
        /// <param name="columns">Mosaic columns</param>
        /// <param name="displays">Topology displays</param>
        public GridTopology(int rows, int columns, GridTopologyDisplay[] displays)
        {
            SetDisplays(rows, columns, displays);
            var possibleDisplaySettings = GetPossibleDisplaySettings();

            if (possibleDisplaySettings.Any())
            {
                SetDisplaySettings(
                    possibleDisplaySettings.OrderByDescending(
                            settings => (long) settings.Width *
                                        settings.Height *
                                        settings.BitsPerPixel *
                                        settings.Frequency)
                        .First());
            }
        }

        /// <summary>
        ///     Creates a new GridTopology
        /// </summary>
        /// <param name="gridTopology">A IGridTopology implamented object</param>
        public GridTopology(IGridTopology gridTopology)
        {
            Rows = gridTopology.Rows;
            Columns = gridTopology.Columns;
            Displays = gridTopology.Displays.Select(display => new GridTopologyDisplay(display)).ToArray();
            SetDisplaySettings(gridTopology.DisplaySettings);
            ApplyWithBezelCorrectedResolution = gridTopology.ApplyWithBezelCorrectedResolution;
            ImmersiveGaming = gridTopology.ImmersiveGaming;
            BaseMosaicPanoramic = gridTopology.BaseMosaicPanoramic;
            DriverReloadAllowed = gridTopology.DriverReloadAllowed;
            AcceleratePrimaryDisplay = gridTopology.AcceleratePrimaryDisplay;
        }

        /// <summary>
        ///     Gets or sets a boolean value enabling SLI acceleration on the primary display while in single-wide mode (For
        ///     Immersive Gaming only).
        /// </summary>
        public bool AcceleratePrimaryDisplay { get; set; }

        /// <summary>
        ///     Gets or sets a boolean value forcing to the bezel-corrected resolution when enabling and doing the modeset
        /// </summary>
        public bool ApplyWithBezelCorrectedResolution { get; set; }

        /// <summary>
        ///     Gets or sets a boolean value enabling the Base Mosaic (Panoramic) instead of Mosaic SLI (for NVS and Quadro-boards
        ///     only)
        /// </summary>
        public bool BaseMosaicPanoramic { get; set; }

        /// <summary>
        ///     Gets the mosaic columns
        /// </summary>
        public int Columns { get; private set; }

        /// <summary>
        ///     Gets topology displays
        /// </summary>
        public GridTopologyDisplay[] Displays { get; private set; }

        /// <summary>
        ///     Gets or sets a boolean value allowing the API to, if necessary, realod the driver (for Vista and above only). Will
        ///     not be persisted. Value undefined on get.
        /// </summary>
        public bool DriverReloadAllowed { get; set; }

        /// <summary>
        ///     Gets the topology Frequency
        /// </summary>
        public int Frequency { get; private set; }

        /// <summary>
        ///     Gets or sets a boolean value enabling as immersive gaming instead of Mosaic SLI (for Quadro-boards only)
        /// </summary>
        public bool ImmersiveGaming { get; set; }

        /// <summary>
        ///     Gets the topology resolution
        /// </summary>
        public Resolution Resolution { get; private set; }

        /// <summary>
        ///     Gets the mosaic rows
        /// </summary>
        public int Rows { get; private set; }

        /// <inheritdoc />
        public bool Equals(GridTopology other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Resolution.Equals(other.Resolution) &&
                   Frequency == other.Frequency &&
                   Rows == other.Rows &&
                   Columns == other.Columns &&
                   Displays.SequenceEqual(other.Displays) &&
                   ApplyWithBezelCorrectedResolution == other.ApplyWithBezelCorrectedResolution &&
                   ImmersiveGaming == other.ImmersiveGaming &&
                   BaseMosaicPanoramic == other.BaseMosaicPanoramic &&
                   AcceleratePrimaryDisplay == other.AcceleratePrimaryDisplay;
        }

        /// <summary>
        ///     Retrieves a list of currently active mosaic grid topologies
        /// </summary>
        /// <returns>An array of GridTopology objects</returns>
        public static GridTopology[] GetGridTopologies()
        {
            return MosaicApi.EnumDisplayGrids().Select(topology => new GridTopology(topology)).ToArray();
        }

        /// <summary>
        ///     Checks for equality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are equal, otherwise false</returns>
        public static bool operator ==(GridTopology left, GridTopology right)
        {
            return right?.Equals(left) ?? ReferenceEquals(left, null);
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(GridTopology left, GridTopology right)
        {
            return !(left == right);
        }

        /// <summary>
        ///     Applies the requested grid topologies
        /// </summary>
        /// <param name="grids">An array of grid topologies to apply</param>
        /// <param name="flags">SetDisplayTopologyFlag flag</param>
        public static void SetGridTopologies(
            GridTopology[] grids,
            SetDisplayTopologyFlag flags = SetDisplayTopologyFlag.NoFlag)
        {
            var gridTopologyV2 = grids.Select(grid => grid.GetGridTopologyV2()).Cast<IGridTopology>().ToArray();

            try
            {
                MosaicApi.SetDisplayGrids(gridTopologyV2, flags);
            }
            catch (NVIDIAApiException ex)
            {
                if (ex.Status != Status.IncompatibleStructureVersion)
                {
                    throw;
                }
            }
            catch (NVIDIANotSupportedException)
            {
                // ignore
            }

            var gridTopologyV1 = grids.Select(grid => grid.GetGridTopologyV1()).Cast<IGridTopology>().ToArray();
            MosaicApi.SetDisplayGrids(gridTopologyV1, flags);
        }

        /// <summary>
        ///     Validates a list of grid topologies
        /// </summary>
        /// <param name="grids">An array of grid topologies to validate</param>
        /// <param name="flags">SetDisplayTopologyFlag flag</param>
        /// <returns>An array of DisplayTopologyStatus object containing the result of the validation</returns>
        public static DisplayTopologyStatus[] ValidateGridTopologies(
            GridTopology[] grids,
            SetDisplayTopologyFlag flags = SetDisplayTopologyFlag.AllowInvalid)
        {
            var gridTopologyV2 = grids.Select(grid => grid.GetGridTopologyV2()).Cast<IGridTopology>().ToArray();

            try
            {
                return MosaicApi.ValidateDisplayGrids(gridTopologyV2, flags);
            }
            catch (NVIDIAApiException ex)
            {
                if (ex.Status != Status.IncompatibleStructureVersion)
                {
                    throw;
                }
            }
            catch (NVIDIANotSupportedException)
            {
                // ignore
            }

            var gridTopologyV1 = grids.Select(grid => grid.GetGridTopologyV1()).Cast<IGridTopology>().ToArray();

            return MosaicApi.ValidateDisplayGrids(gridTopologyV1, flags);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((GridTopology) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Resolution.GetHashCode();
                hashCode = (hashCode * 397) ^ Frequency;
                hashCode = (hashCode * 397) ^ Rows;
                hashCode = (hashCode * 397) ^ Columns;
                hashCode = (hashCode * 397) ^ (Displays?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ ApplyWithBezelCorrectedResolution.GetHashCode();
                hashCode = (hashCode * 397) ^ ImmersiveGaming.GetHashCode();
                hashCode = (hashCode * 397) ^ BaseMosaicPanoramic.GetHashCode();
                hashCode = (hashCode * 397) ^ AcceleratePrimaryDisplay.GetHashCode();

                return hashCode;
            }
        }

        /// <summary>
        ///     Creates and fills a DisplaySettingsV1 object
        /// </summary>
        /// <returns>The newly created DisplaySettingsV1 object</returns>
        public DisplaySettingsV1 GetDisplaySettingsV1()
        {
            return new DisplaySettingsV1(Resolution.Width, Resolution.Height, Resolution.ColorDepth, Frequency);
        }

        /// <summary>
        ///     Creates and fills a GridTopologyV1 object
        /// </summary>
        /// <returns>The newly created GridTopologyV1 object</returns>
        public GridTopologyV1 GetGridTopologyV1()
        {
            var displaySettings = GetDisplaySettingsV1();

            return new GridTopologyV1(Rows, Columns,
                Displays.Select(display => display.GetGridTopologyDisplayV1()).ToArray(), displaySettings,
                ApplyWithBezelCorrectedResolution, ImmersiveGaming, BaseMosaicPanoramic, DriverReloadAllowed,
                AcceleratePrimaryDisplay);
        }

        /// <summary>
        ///     Creates and fills a GridTopologyV2 object
        /// </summary>
        /// <returns>The newly created GridTopologyV2 object</returns>
        public GridTopologyV2 GetGridTopologyV2()
        {
            var displaySettings = GetDisplaySettingsV1();

            return new GridTopologyV2(Rows, Columns,
                Displays.Select(display => display.GetGridTopologyDisplayV2()).ToArray(), displaySettings,
                ApplyWithBezelCorrectedResolution, ImmersiveGaming, BaseMosaicPanoramic, DriverReloadAllowed,
                AcceleratePrimaryDisplay,
                Displays.Any(display => display.PixelShiftType != PixelShiftType.NoPixelShift));
        }

        /// <summary>
        ///     Retrieves a list of possible display settings for this topology
        /// </summary>
        /// <returns>An array of IDisplaySettings implamented objects</returns>
        public IDisplaySettings[] GetPossibleDisplaySettings()
        {
            var gridTopologyV2 = GetGridTopologyV2();

            try
            {
                return MosaicApi.EnumDisplayModes(gridTopologyV2);
            }
            catch (NVIDIAApiException ex)
            {
                if (ex.Status != Status.IncompatibleStructureVersion)
                {
                    throw;
                }
            }
            catch (NVIDIANotSupportedException)
            {
                // ignore
            }

            var gridTopologyV1 = GetGridTopologyV1();

            return MosaicApi.EnumDisplayModes(gridTopologyV1);
        }

        /// <summary>
        ///     Changes topology arrangement and displays
        /// </summary>
        /// <param name="rows">Mosaic rows</param>
        /// <param name="columns">Mosaic columns</param>
        /// <param name="displays">Topology displays</param>
        /// <exception cref="ArgumentOutOfRangeException">Invalid display arrangement.</exception>
        /// <exception cref="ArgumentException">Number of displays should match the arrangement.</exception>
        public void SetDisplays(int rows, int columns, GridTopologyDisplay[] displays)
        {
            if (rows * columns <= 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(rows)}, {nameof(columns)}",
                    "Invalid display arrangement.");
            }

            if (displays.Length != rows * columns)
            {
                throw new ArgumentException("Number of displays should match the arrangement.", nameof(displays));
            }

            Rows = rows;
            Columns = columns;
            Displays = displays;
        }

        /// <summary>
        ///     Changes display settings for the topology
        /// </summary>
        /// <param name="displaySettings">Display settings to use</param>
        public void SetDisplaySettings(IDisplaySettings displaySettings)
        {
            Resolution = new Resolution(displaySettings.Width, displaySettings.Height, displaySettings.BitsPerPixel);
            Frequency = displaySettings.Frequency;
        }
    }
}