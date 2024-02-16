using System.Collections.Generic;
using System.Linq;
using WindowsDisplayAPI.DisplayConfig;
using WindowsDisplayAPI.Native;

namespace WindowsDisplayAPI
{
    /// <summary>
    ///     Represents a Windows Video Controller Display Adapter Device
    /// </summary>
    public class DisplayAdapter : Device
    {
        /// <summary>
        ///     Creates a new DisplayAdapter
        /// </summary>
        /// <param name="devicePath">The device path</param>
        /// <param name="deviceName">The device name</param>
        /// <param name="deviceKey">The device driver registry key</param>
        protected DisplayAdapter(string devicePath, string deviceName, string deviceKey)
            : base(devicePath, deviceName, deviceKey)
        {
        }

        /// <summary>
        ///     Returns a list of all display adapters on this machine
        /// </summary>
        /// <returns>An enumerable list of DisplayAdapters</returns>
        public static IEnumerable<DisplayAdapter> GetDisplayAdapters()
        {
            var device = Native.DeviceContext.Structures.DisplayDevice.Initialize();
            var deviceIds = new List<string>();

            for (uint i = 0; DeviceContextApi.EnumDisplayDevices(null, i, ref device, 0); i++)
            {
                if (!deviceIds.Contains(device.DeviceId))
                {
                    deviceIds.Add(device.DeviceId);

                    yield return new DisplayAdapter(device.DeviceId, device.DeviceString, device.DeviceKey);
                }

                device = Native.DeviceContext.Structures.DisplayDevice.Initialize();
            }
        }

        /// <summary>
        ///     Returns a list of all display devices connected to this adapter
        /// </summary>
        /// <returns>An enumerable list of DisplayDevices</returns>
        public IEnumerable<DisplayDevice> GetDisplayDevices()
        {
            return GetDisplayDevices(null);
        }

        /// <summary>
        ///     Returns the corresponding PathDisplayAdapter instance
        /// </summary>
        /// <returns>An instance of PathDisplayAdapter, or null</returns>
        public PathDisplayAdapter ToPathDisplayAdapter()
        {
            return PathDisplayAdapter.GetAdapters()
                .FirstOrDefault(adapter =>
                    adapter.DevicePath.StartsWith("\\\\?\\" + DevicePath.Replace("\\", "#"))
                );
        }

        internal IEnumerable<DisplayDevice> GetDisplayDevices(bool? filterByAvailability)
        {
            var returned = new Dictionary<string, string>();

            var adapterIndex = -1;

            while (true)
            {
                adapterIndex++;
                var adapter = Native.DeviceContext.Structures.DisplayDevice.Initialize();

                if (!DeviceContextApi.EnumDisplayDevices(null, (uint) adapterIndex, ref adapter, 0))
                {
                    break;
                }

                if (!DevicePath.Equals(adapter.DeviceId))
                {
                    continue;
                }

                var displayIndex = -1;

                while (true)
                {
                    displayIndex++;
                    var display = Native.DeviceContext.Structures.DisplayDevice.Initialize();

                    if (!DeviceContextApi.EnumDisplayDevices(adapter.DeviceName, (uint) displayIndex, ref display, 1))
                    {
                        break;
                    }

                    var displayDevice = DisplayDevice.FromDeviceInformation(this, adapter, display);

                    if (!filterByAvailability.HasValue)
                    {
                        yield return displayDevice;
                    }
                    else if (displayDevice.IsAvailable == filterByAvailability.Value)
                    {
                        if (returned.ContainsKey(display.DeviceId) &&
                            returned[display.DeviceId].Equals(display.DeviceKey)
                        )
                        {
                            continue;
                        }

                        returned.Add(display.DeviceId, display.DeviceKey);

                        yield return displayDevice;
                    }
                }
            }
        }
    }
}