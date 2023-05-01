namespace GHelper.Gpu;

public interface IGpuControl : IDisposable {
    bool IsNvidia { get; }
    bool IsValid { get; }
    int? GetCurrentTemperature();
    int? GetGpuUse();
}
