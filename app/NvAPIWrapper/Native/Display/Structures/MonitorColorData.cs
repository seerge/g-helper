using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <summary>
    /// Contains information about a monitor color data
    /// </summary>
    [StructureVersion(1)]
    [StructLayout(LayoutKind.Explicit, Pack = 8, Size = 12)]
    public struct MonitorColorData : IInitializable
    {
        [FieldOffset(0)]
        internal StructureVersion _Version;
        [FieldOffset(4)]
        private readonly DisplayPortColorFormat _ColorFormat;
        [FieldOffset(8)]
        private readonly DisplayPortColorDepth _ColorDepth;

        /// <summary>
        ///Gets the monitor display port color format
        /// </summary>
        // ReSharper disable once ConvertToAutoProperty
        public DisplayPortColorFormat ColorFormat
        {
            get => _ColorFormat;
        }

        /// <summary>
        /// Gets the monitor display port color depth
        /// </summary>
        // ReSharper disable once ConvertToAutoProperty
        public DisplayPortColorDepth ColorDepth
        {
            get
            {
                switch ((uint) _ColorDepth)
                {
                    case 6:
                        return DisplayPortColorDepth.BPC6;
                    case 8:
                        return DisplayPortColorDepth.BPC8;
                    case 10:
                        return DisplayPortColorDepth.BPC10;
                    case 12:
                        return DisplayPortColorDepth.BPC12;
                    case 16:
                        return DisplayPortColorDepth.BPC16;
                    default:
                        return _ColorDepth;
                }
            }
        }
    }
}