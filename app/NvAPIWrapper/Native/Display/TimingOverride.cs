namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Timing override modes
    /// </summary>
    public enum TimingOverride
    {
        /// <summary>
        ///     Current timing
        /// </summary>
        Current = 0,

        /// <summary>
        ///     Auto timing
        /// </summary>
        Auto,

        /// <summary>
        ///     EDID timing
        /// </summary>
        EDID,

        /// <summary>
        ///     VESA DMT timing
        /// </summary>
        DMT,

        /// <summary>
        ///     VESA DMT timing with reduced blanking
        /// </summary>
        DMTReducedBlanking,

        /// <summary>
        ///     VESA CVT timing
        /// </summary>
        CVT,

        /// <summary>
        ///     VESA CVT timing with reduced blanking
        /// </summary>
        CVTReducedBlanking,

        /// <summary>
        ///     VESA GTF
        /// </summary>
        GTF,

        /// <summary>
        ///     EIA 861x PreDefined timing
        /// </summary>
        EIA861,

        /// <summary>
        ///     AnalogTV PreDefined timing
        /// </summary>
        AnalogTV,

        /// <summary>
        ///     NVIDIA Custom timing
        /// </summary>
        Custom,

        /// <summary>
        ///     NVIDIA PreDefined timing
        /// </summary>
        Predefined,

        /// <summary>
        ///     NVIDIA PreDefined timing
        /// </summary>
        PSF = Predefined,

        /// <summary>
        ///     ASPR timing
        /// </summary>
        ASPR,

        /// <summary>
        ///     Override for SDI timing
        /// </summary>
        SDI,

        /// <summary>
        ///     Not used
        /// </summary>
        Max
    }
}