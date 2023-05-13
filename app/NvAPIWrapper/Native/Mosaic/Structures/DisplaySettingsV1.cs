using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;
using NvAPIWrapper.Native.Interfaces.Mosaic;

namespace NvAPIWrapper.Native.Mosaic.Structures
{
    /// <summary>
    ///     Holds a display setting
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct DisplaySettingsV1 : IDisplaySettings,
        IInitializable,
        IEquatable<DisplaySettingsV1>,
        IEquatable<DisplaySettingsV2>
    {
        internal StructureVersion _Version;
        internal readonly uint _Width;
        internal readonly uint _Height;
        internal readonly uint _BitsPerPixel;
        internal readonly uint _Frequency;

        /// <summary>
        ///     Creates a new DisplaySettingsV1
        /// </summary>
        /// <param name="width">Per-display width</param>
        /// <param name="height">Per-display height</param>
        /// <param name="bitsPerPixel">Bits per pixel</param>
        /// <param name="frequency">Display frequency</param>
        // ReSharper disable once TooManyDependencies
        public DisplaySettingsV1(int width, int height, int bitsPerPixel, int frequency)
        {
            this = typeof(DisplaySettingsV1).Instantiate<DisplaySettingsV1>();
            _Width = (uint) width;
            _Height = (uint) height;
            _BitsPerPixel = (uint) bitsPerPixel;
            _Frequency = (uint) frequency;
        }


        /// <inheritdoc />
        public bool Equals(DisplaySettingsV1 other)
        {
            return _Width == other._Width &&
                   _Height == other._Height &&
                   _BitsPerPixel == other._BitsPerPixel &&
                   _Frequency == other._Frequency;
        }

        /// <inheritdoc />
        public bool Equals(DisplaySettingsV2 other)
        {
            return _Width == other._Width &&
                   _Height == other._Height &&
                   _BitsPerPixel == other._BitsPerPixel &&
                   _Frequency == other._Frequency;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is DisplaySettingsV1 v1 && Equals(v1);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) _Width;
                hashCode = (hashCode * 397) ^ (int) _Height;
                hashCode = (hashCode * 397) ^ (int) _BitsPerPixel;
                hashCode = (hashCode * 397) ^ (int) _Frequency;

                return hashCode;
            }
        }

        /// <summary>
        ///     Checks for equality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are equal, otherwise false</returns>
        public static bool operator ==(DisplaySettingsV1 left, DisplaySettingsV1 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(DisplaySettingsV1 left, DisplaySettingsV1 right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc />
        public int Width
        {
            get => (int) _Width;
        }

        /// <inheritdoc />
        public int Height
        {
            get => (int) _Height;
        }

        /// <inheritdoc />
        public int BitsPerPixel
        {
            get => (int) _BitsPerPixel;
        }

        /// <inheritdoc />
        public int Frequency
        {
            get => (int) _Frequency;
        }

        /// <inheritdoc />
        public uint FrequencyInMillihertz
        {
            get => _Frequency * 1000;
        }
    }
}