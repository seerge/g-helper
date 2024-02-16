using System;
using System.Drawing;
using WindowsDisplayAPI.Native.DisplayConfig;
using WindowsDisplayAPI.Native.DisplayConfig.Structures;

namespace WindowsDisplayAPI.DisplayConfig
{
    /// <summary>
    ///     Contains information about the target signal info
    /// </summary>
    public class PathTargetSignalInfo : IEquatable<PathTargetSignalInfo>
    {
        /// <summary>
        ///     Creates a new PathTargetSignalInfo
        /// </summary>
        /// <param name="activeSize">Specifies the width and height (in pixels) of the active portion of the video signal.</param>
        /// <param name="totalSize">Specifies the width and height (in pixels) of the entire video signal.</param>
        /// <param name="verticalSyncFrequencyInMillihertz">Vertical synchronization frequency.</param>
        /// <param name="scanLineOrdering">The scan-line ordering (for example, progressive or interlaced) of the video signal.</param>
        /// <param name="videoStandard">
        ///     The video standard (if any) that defines the video signal. Supported by WDDM 1.3 and later
        ///     display mini-port drivers running on Windows 8.1 and later.
        /// </param>
        /// <param name="verticalSyncFrequencyDivider">
        ///     The ratio of the VSync rate of a monitor that displays through a Miracast
        ///     connected session to the VSync rate of the Miracast sink. The ratio of the VSync rate of a monitor that displays
        ///     through a Miracast connected session to the VSync rate of the Miracast sink. Supported by WDDM 1.3 and later
        ///     display mini-port drivers running on Windows 8.1 and later.
        /// </param>
        public PathTargetSignalInfo(
            Size activeSize,
            Size totalSize,
            ulong verticalSyncFrequencyInMillihertz,
            DisplayConfigScanLineOrdering scanLineOrdering,
            VideoSignalStandard videoStandard = VideoSignalStandard.Uninitialized,
            ushort verticalSyncFrequencyDivider = 0
        )
        {
            ActiveSize = activeSize;
            ScanLineOrdering = scanLineOrdering;
            TotalSize = totalSize;
            VerticalSyncFrequencyDivider = verticalSyncFrequencyDivider;
            VerticalSyncFrequencyInMillihertz = verticalSyncFrequencyInMillihertz;
            VideoStandard = videoStandard;
            PixelRate = (ulong) (verticalSyncFrequencyInMillihertz / 1000d * totalSize.Width * totalSize.Height);
            HorizontalSyncFrequencyInMillihertz = (ulong) totalSize.Height * verticalSyncFrequencyInMillihertz;
        }

        /// <summary>
        ///     Creates a new PathTargetSignalInfo
        /// </summary>
        /// <param name="displaySetting">A possible display settings</param>
        /// <param name="totalSignalSize">Total signal size</param>
        public PathTargetSignalInfo(DisplayPossibleSetting displaySetting, Size totalSignalSize) :
            this(
                displaySetting.Resolution, totalSignalSize,
                (uint) (displaySetting.Frequency * 1000),
                displaySetting.IsInterlaced
                    ? DisplayConfigScanLineOrdering.InterlacedWithUpperFieldFirst
                    : DisplayConfigScanLineOrdering.Progressive
            )
        {
        }

        internal PathTargetSignalInfo(DisplayConfigVideoSignalInfo signalInfo)
        {
            PixelRate = signalInfo.PixelRate;
            HorizontalSyncFrequencyInMillihertz = signalInfo.HorizontalSyncFrequency.ToValue(1000);
            VerticalSyncFrequencyInMillihertz = signalInfo.VerticalSyncFrequency.ToValue(1000);
            ActiveSize = new Size((int) signalInfo.ActiveSize.Width, (int) signalInfo.ActiveSize.Height);
            TotalSize = new Size((int) signalInfo.TotalSize.Width, (int) signalInfo.TotalSize.Height);
            VideoStandard = signalInfo.VideoStandard;
            VerticalSyncFrequencyDivider = signalInfo.VerticalSyncFrequencyDivider;
            ScanLineOrdering = signalInfo.ScanLineOrdering;
        }

        /// <summary>
        ///     Gets the width and height (in pixels) of the active portion of the video signal
        /// </summary>
        public Size ActiveSize { get; }

