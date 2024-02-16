using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using WindowsDisplayAPI.DisplayConfig;
using WindowsDisplayAPI.Exceptions;
using WindowsDisplayAPI.Native;
using WindowsDisplayAPI.Native.DeviceContext;
using WindowsDisplayAPI.Native.DeviceContext.Structures;
using WindowsDisplayAPI.Native.Structures;

namespace WindowsDisplayAPI
{
    /// <summary>
    ///     Contains information about a display source screen
    /// </summary>
    public class DisplayScreen
    {
        private readonly IntPtr _monitorHandle;

        private DisplayScreen(IntPtr monitorHandle)
        {
            _monitorHandle = monitorHandle;
        }

        /// <summary>
        /// Gets the source identification number
        /// </summary>
        public int SourceId
        {
            get
            {
                var name = ScreenName;

                if (string.IsNullOrWhiteSpace(name))
                {
                    return 0;
                }

                var index = ScreenName.IndexOf("DISPLAY", StringComparison.Ordinal);

                return index < 0 ? 0 : int.Parse(name.Substring(index + 7));
            }
        }

        /// <summary>
        ///     Gets a list of all active screens
        /// </summary>
        /// <returns>An array of <see cref="DisplayScreen" /> instances.</returns>
        public static DisplayScreen[] GetScreens()
        {
            var result = new List<DisplayScreen>();
            var callback = new DeviceContextApi.MonitorEnumProcedure(
                (IntPtr handle, IntPtr dcHandle, ref RectangleL rect, IntPtr callbackObject) =>
                {
                    result.Add(new DisplayScreen(handle));

                    return 1;
                }
            );

            return DeviceContextApi.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, callback, IntPtr.Zero)
                ? result.ToArray()
                : null;
        }

        /// <summary>
        ///     Gets a <see cref="DisplaySetting"/> instance representing the screen current settings
        /// </summary>
        public DisplaySetting CurrentSetting
        {
            get => DisplaySetting.GetCurrentFromScreenName(ScreenName);
        }

        /// <summary>
        ///     Gets a <see cref="DisplaySetting"/> instance representing this screen saved settings
        /// </summary>
        public DisplaySetting SavedSetting
        {
            get => DisplaySetting.GetSavedFromScreenName(ScreenName);
        }

        /// <summary>
        ///     Disables and detaches all devices connected to this screen
        /// </summary>
        /// <param name="apply">Indicating if the changes should be applied immediately, recommended value is false</param>
        public void Disable(bool apply)
        {
            SetSettings(new DisplaySetting(), apply);
        }

        /// <summary>
        ///     Enables and attach all devices connected to this screen
        /// </summary>
        /// <param name="displaySetting">The display settings that should be applied while enabling the display device</param>
        /// <param name="apply">Indicating if the changes should be applied immediately, recommended value is false</param>
        public void Enable(DisplaySetting displaySetting, bool apply = false)
        {
            SetSettings(displaySetting, apply);
        }

        /// <summary>
        ///     Changes the display device settings to a new <see cref="DisplaySetting"/> instance
        /// </summary>
        /// <param name="displaySetting">The display settings that should be applied</param>
        /// <param name="apply">Indicating if the changes should be applied immediately, recommended value is false</param>
        public void SetSettings(DisplaySetting displaySetting, bool apply = false)
        {
            if (!IsValid)
            {
                throw new InvalidDisplayException(null);
            }

            displaySetting.Save(ScreenName, apply);
        }

        /// <summary>
        ///     Get information about the monitor covering the most of a rectangle.
        /// </summary>
        /// <param name="rectangle">The rectangle to get the main monitor information for.</param>
        /// <returns>An instance of <see cref="DisplayScreen" />.</returns>
        public static DisplayScreen FromRectangle(Rectangle rectangle)
        {
            var monitorHandle = DeviceContextApi.MonitorFromRect(
                new RectangleL(rectangle),
                MonitorFromFlag.DefaultToNearest
            );

            return monitorHandle == IntPtr.Zero ? null : new DisplayScreen(monitorHandle);
        }

        /// <summary>
        ///     Get information about the monitor containing or the nearest to a point.
        /// </summary>
        /// <param name="point">The point to get the main monitor information for.</param>
        /// <returns>An instance of <see cref="DisplayScreen" />.</returns>
        public static DisplayScreen FromPoint(Point point)
        {
            var monitorHandle = DeviceContextApi.MonitorFromPoint(
                new PointL(point),
                MonitorFromFlag.DefaultToNearest
            );

            return monitorHandle == IntPtr.Zero ? null : new DisplayScreen(monitorHandle);
        }

