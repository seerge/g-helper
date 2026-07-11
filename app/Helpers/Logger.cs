using System.Diagnostics;
using GHelper.Helpers;

public static class Logger
{
    public static string appPath = Environment.GetFolderPath(ProcessHelper.IsRunningAsSystem() ? Environment.SpecialFolder.CommonApplicationData : Environment.SpecialFolder.ApplicationData) + "\\GHelper";
    public static string logFile = appPath + "\\log.txt";

    private static readonly Random _random = new Random();
    private static readonly object _lock = new object();

    public static void WriteLine(string logMessage)
    {
        Debug.WriteLine($"{DateTime.Now}: {logMessage}");

        lock (_lock)
        {
            try
            {
                if (!Directory.Exists(appPath)) Directory.CreateDirectory(appPath);
                using (StreamWriter w = File.AppendText(logFile))
                {
                    w.WriteLine($"{DateTime.Now}: {logMessage}");
                    w.Close();
                }
            }
            catch { }

            if (_random.Next(100) == 1) Cleanup();
        }
    }

    public static void Cleanup()
    {
        try
        {
            var file = File.ReadAllLines(logFile);
            int skip = Math.Max(0, file.Count() - 2000);
            File.WriteAllLines(logFile, file.Skip(skip).ToArray());
        }
        catch { }
    }

}
