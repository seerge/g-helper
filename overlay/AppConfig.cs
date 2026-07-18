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

    // True unless the key exists and is set to 0 — matches main g-helper's semantics
    // for the `overlay_show_*` keys (defaults to ON if the user never set them).
    public static bool IsNotFalse(string name) => !Exists(name) || Get(name) != 0;

    public static bool IsOverlayGameOnly() => Is("overlay_game_only");

    public static void Set(string name, int value) { values[name] = value.ToString(); Save(); }

    public static void Set(string name, string value) { values[name] = value; Save(); }

    // Registry SystemProductName matches WMI Win32_ComputerSystem.Model that main
    // g-helper uses, without paying the WMI query cost.
    private static string? _model;

    private static string GetModel()
    {
        if (_model != null) return _model;
        try
        {
            using var k = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                @"HARDWARE\DESCRIPTION\System\BIOS");
            _model = k?.GetValue("SystemProductName")?.ToString() ?? "";
        }
        catch { _model = ""; }
        return _model;
    }

    private static bool ContainsModel(string contains)
        => GetModel().IndexOf(contains, StringComparison.OrdinalIgnoreCase) >= 0;

    public static bool IsAlly() => ContainsModel("RC7");

    // Same model list as main g-helper's IsAMDiGPU (incl. IsOnlyAIMAX models).
    public static bool IsAMDiGPU()
        => ContainsModel("GV301RA") || ContainsModel("GV302XA") || ContainsModel("GZ302")
        || ContainsModel("FA401EA") || ContainsModel("HN7306EA") || IsAlly();
}
