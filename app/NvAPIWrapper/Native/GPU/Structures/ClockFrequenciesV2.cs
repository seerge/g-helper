using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;
using NvAPIWrapper.Native.Interfaces.GPU;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Holds clock frequencies associated with a physical GPU and an specified clock type
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(2)]
    public struct ClockFrequenciesV2 : IInitializable, IClockFrequencies
    {
        internal const int MaxClocksPerGpu = 32;

        internal StructureVersion _Version;
        internal readonly uint _ClockTypeAndReserve;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxClocksPerGpu)]
        internal ClockDomainInfo[] _Clocks;

        /// <summary>
        ///     Creates a new ClockFrequenciesV2
        /// </summary>
        /// <param name="clockType">The type of the clock frequency being requested</param>
        public ClockFrequenciesV2(ClockType clockType = ClockType.CurrentClock)
        {
            this = typeof(ClockFrequenciesV2).Instantiate<ClockFrequenciesV2>();
            _ClockTypeAndReserve = 0u.SetBits(0, 2, (uint) clockType);
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<PublicClockDomain, ClockDomainInfo> Clocks
        {
            get => _Clocks
                .Select((value, index) => new {index, value})
                .Where(arg => Enum.IsDefined(typeof(PublicClockDomain), arg.index))
                .ToDictionary(arg => (PublicClockDomain) arg.index, arg => arg.value);
        }

        /// <inheritdoc />
        public ClockType ClockType
        {
            get => (ClockType) _ClockTypeAndReserve.GetBits(0, 2);
        }

        /// <inheritdoc />
        public ClockDomainInfo GraphicsClock
        {
            get => _Clocks[(int) PublicClockDomain.Graphics];
        }

        /// <inheritdoc />
        public ClockDomainInfo MemoryClock
        {
            get => _Clocks[(int) PublicClockDomain.Memory];
        }

        /// <inheritdoc />
        public ClockDomainInfo VideoDecodingClock
        {
            get => _Clocks[(int) PublicClockDomain.Video];
        }

        /// <inheritdoc />
        public ClockDomainInfo ProcessorClock
        {
            get => _Clocks[(int) PublicClockDomain.Processor];
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return
                $"[{ClockType}] 3D Graphics = {GraphicsClock} - Memory = {MemoryClock} - Video Decoding = {VideoDecodingClock} - Processor = {ProcessorClock}";
        }
    }
}