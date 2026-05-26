namespace GHelperOverlay;

// INI-format config at %AppData%\GHelper\overlay.ini.
// Read once at startup; written synchronously on every Set.
public static class AppConfig
{
    private const string Section = "Overlay";

    private static readonly string file = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "GHelper", "overlay.ini");

    private static readonly Dictionary<string, string> values = Load();

    private static Dictionary<string, string> Load()
    {
        var d = new Dictionary<string, string>();
        try
        {
            if (!File.Exists(file)) return d;
            foreach (var raw in File.ReadAllLines(file))
            {
                var line = raw.Trim();
                if (line.Length == 0 || line[0] == ';' || line[0] == '#' || line[0] == '[') continue;
                int eq = line.IndexOf('=');
                if (eq > 0) d[line.Substring(0, eq).Trim()] = line.Substring(eq + 1).Trim();
            }
        }
        catch { }
        return d;
    }

    private static void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(file));
            var lines = new List<string>(values.Count + 1) { "[" + Section + "]" };
            foreach (var kv in values) lines.Add(kv.Key + "=" + kv.Value);
            File.WriteAllLines(file, lines);
        }
        catch { }
    }

    public static bool Exists(string name) => values.ContainsKey(name);

    public static int Get(string name, int empty = -1)
        => values.TryGetValue(name, out var v) && int.TryParse(v, out int r) ? r : empty;

    public static string? GetString(string name, string? empty = null)
        => values.TryGetValue(name, out var v) ? v : empty;

    public static bool Is(string name) => Get(name) == 1;

    public static void Set(string name, int value) { values[name] = value.ToString(); Save(); }

    public static void Set(string name, string value) { values[name] = value; Save(); }
}
