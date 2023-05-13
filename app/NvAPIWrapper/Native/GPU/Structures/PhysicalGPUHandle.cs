using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     PhysicalGPUHandle is a reference to a physical GPU. Each GPU in a multi-GPU board will have its own handle. GPUs
    ///     are assigned a handle even if they are not in use by the OS.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PhysicalGPUHandle : IHandle, IEquatable<PhysicalGPUHandle>
    {
        /// <summary>
        ///     Queryable number of physical GPUs
        /// </summary>
        public const int PhysicalGPUs = 32;

        /// <summary>
        ///     Maximum number of physical GPUs
        /// </summary>
        public const int MaxPhysicalGPUs = 64;

        internal readonly IntPtr _MemoryAddress;

        /// <inheritdoc />
        public bool Equals(PhysicalGPUHandle other)
        {
            return _MemoryAddress.Equals(other._MemoryAddress);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is PhysicalGPUHandle handle && Equals(handle);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _MemoryAddress.GetHashCode();
        }


        /// <inheritdoc />
        public override string ToString()
        {
            return $"PhysicalGPUHandle #{MemoryAddress.ToInt64()}";
        }

        /// <inheritdoc />
        public IntPtr MemoryAddress
        {
            get => _MemoryAddress;
        }

        /// <inheritdoc />
        public bool IsNull
        {
            get => _MemoryAddress == IntPtr.Zero;
        }

        /// <summary>
        ///     Checks for equality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are equal, otherwise false</returns>
        public static bool operator ==(PhysicalGPUHandle left, PhysicalGPUHandle right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(PhysicalGPUHandle left, PhysicalGPUHandle right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        ///     Gets default PhysicalGPUHandle with a null pointer
        /// </summary>
        public static PhysicalGPUHandle DefaultHandle
        {
            get => default(PhysicalGPUHandle);
        }
    }
}