namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     Contains a list of valid cycle types for the piecewise linear control mode
    /// </summary>
    public enum IlluminationPiecewiseLinearCycleType : uint
    {
        /// <summary>
        ///     Half half cycle mode
        /// </summary>
        HalfHalf = 0,

        /// <summary>
        ///     Full half cycle mode
        /// </summary>
        FullHalf,

        /// <summary>
        ///     Full repeat cycle mode
        /// </summary>
        FullRepeat,

        /// <summary>
        ///     Invalid cycle mode
        /// </summary>
        Invalid = 0xFF
    }
}