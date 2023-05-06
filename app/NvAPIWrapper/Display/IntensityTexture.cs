using System;
using System.Linq;

namespace NvAPIWrapper.Display
{
    /// <summary>
    ///     Represents a texture of intensity values
    /// </summary>
    public class IntensityTexture : FloatTexture
    {
        /// <summary>
        ///     Creates a new instance of <see cref="IntensityTexture" />.
        /// </summary>
        /// <param name="width">The texture width.</param>
        /// <param name="height">The texture height.</param>
        public IntensityTexture(int width, int height) : base(width, height, 3)
        {
        }

        private IntensityTexture(int width, int height, float[] floats) : base(width, height, 3, floats)
        {
        }

        /// <summary>
        ///     Returns a new instance of FloatTexture from the passed array of float values.
        /// </summary>
        /// <param name="width">The texture width.</param>
        /// <param name="height">The texture height.</param>
        /// <param name="floats">The array of float values.</param>
        /// <returns>A new instance of <see cref="FloatTexture" />.</returns>
        // ReSharper disable once TooManyArguments
        public static IntensityTexture FromFloatArray(int width, int height, float[] floats)
        {
            if (floats.Length != width * height * 3)
            {
                throw new ArgumentOutOfRangeException(nameof(floats));
            }

            return new IntensityTexture(width, height, floats.ToArray());
        }

        /// <summary>
        ///     Gets the value of intensity pixel at a specific location.
        /// </summary>
        /// <param name="x">The horizontal location.</param>
        /// <param name="y">The vertical location.</param>
        /// <returns>An instance of <see cref="IntensityTexturePixel" />.</returns>
        public IntensityTexturePixel GetPixel(int x, int y)
        {
            return IntensityTexturePixel.FromFloatArray(UnderlyingArray, y * Width + x);
        }

        /// <summary>
        ///     Sets the value of intensity pixel at a specific location
        /// </summary>
        /// <param name="x">The horizontal location.</param>
        /// <param name="y">The vertical location.</param>
        /// <param name="pixel">An instance of <see cref="IntensityTexturePixel" />.</param>
        public void SetPixel(int x, int y, IntensityTexturePixel pixel)
        {
            var index = y * Width + x;
            var floats = pixel.ToFloatArray();

            for (var i = 0; i < Math.Min(Channels, floats.Length); i++)
            {
                UnderlyingArray[index + i] = floats[i];
            }
        }
    }
}