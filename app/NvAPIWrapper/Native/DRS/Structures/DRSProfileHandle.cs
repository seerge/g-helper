using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.DRS.Structures
{
    /// <summary>
    ///     DRSProfileHandle is a reference to a DRS profile.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DRSProfileHandle : IHandle, IEquatable<DRSProfileHandle>
    {
        internal readonly IntPtr _MemoryAddress;

        private DRSProfileHandle(IntPtr memoryAddress)
        {
            _MemoryAddress = memoryAddress;
        }

        /// <inheritdoc />
        public bool Equals(DRSProfileHandle other)
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

            return obj is DRSProfileHandle handle && Equals(handle);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _MemoryAddress.GetHashCode();
        }


        /// <inheritdoc />
        public override string ToString()
        {
            return $"DRSProfileHandle #{MemoryAddress.ToInt64()}";
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
        public static bool operator ==(DRSProfileHandle left, DRSProfileHandle right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(DRSProfileHandle left, DRSProfileHandle right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        ///     Gets default DRSProfileHandle with a null pointer
        /// </summary>
        public static DRSProfileHandle DefaultHandle
        {
            get => default(DRSProfileHandle);
        }

        /// <summary>
        ///     Gets the default global profile handle
        /// </summary>
        public static DRSProfileHandle DefaultGlobalProfileHandle
        {
            get => new DRSProfileHandle(new IntPtr(-1));
        }
    }
}