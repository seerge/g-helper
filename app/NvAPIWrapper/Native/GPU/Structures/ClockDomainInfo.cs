using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Helpers;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Holds information about the clock frequency of an specific clock domain
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct ClockDomainInfo
    {
        internal readonly uint _IsPresent;
        internal readonly uint _Frequency;

        /// <summary>
        ///     Gets a boolean value that indicates if this clock domain is present on this GPU and with the requested clock type.
        /// </summary>
        public bool IsPresent
        {
            get => _IsPresent.GetBit(0);
        }

        /// <summary>
        ///     Gets the clock frequency in kHz
        /// </summary>
        public uint Frequency
        {
            get => _Frequency;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return IsPresent ? $"{_Frequency:N0} kHz" : "N/A";
        }
    }
}