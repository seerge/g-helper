using System;
using NvAPIWrapper.Native.Display;
using NvAPIWrapper.Native.Display.Structures;
using NvAPIWrapper.Native.Interfaces.Display;

namespace NvAPIWrapper.Display
{
    /// <inheritdoc cref="IHDRColorData" />
    public class HDRColorData : IHDRColorData, IEquatable<HDRColorData>
    {
        /// <summary>
        ///     Creates an instance of <see cref="HDRColorData" />.
        /// </summary>
        /// <param name="hdrMode">The hdr mode.</param>
        /// <param name="masteringDisplayData">The display color space configurations.</param>
        /// <param name="colorFormat">The color data color format.</param>
        /// <param name="dynamicRange">The color data dynamic range.</param>
        /// <param name="colorDepth">The color data color depth.</param>
        public HDRColorData(
            ColorDataHDRMode hdrMode,
            MasteringDisplayColorData masteringDisplayData,
            ColorDataFormat? colorFormat = null,
            ColorDataDynamicRange? dynamicRange = null,
            ColorDataDepth? colorDepth = null
        )
        {
            HDRMode = hdrMode;
            MasteringDisplayData = masteringDisplayData;
            ColorFormat = colorFormat;
            DynamicRange = dynamicRange;
            ColorDepth = colorDepth;
        }

        internal HDRColorData(IHDRColorData colorData)
        {
            HDRMode = colorData.HDRMode;
            MasteringDisplayData = colorData.MasteringDisplayData;
            ColorDepth = colorData.ColorDepth;
            ColorFormat = colorData.ColorFormat;
            DynamicRange = colorData.DynamicRange;
        }

        /// <inheritdoc />
        public bool Equals(HDRColorData other)
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
                   DynamicRange == other.DynamicRange &&
                   HDRMode == other.HDRMode &&
                   MasteringDisplayData.Equals(other.MasteringDisplayData);
        }

        /// <inheritdoc />
        public ColorDataDepth? ColorDepth { get; }

        /// <inheritdoc />
        public ColorDataFormat? ColorFormat { get; }

        /// <inheritdoc />
        public ColorDataDynamicRange? DynamicRange { get; }

        /// <inheritdoc />
        public ColorDataHDRMode HDRMode { get; }

        /// <inheritdoc />
        public MasteringDisplayColorData MasteringDisplayData { get; }

        /// <summary>
        ///     Compares two instances of <see cref="HDRColorData" /> for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>true if two instances are equal; otherwise false.</returns>
        public static bool operator ==(HDRColorData left, HDRColorData right)
        {
            return left?.Equals(right) == true;
        }

        /// <summary>
        ///     Compares two instances of <see cref="HDRColorData" /> for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>true if two instances are not equal; otherwise false.</returns>
        public static bool operator !=(HDRColorData left, HDRColorData right)
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

            return Equals((HDRColorData) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ColorDepth.GetHashCode();
                hashCode = (hashCode * 397) ^ ColorFormat.GetHashCode();
                hashCode = (hashCode * 397) ^ DynamicRange.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) HDRMode;
                hashCode = (hashCode * 397) ^ MasteringDisplayData.GetHashCode();

                return hashCode;
            }
        }

        internal HDRColorDataV1 AsHDRColorDataV1(ColorDataHDRCommand command)
        {
            return new HDRColorDataV1(
                command,
                HDRMode,
                MasteringDisplayData
            );
        }

        internal HDRColorDataV2 AsHDRColorDataV2(ColorDataHDRCommand command)
        {
            return new HDRColorDataV2(
                command,
                HDRMode,
                MasteringDisplayData,
                ColorFormat ?? ColorDataFormat.Auto,
                DynamicRange ?? ColorDataDynamicRange.Auto,
                ColorDepth ?? ColorDataDepth.Default
            );
        }
    }
}