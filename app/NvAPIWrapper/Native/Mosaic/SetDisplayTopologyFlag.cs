using System;

namespace NvAPIWrapper.Native.Mosaic
{
    /// <summary>
    ///     Possible flags for setting a display topology
    /// </summary>
    [Flags]
    public enum SetDisplayTopologyFlag : uint
    {
        /// <summary>
        ///     No special flag
        /// </summary>
        NoFlag = 0,

        /// <summary>
        ///     Do not change the current GPU topology. If the NO_DRIVER_RELOAD bit is not specified, then it may still require a
        ///     driver reload.
        /// </summary>
        CurrentGPUTopology = 1,

        /// <summary>
        ///     Do not allow a driver reload. That is, stick with the same master GPU as well as the same SLI configuration.
        /// </summary>
        NoDriverReload = 2,

        /// <summary>
        ///     When choosing a GPU topology, choose the topology with the best performance.
        ///     Without this flag, it will choose the topology that uses the smallest number of GPUs.
        /// </summary>
        MaximizePerformance = 4,

        /// <summary>
        ///     Do not return an error if no configuration will work with all of the grids.
        /// </summary>
        AllowInvalid = 8
    }
}