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
    [StructureVersion(1)]
    // ReSharper disable once RedundantExtendsListEntry
    public struct PathInfoV1 : IPathInfo, IInitializable, IAllocatable, IEquatable<PathInfoV1>
    {
        internal StructureVersion _Version;
        internal readonly uint _ReservedSourceId;
        internal readonly uint _TargetInfoCount;
        internal ValueTypeArray<PathTargetInfoV1> _TargetsInfo;
        internal ValueTypeReference<SourceModeInfo> _SourceModeInfo;

        /// <inheritdoc />
        public uint SourceId
        {
            get => _ReservedSourceId;
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
        ///     Creates a new PathInfoV1
        /// </summary>
        /// <param name="targetsInformation">Information about path targets</param>
        /// <param name="sourceModeInformation">Source mode information</param>
        /// <param name="sourceId">Source Id, can be zero</param>
        public PathInfoV1(
            PathTargetInfoV1[] targetsInformation,
            SourceModeInfo sourceModeInformation,
            uint sourceId = 0)
        {
            this = typeof(PathInfoV1).Instantiate<PathInfoV1>();
            _TargetInfoCount = (uint) targetsInformation.Length;
            _TargetsInfo = ValueTypeArray<PathTargetInfoV1>.FromArray(targetsInformation);
            _SourceModeInfo = ValueTypeReference<SourceModeInfo>.FromValueType(sourceModeInformation);
            _ReservedSourceId = sourceId;
        }

        /// <inheritdoc />
        public bool Equals(PathInfoV1 other)
        {
            return _TargetInfoCount == other._TargetInfoCount &&
                   _TargetsInfo.Equals(other._TargetsInfo) &&
                   _SourceModeInfo.Equals(other._SourceModeInfo);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is PathInfoV1 && Equals((PathInfoV1) obj);
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

                return hashCode;
            }
        }

        /// <summary>
        ///     Creates a new PathInfoV1
        /// </summary>
        /// <param name="targetsInformation">Information about path targets</param>
        /// <param name="sourceId">Source Id, can be zero</param>
        public PathInfoV1(PathTargetInfoV1[] targetsInformation, uint sourceId = 0)
        {
            this = typeof(PathInfoV1).Instantiate<PathInfoV1>();
            _TargetInfoCount = (uint) targetsInformation.Length;
            _TargetsInfo = ValueTypeArray<PathTargetInfoV1>.FromArray(targetsInformation);
            _SourceModeInfo = ValueTypeReference<SourceModeInfo>.Null;
            _ReservedSourceId = sourceId;
        }

        /// <summary>
        ///     Creates a new PathInfoV1
        /// </summary>
        /// <param name="sourceId">Source Id, can be zero</param>
        public PathInfoV1(uint sourceId)
        {
            this = typeof(PathInfoV1).Instantiate<PathInfoV1>();
            _TargetInfoCount = 0;
            _TargetsInfo = ValueTypeArray<PathTargetInfoV1>.Null;
            _SourceModeInfo = ValueTypeReference<SourceModeInfo>.Null;
            _ReservedSourceId = sourceId;
        }

        /// <summary>
        ///     Creates a new PathInfoV1
        /// </summary>
        /// <param name="sourceModeInfo">Source mode information</param>
        /// <param name="sourceId">Source Id, can be zero</param>
        public PathInfoV1(SourceModeInfo sourceModeInfo, uint sourceId)
        {
            this = typeof(PathInfoV1).Instantiate<PathInfoV1>();
            _TargetInfoCount = 0;
            _TargetsInfo = ValueTypeArray<PathTargetInfoV1>.Null;
            _SourceModeInfo = ValueTypeReference<SourceModeInfo>.FromValueType(sourceModeInfo);
            _ReservedSourceId = sourceId;
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
                var targetInfo = typeof(PathTargetInfoV1).Instantiate<PathTargetInfoV1>();
                var targetInfoList = targetInfo.Repeat((int) _TargetInfoCount).AllocateAll();
                _TargetsInfo = ValueTypeArray<PathTargetInfoV1>.FromArray(targetInfoList.ToArray());
            }

            if (_SourceModeInfo.IsNull)
            {
                var sourceModeInfo = typeof(SourceModeInfo).Instantiate<SourceModeInfo>();
                _SourceModeInfo = ValueTypeReference<SourceModeInfo>.FromValueType(sourceModeInfo);
            }
        }
    }
}