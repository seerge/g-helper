using System;
using NvAPIWrapper.GPU;

namespace NvAPIWrapper.Mosaic
{
    /// <summary>
    ///     Holds information about a display in a topology
    /// </summary>
    public class TopologyDisplay : IEquatable<TopologyDisplay>
    {
        internal TopologyDisplay(Native.Mosaic.Structures.TopologyDetails.LayoutCell layoutCell)
        {
            PhysicalGPU = !layoutCell.PhysicalGPUHandle.IsNull ? new PhysicalGPU(layoutCell.PhysicalGPUHandle) : null;
            Output = new GPUOutput(layoutCell.DisplayOutputId, PhysicalGPU);
            Overlap = new Overlap(layoutCell.OverlapX, layoutCell.OverlapY);
        }

        /// <summary>
        ///     Gets the GPU output used for this display
        /// </summary>
        public GPUOutput Output { get; }

        /// <summary>
        ///     Gets the display overlap values
        /// </summary>
        public Overlap Overlap { get; }

        /// <summary>
        ///     Gets the corresponding physical GPU of this display
        /// </summary>
        public PhysicalGPU PhysicalGPU { get; }

        /// <inheritdoc />
        public bool Equals(TopologyDisplay other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return PhysicalGPU.Equals(other.PhysicalGPU) &&
                   Output.Equals(other.Output) &&
                   Overlap.Equals(other.Overlap);
        }

        /// <summary>
        ///     Checks for equality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are equal, otherwise false</returns>
        public static bool operator ==(TopologyDisplay left, TopologyDisplay right)
        {
            return right?.Equals(left) ?? ReferenceEquals(left, null);
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(TopologyDisplay left, TopologyDisplay right)
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

            return Equals((TopologyDisplay) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = PhysicalGPU != null ? PhysicalGPU.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (Output != null ? Output.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Overlap.GetHashCode();

                return hashCode;
            }
        }
    }
}