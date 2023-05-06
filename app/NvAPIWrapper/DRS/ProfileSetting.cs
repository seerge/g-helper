using System;
using NvAPIWrapper.Native.DRS;
using NvAPIWrapper.Native.DRS.Structures;

namespace NvAPIWrapper.DRS
{
    /// <summary>
    ///     Represents a profile setting and its value
    /// </summary>
    public class ProfileSetting
    {
        private readonly DRSSettingV1 _setting;

        internal ProfileSetting(DRSSettingV1 setting)
        {
            _setting = setting;
        }

        /// <summary>
        ///     Gets the current value of the setting
        /// </summary>
        public object CurrentValue
        {
            get
            {
                if (IsPredefinedValueValid && IsCurrentValuePredefined)
                {
                    return _setting.PredefinedValue;
                }

                return _setting.CurrentValue;
            }
        }

        /// <summary>
        ///     Gets a boolean value indicating if the current value is the predefined value.
        /// </summary>
        public bool IsCurrentValuePredefined
        {
            get => _setting.IsCurrentValuePredefined;
        }

        /// <summary>
        ///     Gets a boolean value indicating if this setting had a predefined valid value.
        /// </summary>
        public bool IsPredefinedValueValid
        {
            get => _setting.IsPredefinedValueValid;
        }

        /// <summary>
        ///     Gets the predefined value of this setting.
        /// </summary>
        public object PredefinedValue
        {
            get
            {
                if (!IsPredefinedValueValid)
                {
                    throw new InvalidOperationException("Predefined value is not valid.");
                }

                return _setting.PredefinedValue;
            }
        }

        /// <summary>
        ///     Gets the setting identification number
        /// </summary>
        public uint SettingId
        {
            get => _setting.Id;
        }

        /// <summary>
        ///     Gets additional information regarding this setting including possible valid values
        /// </summary>
        public SettingInfo SettingInfo
        {
            get => SettingInfo.FromId(SettingId);
        }

        /// <summary>
        ///     Gets the profile location of this setting
        /// </summary>
        public DRSSettingLocation SettingLocation
        {
            get => _setting.SettingLocation;
        }

        /// <summary>
        ///     Gets the value type of this setting
        /// </summary>
        public DRSSettingType SettingType
        {
            get => _setting.SettingType;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            string settingName = null;

            try
            {
                settingName = SettingInfo.Name;
            }
            catch
            {
                // ignore;
            }

            if (string.IsNullOrWhiteSpace(settingName))
            {
                settingName = $"#{SettingId:X}";
            }

            if (IsCurrentValuePredefined)
            {
                return $"{settingName} = {CurrentValue ?? "[NULL]"} (Predefined)";
            }

            return $"{settingName} = {CurrentValue ?? "[NULL]"}";
        }
    }
}