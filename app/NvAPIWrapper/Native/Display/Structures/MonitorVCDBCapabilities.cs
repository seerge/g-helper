using System;
using NvAPIWrapper.Native.Helpers;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <summary>
    ///     Contains monitor VCDB capabilities
    /// </summary>
    public struct MonitorVCDBCapabilities
    {
        private readonly byte[] _data;

        internal MonitorVCDBCapabilities(byte[] data)
        {
            if (data.Length != 49)
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }

            _data = data;
        }

        /// <summary>
        ///     Gets a boolean value indicating RGB range quantization
        /// </summary>
        public bool QuantizationRangeRGB
        {
            get => _data[0].GetBit(1);
        }

        /// <summary>
        ///     Gets a boolean value indicating Ycc range quantization
        /// </summary>
        public bool QuantizationRangeYcc
        {
            get => _data[0].GetBit(0);
        }

        public byte ScanInfoConsumerElectronicsVideoFormats
        {
            get => (byte)_data[0].GetBits(6, 2);
        }

        public byte ScanInfoInformationTechnologyVideoFormats
        {
            get => (byte)_data[0].GetBits(4, 2);
        }

        public byte ScanInfoPreferredVideoFormat
        {
            get => (byte)_data[0].GetBits(2, 2);
        }
    }
}