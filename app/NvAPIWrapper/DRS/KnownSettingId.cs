using System.ComponentModel;

namespace NvAPIWrapper.DRS
{
#pragma warning disable 1591
    public enum KnownSettingId : uint
    {
        /// <summary>
        ///     Antialiasing - Line gamma
        /// </summary>
        [Description("Antialiasing - Line gamma")]
        OpenGLAntiAliasingLineGamma = 0x2089BF6C,

        /// <summary>
        ///     Deep color for 3D applications
        /// </summary>
        [Description("Deep color for 3D applications")]
        OpenGLDeepColorScanOut = 0x2097C2F6,

        /// <summary>
        ///     OpenGL default swap interval
        /// </summary>
        [Description("OpenGL default swap interval")]
        OpenGLDefaultSwapInterval = 0x206A6582,

        /// <summary>
        ///     OpenGL default swap interval fraction
        /// </summary>
        [Description("OpenGL default swap interval fraction")]
        OpenGLDefaultSwapIntervalFractional = 0x206C4581,

        /// <summary>
        ///     OpenGL default swap interval sign
        /// </summary>
        [Description("OpenGL default swap interval sign")]
        OpenGLDefaultSwapIntervalSign = 0x20655CFA,

        /// <summary>
        ///     Event Log Severity Threshold
        /// </summary>
        [Description("Event Log Severity Threshold")]
        OpenGLEventLogSeverityThreshold = 0x209DF23E,

        /// <summary>
        ///     Extension String version
        /// </summary>
        [Description("Extension String version")]
        OpenGLExtensionStringVersion = 0x20FF7493,

        /// <summary>
        ///     Buffer-flipping mode
        /// </summary>
        [Description("Buffer-flipping mode")] OpenGLForceBlit = 0x201F619F,

        /// <summary>
        ///     Force Stereo shuttering
        /// </summary>
        [Description("Force Stereo shuttering")]
        OpenGLForceStereo = 0x204D9A0C,

        /// <summary>
        ///     Preferred OpenGL GPU
        /// </summary>
        [Description("Preferred OpenGL GPU")] OpenGLImplicitGPUAffinity = 0x20D0F3E6,

        /// <summary>
        ///     Maximum frames allowed
        /// </summary>
        [Description("Maximum frames allowed")]
        OpenGLMaximumFramesAllowed = 0x208E55E3,

        /// <summary>
        ///     Exported Overlay pixel types
        /// </summary>
        [Description("Exported Overlay pixel types")]
        OpenGLOverlayPixelType = 0x209AE66F,

        /// <summary>
        ///     Enable overlay
        /// </summary>
        [Description("Enable overlay")] OpenGLOverlaySupport = 0x206C28C4,

        /// <summary>
        ///     High level control of the rendering quality on OpenGL
        /// </summary>
        [Description("High level control of the rendering quality on OpenGL")]
        OpenGLQualityEnhancements = 0x20797D6C,

        /// <summary>
        ///     Unified back/depth buffer
        /// </summary>
        [Description("Unified back/depth buffer")]
        OpenGLSingleBackDepthBuffer = 0x20A29055,

        /// <summary>
        ///     Enable NV_gpu_multicast extension
        /// </summary>
        [Description("Enable NV_gpu_multicast extension")]
        OpenGLSLIMulticast = 0x2092D3BE,

        /// <summary>
        ///     Threaded optimization
        /// </summary>
        [Description("Threaded optimization")] OpenGLThreadControl = 0x20C1221E,

        /// <summary>
        ///     Event Log Tmon Severity Threshold
        /// </summary>
        [Description("Event Log Tmon Severity Threshold")]
        OpenGLTMONLevel = 0x202888C1,

        /// <summary>
        ///     Triple buffering
        /// </summary>
        [Description("Triple buffering")] OpenGLTripleBuffer = 0x20FDD1F9,

        /// <summary>
        ///     Antialiasing - Behavior Flags
        /// </summary>
        [Description("Antialiasing - Behavior Flags")]
        AntiAliasingBehaviorFlags = 0x10ECDB82,

        /// <summary>
        ///     Antialiasing - Transparency Multisampling
        /// </summary>
        [Description("Antialiasing - Transparency Multisampling")]
        AntiAliasingModeAlphaToCoverage = 0x10FC2D9C,

        /// <summary>
        ///     Antialiasing - Gamma correction
        /// </summary>
        [Description("Antialiasing - Gamma correction")]
        AntiAliasingModeGammaCorrection = 0x107D639D,

        /// <summary>
        ///     Antialiasing - Setting
        /// </summary>
        [Description("Antialiasing - Setting")]
        AntiAliasingModeMethod = 0x10D773D2,

