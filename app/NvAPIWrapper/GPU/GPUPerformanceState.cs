using System.Linq;
using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.Interfaces.GPU;

namespace NvAPIWrapper.GPU
{
    /// <summary>
    ///     Represents a performance state
    /// </summary>
    public class GPUPerformanceState
    {
        // ReSharper disable once TooManyDependencies
        internal GPUPerformanceState(
            int index,
            IPerformanceState20 performanceState,
            IPerformanceStates20ClockEntry[] statesClockEntries,
            IPerformanceStates20VoltageEntry[] baseVoltageEntries,
            PCIeInformation pcieInformation)
        {
            StateIndex = index;
            StateId = performanceState.StateId;
            IsReadOnly = !performanceState.IsEditable;
            Clocks = statesClockEntries.Select(entry => new GPUPerformanceStateClock(entry)).ToArray();
            Voltages = baseVoltageEntries.Select(entry => new GPUPerformanceStateVoltage(entry)).ToArray();
            PCIeInformation = pcieInformation;
        }

        /// <summary>
        ///     Gets a list of clocks associated with this performance state
        /// </summary>

        public GPUPerformanceStateClock[] Clocks { get; }

        /// <summary>
        ///     Gets a boolean value indicating if this performance state is readonly
        /// </summary>
        public bool IsReadOnly { get; }

        /// <summary>
        ///     Gets the PCI-e information regarding this performance state.
        /// </summary>
        public PCIeInformation PCIeInformation { get; }

        /// <summary>
        ///     Gets the performance state identification
        /// </summary>
        public PerformanceStateId StateId { get; }

        /// <summary>
        ///     Gets the state index
        /// </summary>
        public int StateIndex { get; }

        /// <summary>
        ///     Gets a list of voltages associated with this performance state
        /// </summary>
        public GPUPerformanceStateVoltage[] Voltages { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            if (IsReadOnly)
            {
                return $"{StateId} (ReadOnly)";
            }

            return StateId.ToString();
        }
    }
}