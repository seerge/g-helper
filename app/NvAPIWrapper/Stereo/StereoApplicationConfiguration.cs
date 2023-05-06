using System;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.Stereo;
using NvAPIWrapper.Native.Stereo.Structures;

namespace NvAPIWrapper.Stereo
{
    /// <summary>
    ///     Represents an application registry configuration profile as well as providing static access to system-wide and
    ///     application-wide stereo configurations.
    /// </summary>
    public class StereoApplicationConfiguration
    {
        private StereoApplicationConfiguration(StereoRegistryProfileType profileType)
        {
            ProfileType = profileType;
            StereoApi.CreateConfigurationProfileRegistryKey(profileType);
        }

        /// <summary>
        ///     Gets the default configuration profile for the current application
        /// </summary>
        public static StereoApplicationConfiguration DefaultConfigurationProfile { get; } =
            new StereoApplicationConfiguration(StereoRegistryProfileType.DefaultProfile);

        /// <summary>
        ///     Gets the currently default profile name.
        /// </summary>
        public static string DefaultProfile
        {
            get => StereoApi.GetDefaultProfile();
        }

        /// <summary>
        ///     Gets the DirectX 10 configuration profile for the current application.
        ///     Use this property if only your application supports multiple DirectX versions.
        ///     Otherwise consider using the <see cref="DefaultConfigurationProfile" /> property.
        /// </summary>
        public static StereoApplicationConfiguration DirectX10ConfigurationProfile { get; } =
            new StereoApplicationConfiguration(StereoRegistryProfileType.DirectX10Profile);

        /// <summary>
        ///     Gets the DirectX 9 configuration profile for the current application.
        ///     Use this property if only your application supports multiple DirectX versions.
        ///     Otherwise consider using the <see cref="DefaultConfigurationProfile" /> property.
        /// </summary>
        public static StereoApplicationConfiguration DirectX9ConfigurationProfile { get; } =
            new StereoApplicationConfiguration(StereoRegistryProfileType.DirectX9Profile);

        /// <summary>
        ///     Gets a boolean value indicating if the stereo mode is enable in the registry.
        /// </summary>
        public static bool IsStereoEnable
        {
            get => StereoApi.IsStereoEnabled();
        }

        /// <summary>
        ///     Gets a boolean value indicating if the windowed mode stereo is supported
        /// </summary>
        public static bool IsWindowedModeSupported
        {
            get => StereoApi.IsWindowedModeSupported();
        }

        /// <summary>
        ///     Gets the stereo registry profile type associated with this instance.
        /// </summary>
        public StereoRegistryProfileType ProfileType { get; }


        /// <summary>
        ///     Disables the stereo mode in the registry. The effect is system wide.
        /// </summary>
        public static void DisableStereo()
        {
            StereoApi.DisableStereo();
        }

        /// <summary>
        ///     Enables the stereo mode in the registry. The effect is system wide.
        /// </summary>
        public static void EnableStereo()
        {
            StereoApi.EnableStereo();
        }

        /// <summary>
        ///     Gets the monitor capabilities for the passed monitor handle.
        /// </summary>
        /// <param name="monitorHandle">The monitor handle represented by a pointer.</param>
        /// <returns>The stereo capabilities of the monitor.</returns>
        public static StereoCapabilitiesV1 GetMonitorCapabilities(IntPtr monitorHandle)
        {
            return StereoApi.GetStereoSupport(monitorHandle);
        }

        /// <summary>
        ///     Sets the default stereo profile used by the driver in case te application has no associated profile.
        ///     For the changes to take effect, this method must be called before creating a D3D device.
        /// </summary>
        /// <param name="profileName"></param>
        public static void SetDefaultProfile(string profileName)
        {
            StereoApi.SetDefaultProfile(profileName);
        }

        /// <summary>
        ///     Sets the 3D stereo driver mode.
        /// </summary>
        /// <param name="driverMode"></param>
        public static void SetDriverMode(StereoDriverMode driverMode)
        {
            StereoApi.SetDriverMode(driverMode);
        }

        /// <summary>
        ///     Deletes the entire profile's registry key and therefore resets all customized values.
        /// </summary>
        public void DeleteAllValues()
        {
            StereoApi.DeleteConfigurationProfileRegistryKey(ProfileType);
            StereoApi.CreateConfigurationProfileRegistryKey(ProfileType);
        }

        /// <summary>
        ///     Removes the given value from the profile's registry key.
        /// </summary>
        /// <param name="valueId"></param>
        public void DeleteValue(StereoRegistryIdentification valueId)
        {
            StereoApi.DeleteConfigurationProfileValue(ProfileType, valueId);
        }

        /// <summary>
        ///     Sets the given value under the profile's registry key.
        /// </summary>
        /// <param name="valueId">The identification of the value to be set.</param>
        /// <param name="value">The actual value being set.</param>
        public void SetValue(StereoRegistryIdentification valueId, float value)
        {
            StereoApi.SetConfigurationProfileValue(ProfileType, valueId, value);
        }

        /// <summary>
        ///     Sets the given value under the profile's registry key.
        /// </summary>
        /// <param name="valueId">The identification of the value to be set.</param>
        /// <param name="value">The actual value being set.</param>
        public void SetValue(StereoRegistryIdentification valueId, int value)
        {
            StereoApi.SetConfigurationProfileValue(ProfileType, valueId, value);
        }
    }
}