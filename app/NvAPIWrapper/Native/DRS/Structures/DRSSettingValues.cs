using System;
using System.Linq;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.DRS.Structures
{
    /// <summary>
    ///     Contains a list of all possible values for a setting as well as its default value
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct DRSSettingValues : IInitializable
    {
        internal const int MaximumNumberOfValues = 100;
        internal StructureVersion _Version;
        internal uint _NumberOfValues;
        internal DRSSettingType _SettingType;
        internal DRSSettingValue _DefaultValue;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaximumNumberOfValues)]
        internal DRSSettingValue[] _Values;

        /// <summary>
        ///     Gets the setting's value type
        /// </summary>
        public DRSSettingType SettingType
        {
            get => _SettingType;
        }

        /// <summary>
        ///     Gets a list of possible values for the setting
        /// </summary>
        public object[] Values
        {
            get
            {
                switch (_SettingType)
                {
                    case DRSSettingType.Integer:

                        return ValuesAsInteger().Cast<object>().ToArray();
                    case DRSSettingType.Binary:

                        return ValuesAsBinary().Cast<object>().ToArray();
                    case DRSSettingType.String:
                    case DRSSettingType.UnicodeString:

                        return ValuesAsUnicodeString().Cast<object>().ToArray();
                    default:

                        throw new ArgumentOutOfRangeException(nameof(SettingType));
                }
            }
        }

        /// <summary>
        ///     Gets the default value of the setting
        /// </summary>
        public object DefaultValue
        {
            get
            {
                switch (_SettingType)
                {
                    case DRSSettingType.Integer:

                        return DefaultValueAsInteger();
                    case DRSSettingType.Binary:

                        return DefaultValueAsBinary();
                    case DRSSettingType.String:
                    case DRSSettingType.UnicodeString:

                        return DefaultValueAsUnicodeString();
                    default:

                        throw new ArgumentOutOfRangeException(nameof(SettingType));
                }
            }
        }

        /// <summary>
        ///     Returns the default value as an integer
        /// </summary>
        /// <returns>An integer representing the default value</returns>
        public uint DefaultValueAsInteger()
        {
            return _DefaultValue.AsInteger();
        }

        /// <summary>
        ///     Returns the default value as a byte array
        /// </summary>
        /// <returns>An array of bytes representing the default value</returns>
        public byte[] DefaultValueAsBinary()
        {
            return _DefaultValue.AsBinary();
        }

        /// <summary>
        ///     Returns the default value as an unicode string
        /// </summary>
        /// <returns>A string representing the default value</returns>
        public string DefaultValueAsUnicodeString()
        {
            return _DefaultValue.AsUnicodeString();
        }

        /// <summary>
        ///     Returns the setting's possible values as an array of integers
        /// </summary>
        /// <returns>An array of integers representing the possible values</returns>
        public uint[] ValuesAsInteger()
        {
            return _Values.Take((int) _NumberOfValues).Select(value => value.AsInteger()).ToArray();
        }

        /// <summary>
        ///     Returns the setting's possible values as an array of byte arrays
        /// </summary>
        /// <returns>An array of byte arrays representing the possible values</returns>
        public byte[][] ValuesAsBinary()
        {
            return _Values.Take((int) _NumberOfValues).Select(value => value.AsBinary()).ToArray();
        }

        /// <summary>
        ///     Returns the setting's possible values as an array of unicode strings
        /// </summary>
        /// <returns>An array of unicode strings representing the possible values</returns>
        public string[] ValuesAsUnicodeString()
        {
            return _Values.Take((int) _NumberOfValues).Select(value => value.AsUnicodeString()).ToArray();
        }
    }
}