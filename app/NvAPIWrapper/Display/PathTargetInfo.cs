using System;
using System.Collections.Generic;
using NvAPIWrapper.Native.Display;
using NvAPIWrapper.Native.Display.Structures;
using NvAPIWrapper.Native.Exceptions;
using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.Interfaces.Display;

namespace NvAPIWrapper.Display
{
    /// <summary>
    ///     Represents a display configuration on a path
    /// </summary>
    public class PathTargetInfo : IEquatable<PathTargetInfo>
    {
        private TimingOverride _timingOverride;

        /// <summary>
        ///     Creates a new PathTargetInfo
        /// </summary>
        /// <param name="info">IPathTargetInfo implamented object</param>
        public PathTargetInfo(IPathTargetInfo info)
        {
            DisplayDevice = new DisplayDevice(info.DisplayId);

            if (info.Details.HasValue)
            {
                Rotation = info.Details.Value.Rotation;
                Scaling = info.Details.Value.Scaling;
                TVConnectorType = info.Details.Value.ConnectorType;
                TVFormat = info.Details.Value.TVFormat;
                RefreshRateInMillihertz = info.Details.Value.RefreshRateInMillihertz;
                TimingOverride = info.Details.Value.TimingOverride;
                IsInterlaced = info.Details.Value.IsInterlaced;
                IsClonePrimary = info.Details.Value.IsClonePrimary;
                IsClonePanAndScanTarget = info.Details.Value.IsClonePanAndScanTarget;
                DisableVirtualModeSupport = info.Details.Value.DisableVirtualModeSupport;
                IsPreferredUnscaledTarget = info.Details.Value.IsPreferredUnscaledTarget;
            }

            if (info is PathTargetInfoV2)
            {
                WindowsCCDTargetId = ((PathTargetInfoV2) info).WindowsCCDTargetId;
            }
        }

        /// <summary>
        ///     Creates a new PathTargetInfo
        /// </summary>
        /// <param name="device">DisplayDevice object</param>
        public PathTargetInfo(DisplayDevice device)
        {
            DisplayDevice = device;
        }

        /// <summary>
        ///     Gets or sets the virtual mode support
        /// </summary>
        public bool DisableVirtualModeSupport { get; set; }

        /// <summary>
        ///     Gets corresponding DisplayDevice
        /// </summary>
        public DisplayDevice DisplayDevice { get; }

        /// <summary>
        ///     Gets or sets the pan and scan is availability. Valid only when the target is part of clone
        ///     topology.
        /// </summary>
        public bool IsClonePanAndScanTarget { get; set; }

        /// <summary>
        ///     Gets or sets the primary display in clone configuration. This is *NOT* GDI Primary.
        ///     Only one target can be primary per source. If no primary is specified, the first target will automatically be
        ///     primary.
        /// </summary>
        public bool IsClonePrimary { get; set; }

        /// <summary>
        ///     Gets or sets the interlaced mode flag, ignored if refreshRate == 0
        /// </summary>
        public bool IsInterlaced { get; set; }

        /// <summary>
        ///     Gets or sets the preferred unscaled mode of target
        /// </summary>
        public bool IsPreferredUnscaledTarget { get; set; }

        /// <summary>
        ///     Gets and sets the non-interlaced Refresh Rate of the mode, multiplied by 1000, 0 = ignored
        ///     This is the value which driver reports to the OS.
        /// </summary>
        public uint RefreshRateInMillihertz { get; set; }

        /// <summary>
        ///     Gets and sets the rotation setting
        /// </summary>
        public Rotate Rotation { get; set; }

        /// <summary>
        ///     Gets and sets the scaling setting
        /// </summary>
        public Scaling Scaling { get; set; }

        /// <summary>
        ///     Gets and sets the custom timing of display
        ///     Ignored if TimingOverride == TimingOverride.Current
        /// </summary>
        public TimingOverride TimingOverride
        {
            get => _timingOverride;
            set
            {
                if (value == TimingOverride.Custom)
                {
                    throw new NVIDIANotSupportedException("Custom timing is not supported yet.");
                }

                _timingOverride = value;
            }
        }

        /// <summary>
        ///     Gets and sets the connector type. For TV only, ignored if TVFormat == TVFormat.None.
        /// </summary>
        public ConnectorType TVConnectorType { get; set; }

        /// <summary>
        ///     Gets and sets the TV format. For TV only, otherwise set to TVFormat.None
        /// </summary>
        public TVFormat TVFormat { get; set; }

        /// <summary>
        ///     Gets the Windows CCD target ID. Must be present only for non-NVIDIA adapter, for NVIDIA adapter this parameter is
        ///     ignored.
        /// </summary>
        public uint WindowsCCDTargetId { get; }

