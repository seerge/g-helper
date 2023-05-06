using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Contains information regarding the ECC Memory errors
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct ECCErrorInfoV1 : IInitializable
    {
        internal StructureVersion _Version;
        internal ECCErrorInfo _CurrentErrors;
        internal ECCErrorInfo _AggregatedErrors;

        /// <summary>
        ///     Gets the number of current errors
        /// </summary>
        public ECCErrorInfo CurrentErrors
        {
            get => _CurrentErrors;
        }

        /// <summary>
        ///     Gets the number of aggregated errors
        /// </summary>
        public ECCErrorInfo AggregatedErrors
        {
            get => _AggregatedErrors;
        }

        /// <summary>
        ///     Contains ECC memory error counters information
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct ECCErrorInfo
        {
            internal ulong _SingleBitErrors;
            internal ulong _DoubleBitErrors;

            /// <summary>
            ///     Gets the number of single bit errors
            /// </summary>
            public ulong SingleBitErrors
            {
                get => _SingleBitErrors;
            }

            /// <summary>
            ///     Gets the number of double bit errors
            /// </summary>
            public ulong DoubleBitErrors
            {
                get => _DoubleBitErrors;
            }
        }
    }
}