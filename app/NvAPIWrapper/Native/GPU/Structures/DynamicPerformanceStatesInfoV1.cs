using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;
using NvAPIWrapper.Native.Interfaces.GPU;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Holds information about the dynamic performance states (such as GPU utilization domain)
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct DynamicPerformanceStatesInfoV1 : IInitializable, IUtilizationStatus
    {
        internal const int MaxGpuUtilizations = 8;

        internal StructureVersion _Version;
        internal readonly uint _Flags;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxGpuUtilizations)]
        internal UtilizationDomainInfo[] _UtilizationDomain;

        /// <summary>
        ///     Gets a boolean value indicating if the dynamic performance state is enabled
        /// </summary>
        public bool IsDynamicPerformanceStatesEnabled
        {
            get => _Flags.GetBit(0);
        }

        /// <inheritdoc />
        public Dictionary<UtilizationDomain, IUtilizationDomainInfo> Domains
        {
            get => _UtilizationDomain
                .Select((value, index) => new {index, value})
                .Where(arg => Enum.IsDefined(typeof(UtilizationDomain), arg.index) && arg.value.IsPresent)
                .ToDictionary(arg => (UtilizationDomain) arg.index, arg => arg.value as IUtilizationDomainInfo);
        }

        /// <inheritdoc />
        public IUtilizationDomainInfo GPU
        {
            get => _UtilizationDomain[(int) UtilizationDomain.GPU];
        }

        /// <inheritdoc />
        public IUtilizationDomainInfo FrameBuffer
        {
            get => _UtilizationDomain[(int) UtilizationDomain.FrameBuffer];
        }

        /// <inheritdoc />
        public IUtilizationDomainInfo VideoEngine
        {
            get => _UtilizationDomain[(int) UtilizationDomain.VideoEngine];
        }

        /// <inheritdoc />
        public IUtilizationDomainInfo BusInterface
        {
            get => _UtilizationDomain[(int) UtilizationDomain.BusInterface];
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
        ///     Holds information about a dynamic performance state utilization domain
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct UtilizationDomainInfo : IUtilizationDomainInfo
        {
            internal readonly uint _IsPresent;
            internal readonly uint _Percentage;

            /// <inheritdoc />
            public bool IsPresent
            {
                get => _IsPresent.GetBit(0);
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