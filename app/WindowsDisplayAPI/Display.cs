using System;
using System.Collections.Generic;
using System.Linq;
using WindowsDisplayAPI.Exceptions;
using WindowsDisplayAPI.Native;
using WindowsDisplayAPI.Native.DeviceContext;
using WindowsDisplayAPI.Native.DeviceContext.Structures;

namespace WindowsDisplayAPI
{
    /// <summary>
    ///     Represents a Windows Attached Display Device
    /// </summary>
    public class Display : DisplayDevice
    {
        /// <summary>
        ///     Creates a new Display
        /// </summary>
        /// <param name="device">The DisplayDevice instance to copy information from</param>
        protected Display(DisplayDevice device)
            : base(
                device.DevicePath,
                device.DeviceName,
                device.DeviceKey,
                device.Adapter,
                device.IsAvailable,
                false
            )
        {
        }

        /// <summary>
        ///     Gets the display capabilities.
        /// </summary>
        public MonitorCapabilities Capabilities
        {
            get
            {
                var handle = DCHandle.CreateFromDevice(ScreenName, DevicePath);

                if (!IsValid || handle?.IsInvalid != false)
                {
                    throw new InvalidDisplayException(DevicePath);
                }

                return new MonitorCapabilities(handle);
            }
        }

        /// <inheritdoc />
        public override string DisplayName
        {
            get
            {
                if (IsValid)
                {
                    return DisplayAdapter.GetDisplayAdapters()
                        .SelectMany(adapter => adapter.GetDisplayDevices(base.IsAvailable))
                        .FirstOrDefault(
                            device => device.DevicePath.Equals(DevicePath) && device.DeviceKey.Equals(DeviceKey)
                        )?.DisplayName;
                }

                return ToUnAttachedDisplay()?.DisplayName;
            }
        }

        /// <summary>
        ///     Gets or sets the display gamma ramp look up table.
        /// </summary>
        public DisplayGammaRamp GammaRamp
        {
            get
            {
                var handle = DCHandle.CreateFromDevice(ScreenName, DevicePath);

                if (!IsValid || handle?.IsInvalid != false)
                {
                    throw new InvalidDisplayException(DevicePath);
                }

                var gammaRamp = new GammaRamp();

                return DeviceContextApi.GetDeviceGammaRamp(handle, ref gammaRamp)
                    ? new DisplayGammaRamp(gammaRamp)
                    : null;
            }
            set
            {
                var handle = DCHandle.CreateFromDevice(ScreenName, DevicePath);

                if (!IsValid || handle?.IsInvalid != false)
                {
                    throw new InvalidDisplayException(DevicePath);
                }

                var gammaRamp = value.AsRamp();

                if (!DeviceContextApi.SetDeviceGammaRamp(handle, ref gammaRamp))
                {
                    //throw new ArgumentException("Invalid argument or value passed.", nameof(value));
                }
            }
        }

        /// <inheritdoc />
        public override bool IsAvailable
        {
            get => base.IsAvailable && IsValid;
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

        /// <inheritdoc />
        public override string ScreenName
        {
            get
            {
                if (IsValid)
                {
                    return
                        DisplayAdapter.GetDisplayAdapters()
                            .SelectMany(adapter => adapter.GetDisplayDevices(base.IsAvailable))
                            .FirstOrDefault(
                                device => device.DevicePath.Equals(DevicePath) && device.DeviceKey.Equals(DeviceKey)
                            )?.ScreenName;
                }

                return ToUnAttachedDisplay()?.ScreenName;
            }
        }

        /// <summary>
        ///     Returns a list of all attached displays on this machine
        /// </summary>
        /// <returns>An enumerable list of Displays</returns>
        public static IEnumerable<Display> GetDisplays()
        {
            return DisplayAdapter.GetDisplayAdapters()
                .SelectMany(adapter => adapter.GetDisplayDevices(true))
                .Select(device => new Display(device));
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return IsValid ? $"{GetType().Name}: {DisplayName} ({DeviceName})" : $"{GetType().Name}: Invalid";
        }

        /// <summary>
        ///     Returns the corresponding UnAttachedDisplay device for this display. Only valid when this instance is invalidated
        ///     due to display detachment.
        /// </summary>
        /// <returns></returns>
        public UnAttachedDisplay ToUnAttachedDisplay()
        {
            if (IsValid)
            {
                return null;
            }

            return UnAttachedDisplay.GetUnAttachedDisplays()
                .FirstOrDefault(
                    display => display.DevicePath.Equals(DevicePath) && display.DeviceKey.Equals(DeviceKey)
                );
        }
    }
}