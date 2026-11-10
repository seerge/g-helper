using Microsoft.Win32;
using System.Diagnostics;

namespace GHelper.Mode
{
    public class Routine
    {
        public string Process = "";
        public string Title = "";
        public int Mode = AsusACPI.PerformanceTurbo;

        public string Name => Title.Length > 0 ? Process + " (" + Title + ")" : Process;
    }

    internal class RoutineControl
    {
        private readonly System.Timers.Timer timer = new(3000);

        private bool locked = false;
        private int applied = -1;

        public string Status { get; private set; } = "";

        public RoutineControl()
        {
            timer.Elapsed += (sender, e) => Check();
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
        }

        public void Init()
        {
            timer.Enabled = IsEnabled();
            Check();
        }

        // Power source changes and resume load the mode saved for that source, so routines need to win again afterwards
        public void Reapply()
        {
            lock (timer) applied = -1;
            Check();
        }

        public static bool IsEnabled()
        {
            return AppConfig.Get("routine_lock") >= 0 || GetRoutines().Count > 0;
        }

        // Stored as "process|title|mode;process|title|mode"
        public static List<Routine> GetRoutines()
        {
            var routines = new List<Routine>();
            string? config = AppConfig.GetString("routine_apps");
            if (config is null) return routines;

            foreach (string item in config.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                string[] parts = item.Split('|');
                if (parts.Length < 3 || !int.TryParse(parts[2], out int mode)) continue;
                routines.Add(new Routine { Process = parts[0], Title = parts[1], Mode = mode });
            }

            return routines;
        }

        public static void SetRoutines(List<Routine> routines)
        {
            AppConfig.Set("routine_apps", string.Join(";", routines.Select(r => r.Process + "|" + r.Title.Replace("|", "").Replace(";", "") + "|" + r.Mode)));
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock) locked = true;
            else if (e.Reason == SessionSwitchReason.SessionUnlock || e.Reason == SessionSwitchReason.SessionLogon) locked = false;
            else return;

            Check();
        }

        private static bool IsRunning(Routine routine)
        {
            var processes = Process.GetProcessesByName(routine.Process);
            bool running = routine.Title.Length == 0
                ? processes.Length > 0
                : processes.Any(p => p.MainWindowTitle.Contains(routine.Title, StringComparison.OrdinalIgnoreCase));

            foreach (var process in processes) process.Dispose();
            return running;
        }

        private int GetTarget()
        {
            int lockMode = AppConfig.Get("routine_lock");
            if (locked && lockMode >= 0)
            {
                Status = Properties.Strings.RoutineLocked;
                return lockMode;
            }

            foreach (var routine in GetRoutines())
            {
                if (IsRunning(routine))
                {
                    Status = routine.Name;
                    return routine.Mode;
                }
            }

            Status = "";
            return -1;
        }

        public void Check()
        {
            lock (timer)
            {
                try
                {
                    int target = GetTarget();
                    int previous = AppConfig.Get("routine_previous");

                    if (target >= 0 && Modes.Exists(target))
                    {
                        // Remember the mode user had before routines took over, kept in config to survive restarts
                        if (previous < 0) AppConfig.Set("routine_previous", Modes.GetCurrent());
                        if (target != applied && target != Modes.GetCurrent()) SetMode(target, false);
                        applied = target;
                    }
                    else if (previous >= 0)
                    {
                        AppConfig.Remove("routine_previous");
                        if (Modes.GetCurrent() != previous) SetMode(previous, true);
                        applied = -1;
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteLine("Routines: " + ex.Message);
                }
            }
        }

        // remember: false keeps the mode saved for the current power source untouched,
        // so a routine doesn't replace the mode user picked for battery / charger
        private void SetMode(int mode, bool remember)
        {
            Logger.WriteLine("Routine: " + (Status.Length > 0 ? Status : "Restore") + " -> " + Modes.GetName(mode));
            string sourceKey = "performance_" + Program.PerformanceKey();
            int sourceMode = AppConfig.Get(sourceKey);

            Program.settingsForm.Invoke(delegate
            {
                Program.modeControl.SetPerformanceMode(mode, true);
            });

            if (remember) return;
            if (sourceMode >= 0) AppConfig.Set(sourceKey, sourceMode);
            else AppConfig.Remove(sourceKey);
        }
    }
}
