using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.DRS.Structures
{
    /// <summary>
    ///     DRSSessionHandle is a reference to a DRS session.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DRSSessionHandle : IHandle, IEquatable<DRSSessionHandle>
    {
        internal readonly IntPtr _MemoryAddress;

        /// <inheritdoc />
        public bool Equals(DRSSessionHandle other)
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

            return obj is DRSSessionHandle handle && Equals(handle);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _MemoryAddress.GetHashCode();
        }


        /// <inheritdoc />
        public override string ToString()
        {
            return $"DRSSessionHandle #{MemoryAddress.ToInt64()}";
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
        public static bool operator ==(DRSSessionHandle left, DRSSessionHandle right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(DRSSessionHandle left, DRSSessionHandle right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        ///     Gets default DRSSessionHandle with a null pointer
        /// </summary>
        public static DRSSessionHandle DefaultHandle
        {
            get => default(DRSSessionHandle);
        }
    }
}