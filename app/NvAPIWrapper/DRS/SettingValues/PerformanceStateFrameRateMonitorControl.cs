namespace NvAPIWrapper.DRS.SettingValues
{
#pragma warning disable 1591
    public enum PerformanceStateFrameRateMonitorControl : uint
    {
        Disabled = 0x0,

        ThresholdPctMask = 0xFF,

        MovingAverageXMask = 0xF00,

        MovingAverageXShift = 0x8,

        EnableFineGrained = 0x400000,

        EnableOnVSync = 0x800000,

        VSyncOffsetMask = 0xF000,

        VSyncOffsetShift = 0xC,

        FPSUseFrl = 0x0,

        FPS30 = 0x1E000000,

        FPS60 = 0x3C000000,

        FPSMask = 0xFF000000,

        FPSShift = 0x18,

        OptimalSetting = 0x364,

        VSyncOptimalSetting = 0x80F364,

        Default = 0x0
    }
#pragma warning restore 1591
}