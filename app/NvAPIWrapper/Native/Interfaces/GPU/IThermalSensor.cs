using NvAPIWrapper.Native.GPU;

namespace NvAPIWrapper.Native.Interfaces.GPU
{
    /// <summary>
    ///     Provides information about a single thermal sensor
    /// </summary>
    public interface IThermalSensor
    {
        /// <summary>
        ///     Internal, ADM1032, MAX6649...
        /// </summary>
        ThermalController Controller { get; }

        /// <summary>
        ///     Current temperature value of the thermal sensor in degree Celsius
        /// </summary>
        int CurrentTemperature { get; }

        /// <summary>
        ///     Maximum default temperature value of the thermal sensor in degree Celsius
        /// </summary>
        int DefaultMaximumTemperature { get; }

        /// <summary>
        ///     Minimum default temperature value of the thermal sensor in degree Celsius
        /// </summary>
        int DefaultMinimumTemperature { get; }

        /// <summary>
        ///     Thermal sensor targeted - GPU, memory, chipset, power supply, Visual Computing Device, etc
        /// </summary>
        ThermalSettingsTarget Target { get; }
    }
}