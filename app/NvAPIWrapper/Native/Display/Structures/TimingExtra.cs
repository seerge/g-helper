using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <summary>
    ///     Holds NVIDIA-specific timing extras
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct TimingExtra : IInitializable, IEquatable<TimingExtra>
    {
        internal readonly uint _HardwareFlags;
        internal readonly ushort _RefreshRate;
        internal readonly uint _FrequencyInMillihertz;
        internal readonly ushort _VerticalAspect;
        internal readonly ushort _HorizontalAspect;
        internal readonly ushort _HorizontalPixelRepetition;
        internal readonly uint _Standard;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        internal string _Name;

        /// <summary>
        ///     Creates a new instance of <see cref="TimingExtra" /> structure.
        /// </summary>
        /// <param name="frequencyInHertz">The timing frequency in hertz</param>
        /// <param name="name">The timing source name</param>
        /// <param name="horizontalAspect">The display horizontal aspect</param>
        /// <param name="verticalAspect">The display vertical aspect</param>
        /// <param name="horizontalPixelRepetition">The number of identical horizontal pixels that are repeated; 1 = no repetition</param>
        /// <param name="hardwareFlags">The NVIDIA hardware-based enhancement, such as double-scan.</param>
        public TimingExtra(
            double frequencyInHertz,
            string name,
            ushort horizontalAspect = 0,
            ushort verticalAspect = 0,
            ushort horizontalPixelRepetition = 1,
            uint hardwareFlags = 0
        ) : this(
            (uint) (frequencyInHertz * 1000d),
            (ushort) frequencyInHertz,
            name,
            horizontalAspect,
            verticalAspect,
            horizontalPixelRepetition,
            hardwareFlags
        )
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="TimingExtra" /> structure.
        /// </summary>
        /// <param name="frequencyInMillihertz">The timing frequency in millihertz</param>
        /// <param name="refreshRate">The refresh rate</param>
        /// <param name="name">The timing source name</param>
        /// <param name="horizontalAspect">The display horizontal aspect</param>
        /// <param name="verticalAspect">The display vertical aspect</param>
        /// <param name="horizontalPixelRepetition">The number of identical horizontal pixels that are repeated; 1 = no repetition</param>
        /// <param name="hardwareFlags">The NVIDIA hardware-based enhancement, such as double-scan.</param>
        public TimingExtra(
            uint frequencyInMillihertz,
            ushort refreshRate,
            string name,
            ushort horizontalAspect = 0,
            ushort verticalAspect = 0,
            ushort horizontalPixelRepetition = 1,
            uint hardwareFlags = 0
        )
        {
            this = typeof(TimingExtra).Instantiate<TimingExtra>();
            _FrequencyInMillihertz = frequencyInMillihertz;
            _RefreshRate = refreshRate;
            _HorizontalAspect = horizontalAspect;
            _VerticalAspect = verticalAspect;
            _HorizontalPixelRepetition = horizontalPixelRepetition;
            _HardwareFlags = hardwareFlags;
            _Name = name?.Length > 40 ? name.Substring(0, 40) : name ?? "";
        }

        /// <inheritdoc />
        public bool Equals(TimingExtra other)
        {
            return _HardwareFlags == other._HardwareFlags &&
                   _RefreshRate == other._RefreshRate &&
                   _FrequencyInMillihertz == other._FrequencyInMillihertz &&
                   _VerticalAspect == other._VerticalAspect &&
                   _HorizontalAspect == other._HorizontalAspect &&
                   _HorizontalPixelRepetition == other._HorizontalPixelRepetition &&
                   _Standard == other._Standard;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is TimingExtra extra && Equals(extra);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) _HardwareFlags;
                hashCode = (hashCode * 397) ^ _RefreshRate.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) _FrequencyInMillihertz;
                hashCode = (hashCode * 397) ^ _VerticalAspect.GetHashCode();
                hashCode = (hashCode * 397) ^ _HorizontalAspect.GetHashCode();
                hashCode = (hashCode * 397) ^ _HorizontalPixelRepetition.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) _Standard;

                return hashCode;
            }
        }

        /// <summary>
        ///     Gets the NVIDIA hardware-based enhancement, such as double-scan.
        /// </summary>
        public uint HardwareFlags
        {
            get => _HardwareFlags;
        }

        /// <summary>
        ///     Gets the logical refresh rate to present
        /// </summary>
        public int RefreshRate
        {
            get => _RefreshRate;
        }

        /// <summary>
        ///     Gets the physical vertical refresh rate in 0.001Hz
        /// </summary>
        public int FrequencyInMillihertz
        {
            get => (int) _FrequencyInMillihertz;
        }

        /// <summary>
        ///     Gets the display vertical aspect
        /// </summary>
        public int VerticalAspect
        {
            get => _VerticalAspect;
        }

        /// <summary>
        ///     Gets the display horizontal aspect
        /// </summary>
        public int HorizontalAspect
        {
            get => _HorizontalAspect;
        }

        /// <summary>
        ///     Gets the bit-wise pixel repetition factor: 0x1:no pixel repetition; 0x2:each pixel repeats twice horizontally,..
        /// </summary>
        public int PixelRepetition
        {
            get => _HorizontalPixelRepetition;
        }

        /// <summary>
        ///     Gets the timing standard
        /// </summary>
        public uint Standard
        {
            get => _Standard;
        }

        /// <summary>
        ///     Gets the timing name
        /// </summary>
        public string Name
        {
            get => _Name;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        ///     Checks two instance of <see cref="TimingExtra" /> for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>Returns a boolean value indicating if the two instances are equal; otherwise false</returns>
        public static bool operator ==(TimingExtra left, TimingExtra right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Checks two instance of <see cref="TimingExtra" /> for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>Returns a boolean value indicating if the two instances are equal; otherwise false</returns>
        public static bool operator !=(TimingExtra left, TimingExtra right)
        {
            return !(left == right);
        }
    }
}