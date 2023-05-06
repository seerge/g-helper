using System.Linq;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.GPU.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.Mosaic.Structures
{
    /// <summary>
    ///     Holds information about a topology validity status
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct DisplayTopologyStatus : IInitializable
    {
        /// <summary>
        ///     Maximum number of displays for this structure
        /// </summary>
        public const int MaxDisplays =
            PhysicalGPUHandle.PhysicalGPUs * Constants.Display.AdvancedDisplayHeads;

        internal StructureVersion _Version;
        internal readonly DisplayCapacityProblem _Errors;
        internal readonly DisplayTopologyWarning _Warnings;
        internal readonly uint _DisplayCounts;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxDisplays)]
        internal Display[] _Displays;

        /// <summary>
        ///     Gets all error flags for this topology
        /// </summary>
        public DisplayCapacityProblem Errors
        {
            get => _Errors;
        }

        /// <summary>
        ///     Gets all warning flags for this topology
        /// </summary>
        public DisplayTopologyWarning Warnings
        {
            get => _Warnings;
        }

        /// <summary>
        ///     Gets per display statuses
        /// </summary>
        public Display[] Displays
        {
            get => _Displays.Take((int) _DisplayCounts).ToArray();
        }

        /// <summary>
        ///     Holds information about a display validity status in a topology
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct Display
        {
            internal uint _DisplayId;
            internal DisplayCapacityProblem _Errors;
            internal DisplayTopologyWarning _Warnings;
            internal uint _RawReserved;

            /// <summary>
            ///     Gets the Display identification of this display.
            /// </summary>
            public uint DisplayId
            {
                get => _DisplayId;
            }

            /// <summary>
            ///     Gets all error flags for this display
            /// </summary>
            public DisplayCapacityProblem Errors
            {
                get => _Errors;
            }

            /// <summary>
            ///     Gets all warning flags for this display
            /// </summary>
            public DisplayTopologyWarning Warnings
            {
                get => _Warnings;
            }

            /// <summary>
            ///     Indicates if this display can be rotated
            /// </summary>
            public bool SupportsRotation
            {
                get => _RawReserved.GetBit(0);
            }
        }
    }
}