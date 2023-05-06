using System;
using System.Linq;
using NvAPIWrapper.GPU;
using NvAPIWrapper.Native.Mosaic;

namespace NvAPIWrapper.Mosaic
{
    /// <summary>
    ///     Holds extra information about a topology
    /// </summary>
    public class TopologyDetails : IEquatable<TopologyDetails>
    {
        internal TopologyDetails(Native.Mosaic.Structures.TopologyDetails details)
        {
            Rows = details.Rows;
            Columns = details.Columns;
            LogicalGPU = !details.LogicalGPUHandle.IsNull ? new LogicalGPU(details.LogicalGPUHandle) : null;
            ValidityFlags = details.ValidityFlags;
            Displays =
                details.Layout.Select(cells => cells.Select(cell => new TopologyDisplay(cell)).ToArray()).ToArray();
        }

        /// <summary>
        ///     Gets the number of columns in the topology
        /// </summary>
        public int Columns { get; }

        /// <summary>
        ///     Gets the list of topology displays
        /// </summary>
        public TopologyDisplay[][] Displays { get; }

        /// <summary>
        ///     Gets the logical GPU in charge of controling the topology
        /// </summary>
        public LogicalGPU LogicalGPU { get; }

        /// <summary>
        ///     Gets the number of rows in the topology
        /// </summary>
        public int Rows { get; }

        /// <summary>
        ///     Gets the validity status of this topology
        /// </summary>
        public TopologyValidity ValidityFlags { get; }

        /// <inheritdoc />
        public bool Equals(TopologyDetails other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Rows == other.Rows &&
                   Columns == other.Columns &&
                   LogicalGPU.Equals(other.LogicalGPU) &&
                   Displays.SequenceEqual(other.Displays);
        }

        /// <summary>
        ///     Checks for equality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are equal, otherwise false</returns>
        public static bool operator ==(TopologyDetails left, TopologyDetails right)
        {
            return right?.Equals(left) ?? ReferenceEquals(left, null);
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(TopologyDetails left, TopologyDetails right)
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

            return Equals((TopologyDetails) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Rows;
                hashCode = (hashCode * 397) ^ Columns;
                hashCode = (hashCode * 397) ^ (LogicalGPU != null ? LogicalGPU.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Displays?.GetHashCode() ?? 0);

                return hashCode;
            }
        }
    }
}