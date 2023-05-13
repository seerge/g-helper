using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Contains information regarding GPU performance limitations status
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct PrivatePerformanceStatusV1 : IInitializable
    {
        internal const int MaxNumberOfTimers = 3;
        internal const int MaxNumberOfUnknown5 = 326;

        internal StructureVersion _Version;
        internal uint _Unknown1;
        internal ulong _TimerInNanoSecond;
        internal PerformanceLimit _PerformanceLimit;
        internal uint _Unknown2;
        internal uint _Unknown3;
        internal uint _Unknown4;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxNumberOfTimers)]
        internal ulong[] _TimersInNanoSecond;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxNumberOfUnknown5)]
        internal uint[] _Unknown5;

        /// <summary>
        ///     Gets the current effective performance limitation
        /// </summary>
        public PerformanceLimit PerformanceLimit
        {
            get => _PerformanceLimit;
        }
    }
}