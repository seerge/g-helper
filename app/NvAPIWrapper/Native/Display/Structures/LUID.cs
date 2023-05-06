using System;
using System.Runtime.InteropServices;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <summary>
    ///     Locally unique identifier is a 64-bit value guaranteed to be unique only on the system on which it was generated.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct LUID : IEquatable<LUID>
    {
        /// <summary>
        ///     32Bit unsigned integer, low
        /// </summary>
        public readonly uint LowPart;

        /// <summary>
        ///     32Bit signed integer, high
        /// </summary>
        public readonly int HighPart;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{{{LowPart:X}-{HighPart:X}}}";
        }

        /// <inheritdoc />
        public bool Equals(LUID other)
        {
            return LowPart == other.LowPart && HighPart == other.HighPart;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is LUID luid && Equals(luid);
        }

        /// <summary>
        ///     Checks for equality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are equal, otherwise false</returns>
        public static bool operator ==(LUID left, LUID right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(LUID left, LUID right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) LowPart * 397) ^ HighPart;
            }
        }
    }
}