using System;
using System.Runtime.InteropServices;
using WindowsDisplayAPI.Native.Structures;

namespace WindowsDisplayAPI.Native.DisplayConfig.Structures
{
    // https://msdn.microsoft.com/en-us/library/windows/hardware/ff553986(v=vs.85).aspx
    [StructLayout(LayoutKind.Sequential)]
    internal struct DisplayConfigSourceMode : IEquatable<DisplayConfigSourceMode>
    {
        public const ushort InvalidSourceModeIndex = 0xffff;
        [MarshalAs(UnmanagedType.U4)] public readonly uint Width;
        [MarshalAs(UnmanagedType.U4)] public readonly uint Height;
        [MarshalAs(UnmanagedType.U4)] public readonly DisplayConfigPixelFormat PixelFormat;
        [MarshalAs(UnmanagedType.Struct)] public readonly PointL Position;

        public DisplayConfigSourceMode(uint width, uint height, DisplayConfigPixelFormat pixelFormat, PointL position)
        {
            Width = width;
            Height = height;
            PixelFormat = pixelFormat;
            Position = position;
        }

        public bool Equals(DisplayConfigSourceMode other)
        {
            return Width == other.Width &&
                   Height == other.Height &&
                   PixelFormat == other.PixelFormat &&
                   Position == other.Position;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is DisplayConfigSourceMode mode && Equals(mode);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) Width;
                hashCode = (hashCode * 397) ^ (int) Height;
                hashCode = (hashCode * 397) ^ (int) PixelFormat;
                hashCode = (hashCode * 397) ^ Position.GetHashCode();

                return hashCode;
            }
        }

        public static bool operator ==(DisplayConfigSourceMode left, DisplayConfigSourceMode right)
        {
            return Equals(left, right) || left.Equals(right);
        }

        public static bool operator !=(DisplayConfigSourceMode left, DisplayConfigSourceMode right)
        {
            return !(left == right);
        }
    }
}