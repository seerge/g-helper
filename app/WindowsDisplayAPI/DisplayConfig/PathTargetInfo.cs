using WindowsDisplayAPI.Exceptions;
using WindowsDisplayAPI.Native.DisplayConfig;
using WindowsDisplayAPI.Native.DisplayConfig.Structures;

namespace WindowsDisplayAPI.DisplayConfig
{
    /// <summary>
    ///     Represents a path and its target
    /// </summary>
    public class PathTargetInfo
    {
        private readonly PathTargetDesktopImage _desktopImage;
        private readonly PathTargetSignalInfo _signalInfo;

        /// <summary>
        ///     Creates a new PathTargetInfo
        /// </summary>
        /// <param name="displayTarget">The display target device</param>
        /// <param name="isVirtualModeSupported">A boolean value indicating the target virtual mode support</param>
        public PathTargetInfo(PathDisplayTarget displayTarget, bool isVirtualModeSupported = false)
        {
            DisplayTarget = displayTarget;
            IsVirtualModeSupportedByPath = isVirtualModeSupported;
        }

        /// <summary>
        ///     Creates a new PathTargetInfo
        /// </summary>
        /// <param name="displayTarget">The display target device</param>
        /// <param name="frequencyInMillihertz">Display frequency in millihertz</param>
        /// <param name="scanLineOrdering">Display scan line ordering</param>
        /// <param name="rotation">Display rotation</param>
        /// <param name="scaling">Display scaling</param>
        /// <param name="isVirtualModeSupported">A boolean value indicating the target virtual mode support</param>
        public PathTargetInfo(
            PathDisplayTarget displayTarget,
            ulong frequencyInMillihertz,
            DisplayConfigScanLineOrdering scanLineOrdering = DisplayConfigScanLineOrdering.NotSpecified,
            DisplayConfigRotation rotation = DisplayConfigRotation.NotSpecified,
            DisplayConfigScaling scaling = DisplayConfigScaling.Preferred,
            bool isVirtualModeSupported = false
        ) : this(displayTarget, isVirtualModeSupported)
        {
            FrequencyInMillihertz = frequencyInMillihertz;
            ScanLineOrdering = scanLineOrdering;
            Rotation = rotation;
            Scaling = scaling;
        }

        /// <summary>
        ///     Creates a new PathTargetInfo
        /// </summary>
        /// <param name="displayTarget">The display target device</param>
        /// <param name="signalInfo">The display signal information</param>
        /// <param name="isVirtualModeSupported">A boolean value indicating the target virtual mode support</param>
        public PathTargetInfo(
            PathDisplayTarget displayTarget,
            PathTargetSignalInfo signalInfo,
            bool isVirtualModeSupported = false
        ) : this(displayTarget, isVirtualModeSupported)
        {
            _signalInfo = signalInfo;
            FrequencyInMillihertz = signalInfo.VerticalSyncFrequencyInMillihertz;
            ScanLineOrdering = signalInfo.ScanLineOrdering;
            IsSignalInformationAvailable = true;
        }

        /// <summary>
        ///     Creates a new PathTargetInfo
        /// </summary>
        /// <param name="displayTarget">The display target device</param>
        /// <param name="signalInfo">The display signal information</param>
        /// <param name="rotation">Display rotation</param>
        /// <param name="scaling">Display scaling</param>
        /// <param name="isVirtualModeSupported">A boolean value indicating the target virtual mode support</param>
        public PathTargetInfo(
            PathDisplayTarget displayTarget,
            PathTargetSignalInfo signalInfo,
            DisplayConfigRotation rotation = DisplayConfigRotation.NotSpecified,
            DisplayConfigScaling scaling = DisplayConfigScaling.Preferred,
            bool isVirtualModeSupported = false
        ) : this(
            displayTarget,
            0,
            DisplayConfigScanLineOrdering.NotSpecified,
            rotation,
            scaling,
            isVirtualModeSupported
        )
        {
            _signalInfo = signalInfo;
            FrequencyInMillihertz = signalInfo.VerticalSyncFrequencyInMillihertz;
            ScanLineOrdering = signalInfo.ScanLineOrdering;
            IsSignalInformationAvailable = true;
        }

