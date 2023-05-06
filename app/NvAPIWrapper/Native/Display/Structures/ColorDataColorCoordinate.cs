using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <summary>
    ///     Holds coordinates of a color in the color space
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct ColorDataColorCoordinate : IEquatable<ColorDataColorCoordinate>
    {
        private readonly ushort _X;
        private readonly ushort _Y;

        /// <summary>
        ///     Gets the color space's X coordinate
        /// </summary>
        public float X
        {
            get => (float) _X / 0xC350;
        }

        /// <summary>
        ///     Gets the color space's Y coordinate
        /// </summary>
        public float Y
        {
            get => (float) _Y / 0xC350;
        }

        /// <summary>
        ///     Creates an instance of <see cref="ColorDataColorCoordinate" />.
        /// </summary>
        /// <param name="x">The color space's X coordinate.</param>
        /// <param name="y">The color space's Y coordinate.</param>
        public ColorDataColorCoordinate(float x, float y)
        {
            _X = (ushort) (Math.Min(Math.Max(x, 0), 1) * 0xC350);
            _Y = (ushort) (Math.Min(Math.Max(y, 0), 1) * 0xC350);
        }

        /// <summary>
        ///     Creates an instance of <see cref="ColorDataColorCoordinate" />.
        /// </summary>
        /// <param name="coordinate">The color space's coordinates.</param>
        public ColorDataColorCoordinate(PointF coordinate) : this(coordinate.X, coordinate.Y)
        {
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"({X:F3}, {Y:F3})";
        }

        /// <inheritdoc />
        public bool Equals(ColorDataColorCoordinate other)
        {
            return _X == other._X && _Y == other._Y;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is ColorDataColorCoordinate other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (_X.GetHashCode() * 397) ^ _Y.GetHashCode();
            }
        }

        /// <summary>
        ///     Checks two instance of <see cref="ColorDataColorCoordinate" /> for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>true if both instances are equal, otherwise false.</returns>
        public static bool operator ==(ColorDataColorCoordinate left, ColorDataColorCoordinate right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Checks two instance of <see cref="ColorDataColorCoordinate" /> for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>true if both instances are not equal, otherwise false.</returns>
        public static bool operator !=(ColorDataColorCoordinate left, ColorDataColorCoordinate right)
        {
            return !left.Equals(right);
        }
    }
}