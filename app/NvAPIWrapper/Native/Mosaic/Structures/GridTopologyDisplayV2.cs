using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.Display;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;
using NvAPIWrapper.Native.Interfaces.Mosaic;

namespace NvAPIWrapper.Native.Mosaic.Structures
{
    /// <summary>
    ///     Holds information about a display in a grid topology
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion]
    public struct GridTopologyDisplayV2 : IGridTopologyDisplay, IInitializable, IEquatable<GridTopologyDisplayV2>
    {
        internal StructureVersion _Version;
        internal readonly uint _DisplayId;
        internal readonly int _OverlapX;
        internal readonly int _OverlapY;
        internal readonly Rotate _Rotation;
        internal readonly uint _CloneGroup;
        internal readonly PixelShiftType _PixelShiftType;

        /// <summary>
        ///     Creates a new GridTopologyDisplayV2
        /// </summary>
        /// <param name="displayId">Display identification</param>
        /// <param name="overlapX">Horizontal overlap (+overlap, -gap)</param>
        /// <param name="overlapY">Vertical overlap (+overlap, -gap)</param>
        /// <param name="rotation">Rotation of display</param>
        /// <param name="cloneGroup">Clone group identification; Reserved, must be 0</param>
        /// <param name="pixelShiftType">Type of the pixel shift enabled display</param>
        // ReSharper disable once TooManyDependencies
        public GridTopologyDisplayV2(
            uint displayId,
            int overlapX,
            int overlapY,
            Rotate rotation,
            uint cloneGroup = 0,
            PixelShiftType pixelShiftType = PixelShiftType.NoPixelShift) : this()
        {
            this = typeof(GridTopologyDisplayV2).Instantiate<GridTopologyDisplayV2>();
            _DisplayId = displayId;
            _OverlapX = overlapX;
            _OverlapY = overlapY;
            _Rotation = rotation;
            _CloneGroup = cloneGroup;
            _PixelShiftType = pixelShiftType;
        }

        /// <inheritdoc />
        public bool Equals(GridTopologyDisplayV2 other)
        {
            return _DisplayId == other._DisplayId &&
                   _OverlapX == other._OverlapX &&
                   _OverlapY == other._OverlapY &&
                   _Rotation == other._Rotation &&
                   _CloneGroup == other._CloneGroup &&
                   _PixelShiftType == other._PixelShiftType;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is GridTopologyDisplayV2 v2 && Equals(v2);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) _DisplayId;
                hashCode = (hashCode * 397) ^ _OverlapX;
                hashCode = (hashCode * 397) ^ _OverlapY;
                hashCode = (hashCode * 397) ^ (int) _Rotation;
                hashCode = (hashCode * 397) ^ (int) _CloneGroup;
                hashCode = (hashCode * 397) ^ (int) _PixelShiftType;

                return hashCode;
            }
        }

        /// <summary>
        ///     Checks for equality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are equal, otherwise false</returns>
        public static bool operator ==(GridTopologyDisplayV2 left, GridTopologyDisplayV2 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(GridTopologyDisplayV2 left, GridTopologyDisplayV2 right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc />
        public uint DisplayId
        {
            get => _DisplayId;
        }

        /// <inheritdoc />
        public int OverlapX
        {
            get => _OverlapX;
        }

        /// <inheritdoc />
        public int OverlapY
        {
            get => _OverlapY;
        }

        /// <inheritdoc />
        public Rotate Rotation
        {
            get => _Rotation;
        }

        /// <inheritdoc />
        public uint CloneGroup
        {
            get => _CloneGroup;
        }

        /// <inheritdoc />
        public PixelShiftType PixelShiftType
        {
            get => _PixelShiftType;
        }
    }
}