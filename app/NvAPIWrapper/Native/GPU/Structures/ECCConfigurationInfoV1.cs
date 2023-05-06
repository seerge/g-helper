using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Contains information about the ECC memory configurations
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct ECCConfigurationInfoV1 : IInitializable
    {
        internal StructureVersion _Version;
        internal uint _Flags;

        /// <summary>
        ///     Gets a boolean value indicating if the ECC memory is enabled
        /// </summary>
        public bool IsEnabled
        {
            get => _Flags.GetBit(0);
        }

        /// <summary>
        ///     Gets a boolean value indicating if the ECC memory is enabled by default
        /// </summary>
        public bool IsEnabledByDefault
        {
            get => _Flags.GetBit(1);
        }
    }
}