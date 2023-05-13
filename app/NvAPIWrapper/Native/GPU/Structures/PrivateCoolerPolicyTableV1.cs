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
    ///     Contains information regarding GPU cooler policy table
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct PrivateCoolerPolicyTableV1 : IInitializable
    {
        internal const int MaxNumberOfPolicyLevels = 24;

        internal StructureVersion _Version;
        internal CoolerPolicy _Policy;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxNumberOfPolicyLevels)]
        internal readonly CoolerPolicyTableEntry[] _TableEntries;

        /// <summary>
        ///     Gets an array of policy table entries
        /// </summary>
        /// <param name="count">The number of table entries.</param>
        /// <returns>An array of <see cref="CoolerPolicyTableEntry" /> instances.</returns>
        public CoolerPolicyTableEntry[] TableEntries(int count)
        {
            return _TableEntries.Take(count).ToArray();
        }

        /// <summary>
        ///     Gets the table cooler policy
        /// </summary>
        public CoolerPolicy Policy
        {
            get => _Policy;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="PrivateCoolerPolicyTableV1" />
        /// </summary>
        /// <param name="policy">The table cooler policy.</param>
        /// <param name="policyTableEntries">An array of table entries.</param>
        public PrivateCoolerPolicyTableV1(CoolerPolicy policy, CoolerPolicyTableEntry[] policyTableEntries)
        {
            if (policyTableEntries?.Length > MaxNumberOfPolicyLevels)
            {
                throw new ArgumentException($"Maximum of {MaxNumberOfPolicyLevels} policy levels are configurable.",
                    nameof(policyTableEntries));
            }

            if (policyTableEntries == null || policyTableEntries.Length == 0)
            {
                throw new ArgumentException("Array is null or empty.", nameof(policyTableEntries));
            }

            this = typeof(PrivateCoolerPolicyTableV1).Instantiate<PrivateCoolerPolicyTableV1>();
            _Policy = policy;
            Array.Copy(policyTableEntries, 0, _TableEntries, 0, policyTableEntries.Length);
        }

        /// <summary>
        ///     Contains information regarding a clock boost mask
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct CoolerPolicyTableEntry
        {
            internal uint _EntryId;
            internal uint _CurrentLevel;
            internal uint _DefaultLevel;

            /// <summary>
            ///     Gets the entry identification number
            /// </summary>
            public uint EntryId
            {
                get => _EntryId;
            }

            /// <summary>
            ///     Gets the current level in percentage
            /// </summary>
            public uint CurrentLevel
            {
                get => _CurrentLevel;
            }

            /// <summary>
            ///     Gets the default level in percentage
            /// </summary>
            public uint DefaultLevel
            {
                get => _DefaultLevel;
            }

            /// <summary>
            ///     Creates a new instance of <see cref="CoolerPolicyTableEntry" />.
            /// </summary>
            /// <param name="entryId">The entry identification number.</param>
            /// <param name="currentLevel">The current level in percentage.</param>
            /// <param name="defaultLevel">The default level in percentage.</param>
            public CoolerPolicyTableEntry(uint entryId, uint currentLevel, uint defaultLevel)
            {
                _EntryId = entryId;
                _CurrentLevel = currentLevel;
                _DefaultLevel = defaultLevel;
            }
        }
    }
}