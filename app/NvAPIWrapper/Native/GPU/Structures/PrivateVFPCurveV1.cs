using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Contains information regarding GPU boost frequency curve
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct PrivateVFPCurveV1 : IInitializable
    {
        internal const int MaxNumberOfMasks = 4;
        internal const int MaxNumberOfUnknown1 = 12;
        internal const int MaxNumberOfGPUCurveEntries = 80;
        internal const int MaxNumberOfMemoryCurveEntries = 23;
        internal const int MaxNumberOfUnknown2 = 1064;

        internal StructureVersion _Version;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxNumberOfMasks)]
        internal readonly uint[] _Masks;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxNumberOfUnknown1)]
        internal readonly uint[] _Unknown1;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxNumberOfGPUCurveEntries)]
        internal readonly VFPCurveEntry[] _GPUCurveEntries;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxNumberOfMemoryCurveEntries)]
        internal readonly VFPCurveEntry[] _MemoryCurveEntries;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxNumberOfUnknown2)]
        internal readonly uint[] _Unknown2;


        /// <summary>
        ///     Gets the list of GPU curve entries
        /// </summary>
        public VFPCurveEntry[] GPUCurveEntries
        {
            get => _GPUCurveEntries;
        }

        /// <summary>
        ///     Gets the list of memory curve entries
        /// </summary>
        public VFPCurveEntry[] MemoryCurveEntries
        {
            get => _MemoryCurveEntries;
        }

        /// <summary>
        ///     Contains information regarding a boost frequency curve entry
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct VFPCurveEntry
        {
            internal uint _Unknown1;
            internal uint _FrequencyInkHz;
            internal uint _VoltageInMicroV;
            internal uint _Unknown2;
            internal uint _Unknown3;
            internal uint _Unknown4;
            internal uint _Unknown5;

            /// <summary>
            ///     Gets the frequency in kHz
            /// </summary>
            public uint FrequencyInkHz
            {
                get => _FrequencyInkHz;
            }

            /// <summary>
            ///     Gets the voltage in uV
            /// </summary>
            public uint VoltageInMicroV
            {
                get => _VoltageInMicroV;
            }
        }
    }
}