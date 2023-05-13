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
    [StructureVersion(3)]
    public struct DisplayDriverMemoryInfoV3 : IInitializable, IDisplayDriverMemoryInfo
    {
        internal StructureVersion _Version;
        internal readonly uint _DedicatedVideoMemory;
        internal readonly uint _AvailableDedicatedVideoMemory;
        internal readonly uint _SystemVideoMemory;
        internal readonly uint _SharedSystemMemory;
        internal readonly uint _CurrentAvailableDedicatedVideoMemory;
        internal readonly uint _DedicatedVideoMemoryEvictionsSize;
        internal readonly uint _DedicatedVideoMemoryEvictionCount;

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

        /// <summary>
        ///     Size(in kb) of the total size of memory released as a result of the evictions.
        /// </summary>
        public uint DedicatedVideoMemoryEvictionsSize
        {
            get => _DedicatedVideoMemoryEvictionsSize;
        }

        /// <summary>
        ///     Indicates the number of eviction events that caused an allocation to be removed from dedicated video memory to free
        ///     GPU video memory to make room for other allocations.
        /// </summary>
        public uint DedicatedVideoMemoryEvictionCount
        {
            get => _DedicatedVideoMemoryEvictionCount;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return
                $"{AvailableDedicatedVideoMemoryInkB / 1024} MB ({CurrentAvailableDedicatedVideoMemoryInkB / 1024} MB) / {DedicatedVideoMemoryInkB / 1024} MB";
        }
    }
}