        /// <summary>
        ///     Checks for equality with a PathTargetInfo instance
        /// </summary>
        /// <param name="other">The PathTargetInfo object to check with</param>
        /// <returns>true if both objects are equal, otherwise false</returns>
        public bool Equals(PathTargetInfo other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return _timingOverride == other._timingOverride &&
                   Rotation == other.Rotation &&
                   Scaling == other.Scaling &&
                   RefreshRateInMillihertz == other.RefreshRateInMillihertz &&
                   (TVFormat == TVFormat.None || TVConnectorType == other.TVConnectorType) &&
                   TVFormat == other.TVFormat &&
                   DisplayDevice.Equals(other.DisplayDevice) &&
                   IsInterlaced == other.IsInterlaced &&
                   IsClonePrimary == other.IsClonePrimary &&
                   IsClonePanAndScanTarget == other.IsClonePanAndScanTarget &&
                   DisableVirtualModeSupport == other.DisableVirtualModeSupport &&
                   IsPreferredUnscaledTarget == other.IsPreferredUnscaledTarget;
        }

        /// <summary>
        ///     Checks for equality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are equal, otherwise false</returns>
        public static bool operator ==(PathTargetInfo left, PathTargetInfo right)
        {
            return right?.Equals(left) ?? ReferenceEquals(left, null);
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(PathTargetInfo left, PathTargetInfo right)
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

            return Equals((PathTargetInfo) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) _timingOverride;
                hashCode = (hashCode * 397) ^ (int) Rotation;
                hashCode = (hashCode * 397) ^ (int) Scaling;
                hashCode = (hashCode * 397) ^ (int) RefreshRateInMillihertz;
                hashCode = (hashCode * 397) ^ (int) TVFormat;
                hashCode = (hashCode * 397) ^ (TVFormat != TVFormat.None ? (int) TVConnectorType : 0);
                hashCode = (hashCode * 397) ^ (DisplayDevice?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ IsInterlaced.GetHashCode();
                hashCode = (hashCode * 397) ^ IsClonePrimary.GetHashCode();
                hashCode = (hashCode * 397) ^ IsClonePanAndScanTarget.GetHashCode();
                hashCode = (hashCode * 397) ^ DisableVirtualModeSupport.GetHashCode();
                hashCode = (hashCode * 397) ^ IsPreferredUnscaledTarget.GetHashCode();

                return hashCode;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var strs = new List<string>
            {
                DisplayDevice.ToString()
            };

            if (RefreshRateInMillihertz > 0)
            {
                strs.Add($"@ {RefreshRateInMillihertz / 1000}hz");
            }

            if (TVFormat != TVFormat.None)
            {
                strs.Add($"- TV {TVFormat}");
            }

            strs.Add(IsInterlaced ? "Interlaced" : "Progressive");

            if (Rotation != Rotate.Degree0)
            {
                strs.Add($"- Rotation: {Rotation}");
            }

            return string.Join(" ", strs);
        }


        /// <summary>
        ///     Creates and fills a PathAdvancedTargetInfo object
        /// </summary>
        /// <returns>The newly created PathAdvancedTargetInfo object</returns>
        public PathAdvancedTargetInfo GetPathAdvancedTargetInfo()
        {
            if (TVFormat == TVFormat.None)
            {
                return new PathAdvancedTargetInfo(Rotation, Scaling, RefreshRateInMillihertz, TimingOverride,
                    IsInterlaced, IsClonePrimary, IsClonePanAndScanTarget, DisableVirtualModeSupport,
                    IsPreferredUnscaledTarget);
            }

            return new PathAdvancedTargetInfo(Rotation, Scaling, TVFormat, TVConnectorType, RefreshRateInMillihertz,
                TimingOverride, IsInterlaced, IsClonePrimary, IsClonePanAndScanTarget, DisableVirtualModeSupport,
                IsPreferredUnscaledTarget);
        }

        /// <summary>
        ///     Creates and fills a PathTargetInfoV1 object
        /// </summary>
        /// <returns>The newly created PathTargetInfoV1 object</returns>
        public PathTargetInfoV1 GetPathTargetInfoV1()
        {
            var pathAdvancedTargetInfo = GetPathAdvancedTargetInfo();

            return new PathTargetInfoV1(DisplayDevice.DisplayId, pathAdvancedTargetInfo);
        }

        /// <summary>
        ///     Creates and fills a PathTargetInfoV2 object
        /// </summary>
        /// <returns>The newly created PathTargetInfoV2 object</returns>
        public PathTargetInfoV2 GetPathTargetInfoV2()
        {
            var pathAdvancedTargetInfo = GetPathAdvancedTargetInfo();

            return new PathTargetInfoV2(DisplayDevice.DisplayId, WindowsCCDTargetId, pathAdvancedTargetInfo);
        }
    }
}