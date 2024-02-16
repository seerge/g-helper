using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using WindowsDisplayAPI.Exceptions;
using WindowsDisplayAPI.Native;
using WindowsDisplayAPI.Native.DisplayConfig;
using WindowsDisplayAPI.Native.DisplayConfig.Structures;
using WindowsDisplayAPI.Native.Structures;

namespace WindowsDisplayAPI.DisplayConfig
{
    /// <summary>
    ///     Represents a path root information
    /// </summary>
    public class PathInfo
    {
        private readonly uint _cloneGroupId;
        private readonly DisplayConfigPixelFormat _pixelFormat;
        private readonly Point _position;
        private readonly Size _resolution;

        /// <summary>
        ///     Creates a new PathInfo
        /// </summary>
        /// <param name="displaySource">The display source</param>
        /// <param name="position">The display position in desktop</param>
        /// <param name="resolution">The display resolution</param>
        /// <param name="pixelFormat">The display pixel format</param>
        public PathInfo(
            PathDisplaySource displaySource,
            Point position,
            Size resolution,
            DisplayConfigPixelFormat pixelFormat
        ) : this(displaySource)
        {
            _position = position;
            _resolution = resolution;
            _pixelFormat = pixelFormat;
            IsModeInformationAvailable = true;
        }

        /// <summary>
        ///     Creates a new PathInfo
        /// </summary>
        /// <param name="displaySource">The display source</param>
        /// <param name="position">The display position in desktop</param>
        /// <param name="resolution">The display resolution</param>
        /// <param name="pixelFormat">The display pixel format</param>
        /// <param name="cloneGroup">The display clone group, only valid for virtual aware paths</param>
        public PathInfo(
            PathDisplaySource displaySource,
            Point position,
            Size resolution,
            DisplayConfigPixelFormat pixelFormat,
            uint cloneGroup
        ) : this(displaySource, cloneGroup)
        {
            _position = position;
            _resolution = resolution;
            _pixelFormat = pixelFormat;
            IsModeInformationAvailable = true;
        }


        /// <summary>
        ///     Creates a new PathInfo
        /// </summary>
        /// <param name="displaySource">The display source</param>
        /// <param name="position">The display position in desktop</param>
        /// <param name="resolution">The display resolution</param>
        /// <param name="pixelFormat">The display pixel format</param>
        /// <param name="pathTargetInfos">An array of target information</param>
        public PathInfo(
            PathDisplaySource displaySource,
            Point position,
            Size resolution,
            DisplayConfigPixelFormat pixelFormat,
            PathTargetInfo[] pathTargetInfos
        ) : this(displaySource, position, resolution, pixelFormat)
        {
            TargetsInfo = pathTargetInfos;
        }


        /// <summary>
        ///     Creates a new PathInfo
        /// </summary>
        /// <param name="displaySource">The display source</param>
        /// <param name="position">The display position in desktop</param>
        /// <param name="resolution">The display resolution</param>
        /// <param name="pixelFormat">The display pixel format</param>
        /// <param name="pathTargetInfos">An array of target information</param>
        /// <param name="cloneGroup">The display clone group, only valid for virtual aware paths</param>
        public PathInfo(
            PathDisplaySource displaySource,
            Point position,
            Size resolution,
            DisplayConfigPixelFormat pixelFormat,
            PathTargetInfo[] pathTargetInfos,
            uint cloneGroup
        ) : this(displaySource, position, resolution, pixelFormat, cloneGroup)
        {
            TargetsInfo = pathTargetInfos;
        }

        /// <summary>
        ///     Creates a new PathInfo
        /// </summary>
        /// <param name="displaySource">The display source</param>
        public PathInfo(PathDisplaySource displaySource)
        {
            DisplaySource = displaySource;
        }

        /// <summary>
        ///     Creates a new PathInfo
        /// </summary>
        /// <param name="displaySource">The display source</param>
        /// <param name="cloneGroup">The display clone group, only valid for virtual aware paths</param>
        public PathInfo(PathDisplaySource displaySource, uint cloneGroup) : this(displaySource)
        {
            IsCloneMember = true;
            _cloneGroupId = cloneGroup;
        }

