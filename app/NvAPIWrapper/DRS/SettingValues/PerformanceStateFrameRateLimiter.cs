namespace NvAPIWrapper.DRS.SettingValues
{
#pragma warning disable 1591
    public enum PerformanceStateFrameRateLimiter : uint
    {
        Disabled = 0x0,

        FPS20 = 0x14,

        FPS30 = 0x1E,

        FPS40 = 0x28,

        Fpsmask = 0xFF,

        NoAlign = 0x4000,

        BBQM = 0x8000,

        LowerFPSToAlign = 0x20000,

        ForceVSyncOff = 0x40000,

        GpsWeb = 0x80000,

        Disallowed = 0x200000,

        UseCPUWait = 0x400000,

        NoLagOffset = 0x800000,

        Accurate = 0x10000000,

        AllowWindowed = 0x20000000,

        ForceOn = 0x40000000,

        Enabled = 0x80000000,

        OpenGLRemoteDesktop = 0xE000003C,

        Mask = 0xF0EEC0FF,

        Default = 0x0
    }
#pragma warning restore 1591
}