using System;
using NvAPIWrapper.Display;
using NvAPIWrapper.Native.Display;
using NvAPIWrapper.Native.Interfaces.Mosaic;
using NvAPIWrapper.Native.Mosaic;
using NvAPIWrapper.Native.Mosaic.Structures;

namespace NvAPIWrapper.Mosaic
{
    /// <summary>
    ///     Represents a display in a mosaic grid topology
    /// </summary>
    public class GridTopologyDisplay : IEquatable<GridTopologyDisplay>
    {
        /// <summary>
        ///     Creates a mew GridTopologyDisplay
        /// </summary>
        /// <param name="displayId">Corresponding display identification</param>
        /// <param name="overlap">The overlap values</param>
        /// <param name="rotation">The display rotation</param>
        /// <param name="cloneGroup">The display clone group</param>
        /// <param name="pixelShiftType">The display pixel shift type</param>
        public GridTopologyDisplay(
            uint displayId,
            Overlap overlap = default(Overlap),
            Rotate rotation = Rotate.Degree0,
            uint cloneGroup = 0,
            PixelShiftType pixelShiftType = PixelShiftType.NoPixelShift)
            : this(new DisplayDevice(displayId), overlap, rotation, cloneGroup, pixelShiftType)
        {
        }

        /// <summary>
        ///     Creates a mew GridTopologyDisplay
        /// </summary>
        /// <param name="display">Corresponding display device</param>
        /// <param name="overlap">The overlap values</param>
        /// <param name="rotation">The display rotation</param>
        /// <param name="cloneGroup">The display clone group</param>
        /// <param name="pixelShiftType">The display pixel shift type</param>
        public GridTopologyDisplay(
            DisplayDevice display,
            Overlap overlap = default(Overlap),
            Rotate rotation = Rotate.Degree0,
            uint cloneGroup = 0,
            PixelShiftType pixelShiftType = PixelShiftType.NoPixelShift)
        {
            DisplayDevice = display;
            Overlap = overlap;
            Rotation = rotation;
            CloneGroup = cloneGroup;
            PixelShiftType = pixelShiftType;
        }

        /// <summary>
        ///     Creates a mew GridTopologyDisplay
        /// </summary>
        /// <param name="gridTopologyDisplay">IGridTopologyDisplay implamented object</param>
        public GridTopologyDisplay(IGridTopologyDisplay gridTopologyDisplay)
            : this(
                new DisplayDevice(gridTopologyDisplay.DisplayId),
                new Overlap(gridTopologyDisplay.OverlapX, gridTopologyDisplay.OverlapY),
                gridTopologyDisplay.Rotation, gridTopologyDisplay.CloneGroup, gridTopologyDisplay.PixelShiftType)
        {
        }

        /// <summary>
        ///     Gets the clone group identification; Reserved, must be 0
        /// </summary>
        public uint CloneGroup { get; set; }

        /// <summary>
        ///     Gets the corresponding DisplayDevice
        /// </summary>
        public DisplayDevice DisplayDevice { get; }

        /// <summary>
        ///     Gets the overlap values
        /// </summary>
        public Overlap Overlap { get; set; }

        /// <summary>
        ///     Gets the type of display pixel shift
        /// </summary>
        public PixelShiftType PixelShiftType { get; set; }

        /// <summary>
        ///     Gets the rotation of the display
        /// </summary>
        public Rotate Rotation { get; set; }

        /// <inheritdoc />
        public bool Equals(GridTopologyDisplay other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return DisplayDevice.Equals(other.DisplayDevice) &&
                   Overlap.Equals(other.Overlap) &&
                   Rotation == other.Rotation &&
                   CloneGroup == other.CloneGroup &&
                   PixelShiftType == other.PixelShiftType;
        }

        /// <summary>
        ///     Checks for equality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are equal, otherwise false</returns>
        public static bool operator ==(GridTopologyDisplay left, GridTopologyDisplay right)
        {
            return right?.Equals(left) ?? ReferenceEquals(left, null);
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(GridTopologyDisplay left, GridTopologyDisplay right)
        {
            return !(left == right);
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

            return Equals((GridTopologyDisplay) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = DisplayDevice?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ Overlap.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) Rotation;
                hashCode = (hashCode * 397) ^ (int) CloneGroup;
                hashCode = (hashCode * 397) ^ (int) PixelShiftType;

                return hashCode;
            }
        }

        /// <summary>
        ///     Creates and fills a GridTopologyDisplayV1 object
        /// </summary>
        /// <returns>The newly created GridTopologyDisplayV1 object</returns>
        public GridTopologyDisplayV1 GetGridTopologyDisplayV1()
        {
            return new GridTopologyDisplayV1(DisplayDevice.DisplayId, Overlap.HorizontalOverlap,
                Overlap.VerticalOverlap,
                Rotation, CloneGroup);
        }

        /// <summary>
        ///     Creates and fills a GridTopologyDisplayV2 object
        /// </summary>
        /// <returns>The newly created GridTopologyDisplayV2 object</returns>
        public GridTopologyDisplayV2 GetGridTopologyDisplayV2()
        {
            return new GridTopologyDisplayV2(DisplayDevice.DisplayId, Overlap.HorizontalOverlap,
                Overlap.VerticalOverlap,
                Rotation, CloneGroup, PixelShiftType);
        }
    }
}