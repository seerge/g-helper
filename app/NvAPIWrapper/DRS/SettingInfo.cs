using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.DRS;

namespace NvAPIWrapper.DRS
{
    /// <summary>
    ///     Contains information about a setting
    /// </summary>
    public class SettingInfo
    {
        private static uint[] _availableSettingIds;

        private SettingInfo(uint settingId)
        {
            SettingId = settingId;
        }

        /// <summary>
        ///     Gets an array of available possible valid values.
        /// </summary>
        public object[] AvailableValues
        {
            get
            {
                if (!IsAvailable)
                {
                    return null;
                }

                return DRSApi.EnumAvailableSettingValues(SettingId).Values;
            }
        }

        /// <summary>
        ///     Gets the default value of this setting
        /// </summary>
        public object DefaultValue
        {
            get
            {
                if (!IsAvailable)
                {
                    return null;
                }

                var values = DRSApi.EnumAvailableSettingValues(SettingId);

                return values.DefaultValue;
            }
        }

        /// <summary>
        ///     Gets a boolean value indicating if this setting is available on this machine and with the current version of NVIDIA
        ///     driver
        /// </summary>
        public bool IsAvailable
        {
            get => GetAvailableSetting().Any(info => info.SettingId == SettingId);
        }

        /// <summary>
        ///     Gets a boolean value indicating if this setting is know by this library
        /// </summary>
        public bool IsKnown
        {
            get => IsSettingKnown(SettingId);
        }

        /// <summary>
        ///     Gets the description of this setting from the library or <see langword="null" /> if this setting is not known by
        ///     the library.
        /// </summary>
        public string KnownDescription
        {
            get
            {
                if (!IsKnown || KnownSettingId == null)
                {
                    return null;
                }

                return GetSettingDescription(KnownSettingId.Value);
            }
        }

        /// <summary>
        ///     Gets the known identification number of this setting from the library or <see langword="null" /> if this setting is
        ///     not known by the library.
        /// </summary>
        public KnownSettingId? KnownSettingId
        {
            get
            {
                if (!IsKnown)
                {
                    return null;
                }

                return GetKnownSettingId(SettingId);
            }
        }

