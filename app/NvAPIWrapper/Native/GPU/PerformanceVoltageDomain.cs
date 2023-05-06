using NvAPIWrapper.Native.GPU.Structures;

namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     Contains the list of possible voltage domains
    /// </summary>
    public enum PerformanceVoltageDomain : uint
    {
        /// <summary>
        ///     GPU Core
        /// </summary>
        Core = 0,

        /// <summary>
        ///     Undefined voltage domain
        /// </summary>
        Undefined = PerformanceStatesInfoV2.MaxPerformanceStateVoltages
    }
}