        /// <summary>
        ///     Creates a new PathInfo
        /// </summary>
        /// <param name="displaySource">The display source</param>
        /// <param name="pathTargetInfos">An array of target information</param>
        public PathInfo(
            PathDisplaySource displaySource,
            PathTargetInfo[] pathTargetInfos
        ) : this(displaySource)
        {
            TargetsInfo = pathTargetInfos;
        }

        /// <summary>
        ///     Creates a new PathInfo
        /// </summary>
        /// <param name="displaySource">The display source</param>
        /// <param name="pathTargetInfos">An array of target information</param>
        /// <param name="cloneGroup">The display clone group, only valid for virtual aware paths</param>
        public PathInfo(
            PathDisplaySource displaySource,
            PathTargetInfo[] pathTargetInfos,
            uint cloneGroup
        ) : this(displaySource, cloneGroup)
        {
            TargetsInfo = pathTargetInfos;
        }

        private PathInfo(
            DisplayConfigPathSourceInfo sourceInfo,
            DisplayConfigSourceMode? sourceMode,
            IEnumerable<
                Tuple<
                    DisplayConfigPathInfoFlags,
                    DisplayConfigPathTargetInfo,
                    DisplayConfigTargetMode?,
                    DisplayConfigDesktopImageInfo?
                >
            > targets
        )
        {
            DisplaySource = new PathDisplaySource(new PathDisplayAdapter(sourceInfo.AdapterId), sourceInfo.SourceId);

            IsInUse = sourceInfo.StatusFlags.HasFlag(DisplayConfigPathSourceInfoFlags.InUse);
            IsModeInformationAvailable = sourceMode.HasValue;

            if (sourceMode.HasValue)
            {
                _resolution = new Size((int) sourceMode.Value.Width, (int) sourceMode.Value.Height);
                _pixelFormat = sourceMode.Value.PixelFormat;
                _position = new Point(sourceMode.Value.Position.X, sourceMode.Value.Position.Y);
            }

            TargetsInfo = targets.Select(t => new PathTargetInfo(t.Item1, t.Item2, t.Item3, t.Item4)).ToArray();

            if (TargetsInfo.Any(info => info.IsVirtualModeSupportedByPath) &&
                sourceInfo.CloneGroupId != DisplayConfigPathSourceInfo.InvalidCloneGroupId
            )
            {
                _cloneGroupId = sourceInfo.CloneGroupId;
                IsCloneMember = true;
            }
        }

        /// <summary>
        ///     Gets a valid identifier used to show which clone group the path is a member of
        /// </summary>
        /// <exception cref="NotACloneMemberException">This path is not a clone member</exception>
        public uint CloneGroupId
        {
            get
            {
                if (!IsCloneMember)
                {
                    throw new NotACloneMemberException(
                        "The display source is not part of a clone group."
                    );
                }

                return _cloneGroupId;
            }
        }

        /// <summary>
        ///     Gets extra information about the representing display source
        /// </summary>
        public PathDisplaySource DisplaySource { get; }

        /// <summary>
        ///     Gets a boolean value indicating if this path is a member of a clone group
        /// </summary>
        public bool IsCloneMember { get; }

        /// <summary>
        ///     Gets a boolean value indicating if this path is the primary GDI path
        /// </summary>
        /// <exception cref="MissingModeException">Source mode information is missing</exception>
        public bool IsGDIPrimary
        {
            get => Position.IsEmpty;
        }

        /// <summary>
        ///     Gets a boolean value indicating if the source is in use by at least one active path
        /// </summary>
        public bool IsInUse { get; }

        /// <summary>
        ///     Gets a boolean value indicating if the source mode information is available
        /// </summary>
        public bool IsModeInformationAvailable { get; }

