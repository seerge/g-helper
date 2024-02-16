using System;
using System.Runtime.InteropServices;
using WindowsDisplayAPI.Native.Structures;

namespace WindowsDisplayAPI.Native.DisplayConfig.Structures
{
    // https://msdn.microsoft.com/en-us/library/windows/hardware/mt622102(v=vs.85).aspx
    [StructLayout(LayoutKind.Sequential)]
    internal struct DisplayConfigDesktopImageInfo : IEquatable<DisplayConfigDesktopImageInfo>
    {
        public const ushort InvalidDesktopImageModeIndex = 0xffff;
        [MarshalAs(UnmanagedType.Struct)] public readonly PointL PathSourceSize;
        [MarshalAs(UnmanagedType.Struct)] public readonly RectangleL DesktopImageRegion;
        [MarshalAs(UnmanagedType.Struct)] public readonly RectangleL DesktopImageClip;

        public DisplayConfigDesktopImageInfo(
            PointL pathSourceSize,
            RectangleL desktopImageRegion,
            RectangleL desktopImageClip)
        {
            PathSourceSize = pathSourceSize;
            DesktopImageRegion = desktopImageRegion;
            DesktopImageClip = desktopImageClip;
        }

        public bool Equals(DisplayConfigDesktopImageInfo other)
        {
            return PathSourceSize == other.PathSourceSize &&
                   DesktopImageRegion == other.DesktopImageRegion &&
                   DesktopImageClip == other.DesktopImageClip;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is DisplayConfigDesktopImageInfo info && Equals(info);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = PathSourceSize.GetHashCode();
                hashCode = (hashCode * 397) ^ DesktopImageRegion.GetHashCode();
                hashCode = (hashCode * 397) ^ DesktopImageClip.GetHashCode();

                return hashCode;
            }
        }

        public static bool operator ==(DisplayConfigDesktopImageInfo left, DisplayConfigDesktopImageInfo right)
        {
            return Equals(left, right) || left.Equals(right);
        }

        public static bool operator !=(DisplayConfigDesktopImageInfo left, DisplayConfigDesktopImageInfo right)
        {
            return !(left == right);
        }
    }
}