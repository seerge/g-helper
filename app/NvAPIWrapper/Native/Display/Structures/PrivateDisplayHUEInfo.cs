using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <summary>
    ///     Holds the current and the default HUE information
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct PrivateDisplayHUEInfo : IInitializable
    {
        internal StructureVersion _Version;
        internal int _CurrentAngle;
        internal int _DefaultAngle;

        /// <summary>
        ///     Gets or sets the current HUE offset angle [0-359]
        /// </summary>
        public int CurrentAngle
        {
            get => _CurrentAngle;
        }

        /// <summary>
        ///     Gets or sets the default HUE offset angle [0-359]
        /// </summary>
        public int DefaultAngle
        {
            get => _DefaultAngle;
        }
    }
}