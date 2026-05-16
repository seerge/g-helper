using System.Runtime.InteropServices;

namespace GHelper.Helpers
{
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
                    try
                    {
                        await prerequisite.WaitAsync(timeout ?? TimeSpan.FromSeconds(3));
                    }
                    catch { }
                }

                Trim();
            });
        }

        private static void Trim()
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true, compacting: true);
            GC.WaitForPendingFinalizers();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true, compacting: true);

            using var p = System.Diagnostics.Process.GetCurrentProcess();
            SetProcessWorkingSetSize(p.Handle, -1, -1);
        }
    }
}
