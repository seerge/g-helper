namespace GHelper.Gpu;

public  interface IGpuControl : IDisposable {
    bool IsNvidia { get; }
    bool IsValid { get; }
    public string FullName { get; }
    int? GetCurrentTemperature();
    int? GetGpuUse();
    (long usedMb, long totalMb)? GetVramInfo();
    float? GetGpuPower();
    void KillGPUApps();

}
