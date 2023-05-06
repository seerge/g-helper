using System;
using System.Runtime.InteropServices;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <summary>
    ///     Holds a [Width, Height] pair as the resolution of a display device, as well as a color format
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct Resolution : IEquatable<Resolution>
    {
        internal readonly uint _Width;
        internal readonly uint _Height;
        internal readonly uint _ColorDepth;

        /// <summary>
        ///     Creates a new Resolution
        /// </summary>
        /// <param name="width">Display resolution width</param>
        /// <param name="height">Display resolution height</param>
        /// <param name="colorDepth">Display color depth</param>
        public Resolution(int width, int height, int colorDepth)
        {
            _Width = (uint) width;
            _Height = (uint) height;
            _ColorDepth = (uint) colorDepth;
        }

        /// <inheritdoc />
        public bool Equals(Resolution other)
        {
            return _Width == other._Width && _Height == other._Height && _ColorDepth == other._ColorDepth;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Resolution resolution && Equals(resolution);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) _Width;
                hashCode = (hashCode * 397) ^ (int) _Height;
                hashCode = (hashCode * 397) ^ (int) _ColorDepth;

                return hashCode;
            }
        }

        /// <summary>
        ///     Checks for equality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are equal, otherwise false</returns>
        public static bool operator ==(Resolution left, Resolution right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(Resolution left, Resolution right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"({Width}, {Height}) @ {ColorDepth}bpp";
        }

        /// <summary>
        ///     Display resolution width
        /// </summary>
        public int Width
        {
            get => (int) _Width;
        }

        /// <summary>
        ///     Display resolution height
        /// </summary>
        public int Height
        {
            get => (int) _Height;
        }

        /// <summary>
        ///     Display color depth
        /// </summary>
        public int ColorDepth
        {
            get => (int) _ColorDepth;
        }
    }
}