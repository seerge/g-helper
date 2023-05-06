namespace NvAPIWrapper.DRS.SettingValues
{
#pragma warning disable 1591
    public enum PreferredPerformanceState : uint
    {
        Adaptive = 0x0,

        PreferMaximum = 0x1,

        DriverControlled = 0x2,

        PreferConsistentPerformance = 0x3,

        PreferMinimum = 0x4,

        OptimalPower = 0x5,

        Minimum = 0x0,

        Maximum = 0x5,

        Default = 0x5
    }
#pragma warning restore 1591
}