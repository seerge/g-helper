using System;
using System.Runtime.InteropServices;

namespace WindowsDisplayAPI.Native.DisplayConfig.Structures
{
    // https://msdn.microsoft.com/en-us/library/windows/hardware/ff554007(v=vs.85).aspx
    [StructLayout(LayoutKind.Sequential)]
    internal struct DisplayConfigVideoSignalInfo : IEquatable<DisplayConfigVideoSignalInfo>
    {
        [MarshalAs(UnmanagedType.U8)] public readonly ulong PixelRate;
        [MarshalAs(UnmanagedType.Struct)] public readonly DisplayConfigRational HorizontalSyncFrequency;
        [MarshalAs(UnmanagedType.Struct)] public readonly DisplayConfigRational VerticalSyncFrequency;
        [MarshalAs(UnmanagedType.Struct)] public readonly DisplayConfig2DRegion ActiveSize;
        [MarshalAs(UnmanagedType.Struct)] public readonly DisplayConfig2DRegion TotalSize;
        [MarshalAs(UnmanagedType.U2)] public readonly VideoSignalStandard VideoStandard;
        [MarshalAs(UnmanagedType.U2)] public readonly ushort VerticalSyncFrequencyDivider;
        [MarshalAs(UnmanagedType.U4)] public readonly DisplayConfigScanLineOrdering ScanLineOrdering;

        public DisplayConfigVideoSignalInfo(
            ulong pixelRate,
            DisplayConfigRational horizontalSyncFrequency,
            DisplayConfigRational verticalSyncFrequency,
            DisplayConfig2DRegion activeSize,
            DisplayConfig2DRegion totalSize,
            VideoSignalStandard videoStandard,
            ushort verticalSyncFrequencyDivider,
            DisplayConfigScanLineOrdering scanLineOrdering)
        {
            PixelRate = pixelRate;
            HorizontalSyncFrequency = horizontalSyncFrequency;
            VerticalSyncFrequency = verticalSyncFrequency;
            ActiveSize = activeSize;
            TotalSize = totalSize;
            VideoStandard = videoStandard;
            VerticalSyncFrequencyDivider = verticalSyncFrequencyDivider;
            ScanLineOrdering = scanLineOrdering;
        }

        public bool Equals(DisplayConfigVideoSignalInfo other)
        {
            return PixelRate == other.PixelRate &&
                   HorizontalSyncFrequency == other.HorizontalSyncFrequency &&
                   VerticalSyncFrequency == other.VerticalSyncFrequency &&
                   ActiveSize == other.ActiveSize &&
                   TotalSize == other.TotalSize &&
                   VideoStandard == other.VideoStandard &&
                   VerticalSyncFrequencyDivider == other.VerticalSyncFrequencyDivider &&
                   ScanLineOrdering == other.ScanLineOrdering;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is DisplayConfigVideoSignalInfo info && Equals(info);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = PixelRate.GetHashCode();
                hashCode = (hashCode * 397) ^ HorizontalSyncFrequency.GetHashCode();
                hashCode = (hashCode * 397) ^ VerticalSyncFrequency.GetHashCode();
                hashCode = (hashCode * 397) ^ ActiveSize.GetHashCode();
                hashCode = (hashCode * 397) ^ TotalSize.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) VideoStandard;
                hashCode = (hashCode * 397) ^ VerticalSyncFrequencyDivider.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) ScanLineOrdering;

                return hashCode;
            }
        }

        public static bool operator ==(DisplayConfigVideoSignalInfo left, DisplayConfigVideoSignalInfo right)
        {
            return Equals(left, right) || left.Equals(right);
        }

        public static bool operator !=(DisplayConfigVideoSignalInfo left, DisplayConfigVideoSignalInfo right)
        {
            return !(left == right);
        }
    }
}