using System.Runtime.InteropServices;

namespace GHelper.Helpers
{
    /// <summary>
    /// Trims the process working set after heavy UI is closed.
    ///
    /// Safe to call from any thread. Runs asynchronously so it never blocks the UI.
    ///
    /// Downsides to be aware of:
    ///   - One-time CPU spike (~10-50 ms) from the compacting gen-2 GC sweep.
    ///   - Hard page faults on the NEXT open of any major form, because trimmed pages
    ///     have to be paged back in. In practice this is imperceptible for infrequent
    ///     user-initiated actions like opening a settings window.
    ///   - Should never be called on a hot path (timers, per-frame callbacks, etc.).
    /// </summary>
    public static class MemoryHelper
    {
        [DllImport("kernel32.dll")]
        private static extern bool SetProcessWorkingSetSize(IntPtr hProcess, nint dwMin, nint dwMax);

        /// <summary>
        /// Optionally waits for <paramref name="prerequisite"/> to complete (e.g. a background
        /// ETW pump task), then runs a full compacting GC and trims the OS working set.
        /// All work happens on a thread-pool thread — the caller is never blocked.
        /// </summary>
        public static void TrimAfter(Task? prerequisite = null, TimeSpan? timeout = null)
        {
            Task.Run(async () =>
            {
                if (prerequisite != null)
                {
                    try
                    {
                        await prerequisite.WaitAsync(timeout ?? TimeSpan.FromSeconds(3));
                    }
                    catch { /* timeout or cancellation — trim anyway */ }
                }

                Trim();
            });
        }

        private static void Trim()
        {
            // Two-pass: first pass collects gen-0/1, WaitForPendingFinalizers lets
            // finalizable objects (GDI handles, unmanaged wrappers) be released, then
            // the second compacting pass consolidates the heap before the OS trim.
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true, compacting: true);
            GC.WaitForPendingFinalizers();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true, compacting: true);

            // SetProcessWorkingSetSize(-1, -1) signals Windows to immediately evict all
            // non-essential pages from the process working set back to the standby list.
            using var p = System.Diagnostics.Process.GetCurrentProcess();
            SetProcessWorkingSetSize(p.Handle, -1, -1);
        }
    }
}
