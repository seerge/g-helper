using System.Collections.Generic;
using System.Globalization;

namespace GHelper.Peripherals.Keyboard
{
    public record struct KeyDef(string Name, float Width = 1f, float Gap = 0f, float Height = 1f, float Top = 0f);

    // Per-model key layouts; LedIds[i] is the LED id for grid key i.
    public static class AuraKeyboardLayouts
    {
        // single source of truth: key name -> wire code. UI grid shortens long names via ShortLabel()
        public static readonly Dictionary<string, ushort> Keys = new()
        {
            ["A"] = 0x1F, ["B"] = 0x32, ["C"] = 0x30, ["D"] = 0x21, ["E"] = 0x13, ["F"] = 0x22,
            ["G"] = 0x23, ["H"] = 0x24, ["I"] = 0x18, ["J"] = 0x25, ["K"] = 0x26, ["L"] = 0x27,
            ["M"] = 0x34, ["N"] = 0x33, ["O"] = 0x19, ["P"] = 0x1A, ["Q"] = 0x11, ["R"] = 0x14,
            ["S"] = 0x20, ["T"] = 0x15, ["U"] = 0x17, ["V"] = 0x31, ["W"] = 0x12, ["X"] = 0x2F,
            ["Y"] = 0x16, ["Z"] = 0x2E, ["1"] = 0x02, ["2"] = 0x03, ["3"] = 0x04, ["4"] = 0x05,
            ["5"] = 0x06, ["6"] = 0x07, ["7"] = 0x08, ["8"] = 0x09, ["9"] = 0x0A, ["0"] = 0x0B,
            ["Enter"] = 0x2B, ["Esc"] = 0x6E, ["Bksp"] = 0x0F, ["Tab"] = 0x10, ["Space"] = 0x3D, ["-"] = 0x0C,
            ["="] = 0x0D, ["["] = 0x1B, ["]"] = 0x1C, ["\\"] = 0x1D, [";"] = 0x28, ["'"] = 0x29,
            ["`"] = 0x01, [","] = 0x35, ["."] = 0x36, ["/"] = 0x37, ["#"] = 0x2A, ["<"] = 0x2D,
            ["Caps"] = 0x1E, ["F1"] = 0x70, ["F2"] = 0x71, ["F3"] = 0x72, ["F4"] = 0x73, ["F5"] = 0x74,
            ["F6"] = 0x75, ["F7"] = 0x76, ["F8"] = 0x77, ["F9"] = 0x78, ["F10"] = 0x79, ["F11"] = 0x7A,
            ["F12"] = 0x7B, ["PrtSc"] = 0x7C, ["ScrLk"] = 0x7D, ["Pause"] = 0x7E, ["Ins"] = 0x4B, ["Home"] = 0x50,
            ["Pg Up"] = 0x55, ["Del"] = 0x4C, ["End"] = 0x51, ["Pg Dn"] = 0x56, ["→"] = 0x59, ["←"] = 0x4F,
            ["↓"] = 0x54, ["↑"] = 0x53, ["Num"] = 0x5A, ["Num /"] = 0x5F, ["Num *"] = 0x64, ["Num -"] = 0x69,
            ["Num +"] = 0x6A, ["Num Enter"] = 0x6C, ["Num 1"] = 0x5D, ["Num 2"] = 0x62, ["Num 3"] = 0x67, ["Num 4"] = 0x5C,
            ["Num 5"] = 0x61, ["Num 6"] = 0x66, ["Num 7"] = 0x5B, ["Num 8"] = 0x60, ["Num 9"] = 0x65, ["Num 0"] = 0x63,
            ["Num ."] = 0x68, ["Ctrl"] = 0x3A, ["Shift"] = 0x2C, ["Alt"] = 0x3C, ["Win"] = 0x3B, ["Menu"] = 0x41,
            ["Right Ctrl"] = 0x40, ["Right Shift"] = 0x39, ["Right Alt"] = 0x3E, ["Right Win"] = 0x80,
        };

        public static readonly Dictionary<string, ushort> MediaKeys = new()
        {
            ["M1"] = 0xB7, ["M2"] = 0xB6, ["M3"] = 0xB5, ["M4"] = 0xB4,
        };

        public static readonly Dictionary<string, ushort> MultimediaTargets = new()
        {
            ["Play/Pause"] = 0xB7, ["Stop"] = 0xB6, ["Next Track"] = 0xB5, ["Previous Track"] = 0xB4,
            ["Mute"] = 0xB8, ["Volume Up"] = 0xBB, ["Volume Down"] = 0xBC,
        };

        public static readonly Dictionary<string, ushort> MouseTargets = new()
        {
            ["Mouse Left"] = 0x01F0, ["Mouse Right"] = 0x01F1, ["Mouse Middle"] = 0x01F2,
            ["Scroll Up"] = 0x01E8, ["Scroll Down"] = 0x01E9,
        };

        public static string ShortLabel(string name)
        {
            if (name.StartsWith("Num ")) return name["Num ".Length..];
            if (name.StartsWith("Right ")) return name["Right ".Length..];
            return name;
        }

        public static readonly Dictionary<int, Dictionary<string, string>> RegionalLegends = new()
        {
            [3] = new() { ["Y"] = "Z", ["Z"] = "Y", ["`"] = "^", ["-"] = "ß", ["="] = "´", ["["] = "Ü", ["]"] = "+", [";"] = "Ö", ["'"] = "Ä", ["/"] = "-", ["Right Alt"] = "Alt Gr" },
            [4] = new() { ["<"] = "\\" },
            [5] = new() { ["Q"] = "A", ["A"] = "Q", ["W"] = "Z", ["Z"] = "W", ["M"] = ",", [";"] = "M", [","] = ";", ["."] = ":", ["/"] = "!", ["`"] = "²", ["["] = "^", ["]"] = "$", ["'"] = "ù", ["#"] = "*", ["-"] = ")" },
            [7] = new() { ["Y"] = "Z", ["Z"] = "Y", ["`"] = "0", ["0"] = "Ö", ["-"] = "Ü", ["="] = "Ó", ["["] = "Ő", ["]"] = "Ú", [";"] = "É", ["'"] = "Á", ["#"] = "Ű", ["<"] = "Í", ["/"] = "-" },
            [8] = new() { ["`"] = "\\", ["-"] = "'", ["="] = "ì", ["["] = "è", ["]"] = "+", [";"] = "ò", ["'"] = "à", ["#"] = "ù", ["/"] = "-" },
            [11] = new() { ["`"] = "|", ["-"] = "+", ["="] = "\\", ["["] = "Å", ["]"] = "¨", [";"] = "ÖØ", ["'"] = "ÄÆ", ["#"] = "'", ["/"] = "-" },
            [12] = new() { ["`"] = "\\", ["-"] = "'", ["="] = "«", ["["] = "+", ["]"] = "´", [";"] = "Ç", ["'"] = "º", ["#"] = "~", ["/"] = "-" },
            [15] = new() { ["`"] = "º", ["-"] = "'", ["="] = "¡", ["["] = "`", ["]"] = "+", [";"] = "Ñ", ["'"] = "´", ["#"] = "Ç", ["/"] = "-" },
            [18] = new() { ["`"] = "\"", ["-"] = "*", ["="] = "-", ["["] = "Ğ", ["]"] = "Ü", [";"] = "Ş", ["'"] = "İ", ["#"] = ",", [","] = "Ö", ["."] = "Ç", ["/"] = "." },
        };

        private static readonly Dictionary<string, int> CultureLayouts = new()
        {
            ["de"] = 3, ["en-GB"] = 4, ["fr"] = 5, ["hu"] = 7, ["it"] = 8, ["nb"] = 11, ["nn"] = 11, ["sv"] = 11, ["da"] = 11, ["fi"] = 11, ["pt-PT"] = 12, ["es"] = 15, ["tr"] = 18,
        };

        public static int CultureLayout(CultureInfo culture) => CultureLayouts.GetValueOrDefault(culture.Name, CultureLayouts.GetValueOrDefault(culture.TwoLetterISOLanguageName, -1));

        public static string Legend(string name, int multiLayout)
        {
            if (RegionalLegends.TryGetValue(multiLayout, out var region) && region.TryGetValue(name, out var legend)) return legend;
            return ShortLabel(name);
        }

        public record LayoutData(List<KeyDef[]> Rows, byte[] LedIds);

        private static readonly List<KeyDef[]> FullSizeRows = new()
        {
            new KeyDef[] { new("Esc"), new("F1", 1f, 1.0f), new("F2"), new("F3"), new("F4"), new("F5", 1f, 0.5f), new("F6"), new("F7"), new("F8"), new("F9", 1f, 0.5f), new("F10"), new("F11"), new("F12"), new("PrtSc", 1f, 0.5f), new("ScrLk"), new("Pause") },
            new KeyDef[] { new("`"), new("1"), new("2"), new("3"), new("4"), new("5"), new("6"), new("7"), new("8"), new("9"), new("0"), new("-"), new("="), new("Bksp", 2f, 0.0f), new("Ins", 1f, 0.5f), new("Home"), new("Pg Up"), new("Num", 1f, 0.5f), new("Num /"), new("Num *"), new("Num -") },
            new KeyDef[] { new("Tab", 1.5f, 0.0f), new("Q"), new("W"), new("E"), new("R"), new("T"), new("Y"), new("U"), new("I"), new("O"), new("P"), new("["), new("]"), new("\\", 1.5f, 0.0f), new("Del", 1f, 0.5f), new("End"), new("Pg Dn"), new("Num 7", 1f, 0.5f), new("Num 8"), new("Num 9"), new("Num +", 1f, 0.0f, 2f) },
            new KeyDef[] { new("Caps", 1.75f, 0.0f), new("A"), new("S"), new("D"), new("F"), new("G"), new("H"), new("J"), new("K"), new("L"), new(";"), new("'"), new("Enter", 2.25f, 0.0f), new("Num 4", 1f, 4.0f), new("Num 5"), new("Num 6") },
            new KeyDef[] { new("Shift", 2.25f, 0.0f), new("Z"), new("X"), new("C"), new("V"), new("B"), new("N"), new("M"), new(","), new("."), new("/"), new("Right Shift", 2.75f, 0.0f), new("↑", 1f, 1.5f), new("Num 1", 1f, 1.5f), new("Num 2"), new("Num 3"), new("Num Enter", 1f, 0.0f, 2f) },
            new KeyDef[] { new("Ctrl", 1.25f, 0.0f), new("Win", 1.25f, 0.0f), new("Alt", 1.25f, 0.0f), new("Space", 6.25f, 0.0f), new("Right Alt", 1.25f, 0.0f), new("Fn", 1.25f, 0.0f), new("Menu", 1.25f, 0.0f), new("Right Ctrl", 1.25f, 0.0f), new("←", 1f, 0.5f), new("↓"), new("→"), new("Num 0", 2f, 0.5f), new("Num .") },
        };

