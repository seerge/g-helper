using System;
using System.Runtime.InteropServices;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <summary>
    ///     Holds a [X,Y] pair as a position on a 2D plane
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct Position : IEquatable<Position>
    {
        internal readonly int _X;
        internal readonly int _Y;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"[{X}, {Y}]";
        }

        /// <inheritdoc />
        public bool Equals(Position other)
        {
            return _X == other._X && _Y == other._Y;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Position position && Equals(position);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (_X * 397) ^ _Y;
            }
        }

        /// <summary>
        ///     Checks for equality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are equal, otherwise false</returns>
        public static bool operator ==(Position left, Position right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(Position left, Position right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        ///     Creates a new Position
        /// </summary>
        /// <param name="x">X value</param>
        /// <param name="y">Y value</param>
        public Position(int x, int y)
        {
            _X = x;
            _Y = y;
        }

        /// <summary>
        ///     X value
        /// </summary>
        public int X
        {
            get => _X;
        }

        /// <summary>
        ///     Y value
        /// </summary>
        public int Y
        {
            get => _Y;
        }
    }
}