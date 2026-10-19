namespace GHelper.Theme
{
    /// <summary>
    /// NOAA solar position algorithm. Pure arithmetic: time and coordinates in,
    /// elevation angle out. No dependency on WinForms, the registry or the clock,
    /// which is what keeps it verifiable without a test project.
    /// </summary>
    public static class SolarPosition
    {
        /// <summary>
        /// Sun elevation angle in degrees above the horizon (negative below) for
        /// the given UTC instant and position. Latitude and longitude are signed
        /// decimal degrees, north and east positive.
        /// </summary>
        public static double Elevation(DateTime utc, double latitude, double longitude)
        {
            double jd = JulianDay(utc);
            double t = (jd - 2451545.0) / 36525.0;

            // Geometric mean longitude and mean anomaly of the sun, degrees
            double l0 = Mod360(280.46646 + t * (36000.76983 + t * 0.0003032));
            double m = 357.52911 + t * (35999.05029 - t * 0.0001537);
            double e = 0.016708634 - t * (0.000042037 + t * 0.0000001267);

            double mRad = Rad(m);
            double center = Math.Sin(mRad) * (1.914602 - t * (0.004817 + t * 0.000014))
                          + Math.Sin(2 * mRad) * (0.019993 - t * 0.000101)
                          + Math.Sin(3 * mRad) * 0.000289;

            double trueLong = l0 + center;
            double omega = 125.04 - 1934.136 * t;
            double appLong = trueLong - 0.00569 - 0.00478 * Math.Sin(Rad(omega));

            double meanObliquity = 23.0 + (26.0 + ((21.448 - t * (46.815 + t * (0.00059 - t * 0.001813)))) / 60.0) / 60.0;
            double obliquity = meanObliquity + 0.00256 * Math.Cos(Rad(omega));

            double declination = Deg(Math.Asin(Math.Sin(Rad(obliquity)) * Math.Sin(Rad(appLong))));

            // Equation of time, minutes
            double y = Math.Tan(Rad(obliquity / 2.0));
            y *= y;
            double eqTime = 4.0 * Deg(
                y * Math.Sin(2.0 * Rad(l0))
                - 2.0 * e * Math.Sin(mRad)
                + 4.0 * e * y * Math.Sin(mRad) * Math.Cos(2.0 * Rad(l0))
                - 0.5 * y * y * Math.Sin(4.0 * Rad(l0))
                - 1.25 * e * e * Math.Sin(2.0 * mRad));

            double minutesUtc = utc.TimeOfDay.TotalMinutes;
            // Modulo 1440 keeps the hour angle in range across the UTC day boundary.
            double trueSolarTime = Mod(minutesUtc + eqTime + 4.0 * longitude, 1440.0);

            double hourAngle = trueSolarTime / 4.0 - 180.0;

            double latRad = Rad(latitude);
            double decRad = Rad(declination);
            double cosZenith = Math.Sin(latRad) * Math.Sin(decRad)
                             + Math.Cos(latRad) * Math.Cos(decRad) * Math.Cos(Rad(hourAngle));

            // Floating error puts this outside [-1, 1] at the poles.
            if (cosZenith > 1.0) cosZenith = 1.0;
            else if (cosZenith < -1.0) cosZenith = -1.0;

            return 90.0 - Deg(Math.Acos(cosZenith));
        }

        private static double JulianDay(DateTime utc)
        {
            int year = utc.Year;
            int month = utc.Month;
            double day = utc.Day + utc.TimeOfDay.TotalDays;

            if (month <= 2)
            {
                year -= 1;
                month += 12;
            }

            double a = Math.Floor(year / 100.0);
            double b = 2 - a + Math.Floor(a / 4.0);

            return Math.Floor(365.25 * (year + 4716))
                 + Math.Floor(30.6001 * (month + 1))
                 + day + b - 1524.5;
        }

        private static double Rad(double degrees) => degrees * Math.PI / 180.0;

        private static double Deg(double radians) => radians * 180.0 / Math.PI;

        private static double Mod360(double value) => Mod(value, 360.0);

        private static double Mod(double value, double modulus)
        {
            double r = value % modulus;
            return r < 0 ? r + modulus : r;
        }
    }
}