        private static List<KeyDef[]> ToIso(List<KeyDef[]> rows)
        {
            var iso = new List<KeyDef[]>();
            foreach (var row in rows)
            {
                var keys = new List<KeyDef>();
                foreach (var def in row)
                {
                    if (def.Name == "\\") keys.Add(def with { Name = "Enter" });
                    else if (def.Name == "Enter") { keys.Add(new("#")); keys.Add(def with { Width = 1.25f }); }
                    else if (def.Name == "Shift") { keys.Add(def with { Width = 1.25f }); keys.Add(new("<")); }
                    else keys.Add(def);
                }
                iso.Add(keys.ToArray());
            }
            return iso;
        }

        private static KeyDef[] Short(float top, params KeyDef[] keys) => Array.ConvertAll(keys, key => key with { Height = 0.5f, Top = top });

        private static readonly List<KeyDef[]> FalchionRows = new()
        {
            new KeyDef[] { new("Esc"), new("1"), new("2"), new("3"), new("4"), new("5"), new("6"), new("7"), new("8"), new("9"), new("0"), new("-"), new("="), new("Bksp", 2f, 0.0f), new("Ins") },
            new KeyDef[] { new("Tab", 1.5f, 0.0f), new("Q"), new("W"), new("E"), new("R"), new("T"), new("Y"), new("U"), new("I"), new("O"), new("P"), new("["), new("]"), new("\\", 1.5f, 0.0f), new("Del") },
            new KeyDef[] { new("Caps", 1.75f, 0.0f), new("A"), new("S"), new("D"), new("F"), new("G"), new("H"), new("J"), new("K"), new("L"), new(";"), new("'"), new("Enter", 2.25f, 0.0f), new("Pg Up") },
            new KeyDef[] { new("Shift", 2.25f, 0.0f), new("Z"), new("X"), new("C"), new("V"), new("B"), new("N"), new("M"), new(","), new("."), new("/"), new("Right Shift", 1.75f, 0.0f), new("↑"), new("Pg Dn") },
            new KeyDef[] { new("Ctrl", 1.25f, 0.0f), new("Win", 1.25f, 0.0f), new("Alt", 1.25f, 0.0f), new("Space", 6.25f, 0.0f), new("Right Alt"), new("Fn"), new("Right Ctrl"), new("←"), new("↓"), new("→") },
        };

        private static readonly List<KeyDef[]> AzothRows = new()
        {
            new KeyDef[] { new("Esc"), new("F1"), new("F2"), new("F3"), new("F4"), new("F5"), new("F6"), new("F7"), new("F8"), new("F9"), new("F10"), new("F11"), new("F12") },
            new KeyDef[] { new("`"), new("1"), new("2"), new("3"), new("4"), new("5"), new("6"), new("7"), new("8"), new("9"), new("0"), new("-"), new("="), new("Bksp", 2f, 0.0f), new("Ins") },
            new KeyDef[] { new("Tab", 1.5f, 0.0f), new("Q"), new("W"), new("E"), new("R"), new("T"), new("Y"), new("U"), new("I"), new("O"), new("P"), new("["), new("]"), new("\\", 1.5f, 0.0f), new("Del") },
            new KeyDef[] { new("Caps", 1.75f, 0.0f), new("A"), new("S"), new("D"), new("F"), new("G"), new("H"), new("J"), new("K"), new("L"), new(";"), new("'"), new("Enter", 2.25f, 0.0f), new("Pg Up") },
            new KeyDef[] { new("Shift", 2.25f, 0.0f), new("Z"), new("X"), new("C"), new("V"), new("B"), new("N"), new("M"), new(","), new("."), new("/"), new("Right Shift", 1.75f, 0.0f), new("↑"), new("Pg Dn") },
            new KeyDef[] { new("Ctrl", 1.25f, 0.0f), new("Win", 1.25f, 0.0f), new("Alt", 1.25f, 0.0f), new("Space", 6.25f, 0.0f), new("Right Alt"), new("Fn"), new("Right Ctrl"), new("←"), new("↓"), new("→") },
        };

        private static readonly List<KeyDef[]> StrixScopeIIRows = new()
        {
            new KeyDef[] { new("Esc"), new("F1", 1f, 1.0f), new("F2"), new("F3"), new("F4"), new("F5", 1f, 0.5f), new("F6"), new("F7"), new("F8"), new("F9", 1f, 0.5f), new("F10"), new("F11"), new("F12"), new("PrtSc", 1f, 0.5f), new("ScrLk"), new("Pause"), new("Logo", 2f, 1.5f) },
            new KeyDef[] { new("`"), new("1"), new("2"), new("3"), new("4"), new("5"), new("6"), new("7"), new("8"), new("9"), new("0"), new("-"), new("="), new("Bksp", 2f, 0.0f), new("Ins", 1f, 0.5f), new("Home"), new("Pg Up"), new("Num", 1f, 0.5f), new("Num /"), new("Num *"), new("Num -") },
            new KeyDef[] { new("Tab", 1.5f, 0.0f), new("Q"), new("W"), new("E"), new("R"), new("T"), new("Y"), new("U"), new("I"), new("O"), new("P"), new("["), new("]"), new("\\", 1.5f, 0.0f), new("Del", 1f, 0.5f), new("End"), new("Pg Dn"), new("Num 7", 1f, 0.5f), new("Num 8"), new("Num 9"), new("Num +", 1f, 0.0f, 2f) },
            new KeyDef[] { new("Caps", 1.75f, 0.0f), new("A"), new("S"), new("D"), new("F"), new("G"), new("H"), new("J"), new("K"), new("L"), new(";"), new("'"), new("Enter", 2.25f, 0.0f), new("Num 4", 1f, 4.0f), new("Num 5"), new("Num 6") },
            new KeyDef[] { new("Shift", 2.25f, 0.0f), new("Z"), new("X"), new("C"), new("V"), new("B"), new("N"), new("M"), new(","), new("."), new("/"), new("Right Shift", 2.75f, 0.0f), new("↑", 1f, 1.5f), new("Num 1", 1f, 1.5f), new("Num 2"), new("Num 3"), new("Num Enter", 1f, 0.0f, 2f) },
            new KeyDef[] { new("Ctrl", 1.25f, 0.0f), new("Win", 1.25f, 0.0f), new("Alt", 1.25f, 0.0f), new("Space", 2.0833333333333335f, 0.0f), new("Space", 2.0833333333333335f, 0.0f), new("Space", 2.0833333333333335f, 0.0f), new("Right Alt", 1.25f, 0.0f), new("Fn", 1.25f, 0.0f), new("Menu", 1.25f, 0.0f), new("Right Ctrl", 1.25f, 0.0f), new("←", 1f, 0.5f), new("↓"), new("→"), new("Num 0", 2f, 0.5f), new("Num .") },
        };

        private static readonly List<KeyDef[]> StrixScopeII96Rows = new()
        {
            new KeyDef[] { new("Esc"), new("F1", 1f, 1.0f), new("F2"), new("F3"), new("F4"), new("F5", 1f, 0.5f), new("F6"), new("F7"), new("F8"), new("F9", 1f, 0.5f), new("F10"), new("F11"), new("F12"), new("Ins"), new("Del"), new("Pg Up"), new("Pg Dn") },
            new KeyDef[] { new("`"), new("1"), new("2"), new("3"), new("4"), new("5"), new("6"), new("7"), new("8"), new("9"), new("0"), new("-"), new("="), new("Bksp", 2f, 0.0f), new("Num"), new("Num /"), new("Num *"), new("Num -") },
            new KeyDef[] { new("Tab", 1.5f, 0.0f), new("Q"), new("W"), new("E"), new("R"), new("T"), new("Y"), new("U"), new("I"), new("O"), new("P"), new("["), new("]"), new("\\", 1.5f, 0.0f), new("Num 7"), new("Num 8"), new("Num 9"), new("Num +", 1f, 0.0f, 2f) },
            new KeyDef[] { new("Caps", 1.75f, 0.0f), new("A"), new("S"), new("D"), new("F"), new("G"), new("H"), new("J"), new("K"), new("L"), new(";"), new("'"), new("Enter", 2.25f, 0.0f), new("Num 4"), new("Num 5"), new("Num 6") },
            new KeyDef[] { new("Shift", 2.25f, 0.0f), new("Z"), new("X"), new("C"), new("V"), new("B"), new("N"), new("M"), new(","), new("."), new("/"), new("Right Shift", 1.75f, 0.0f), new("↑"), new("Num 1"), new("Num 2"), new("Num 3"), new("Num Enter", 1f, 0.0f, 2f) },
            new KeyDef[] { new("Ctrl", 1.25f, 0.0f), new("Win", 1.25f, 0.0f), new("Alt", 1.25f, 0.0f), new("Space", 1.9166667f, 0.0f), new("Space", 1.9166667f, 0.0f), new("Space", 1.9166667f, 0.0f), new("Right Alt", 1.25f, 0.0f), new("Fn", 1.25f, 0.0f), new("Right Ctrl"), new("←"), new("↓"), new("→"), new("Num 0"), new("Num .") },
        };

        private static readonly List<KeyDef[]> ClaymoreNoNumpadRows = new()
        {
            new KeyDef[] { new("Esc"), new("F1", 1f, 1.0f), new("F2"), new("F3"), new("F4"), new("F5", 1f, 0.5f), new("F6"), new("F7"), new("F8"), new("F9", 1f, 0.5f), new("F10"), new("F11"), new("F12"), new("PrtSc", 1f, 0.5f), new("ScrLk"), new("Pause") },
            new KeyDef[] { new("`"), new("1"), new("2"), new("3"), new("4"), new("5"), new("6"), new("7"), new("8"), new("9"), new("0"), new("-"), new("="), new("Bksp", 2f, 0.0f), new("Ins", 1f, 0.5f), new("Home"), new("Pg Up") },
            new KeyDef[] { new("Tab", 1.5f, 0.0f), new("Q"), new("W"), new("E"), new("R"), new("T"), new("Y"), new("U"), new("I"), new("O"), new("P"), new("["), new("]"), new("\\", 1.5f, 0.0f), new("Del", 1f, 0.5f), new("End"), new("Pg Dn") },
            new KeyDef[] { new("Caps", 1.75f, 0.0f), new("A"), new("S"), new("D"), new("F"), new("G"), new("H"), new("J"), new("K"), new("L"), new(";"), new("'"), new("Enter", 2.25f, 0.0f) },
            new KeyDef[] { new("Shift", 2.25f, 0.0f), new("Z"), new("X"), new("C"), new("V"), new("B"), new("N"), new("M"), new(","), new("."), new("/"), new("Right Shift", 2.75f, 0.0f), new("↑", 1f, 1.5f) },
            new KeyDef[] { new("Ctrl", 1.25f, 0.0f), new("Win", 1.25f, 0.0f), new("Alt", 1.25f, 0.0f), new("Space", 6.25f, 0.0f), new("Right Alt", 1.25f, 0.0f), new("Fn", 1.25f, 0.0f), new("Menu", 1.25f, 0.0f), new("Right Ctrl", 1.25f, 0.0f), new("←", 1f, 0.5f), new("↓"), new("→") },
        };

