using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <summary>
    ///     UnAttachedDisplayHandle is a one-to-one map to the GDI handle of an unattached display.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct UnAttachedDisplayHandle : IHandle, IEquatable<UnAttachedDisplayHandle>
    {
        internal readonly IntPtr _MemoryAddress;

        /// <inheritdoc />
        public bool Equals(UnAttachedDisplayHandle other)
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

            return obj is UnAttachedDisplayHandle && Equals((UnAttachedDisplayHandle) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _MemoryAddress.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"UnAttachedDisplayHandle #{MemoryAddress.ToInt64()}";
        }

        /// <summary>
        ///     Checks for equality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are equal, otherwise false</returns>
        public static bool operator ==(UnAttachedDisplayHandle left, UnAttachedDisplayHandle right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(UnAttachedDisplayHandle left, UnAttachedDisplayHandle right)
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