using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Contains information regarding the ECC Memory status
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct ECCStatusInfoV1 : IInitializable
    {
        internal StructureVersion _Version;
        internal uint _IsSupported;
        internal ECCConfiguration _ConfigurationOptions;
        internal uint _IsEnabled;

        /// <summary>
        ///     Gets a boolean value indicating if the ECC memory is available and supported
        /// </summary>
        public bool IsSupported
        {
            get => _IsSupported.GetBit(0);
        }

        /// <summary>
        ///     Gets the ECC memory configurations
        /// </summary>
        public ECCConfiguration ConfigurationOptions
        {
            get => _ConfigurationOptions;
        }

        /// <summary>
        ///     Gets boolean value indicating if the ECC memory is currently enabled
        /// </summary>
        public bool IsEnabled
        {
            get => _IsEnabled.GetBit(0);
        }
    }
}