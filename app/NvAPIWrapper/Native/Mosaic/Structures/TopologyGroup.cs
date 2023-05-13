using System;
using System.Linq;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.Mosaic.Structures
{
    /// <summary>
    ///     This structure defines a group of topologies that work together to create one overall layout.  All of the supported
    ///     topologies are represented with this structure.
    ///     For example, a 'Passive Stereo' topology would be represented with this structure, and would have separate topology
    ///     details for the left and right eyes. The count would be 2. A 'Basic' topology is also represented by this
    ///     structure, with a count of 1.
    ///     The structure is primarily used internally, but is exposed to applications in a read-only fashion because there are
    ///     some details in it that might be useful (like the number of rows/cols, or connected display information).  A user
    ///     can get the filled-in structure by calling NvAPI_Mosaic_GetTopoGroup().
    ///     You can then look at the detailed values within the structure.  There are no entry points which take this structure
    ///     as input (effectively making it read-only).
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct TopologyGroup : IInitializable, IEquatable<TopologyGroup>
    {
        /// <summary>
        ///     Maximum number of topologies per each group
        /// </summary>
        public const int MaxTopologyPerGroup = 2;

        internal StructureVersion _Version;
        internal readonly TopologyBrief _Brief;
        internal readonly uint _TopologiesCount;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxTopologyPerGroup)]
        internal readonly TopologyDetails[]
            _TopologyDetails;

        /// <summary>
        ///     The brief details of this topology
        /// </summary>
        public TopologyBrief Brief
        {
            get => _Brief;
        }

        /// <summary>
        ///     Information about the topologies within this group
        /// </summary>
        public TopologyDetails[] TopologyDetails
        {
            get => _TopologyDetails.Take((int) _TopologiesCount).ToArray();
        }

        /// <inheritdoc />
        public bool Equals(TopologyGroup other)
        {
            return _Brief.Equals(other._Brief) &&
                   _TopologiesCount == other._TopologiesCount &&
                   _TopologyDetails.SequenceEqual(other._TopologyDetails);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is TopologyGroup group && Equals(group);
        }

        /// <summary>
        ///     Checks for equality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are equal, otherwise false</returns>
        public static bool operator ==(TopologyGroup left, TopologyGroup right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(TopologyGroup left, TopologyGroup right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _Brief.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) _TopologiesCount;
                hashCode = (hashCode * 397) ^ (_TopologyDetails?.GetHashCode() ?? 0);

                return hashCode;
            }
        }
    }
}