using NvAPIWrapper.Native.GPU;

namespace NvAPIWrapper.Native.Interfaces.GPU
{
    /// <summary>
    ///     Holds information regarding a performance state
    /// </summary>
    public interface IPerformanceState
    {
        /// <summary>
        ///     Gets a boolean value indicating if this performance state is overclockable
        /// </summary>
        bool IsOverclockable { get; }

        /// <summary>
        ///     Gets a boolean value indicating if this performance state is currently overclocked
        /// </summary>
        bool IsOverclocked { get; }

        /// <summary>
        ///     Gets a boolean value indicating if this performance state is limited to use PCIE generation 1 or PCIE generation 2
        /// </summary>
        bool IsPCIELimited { get; }

        /// <summary>
        ///     Gets the performance state identification
        /// </summary>
        PerformanceStateId StateId { get; }
    }
}