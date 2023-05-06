using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.Display.Structures;
using NvAPIWrapper.Native.General;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Helpers.Structures;
using NvAPIWrapper.Native.Stereo;
using NvAPIWrapper.Native.Stereo.Structures;

// ReSharper disable InconsistentNaming
namespace NvAPIWrapper.Native.Delegates
{
    internal static class Stereo
    {
        [FunctionId(FunctionId.NvAPI_D3D1x_CreateSwapChain)]
        public delegate Status NvAPI_D3D1x_CreateSwapChain(
            [In] StereoHandle stereoHandle,
            [In] IntPtr dxgiSwapChainDescription,
            [Out] out IntPtr dxgiSwapChain,
            [In] StereoSwapChainMode mode
        );

        [FunctionId(FunctionId.NvAPI_D3D9_CreateSwapChain)]
        public delegate Status NvAPI_D3D9_CreateSwapChain(
            [In] StereoHandle stereoHandle,
            [In] IntPtr d3dPresentParameters,
            [Out] out IntPtr direct3DSwapChain9,
            [In] StereoSwapChainMode mode
        );

        [FunctionId(FunctionId.NvAPI_Stereo_Activate)]
        public delegate Status NvAPI_Stereo_Activate(
            [In] StereoHandle stereoHandle
        );

        [FunctionId(FunctionId.NvAPI_Stereo_CaptureJpegImage)]
        public delegate Status NvAPI_Stereo_CaptureJpegImage(
            [In] StereoHandle stereoHandle,
            [In] uint quality
        );

        [FunctionId(FunctionId.NvAPI_Stereo_CapturePngImage)]
        public delegate Status NvAPI_Stereo_CapturePngImage(
            [In] StereoHandle stereoHandle
        );
        
        [FunctionId(FunctionId.NvAPI_Stereo_CreateConfigurationProfileRegistryKey)]
        public delegate Status NvAPI_Stereo_CreateConfigurationProfileRegistryKey(
            [In] StereoRegistryProfileType registryProfileType
        );

        [FunctionId(FunctionId.NvAPI_Stereo_CreateHandleFromIUnknown)]
        public delegate Status NvAPI_Stereo_CreateHandleFromIUnknown(
            [In] IntPtr d3dDevice,
            [Out] out StereoHandle stereoHandle
        );

        [FunctionId(FunctionId.NvAPI_Stereo_Deactivate)]
        public delegate Status NvAPI_Stereo_Deactivate(
            [In] StereoHandle stereoHandle
        );
        
        [FunctionId(FunctionId.NvAPI_Stereo_Debug_WasLastDrawStereoized)]
        public delegate Status NvAPI_Stereo_Debug_WasLastDrawStereoized(
            [In] StereoHandle stereoHandle,
            [Out] out byte wasStereo
        );

        [FunctionId(FunctionId.NvAPI_Stereo_DecreaseConvergence)]
        public delegate Status NvAPI_Stereo_DecreaseConvergence(
            [In] StereoHandle stereoHandle
        );

        [FunctionId(FunctionId.NvAPI_Stereo_DecreaseSeparation)]
        public delegate Status NvAPI_Stereo_DecreaseSeparation(
            [In] StereoHandle stereoHandle
        );

        [FunctionId(FunctionId.NvAPI_Stereo_DeleteConfigurationProfileRegistryKey)]
        public delegate Status NvAPI_Stereo_DeleteConfigurationProfileRegistryKey(
            [In] StereoRegistryProfileType registryProfileType
        );

        [FunctionId(FunctionId.NvAPI_Stereo_DeleteConfigurationProfileValue)]
        public delegate Status NvAPI_Stereo_DeleteConfigurationProfileValue(
            [In] StereoRegistryProfileType registryProfileType,
            [In] StereoRegistryIdentification registryId
        );
        
        [FunctionId(FunctionId.NvAPI_Stereo_DestroyHandle)]
        public delegate Status NvAPI_Stereo_DestroyHandle(
            [In] StereoHandle stereoHandle
        );

        [FunctionId(FunctionId.NvAPI_Stereo_Disable)]
        public delegate Status NvAPI_Stereo_Disable();

        [FunctionId(FunctionId.NvAPI_Stereo_Enable)]
        public delegate Status NvAPI_Stereo_Enable();


        [FunctionId(FunctionId.NvAPI_Stereo_GetConvergence)]
        public delegate Status NvAPI_Stereo_GetConvergence(
            [In] StereoHandle stereoHandle,
            [Out] out float convergence
        );

        [FunctionId(FunctionId.NvAPI_Stereo_GetDefaultProfile)]
        public delegate Status NvAPI_Stereo_GetDefaultProfile(
            [In] uint bufferSize,
            [In] IntPtr stringBuffer,
            [Out] out uint stringSize
        );

        [FunctionId(FunctionId.NvAPI_Stereo_GetEyeSeparation)]
        public delegate Status NvAPI_Stereo_GetEyeSeparation(
            [In] StereoHandle stereoHandle,
            [Out] out float separation
        );
        
        [FunctionId(FunctionId.NvAPI_Stereo_GetFrustumAdjustMode)]
        public delegate Status NvAPI_Stereo_GetFrustumAdjustMode(
            [In] StereoHandle stereoHandle,
            [Out] out StereoFrustumAdjustMode frustumAdjustMode
        );

