using System;

namespace NvAPIWrapper.Native.Interfaces
{
    /// <summary>
    ///     Interface for all pointer based handles
    /// </summary>
    public interface IHandle
    {
        /// <summary>
        ///     Returns true if the handle is null and not pointing to a valid location in the memory
        /// </summary>
        bool IsNull { get; }

        /// <summary>
        ///     Gets the address of the handle in the memory
        /// </summary>
        IntPtr MemoryAddress { get; }
    }
}