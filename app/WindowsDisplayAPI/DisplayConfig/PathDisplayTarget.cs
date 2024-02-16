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
    ///     Represents a display path target (Display Device)
    /// </summary>
    public class PathDisplayTarget : IEquatable<PathDisplayTarget>
    {
        /// <summary>
        ///     Creates a new PathDisplayTarget
        /// </summary>
        /// <param name="adapter">Display adapter</param>
        /// <param name="targetId">Display target identification</param>
        public PathDisplayTarget(PathDisplayAdapter adapter, uint targetId) : this(adapter, targetId, false)
        {
            IsAvailable = GetDisplayTargets().Any(target => target == this);
        }

        internal PathDisplayTarget(PathDisplayAdapter adapter, uint targetId, bool isAvailable)
        {
            Adapter = adapter;
            TargetId = targetId;
            IsAvailable = isAvailable;
        }

        /// <summary>
        ///     Gets the path display adapter
        /// </summary>
        public PathDisplayAdapter Adapter { get; }

        /// <summary>
        ///     Sets the display boot persistence for the target display device
        /// </summary>
        /// <exception cref="TargetNotAvailableException"></exception>
        /// <exception cref="Win32Exception"></exception>
        public bool BootPersistence
        {
            set
            {
                if (!IsAvailable)
                {
                    throw new TargetNotAvailableException("Extra information about the target is not available.",
                        Adapter.AdapterId, TargetId);
                }

                var targetPersistence = new DisplayConfigSetTargetPersistence(Adapter.AdapterId, TargetId, value);
                var result = DisplayConfigApi.DisplayConfigSetDeviceInfo(ref targetPersistence);

                if (result != Win32Status.Success)
                {
                    throw new Win32Exception((int) result);
                }
            }
        }

        /// <summary>
        ///     Gets the one-based instance number of this particular target only when the adapter has multiple targets of this
        ///     type. The connector instance is a consecutive one-based number that is unique within each adapter. If this is the
        ///     only target of this type on the adapter, this value is zero.
        /// </summary>
        /// <exception cref="Win32Exception">Error code can be retrieved from Win32Exception.NativeErrorCode property</exception>
        /// <exception cref="TargetNotAvailableException">The target is not available</exception>
        public int ConnectorInstance
        {
            get
            {
                if (!IsAvailable)
                {
                    throw new TargetNotAvailableException("Extra information about the target is not available.",
                        Adapter.AdapterId, TargetId);
                }

                var targetName = new DisplayConfigTargetDeviceName(Adapter.AdapterId, TargetId);
                var result = DisplayConfigApi.DisplayConfigGetDeviceInfo(ref targetName);

                if (result == Win32Status.Success)
                {
                    return (int) targetName.ConnectorInstance;
                }

                throw new Win32Exception((int) result);
            }
        }

        /// <summary>
        ///     Gets the display device path
        /// </summary>
        /// <exception cref="Win32Exception">Error code can be retrieved from Win32Exception.NativeErrorCode property</exception>
        /// <exception cref="TargetNotAvailableException">The target is not available</exception>
        public string DevicePath
        {
            get
            {
                if (!IsAvailable)
                {
                    throw new TargetNotAvailableException("Extra information about the target is not available.",
                        Adapter.AdapterId, TargetId);
                }

                var targetName = new DisplayConfigTargetDeviceName(Adapter.AdapterId, TargetId);
                var result = DisplayConfigApi.DisplayConfigGetDeviceInfo(ref targetName);

                if (result == Win32Status.Success)
                {
                    return targetName.MonitorDevicePath;
                }

                throw new Win32Exception((int) result);
            }
        }

        /// <summary>
        ///     Gets the display manufacture 3 character code from the display EDID manufacture identification
        /// </summary>
        /// <exception cref="TargetNotAvailableException">The target is not available</exception>
        /// <exception cref="Win32Exception">Error code can be retrieved from Win32Exception.NativeErrorCode property</exception>
        /// <exception cref="InvalidEDIDInformation">The EDID information does not contain this value</exception>
        public string EDIDManufactureCode
        {
            get
            {
                var edidCode = EDIDManufactureId;
                edidCode = ((edidCode & 0xff00) >> 8) | ((edidCode & 0x00ff) << 8);
                var byte1 = (byte) 'A' + (edidCode & 0x1f) - 1;
                var byte2 = (byte) 'A' + ((edidCode >> 5) & 0x1f) - 1;
                var byte3 = (byte) 'A' + ((edidCode >> 10) & 0x1f) - 1;

                return $"{Convert.ToChar(byte3)}{Convert.ToChar(byte2)}{Convert.ToChar(byte1)}";
            }
        }

        /// <summary>
        ///     Gets the display manufacture identification from the display EDID information
        /// </summary>
        /// <exception cref="TargetNotAvailableException">The target is not available</exception>
        /// <exception cref="Win32Exception">Error code can be retrieved from Win32Exception.NativeErrorCode property</exception>
        /// <exception cref="InvalidEDIDInformation">The EDID information does not contain this value</exception>
        public int EDIDManufactureId
        {
            get
            {
                if (!IsAvailable)
                {
                    throw new TargetNotAvailableException("Extra information about the target is not available.",
                        Adapter.AdapterId, TargetId);
                }

                var targetName = new DisplayConfigTargetDeviceName(Adapter.AdapterId, TargetId);
                var result = DisplayConfigApi.DisplayConfigGetDeviceInfo(ref targetName);

                if (result == Win32Status.Success)
                {
                    if (targetName.Flags.HasFlag(DisplayConfigTargetDeviceNameFlags.EDIDIdsValid))
                    {
                        return targetName.EDIDManufactureId;
                    }

                    throw new InvalidEDIDInformation("EDID does not contain necessary information.");
                }

                throw new Win32Exception((int) result);
            }
        }

        /// <summary>
        ///     Gets the display product identification from the display EDID information
        /// </summary>
        /// <exception cref="TargetNotAvailableException">The target is not available</exception>
        /// <exception cref="Win32Exception">Error code can be retrieved from Win32Exception.NativeErrorCode property</exception>
        /// <exception cref="InvalidEDIDInformation">The EDID information does not contain this value</exception>
        public int EDIDProductCode
        {
            get
            {
                if (!IsAvailable)
                {
                    throw new TargetNotAvailableException("Extra information about the target is not available.",
                        Adapter.AdapterId, TargetId);
                }

                var targetName = new DisplayConfigTargetDeviceName(Adapter.AdapterId, TargetId);
                var result = DisplayConfigApi.DisplayConfigGetDeviceInfo(ref targetName);

                if (result == Win32Status.Success)
                {
                    if (targetName.Flags.HasFlag(DisplayConfigTargetDeviceNameFlags.EDIDIdsValid))
                    {
                        return targetName.EDIDProductCodeId;
                    }

                    throw new InvalidEDIDInformation("EDID does not contain necessary information.");
                }

                throw new Win32Exception((int) result);
            }
        }

        /// <summary>
        ///     Gets the display friendly name from the display EDID information
        /// </summary>
        /// <exception cref="TargetNotAvailableException">The target is not available</exception>
        /// <exception cref="Win32Exception">Error code can be retrieved from Win32Exception.NativeErrorCode property</exception>
        public string FriendlyName
        {
            get
            {
                if (!IsAvailable)
                {
                    throw new TargetNotAvailableException("Extra information about the target is not available.",
                        Adapter.AdapterId, TargetId);
                }

                var targetName = new DisplayConfigTargetDeviceName(Adapter.AdapterId, TargetId);
                var result = DisplayConfigApi.DisplayConfigGetDeviceInfo(ref targetName);

                if (result == Win32Status.Success)
                {
                    return targetName.MonitorFriendlyDeviceName;
                }

                throw new Win32Exception((int) result);
            }
        }

        /// <summary>
        ///     Gets a boolean value indicating the device availability
        /// </summary>
        public bool IsAvailable { get; }

        /// <summary>
        ///     Gets the display device preferred resolution
        /// </summary>
        /// <exception cref="TargetNotAvailableException">The target is not available</exception>
        /// <exception cref="Win32Exception">Error code can be retrieved from Win32Exception.NativeErrorCode property</exception>
        public Size PreferredResolution
        {
            get
            {
                if (!IsAvailable)
                {
                    throw new TargetNotAvailableException("Extra information about the target is not available.",
                        Adapter.AdapterId, TargetId);
                }

                var targetPreferredMode = new DisplayConfigTargetPreferredMode(Adapter.AdapterId, TargetId);
                var result = DisplayConfigApi.DisplayConfigGetDeviceInfo(ref targetPreferredMode);

                if (result == Win32Status.Success)
                {
                    return new Size((int) targetPreferredMode.Width, (int) targetPreferredMode.Height);
                }

                throw new Win32Exception((int) result);
            }
        }

        /// <summary>
        ///     Gets the display device preferred signal information
        /// </summary>
        /// <exception cref="TargetNotAvailableException">The target is not available</exception>
        /// <exception cref="Win32Exception">Error code can be retrieved from Win32Exception.NativeErrorCode property</exception>
        public PathTargetSignalInfo PreferredSignalMode
        {
            get
            {
                if (!IsAvailable)
                {
                    throw new TargetNotAvailableException("Extra information about the target is not available.",
                        Adapter.AdapterId, TargetId);
                }

                var targetPreferredMode = new DisplayConfigTargetPreferredMode(Adapter.AdapterId, TargetId);
                var result = DisplayConfigApi.DisplayConfigGetDeviceInfo(ref targetPreferredMode);

                if (result == Win32Status.Success)
                {
                    return new PathTargetSignalInfo(targetPreferredMode.TargetMode.TargetVideoSignalInfo);
                }

                throw new Win32Exception((int) result);
            }
        }

        /// <summary>
        ///     Gets the target identification
        /// </summary>
        public uint TargetId { get; }

        /// <summary>
        ///     Gets or sets the device virtual resolution support
        /// </summary>
        /// <exception cref="TargetNotAvailableException">The target is not available</exception>
        /// <exception cref="Win32Exception">Error code can be retrieved from Win32Exception.NativeErrorCode property</exception>
        public bool VirtualResolutionSupport
        {
            get
            {
                if (!IsAvailable)
                {
                    throw new TargetNotAvailableException("Extra information about the target is not available.",
                        Adapter.AdapterId, TargetId);
                }

                var targetSupportVirtualResolution = new DisplayConfigSupportVirtualResolution(Adapter.AdapterId,
                    TargetId);
                var result = DisplayConfigApi.DisplayConfigGetDeviceInfo(ref targetSupportVirtualResolution);

                if (result == Win32Status.Success)
                {
                    return !targetSupportVirtualResolution.DisableMonitorVirtualResolution;
                }

                throw new Win32Exception((int) result);
            }
            set
            {
                if (!IsAvailable)
                {
                    throw new TargetNotAvailableException("Extra information about the target is not available.",
                        Adapter.AdapterId, TargetId);
                }

                var targetSupportVirtualResolution = new DisplayConfigSupportVirtualResolution(Adapter.AdapterId,
                    TargetId, !value);
                var result = DisplayConfigApi.DisplayConfigSetDeviceInfo(ref targetSupportVirtualResolution);

                if (result != Win32Status.Success)
                {
                    throw new Win32Exception((int) result);
                }
            }
        }

        /// <inheritdoc />
        public bool Equals(PathDisplayTarget other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Adapter == other.Adapter && TargetId == other.TargetId;
        }

        /// <summary>
        ///     Retrieving a list of all display targets from the currently active and inactive paths
        /// </summary>
        /// <returns>An array of PathDisplayTarget instances</returns>
        public static PathDisplayTarget[] GetDisplayTargets()
        {
            var targets = new Dictionary<Tuple<LUID, uint>, PathDisplayTarget>();

            foreach (var pathInfo in PathInfo.GetAllPaths())
            foreach (var pathTargetInfo in pathInfo.TargetsInfo.Where(info => info.DisplayTarget.IsAvailable))
            {
                var key = Tuple.Create(
                    pathTargetInfo.DisplayTarget.Adapter.AdapterId,
                    pathTargetInfo.DisplayTarget.TargetId
                );

                if (!pathTargetInfo.DisplayTarget.Adapter.IsInvalid && !targets.ContainsKey(key))
                {
                    targets.Add(key, pathTargetInfo.DisplayTarget);
                }
            }

            return targets.Values.ToArray();
        }

        /// <summary>
        ///     Checks for equality of two PathDisplayTarget instances
        /// </summary>
        /// <param name="left">The first instance</param>
        /// <param name="right">The second instance</param>
        /// <returns>true if both instances are equal, otherwise false</returns>
        public static bool operator ==(PathDisplayTarget left, PathDisplayTarget right)
        {
            return Equals(left, right) || left?.Equals(right) == true;
        }

        /// <summary>
        ///     Checks for inequality of two PathDisplayTarget instances
        /// </summary>
        /// <param name="left">The first instance</param>
        /// <param name="right">The second instance</param>
        /// <returns>true if both instances are not equal, otherwise false</returns>
        public static bool operator !=(PathDisplayTarget left, PathDisplayTarget right)
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

            return obj.GetType() == GetType() && Equals((PathDisplayTarget) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Adapter != null ? Adapter.GetHashCode() : 0) * 397) ^ (int) TargetId;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return FriendlyName;
        }

