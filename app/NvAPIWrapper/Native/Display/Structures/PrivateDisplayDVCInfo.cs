using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Interfaces;
using NvAPIWrapper.Native.Interfaces.Display;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <inheritdoc cref="IDisplayDVCInfo" />
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct PrivateDisplayDVCInfo : IInitializable, IDisplayDVCInfo
    {
        internal StructureVersion _Version;
        internal int _CurrentLevel;
        internal int _MinimumLevel;
        internal int _MaximumLevel;

        /// <inheritdoc />
        public int CurrentLevel
        {
            get => _CurrentLevel;
        }

        /// <inheritdoc />
        public int MinimumLevel
        {
            get => _MinimumLevel;
        }

        /// <inheritdoc />
        int IDisplayDVCInfo.DefaultLevel
        {
            get => 0;
        }

        /// <inheritdoc />
        public int MaximumLevel
        {
            get => _MaximumLevel;
        }
    }
}