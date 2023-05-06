using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Contains information regarding GPU PCI-e connection configurations
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(2)]
    public struct PrivatePCIeInfoV2 : IInitializable
    {
        internal StructureVersion _Version;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        internal readonly PCIePerformanceStateInfo[] _PCIePerformanceStateInfos;

        /// <summary>
        ///     Gets the list of performance state PCI-e configurations information
        /// </summary>
        public PCIePerformanceStateInfo[] PCIePerformanceStateInfos
        {
            get => _PCIePerformanceStateInfos;
        }

        /// <summary>
        ///     Contains information regarding a performance state PCI-e connection
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct PCIePerformanceStateInfo
        {
            internal readonly uint _TransferRate;
            internal readonly PCIeGeneration _Version;
            internal readonly uint _LanesNumber;
            internal readonly PCIeGeneration _Generation;

            /// <summary>
            ///     Gets the PCI-e transfer rate in Mega Transfers per Second
            /// </summary>
            public uint TransferRateInMTps
            {
                get => _TransferRate;
            }

            /// <summary>
            ///     Gets the PCI-e generation
            /// </summary>
            public PCIeGeneration Generation
            {
                get => _Generation;
            }

            /// <summary>
            ///     Gets the PCI-e down stream lanes
            /// </summary>
            public uint Lanes
            {
                get => _LanesNumber;
            }

            /// <summary>
            ///     Gets the PCI-e version
            /// </summary>
            public PCIeGeneration Version
            {
                get => _Version;
            }
        }
    }
}