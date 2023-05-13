using System;
using System.Linq;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Holds information regarding available devices illumination settings
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct IlluminationDeviceControlParametersV1 : IInitializable
    {
        private const int MaximumNumberOfReserved = 64;
        private const int MaximumNumberOfDevices = 32;
        internal StructureVersion _Version;
        internal uint _NumberOfDevices;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaximumNumberOfReserved)]
        internal byte[] _Reserved;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaximumNumberOfDevices)]
        internal IlluminationDeviceControlV1[] _Devices;

        /// <summary>
        ///     Creates a new instance of <see cref="IlluminationDeviceControlParametersV1" />.
        /// </summary>
        /// <param name="devices">The list of illumination settings of devices.</param>
        public IlluminationDeviceControlParametersV1(IlluminationDeviceControlV1[] devices)
        {
            if (!(devices?.Length > 0) || devices.Length > MaximumNumberOfDevices)
            {
                throw new ArgumentOutOfRangeException(nameof(devices));
            }

            this = typeof(IlluminationDeviceControlParametersV1).Instantiate<IlluminationDeviceControlParametersV1>();
            _NumberOfDevices = (uint) devices.Length;
            Array.Copy(devices, 0, _Devices, 0, devices.Length);
        }

        /// <summary>
        ///     Gets a list of available illumination settings of devices.
        /// </summary>
        public IlluminationDeviceControlV1[] Devices
        {
            get => _Devices.Take((int) _NumberOfDevices).ToArray();
        }
    }
}