using System.Collections.Generic;
using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.GPU.Structures;

namespace NvAPIWrapper.Native.Interfaces.GPU
{
    /// <summary>
    ///     Interface for all ClockFrequencies structures
    /// </summary>
    public interface IClockFrequencies
    {
        /// <summary>
        ///     Gets all valid clocks
        /// </summary>
        IReadOnlyDictionary<PublicClockDomain, ClockDomainInfo> Clocks { get; }

        /// <summary>
        ///     Gets the type of clock frequencies provided with this object
        /// </summary>
        ClockType ClockType { get; }

        /// <summary>
        ///     Gets graphics engine clock
        /// </summary>
        ClockDomainInfo GraphicsClock { get; }

        /// <summary>
        ///     Gets memory decoding clock
        /// </summary>
        ClockDomainInfo MemoryClock { get; }

        /// <summary>
        ///     Gets processor clock
        /// </summary>
        ClockDomainInfo ProcessorClock { get; }

        /// <summary>
        ///     Gets video decoding clock
        /// </summary>
        ClockDomainInfo VideoDecodingClock { get; }
    }
}