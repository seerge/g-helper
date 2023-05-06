using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Holds information regarding the data necessary for synchronization.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct IlluminationDeviceSyncV1 : IInitializable
    {
        private const int MaximumNumberOfReserved = 64;
        internal byte _IsSync;
        internal ulong _TimeStampInMS;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaximumNumberOfReserved)]
        internal byte[] _Reserved;

        /// <summary>
        ///     Creates a new instance of <see cref="IlluminationDeviceSyncV1" />
        /// </summary>
        /// <param name="isSync">A boolean value indicating if synchronization is enabled.</param>
        /// <param name="timeStampInMS">The synchronization timestamp in ms</param>
        public IlluminationDeviceSyncV1(bool isSync, ulong timeStampInMS)
        {
            this = typeof(IlluminationDeviceSyncV1).Instantiate<IlluminationDeviceSyncV1>();
            _IsSync = isSync ? (byte) 1 : (byte) 0;
            _TimeStampInMS = timeStampInMS;
        }

        /// <summary>
        ///     Gets a boolean value indicating the need for synchronization.
        /// </summary>
        public bool IsSync
        {
            get => _IsSync > 0;
        }

        /// <summary>
        ///     Gets the timestamp in milliseconds required for synchronization.
        /// </summary>
        public ulong TimeStampInMS
        {
            get => _TimeStampInMS;
        }
    }
}