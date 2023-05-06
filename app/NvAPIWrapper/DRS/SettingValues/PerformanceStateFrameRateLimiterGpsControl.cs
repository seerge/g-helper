namespace NvAPIWrapper.DRS.SettingValues
{
#pragma warning disable 1591
    public enum PerformanceStateFrameRateLimiterGpsControl : uint
    {
        Disabled = 0x0,

        DecreaseFilterMask = 0x1FF,

        PauseTimeMask = 0xFE00,

        PauseTimeShift = 0x9,

        TargetRenderTimeMask = 0xFF0000,

        TargetRenderTimeShift = 0x10,

        PerformanceStepSizeMask = 0x1F000000,

        PerformanceStepSizeShift = 0x18,

        IncreaseFilterMask = 0xE0000000,

        IncreaseFilterShift = 0x1D,

        OptimalSetting = 0x4A5A3219,

        Default = 0x0
    }
#pragma warning restore 1591
}