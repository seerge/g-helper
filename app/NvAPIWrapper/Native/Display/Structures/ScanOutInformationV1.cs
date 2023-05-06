using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Interfaces;
using Rectangle = NvAPIWrapper.Native.General.Structures.Rectangle;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <summary>
    ///     Contains information regarding the scan-out configurations
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct ScanOutInformationV1 : IInitializable
    {
        internal StructureVersion _Version;
        internal Rectangle _SourceDesktopRectangle;
        internal Rectangle _SourceViewPortRectangle;
        internal Rectangle _TargetViewPortRectangle;
        internal uint _TargetDisplayWidth;
        internal uint _TargetDisplayHeight;
        internal uint _CloneImportance;
        internal Rotate _SourceToTargetRotation;

        /// <summary>
        ///     Gets the operating system display device rectangle in desktop coordinates displayId is scanning out from.
        /// </summary>
        public Rectangle SourceDesktopRectangle
        {
            get => _SourceDesktopRectangle;
        }

        /// <summary>
        ///     Gets the area inside the SourceDesktopRectangle which is scanned out to the display.
        /// </summary>
        public Rectangle SourceViewPortRectangle
        {
            get => _SourceViewPortRectangle;
        }

        /// <summary>
        ///     Gets the area inside the rectangle described by targetDisplayWidth/Height SourceViewPortRectangle is scanned out
        ///     to.
        /// </summary>
        public Rectangle TargetViewPortRectangle
        {
            get => _TargetViewPortRectangle;
        }

        /// <summary>
        ///     Gets the horizontal size of the active resolution scanned out to the display.
        /// </summary>
        public uint TargetDisplayWidth
        {
            get => _TargetDisplayWidth;
        }

        /// <summary>
        ///     Gets the vertical size of the active resolution scanned out to the display.
        /// </summary>
        public uint TargetDisplayHeight
        {
            get => _TargetDisplayHeight;
        }

        /// <summary>
        ///     Gets the clone importance assigned to the target if the target is a cloned view of the SourceDesktopRectangle
        ///     (0:primary,1 secondary,...).
        /// </summary>
        public uint CloneImportance
        {
            get => _CloneImportance;
        }

        /// <summary>
        ///     Gets the rotation performed between the SourceViewPortRectangle and the TargetViewPortRectangle.
        /// </summary>
        public Rotate SourceToTargetRotation
        {
            get => _SourceToTargetRotation;
        }
    }
}