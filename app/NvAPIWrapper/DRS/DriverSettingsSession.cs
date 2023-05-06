using System;
using System.Collections.Generic;
using System.Linq;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.DRS.Structures;

namespace NvAPIWrapper.DRS
{
    /// <summary>
    ///     Represents a driver settings session. This is the starting point for using DRS set of functionalities.
    /// </summary>
    public class DriverSettingsSession : IDisposable
    {
        internal DriverSettingsSession(DRSSessionHandle handle)
        {
            Handle = handle;
        }

        private DriverSettingsSession() : this(DRSApi.CreateSession())
        {
        }

        /// <summary>
        ///     Gets the base settings profile
        /// </summary>
        public DriverSettingsProfile BaseProfile
        {
            get
            {
                var profileHandle = DRSApi.GetBaseProfile(Handle);

                if (profileHandle.IsNull)
                {
                    return null;
                }

                return new DriverSettingsProfile(profileHandle, this);
            }
        }

        /// <summary>
        ///     Gets the global settings profile
        /// </summary>
        public DriverSettingsProfile CurrentGlobalProfile
        {
            get
            {
                var profileHandle = DRSApi.GetCurrentGlobalProfile(Handle);

                if (profileHandle.IsNull)
                {
                    return null;
                }

                return new DriverSettingsProfile(profileHandle, this);
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (string.IsNullOrEmpty(value.Name))
                {
                    throw new ArgumentException("Profile name can not be empty.", nameof(value));
                }

                DRSApi.SetCurrentGlobalProfile(Handle, value.Name);
            }
        }

        /// <summary>
        ///     Gets the session handle
        /// </summary>
        public DRSSessionHandle Handle { get; }

        /// <summary>
        ///     Gets the number of registered profiles
        /// </summary>
        public int NumberOfProfiles
        {
            get => DRSApi.GetNumberOfProfiles(Handle);
        }

        /// <summary>
        ///     Gets the list of all registered profiles
        /// </summary>
        public IEnumerable<DriverSettingsProfile> Profiles
        {
            get { return DRSApi.EnumProfiles(Handle).Select(handle => new DriverSettingsProfile(handle, this)); }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Creates a new session and load the settings
        /// </summary>
        /// <returns>A new instance of <see cref="DriverSettingsSession" /> representing a session.</returns>
        public static DriverSettingsSession CreateAndLoad()
        {
            var session = new DriverSettingsSession();
            session.Load();

            return session;
        }

        /// <summary>
        ///     Creates a new session and load the settings from a file
        /// </summary>
        /// <param name="fileName">The full path of file to load settings from.</param>
        /// <returns>A new instance of <see cref="DriverSettingsSession" /> representing a session.</returns>
        public static DriverSettingsSession CreateAndLoad(string fileName)
        {
            var session = new DriverSettingsSession();
            session.Load(fileName);

            return session;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Handle} ({NumberOfProfiles} Profiles)";
        }

        /// <summary>
        ///     Finds an application by name. This method is useful when passed a full path of a file as it does return an
        ///     application almost always describing the NVIDIA driver behavior regarding the passed executable file.
        /// </summary>
        /// <param name="applicationName">The name of the application (with extension) or the full path of an executable file.</param>
        /// <returns>An instance of <see cref="ProfileApplication" /> class.</returns>
        public ProfileApplication FindApplication(string applicationName)
        {
            var application = DRSApi.FindApplicationByName(Handle, applicationName, out var profileHandle);

            if (application == null || !profileHandle.HasValue || profileHandle.Value.IsNull)
            {
                return null;
            }

            var profile = new DriverSettingsProfile(profileHandle.Value, this);

            return new ProfileApplication(application, profile);
        }

        /// <summary>
        ///     Finds a profile based on the application named passed. This method is useful when passed a full path of a file as
        ///     it does return a profile almost always describing the NVIDIA driver behavior regarding the passed executable file.
        /// </summary>
        /// <param name="applicationName">The name of the application (with extension) or the full path of an executable file.</param>
        /// <returns>
        ///     An instance of <see cref="DriverSettingsProfile" /> class describing the NVIDIA driver behavior regarding the
        ///     passed executable file.
        /// </returns>
        public DriverSettingsProfile FindApplicationProfile(string applicationName)
        {
            var application = DRSApi.FindApplicationByName(Handle, applicationName, out var profileHandle);

            if (application == null || !profileHandle.HasValue || profileHandle.Value.IsNull)
            {
                return null;
            }

            return new DriverSettingsProfile(profileHandle.Value, this);
        }

        /// <summary>
        ///     Finds a profile based on its name.
        /// </summary>
        /// <param name="profileName">The profile name to search for.</param>
        /// <returns>An instance of <see cref="DriverSettingsProfile" /> class.</returns>
        public DriverSettingsProfile FindProfileByName(string profileName)
        {
            var profileHandle = DRSApi.FindProfileByName(Handle, profileName);

            if (profileHandle.IsNull)
            {
                return null;
            }

            return new DriverSettingsProfile(profileHandle, this);
        }

        /// <summary>
        ///     Resets all settings to default.
        /// </summary>
        public void RestoreDefaults()
        {
            DRSApi.RestoreDefaults(Handle);
        }

        /// <summary>
        ///     Saves the current session settings
        /// </summary>
        public void Save()
        {
            DRSApi.SaveSettings(Handle);
        }

        /// <summary>
        ///     Saves the current session settings to a file
        /// </summary>
        /// <param name="fileName">The full path of file to save settings to.</param>
        public void Save(string fileName)
        {
            DRSApi.SaveSettings(Handle, fileName);
        }

        private void Load()
        {
            DRSApi.LoadSettings(Handle);
        }

        private void Load(string fileName)
        {
            DRSApi.LoadSettings(Handle, fileName);
        }

        private void ReleaseUnmanagedResources()
        {
            DRSApi.DestroySession(Handle);
        }

        /// <inheritdoc />
        ~DriverSettingsSession()
        {
            ReleaseUnmanagedResources();
        }
    }
}