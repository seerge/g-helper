namespace NvAPIWrapper.DRS.SettingValues
{
#pragma warning disable 1591
    public enum ShimRenderingOptions : uint
    {
        DefaultRenderingMode = 0x0,

        DisableAsyncPresent = 0x1,

        EHShellDetect = 0x2,

        FlashplayerHostDetect = 0x4,

        VideoDRMApplicationDetect = 0x8,

        IgnoreOverrides = 0x10,

        Reserved1 = 0x20,

        EnableDWMAsyncPresent = 0x40,

        Reserved2 = 0x80,

        AllowInheritance = 0x100,

        DisableWrappers = 0x200,

        DisableDxgiWrappers = 0x400,

        PruneUnsupportedFormats = 0x800,

        EnableAlphaFormat = 0x1000,

        IGPUTranscoding = 0x2000,

        DisableCUDA = 0x4000,

        AllowCpCapabilitiesForVideo = 0x8000,

        IGPUTranscodingFwdOptimus = 0x10000,

        DisableDuringSecureBoot = 0x20000,

        InvertForQuadro = 0x40000,

        InvertForMSHybrid = 0x80000,

        RegisterProcessEnableGold = 0x100000,

        HandleWindowedModePerformanceOptimal = 0x200000,

        HandleWin7AsyncRuntimeBug = 0x400000,

        ExplicitAdapterOptedByApplication = 0x800000,

        Default = 0x0
    }
#pragma warning restore 1591
}