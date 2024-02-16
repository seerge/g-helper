using System;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Runtime.InteropServices;

namespace WindowsDisplayAPI.Native.Structures
{
    // https://msdn.microsoft.com/en-us/library/vs/alm/dd162907(v=vs.85).aspx
    [StructLayout(LayoutKind.Sequential)]
    internal struct RectangleL : IEquatable<RectangleL>
    {
        [MarshalAs(UnmanagedType.U4)] public readonly int Left;
        [MarshalAs(UnmanagedType.U4)] public readonly int Top;
        [MarshalAs(UnmanagedType.U4)] public readonly int Right;
        [MarshalAs(UnmanagedType.U4)] public readonly int Bottom;

        [Pure]
        public Rectangle ToRectangle()
        {
            return new Rectangle(Left, Top, Right - Left, Bottom - Top);
        }

        public RectangleL(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public RectangleL(Rectangle rectangle) : this(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom)
        {
        }

        public bool Equals(RectangleL other)
        {
            return Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is RectangleL rectangle && Equals(rectangle);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Left;
                hashCode = (hashCode * 397) ^ Top;
                hashCode = (hashCode * 397) ^ Right;
                hashCode = (hashCode * 397) ^ Bottom;

                return hashCode;
            }
        }

        public static bool operator ==(RectangleL left, RectangleL right)
        {
            return Equals(left, right) || left.Equals(right);
        }

        public static bool operator !=(RectangleL left, RectangleL right)
        {
            return !(left == right);
        }
    }
}