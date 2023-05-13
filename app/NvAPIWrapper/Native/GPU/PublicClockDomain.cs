using NvAPIWrapper.Native.GPU.Structures;

namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     Contains the list of clocks available to public
    /// </summary>
    public enum PublicClockDomain
    {
        /// <summary>
        ///     Undefined
        /// </summary>
        Undefined = ClockFrequenciesV1.MaxClocksPerGPU,

        /// <summary>
        ///     3D graphics clock
        /// </summary>
        Graphics = 0,

        /// <summary>
        ///     Memory clock
        /// </summary>
        Memory = 4,

        /// <summary>
        ///     Processor clock
        /// </summary>
        Processor = 7,

        /// <summary>
        ///     Video decoding clock
        /// </summary>
        Video = 8
    }
}