using System.Reflection;
#if PRESETS_IMPLEMENTED
using GHelper.USB;
#endif

internal static class PresetTests
{
    public static void Run()
    {
        Check(Assembly.GetExecutingAssembly().GetType("GHelper.USB.RgbPresets") != null, "ready-made preset catalog exists");
#if PRESETS_IMPLEMENTED
        var original = CustomRgbScene.CreateDefault();
        Check(RgbPresets.All.Count == 22, "22 ready-made look families are available");
        Check(RgbPresets.All.Sum(p => p.Variants.Length) == 64, "64 ready-made variants are available");
        foreach (var look in RgbPresets.All)
        {
            Check(look.Variants.Length == (look.Id == "off" ? 1 : 3), look.Name + " has its named variants");
            var signatures = new HashSet<string>();
            foreach (var variant in look.Variants)
            {
                var scene = RgbPresets.Create(look.Id, variant.Id, original, RgbPresetTarget.Both, 1, 75);
                Check(scene.Sections.Length == 8 && scene.Sections.All(s => s.Brightness == 75), look.Name + "/" + variant.Name + " configures both surfaces");
                signatures.Add(string.Join(";", new[] { 0d, 0.7, 1.6 }.SelectMany(t => scene.Render(t)).Select(c => c.ToArgb())));
            }
            Check(signatures.Count == look.Variants.Length, look.Name + " variants visibly differ over time");
        }
        var wave = RgbPresets.Create("rainbow", "classic", original, RgbPresetTarget.Both, 1, 100);
        Check(wave.Render(0).Take(4).Select(c => c.ToArgb()).Distinct().Count() == 4, "rainbow wave spreads colors across the keyboard");
        var cycle = RgbPresets.Create("spectrum", "classic", original, RgbPresetTarget.Both, 1, 100);
        Check(cycle.Render(0).Take(4).Select(c => c.ToArgb()).Distinct().Count() == 1, "spectrum cycle changes all zones together");
        var keyboard = RgbPresets.Create("ocean", "deep", original, RgbPresetTarget.Keyboard, 1, 50);
        Check(keyboard.Sections.Skip(4).Select(s => s.Brightness).SequenceEqual(original.Sections.Skip(4).Select(s => s.Brightness)), "keyboard-only preset preserves the lightbar");
        var bar = RgbPresets.Create("sunset", "golden", original, RgbPresetTarget.Lightbar, 1, 50);
        Check(bar.Sections.Take(4).Select(s => s.Brightness).SequenceEqual(original.Sections.Take(4).Select(s => s.Brightness)), "lightbar-only preset preserves the keyboard");
        var fast = RgbPresets.Create("ocean", "deep", original, RgbPresetTarget.Both, 0.5, 100);
        var normal = RgbPresets.Create("ocean", "deep", original, RgbPresetTarget.Both, 1, 100);
        Check(fast.Sections[0].PeriodSeconds == normal.Sections[0].PeriodSeconds / 2, "simple speed selection scales the animation period");
        Check(original.Sections.All(s => s.Brightness == 100), "choosing a preset does not mutate the original scene");
        var recolored = RgbPresets.Create("christmas", "split", original, RgbPresetTarget.Keyboard, 1, 100, ["#112233", "#445566"]);
        Check(recolored.Sections[0].Palette[0] == "#112233" && recolored.Sections[3].Palette[0] == "#445566", "custom colors preserve the split's placement");
        var reversed = RgbPresets.Create("rainbow", "reverse", original, RgbPresetTarget.Both, 1, 100, reverse: true);
        Check(!reversed.Sections[0].Reverse, "direction adjustment reverses the preset's own direction");
        var off = RgbPresets.Create("off", "off", original, RgbPresetTarget.Keyboard, 1, 100);
        Check(off.Render(1).Take(4).All(c => c.ToArgb() == System.Drawing.Color.Black.ToArgb())
            && off.Render(1).Skip(4).Select(c => c.ToArgb()).SequenceEqual(original.Render(1).Skip(4).Select(c => c.ToArgb())), "Lights Off affects only the chosen surface");
#endif
    }
    private static void Check(bool ok, string name) { if (!ok) throw new Exception("FAIL: " + name); Console.WriteLine("PASS: " + name); }
}
