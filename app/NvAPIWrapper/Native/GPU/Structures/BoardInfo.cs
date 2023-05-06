using System;
using System.Linq;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Holds the board information (a unique GPU Board Serial Number) stored in the InfoROM
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct BoardInfo : IInitializable, IEquatable<BoardInfo>
    {
        internal StructureVersion _Version;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        internal byte[] _SerialNumber;

        /// <summary>
        ///     Board Serial Number
        /// </summary>
        public byte[] SerialNumber
        {
            get => _SerialNumber;
        }

        /// <inheritdoc />
        public bool Equals(BoardInfo other)
        {
            return _SerialNumber.SequenceEqual(other._SerialNumber);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is BoardInfo info && Equals(info);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return _SerialNumber?.GetHashCode() ?? 0;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return SerialNumber == null ? "Unknown" : "Serial " + BitConverter.ToString(SerialNumber);
        }

        /// <summary>
        ///     Checks for equality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are equal, otherwise false</returns>
        public static bool operator ==(BoardInfo left, BoardInfo right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(BoardInfo left, BoardInfo right)
        {
            return !left.Equals(right);
        }
    }
}