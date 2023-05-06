using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Exceptions;
using NvAPIWrapper.Native.General;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Helpers.Structures;
using NvAPIWrapper.Native.Stereo;
using NvAPIWrapper.Native.Stereo.Structures;

namespace NvAPIWrapper.Native
{
    /// <summary>
    ///     Contains Stereo static functions
    /// </summary>
    // ReSharper disable once ClassTooBig
    public static class StereoApi
    {
        /// <summary>
        ///     This API activates stereo for the device interface corresponding to the given stereo handle.
        ///     Activating stereo is possible only if stereo was enabled previously in the registry.
        ///     If stereo is not activated, then calls to functions that require that stereo is activated have no effect,
        ///     and will return the appropriate error code.
        /// </summary>
        /// <param name="handle">Stereo handle corresponding to the device interface.</param>
        public static void ActivateStereo(StereoHandle handle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_Activate>()(
                handle
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API captures the current stereo image in JPEG stereo format with the given quality.
        ///     Only the last capture call per flip will be effective.
        /// </summary>
        /// <param name="handle">Stereo handle that corresponds to the device interface.</param>
        /// <param name="quality">Quality of the JPEG image to be captured. Integer value between 0 and 100.</param>
        public static void CaptureJpegImage(StereoHandle handle, uint quality)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_CaptureJpegImage>()(
                handle,
                quality
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API captures the current stereo image in PNG stereo format.
        ///     Only the last capture call per flip will be effective.
        /// </summary>
        /// <param name="handle">Stereo handle that corresponds to the device interface.</param>
        public static void CapturePngImage(StereoHandle handle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_CapturePngImage>()(
                handle
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     Creates new configuration registry key for current application.
        ///     If there is no configuration profile prior to the function call,
        ///     this API tries to create a new configuration profile registry key
        ///     for a given application and fill it with the default values.
        ///     If an application already has a configuration profile registry key, the API does nothing.
        ///     The name of the key is automatically set to the name of the executable that calls this function.
        ///     Because of this, the executable should have a distinct and unique name.
        ///     If the application is using only one version of DirectX, then the default profile type will be appropriate.
        ///     If the application is using more than one version of DirectX from the same executable,
        ///     it should use the appropriate profile type for each configuration profile.
        /// </summary>
        /// <param name="registryProfileType">Type of profile the application wants to create.</param>
        public static void CreateConfigurationProfileRegistryKey(
            StereoRegistryProfileType registryProfileType)
        {
            var status = DelegateFactory
                .GetDelegate<Delegates.Stereo.NvAPI_Stereo_CreateConfigurationProfileRegistryKey>()(
                    registryProfileType
                );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API creates a stereo handle that is used in subsequent calls related to a given device interface.
        ///     This must be called before any other NvAPI_Stereo_ function for that handle.
        ///     Multiple devices can be used at one time using multiple calls to this function (one per each device).
        ///     HOW TO USE: After the Direct3D device is created, create the stereo handle.
        ///     On call success:
        ///     -# Use all other functions that have stereo handle as first parameter.
        ///     -# After the device interface that corresponds to the the stereo handle is destroyed,
        ///     the application should call NvAPI_DestroyStereoHandle() for that stereo handle.
        /// </summary>
        /// <param name="d3dDevice">Pointer to IUnknown interface that is IDirect3DDevice9* in DX9, ID3D10Device*.</param>
        /// <returns>Newly created stereo handle.</returns>
        // ReSharper disable once InconsistentNaming
        public static StereoHandle CreateHandleFromIUnknown(IntPtr d3dDevice)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_CreateHandleFromIUnknown>()(
                d3dDevice,
                out var stereoHandle
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return stereoHandle;
        }

        // ReSharper disable once CommentTypo
        /// <summary>
        ///     This API allows the user to create a mono or a stereo swap chain.
        ///     NOTE: NvAPI_D3D1x_CreateSwapChain is a wrapper of the method IDXGIFactory::CreateSwapChain which
        ///     additionally notifies the D3D driver of the mode in which the swap chain is to be
        ///     created.
        /// </summary>
        /// <param name="handle">
        ///     Stereo handle that corresponds to the device interface. The device that will write 2D images to
        ///     the swap chain.
        /// </param>
        /// <param name="dxgiSwapChainDescription">
        ///     A pointer to the swap-chain description (DXGI_SWAP_CHAIN_DESC). This parameter
        ///     cannot be NULL.
        /// </param>
        /// <param name="swapChainMode">The stereo mode fot the swap chain.</param>
        /// <returns>A pointer to the swap chain created.</returns>
        public static IntPtr D3D1XCreateSwapChain(
            StereoHandle handle,
            IntPtr dxgiSwapChainDescription,
            StereoSwapChainMode swapChainMode)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_D3D1x_CreateSwapChain>()(
                handle,
                dxgiSwapChainDescription,
                out var dxgiSwapChain,
                swapChainMode
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return dxgiSwapChain;
        }

        /// <summary>
        ///     This API allows the user to create a mono or a stereo swap chain.
        ///     NOTE: NvAPI_D3D9_CreateSwapChain is a wrapper of the method IDirect3DDevice9::CreateAdditionalSwapChain which
        ///     additionally notifies the D3D driver if the swap chain creation mode must be stereo or mono.
        /// </summary>
        /// <param name="handle">Stereo handle that corresponds to the device interface.</param>
        /// <param name="d3dPresentParameters">A pointer to the swap-chain description (DXGI). This parameter cannot be NULL.</param>
        /// <param name="swapChainMode">The stereo mode for the swap chain.</param>
        /// <returns>A pointer to the swap chain created.</returns>
        public static IntPtr D3D9CreateSwapChain(
            StereoHandle handle,
            // ReSharper disable once InconsistentNaming
            IntPtr d3dPresentParameters,
            StereoSwapChainMode swapChainMode)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_D3D9_CreateSwapChain>()(
                handle,
                d3dPresentParameters,
                out var direct3DSwapChain9,
                swapChainMode
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return direct3DSwapChain9;
        }

        /// <summary>
        ///     This API deactivates stereo for the given device interface.
        ///     If stereo is not activated, then calls to functions that require that stereo is activated have no effect,
        ///     and will return the appropriate error code.
        /// </summary>
        /// <param name="handle">Stereo handle that corresponds to the device interface.</param>
        public static void DeactivateStereo(StereoHandle handle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_Deactivate>()(
                handle
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API decreases convergence for the given device interface (just like the Ctrl+F5 hot-key).
        /// </summary>
        /// <param name="handle">Stereo handle that corresponds to the device interface.</param>
        public static void DecreaseConvergence(StereoHandle handle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_DecreaseConvergence>()(
                handle
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API decreases separation for the given device interface (just like the Ctrl+F3 hot-key).
        /// </summary>
        /// <param name="handle">Stereo handle that corresponds to the device interface.</param>
        public static void DecreaseSeparation(StereoHandle handle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_DecreaseSeparation>()(
                handle
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     Removes configuration registry key for current application.
        ///     If an application already has a configuration profile prior to this function call,
        ///     the function attempts to remove the application's configuration profile registry key from the registry.
        ///     If there is no configuration profile registry key prior to the function call,
        ///     the function does nothing and does not report an error.
        /// </summary>
        /// <param name="registryProfileType">Type of profile that the application wants to delete.</param>
        public static void DeleteConfigurationProfileRegistryKey(
            StereoRegistryProfileType registryProfileType)
        {
            var status = DelegateFactory
                .GetDelegate<Delegates.Stereo.NvAPI_Stereo_DeleteConfigurationProfileRegistryKey>()(
                    registryProfileType
                );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API removes the given value from the application's configuration profile registry key.
        ///     If there is no such value, the function does nothing and does not report an error.
        /// </summary>
        /// <param name="registryProfileType">The type of profile the application wants to access.</param>
        /// <param name="registryId">ID of the value that is being deleted.</param>
        public static void DeleteConfigurationProfileValue(
            StereoRegistryProfileType registryProfileType,
            StereoRegistryIdentification registryId)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_DeleteConfigurationProfileValue>()(
                registryProfileType,
                registryId
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API destroys the stereo handle created with one of the NvAPI_Stereo_CreateHandleFrom() functions.
        ///     This should be called after the device corresponding to the handle has been destroyed.
        /// </summary>
        /// <param name="handle">Stereo handle that is to be destroyed.</param>
        public static void DestroyHandle(StereoHandle handle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_DestroyHandle>()(
                handle
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API disables stereo mode in the registry.
        ///     Calls to this function affect the entire system.
        ///     If stereo is not enabled, then calls to functions that require that stereo is enabled have no effect,
        ///     and will return the appropriate error code.
        /// </summary>
        public static void DisableStereo()
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_Disable>()();

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This APU enables stereo mode in the registry.
        ///     Calls to this function affect the entire system.
        ///     If stereo is not enabled, then calls to functions that require that stereo is enabled have no effect,
        ///     and will return the appropriate error code.
        /// </summary>
        public static void EnableStereo()
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_Enable>()();

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API gets the current convergence value.
        /// </summary>
        /// <param name="handle">Stereo handle that corresponds to the device interface.</param>
        /// <returns>Current convergence value</returns>
        public static float GetConvergence(StereoHandle handle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_GetConvergence>()(
                handle,
                out var convergence
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return convergence;
        }

        /// <summary>
        ///     This API retrieves the current default stereo profile.
        /// </summary>
        /// <returns>Default stereo profile name.</returns>
        public static string GetDefaultProfile()
        {
            var stringCapacity = 256;
            var stringAddress = Marshal.AllocHGlobal(stringCapacity);

            try
            {
                var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_GetDefaultProfile>()(
                    (uint) stringCapacity,
                    stringAddress,
                    out var stringSize
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                if (stringSize == 0)
                {
                    return null;
                }

                return Marshal.PtrToStringAnsi(stringAddress, (int) stringSize);
            }
            finally
            {
                Marshal.FreeHGlobal(stringAddress);
            }
        }

        /// <summary>
        ///     This API returns eye separation as a ratio of [between eye distance]/[physical screen width].
        /// </summary>
        /// <param name="handle">Stereo handle that corresponds to the device interface.</param>
        /// <returns>Eye separation</returns>
        public static float GetEyeSeparation(StereoHandle handle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_GetEyeSeparation>()(
                handle,
                out var eyeSeparation
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return eyeSeparation;
        }

        /// <summary>
        ///     This API gets the current frustum adjust mode value.
        /// </summary>
        /// <param name="handle">Stereo handle that corresponds to the device interface.</param>
        /// <returns>Current frustum value</returns>
        public static StereoFrustumAdjustMode GetFrustumAdjustMode(StereoHandle handle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_GetFrustumAdjustMode>()(
                handle,
                out var frustumAdjustMode
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return frustumAdjustMode;
        }

        /// <summary>
        ///     This API gets current separation value (in percents).
        /// </summary>
        /// <param name="handle">Stereo handle that corresponds to the device interface.</param>
        /// <returns>Current separation percentage</returns>
        public static float GetSeparation(StereoHandle handle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_GetSeparation>()(
                handle,
                out var separationPercentage
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return separationPercentage;
        }

        /// <summary>
        ///     This API checks what kind of stereo support is currently supported on a particular display.
        ///     If the the display is prohibited from showing stereo (e.g. secondary in a multi-mon setup), we will
        ///     return 0 for all stereo modes (full screen exclusive, automatic windowed, persistent windowed).
        ///     Otherwise, we will check which stereo mode is supported. On 120Hz display, this will be what
        ///     the user chooses in control panel. On HDMI 1.4 display, persistent windowed mode is always assumed to be
        ///     supported. Note that this function does not check if the CURRENT RESOLUTION/REFRESH RATE can support
        ///     stereo. For HDMI 1.4, it is the application's responsibility to change the resolution/refresh rate to one that is
        ///     3D compatible. For 120Hz, the driver will ALWAYS force 120Hz anyway.
        /// </summary>
        /// <param name="monitorHandle">Monitor that app is going to run on</param>
        /// <returns>An instance of <see cref="StereoCapabilitiesV1" /> structure.</returns>
        public static StereoCapabilitiesV1 GetStereoSupport(IntPtr monitorHandle)
        {
            var instance = typeof(StereoCapabilitiesV1).Instantiate<StereoCapabilitiesV1>();

            using (var reference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_GetStereoSupport>()(
                    monitorHandle,
                    reference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return reference.ToValueType<StereoCapabilitiesV1>(typeof(StereoCapabilitiesV1));
            }
        }

        /// <summary>
        ///     This API gets surface creation mode for this device interface.
        /// </summary>
        /// <param name="handle">Stereo handle that corresponds to the device interface.</param>
        /// <returns>The current creation mode for this device interface.</returns>
        public static StereoSurfaceCreateMode GetSurfaceCreationMode(StereoHandle handle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_GetSurfaceCreationMode>()(
                handle,
                out var surfaceCreateMode
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return surfaceCreateMode;
        }

        /// <summary>
        ///     This API increases convergence for given the device interface (just like the Ctrl+F6 hot-key).
        /// </summary>
        /// <param name="handle">Stereo handle that corresponds to the device interface.</param>
        public static void IncreaseConvergence(StereoHandle handle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_IncreaseConvergence>()(
                handle
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API increases separation for the given device interface (just like the Ctrl+F4 hot-key).
        /// </summary>
        /// <param name="handle">Stereo handle that corresponds to the device interface.</param>
        public static void IncreaseSeparation(StereoHandle handle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_IncreaseSeparation>()(
                handle
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API allows an application to enable stereo viewing, without the need of a GUID/Key pair
        ///     This API cannot be used to enable stereo viewing on 3DTV.
        ///     HOW TO USE:    Call this function immediately after device creation, then follow with a reset. \n
        ///     Very generically:
        ///     Create Device->Create Stereo Handle->InitActivation->Reset Device
        /// </summary>
        /// <param name="handle">Stereo handle corresponding to the device interface.</param>
        /// <param name="activationFlag">Flags to enable or disable delayed activation.</param>
        public static void InitActivation(StereoHandle handle, StereoActivationFlag activationFlag)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_InitActivation>()(
                handle,
                activationFlag
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API checks if stereo is activated for the given device interface.
        /// </summary>
        /// <param name="handle">Stereo handle that corresponds to the device interface.</param>
        /// <returns>Address where result of the inquiry will be placed.</returns>
        public static bool IsStereoActivated(StereoHandle handle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_IsActivated>()(
                handle,
                out var isStereoActive
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return isStereoActive > 0;
        }

        /// <summary>
        ///     This API checks if stereo mode is enabled in the registry.
        /// </summary>
        /// <returns>true if the stereo is enable; otherwise false</returns>
        public static bool IsStereoEnabled()
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_IsEnabled>()(
                out var isEnable
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return isEnable > 0;
        }

        /// <summary>
        ///     This API returns availability of windowed mode stereo
        /// </summary>
        /// <returns>true if windowed mode is supported; otherwise false</returns>
        public static bool IsWindowedModeSupported()
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_IsWindowedModeSupported>()(
                out var supported
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return supported > 0;
        }

        /// <summary>
        ///     This API turns on/off reverse stereo blit.
        ///     After reversed stereo blit control is turned on, blits from the stereo surface will
        ///     produce the right-eye image in the left side of the destination surface and the left-eye
        ///     image in the right side of the destination surface.
        ///     In DirectX 9, the destination surface must be created as the render target, and StretchRect must be used.
        ///     Conditions:
        ///     - DstWidth == 2*SrcWidth
        ///     - DstHeight == SrcHeight
        ///     - Src surface is the stereo surface.
        ///     - SrcRect must be {0,0,SrcWidth,SrcHeight}
        ///     - DstRect must be {0,0,DstWidth,DstHeight}
        ///     In DirectX 10, ResourceCopyRegion must be used.
        ///     Conditions:
        ///     - DstWidth == 2*SrcWidth
        ///     - DstHeight == SrcHeight
        ///     - dstX == 0,
        ///     - dstY == 0,
        ///     - dstZ == 0,
        ///     - SrcBox: left=top=front==0; right==SrcWidth; bottom==SrcHeight; back==1;
        /// </summary>
        /// <param name="handle">Stereo handle corresponding to the device interface.</param>
        /// <param name="turnOn">A boolean value to enable or disable blit control</param>
        public static void ReverseStereoBlitControl(StereoHandle handle, bool turnOn)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_ReverseStereoBlitControl>()(
                handle,
                (byte) (turnOn ? 1 : 0)
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API sets the back buffer to left or right in Direct stereo mode.
        /// </summary>
        /// <param name="handle">Stereo handle that corresponds to the device interface.</param>
        /// <param name="activeEye">Defines active eye in Direct stereo mode</param>
        public static void SetActiveEye(StereoHandle handle, StereoActiveEye activeEye)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_SetActiveEye>()(
                handle,
                activeEye
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API sets the given parameter value under the application's registry key.
        ///     If the value does not exist under the application's registry key, the value will be created under the key.
        /// </summary>
        /// <param name="registryProfileType">The type of profile the application wants to access.</param>
        /// <param name="registryId">ID of the value that is being set.</param>
        /// <param name="value">Value that is being set.</param>
        public static void SetConfigurationProfileValue(
            StereoRegistryProfileType registryProfileType,
            StereoRegistryIdentification registryId,
            float value)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_SetConfigurationProfileValueFloat>()(
                registryProfileType,
                registryId,
                ref value
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API sets the given parameter value under the application's registry key.
        ///     If the value does not exist under the application's registry key, the value will be created under the key.
        /// </summary>
        /// <param name="registryProfileType">The type of profile the application wants to access.</param>
        /// <param name="registryId">ID of the value that is being set.</param>
        /// <param name="value">Value that is being set.</param>
        public static void SetConfigurationProfileValue(
            StereoRegistryProfileType registryProfileType,
            StereoRegistryIdentification registryId,
            int value)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_SetConfigurationProfileValueInteger>()(
                    registryProfileType,
                    registryId,
                    ref value
                );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API sets convergence to the given value.
        /// </summary>
        /// <param name="handle">Stereo handle that corresponds to the device interface.</param>
        /// <param name="convergence">New value for convergence.</param>
        public static void SetConvergence(StereoHandle handle, float convergence)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_SetConvergence>()(
                handle,
                convergence
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API defines the stereo profile used by the driver in case the application has no associated profile.
        ///     To take effect, this API must be called before D3D device is created. Calling once a device has been created will
        ///     not affect the current device.
        /// </summary>
        /// <param name="profileName">Default profile name. </param>
        public static void SetDefaultProfile(string profileName)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_SetDefaultProfile>()(
                profileName
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API sets the 3D stereo driver mode: Direct or Automatic
        /// </summary>
        /// <param name="driverMode">Defines the 3D stereo driver mode: Direct or Automatic</param>
        public static void SetDriverMode(StereoDriverMode driverMode)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_SetDriverMode>()(
                driverMode
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API sets the current frustum adjust mode value.
        /// </summary>
        /// <param name="handle">Stereo handle that corresponds to the device interface.</param>
        /// <param name="frustumAdjustMode">New value for frustum adjust mode.</param>
        public static void SetFrustumAdjustMode(
            StereoHandle handle,
            StereoFrustumAdjustMode frustumAdjustMode)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_SetFrustumAdjustMode>()(
                handle,
                frustumAdjustMode
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API checks if the last draw call was stereoized. It is a very expensive to call and should be used for
        ///     debugging purpose *only*.
        /// </summary>
        /// <param name="handle">Stereo handle that corresponds to the device interface.</param>
        /// <returns>true if the last draw was a stereo draw; otherwise false</returns>
        public static bool WasLastDrawStereoizedDebug(StereoHandle handle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_Debug_WasLastDrawStereoized>()(
                handle,
                out var supported
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return supported > 0;
        } // ReSharper disable CommentTypo
        /// <summary>
        ///     This API is a Setup notification message that the stereo driver uses to notify the application
        ///     when the user changes the stereo driver state.
        ///     When the user changes the stereo state (Activated or Deactivated, separation or conversion)
        ///     the stereo driver posts a defined message with the following parameters:
        ///     lParam  is the current conversion. (Actual conversion is *(float*)&amp;lParam )
        ///     wParam == MAKEWPARAM(l, h) where
        ///     - l == 0 if stereo is deactivated
        ///     - l == 1 if stereo is deactivated
        ///     - h is the current separation. (Actual separation is float(h*100.f/0xFFFF)
        ///     Call this API with NULL hWnd to prohibit notification.
        /// </summary>
        /// <param name="handle">Stereo handle corresponding to the device interface.</param>
        /// <param name="windowsHandle">
        ///     Window handle that will be notified when the user changes the stereo driver state. Actual
        ///     handle must be cast to an <see cref="ulong" />.
        /// </param>
        /// <param name="messageId">MessageID of the message that will be posted to window</param>
        public static void SetNotificationMessage(
            StereoHandle handle,
            ulong windowsHandle,
            ulong messageId)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_SetNotificationMessage>()(
                handle,
                windowsHandle,
                messageId
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API sets separation to given percentage.
        /// </summary>
        /// <param name="handle">Stereo handle that corresponds to the device interface.</param>
        /// <param name="separationPercentage">New value for separation percentage.</param>
        public static void SetSeparation(StereoHandle handle, float separationPercentage)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_SetSeparation>()(
                handle,
                separationPercentage
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API sets surface creation mode for this device interface.
        /// </summary>
        /// <param name="handle">Stereo handle that corresponds to the device interface.</param>
        /// <param name="surfaceCreateMode">New surface creation mode for this device interface.</param>
        public static void SetSurfaceCreationMode(
            StereoHandle handle,
            StereoSurfaceCreateMode surfaceCreateMode)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_SetSurfaceCreationMode>()(
                handle,
                surfaceCreateMode
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API allows an application to trigger creation of a stereo desktop,
        ///     in case the creation was stopped on application launch.
        /// </summary>
        /// <param name="handle">Stereo handle that corresponds to the device interface.</param>
        public static void TriggerActivation(StereoHandle handle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Stereo.NvAPI_Stereo_Trigger_Activation>()(
                handle
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }
    }
}