        private static readonly List<KeyDef[]> StrixScopeRXTKLRows = new()
        {
            new KeyDef[] { new("Esc"), new("F1", 1f, 1.0f), new("F2"), new("F3"), new("F4"), new("F5", 1f, 0.5f), new("F6"), new("F7"), new("F8"), new("F9", 1f, 0.5f), new("F10"), new("F11"), new("F12") },
            new KeyDef[] { new("`"), new("1"), new("2"), new("3"), new("4"), new("5"), new("6"), new("7"), new("8"), new("9"), new("0"), new("-"), new("="), new("Bksp", 2f, 0.0f), new("Ins", 1f, 0.25f), new("Home"), new("Pg Up") },
            new KeyDef[] { new("Tab", 1.5f, 0.0f), new("Q"), new("W"), new("E"), new("R"), new("T"), new("Y"), new("U"), new("I"), new("O"), new("P"), new("["), new("]"), new("\\", 1.5f, 0.0f), new("Del", 1f, 0.25f), new("End"), new("Pg Dn") },
            new KeyDef[] { new("Caps", 1.75f, 0.0f), new("A"), new("S"), new("D"), new("F"), new("G"), new("H"), new("J"), new("K"), new("L"), new(";"), new("'"), new("Enter", 2.25f, 0.0f) },
            new KeyDef[] { new("Shift", 2.25f, 0.0f), new("Z"), new("X"), new("C"), new("V"), new("B"), new("N"), new("M"), new(","), new("."), new("/"), new("Right Shift", 2.75f, 0.0f), new("↑", 1f, 1.25f) },
            new KeyDef[] { new("Ctrl", 1.25f, 0.0f), new("Win", 1.25f, 0.0f), new("Alt", 1.25f, 0.0f), new("Space", 6.25f, 0.0f), new("Right Alt", 1.25f, 0.0f), new("Fn", 1.25f, 0.0f), new("Menu", 1.25f, 0.0f), new("Right Ctrl", 1.25f, 0.0f), new("←", 1f, 0.25f), new("↓"), new("→") },
        };

        private static readonly List<KeyDef[]> StrixScopeII96RxRows = new(StrixScopeII96Rows.GetRange(0, 5))
        {
            new KeyDef[] { new("Ctrl", 1.25f, 0.0f), new("Win", 1.25f, 0.0f), new("Alt", 1.25f, 0.0f), new("Space", 1.15f, 0.0f), new("Space", 1.15f, 0.0f), new("Space", 1.15f, 0.0f), new("Space", 1.15f, 0.0f), new("Space", 1.15f, 0.0f), new("Right Alt", 1.25f, 0.0f), new("Fn", 1.25f, 0.0f), new("Right Ctrl"), new("←"), new("↓"), new("→"), new("Num 0"), new("Num .") },
        };

        private static readonly List<KeyDef[]> TUFK3GenIIRows = new()
        {
            new KeyDef[] { new("Esc"), new("F1", 1f, 1.0f), new("F2"), new("F3"), new("F4"), new("F5", 1f, 0.5f), new("F6"), new("F7"), new("F8"), new("F9", 1f, 0.5f), new("F10"), new("F11"), new("F12"), new("Del"), new("Ins"), new("Pg Up"), new("Pg Dn") },
            new KeyDef[] { new("`"), new("1"), new("2"), new("3"), new("4"), new("5"), new("6"), new("7"), new("8"), new("9"), new("0"), new("-"), new("="), new("Bksp", 2f, 0.0f), new("Num"), new("Num /"), new("Num *"), new("Num -") },
            new KeyDef[] { new("Tab", 1.5f, 0.0f), new("Q"), new("W"), new("E"), new("R"), new("T"), new("Y"), new("U"), new("I"), new("O"), new("P"), new("["), new("]"), new("\\", 1.5f, 0.0f), new("Num 7"), new("Num 8"), new("Num 9"), new("Num +", 1f, 0.0f, 2f) },
            new KeyDef[] { new("Caps", 1.75f, 0.0f), new("A"), new("S"), new("D"), new("F"), new("G"), new("H"), new("J"), new("K"), new("L"), new(";"), new("'"), new("Enter", 2.25f, 0.0f), new("Num 4"), new("Num 5"), new("Num 6") },
            new KeyDef[] { new("Shift", 2.25f, 0.0f), new("Z"), new("X"), new("C"), new("V"), new("B"), new("N"), new("M"), new(","), new("."), new("/"), new("Right Shift", 1.75f, 0.0f), new("↑"), new("Num 1"), new("Num 2"), new("Num 3"), new("Num Enter", 1f, 0.0f, 2f) },
            new KeyDef[] { new("Ctrl", 1.25f, 0.0f), new("Win", 1.25f, 0.0f), new("Alt", 1.25f, 0.0f), new("Space", 7.0f, 0.0f), new("Fn", 1.25f, 0.0f), new("Right Ctrl"), new("←"), new("↓"), new("→"), new("Num 0"), new("Num .") },
        };

        private static readonly List<KeyDef[]> Strix15Rows = new()
        {
            Short(0f, new("Vol-", 1f, 2.0f), new("Vol+"), new("Mic"), new("Fan"), new("ROG")),
            Short(0.25f, new("Esc"), new("F1", 1f, 1.0f), new("F2"), new("F3"), new("F4"), new("F5", 1f, 0.5f), new("F6"), new("F7"), new("F8"), new("F9", 1f, 0.5f), new("F10"), new("F11"), new("F12"), new("Del", 1f, 0.3333333f)),
            new KeyDef[] { new("`"), new("1"), new("2"), new("3"), new("4"), new("5"), new("6"), new("7"), new("8"), new("9"), new("0"), new("-"), new("="), new("Bksp", 0.6666667f, 0.0f), new("Bksp", 0.6666667f, 0.0f), new("Bksp", 0.6666667f, 0.0f), new("Play", 1f, 0.3333333f) },
            new KeyDef[] { new("Tab", 1.5f, 0.0f), new("Q"), new("W"), new("E"), new("R"), new("T"), new("Y"), new("U"), new("I"), new("O"), new("P"), new("["), new("]"), new("\\", 1.5f, 0.0f), new("Stop", 1f, 0.3333333f) },
            new KeyDef[] { new("Caps", 1.8333333f, 0.0f), new("A"), new("S"), new("D"), new("F"), new("G"), new("H"), new("J"), new("K"), new("L"), new(";"), new("'"), new("Enter", 0.7222222f, 0.0f), new("Enter", 0.7222222f, 0.0f), new("Enter", 0.7222222f, 0.0f), new("Prev", 1f, 0.3333333f) },
            new KeyDef[] { new("Shift", 2.3333333f, 0.0f), new("Z"), new("X"), new("C"), new("V"), new("B"), new("N"), new("M"), new(","), new("."), new("/"), new("Right Shift", 0.6666667f, 0.0f), new("Right Shift", 0.6666667f, 0.0f), new("Right Shift", 0.6666667f, 0.0f), new("Right Shift", 0.6666667f, 0.0f), new("Next", 1f, 0.3333333f) },
            new KeyDef[] { new("Ctrl", 1.3333333f, 0.0f), new("Fn"), new("Win"), new("Alt"), new("Space"), new("Space"), new("Space"), new("Space"), new("Space"), new("Right Alt"), new("PrtSc"), new("Right Ctrl", 1.1666667f, 0.0f), new("←", 0.8333333f, 0.0f, 0.5f, 0.5f), new("↑", 0.8333333f, 0.0f, 0.5f), new("↓", 0.8333333f, -0.8333333f, 0.5f, 0.5f), new("→", 0.8333333f, 0.0f, 0.5f, 0.5f), new("Fn", 1f, 0.3333333f) },
            Short(0.25f, new("Lightbar", 2.7222222f, 0.0f), new("Lightbar", 2.7222222f, 0.0f), new("Lightbar", 2.7222222f, 0.0f), new("Lightbar", 2.7222222f, 0.0f), new("Lightbar", 2.7222222f, 0.0f), new("Lightbar", 2.7222222f, 0.0f)),
        };

        private static readonly List<KeyDef[]> Scar15Rows = new()
        {
            Short(0f, new("Vol-", 1f, 2.0f), new("Vol+"), new("Mic"), new("Fan"), new("ROG")),
            Short(0.25f, new("Esc"), new("F1", 1f, 1.0f), new("F2"), new("F3"), new("F4"), new("F5", 1f, 0.5f), new("F6"), new("F7"), new("F8"), new("F9", 1f, 0.5f), new("F10"), new("F11"), new("F12"), new("Del")),
            new KeyDef[] { new("`"), new("1"), new("2"), new("3"), new("4"), new("5"), new("6"), new("7"), new("8"), new("9"), new("0"), new("-"), new("="), new("Bksp", 0.6666667f, 0.0f), new("Bksp", 0.6666667f, 0.0f), new("Bksp", 0.6666667f, 0.0f), new("Play") },
            new KeyDef[] { new("Tab", 1.5f, 0.0f), new("Q"), new("W"), new("E"), new("R"), new("T"), new("Y"), new("U"), new("I"), new("O"), new("P"), new("["), new("]"), new("\\", 1.5f, 0.0f), new("Stop") },
            new KeyDef[] { new("Caps", 1.8333333f, 0.0f), new("A"), new("S"), new("D"), new("F"), new("G"), new("H"), new("J"), new("K"), new("L"), new(";"), new("'"), new("Enter", 0.7222222f, 0.0f), new("Enter", 0.7222222f, 0.0f), new("Enter", 0.7222222f, 0.0f), new("Prev") },
            new KeyDef[] { new("Shift", 2.3333333f, 0.0f), new("Z"), new("X"), new("C"), new("V"), new("B"), new("N"), new("M"), new(","), new("."), new("/"), new("Right Shift", 0.5555556f, 0.0f), new("Right Shift", 0.5555556f, 0.0f), new("Right Shift", 0.5555556f, 0.0f), new("↑"), new("Next") },
            new KeyDef[] { new("Ctrl", 1.3333333f, 0.0f), new("Fn"), new("Win"), new("Alt"), new("Space"), new("Space"), new("Space"), new("Space"), new("Space"), new("Right Alt"), new("PrtSc"), new("Right Ctrl"), new("←", 1f, 0.6666667f), new("↓"), new("→") },
            Short(0.25f, new("Lightbar", 2.6666667f, 0.0f), new("Lightbar", 2.6666667f, 0.0f), new("Lightbar", 2.6666667f, 0.0f), new("Lightbar", 2.6666667f, 0.0f), new("Lightbar", 2.6666667f, 0.0f), new("Lightbar", 2.6666667f, 0.0f)),
        };

        private static readonly List<KeyDef[]> Strix16Rows = new(Scar15Rows.GetRange(0, 7))
        {
            Short(0.25f, new("Lightbar", 4f, 0.0f), new("Lightbar", 4f, 0.0f), new("Lightbar", 4f, 0.0f), new("Lightbar", 4f, 0.0f)),
        };

