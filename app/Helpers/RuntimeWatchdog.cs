using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace GHelper.Helpers
{
    /// <summary>
    /// G-Helper is a framework-dependent app that runs for days. When Windows Update / the .NET installer
    /// services the runtime (e.g. 10.0.13 -> 10.0.14), it deletes the old shared framework folders while the
    /// process is still using them. Any assembly that wasn't loaded yet (e.g. System.Diagnostics.PerformanceCounter)
    /// then fails with FileNotFoundException and the app crashes.
    /// On such a crash this restarts G-Helper, so it comes back up on the new runtime.
    /// Nothing runs until an exception actually happens.
    /// </summary>
    public static class RuntimeWatchdog
    {
        static string[] runtimeDirs = Array.Empty<string>();
        static int restarting;

        public static void Start()
        {
            try
            {
                runtimeDirs = new[]
                {
                    Path.GetDirectoryName(typeof(object).Assembly.Location),  // Microsoft.NETCore.App\x.y.z
                    Path.GetDirectoryName(typeof(Form).Assembly.Location),    // Microsoft.WindowsDesktop.App\x.y.z
                }
                .Where(dir => !string.IsNullOrEmpty(dir) && Directory.Exists(dir))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray()!;

                if (runtimeDirs.Length == 0) return;

                // JIT the crash path now, while the runtime files still exist
                const BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
                foreach (var name in new[] { nameof(OnUnhandledException), nameof(OnThreadException), nameof(TryRestart), nameof(IsRuntimeGone), nameof(IsReplacementInstalled), nameof(IsAssemblyServicedAway), nameof(Restart) })
                    RuntimeHelpers.PrepareMethod(typeof(RuntimeWatchdog).GetMethod(name, flags)!.MethodHandle);

                AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
                Application.ThreadException += OnThreadException;
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Runtime watchdog failed to start: " + ex.Message);
            }
        }

        // Background threads: process is terminating anyway, relaunch on the new runtime
        static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            TryRestart(e.ExceptionObject as Exception);
        }

        // UI thread: same as WinForms default behaviour, unless the runtime is gone
        static void OnThreadException(object sender, ThreadExceptionEventArgs e)
        {
            if (TryRestart(e.Exception)) return;

            Logger.WriteLine("Unhandled UI: " + e.Exception);
            using var dialog = new ThreadExceptionDialog(e.Exception);
            if (dialog.ShowDialog() == DialogResult.Abort) Application.Exit();
        }

        // Only when the runtime has vanished, otherwise a regular bug would turn into a restart loop
        static bool TryRestart(Exception? ex)
        {
            bool loadError = false;
            string? missingAssembly = null;
            for (var inner = ex; inner is not null; inner = inner.InnerException)
            {
                if (inner is FileNotFoundException fnf) missingAssembly ??= fnf.FileName;
                else if (inner is FileLoadException fle) missingAssembly ??= fle.FileName;
                else if (inner is not TypeLoadException) continue;
                loadError = true;
            }

            if (!loadError) return false;

            // Installer either removed the whole runtime folder, or only the files that weren't locked by us
            bool serviced = (IsRuntimeGone() && IsReplacementInstalled()) || IsAssemblyServicedAway(missingAssembly);
            if (!serviced) return false;

            Restart("crashed after .NET runtime update: " + ex!.GetType().Name + " " + missingAssembly);
            return true;
        }

        // Assembly is missing from our runtime folder, but present in another installed version of the same framework
        static bool IsAssemblyServicedAway(string? assembly)
        {
            if (string.IsNullOrEmpty(assembly)) return false;
            try
            {
                var file = new AssemblyName(assembly).Name + ".dll";
                foreach (var dir in runtimeDirs)
                {
                    if (File.Exists(Path.Combine(dir, file))) continue;

                    var frameworkDir = Path.GetDirectoryName(dir);
                    if (frameworkDir is null || !Directory.Exists(frameworkDir)) continue;

                    var major = Path.GetFileName(dir).Split('.')[0] + ".";
                    foreach (var candidate in Directory.GetDirectories(frameworkDir, major + "*"))
                        if (!string.Equals(candidate.TrimEnd('\\'), dir.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase) && File.Exists(Path.Combine(candidate, file)))
                            return true;
                }
            }
            catch
            {
            }
            return false;
        }

        static bool IsRuntimeGone()
        {
            foreach (var dir in runtimeDirs)
                if (!Directory.Exists(dir)) return true;
            return false;
        }

        // The apphost will pick the newest patch of the same major version on relaunch, make sure there is one
        static bool IsReplacementInstalled()
        {
            try
            {
                foreach (var dir in runtimeDirs)
                {
                    var frameworkDir = Path.GetDirectoryName(dir);
                    var major = Path.GetFileName(dir).Split('.')[0] + ".";
                    if (frameworkDir is null || !Directory.Exists(frameworkDir)) return false;

                    // Plain loops only: runtime files are already gone here, keep it to CoreLib
                    bool found = false;
                    foreach (var candidate in Directory.GetDirectories(frameworkDir, major + "*"))
                        if (Directory.GetFiles(candidate, "*.dll").Length > 0) { found = true; break; }
                    if (!found) return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        static void Restart(string reason)
        {
            if (Interlocked.Exchange(ref restarting, 1) == 1) return;

            try
            {
                Logger.WriteLine("Runtime watchdog: restarting, " + reason);
                // New instance signals this one to exit (or kills it) in ProcessHelper.CheckAlreadyRunning
                var exe = Environment.ProcessPath!;
                Process.Start(new ProcessStartInfo(exe)
                {
                    UseShellExecute = false,
                    WorkingDirectory = Path.GetDirectoryName(exe)
                })?.Dispose();
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Runtime watchdog: restart failed: " + ex.Message);
            }
        }
    }
}
