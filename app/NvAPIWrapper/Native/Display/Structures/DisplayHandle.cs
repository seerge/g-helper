using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <summary>
    ///     DisplayHandle is a one-to-one map to the GDI handle of an attached display in the Windows Display Properties
    ///     Settings page.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DisplayHandle : IHandle, IEquatable<DisplayHandle>
    {
        internal readonly IntPtr _MemoryAddress;

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

        /// <inheritdoc />
        public bool Equals(DisplayHandle other)
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

            return obj is DisplayHandle handle && Equals(handle);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _MemoryAddress.GetHashCode();
        }

        /// <summary>
        ///     Checks for equality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are equal, otherwise false</returns>
        public static bool operator ==(DisplayHandle left, DisplayHandle right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(DisplayHandle left, DisplayHandle right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"DisplayHandle #{MemoryAddress.ToInt64()}";
        }

        /// <summary>
        ///     Gets default DisplayHandle with a null pointer
        /// </summary>
        public static DisplayHandle DefaultHandle
        {
            get => default(DisplayHandle);
        }
    }
}