using System;
using System.Linq;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.DRS.Structures
{
    /// <summary>
    ///     Represents a NVIDIA driver setting
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct DRSSettingV1 : IInitializable
    {
        internal StructureVersion _Version;
        internal UnicodeString _SettingName;
        internal uint _SettingId;
        internal DRSSettingType _SettingType;
        internal DRSSettingLocation _SettingLocation;
        internal uint _IsCurrentPredefined;
        internal uint _IsPredefinedValid;
        internal DRSSettingValue _PredefinedValue;
        internal DRSSettingValue _CurrentValue;

        /// <summary>
        ///     Creates a new instance of <see cref="DRSSettingV1" /> containing the passed value.
        /// </summary>
        /// <param name="id">The setting identification number.</param>
        /// <param name="settingType">The type of the setting's value</param>
        /// <param name="value">The setting's value</param>
        public DRSSettingV1(uint id, DRSSettingType settingType, object value)
        {
            this = typeof(DRSSettingV1).Instantiate<DRSSettingV1>();
            Id = id;
            IsPredefinedValueValid = false;
            _SettingType = settingType;
            CurrentValue = value;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="DRSSettingV1" /> containing the passed value.
        /// </summary>
        /// <param name="id">The setting identification number.</param>
        /// <param name="value">The setting's value</param>
        public DRSSettingV1(uint id, string value) : this(id, DRSSettingType.String, value)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="DRSSettingV1" /> containing the passed value.
        /// </summary>
        /// <param name="id">The setting identification number.</param>
        /// <param name="value">The setting's value</param>
        public DRSSettingV1(uint id, uint value) : this(id, DRSSettingType.Integer, value)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="DRSSettingV1" /> containing the passed value.
        /// </summary>
        /// <param name="id">The setting identification number.</param>
        /// <param name="value">The setting's value</param>
        public DRSSettingV1(uint id, byte[] value) : this(id, DRSSettingType.Binary, value)
        {
        }

        /// <summary>
        ///     Gets the name of the setting
        /// </summary>
        public string Name
        {
            get => _SettingName.Value;
        }

        /// <summary>
        ///     Gets the identification number of the setting
        /// </summary>
        public uint Id
        {
            get => _SettingId;
            private set => _SettingId = value;
        }

        /// <summary>
        ///     Gets the setting's value type
        /// </summary>
        public DRSSettingType SettingType
        {
            get => _SettingType;
            private set => _SettingType = value;
        }

        /// <summary>
        ///     Gets the setting location
        /// </summary>
        public DRSSettingLocation SettingLocation
        {
            get => _SettingLocation;
        }

        /// <summary>
        ///     Gets a boolean value indicating if the current value is the predefined value
        /// </summary>
        public bool IsCurrentValuePredefined
        {
            get => _IsCurrentPredefined > 0;
            private set => _IsCurrentPredefined = value ? 1u : 0u;
        }

        /// <summary>
        ///     Gets a boolean value indicating if the predefined value is available and valid
        /// </summary>
        public bool IsPredefinedValueValid
        {
            get => _IsPredefinedValid > 0;
            private set => _IsPredefinedValid = value ? 1u : 0u;
        }

        /// <summary>
        ///     Returns the predefined value as an integer
        /// </summary>
        /// <returns>An integer representing the predefined value</returns>
        public uint GetPredefinedValueAsInteger()
        {
            return _PredefinedValue.AsInteger();
        }

        /// <summary>
        ///     Returns the predefined value as an array of bytes
        /// </summary>
        /// <returns>An byte array representing the predefined value</returns>
        public byte[] GetPredefinedValueAsBinary()
        {
            return _PredefinedValue.AsBinary();
        }

        /// <summary>
        ///     Returns the predefined value as an unicode string
        /// </summary>
        /// <returns>An unicode string representing the predefined value</returns>
        public string GetPredefinedValueAsUnicodeString()
        {
            return _PredefinedValue.AsUnicodeString();
        }

        /// <summary>
        ///     Gets the setting's predefined value
        /// </summary>
        public object PredefinedValue
        {
            get
            {
                if (!IsPredefinedValueValid)
                {
                    return null;
                }

                switch (_SettingType)
                {
                    case DRSSettingType.Integer:

                        return GetPredefinedValueAsInteger();
                    case DRSSettingType.Binary:

                        return GetPredefinedValueAsBinary();
                    case DRSSettingType.String:
                    case DRSSettingType.UnicodeString:

                        return GetPredefinedValueAsUnicodeString();
                    default:

                        throw new ArgumentOutOfRangeException(nameof(SettingType));
                }
            }
        }

        /// <summary>
        ///     Returns the current value as an integer
        /// </summary>
        /// <returns>An integer representing the current value</returns>
        public uint GetCurrentValueAsInteger()
        {
            return _CurrentValue.AsInteger();
        }

        /// <summary>
        ///     Returns the current value as an array of bytes
        /// </summary>
        /// <returns>An byte array representing the current value</returns>
        public byte[] GetCurrentValueAsBinary()
        {
            return _CurrentValue.AsBinary();
        }

        /// <summary>
        ///     Returns the current value as an unicode string
        /// </summary>
        /// <returns>An unicode string representing the current value</returns>
        public string GetCurrentValueAsUnicodeString()
        {
            return _CurrentValue.AsUnicodeString();
        }

        /// <summary>
        ///     Sets the passed value as the current value
        /// </summary>
        /// <param name="value">The new value for the setting</param>
        public void SetCurrentValueAsInteger(uint value)
        {
            if (SettingType != DRSSettingType.Integer)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Passed argument is invalid for this setting.");
            }

            _CurrentValue = new DRSSettingValue(value);
            IsCurrentValuePredefined = IsPredefinedValueValid && (uint) CurrentValue == (uint) PredefinedValue;
        }

        /// <summary>
        ///     Sets the passed value as the current value
        /// </summary>
        /// <param name="value">The new value for the setting</param>
        public void SetCurrentValueAsBinary(byte[] value)
        {
            if (SettingType != DRSSettingType.Binary)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Passed argument is invalid for this setting.");
            }

            _CurrentValue = new DRSSettingValue(value);
            IsCurrentValuePredefined =
                IsPredefinedValueValid &&
                ((byte[]) CurrentValue)?.SequenceEqual((byte[]) PredefinedValue ?? new byte[0]) == true;
        }

        /// <summary>
        ///     Sets the passed value as the current value
        /// </summary>
        /// <param name="value">The new value for the setting</param>
        public void SetCurrentValueAsUnicodeString(string value)
        {
            if (SettingType != DRSSettingType.UnicodeString)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Passed argument is invalid for this setting.");
            }

            _CurrentValue = new DRSSettingValue(value);
            IsCurrentValuePredefined =
                IsPredefinedValueValid &&
                string.Equals(
                    (string) CurrentValue,
                    (string) PredefinedValue,
                    StringComparison.InvariantCulture
                );
        }

        /// <summary>
        ///     Gets or sets the setting's current value
        /// </summary>
        public object CurrentValue
        {
            get
            {
                switch (_SettingType)
                {
                    case DRSSettingType.Integer:

                        return GetCurrentValueAsInteger();
                    case DRSSettingType.Binary:

                        return GetCurrentValueAsBinary();
                    case DRSSettingType.String:
                    case DRSSettingType.UnicodeString:

                        return GetCurrentValueAsUnicodeString();
                    default:

                        throw new ArgumentOutOfRangeException(nameof(SettingType));
                }
            }
            private set
            {
                if (value is int intValue)
                {
                    SetCurrentValueAsInteger((uint) intValue);
                }
                else if (value is uint unsignedIntValue)
                {
                    SetCurrentValueAsInteger(unsignedIntValue);
                }
                else if (value is short shortValue)
                {
                    SetCurrentValueAsInteger((uint) shortValue);
                }
                else if (value is ushort unsignedShortValue)
                {
                    SetCurrentValueAsInteger(unsignedShortValue);
                }
                else if (value is long longValue)
                {
                    SetCurrentValueAsInteger((uint) longValue);
                }
                else if (value is ulong unsignedLongValue)
                {
                    SetCurrentValueAsInteger((uint) unsignedLongValue);
                }
                else if (value is byte byteValue)
                {
                    SetCurrentValueAsInteger(byteValue);
                }
                else if (value is string stringValue)
                {
                    SetCurrentValueAsUnicodeString(stringValue);
                }
                else if (value is byte[] binaryValue)
                {
                    SetCurrentValueAsBinary(binaryValue);
                }
                else
                {
                    throw new ArgumentException("Unacceptable argument type.", nameof(value));
                }
            }
        }
    }
}