namespace NvAPIWrapper.Native.General
{
    /// <summary>
    ///     NvAPI status codes
    /// </summary>
    public enum Status
    {
        /// <summary>
        ///     Success. Request is completed.
        /// </summary>
        Ok = 0,

        /// <summary>
        ///     Generic error
        /// </summary>
        Error = -1,

        /// <summary>
        ///     NVAPI support library cannot be loaded.
        /// </summary>
        LibraryNotFound = -2,

        /// <summary>
        ///     Not implemented in current driver installation
        /// </summary>
        NoImplementation = -3,

        /// <summary>
        ///     NvAPI_Initialize() has not been called (successfully)
        /// </summary>
        ApiNotInitialized = -4,

        /// <summary>
        ///     Invalid argument
        /// </summary>
        InvalidArgument = -5,

        /// <summary>
        ///     No NVIDIA display driver was found
        /// </summary>
        NvidiaDeviceNotFound = -6,

        /// <summary>
        ///     No more to enumerate
        /// </summary>
        EndEnumeration = -7,

        /// <summary>
        ///     Invalid handle
        /// </summary>
        InvalidHandle = -8,

        /// <summary>
        ///     An argument's structure version is not supported
        /// </summary>
        IncompatibleStructureVersion = -9,

        /// <summary>
        ///     Handle is no longer valid (likely due to GPU or display re-configuration)
        /// </summary>
        HandleInvalidated = -10,

        /// <summary>
        ///     No NVIDIA OpenGL context is current (but needs to be)
        /// </summary>
        OpenGLContextNotCurrent = -11,

        /// <summary>
        ///     An invalid pointer, usually NULL, was passed as a parameter
        /// </summary>
        InvalidPointer = -14,

        /// <summary>
        ///     OpenGL Expert is not supported by the current drivers
        /// </summary>
        NoGLExpert = -12,

        /// <summary>
        ///     OpenGL Expert is supported, but driver instrumentation is currently disabled
        /// </summary>
        InstrumentationDisabled = -13,

        /// <summary>
        ///     Expected a logical GPU handle for one or more parameters
        /// </summary>
        ExpectedLogicalGPUHandle = -100,

        /// <summary>
        ///     Expected a physical GPU handle for one or more parameters
        /// </summary>
        ExpectedPhysicalGPUHandle = -101,

        /// <summary>
        ///     Expected an NV display handle for one or more parameters
        /// </summary>
        ExpectedDisplayHandle = -102,

        /// <summary>
        ///     Used in some commands to indicate that the combination of parameters is not valid
        /// </summary>
        InvalidCombination = -103,

        /// <summary>
        ///     Requested feature not supported in the selected GPU
        /// </summary>
        NotSupported = -104,

        /// <summary>
        ///     NO port Id found for I2C transaction
        /// </summary>
        PortIdNotFound = -105,

        /// <summary>
        ///     Expected an unattached display handle as one of the input param
        /// </summary>
        ExpectedUnattachedDisplayHandle = -106,

        /// <summary>
        ///     Invalid performance level
        /// </summary>
        InvalidPerformanceLevel = -107,

        /// <summary>
        ///     Device is busy, request not fulfilled
        /// </summary>
        DeviceBusy = -108,

        /// <summary>
        ///     NVIDIA persist file is not found
        /// </summary>
        NvPersistFileNotFound = -109,

        /// <summary>
        ///     NVIDIA persist data is not found
        /// </summary>
        PersistDataNotFound = -110,

        /// <summary>
        ///     Expected TV output display
        /// </summary>
        ExpectedTVDisplay = -111,

        /// <summary>
        ///     Expected TV output on D Connector - HDTV_EIAJ4120.
        /// </summary>
        ExpectedTVDisplayOnDConnector = -112,

        /// <summary>
        ///     SLI is not active on this device
        /// </summary>
        NoActiveSLITopology = -113,

        /// <summary>
        ///     Setup of SLI rendering mode is not possible right now
        /// </summary>
        SLIRenderingModeNotAllowed = -114,

        /// <summary>
        ///     Expected digital flat panel
        /// </summary>
        ExpectedDigitalFlatPanel = -115,

        /// <summary>
        ///     Argument exceeds expected size
        /// </summary>
        ArgumentExceedMaxSize = -116,

