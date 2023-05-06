using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;
using NvAPIWrapper.Native.Interfaces.Mosaic;

namespace NvAPIWrapper.Native.Mosaic.Structures
{
    /// <summary>
    ///     Holds information about a grid topology
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(2)]
    public struct GridTopologyV2 : IGridTopology, IInitializable, IEquatable<GridTopologyV2>
    {
        /// <summary>
        ///     Maximum number of displays in a topology
        /// </summary>
        public const int MaxDisplays = GridTopologyV1.MaxDisplays;

        internal StructureVersion _Version;
        internal readonly uint _Rows;
        internal readonly uint _Columns;
        internal readonly uint _DisplayCount;
        internal uint _RawReserved;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxDisplays)]
        internal readonly GridTopologyDisplayV2[] _Displays;

        internal readonly DisplaySettingsV1 _DisplaySettings;

        /// <summary>
        ///     Creates a new GridTopologyV2
        /// </summary>
        /// <param name="rows">Number of rows</param>
        /// <param name="columns">Number of columns</param>
        /// <param name="displays">Topology displays; Displays are done as [(row * columns) + column]</param>
        /// <param name="displaySettings">Display settings</param>
        /// <param name="applyWithBezelCorrectedResolution">
        ///     When enabling and doing the mode-set, do we switch to the
        ///     bezel-corrected resolution
        /// </param>
        /// <param name="immersiveGaming">Enable as immersive gaming instead of Mosaic SLI (for Quadro-boards only)</param>
        /// <param name="baseMosaicPanoramic">
        ///     Enable as Base Mosaic (Panoramic) instead of Mosaic SLI (for NVS and Quadro-boards
        ///     only)
        /// </param>
        /// <param name="driverReloadAllowed">
        ///     If necessary, reloading the driver is permitted (for Vista and above only). Will not
        ///     be persisted.
        /// </param>
        /// <param name="acceleratePrimaryDisplay">
        ///     Enable SLI acceleration on the primary display while in single-wide mode (For
        ///     Immersive Gaming only). Will not be persisted.
        /// </param>
        /// <param name="pixelShift">Enable Pixel shift</param>
        /// <exception cref="ArgumentOutOfRangeException">Total number of topology displays is below or equal to zero</exception>
        /// <exception cref="ArgumentException">Number of displays doesn't match the arrangement</exception>
        // ReSharper disable once TooManyDependencies
        public GridTopologyV2(
            int rows,
            int columns,
            GridTopologyDisplayV2[] displays,
            DisplaySettingsV1 displaySettings,
            bool applyWithBezelCorrectedResolution,
            bool immersiveGaming,
            bool baseMosaicPanoramic,
            bool driverReloadAllowed,
            bool acceleratePrimaryDisplay,
            bool pixelShift)
        {
            if (rows * columns <= 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(rows)}, {nameof(columns)}",
                    "Invalid display arrangement.");
            }

            if (displays.Length > MaxDisplays)
            {
                throw new ArgumentException("Too many displays.");
            }

            if (displays.Length != rows * columns)
            {
                throw new ArgumentException("Number of displays should match the arrangement.", nameof(displays));
            }

            this = typeof(GridTopologyV2).Instantiate<GridTopologyV2>();
            _Rows = (uint) rows;
            _Columns = (uint) columns;
            _DisplayCount = (uint) displays.Length;
            _Displays = displays;
            _DisplaySettings = displaySettings;
            ApplyWithBezelCorrectedResolution = applyWithBezelCorrectedResolution;
            ImmersiveGaming = immersiveGaming;
            BaseMosaicPanoramic = baseMosaicPanoramic;
            DriverReloadAllowed = driverReloadAllowed;
            AcceleratePrimaryDisplay = acceleratePrimaryDisplay;
            PixelShift = pixelShift;
            Array.Resize(ref _Displays, MaxDisplays);
        }

        /// <inheritdoc />
        public bool Equals(GridTopologyV2 other)
        {
            return _Rows == other._Rows &&
                   _Columns == other._Columns &&
                   _DisplayCount == other._DisplayCount &&
                   _RawReserved == other._RawReserved &&
                   _Displays.SequenceEqual(other._Displays) &&
                   _DisplaySettings.Equals(other._DisplaySettings);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is GridTopologyV2 v2 && Equals(v2);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) _Rows;
                hashCode = (hashCode * 397) ^ (int) _Columns;
                hashCode = (hashCode * 397) ^ (int) _DisplayCount;
                // ReSharper disable once NonReadonlyMemberInGetHashCode
                hashCode = (hashCode * 397) ^ (int) _RawReserved;
                hashCode = (hashCode * 397) ^ (_Displays?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ _DisplaySettings.GetHashCode();

                return hashCode;
            }
        }

        /// <inheritdoc />
        public int Rows
        {
            get => (int) _Rows;
        }

        /// <inheritdoc />
        public int Columns
        {
            get => (int) _Columns;
        }

        /// <inheritdoc />
        public IEnumerable<IGridTopologyDisplay> Displays
        {
            get => _Displays.Take((int) _DisplayCount).Cast<IGridTopologyDisplay>();
        }

        /// <inheritdoc />
        public DisplaySettingsV1 DisplaySettings
        {
            get => _DisplaySettings;
        }

        /// <inheritdoc />
        public bool ApplyWithBezelCorrectedResolution
        {
            get => _RawReserved.GetBit(0);
            private set => _RawReserved = _RawReserved.SetBit(0, value);
        }

        /// <inheritdoc />
        public bool ImmersiveGaming
        {
            get => _RawReserved.GetBit(1);
            private set => _RawReserved = _RawReserved.SetBit(1, value);
        }

        /// <inheritdoc />
        public bool BaseMosaicPanoramic
        {
            get => _RawReserved.GetBit(2);
            private set => _RawReserved = _RawReserved.SetBit(2, value);
        }

        /// <inheritdoc />
        public bool DriverReloadAllowed
        {
            get => _RawReserved.GetBit(3);
            private set => _RawReserved = _RawReserved.SetBit(3, value);
        }

        /// <inheritdoc />
        public bool AcceleratePrimaryDisplay
        {
            get => _RawReserved.GetBit(4);
            private set => _RawReserved = _RawReserved.SetBit(4, value);
        }

        /// <summary>
        ///     Enable Pixel shift
        /// </summary>
        public bool PixelShift
        {
            get => _RawReserved.GetBit(5);
            private set => _RawReserved = _RawReserved.SetBit(5, value);
        }
    }
}