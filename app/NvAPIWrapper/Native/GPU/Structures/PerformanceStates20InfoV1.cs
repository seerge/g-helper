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
    /// <inheritdoc cref="IPerformanceStates20Info" />
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct PerformanceStates20InfoV1 : IInitializable, IPerformanceStates20Info
    {
        internal const int MaxPerformanceStates = 16;
        internal const int MaxPerformanceStatesClocks = 8;
        internal const int MaxPerformanceStatesBaseVoltages = 4;

        internal StructureVersion _Version;
        internal uint _Flags;
        internal uint _NumberOfPerformanceStates;
        internal uint _NumberOfClocks;
        internal uint _NumberOfBaseVoltages;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxPerformanceStates)]
        internal PerformanceState20[] _PerformanceStates;

        /// <summary>
        ///     Creates a new instance of <see cref="PerformanceStates20InfoV1" />
        /// </summary>
        /// <param name="performanceStates">The list of performance states and their settings.</param>
        /// <param name="clocksCount">Number of clock frequencies per each performance state.</param>
        /// <param name="baseVoltagesCount">Number of base voltage per each performance state.</param>
        public PerformanceStates20InfoV1(
            PerformanceState20[] performanceStates,
            uint clocksCount,
            uint baseVoltagesCount)
        {
            if (performanceStates?.Length > MaxPerformanceStatesClocks)
            {
                throw new ArgumentException(
                    $"Maximum of {MaxPerformanceStates} performance states are configurable.",
                    nameof(performanceStates)
                );
            }

            if (performanceStates == null)
            {
                throw new ArgumentNullException(nameof(performanceStates));
            }

            this = typeof(PerformanceStates20InfoV1).Instantiate<PerformanceStates20InfoV1>();
            _NumberOfClocks = clocksCount;
            _NumberOfBaseVoltages = baseVoltagesCount;
            _NumberOfPerformanceStates = (uint) performanceStates.Length;
            Array.Copy(performanceStates, 0, _PerformanceStates, 0, performanceStates.Length);
        }

        /// <inheritdoc />
        public IPerformanceStates20VoltageEntry[] GeneralVoltages
        {
            get => new IPerformanceStates20VoltageEntry[0];
        }

        /// <inheritdoc />
        public bool IsEditable
        {
            get => _Flags.GetBit(0);
        }

        /// <summary>
        ///     Gets an array of valid power states for the GPU
        /// </summary>
        public PerformanceState20[] PerformanceStates
        {
            get => _PerformanceStates.Take((int) _NumberOfPerformanceStates).ToArray();
        }

        /// <inheritdoc />
        IPerformanceState20[] IPerformanceStates20Info.PerformanceStates
        {
            get => PerformanceStates.Cast<IPerformanceState20>().ToArray();
        }

        /// <summary>
        ///     Gets a dictionary for valid power states and their clock frequencies
        /// </summary>
        public IDictionary<PerformanceStateId, PerformanceStates20ClockEntryV1[]> Clocks
        {
            get
            {
                var clocks = (int) _NumberOfClocks;

                return PerformanceStates.ToDictionary(
                    state20 => state20.StateId,
                    state20 => state20._Clocks.Take(clocks).ToArray()
                );
            }
        }

        /// <inheritdoc />
        IDictionary<PerformanceStateId, IPerformanceStates20ClockEntry[]> IPerformanceStates20Info.Clocks
        {
            get
            {
                return Clocks.ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value.Cast<IPerformanceStates20ClockEntry>().ToArray()
                );
            }
        }

        /// <summary>
        ///     Gets a dictionary for valid power states and their voltage settings
        /// </summary>
        public IReadOnlyDictionary<PerformanceStateId, PerformanceStates20BaseVoltageEntryV1[]> Voltages
        {
            get
            {
                var baseVoltages = (int) _NumberOfBaseVoltages;

                return PerformanceStates.ToDictionary(
                    state20 => state20.StateId,
                    state20 => state20._BaseVoltages.Take(baseVoltages).ToArray()
                );
            }
        }

        /// <inheritdoc />
        IReadOnlyDictionary<PerformanceStateId, IPerformanceStates20VoltageEntry[]> IPerformanceStates20Info.Voltages
        {
            get
            {
                return Voltages.ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value.Cast<IPerformanceStates20VoltageEntry>().ToArray()
                );
            }
        }

        /// <inheritdoc cref="IPerformanceState20" />
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct PerformanceState20 : IInitializable, IPerformanceState20
        {
            internal PerformanceStateId _Id;
            internal uint _Flags;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxPerformanceStatesClocks)]
            internal PerformanceStates20ClockEntryV1[] _Clocks;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxPerformanceStatesBaseVoltages)]
            internal PerformanceStates20BaseVoltageEntryV1[] _BaseVoltages;

            /// <summary>
            ///     Creates a new instance of <see cref="PerformanceState20" />.
            /// </summary>
            /// <param name="stateId">The performance identification number.</param>
            /// <param name="clocks">The list of clock entries.</param>
            /// <param name="baseVoltages">The list of base voltages.</param>
            public PerformanceState20(
                PerformanceStateId stateId,
                PerformanceStates20ClockEntryV1[] clocks,
                PerformanceStates20BaseVoltageEntryV1[] baseVoltages)
            {
                if (clocks?.Length > MaxPerformanceStatesClocks)
                {
                    throw new ArgumentException(
                        $"Maximum of {MaxPerformanceStatesClocks} clocks are configurable.",
                        nameof(clocks)
                    );
                }

                if (clocks == null)
                {
                    throw new ArgumentNullException(nameof(clocks));
                }

                if (baseVoltages?.Length > MaxPerformanceStatesBaseVoltages)
                {
                    throw new ArgumentException(
                        $"Maximum of {MaxPerformanceStatesBaseVoltages} base voltages are configurable.",
                        nameof(baseVoltages)
                    );
                }

                if (baseVoltages == null)
                {
                    throw new ArgumentNullException(nameof(baseVoltages));
                }

                this = typeof(PerformanceState20).Instantiate<PerformanceState20>();
                _Id = stateId;
                Array.Copy(clocks, 0, _Clocks, 0, clocks.Length);
                Array.Copy(baseVoltages, 0, _BaseVoltages, 0, baseVoltages.Length);
            }

            /// <inheritdoc />
            public PerformanceStateId StateId
            {
                get => _Id;
            }

            /// <inheritdoc />
            public bool IsEditable
            {
                get => _Flags.GetBit(0);
            }
        }
    }
}