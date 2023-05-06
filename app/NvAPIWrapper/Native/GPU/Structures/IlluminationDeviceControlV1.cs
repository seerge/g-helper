using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Holds information regarding a device illumination settings
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct IlluminationDeviceControlV1 : IInitializable
    {
        private const int MaximumNumberOfReserved = 64;
        internal IlluminationDeviceType _DeviceType;
        internal IlluminationDeviceSyncV1 _SyncInformation;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaximumNumberOfReserved)]
        internal byte[] _Reserved;

        /// <summary>
        ///     Creates a new instance of <see cref="IlluminationDeviceControlV1" />.
        /// </summary>
        /// <param name="deviceType">The device type.</param>
        /// <param name="syncInformation">The device sync information.</param>
        public IlluminationDeviceControlV1(IlluminationDeviceType deviceType, IlluminationDeviceSyncV1 syncInformation)
        {
            this = typeof(IlluminationDeviceControlV1).Instantiate<IlluminationDeviceControlV1>();
            _DeviceType = deviceType;
            _SyncInformation = syncInformation;
        }

        /// <summary>
        ///     Gets the illumination device type
        /// </summary>
        public IlluminationDeviceType DeviceType
        {
            get => _DeviceType;
        }

        /// <summary>
        ///     Gets the illumination synchronization information
        /// </summary>
        public IlluminationDeviceSyncV1 SyncInformation
        {
            get => _SyncInformation;
        }
    }
}