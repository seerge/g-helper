using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Interfaces;
using NvAPIWrapper.Native.Interfaces.GPU;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Holds information about the system's display driver memory.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(2)]
    public struct DisplayDriverMemoryInfoV2 : IInitializable, IDisplayDriverMemoryInfo
    {
        internal StructureVersion _Version;
        internal readonly uint _DedicatedVideoMemory;
        internal readonly uint _AvailableDedicatedVideoMemory;
        internal readonly uint _SystemVideoMemory;
        internal readonly uint _SharedSystemMemory;
        internal readonly uint _CurrentAvailableDedicatedVideoMemory;

        /// <inheritdoc />
        public uint DedicatedVideoMemoryInkB
        {
            get => _DedicatedVideoMemory;
        }

        /// <inheritdoc />
        public uint AvailableDedicatedVideoMemoryInkB
        {
            get => _AvailableDedicatedVideoMemory;
        }

        /// <inheritdoc />
        public uint SystemVideoMemoryInkB
        {
            get => _SystemVideoMemory;
        }

        /// <inheritdoc />
        public uint SharedSystemMemoryInkB
        {
            get => _SharedSystemMemory;
        }

        /// <inheritdoc />
        public uint CurrentAvailableDedicatedVideoMemoryInkB
        {
            get => _CurrentAvailableDedicatedVideoMemory;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return
                $"{AvailableDedicatedVideoMemoryInkB / 1024} MB ({CurrentAvailableDedicatedVideoMemoryInkB / 1024} MB) / {DedicatedVideoMemoryInkB / 1024} MB";
        }
    }
}