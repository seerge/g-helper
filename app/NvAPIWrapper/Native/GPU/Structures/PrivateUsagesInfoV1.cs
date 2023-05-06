using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Interfaces;
using NvAPIWrapper.Native.Interfaces.GPU;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Holds information about the GPU usage statistics
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct PrivateUsagesInfoV1 : IInitializable, IUtilizationStatus
    {
        internal const int MaxNumberOfUsageEntries = DynamicPerformanceStatesInfoV1.MaxGpuUtilizations;

        internal StructureVersion _Version;
        internal uint _Unknown;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxNumberOfUsageEntries, ArraySubType = UnmanagedType.Struct)]
        internal UsagesInfoEntry[] _UsagesInfoEntries;

        /// <inheritdoc />
        public Dictionary<UtilizationDomain, IUtilizationDomainInfo> Domains
        {
            get => _UsagesInfoEntries
                .Select((value, index) => new {index, value})
                .Where(arg => Enum.IsDefined(typeof(UtilizationDomain), arg.index) && arg.value.IsPresent)
                .ToDictionary(arg => (UtilizationDomain) arg.index, arg => arg.value as IUtilizationDomainInfo);
        }

        /// <inheritdoc />
        public IUtilizationDomainInfo GPU
        {
            get => _UsagesInfoEntries[(int) UtilizationDomain.GPU];
        }

        /// <inheritdoc />
        public IUtilizationDomainInfo FrameBuffer
        {
            get => _UsagesInfoEntries[(int) UtilizationDomain.FrameBuffer];
        }

        /// <inheritdoc />
        public IUtilizationDomainInfo VideoEngine
        {
            get => _UsagesInfoEntries[(int) UtilizationDomain.VideoEngine];
        }

        /// <inheritdoc />
        public IUtilizationDomainInfo BusInterface
        {
            get => _UsagesInfoEntries[(int) UtilizationDomain.BusInterface];
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"GPU = {GPU} - " +
                   $"FrameBuffer = {FrameBuffer} - " +
                   $"VideoEngine = {VideoEngine} - " +
                   $"BusInterface = {BusInterface}";
        }

        /// <summary>
        ///     Holds information about the usage statistics for a domain
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct UsagesInfoEntry : IUtilizationDomainInfo
        {
            internal uint _IsPresent;
            internal uint _Percentage;
            internal uint _Unknown1;
            internal uint _Unknown2;

            /// <inheritdoc />
            public bool IsPresent
            {
                get => _IsPresent > 0;
            }

            /// <inheritdoc />
            public uint Percentage
            {
                get => _Percentage;
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return IsPresent ? $"{Percentage}%" : "N/A";
            }
        }
    }
}