        /// <summary>
        ///     Creates a new PathTargetInfo
        /// </summary>
        /// <param name="displayTarget">The display target device</param>
        /// <param name="signalInfo">The display signal information</param>
        /// <param name="desktopImage">The display desktop image information</param>
        /// <param name="isVirtualModeSupported">A boolean value indicating the target virtual mode support</param>
        public PathTargetInfo(
            PathDisplayTarget displayTarget,
            PathTargetSignalInfo signalInfo,
            PathTargetDesktopImage desktopImage,
            bool isVirtualModeSupported = false
        ) : this(displayTarget, signalInfo, isVirtualModeSupported)
        {
            _desktopImage = desktopImage;
            IsDesktopImageInformationAvailable = true;
        }

        /// <summary>
        ///     Creates a new PathTargetInfo
        /// </summary>
        /// <param name="displayTarget">The display target device</param>
        /// <param name="signalInfo">The display signal information</param>
        /// <param name="desktopImage">The display desktop image information</param>
        /// <param name="rotation">Display rotation</param>
        /// <param name="scaling">Display scaling</param>
        /// <param name="isVirtualModeSupported">A boolean value indicating the target virtual mode support</param>
        public PathTargetInfo(
            PathDisplayTarget displayTarget,
            PathTargetSignalInfo signalInfo,
            PathTargetDesktopImage desktopImage,
            DisplayConfigRotation rotation = DisplayConfigRotation.NotSpecified,
            DisplayConfigScaling scaling = DisplayConfigScaling.Preferred,
            bool isVirtualModeSupported = false
        ) : this(displayTarget, signalInfo, rotation, scaling, isVirtualModeSupported)
        {
            _desktopImage = desktopImage;
            IsDesktopImageInformationAvailable = true;
        }

        internal PathTargetInfo(
            DisplayConfigPathInfoFlags pathFlags,
            DisplayConfigPathTargetInfo targetInfo,
            DisplayConfigTargetMode? targetMode,
            DisplayConfigDesktopImageInfo? desktopImageMode
            )
        {
            IsPathActive = pathFlags.HasFlag(DisplayConfigPathInfoFlags.Active);
            IsVirtualModeSupportedByPath = pathFlags.HasFlag(DisplayConfigPathInfoFlags.SupportVirtualMode);

            DisplayTarget = new PathDisplayTarget(
                new PathDisplayAdapter(targetInfo.AdapterId),
                targetInfo.TargetId,
                targetInfo.TargetAvailable
            );

            OutputTechnology = targetInfo.OutputTechnology;
            Rotation = targetInfo.Rotation;
            Scaling = targetInfo.Scaling;
            ScanLineOrdering = targetInfo.ScanLineOrdering;
            FrequencyInMillihertz = targetInfo.RefreshRate.ToValue(1000);
            ForcedBootAvailability = targetInfo.StatusFlags.HasFlag(
                DisplayConfigPathTargetInfoFlags.AvailabilityBoot
            );
            ForcedPathAvailability = targetInfo.StatusFlags.HasFlag(
                DisplayConfigPathTargetInfoFlags.AvailabilityPath
            );
            ForcedSystemAvailability = targetInfo.StatusFlags.HasFlag(
                DisplayConfigPathTargetInfoFlags.AvailabilitySystem
            );
            IsCurrentlyInUse = targetInfo.StatusFlags.HasFlag(
                DisplayConfigPathTargetInfoFlags.InUse
            );
            IsForcible = targetInfo.StatusFlags.HasFlag(
                DisplayConfigPathTargetInfoFlags.Forcible
            );

            IsSignalInformationAvailable = targetMode.HasValue;

            if (targetMode.HasValue)
            {
                _signalInfo = new PathTargetSignalInfo(targetMode.Value.TargetVideoSignalInfo);
            }

            IsDesktopImageInformationAvailable = desktopImageMode.HasValue;

            if (desktopImageMode.HasValue)
            {
                _desktopImage = new PathTargetDesktopImage(desktopImageMode.Value);
            }
        }

