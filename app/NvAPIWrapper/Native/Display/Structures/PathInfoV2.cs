using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Helpers.Structures;
using NvAPIWrapper.Native.Interfaces;
using NvAPIWrapper.Native.Interfaces.Display;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <summary>
    ///     Holds information about a path
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(2)]
    // ReSharper disable once RedundantExtendsListEntry
    public struct PathInfoV2 : IPathInfo, IInitializable, IAllocatable, IEquatable<PathInfoV2>
    {
        internal StructureVersion _Version;
        internal readonly uint _SourceId;
        internal readonly uint _TargetInfoCount;
        internal ValueTypeArray<PathTargetInfoV2> _TargetsInfo;
        internal ValueTypeReference<SourceModeInfo> _SourceModeInfo;
        internal readonly uint _RawReserved;
        internal ValueTypeReference<LUID> _OSAdapterLUID;

        /// <inheritdoc />
        public uint SourceId
        {
            get => _SourceId;
        }

        /// <inheritdoc />
        public bool Equals(PathInfoV2 other)
        {
            return _TargetInfoCount == other._TargetInfoCount &&
                   _TargetsInfo.Equals(other._TargetsInfo) &&
                   _SourceModeInfo.Equals(other._SourceModeInfo) &&
                   _RawReserved == other._RawReserved;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is PathInfoV2 v2 && Equals(v2);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) _TargetInfoCount;
                // ReSharper disable once NonReadonlyMemberInGetHashCode
                hashCode = (hashCode * 397) ^ _TargetsInfo.GetHashCode();
                // ReSharper disable once NonReadonlyMemberInGetHashCode
                hashCode = (hashCode * 397) ^ _SourceModeInfo.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) _RawReserved;

                return hashCode;
            }
        }

        /// <inheritdoc />
        public IEnumerable<IPathTargetInfo> TargetsInfo
        {
            get => _TargetsInfo.ToArray((int) _TargetInfoCount)?.Cast<IPathTargetInfo>() ?? new IPathTargetInfo[0];
        }

        /// <inheritdoc />
        public SourceModeInfo SourceModeInfo
        {
            get => _SourceModeInfo.ToValueType() ?? default(SourceModeInfo);
        }

        /// <summary>
        ///     True for non-NVIDIA adapter.
        /// </summary>
        public bool IsNonNVIDIAAdapter
        {
            get => _RawReserved.GetBit(0);
        }

        /// <summary>
        ///     Used by Non-NVIDIA adapter for OS Adapter of LUID
        /// </summary>
        public LUID? OSAdapterLUID
        {
            get => _OSAdapterLUID.ToValueType();
        }

        /// <summary>
        ///     Creates a new PathInfoV2
        /// </summary>
        /// <param name="targetInformations">Information about path targets</param>
        /// <param name="sourceModeInfo">Source mode information</param>
        /// <param name="sourceId">Source Id, can be zero</param>
        public PathInfoV2(PathTargetInfoV2[] targetInformations, SourceModeInfo sourceModeInfo, uint sourceId = 0)
        {
            this = typeof(PathInfoV2).Instantiate<PathInfoV2>();
            _TargetInfoCount = (uint) targetInformations.Length;
            _TargetsInfo = ValueTypeArray<PathTargetInfoV2>.FromArray(targetInformations);
            _SourceModeInfo = ValueTypeReference<SourceModeInfo>.FromValueType(sourceModeInfo);
            _SourceId = sourceId;
        }

        /// <summary>
        ///     Creates a new PathInfoV2
        /// </summary>
        /// <param name="targetInformations">Information about path targets</param>
        /// <param name="sourceId">Source Id, can be zero</param>
        public PathInfoV2(PathTargetInfoV2[] targetInformations, uint sourceId = 0)
        {
            this = typeof(PathInfoV2).Instantiate<PathInfoV2>();
            _TargetInfoCount = (uint) targetInformations.Length;
            _TargetsInfo = ValueTypeArray<PathTargetInfoV2>.FromArray(targetInformations);
            _SourceModeInfo = ValueTypeReference<SourceModeInfo>.Null;
            _SourceId = sourceId;
        }


        /// <summary>
        ///     Creates a new PathInfoV2
        /// </summary>
        /// <param name="sourceId">Source Id, can be zero</param>
        public PathInfoV2(uint sourceId)
        {
            this = typeof(PathInfoV2).Instantiate<PathInfoV2>();
            _TargetInfoCount = 0;
            _TargetsInfo = ValueTypeArray<PathTargetInfoV2>.Null;
            _SourceModeInfo = ValueTypeReference<SourceModeInfo>.Null;
            _SourceId = sourceId;
        }

        /// <summary>
        ///     Creates a new PathInfoV2
        /// </summary>
        /// <param name="sourceModeInfo">Source mode information</param>
        /// <param name="sourceId">Source Id, can be zero</param>
        public PathInfoV2(SourceModeInfo sourceModeInfo, uint sourceId)
        {
            this = typeof(PathInfoV2).Instantiate<PathInfoV2>();
            _TargetInfoCount = 0;
            _TargetsInfo = ValueTypeArray<PathTargetInfoV2>.Null;
            _SourceModeInfo = ValueTypeReference<SourceModeInfo>.FromValueType(sourceModeInfo);
            _SourceId = sourceId;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            TargetsInfo.DisposeAll();
            _TargetsInfo.Dispose();
            _SourceModeInfo.Dispose();
        }

        void IAllocatable.Allocate()
        {
            if (_TargetInfoCount > 0 && _TargetsInfo.IsNull)
            {
                var targetInfo = typeof(PathTargetInfoV2).Instantiate<PathTargetInfoV2>();
                var targetInfoList = targetInfo.Repeat((int) _TargetInfoCount).AllocateAll();
                _TargetsInfo = ValueTypeArray<PathTargetInfoV2>.FromArray(targetInfoList.ToArray());
            }

            if (_SourceModeInfo.IsNull)
            {
                var sourceModeInfo = typeof(SourceModeInfo).Instantiate<SourceModeInfo>();
                _SourceModeInfo = ValueTypeReference<SourceModeInfo>.FromValueType(sourceModeInfo);
            }
        }
    }
}