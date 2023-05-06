using System;

namespace NvAPIWrapper.Display
{
    /// <summary>
    ///     Represents a RGB intensity texture pixel
    /// </summary>
    public class IntensityTexturePixel : IEquatable<IntensityTexturePixel>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="IntensityTexturePixel" />.
        /// </summary>
        /// <param name="redIntensity">The intensity of the red light (0-1)</param>
        /// <param name="greenIntensity">The intensity of the green light (0-1)</param>
        /// <param name="blueIntensity">The intensity of the blue light (0-1)</param>
        public IntensityTexturePixel(float redIntensity, float greenIntensity, float blueIntensity)
        {
            RedIntensity = Math.Max(Math.Min(redIntensity, 1), 0);
            GreenIntensity = Math.Max(Math.Min(greenIntensity, 1), 0);
            BlueIntensity = Math.Max(Math.Min(blueIntensity, 1), 0);
        }

        /// <summary>
        ///     Gets the intensity of the blue light (0-1)
        /// </summary>
        public float BlueIntensity { get; }

        /// <summary>
        ///     Gets the intensity of the green light (0-1)
        /// </summary>
        public float GreenIntensity { get; }

        /// <summary>
        ///     Gets the intensity of the red light (0-1)
        /// </summary>
        public float RedIntensity { get; }

        /// <inheritdoc />
        public bool Equals(IntensityTexturePixel other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Math.Abs(RedIntensity - other.RedIntensity) < 0.0001 &&
                   Math.Abs(GreenIntensity - other.GreenIntensity) < 0.0001 &&
                   Math.Abs(BlueIntensity - other.BlueIntensity) < 0.0001;
        }

        /// <summary>
        ///     Compares two instance of <see cref="IntensityTexturePixel" /> for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns><see langword="true" /> if both instances are equal, otherwise <see langword="false" /></returns>
        public static bool operator ==(IntensityTexturePixel left, IntensityTexturePixel right)
        {
            return Equals(left, right) || left?.Equals(right) == true;
        }

        /// <summary>
        ///     Compares two instance of <see cref="IntensityTexturePixel" /> for in-equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns><see langword="true" /> if both instances are not equal, otherwise <see langword="false" /></returns>
        public static bool operator !=(IntensityTexturePixel left, IntensityTexturePixel right)
        {
            return !(left == right);
        }

        internal static IntensityTexturePixel FromFloatArray(float[] floats, int index)
        {
            return new IntensityTexturePixel(floats[index], floats[index + 1], floats[index + 2]);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return Equals(obj as IntensityTexturePixel);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = RedIntensity.GetHashCode();
                hashCode = (hashCode * 397) ^ GreenIntensity.GetHashCode();
                hashCode = (hashCode * 397) ^ BlueIntensity.GetHashCode();

                return hashCode;
            }
        }

        internal float[] ToFloatArray()
        {
            return new[] {RedIntensity, GreenIntensity, BlueIntensity};
        }
    }
}