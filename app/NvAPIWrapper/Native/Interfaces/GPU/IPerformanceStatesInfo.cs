using System.Collections.Generic;
using NvAPIWrapper.Native.GPU;

namespace NvAPIWrapper.Native.Interfaces.GPU
{
    /// <summary>
    ///     Holds information regarding performance states status of a GPU
    /// </summary>
    public interface IPerformanceStatesInfo
    {
        /// <summary>
        ///     Gets a boolean value indicating if the device is capable of dynamic performance state switching
        /// </summary>
        bool IsCapableOfDynamicPerformance { get; }

        /// <summary>
        ///     Gets a boolean value indicating if the dynamic performance state switching is enable
        /// </summary>
        bool IsDynamicPerformanceEnable { get; }

        /// <summary>
        ///     Gets a boolean value indicating if the performance monitoring is enable
        /// </summary>
        bool IsPerformanceMonitorEnable { get; }

        /// <summary>
        ///     Gets an array of valid and available performance states information
        /// </summary>
        IPerformanceState[] PerformanceStates { get; }

        /// <summary>
        ///     Gets a dictionary of valid and available performance states and their clock information as an array
        /// </summary>
        IReadOnlyDictionary<PerformanceStateId, IPerformanceStatesClock[]> PerformanceStatesClocks { get; }

        /// <summary>
        ///     Gets a dictionary of valid and available performance states and their voltage information as an array
        /// </summary>
        IReadOnlyDictionary<PerformanceStateId, IPerformanceStatesVoltage[]> PerformanceStatesVoltages { get; }
    }
}