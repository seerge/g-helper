using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.Display;
using NvAPIWrapper.Native.Display.Structures;
using NvAPIWrapper.Native.General;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.GPU.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Helpers.Structures;
using Rectangle = NvAPIWrapper.Native.General.Structures.Rectangle;

// ReSharper disable InconsistentNaming

namespace NvAPIWrapper.Native.Delegates
{
    internal static class Display
    {
        [FunctionId(FunctionId.NvAPI_CreateDisplayFromUnAttachedDisplay)]
        public delegate Status NvAPI_CreateDisplayFromUnAttachedDisplay(
            [In] UnAttachedDisplayHandle display,
            [Out] out DisplayHandle newDisplay
        );

        [FunctionId(FunctionId.NvAPI_Disp_ColorControl)]
        public delegate Status NvAPI_Disp_ColorControl(
            [In] uint displayId,
            [In]
            [Out]
            [Accepts(
                typeof(ColorDataV5),
                typeof(ColorDataV4),
                typeof(ColorDataV3),
                typeof(ColorDataV2),
                typeof(ColorDataV1)
            )]
            ValueTypeReference colorData
        );

        [FunctionId(FunctionId.NvAPI_DISP_DeleteCustomDisplay)]
        public delegate Status NvAPI_DISP_DeleteCustomDisplay(
            [In] [Accepts(typeof(uint))] ValueTypeArray displayIds,
            [In] uint count,
            [In] [Accepts(typeof(CustomDisplay))] ValueTypeReference customDisplay
        );

        [FunctionId(FunctionId.NvAPI_DISP_EnumCustomDisplay)]
        public delegate Status NvAPI_DISP_EnumCustomDisplay(
            [In] uint displayId,
            [In] uint index,
            [In] [Accepts(typeof(CustomDisplay))] ValueTypeReference customDisplay
        );

        [FunctionId(FunctionId.NvAPI_DISP_GetAssociatedUnAttachedNvidiaDisplayHandle)]
        public delegate Status NvAPI_DISP_GetAssociatedUnAttachedNvidiaDisplayHandle(
            [In] [MarshalAs(UnmanagedType.LPStr)] string displayName,
            [Out] out UnAttachedDisplayHandle display
        );

        [FunctionId(FunctionId.NvAPI_DISP_GetDisplayConfig)]
        public delegate Status NvAPI_DISP_GetDisplayConfig(
            [In] [Out] ref uint pathInfoCount,
            [In] [Accepts(typeof(PathInfoV2), typeof(PathInfoV1))]
            ValueTypeArray pathInfos
        );

        [FunctionId(FunctionId.NvAPI_DISP_GetDisplayIdByDisplayName)]
        public delegate Status NvAPI_DISP_GetDisplayIdByDisplayName([In] string displayName, [Out] out uint displayId);

        [FunctionId(FunctionId.NvAPI_DISP_GetGDIPrimaryDisplayId)]
        public delegate Status NvAPI_DISP_GetGDIPrimaryDisplayId([Out] out uint displayId);

        [FunctionId(FunctionId.NvAPI_Disp_GetHdrCapabilities)]
        public delegate Status NvAPI_Disp_GetHdrCapabilities(
            [In] uint displayId,
            [In] [Out] [Accepts(typeof(HDRCapabilitiesV1))]
            ValueTypeReference hdrCapabilities
        );

        [FunctionId(FunctionId.NvAPI_DISP_GetMonitorCapabilities)]
        public delegate Status NvAPI_DISP_GetMonitorCapabilities(
            [In] uint displayId,
            [In] [Accepts(typeof(MonitorCapabilities))]
            ValueTypeReference capabilities
        );

        [FunctionId(FunctionId.NvAPI_DISP_GetMonitorColorCapabilities)]
        public delegate Status NvAPI_DISP_GetMonitorColorCapabilities(
            [In] uint displayId,
            [In] [Accepts(typeof(MonitorColorData))]
            ValueTypeArray capabilities,
            [In] [Out] ref uint count
        );

        [FunctionId(FunctionId.NvAPI_DISP_GetTiming)]
        public delegate Status NvAPI_DISP_GetTiming(
            [In] uint displayId,
            [In] [Accepts(typeof(TimingInput))] ValueTypeReference timingInput,
            [In] [Accepts(typeof(Timing))] ValueTypeReference timing
        );

        [FunctionId(FunctionId.NvAPI_Disp_HdrColorControl)]
        public delegate Status NvAPI_Disp_HdrColorControl(
            [In] uint displayId,
            [In] [Out] [Accepts(typeof(HDRColorDataV2), typeof(HDRColorDataV1))]
            ValueTypeReference hdrColorData
        );

        [FunctionId(FunctionId.NvAPI_Disp_InfoFrameControl)]
        public delegate Status NvAPI_Disp_InfoFrameControl(
            [In] uint displayId,
            [In] [Accepts(typeof(InfoFrameData))] ValueTypeReference infoFrameData
        );

