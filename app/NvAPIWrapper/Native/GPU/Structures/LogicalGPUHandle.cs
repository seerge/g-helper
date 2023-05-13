using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     LogicalGPUHandle is a reference to one or more physical GPUs acting as a single logical device. A single GPU will
    ///     have a single logical GPU handle and a single physical GPU handle. Two GPUs acting in an SLI configuration will
    ///     have a single logical GPU handle and two physical GPU handles.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct LogicalGPUHandle : IHandle, IEquatable<LogicalGPUHandle>
    {
        /// <summary>
        ///     Maximum number of logical GPUs
        /// </summary>
        public const int MaxLogicalGPUs = 64;

        internal readonly IntPtr _MemoryAddress;

        /// <inheritdoc />
        public bool Equals(LogicalGPUHandle other)
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

            return obj is LogicalGPUHandle handle && Equals(handle);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _MemoryAddress.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"LogicalGPUHandle #{MemoryAddress.ToInt64()}";
        }

        /// <summary>
        ///     Checks for equality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are equal, otherwise false</returns>
        public static bool operator ==(LogicalGPUHandle left, LogicalGPUHandle right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(LogicalGPUHandle left, LogicalGPUHandle right)
        {
            return !left.Equals(right);
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
    }
}