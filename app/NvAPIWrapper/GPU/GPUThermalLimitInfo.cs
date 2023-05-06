using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.GPU.Structures;

namespace NvAPIWrapper.GPU
{
    /// <summary>
    ///     Holds information regarding a possible thermal limit policy and its acceptable range
    /// </summary>
    public class GPUThermalLimitInfo
    {
        internal GPUThermalLimitInfo(PrivateThermalPoliciesInfoV2.ThermalPoliciesInfoEntry policiesInfoEntry)
        {
            Controller = policiesInfoEntry.Controller;
            MinimumTemperature = policiesInfoEntry.MinimumTemperature;
            DefaultTemperature = policiesInfoEntry.DefaultTemperature;
            MaximumTemperature = policiesInfoEntry.MaximumTemperature;
        }

        /// <summary>
        ///     Gets the policy's thermal controller
        /// </summary>
        public ThermalController Controller { get; }

        /// <summary>
        ///     Gets the default policy target temperature in degree Celsius
        /// </summary>
        public int DefaultTemperature { get; }


        /// <summary>
        ///     Gets the maximum possible policy target temperature in degree Celsius
        /// </summary>
        public int MaximumTemperature { get; }

        /// <summary>
        ///     Gets the minimum possible policy target temperature in degree Celsius
        /// </summary>
        public int MinimumTemperature { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return
                $"[{Controller}] Default: {DefaultTemperature}°C - Range: ({MinimumTemperature}°C - {MaximumTemperature}°C)";
        }
    }
}