#if !NETSTANDARD
        /// <summary>
        ///     Opens the registry key of the Windows PnP manager for this display target
        /// </summary>
        /// <returns>A RegistryKey instance for successful call, otherwise null</returns>
        public Microsoft.Win32.RegistryKey OpenDevicePnPKey()
        {
            if (string.IsNullOrWhiteSpace(DevicePath)) {
                return null;
            }

            var path = DevicePath;
            if (path.StartsWith("\\\\?\\"))
            {
                path = path.Substring(4).Replace("#", "\\");
                if (path.EndsWith("}"))
                {
                    var guidIndex = path.LastIndexOf("{", StringComparison.InvariantCulture);
                    if (guidIndex > 0) {
                        path = path.Substring(0, guidIndex);
                    }
                }
            }

            return Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                "SYSTEM\\CurrentControlSet\\Enum\\" + path,
                Microsoft.Win32.RegistryKeyPermissionCheck.ReadSubTree
            );
        }
#endif

        /// <summary>
        ///     Returns the corresponding <see cref="DisplayDevice"/> instance
        /// </summary>
        /// <returns>An instance of <see cref="DisplayDevice"/>, or null</returns>
        public DisplayDevice ToDisplayDevice()
        {
            return
                DisplayAdapter.GetDisplayAdapters()
                    .SelectMany(adapter => adapter.GetDisplayDevices())
                    .FirstOrDefault(device => device.DevicePath.Equals(DevicePath));
        }
    }
}