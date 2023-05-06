using System;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.Stereo;
using NvAPIWrapper.Native.Stereo.Structures;

namespace NvAPIWrapper.Stereo
{
    /// <summary>
    ///     Represents an stereo session created for a D3D device by wrapping over a <see cref="StereoHandle" />
    /// </summary>
    public class StereoDeviceSession : IDisposable
    {
        /// <summary>
        ///     Create a new instance of <see cref="StereoDeviceSession" /> directly from a <see cref="StereoHandle" />
        /// </summary>
        /// <param name="handle">The <see cref="StereoHandle" /> to represent.</param>
        public StereoDeviceSession(StereoHandle handle)
        {
            Handle = handle;
        }

        /// <summary>
        ///     Gets and sets the current convergence value
        /// </summary>
        public float Convergence
        {
            get => StereoApi.GetConvergence(Handle);
            set => StereoApi.SetConvergence(Handle, value);
        }

        /// <summary>
        ///     Gets the eye separation as a ratio of [between eye distance]/[physical screen width].
        /// </summary>
        public float EyeSeparation
        {
            get => StereoApi.GetEyeSeparation(Handle);
        }

        /// <summary>
        ///     Gets and sets the current frustum adjust mode value.
        /// </summary>
        public StereoFrustumAdjustMode FrustumAdjustMode
        {
            get => StereoApi.GetFrustumAdjustMode(Handle);
            set => StereoApi.SetFrustumAdjustMode(Handle, value);
        }

        /// <summary>
        ///     Gets the underlying <see cref="StereoHandle" /> represented by this instance of <see cref="StereoDeviceSession" />
        /// </summary>
        public StereoHandle Handle { get; private set; }

        /// <summary>
        ///     Gets a boolean value indicating if the stereo is activated.
        /// </summary>
        public bool IsStereoActivated
        {
            get => StereoApi.IsStereoActivated(Handle);
        }

        /// <summary>
        ///     Gets a boolean value indicating if this instance is valid.
        /// </summary>
        public bool IsValid
        {
            get => !Handle.IsNull;
        }

        /// <summary>
        ///     Gets and sets the current separation value in percentage.
        /// </summary>
        public float Separation
        {
            get => StereoApi.GetSeparation(Handle);
            set => StereoApi.SetSeparation(Handle, value);
        }

