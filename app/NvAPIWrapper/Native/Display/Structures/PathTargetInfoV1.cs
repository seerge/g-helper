using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Helpers.Structures;
using NvAPIWrapper.Native.Interfaces;
using NvAPIWrapper.Native.Interfaces.Display;

// ReSharper disable RedundantExtendsListEntry

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <summary>
    ///     Holds information about a path's target
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct PathTargetInfoV1 : IPathTargetInfo,
        IInitializable,
        IDisposable,
        IAllocatable,
        IEquatable<PathTargetInfoV1>,
        IEquatable<PathTargetInfoV2>
    {
        internal readonly uint _DisplayId;
        internal ValueTypeReference<PathAdvancedTargetInfo> _Details;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"PathTargetInfoV2: Display #{_DisplayId}";
        }

        /// <inheritdoc />
        public uint DisplayId
        {
            get => _DisplayId;
        }

        /// <inheritdoc />
        public bool Equals(PathTargetInfoV1 other)
        {
            return _DisplayId == other._DisplayId && _Details.Equals(other._Details);
        }

        /// <inheritdoc />
        public bool Equals(PathTargetInfoV2 other)
        {
            return _DisplayId == other._DisplayId && _Details.Equals(other._Details);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is PathTargetInfoV1 && Equals((PathTargetInfoV1) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable once NonReadonlyMemberInGetHashCode
                return ((int) _DisplayId * 397) ^ _Details.GetHashCode();
            }
        }

        /// <inheritdoc />
        public PathAdvancedTargetInfo? Details
        {
            get => _Details.ToValueType() ?? default(PathAdvancedTargetInfo);
        }

        /// <summary>
        ///     Creates a new PathTargetInfoV1
        /// </summary>
        /// <param name="displayId">Display Id</param>
        public PathTargetInfoV1(uint displayId) : this()
        {
            _DisplayId = displayId;
        }

        /// <summary>
        ///     Creates a new PathTargetInfoV1
        /// </summary>
        /// <param name="displayId">Display Id</param>
        /// <param name="details">Extra information</param>
        public PathTargetInfoV1(uint displayId, PathAdvancedTargetInfo details) : this(displayId)
        {
            _Details = ValueTypeReference<PathAdvancedTargetInfo>.FromValueType(details);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _Details.Dispose();
        }

        void IAllocatable.Allocate()
        {
            if (_Details.IsNull)
            {
                var detail = typeof(PathAdvancedTargetInfo).Instantiate<PathAdvancedTargetInfo>();
                _Details = ValueTypeReference<PathAdvancedTargetInfo>.FromValueType(detail);
            }
        }
    }
}