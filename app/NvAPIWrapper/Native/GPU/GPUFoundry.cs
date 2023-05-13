using System.Diagnostics.CodeAnalysis;

namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     Holds a list of known GPU foundries
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public enum GPUFoundry : uint
    {
        /// <summary>
        ///     Unknown foundry
        /// </summary>
        Unknown,

        /// <summary>
        ///     Taiwan Semiconductor Manufacturing Company Limited
        /// </summary>
        TSMC,

        /// <summary>
        ///     United Microelectronics
        /// </summary>
        UMC,

        /// <summary>
        ///     International Business Machines Corporation
        /// </summary>
        IBM,

        /// <summary>
        ///     Semiconductor Manufacturing International Corporation
        /// </summary>
        SMIC,

        /// <summary>
        ///     Chartered Semiconductor Manufacturing
        /// </summary>
        CSM,

        /// <summary>
        ///     Toshiba Corporation
        /// </summary>
        Toshiba
    }
}