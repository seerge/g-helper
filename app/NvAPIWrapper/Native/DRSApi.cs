using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NvAPIWrapper.Native.DRS.Structures;
using NvAPIWrapper.Native.Exceptions;
using NvAPIWrapper.Native.General;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Helpers.Structures;
using NvAPIWrapper.Native.Interfaces.DRS;

namespace NvAPIWrapper.Native
{
    /// <summary>
    ///     Contains driver settings static functions
    /// </summary>
    // ReSharper disable once ClassTooBig
    public static class DRSApi
    {
        /// <summary>
        ///     This API adds an executable name to a profile.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profileHandle">Input profile handle.</param>
        /// <param name="application">Input <see cref="IDRSApplication" /> instance containing the executable name.</param>
        /// <returns>The newly created instance of <see cref="IDRSApplication" />.</returns>
        public static IDRSApplication CreateApplication(
            DRSSessionHandle sessionHandle,
            DRSProfileHandle profileHandle,
            IDRSApplication application)
        {
            using (var applicationReference = ValueTypeReference.FromValueType(application, application.GetType()))
            {
                var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_CreateApplication>()(
                    sessionHandle,
                    profileHandle,
                    applicationReference
                );

                if (status == Status.IncompatibleStructureVersion)
                {
                    throw new NVIDIANotSupportedException("This operation is not supported.");
                }

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return applicationReference.ToValueType<IDRSApplication>(application.GetType());
            }
        }

        /// <summary>
        ///     This API creates an empty profile.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profile">Input to the <see cref="DRSProfileV1" /> instance.</param>
        /// <returns>The newly created profile handle.</returns>
        public static DRSProfileHandle CreateProfile(DRSSessionHandle sessionHandle, DRSProfileV1 profile)
        {
            using (var profileReference = ValueTypeReference.FromValueType(profile))
            {
                var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_CreateProfile>()(
                    sessionHandle,
                    profileReference,
                    out var profileHandle
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return profileHandle;
            }
        }

        /// <summary>
        ///     This API allocates memory and initializes the session.
        /// </summary>
        /// <returns>The newly created session handle.</returns>
        public static DRSSessionHandle CreateSession()
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_CreateSession>()(out var sessionHandle);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return sessionHandle;
        }

