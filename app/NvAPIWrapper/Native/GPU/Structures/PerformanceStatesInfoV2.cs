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
    /// <inheritdoc cref="IPerformanceStatesInfo" />
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(2)]
    public struct PerformanceStatesInfoV2 : IInitializable, IPerformanceStatesInfo
    {
        internal const int MaxPerformanceStates = PerformanceStatesInfoV1.MaxPerformanceStates;
        internal const int MaxPerformanceStateClocks = PerformanceStatesInfoV1.MaxPerformanceStateClocks;
        internal const int MaxPerformanceStateVoltages = 16;

        internal StructureVersion _Version;
        internal uint _Flags;
        internal uint _NumberOfPerformanceStates;
        internal uint _NumberOfClocks;
        internal uint _NumberOfVoltages;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxPerformanceStates)]
        internal PerformanceState[] _PerformanceStates;

        /// <inheritdoc />
        public bool IsPerformanceMonitorEnable
        {
            get => _Flags.GetBit(0);
        }

        /// <inheritdoc />
        public bool IsCapableOfDynamicPerformance
        {
            get => _Flags.GetBit(1);
        }

        /// <inheritdoc />
        public bool IsDynamicPerformanceEnable
        {
            get => _Flags.GetBit(2);
        }

        /// <summary>
        ///     Gets an array of valid and available performance states information
        /// </summary>
        public PerformanceState[] PerformanceStates
        {
            get => _PerformanceStates.Take((int) _NumberOfPerformanceStates).ToArray();
        }

        /// <inheritdoc />
        IPerformanceState[] IPerformanceStatesInfo.PerformanceStates
        {
            get => PerformanceStates.Cast<IPerformanceState>().ToArray();
        }

        /// <summary>
        ///     Gets a dictionary of valid and available performance states and their voltage information as an array
        /// </summary>
        public IReadOnlyDictionary<PerformanceStateId, PerformanceState.PerformanceStatesVoltage[]>
            PerformanceStatesVoltages
        {
            get
            {
                var voltages = (int) _NumberOfVoltages;

                return PerformanceStates.ToDictionary(
                    state => state.StateId,
                    state => state._Voltages.Take(voltages).ToArray()
                );
            }
        }

        /// <inheritdoc />
        IReadOnlyDictionary<PerformanceStateId, IPerformanceStatesVoltage[]> IPerformanceStatesInfo.
            PerformanceStatesVoltages
        {
            get
            {
                return PerformanceStatesVoltages.ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value.Cast<IPerformanceStatesVoltage>().ToArray()
                );
            }
        }

        /// <summary>
        ///     Gets a dictionary of valid and available performance states and their clock information as an array
        /// </summary>
        public IReadOnlyDictionary<PerformanceStateId, PerformanceState.PerformanceStatesClock[]>
            PerformanceStatesClocks
        {
            get
            {
                var clocks = (int) _NumberOfClocks;

                return PerformanceStates.ToDictionary(
                    state => state.StateId,
                    state => state._Clocks.Take(clocks).ToArray()
                );
            }
        }

        /// <inheritdoc />
        IReadOnlyDictionary<PerformanceStateId, IPerformanceStatesClock[]> IPerformanceStatesInfo.
            PerformanceStatesClocks
        {
            get
            {
                return PerformanceStatesClocks.ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value.Cast<IPerformanceStatesClock>().ToArray()
                );
            }
        }

        /// <inheritdoc cref="IPerformanceState" />
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct PerformanceState : IInitializable, IPerformanceState
        {
            internal PerformanceStateId _Id;
            internal uint _Flags;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxPerformanceStateClocks)]
            internal PerformanceStatesClock[] _Clocks;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxPerformanceStateVoltages)]
            internal PerformanceStatesVoltage[] _Voltages;

            /// <inheritdoc />
            public PerformanceStateId StateId
            {
                get => _Id;
            }

            /// <inheritdoc />
            public bool IsPCIELimited
            {
                get => _Flags.GetBit(0);
            }

            /// <inheritdoc />
            public bool IsOverclocked
            {
                get => _Flags.GetBit(1);
            }

            /// <inheritdoc />
            public bool IsOverclockable
            {
                get => _Flags.GetBit(2);
            }

            /// <inheritdoc cref="IPerformanceStatesVoltage" />
            [StructLayout(LayoutKind.Sequential, Pack = 8)]
            public struct PerformanceStatesVoltage : IInitializable, IPerformanceStatesVoltage
            {
                internal PerformanceVoltageDomain _Id;
                internal uint _Flags;
                internal uint _Value;

                /// <inheritdoc />
                public PerformanceVoltageDomain DomainId
                {
                    get => _Id;
                }

                /// <inheritdoc />
                public uint Value
                {
                    get => _Value;
                }
            }

            /// <inheritdoc cref="IPerformanceStatesClock" />
            [StructLayout(LayoutKind.Sequential, Pack = 8)]
            public struct PerformanceStatesClock : IInitializable, IPerformanceStatesClock
            {
                internal PublicClockDomain _Id;
                internal uint _Flags;
                internal uint _Frequency;

                /// <summary>
                ///     Gets a boolean value indicating if this clock domain is overclockable
                /// </summary>
                public bool IsOverclockable
                {
                    get => _Flags.GetBit(0);
                }

                /// <inheritdoc />
                public PublicClockDomain DomainId
                {
                    get => _Id;
                }

                /// <inheritdoc />
                public uint Frequency
                {
                    get => _Frequency;
                }
            }
        }
    }
}