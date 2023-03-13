namespace GHelper.Gpu;

public interface IGpuTemperatureProvider : IDisposable {
    bool IsValid { get; }
    int? GetCurrentTemperature();
}
