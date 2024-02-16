using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using WindowsDisplayAPI.Exceptions;
using WindowsDisplayAPI.Native;
using WindowsDisplayAPI.Native.DeviceContext;
using WindowsDisplayAPI.Native.DeviceContext.Structures;
using WindowsDisplayAPI.Native.Structures;

namespace WindowsDisplayAPI
{
    /// <summary>
    ///     Holds configurations of a windows display
    /// </summary>
    public class DisplaySetting : DisplayPossibleSetting
    {
        /// <summary>
        ///     Creates a new <see cref="DisplaySetting" /> instance.
        /// </summary>
        /// <param name="validSetting">The basic configuration information object</param>
        /// <param name="position">Display position on desktop</param>
        public DisplaySetting(DisplayPossibleSetting validSetting, Point position = default)
            : this(validSetting, position, DisplayOrientation.Identity, DisplayFixedOutput.Default)
        {
        }

        /// <summary>
        ///     Creates a new <see cref="DisplaySetting" /> instance.
        /// </summary>
        /// <param name="validSetting">The basic configuration information object</param>
        /// <param name="position">Display position on desktop</param>
        /// <param name="orientation">Display orientation and rotation</param>
        /// <param name="outputScalingMode">
        ///     Display output behavior in case of presenting a low-resolution mode on a
        ///     higher-resolution display
        /// </param>
        public DisplaySetting(
            DisplayPossibleSetting validSetting,
            Point position,
            DisplayOrientation orientation,
            DisplayFixedOutput outputScalingMode)
            : this(
                validSetting.Resolution, position, validSetting.ColorDepth, validSetting.Frequency,
                validSetting.IsInterlaced, orientation, outputScalingMode
            )
        {
        }

        /// <summary>
        ///     Creates a new <see cref="DisplaySetting" /> instance.
        /// </summary>
        /// <param name="resolution">Display resolution</param>
        /// <param name="position">Display position on desktop</param>
        /// <param name="frequency">Display frequency</param>
        public DisplaySetting(Size resolution, Point position, int frequency)
            : this(resolution, position, ColorDepth.Depth32Bit, frequency)
        {
        }

        /// <summary>
        ///     Creates a new <see cref="DisplaySetting" /> instance.
        /// </summary>
        /// <param name="resolution">Display resolution</param>
        /// <param name="frequency">Display frequency</param>
        public DisplaySetting(Size resolution, int frequency)
            : this(resolution, new Point(0, 0), ColorDepth.Depth32Bit, frequency)
        {
        }

        /// <summary>
        ///     Creates a new <see cref="DisplaySetting" /> instance.
        /// </summary>
        /// <param name="resolution">Display resolution</param>
        /// <param name="position">Display position on desktop</param>
        /// <param name="frequency">Display frequency</param>
        /// <param name="colorDepth">Display color depth</param>
        /// <param name="isInterlaced">Indicating if display is using interlaces scan out</param>
        /// <param name="orientation">Display orientation and rotation</param>
        /// <param name="outputScalingMode">
        ///     Display output behavior in case of presenting a low-resolution mode on a
        ///     higher-resolution display
        /// </param>
        public DisplaySetting(
            Size resolution,
            Point position,
            ColorDepth colorDepth,
            int frequency,
            bool isInterlaced = false,
            DisplayOrientation orientation = DisplayOrientation.Identity,
            DisplayFixedOutput outputScalingMode = DisplayFixedOutput.Default
        ) : base(resolution, frequency, colorDepth, isInterlaced)
        {
            Position = position;
            Orientation = orientation;
            OutputScalingMode = outputScalingMode;
        }

        internal DisplaySetting() : base(default)
        {
            IsEnable = false;
        }

        private DisplaySetting(DeviceMode deviceMode) : base(deviceMode)
        {
            Position = new Point(deviceMode.Position.X, deviceMode.Position.Y);
            Orientation = deviceMode.DisplayOrientation;
            OutputScalingMode = deviceMode.DisplayFixedOutput;

            if (Resolution.IsEmpty && Position.IsEmpty)
            {
                IsEnable = false;
            }
        }

        /// <summary>
        ///     Gets a boolean value indicating if this instance is currently enable
        /// </summary>
        public bool IsEnable { get; } = true;

        /// <summary>
        ///     Gets or sets the orientation of the display monitor
        /// </summary>
        public DisplayOrientation Orientation { get; }

        /// <summary>
        ///     Gets output behavior in case of presenting a low-resolution mode on a higher-resolution display
        /// </summary>
        public DisplayFixedOutput OutputScalingMode { get; }

        /// <summary>
        ///     Gets or sets the position of the display monitor
        /// </summary>
        public Point Position { get; }

        /// <summary>
        ///     Applies settings that are saved using SaveDisplaySettings() or other similar methods but not yet applied
        /// </summary>
        public static void ApplySavedSettings()
        {
            var result = DeviceContextApi.ChangeDisplaySettingsEx(
                null,
                IntPtr.Zero,
                IntPtr.Zero,
                ChangeDisplaySettingsFlags.Reset,
                IntPtr.Zero
            );

            if (result != ChangeDisplaySettingsExResults.Successful)
            {
                throw new ModeChangeException($"[{result}]: Applying saved settings failed.", null, result);
            }
        }

        /// <summary>
        ///     Returns the current display settings of a screen
        /// </summary>
        /// <param name="screenName">The name of the screen.</param>
        /// <returns>An instance of <see cref="DisplaySetting" /></returns>
        public static DisplaySetting GetCurrentFromScreenName(string screenName)
        {
            return new DisplaySetting(GetDeviceMode(screenName, DisplaySettingsMode.CurrentSettings));
        }

