using System;
using NvAPIWrapper.Native.Display;
using NvAPIWrapper.Native.Display.Structures;

namespace NvAPIWrapper.Display
{
    /// <summary>
    ///     Hold information about a custom display resolution
    /// </summary>
    public class CustomResolution : IEquatable<CustomResolution>
    {
        /// <summary>
        ///     Creates an instance of <see cref="CustomResolution" />.
        /// </summary>
        /// <param name="width">The screen width.</param>
        /// <param name="height">The screen height.</param>
        /// <param name="colorFormat">The color format.</param>
        /// <param name="timing">The resolution timing.</param>
        /// <param name="xRatio">The horizontal scaling ratio.</param>
        /// <param name="yRatio">The vertical scaling ratio.</param>
        public CustomResolution(
            uint width,
            uint height,
            ColorFormat colorFormat,
            Timing timing,
            float xRatio = 1,
            float yRatio = 1
        )
        {
            if (xRatio <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(xRatio));
            }

            if (yRatio <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(yRatio));
            }

            Width = width;
            Height = height;
            ColorFormat = colorFormat;
            XRatio = xRatio;
            YRatio = yRatio;
            Timing = timing;

            switch (ColorFormat)
            {
                case ColorFormat.P8:
                    ColorDepth = 8;

                    break;
                case ColorFormat.R5G6B5:
                    ColorDepth = 16;

                    break;
                case ColorFormat.A8R8G8B8:
                    ColorDepth = 24;

                    break;
                case ColorFormat.A16B16G16R16F:
                    ColorDepth = 32;

                    break;
                default:
                    throw new ArgumentException("Color format is invalid.", nameof(colorFormat));
            }
        }

        /// <summary>
        ///     Creates an instance of <see cref="CustomResolution" />.
        /// </summary>
        /// <param name="width">The screen width.</param>
        /// <param name="height">The screen height.</param>
        /// <param name="colorDepth">The color depth.</param>
        /// <param name="timing">The resolution timing.</param>
        /// <param name="xRatio">The horizontal scaling ratio.</param>
        /// <param name="yRatio">The vertical scaling ratio.</param>
        public CustomResolution(
            uint width,
            uint height,
            uint colorDepth,
            Timing timing,
            float xRatio = 1,
            float yRatio = 1)
        {
            if (xRatio <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(xRatio));
            }

            if (yRatio <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(yRatio));
            }

            if (colorDepth != 0 && colorDepth != 8 && colorDepth != 16 && colorDepth != 24 && colorDepth != 32)
            {
                throw new ArgumentOutOfRangeException(nameof(colorDepth));
            }

            Width = width;
            Height = height;
            ColorDepth = colorDepth;
            ColorFormat = ColorFormat.Unknown;
            XRatio = xRatio;
            YRatio = yRatio;
            Timing = timing;
        }

        internal CustomResolution(CustomDisplay customDisplay)
        {
            Width = customDisplay.Width;
            Height = customDisplay.Height;
            ColorDepth = customDisplay.Depth;
            ColorFormat = customDisplay.ColorFormat;
            Timing = customDisplay.Timing;
            XRatio = customDisplay.XRatio;
            YRatio = customDisplay.YRatio;
        }

        /// <summary>
        ///     Gets the source surface color depth. "0" means all 8/16/32bpp.
        /// </summary>
        public uint ColorDepth { get; }

        /// <summary>
        ///     Gets the color format (optional)
        /// </summary>
        public ColorFormat ColorFormat { get; }

        /// <summary>
        ///     Gets the source surface (source mode) height.
        /// </summary>
        public uint Height { get; }

        /// <summary>
        ///     Gets the timing used to program TMDS/DAC/LVDS/HDMI/TVEncoder, etc.
        /// </summary>
        public Timing Timing { get; }

        /// <summary>
        ///     Gets the source surface (source mode) width.
        /// </summary>
        public uint Width { get; }

        /// <summary>
        ///     Gets the horizontal scaling ratio.
        /// </summary>
        public float XRatio { get; }

        /// <summary>
        ///     Gets the vertical scaling ratio.
        /// </summary>
        public float YRatio { get; }

        /// <inheritdoc />
        public bool Equals(CustomResolution other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Width == other.Width &&
                   Height == other.Height &&
                   ColorDepth == other.ColorDepth &&
                   Timing.Equals(other.Timing) &&
                   ColorFormat == other.ColorFormat &&
                   XRatio.Equals(other.XRatio) &&
                   YRatio.Equals(other.YRatio);
        }

        /// <summary>
        ///     Compares two instance of <see cref="CustomResolution" /> for equality.
        /// </summary>
        /// <param name="left">An first instance of <see cref="CustomResolution" /> to compare.</param>
        /// <param name="right">An Second instance of <see cref="CustomResolution" /> to compare.</param>
        /// <returns>True if both instances are equal, otherwise false.</returns>
        public static bool operator ==(CustomResolution left, CustomResolution right)
        {
            return Equals(left, right);
        }

        /// <summary>
        ///     Compares two instance of <see cref="CustomResolution" /> for inequality.
        /// </summary>
        /// <param name="left">An first instance of <see cref="CustomResolution" /> to compare.</param>
        /// <param name="right">An Second instance of <see cref="CustomResolution" /> to compare.</param>
        /// <returns>True if both instances are not equal, otherwise false.</returns>
        public static bool operator !=(CustomResolution left, CustomResolution right)
        {
            return !Equals(left, right);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((CustomResolution) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) Width;
                hashCode = (hashCode * 397) ^ (int) Height;
                hashCode = (hashCode * 397) ^ (int) ColorDepth;
                hashCode = (hashCode * 397) ^ Timing.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) ColorFormat;
                hashCode = (hashCode * 397) ^ XRatio.GetHashCode();
                hashCode = (hashCode * 397) ^ YRatio.GetHashCode();

                return hashCode;
            }
        }

        internal CustomDisplay AsCustomDisplay(bool hardwareModeSetOnly)
        {
            return new CustomDisplay(Width, Height, ColorDepth, ColorFormat, XRatio, YRatio, Timing,
                hardwareModeSetOnly);
        }
    }
}