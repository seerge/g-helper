using System;
using System.Runtime.InteropServices;

namespace WindowsDisplayAPI.Native.DisplayConfig.Structures
{
    // https://msdn.microsoft.com/en-us/library/windows/hardware/ff553993(v=vs.85).aspx
    [StructLayout(LayoutKind.Sequential)]
    internal struct DisplayConfigTargetMode : IEquatable<DisplayConfigTargetMode>
    {
        public const ushort InvalidTargetModeIndex = 0xffff;
        [MarshalAs(UnmanagedType.Struct)] public readonly DisplayConfigVideoSignalInfo TargetVideoSignalInfo;

        public DisplayConfigTargetMode(DisplayConfigVideoSignalInfo targetVideoSignalInfo)
        {
            TargetVideoSignalInfo = targetVideoSignalInfo;
        }

        public bool Equals(DisplayConfigTargetMode other)
        {
            return TargetVideoSignalInfo == other.TargetVideoSignalInfo;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is DisplayConfigTargetMode mode && Equals(mode);
        }

        public override int GetHashCode()
        {
            return TargetVideoSignalInfo.GetHashCode();
        }

        public static bool operator ==(DisplayConfigTargetMode left, DisplayConfigTargetMode right)
        {
            return Equals(left, right) || left.Equals(right);
        }

        public static bool operator !=(DisplayConfigTargetMode left, DisplayConfigTargetMode right)
        {
            return !(left == right);
        }
    }
}