namespace NvAPIWrapper.Native.Interfaces.GPU
{
    /// <summary>
    ///     Holds information regarding the clock frequency of a fixed frequency clock domain
    /// </summary>
    public interface IPerformanceStates20ClockDependentSingleFrequency
    {
        /// <summary>
        ///     Gets the clock frequency of a clock domain in kHz
        /// </summary>
        uint FrequencyInkHz { get; }
    }
}