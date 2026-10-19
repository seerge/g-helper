namespace GHelper.Theme
{
    /// <summary>
    /// A position and the sun elevation at which the theme turns light.
    /// </summary>
    public readonly record struct SolarSite(double Latitude, double Longitude, double ThresholdDegrees);

    /// <summary>
    /// Answers "what should the theme be right now?" and "when does that answer
    /// next change?" over <see cref="SolarPosition"/>. Pure: time in, answers out.
    /// </summary>
    public static class ThemeSchedule
    {
        private const int SearchMinutes = 24 * 60;
        private const int BisectSteps = 6;

        /// <summary>
        /// True when the sun is at or above the site's threshold. The threshold
        /// itself counts as light, so the predicate has no undefined point.
        /// </summary>
        public static bool IsLight(DateTime utc, SolarSite site)
            => SolarPosition.Elevation(utc, site.Latitude, site.Longitude) >= site.ThresholdDegrees;

        /// <summary>
        /// The first instant after <paramref name="utc"/> where <see cref="IsLight"/>
        /// differs from its value at <paramref name="utc"/>, searched up to 24 hours
        /// ahead. Null when there is none — polar day or polar night.
        /// </summary>
        public static DateTime? NextCrossing(DateTime utc, SolarSite site)
        {
            bool current = IsLight(utc, site);
            DateTime low = utc;

            for (int minute = 1; minute <= SearchMinutes; minute++)
            {
                DateTime high = utc.AddMinutes(minute);
                if (IsLight(high, site) != current) return Bisect(low, high, site);
                low = high;
            }

            return null;
        }

        /// <summary>
        /// The most recent instant at or before <paramref name="utc"/> where the
        /// value changed to what it is now, searched up to 24 hours back. Null when
        /// there is none.
        /// </summary>
        public static DateTime? PreviousCrossing(DateTime utc, SolarSite site)
        {
            bool current = IsLight(utc, site);
            DateTime high = utc;

            for (int minute = 1; minute <= SearchMinutes; minute++)
            {
                DateTime low = utc.AddMinutes(-minute);
                if (IsLight(low, site) != current) return Bisect(low, high, site);
                high = low;
            }

            return null;
        }

        /// <summary>
        /// Narrows a one-minute bracket whose ends disagree down to the earliest
        /// instant carrying the later end's value. One minute cannot hide a
        /// crossing: elevation near any usable threshold moves monotonically and
        /// far slower than that.
        /// </summary>
        private static DateTime Bisect(DateTime low, DateTime high, SolarSite site)
        {
            bool target = IsLight(high, site);

            for (int i = 0; i < BisectSteps; i++)
            {
                DateTime mid = low.AddTicks((high - low).Ticks / 2);
                if (IsLight(mid, site) == target) high = mid;
                else low = mid;
            }

            DateTime second = new DateTime(high.Ticks - high.Ticks % TimeSpan.TicksPerSecond, DateTimeKind.Utc);
            // Truncating down can land a tick before the crossing; step forward so a
            // returned crossing instant always carries the value it crossed into.
            return IsLight(second, site) == target ? second : second.AddSeconds(1);
        }
    }
}
