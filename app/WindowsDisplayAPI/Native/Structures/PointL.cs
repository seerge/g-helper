using System;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Runtime.InteropServices;

namespace WindowsDisplayAPI.Native.Structures
{
    // https://msdn.microsoft.com/en-us/library/vs/alm/dd162807(v=vs.85).aspx
    [StructLayout(LayoutKind.Sequential)]
    internal struct PointL : IEquatable<PointL>
    {
        [MarshalAs(UnmanagedType.I4)] public readonly int X;
        [MarshalAs(UnmanagedType.I4)] public readonly int Y;

        [Pure]
        public Point ToPoint()
        {
            return new Point(X, Y);
        }

        [Pure]
        public Size ToSize()
        {
            return new Size(X, Y);
        }

        public PointL(Point point) : this(point.X, point.Y)
        {
        }

        public PointL(Size size) : this(size.Width, size.Height)
        {
        }

        public PointL(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(PointL other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is PointL point && Equals(point);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }

        public static bool operator ==(PointL left, PointL right)
        {
            return Equals(left, right) || left.Equals(right);
        }

        public static bool operator !=(PointL left, PointL right)
        {
            return !(left == right);
        }
    }
}