        /// <summary>
        ///     Inhibit ON due to one of the flags in NV_GPU_DISPLAY_CHANGE_INHIBIT or SLI Active
        /// </summary>
        DeviceSwitchingNotAllowed = -117,

        /// <summary>
        ///     Testing clocks not supported
        /// </summary>
        TestingClocksNotSupported = -118,

        /// <summary>
        ///     The specified underscan config is from an unknown source (e.g. INF)
        /// </summary>
        UnknownUnderScanConfig = -119,

        /// <summary>
        ///     Timeout while reconfiguring GPUs
        /// </summary>
        TimeoutReConfiguringGPUTopology = -120,

        /// <summary>
        ///     Requested data was not found
        /// </summary>
        DataNotFound = -121,

        /// <summary>
        ///     Expected analog display
        /// </summary>
        ExpectedAnalogDisplay = -122,

        /// <summary>
        ///     No SLI video bridge present
        /// </summary>
        NoVideoLink = -123,

        /// <summary>
        ///     NvAPI requires reboot for its settings to take effect
        /// </summary>
        RequiresReboot = -124,

        /// <summary>
        ///     The function is not supported with the current hybrid mode.
        /// </summary>
        InvalidHybridMode = -125,

        /// <summary>
        ///     The target types are not all the same
        /// </summary>
        MixedTargetTypes = -126,

        /// <summary>
        ///     The function is not supported from 32-bit on a 64-bit system
        /// </summary>
        SYSWOW64NotSupported = -127,

        /// <summary>
        ///     There is any implicit GPU topology active. Use NVAPI_SetHybridMode to change topology.
        /// </summary>
        ImplicitSetGPUTopologyChangeNotAllowed = -128,


        /// <summary>
        ///     Prompt the user to close all non-migratable applications.
        /// </summary>
        RequestUserToCloseNonMigratableApps = -129,

        /// <summary>
        ///     Could not allocate sufficient memory to complete the call
        /// </summary>
        OutOfMemory = -130,

        /// <summary>
        ///     The previous operation that is transferring information to or from this surface is incomplete
        /// </summary>
        WasStillDrawing = -131,

        /// <summary>
        ///     The file was not found
        /// </summary>
        FileNotFound = -132,

        /// <summary>
        ///     There are too many unique instances of a particular type of state object
        /// </summary>
        TooManyUniqueStateObjects = -133,


        /// <summary>
        ///     The method call is invalid. For example, a method's parameter may not be a valid pointer
        /// </summary>
        InvalidCall = -134,

        /// <summary>
        ///     d3d10_1.dll can not be loaded
        /// </summary>
        D3D101LibraryNotFound = -135,

        /// <summary>
        ///     Couldn't find the function in loaded DLL library
        /// </summary>
        FunctionNotFound = -136,

        /// <summary>
        ///     Current User is not Administrator
        /// </summary>
        InvalidUserPrivilege = -137,

        /// <summary>
        ///     The handle corresponds to GDIPrimary
        /// </summary>
        ExpectedNonPrimaryDisplayHandle = -138,

        /// <summary>
        ///     Setting PhysX GPU requires that the GPU is compute capable
        /// </summary>
        ExpectedComputeGPUHandle = -139,

        /// <summary>
        ///     Stereo part of NvAPI failed to initialize completely. Check if stereo driver is installed.
        /// </summary>
        StereoNotInitialized = -140,

        /// <summary>
        ///     Access to stereo related registry keys or values failed.
        /// </summary>
        StereoRegistryAccessFailed = -141,

        /// <summary>
        ///     Given registry profile type is not supported.
        /// </summary>
        StereoRegistryProfileTypeNotSupported = -142,

        /// <summary>
        ///     Given registry value is not supported.
        /// </summary>
        StereoRegistryValueNotSupported = -143,

        /// <summary>
        ///     Stereo is not enabled and function needed it to execute completely.
        /// </summary>
        StereoNotEnabled = -144,

        /// <summary>
        ///     Stereo is not turned on and function needed it to execute completely.
        /// </summary>
        StereoNotTurnedOn = -145,

        /// <summary>
        ///     Invalid device interface.
        /// </summary>
        StereoInvalidDeviceInterface = -146,


        /// <summary>
        ///     Separation percentage or JPEG image capture quality out of [0-100] range.
        /// </summary>
        StereoParameterOutOfRange = -147,

