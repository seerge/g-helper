namespace NvAPIWrapper.Native.Interfaces.GPU
{
    /// <summary>
    ///     Holds a list of thermal sensors
    /// </summary>
    public interface IThermalSettings
    {
        /// <summary>
        ///     Gets a list of requested thermal sensor information
        /// </summary>
        IThermalSensor[] Sensors { get; }
    }
}