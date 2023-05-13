using System.Collections.Generic;
using NvAPIWrapper.Native.GPU;

namespace NvAPIWrapper.Native.Interfaces.GPU
{
    /// <summary>
    ///     Holds information regarding the valid power states and their clock and voltage settings as well as general
    ///     over-volting settings
    /// </summary>
    public interface IPerformanceStates20Info
    {
        /// <summary>
        ///     Gets a dictionary for valid power states and their clock frequencies
        /// </summary>
        IDictionary<PerformanceStateId, IPerformanceStates20ClockEntry[]> Clocks { get; }

        /// <summary>
        ///     Gets the list of general over-volting settings
        /// </summary>
        IPerformanceStates20VoltageEntry[] GeneralVoltages { get; }

        /// <summary>
        ///     Gets a boolean value indicating if performance states are editable
        /// </summary>
        bool IsEditable { get; }

        /// <summary>
        ///     Gets an array of valid power states for the GPU
        /// </summary>
        IPerformanceState20[] PerformanceStates { get; }

        /// <summary>
        ///     Gets a dictionary for valid power states and their voltage settings
        /// </summary>
        IReadOnlyDictionary<PerformanceStateId, IPerformanceStates20VoltageEntry[]> Voltages { get; }
    }
}