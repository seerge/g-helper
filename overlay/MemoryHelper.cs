using System.Runtime.InteropServices;

namespace GHelperOverlay;

public static class MemoryHelper
{
    [DllImport("kernel32.dll")]
    private static extern bool SetProcessWorkingSetSize(IntPtr hProcess, nint dwMin, nint dwMax);

    public static void TrimAfter(Task? prerequisite = null, TimeSpan? timeout = null)
    {
        Task.Run(async () =>
        {
            if (prerequisite != null)
            {
                // Task.WaitAsync isn't on net48 — race the task against a delay instead.
                var t = timeout ?? TimeSpan.FromSeconds(3);
                try { await Task.WhenAny(prerequisite, Task.Delay(t)); }
                catch { }
            }
            Trim();
        });
    }

    private static void Trim()
    {
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);
        GC.WaitForPendingFinalizers();
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);

        using var p = System.Diagnostics.Process.GetCurrentProcess();
        SetProcessWorkingSetSize(p.Handle, -1, -1);
    }
}
