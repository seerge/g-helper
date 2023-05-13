using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.Stereo.Structures
{
    /// <summary>
    ///     Holds information regarding the stereo capabilities of a monitor
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct StereoCapabilitiesV1 : IInitializable
    {
        internal StructureVersion _Version;
        internal uint _Flags;
        internal uint _Reserved1;
        internal uint _Reserved2;
        internal uint _Reserved3;

        /// <summary>
        ///     Gets a boolean value indicating if no windowed mode is supported
        /// </summary>
        public bool IsNoWindowedModeSupported
        {
            get => _Flags.GetBit(0);
        }

        /// <summary>
        ///     Gets a boolean value indicating if automatic windowed mode is supported
        /// </summary>
        public bool IsAutomaticWindowedModeSupported
        {
            get => _Flags.GetBit(1);
        }

        /// <summary>
        ///     Gets a boolean value indicating if the persistent windowed mode is supported
        /// </summary>
        public bool IsPersistentWindowedModeSupported
        {
            get => _Flags.GetBit(2);
        }
    }
}