        /// <summary>
        ///     Antialiasing - Transparency Supersampling
        /// </summary>
        [Description("Antialiasing - Transparency Supersampling")]
        AntiAliasingModeReplay = 0x10D48A85,

        /// <summary>
        ///     Antialiasing - Mode
        /// </summary>
        [Description("Antialiasing - Mode")] AntiAliasingModeSelector = 0x107EFC5B,

        /// <summary>
        ///     Antialiasing - SLI AA
        /// </summary>
        [Description("Antialiasing - SLI AA")] AntiAliasingModeSelectorSLIAntiAliasing = 0x107AFC5B,

        /// <summary>
        ///     Anisotropic filtering setting
        /// </summary>
        [Description("Anisotropic filtering setting")]
        AnisotropicModeLevel = 0x101E61A9,

        /// <summary>
        ///     Anisotropic filtering mode
        /// </summary>
        [Description("Anisotropic filtering mode")]
        AnisotropicModeSelector = 0x10D2BB16,

        /// <summary>
        ///     NVIDIA Predefined Ansel Usage
        /// </summary>
        [Description("NVIDIA Predefined Ansel Usage")]
        AnselAllow = 0x1035DB89,

        /// <summary>
        ///     Enable Ansel
        /// </summary>
        [Description("Enable Ansel")] AnselEnable = 0x1075D972,

        /// <summary>
        ///     Ansel flags for enabled applications
        /// </summary>
        [Description("Ansel flags for enabled applications")]
        AnselWhiteListed = 0x1085DA8A,

        /// <summary>
        ///     Application Profile Notification Popup Timeout
        /// </summary>
        [Description("Application Profile Notification Popup Timeout")]
        ApplicationProfileNotificationTimeOut = 0x104554B6,

        /// <summary>
        ///     Steam Application ID
        /// </summary>
        [Description("Steam Application ID")] ApplicationSteamId = 0x107CDDBC,

        /// <summary>
        ///     Battery Boost
        /// </summary>
        [Description("Battery Boost")] BatteryBoost = 0x10115C89,

        /// <summary>
        ///     Do not display this profile in the Control Panel
        /// </summary>
        [Description("Do not display this profile in the Control Panel")]
        ControlPanelHiddenProfile = 0x106D5CFF,

        /// <summary>
        ///     List of Universal GPU ids
        /// </summary>
        [Description("List of Universal GPU ids")]
        CUDAExcludedGPUs = 0x10354FF8,

        /// <summary>
        ///     Maximum GPU Power
        /// </summary>
        [Description("Maximum GPU Power")] D3DOpenGLGPUMaximumPower = 0x10D1EF29,

        /// <summary>
        ///     Export Performance Counters
        /// </summary>
        [Description("Export Performance Counters")]
        ExportPerformanceCounters = 0x108F0841,

        /// <summary>
        ///     NVIDIA Predefined FXAA Usage
        /// </summary>
        [Description("NVIDIA Predefined FXAA Usage")]
        FXAAAllow = 0x1034CB89,

        /// <summary>
        ///     Enable FXAA
        /// </summary>
        [Description("Enable FXAA")] FXAAEnable = 0x1074C972,

        /// <summary>
        ///     Enable FXAA Indicator
        /// </summary>
        [Description("Enable FXAA Indicator")] FXAAIndicatorEnable = 0x1068FB9C,

        /// <summary>
        ///     SLI indicator
        /// </summary>
        [Description("SLI indicator")] MCSFRShowSplit = 0x10287051,

        /// <summary>
        ///     NVIDIA Quality upscaling
        /// </summary>
        [Description("NVIDIA Quality upscaling")]
        NvidiaQualityUpScaling = 0x10444444,

        /// <summary>
        ///     Maximum AA samples allowed for a given application
        /// </summary>
        [Description("Maximum AA samples allowed for a given application")]
        OptimusMaximumAntiAliasing = 0x10F9DC83,

        /// <summary>
        ///     Display the PhysX indicator
        /// </summary>
        [Description("Display the PhysX indicator")]
        PhysxIndicator = 0x1094F16F,

        /// <summary>
        ///     Power management mode
        /// </summary>
        [Description("Power management mode")] PreferredPerformanceState = 0x1057EB71,

        /// <summary>
        ///     No override of Anisotropic filtering
        /// </summary>
        [Description("No override of Anisotropic filtering")]
        PreventUiAnisotropicOverride = 0x103BCCB5,

        /// <summary>
        ///     Frame Rate Limiter
        /// </summary>
        [Description("Frame Rate Limiter")] PerformanceStateFrameRateLimiter = 0x10834FEE,

