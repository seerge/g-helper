using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Contains information regarding GPU clock boost table
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct PrivateClockBoostTableV1 : IInitializable
    {
        internal const int MaxNumberOfMasks = 4;
        internal const int MaxNumberOfUnknown1 = 12;
        internal const int MaxNumberOfGPUDeltas = 80;
        internal const int MaxNumberOfMemoryFilled = 23;
        internal const int MaxNumberOfMemoryDeltas = 23;
        internal const int MaxNumberOfUnknown2 = 1529;

        internal StructureVersion _Version;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxNumberOfMasks)]
        internal uint[] _Masks;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxNumberOfUnknown1)]
        internal uint[] _Unknown1;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxNumberOfGPUDeltas)]
        internal GPUDelta[] _GPUDeltas;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxNumberOfMemoryFilled)]
        internal uint[] _MemoryFilled;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxNumberOfMemoryDeltas)]
        internal int[] _MemoryDeltas;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxNumberOfUnknown2)]
        internal uint[] _Unknown2;

        /// <summary>
        ///     Gets a list of clock delta entries
        /// </summary>
        public GPUDelta[] GPUDeltas
        {
            get => _GPUDeltas;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="PrivateClockBoostTableV1" />
        /// </summary>
        /// <param name="gpuDeltas">The list of GPU clock frequency delta entries.</param>
        // ReSharper disable once TooManyDependencies
        public PrivateClockBoostTableV1(GPUDelta[] gpuDeltas)
        {
            if (gpuDeltas?.Length > MaxNumberOfGPUDeltas)
            {
                throw new ArgumentException($"Maximum of {MaxNumberOfGPUDeltas} GPU delta values are configurable.",
                    nameof(gpuDeltas));
            }

            if (gpuDeltas == null)
            {
                throw new ArgumentNullException(nameof(gpuDeltas));
            }

            this = typeof(PrivateClockBoostTableV1).Instantiate<PrivateClockBoostTableV1>();

            Array.Copy(gpuDeltas, 0, _GPUDeltas, 0, gpuDeltas.Length);
        }


        /// <summary>
        ///     Contains information regarding a GPU delta entry in the clock boost table
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct GPUDelta
        {
            internal uint _Unknown1;
            internal uint _Unknown2;
            internal uint _Unknown3;
            internal uint _Unknown4;
            internal uint _Unknown5;
            internal int _FrequencyDeltaInkHz;
            internal uint _Unknown7;
            internal uint _Unknown8;
            internal uint _Unknown9;

            /// <summary>
            ///     Gets the frequency delta in kHz
            /// </summary>
            public int FrequencyDeltaInkHz
            {
                get => _FrequencyDeltaInkHz;
            }

            /// <summary>
            ///     Creates a new instance of GPUDelta.
            /// </summary>
            /// <param name="frequencyDeltaInkHz">The clock frequency in kHz.</param>
            public GPUDelta(int frequencyDeltaInkHz) : this()
            {
                _FrequencyDeltaInkHz = frequencyDeltaInkHz;
            }
        }
    }
}