        private static readonly List<KeyDef[]> Strix18Rows = new()
        {
            Short(0f, new("Vol-", 1f, 2.0f), new("Vol+"), new("Mic"), new("Fan"), new("ROG")),
            Short(0.25f, new("Esc"), new("F1", 1f, 1.0f), new("F2"), new("F3"), new("F4"), new("F5", 1f, 0.4166667f), new("F6"), new("F7"), new("F8"), new("F9", 1f, 0.4166667f), new("F10"), new("F11"), new("F12"), new("Del"), new("Pause"), new("PrtSc"), new("Home")),
            new KeyDef[] { new("`"), new("1"), new("2"), new("3"), new("4"), new("5"), new("6"), new("7"), new("8"), new("9"), new("0"), new("-"), new("="), new("Bksp", 0.6111111f, 0.0f), new("Bksp", 0.6111111f, 0.0f), new("Bksp", 0.6111111f, 0.0f), new("Num"), new("Num /"), new("Num *"), new("Num -") },
            new KeyDef[] { new("Tab", 1.5f, 0.0f), new("Q"), new("W"), new("E"), new("R"), new("T"), new("Y"), new("U"), new("I"), new("O"), new("P"), new("["), new("]"), new("\\", 1.3333333f, 0.0f), new("Num 7"), new("Num 8"), new("Num 9"), new("Num +", 1f, 0.0f, 2f) },
            new KeyDef[] { new("Caps", 1.75f, 0.0f), new("A"), new("S"), new("D"), new("F"), new("G"), new("H"), new("J"), new("K"), new("L"), new(";"), new("'"), new("Enter", 0.6944444f, 0.0f), new("Enter", 0.6944444f, 0.0f), new("Enter", 0.6944444f, 0.0f), new("Num 4"), new("Num 5"), new("Num 6") },
            new KeyDef[] { new("Shift", 2.25f, 0.0f), new("Z"), new("X"), new("C"), new("V"), new("B"), new("N"), new("M"), new(","), new("."), new("/"), new("Right Shift", 0.5277778f, 0.0f), new("Right Shift", 0.5277778f, 0.0f), new("Right Shift", 0.5277778f, 0.0f), new("↑"), new("Num 1"), new("Num 2"), new("Num 3"), new("Num Enter", 1f, 0.0f, 2f) },
            new KeyDef[] { new("Ctrl", 1.3333333f, 0.0f), new("Fn"), new("Win"), new("Alt"), new("Space", 0.9833333f, 0.0f), new("Space", 0.9833333f, 0.0f), new("Space", 0.9833333f, 0.0f), new("Space", 0.9833333f, 0.0f), new("Space", 0.9833333f, 0.0f), new("Right Alt"), new("PrtSc"), new("Right Ctrl"), new("←", 1f, 0.5833333f), new("↓"), new("→"), new("Num 0"), new("Num .") },
            Short(0.25f, new("Lightbar", 2.3541667f, 0.0f), new("Lightbar", 2.3541667f, 0.0f), new("Lightbar", 2.3541667f, 0.0f), new("Lightbar", 2.3541667f, 0.0f), new("Lightbar", 2.3541667f, 0.0f), new("Lightbar", 2.3541667f, 0.0f), new("Lightbar", 2.3541667f, 0.0f), new("Lightbar", 2.3541667f, 0.0f)),
        };

        private static readonly List<KeyDef[]> Strix2019Rows = new()
        {
            Short(0f, new("Vol-", 1f, 1.5f), new("Vol+"), new("Mic"), new("Fan"), new("ROG")),
            Short(0.25f, new("Esc"), new("F1", 1f, 0.5f), new("F2"), new("F3"), new("F4"), new("F5", 1f, 0.5833333f), new("F6"), new("F7"), new("F8"), new("F9", 1f, 0.5833333f), new("F10"), new("F11"), new("F12"), new("Del", 1f, 0.3333333f)),
            new KeyDef[] { new("`", 0.75f, 0.0f), new("1"), new("2"), new("3"), new("4"), new("5"), new("6"), new("7"), new("8"), new("9"), new("0"), new("-"), new("="), new("Bksp", 0.6388889f, 0.0f), new("Bksp", 0.6388889f, 0.0f), new("Bksp", 0.6388889f, 0.0f), new("Home", 1f, 0.3333333f) },
            new KeyDef[] { new("Tab", 1.3333333f, 0.0f), new("Q"), new("W"), new("E"), new("R"), new("T"), new("Y"), new("U"), new("I"), new("O"), new("P"), new("["), new("]"), new("\\", 1.3333333f, 0.0f), new("Pg Up", 1f, 0.3333333f) },
            new KeyDef[] { new("Caps", 1.5f, 0.0f), new("A"), new("S"), new("D"), new("F"), new("G"), new("H"), new("J"), new("K"), new("L"), new(";"), new("'"), new("Enter", 0.7222222f, 0.0f), new("Enter", 0.7222222f, 0.0f), new("Enter", 0.7222222f, 0.0f), new("Pg Dn", 1f, 0.3333333f) },
            new KeyDef[] { new("Shift", 2f, 0.0f), new("Z"), new("X"), new("C"), new("V"), new("B"), new("N"), new("M"), new(","), new("."), new("/"), new("Right Shift", 0.6666667f, 0.0f), new("Right Shift", 0.6666667f, 0.0f), new("Right Shift", 0.6666667f, 0.0f), new("Right Shift", 0.6666667f, 0.0f), new("End", 1f, 0.3333333f) },
            new KeyDef[] { new("Ctrl"), new("Fn"), new("Win"), new("Alt"), new("Space"), new("Space"), new("Space"), new("Space"), new("Space"), new("Right Alt"), new("PrtSc"), new("Right Ctrl"), new("←", 0.8333333f, 0.0833333f, 0.5f, 0.5f), new("↑", 0.8333333f, 0.0f, 0.5f), new("↓", 0.8333333f, -0.8333333f, 0.5f, 0.5f), new("→", 0.8333333f, 0.0f, 0.5f, 0.5f), new("Fn", 1f, 0.4166667f) },
        };

        private static readonly List<KeyDef[]> ZephyrusRows = new()
        {
            Short(0f, new("Aero", 4.3333333f, 0.5f), new("Aero", 4.3333333f, 5.3333333f)),
            Short(0f, new("Esc", 0.8627451f), new("F1", 0.8627451f), new("F2", 0.8627451f), new("F3", 0.8627451f), new("F4", 0.8627451f), new("F5", 0.8627451f), new("F6", 0.8627451f), new("F7", 0.8627451f), new("F8", 0.8627451f), new("F9", 0.8627451f), new("F10", 0.8627451f), new("F11", 0.8627451f), new("F12", 0.8627451f), new("Del", 0.8627451f), new("ROG", 0.8627451f), new("Screen", 0.8627451f), new("Arrows", 0.8627451f)),
            new KeyDef[] { new("`", 0.75f, 0.0f), new("1"), new("2"), new("3"), new("4"), new("5"), new("6"), new("7"), new("8"), new("9"), new("0"), new("-"), new("="), new("Bksp", 0.6388889f, 0.0f), new("Bksp", 0.6388889f, 0.0f), new("Bksp", 0.6388889f, 0.0f) },
            new KeyDef[] { new("Tab", 1.3333333f, 0.0f), new("Q"), new("W"), new("E"), new("R"), new("T"), new("Y"), new("U"), new("I"), new("O"), new("P"), new("["), new("]"), new("\\", 1.3333333f, 0.0f) },
            new KeyDef[] { new("Caps", 1.5f, 0.0f), new("A"), new("S"), new("D"), new("F"), new("G"), new("H"), new("J"), new("K"), new("L"), new(";"), new("'"), new("Enter", 0.7222222f, 0.0f), new("Enter", 0.7222222f, 0.0f), new("Enter", 0.7222222f, 0.0f) },
            new KeyDef[] { new("Shift", 2f, 0.0f), new("Z"), new("X"), new("C"), new("V"), new("B"), new("N"), new("M"), new(","), new("."), new("/"), new("Right Shift", 0.6666667f, 0.0f), new("Right Shift", 0.6666667f, 0.0f), new("Right Shift", 0.6666667f, 0.0f), new("Right Shift", 0.6666667f, 0.0f) },
            new KeyDef[] { new("Ctrl"), new("Fn"), new("Win"), new("Alt"), new("Space", 1.2f, 0.0f), new("Space", 1.2f, 0.0f), new("Space", 1.2f, 0.0f), new("Space", 1.2f, 0.0f), new("Space", 1.2f, 0.0f), new("Right Alt"), new("Right Ctrl"), new("←", 0.8333333f, 0.0833333f, 0.5f, 0.5f), new("↑", 0.8333333f, 0.0f, 0.5f), new("↓", 0.8333333f, -0.8333333f, 0.5f, 0.5f), new("→", 0.8333333f, 0.0f, 0.5f, 0.5f) },
        };

        private static readonly List<KeyDef[]> Strix17Rows = new()
        {
            Short(0f, new("Vol-", 1f, 2.0f), new("Vol+"), new("Mic"), new("Fan"), new("ROG")),
            Short(0.25f, new("Esc"), new("F1", 1f, 1.0f), new("F2"), new("F3"), new("F4"), new("F5", 1f, 0.4166667f), new("F6"), new("F7"), new("F8"), new("F9", 1f, 0.4166667f), new("F10"), new("F11"), new("F12"), new("Del", 1f, 0.4166667f), new("Pause"), new("PrtSc"), new("Home")),
            new KeyDef[] { new("`"), new("1"), new("2"), new("3"), new("4"), new("5"), new("6"), new("7"), new("8"), new("9"), new("0"), new("-"), new("="), new("Bksp", 0.6111111f, 0.0f), new("Bksp", 0.6111111f, 0.0f), new("Bksp", 0.6111111f, 0.0f), new("Num", 1f, 0.4166667f), new("Num /"), new("Num *"), new("Num -") },
            new KeyDef[] { new("Tab", 1.5f, 0.0f), new("Q"), new("W"), new("E"), new("R"), new("T"), new("Y"), new("U"), new("I"), new("O"), new("P"), new("["), new("]"), new("\\", 1.3333333f, 0.0f), new("Num 7", 1f, 0.4166667f), new("Num 8"), new("Num 9"), new("Num +", 1f, 0.0f, 2f) },
            new KeyDef[] { new("Caps", 1.75f, 0.0f), new("A"), new("S"), new("D"), new("F"), new("G"), new("H"), new("J"), new("K"), new("L"), new(";"), new("'"), new("Enter", 0.6944444f, 0.0f), new("Enter", 0.6944444f, 0.0f), new("Enter", 0.6944444f, 0.0f), new("Num 4", 1f, 0.4166667f), new("Num 5"), new("Num 6") },
            new KeyDef[] { new("Shift", 2.25f, 0.0f), new("Z"), new("X"), new("C"), new("V"), new("B"), new("N"), new("M"), new(","), new("."), new("/"), new("Right Shift", 0.6458333f, 0.0f), new("Right Shift", 0.6458333f, 0.0f), new("Right Shift", 0.6458333f, 0.0f), new("Right Shift", 0.6458333f, 0.0f), new("Num 1", 1f, 0.4166667f), new("Num 2"), new("Num 3"), new("Num Enter", 1f, 0.0f, 2f) },
            new KeyDef[] { new("Ctrl", 1.3333333f, 0.0f), new("Fn"), new("Win"), new("Alt"), new("Space", 1.0333333f, 0.0f), new("Space", 1.0333333f, 0.0f), new("Space", 1.0333333f, 0.0f), new("Space", 1.0333333f, 0.0f), new("Space", 1.0333333f, 0.0f), new("Right Alt"), new("Right Ctrl", 1.1666667f, 0.0f), new("←", 1f, 0.1666667f, 0.5f, 0.5f), new("↑", 1f, 0.0f, 0.5f), new("↓", 1f, -1.0f, 0.5f, 0.5f), new("→", 1f, 0.0f, 0.5f, 0.5f), new("Num 0", 2f, 0.4166667f), new("Num .") },
            Short(0.25f, new("Lightbar", 3.2083333f, 0.0f), new("Lightbar", 3.2083333f, 0.0f), new("Lightbar", 3.2083333f, 0.0f), new("Lightbar", 3.2083333f, 0.0f), new("Lightbar", 3.2083333f, 0.0f), new("Lightbar", 3.2083333f, 0.0f)),
        };

