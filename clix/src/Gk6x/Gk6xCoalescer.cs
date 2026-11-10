// Runs an action on a background task at most once per burst of requests (debounce), never
// concurrently with itself, and never more often than a minimum interval. No polling: the task
// exits when there is nothing pending.

using System;
using System.Threading.Tasks;

namespace GHelper.Peripherals.Keyboard.Gk6x
{
    public sealed class Gk6xCoalescer
    {
        private readonly Func<bool> action; // returns true when it actually wrote to the device
        private readonly int debounceMs;
        private readonly int minIntervalMs;
        private readonly Action<string> log;
        private readonly object stateLock = new();
        private bool pending;
        private bool running;
        private long lastWriteTick = long.MinValue / 2;

        public Gk6xCoalescer(Func<bool> action, int debounceMs, int minIntervalMs, Action<string>? log = null)
        {
            this.action = action;
            this.debounceMs = debounceMs;
            this.minIntervalMs = minIntervalMs;
            this.log = log ?? (_ => { });
        }

        /// <summary>Returns the worker task if a new one was started (for tests), otherwise null.</summary>
        public Task? Request()
        {
            lock (stateLock)
            {
                pending = true;
                if (running) return null;
                running = true;
            }
            return Task.Run(Worker);
        }

        private async Task Worker()
        {
            while (true)
            {
                await Task.Delay(debounceMs).ConfigureAwait(false);
                lock (stateLock)
                {
                    if (!pending) { running = false; return; }
                    pending = false;
                }

                long wait = minIntervalMs - (Environment.TickCount64 - lastWriteTick);
                if (wait > 0) await Task.Delay((int)wait).ConfigureAwait(false);

                try
                {
                    if (action()) lastWriteTick = Environment.TickCount64;
                }
                catch (Exception e)
                {
                    log("GK6X: sync failed: " + e.Message);
                }
            }
        }
    }
}
