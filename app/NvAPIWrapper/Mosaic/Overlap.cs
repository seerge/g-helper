using System;

namespace NvAPIWrapper.Mosaic
{
    /// <summary>
    ///     Holds mosaic overlap values
    /// </summary>
    public struct Overlap : IEquatable<Overlap>
    {
        /// <summary>
        ///     Creates a new Overlap
        /// </summary>
        /// <param name="overlapX">Horizontal overlap</param>
        /// <param name="overlapY">Vertical overlap</param>
        public Overlap(int overlapX, int overlapY)
        {
            HorizontalOverlap = overlapX;
            VerticalOverlap = overlapY;
        }

        /// <summary>
        ///     Gets or sets horizontal overlap (OverlapX)
        /// </summary>
        public int HorizontalOverlap { get; }

        /// <summary>
        ///     Gets or sets vertical overlap (OverlapY)
        /// </summary>
        public int VerticalOverlap { get; }

        /// <inheritdoc />
        public bool Equals(Overlap other)
        {
            return HorizontalOverlap == other.HorizontalOverlap && VerticalOverlap == other.VerticalOverlap;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Overlap && Equals((Overlap) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (HorizontalOverlap * 397) ^ VerticalOverlap;
            }
        }

        /// <summary>
        ///     Checks for equality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are equal, otherwise false</returns>
        public static bool operator ==(Overlap left, Overlap right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(Overlap left, Overlap right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"({HorizontalOverlap}, {VerticalOverlap})";
        }
    }
}