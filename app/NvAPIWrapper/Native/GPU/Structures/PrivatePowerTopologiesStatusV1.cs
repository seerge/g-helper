using System.Linq;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Contains information regarding GPU power topology status
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct PrivatePowerTopologiesStatusV1 : IInitializable
    {
        internal const int MaxNumberOfPowerTopologiesStatusEntries = 4;

        internal StructureVersion _Version;
        internal readonly uint _PowerTopologiesStatusEntriesCount;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxNumberOfPowerTopologiesStatusEntries,
            ArraySubType = UnmanagedType.Struct)]
        internal readonly PowerTopologiesStatusEntry[] _PowerTopologiesStatusEntries;

        /// <summary>
        ///     Gets a list of power topology status entries
        /// </summary>
        public PowerTopologiesStatusEntry[] PowerPolicyStatusEntries
        {
            get => _PowerTopologiesStatusEntries.Take((int) _PowerTopologiesStatusEntriesCount).ToArray();
        }

        /// <summary>
        ///     Contains information regarding a power topology status entry
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct PowerTopologiesStatusEntry
        {
            internal PowerTopologyDomain _Domain;
            internal uint _Unknown2;
            internal uint _PowerUsageInPCM;
            internal uint _Unknown3;

            /// <summary>
            ///     Gets the power topology domain
            /// </summary>
            public PowerTopologyDomain Domain
            {
                get => _Domain;
            }

            /// <summary>
            ///     Gets the power usage in per cent mille
            /// </summary>
            public uint PowerUsageInPCM
            {
                get => _PowerUsageInPCM;
            }
        }
    }
}