using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;

namespace GHelper.Helpers;

// Installs and starts the GHelperOverlay uiAccess renderer. uiAccess requires a
// signed exe under Program Files, hence the install step; when any of that isn't
// possible the caller falls back to the in-process overlay window.
public static class OverlayLauncher
{
    private const string EmbeddedResourceName = "GHelperOverlay.exe";

    public static string InstalledPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
        "GHelper", "GHelperOverlay.exe");

    public static bool Start()
    {
        if (IsRunning()) return true;

        if (!HasEmbedded())
        {
            Logger.WriteLine("OverlayLauncher: no embedded renderer in this build");
            return false;
        }

        if (NeedsInstall())
        {
            if (!ProcessHelper.IsUserAdministrator())
            {
                Logger.WriteLine("OverlayLauncher: renderer install needs admin - using local overlay");
                return false;
            }
            if (!InstallEmbedded()) return false;
        }

        try
        {
            Process.Start(new ProcessStartInfo(InstalledPath) { UseShellExecute = true });
            return true;
        }
        catch (Exception ex)
        {
            Logger.WriteLine("OverlayLauncher start failed: " + ex.Message);
            return false;
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
    }

    private static bool IsRunning()
    {
        var procs = Process.GetProcessesByName("GHelperOverlay");
        foreach (var p in procs) p.Dispose();
        return procs.Length > 0;
    }

    private static bool InstallEmbedded()
    {
        try
        {
            string dst = InstalledPath;
            Directory.CreateDirectory(Path.GetDirectoryName(dst)!);
            Stop();
            File.WriteAllBytes(dst, ReadEmbedded());
            Logger.WriteLine("Overlay renderer installed to " + dst);
            return true;
        }
        catch (Exception ex)
        {
            Logger.WriteLine("OverlayLauncher install failed: " + ex.Message);
            return false;
        }
    }

    private static bool HasEmbedded()
        => Assembly.GetExecutingAssembly().GetManifestResourceStream(EmbeddedResourceName) != null;

    private static byte[] ReadEmbedded()
    {
        using var s = Assembly.GetExecutingAssembly().GetManifestResourceStream(EmbeddedResourceName)!;
        using var ms = new MemoryStream();
        s.CopyTo(ms);
        return ms.ToArray();
    }

    private static bool NeedsInstall()
    {
        if (!File.Exists(InstalledPath)) return true;
        return !SHA256.HashData(ReadEmbedded()).SequenceEqual(SHA256.HashData(File.ReadAllBytes(InstalledPath)));
    }
}