        public static readonly Dictionary<string, LayoutData> Data = new()
        {
            ["Strix15"] = new(Strix15Rows,
            new byte[] { 2, 3, 4, 5, 6, 21, 23, 24, 25, 26, 28, 29, 30, 31, 33, 34, 35, 36, 37, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 79, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 97, 98, 99, 100, 105, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 159, 139, 160, 161, 142, 174, 173, 172, 171, 170, 169 }),
            ["Strix16"] = new(Strix16Rows,
            new byte[] { 2, 3, 4, 5, 6, 21, 23, 24, 25, 26, 28, 29, 30, 31, 33, 34, 35, 36, 37, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 79, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 97, 98, 99, 100, 105, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 140, 141, 142, 173, 172, 170, 169 }),
            ["Scar15"] = new(Scar15Rows,
            new byte[] { 2, 3, 4, 5, 6, 21, 23, 24, 25, 26, 28, 29, 30, 31, 33, 34, 35, 36, 37, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 79, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 97, 98, 99, 100, 105, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 139, 121, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 142, 137, 159, 160, 161, 174, 173, 172, 171, 170, 169 }),
            ["Strix18"] = new(Strix18Rows,
            new byte[] { 2, 3, 4, 5, 6, 21, 23, 24, 25, 26, 28, 29, 30, 31, 33, 34, 35, 36, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 97, 98, 99, 101, 102, 103, 105, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 141, 142, 143, 144, 145, 176, 174, 173, 172, 171, 170, 169, 177 }),
            ["Strix2019"] = new(Strix2019Rows,
            new byte[] { 2, 3, 4, 5, 6, 21, 23, 24, 25, 26, 28, 29, 30, 31, 33, 34, 35, 36, 37, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 79, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 97, 98, 99, 100, 105, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 159, 139, 160, 161, 142 }),
            ["Zephyrus"] = new(ZephyrusRows,
            new byte[] { 0, 19, 21, 23, 24, 25, 26, 28, 29, 30, 31, 33, 34, 35, 36, 37, 38, 39, 40, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 97, 98, 99, 105, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 138, 159, 140, 160, 161 }),
            ["Strix17"] = new(Strix17Rows,
            new byte[] { 2, 3, 4, 5, 6, 21, 23, 24, 25, 26, 28, 29, 30, 31, 33, 34, 35, 36, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 97, 98, 99, 101, 102, 103, 105, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 137, 159, 139, 160, 161, 144, 145, 174, 173, 172, 171, 170, 169 }),
            ["Azoth"] = new(AzothRows,
            new byte[] { 0x00, 0x08, 0x10, 0x18, 0x20, 0x28, 0x30, 0x38, 0x40, 0x48, 0x50, 0x58, 0x60, 0x01, 0x09, 0x11, 0x19, 0x21, 0x29, 0x31, 0x39, 0x41, 0x49, 0x51, 0x59, 0x61, 0x69, 0x79, 0x02, 0x0A, 0x12, 0x1A, 0x22, 0x2A, 0x32, 0x3A, 0x42, 0x4A, 0x52, 0x5A, 0x62, 0x6A, 0x7A, 0x03, 0x0B, 0x13, 0x1B, 0x23, 0x2B, 0x33, 0x3B, 0x43, 0x4B, 0x53, 0x5B, 0x6B, 0x7B, 0x04, 0x14, 0x1C, 0x24, 0x2C, 0x34, 0x3C, 0x44, 0x4C, 0x54, 0x5C, 0x64, 0x74, 0x7C, 0x05, 0x0D, 0x15, 0x35, 0x55, 0x5D, 0x65, 0x6D, 0x75, 0x7D }),
            ["AzothISO"] = new(ToIso(AzothRows),
            new byte[] { 0x00, 0x08, 0x10, 0x18, 0x20, 0x28, 0x30, 0x38, 0x40, 0x48, 0x50, 0x58, 0x60, 0x01, 0x09, 0x11, 0x19, 0x21, 0x29, 0x31, 0x39, 0x41, 0x49, 0x51, 0x59, 0x61, 0x69, 0x79, 0x02, 0x0A, 0x12, 0x1A, 0x22, 0x2A, 0x32, 0x3A, 0x42, 0x4A, 0x52, 0x5A, 0x62, 0x6B, 0x7A, 0x03, 0x0B, 0x13, 0x1B, 0x23, 0x2B, 0x33, 0x3B, 0x43, 0x4B, 0x53, 0x5B, 0x63, 0x6B, 0x7B, 0x04, 0x0C, 0x14, 0x1C, 0x24, 0x2C, 0x34, 0x3C, 0x44, 0x4C, 0x54, 0x5C, 0x64, 0x74, 0x7C, 0x05, 0x0D, 0x15, 0x35, 0x55, 0x5D, 0x65, 0x6D, 0x75, 0x7D }),
            ["StrixScopeII"] = new(StrixScopeIIRows,
            new byte[] { 0x00, 0x18, 0x20, 0x28, 0x30, 0x40, 0x48, 0x50, 0x58, 0x60, 0x68, 0x70, 0x78, 0x80, 0x88, 0x90, 0xA0, 0x01, 0x11, 0x19, 0x21, 0x29, 0x31, 0x39, 0x41, 0x49, 0x51, 0x59, 0x61, 0x69, 0x79, 0x81, 0x89, 0x91, 0x99, 0xA1, 0xA9, 0xB1, 0x02, 0x12, 0x1A, 0x22, 0x2A, 0x32, 0x3A, 0x42, 0x4A, 0x52, 0x5A, 0x62, 0x6A, 0x7A, 0x82, 0x8A, 0x92, 0x9A, 0xA2, 0xAA, 0xB2, 0x03, 0x13, 0x1B, 0x23, 0x2B, 0x33, 0x3B, 0x43, 0x4B, 0x53, 0x5B, 0x63, 0x7B, 0x9B, 0xA3, 0xAB, 0x04, 0x14, 0x1C, 0x24, 0x2C, 0x34, 0x3C, 0x44, 0x4C, 0x54, 0x5C, 0x7C, 0x8C, 0x9C, 0xA4, 0xAC, 0xB4, 0x05, 0x0D, 0x15, 0x2D, 0x35, 0x3D, 0x4D, 0x5D, 0x65, 0x7D, 0x85, 0x8D, 0x95, 0x9D, 0xAD }),
            ["StrixScopeII96"] = new(StrixScopeII96Rows,
            new byte[] { 0x00, 0x08, 0x10, 0x18, 0x20, 0x28, 0x30, 0x38, 0x40, 0x48, 0x50, 0x58, 0x60, 0x68, 0x70, 0x78, 0x80, 0x01, 0x09, 0x11, 0x19, 0x21, 0x29, 0x31, 0x39, 0x41, 0x49, 0x51, 0x59, 0x61, 0x69, 0x79, 0x81, 0x89, 0x91, 0x02, 0x0A, 0x12, 0x1A, 0x22, 0x2A, 0x32, 0x3A, 0x42, 0x4A, 0x52, 0x5A, 0x62, 0x6A, 0x7A, 0x82, 0x8A, 0x92, 0x03, 0x0B, 0x13, 0x1B, 0x23, 0x2B, 0x33, 0x3B, 0x43, 0x4B, 0x53, 0x5B, 0x6B, 0x7B, 0x83, 0x8B, 0x04, 0x14, 0x1C, 0x24, 0x2C, 0x34, 0x3C, 0x44, 0x4C, 0x54, 0x5C, 0x64, 0x74, 0x7C, 0x84, 0x8C, 0x94, 0x05, 0x0D, 0x15, 0x2D, 0x35, 0x3D, 0x55, 0x5D, 0x65, 0x6D, 0x75, 0x7D, 0x85, 0x8D }),
            ["StrixScopeII96Rx"] = new(StrixScopeII96RxRows,
            new byte[] { 0x00, 0x08, 0x10, 0x18, 0x20, 0x28, 0x30, 0x38, 0x40, 0x48, 0x50, 0x58, 0x60, 0x68, 0x70, 0x78, 0x80, 0x01, 0x09, 0x11, 0x19, 0x21, 0x29, 0x31, 0x39, 0x41, 0x49, 0x51, 0x59, 0x61, 0x69, 0x79, 0x81, 0x89, 0x91, 0x02, 0x0A, 0x12, 0x1A, 0x22, 0x2A, 0x32, 0x3A, 0x42, 0x4A, 0x52, 0x5A, 0x62, 0x6A, 0x7A, 0x82, 0x8A, 0x92, 0x03, 0x0B, 0x13, 0x1B, 0x23, 0x2B, 0x33, 0x3B, 0x43, 0x4B, 0x53, 0x5B, 0x6B, 0x7B, 0x83, 0x8B, 0x04, 0x14, 0x1C, 0x24, 0x2C, 0x34, 0x3C, 0x44, 0x4C, 0x54, 0x5C, 0x64, 0x74, 0x7C, 0x84, 0x8C, 0x94, 0x05, 0x0D, 0x15, 0x25, 0x2D, 0x35, 0x3D, 0x45, 0x55, 0x5D, 0x65, 0x6D, 0x75, 0x7D, 0x85, 0x8D }),
            ["StrixFlareII"] = new(FullSizeRows,
            new byte[] { 0x00, 0x18, 0x20, 0x28, 0x30, 0x40, 0x48, 0x50, 0x58, 0x60, 0x68, 0x70, 0x78, 0x80, 0x88, 0x90, 0x01, 0x11, 0x19, 0x21, 0x29, 0x31, 0x39, 0x41, 0x49, 0x51, 0x59, 0x61, 0x69, 0x79, 0x81, 0x89, 0x91, 0x99, 0xA1, 0xA9, 0xB1, 0x02, 0x12, 0x1A, 0x22, 0x2A, 0x32, 0x3A, 0x42, 0x4A, 0x52, 0x5A, 0x62, 0x6A, 0x7A, 0x82, 0x8A, 0x92, 0x9A, 0xA2, 0xAA, 0xB2, 0x03, 0x13, 0x1B, 0x23, 0x2B, 0x33, 0x3B, 0x43, 0x4B, 0x53, 0x5B, 0x63, 0x7B, 0x9B, 0xA3, 0xAB, 0x04, 0x14, 0x1C, 0x24, 0x2C, 0x34, 0x3C, 0x44, 0x4C, 0x54, 0x5C, 0x7C, 0x8C, 0x9C, 0xA4, 0xAC, 0xB4, 0x05, 0x15, 0x1D, 0x35, 0x4D, 0x5D, 0x65, 0x7D, 0x85, 0x8D, 0x95, 0x9D, 0xAD }),
            ["Falchion"] = new(FalchionRows,
            new byte[] { 0x00, 0x08, 0x10, 0x18, 0x20, 0x28, 0x30, 0x38, 0x40, 0x48, 0x50, 0x58, 0x60, 0x68, 0x70, 0x01, 0x09, 0x11, 0x19, 0x21, 0x29, 0x31, 0x39, 0x41, 0x49, 0x51, 0x59, 0x61, 0x69, 0x71, 0x02, 0x0A, 0x12, 0x1A, 0x22, 0x2A, 0x32, 0x3A, 0x42, 0x4A, 0x52, 0x5A, 0x6A, 0x72, 0x03, 0x13, 0x1B, 0x23, 0x2B, 0x33, 0x3B, 0x43, 0x4B, 0x53, 0x5B, 0x63, 0x6B, 0x73, 0x04, 0x0C, 0x14, 0x34, 0x4C, 0x54, 0x5C, 0x64, 0x6C, 0x74 }),
            // RX Low Profile LED matrix reserves row 0 for an F row the 65% doesn't have: every id is Falchion +1
            ["FalchionRX"] = new(FalchionRows,
            new byte[] { 0x01, 0x09, 0x11, 0x19, 0x21, 0x29, 0x31, 0x39, 0x41, 0x49, 0x51, 0x59, 0x61, 0x69, 0x71, 0x02, 0x0A, 0x12, 0x1A, 0x22, 0x2A, 0x32, 0x3A, 0x42, 0x4A, 0x52, 0x5A, 0x62, 0x6A, 0x72, 0x03, 0x0B, 0x13, 0x1B, 0x23, 0x2B, 0x33, 0x3B, 0x43, 0x4B, 0x53, 0x5B, 0x6B, 0x73, 0x04, 0x14, 0x1C, 0x24, 0x2C, 0x34, 0x3C, 0x44, 0x4C, 0x54, 0x5C, 0x64, 0x6C, 0x74, 0x05, 0x0D, 0x15, 0x35, 0x4D, 0x55, 0x5D, 0x65, 0x6D, 0x75 }),
            ["TUFK7"] = new(FullSizeRows,
            new byte[] { 0x00, 0x18, 0x20, 0x28, 0x30, 0x40, 0x48, 0x50, 0x58, 0x60, 0x68, 0x70, 0x78, 0x80, 0x88, 0x90, 0x01, 0x11, 0x19, 0x21, 0x29, 0x31, 0x39, 0x41, 0x49, 0x51, 0x59, 0x61, 0x69, 0x79, 0x81, 0x89, 0x91, 0x99, 0xA1, 0xA9, 0xB1, 0x02, 0x12, 0x1A, 0x22, 0x2A, 0x32, 0x3A, 0x42, 0x4A, 0x52, 0x5A, 0x62, 0x6A, 0x7A, 0x82, 0x8A, 0x92, 0x9A, 0xA2, 0xAA, 0xB2, 0x03, 0x13, 0x1B, 0x23, 0x2B, 0x33, 0x3B, 0x43, 0x4B, 0x53, 0x5B, 0x63, 0x7B, 0x9B, 0xA3, 0xAB, 0x04, 0x14, 0x1C, 0x24, 0x2C, 0x34, 0x3C, 0x44, 0x4C, 0x54, 0x5C, 0x7C, 0x8C, 0x9C, 0xA4, 0xAC, 0xB4, 0x05, 0x0D, 0x15, 0x35, 0x4D, 0x5D, 0x65, 0x7D, 0x85, 0x8D, 0x95, 0x9D, 0xAD }),
            ["TUFK3GenII"] = new(TUFK3GenIIRows,
            new byte[] { 0x00, 0x10, 0x18, 0x20, 0x28, 0x30, 0x38, 0x40, 0x48, 0x58, 0x60, 0x68, 0x70, 0x80, 0x78, 0x88, 0x90, 0x01, 0x09, 0x11, 0x19, 0x21, 0x29, 0x31, 0x39, 0x41, 0x49, 0x51, 0x59, 0x61, 0x71, 0x79, 0x81, 0x89, 0x91, 0x02, 0x0A, 0x12, 0x1A, 0x22, 0x2A, 0x32, 0x3A, 0x42, 0x4A, 0x52, 0x5A, 0x62, 0x72, 0x7A, 0x82, 0x8A, 0x92, 0x03, 0x0B, 0x13, 0x1B, 0x23, 0x2B, 0x33, 0x3B, 0x43, 0x4B, 0x53, 0x5B, 0x73, 0x7B, 0x83, 0x8B, 0x04, 0x14, 0x1C, 0x24, 0x2C, 0x34, 0x3C, 0x44, 0x4C, 0x54, 0x5C, 0x6C, 0x74, 0x7C, 0x84, 0x8C, 0x94, 0x05, 0x0D, 0x15, 0x35, 0x55, 0x5D, 0x6D, 0x75, 0x7D, 0x85, 0x8D }),
            ["ClaymoreNoNumpad"] = new(ClaymoreNoNumpadRows,
            new byte[] { 0x00, 0x10, 0x18, 0x20, 0x28, 0x38, 0x40, 0x48, 0x50, 0x60, 0x68, 0x70, 0x78, 0x80, 0x88, 0x90, 0x01, 0x09, 0x11, 0x19, 0x21, 0x29, 0x31, 0x39, 0x41, 0x49, 0x51, 0x59, 0x61, 0x69, 0x81, 0x89, 0x91, 0x02, 0x0A, 0x12, 0x1A, 0x22, 0x2A, 0x32, 0x3A, 0x42, 0x4A, 0x52, 0x5A, 0x62, 0x6A, 0x82, 0x8A, 0x92, 0x03, 0x0B, 0x13, 0x1B, 0x23, 0x2B, 0x33, 0x3B, 0x43, 0x4B, 0x53, 0x5B, 0x6B, 0x04, 0x0C, 0x14, 0x1C, 0x24, 0x2C, 0x34, 0x3C, 0x44, 0x4C, 0x54, 0x6C, 0x8C, 0x05, 0x0D, 0x15, 0x25, 0x4D, 0x55, 0x5D, 0x6D, 0x85, 0x8D, 0x95 }),
            ["StrixScopeRXTKL"] = new(StrixScopeRXTKLRows,
            new byte[] { 0x00, 0x18, 0x20, 0x28, 0x30, 0x40, 0x48, 0x50, 0x58, 0x60, 0x68, 0x70, 0x78, 0x01, 0x11, 0x19, 0x21, 0x29, 0x31, 0x39, 0x41, 0x49, 0x51, 0x59, 0x61, 0x69, 0x79, 0x81, 0x89, 0x91, 0x02, 0x12, 0x1A, 0x22, 0x2A, 0x32, 0x3A, 0x42, 0x4A, 0x52, 0x5A, 0x62, 0x6A, 0x7A, 0x82, 0x8A, 0x92, 0x03, 0x13, 0x1B, 0x23, 0x2B, 0x33, 0x3B, 0x43, 0x4B, 0x53, 0x5B, 0x63, 0x7B, 0x04, 0x14, 0x1C, 0x24, 0x2C, 0x34, 0x3C, 0x44, 0x4C, 0x54, 0x5C, 0x7C, 0x8C, 0x05, 0x15, 0x1D, 0x35, 0x4D, 0x5D, 0x65, 0x7D, 0x85, 0x8D, 0x95 }),
            ["StrixScopeIIISO"] = new(ToIso(StrixScopeIIRows),
            new byte[] { 0x00, 0x18, 0x20, 0x28, 0x30, 0x40, 0x48, 0x50, 0x58, 0x60, 0x68, 0x70, 0x78, 0x80, 0x88, 0x90, 0xA0, 0x01, 0x11, 0x19, 0x21, 0x29, 0x31, 0x39, 0x41, 0x49, 0x51, 0x59, 0x61, 0x69, 0x79, 0x81, 0x89, 0x91, 0x99, 0xA1, 0xA9, 0xB1, 0x02, 0x12, 0x1A, 0x22, 0x2A, 0x32, 0x3A, 0x42, 0x4A, 0x52, 0x5A, 0x62, 0x6A, 0x7B, 0x82, 0x8A, 0x92, 0x9A, 0xA2, 0xAA, 0xB2, 0x03, 0x13, 0x1B, 0x23, 0x2B, 0x33, 0x3B, 0x43, 0x4B, 0x53, 0x5B, 0x63, 0x6B, 0x7B, 0x9B, 0xA3, 0xAB, 0x04, 0x0C, 0x14, 0x1C, 0x24, 0x2C, 0x34, 0x3C, 0x44, 0x4C, 0x54, 0x5C, 0x7C, 0x8C, 0x9C, 0xA4, 0xAC, 0xB4, 0x05, 0x0D, 0x15, 0x2D, 0x35, 0x3D, 0x4D, 0x5D, 0x65, 0x7D, 0x85, 0x8D, 0x95, 0x9D, 0xAD }),
            ["StrixScopeRXTKLISO"] = new(ToIso(StrixScopeRXTKLRows),
            new byte[] { 0x00, 0x18, 0x20, 0x28, 0x30, 0x40, 0x48, 0x50, 0x58, 0x60, 0x68, 0x70, 0x78, 0x01, 0x11, 0x19, 0x21, 0x29, 0x31, 0x39, 0x41, 0x49, 0x51, 0x59, 0x61, 0x69, 0x79, 0x81, 0x89, 0x91, 0x02, 0x12, 0x1A, 0x22, 0x2A, 0x32, 0x3A, 0x42, 0x4A, 0x52, 0x5A, 0x62, 0x6A, 0x7B, 0x82, 0x8A, 0x92, 0x03, 0x13, 0x1B, 0x23, 0x2B, 0x33, 0x3B, 0x43, 0x4B, 0x53, 0x5B, 0x63, 0x6B, 0x7B, 0x04, 0x0C, 0x14, 0x1C, 0x24, 0x2C, 0x34, 0x3C, 0x44, 0x4C, 0x54, 0x5C, 0x7C, 0x8C, 0x05, 0x15, 0x1D, 0x35, 0x4D, 0x5D, 0x65, 0x7D, 0x85, 0x8D, 0x95 }),
            ["StrixFlareIIISO"] = new(ToIso(FullSizeRows),
            new byte[] { 0x00, 0x18, 0x20, 0x28, 0x30, 0x40, 0x48, 0x50, 0x58, 0x60, 0x68, 0x70, 0x78, 0x80, 0x88, 0x90, 0x01, 0x11, 0x19, 0x21, 0x29, 0x31, 0x39, 0x41, 0x49, 0x51, 0x59, 0x61, 0x69, 0x79, 0x81, 0x89, 0x91, 0x99, 0xA1, 0xA9, 0xB1, 0x02, 0x12, 0x1A, 0x22, 0x2A, 0x32, 0x3A, 0x42, 0x4A, 0x52, 0x5A, 0x62, 0x6A, 0x7B, 0x82, 0x8A, 0x92, 0x9A, 0xA2, 0xAA, 0xB2, 0x03, 0x13, 0x1B, 0x23, 0x2B, 0x33, 0x3B, 0x43, 0x4B, 0x53, 0x5B, 0x63, 0x6B, 0x7B, 0x9B, 0xA3, 0xAB, 0x04, 0x0C, 0x14, 0x1C, 0x24, 0x2C, 0x34, 0x3C, 0x44, 0x4C, 0x54, 0x5C, 0x7C, 0x8C, 0x9C, 0xA4, 0xAC, 0xB4, 0x05, 0x15, 0x1D, 0x35, 0x4D, 0x5D, 0x65, 0x7D, 0x85, 0x8D, 0x95, 0x9D, 0xAD }),
            ["TUFK7ISO"] = new(ToIso(FullSizeRows),
            new byte[] { 0x00, 0x18, 0x20, 0x28, 0x30, 0x40, 0x48, 0x50, 0x58, 0x60, 0x68, 0x70, 0x78, 0x80, 0x88, 0x90, 0x01, 0x11, 0x19, 0x21, 0x29, 0x31, 0x39, 0x41, 0x49, 0x51, 0x59, 0x61, 0x69, 0x79, 0x81, 0x89, 0x91, 0x99, 0xA1, 0xA9, 0xB1, 0x02, 0x12, 0x1A, 0x22, 0x2A, 0x32, 0x3A, 0x42, 0x4A, 0x52, 0x5A, 0x62, 0x6A, 0x7B, 0x82, 0x8A, 0x92, 0x9A, 0xA2, 0xAA, 0xB2, 0x03, 0x13, 0x1B, 0x23, 0x2B, 0x33, 0x3B, 0x43, 0x4B, 0x53, 0x5B, 0x63, 0x6B, 0x7B, 0x9B, 0xA3, 0xAB, 0x04, 0x0C, 0x14, 0x1C, 0x24, 0x2C, 0x34, 0x3C, 0x44, 0x4C, 0x54, 0x5C, 0x7C, 0x8C, 0x9C, 0xA4, 0xAC, 0xB4, 0x05, 0x0D, 0x15, 0x35, 0x4D, 0x5D, 0x65, 0x7D, 0x85, 0x8D, 0x95, 0x9D, 0xAD }),
            ["TUFK3GenIIISO"] = new(ToIso(TUFK3GenIIRows),
            new byte[] { 0x00, 0x10, 0x18, 0x20, 0x28, 0x30, 0x38, 0x40, 0x48, 0x58, 0x60, 0x68, 0x70, 0x80, 0x78, 0x88, 0x90, 0x01, 0x09, 0x11, 0x19, 0x21, 0x29, 0x31, 0x39, 0x41, 0x49, 0x51, 0x59, 0x61, 0x71, 0x79, 0x81, 0x89, 0x91, 0x02, 0x0A, 0x12, 0x1A, 0x22, 0x2A, 0x32, 0x3A, 0x42, 0x4A, 0x52, 0x5A, 0x62, 0x73, 0x7A, 0x82, 0x8A, 0x92, 0x03, 0x0B, 0x13, 0x1B, 0x23, 0x2B, 0x33, 0x3B, 0x43, 0x4B, 0x53, 0x5B, 0x6B, 0x73, 0x7B, 0x83, 0x8B, 0x04, 0x0C, 0x14, 0x1C, 0x24, 0x2C, 0x34, 0x3C, 0x44, 0x4C, 0x54, 0x5C, 0x6C, 0x74, 0x7C, 0x84, 0x8C, 0x94, 0x05, 0x0D, 0x15, 0x35, 0x55, 0x5D, 0x6D, 0x75, 0x7D, 0x85, 0x8D }),
            ["FalchionISO"] = new(ToIso(FalchionRows),
            new byte[] { 0x00, 0x08, 0x10, 0x18, 0x20, 0x28, 0x30, 0x38, 0x40, 0x48, 0x50, 0x58, 0x60, 0x68, 0x70, 0x01, 0x09, 0x11, 0x19, 0x21, 0x29, 0x31, 0x39, 0x41, 0x49, 0x51, 0x59, 0x61, 0x6A, 0x71, 0x02, 0x0A, 0x12, 0x1A, 0x22, 0x2A, 0x32, 0x3A, 0x42, 0x4A, 0x52, 0x5A, 0x62, 0x6A, 0x72, 0x03, 0x0B, 0x13, 0x1B, 0x23, 0x2B, 0x33, 0x3B, 0x43, 0x4B, 0x53, 0x5B, 0x63, 0x6B, 0x73, 0x04, 0x0C, 0x14, 0x34, 0x4C, 0x54, 0x5C, 0x64, 0x6C, 0x74 }),
            ["FalchionRXISO"] = new(ToIso(FalchionRows),
            new byte[] { 0x01, 0x09, 0x11, 0x19, 0x21, 0x29, 0x31, 0x39, 0x41, 0x49, 0x51, 0x59, 0x61, 0x69, 0x71, 0x02, 0x0A, 0x12, 0x1A, 0x22, 0x2A, 0x32, 0x3A, 0x42, 0x4A, 0x52, 0x5A, 0x62, 0x6B, 0x72, 0x03, 0x0B, 0x13, 0x1B, 0x23, 0x2B, 0x33, 0x3B, 0x43, 0x4B, 0x53, 0x5B, 0x63, 0x6B, 0x73, 0x04, 0x0C, 0x14, 0x1C, 0x24, 0x2C, 0x34, 0x3C, 0x44, 0x4C, 0x54, 0x5C, 0x64, 0x6C, 0x74, 0x05, 0x0D, 0x15, 0x35, 0x4D, 0x55, 0x5D, 0x65, 0x6D, 0x75 }),
            ["StrixScopeII96ISO"] = new(ToIso(StrixScopeII96Rows),
            new byte[] { 0x00, 0x08, 0x10, 0x18, 0x20, 0x28, 0x30, 0x38, 0x40, 0x48, 0x50, 0x58, 0x60, 0x68, 0x70, 0x78, 0x80, 0x01, 0x09, 0x11, 0x19, 0x21, 0x29, 0x31, 0x39, 0x41, 0x49, 0x51, 0x59, 0x61, 0x69, 0x79, 0x81, 0x89, 0x91, 0x02, 0x0A, 0x12, 0x1A, 0x22, 0x2A, 0x32, 0x3A, 0x42, 0x4A, 0x52, 0x5A, 0x62, 0x6B, 0x7A, 0x82, 0x8A, 0x92, 0x03, 0x0B, 0x13, 0x1B, 0x23, 0x2B, 0x33, 0x3B, 0x43, 0x4B, 0x53, 0x5B, 0x63, 0x6B, 0x7B, 0x83, 0x8B, 0x04, 0x0C, 0x14, 0x1C, 0x24, 0x2C, 0x34, 0x3C, 0x44, 0x4C, 0x54, 0x5C, 0x64, 0x74, 0x7C, 0x84, 0x8C, 0x94, 0x05, 0x0D, 0x15, 0x2D, 0x35, 0x3D, 0x55, 0x5D, 0x65, 0x6D, 0x75, 0x7D, 0x85, 0x8D }),
            ["StrixScopeII96RxISO"] = new(ToIso(StrixScopeII96RxRows),
            new byte[] { 0x00, 0x08, 0x10, 0x18, 0x20, 0x28, 0x30, 0x38, 0x40, 0x48, 0x50, 0x58, 0x60, 0x68, 0x70, 0x78, 0x80, 0x01, 0x09, 0x11, 0x19, 0x21, 0x29, 0x31, 0x39, 0x41, 0x49, 0x51, 0x59, 0x61, 0x69, 0x79, 0x81, 0x89, 0x91, 0x02, 0x0A, 0x12, 0x1A, 0x22, 0x2A, 0x32, 0x3A, 0x42, 0x4A, 0x52, 0x5A, 0x62, 0x6B, 0x7A, 0x82, 0x8A, 0x92, 0x03, 0x0B, 0x13, 0x1B, 0x23, 0x2B, 0x33, 0x3B, 0x43, 0x4B, 0x53, 0x5B, 0x63, 0x6B, 0x7B, 0x83, 0x8B, 0x04, 0x0C, 0x14, 0x1C, 0x24, 0x2C, 0x34, 0x3C, 0x44, 0x4C, 0x54, 0x5C, 0x64, 0x74, 0x7C, 0x84, 0x8C, 0x94, 0x05, 0x0D, 0x15, 0x25, 0x2D, 0x35, 0x3D, 0x45, 0x55, 0x5D, 0x65, 0x6D, 0x75, 0x7D, 0x85, 0x8D }),
            ["ClaymoreNoNumpadISO"] = new(ToIso(ClaymoreNoNumpadRows),
            new byte[] { 0x00, 0x10, 0x18, 0x20, 0x28, 0x38, 0x40, 0x48, 0x50, 0x60, 0x68, 0x70, 0x78, 0x80, 0x88, 0x90, 0x01, 0x09, 0x11, 0x19, 0x21, 0x29, 0x31, 0x39, 0x41, 0x49, 0x51, 0x59, 0x61, 0x69, 0x81, 0x89, 0x91, 0x02, 0x0A, 0x12, 0x1A, 0x22, 0x2A, 0x32, 0x3A, 0x42, 0x4A, 0x52, 0x5A, 0x62, 0x6B, 0x82, 0x8A, 0x92, 0x03, 0x0B, 0x13, 0x1B, 0x23, 0x2B, 0x33, 0x3B, 0x43, 0x4B, 0x53, 0x5B, 0x63, 0x6B, 0x04, 0x0C, 0x14, 0x1C, 0x24, 0x2C, 0x34, 0x3C, 0x44, 0x4C, 0x54, 0x5C, 0x6C, 0x8C, 0x05, 0x0D, 0x15, 0x25, 0x4D, 0x55, 0x5D, 0x6D, 0x85, 0x8D, 0x95 }),
        };

