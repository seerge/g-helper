using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.DRS.Structures;
using NvAPIWrapper.Native.General;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Helpers.Structures;

namespace NvAPIWrapper.Native.Delegates
{
    // ReSharper disable InconsistentNaming
    internal static class DRS
    {
        [FunctionId(FunctionId.NvAPI_DRS_CreateApplication)]
        public delegate Status NvAPI_DRS_CreateApplication(
            [In] DRSSessionHandle sessionHandle,
            [In] DRSProfileHandle profileHandle,
            [In]
            [Accepts(
                typeof(DRSApplicationV4),
                typeof(DRSApplicationV3),
                typeof(DRSApplicationV2),
                typeof(DRSApplicationV1)
            )]
            ValueTypeReference application
        );

        [FunctionId(FunctionId.NvAPI_DRS_CreateProfile)]
        public delegate Status NvAPI_DRS_CreateProfile(
            [In] DRSSessionHandle sessionHandle,
            [In] [Accepts(typeof(DRSProfileV1))] ValueTypeReference profile,
            [Out] out DRSProfileHandle profileHandle
        );

        [FunctionId(FunctionId.NvAPI_DRS_CreateSession)]
        public delegate Status NvAPI_DRS_CreateSession([Out] out DRSSessionHandle sessionHandle);

        [FunctionId(FunctionId.NvAPI_DRS_DeleteApplication)]
        public delegate Status NvAPI_DRS_DeleteApplication(
            [In] DRSSessionHandle sessionHandle,
            [In] DRSProfileHandle profileHandle,
            [In] UnicodeString applicationName
        );

        [FunctionId(FunctionId.NvAPI_DRS_DeleteApplicationEx)]
        public delegate Status NvAPI_DRS_DeleteApplicationEx(
            [In] DRSSessionHandle sessionHandle,
            [In] DRSProfileHandle profileHandle,
            [In]
            [Accepts(typeof(DRSApplicationV1), typeof(DRSApplicationV2), typeof(DRSApplicationV3),
                typeof(DRSApplicationV4))]
            ValueTypeReference application
        );

        [FunctionId(FunctionId.NvAPI_DRS_DeleteProfile)]
        public delegate Status NvAPI_DRS_DeleteProfile(
            [In] DRSSessionHandle sessionHandle,
            [In] DRSProfileHandle profileHandle
        );

        [FunctionId(FunctionId.NvAPI_DRS_DeleteProfileSetting)]
        public delegate Status NvAPI_DRS_DeleteProfileSetting(
            [In] DRSSessionHandle sessionHandle,
            [In] DRSProfileHandle profileHandle,
            [In] uint settingId
        );

        [FunctionId(FunctionId.NvAPI_DRS_DestroySession)]
        public delegate Status NvAPI_DRS_DestroySession([In] DRSSessionHandle sessionHandle);

        [FunctionId(FunctionId.NvAPI_DRS_EnumApplications)]
        public delegate Status NvAPI_DRS_EnumApplications(
            [In] DRSSessionHandle sessionHandle,
            [In] DRSProfileHandle profileHandle,
            [In] uint index,
            [In] [Out] ref uint count,
            [In]
            [Accepts(
                typeof(DRSApplicationV4),
                typeof(DRSApplicationV3),
                typeof(DRSApplicationV2),
                typeof(DRSApplicationV1)
            )]
            ValueTypeArray applications
        );

        [FunctionId(FunctionId.NvAPI_DRS_EnumAvailableSettingIds)]
        public delegate Status NvAPI_DRS_EnumAvailableSettingIds(
            [In] [Accepts(typeof(uint))] ValueTypeArray settingIds,
            [In] [Out] ref uint count
        );

        [FunctionId(FunctionId.NvAPI_DRS_EnumAvailableSettingValues)]
        public delegate Status NvAPI_DRS_EnumAvailableSettingValues(
            [In] uint settingId,
            [In] [Out] ref uint count,
            [In] [Out] [Accepts(typeof(DRSSettingValues))]
            ValueTypeReference settingValues
        );

        [FunctionId(FunctionId.NvAPI_DRS_EnumProfiles)]
        public delegate Status NvAPI_DRS_EnumProfiles(
            [In] DRSSessionHandle sessionHandle,
            [In] uint index,
            [Out] out DRSProfileHandle profileHandle
        );

        [FunctionId(FunctionId.NvAPI_DRS_EnumSettings)]
        public delegate Status NvAPI_DRS_EnumSettings(
            [In] DRSSessionHandle sessionHandle,
            [In] DRSProfileHandle profileHandle,
            [In] uint index,
            [In] [Out] ref uint count,
            [In] [Out] [Accepts(typeof(DRSSettingV1))]
            ValueTypeArray settings
        );

        [FunctionId(FunctionId.NvAPI_DRS_FindApplicationByName)]
        public delegate Status NvAPI_DRS_FindApplicationByName(
            [In] DRSSessionHandle sessionHandle,
            [In] UnicodeString applicationName,
            [Out] out DRSProfileHandle profileHandle,
            [In]
            [Accepts(
                typeof(DRSApplicationV4),
                typeof(DRSApplicationV3),
                typeof(DRSApplicationV2),
                typeof(DRSApplicationV1)
            )]
            ValueTypeReference application
        );

