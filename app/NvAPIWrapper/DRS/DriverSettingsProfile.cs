using System;
using System.Collections.Generic;
using System.Linq;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.DRS;
using NvAPIWrapper.Native.DRS.Structures;

namespace NvAPIWrapper.DRS
{
    /// <summary>
    ///     Represents a NVIDIA driver settings profile
    /// </summary>
    public class DriverSettingsProfile
    {
        internal DriverSettingsProfile(DRSProfileHandle handle, DriverSettingsSession parentSession)
        {
            Handle = handle;
            Session = parentSession;
        }

        /// <summary>
        ///     Gets a list of applications under this profile
        /// </summary>
        public IEnumerable<ProfileApplication> Applications
        {
            get
            {
                if (!IsValid)
                {
                    throw new InvalidOperationException(
                        "Can not perform this operation with an invalid profile instance."
                    );
                }

                return DRSApi.EnumApplications(Session.Handle, Handle)
                    .Select(application => new ProfileApplication(application, this));
            }
        }

        /// <summary>
        ///     Gets or sets the profile support value for GPU series
        /// </summary>
        public DRSGPUSupport GPUSupport
        {
            get
            {
                if (!IsValid)
                {
                    throw new InvalidOperationException(
                        "Can not perform this operation with an invalid profile instance."
                    );
                }

                var profileInfo = DRSApi.GetProfileInfo(Session.Handle, Handle);

                return profileInfo.GPUSupport;
            }
            set
            {
                if (!IsValid)
                {
                    throw new InvalidOperationException(
                        "Can not perform this operation with an invalid profile instance."
                    );
                }

                var profileInfo = DRSApi.GetProfileInfo(Session.Handle, Handle);
                profileInfo.GPUSupport = value;
                DRSApi.SetProfileInfo(Session.Handle, Handle, profileInfo);
            }
        }

        /// <summary>
        ///     Gets the profile handle
        /// </summary>
        public DRSProfileHandle Handle { get; private set; }

        /// <summary>
        ///     Gets a boolean value indicating if this profile is predefined
        /// </summary>
        public bool IsPredefined
        {
            get
            {
                if (!IsValid)
                {
                    throw new InvalidOperationException(
                        "Can not perform this operation with an invalid profile instance."
                    );
                }

                var profileInfo = DRSApi.GetProfileInfo(Session.Handle, Handle);

                return profileInfo.IsPredefined;
            }
        }

        /// <summary>
        ///     Gets a boolean value indicating if this profile is valid and contains a non-zero handle
        /// </summary>
        public bool IsValid
        {
            get => !Handle.IsNull;
        }

        /// <summary>
        ///     Gets the name of the profile
        /// </summary>
        public string Name
        {
            get
            {
                if (!IsValid)
                {
                    throw new InvalidOperationException(
                        "Can not perform this operation with an invalid profile instance."
                    );
                }

                var profileInfo = DRSApi.GetProfileInfo(Session.Handle, Handle);

                return profileInfo.Name;
            }
        }

        /// <summary>
        ///     Gets the number of application registered under this profile
        /// </summary>
        public int NumberOfApplications
        {
            get
            {
                if (!IsValid)
                {
                    throw new InvalidOperationException(
                        "Can not perform this operation with an invalid profile instance."
                    );
                }

                var profileInfo = DRSApi.GetProfileInfo(Session.Handle, Handle);

                return profileInfo.NumberOfApplications;
            }
        }

        /// <summary>
        ///     Gets the number of settings under this profile
        /// </summary>
        public int NumberOfSettings
        {
            get
            {
                if (!IsValid)
                {
                    throw new InvalidOperationException(
                        "Can not perform this operation with an invalid profile instance."
                    );
                }

                var profileInfo = DRSApi.GetProfileInfo(Session.Handle, Handle);

                return profileInfo.NumberOfSettings;
            }
        }

        /// <summary>
        ///     Gets the session that had queried this profile
        /// </summary>
        public DriverSettingsSession Session { get; }

        /// <summary>
        ///     Gets a list of settings under this profile
        /// </summary>
        public IEnumerable<ProfileSetting> Settings
        {
            get
            {
                if (!IsValid)
                {
                    throw new InvalidOperationException(
                        "Can not perform this operation with an invalid profile instance."
                    );
                }

                return DRSApi.EnumSettings(Session.Handle, Handle).Select(setting => new ProfileSetting(setting));
            }
        }

