using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.Mosaic.Structures
{
    /// <summary>
    ///     Holds brief information about a topology
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct TopologyBrief : IInitializable, IEquatable<TopologyBrief>
    {
        internal StructureVersion _Version;
        internal readonly Topology _Topology;
        internal readonly uint _IsEnable;
        internal readonly uint _IsPossible;

        /// <summary>
        ///     Creates a new TopologyBrief
        /// </summary>
        /// <param name="topology">The topology</param>
        public TopologyBrief(Topology topology)
        {
            this = typeof(TopologyBrief).Instantiate<TopologyBrief>();
            _Topology = topology;
        }

        /// <inheritdoc />
        public bool Equals(TopologyBrief other)
        {
            return _Topology == other._Topology;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is TopologyBrief brief && Equals(brief);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (int) _Topology;
        }

        /// <summary>
        ///     Checks for equality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are equal, otherwise false</returns>
        public static bool operator ==(TopologyBrief left, TopologyBrief right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(TopologyBrief left, TopologyBrief right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        ///     The topology
        /// </summary>
        public Topology Topology
        {
            get => _Topology;
        }

        /// <summary>
        ///     Indicates if the topology is enable
        /// </summary>
        public bool IsEnable
        {
            get => _IsEnable > 0;
        }

        /// <summary>
        ///     Indicates if the topology is possible
        /// </summary>
        public bool IsPossible
        {
            get => _IsPossible > 0;
        }
    }
}