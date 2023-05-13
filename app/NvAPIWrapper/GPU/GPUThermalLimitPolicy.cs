using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.GPU.Structures;

namespace NvAPIWrapper.GPU
{
    /// <summary>
    ///     Holds information regarding a currently active temperature limit policy
    /// </summary>
    public class GPUThermalLimitPolicy
    {
        internal GPUThermalLimitPolicy(PrivateThermalPoliciesStatusV2.ThermalPoliciesStatusEntry thermalPoliciesEntry)
        {
            Controller = thermalPoliciesEntry.Controller;
            PerformanceStateId = thermalPoliciesEntry.PerformanceStateId;
            TargetTemperature = thermalPoliciesEntry.TargetTemperature;
        }

        /// <summary>
        ///     Gets the policy's thermal controller
        /// </summary>
        public ThermalController Controller { get; }

        /// <summary>
        ///     Gets the corresponding performance state identification
        /// </summary>
        public PerformanceStateId PerformanceStateId { get; }

        /// <summary>
        ///     Gets the current policy target temperature in degree Celsius
        /// </summary>
        public int TargetTemperature { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return
                $"{PerformanceStateId} [{Controller}] Target: {TargetTemperature}°C";
        }
    }
}