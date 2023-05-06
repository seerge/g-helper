using System;

namespace NvAPIWrapper.Mosaic
{
    /// <summary>
    ///     Holds maximum and minimum possible values for overlaps
    /// </summary>
    public struct OverlapLimit : IEquatable<OverlapLimit>
    {
        internal OverlapLimit(int minOverlapX, int maxOverlapX, int minOverlapY, int maxOverlapY)
        {
            MinimumHorizontalOverlap = minOverlapX;
            MaximumHorizontalOverlap = maxOverlapX;
            MinimumVerticalOverlap = minOverlapY;
            MaximumVerticalOverlap = maxOverlapY;
        }

        /// <summary>
        ///     Minimum value for horizontal overlap (OverlapX) or maximum value of horizontal gap
        /// </summary>
        public int MinimumHorizontalOverlap { get; }

        /// <summary>
        ///     Maximum value for horizontal overlap (OverlapX)
        /// </summary>
        public int MaximumHorizontalOverlap { get; }

        /// <summary>
        ///     Minimum value for vertical overlap (OverlapY) or maximum value of vertical gap
        /// </summary>
        public int MinimumVerticalOverlap { get; }

        /// <summary>
        ///     Maximum value for vertical overlap (OverlapY)
        /// </summary>
        public int MaximumVerticalOverlap { get; }

        /// <inheritdoc />
        public bool Equals(OverlapLimit other)
        {
            return MinimumHorizontalOverlap == other.MinimumHorizontalOverlap &&
                   MaximumHorizontalOverlap == other.MaximumHorizontalOverlap &&
                   MinimumVerticalOverlap == other.MinimumVerticalOverlap &&
                   MaximumVerticalOverlap == other.MaximumVerticalOverlap;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is OverlapLimit && Equals((OverlapLimit) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = MinimumHorizontalOverlap;
                hashCode = (hashCode * 397) ^ MaximumHorizontalOverlap;
                hashCode = (hashCode * 397) ^ MinimumVerticalOverlap;
                hashCode = (hashCode * 397) ^ MaximumVerticalOverlap;

                return hashCode;
            }
        }

        /// <summary>
        ///     Checks for equality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are equal, otherwise false</returns>
        public static bool operator ==(OverlapLimit left, OverlapLimit right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(OverlapLimit left, OverlapLimit right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return
                $"[{MinimumHorizontalOverlap}, {MaximumHorizontalOverlap}], [{MinimumVerticalOverlap}, {MaximumVerticalOverlap}]";
        }

        /// <summary>
        ///     Checks to see if the value falls in to the acceptable horizontal overlap range
        /// </summary>
        /// <param name="overlapX">The horizontal overlap value</param>
        /// <returns>true if the value falls into the range, otherwise false</returns>
        public bool IsInHorizontalRange(int overlapX)
        {
            return overlapX >= MinimumHorizontalOverlap && overlapX <= MaximumHorizontalOverlap;
        }

        /// <summary>
        ///     Checks to see if the value falls in to the acceptable vertical overlap range
        /// </summary>
        /// <param name="overlapY">The vertical overlap value</param>
        /// <returns>true if the value falls into the range, otherwise false</returns>
        public bool IsInVerticalRange(int overlapY)
        {
            return overlapY >= MinimumVerticalOverlap && overlapY <= MaximumVerticalOverlap;
        }

        /// <summary>
        ///     Checks to see if the overlap values fall in to the acceptable overlap ranges
        /// </summary>
        /// <param name="overlap">The overlap values</param>
        /// <returns>true if the values fall into the range, otherwise false</returns>
        public bool IsInRange(Overlap overlap)
        {
            return IsInHorizontalRange(overlap.HorizontalOverlap) && IsInVerticalRange(overlap.VerticalOverlap);
        }
    }
}