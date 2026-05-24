using System.Diagnostics;

namespace GHelperOverlay;

public static class Logger
{
    // Shared %AppData%\GHelper directory, with an overlay-specific filename so
    // we never overwrite g-helper's log.txt.
    public static string appPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GHelper");
    public static string logFile = Path.Combine(appPath, "overlay-log.txt");

    private static readonly Random _random = new();

    public static void WriteLine(string message)
    {
        Debug.WriteLine($"{DateTime.Now}: {message}");
        try
        {
            if (!Directory.Exists(appPath)) Directory.CreateDirectory(appPath);
            using var w = File.AppendText(logFile);
            w.WriteLine($"{DateTime.Now}: {message}");
        }
        catch { }

        if (_random.Next(100) == 1) Cleanup();
    }

    private static void Cleanup()
    {
        try
        {
            var lines = File.ReadAllLines(logFile);
            int skip = Math.Max(0, lines.Length - 2000);
            File.WriteAllLines(logFile, lines.Skip(skip).ToArray());
        }
        catch { }
    }
}
