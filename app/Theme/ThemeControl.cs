namespace GHelper.Theme
{
    /// <summary>
    /// Wires the solar layer to the registry. Evaluates on startup, session
    /// switch, resume and clock change, arms exactly one timer for the next
    /// crossing, and persists one value: the UTC instant of the last crossing it
    /// applied. Between crossings it never writes, which is what makes a manual
    /// toggle stand.
    /// </summary>
    public class ThemeControl
    {
        private readonly System.Windows.Forms.Timer timer = new();
        private DateTime? nextSwitchUtc;

        // The broadcast in WindowsTheme.Apply blocks its thread for seconds while it
        // waits for every top-level window to acknowledge, so applies run off the UI
        // thread: on it, the window cannot pump the very message the broadcast waits
        // for, and a keypress took 7-11s to land. writeLock serialises the writes;
        // stateLock guards the last requested theme, so two quick toggles cannot both
        // read the same registry value and coalesce into one.
        private readonly object writeLock = new();
        private readonly object stateLock = new();
        private bool? requestedLight;

        /// <summary>
        /// The armed crossing in local time, or null when the schedule is off or
        /// there is no crossing within 24 hours.
        /// </summary>
        public DateTime? NextSwitchLocal => nextSwitchUtc?.ToLocalTime();

        /// <summary>
        /// Raised after any apply, scheduled or manual, so the UI can follow.
        /// </summary>
        public event Action? ThemeApplied;

        public ThemeControl()
        {
            timer.Tick += OnTick;
        }

        /// <summary>
        /// First evaluation and timer arm. Called once at startup.
        /// </summary>
        public void Init()
        {
            Logger.WriteLine($"Theme: schedule is {(ThemeConfig.ScheduleEnabled ? "enabled" : "disabled")}");
            Evaluate();
        }

        /// <summary>
        /// Answers "what should the theme be right now?" and arms the next
        /// crossing. Safe to call from any system-event handler. Never throws.
        /// </summary>
        public void Evaluate()
        {
            try
            {
                if (!ThemeConfig.ScheduleEnabled)
                {
                    Disarm();
                    nextSwitchUtc = null;
                    return;
                }

                SolarSite site = ThemeConfig.Site;
                DateTime now = DateTime.UtcNow;

                DateTime? last = ThemeSchedule.PreviousCrossing(now, site);
                DateTime? recorded = ThemeConfig.LastCrossing;
                bool light = ThemeSchedule.IsLight(now, site);

                if (recorded is null)
                {
                    // First run: land on the correct theme instead of waiting up to
                    // twelve hours for the next crossing.
                    Apply(light);
                    ThemeConfig.LastCrossing = last ?? now;
                    Logger.WriteLine($"Theme: first evaluation applied {Name(light)}, recorded crossing {ThemeConfig.LastCrossing:O}");
                }
                else if (last is not null && last > recorded)
                {
                    Apply(light);
                    ThemeConfig.LastCrossing = last;
                    Logger.WriteLine($"Theme: crossing at {last:O} is newer than the recorded {recorded:O}, applied {Name(light)}");
                }
                else
                {
                    Logger.WriteLine($"Theme: no unapplied crossing (recorded {recorded:O}, latest {last:O}), leaving the theme alone");
                }

                Arm(now, site);
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Theme: evaluation failed ({ex.Message})");
            }
        }

        /// <summary>
        /// Manual toggle. Deliberately does not touch the recorded crossing, so a
        /// hand-made choice stands until the schedule next disagrees with it.
        /// Returns the theme it set, true for light.
        /// </summary>
        public bool Toggle()
        {
            bool light;
            lock (stateLock) light = !(requestedLight ?? WindowsTheme.IsLight());

            Logger.WriteLine($"Theme: manual toggle to {Name(light)}");
            Apply(light);
            return light;
        }

        private void Apply(bool light)
        {
            lock (stateLock) requestedLight = light;

            Task.Run(() =>
            {
                lock (writeLock) WindowsTheme.Apply(light);
                RaiseThemeApplied();
            });
        }

        /// <summary>
        /// Raises ThemeApplied on the UI thread, so handlers can touch controls.
        /// </summary>
        private void RaiseThemeApplied()
        {
            Action? handler = ThemeApplied;
            if (handler is null) return;

            try
            {
                Form? form = Program.settingsForm;
                if (form is not null && !form.IsDisposed && form.InvokeRequired) form.BeginInvoke(handler);
                else handler();
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Theme: could not refresh the UI after an apply ({ex.Message})");
            }
        }

        private void Arm(DateTime now, SolarSite site)
        {
            Disarm();

            DateTime? next = ThemeSchedule.NextCrossing(now, site);
            // No crossing within 24 hours - polar day or night. Re-arm at local
            // midnight rather than not at all, so the search runs again tomorrow.
            DateTime target = next ?? DateTime.Now.Date.AddDays(1).ToUniversalTime();

            nextSwitchUtc = next;

            // Two seconds of slack keeps the level test on the far side of the crossing.
            double milliseconds = (target - now).TotalMilliseconds + 2000;
            if (milliseconds < 1000) milliseconds = 1000;
            if (milliseconds > int.MaxValue) milliseconds = int.MaxValue;

            timer.Interval = (int)milliseconds;
            timer.Start();

            Logger.WriteLine(next is null
                ? $"Theme: no crossing within 24h, re-arming at local midnight ({target.ToLocalTime():HH:mm})"
                : $"Theme: armed for the next crossing at {target.ToLocalTime():yyyy-MM-dd HH:mm:ss} local");
        }

        private void Disarm() => timer.Stop();

        private void OnTick(object? sender, EventArgs e)
        {
            timer.Stop();
            Logger.WriteLine("Theme: scheduled crossing reached");
            Evaluate();
        }

        private static string Name(bool light) => light ? "light" : "dark";
    }
}