        /// <summary>
        ///     Gets a boolean value indicating the DisplayConfig (CCD API) availability on this system
        /// </summary>
        public static bool IsSupported
        {
            get
            {
                try
                {
                    return DisplayConfigApi.GetDisplayConfigBufferSizes(
                            QueryDeviceConfigFlags.AllPaths,
                            out _,
                            out _
                        ) == Win32Status.Success;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        ///     Gets a boolean value indicating the virtual display mode support on this system
        /// </summary>
        public static bool IsVirtualModeSupported
        {
            get
            {
                try
                {
                    return PathDisplayTarget
                        .GetDisplayTargets()
                        .Any(t => t.VirtualResolutionSupport);
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        ///     Gets the specifies the pixel format of the source mode
        /// </summary>
        /// <exception cref="MissingModeException">Source mode information is missing</exception>
        public DisplayConfigPixelFormat PixelFormat
        {
            get
            {
                if (!IsModeInformationAvailable)
                {
                    throw new MissingModeException(
                        "Source mode information is missing or not available.",
                        DisplayConfigModeInfoType.Source
                    );
                }

                return _pixelFormat;
            }
        }

        /// <summary>
        ///     Gets the position in the desktop coordinate space of the upper-left corner of this source surface. The source
        ///     surface that is located at (0, 0) is always the primary source surface.
        /// </summary>
        /// <exception cref="MissingModeException">Source mode information is missing</exception>
        public Point Position
        {
            get
            {
                if (!IsModeInformationAvailable)
                {
                    throw new MissingModeException(
                        "Source mode information is missing or not available.",
                        DisplayConfigModeInfoType.Source
                    );
                }

                return _position;
            }
        }

        /// <summary>
        ///     Gets the size of the source mode
        /// </summary>
        /// <exception cref="MissingModeException">Source mode information is missing</exception>
        public Size Resolution
        {
            get
            {
                if (!IsModeInformationAvailable)
                {
                    throw new MissingModeException(
                        "Source mode information is missing or not available.",
                        DisplayConfigModeInfoType.Source
                    );
                }

                return _resolution;
            }
        }

        /// <summary>
        ///     Gets the list of target information
        /// </summary>
        public PathTargetInfo[] TargetsInfo { get; } = new PathTargetInfo[0];

        /// <summary>
        ///     Applies an array of paths
        /// </summary>
        /// <param name="pathInfos">The array of paths</param>
        /// <param name="allowChanges">true to allow changes and reordering of the provided paths, otherwise false</param>
        /// <param name="saveToDatabase">true to save the paths to the persistence database if call succeed, otherwise false</param>
        /// <param name="forceModeEnumeration">true to force driver mode enumeration before applying the paths</param>
        /// <exception cref="PathChangeException">Error in changing paths</exception>
        public static void ApplyPathInfos(
            IEnumerable<PathInfo> pathInfos,
            bool allowChanges = true,
            bool saveToDatabase = false,
            bool forceModeEnumeration = false
        )
        {
            var pathInfosArray = pathInfos.ToArray();

            if (!ValidatePathInfos(pathInfosArray, allowChanges))
            {
                throw new PathChangeException("Invalid paths information.");
            }

            var displayConfigPathInfos = GetDisplayConfigPathInfos(pathInfosArray, out var displayConfigModeInfos);

            if (displayConfigPathInfos.Length <= 0)
            {
                return;
            }

            var flags = displayConfigModeInfos.Length == 0
                ? SetDisplayConfigFlags.TopologySupplied
                : SetDisplayConfigFlags.UseSuppliedDisplayConfig;

            if (allowChanges)
            {
                flags |= displayConfigModeInfos.Length == 0
                    ? SetDisplayConfigFlags.AllowPathOrderChanges
                    : SetDisplayConfigFlags.AllowChanges;
            }
            else if (displayConfigModeInfos.Length > 0)
            {
                flags |= SetDisplayConfigFlags.NoOptimization;
            }

            if (saveToDatabase && displayConfigModeInfos.Length > 0)
            {
                flags |= SetDisplayConfigFlags.SaveToDatabase;
            }

            if (forceModeEnumeration && displayConfigModeInfos.Length > 0)
            {
                flags |= SetDisplayConfigFlags.ForceModeEnumeration;
            }

            var result =
                DisplayConfigApi.SetDisplayConfig(
                    (uint) displayConfigPathInfos.Length,
                    displayConfigPathInfos,
                    (uint) displayConfigModeInfos.Length,
                    displayConfigModeInfos.Length > 0 ? displayConfigModeInfos : null,
                    SetDisplayConfigFlags.Apply | flags
                );

            if (result != Win32Status.Success)
            {
                throw new PathChangeException(
                    "An error occurred while applying the paths information.",
                    new Win32Exception((int) result)
                );
            }
        }

        /// <summary>
        ///     Applies a saved topology
        /// </summary>
        /// <param name="topology">The topology identification to apply</param>
        /// <param name="allowPersistence">true to allows persistence of the changes, otherwise false</param>
        /// <exception cref="PathChangeException">Error in changing paths</exception>
        public static void ApplyTopology(DisplayConfigTopologyId topology, bool allowPersistence = false)
        {
            if (!ValidateTopology(topology))
            {
                throw new PathChangeException("Invalid topology request.");
            }

            var flags = (SetDisplayConfigFlags) topology;

            if (allowPersistence)
            {
                flags |= SetDisplayConfigFlags.PathPersistIfRequired;
            }

            var result = DisplayConfigApi.SetDisplayConfig(
                0,
                null,
                0,
                null,
                SetDisplayConfigFlags.Apply | flags
            );

            if (result != Win32Status.Success)
            {
                throw new PathChangeException(
                    "An error occurred while applying the requested topology.",
                    new Win32Exception((int) result)
                );
            }
        }

        /// <summary>
        ///     Retrieves the list of active paths
        /// </summary>
        /// <param name="virtualModeAware">true if the caller expects virtual mode settings, otherwise false</param>
        /// <returns>An array of PathInfos</returns>
        public static PathInfo[] GetActivePaths(bool virtualModeAware = false)
        {
            return GetPathInfos(
                virtualModeAware
                    ? QueryDeviceConfigFlags.OnlyActivePaths | QueryDeviceConfigFlags.VirtualModeAware
                    : QueryDeviceConfigFlags.OnlyActivePaths,
                out _
            );
        }

        /// <summary>
        ///     Retrieves the list of all paths, active or inactive
        /// </summary>
        /// <param name="virtualModeAware">true if the caller expects virtual mode settings, otherwise false</param>
        /// <returns>An array of PathInfos</returns>
        public static PathInfo[] GetAllPaths(bool virtualModeAware = false)
        {
            return GetPathInfos(
                virtualModeAware
                    ? QueryDeviceConfigFlags.AllPaths | QueryDeviceConfigFlags.VirtualModeAware
                    : QueryDeviceConfigFlags.AllPaths,
                out _
            );
        }

        /// <summary>
        ///     Retrieves the list of currently active topology paths
        /// </summary>
        /// <returns>An array of PathInfos</returns>
        public static PathInfo[] GetCurrentDatabasePaths()
        {
            return GetPathInfos(QueryDeviceConfigFlags.DatabaseCurrent, out _);
        }

        /// <summary>
        ///     Gets the current active topology identification
        /// </summary>
        /// <returns>The topology identification</returns>
        public static DisplayConfigTopologyId GetCurrentTopology()
        {
            GetPathInfos(QueryDeviceConfigFlags.DatabaseCurrent, out var currentDatabaseType);
            return currentDatabaseType;
        }

        /// <summary>
        ///     Validates an array of paths before applying
        /// </summary>
        /// <param name="pathInfos">The array of paths</param>
        /// <param name="allowChanges">true to allow changes and reordering of the provided paths, otherwise false</param>
        /// <returns>true if the provided paths are valid, otherwise false</returns>
        public static bool ValidatePathInfos(IEnumerable<PathInfo> pathInfos, bool allowChanges = true)
        {
            var displayConfigPathInfos = GetDisplayConfigPathInfos(pathInfos, out var displayConfigModeInfos);

            if (displayConfigPathInfos.Length <= 0)
            {
                return false;
            }

            var flags = displayConfigModeInfos.Length == 0
                ? SetDisplayConfigFlags.TopologySupplied
                : SetDisplayConfigFlags.UseSuppliedDisplayConfig;

            if (allowChanges)
            {
                flags |= displayConfigModeInfos.Length == 0
                    ? SetDisplayConfigFlags.AllowPathOrderChanges
                    : SetDisplayConfigFlags.AllowChanges;
            }

            return
                DisplayConfigApi.SetDisplayConfig(
                    (uint) displayConfigPathInfos.Length,
                    displayConfigPathInfos,
                    (uint) displayConfigModeInfos.Length,
                    displayConfigModeInfos.Length > 0 ? displayConfigModeInfos : null,
                    SetDisplayConfigFlags.Validate | flags
                ) ==
                Win32Status.Success;
        }

        /// <summary>
        ///     Validates a topology before applying
        /// </summary>
        /// <param name="topology">The topology identification</param>
        /// <returns>true if topology is applicable, otherwise false</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static bool ValidateTopology(DisplayConfigTopologyId topology)
        {
            if (topology == DisplayConfigTopologyId.None)
            {
                throw new ArgumentOutOfRangeException(nameof(topology), "Topology should not be empty.");
            }

            var flags = (SetDisplayConfigFlags) topology;

            return DisplayConfigApi.SetDisplayConfig(
                       0,
                       null,
                       0,
                       null,
                       SetDisplayConfigFlags.Validate | flags
                   ) ==
                   Win32Status.Success;
        }

        private static uint AddMode(ref List<DisplayConfigModeInfo> modes, DisplayConfigModeInfo mode)
        {
            var existingMode = modes.FindIndex(info =>
                info.InfoType == mode.InfoType &&
                info.AdapterId == mode.AdapterId &&
                info.Id == mode.Id
            );

            if (existingMode > 0)
            {
                if (modes[existingMode] == mode)
                {
                    return (uint) existingMode;
                }

                throw new DuplicateModeException(
                    "Provided list of path information, contains one or more duplicate but not identical modes."
                );
            }

            modes.Add(mode);

            return (uint) modes.Count - 1;
        }

        // ReSharper disable once CyclomaticComplexity
        private static DisplayConfigPathInfo[] GetDisplayConfigPathInfos(
            IEnumerable<PathInfo> pathInfos,
            out DisplayConfigModeInfo[] modeInfos)
        {
            var displayConfigPathInfos = new List<DisplayConfigPathInfo>();
            var displayConfigModeInfos = new List<DisplayConfigModeInfo>();

            foreach (var pathInfo in pathInfos)
            {
                var sourceMode = pathInfo.GetDisplayConfigSourceMode();
                var sourceModeIndex = sourceMode.HasValue
                    ? AddMode(
                        ref displayConfigModeInfos,
                        new DisplayConfigModeInfo(
                            pathInfo.DisplaySource.Adapter.AdapterId,
                            pathInfo.DisplaySource.SourceId,
                            sourceMode.Value
                        )
                    )
                    : 0u;
                var sourceInfo = pathInfo.IsCloneMember
                    ? new DisplayConfigPathSourceInfo(
                        pathInfo.DisplaySource.Adapter.AdapterId,
                        pathInfo.DisplaySource.SourceId,
                        sourceMode.HasValue ? (ushort) sourceModeIndex : DisplayConfigSourceMode.InvalidSourceModeIndex,
                        (ushort) pathInfo.CloneGroupId
                    )
                    : new DisplayConfigPathSourceInfo(
                        pathInfo.DisplaySource.Adapter.AdapterId,
                        pathInfo.DisplaySource.SourceId,
                        sourceMode.HasValue ? sourceModeIndex : DisplayConfigModeInfo.InvalidModeIndex
                    );

                if (pathInfo.TargetsInfo == null || pathInfo.TargetsInfo.Length == 0)
                {
                    displayConfigPathInfos.Add(
                        new DisplayConfigPathInfo(sourceInfo,
                            DisplayConfigPathInfoFlags.Active
                        )
                    );
                }
                else
                {
                    foreach (var target in pathInfo.TargetsInfo)
                    {
                        var flags = DisplayConfigPathInfoFlags.None;

                        if (target.IsPathActive)
                        {
                            flags |= DisplayConfigPathInfoFlags.Active;
                        }

                        if (target.IsVirtualModeSupportedByPath)
                        {
                            flags |= DisplayConfigPathInfoFlags.SupportVirtualMode;
                        }

                        var targetMode = target.GetDisplayConfigTargetMode();
                        var targetModeIndex = targetMode.HasValue
                            ? AddMode(ref displayConfigModeInfos,
                                new DisplayConfigModeInfo(
                                    target.DisplayTarget.Adapter.AdapterId,
                                    target.DisplayTarget.TargetId, targetMode.Value
                                )
                            )
                            : 0u;
                        DisplayConfigPathTargetInfo targetInfo;

                        if (target.IsVirtualModeSupportedByPath)
                        {
                            sourceInfo = new DisplayConfigPathSourceInfo(
                                pathInfo.DisplaySource.Adapter.AdapterId,
                                pathInfo.DisplaySource.SourceId,
                                sourceMode.HasValue
                                    ? (ushort) sourceModeIndex
                                    : DisplayConfigSourceMode.InvalidSourceModeIndex,
                                pathInfo.IsCloneMember
                                    ? (ushort) pathInfo.CloneGroupId
                                    : DisplayConfigPathSourceInfo.InvalidCloneGroupId
                            );
                            var desktopMode = target.GetDisplayConfigDesktopImageInfo();
                            var desktopModeIndex = desktopMode.HasValue
                                ? AddMode(ref displayConfigModeInfos,
                                    new DisplayConfigModeInfo(
                                        target.DisplayTarget.Adapter.AdapterId,
                                        target.DisplayTarget.TargetId,
                                        desktopMode.Value
                                    )
                                )
                                : 0u;
                            targetInfo = new DisplayConfigPathTargetInfo(
                                target.DisplayTarget.Adapter.AdapterId,
                                target.DisplayTarget.TargetId,
                                targetMode.HasValue
                                    ? (ushort) targetModeIndex
                                    : DisplayConfigTargetMode.InvalidTargetModeIndex,
                                desktopMode.HasValue
                                    ? (ushort) desktopModeIndex
                                    : DisplayConfigDesktopImageInfo.InvalidDesktopImageModeIndex,
                                target.OutputTechnology,
                                target.Rotation,
                                target.Scaling,
                                target.ScanLineOrdering == DisplayConfigScanLineOrdering.NotSpecified
                                    ? new DisplayConfigRational()
                                    : new DisplayConfigRational(target.FrequencyInMillihertz, 1000, true),
                                target.ScanLineOrdering,
                                true
                            );
                        }
                        else
                        {
                            targetInfo = new DisplayConfigPathTargetInfo(
                                target.DisplayTarget.Adapter.AdapterId,
                                target.DisplayTarget.TargetId,
                                targetMode.HasValue ? targetModeIndex : DisplayConfigModeInfo.InvalidModeIndex,
                                target.OutputTechnology,
                                target.Rotation,
                                target.Scaling,
                                target.ScanLineOrdering == DisplayConfigScanLineOrdering.NotSpecified
                                    ? new DisplayConfigRational()
                                    : new DisplayConfigRational(target.FrequencyInMillihertz, 1000, true),
                                target.ScanLineOrdering,
                                true
                            );
                        }

                        displayConfigPathInfos.Add(new DisplayConfigPathInfo(sourceInfo, targetInfo, flags));
                    }
                }
            }

            modeInfos = displayConfigModeInfos.ToArray();

            return displayConfigPathInfos.ToArray();
        }

        private static PathInfo[] GetPathInfos(QueryDeviceConfigFlags flags, out DisplayConfigTopologyId topologyId)
        {
            DisplayConfigPathInfo[] displayPaths;
            DisplayConfigModeInfo[] displayModes;
            uint pathCount;

            while (true)
            {
                var error = DisplayConfigApi.GetDisplayConfigBufferSizes(flags,
                    out pathCount,
                    out var modeCount);

                if (error != Win32Status.Success)
                {
                    throw new Win32Exception((int) error);
                }

                displayPaths = new DisplayConfigPathInfo[pathCount];
                displayModes = new DisplayConfigModeInfo[modeCount];

                if (flags == QueryDeviceConfigFlags.DatabaseCurrent)
                {
                    error = DisplayConfigApi.QueryDisplayConfig(
                        flags,
                        ref pathCount,
                        displayPaths,
                        ref modeCount,
                        displayModes,
                        out topologyId
                    );
                }
                else
                {
                    topologyId = DisplayConfigTopologyId.None;
                    error = DisplayConfigApi.QueryDisplayConfig(
                        flags,
                        ref pathCount,
                        displayPaths,
                        ref modeCount,
                        displayModes,
                        IntPtr.Zero
                    );
                }

                if (error == Win32Status.Success)
                {
                    break;
                }

                if (error != Win32Status.ErrorInsufficientBuffer)
                {
                    throw new Win32Exception((int) error);
                }
            }

            var pathInfos =
                new Dictionary<
                    uint,
                    Tuple<
                        DisplayConfigPathSourceInfo,
                        DisplayConfigSourceMode?,
                        List<
                            Tuple<
                                DisplayConfigPathInfoFlags,
                                DisplayConfigPathTargetInfo,
                                DisplayConfigTargetMode?,
                                DisplayConfigDesktopImageInfo?
                            >
                        >
                    >
                >();

            var sourceId = uint.MaxValue;

            for (var i = 0u; i < pathCount; i++)
            {
                var displayPath = displayPaths[i];
                DisplayConfigSourceMode? sourceMode = null;
                var key = sourceId;
                var isVirtualSupported = displayPath.Flags.HasFlag(DisplayConfigPathInfoFlags.SupportVirtualMode);

                if (isVirtualSupported &&
                    displayPath.SourceInfo.SourceModeInfoIndex != DisplayConfigSourceMode.InvalidSourceModeIndex &&
                    displayModes[displayPath.SourceInfo.SourceModeInfoIndex].InfoType ==
                    DisplayConfigModeInfoType.Source)
                {
                    sourceMode = displayModes[displayPath.SourceInfo.SourceModeInfoIndex].SourceMode;
                    key = displayPath.SourceInfo.SourceModeInfoIndex;
                }
                else if (!isVirtualSupported &&
                         displayPath.SourceInfo.ModeInfoIndex != DisplayConfigModeInfo.InvalidModeIndex &&
                         displayModes[displayPath.SourceInfo.ModeInfoIndex].InfoType ==
                         DisplayConfigModeInfoType.Source)
                {
                    sourceMode = displayModes[displayPath.SourceInfo.ModeInfoIndex].SourceMode;
                    key = displayPath.SourceInfo.ModeInfoIndex;
                }
                else
                {
                    sourceId--;
                }

                if (!pathInfos.ContainsKey(key))
                {
                    pathInfos.Add(
                        key,
                        Tuple.Create(
                            displayPath.SourceInfo,
                            sourceMode,
                            new List<
                                Tuple<
                                    DisplayConfigPathInfoFlags,
                                    DisplayConfigPathTargetInfo,
                                    DisplayConfigTargetMode?,
                                    DisplayConfigDesktopImageInfo?
                                >
                            >()
                        )
                    );
                }

                DisplayConfigTargetMode? targetMode = null;

                if (isVirtualSupported &&
                    displayPath.TargetInfo.TargetModeInfoIndex != DisplayConfigTargetMode.InvalidTargetModeIndex &&
                    displayModes[displayPath.TargetInfo.TargetModeInfoIndex].InfoType == DisplayConfigModeInfoType.Target
                )
                {
                    targetMode = displayModes[displayPath.TargetInfo.TargetModeInfoIndex].TargetMode;
                }
                else if (!isVirtualSupported &&
                         displayPath.TargetInfo.ModeInfoIndex != DisplayConfigModeInfo.InvalidModeIndex &&
                         displayModes[displayPath.TargetInfo.ModeInfoIndex].InfoType == DisplayConfigModeInfoType.Target
                )
                {
                    targetMode = displayModes[displayPath.TargetInfo.ModeInfoIndex].TargetMode;
                }

                DisplayConfigDesktopImageInfo? desktopImageMode = null;

                if (isVirtualSupported &&
                    displayPath.TargetInfo.DesktopModeInfoIndex !=
                    DisplayConfigDesktopImageInfo.InvalidDesktopImageModeIndex &&
                    displayModes[displayPath.TargetInfo.DesktopModeInfoIndex].InfoType ==
                    DisplayConfigModeInfoType.DesktopImage)
                {
                    desktopImageMode = displayModes[displayPath.TargetInfo.DesktopModeInfoIndex].DesktopImageInfo;
                }

                pathInfos[key].Item3.Add(
                    Tuple.Create(displayPath.Flags, displayPath.TargetInfo, targetMode, desktopImageMode)
                );
            }

            return pathInfos.Select(
                pair => new PathInfo(pair.Value.Item1, pair.Value.Item2, pair.Value.Item3)
            ).ToArray();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{DisplaySource}: {Resolution} @ {Position}";
        }

        private DisplayConfigSourceMode? GetDisplayConfigSourceMode()
        {
            if (IsModeInformationAvailable)
            {
                return new DisplayConfigSourceMode(
                    (uint) Resolution.Width,
                    (uint) Resolution.Height,
                    PixelFormat,
                    new PointL(Position)
                );
            }

            return null;
        }
    }
}