        /// <summary>
        ///     Frame Rate Limiter 2 Control
        /// </summary>
        [Description("Frame Rate Limiter 2 Control")]
        PerformanceStateFrameRateLimiter2Control = 0x10834FFF,

        /// <summary>
        ///     Frame Rate Monitor
        /// </summary>
        [Description("Frame Rate Monitor")] PerformanceStateFrameRateLimiterGpsControl = 0x10834F01,

        /// <summary>
        ///     Frame Rate Monitor Control
        /// </summary>
        [Description("Frame Rate Monitor Control")]
        PerformanceStateFrameRateMonitorControl = 0x10834F05,

        /// <summary>
        ///     Maximum resolution allowed for a given application
        /// </summary>
        [Description("Maximum resolution allowed for a given application")]
        ShimMaxResolution = 0x10F9DC82,

        /// <summary>
        ///     Optimus flags for enabled applications
        /// </summary>
        [Description("Optimus flags for enabled applications")]
        ShimMCCOMPAT = 0x10F9DC80,

        /// <summary>
        ///     Enable application for Optimus
        /// </summary>
        [Description("Enable application for Optimus")]
        ShimRenderingMode = 0x10F9DC81,

        /// <summary>
        ///     Shim Rendering Mode Options per application for Optimus
        /// </summary>
        [Description("Shim Rendering Mode Options per application for Optimus")]
        ShimRenderingOptions = 0x10F9DC84,

        /// <summary>
        ///     Number of GPUs to use on SLI rendering mode
        /// </summary>
        [Description("Number of GPUs to use on SLI rendering mode")]
        SLIGPUCount = 0x1033DCD1,

        /// <summary>
        ///     NVIDIA predefined number of GPUs to use on SLI rendering mode
        /// </summary>
        [Description("NVIDIA predefined number of GPUs to use on SLI rendering mode")]
        SLIPredefinedGPUCount = 0x1033DCD2,

        /// <summary>
        ///     NVIDIA predefined number of GPUs to use on SLI rendering mode on DirectX 10
        /// </summary>
        [Description("NVIDIA predefined number of GPUs to use on SLI rendering mode on DirectX 10")]
        SLIPredefinedGPUCountDX10 = 0x1033DCD3,

        /// <summary>
        ///     NVIDIA predefined SLI mode
        /// </summary>
        [Description("NVIDIA predefined SLI mode")]
        SLIPredefinedMode = 0x1033CEC1,

        /// <summary>
        ///     NVIDIA predefined SLI mode on DirectX 10
        /// </summary>
        [Description("NVIDIA predefined SLI mode on DirectX 10")]
        SLIPredefinedModeDX10 = 0x1033CEC2,

        /// <summary>
        ///     SLI rendering mode
        /// </summary>
        [Description("SLI rendering mode")] SLIRenderingMode = 0x1033CED1,

        /// <summary>
        ///     Virtual Reality pre-rendered frames
        /// </summary>
        [Description("Virtual Reality pre-rendered frames")]
        VRPreRenderLimit = 0x10111133,

        /// <summary>
        ///     Toggle the VRR global feature
        /// </summary>
        [Description("Toggle the VRR global feature")]
        VRRFeatureIndicator = 0x1094F157,

        /// <summary>
        ///     Display the VRR Overlay Indicator
        /// </summary>
        [Description("Display the VRR Overlay Indicator")]
        VRROverlayIndicator = 0x1095F16F,

        /// <summary>
        ///     VRR requested state
        /// </summary>
        [Description("VRR requested state")] VRRRequestState = 0x1094F1F7,

        /// <summary>
        ///     G-SYNC
        /// </summary>
        [Description("G-SYNC")] VRRApplicationOverride = 0x10A879CF,

        /// <summary>
        ///     G-SYNC
        /// </summary>
        [Description("G-SYNC")] VRRApplicationOverrideRequestState = 0x10A879AC,

        /// <summary>
        ///     Enable G-SYNC globally
        /// </summary>
        [Description("Enable G-SYNC globally")]
        VRRMode = 0x1194F158,

        /// <summary>
        ///     Flag to control smooth AFR behavior
        /// </summary>
        [Description("Flag to control smooth AFR behavior")]
        VSyncSmoothAFR = 0x101AE763,

        /// <summary>
        ///     Variable refresh Rate
        /// </summary>
        [Description("Variable refresh Rate")] VSyncVRRControl = 0x10A879CE,

        /// <summary>
        ///     Vsync - Behavior Flags
        /// </summary>
        [Description("Vsync - Behavior Flags")]
        VSyncBehaviorFlags = 0x10FDEC23,

        /// <summary>
        ///     Stereo - Swap eyes
        /// </summary>
        [Description("Stereo - Swap eyes")] WKSAPIStereoEyesExchange = 0x11AE435C,

