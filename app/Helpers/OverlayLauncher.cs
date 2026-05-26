using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;

namespace GHelper.Helpers;

public static class OverlayLauncher
{
    private const string EmbeddedResourceName = "GHelperOverlay.exe";

    public static string InstalledPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
        "GHelper", "GHelperOverlay.exe");

    public static void Start()
    {
        if (!HasEmbedded())
        {
            Program.hardwareOverlay?.StartOverlay();
            return;
        }

        if (NeedsInstall())
        {
            if (ProcessHelper.IsUserAdministrator())
            {
                InstallEmbedded();
            }
            else
            {
                ProcessHelper.RunAsAdmin();
                return;
            }
        }

        try { Program.hardwareOverlay?.StopOverlay(); } catch { }
        if (!IsExternalRunning())
            try { Process.Start(new ProcessStartInfo(InstalledPath) { UseShellExecute = true }); }
            catch (Exception ex) { Logger.WriteLine("OverlayLauncher external start failed: " + ex.Message); }
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

    public static void InstallEmbedded()
    {
        try
        {
            string dst = InstalledPath;
            string dstDir = Path.GetDirectoryName(dst)!;
            Directory.CreateDirectory(dstDir);

            foreach (var p in Process.GetProcessesByName("GHelperOverlay"))
            {
                try { p.Kill(); p.WaitForExit(2000); }
                catch { }
                finally { p.Dispose(); }
            }

            File.WriteAllBytes(dst, ReadEmbedded());
            Logger.WriteLine("Overlay installed to " + dst);
        }
        catch (Exception ex)
        {
            Logger.WriteLine("OverlayLauncher install failed: " + ex.Message);
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
        var embedded = SHA256.HashData(ReadEmbedded());
        var installed = SHA256.HashData(File.ReadAllBytes(InstalledPath));
        return !embedded.SequenceEqual(installed);
    }

    private static bool IsExternalRunning() => Process.GetProcessesByName("GHelperOverlay").Length > 0;
}
