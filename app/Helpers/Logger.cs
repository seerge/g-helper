using System.Diagnostics;

public static class Logger
{
    public static string appPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\GHelper";
    public static string logFile = appPath + "\\log.txt";

    private static readonly Random _random = new Random();

    public static void WriteLine(string logMessage)
    {
        var stamp = DateTime.Now;
        Debug.WriteLine($"{stamp}: {logMessage}");

        try
        {
            if (!Directory.Exists(appPath)) Directory.CreateDirectory(appPath);
            using var fs = new FileStream(logFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            using var w = new StreamWriter(fs);
            w.WriteLine($"{stamp} [{Environment.UserName}]: {logMessage}");
        }
        catch { }

        if (_random.Next(100) == 1) Cleanup();
    }

    public static void Cleanup()
    {
        try
        {
            var file = File.ReadAllLines(logFile);
            int skip = Math.Max(0, file.Length - 4000);
            File.WriteAllLines(logFile, file.Skip(skip).ToArray());
        }
        catch { }
    }

}
