using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.Interfaces;
using NvAPIWrapper.Native.Interfaces.General;

namespace NvAPIWrapper.Native.General.Structures
{
    /// <summary>
    ///     Holds information about the system's chipset.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(2)]
    public struct ChipsetInfoV2 : IInitializable, IChipsetInfo, IEquatable<ChipsetInfoV2>
    {
        internal StructureVersion _Version;
        internal readonly uint _VendorId;
        internal readonly uint _DeviceId;
        internal readonly ShortString _VendorName;
        internal readonly ShortString _ChipsetName;
        internal readonly ChipsetInfoFlag _Flags;

        /// <inheritdoc />
        public bool Equals(ChipsetInfoV2 other)
        {
            return _VendorId == other._VendorId &&
                   _DeviceId == other._DeviceId &&
                   _VendorName.Equals(other._VendorName) &&
                   _ChipsetName.Equals(other._ChipsetName) &&
                   _Flags == other._Flags;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is ChipsetInfoV2 v2 && Equals(v2);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) _VendorId;
                hashCode = (hashCode * 397) ^ (int) _DeviceId;
                hashCode = (hashCode * 397) ^ _VendorName.GetHashCode();
                hashCode = (hashCode * 397) ^ _ChipsetName.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) _Flags;

                return hashCode;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{VendorName} {ChipsetName}";
        }

        /// <inheritdoc />
        public int VendorId
        {
            get => (int) _VendorId;
        }

        /// <inheritdoc />
        public int DeviceId
        {
            get => (int) _DeviceId;
        }

        /// <inheritdoc />
        public string VendorName
        {
            get => _VendorName.Value;
        }

        /// <inheritdoc />
        public string ChipsetName
        {
            get => _ChipsetName.Value;
        }

        /// <inheritdoc />
        public ChipsetInfoFlag Flags
        {
            get => _Flags;
        }
    }
}