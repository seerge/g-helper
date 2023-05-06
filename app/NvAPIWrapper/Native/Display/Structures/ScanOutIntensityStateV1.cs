using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <summary>
    ///     Contains information regarding the scan-out intensity state
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct ScanOutIntensityStateV1 : IInitializable
    {
        internal StructureVersion _Version;
        internal uint _IsEnabled;

        /// <summary>
        ///     Gets a boolean value indicating if the scan out intensity is enabled or not
        /// </summary>
        public bool IsEnabled
        {
            get => _IsEnabled > 0;
        }
    }
}