using NvAPIWrapper.Native.GPU;

namespace NvAPIWrapper.Native.Interfaces.GPU
{
    /// <summary>
    ///     Holds information regarding a performance state v2
    /// </summary>
    public interface IPerformanceState20
    {
        /// <summary>
        ///     Gets a boolean value indicating if this performance state is editable
        /// </summary>
        bool IsEditable { get; }

        /// <summary>
        ///     Gets the performance state identification
        /// </summary>
        PerformanceStateId StateId { get; }
    }
}