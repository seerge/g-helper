using System.Linq;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Holds information regarding available illumination devices
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct IlluminationDeviceInfoParametersV1 : IInitializable
    {
        private const int MaximumNumberOfReserved = 64;
        private const int MaximumNumberOfDevices = 32;
        internal StructureVersion _Version;
        internal uint _NumberOfDevices;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaximumNumberOfReserved)]
        internal byte[] _Reserved;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaximumNumberOfDevices)]
        internal IlluminationDeviceInfoV1[] _Devices;

        /// <summary>
        ///     Gets an array containing all available illumination devices
        /// </summary>
        public IlluminationDeviceInfoV1[] Devices
        {
            get => _Devices.Take((int) _NumberOfDevices).ToArray();
        }
    }
}