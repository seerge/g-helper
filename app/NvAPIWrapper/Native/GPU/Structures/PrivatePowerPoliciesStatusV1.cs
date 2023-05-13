using System;
using System.Linq;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Contains information regarding GPU power policies status
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct PrivatePowerPoliciesStatusV1 : IInitializable
    {
        internal const int MaxNumberOfPowerPoliciesStatusEntries = 4;

        internal StructureVersion _Version;
        internal readonly uint _PowerPoliciesStatusEntriesCount;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxNumberOfPowerPoliciesStatusEntries,
            ArraySubType = UnmanagedType.Struct)]
        internal readonly PowerPolicyStatusEntry[] _PowerPoliciesStatusEntries;

        /// <summary>
        ///     Gets a list of power policy status entries
        /// </summary>
        public PowerPolicyStatusEntry[] PowerPolicyStatusEntries
        {
            get => _PowerPoliciesStatusEntries.Take((int) _PowerPoliciesStatusEntriesCount).ToArray();
        }

        /// <summary>
        ///     Creates a new instance of <see cref="PrivatePowerPoliciesStatusV1" />
        /// </summary>
        /// <param name="powerPoliciesStatusEntries">The list of power policy status entries.</param>
        public PrivatePowerPoliciesStatusV1(PowerPolicyStatusEntry[] powerPoliciesStatusEntries)
        {
            if (powerPoliciesStatusEntries?.Length > MaxNumberOfPowerPoliciesStatusEntries)
            {
                throw new ArgumentException(
                    $"Maximum of {MaxNumberOfPowerPoliciesStatusEntries} power policies entries are configurable.",
                    nameof(powerPoliciesStatusEntries)
                );
            }

            if (powerPoliciesStatusEntries == null || powerPoliciesStatusEntries.Length == 0)
            {
                throw new ArgumentException("Array is null or empty.", nameof(powerPoliciesStatusEntries));
            }

            this = typeof(PrivatePowerPoliciesStatusV1).Instantiate<PrivatePowerPoliciesStatusV1>();
            _PowerPoliciesStatusEntriesCount = (uint) powerPoliciesStatusEntries.Length;
            Array.Copy(
                powerPoliciesStatusEntries,
                0,
                _PowerPoliciesStatusEntries,
                0,
                powerPoliciesStatusEntries.Length
            );
        }

        /// <summary>
        ///     Contains information regarding a power policies status entry
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct PowerPolicyStatusEntry
        {
            internal PerformanceStateId _PerformanceStateId;
            internal uint _Unknown1;
            internal uint _PowerTargetInPCM;
            internal uint _Unknown2;

            /// <summary>
            ///     Gets the performance state identification number
            /// </summary>
            public PerformanceStateId PerformanceStateId
            {
                get => _PerformanceStateId;
            }

            /// <summary>
            ///     Creates a new instance of PowerPolicyStatusEntry.
            /// </summary>
            /// <param name="powerTargetInPCM">The power limit target in per cent mille.</param>
            public PowerPolicyStatusEntry(uint powerTargetInPCM) : this()
            {
                _PowerTargetInPCM = powerTargetInPCM;
            }

            /// <summary>
            ///     Gets the power limit target in per cent mille
            /// </summary>
            public uint PowerTargetInPCM
            {
                get => _PowerTargetInPCM;
            }
        }
    }
}