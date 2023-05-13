using System.Linq;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Contains information regarding GPU thermal policies
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(2)]
    public struct PrivateThermalPoliciesInfoV2 : IInitializable
    {
        internal const int MaxNumberOfThermalPoliciesInfoEntries = 4;

        internal StructureVersion _Version;
        internal readonly byte _ThermalPoliciesInfoCount;
        internal readonly byte _Unknown;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxNumberOfThermalPoliciesInfoEntries,
            ArraySubType = UnmanagedType.Struct)]
        internal readonly ThermalPoliciesInfoEntry[] _ThermalPoliciesInfoEntries;

        /// <summary>
        ///     Gets a list of thermal policy entries
        /// </summary>
        public ThermalPoliciesInfoEntry[] ThermalPoliciesInfoEntries
        {
            get => _ThermalPoliciesInfoEntries.Take(_ThermalPoliciesInfoCount).ToArray();
        }

        /// <summary>
        ///     Contains information regarding a thermal policies entry
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct ThermalPoliciesInfoEntry
        {
            internal ThermalController _Controller;
            internal uint _Unknown1;
            internal int _MinimumTemperature;
            internal int _DefaultTemperature;
            internal int _MaximumTemperature;
            internal uint _Unknown2;

            /// <summary>
            ///     Gets the thermal controller
            /// </summary>
            public ThermalController Controller
            {
                get => _Controller;
            }

            /// <summary>
            ///     Gets the minimum temperature limit target
            /// </summary>
            public int MinimumTemperature
            {
                get => _MinimumTemperature >> 8;
            }

            /// <summary>
            ///     Gets the default temperature limit target
            /// </summary>
            public int DefaultTemperature
            {
                get => _DefaultTemperature >> 8;
            }

            /// <summary>
            ///     Gets the maximum temperature limit target
            /// </summary>
            public int MaximumTemperature
            {
                get => _MaximumTemperature >> 8;
            }
        }
    }
}