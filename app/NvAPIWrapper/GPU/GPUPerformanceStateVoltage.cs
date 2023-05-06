using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.Interfaces.GPU;

namespace NvAPIWrapper.GPU
{
    /// <summary>
    ///     Represents a performance state voltage settings
    /// </summary>
    public class GPUPerformanceStateVoltage
    {
        internal GPUPerformanceStateVoltage(IPerformanceStates20VoltageEntry states20BaseVoltageEntry)
        {
            VoltageDomain = states20BaseVoltageEntry.DomainId;
            IsReadOnly = !states20BaseVoltageEntry.IsEditable;

            CurrentVoltageInMicroVolt = states20BaseVoltageEntry.ValueInMicroVolt;
            VoltageDeltaInMicroVolt = states20BaseVoltageEntry.ValueDeltaInMicroVolt.DeltaValue;
            BaseVoltageInMicroVolt = (int) (CurrentVoltageInMicroVolt - VoltageDeltaInMicroVolt);

            VoltageDeltaRangeInMicroVolt = new GPUPerformanceStateValueRange(
                states20BaseVoltageEntry.ValueDeltaInMicroVolt.DeltaRange.Minimum,
                states20BaseVoltageEntry.ValueDeltaInMicroVolt.DeltaRange.Maximum
            );
        }

        /// <summary>
        ///     Gets the base voltage in uV
        /// </summary>
        public int BaseVoltageInMicroVolt { get; }

        /// <summary>
        ///     Gets the current voltage in uV
        /// </summary>
        public uint CurrentVoltageInMicroVolt { get; }

        /// <summary>
        ///     Gets a boolean value indicating if this voltage is readonly
        /// </summary>
        public bool IsReadOnly { get; }

        /// <summary>
        ///     Gets the voltage delta in uV
        /// </summary>
        public int VoltageDeltaInMicroVolt { get; }

        /// <summary>
        ///     Gets the voltage delta range in uV
        /// </summary>
        public GPUPerformanceStateValueRange VoltageDeltaRangeInMicroVolt { get; }

        /// <summary>
        ///     Gets the voltage domain
        /// </summary>
        public PerformanceVoltageDomain VoltageDomain { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            var title = IsReadOnly ? $"{VoltageDomain} (ReadOnly)" : VoltageDomain.ToString();

            return
                $"{title}: ({BaseVoltageInMicroVolt}) + ({VoltageDeltaInMicroVolt}) = ({CurrentVoltageInMicroVolt})";
        }
    }
}