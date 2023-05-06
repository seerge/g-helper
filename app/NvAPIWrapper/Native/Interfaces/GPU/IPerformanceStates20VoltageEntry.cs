using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.GPU.Structures;

namespace NvAPIWrapper.Native.Interfaces.GPU
{
    /// <summary>
    ///     Holds information regarding the voltage of a voltage domain
    /// </summary>
    public interface IPerformanceStates20VoltageEntry
    {
        /// <summary>
        ///     Gets the voltage domain identification
        /// </summary>
        PerformanceVoltageDomain DomainId { get; }

        /// <summary>
        ///     Gets a boolean value indicating this voltage domain is editable
        /// </summary>
        bool IsEditable { get; }

        /// <summary>
        ///     Gets the base voltage delta and the range of valid values for the delta value
        /// </summary>
        PerformanceStates20ParameterDelta ValueDeltaInMicroVolt { get; }

        /// <summary>
        ///     Gets the current value of this voltage domain in uV
        /// </summary>
        uint ValueInMicroVolt { get; }
    }
}