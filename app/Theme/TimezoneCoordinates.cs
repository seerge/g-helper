namespace GHelper.Theme
{
    /// <summary>
    /// IANA timezone to a representative coordinate. Deliberately rough: a zone
    /// says little about latitude, and this exists only so the schedule does
    /// something sensible before anyone opens the settings dialog. The dialog is
    /// how coordinates become accurate.
    /// </summary>
    public static class TimezoneCoordinates
    {
        private static readonly Dictionary<string, (double Latitude, double Longitude)> Zones =
            new(StringComparer.OrdinalIgnoreCase)
        {
            // North America
            { "America/New_York", (40.71, -74.01) },
            { "America/Toronto", (43.65, -79.38) },
            { "America/Detroit", (42.33, -83.05) },
            { "America/Chicago", (41.85, -87.65) },
            { "America/Winnipeg", (49.90, -97.14) },
            { "America/Mexico_City", (19.43, -99.13) },
            { "America/Denver", (39.74, -104.98) },
            { "America/Phoenix", (33.45, -112.07) },
            { "America/Edmonton", (53.55, -113.49) },
            { "America/Los_Angeles", (34.05, -118.24) },
            { "America/Vancouver", (49.28, -123.12) },
            { "America/Anchorage", (61.22, -149.90) },
            { "Pacific/Honolulu", (21.31, -157.86) },
            { "America/Halifax", (44.65, -63.57) },

            // South America
            { "America/Sao_Paulo", (-23.55, -46.63) },
            { "America/Argentina/Buenos_Aires", (-34.60, -58.38) },
            { "America/Bogota", (4.71, -74.07) },
            { "America/Santiago", (-33.45, -70.67) },

            // Europe
            { "Europe/London", (51.51, -0.13) },
            { "Europe/Dublin", (53.35, -6.26) },
            { "Europe/Lisbon", (38.72, -9.14) },
            { "Europe/Madrid", (40.42, -3.70) },
            { "Europe/Paris", (48.86, 2.35) },
            { "Europe/Brussels", (50.85, 4.35) },
            { "Europe/Amsterdam", (52.37, 4.90) },
            { "Europe/Berlin", (52.52, 13.41) },
            { "Europe/Zurich", (47.38, 8.54) },
            { "Europe/Rome", (41.90, 12.50) },
            { "Europe/Vienna", (48.21, 16.37) },
            { "Europe/Prague", (50.08, 14.44) },
            { "Europe/Warsaw", (52.23, 21.01) },
            { "Europe/Stockholm", (59.33, 18.07) },
            { "Europe/Oslo", (59.91, 10.75) },
            { "Europe/Helsinki", (60.17, 24.94) },
            { "Europe/Athens", (37.98, 23.73) },
            { "Europe/Istanbul", (41.01, 28.98) },
            { "Europe/Kyiv", (50.45, 30.52) },
            { "Europe/Moscow", (55.76, 37.62) },

            // Middle East and Africa
            { "Asia/Jerusalem", (31.77, 35.21) },
            { "Asia/Dubai", (25.20, 55.27) },
            { "Africa/Cairo", (30.04, 31.24) },
            { "Africa/Lagos", (6.52, 3.38) },
            { "Africa/Johannesburg", (-26.20, 28.05) },

            // Asia
            { "Asia/Karachi", (24.86, 67.01) },
            { "Asia/Kolkata", (19.08, 72.88) },
            { "Asia/Dhaka", (23.81, 90.41) },
            { "Asia/Bangkok", (13.76, 100.50) },
            { "Asia/Jakarta", (-6.21, 106.85) },
            { "Asia/Singapore", (1.35, 103.82) },
            { "Asia/Shanghai", (31.23, 121.47) },
            { "Asia/Hong_Kong", (22.32, 114.17) },
            { "Asia/Taipei", (25.03, 121.57) },
            { "Asia/Seoul", (37.57, 126.98) },
            { "Asia/Tokyo", (35.68, 139.69) },

            // Oceania
            { "Australia/Perth", (-31.95, 115.86) },
            { "Australia/Adelaide", (-34.93, 138.60) },
            { "Australia/Brisbane", (-27.47, 153.03) },
            { "Australia/Melbourne", (-37.81, 144.96) },
            { "Australia/Sydney", (-33.87, 151.21) },
            { "Pacific/Auckland", (-36.85, 174.76) },
        };

        /// <summary>
        /// The representative coordinate for an IANA zone id, or (0, 0) for an
        /// unknown or null zone.
        /// </summary>
        public static (double Latitude, double Longitude) ForIanaZone(string? ianaId)
        {
            if (string.IsNullOrWhiteSpace(ianaId)) return (0.0, 0.0);
            return Zones.TryGetValue(ianaId.Trim(), out var coordinate) ? coordinate : (0.0, 0.0);
        }
    }
}
