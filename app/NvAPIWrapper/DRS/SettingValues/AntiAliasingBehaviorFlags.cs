namespace NvAPIWrapper.DRS.SettingValues
{
#pragma warning disable 1591
    public enum AntiAliasingBehaviorFlags : uint
    {
        None = 0x0,

        TreatOverrideAsApplicationControlled = 0x1,

        TreatOverrideAsEnhance = 0x2,

        DisableOverride = 0x3,

        TreatEnhanceAsApplicationControlled = 0x4,

        TreatEnhanceAsOverride = 0x8,

        DisableEnhance = 0xC,

        MapVCAAToMultiSampling = 0x10000,

        SLIDisableTransparencySupersampling = 0x20000,

        DisableCplaa = 0x40000,

        SkipRTDIMCheckForEnhance = 0x80000,

        DisableSLIAntiAliasing = 0x100000,

        Default = 0x0,

        AntiAliasingRTBPPDIV4 = 0xF0000000,

        AntiAliasingRTBPPDIV4Shift = 0x1C,

        NonAntiAliasingRTBPPDIV4 = 0xF000000,

        NonAntiAliasingRTBPPDIV4Shift = 0x18,

        Mask = 0xFF1F000F
    }
#pragma warning restore 1591
}