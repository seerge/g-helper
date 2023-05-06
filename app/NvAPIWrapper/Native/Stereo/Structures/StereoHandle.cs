using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.Stereo.Structures
{
    /// <summary>
    /// Holds a handle representing a Device Stereo Session
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct StereoHandle : IHandle, IEquatable<StereoHandle>
    {
        internal readonly IntPtr _MemoryAddress;

        /// <inheritdoc />
        public bool Equals(StereoHandle other)
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

            return obj is StereoHandle handle && Equals(handle);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _MemoryAddress.GetHashCode();
        }


        /// <inheritdoc />
        public override string ToString()
        {
            return $"StereoHandle #{MemoryAddress.ToInt64()}";
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
        public static bool operator ==(StereoHandle left, StereoHandle right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(StereoHandle left, StereoHandle right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        ///     Gets default StereoHandle with a null pointer
        /// </summary>
        public static StereoHandle DefaultHandle
        {
            get => default(StereoHandle);
        }
    }
}