        /// <summary>
        ///     Creates a new profile
        /// </summary>
        /// <param name="session">The session to create this profile in.</param>
        /// <param name="profileName">The name of the profile.</param>
        /// <param name="gpuSupport">The supported GPU series for this profile.</param>
        /// <returns>An instance of <see cref="DriverSettingsProfile" /> representing this newly created profile.</returns>
        public static DriverSettingsProfile CreateProfile(
            DriverSettingsSession session,
            string profileName,
            DRSGPUSupport? gpuSupport = null)
        {
            gpuSupport = gpuSupport ?? new DRSGPUSupport();
            var profileInfo = new DRSProfileV1(profileName, gpuSupport.Value);
            var profileHandle = DRSApi.CreateProfile(session.Handle, profileInfo);

            return new DriverSettingsProfile(profileHandle, session);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            if (!IsValid)
            {
                return "[Invalid]";
            }

            if (IsPredefined)
            {
                return $"{Name} (Predefined)";
            }

            return Name;
        }

        /// <summary>
        ///     Deletes this profile and makes this instance invalid.
        /// </summary>
        public void Delete()
        {
            if (!IsValid)
            {
                throw new InvalidOperationException(
                    "Can not perform this operation with an invalid profile instance."
                );
            }

            DRSApi.DeleteProfile(Session.Handle, Handle);
            Handle = DRSProfileHandle.DefaultHandle;
        }

        /// <summary>
        ///     Deletes an application by its name.
        /// </summary>
        /// <param name="applicationName">The name of the application to be deleted.</param>
        public void DeleteApplicationByName(string applicationName)
        {
            if (!IsValid)
            {
                throw new InvalidOperationException(
                    "Can not perform this operation with an invalid profile instance."
                );
            }

            DRSApi.DeleteApplication(Session.Handle, Handle, applicationName);
        }

        /// <summary>
        ///     Deletes a setting by its identification number
        /// </summary>
        /// <param name="settingId">The identification number of the setting to be deleted.</param>
        public void DeleteSetting(uint settingId)
        {
            if (!IsValid)
            {
                throw new InvalidOperationException(
                    "Can not perform this operation with an invalid profile instance."
                );
            }

            DRSApi.DeleteProfileSetting(Session.Handle, Handle, settingId);
        }

        /// <summary>
        ///     Deletes a setting by its known identification number.
        /// </summary>
        /// <param name="settingId">The known identification number of the setting to be deleted.</param>
        public void DeleteSetting(KnownSettingId settingId)
        {
            DeleteSetting(SettingInfo.GetSettingId(settingId));
        }

        /// <summary>
        ///     Finds an application by its name.
        /// </summary>
        /// <param name="applicationName">The name of the application to search for.</param>
        /// <returns>
        ///     An instance of <see cref="ProfileApplication" /> if an application is found; otherwise <see langword="null" />
        ///     .
        /// </returns>
        public ProfileApplication GetApplicationByName(string applicationName)
        {
            if (!IsValid)
            {
                throw new InvalidOperationException(
                    "Can not perform this operation with an invalid profile instance."
                );
            }

            var application = DRSApi.GetApplicationInfo(Session.Handle, Handle, applicationName);

            if (application == null)
            {
                return null;
            }

            return new ProfileApplication(application, this);
        }

        /// <summary>
        ///     Searches for a setting using its identification number.
        /// </summary>
        /// <param name="settingId">The identification number of the setting to search for.</param>
        /// <returns>An instance of <see cref="ProfileSetting" /> if a setting is found; otherwise <see langword="null" />.</returns>
        public ProfileSetting GetSetting(uint settingId)
        {
            if (!IsValid)
            {
                throw new InvalidOperationException(
                    "Can not perform this operation with an invalid profile instance."
                );
            }

            var setting = DRSApi.GetSetting(Session.Handle, Handle, settingId);

            if (setting == null)
            {
                return null;
            }

            return new ProfileSetting(setting.Value);
        }


        /// <summary>
        ///     Searches for a setting using its known identification number.
        /// </summary>
        /// <param name="settingId">The known identification number of the setting to search for.</param>
        /// <returns>An instance of <see cref="ProfileSetting" /> if a setting is found; otherwise <see langword="null" />.</returns>
        public ProfileSetting GetSetting(KnownSettingId settingId)
        {
            return GetSetting(SettingInfo.GetSettingId(settingId));
        }

