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
        // for, and a keypress took 7-11s to land. One worker drains them, so the last
        // theme asked for is the last one written - queued tasks contending for a lock
        // can be granted it in any order and land backwards.
        private readonly object stateLock = new();
        private (bool Light, Action? OnSuccess)? pending;
        private bool working;

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
            // SystemEvents raises PowerModeChanged, SessionSwitch and TimeChanged on
            // its own thread, and System.Windows.Forms.Timer is a hidden-window timer
            // that only its owning thread may start or stop. Get onto the UI thread
            // before touching it. At startup the handle does not exist yet and we are
            // already on that thread, so the call runs inline.
            Form? form = Program.settingsForm;
            if (form is not null && !form.IsDisposed && form.IsHandleCreated && form.InvokeRequired)
            {
                form.BeginInvoke(EvaluateCore);
                return;
            }

            EvaluateCore();
        }

        private void EvaluateCore()
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

                // A crossing recorded ahead of now means the clock moved, not that the
                // future already happened. Left alone it outranks every real crossing
                // until time catches up, and the schedule goes quiet for that long.
                if (recorded is not null && recorded > now)
                {
                    Logger.WriteLine($"Theme: recorded crossing {recorded:O} is in the future, discarding it (clock change?)");
                    recorded = null;
                }

                if (recorded is null)
                {
                    // First run: land on the correct theme instead of waiting up to
                    // twelve hours for the next crossing.
                    DateTime crossing = last ?? now;
                    Apply(light, () => Record(crossing, $"first evaluation applied {Name(light)}"));
                }
                else if (last is not null && last > recorded)
                {
                    DateTime crossing = last.Value;
                    DateTime was = recorded.Value;
                    Apply(light, () => Record(crossing, $"crossing is newer than the recorded {was:O}, applied {Name(light)}"));
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
            // Toggle away from whatever is still queued; once the worker has taken it
            // the registry is the truth again. Holding the last applied value instead
            // would go stale the moment the theme changed outside G-Helper, and the
            // next press would be a no-op.
            bool light;
            lock (stateLock) light = !(pending?.Light ?? WindowsTheme.IsLight());

            Logger.WriteLine($"Theme: manual toggle to {Name(light)}");
            Apply(light);
            return light;
        }

        private void Apply(bool light, Action? onSuccess = null)
        {
            lock (stateLock)
            {
                // A superseded apply hands its bookkeeping on rather than dropping it,
                // so a manual toggle landing on top of a scheduled one still records
                // the crossing that scheduled one was for.
                pending = (light, onSuccess ?? pending?.OnSuccess);
                if (working) return;
                working = true;
            }

            Task.Run(Drain);
        }

        /// <summary>
        /// Writes pending themes one at a time until none is left. Only the latest
        /// request survives, so a burst of toggles costs one broadcast, not one each.
        /// </summary>
        private void Drain()
        {
            while (true)
            {
                bool light;
                Action? onSuccess;

                lock (stateLock)
                {
                    if (pending is null)
                    {
                        working = false;
                        return;
                    }

                    (light, onSuccess) = pending.Value;
                    pending = null;
                }

                if (WindowsTheme.Apply(light)) onSuccess?.Invoke();
                RaiseThemeApplied();
            }
        }

        private void Record(DateTime crossing, string what)
        {
            ThemeConfig.LastCrossing = crossing;
            Logger.WriteLine($"Theme: {what}, recorded crossing {crossing:O}");
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
                // InvokeRequired is false before the handle exists, which would run the
                // handler right here on the pool thread. SettingsForm paints the glyph
                // in its own constructor, so there is nothing to lose by skipping.
                Form? form = Program.settingsForm;
                if (form is null || form.IsDisposed || !form.IsHandleCreated) return;

                if (form.InvokeRequired) form.BeginInvoke(handler);
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
