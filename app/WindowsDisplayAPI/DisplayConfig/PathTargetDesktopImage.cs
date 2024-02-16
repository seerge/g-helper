using System;
using System.Drawing;
using WindowsDisplayAPI.Native.DisplayConfig.Structures;
using WindowsDisplayAPI.Native.Structures;

namespace WindowsDisplayAPI.DisplayConfig
{
    /// <summary>
    ///     Contains information about the target desktop image
    /// </summary>
    public class PathTargetDesktopImage : IEquatable<PathTargetDesktopImage>
    {
        /// <summary>
        ///     Creates a new PathTargetDesktopImage
        /// </summary>
        /// <param name="monitorSurfaceSize">Size of the VidPn source surface that is being displayed on the monitor</param>
        /// <param name="imageRegion">
        ///     Where the desktop image will be positioned within monitor surface size. Region must be
        ///     completely inside the bounds of the monitor surface size.
        /// </param>
        /// <param name="imageClip">
        ///     Which part of the desktop image for this clone group will be displayed on this path. This
        ///     currently must be set to the desktop size.
        /// </param>
        public PathTargetDesktopImage(Size monitorSurfaceSize, Rectangle imageRegion, Rectangle imageClip)
        {
            ImageClip = imageClip;
            ImageRegion = imageRegion;
            MonitorSurfaceSize = monitorSurfaceSize;
        }

        internal PathTargetDesktopImage(DisplayConfigDesktopImageInfo desktopImage)
        {
            MonitorSurfaceSize = desktopImage.PathSourceSize.ToSize();
            ImageRegion = desktopImage.DesktopImageRegion.ToRectangle();
            ImageClip = desktopImage.DesktopImageClip.ToRectangle();
        }

        /// <summary>
        ///     Gets part of the desktop image for this clone group that will be displayed on this path. This currently must be set
        ///     to the desktop size.
        /// </summary>
        public Rectangle ImageClip { get; }

        /// <summary>
        ///     Gets the part that the desktop image will be positioned within monitor surface size. Region must be completely
        ///     inside the bounds of the monitor surface size.
        /// </summary>
        public Rectangle ImageRegion { get; }

        /// <summary>
        ///     Gets the size of the VidPn source surface that is being displayed on the monitor
        /// </summary>
        public Size MonitorSurfaceSize { get; }

        /// <inheritdoc />
        public bool Equals(PathTargetDesktopImage other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return ImageClip == other.ImageClip &&
                   ImageRegion == other.ImageRegion &&
                   MonitorSurfaceSize == other.MonitorSurfaceSize;
        }

        /// <summary>
        ///     Checks for equality of two PathTargetDesktopImage instances
        /// </summary>
        /// <param name="left">The first instance</param>
        /// <param name="right">The second instance</param>
        /// <returns>true if both instances are equal, otherwise false</returns>
        public static bool operator ==(PathTargetDesktopImage left, PathTargetDesktopImage right)
        {
            return Equals(left, right) || left?.Equals(right) == true;
        }

        /// <summary>
        ///     Checks for inequality of two PathTargetDesktopImage instances
        /// </summary>
        /// <param name="left">The first instance</param>
        /// <param name="right">The second instance</param>
        /// <returns>true if both instances are not equal, otherwise false</returns>
        public static bool operator !=(PathTargetDesktopImage left, PathTargetDesktopImage right)
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

            return obj.GetType() == GetType() && Equals((PathTargetDesktopImage) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ImageClip.GetHashCode();
                hashCode = (hashCode * 397) ^ ImageRegion.GetHashCode();
                hashCode = (hashCode * 397) ^ MonitorSurfaceSize.GetHashCode();

                return hashCode;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{ImageClip} => {ImageRegion} @ {MonitorSurfaceSize}";
        }

        internal DisplayConfigDesktopImageInfo GetDisplayConfigDesktopImageInfo()
        {
            return new DisplayConfigDesktopImageInfo(
                new PointL(MonitorSurfaceSize),
                new RectangleL(ImageRegion),
                new RectangleL(ImageClip)
            );
        }
    }
}