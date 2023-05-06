using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.Interfaces.GPU;

namespace NvAPIWrapper.GPU
{
    /// <summary>
    ///     Represents a performance state clock settings
    /// </summary>
    public class GPUPerformanceStateClock
    {
        internal GPUPerformanceStateClock(IPerformanceStates20ClockEntry states20ClockEntry)
        {
            ClockDomain = states20ClockEntry.DomainId;
            IsReadOnly = !states20ClockEntry.IsEditable;
            ClockDeltaInkHz = states20ClockEntry.FrequencyDeltaInkHz.DeltaValue;
            ClockDeltaRangeInkHz = new GPUPerformanceStateValueRange(
                states20ClockEntry.FrequencyDeltaInkHz.DeltaRange.Minimum,
                states20ClockEntry.FrequencyDeltaInkHz.DeltaRange.Maximum
            );

            if (states20ClockEntry.ClockType == PerformanceStates20ClockType.Range)
            {
                CurrentClockInkHz = new GPUPerformanceStateValueRange(
                    states20ClockEntry.FrequencyRange.MinimumFrequencyInkHz,
                    states20ClockEntry.FrequencyRange.MaximumFrequencyInkHz
                );
                BaseClockInkHz = new GPUPerformanceStateValueRange(
                    CurrentClockInkHz.Minimum - ClockDeltaInkHz,
                    CurrentClockInkHz.Maximum - ClockDeltaInkHz
                );
                DependentVoltageDomain = states20ClockEntry.FrequencyRange.VoltageDomainId;
                DependentVoltageRangeInMicroVolt = new GPUPerformanceStateValueRange(
                    states20ClockEntry.FrequencyRange.MinimumVoltageInMicroVolt,
                    states20ClockEntry.FrequencyRange.MaximumVoltageInMicroVolt
                );
            }
            else
            {
                CurrentClockInkHz = new GPUPerformanceStateValueRange(
                    states20ClockEntry.SingleFrequency.FrequencyInkHz
                );
                BaseClockInkHz = new GPUPerformanceStateValueRange(
                    CurrentClockInkHz.Minimum - ClockDeltaInkHz
                );
                DependentVoltageDomain = PerformanceVoltageDomain.Undefined;
                DependentVoltageRangeInMicroVolt = null;
            }
        }

        /// <summary>
        ///     Gets the base clock frequency in kHz
        /// </summary>
        public GPUPerformanceStateValueRange BaseClockInkHz { get; }

        /// <summary>
        ///     Gets the clock frequency delta in kHz
        /// </summary>
        public int ClockDeltaInkHz { get; }

        /// <summary>
        ///     Gets the clock frequency delta range in kHz
        /// </summary>
        public GPUPerformanceStateValueRange ClockDeltaRangeInkHz { get; }

        /// <summary>
        ///     Gets the clock domain
        /// </summary>
        public PublicClockDomain ClockDomain { get; }

        /// <summary>
        ///     Gets the current clock frequency in kHz
        /// </summary>
        public GPUPerformanceStateValueRange CurrentClockInkHz { get; }

        /// <summary>
        ///     Gets the dependent voltage domain
        /// </summary>
        public PerformanceVoltageDomain DependentVoltageDomain { get; }

        /// <summary>
        ///     Gets the dependent voltage range in uV
        /// </summary>
        public GPUPerformanceStateValueRange DependentVoltageRangeInMicroVolt { get; }

        /// <summary>
        ///     Gets a boolean value indicating if this clock setting is readonly
        /// </summary>
        public bool IsReadOnly { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            var title = IsReadOnly ? $"{ClockDomain} (ReadOnly)" : ClockDomain.ToString();

            return
                $"{title}: {BaseClockInkHz} + ({ClockDeltaInkHz}) = {CurrentClockInkHz}";
        }
    }
}