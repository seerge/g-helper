using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Contains information regarding GPU clock boost masks
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct PrivateClockBoostMasksV1 : IInitializable
    {
        internal const int MaxMasks = 4;
        internal const int MaxUnknown1 = 8;
        internal const int MaxClockBoostMasks = 103;
        internal const int MaxUnknown2 = 916;

        internal StructureVersion _Version;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxMasks)]
        internal readonly uint[] _Masks;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxUnknown1)]
        internal readonly uint[] _Unknown1;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxClockBoostMasks)]
        internal readonly ClockBoostMask[] _ClocksBoostMasks;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxUnknown2)]
        internal readonly uint[] _Unknown2;

        /// <summary>
        ///     Gets a list of clock boost masks
        /// </summary>
        public ClockBoostMask[] ClockBoostMasks
        {
            get => _ClocksBoostMasks;
        }

        /// <summary>
        ///     Contains information regarding a clock boost mask
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct ClockBoostMask
        {
            internal readonly uint _Unknown;
            internal readonly uint _Unknown2;
            internal readonly uint _Unknown3;
            internal readonly uint _Unknown4;
            internal readonly int _MemoryDelta;
            internal readonly int _GPUDelta;

            /// <summary>
            ///     Memory clock frequency delta
            /// </summary>
            public int MemoryDelta
            {
                get => _MemoryDelta;
            }

            /// <summary>
            ///     GPU clock frequency delta
            /// </summary>
            public int GPUDelta
            {
                get => _GPUDelta;
            }
        }
    }
}