        /// <summary>
        ///     Get information about the screen covering the most of a window.
        /// </summary>
        /// <param name="hWnd">The window handle to get the main screen information for.</param>
        /// <returns>An instance of <see cref="DisplayScreen" />.</returns>
        public static DisplayScreen FromWindow(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
            {
                throw new ArgumentException("Invalid window handle provided.", nameof(hWnd));
            }

            var monitorHandle = DeviceContextApi.MonitorFromWindow(
                hWnd,
                MonitorFromFlag.DefaultToNearest
            );

            return monitorHandle == IntPtr.Zero ? null : new DisplayScreen(monitorHandle);
        }

#if !NETSTANDARD
        /// <summary>
        ///     Returns the corresponding <see cref="System.Windows.Forms.Screen" /> instance
        /// </summary>
        /// <returns>A instance of Screen object</returns>
        public System.Windows.Forms.Screen GetWinFormScreen()
        {
            if (!IsValid)
                throw new Exceptions.InvalidDisplayException();
            try
            {
                return System.Windows.Forms.Screen.AllScreens.FirstOrDefault(screen => screen.DeviceName.Equals(ScreenName));
            }
            catch
            {
                // ignored
            }
            return null;
        }
#endif

        /// <summary>
        ///     Get the corresponding <see cref="Display" /> instances.
        /// </summary>
        /// <returns>An array of <see cref="Display" /> instances.</returns>
        public Display[] GetDisplays()
        {
            return Display.GetDisplays().Where(display => display.ScreenName.Equals(ScreenName)).ToArray();
        }

        /// <summary>
        ///     Gets the bounds of the monitor
        /// </summary>
        public Rectangle Bounds
        {
            get => GetMonitorInfo()?.Bounds.ToRectangle() ?? Rectangle.Empty;
        }

        /// <summary>
        ///     Gets the source name of the screen
        /// </summary>
        public string ScreenName
        {
            get => GetMonitorInfo()?.DisplayName;
        }

        /// <summary>
        ///     Gets a boolean value indicating if this is the primary display
        /// </summary>
        public bool IsPrimary
        {
            get => GetMonitorInfo()?.Flags.HasFlag(MonitorInfoFlags.Primary) ?? false;
        }

        /// <summary>
        ///     Gets a boolean value indicating if this instance contains valid information.
        /// </summary>
        public bool IsValid
        {
            get => GetMonitorInfo() != null;
        }

        /// <summary>
        ///     Gets the working area of the monitor
        /// </summary>
        public Rectangle WorkingArea
        {
            get => GetMonitorInfo()?.WorkingArea.ToRectangle() ?? Rectangle.Empty;
        }

        /// <summary>
        ///     Returns a list of possible display setting for this screen
        /// </summary>
        /// <returns>An enumerable list of <see cref="DisplayPossibleSetting"/> instances</returns>
        public IEnumerable<DisplayPossibleSetting> GetPossibleSettings()
        {
            if (!IsValid)
            {
                yield break;
            }

            var index = -1;
            while (true)
            {
                index++;
                var deviceMode = new DeviceMode(DeviceModeFields.None);
                if (!DeviceContextApi.EnumDisplaySettings(ScreenName, (DisplaySettingsMode)index, ref deviceMode))
                {
                    break;
                }
                yield return new DisplayPossibleSetting(deviceMode);
            }
        }

        /// <summary>
        ///     Returns the best possible display setting for this screen
        /// </summary>
        /// <returns>A <see cref="DisplayPossibleSetting"/> instance</returns>
        public DisplayPossibleSetting GetPreferredSetting()
        {
            return IsValid
                ? GetPossibleSettings()
                    .OrderByDescending(setting => (int)setting.ColorDepth)
                    .ThenByDescending(setting => (ulong)setting.Resolution.Width * (ulong)setting.Resolution.Height)
                    .ThenByDescending(setting => setting.Frequency)
                    .FirstOrDefault()
                : null;
        }

        /// <summary>
        ///     Returns the corresponding <see cref="PathDisplaySource"/> instance
        /// </summary>
        /// <returns>An instance of <see cref="PathDisplaySource"/>, or null</returns>
        public PathDisplaySource ToPathDisplaySource()
        {
            return PathDisplaySource
                .GetDisplaySources()
                .FirstOrDefault(source => source.DisplayName.Equals(ScreenName));
        }

        private MonitorInfo? GetMonitorInfo()
        {
            var monitorInfo = MonitorInfo.Initialize();

            if (DeviceContextApi.GetMonitorInfo(_monitorHandle, ref monitorInfo))
            {
                return monitorInfo;
            }

            return null;
        }
    }
}