        private static readonly (string Model, string Layout)[] LaptopLayouts =
        {
            ("G733P", "Strix17"), ("G713", "Strix17"),
            ("G814", "Strix18"), ("G815", "Strix18"), ("G834", "Strix18"), ("G835", "Strix18"),
            ("G614", "Strix16"), ("G615", "Strix16"), ("G634", "Strix16"), ("G635", "Strix16"),
            ("G533", "Scar15"),
            ("GX502", "Strix2019"), ("G532", "Strix2019"), ("G531", "Strix2019"), ("GL531", "Strix2019"), ("G731", "Strix2019"), ("G512", "Strix2019"),
            ("GX531", "Zephyrus"), ("GX550", "Zephyrus"), ("GX551", "Zephyrus"), ("GX650", "Zephyrus"), ("GX701", "Zephyrus"),
        };

        public static IEnumerable<string> LaptopNames => LaptopLayouts.Select(l => l.Layout).Append("Strix15").Distinct().OrderBy(n => n);

        private static readonly string[] IsoLaptops = { "Strix16", "Zephyrus" };

        public static LayoutData Laptop(string? name = null, bool iso = false)
        {
            if (name is not null && Data.TryGetValue(name, out var layout)) return layout;
            name = LaptopLayouts.FirstOrDefault(l => AppConfig.ContainsModel(l.Model)).Layout ?? "Strix15";
            layout = Data[name];
            if (!iso || !IsoLaptops.Contains(name)) return layout;
            var rows = layout.Rows.Select(row => row.SelectMany(def => def.Name switch
            {
                "\\" => new[] { def with { Name = "#" } },
                "Shift" => new[] { def with { Width = def.Width - 1 }, new KeyDef("<") },
                _ => new[] { def },
            }).ToArray()).ToList();
            return new(rows, layout.LedIds.SelectMany(id => id == 105 ? new byte[] { 105, 106 } : new[] { id }).ToArray());
        }

