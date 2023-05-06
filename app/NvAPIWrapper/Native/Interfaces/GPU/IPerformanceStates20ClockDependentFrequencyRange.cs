using NvAPIWrapper.Native.GPU;

namespace NvAPIWrapper.Native.Interfaces.GPU
{
    /// <summary>
    ///     Holds information regarding the frequency range of a clock domain as well as the dependent voltage domain and the
    ///     range of the voltage
    /// </summary>
    public interface IPerformanceStates20ClockDependentFrequencyRange
    {
        /// <summary>
        ///     Gets the maximum clock frequency in kHz
        /// </summary>
        uint MaximumFrequencyInkHz { get; }

        /// <summary>
        ///     Gets the dependent voltage domain's maximum voltage in uV
        /// </summary>
        uint MaximumVoltageInMicroVolt { get; }

        /// <summary>
        ///     Gets the minimum clock frequency in kHz
        /// </summary>
        uint MinimumFrequencyInkHz { get; }

        /// <summary>
        ///     Gets the dependent voltage domain's minimum voltage in uV
        /// </summary>
        uint MinimumVoltageInMicroVolt { get; }

        /// <summary>
        ///     Gets the dependent voltage domain identification
        /// </summary>
        PerformanceVoltageDomain VoltageDomainId { get; }
    }
}