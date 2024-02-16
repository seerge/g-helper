using System;
using System.Runtime.InteropServices;
using WindowsDisplayAPI.Native.Structures;

namespace WindowsDisplayAPI.Native.DisplayConfig.Structures
{
    // https://msdn.microsoft.com/en-us/library/windows/hardware/ff553933(v=vs.85).aspx
    [StructLayout(LayoutKind.Explicit)] // Size = 64
    internal struct DisplayConfigModeInfo : IEquatable<DisplayConfigModeInfo>
    {
        public const uint InvalidModeIndex = 0xffffffff;

        [MarshalAs(UnmanagedType.U4)] [FieldOffset(0)]
        public readonly DisplayConfigModeInfoType InfoType;

        [MarshalAs(UnmanagedType.U4)] [FieldOffset(4)]
        public readonly uint Id;

        [MarshalAs(UnmanagedType.Struct)] [FieldOffset(8)]
        public readonly LUID AdapterId;

        [MarshalAs(UnmanagedType.Struct)] [FieldOffset(16)]
        public readonly DisplayConfigTargetMode TargetMode;

        [MarshalAs(UnmanagedType.Struct)] [FieldOffset(16)]
        public readonly DisplayConfigSourceMode SourceMode;

        [MarshalAs(UnmanagedType.Struct)] [FieldOffset(16)]
        public readonly DisplayConfigDesktopImageInfo
            DesktopImageInfo;

        public DisplayConfigModeInfo(LUID adapterId, uint id, DisplayConfigTargetMode targetMode) : this()
        {
            AdapterId = adapterId;
            Id = id;
            TargetMode = targetMode;
            InfoType = DisplayConfigModeInfoType.Target;
        }

        public DisplayConfigModeInfo(LUID adapterId, uint id, DisplayConfigSourceMode sourceMode) : this()
        {
            AdapterId = adapterId;
            Id = id;
            SourceMode = sourceMode;
            InfoType = DisplayConfigModeInfoType.Source;
        }

        public DisplayConfigModeInfo(LUID adapterId, uint id, DisplayConfigDesktopImageInfo desktopImageInfo) : this()
        {
            AdapterId = adapterId;
            Id = id;
            DesktopImageInfo = desktopImageInfo;
            InfoType = DisplayConfigModeInfoType.DesktopImage;
        }

        public bool Equals(DisplayConfigModeInfo other)
        {
            return InfoType == other.InfoType &&
                   Id == other.Id &&
                   AdapterId == other.AdapterId &&
                   (InfoType == DisplayConfigModeInfoType.Source && SourceMode == other.SourceMode ||
                    InfoType == DisplayConfigModeInfoType.Target && TargetMode == other.TargetMode ||
                    InfoType == DisplayConfigModeInfoType.DesktopImage &&
                    DesktopImageInfo == other.DesktopImageInfo);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is DisplayConfigModeInfo info && Equals(info);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) InfoType;
                hashCode = (hashCode * 397) ^ (int) Id;
                hashCode = (hashCode * 397) ^ AdapterId.GetHashCode();

                switch (InfoType)
                {
                    case DisplayConfigModeInfoType.Source:
                        hashCode = (hashCode * 397) ^ SourceMode.GetHashCode();

                        break;
                    case DisplayConfigModeInfoType.Target:
                        hashCode = (hashCode * 397) ^ TargetMode.GetHashCode();

                        break;
                    case DisplayConfigModeInfoType.DesktopImage:
                        hashCode = (hashCode * 397) ^ DesktopImageInfo.GetHashCode();

                        break;
                }

                return hashCode;
            }
        }

        public static bool operator ==(DisplayConfigModeInfo left, DisplayConfigModeInfo right)
        {
            return Equals(left, right) || left.Equals(right);
        }

        public static bool operator !=(DisplayConfigModeInfo left, DisplayConfigModeInfo right)
        {
            return !(left == right);
        }
    }
}