        /// <summary>
        ///     Given frustum adjust mode is not supported.
        /// </summary>
        StereoFrustumAdjustModeNotSupported = -148,

        /// <summary>
        ///     The mosaic topology is not possible given the current state of HW
        /// </summary>
        TopologyNotPossible = -149,

        /// <summary>
        ///     An attempt to do a display resolution mode change has failed
        /// </summary>
        ModeChangeFailed = -150,

        /// <summary>
        ///     d3d11.dll/d3d11_beta.dll cannot be loaded.
        /// </summary>
        D3D11LibraryNotFound = -151,

        /// <summary>
        ///     Address outside of valid range.
        /// </summary>
        InvalidAddress = -152,

        /// <summary>
        ///     The pre-allocated string is too small to hold the result.
        /// </summary>
        StringTooSmall = -153,

        /// <summary>
        ///     The input does not match any of the available devices.
        /// </summary>
        MatchingDeviceNotFound = -154,

        /// <summary>
        ///     Driver is running.
        /// </summary>
        DriverRunning = -155,

        /// <summary>
        ///     Driver is not running.
        /// </summary>
        DriverNotRunning = -156,

        /// <summary>
        ///     A driver reload is required to apply these settings.
        /// </summary>
        ErrorDriverReloadRequired = -157,

        /// <summary>
        ///     Intended setting is not allowed.
        /// </summary>
        SetNotAllowed = -158,

        /// <summary>
        ///     Information can't be returned due to "advanced display topology".
        /// </summary>
        AdvancedDisplayTopologyRequired = -159,

        /// <summary>
        ///     Setting is not found.
        /// </summary>
        SettingNotFound = -160,

        /// <summary>
        ///     Setting size is too large.
        /// </summary>
        SettingSizeTooLarge = -161,

        /// <summary>
        ///     There are too many settings for a profile.
        /// </summary>
        TooManySettingsInProfile = -162,

        /// <summary>
        ///     Profile is not found.
        /// </summary>
        ProfileNotFound = -163,

        /// <summary>
        ///     Profile name is duplicated.
        /// </summary>
        ProfileNameInUse = -164,

        /// <summary>
        ///     Profile name is empty.
        /// </summary>
        ProfileNameEmpty = -165,

        /// <summary>
        ///     Application not found in the Profile.
        /// </summary>
        ExecutableNotFound = -166,

        /// <summary>
        ///     Application already exists in the other profile.
        /// </summary>
        ExecutableAlreadyInUse = -167,

        /// <summary>
        ///     Data Type mismatch
        /// </summary>
        DataTypeMismatch = -168,

        /// <summary>
        ///     The profile passed as parameter has been removed and is no longer valid.
        /// </summary>
        ProfileRemoved = -169,

        /// <summary>
        ///     An unregistered resource was passed as a parameter.
        /// </summary>
        UnregisteredResource = -170,

        /// <summary>
        ///     The DisplayId corresponds to a display which is not within the normal outputId range.
        /// </summary>
        IdOutOfRange = -171,

        /// <summary>
        ///     Display topology is not valid so the driver cannot do a mode set on this configuration.
        /// </summary>
        DisplayConfigValidationFailed = -172,

        /// <summary>
        ///     Display Port Multi-Stream topology has been changed.
        /// </summary>
        DPMSTChanged = -173,

        /// <summary>
        ///     Input buffer is insufficient to hold the contents.
        /// </summary>
        InsufficientBuffer = -174,

        /// <summary>
        ///     No access to the caller.
        /// </summary>
        AccessDenied = -175,

        /// <summary>
        ///     The requested action cannot be performed without Mosaic being enabled.
        /// </summary>
        MosaicNotActive = -176,

        /// <summary>
        ///     The surface is relocated away from video memory.
        /// </summary>
        ShareResourceRelocated = -177,

        /// <summary>
        ///     The user should disable DWM before calling NvAPI.
        /// </summary>
        RequestUserToDisableDWM = -178,

        /// <summary>
        ///     D3D device status is "D3DERR_DEVICELOST" or "D3DERR_DEVICENOTRESET" - the user has to reset the device.
        /// </summary>
        D3DDeviceLost = -179,

        /// <summary>
        ///     The requested action cannot be performed in the current state.
        /// </summary>
        InvalidConfiguration = -180,

