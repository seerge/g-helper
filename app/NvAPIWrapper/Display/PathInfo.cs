using System;
using System.Linq;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.Display;
using NvAPIWrapper.Native.Display.Structures;
using NvAPIWrapper.Native.Exceptions;
using NvAPIWrapper.Native.General;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces.Display;

namespace NvAPIWrapper.Display
{
    /// <summary>
    ///     Represents a configuration path
    /// </summary>
    public class PathInfo : IEquatable<PathInfo>
    {
        /// <summary>
        ///     Creates a new PathInfo
        /// </summary>
        /// <param name="resolution">Display resolution</param>
        /// <param name="colorFormat">Display color format</param>
        /// <param name="targetInfos">Target configuration informations</param>
        public PathInfo(Resolution resolution, ColorFormat colorFormat, PathTargetInfo[] targetInfos)
        {
            Resolution = resolution;
            ColorFormat = colorFormat;
            TargetsInfo = targetInfos;
        }

        /// <summary>
        ///     Creates a new PathInfo
        /// </summary>
        /// <param name="info">IPathInfo implamented object</param>
        public PathInfo(IPathInfo info)
        {
            SourceId = info.SourceId;
            Resolution = info.SourceModeInfo.Resolution;
            ColorFormat = info.SourceModeInfo.ColorFormat;
            Position = info.SourceModeInfo.Position;
            SpanningOrientation = info.SourceModeInfo.SpanningOrientation;
            IsGDIPrimary = info.SourceModeInfo.IsGDIPrimary;
            IsSLIFocus = info.SourceModeInfo.IsSLIFocus;
            TargetsInfo =
                info.TargetsInfo.Select(targetInfo => new PathTargetInfo(targetInfo)).ToArray();

            if (info is PathInfoV2)
            {
                OSAdapterLUID = ((PathInfoV2) info).OSAdapterLUID;
            }
        }

        /// <summary>
        ///     Gets or sets the display color format
        /// </summary>
        public ColorFormat ColorFormat { get; set; }

        /// <summary>
        ///     Gets or sets a boolean value indicating if the this is the primary GDI display
        /// </summary>
        public bool IsGDIPrimary { get; set; }

        /// <summary>
        ///     Gets or sets a boolean value indicating if the this is the SLI focus display
        /// </summary>
        public bool IsSLIFocus { get; set; }

        /// <summary>
        ///     Gets OS Adapter of LUID for Non-NVIDIA adapters
        /// </summary>
        public LUID? OSAdapterLUID { get; }

        /// <summary>
        ///     Gets or sets the display position
        /// </summary>
        public Position Position { get; set; }

        /// <summary>
        ///     Gets or sets the display resolution
        /// </summary>
        public Resolution Resolution { get; set; }

        /// <summary>
        ///     Gets or sets the Windows CCD display source identification. This can be optionally set.
        /// </summary>
        public uint SourceId { get; set; }

        /// <summary>
        ///     Gets or sets the display spanning orientation, valid for XP only
        /// </summary>
        public SpanningOrientation SpanningOrientation { get; set; }


        /// <summary>
        ///     Gets information about path targets
        /// </summary>
        public PathTargetInfo[] TargetsInfo { get; }


        /// <summary>
        ///     Checks for equality with a PathInfo instance
        /// </summary>
        /// <param name="other">The PathInfo object to check with</param>
        /// <returns>true if both objects are equal, otherwise false</returns>
        public bool Equals(PathInfo other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Resolution.Equals(other.Resolution) &&
                   ColorFormat == other.ColorFormat &&
                   Position.Equals(other.Position) &&
                   SpanningOrientation == other.SpanningOrientation &&
                   IsGDIPrimary == other.IsGDIPrimary &&
                   IsSLIFocus == other.IsSLIFocus &&
                   TargetsInfo.SequenceEqual(other.TargetsInfo);
        }


        /// <summary>
        ///     Creates and fills a PathInfo object
        /// </summary>
        /// <returns>The newly created PathInfo object</returns>
        public static PathInfo[] GetDisplaysConfig()
        {
            var configs = DisplayApi.GetDisplayConfig();
            var logicalDisplays = configs.Select(info => new PathInfo(info)).ToArray();
            configs.DisposeAll();

            return logicalDisplays;
        }

