using System.Diagnostics.CodeAnalysis;

namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     Holds a list of known memory makers
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    // ReSharper disable CommentTypo
    public enum GPUMemoryMaker : uint
    {
        /// <summary>
        ///     Unknown memory maker
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///     Samsung Group
        /// </summary>
        Samsung,

        /// <summary>
        ///     Qimonda AG
        /// </summary>
        Qimonda,

        /// <summary>
        ///     Elpida Memory, Inc.
        /// </summary>
        Elpida,

        /// <summary>
        ///     Etron Technology, Inc.
        /// </summary>
        Etron,

        /// <summary>
        ///     Nanya Technology Corporation
        /// </summary>
        Nanya,

        /// <summary>
        ///     SK Hynix
        /// </summary>
        Hynix,

        /// <summary>
        ///     Mosel Vitelic Corporation
        /// </summary>
        Mosel,

        /// <summary>
        ///     Winbond Electronics Corporation
        /// </summary>
        Winbond,

        /// <summary>
        ///     Elite Semiconductor Memory Technology Inc.
        /// </summary>
        Elite,

        /// <summary>
        ///     Micron Technology, Inc.
        /// </summary>
        Micron
    }
}