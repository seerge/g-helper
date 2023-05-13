using System.Collections.Generic;
using System.Linq;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.GPU;

namespace NvAPIWrapper.GPU
{
    /// <summary>
    ///     Contains information regarding the GPU performance control and limitations
    /// </summary>
    public class GPUPerformanceControl
    {
        internal GPUPerformanceControl(PhysicalGPU physicalGPU)
        {
            PhysicalGPU = physicalGPU;
        }

        /// <summary>
        ///     Gets the current active performance limitation
        /// </summary>
        public PerformanceLimit CurrentActiveLimit
        {
            get => GPUApi.PerformancePoliciesGetStatus(PhysicalGPU.Handle).PerformanceLimit;
        }

        /// <summary>
        ///     Gets the current performance decrease reason
        /// </summary>
        public PerformanceDecreaseReason CurrentPerformanceDecreaseReason
        {
            get => GPUApi.GetPerformanceDecreaseInfo(PhysicalGPU.Handle);
        }


        /// <summary>
        ///     Gets a boolean value indicating if no load limit is supported with this GPU
        /// </summary>
        public bool IsNoLoadLimitSupported
        {
            get => GPUApi.PerformancePoliciesGetInfo(PhysicalGPU.Handle).IsNoLoadLimitSupported;
        }


        /// <summary>
        ///     Gets a boolean value indicating if power limit is supported with this GPU
        /// </summary>
        public bool IsPowerLimitSupported
        {
            get => GPUApi.PerformancePoliciesGetInfo(PhysicalGPU.Handle).IsPowerLimitSupported;
        }


        /// <summary>
        ///     Gets a boolean value indicating if temperature limit is supported with this GPU
        /// </summary>
        public bool IsTemperatureLimitSupported
        {
            get => GPUApi.PerformancePoliciesGetInfo(PhysicalGPU.Handle).IsTemperatureLimitSupported;
        }

        /// <summary>
        ///     Gets a boolean value indicating if voltage limit is supported with this GPU
        /// </summary>
        public bool IsVoltageLimitSupported
        {
            get => GPUApi.PerformancePoliciesGetInfo(PhysicalGPU.Handle).IsVoltageLimitSupported;
        }

        /// <summary>
        ///     Gets the physical GPU that this instance describes
        /// </summary>
        public PhysicalGPU PhysicalGPU { get; }

        /// <summary>
        ///     Gets information regarding possible power limit policies and their acceptable range
        /// </summary>
        public IEnumerable<GPUPowerLimitInfo> PowerLimitInformation
        {
            get
            {
                return GPUApi.ClientPowerPoliciesGetInfo(PhysicalGPU.Handle).PowerPolicyInfoEntries
                    .Select(entry => new GPUPowerLimitInfo(entry));
            }
        }

        /// <summary>
        ///     Gets the current active power limit policies
        /// </summary>
        public IEnumerable<GPUPowerLimitPolicy> PowerLimitPolicies
        {
            get
            {
                // TODO: GPUApi.ClientPowerPoliciesSetStatus();
                return GPUApi.ClientPowerPoliciesGetStatus(PhysicalGPU.Handle).PowerPolicyStatusEntries
                    .Select(entry => new GPUPowerLimitPolicy(entry));
            }
        }

        /// <summary>
        ///     Gets information regarding possible thermal limit policies and their acceptable range
        /// </summary>
        public IEnumerable<GPUThermalLimitInfo> ThermalLimitInformation
        {
            get
            {
                return GPUApi.GetThermalPoliciesInfo(PhysicalGPU.Handle).ThermalPoliciesInfoEntries
                    .Select(entry => new GPUThermalLimitInfo(entry));
            }
        }

        /// <summary>
        ///     Gets the current active thermal limit policies
        /// </summary>
        public IEnumerable<GPUThermalLimitPolicy> ThermalLimitPolicies
        {
            get
            {
                // TODO: GPUApi.SetThermalPoliciesStatus();
                return GPUApi.GetThermalPoliciesStatus(PhysicalGPU.Handle).ThermalPoliciesStatusEntries
                    .Select(entry => new GPUThermalLimitPolicy(entry));
            }
        }
    }
}