        /// <summary>
        ///     Call failed as stereo handshake not completed.
        /// </summary>
        StereoHandshakeNotDone = -181,

        /// <summary>
        ///     The path provided was too short to determine the correct NVDRS_APPLICATION
        /// </summary>
        ExecutablePathIsAmbiguous = -182,

        /// <summary>
        ///     Default stereo profile is not currently defined
        /// </summary>
        DefaultStereoProfileIsNotDefined = -183,

        /// <summary>
        ///     Default stereo profile does not exist
        /// </summary>
        DefaultStereoProfileDoesNotExist = -184,

        /// <summary>
        ///     A cluster is already defined with the given configuration.
        /// </summary>
        ClusterAlreadyExists = -185,

        /// <summary>
        ///     The input display id is not that of a multi stream enabled connector or a display device in a multi stream topology
        /// </summary>
        DPMSTDisplayIdExpected = -186,

        /// <summary>
        ///     The input display id is not valid or the monitor associated to it does not support the current operation
        /// </summary>
        InvalidDisplayId = -187,

        /// <summary>
        ///     While playing secure audio stream, stream goes out of sync
        /// </summary>
        StreamIsOutOfSync = -188,

        /// <summary>
        ///     Older audio driver version than required
        /// </summary>
        IncompatibleAudioDriver = -189,

        /// <summary>
        ///     Value already set, setting again not allowed.
        /// </summary>
        ValueAlreadySet = -190,

        /// <summary>
        ///     Requested operation timed out
        /// </summary>
        Timeout = -191,

        /// <summary>
        ///     The requested workstation feature set has incomplete driver internal allocation resources
        /// </summary>
        GPUWorkstationFeatureIncomplete = -192,

        /// <summary>
        ///     Call failed because InitActivation was not called.
        /// </summary>
        StereoInitActivationNotDone = -193,

        /// <summary>
        ///     The requested action cannot be performed without Sync being enabled.
        /// </summary>
        SyncNotActive = -194,

        /// <summary>
        ///     The requested action cannot be performed without Sync Master being enabled.
        /// </summary>
        SyncMasterNotFound = -195,

        /// <summary>
        ///     Invalid displays passed in the NV_GSYNC_DISPLAY pointer.
        /// </summary>
        InvalidSyncTopology = -196,

        /// <summary>
        ///     The specified signing algorithm is not supported. Either an incorrect value was entered or the current installed
        ///     driver/hardware does not support the input value.
        /// </summary>
        ECIDSignAlgoUnsupported = -197,

        /// <summary>
        ///     The encrypted public key verification has failed.
        /// </summary>
        ECIDKeyVerificationFailed = -198,

        /// <summary>
        ///     The device's firmware is out of date.
        /// </summary>
        FirmwareOutOfDate = -199,

        /// <summary>
        ///     The device's firmware is not supported.
        /// </summary>
        FirmwareRevisionNotSupported = -200,

        /// <summary>
        ///     The caller is not authorized to modify the License.
        /// </summary>
        LicenseCallerAuthenticationFailed = -201,

        /// <summary>
        ///     The user tried to use a deferred context without registering the device first
        /// </summary>
        D3DDeviceNotRegistered = -202,

        /// <summary>
        ///     Head or SourceId was not reserved for the VR Display before doing the Mode-Set.
        /// </summary>
        ResourceNotAcquired = -203,

        /// <summary>
        ///     Provided timing is not supported.
        /// </summary>
        TimingNotSupported = -204,

        /// <summary>
        ///     HDCP Encryption Failed for the device. Would be applicable when the device is HDCP Capable.
        /// </summary>
        HDCPEncryptionFailed = -205,

        /// <summary>
        ///     Provided mode is over sink device pclk limitation.
        /// </summary>
        PCLKLimitationFailed = -206,

        /// <summary>
        ///     No connector on GPU found.
        /// </summary>
        NoConnectorFound = -207,

        /// <summary>
        ///     When a non-HDCP capable HMD is connected, we would inform user by this code.
        /// </summary>
        HDCPDisabled = -208,

        /// <summary>
        ///     At least an API is still being called
        /// </summary>
        ApiInUse = -209,

        /// <summary>
        ///     No display found on Nvidia GPU(s).
        /// </summary>
        NVIDIADisplayNotFound = -210
    }
}