        /// <summary>
        ///     Returns the saved display settings of a screen
        /// </summary>
        /// <param name="screenName">The name of the screen.</param>
        /// <returns>An instance of <see cref="DisplaySetting" /></returns>
        public static DisplaySetting GetSavedFromScreenName(string screenName)
        {
            return new DisplaySetting(GetDeviceMode(screenName, DisplaySettingsMode.RegistrySettings));
        }

        /// <summary>
        ///     Sets and possibility applies a list of display settings
        /// </summary>
        /// <param name="newSettingPairs">
        ///     A key value dictionary of <see cref="DisplayDevice" /> and <see cref="DisplaySetting" />
        ///     instances.
        /// </param>
        /// <param name="applyNow">Indicating if the changes should be applied immediately, recommended value is false</param>
        public static void SaveDisplaySettings(
            Dictionary<DisplayScreen, DisplaySetting> newSettingPairs,
            bool applyNow)
        {
            SaveDisplaySettings(
                newSettingPairs.ToDictionary(pair => pair.Key.ScreenName, pair => pair.Value),
                applyNow,
                true
            );
        }

        /// <summary>
        ///     Sets and possibility applies a list of display settings
        /// </summary>
        /// <param name="newSettingPairs">A key value dictionary of source ids and <see cref="DisplaySetting" /> instance</param>
        /// <param name="applyNow">Indicating if the changes should be applied immediately, recommended value is false</param>
        public static void SaveDisplaySettings(
            Dictionary<int, DisplaySetting> newSettingPairs,
            bool applyNow)
        {
            SaveDisplaySettings(
                newSettingPairs.ToDictionary(pair => $"\\\\.\\DISPLAY{pair.Key:D}", pair => pair.Value),
                applyNow,
                true
            );
        }

        private static DeviceMode GetDeviceMode(string screenName, DisplaySettingsMode flags)
        {
            var deviceMode = new DeviceMode(DeviceModeFields.None);

            return !string.IsNullOrWhiteSpace(screenName) &&
                   DeviceContextApi.EnumDisplaySettings(
                       screenName,
                       flags,
                       ref deviceMode
                   )
                ? deviceMode
                : default;
        }

        private static void SaveDisplaySettings(
            Dictionary<string, DisplaySetting> newSettings,
            bool applyNow,
            bool retry
        )
        {
            var screens = DisplayScreen.GetScreens()
                .Where(screen => screen.IsValid)
                .ToList();

            var rollbackSettings = screens
                .ToDictionary(screen => screen.ScreenName, screen => screen.CurrentSetting);

            try
            {
                foreach (var newSetting in newSettings)
                {
                    screens.Remove(
                        screens.FirstOrDefault(
                            screen => screen.ScreenName.Equals(newSetting.Key)
                        )
                    );
                    newSetting.Value.Save(newSetting.Key, false);
                }

                // Disable missing monitors
                foreach (var screen in screens.Where(screen => screen.IsValid))
                {
                    screen.Disable(false);
                }

                if (applyNow)
                {
                    ApplySavedSettings();
                }
            }
            catch (ModeChangeException)
            {
                if (retry)
                {
                    SaveDisplaySettings(rollbackSettings, false, false);
                }

                throw;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return IsEnable
                ? $"{Resolution} {(IsInterlaced ? "Interlaced" : "Progressive")} {Frequency}hz @ {ColorDepth} @ {Position}"
                : "Disabled";
        }

        internal void Save(string screenName, bool reset)
        {
            var deviceMode = GetDeviceMode(screenName);
            var flags = ChangeDisplaySettingsFlags.UpdateRegistry | ChangeDisplaySettingsFlags.Global;
            flags |= reset ? ChangeDisplaySettingsFlags.Reset : ChangeDisplaySettingsFlags.NoReset;

            if (IsEnable && Position.X == 0 && Position.Y == 0)
            {
                flags |= ChangeDisplaySettingsFlags.SetPrimary;
            }

            var result = DeviceContextApi.ChangeDisplaySettingsEx(
                screenName,
                ref deviceMode,
                IntPtr.Zero,
                flags,
                IntPtr.Zero
            );

            if (result != ChangeDisplaySettingsExResults.Successful)
            {
                throw new ModeChangeException($"[{result}]: Applying saved settings failed.", null, result);
            }
        }

        private DeviceMode GetDeviceMode(string screenName)
        {
            DeviceMode deviceMode;

            if (IsEnable)
            {
                var flags = DisplayFlags.None;

                if (IsInterlaced)
                {
                    flags |= DisplayFlags.Interlaced;
                }

                deviceMode = new DeviceMode(
                    screenName,
                    new PointL(Position),
                    Orientation,
                    OutputScalingMode,
                    (uint) ColorDepth,
                    (uint) Resolution.Width,
                    (uint) Resolution.Height,
                    flags,
                    (uint) Frequency
                );
            }
            else
            {
                deviceMode = new DeviceMode(
                    screenName,
                    DeviceModeFields.PelsWidth | DeviceModeFields.PelsHeight | DeviceModeFields.Position
                );
            }

            if (string.IsNullOrWhiteSpace(deviceMode.DeviceName))
            {
                throw new MissingDisplayException("Display screen is missing or invalid.", null);
            }

            return deviceMode;
        }
    }
}