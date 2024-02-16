using System.Runtime.InteropServices;
using WindowsDisplayAPI.Native.Structures;

namespace WindowsDisplayAPI.Native.DisplayConfig.Structures
{
    // https://msdn.microsoft.com/en-us/library/windows/hardware/ff553954(v=vs.85).aspx
    [StructLayout(LayoutKind.Sequential)]
    internal struct DisplayConfigPathTargetInfo
    {
        [MarshalAs(UnmanagedType.Struct)] public readonly LUID AdapterId;
        [MarshalAs(UnmanagedType.U4)] public readonly uint TargetId;
        [MarshalAs(UnmanagedType.U4)] public readonly uint ModeInfoIndex;
        [MarshalAs(UnmanagedType.U4)] public readonly DisplayConfigVideoOutputTechnology OutputTechnology;
        [MarshalAs(UnmanagedType.U4)] public readonly DisplayConfigRotation Rotation;
        [MarshalAs(UnmanagedType.U4)] public readonly DisplayConfigScaling Scaling;
        [MarshalAs(UnmanagedType.Struct)] public readonly DisplayConfigRational RefreshRate;
        [MarshalAs(UnmanagedType.U4)] public readonly DisplayConfigScanLineOrdering ScanLineOrdering;
        [MarshalAs(UnmanagedType.Bool)] public readonly bool TargetAvailable;
        [MarshalAs(UnmanagedType.U4)] public readonly DisplayConfigPathTargetInfoFlags StatusFlags;

        public ushort TargetModeInfoIndex
        {
            get => (ushort) ((ModeInfoIndex << 16) >> 16);
        }

        public ushort DesktopModeInfoIndex
        {
            get => (ushort) (ModeInfoIndex >> 16);
        }

        public DisplayConfigPathTargetInfo(
            LUID adapterId,
            uint targetId,
            uint modeInfoIndex,
            DisplayConfigVideoOutputTechnology outputTechnology,
            DisplayConfigRotation rotation,
            DisplayConfigScaling scaling,
            DisplayConfigRational refreshRate,
            DisplayConfigScanLineOrdering scanLineOrdering,
            bool targetAvailable) : this()
        {
            AdapterId = adapterId;
            TargetId = targetId;
            ModeInfoIndex = modeInfoIndex;
            OutputTechnology = outputTechnology;
            Rotation = rotation;
            Scaling = scaling;
            RefreshRate = refreshRate;
            ScanLineOrdering = scanLineOrdering;
            TargetAvailable = targetAvailable;
        }

        public DisplayConfigPathTargetInfo(
            LUID adapterId,
            uint targetId,
            ushort targetModeInfoIndex,
            ushort desktopModeInfoIndex,
            DisplayConfigVideoOutputTechnology outputTechnology,
            DisplayConfigRotation rotation,
            DisplayConfigScaling scaling,
            DisplayConfigRational refreshRate,
            DisplayConfigScanLineOrdering scanLineOrdering,
            bool targetAvailable)
            : this(
                adapterId, targetId, 0, outputTechnology, rotation, scaling, refreshRate, scanLineOrdering,
                targetAvailable)
        {
            ModeInfoIndex = (uint) (targetModeInfoIndex + (desktopModeInfoIndex << 16));
        }
    }
}