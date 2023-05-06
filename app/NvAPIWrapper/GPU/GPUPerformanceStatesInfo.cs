using System.Collections.Generic;
using System.Linq;
using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.GPU.Structures;
using NvAPIWrapper.Native.Interfaces.GPU;

namespace NvAPIWrapper.GPU
{
    /// <summary>
    ///     Holds the retrieved performance states information
    /// </summary>
    public class GPUPerformanceStatesInformation
    {
        internal GPUPerformanceStatesInformation(
            IPerformanceStates20Info states20Info,
            PerformanceStateId currentPerformanceStateId,
            PrivatePCIeInfoV2? pciInformation)
        {
            IsReadOnly = !states20Info.IsEditable;

            GlobalVoltages = states20Info.GeneralVoltages
                .Select(entry => new GPUPerformanceStateVoltage(entry))
                .ToArray();

            var clocks = states20Info.Clocks;
            var baseVoltages = states20Info.Voltages;

            PerformanceStates = states20Info.PerformanceStates.Select((state20, i) =>
            {
                PCIeInformation statePCIeInfo = null;

                if (pciInformation != null && pciInformation.Value.PCIePerformanceStateInfos.Length > i)
                {
                    statePCIeInfo = new PCIeInformation(pciInformation.Value.PCIePerformanceStateInfos[i]);
                }

                return new GPUPerformanceState(
                    i,
                    state20,
                    clocks[state20.StateId],
                    baseVoltages[state20.StateId],
                    statePCIeInfo
                );
            }).ToArray();

            CurrentPerformanceState =
                PerformanceStates.FirstOrDefault(performanceState =>
                    performanceState.StateId == currentPerformanceStateId);
        }

        /// <summary>
        ///     Gets the currently active performance state
        /// </summary>
        public GPUPerformanceState CurrentPerformanceState { get; }

        /// <summary>
        ///     Gets a list of global voltage settings
        /// </summary>
        public GPUPerformanceStateVoltage[] GlobalVoltages { get; }

        /// <summary>
        ///     Gets a boolean value indicating if performance states are readonly
        /// </summary>
        public bool IsReadOnly { get; }

        /// <summary>
        ///     Gets a list of all available performance states
        /// </summary>
        public GPUPerformanceState[] PerformanceStates { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            if (PerformanceStates.Length == 0)
            {
                return "No Performance State Available";
            }

            return string.Join(
                ", ",
                PerformanceStates
                    .Select(
                        state =>
                        {
                            var attributes = new List<string>();

                            if (state.IsReadOnly)
                            {
                                attributes.Add("ReadOnly");
                            }

                            if (CurrentPerformanceState.StateId == state.StateId)
                            {
                                attributes.Add("Active");
                            }

                            if (attributes.Any())
                            {
                                return $"{state.StateId} ({string.Join(" - ", attributes)})";
                            }

                            return state.StateId.ToString();
                        })
            );
        }
    }
}