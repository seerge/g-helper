using System;

namespace NvAPIWrapper.Native.Mosaic
{
    /// <summary>
    ///     Possible display problems in a topology validation process
    /// </summary>
    [Flags]
    public enum DisplayCapacityProblem : uint
    {
        /// <summary>
        ///     No problem
        /// </summary>
        NoProblem = 0,

        /// <summary>
        ///     Display is connected to the wrong GPU
        /// </summary>
        DisplayOnInvalidGPU = 1,

        /// <summary>
        ///     Display is connected to the wrong connector
        /// </summary>
        DisplayOnWrongConnector = 2,

        /// <summary>
        ///     Timing configuration is missing
        /// </summary>
        NoCommonTimings = 4,

        /// <summary>
        ///     EDID information is missing
        /// </summary>
        NoEDIDAvailable = 8,

        /// <summary>
        ///     Output type combination is not valid
        /// </summary>
        MismatchedOutputType = 16,

        /// <summary>
        ///     There is no display connected
        /// </summary>
        NoDisplayConnected = 32,

        /// <summary>
        ///     GPU is missing
        /// </summary>
        NoGPUTopology = 64,

        /// <summary>
        ///     Not supported
        /// </summary>
        NotSupported = 128,

        /// <summary>
        ///     SLI Bridge is missing
        /// </summary>
        NoSLIBridge = 256,

        /// <summary>
        ///     ECC is enable
        /// </summary>
        ECCEnabled = 512,

        /// <summary>
        ///     Topology is not supported by GPU
        /// </summary>
        GPUTopologyNotSupported = 1024
    }
}