        /// <summary>
        ///     Checks for equality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are equal, otherwise false</returns>
        public static bool operator ==(PathInfo left, PathInfo right)
        {
            return right?.Equals(left) ?? ReferenceEquals(left, null);
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(PathInfo left, PathInfo right)
        {
            return !(left == right);
        }

        /// <summary>
        ///     Applies one or more path information configurations
        /// </summary>
        /// <param name="pathInfos">An array of path information configuration</param>
        /// <param name="flags">DisplayConfigFlags flags</param>
        public static void SetDisplaysConfig(PathInfo[] pathInfos, DisplayConfigFlags flags)
        {
            try
            {
                var configsV2 = pathInfos.Select(config => config.GetPathInfoV2()).Cast<IPathInfo>().ToArray();
                DisplayApi.SetDisplayConfig(configsV2, flags);
                configsV2.DisposeAll();
            }
            catch (NVIDIAApiException ex)
            {
                if (ex.Status != Status.IncompatibleStructureVersion)
                {
                    throw;
                }
            }
            catch (NVIDIANotSupportedException)
            {
                // ignore
            }

            var configsV1 = pathInfos.Select(config => config.GetPathInfoV1()).Cast<IPathInfo>().ToArray();
            DisplayApi.SetDisplayConfig(configsV1, flags);
            configsV1.DisposeAll();
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

            return Equals((PathInfo) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Resolution.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) ColorFormat;
                hashCode = (hashCode * 397) ^ Position.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) SpanningOrientation;
                hashCode = (hashCode * 397) ^ IsGDIPrimary.GetHashCode();
                hashCode = (hashCode * 397) ^ IsSLIFocus.GetHashCode();
                hashCode = (hashCode * 397) ^ (TargetsInfo?.GetHashCode() ?? 0);

                return hashCode;
            }
        }


        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Resolution} @ {Position} [{TargetsInfo.Length}]";
        }

        /// <summary>
        ///     Creates and fills a GetPathInfoV1 object
        /// </summary>
        /// <returns>The newly created GetPathInfoV1 object</returns>
        public PathInfoV1 GetPathInfoV1()
        {
            var sourceModeInfo = GetSourceModeInfo();
            var pathTargetInfoV1 = GetPathTargetInfoV1Array();

            return new PathInfoV1(pathTargetInfoV1, sourceModeInfo, SourceId);
        }

        /// <summary>
        ///     Creates and fills a GetPathInfoV2 object
        /// </summary>
        /// <returns>The newly created GetPathInfoV2 object</returns>
        public PathInfoV2 GetPathInfoV2()
        {
            var sourceModeInfo = GetSourceModeInfo();
            var pathTargetInfoV2 = GetPathTargetInfoV2Array();

            return new PathInfoV2(pathTargetInfoV2, sourceModeInfo, SourceId);
        }

        /// <summary>
        ///     Creates and fills an array of GetPathTargetInfoV1 object
        /// </summary>
        /// <returns>The newly created array of GetPathTargetInfoV1 objects</returns>
        public PathTargetInfoV1[] GetPathTargetInfoV1Array()
        {
            return TargetsInfo.Select(config => config.GetPathTargetInfoV1()).ToArray();
        }

        /// <summary>
        ///     Creates and fills an array of GetPathTargetInfoV2 object
        /// </summary>
        /// <returns>The newly created array of GetPathTargetInfoV2 objects</returns>
        public PathTargetInfoV2[] GetPathTargetInfoV2Array()
        {
            return TargetsInfo.Select(config => config.GetPathTargetInfoV2()).ToArray();
        }

        /// <summary>
        ///     Creates and fills a SourceModeInfo object
        /// </summary>
        /// <returns>The newly created SourceModeInfo object</returns>
        public SourceModeInfo GetSourceModeInfo()
        {
            return new SourceModeInfo(Resolution, ColorFormat, Position, SpanningOrientation, IsGDIPrimary, IsSLIFocus);
        }
    }
}