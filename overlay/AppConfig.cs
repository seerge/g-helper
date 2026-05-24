using System.Text.Json;

namespace GHelperOverlay;

// Minimal config store for the standalone overlay.
// Lives at %AppData%\GHelperOverlay\config.json so it does not collide with
// the main g-helper config — the overlay is fully self-contained.
public static class AppConfig
{
    private static readonly string configFile = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "GHelperOverlay", "config.json");

    private static Dictionary<string, object> config = new();
    private static readonly object configLock = new();
    private static readonly System.Timers.Timer writeTimer = new(2000) { AutoReset = false };

    static AppConfig()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(configFile)!);
            if (File.Exists(configFile))
                config = JsonSerializer.Deserialize<Dictionary<string, object>>(
                    File.ReadAllText(configFile)) ?? new();
        }
        catch (Exception ex) { Logger.WriteLine($"AppConfig load failed: {ex.Message}"); }

        writeTimer.Elapsed += (_, _) => Flush();
    }

    public static bool Exists(string name)
    {
        lock (configLock) return config.ContainsKey(name);
    }

    public static int Get(string name, int empty = -1)
    {
        lock (configLock)
            return config.TryGetValue(name, out var v) && int.TryParse(v?.ToString(), out int r) ? r : empty;
    }

    public static string? GetString(string name, string? empty = null)
    {
        lock (configLock)
            return config.TryGetValue(name, out var v) ? v?.ToString() : empty;
    }

    public static bool Is(string name) => Get(name) == 1;

    public static void Set(string name, int value)
    {
        lock (configLock) config[name] = value;
        Schedule();
    }

    public static void Set(string name, string value)
    {
        lock (configLock) config[name] = value;
        Schedule();
    }

    private static void Schedule()
    {
        writeTimer.Stop();
        writeTimer.Start();
    }

    private static void Flush()
    {
        string json;
        lock (configLock) json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        try { File.WriteAllText(configFile, json); }
        catch (Exception ex) { Logger.WriteLine($"AppConfig write failed: {ex.Message}"); }
    }
}