        /// <summary>
        ///     Gets the horizontal synchronization frequency
        /// </summary>
        public ulong HorizontalSyncFrequencyInMillihertz { get; }

        /// <summary>
        ///     Gets the pixel clock rate
        /// </summary>
        public ulong PixelRate { get; }

        /// <summary>
        ///     Gets the scan-line ordering (for example, progressive or interlaced) of the video signal
        /// </summary>
        public DisplayConfigScanLineOrdering ScanLineOrdering { get; }

        /// <summary>
        ///     Gets the width and height (in pixels) of the entire video signal
        /// </summary>
        public Size TotalSize { get; }

        /// <summary>
        ///     Gets the ratio of the VSync rate of a monitor that displays through a Miracast connected session to the VSync rate
        ///     of the Miracast sink. The ratio of the VSync rate of a monitor that displays through a Miracast connected session
        ///     to the VSync rate of the Miracast sink. Supported by WDDM 1.3 and later display mini-port drivers running on Windows
        ///     8.1 and later
        /// </summary>
        public ushort VerticalSyncFrequencyDivider { get; }

        /// <summary>
        ///     Gets the vertical synchronization frequency
        /// </summary>
        public ulong VerticalSyncFrequencyInMillihertz { get; }

        /// <summary>
        ///     Gets the video standard (if any) that defines the video signal. Supported by WDDM 1.3 and later display mini-port
        ///     drivers running on Windows 8.1 and later
        /// </summary>
        public VideoSignalStandard VideoStandard { get; }

        /// <inheritdoc />
        public bool Equals(PathTargetSignalInfo other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return ActiveSize == other.ActiveSize &&
                   HorizontalSyncFrequencyInMillihertz == other.HorizontalSyncFrequencyInMillihertz &&
                   PixelRate == other.PixelRate &&
                   ScanLineOrdering == other.ScanLineOrdering &&
                   TotalSize == other.TotalSize &&
                   VerticalSyncFrequencyDivider == other.VerticalSyncFrequencyDivider &&
                   VerticalSyncFrequencyInMillihertz == other.VerticalSyncFrequencyInMillihertz &&
                   VideoStandard == other.VideoStandard;
        }

        /// <summary>
        ///     Checks for equality of two PathTargetSignalInfo instances
        /// </summary>
        /// <param name="left">The first instance</param>
        /// <param name="right">The second instance</param>
        /// <returns>true if both instances are equal, otherwise false</returns>
        public static bool operator ==(PathTargetSignalInfo left, PathTargetSignalInfo right)
        {
            return Equals(left, right) || left?.Equals(right) == true;
        }

        /// <summary>
        ///     Checks for inequality of two PathTargetSignalInfo instances
        /// </summary>
        /// <param name="left">The first instance</param>
        /// <param name="right">The second instance</param>
        /// <returns>true if both instances are not equal, otherwise false</returns>
        public static bool operator !=(PathTargetSignalInfo left, PathTargetSignalInfo right)
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

            return obj.GetType() == GetType() && Equals((PathTargetSignalInfo) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ActiveSize.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) HorizontalSyncFrequencyInMillihertz;
                hashCode = (hashCode * 397) ^ PixelRate.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) ScanLineOrdering;
                hashCode = (hashCode * 397) ^ TotalSize.GetHashCode();
                hashCode = (hashCode * 397) ^ VerticalSyncFrequencyDivider.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) VerticalSyncFrequencyInMillihertz;
                hashCode = (hashCode * 397) ^ (int) VideoStandard;

                return hashCode;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{ActiveSize} @ {VerticalSyncFrequencyInMillihertz / 1000}hz {VideoStandard}";
        }

        internal DisplayConfigVideoSignalInfo GetDisplayConfigVideoSignalInfo()
        {
            return new DisplayConfigVideoSignalInfo(
                PixelRate,
                new DisplayConfigRational(HorizontalSyncFrequencyInMillihertz, 1000, true),
                new DisplayConfigRational(VerticalSyncFrequencyInMillihertz, 1000, true),
                new DisplayConfig2DRegion((uint) ActiveSize.Width, (uint) ActiveSize.Height),
                new DisplayConfig2DRegion((uint) TotalSize.Width, (uint) TotalSize.Height),
                VideoStandard,
                VerticalSyncFrequencyDivider,
                ScanLineOrdering
            );
        }
    }
}