        [FunctionId(FunctionId.NvAPI_DISP_RevertCustomDisplayTrial)]
        public delegate Status NvAPI_DISP_RevertCustomDisplayTrial(
            [In] [Accepts(typeof(uint))] ValueTypeArray displayIds,
            [In] uint count
        );

        [FunctionId(FunctionId.NvAPI_DISP_SaveCustomDisplay)]
        public delegate Status NvAPI_DISP_SaveCustomDisplay(
            [In] [Accepts(typeof(uint))] ValueTypeArray displayIds,
            [In] uint count,
            [In] uint isThisOutputIdOnly,
            [In] uint isThisMonitorIdOnly
        );

        [FunctionId(FunctionId.NvAPI_DISP_SetDisplayConfig)]
        public delegate Status NvAPI_DISP_SetDisplayConfig(
            [In] uint pathInfoCount,
            [In] [Accepts(typeof(PathInfoV2), typeof(PathInfoV1))]
            ValueTypeArray pathInfos,
            [In] DisplayConfigFlags flags
        );

        [FunctionId(FunctionId.NvAPI_DISP_TryCustomDisplay)]
        public delegate Status NvAPI_DISP_TryCustomDisplay(
            [In] [Accepts(typeof(uint))] ValueTypeArray displayIds,
            [In] uint count,
            [In] [Accepts(typeof(CustomDisplay))] ValueTypeArray customDisplays
        );

        [FunctionId(FunctionId.NvAPI_EnumNvidiaDisplayHandle)]
        public delegate Status NvAPI_EnumNvidiaDisplayHandle(
            [In] uint enumId,
            [Out] out DisplayHandle display
        );

        [FunctionId(FunctionId.NvAPI_EnumNvidiaUnAttachedDisplayHandle)]
        public delegate Status NvAPI_EnumNvidiaUnAttachedDisplayHandle(
            [In] uint enumId,
            [Out] out UnAttachedDisplayHandle display
        );

        [FunctionId(FunctionId.NvAPI_GetAssociatedDisplayOutputId)]
        public delegate Status NvAPI_GetAssociatedDisplayOutputId(
            [In] DisplayHandle display,
            [Out] out OutputId outputId
        );

        [FunctionId(FunctionId.NvAPI_GetAssociatedNvidiaDisplayHandle)]
        public delegate Status NvAPI_GetAssociatedNvidiaDisplayHandle(
            [In] [MarshalAs(UnmanagedType.LPStr)] string displayName,
            [Out] out DisplayHandle display
        );

        [FunctionId(FunctionId.NvAPI_GetAssociatedNvidiaDisplayName)]
        public delegate Status NvAPI_GetAssociatedNvidiaDisplayName(
            [In] DisplayHandle display,
            [Out] out ShortString displayName
        );

        [FunctionId(FunctionId.NvAPI_GetDisplayDriverBuildTitle)]
        public delegate Status NvAPI_GetDisplayDriverBuildTitle(
            [In] DisplayHandle displayHandle,
            [Out] out ShortString name
        );

        [FunctionId(FunctionId.NvAPI_GetDisplayDriverMemoryInfo)]
        public delegate Status NvAPI_GetDisplayDriverMemoryInfo(
            [In] DisplayHandle displayHandle,
            [In]
            [Accepts(
                typeof(DisplayDriverMemoryInfoV3),
                typeof(DisplayDriverMemoryInfoV2),
                typeof(DisplayDriverMemoryInfoV1)
            )]
            ValueTypeReference memoryInfo
        );

        [FunctionId(FunctionId.NvAPI_GetDVCInfo)]
        public delegate Status NvAPI_GetDVCInfo(
            [In] DisplayHandle displayHandle,
            [In] OutputId displayId,
            [In] [Accepts(typeof(PrivateDisplayDVCInfo))]
            ValueTypeReference dvcInfo
        );

        [FunctionId(FunctionId.NvAPI_GetDVCInfoEx)]
        public delegate Status NvAPI_GetDVCInfoEx(
            [In] DisplayHandle displayHandle,
            [In] OutputId displayId,
            [In] [Accepts(typeof(PrivateDisplayDVCInfoEx))]
            ValueTypeReference dvcInfo
        );

        [FunctionId(FunctionId.NvAPI_GetHDMISupportInfo)]
        public delegate Status NvAPI_GetHDMISupportInfo(
            [In] DisplayHandle displayHandle,
            [In] uint displayIdOrOutputId,
            [In] [Accepts(typeof(HDMISupportInfoV2), typeof(HDMISupportInfoV1))]
            ValueTypeReference supportInfo
        );

