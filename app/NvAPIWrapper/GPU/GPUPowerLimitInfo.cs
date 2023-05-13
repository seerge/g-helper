using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.GPU.Structures;

namespace NvAPIWrapper.GPU
{
    /// <summary>
    ///     Holds information regarding a possible power limit policy and its acceptable range
    /// </summary>
    public class GPUPowerLimitInfo
    {
        internal GPUPowerLimitInfo(PrivatePowerPoliciesInfoV1.PowerPolicyInfoEntry powerPolicyInfoEntry)
        {
            PerformanceStateId = powerPolicyInfoEntry.PerformanceStateId;
            MinimumPowerInPCM = powerPolicyInfoEntry.MinimumPowerInPCM;
            DefaultPowerInPCM = powerPolicyInfoEntry.DefaultPowerInPCM;
            MaximumPowerInPCM = powerPolicyInfoEntry.MaximumPowerInPCM;
        }

        /// <summary>
        ///     Gets the default policy target power in per cent mille (PCM)
        /// </summary>
        public uint DefaultPowerInPCM { get; }

        /// <summary>
        ///     Gets the default policy target power in percentage
        /// </summary>
        public float DefaultPowerInPercent
        {
            get => DefaultPowerInPCM / 1000f;
        }

        /// <summary>
        ///     Gets the maximum possible policy target power in per cent mille (PCM)
        /// </summary>
        public uint MaximumPowerInPCM { get; }

        /// <summary>
        ///     Gets the maximum possible policy target power in percentage
        /// </summary>
        public float MaximumPowerInPercent
        {
            get => MaximumPowerInPCM / 1000f;
        }

        /// <summary>
        ///     Gets the minimum possible policy target power in per cent mille (PCM)
        /// </summary>
        public uint MinimumPowerInPCM { get; }

        /// <summary>
        ///     Gets the minimum possible policy target power in percentage
        /// </summary>
        public float MinimumPowerInPercent
        {
            get => MinimumPowerInPCM / 1000f;
        }

        /// <summary>
        ///     Gets the corresponding performance state identification
        /// </summary>
        public PerformanceStateId PerformanceStateId { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return
                $"[{PerformanceStateId}] Default: {DefaultPowerInPercent}% - Range: ({MinimumPowerInPercent}% - {MaximumPowerInPercent}%)";
        }
    }
}