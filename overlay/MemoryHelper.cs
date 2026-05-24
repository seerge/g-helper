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
                try { await prerequisite.WaitAsync(timeout ?? TimeSpan.FromSeconds(3)); }
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
