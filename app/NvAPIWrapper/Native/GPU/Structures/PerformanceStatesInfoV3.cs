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
    [StructureVersion(3)]
    public struct PerformanceStatesInfoV3 : IInitializable, IPerformanceStatesInfo
    {
        internal const int MaxPerformanceStates = PerformanceStatesInfoV2.MaxPerformanceStates;
        internal const int MaxPerformanceStateClocks = PerformanceStatesInfoV2.MaxPerformanceStateClocks;
        internal const int MaxPerformanceStateVoltages = PerformanceStatesInfoV2.MaxPerformanceStateVoltages;

        internal StructureVersion _Version;
        internal uint _Flags;
        internal uint _NumberOfPerformanceStates;
        internal uint _NumberOfClocks;
        internal uint _NumberOfVoltages;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxPerformanceStates)]
        internal PerformanceStatesInfoV2.PerformanceState[] _PerformanceStates;

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
        public PerformanceStatesInfoV2.PerformanceState[] PerformanceStates
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
        public IReadOnlyDictionary<PerformanceStateId, PerformanceStatesInfoV2.PerformanceState.PerformanceStatesVoltage
                []>
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
        public IReadOnlyDictionary<PerformanceStateId, PerformanceStatesInfoV2.PerformanceState.PerformanceStatesClock[]
            >
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
    }
}