        public static int[] Zones(List<KeyDef[]> rows)
        {
            float width = rows.Max(row => row.Sum(key => key.Gap + key.Width));
            var zones = new List<int>();
            foreach (var row in rows)
            {
                float x = 0;
                for (int i = 0; i < row.Length; i++)
                {
                    x += row[i].Gap;
                    zones.Add(row[i].Name == "Lightbar" ? 4 + i * 4 / row.Length : Math.Clamp((int)(4 * (x + row[i].Width / 2) / width), 0, 3));
                    x += row[i].Width;
                }
            }
            return zones.ToArray();
        }

        private const int LAMP_ROW_SPREAD_UM = 6000;
        private const int LAMP_PITCH_UM = 19000;
        private static readonly HashSet<int> WideKeys = new() { 0x28, 0x2A, 0x2B, 0x2C, 0x31, 0x39, 0xE0, 0xE1, 0xE5 };

        public static (List<KeyDef[]> Rows, int[] Targets)? MapLamps(LayoutData layout, (int X, int Y, int Usage, int Purpose)[] lamps)
        {
            var all = Enumerable.Range(0, lamps.Length).ToList();
            bool IsKey(int i) => (lamps[i].Purpose & 0x01) != 0;
            bool IsLogo(int i) => !IsKey(i) && (lamps[i].Purpose & 0x04) != 0;

            var byName = all.Where(i => IsKey(i) && UsageName(lamps[i].Usage) != "").GroupBy(i => UsageName(lamps[i].Usage)).ToDictionary(g => g.Key, g => g.First());
            if (byName.TryGetValue("\\", out int backslash)) byName.TryAdd("#", backslash);
            if (byName.Count == 0) return null;

            var keyRows = new List<List<int>>();
            foreach (int i in all.Where(IsKey).OrderBy(i => lamps[i].Y))
            {
                if (keyRows.Count == 0 || lamps[i].Y - lamps[keyRows[^1][0]].Y > LAMP_ROW_SPREAD_UM) keyRows.Add(new List<int>());
                keyRows[^1].Add(i);
            }
            var blanks = keyRows.Select(r => new Queue<int>(r.Where(i => lamps[i].Usage == 0).OrderBy(i => lamps[i].X))).ToList();

            int keyTop = lamps[keyRows[0][0]].Y;
            double mid = (lamps.Min(l => l.X) + lamps.Max(l => l.X)) / 2.0;
            var bars = all.Where(i => !IsKey(i) && !IsLogo(i)).ToList();
            var front = bars.Where(i => lamps[i].Y >= keyTop).OrderBy(i => lamps[i].X).ThenBy(i => lamps[i].X < mid ? lamps[i].Y : -lamps[i].Y).ToList();
            var rear = bars.Where(i => lamps[i].Y < keyTop).OrderBy(i => lamps[i].X).ToList();
            var logo = all.Where(IsLogo).ToList();

            float width = layout.Rows.Max(r => r.Sum(d => d.Gap + d.Width));
            var rows = new List<KeyDef[]>();
            var targets = new List<int>();
            if (rear.Count > 0) rows.Add(rear.Select(_ => new KeyDef("", width / rear.Count, 0, 0.5f)).ToArray());
            targets.AddRange(rear);

            var table = layout.Rows.Select(r => front.Count > 0 && r.All(d => d.Name == "Lightbar") ? Enumerable.Repeat(r[0] with { Width = r.Sum(d => d.Width) / front.Count }, front.Count).ToArray() : r).ToList();
            float firstWidth = table[0].Sum(d => d.Gap + d.Width);
            table[0] = table[0].Select(d => d with { Top = d.Top + 0.15f })
                .Concat(logo.Select((_, k) => new KeyDef("Logo", 1, k == 0 ? width - firstWidth - logo.Count : 0, 0.5f, 0.15f))).ToArray();
            var logos = new Queue<int>(logo);

            int row = 0, bar = 0, barCount = table.Sum(r => r.Count(d => d.Name == "Lightbar"));
            foreach (var defs in table)
            {
                var queue = defs.All(d => d.Name == "Lightbar") ? null : blanks.ElementAtOrDefault(row++);
                foreach (var def in defs)
                {
                    if (def.Name == "Lightbar") targets.Add(front.Count > 0 ? front[bar++ * front.Count / barCount] : int.MaxValue);
                    else if (def.Name == "Logo") targets.Add(logos.Dequeue());
                    else if (byName.TryGetValue(def.Name, out int i)) targets.Add(i);
                    else targets.Add(queue?.Count > 0 ? queue.Dequeue() : int.MaxValue);
                }
                rows.Add(defs);
            }
            return (rows, targets.ToArray());
        }