        /// <summary>
        ///     This API removes an executable from a profile.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profileHandle">Input profile handle.</param>
        /// <param name="application">Input all the information about the application to be removed.</param>
        public static void DeleteApplication(
            DRSSessionHandle sessionHandle,
            DRSProfileHandle profileHandle,
            IDRSApplication application)
        {
            using (var applicationReference = ValueTypeReference.FromValueType(application, application.GetType()))
            {
                var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_DeleteApplicationEx>()(
                    sessionHandle,
                    profileHandle,
                    applicationReference
                );

                if (status == Status.IncompatibleStructureVersion)
                {
                    throw new NVIDIANotSupportedException("This operation is not supported.");
                }

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// <summary>
        ///     This API removes an executable name from a profile.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profileHandle">Input profile handle.</param>
        /// <param name="applicationName">Input the executable name to be removed.</param>
        public static void DeleteApplication(
            DRSSessionHandle sessionHandle,
            DRSProfileHandle profileHandle,
            string applicationName)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_DeleteApplication>()(
                sessionHandle,
                profileHandle,
                new UnicodeString(applicationName)
            );

            if (status == Status.IncompatibleStructureVersion)
            {
                throw new NVIDIANotSupportedException("This operation is not supported.");
            }

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API deletes a profile or sets it back to a predefined value.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profileHandle">Input profile handle.</param>
        public static void DeleteProfile(DRSSessionHandle sessionHandle, DRSProfileHandle profileHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_DeleteProfile>()(
                sessionHandle,
                profileHandle
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API deletes a setting or sets it back to predefined value.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profileHandle">Input profile handle.</param>
        /// <param name="settingId">Input settingId to be deleted.</param>
        public static void DeleteProfileSetting(
            DRSSessionHandle sessionHandle,
            DRSProfileHandle profileHandle,
            uint settingId)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_DeleteProfileSetting>()(
                sessionHandle,
                profileHandle,
                settingId
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API frees the allocated resources for the session handle.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        public static void DestroySession(DRSSessionHandle sessionHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_DestroySession>()(sessionHandle);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API enumerates all the applications in a given profile.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profileHandle">Input profile handle.</param>
        /// <returns>Instances of <see cref="IDRSApplication" /> with all the attributes filled.</returns>
        [SuppressMessage("ReSharper", "EventExceptionNotDocumented")]
        public static IEnumerable<IDRSApplication> EnumApplications(
            DRSSessionHandle sessionHandle,
            DRSProfileHandle profileHandle)
        {
            var maxApplicationsPerRequest = 8;
            var enumApplications = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_EnumApplications>();

            foreach (var acceptType in enumApplications.Accepts())
            {
                var i = 0u;

                while (true)
                {
                    var instances = acceptType.Instantiate<IDRSApplication>().Repeat(maxApplicationsPerRequest);

                    using (var applicationsReference = ValueTypeArray.FromArray(instances, acceptType))
                    {
                        var count = (uint) instances.Length;
                        var status = enumApplications(
                            sessionHandle,
                            profileHandle,
                            i,
                            ref count,
                            applicationsReference
                        );

                        if (status == Status.IncompatibleStructureVersion)
                        {
                            break;
                        }

                        if (status == Status.EndEnumeration)
                        {
                            yield break;
                        }

                        if (status != Status.Ok)
                        {
                            throw new NVIDIAApiException(status);
                        }

                        foreach (var application in applicationsReference.ToArray<IDRSApplication>(
                            (int) count,
                            acceptType))
                        {
                            yield return application;
                            i++;
                        }

                        if (count < maxApplicationsPerRequest)
                        {
                            yield break;
                        }
                    }
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// <summary>
        ///     This API enumerates all the Ids of all the settings recognized by NVAPI.
        /// </summary>
        /// <returns>An array of <see cref="uint" />s filled with the settings identification numbers of available settings.</returns>
        public static uint[] EnumAvailableSettingIds()
        {
            var settingIdsCount = (uint) ushort.MaxValue;
            var settingIds = 0u.Repeat((int) settingIdsCount);

            using (var settingIdsArray = ValueTypeArray.FromArray(settingIds))
            {
                var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_EnumAvailableSettingIds>()(
                    settingIdsArray,
                    ref settingIdsCount
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return settingIdsArray.ToArray<uint>((int) settingIdsCount);
            }
        }

        /// <summary>
        ///     This API enumerates all available setting values for a given setting.
        /// </summary>
        /// <param name="settingId">Input settingId.</param>
        /// <returns>All available setting values.</returns>
        public static DRSSettingValues EnumAvailableSettingValues(uint settingId)
        {
            var settingValuesCount = (uint) DRSSettingValues.MaximumNumberOfValues;
            var settingValues = typeof(DRSSettingValues).Instantiate<DRSSettingValues>();

            using (var settingValuesReference = ValueTypeReference.FromValueType(settingValues))
            {
                var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_EnumAvailableSettingValues>()(
                    settingId,
                    ref settingValuesCount,
                    settingValuesReference
                );

                if (status == Status.IncompatibleStructureVersion)
                {
                    throw new NVIDIANotSupportedException("This operation is not supported.");
                }

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return settingValuesReference.ToValueType<DRSSettingValues>(typeof(DRSSettingValues));
            }
        }

        /// <summary>
        ///     This API enumerates through all the profiles in the session.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <returns>Instances of <see cref="DRSProfileHandle" /> each representing a profile.</returns>
        public static IEnumerable<DRSProfileHandle> EnumProfiles(DRSSessionHandle sessionHandle)
        {
            var i = 0u;

            while (true)
            {
                var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_EnumProfiles>()(
                    sessionHandle,
                    i,
                    out var profileHandle
                );

                if (status == Status.EndEnumeration)
                {
                    yield break;
                }

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                yield return profileHandle;
                i++;
            }
        }

        /// <summary>
        ///     This API enumerates all the settings of a given profile.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profileHandle">Input profile handle.</param>
        /// <returns>Instances of <see cref="DRSSettingV1" />.</returns>
        [SuppressMessage("ReSharper", "EventExceptionNotDocumented")]
        public static IEnumerable<DRSSettingV1> EnumSettings(
            DRSSessionHandle sessionHandle,
            DRSProfileHandle profileHandle)
        {
            var maxSettingsPerRequest = 32;
            var enumSettings = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_EnumSettings>();

            var i = 0u;

            while (true)
            {
                var instances = typeof(DRSSettingV1).Instantiate<DRSSettingV1>().Repeat(maxSettingsPerRequest);

                using (var applicationsReference = ValueTypeArray.FromArray(instances))
                {
                    var count = (uint) instances.Length;
                    var status = enumSettings(
                        sessionHandle,
                        profileHandle,
                        i,
                        ref count,
                        applicationsReference
                    );

                    if (status == Status.IncompatibleStructureVersion)
                    {
                        throw new NVIDIANotSupportedException("This operation is not supported.");
                    }

                    if (status == Status.EndEnumeration)
                    {
                        yield break;
                    }

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    foreach (var application in applicationsReference.ToArray<DRSSettingV1>(
                        (int) count,
                        typeof(DRSSettingV1))
                    )
                    {
                        yield return application;
                        i++;
                    }

                    if (count < maxSettingsPerRequest)
                    {
                        yield break;
                    }
                }
            }
        }

        /// <summary>
        ///     This API searches the application and the associated profile for the given application name.
        ///     If a fully qualified path is provided, this function will always return the profile
        ///     the driver will apply upon running the application (on the path provided).
        /// </summary>
        /// <param name="sessionHandle">Input to the hSession handle</param>
        /// <param name="applicationName">Input appName. For best results, provide a fully qualified path of the type</param>
        /// <param name="profileHandle">The profile handle of the profile that the found application belongs to.</param>
        /// <returns>An instance of <see cref="IDRSApplication" />.</returns>
        [SuppressMessage("ReSharper", "EventExceptionNotDocumented")]
        public static IDRSApplication FindApplicationByName(
            DRSSessionHandle sessionHandle,
            string applicationName,
            out DRSProfileHandle? profileHandle)
        {
            var findApplicationByName = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_FindApplicationByName>();

            foreach (var acceptType in findApplicationByName.Accepts())
            {
                var instance = acceptType.Instantiate<IDRSApplication>();

                using (var applicationReference = ValueTypeReference.FromValueType(instance, acceptType))
                {
                    var status = findApplicationByName(
                        sessionHandle,
                        new UnicodeString(applicationName),
                        out var applicationProfileHandle,
                        applicationReference
                    );

                    if (status == Status.IncompatibleStructureVersion)
                    {
                        continue;
                    }

                    if (status == Status.ExecutableNotFound)
                    {
                        profileHandle = null;

                        return null;
                    }

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    profileHandle = applicationProfileHandle;

                    return applicationReference.ToValueType<IDRSApplication>(acceptType);
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// <summary>
        ///     This API finds a profile in the current session.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profileName">Input profileName.</param>
        /// <returns>The profile handle.</returns>
        public static DRSProfileHandle FindProfileByName(DRSSessionHandle sessionHandle, string profileName)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_FindProfileByName>()(
                sessionHandle,
                new UnicodeString(profileName),
                out var profileHandle
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return profileHandle;
        }

        /// <summary>
        ///     This API gets information about the given application.  The input application name
        ///     must match exactly what the Profile has stored for the application.
        ///     This function is better used to retrieve application information from a previous
        ///     enumeration.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profileHandle">Input profile handle.</param>
        /// <param name="applicationName">Input application name.</param>
        /// <returns>
        ///     An instance of <see cref="IDRSApplication" /> with all attributes filled if found; otherwise
        ///     <see langword="null" />.
        /// </returns>
        [SuppressMessage("ReSharper", "EventExceptionNotDocumented")]
        public static IDRSApplication GetApplicationInfo(
            DRSSessionHandle sessionHandle,
            DRSProfileHandle profileHandle,
            string applicationName)
        {
            var getApplicationInfo = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_GetApplicationInfo>();

            foreach (var acceptType in getApplicationInfo.Accepts())
            {
                var instance = acceptType.Instantiate<IDRSApplication>();

                using (var applicationReference = ValueTypeReference.FromValueType(instance, acceptType))
                {
                    var status = getApplicationInfo(
                        sessionHandle,
                        profileHandle,
                        new UnicodeString(applicationName),
                        applicationReference
                    );

                    if (status == Status.IncompatibleStructureVersion)
                    {
                        continue;
                    }

                    if (status == Status.ExecutableNotFound)
                    {
                        return null;
                    }

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    return applicationReference.ToValueType<IDRSApplication>(acceptType);
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// <summary>
        ///     Returns the handle to the current global profile.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <returns>Base profile handle.</returns>
        public static DRSProfileHandle GetBaseProfile(DRSSessionHandle sessionHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_GetBaseProfile>()(
                sessionHandle,
                out var profileHandle
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return profileHandle;
        }

        /// <summary>
        ///     This API returns the handle to the current global profile.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <returns>Current global profile handle.</returns>
        public static DRSProfileHandle GetCurrentGlobalProfile(DRSSessionHandle sessionHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_GetCurrentGlobalProfile>()(
                sessionHandle,
                out var profileHandle
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return profileHandle;
        }

        /// <summary>
        ///     This API obtains the number of profiles in the current session object.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <returns>Number of profiles in the current session.</returns>
        public static int GetNumberOfProfiles(DRSSessionHandle sessionHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_GetNumProfiles>()(
                sessionHandle,
                out var profileCount
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return (int) profileCount;
        }

        /// <summary>
        ///     This API gets information about the given profile.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profileHandle">Input profile handle.</param>
        /// <returns>An instance of <see cref="DRSProfileV1" /> with all attributes filled.</returns>
        public static DRSProfileV1 GetProfileInfo(DRSSessionHandle sessionHandle, DRSProfileHandle profileHandle)
        {
            var profile = typeof(DRSProfileV1).Instantiate<DRSProfileV1>();

            using (var profileReference = ValueTypeReference.FromValueType(profile))
            {
                var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_GetProfileInfo>()(
                    sessionHandle,
                    profileHandle,
                    profileReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return profileReference.ToValueType<DRSProfileV1>().GetValueOrDefault();
            }
        }

        /// <summary>
        ///     This API gets information about the given setting.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profileHandle">Input profile handle.</param>
        /// <param name="settingId">Input settingId.</param>
        /// <returns>An instance of <see cref="DRSSettingV1" /> describing the setting if found; otherwise <see langword="null" />.</returns>
        public static DRSSettingV1? GetSetting(
            DRSSessionHandle sessionHandle,
            DRSProfileHandle profileHandle,
            uint settingId)
        {
            var instance = typeof(DRSSettingV1).Instantiate<DRSSettingV1>();

            using (var settingReference = ValueTypeReference.FromValueType(instance, typeof(DRSSettingV1)))
            {
                var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_GetSetting>()(
                    sessionHandle,
                    profileHandle,
                    settingId,
                    settingReference
                );

                if (status == Status.IncompatibleStructureVersion)
                {
                    throw new NVIDIANotSupportedException("This operation is not supported.");
                }

                if (status == Status.SettingNotFound)
                {
                    return null;
                }

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return settingReference.ToValueType<DRSSettingV1>(typeof(DRSSettingV1));
            }
        }

        /// <summary>
        ///     This API gets the binary identification number of a setting given the setting name.
        /// </summary>
        /// <param name="settingName">Input Unicode settingName.</param>
        /// <returns>The corresponding settingId.</returns>
        public static uint GetSettingIdFromName(string settingName)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_GetSettingIdFromName>()(
                new UnicodeString(settingName),
                out var settingId
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return settingId;
        }

        /// <summary>
        ///     This API gets the setting name given the binary identification number.
        /// </summary>
        /// <param name="settingId">Input settingId.</param>
        /// <returns>Corresponding settingName.</returns>
        public static string GetSettingNameFromId(uint settingId)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_GetSettingNameFromId>()(
                settingId,
                out var settingName
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return settingName.Value;
        }

        /// <summary>
        ///     This API loads and parses the settings data.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        public static void LoadSettings(DRSSessionHandle sessionHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_LoadSettings>()(sessionHandle);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API loads settings from the given file path.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle</param>
        /// <param name="fileName">Binary full file path.</param>
        public static void LoadSettings(DRSSessionHandle sessionHandle, string fileName)
        {
            var unicodeFileName = new UnicodeString(fileName);
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_LoadSettingsFromFile>()(
                sessionHandle,
                unicodeFileName
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API restores the whole system to predefined(default) values.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        public static void RestoreDefaults(DRSSessionHandle sessionHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_RestoreAllDefaults>()(
                sessionHandle
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API restores the given profile to predefined(default) values.
        ///     Any and all user specified modifications will be removed.
        ///     If the whole profile was set by the user, the profile will be removed.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profileHandle">Input profile handle.</param>
        public static void RestoreDefaults(
            DRSSessionHandle sessionHandle,
            DRSProfileHandle profileHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_RestoreProfileDefault>()(
                sessionHandle,
                profileHandle
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API restores the given profile setting to predefined(default) values.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profileHandle">Input profile handle.</param>
        /// <param name="settingId">Input settingId.</param>
        public static void RestoreDefaults(
            DRSSessionHandle sessionHandle,
            DRSProfileHandle profileHandle,
            uint settingId)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_RestoreProfileDefaultSetting>()(
                sessionHandle,
                profileHandle,
                settingId
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API saves the settings data to the system.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        public static void SaveSettings(DRSSessionHandle sessionHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_SaveSettings>()(sessionHandle);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API saves settings to the given file path.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="fileName">Binary full file path.</param>
        public static void SaveSettings(DRSSessionHandle sessionHandle, string fileName)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_SaveSettingsToFile>()(
                sessionHandle,
                new UnicodeString(fileName)
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API sets the current global profile in the driver.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profileName">Input the new current global profile name.</param>
        public static void SetCurrentGlobalProfile(DRSSessionHandle sessionHandle, string profileName)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_SetCurrentGlobalProfile>()(
                sessionHandle,
                new UnicodeString(profileName)
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     Specifies flags for a given profile. Currently only the GPUSupport is
        ///     used to update the profile. Neither the name, number of settings or applications
        ///     or other profile information can be changed with this function.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profileHandle">Input profile handle.</param>
        /// <param name="profile">Input the new profile info.</param>
        public static void SetProfileInfo(
            DRSSessionHandle sessionHandle,
            DRSProfileHandle profileHandle,
            DRSProfileV1 profile)
        {
            using (var profileReference = ValueTypeReference.FromValueType(profile))
            {
                var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_SetProfileInfo>()(
                    sessionHandle,
                    profileHandle,
                    profileReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// <summary>
        ///     This API adds/modifies a setting to a profile.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profileHandle">Input profile handle.</param>
        /// <param name="setting">
        ///     An instance of <see cref="DRSSettingV1" /> containing the setting identification number and new
        ///     value for the setting.
        /// </param>
        public static void SetSetting(
            DRSSessionHandle sessionHandle,
            DRSProfileHandle profileHandle,
            DRSSettingV1 setting)
        {
            using (var settingReference = ValueTypeReference.FromValueType(setting, setting.GetType()))
            {
                var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_SetSetting>()(
                    sessionHandle,
                    profileHandle,
                    settingReference
                );

                if (status == Status.IncompatibleStructureVersion)
                {
                    throw new NVIDIANotSupportedException("This operation is not supported.");
                }

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }
    }
}