        /// <summary>
        ///     Restores applications and settings of this profile to their default. This also deletes custom profiles resulting in
        ///     their handles becoming invalid.
        /// </summary>
        public void RestoreDefaults()
        {
            if (!IsValid)
            {
                throw new InvalidOperationException(
                    "Can not perform this operation with an invalid profile instance."
                );
            }

            var isPredefined = IsPredefined;
            DRSApi.RestoreDefaults(Session.Handle, Handle);

            if (!isPredefined)
            {
                Handle = DRSProfileHandle.DefaultHandle;
            }
        }

        /// <summary>
        ///     Restores a setting to its default value.
        /// </summary>
        /// <param name="settingId">The identification number of the setting.</param>
        public void RestoreSettingToDefault(uint settingId)
        {
            if (!IsValid)
            {
                throw new InvalidOperationException(
                    "Can not perform this operation with an invalid profile instance."
                );
            }

            DRSApi.RestoreDefaults(Session.Handle, Handle, settingId);
        }

        /// <summary>
        ///     Restores a setting to its default value.
        /// </summary>
        /// <param name="settingId">The known identification number of the setting.</param>
        public void RestoreSettingToDefault(KnownSettingId settingId)
        {
            RestoreSettingToDefault(SettingInfo.GetSettingId(settingId));
        }

        /// <summary>
        ///     Sets a new value for a setting or creates a new setting and sets its value
        /// </summary>
        /// <param name="settingId">The known identification number of the setting to change its value.</param>
        /// <param name="settingType">The type of the setting value.</param>
        /// <param name="value">The new value for the setting.</param>
        public void SetSetting(KnownSettingId settingId, DRSSettingType settingType, object value)
        {
            SetSetting(SettingInfo.GetSettingId(settingId), settingType, value);
        }

        /// <summary>
        ///     Sets a new value for a setting or creates a new setting and sets its value
        /// </summary>
        /// <param name="settingId">The known identification number of the setting to change its value.</param>
        /// <param name="value">The new value for the setting.</param>
        public void SetSetting(KnownSettingId settingId, string value)
        {
            SetSetting(SettingInfo.GetSettingId(settingId), value);
        }

        /// <summary>
        ///     Sets a new value for a setting or creates a new setting and sets its value
        /// </summary>
        /// <param name="settingId">The known identification number of the setting to change its value.</param>
        /// <param name="value">The new value for the setting.</param>
        public void SetSetting(KnownSettingId settingId, byte[] value)
        {
            SetSetting(SettingInfo.GetSettingId(settingId), value);
        }

        /// <summary>
        ///     Sets a new value for a setting or creates a new setting and sets its value
        /// </summary>
        /// <param name="settingId">The known identification number of the setting to change its value.</param>
        /// <param name="value">The new value for the setting.</param>
        public void SetSetting(KnownSettingId settingId, uint value)
        {
            SetSetting(SettingInfo.GetSettingId(settingId), value);
        }

        /// <summary>
        ///     Sets a new value for a setting or creates a new setting and sets its value
        /// </summary>
        /// <param name="settingId">The identification number of the setting to change its value.</param>
        /// <param name="settingType">The type of the setting value.</param>
        /// <param name="value">The new value for the setting.</param>
        public void SetSetting(uint settingId, DRSSettingType settingType, object value)
        {
            if (!IsValid)
            {
                throw new InvalidOperationException(
                    "Can not perform this operation with an invalid profile instance."
                );
            }

            var setting = new DRSSettingV1(settingId, settingType, value);

            DRSApi.SetSetting(Session.Handle, Handle, setting);
        }

        /// <summary>
        ///     Sets a new value for a setting or creates a new setting and sets its value
        /// </summary>
        /// <param name="settingId">The identification number of the setting to change its value.</param>
        /// <param name="value">The new value for the setting.</param>
        public void SetSetting(uint settingId, string value)
        {
            SetSetting(settingId, DRSSettingType.UnicodeString, value);
        }

        /// <summary>
        ///     Sets a new value for a setting or creates a new setting and sets its value
        /// </summary>
        /// <param name="settingId">The identification number of the setting to change its value.</param>
        /// <param name="value">The new value for the setting.</param>
        public void SetSetting(uint settingId, byte[] value)
        {
            SetSetting(settingId, DRSSettingType.Binary, value);
        }

        /// <summary>
        ///     Sets a new value for a setting or creates a new setting and sets its value
        /// </summary>
        /// <param name="settingId">The identification number of the setting to change its value.</param>
        /// <param name="value">The new value for the setting.</param>
        public void SetSetting(uint settingId, uint value)
        {
            SetSetting(settingId, DRSSettingType.Integer, value);
        }
    }
}