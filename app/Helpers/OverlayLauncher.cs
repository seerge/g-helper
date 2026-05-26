using System.Diagnostics;
using GHelper.Overlay;

namespace GHelper.Helpers;

public static class OverlayLauncher
{
    public static string? ExternalPath
    {
        get
        {
            var p = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                "GHelper", "GHelperOverlay.exe");
            return File.Exists(p) ? p : null;
        }
    }

    public static void Start()
    {
        var path = ExternalPath;
        if (path != null)
        {
            try { Program.hardwareOverlay?.StopOverlay(); } catch { }
            if (!IsExternalRunning())
                try { Process.Start(new ProcessStartInfo(path) { UseShellExecute = true }); }
                catch (Exception ex) { Logger.WriteLine("OverlayLauncher external start failed: " + ex.Message); }
        }
        else
        {
            Program.hardwareOverlay?.StartOverlay();
        }
    }

    public static void Stop()
    {
        foreach (var p in Process.GetProcessesByName("GHelperOverlay"))
        {
            try { p.Kill(); }
            catch (Exception ex) { Logger.WriteLine("OverlayLauncher kill failed: " + ex.Message); }
            finally { p.Dispose(); }
        }
        try { Program.hardwareOverlay?.StopOverlay(); } catch { }
    }

    private static bool IsExternalRunning() => Process.GetProcessesByName("GHelperOverlay").Length > 0;
}
