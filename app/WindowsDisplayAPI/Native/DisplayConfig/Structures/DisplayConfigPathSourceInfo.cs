using System.Runtime.InteropServices;
using WindowsDisplayAPI.Native.Structures;

namespace WindowsDisplayAPI.Native.DisplayConfig.Structures
{
    // https://msdn.microsoft.com/en-us/library/windows/hardware/ff553951(v=vs.85).aspx
    [StructLayout(LayoutKind.Sequential)]
    internal struct DisplayConfigPathSourceInfo
    {
        public const ushort InvalidCloneGroupId = 0xffff;

        [MarshalAs(UnmanagedType.Struct)] public readonly LUID AdapterId;
        [MarshalAs(UnmanagedType.U4)] public readonly uint SourceId;
        [MarshalAs(UnmanagedType.U4)] public readonly uint ModeInfoIndex;
        [MarshalAs(UnmanagedType.U4)] public readonly DisplayConfigPathSourceInfoFlags StatusFlags;

        public ushort SourceModeInfoIndex
        {
            get => (ushort) ((ModeInfoIndex << 16) >> 16);
        }

        public ushort CloneGroupId
        {
            get => (ushort) (ModeInfoIndex >> 16);
        }

        public DisplayConfigPathSourceInfo(LUID adapterId, uint sourceId, uint modeInfoIndex) : this()
        {
            AdapterId = adapterId;
            SourceId = sourceId;
            ModeInfoIndex = modeInfoIndex;
        }

        public DisplayConfigPathSourceInfo(
            LUID adapterId,
            uint sourceId,
            ushort sourceModeInfoIndex,
            ushort cloneGroupId) : this(adapterId, sourceId, 0)
        {
            ModeInfoIndex = (uint) (sourceModeInfoIndex + (cloneGroupId << 16));
        }
    }
}