using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <summary>
    ///     Hold information about a custom display resolution
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct CustomDisplay : IInitializable
    {
        internal StructureVersion _Version;
        internal uint _Width;
        internal uint _Height;
        internal uint _Depth;
        internal ColorFormat _ColorFormat;
        internal ViewPortF _SourcePartition;
        internal float _XRatio;
        internal float _YRatio;
        internal Timing _Timing;
        internal uint _Flags;

        /// <summary>
        ///     Gets the source surface (source mode) width.
        /// </summary>
        public uint Width
        {
            get => _Width;
        }

        /// <summary>
        ///     Gets the source surface (source mode) height.
        /// </summary>
        public uint Height
        {
            get => _Height;
        }

        /// <summary>
        ///     Gets the source surface color depth. "0" means all 8/16/32bpp.
        /// </summary>
        public uint Depth
        {
            get => _Depth;
        }

        /// <summary>
        ///     Gets the color format (optional)
        /// </summary>
        public ColorFormat ColorFormat
        {
            get => _ColorFormat;
        }

        /// <summary>
        ///     Gets the source partition viewport. All values are between [0, 1]. For multi-mon support, should be set to
        ///     (0,0,1.0,1.0) for now.
        /// </summary>
        public ViewPortF SourcePartition
        {
            get => _SourcePartition;
        }

        /// <summary>
        ///     Gets the horizontal scaling ratio.
        /// </summary>
        public float XRatio
        {
            get => _XRatio;
        }

        /// <summary>
        ///     Gets the vertical scaling ratio.
        /// </summary>
        public float YRatio
        {
            get => _YRatio;
        }

        /// <summary>
        ///     Gets the timing used to program TMDS/DAC/LVDS/HDMI/TVEncoder, etc.
        /// </summary>
        public Timing Timing
        {
            get => _Timing;
        }

        /// <summary>
        ///     Gets a boolean value indicating that a hardware mode-set without OS update should be performed.
        /// </summary>
        public bool HardwareModeSetOnly
        {
            get => _Flags.GetBit(0);
        }

        /// <summary>
        ///     Creates an instance of <see cref="CustomDisplay" />
        /// </summary>
        /// <param name="width">The source surface (source mode) width.</param>
        /// <param name="height">The source surface (source mode) height.</param>
        /// <param name="depth">The source surface color depth. "0" means all 8/16/32bpp.</param>
        /// <param name="colorFormat">The color format (optional)</param>
        /// <param name="xRatio">The horizontal scaling ratio.</param>
        /// <param name="yRatio">The vertical scaling ratio.</param>
        /// <param name="timing">The timing used to program TMDS/DAC/LVDS/HDMI/TVEncoder, etc.</param>
        /// <param name="hwModeSetOnly">A boolean value indicating that a hardware mode-set without OS update should be performed.</param>
        public CustomDisplay(
            uint width,
            uint height,
            uint depth,
            ColorFormat colorFormat,
            float xRatio,
            float yRatio,
            Timing timing,
            bool hwModeSetOnly
        )
        {
            this = typeof(CustomDisplay).Instantiate<CustomDisplay>();

            _Width = width;
            _Height = height;
            _Depth = depth;
            _ColorFormat = colorFormat;
            _SourcePartition = new ViewPortF(0, 0, 1, 1);
            _XRatio = xRatio;
            _YRatio = yRatio;
            _Timing = timing;
            _Flags = _Flags.SetBit(0, hwModeSetOnly);
        }
    }
}