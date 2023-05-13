namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Holds a list of possible scan out composition parameter values
    /// </summary>
    public enum ScanOutCompositionParameterValue : uint
    {
        /// <summary>
        ///     Default parameter value
        /// </summary>
        Default = 0,

        /// <summary>
        ///     BiLinear value for the warping re-sampling method parameter
        /// </summary>
        WarpingReSamplingMethodBiLinear = 0x100,

        /// <summary>
        ///     Bicubic Triangular value for the warping re-sampling method parameter
        /// </summary>
        WarpingReSamplingMethodBicubicTriangular = 0x101,

        /// <summary>
        ///     Bicubic Bell Shaped value for the warping re-sampling method parameter
        /// </summary>
        WarpingReSamplingMethodBicubicBellShaped = 0x102,

        /// <summary>
        ///     Bicubic B-Spline value for the warping re-sampling method parameter
        /// </summary>
        WarpingReSamplingMethodBicubicBSpline = 0x103,

        /// <summary>
        ///     Bicubic Adaptive Triangular value for the warping re-sampling method parameter
        /// </summary>
        WarpingReSamplingMethodBicubicAdaptiveTriangular = 0x104,

        /// <summary>
        ///     Bicubic Adaptive Bell Shaped value for the warping re-sampling method parameter
        /// </summary>
        WarpingReSamplingMethodBicubicAdaptiveBellShaped = 0x105,

        /// <summary>
        ///     Bicubic Adaptive B-Spline value for the warping re-sampling method parameter
        /// </summary>
        WarpingReSamplingMethodBicubicAdaptiveBSpline = 0x106
    }
}