using System;
using System.Runtime.InteropServices;

namespace WindowsDisplayAPI.Native.DisplayConfig.Structures
{
    // https://msdn.microsoft.com/en-us/library/windows/hardware/ff553913(v=vs.85).aspx
    [StructLayout(LayoutKind.Sequential)]
    internal struct DisplayConfig2DRegion : IEquatable<DisplayConfig2DRegion>
    {
        [MarshalAs(UnmanagedType.U4)] public readonly uint Width;
        [MarshalAs(UnmanagedType.U4)] public readonly uint Height;

        public DisplayConfig2DRegion(uint width, uint height)
        {
            Width = width;
            Height = height;
        }

        public bool Equals(DisplayConfig2DRegion other)
        {
            return Width == other.Width && Height == other.Height;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is DisplayConfig2DRegion region && Equals(region);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) Width * 397) ^ (int) Height;
            }
        }

        public static bool operator ==(DisplayConfig2DRegion left, DisplayConfig2DRegion right)
        {
            return Equals(left, right) || left.Equals(right);
        }

        public static bool operator !=(DisplayConfig2DRegion left, DisplayConfig2DRegion right)
        {
            return !(left == right);
        }
    }
}