        /// <summary>
        ///     Gets the type of a static class or an enum containing possible known values for this setting from the library or
        ///     <see langword="null" /> if this setting is not known by the library
        /// </summary>
        public Type KnownValueType
        {
            get
            {
                if (!IsKnown || !IsAvailable)
                {
                    return null;
                }

                var name = KnownSettingId.ToString();
                var nameSpace = typeof(SettingInfo).Namespace + ".SettingValues";

                if (SettingType == DRSSettingType.Integer)
                {
                    return Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(type =>
                        type.IsEnum &&
                        type.Namespace?.Equals(nameSpace, StringComparison.InvariantCultureIgnoreCase) == true &&
                        type.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                }

                if (SettingType == DRSSettingType.String || SettingType == DRSSettingType.UnicodeString)
                {
                    return Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(type =>
                        type.IsClass &&
                        type.Namespace?.Equals(nameSpace, StringComparison.InvariantCultureIgnoreCase) == true &&
                        type.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                }

                return null;
            }
        }

        /// <summary>
        ///     Gets the name of the setting from NVIDIA driver or <see langword="null" /> if the setting is not available on this
        ///     machine.
        /// </summary>
        public string Name
        {
            get
            {
                if (!IsAvailable)
                {
                    return null;
                }

                return DRSApi.GetSettingNameFromId(SettingId);
            }
        }

        /// <summary>
        ///     Gets the setting identification number
        /// </summary>
        public uint SettingId { get; }

        /// <summary>
        ///     Gets the value type of the setting from NVIDIA driver or <see langword="null" /> if the setting is not available on
        ///     this machine.
        /// </summary>
        public DRSSettingType? SettingType
        {
            get
            {
                if (!IsAvailable)
                {
                    return null;
                }

                var values = DRSApi.EnumAvailableSettingValues(SettingId);

                return values.SettingType;
            }
        }

        /// <summary>
        ///     Gets information regarding a setting from its identification number.
        /// </summary>
        /// <param name="settingId">The identification number of the setting to get information about.</param>
        /// <returns>An instance of <see cref="SettingInfo" /> containing information about the setting.</returns>
        public static SettingInfo FromId(uint settingId)
        {
            return new SettingInfo(settingId);
        }

        /// <summary>
        ///     Gets information regarding a setting from its known identification number.
        /// </summary>
        /// <param name="settingId">The known identification number of the setting to get information about.</param>
        /// <returns>An instance of <see cref="SettingInfo" /> containing information about the setting.</returns>
        public static SettingInfo FromKnownSettingId(KnownSettingId settingId)
        {
            return FromId(GetSettingId(settingId));
        }

        /// <summary>
        ///     Gets information regarding a setting from its name.
        /// </summary>
        /// <param name="settingName">The name of the setting to get information about.</param>
        /// <returns>An instance of <see cref="SettingInfo" /> containing information about the setting.</returns>
        public static SettingInfo FromName(string settingName)
        {
            var settingId = DRSApi.GetSettingIdFromName(settingName);

            return FromId(settingId);
        }

        /// <summary>
        ///     Gets a list of all available setting on this machine
        /// </summary>
        /// <returns>Instances of <see cref="SettingInfo" /> each representing a available setting on this machine.</returns>
        public static SettingInfo[] GetAvailableSetting()
        {
            if (_availableSettingIds == null)
            {
                _availableSettingIds = DRSApi.EnumAvailableSettingIds();
            }

            return _availableSettingIds.Select(FromId).ToArray();
        }

        /// <summary>
        ///     Gets the known identification number of a setting from its identification number
        /// </summary>
        /// <param name="settingId">The setting identification number.</param>
        /// <returns>The known setting identification number if the setting is known; otherwise <see langword="null" />.</returns>
        public static KnownSettingId? GetKnownSettingId(uint settingId)
        {
            if (!IsSettingKnown(settingId))
            {
                return null;
            }

            return (KnownSettingId) settingId;
        }

        /// <summary>
        ///     Gets the known setting description from its identification number
        /// </summary>
        /// <param name="knownSettingId">The known setting identification number.</param>
        /// <returns>The known setting description if available; otherwise <see langword="null" />.</returns>
        public static string GetSettingDescription(KnownSettingId knownSettingId)
        {
            var enumName = Enum.GetName(typeof(KnownSettingId), knownSettingId);

            if (enumName == null)
            {
                return null;
            }

            var enumField = typeof(KnownSettingId).GetField(enumName);

            if (enumField == null)
            {
                return null;
            }

            var descriptionAttribute = enumField
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .OfType<DescriptionAttribute>()
                .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(descriptionAttribute?.Description))
            {
                return null;
            }

            return descriptionAttribute.Description;
        }

        /// <summary>
        ///     Gets the identification number of a setting from its known identification number
        /// </summary>
        /// <param name="knownSettingId">The known setting identification number.</param>
        /// <returns>The setting identification number.</returns>
        public static uint GetSettingId(KnownSettingId knownSettingId)
        {
            return (uint) knownSettingId;
        }

        /// <summary>
        ///     Checks if a setting is known by this library.
        /// </summary>
        /// <param name="settingId">The setting identification number.</param>
        /// <returns>true if setting is known by this library; otherwise false.</returns>
        public static bool IsSettingKnown(uint settingId)
        {
            return Enum.IsDefined(typeof(KnownSettingId), settingId);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            try
            {
                var settingName = Name;

                if (!string.IsNullOrWhiteSpace(settingName))
                {
                    return settingName;
                }
            }
            catch
            {
                // ignore;
            }

            return $"#{SettingId:X}";
        }

        /// <summary>
        ///     Tries to resolve the name of a known value using its actual value
        /// </summary>
        /// <param name="value">The actual value</param>
        /// <returns>The name of the known value member.</returns>
        public string ResolveKnownValueName(object value)
        {
            if (!IsKnown)
            {
                return null;
            }

            var valueType = KnownValueType;

            if (valueType == null)
            {
                return null;
            }

            if (valueType.IsEnum)
            {
                return Enum.GetName(valueType, value);
            }

            var comparerType = typeof(EqualityComparer<>).MakeGenericType(value.GetType());
            var comparer = comparerType.GetProperty(nameof(EqualityComparer<object>.Default))?.GetValue(null);

            if (!(comparer is IEqualityComparer equalityComparer))
            {
                return null;
            }

            return valueType.GetFields()
                .FirstOrDefault(info =>
                    info.IsStatic &&
                    equalityComparer.Equals(info.GetValue(null), value)
                )?.Name;
        }
    }
}