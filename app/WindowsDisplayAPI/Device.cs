namespace WindowsDisplayAPI
{
    /// <summary>
    ///     Represents a Windows Video Device including Display Devices and Video Controllers
    /// </summary>
    public abstract class Device
    {
        /// <summary>
        ///     Creates a new Device
        /// </summary>
        /// <param name="devicePath">The device path</param>
        /// <param name="deviceName">The device name</param>
        /// <param name="deviceKey">The device driver registry key</param>
        protected Device(string devicePath, string deviceName, string deviceKey)
        {
            DevicePath = devicePath;
            DeviceName = deviceName;
            DeviceKey = deviceKey;
        }

        /// <summary>
        ///     Gets the registry address of the device driver and configuration
        /// </summary>
        public virtual string DeviceKey { get; }

        /// <summary>
        ///     Gets the Windows device name
        /// </summary>
        public virtual string DeviceName { get; }

        /// <summary>
        ///     Gets the Windows device path
        /// </summary>
        public virtual string DevicePath { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{GetType().Name}: {DeviceName}";
        }

#if !NETSTANDARD
        /// <summary>
        ///     Opens the registry key at the address specified by the DeviceKey property
        /// </summary>
        /// <returns>A RegistryKey instance for successful call, otherwise null</returns>
        /// <exception cref="WindowsDisplayAPI.Exceptions.InvalidRegistryAddressException">Registry address is invalid or unknown.</exception>
        public Microsoft.Win32.RegistryKey OpenDeviceKey()
        {
            if (string.IsNullOrWhiteSpace(DeviceKey)) {
                return null;
            }

            const string machineRootName = "\\Registry\\Machine\\";
            const string userRootName = "\\Registry\\Current\\";

            if (DeviceKey.StartsWith(machineRootName, System.StringComparison.InvariantCultureIgnoreCase)) {
                return Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                    DeviceKey.Substring(machineRootName.Length),
                    Microsoft.Win32.RegistryKeyPermissionCheck.ReadSubTree
                );
            }

            if (DeviceKey.StartsWith(userRootName, System.StringComparison.InvariantCultureIgnoreCase)) {
                return Microsoft.Win32.Registry.Users.OpenSubKey(
                    DeviceKey.Substring(userRootName.Length),
                    Microsoft.Win32.RegistryKeyPermissionCheck.ReadSubTree
                );
            }

            throw new Exceptions.InvalidRegistryAddressException("Registry address is invalid or unknown.");
        }

        /// <summary>
        ///     Opens the registry key of the Windows PnP manager for this device
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
                    var guidIndex = path.LastIndexOf("{", System.StringComparison.InvariantCulture);
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
    }
}