        [FunctionId(FunctionId.NvAPI_GetHUEInfo)]
        public delegate Status NvAPI_GetHUEInfo(
            [In] DisplayHandle displayHandle,
            [In] OutputId displayId,
            [In] [Accepts(typeof(PrivateDisplayHUEInfo))]
            ValueTypeReference hueInfo
        );

        [FunctionId(FunctionId.NvAPI_GetSupportedViews)]
        public delegate Status NvAPI_GetSupportedViews(
            [In] DisplayHandle display,
            [In] [Accepts(typeof(TargetViewMode))] ValueTypeArray viewModes,
            [Out] [In] ref uint viewCount
        );

        [FunctionId(FunctionId.NvAPI_GetUnAttachedAssociatedDisplayName)]
        public delegate Status NvAPI_GetUnAttachedAssociatedDisplayName(
            [In] UnAttachedDisplayHandle display,
            [Out] out ShortString displayName
        );

        [FunctionId(FunctionId.NvAPI_GPU_GetScanoutCompositionParameter)]
        public delegate Status NvAPI_GPU_GetScanOutCompositionParameter(
            [In] uint displayId,
            [In] ScanOutCompositionParameter parameter,
            [Out] out ScanOutCompositionParameterValue parameterValue,
            [Out] out float container
        );

        [FunctionId(FunctionId.NvAPI_GPU_GetScanoutConfiguration)]
        public delegate Status NvAPI_GPU_GetScanOutConfiguration(
            [In] uint displayId,
            [In] [Accepts(typeof(Rectangle))] ValueTypeReference desktopRectangle,
            [In] [Accepts(typeof(Rectangle))] ValueTypeReference scanOutRectangle
        );

        [FunctionId(FunctionId.NvAPI_GPU_GetScanoutConfigurationEx)]
        public delegate Status NvAPI_GPU_GetScanOutConfigurationEx(
            [In] uint displayId,
            [In] [Accepts(typeof(ScanOutInformationV1))]
            ValueTypeReference scanOutInformation
        );

        [FunctionId(FunctionId.NvAPI_GPU_GetScanoutIntensityState)]
        public delegate Status NvAPI_GPU_GetScanOutIntensityState(
            [In] uint displayId,
            [In] [Accepts(typeof(ScanOutIntensityStateV1))]
            ValueTypeReference scanOutIntensityState
        );

        [FunctionId(FunctionId.NvAPI_GPU_GetScanoutWarpingState)]
        public delegate Status NvAPI_GPU_GetScanOutWarpingState(
            [In] uint displayId,
            [In] [Accepts(typeof(ScanOutWarpingStateV1))]
            ValueTypeReference scanOutWarpingState
        );

        [FunctionId(FunctionId.NvAPI_GPU_SetScanoutCompositionParameter)]
        public delegate Status NvAPI_GPU_SetScanOutCompositionParameter(
            [In] uint displayId,
            [In] ScanOutCompositionParameter parameter,
            [In] ScanOutCompositionParameterValue parameterValue,
            [In] ref float container
        );

        [FunctionId(FunctionId.NvAPI_GPU_SetScanoutIntensity)]
        public delegate Status NvAPI_GPU_SetScanOutIntensity(
            [In] uint displayId,
            [In] [Accepts(typeof(ScanOutIntensityV2), typeof(ScanOutIntensityV1))]
            ValueTypeReference scanOutIntensityData,
            [Out] out int isSticky
        );

        [FunctionId(FunctionId.NvAPI_GPU_SetScanoutWarping)]
        public delegate Status NvAPI_GPU_SetScanOutWarping(
            [In] uint displayId,
            [In] [Accepts(typeof(ScanOutWarpingV1))]
            ValueTypeReference scanOutWarping,
            [In] [Out] ref int maximumNumberOfVertices,
            [Out] out int isSticky
        );

        [FunctionId(FunctionId.NvAPI_SetDVCLevel)]
        public delegate Status NvAPI_SetDVCLevel(
            [In] DisplayHandle displayHandle,
            [In] OutputId displayId,
            [In] int dvcLevel
        );

        [FunctionId(FunctionId.NvAPI_SetDVCLevelEx)]
        public delegate Status NvAPI_SetDVCLevelEx(
            [In] DisplayHandle displayHandle,
            [In] OutputId displayId,
            [In] [Accepts(typeof(PrivateDisplayDVCInfoEx))]
            ValueTypeReference dvcInfo
        );

        [FunctionId(FunctionId.NvAPI_SetHUEAngle)]
        public delegate Status NvAPI_SetHUEAngle(
            [In] DisplayHandle displayHandle,
            [In] OutputId displayId,
            [In] int hueAngle
        );

        [FunctionId(FunctionId.NvAPI_SetRefreshRateOverride)]
        public delegate Status NvAPI_SetRefreshRateOverride(
            [In] DisplayHandle displayHandle,
            [In] OutputId outputMask,
            [In] float refreshRate,
            [In] uint isDeferred
        );
    }
}