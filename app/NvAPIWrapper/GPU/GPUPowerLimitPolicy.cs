using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.GPU.Structures;

namespace NvAPIWrapper.GPU
{
    /// <summary>
    ///     Holds information regarding a currently active power limit policy
    /// </summary>
    public class GPUPowerLimitPolicy
    {
        internal GPUPowerLimitPolicy(PrivatePowerPoliciesStatusV1.PowerPolicyStatusEntry powerPolicyStatusEntry)
        {
            PerformanceStateId = powerPolicyStatusEntry.PerformanceStateId;
            PowerTargetInPCM = powerPolicyStatusEntry.PowerTargetInPCM;
        }

        /// <summary>
        ///     Gets the corresponding performance state identification
        /// </summary>
        public PerformanceStateId PerformanceStateId { get; }

        /// <summary>
        ///     Gets the current policy target power in per cent mille (PCM)
        /// </summary>
        public uint PowerTargetInPCM { get; }

        /// <summary>
        ///     Gets the current policy target power in percentage
        /// </summary>
        public float PowerTargetInPercent
        {
            get => PowerTargetInPCM / 1000f;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{PerformanceStateId} Target: {PowerTargetInPercent}%";
        }
    }
}