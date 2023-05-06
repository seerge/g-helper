using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.GPU.Structures;

namespace NvAPIWrapper.Native.Interfaces.GPU
{
    /// <summary>
    ///     Holds information regarding a clock domain of a performance states
    /// </summary>
    public interface IPerformanceStates20ClockEntry
    {
        /// <summary>
        ///     Gets the type of clock frequency
        /// </summary>
        PerformanceStates20ClockType ClockType { get; }

        /// <summary>
        ///     Gets the domain identification
        /// </summary>
        PublicClockDomain DomainId { get; }

        /// <summary>
        ///     Gets the current base frequency delta value and the range for a valid delta value
        /// </summary>
        PerformanceStates20ParameterDelta FrequencyDeltaInkHz { get; }

        /// <summary>
        ///     Gets the fixed frequency of the clock
        /// </summary>
        IPerformanceStates20ClockDependentFrequencyRange FrequencyRange { get; }


        /// <summary>
        ///     Gets a boolean value indicating if this clock is editable
        /// </summary>
        bool IsEditable { get; }

        /// <summary>
        ///     Gets the range of clock frequency and related voltage information if present
        /// </summary>
        IPerformanceStates20ClockDependentSingleFrequency SingleFrequency { get; }
    }
}