        public static (List<KeyDef[]> Rows, int[] Targets) FromLamps((int X, int Y, int Usage, int Purpose)[] lamps)
        {
            var groups = new List<List<int>>();
            foreach (int i in Enumerable.Range(0, lamps.Length).OrderBy(i => lamps[i].Y))
            {
                if (groups.Count == 0 || lamps[i].Y - lamps[groups[^1][0]].Y > LAMP_ROW_SPREAD_UM) groups.Add(new List<int>());
                groups[^1].Add(i);
            }
            groups = groups.Select(g => g.OrderBy(i => lamps[i].X).ToList()).ToList();

            var deltas = groups.SelectMany(g => g.Zip(g.Skip(1), (a, b) => lamps[b].X - lamps[a].X)).Where(d => d > 0).Order().ToList();
            double pitch = deltas.Count > 0 ? deltas[deltas.Count / 2] : LAMP_PITCH_UM;
            int minX = lamps.Min(l => l.X);

            var rows = new List<KeyDef[]>();
            var targets = new List<int>();
            foreach (var group in groups)
            {
                var row = new KeyDef[group.Count];
                double right = 0;
                for (int k = 0; k < group.Count; k++)
                {
                    var lamp = lamps[group[k]];
                    double width = group.Count < 2 ? 1 : Math.Clamp(Math.Abs(lamps[group[k + 1 < group.Count ? k + 1 : k - 1]].X - lamp.X) / pitch, 0.5, 1);
                    if (WideKeys.Contains(lamp.Usage) && group.Count > 1)
                    {
                        int prev = k > 0 ? lamps[group[k - 1]].X : 2 * lamp.X - lamps[group[k + 1]].X;
                        int next = k + 1 < group.Count ? lamps[group[k + 1]].X : 2 * lamp.X - prev;
                        width = Math.Max(1, (next - prev) / pitch - 1);
                    }
                    double left = (lamp.X - minX) / pitch + 0.5 - width / 2;
                    double gap = left - right < 0.1 ? 0 : left - right;
                    row[k] = new KeyDef(UsageName(lamp.Usage), (float)width, (float)gap);
                    right += gap + width;
                    targets.Add(group[k]);
                }
                rows.Add(row);
            }
            return (rows, targets.ToArray());
        }

        private static readonly Dictionary<int, string> UsageNames = new()
        {
            [0x28] = "Enter", [0x29] = "Esc", [0x2A] = "Bksp", [0x2B] = "Tab", [0x2C] = "Space", [0x2D] = "-", [0x2E] = "=", [0x2F] = "[",
            [0x30] = "]", [0x31] = "\\", [0x32] = "#", [0x33] = ";", [0x34] = "'", [0x35] = "`", [0x36] = ",", [0x37] = ".", [0x38] = "/",
            [0x39] = "Caps", [0x46] = "PrtSc", [0x47] = "ScrLk", [0x48] = "Pause", [0x49] = "Ins", [0x4A] = "Home", [0x4B] = "Pg Up", [0x4C] = "Del",
            [0x4D] = "End", [0x4E] = "Pg Dn", [0x4F] = "→", [0x50] = "←", [0x51] = "↓", [0x52] = "↑", [0x53] = "Num", [0x54] = "Num /",
            [0x55] = "Num *", [0x56] = "Num -", [0x57] = "Num +", [0x58] = "Num Enter", [0x62] = "Num 0", [0x63] = "Num .", [0x64] = "<", [0x65] = "Menu",
            [0xE0] = "Ctrl", [0xE1] = "Shift", [0xE2] = "Alt", [0xE3] = "Win", [0xE4] = "Right Ctrl", [0xE5] = "Right Shift", [0xE6] = "Right Alt", [0xE7] = "Right Win",
        };

        private static string UsageName(int usage) => usage switch
        {
            >= 0x04 and <= 0x1D => ((char)('A' + usage - 0x04)).ToString(),
            >= 0x1E and <= 0x26 => ((char)('1' + usage - 0x1E)).ToString(),
            0x27 => "0",
            >= 0x3A and <= 0x45 => "F" + (usage - 0x39),
            >= 0x59 and <= 0x61 => "Num " + (usage - 0x58),
            _ => UsageNames.GetValueOrDefault(usage, ""),
        };
    }
}
