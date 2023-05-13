using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.DRS.Structures
{
    /// <summary>
    ///     Represents a setting value
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct DRSSettingValue : IInitializable
    {
        private const int UnicodeStringLength = UnicodeString.UnicodeStringLength;
        private const int BinaryDataMax = 4096;

        // Math.Max(BinaryDataMax + sizeof(uint), UnicodeStringLength * sizeof(ushort))
        private const int FullStructureSize = 4100;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = FullStructureSize, ArraySubType = UnmanagedType.U1)]
        internal byte[] _BinaryValue;

        /// <summary>
        ///     Creates a new instance of <see cref="DRSSettingValue" /> containing the passed unicode string as the value
        /// </summary>
        /// <param name="value">The unicode string value</param>
        public DRSSettingValue(string value)
        {
            if (value?.Length > UnicodeStringLength)
            {
                value = value.Substring(0, UnicodeStringLength);
            }

            _BinaryValue = new byte[FullStructureSize];

            var stringBytes = Encoding.Unicode.GetBytes(value ?? string.Empty);
            Array.Copy(stringBytes, 0, _BinaryValue, 0, Math.Min(stringBytes.Length, _BinaryValue.Length));
        }

        /// <summary>
        ///     Creates a new instance of <see cref="DRSSettingValue" /> containing the passed byte array as the value
        /// </summary>
        /// <param name="value">The byte array value</param>
        public DRSSettingValue(byte[] value)
        {
            _BinaryValue = new byte[FullStructureSize];

            if (value?.Length > 0)
            {
                var arrayLength = Math.Min(value.Length, BinaryDataMax);
                var arrayLengthBytes = BitConverter.GetBytes((uint) arrayLength);
                Array.Copy(arrayLengthBytes, 0, _BinaryValue, 0, arrayLengthBytes.Length);
                Array.Copy(value, 0, _BinaryValue, arrayLengthBytes.Length, arrayLength);
            }
        }

        /// <summary>
        ///     Creates a new instance of <see cref="DRSSettingValue" /> containing the passed integer as the value
        /// </summary>
        /// <param name="value">The integer value</param>
        public DRSSettingValue(uint value)
        {
            _BinaryValue = new byte[FullStructureSize];
            var arrayLengthBytes = BitConverter.GetBytes(value);
            Array.Copy(arrayLengthBytes, 0, _BinaryValue, 0, arrayLengthBytes.Length);
        }

        /// <summary>
        ///     Returns the value as an integer
        /// </summary>
        /// <returns>An integer representing the value</returns>
        public uint AsInteger()
        {
            return BitConverter.ToUInt32(_BinaryValue, 0);
        }

        /// <summary>
        ///     Returns the value as an array of bytes
        /// </summary>
        /// <returns>An array of bytes representing the value</returns>
        public byte[] AsBinary()
        {
            return _BinaryValue.Skip(sizeof(uint)).Take((int) AsInteger()).ToArray();
        }

        /// <summary>
        ///     Returns the value as an unicode string
        /// </summary>
        /// <returns>An unicode string representing the value</returns>
        public string AsUnicodeString()
        {
            return Encoding.Unicode.GetString(_BinaryValue).TrimEnd('\0');
        }
    }
}