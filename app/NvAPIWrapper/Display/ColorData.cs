using System;
using NvAPIWrapper.Native.Display;
using NvAPIWrapper.Native.Display.Structures;
using NvAPIWrapper.Native.Interfaces.Display;

namespace NvAPIWrapper.Display
{
    /// <inheritdoc cref="IColorData" />
    public class ColorData : IColorData, IEquatable<ColorData>
    {
        /// <summary>
        ///     Creates an instance of <see cref="ColorData" /> to modify the color data
        /// </summary>
        /// <param name="colorFormat">The color data color format.</param>
        /// <param name="colorimetry">The color data color space.</param>
        /// <param name="dynamicRange">The color data dynamic range.</param>
        /// <param name="colorDepth">The color data color depth.</param>
        /// <param name="colorSelectionPolicy">The color data selection policy.</param>
        /// <param name="desktopColorDepth">The color data desktop color depth.</param>
        public ColorData(
            ColorDataFormat colorFormat = ColorDataFormat.Auto,
            ColorDataColorimetry colorimetry = ColorDataColorimetry.Auto,
            ColorDataDynamicRange? dynamicRange = null,
            ColorDataDepth? colorDepth = null,
            ColorDataSelectionPolicy? colorSelectionPolicy = null,
            ColorDataDesktopDepth? desktopColorDepth = null
        )
        {
            ColorFormat = colorFormat;
            Colorimetry = colorimetry;
            DynamicRange = dynamicRange;
            ColorDepth = colorDepth;
            SelectionPolicy = colorSelectionPolicy;
            DesktopColorDepth = desktopColorDepth;
        }

        internal ColorData(IColorData colorData)
        {
            ColorDepth = colorData.ColorDepth;
            DynamicRange = colorData.DynamicRange;
            ColorFormat = colorData.ColorFormat;
            Colorimetry = colorData.Colorimetry;
            SelectionPolicy = colorData.SelectionPolicy;
            DesktopColorDepth = colorData.DesktopColorDepth;
        }

        /// <inheritdoc />
        public ColorDataDepth? ColorDepth { get; }

        /// <inheritdoc />
        public ColorDataFormat ColorFormat { get; }

        /// <inheritdoc />
        public ColorDataColorimetry Colorimetry { get; }

        /// <inheritdoc />
        public ColorDataDesktopDepth? DesktopColorDepth { get; }

        /// <inheritdoc />
        public ColorDataDynamicRange? DynamicRange { get; }

        /// <inheritdoc />
        public ColorDataSelectionPolicy? SelectionPolicy { get; }

        /// <inheritdoc />
        public bool Equals(ColorData other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return ColorDepth == other.ColorDepth &&
                   ColorFormat == other.ColorFormat &&
                   Colorimetry == other.Colorimetry &&
                   DesktopColorDepth == other.DesktopColorDepth &&
                   DynamicRange == other.DynamicRange &&
                   SelectionPolicy == other.SelectionPolicy;
        }

        /// <summary>
        ///     Compares two instances of <see cref="ColorData" /> for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>true if two instances are equal; otherwise false.</returns>
        public static bool operator ==(ColorData left, ColorData right)
        {
            return left?.Equals(right) == true;
        }

        /// <summary>
        ///     Compares two instances of <see cref="ColorData" /> for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>true if two instances are not equal; otherwise false.</returns>
        public static bool operator !=(ColorData left, ColorData right)
        {
            return !(left == right);
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

            return Equals((ColorData) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ColorDepth.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) ColorFormat;
                hashCode = (hashCode * 397) ^ (int) Colorimetry;
                hashCode = (hashCode * 397) ^ DesktopColorDepth.GetHashCode();
                hashCode = (hashCode * 397) ^ DynamicRange.GetHashCode();
                hashCode = (hashCode * 397) ^ SelectionPolicy.GetHashCode();

                return hashCode;
            }
        }

        internal ColorDataV1 AsColorDataV1(ColorDataCommand command)
        {
            return new ColorDataV1(
                command,
                ColorFormat,
                Colorimetry
            );
        }

        internal ColorDataV2 AsColorDataV2(ColorDataCommand command)
        {
            return new ColorDataV2(
                command,
                ColorFormat,
                Colorimetry,
                DynamicRange ?? ColorDataDynamicRange.Auto
            );
        }

        internal ColorDataV3 AsColorDataV3(ColorDataCommand command)
        {
            return new ColorDataV3(
                command,
                ColorFormat,
                Colorimetry,
                DynamicRange ?? ColorDataDynamicRange.Auto,
                ColorDepth ?? ColorDataDepth.Default
            );
        }

        internal ColorDataV4 AsColorDataV4(ColorDataCommand command)
        {
            return new ColorDataV4(
                command,
                ColorFormat,
                Colorimetry,
                DynamicRange ?? ColorDataDynamicRange.Auto,
                ColorDepth ?? ColorDataDepth.Default,
                SelectionPolicy ?? ColorDataSelectionPolicy.Default
            );
        }

        internal ColorDataV5 AsColorDataV5(ColorDataCommand command)
        {
            return new ColorDataV5(
                command,
                ColorFormat,
                Colorimetry,
                DynamicRange ?? ColorDataDynamicRange.Auto,
                ColorDepth ?? ColorDataDepth.Default,
                SelectionPolicy ?? ColorDataSelectionPolicy.Default,
                DesktopColorDepth ?? ColorDataDesktopDepth.Default
            );
        }
    }
}