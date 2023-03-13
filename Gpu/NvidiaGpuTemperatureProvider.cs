using NvAPIWrapper.GPU;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.Exceptions;
using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.Interfaces.GPU;

namespace GHelper.Gpu; 

public class NvidiaGpuTemperatureProvider : IGpuTemperatureProvider {
    private readonly PhysicalGPU? _internalGpu;

    public NvidiaGpuTemperatureProvider() {
        _internalGpu = GetInternalDiscreteGpu();
    }

    public bool IsValid => _internalGpu != null;

    public int? GetCurrentTemperature() {
        if (!IsValid)
            return null;

        IThermalSensor? gpuSensor = 
            GPUApi.GetThermalSettings(_internalGpu!.Handle).Sensors
            .FirstOrDefault(s => s.Target == ThermalSettingsTarget.GPU);

        if (gpuSensor == null)
            return null;

        return gpuSensor.CurrentTemperature;
    }

    public void Dispose() {
    }
    
    private static PhysicalGPU? GetInternalDiscreteGpu() {
        try {
            return PhysicalGPU
                .GetPhysicalGPUs()
                .FirstOrDefault(gpu => gpu.SystemType == SystemType.Laptop);
        } catch (NVIDIAApiException) {
            return null;
        }
    }
}
