using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using WindowsDisplayAPI.Native;
using WindowsDisplayAPI.Native.DisplayConfig.Structures;
using WindowsDisplayAPI.Native.Structures;

namespace WindowsDisplayAPI.DisplayConfig
{
    /// <summary>
    ///     Represents a path display adapter
    /// </summary>
    public class PathDisplayAdapter : IEquatable<PathDisplayAdapter>
    {
        /// <summary>
        ///     Creates a new PathDisplayAdapter
        /// </summary>
        /// <param name="adapterId">The adapter local unique identification</param>
        public PathDisplayAdapter(LUID adapterId)
        {
            AdapterId = adapterId;
        }

        /// <summary>
        ///     Gets the display adapter local identification LUID
        /// </summary>
        public LUID AdapterId { get; }

        /// <summary>
        ///     Gets the display adapter device path
        /// </summary>
        /// <exception cref="Win32Exception">Error code can be retrieved from Win32Exception.NativeErrorCode property</exception>
        public string DevicePath
        {
            get
            {
                var adapterName = new DisplayConfigAdapterName(AdapterId);
                var result = DisplayConfigApi.DisplayConfigGetDeviceInfo(ref adapterName);

                if (result == Win32Status.Success)
                {
                    return adapterName.AdapterDevicePath;
                }

                throw new Win32Exception((int) result);
            }
        }

        /// <summary>
        ///     Gets a boolean value indicating the instance validity
        /// </summary>
        public bool IsInvalid
        {
            get => AdapterId.IsEmpty();
        }

        /// <inheritdoc />
        public bool Equals(PathDisplayAdapter other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return AdapterId == other.AdapterId;
        }

        /// <summary>
        ///     Retrieving a list of all adapters from the currently active and inactive paths
        /// </summary>
        /// <returns>An array of PathDisplayAdapter instances</returns>
        public static PathDisplayAdapter[] GetAdapters()
        {
            var adapters = new Dictionary<LUID, PathDisplayAdapter>();

            foreach (var pathInfo in PathInfo.GetAllPaths())
            {
                if (!pathInfo.DisplaySource.Adapter.IsInvalid &&
                    !adapters.ContainsKey(pathInfo.DisplaySource.Adapter.AdapterId))
                {
                    adapters.Add(pathInfo.DisplaySource.Adapter.AdapterId, pathInfo.DisplaySource.Adapter);
                }

                foreach (var pathTargetInfo in pathInfo.TargetsInfo)
                {
                    if (!pathTargetInfo.DisplayTarget.Adapter.IsInvalid &&
                        !adapters.ContainsKey(pathTargetInfo.DisplayTarget.Adapter.AdapterId))
                    {
                        adapters.Add(pathTargetInfo.DisplayTarget.Adapter.AdapterId, pathTargetInfo.DisplayTarget.Adapter);
                    }
                }
            }

            return adapters.Values.ToArray();
        }

        /// <summary>
        ///     Checks for equality of two PathDisplayAdapter instances
        /// </summary>
        /// <param name="left">The first instance</param>
        /// <param name="right">The second instance</param>
        /// <returns>true if both instances are equal, otherwise false</returns>
        public static bool operator ==(PathDisplayAdapter left, PathDisplayAdapter right)
        {
            return Equals(left, right) || left?.Equals(right) == true;
        }

        /// <summary>
        ///     Checks for inequality of two PathDisplayAdapter instances
        /// </summary>
        /// <param name="left">The first instance</param>
        /// <param name="right">The second instance</param>
        /// <returns>true if both instances are not equal, otherwise false</returns>
        public static bool operator !=(PathDisplayAdapter left, PathDisplayAdapter right)
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

            return obj.GetType() == GetType() && Equals((PathDisplayAdapter) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return AdapterId.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return DevicePath;
        }

#if !NETSTANDARD
        /// <summary>
        ///     Opens the registry key of the Windows PnP manager for this display adapter
        /// </summary>
        /// <returns>A RegistryKey instance for successful call, otherwise null</returns>
        public Microsoft.Win32.RegistryKey OpenDevicePnPKey()
        {
            if (string.IsNullOrWhiteSpace(DevicePath))
                return null;
            var path = DevicePath;
            if (path.StartsWith("\\\\?\\"))
            {
                path = path.Substring(4).Replace("#", "\\");
                if (path.EndsWith("}"))
                {
                    var guidIndex = path.LastIndexOf("{", StringComparison.InvariantCulture);
                    if (guidIndex > 0)
                        path = path.Substring(0, guidIndex);
                }
            }
            return Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum\\" + path,
                Microsoft.Win32.RegistryKeyPermissionCheck.ReadSubTree);
        }
#endif

        /// <summary>
        ///     Gets the corresponding DisplayAdapter instance
        /// </summary>
        /// <returns>An instance of DisplayAdapter, or null</returns>
        public DisplayAdapter ToDisplayAdapter()
        {
            return DisplayAdapter.GetDisplayAdapters()
                .FirstOrDefault(
                    adapter => DevicePath.StartsWith("\\\\?\\" + adapter.DevicePath.Replace("\\", "#"))
                );
        }
    }
}