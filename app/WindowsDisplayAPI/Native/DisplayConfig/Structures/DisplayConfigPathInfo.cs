using System.Runtime.InteropServices;

namespace WindowsDisplayAPI.Native.DisplayConfig.Structures
{
    // https://msdn.microsoft.com/en-us/library/windows/hardware/ff553945(v=vs.85).aspx
    [StructLayout(LayoutKind.Sequential)]
    internal struct DisplayConfigPathInfo
    {
        [MarshalAs(UnmanagedType.Struct)] public readonly DisplayConfigPathSourceInfo SourceInfo;
        [MarshalAs(UnmanagedType.Struct)] public readonly DisplayConfigPathTargetInfo TargetInfo;
        [MarshalAs(UnmanagedType.U4)] public readonly DisplayConfigPathInfoFlags Flags;

        public DisplayConfigPathInfo(
            DisplayConfigPathSourceInfo sourceInfo,
            DisplayConfigPathTargetInfo targetInfo,
            DisplayConfigPathInfoFlags flags)
        {
            SourceInfo = sourceInfo;
            TargetInfo = targetInfo;
            Flags = flags;
        }

        public DisplayConfigPathInfo(DisplayConfigPathSourceInfo sourceInfo, DisplayConfigPathInfoFlags flags)
        {
            SourceInfo = sourceInfo;
            Flags = flags;
            TargetInfo = new DisplayConfigPathTargetInfo();
        }
    }
}