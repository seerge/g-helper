using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;
using NvAPIWrapper.Native.Interfaces.Display;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <inheritdoc cref="IDisplayDVCInfo" />
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct PrivateDisplayDVCInfoEx : IInitializable, IDisplayDVCInfo
    {
        internal StructureVersion _Version;
        internal int _CurrentLevel;
        internal int _MinimumLevel;
        internal int _MaximumLevel;
        internal int _DefaultLevel;

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
        public int MaximumLevel
        {
            get => _MaximumLevel;
        }

        /// <inheritdoc />
        public int DefaultLevel
        {
            get => _DefaultLevel;
        }

        internal PrivateDisplayDVCInfoEx(int currentLevel)
        {
            this = typeof(PrivateDisplayDVCInfoEx).Instantiate<PrivateDisplayDVCInfoEx>();
            _CurrentLevel = currentLevel;
        }
    }
}