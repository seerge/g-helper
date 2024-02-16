using System.Linq;
using WindowsDisplayAPI.DisplayConfig;
using WindowsDisplayAPI.Native.DeviceContext;

namespace WindowsDisplayAPI
{
    /// <summary>
    ///     Represents a Windows Display Device
    /// </summary>
    public class DisplayDevice : Device
    {
        /// <summary>
        ///     Creates a new DisplayDevice
        /// </summary>
        /// <param name="devicePath">The device path</param>
        /// <param name="deviceName">The device name</param>
        /// <param name="deviceKey">The device driver registry key</param>
        protected DisplayDevice(string devicePath, string deviceName, string deviceKey)
            : base(devicePath, deviceName, deviceKey)
        {
        }

        /// <summary>
        ///     Gets the corresponding <see cref="DisplayScreen" /> instance.
        /// </summary>
        public DisplayScreen DisplayScreen
        {
            get => DisplayScreen.GetScreens().FirstOrDefault(info => info.ScreenName.Equals(ScreenName));
        }

        /// <summary>
        ///     Creates a new DisplayDevice
        /// </summary>
        /// <param name="devicePath">The device path</param>
        /// <param name="deviceName">The device name</param>
        /// <param name="deviceKey">The device driver registry key</param>
        /// <param name="adapter">The device parent DisplayAdapter</param>
        /// <param name="isAvailable">true if the device is attached, otherwise false</param>
        /// <param name="isValid">true if this instance is valid, otherwise false</param>
        protected DisplayDevice(
            string devicePath,
            string deviceName,
            string deviceKey,
            DisplayAdapter adapter,
            bool isAvailable,
            bool isValid)
            : this(devicePath, deviceName, deviceKey)
        {
            Adapter = adapter;
            IsAvailable = isAvailable;
            IsValid = isValid;
        }

        /// <summary>
        ///     Creates a new DisplayDevice
        /// </summary>
        /// <param name="devicePath">The device path</param>
        /// <param name="deviceName">The device name</param>
        /// <param name="deviceKey">The device driver registry key</param>
        /// <param name="adapter">The device parent DisplayAdapter</param>
        /// <param name="displayName">The device source display name</param>
        /// <param name="displayFullName">The device target display name</param>
        /// <param name="isAvailable">true if the device is attached, otherwise false</param>
        /// <param name="isValid">true if this instance is valid, otherwise false</param>
        protected DisplayDevice(
            string devicePath,
            string deviceName,
            string deviceKey,
            DisplayAdapter adapter,
            string displayName,
            string displayFullName,
            bool isAvailable,
            bool isValid)
            : this(devicePath, deviceName, deviceKey, adapter, isAvailable, isValid)
        {
            ScreenName = displayName;
            DisplayName = displayFullName;
        }

        /// <summary>
        ///     Gets the display device driving display adapter instance
        /// </summary>
        public virtual DisplayAdapter Adapter { get; }

        /// <summary>
        ///     Gets the display device target name
        /// </summary>
        public virtual string DisplayName { get; }

        /// <summary>
        ///     Gets the display device source name
        /// </summary>
        public virtual string ScreenName { get; }

        /// <summary>
        ///     Gets a boolean value indicating if this display device is currently attached
        /// </summary>
        public virtual bool IsAvailable { get; }

        /// <summary>
        ///     Gets a boolean value indicating if this instance is no longer valid, this may happen when display device attached
        ///     status changes
        /// </summary>
        public virtual bool IsValid { get; }

        internal static DisplayDevice FromDeviceInformation(
            DisplayAdapter adapter,
            Native.DeviceContext.Structures.DisplayDevice sourceDevice,
            Native.DeviceContext.Structures.DisplayDevice targetDevice
        )
        {
            return new DisplayDevice(
                targetDevice.DeviceId,
                targetDevice.DeviceString,
                targetDevice.DeviceKey,
                adapter,
                sourceDevice.DeviceName,
                targetDevice.DeviceName,
                targetDevice.StateFlags.HasFlag(DisplayDeviceStateFlags.AttachedToDesktop),
                true
            );
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(DeviceName)
                ? $"{GetType().Name}: {DisplayName} - IsAvailable: {IsAvailable}"
                : $"{GetType().Name}: {DisplayName} ({DeviceName}) - IsAvailable: {IsAvailable}";
        }

        /// <summary>
        ///     Returns the corresponding PathDisplayTarget instance
        /// </summary>
        /// <returns>An instance of PathDisplayTarget, or null</returns>
        public PathDisplayTarget ToPathDisplayTarget()
        {
            return PathDisplayTarget
                .GetDisplayTargets()
                .FirstOrDefault(target => target.DevicePath.Equals(DevicePath));
        }
    }
}