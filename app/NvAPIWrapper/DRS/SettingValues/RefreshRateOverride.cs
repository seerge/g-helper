namespace NvAPIWrapper.DRS.SettingValues
{
#pragma warning disable 1591
    public enum RefreshRateOverride : uint
    {
        ApplicationControlled = 0x0,

        HighestAvailable = 0x1,

        LowLatencyRefreshRateMask = 0xFF0,

        Default = 0x0
    }
#pragma warning restore 1591
}