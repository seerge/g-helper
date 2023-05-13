namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     List of possible thermal sensor controllers
    /// </summary>
    public enum ThermalController
    {
        /// <summary>
        ///     No Thermal Controller
        /// </summary>
        None = 0,

        /// <summary>
        ///     GPU acting as thermal controller
        /// </summary>
        GPU,

        /// <summary>
        ///     ADM1032 Thermal Controller
        /// </summary>
        ADM1032,

        /// <summary>
        ///     MAX6649 Thermal Controller
        /// </summary>
        MAX6649,

        /// <summary>
        ///     MAX1617 Thermal Controller
        /// </summary>
        MAX1617,

        /// <summary>
        ///     LM99 Thermal Controller
        /// </summary>
        LM99,

        /// <summary>
        ///     LM89 Thermal Controller
        /// </summary>
        LM89,

        /// <summary>
        ///     LM64 Thermal Controller
        /// </summary>
        LM64,

        /// <summary>
        ///     ADT7473 Thermal Controller
        /// </summary>
        ADT7473,

        /// <summary>
        ///     SBMAX6649 Thermal Controller
        /// </summary>
        SBMAX6649,

        /// <summary>
        ///     VideoBios acting as thermal controller
        /// </summary>
        VideoBiosEvent,

        /// <summary>
        ///     Operating System acting as thermal controller
        /// </summary>
        OperatingSystem,

        /// <summary>
        ///     Unknown Thermal Controller
        /// </summary>
        Unknown = -1
    }
}