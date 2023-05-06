using System;

namespace NvAPIWrapper.GPU
{
    /// <summary>
    ///     Contains information about the PCI connection
    /// </summary>
    public class PCIIdentifiers : IEquatable<PCIIdentifiers>
    {
        // ReSharper disable once TooManyDependencies
        internal PCIIdentifiers(uint deviceId, uint subSystemId, uint revisionId, int externalDeviceId = 0)
        {
            DeviceId = deviceId;
            SubSystemId = subSystemId;
            RevisionId = revisionId;

            if (externalDeviceId > 0)
            {
                ExternalDeviceId = (ushort) externalDeviceId;
            }
            else
            {
                ExternalDeviceId = (ushort) (deviceId >> 16);
            }

            VendorId = (ushort) ((DeviceId << 16) >> 16);
        }

        /// <summary>
        ///     Gets the internal PCI device identifier
        /// </summary>
        public uint DeviceId { get; }

        /// <summary>
        ///     Gets the external PCI device identifier
        /// </summary>
        public ushort ExternalDeviceId { get; }

        /// <summary>
        ///     Gets the internal PCI device-specific revision identifier
        /// </summary>
        public uint RevisionId { get; }

        /// <summary>
        ///     Gets the internal PCI subsystem identifier
        /// </summary>
        public uint SubSystemId { get; }

        /// <summary>
        ///     Gets the vendor identification calculated from internal device identification
        /// </summary>
        public ushort VendorId { get; }

        /// <inheritdoc />
        public bool Equals(PCIIdentifiers other)
        {
            return DeviceId == other.DeviceId &&
                   SubSystemId == other.SubSystemId &&
                   RevisionId == other.RevisionId;
        }

        /// <summary>
        ///     Checks for equality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are equal, otherwise false</returns>
        public static bool operator ==(PCIIdentifiers left, PCIIdentifiers right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(PCIIdentifiers left, PCIIdentifiers right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is PCIIdentifiers identifiers && Equals(identifiers);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) DeviceId;
                hashCode = (hashCode * 397) ^ (int) SubSystemId;
                hashCode = (hashCode * 397) ^ (int) RevisionId;

                return hashCode;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"PCI\\VEN_{VendorId:X}&DEV_{ExternalDeviceId:X}&SUBSYS_{SubSystemId:X}&REV_{RevisionId:X}";
        }
    }
}