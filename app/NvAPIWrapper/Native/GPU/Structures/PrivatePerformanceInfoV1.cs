using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Contains information regarding GPU performance limitations
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct PrivatePerformanceInfoV1 : IInitializable
    {
        internal const int MaxNumberOfUnknown2 = 16;

        internal StructureVersion _Version;
        internal uint _Unknown1;
        internal PerformanceLimit _SupportedLimits;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxNumberOfUnknown2)]
        internal uint[] _Unknown2;

        /// <summary>
        ///     Gets a boolean value indicating if performance limit by power usage is supported.
        /// </summary>
        public bool IsPowerLimitSupported
        {
            get => _SupportedLimits.HasFlag(PerformanceLimit.PowerLimit);
        }


        /// <summary>
        ///     Gets a boolean value indicating if performance limit by temperature is supported.
        /// </summary>
        public bool IsTemperatureLimitSupported
        {
            get => _SupportedLimits.HasFlag(PerformanceLimit.TemperatureLimit);
        }


        /// <summary>
        ///     Gets a boolean value indicating if performance limit by voltage usage is supported.
        /// </summary>
        public bool IsVoltageLimitSupported
        {
            get => _SupportedLimits.HasFlag(PerformanceLimit.VoltageLimit);
        }


        /// <summary>
        ///     Gets a boolean value indicating if performance limit by detecting no load is supported.
        /// </summary>
        public bool IsNoLoadLimitSupported
        {
            get => _SupportedLimits.HasFlag(PerformanceLimit.NoLoadLimit);
        }
    }
}