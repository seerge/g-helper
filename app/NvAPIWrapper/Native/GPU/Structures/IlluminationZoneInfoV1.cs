using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Holds information regarding a illumination zone
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct IlluminationZoneInfoV1 : IInitializable
    {
        private const int MaximumNumberOfReserved = 64;
        private const int MaximumNumberOfDataBytes = 64;

        internal IlluminationZoneType _ZoneType;
        internal byte _DeviceIndex;
        internal byte _ProviderIndex;
        internal IlluminationZoneLocation _ZoneLocation;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaximumNumberOfDataBytes)]
        internal byte[] _Data;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaximumNumberOfReserved)]
        internal byte[] _Reserved;

        /// <summary>
        ///     Gets the index of the illumination device that controls this zone.
        /// </summary>
        public int DeviceIndex
        {
            get => _DeviceIndex;
        }

        /// <summary>
        ///     Gets the provider index used for representing logical to physical zone mapping.
        /// </summary>
        public int ProviderIndex
        {
            get => _ProviderIndex;
        }

        /// <summary>
        ///     Gets the location of the zone on the board.
        /// </summary>
        public IlluminationZoneLocation ZoneLocation
        {
            get => _ZoneLocation;
        }

        /// <summary>
        ///     Gets the zone type.
        /// </summary>
        internal IlluminationZoneType ZoneType
        {
            get => _ZoneType;
        }
    }
}