        /// <summary>
        ///     Gets and sets the current surface creation mode
        /// </summary>
        public StereoSurfaceCreateMode SurfaceCreationMode
        {
            get => StereoApi.GetSurfaceCreationMode(Handle);
            set => StereoApi.SetSurfaceCreationMode(Handle, value);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Creates a new instance of <see cref="StereoDeviceSession" /> from a D3D Device implementing the IUnknown
        ///     interface.
        /// </summary>
        /// <param name="d3dDevice"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public static StereoDeviceSession CreateFromIUnknownD3DDevice(IntPtr d3dDevice)
        {
            var handle = StereoApi.CreateHandleFromIUnknown(d3dDevice);

            if (handle.IsNull)
            {
                return null;
            }

            return new StereoDeviceSession(handle);
        }

        /// <summary>
        ///     Activates stereo for this device. Activating stereo is only possible if stereo is already enabled in the registry.
        /// </summary>
        public void ActivateStereo()
        {
            StereoApi.ActivateStereo(Handle);
        }

        /// <summary>
        ///     Captures the current stereo image in JPEG stereo format with the given quality. Only the last capture call per flip
        ///     will be effective.
        /// </summary>
        /// <param name="quality">Quality of the JPEG image to be captured. Integer value between 0 and 100.</param>
        public void CaptureJPEGImage(uint quality)
        {
            StereoApi.CaptureJpegImage(Handle, quality);
        }

        /// <summary>
        ///     Captures the current stereo image in PNG stereo format. Only the last capture call per flip will be effective.
        /// </summary>
        public void CapturePNGImage()
        {
            StereoApi.CapturePngImage(Handle);
        }

        // ReSharper disable once CommentTypo
        /// <summary>
        ///     Creates a mono or a stereo swap chain by wrapping the IDXGIFactory::CreateSwapChain method and notifying the device
        ///     with additional information regarding the stereo swap chain mode selected.
        /// </summary>
        /// <param name="dxgiSwapChainDescription">
        ///     A pointer to the swap-chain description (DXGI_SWAP_CHAIN_DESC). This parameter
        ///     cannot be NULL.
        /// </param>
        /// <param name="swapChainMode"></param>
        /// <returns>A pointer to the swap chain created.</returns>
        public IntPtr D3D1XCreateSwapChain(IntPtr dxgiSwapChainDescription, StereoSwapChainMode swapChainMode)
        {
            return StereoApi.D3D1XCreateSwapChain(Handle, dxgiSwapChainDescription, swapChainMode);
        }

        // ReSharper disable once CommentTypo
        /// <summary>
        ///     Creates a mono or a stereo swap chain by wrapping the IDirect3DDevice9::CreateAdditionalSwapChain method and
        ///     notifying the device with additional information regarding the stereo swap chain mode selected.
        /// </summary>
        /// <param name="d3dPresentParameters">A pointer to the swap-chain description (DXGI). This parameter cannot be NULL.</param>
        /// <param name="swapChainMode"></param>
        /// <returns>A pointer to the swap chain created.</returns>
        // ReSharper disable once InconsistentNaming
        public IntPtr D3D9CreateSwapChain(IntPtr d3dPresentParameters, StereoSwapChainMode swapChainMode)
        {
            return StereoApi.D3D9CreateSwapChain(Handle, d3dPresentParameters, swapChainMode);
        }

        /// <summary>
        ///     Deactivates stereo for this device.
        /// </summary>
        public void DeactivateStereo()
        {
            StereoApi.DeactivateStereo(Handle);
        }

        /// <summary>
        ///     Decreases convergence for this device (just like the Ctrl+F5 hot-key).
        /// </summary>
        public void DecreaseConvergence()
        {
            StereoApi.DecreaseConvergence(Handle);
        }

        /// <summary>
        ///     Decreases separation for this device (just like the Ctrl+F3 hot-key).
        /// </summary>
        public void DecreaseSeparation()
        {
            StereoApi.DecreaseSeparation(Handle);
        }

        /// <summary>
        ///     Increases convergence for this device (just like the Ctrl+F6 hot-key).
        /// </summary>
        public void IncreaseConvergence()
        {
            StereoApi.IncreaseConvergence(Handle);
        }

        /// <summary>
        ///     Increases separation for this device (just like the Ctrl+F4 hot-key).
        /// </summary>
        public void IncreaseSeparation()
        {
            StereoApi.IncreaseSeparation(Handle);
        }

        /// <summary>
        ///     This API allows an application to enable stereo viewing, without the need of a GUID/Key pair
        ///     This API cannot be used to enable stereo viewing on 3DTV.
        ///     HOW TO USE:    Call this function immediately after device creation, then follow with a reset. \n
        ///     Very generically:
        ///     Create Device->Create Stereo Handle->InitActivation->Reset Device
        /// </summary>
        /// <param name="activationFlag">Flags to enable or disable delayed activation.</param>
        public void InitActivation(StereoActivationFlag activationFlag = StereoActivationFlag.Immediate)
        {
            StereoApi.InitActivation(Handle, activationFlag);
        }

        /// <summary>
        ///     Turns reverse stereo blit on or off.
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
        /// <param name="turnOn">A boolean value to enable or disable blit control</param>
        public void ReverseStereoBlitControl(bool turnOn)
        {
            StereoApi.ReverseStereoBlitControl(Handle, turnOn);
        }

        /// <summary>
        ///     Sets the back buffer to left or right in direct stereo mode.
        /// </summary>
        /// <param name="activeKey">Defines active eye in Direct stereo mode.</param>
        public void SetActiveEye(StereoActiveEye activeKey)
        {
            StereoApi.SetActiveEye(Handle, activeKey);
        } 
        
        // ReSharper disable CommentTypo
        /// <summary>
        ///     Asks the stereo driver to notify the application with a notification messages
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
        /// <param name="windowsHandle">
        ///     Window handle that will be notified when the user changes the stereo driver state. Actual
        ///     handle must be cast to an <see cref="ulong" />.
        /// </param>
        /// <param name="messageId">MessageID of the message that will be posted to window</param>
        public void SetNotificationMessage(ulong windowsHandle, ulong messageId)
        {
            StereoApi.SetNotificationMessage(Handle, windowsHandle, messageId);
        }

        /// <summary>
        ///     Triggers the creation of a stereo desktop in case the creation was stopped on application launch.
        /// </summary>
        public void TriggerActivation()
        {
            StereoApi.TriggerActivation(Handle);
        }

        /// <summary>
        ///     Checks if the last draw call was stereoized. It is a very expensive to call and should be used for debugging
        ///     purpose *only*.
        /// </summary>
        /// <returns>true if the last draw was stereoized; otherwise false.</returns>
        public bool WasLastDrawStereoizedDebugOnly()
        {
            return StereoApi.WasLastDrawStereoizedDebug(Handle);
        }

        private void ReleaseUnmanagedResources()
        {
            StereoApi.DestroyHandle(Handle);
            Handle = StereoHandle.DefaultHandle;
        }

        /// <inheritdoc />
        ~StereoDeviceSession()
        {
            ReleaseUnmanagedResources();
        }
    }
}