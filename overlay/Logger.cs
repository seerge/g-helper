using System.Diagnostics;

namespace GHelperOverlay;

public static class Logger
{
    public static string appPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GHelperOverlay");
    public static string logFile = Path.Combine(appPath, "log.txt");

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