        /// <summary>
        ///     Gets an instance of PathTargetDesktopImage containing information about this target desktop image
        /// </summary>
        /// <exception cref="MissingModeException">Target mode information is missing</exception>
        public PathTargetDesktopImage DesktopImage
        {
            get
            {
                if (!IsDesktopImageInformationAvailable)
                {
                    throw new MissingModeException(
                        "Desktop image information is missing or not available.",
                        DisplayConfigModeInfoType.DesktopImage
                    );
                }

                return _desktopImage;
            }
        }

        /// <summary>
        ///     Gets extra information about the representing display target device
        /// </summary>
        public PathDisplayTarget DisplayTarget { get; }

        /// <summary>
        ///     Gets a boolean value indicating that the output is currently being forced in a boot-persistent manner
        /// </summary>
        public bool ForcedBootAvailability { get; }

        /// <summary>
        ///     Gets a boolean value indicating that the output is currently being forced in a path-persistent manner
        /// </summary>
        public bool ForcedPathAvailability { get; }

        /// <summary>
        ///     Gets a boolean value indicating that the output is currently being forced in a non-persistent manner
        /// </summary>
        public bool ForcedSystemAvailability { get; }

        /// <summary>
        ///     Gets a value that specifies the refresh rate of the target
        /// </summary>
        public ulong FrequencyInMillihertz { get; }

        /// <summary>
        ///     Gets a boolean value indicating if the target is in use on an active path
        /// </summary>
        public bool IsCurrentlyInUse { get; }

        /// <summary>
        ///     Gets a boolean value indicating the presence of the desktop image information
        /// </summary>
        public bool IsDesktopImageInformationAvailable { get; }

        /// <summary>
        ///     Gets a boolean value indicating that the output can be forced on this target even if a monitor is not detected
        /// </summary>
        public bool IsForcible { get; }

        /// <summary>
        ///     Gets a boolean value indicating if this path is or should be active
        /// </summary>
        public bool IsPathActive { get; } = true;

        /// <summary>
        ///     Gets a boolean value indicating the presence of the signal information
        /// </summary>
        public bool IsSignalInformationAvailable { get; }

        /// <summary>
        ///     Gets a boolean value that indicates if the path supports virtual mode
        /// </summary>
        public bool IsVirtualModeSupportedByPath { get; }

        /// <summary>
        ///     Gets the type of the display device connection
        /// </summary>
        public DisplayConfigVideoOutputTechnology OutputTechnology { get; } = DisplayConfigVideoOutputTechnology.Other;

        /// <summary>
        ///     Gets the rotation of the target
        /// </summary>
        public DisplayConfigRotation Rotation { get; }

        /// <summary>
        ///     Gets the value that specifies how the source image is scaled to the target
        /// </summary>
        public DisplayConfigScaling Scaling { get; }

        /// <summary>
        ///     Gets the value that specifies the scan-line ordering of the output on the target
        /// </summary>
        public DisplayConfigScanLineOrdering ScanLineOrdering { get; }

        /// <summary>
        ///     Gets the target device signal information
        /// </summary>
        /// <exception cref="MissingModeException">Target mode information is missing</exception>
        public PathTargetSignalInfo SignalInfo
        {
            get
            {
                if (!IsSignalInformationAvailable)
                {
                    throw new MissingModeException(
                        "Target mode information is missing or not available.",
                        DisplayConfigModeInfoType.Target
                    );
                }

                return _signalInfo;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{DisplayTarget}: {FrequencyInMillihertz / 1000}hz{(IsCurrentlyInUse ? " [In Use]" : "")}";
        }

        internal DisplayConfigDesktopImageInfo? GetDisplayConfigDesktopImageInfo()
        {
            if (IsDesktopImageInformationAvailable)
            {
                return DesktopImage.GetDisplayConfigDesktopImageInfo();
            }

            return null;
        }

        internal DisplayConfigTargetMode? GetDisplayConfigTargetMode()
        {
            if (IsSignalInformationAvailable)
            {
                return new DisplayConfigTargetMode(SignalInfo.GetDisplayConfigVideoSignalInfo());
            }

            return null;
        }
    }
}