        [FunctionId(FunctionId.NvAPI_Stereo_GetSeparation)]
        public delegate Status NvAPI_Stereo_GetSeparation(
            [In] StereoHandle stereoHandle,
            [Out] out float separationPercentage
        );


        [FunctionId(FunctionId.NvAPI_Stereo_GetStereoSupport)]
        public delegate Status NvAPI_Stereo_GetStereoSupport(
            [In] IntPtr monitorHandle,
            [In] [Accepts(typeof(StereoCapabilitiesV1))]
            ValueTypeReference capabilities
        );

        [FunctionId(FunctionId.NvAPI_Stereo_GetSurfaceCreationMode)]
        public delegate Status NvAPI_Stereo_GetSurfaceCreationMode(
            [In] StereoHandle stereoHandle,
            [Out] out StereoSurfaceCreateMode surfaceCreateMode
        );

        [FunctionId(FunctionId.NvAPI_Stereo_IncreaseConvergence)]
        public delegate Status NvAPI_Stereo_IncreaseConvergence(
            [In] StereoHandle stereoHandle
        );

        [FunctionId(FunctionId.NvAPI_Stereo_IncreaseSeparation)]
        public delegate Status NvAPI_Stereo_IncreaseSeparation(
            [In] StereoHandle stereoHandle
        );

        [FunctionId(FunctionId.NvAPI_Stereo_InitActivation)]
        public delegate Status NvAPI_Stereo_InitActivation(
            [In] StereoHandle stereoHandle,
            [In] StereoActivationFlag flag
        );

        [FunctionId(FunctionId.NvAPI_Stereo_IsActivated)]
        public delegate Status NvAPI_Stereo_IsActivated(
            [In] StereoHandle stereoHandle,
            [Out] out byte isStereoActive
        );

        [FunctionId(FunctionId.NvAPI_Stereo_IsEnabled)]
        public delegate Status NvAPI_Stereo_IsEnabled([Out] out byte isEnable);

        [FunctionId(FunctionId.NvAPI_Stereo_IsWindowedModeSupported)]
        public delegate Status NvAPI_Stereo_IsWindowedModeSupported([Out] out byte isEnable);

        [FunctionId(FunctionId.NvAPI_Stereo_ReverseStereoBlitControl)]
        public delegate Status NvAPI_Stereo_ReverseStereoBlitControl(
            [In] StereoHandle stereoHandle,
            [In] byte turnOn
        );

        [FunctionId(FunctionId.NvAPI_Stereo_SetActiveEye)]
        public delegate Status NvAPI_Stereo_SetActiveEye(
            [In] StereoHandle stereoHandle,
            [In] StereoActiveEye activeEye
        );

        [FunctionId(FunctionId.NvAPI_Stereo_SetConfigurationProfileValue)]
        public delegate Status NvAPI_Stereo_SetConfigurationProfileValueFloat(
            [In] StereoRegistryProfileType registryProfileType,
            [In] StereoRegistryIdentification registryId,
            [In] ref float value
        );

        [FunctionId(FunctionId.NvAPI_Stereo_SetConfigurationProfileValue)]
        public delegate Status NvAPI_Stereo_SetConfigurationProfileValueInteger(
            [In] StereoRegistryProfileType registryProfileType,
            [In] StereoRegistryIdentification registryId,
            [In] ref int value
        );

        [FunctionId(FunctionId.NvAPI_Stereo_SetConvergence)]
        public delegate Status NvAPI_Stereo_SetConvergence(
            [In] StereoHandle stereoHandle,
            [In] float newConvergence
        );

        [FunctionId(FunctionId.NvAPI_Stereo_SetDefaultProfile)]
        public delegate Status NvAPI_Stereo_SetDefaultProfile(
            [In] [MarshalAs(UnmanagedType.LPStr)] string profileName
        );

        [FunctionId(FunctionId.NvAPI_Stereo_SetDriverMode)]
        public delegate Status NvAPI_Stereo_SetDriverMode(
            [In] StereoDriverMode driverMode
        );

        [FunctionId(FunctionId.NvAPI_Stereo_SetFrustumAdjustMode)]
        public delegate Status NvAPI_Stereo_SetFrustumAdjustMode(
            [In] StereoHandle stereoHandle,
            [In] StereoFrustumAdjustMode frustumAdjustMode
        );
        
        [FunctionId(FunctionId.NvAPI_Stereo_SetNotificationMessage)]
        public delegate Status NvAPI_Stereo_SetNotificationMessage(
            [In] StereoHandle stereoHandle,
            [In] ulong windowHandle,
            [In] ulong messageId
        );

        [FunctionId(FunctionId.NvAPI_Stereo_SetSeparation)]
        public delegate Status NvAPI_Stereo_SetSeparation(
            [In] StereoHandle stereoHandle,
            [In] float newSeparationPercentage
        );

        [FunctionId(FunctionId.NvAPI_Stereo_SetSurfaceCreationMode)]
        public delegate Status NvAPI_Stereo_SetSurfaceCreationMode(
            [In] StereoHandle stereoHandle,
            [In] StereoSurfaceCreateMode newSurfaceCreateMode
        );

        [FunctionId(FunctionId.NvAPI_Stereo_Trigger_Activation)]
        public delegate Status NvAPI_Stereo_Trigger_Activation(
            [In] StereoHandle stereoHandle
        );
    }
}