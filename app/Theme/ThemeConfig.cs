using System.Globalization;

namespace GHelper.Theme
{
    /// <summary>
    /// The five config keys this feature owns. The bare key "theme" belongs to
    /// upstream (RForm.InitColors) and is deliberately not reused.
    /// </summary>
    public static class ThemeConfig
    {
        public const int DefaultThresholdDegrees = 10;
        private const int MinThresholdDegrees = 0;
        private const int MaxThresholdDegrees = 20;

        /// <summary>
        /// The schedule's on/off switch. Ships off: a feature that starts moving
        /// the theme on its own without being asked is the first anti-goal.
        /// </summary>
        public static bool ScheduleEnabled
        {
            get => AppConfig.Is("theme_schedule");
            set => AppConfig.Set("theme_schedule", value ? 1 : 0);
        }

        /// <summary>
        /// The configured position and threshold. On first read, or when either
        /// coordinate is absent or unparseable, seeds from the system timezone and
        /// persists the seed. Never throws.
        /// </summary>
        public static SolarSite Site
        {
            get
            {
                int threshold = Math.Clamp(AppConfig.Get("theme_elevation", DefaultThresholdDegrees),
                                           MinThresholdDegrees, MaxThresholdDegrees);

                if (TryParseCoordinate(AppConfig.GetString("theme_lat"), 90, out double latitude)
                    && TryParseCoordinate(AppConfig.GetString("theme_lon"), 180, out double longitude))
                    return new SolarSite(latitude, longitude, threshold);

                (latitude, longitude) = SeedFromTimezone();
                PersistCoordinates(latitude, longitude);
                return new SolarSite(latitude, longitude, threshold);
            }
        }

        /// <summary>
        /// Persists a position and threshold, rejecting out-of-range values.
        /// </summary>
        public static void SetSite(double latitude, double longitude, int thresholdDegrees)
        {
            if (!IsValidCoordinate(latitude, 90)) throw new ArgumentOutOfRangeException(nameof(latitude));
            if (!IsValidCoordinate(longitude, 180)) throw new ArgumentOutOfRangeException(nameof(longitude));
            if (thresholdDegrees < MinThresholdDegrees || thresholdDegrees > MaxThresholdDegrees)
                throw new ArgumentOutOfRangeException(nameof(thresholdDegrees));

            PersistCoordinates(latitude, longitude);
            AppConfig.Set("theme_elevation", thresholdDegrees);
        }

        /// <summary>
        /// The UTC instant of the last crossing applied — the feature's entire
        /// persisted state. Null when nothing has been applied yet.
        /// </summary>
        public static DateTime? LastCrossing
        {
            get
            {
                string? stored = AppConfig.GetString("theme_last_crossing");
                if (string.IsNullOrWhiteSpace(stored)) return null;

                // RoundtripKind cannot be combined with AdjustToUniversal - that pair
                // throws - so normalise the parsed kind by hand instead.
                if (!DateTime.TryParse(stored, CultureInfo.InvariantCulture,
                        DateTimeStyles.RoundtripKind, out DateTime parsed)) return null;

                return parsed.Kind switch
                {
                    DateTimeKind.Utc => parsed,
                    DateTimeKind.Local => parsed.ToUniversalTime(),
                    _ => DateTime.SpecifyKind(parsed, DateTimeKind.Utc),
                };
            }
            set
            {
                if (value is null) AppConfig.Remove("theme_last_crossing");
                else AppConfig.Set("theme_last_crossing",
                        DateTime.SpecifyKind(value.Value, DateTimeKind.Utc).ToString("O", CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// Invariant-culture coordinate parse, rejecting NaN, infinity and values
        /// beyond <paramref name="limit"/>. Used by the config reader and by the
        /// settings dialog's validation, so both agree on what is acceptable.
        /// </summary>
        public static bool TryParseCoordinate(string? text, double limit, out double value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(text)) return false;

            if (!double.TryParse(text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double parsed))
                return false;

            if (!IsValidCoordinate(parsed, limit)) return false;

            value = parsed;
            return true;
        }

        private static bool IsValidCoordinate(double value, double limit)
            => !double.IsNaN(value) && !double.IsInfinity(value) && Math.Abs(value) <= limit;

        private static (double Latitude, double Longitude) SeedFromTimezone()
        {
            try
            {
                string windowsId = TimeZoneInfo.Local.Id;
                if (!TimeZoneInfo.TryConvertWindowsIdToIanaId(windowsId, out string? ianaId))
                    ianaId = windowsId;

                var seed = TimezoneCoordinates.ForIanaZone(ianaId);
                Logger.WriteLine($"Theme: seeding coordinates from timezone {windowsId} ({ianaId}) -> {seed.Latitude}, {seed.Longitude}");
                return seed;
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Theme: timezone seeding failed, using 0,0 ({ex.Message})");
                return (0.0, 0.0);
            }
        }

        private static void PersistCoordinates(double latitude, double longitude)
        {
            AppConfig.Set("theme_lat", latitude.ToString(CultureInfo.InvariantCulture));
            AppConfig.Set("theme_lon", longitude.ToString(CultureInfo.InvariantCulture));
        }
    }
}