        [FunctionId(FunctionId.NvAPI_DRS_FindProfileByName)]
        public delegate Status NvAPI_DRS_FindProfileByName(
            [In] DRSSessionHandle sessionHandle,
            [In] UnicodeString profileName,
            [Out] out DRSProfileHandle profileHandle
        );

        [FunctionId(FunctionId.NvAPI_DRS_GetApplicationInfo)]
        public delegate Status NvAPI_DRS_GetApplicationInfo(
            [In] DRSSessionHandle sessionHandle,
            [In] DRSProfileHandle profileHandle,
            [In] UnicodeString applicationName,
            [In]
            [Accepts(
                typeof(DRSApplicationV4),
                typeof(DRSApplicationV3),
                typeof(DRSApplicationV2),
                typeof(DRSApplicationV1)
            )]
            ValueTypeReference application
        );

        [FunctionId(FunctionId.NvAPI_DRS_GetBaseProfile)]
        public delegate Status NvAPI_DRS_GetBaseProfile(
            [In] DRSSessionHandle sessionHandle,
            [Out] out DRSProfileHandle profileHandle
        );

        [FunctionId(FunctionId.NvAPI_DRS_GetCurrentGlobalProfile)]
        public delegate Status NvAPI_DRS_GetCurrentGlobalProfile(
            [In] DRSSessionHandle sessionHandle,
            [Out] out DRSProfileHandle profileHandle
        );

        [FunctionId(FunctionId.NvAPI_DRS_GetNumProfiles)]
        public delegate Status NvAPI_DRS_GetNumProfiles([In] DRSSessionHandle sessionHandle, [Out] out uint count);

        [FunctionId(FunctionId.NvAPI_DRS_GetProfileInfo)]
        public delegate Status NvAPI_DRS_GetProfileInfo(
            [In] DRSSessionHandle sessionHandle,
            [In] DRSProfileHandle profileHandle,
            [In] [Accepts(typeof(DRSProfileV1))] ValueTypeReference profile
        );

        [FunctionId(FunctionId.NvAPI_DRS_GetSetting)]
        public delegate Status NvAPI_DRS_GetSetting(
            [In] DRSSessionHandle sessionHandle,
            [In] DRSProfileHandle profileHandle,
            [In] uint settingId,
            [Out] [Accepts(typeof(DRSSettingV1))] ValueTypeReference setting
        );

        [FunctionId(FunctionId.NvAPI_DRS_GetSettingIdFromName)]
        public delegate Status NvAPI_DRS_GetSettingIdFromName(
            [In] UnicodeString settingName,
            [Out] out uint settingId
        );

        [FunctionId(FunctionId.NvAPI_DRS_GetSettingNameFromId)]
        public delegate Status NvAPI_DRS_GetSettingNameFromId(
            [In] uint settingId,
            [Out] out UnicodeString settingName
        );

        [FunctionId(FunctionId.NvAPI_DRS_LoadSettings)]
        public delegate Status NvAPI_DRS_LoadSettings([In] DRSSessionHandle sessionHandle);

        [FunctionId(FunctionId.NvAPI_DRS_LoadSettingsFromFile)]
        public delegate Status NvAPI_DRS_LoadSettingsFromFile(
            [In] DRSSessionHandle sessionHandle,
            [In] UnicodeString fileName
        );


        [FunctionId(FunctionId.NvAPI_DRS_RestoreAllDefaults)]
        public delegate Status NvAPI_DRS_RestoreAllDefaults(
            [In] DRSSessionHandle sessionHandle
        );


        [FunctionId(FunctionId.NvAPI_DRS_RestoreProfileDefault)]
        public delegate Status NvAPI_DRS_RestoreProfileDefault(
            [In] DRSSessionHandle sessionHandle,
            [In] DRSProfileHandle profileHandle
        );

        [FunctionId(FunctionId.NvAPI_DRS_RestoreProfileDefaultSetting)]
        public delegate Status NvAPI_DRS_RestoreProfileDefaultSetting(
            [In] DRSSessionHandle sessionHandle,
            [In] DRSProfileHandle profileHandle,
            [In] uint settingId
        );

        [FunctionId(FunctionId.NvAPI_DRS_SaveSettings)]
        public delegate Status NvAPI_DRS_SaveSettings([In] DRSSessionHandle sessionHandle);

        [FunctionId(FunctionId.NvAPI_DRS_SaveSettingsToFile)]
        public delegate Status NvAPI_DRS_SaveSettingsToFile(
            [In] DRSSessionHandle sessionHandle,
            [In] UnicodeString fileName
        );

        [FunctionId(FunctionId.NvAPI_DRS_SetCurrentGlobalProfile)]
        public delegate Status NvAPI_DRS_SetCurrentGlobalProfile(
            [In] DRSSessionHandle sessionHandle,
            [In] UnicodeString profileName
        );

        [FunctionId(FunctionId.NvAPI_DRS_SetProfileInfo)]
        public delegate Status NvAPI_DRS_SetProfileInfo(
            [In] DRSSessionHandle sessionHandle,
            [In] DRSProfileHandle profileHandle,
            [In] [Accepts(typeof(DRSProfileV1))] ValueTypeReference profile
        );

        [FunctionId(FunctionId.NvAPI_DRS_SetSetting)]
        public delegate Status NvAPI_DRS_SetSetting(
            [In] DRSSessionHandle sessionHandle,
            [In] DRSProfileHandle profileHandle,
            [In] [Accepts(typeof(DRSSettingV1))] ValueTypeReference setting
        );
    }
}