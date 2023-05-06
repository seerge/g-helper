using System;
using System.Linq;

namespace NvAPIWrapper.Display
{
    /// <summary>
    ///     Represents a texture of float values
    /// </summary>
    public class FloatTexture : IEquatable<FloatTexture>
    {
        /// <summary>
        ///     Underlying float array containing the values of all channels in all pixels
        /// </summary>
        protected readonly float[] UnderlyingArray;

        /// <summary>
        ///     Creates a new instance of <see cref="FloatTexture" />.
        /// </summary>
        /// <param name="width">The texture width.</param>
        /// <param name="height">The texture height.</param>
        /// <param name="channels">The number of texture channels.</param>
        public FloatTexture(int width, int height, int channels) : this(width, height, channels, null)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="FloatTexture" />.
        /// </summary>
        /// <param name="width">The texture width.</param>
        /// <param name="height">The texture height.</param>
        /// <param name="channels">The number of texture channels.</param>
        /// <param name="array">The underlying array containing all float values.</param>
        // ReSharper disable once TooManyDependencies
        protected FloatTexture(int width, int height, int channels, float[] array)
        {
            Width = width;
            Height = height;
            Channels = channels;
            UnderlyingArray = array ?? new float[width * height * channels];
        }

        /// <summary>
        ///     Gets the number of texture channels
        /// </summary>
        public int Channels { get; }

        /// <summary>
        ///     Gets the texture height in pixel
        /// </summary>
        public int Height { get; }

        /// <summary>
        ///     Gets the texture width in pixels
        /// </summary>
        public int Width { get; }

        /// <inheritdoc />
        public bool Equals(FloatTexture other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (other.UnderlyingArray.Length != UnderlyingArray.Length)
            {
                return false;
            }

            if (other.Width != Width || other.Height != Height || other.Channels != Channels)
            {
                return false;
            }

            return !UnderlyingArray.Where((t, i) => Math.Abs(other.UnderlyingArray[i] - t) > 0.0001).Any();
        }

        /// <summary>
        ///     Returns a new instance of FloatTexture from the passed array of float values.
        /// </summary>
        /// <param name="width">The texture width.</param>
        /// <param name="height">The texture height.</param>
        /// <param name="channels">The texture channels.</param>
        /// <param name="floats">The array of float values.</param>
        /// <returns>A new instance of <see cref="FloatTexture" />.</returns>
        // ReSharper disable once TooManyArguments
        public static FloatTexture FromFloatArray(int width, int height, int channels, float[] floats)
        {
            if (floats.Length != width * height * channels)
            {
                throw new ArgumentOutOfRangeException(nameof(floats));
            }

            return new FloatTexture(width, height, channels, floats.ToArray());
        }

        /// <summary>
        ///     Compares two instance of <see cref="FloatTexture" /> for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns><see langword="true" /> if both instances are equal, otherwise <see langword="false" /></returns>
        public static bool operator ==(FloatTexture left, FloatTexture right)
        {
            return Equals(left, right) || left?.Equals(right) == true;
        }

        /// <summary>
        ///     Compares two instance of <see cref="FloatTexture" /> for in-equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns><see langword="true" /> if both instances are not equal, otherwise <see langword="false" /></returns>
        public static bool operator !=(FloatTexture left, FloatTexture right)
        {
            return !(left == right);
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

            return Equals(obj as FloatTexture);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return UnderlyingArray.GetHashCode();
        }

        /// <summary>
        ///     Gets the values of each channel at a specific location
        /// </summary>
        /// <param name="x">The horizontal location.</param>
        /// <param name="y">The vertical location.</param>
        /// <returns>An array of float values each representing a channel value.</returns>
        public float[] GetValues(int x, int y)
        {
            return UnderlyingArray.Skip(y * Width + x).Take(Channels).ToArray();
        }

        /// <summary>
        ///     Sets the value of each channel at a specific location
        /// </summary>
        /// <param name="x">The horizontal location.</param>
        /// <param name="y">The vertical location.</param>
        /// <param name="floats">An array of float values each representing a channel value.</param>
        public void SetValues(int x, int y, params float[] floats)
        {
            var index = y * Width + x;

            for (var i = 0; i < Math.Min(Channels, floats.Length); i++)
            {
                UnderlyingArray[index + i] = floats[i];
            }
        }

        /// <summary>
        ///     Returns this instance of <see cref="FloatTexture" /> as an array of float values.
        /// </summary>
        /// <returns>An array of float values representing this instance of <see cref="FloatTexture" />.</returns>
        public float[] ToFloatArray()
        {
            // Returns a copy of the underlying array
            return UnderlyingArray.ToArray();
        }
    }
}