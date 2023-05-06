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
    public struct PathTargetInfoV2 : IPathTargetInfo,
        IInitializable,
        IDisposable,
        IAllocatable,
        IEquatable<PathTargetInfoV2>,
        IEquatable<PathTargetInfoV1>
    {
        internal readonly uint _DisplayId;
        internal ValueTypeReference<PathAdvancedTargetInfo> _Details;
        internal readonly uint _WindowsCCDTargetId;

        /// <inheritdoc />
        public uint DisplayId
        {
            get => _DisplayId;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"PathTargetInfoV2: Display #{_DisplayId}";
        }

        /// <inheritdoc />
        public PathAdvancedTargetInfo? Details
        {
            get => _Details.ToValueType();
        }

        /// <summary>
        ///     Windows CCD target ID. Must be present only for non-NVIDIA adapter, for NVIDIA adapter this parameter is ignored.
        /// </summary>
        public uint WindowsCCDTargetId
        {
            get => _WindowsCCDTargetId;
        }

        /// <summary>
        ///     Creates a new PathTargetInfoV1
        /// </summary>
        /// <param name="displayId">Display Id</param>
        public PathTargetInfoV2(uint displayId) : this()
        {
            _DisplayId = displayId;
        }

        /// <inheritdoc />
        public bool Equals(PathTargetInfoV2 other)
        {
            return _DisplayId == other._DisplayId && _Details.Equals(other._Details);
        }

        /// <inheritdoc />
        public bool Equals(PathTargetInfoV1 other)
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

            return obj is PathTargetInfoV2 v2 && Equals(v2);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) _DisplayId;
                // ReSharper disable once NonReadonlyMemberInGetHashCode
                hashCode = (hashCode * 397) ^ _Details.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) _WindowsCCDTargetId;

                return hashCode;
            }
        }

        /// <summary>
        ///     Creates a new PathTargetInfoV1
        /// </summary>
        /// <param name="displayId">Display Id</param>
        /// <param name="windowsCCDTargetId">Windows CCD target Id</param>
        public PathTargetInfoV2(uint displayId, uint windowsCCDTargetId) : this(displayId)
        {
            _WindowsCCDTargetId = windowsCCDTargetId;
        }

        /// <summary>
        ///     Creates a new PathTargetInfoV1
        /// </summary>
        /// <param name="displayId">Display Id</param>
        /// <param name="details">Extra information</param>
        public PathTargetInfoV2(uint displayId, PathAdvancedTargetInfo details) : this(displayId)
        {
            _Details = ValueTypeReference<PathAdvancedTargetInfo>.FromValueType(details);
        }

        /// <summary>
        ///     Creates a new PathTargetInfoV1
        /// </summary>
        /// <param name="displayId">Display Id</param>
        /// <param name="windowsCCDTargetId">Windows CCD target Id</param>
        /// <param name="details">Extra information</param>
        public PathTargetInfoV2(uint displayId, uint windowsCCDTargetId, PathAdvancedTargetInfo details)
            : this(displayId, windowsCCDTargetId)
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