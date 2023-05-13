using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    [StructureVersion(1)]
    public struct PerformanceStatesInfoV1 : IInitializable, IPerformanceStatesInfo
    {
        internal const int MaxPerformanceStates = 16;
        internal const int MaxPerformanceStateClocks = 32;

        internal StructureVersion _Version;
        internal uint _Flags;
        internal uint _NumberOfPerformanceStates;
        internal uint _NumberOfClocks;

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

        /// <inheritdoc />
        public IReadOnlyDictionary<PerformanceStateId, IPerformanceStatesVoltage[]> PerformanceStatesVoltages
        {
            get => new ReadOnlyDictionary<PerformanceStateId, IPerformanceStatesVoltage[]>(
                new Dictionary<PerformanceStateId, IPerformanceStatesVoltage[]>()
            );
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


            /// <inheritdoc cref="IPerformanceStatesClock" />
            [StructLayout(LayoutKind.Sequential, Pack = 8)]
            public struct PerformanceStatesClock : IInitializable, IPerformanceStatesClock
            {
                internal PublicClockDomain _Id;
                internal uint _Flags;
                internal uint _Frequency;

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