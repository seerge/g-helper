using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Helpers;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <summary>
    ///     Holds VESA scan out timing parameters
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct Timing : IEquatable<Timing>
    {
        internal readonly ushort _HorizontalVisible;
        internal readonly ushort _HorizontalBorder;
        internal readonly ushort _HorizontalFrontPorch;
        internal readonly ushort _HorizontalSyncWidth;
        internal readonly ushort _HorizontalTotal;
        internal readonly TimingHorizontalSyncPolarity _HorizontalSyncPolarity;
        internal readonly ushort _VerticalVisible;
        internal readonly ushort _VerticalBorder;
        internal readonly ushort _VerticalFrontPorch;
        internal readonly ushort _VerticalSyncWidth;
        internal readonly ushort _VerticalTotal;
        internal readonly TimingVerticalSyncPolarity _VerticalSyncPolarity;
        internal readonly TimingScanMode _ScanMode;
        internal readonly uint _PixelClockIn10KHertz;
        internal readonly TimingExtra _Extra;

        /// <summary>
        ///     Creates an instance of <see cref="Timing" /> structure.
        /// </summary>
        /// <param name="horizontalVisible">The horizontal visible pixels</param>
        /// <param name="verticalVisible">The vertical visible pixels</param>
        /// <param name="horizontalBorder">The horizontal border pixels</param>
        /// <param name="verticalBorder">The vertical border pixels</param>
        /// <param name="horizontalFrontPorch">The horizontal front porch pixels</param>
        /// <param name="verticalFrontPorch">The vertical front porch pixels</param>
        /// <param name="horizontalSyncWidth">The horizontal sync width pixels</param>
        /// <param name="verticalSyncWidth">The vertical sync width pixels</param>
        /// <param name="horizontalTotal">The horizontal total pixels</param>
        /// <param name="verticalTotal">The vertical total pixels</param>
        /// <param name="horizontalPolarity">The horizontal sync polarity</param>
        /// <param name="verticalPolarity">The vertical sync polarity</param>
        /// <param name="scanMode">The scan mode</param>
        /// <param name="extra">The extra timing information</param>
        public Timing(
            ushort horizontalVisible,
            ushort verticalVisible,
            ushort horizontalBorder,
            ushort verticalBorder,
            ushort horizontalFrontPorch,
            ushort verticalFrontPorch,
            ushort horizontalSyncWidth,
            ushort verticalSyncWidth,
            ushort horizontalTotal,
            ushort verticalTotal,
            TimingHorizontalSyncPolarity horizontalPolarity,
            TimingVerticalSyncPolarity verticalPolarity,
            TimingScanMode scanMode,
            TimingExtra extra
        )
        {
            this = typeof(Timing).Instantiate<Timing>();

            _HorizontalVisible = horizontalVisible;
            _HorizontalBorder = horizontalBorder;
            _HorizontalFrontPorch = horizontalFrontPorch;
            _HorizontalSyncWidth = horizontalSyncWidth;
            _HorizontalTotal = horizontalTotal;
            _HorizontalSyncPolarity = horizontalPolarity;

            _VerticalVisible = verticalVisible;
            _VerticalBorder = verticalBorder;
            _VerticalFrontPorch = verticalFrontPorch;
            _VerticalSyncWidth = verticalSyncWidth;
            _VerticalTotal = verticalTotal;
            _VerticalSyncPolarity = verticalPolarity;

            _ScanMode = scanMode;
            _PixelClockIn10KHertz =
                (uint) (horizontalTotal * verticalTotal * (extra.FrequencyInMillihertz / 1000d) / 10000);

            _Extra = extra;
        }

        /// <summary>
        ///     Creates an instance of <see cref="Timing" /> structure.
        /// </summary>
        /// <param name="horizontalVisible">The horizontal visible pixels</param>
        /// <param name="verticalVisible">The vertical visible pixels</param>
        /// <param name="horizontalBorder">The horizontal border pixels</param>
        /// <param name="verticalBorder">The vertical border pixels</param>
        /// <param name="horizontalFrontPorch">The horizontal front porch pixels</param>
        /// <param name="verticalFrontPorch">The vertical front porch pixels</param>
        /// <param name="horizontalSyncWidth">The horizontal sync width pixels</param>
        /// <param name="verticalSyncWidth">The vertical sync width pixels</param>
        /// <param name="horizontalTotal">The horizontal total pixels</param>
        /// <param name="verticalTotal">The vertical total pixels</param>
        /// <param name="horizontalPolarity">The horizontal sync polarity</param>
        /// <param name="verticalPolarity">The vertical sync polarity</param>
        /// <param name="scanMode">The scan mode</param>
        /// <param name="refreshRateFrequencyInHz">The frequency in hertz</param>
        /// <param name="horizontalPixelRepetition">The number of identical horizontal pixels that are repeated; 1 = no repetition</param>
        public Timing(
            ushort horizontalVisible,
            ushort verticalVisible,
            ushort horizontalBorder,
            ushort verticalBorder,
            ushort horizontalFrontPorch,
            ushort verticalFrontPorch,
            ushort horizontalSyncWidth,
            ushort verticalSyncWidth,
            ushort horizontalTotal,
            ushort verticalTotal,
            TimingHorizontalSyncPolarity horizontalPolarity,
            TimingVerticalSyncPolarity verticalPolarity,
            TimingScanMode scanMode,
            double refreshRateFrequencyInHz,
            ushort horizontalPixelRepetition = 1
        ) : this(
            horizontalVisible, verticalVisible, horizontalBorder,
            verticalBorder, horizontalFrontPorch, verticalFrontPorch,
            horizontalSyncWidth, verticalSyncWidth, horizontalTotal,
            verticalTotal, horizontalPolarity, verticalPolarity, scanMode,
            new TimingExtra(
                refreshRateFrequencyInHz,
                $"CUST:{horizontalVisible}x{verticalVisible}x{refreshRateFrequencyInHz:F3}Hz",
                0,
                0,
                horizontalPixelRepetition
            )
        )
        {
        }

        /// <inheritdoc />
        public bool Equals(Timing other)
        {
            return _HorizontalVisible == other._HorizontalVisible &&
                   _HorizontalBorder == other._HorizontalBorder &&
                   _HorizontalFrontPorch == other._HorizontalFrontPorch &&
                   _HorizontalSyncWidth == other._HorizontalSyncWidth &&
                   _HorizontalTotal == other._HorizontalTotal &&
                   _HorizontalSyncPolarity == other._HorizontalSyncPolarity &&
                   _VerticalVisible == other._VerticalVisible &&
                   _VerticalBorder == other._VerticalBorder &&
                   _VerticalFrontPorch == other._VerticalFrontPorch &&
                   _VerticalSyncWidth == other._VerticalSyncWidth &&
                   _VerticalTotal == other._VerticalTotal &&
                   _VerticalSyncPolarity == other._VerticalSyncPolarity &&
                   _ScanMode == other._ScanMode &&
                   _PixelClockIn10KHertz == other._PixelClockIn10KHertz &&
                   _Extra.Equals(other._Extra);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Timing timing && Equals(timing);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _HorizontalVisible.GetHashCode();
                hashCode = (hashCode * 397) ^ _HorizontalBorder.GetHashCode();
                hashCode = (hashCode * 397) ^ _HorizontalFrontPorch.GetHashCode();
                hashCode = (hashCode * 397) ^ _HorizontalSyncWidth.GetHashCode();
                hashCode = (hashCode * 397) ^ _HorizontalTotal.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) _HorizontalSyncPolarity;
                hashCode = (hashCode * 397) ^ _VerticalVisible.GetHashCode();
                hashCode = (hashCode * 397) ^ _VerticalBorder.GetHashCode();
                hashCode = (hashCode * 397) ^ _VerticalFrontPorch.GetHashCode();
                hashCode = (hashCode * 397) ^ _VerticalSyncWidth.GetHashCode();
                hashCode = (hashCode * 397) ^ _VerticalTotal.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) _VerticalSyncPolarity;
                hashCode = (hashCode * 397) ^ (int) _ScanMode;
                hashCode = (hashCode * 397) ^ (int) _PixelClockIn10KHertz;
                hashCode = (hashCode * 397) ^ _Extra.GetHashCode();

                return hashCode;
            }
        }

        /// <summary>
        ///     Checks two instance of <see cref="Timing" /> for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>Returns a boolean value indicating if the two instances are equal; otherwise false</returns>
        public static bool operator ==(Timing left, Timing right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Checks two instance of <see cref="Timing" /> for in equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>Returns a boolean value indicating if the two instances are not equal; otherwise false</returns>
        public static bool operator !=(Timing left, Timing right)
        {
            return !(left == right);
        }

        /// <summary>
        ///     Get the horizontal visible pixels
        /// </summary>
        public int HorizontalVisible
        {
            get => _HorizontalVisible;
        }

        /// <summary>
        ///     Get the horizontal border pixels
        /// </summary>
        public int HorizontalBorder
        {
            get => _HorizontalBorder;
        }

        /// <summary>
        ///     Get the horizontal front porch pixels
        /// </summary>
        public int HorizontalFrontPorch
        {
            get => _HorizontalFrontPorch;
        }

        /// <summary>
        ///     Get the horizontal sync width pixels
        /// </summary>
        public int HorizontalSyncWidth
        {
            get => _HorizontalSyncWidth;
        }

        /// <summary>
        ///     Get the horizontal total pixels
        /// </summary>
        public int HorizontalTotal
        {
            get => _HorizontalTotal;
        }

        /// <summary>
        ///     Get the horizontal sync polarity
        /// </summary>
        public TimingHorizontalSyncPolarity HorizontalSyncPolarity
        {
            get => _HorizontalSyncPolarity;
        }

        /// <summary>
        ///     Get the vertical visible pixels
        /// </summary>
        public int VerticalVisible
        {
            get => _VerticalVisible;
        }

        /// <summary>
        ///     Get the vertical border pixels
        /// </summary>
        public int VerticalBorder
        {
            get => _VerticalBorder;
        }

        /// <summary>
        ///     Get the vertical front porch pixels
        /// </summary>
        public int VerticalFrontPorch
        {
            get => _VerticalFrontPorch;
        }

        /// <summary>
        ///     Get the vertical sync width pixels
        /// </summary>
        public int VerticalSyncWidth
        {
            get => _VerticalSyncWidth;
        }

        /// <summary>
        ///     Get the vertical total pixels
        /// </summary>
        public int VerticalTotal
        {
            get => _VerticalTotal;
        }

        /// <summary>
        ///     Get the vertical sync polarity
        /// </summary>
        public TimingVerticalSyncPolarity VerticalSyncPolarity
        {
            get => _VerticalSyncPolarity;
        }

        /// <summary>
        ///     Get the scan mode
        /// </summary>
        public TimingScanMode ScanMode
        {
            get => _ScanMode;
        }

        /// <summary>
        ///     Get the pixel clock in 10 kHz
        /// </summary>
        public int PixelClockIn10KHertz
        {
            get => (int) _PixelClockIn10KHertz;
        }

        /// <summary>
        ///     Get the other timing related extras
        /// </summary>
        public TimingExtra Extra
        {
            get => _Extra;
        }

        /// <summary>
        ///     Gets the horizontal active pixels
        /// </summary>
        public int HorizontalActive
        {
            get => HorizontalVisible + HorizontalBorder;
        }

        /// <summary>
        ///     Gets the vertical active pixels
        /// </summary>
        public int VerticalActive
        {
            get => VerticalVisible + VerticalBorder;
        }

        /// <summary>
        ///     Gets the horizontal back porch pixels
        /// </summary>
        public int HorizontalBackPorch
        {
            get => HorizontalBlanking - (HorizontalFrontPorch + HorizontalSyncWidth);
        }

        /// <summary>
        ///     Gets the horizontal blanking pixels
        /// </summary>
        public int HorizontalBlanking
        {
            get => HorizontalTotal - (HorizontalActive + HorizontalBorder);
        }

        /// <summary>
        ///     Gets vertical back porch pixels
        /// </summary>
        public int VerticalBackPorch
        {
            get => VerticalBlanking - (VerticalFrontPorch + VerticalSyncWidth);
        }

        /// <summary>
        ///     Gets the vertical blanking pixels
        /// </summary>
        public int VerticalBlanking
        {
            get => VerticalTotal - (VerticalActive + VerticalBorder);
        }
    }
}