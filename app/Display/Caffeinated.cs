using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Timers;
using static System.Windows.Forms.AxHost;

namespace GHelper.Display
{
    public static class Caffeinated
    {
        private static bool isActivated = false;
        private static DateTime? endTime;
        private static readonly System.Timers.Timer timer = new();
        private const int DefaultDuration = 480; // Default duration in minutes (8 hours)
        public static event EventHandler? CaffeinatedStateChanged;

        static Caffeinated()
        {
            timer.Elapsed += Timer_Elapsed;
        }

        public static bool IsActive => isActivated;
        public static DateTime? EndTime => endTime;
        public static int CustomCaffeinatedDuration => AppConfig.Get("caffeinated_duration", DefaultDuration);

        private static bool Activate()
        {
            int durationInMinutes = CustomCaffeinatedDuration;
            uint sleepDisabled = NativeMethods.ES_CONTINUOUS | NativeMethods.ES_DISPLAY_REQUIRED;
            uint previousState = NativeMethods.SetThreadExecutionState(sleepDisabled);

            if (previousState == 0)
            {
                Debug.WriteLine("Call to SetThreadExecutionState failed.");
                return false;
            }

            int timerIntervalInMilliseconds = durationInMinutes * 60 * 1000;

            if (timerIntervalInMilliseconds > 0)
            {
                timer.Interval = timerIntervalInMilliseconds;
                timer.Start();
                endTime = DateTime.Now.AddMilliseconds(timerIntervalInMilliseconds);
            }
            else
            {
                // Duration of 0 means indefinite
                endTime = null;
            }

            isActivated = true;
            CaffeinatedStateChanged?.Invoke(null, EventArgs.Empty);
            return true;
        }

        private static bool Deactivate()
        {
            timer.Stop();

            uint result = NativeMethods.SetThreadExecutionState(NativeMethods.ES_CONTINUOUS);
            if (result == 0)
            {
                Debug.WriteLine("Call to SetThreadExecutionState failed.");
                return false;
            }

            isActivated = false;
            CaffeinatedStateChanged?.Invoke(null, EventArgs.Empty);
            endTime = null;
            return true;
        }

        public static string GetStatus()
        {
            if (!isActivated)
            {
                return "Sleep allowed";
            }

            if (endTime == null)
            {
                return "No sleep indefinitely";
            }

            // Calculate time remaining
            TimeSpan remaining = endTime.Value - DateTime.Now;
            if (remaining.TotalSeconds <= 0)
            {
                return "Sleep allowed";
            }

            return $"No sleep for {FormatTimeRemaining(remaining)}";
        }

        private static string FormatTimeRemaining(TimeSpan time)
        {
            if (time.TotalHours >= 1)
            {
                return $"{Math.Floor(time.TotalHours)} hour{(Math.Floor(time.TotalHours) != 1 ? "s" : "")} {time.Minutes} minute{(time.Minutes != 1 ? "s" : "")}";
            }
            else if (time.TotalMinutes >= 1)
            {
                return $"{time.Minutes} minute{(time.Minutes != 1 ? "s" : "")} {time.Seconds} second{(time.Seconds != 1 ? "s" : "")}";
            }
            else
            {
                return $"{time.Seconds} second{(time.Seconds != 1 ? "s" : "")}";
            }
        }
        public static void Toggle()
        {
            if (isActivated)
                Deactivate();
            else
                Activate();
        }
        private static void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            // Marshaling the Deactivate() call to the UI thread
            Program.settingsForm.BeginInvoke(Deactivate);
            Logger.WriteLine("Caffeinated Deactivated (Time Expiration)");
        }

    }

    //// Native methods for Windows API calls
    //internal static class NativeMethods
    //{
    //    // ES_CONTINUOUS flag - Informs the system that the state being set should remain in effect until the next call
    //    public const uint ES_CONTINUOUS = 0x80000000;

    //    // ES_SYSTEM_REQUIRED - Forces the system to be in the working state
    //    public const uint ES_SYSTEM_REQUIRED = 0x00000001;

    //    // ES_DISPLAY_REQUIRED - Forces the display to be on
    //    public const uint ES_DISPLAY_REQUIRED = 0x00000002;

    //    // ES_AWAYMODE_REQUIRED - Enables away mode, prevents sleep idle timeout
    //    public const uint ES_AWAYMODE_REQUIRED = 0x00000040;

    //    // SetThreadExecutionState - Function that prevents the system from entering sleep or turning off the display
    //    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    //    public static extern uint SetThreadExecutionState(uint esFlags);
    //}
}


