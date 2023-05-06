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
    [StructureVersion(4)]
    public struct ChipsetInfoV4 : IInitializable, IChipsetInfo, IEquatable<ChipsetInfoV4>
    {
        internal StructureVersion _Version;
        internal readonly uint _VendorId;
        internal readonly uint _DeviceId;
        internal readonly ShortString _VendorName;
        internal readonly ShortString _ChipsetName;
        internal readonly ChipsetInfoFlag _Flags;
        internal readonly uint _SubSystemVendorId;
        internal readonly uint _SubSystemDeviceId;
        internal readonly ShortString _SubSystemVendorName;
        internal readonly uint _HostBridgeVendorId;
        internal readonly uint _HostBridgeDeviceId;
        internal readonly uint _HostBridgeSubSystemVendorId;
        internal readonly uint _HostBridgeSubSystemDeviceId;

        /// <inheritdoc />
        public bool Equals(ChipsetInfoV4 other)
        {
            return _VendorId == other._VendorId &&
                   _DeviceId == other._DeviceId &&
                   _VendorName.Equals(other._VendorName) &&
                   _ChipsetName.Equals(other._ChipsetName) &&
                   _Flags == other._Flags &&
                   _SubSystemVendorId == other._SubSystemVendorId &&
                   _SubSystemDeviceId == other._SubSystemDeviceId &&
                   _SubSystemVendorName.Equals(other._SubSystemVendorName) &&
                   _HostBridgeVendorId == other._HostBridgeVendorId &&
                   _HostBridgeDeviceId == other._HostBridgeDeviceId &&
                   _HostBridgeSubSystemVendorId == other._HostBridgeSubSystemVendorId &&
                   _HostBridgeSubSystemDeviceId == other._HostBridgeSubSystemDeviceId;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is ChipsetInfoV4 v4 && Equals(v4);
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
                hashCode = (hashCode * 397) ^ (int) _SubSystemVendorId;
                hashCode = (hashCode * 397) ^ (int) _SubSystemDeviceId;
                hashCode = (hashCode * 397) ^ _SubSystemVendorName.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) _HostBridgeVendorId;
                hashCode = (hashCode * 397) ^ (int) _HostBridgeDeviceId;
                hashCode = (hashCode * 397) ^ (int) _HostBridgeSubSystemVendorId;
                hashCode = (hashCode * 397) ^ (int) _HostBridgeSubSystemDeviceId;

                return hashCode;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{SubSystemVendorName} {VendorName} {ChipsetName}";
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

        /// <summary>
        ///     Chipset subsystem vendor identification
        /// </summary>
        public int SubSystemVendorId
        {
            get => (int) _SubSystemVendorId;
        }

        /// <summary>
        ///     Chipset subsystem device identification
        /// </summary>
        public int SubSystemDeviceId
        {
            get => (int) _SubSystemDeviceId;
        }

        /// <summary>
        ///     Chipset subsystem vendor name
        /// </summary>
        public string SubSystemVendorName
        {
            get => _SubSystemVendorName.Value;
        }

        /// <summary>
        ///     Host bridge vendor identification
        /// </summary>
        public int HostBridgeVendorId
        {
            get => (int) _HostBridgeVendorId;
        }

        /// <summary>
        ///     Host bridge device identification
        /// </summary>
        public int HostBridgeDeviceId
        {
            get => (int) _HostBridgeDeviceId;
        }

        /// <summary>
        ///     Host bridge subsystem vendor identification
        /// </summary>
        public int HostBridgeSubSystemVendorId
        {
            get => (int) _HostBridgeSubSystemVendorId;
        }

        /// <summary>
        ///     Host bridge subsystem device identification
        /// </summary>
        public int HostBridgeSubSystemDeviceId
        {
            get => (int) _HostBridgeSubSystemDeviceId;
        }
    }
}