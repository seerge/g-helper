using GHelper.Peripherals;
using GHelper.Peripherals.Keyboard;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace GHelper.USB
{
    // Follows the Windows accent colour: laptop Aura (and mice synced to it) take a vivid
    // version of the accent, external keyboards get theme effects coloured from the accent
    // and the wallpaper's own colours.
    // Enabled with "aura_accent_sync": 1 in config.json.
    public static class AccentSync
    {
        public static bool Enabled => AppConfig.Is("aura_accent_sync");

        private static int lastAccent;
        private static bool watching;

        [DllImport("advapi32.dll")]
        private static extern int RegNotifyChangeKeyValue(SafeRegistryHandle hKey, bool watchSubtree, int notifyFilter, IntPtr hEvent, bool asynchronous);
        private const int REG_NOTIFY_CHANGE_LAST_SET = 0x4;

        // The accent the taskbar / Start use (incl. automatic accent from the wallpaper);
        // DWM\AccentColor can stay stale when the accent is picked automatically
        private const string AccentKey = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Accent";

        // Automatic accent doesn't always broadcast a settings change,
        // so wait on the accent key itself and re-apply whenever a value in it is written
        public static void Watch()
        {
            if (!Enabled || watching) return;
            watching = true;

            new Thread(() =>
            {
                using var key = Registry.CurrentUser.OpenSubKey(AccentKey);
                if (key is null) return;
                while (RegNotifyChangeKeyValue(key.Handle, false, REG_NOTIFY_CHANGE_LAST_SET, IntPtr.Zero, false) == 0)
                {
                    Thread.Sleep(500); // the palette is written as several values in a row
                    Logger.WriteLine("AccentSync: accent palette changed");
                    try { Apply(); } catch (Exception e) { Logger.WriteLine("AccentSync: " + e.Message); }
                }
            })
            { IsBackground = true, Name = "AccentSync" }.Start();
        }

        // Called on startup and on every theme / colour preference change; true when it applied the lighting
        public static bool Apply()
        {
            if (!Enabled) return false;

            Color? accent = ReadAccent();
            if (accent is null) return false;
            if (accent.Value.ToArgb() == lastAccent) return false;
            lastAccent = accent.Value.ToArgb();

            Palette p = ReadPalette();
            Logger.WriteLine($"AccentSync: accent {ToHex(accent.Value)} -> {ToHex(p.Main)}, wallpaper {string.Join(" ", p.Wallpaper.Select(ToHex))}");

            AppConfig.Set("aura_color", p.Main.ToArgb());
            AppConfig.Set("aura_color2", p.Secondary.ToArgb());
            Aura.ApplyAura();

            ApplyKeyboards();
            return true;
        }

        private static void ApplyKeyboards()
        {
            foreach (var kb in PeripheralsProvider.ConnectedKeyboards.ToList())
                Task.Run(() => { try { ApplyKeyboard(kb); } catch { } });
        }

        // Keyboard effect while accent sync is on: two per-key layouts drawn here,
        // or a hardware effect (stored as its KeyboardLightingMode value) fed with theme colours
        public const int StyleGradient = -1;
        public const int StyleFunctional = -2;

        public static int Style => AppConfig.Get("kb_accent_style", StyleGradient);

        public static List<int> Styles(AsusKeyboard kb)
        {
            var styles = new List<int> { StyleGradient, StyleFunctional };
            foreach (var mode in kb.SupportedLightingModes())
                if (mode != KeyboardLightingMode.Direct && kb.SupportsColorSetting(mode)) styles.Add((int)mode);
            return styles;
        }

        public static string StyleName(int style) => style switch
        {
            StyleGradient => Properties.Strings.KeyboardThemeGradient,
            StyleFunctional => Properties.Strings.KeyboardFunctional,
            _ => AsusKeyboardSettings.LightingModeName((KeyboardLightingMode)style),
        };

        public static void SetStyle(int style)
        {
            AppConfig.Set("kb_accent_style", style);
            Logger.WriteLine("AccentSync: keyboard style " + StyleName(style));
            ApplyKeyboards();
        }

        // The keyboard the main-window controls show and cycle
        public static AsusKeyboard? MainKeyboard => PeripheralsProvider.ConnectedKeyboards.FirstOrDefault(k => k.HasRGB() && k.HasPerKeyRGB());

        // step +1 = next effect, -1 = previous
        public static void NextStyle(int step = 1)
        {
            var kb = MainKeyboard;
            if (kb is null) return;
            var styles = Styles(kb);
            SetStyle(styles[(styles.IndexOf(Style) + step + styles.Count) % styles.Count]);
        }

        // Speed / brightness live in the active keyboard profile; keep its mode and colours
        public static void SetTuning(AuraSpeed? speed = null, int? brightness = null)
        {
            foreach (var kb in PeripheralsProvider.ConnectedKeyboards.ToList())
                kb.StoreLighting(kb.StoredMode, kb.StoredColor, kb.StoredColor2, kb.StoredColor3,
                    speed ?? kb.StoredSpeed, brightness ?? kb.StoredBrightness);
            ApplyKeyboards();
        }

        // Returns false when accent sync doesn't drive this keyboard, so the caller restores its own lighting
        public static bool ApplyKeyboard(AsusKeyboard kb)
        {
            if (!Enabled || PeripheralsProvider.IsKeyboardAuraSync) return false;
            if (!kb.HasRGB() || !kb.HasPerKeyRGB() || !kb.IsDeviceReady) return false;
            if (ReadAccent() is null) return false;

            Palette p = ReadPalette();
            Color[]? keys = KeyColors(kb, p);
            if (keys is not null) return kb.SetLedColors(keys);

            return kb.ApplyLighting((KeyboardLightingMode)Style, p.Main, p.Secondary, kb.StoredSpeed, kb.StoredBrightness);
        }

        // Per-key colours of the current style for the keyboard (also used by the settings preview); null for hardware effects
        public static Color[]? KeyColors(AsusKeyboard kb) => ReadAccent() is null ? null : KeyColors(kb, ReadPalette());

        private static Color[]? KeyColors(AsusKeyboard kb, Palette p)
        {
            int style = Styles(kb).Contains(Style) ? Style : StyleGradient;
            if (style == StyleGradient) return PerKey(kb, p, (row, rows, _) => GradientRow(p, row, rows));
            if (style == StyleFunctional) return PerKey(kb, p, (_, _, key) => FunctionalKey(p, key.Name));
            return null;
        }

        private static Color[] PerKey(AsusKeyboard kb, Palette p, Func<int, int, KeyDef, Color> color)
        {
            var rows = kb.KeyLayout();
            var colors = new List<Color>(kb.LedCount());

            for (int r = 0; r < rows.Count; r++)
                foreach (var key in rows[r]) colors.Add(color(r, rows.Count, key));

            while (colors.Count < kb.LedCount()) colors.Add(p.Main); // media keys
            return colors.ToArray();
        }

        // Rows run from a pale tint of the main colour (top) to the wallpaper's second colour (bottom)
        private static Color GradientRow(Palette p, int row, int rows)
        {
            double t = rows > 1 ? (double)row / (rows - 1) : 0.5;
            return Mix(p.Tone(0.35, 0.3), p.Second ?? p.Main, t);
        }

        private static readonly HashSet<string> HighlightKeys = new() { "Esc", "Logo", "Q", "W", "E", "R", "A", "S", "D", "F" };
        private static readonly HashSet<string> ModifierKeys = new() { "Tab", "Caps", "Shift", "Right Shift", "Ctrl", "Right Ctrl", "Win", "Alt", "Right Alt", "Fn", "Menu" };
        private static readonly HashSet<string> NavKeys = new() { "PrtSc", "ScrLk", "Pause", "Ins", "Home", "Pg Up", "Del", "End", "Pg Dn", "←", "↑", "→", "↓" };

        // Keys grouped by function: F row in the main colour, gaming cluster in the wallpaper colour
        // furthest from it, modifiers / navigation in the other wallpaper colours, typing keys a pale tint.
        // Groups the wallpaper has no colour for fall back to tints (or grey levels) of the main colour.
        private static Color FunctionalKey(Palette p, string name)
        {
            if (HighlightKeys.Contains(name)) return p.Contrast ?? p.Tone(0.35, 1);
            if (name.Length > 1 && name[0] == 'F' && char.IsDigit(name[1])) return p.Tone(-1, 0.8);
            if (ModifierKeys.Contains(name)) return p.Second ?? p.Tone(0.6, 0.55);
            if (NavKeys.Contains(name)) return p.Third ?? p.Tone(0.45, 0.7);
            if (name.StartsWith("Num")) return p.Tone(0.5, 0.4);
            return p.Tone(0.12, 0.2);
        }

        // Main: the accent, or the wallpaper's leading colour when the accent is washed out (white / grey themes).
        // Contrast: the wallpaper colour furthest round the hue wheel; Second / Third: the next most common ones.
        private record Palette(Color Main, Color? Contrast, Color? Second, Color? Third, List<Color> Wallpaper)
        {
            public bool IsGrey => Main.GetSaturation() == 0;

            public Color Secondary => Second ?? Contrast ?? Tone(0.5, 0.5);

            // the main hue at saturation s, or a grey level when there is no hue at all; s < 0 keeps the main colour
            public Color Tone(double s, double greyLevel)
            {
                if (IsGrey) return Level(Color.White, greyLevel);
                return s < 0 ? Main : FromHsv(Main.GetHue(), s, 1);
            }
        }

        private static Palette ReadPalette()
        {
            Color? accent = ReadAccent();
            List<Color> wall = WallpaperColors();

            Color main = accent is Color a && Saturation(a) >= 0.2 ? Vivid(a)
                : wall.Count > 0 ? wall[0]
                : Color.White;

            var others = wall.Where(c => HueDistance(c, main) >= 30).ToList();
            Color? contrast = others.Count > 0 ? others.MaxBy(c => HueDistance(c, main)) : null;
            var rest = others.Where(c => c != contrast).ToList();

            return new Palette(main, contrast, rest.Count > 0 ? rest[0] : null, rest.Count > 1 ? rest[1] : null, wall);
        }

        private static string? wallpaperKey;
        private static List<Color> wallpaperColors = new();

        // Vivid versions of the wallpaper's main colours, most common first, at least 30° apart; cached per file version
        private static List<Color> WallpaperColors()
        {
            try
            {
                using var desktop = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop");
                string? path = desktop?.GetValue("WallPaper") as string;
                if (string.IsNullOrEmpty(path) || !File.Exists(path)) return new();

                string key = path + File.GetLastWriteTimeUtc(path).Ticks;
                if (key == wallpaperKey) return wallpaperColors;

                const int bins = 24; // 15° hue buckets
                var count = new int[bins];
                var sum = new (long r, long g, long b)[bins];
                int total = 0;

                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                using (var image = Image.FromStream(fs))
                using (var small = new Bitmap(image, 64, 36))
                {
                    for (int x = 0; x < small.Width; x++)
                        for (int y = 0; y < small.Height; y++)
                        {
                            Color c = small.GetPixel(x, y);
                            total++;
                            if (Saturation(c) < 0.25 || Math.Max(c.R, Math.Max(c.G, c.B)) < 50) continue;
                            int bin = (int)(c.GetHue() / 15) % bins;
                            count[bin]++;
                            sum[bin].r += c.R; sum[bin].g += c.G; sum[bin].b += c.B;
                        }
                }

                var colors = new List<Color>();
                foreach (int bin in Enumerable.Range(0, bins).OrderByDescending(b => count[b]))
                {
                    if (count[bin] < total * 0.01) break; // ignore specks
                    Color avg = Color.FromArgb((int)(sum[bin].r / count[bin]), (int)(sum[bin].g / count[bin]), (int)(sum[bin].b / count[bin]));
                    Color vivid = Vivid(avg);
                    if (colors.All(c => HueDistance(c, vivid) >= 30)) colors.Add(vivid);
                    if (colors.Count == 4) break;
                }

                wallpaperKey = key;
                wallpaperColors = colors;
                return colors;
            }
            catch (Exception e)
            {
                Logger.WriteLine("AccentSync: wallpaper " + e.Message);
                return new();
            }
        }

        // AccentColorMenu is stored as 0xAABBGGRR
        private static Color? ReadAccent()
        {
            using var key = Registry.CurrentUser.OpenSubKey(AccentKey);
            if (key?.GetValue("AccentColorMenu") is not int abgr) return null;
            return Color.FromArgb(abgr & 0xFF, (abgr >> 8) & 0xFF, (abgr >> 16) & 0xFF);
        }

        // HSV saturation
        private static double Saturation(Color c)
        {
            int max = Math.Max(c.R, Math.Max(c.G, c.B));
            int min = Math.Min(c.R, Math.Min(c.G, c.B));
            return max == 0 ? 0 : (double)(max - min) / max;
        }

        // LEDs wash out muted colours: keep the hue, push saturation up and brightness to full
        private static Color Vivid(Color c) => FromHsv(c.GetHue(), Math.Max(Saturation(c), 0.8), 1);

        private static double HueDistance(Color a, Color b)
        {
            double d = Math.Abs(a.GetHue() - b.GetHue()) % 360;
            return d > 180 ? 360 - d : d;
        }

        private static Color Mix(Color a, Color b, double t) =>
            Color.FromArgb((int)(a.R + (b.R - a.R) * t), (int)(a.G + (b.G - a.G) * t), (int)(a.B + (b.B - a.B) * t));

        private static Color Level(Color c, double v) => Color.FromArgb((int)(c.R * v), (int)(c.G * v), (int)(c.B * v));

        private static Color FromHsv(double h, double s, double v)
        {
            double c = v * s;
            double x = c * (1 - Math.Abs(h / 60 % 2 - 1));
            double m = v - c;

            (double r, double g, double b) = (h / 60) switch
            {
                < 1 => (c, x, 0d),
                < 2 => (x, c, 0d),
                < 3 => (0d, c, x),
                < 4 => (0d, x, c),
                < 5 => (x, 0d, c),
                _ => (c, 0d, x),
            };

            return Color.FromArgb((int)Math.Round((r + m) * 255), (int)Math.Round((g + m) * 255), (int)Math.Round((b + m) * 255));
        }

        private static string ToHex(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";
    }
}
