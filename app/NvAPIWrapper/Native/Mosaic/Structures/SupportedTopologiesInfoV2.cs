using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Interfaces;
using NvAPIWrapper.Native.Interfaces.Mosaic;

namespace NvAPIWrapper.Native.Mosaic.Structures
{
    /// <summary>
    ///     Holds information about supported topologies
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(2)]
    public struct SupportedTopologiesInfoV2 : ISupportedTopologiesInfo,
        IInitializable,
        IEquatable<SupportedTopologiesInfoV2>
    {
        /// <summary>
        ///     Maximum number of display settings possible to retrieve
        /// </summary>
        public const int MaxSettings = SupportedTopologiesInfoV1.MaxSettings;

        internal StructureVersion _Version;
        internal readonly uint _TopologyBriefsCount;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int) Topology.Max)]
        internal readonly TopologyBrief[]
            _TopologyBriefs;

        internal readonly uint _DisplaySettingsCount;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxSettings)]
        internal readonly DisplaySettingsV2[]
            _DisplaySettings;

        /// <inheritdoc />
        public bool Equals(SupportedTopologiesInfoV2 other)
        {
            return _TopologyBriefsCount == other._TopologyBriefsCount &&
                   _TopologyBriefs.SequenceEqual(other._TopologyBriefs) &&
                   _DisplaySettingsCount == other._DisplaySettingsCount &&
                   _DisplaySettings.SequenceEqual(other._DisplaySettings);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is SupportedTopologiesInfoV2 v2 && Equals(v2);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) _TopologyBriefsCount;
                hashCode = (hashCode * 397) ^ (_TopologyBriefs?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (int) _DisplaySettingsCount;
                hashCode = (hashCode * 397) ^ (_DisplaySettings?.GetHashCode() ?? 0);

                return hashCode;
            }
        }

        /// <inheritdoc />
        public IEnumerable<TopologyBrief> TopologyBriefs
        {
            get => _TopologyBriefs.Take((int) _TopologyBriefsCount);
        }

        /// <inheritdoc />
        public IEnumerable<IDisplaySettings> DisplaySettings
        {
            get => _DisplaySettings.Take((int) _DisplaySettingsCount).Cast<IDisplaySettings>();
        }
    }
}