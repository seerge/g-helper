using System;

namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     Contains the flags used by the GPUApi.GetPerformanceStatesInfo() function
    /// </summary>
    [Flags]
    public enum GetPerformanceStatesInfoFlags
    {
        /// <summary>
        ///     Current performance states settings
        /// </summary>
        Current = 0,

        /// <summary>
        ///     Default performance states settings
        /// </summary>
        Default = 1,

        /// <summary>
        ///     Maximum range of performance states values
        /// </summary>
        Maximum = 2,

        /// <summary>
        ///     Minimum range of performance states values
        /// </summary>
        Minimum = 4
    }
}