        /// <summary>
        ///     Stereo - Display mode
        /// </summary>
        [Description("Stereo - Display mode")] WKSAPIStereoMode = 0x11E91A61,

        /// <summary>
        ///     Memory Allocation Policy
        /// </summary>
        [Description("Memory Allocation Policy")]
        WKSMemoryAllocationPolicy = 0x11112233,

        /// <summary>
        ///     Stereo - Dongle Support
        /// </summary>
        [Description("Stereo - Dongle Support")]
        WKSStereoDongleSupport = 0x112493BD,

        /// <summary>
        ///     Stereo - Enable
        /// </summary>
        [Description("Stereo - Enable")] WKSStereoSupport = 0x11AA9E99,

        /// <summary>
        ///     Stereo � swap mode
        /// </summary>
        [Description("Stereo � swap mode")] WKSStereoSwapMode = 0x11333333,

        /// <summary>
        ///     Ambient Occlusion
        /// </summary>
        [Description("Ambient Occlusion")] AmbientOcclusionMode = 0x667329,

        /// <summary>
        ///     NVIDIA Predefined Ambient Occlusion Usage
        /// </summary>
        [Description("NVIDIA Predefined Ambient Occlusion Usage")]
        AmbientOcclusionModeActive = 0x664339,

        /// <summary>
        ///     Texture filtering - Driver Controlled LOD Bias
        /// </summary>
        [Description("Texture filtering - Driver Controlled LOD Bias")]
        AutoLODBiasAdjust = 0x638E8F,

        /// <summary>
        ///     Export Performance Counters for DX9 only
        /// </summary>
        [Description("Export Performance Counters for DX9 only")]
        ExportPerformanceCountersDX9Only = 0xB65E72,

        /// <summary>
        ///     ICafe Settings
        /// </summary>
        [Description("ICafe Settings")] ICafeLogoConfig = 0xDB1337,

        /// <summary>
        ///     Texture filtering - LOD Bias
        /// </summary>
        [Description("Texture filtering - LOD Bias")]
        LODBiasAdjust = 0x738E8F,

        /// <summary>
        ///     Enable sample interleaving (MFAA)
        /// </summary>
        [Description("Enable sample interleaving (MFAA)")]
        MaxwellBSampleInterleave = 0x98C1AC,

        /// <summary>
        ///     Maximum pre-rendered frames
        /// </summary>
        [Description("Maximum pre-rendered frames")]
        PreRenderLimit = 0x7BA09E,

        /// <summary>
        ///     Shader Cache
        /// </summary>
        [Description("Shader Cache")] PerformanceStateShaderDiskCache = 0x198FFF,

        /// <summary>
        ///     Texture filtering - Anisotropic sample optimization
        /// </summary>
        [Description("Texture filtering - Anisotropic sample optimization")]
        PerformanceStateTextureFilteringAnisotropicOptimization = 0xE73211,

        /// <summary>
        ///     Texture filtering - Anisotropic filter optimization
        /// </summary>
        [Description("Texture filtering - Anisotropic filter optimization")]
        PerformanceStateTextureFilteringBiLinearInAnisotropic = 0x84CD70,

        /// <summary>
        ///     Texture filtering - Trilinear optimization
        /// </summary>
        [Description("Texture filtering - Trilinear optimization")]
        PerformanceStateTextureFilteringDisableTrilinearSlope = 0x2ECAF2,

        /// <summary>
        ///     Texture filtering - Negative LOD bias
        /// </summary>
        [Description("Texture filtering - Negative LOD bias")]
        PerformanceStateTextureFilteringNoNegativeLODBias = 0x19BB68,

        /// <summary>
        ///     Texture filtering - Quality
        /// </summary>
        [Description("Texture filtering - Quality")]
        QualityEnhancements = 0xCE2691,

        /// <summary>
        ///     Preferred refresh rate
        /// </summary>
        [Description("Preferred refresh rate")]
        RefreshRateOverride = 0x64B541,

        /// <summary>
        ///     PowerThrottle
        /// </summary>
        [Description("PowerThrottle")] SetPowerThrottleForPCIeCompliance = 0xAE785C,

        /// <summary>
        ///     VAB Default Data
        /// </summary>
        [Description("VAB Default Data")] SetVABData = 0xAB8687,

        /// <summary>
        ///     Vertical Sync
        /// </summary>
        [Description("Vertical Sync")] VSyncMode = 0xA879CF,

        /// <summary>
        ///     Vertical Sync Tear Control
        /// </summary>
        [Description("Vertical Sync Tear Control")]
        VSyncTearControl = 0x5A375C,

        InvalidSetting = 0xFFFFFFFF
    }
#pragma warning restore 1591
}