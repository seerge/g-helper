using System;
using System.Linq;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.GPU.Structures;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.Mosaic.Structures
{
    /// <summary>
    ///     Holds extra details about a topology
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct TopologyDetails : IInitializable, IEquatable<TopologyDetails>
    {
        /// <summary>
        ///     Maximum number of rows in a topology detail
        /// </summary>
        public const int MaxLayoutRows = 8;

        /// <summary>
        ///     Maximum number of columns in a topology detail
        /// </summary>
        public const int MaxLayoutColumns = 8;

        internal StructureVersion _Version;
        internal readonly LogicalGPUHandle _LogicalGPUHandle;
        internal readonly TopologyValidity _ValidityFlags;
        internal readonly uint _Rows;
        internal readonly uint _Columns;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxLayoutRows)]
        internal readonly LayoutRow[] _LayoutRows;

        /// <inheritdoc />
        public bool Equals(TopologyDetails other)
        {
            return _LogicalGPUHandle.Equals(other._LogicalGPUHandle) &&
                   _ValidityFlags == other._ValidityFlags &&
                   _Rows == other._Rows &&
                   _Columns == other._Columns &&
                   _LayoutRows.SequenceEqual(other._LayoutRows);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is TopologyDetails details && Equals(details);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _LogicalGPUHandle.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) _ValidityFlags;
                hashCode = (hashCode * 397) ^ (int) _Rows;
                hashCode = (hashCode * 397) ^ (int) _Columns;
                hashCode = (hashCode * 397) ^ (_LayoutRows?.GetHashCode() ?? 0);

                return hashCode;
            }
        }

        /// <summary>
        ///     Checks for equality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are equal, otherwise false</returns>
        public static bool operator ==(TopologyDetails left, TopologyDetails right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(TopologyDetails left, TopologyDetails right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        ///     Logical GPU for this topology
        /// </summary>
        public LogicalGPUHandle LogicalGPUHandle
        {
            get => _LogicalGPUHandle;
        }

        /// <summary>
        ///     Indicates topology validity. TopologyValidity.Valid means topology is valid with the current hardware.
        /// </summary>
        public TopologyValidity ValidityFlags
        {
            get => _ValidityFlags;
        }

        /// <summary>
        ///     Number of displays in a row
        /// </summary>
        public int Rows
        {
            get => (int) _Rows;
        }

        /// <summary>
        ///     Number of displays in a column
        /// </summary>
        public int Columns
        {
            get => (int) _Columns;
        }

        /// <summary>
        ///     Gets a 2D array of layout cells containing information about the display layout of the topology
        /// </summary>
        public LayoutCell[][] Layout
        {
            get
            {
                var columns = (int) _Columns;

                return _LayoutRows.Take((int) _Rows).Select(row => row.LayoutCells.Take(columns).ToArray()).ToArray();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct LayoutRow : IInitializable, IEquatable<LayoutRow>
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxLayoutColumns)]
            internal readonly LayoutCell[]
                LayoutCells;

            public bool Equals(LayoutRow other)
            {
                return LayoutCells.SequenceEqual(other.LayoutCells);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }

                return obj is LayoutRow row && Equals(row);
            }

            public override int GetHashCode()
            {
                return LayoutCells?.GetHashCode() ?? 0;
            }
        }

        /// <summary>
        ///     Holds information about a topology display
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct LayoutCell : IEquatable<LayoutCell>
        {
            internal readonly PhysicalGPUHandle _PhysicalGPUHandle;
            internal readonly OutputId _DisplayOutputId;
            internal readonly int _OverlapX;
            internal readonly int _OverlapY;

            /// <inheritdoc />
            public bool Equals(LayoutCell other)
            {
                return _PhysicalGPUHandle.Equals(other._PhysicalGPUHandle) &&
                       _DisplayOutputId == other._DisplayOutputId &&
                       _OverlapX == other._OverlapX &&
                       _OverlapY == other._OverlapY;
            }

            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }

                return obj is LayoutCell cell && Equals(cell);
            }

            /// <inheritdoc />
            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = _PhysicalGPUHandle.GetHashCode();
                    hashCode = (hashCode * 397) ^ (int) _DisplayOutputId;
                    hashCode = (hashCode * 397) ^ _OverlapX;
                    hashCode = (hashCode * 397) ^ _OverlapY;

                    return hashCode;
                }
            }

            /// <summary>
            ///     Checks for equality between two objects of same type
            /// </summary>
            /// <param name="left">The first object</param>
            /// <param name="right">The second object</param>
            /// <returns>true, if both objects are equal, otherwise false</returns>
            public static bool operator ==(LayoutCell left, LayoutCell right)
            {
                return left.Equals(right);
            }

            /// <summary>
            ///     Checks for inequality between two objects of same type
            /// </summary>
            /// <param name="left">The first object</param>
            /// <param name="right">The second object</param>
            /// <returns>true, if both objects are not equal, otherwise false</returns>
            public static bool operator !=(LayoutCell left, LayoutCell right)
            {
                return !left.Equals(right);
            }

            /// <summary>
            ///     Physical GPU to be used in the topology (0 if GPU missing)
            /// </summary>
            public PhysicalGPUHandle PhysicalGPUHandle
            {
                get => _PhysicalGPUHandle;
            }

            /// <summary>
            ///     Connected display target (0 if no display connected)
            /// </summary>
            public OutputId DisplayOutputId
            {
                get => _DisplayOutputId;
            }

            /// <summary>
            ///     Pixels of overlap on left of target: (+overlap, -gap)
            /// </summary>
            public int OverlapX
            {
                get => _OverlapX;
            }

            /// <summary>
            ///     Pixels of overlap on top of target: (+overlap, -gap)
            /// </summary>
            public int OverlapY
            {
                get => _OverlapY;
            }
        }
    }
}