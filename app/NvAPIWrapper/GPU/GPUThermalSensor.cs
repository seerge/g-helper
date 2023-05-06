using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.Interfaces.GPU;

namespace NvAPIWrapper.GPU
{
    /// <summary>
    ///     Represents a thermal sensor
    /// </summary>
    public class GPUThermalSensor : IThermalSensor
    {
        internal GPUThermalSensor(int sensorId, IThermalSensor thermalSensor)
        {
            SensorId = sensorId;
            Controller = thermalSensor.Controller;
            DefaultMinimumTemperature = thermalSensor.DefaultMinimumTemperature;
            DefaultMaximumTemperature = thermalSensor.DefaultMaximumTemperature;
            CurrentTemperature = thermalSensor.CurrentTemperature;
            Target = thermalSensor.Target;
        }

        /// <summary>
        ///     Gets the sensor identification number or index
        /// </summary>
        public int SensorId { get; set; }

        /// <inheritdoc />
        public ThermalController Controller { get; }

        /// <inheritdoc />
        public int CurrentTemperature { get; }

        /// <inheritdoc />
        public int DefaultMaximumTemperature { get; }

        /// <inheritdoc />
        public int DefaultMinimumTemperature { get; }

        /// <inheritdoc />
        public ThermalSettingsTarget Target { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return
                $"[{Target} @ {Controller}] Current: {CurrentTemperature}°C - Default Range: [({DefaultMinimumTemperature}°C) , ({DefaultMaximumTemperature}°C)]";
        }
    }
}