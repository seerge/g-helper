using System.Collections.Generic;
using System.Linq;

namespace WindowsDisplayAPI
{
    /// <summary>
    ///     Represents a Windows UnAttached Display Device
    /// </summary>
    public class UnAttachedDisplay : DisplayDevice
    {
        /// <summary>
        ///     Creates a new UnAttachedDisplay
        /// </summary>
        /// <param name="device">The DisplayDevice instance to copy information from</param>
        protected UnAttachedDisplay(DisplayDevice device)
            : base(
                device.DevicePath,
                device.DeviceName,
                device.DeviceKey,
                device.Adapter,
                device.ScreenName,
                device.DisplayName,
                device.IsAvailable,
                false
            )
        {
        }

        /// <inheritdoc />
        public override bool IsAvailable
        {
            get => base.IsAvailable || !IsValid;
        }

        /// <inheritdoc />
        public override bool IsValid
        {
            get
            {
                return DisplayAdapter.GetDisplayAdapters()
                    .SelectMany(adapter => adapter.GetDisplayDevices(base.IsAvailable))
                    .Any(
                        device => device.DevicePath.Equals(DevicePath) && device.DeviceKey.Equals(DeviceKey)
                    );
            }
        }

        /// <summary>
        ///     Returns a list of all unattached displays on this machine
        /// </summary>
        /// <returns>An enumerable list of UnAttachedDisplay</returns>
        public static IEnumerable<UnAttachedDisplay> GetUnAttachedDisplays()
        {
            return DisplayAdapter.GetDisplayAdapters()
                .SelectMany(adapter => adapter.GetDisplayDevices(false))
                .Select(device => new UnAttachedDisplay(device));
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return IsValid ? $"{GetType().Name}: {DisplayName} ({DeviceName})" : $"{GetType().Name}: Invalid";
        }

        /// <summary>
        ///     Returns the corresponding Display device for this unattached display. Only functions when this instance is invalidated
        ///     due to display attachment.
        /// </summary>
        /// <returns></returns>
        public Display ToDisplay()
        {
            return IsValid
                ? null
                : Display.GetDisplays()
                    .FirstOrDefault(
                        display => display.DevicePath.Equals(DevicePath) && display.DeviceKey.Equals(DeviceKey)
                    );
        }
    }
}