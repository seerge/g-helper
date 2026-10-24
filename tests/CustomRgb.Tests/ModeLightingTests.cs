using System.Drawing;
using GHelper.USB;

internal static class ModeLightingTests
{
    public static void Run()
    {
        var options = ModeLightingSettings.Default(2) with
        {
            Keyboard = new ModeSurface { Colors = ["#FF0000", "#0000FF"], Seconds = 4 },
            Lightbar = new ModeSurface { Colors = ["#00FF00"], Seconds = 8 }
        };
        Check(options.Render(2, 0)[0].R == 255 && options.Render(2, 2)[0].B == 255, "cycle follows the chosen palette");
        Check(options.Render(2, 1).Take(4).Distinct().Count() == 1, "cycle changes keyboard zones together");
        Check(options.Render(2, 2)[4].G == 255, "lightbar palette is independent");
        var wave = options.Render(3, 0);
        Check(wave.Take(4).Distinct().Count() >= 3, "themed rainbow moves colors across four zones");
        var gradient = options with { Keyboard = options.Keyboard with { Motion = GradientMotion.Still } };
        Check(gradient.Render(24, 0)[0].R == 255 && gradient.Render(24, 0)[3].B == 255, "gradient retains two endpoints");
        Check(gradient.Render(24, 0).SequenceEqual(gradient.Render(24, 1)), "still gradient remains steady");
        foreach (var motion in new[] { GradientMotion.Flow, GradientMotion.BackAndForth })
        {
            var animated = gradient with { Keyboard = gradient.Keyboard with { Motion = motion } };
            Check(!animated.Render(24, 0).Take(4).SequenceEqual(animated.Render(24, 1).Take(4)), motion + " animates the gradient");
        }
        Check(!ModeLightingSettings.Supports(10), "Strobe stays on its original native color control and playback");
        var roundtrip = ModeLightingSettings.Parse(options.ToJson(2), 2);
        Check(roundtrip.Render(2, 0.7).SequenceEqual(options.Render(2, 0.7)), "per-mode settings survive JSON round trip");
        Check(options.Normalize(24).Keyboard.Colors.Length <= 2, "gradient palette is limited to two colors");
        var boosted = new AmbientOptions { Boost = 200, ShadowLift = 40, Saturation = 200 };
        var gray = boosted.Transform(Color.FromArgb(30, 30, 30));
        Check(gray.R > 30 && gray.R == gray.G && gray.G == gray.B, "ambient lifts dim gray without inventing hue");
        Check(boosted.Transform(Color.Black).ToArgb() == Color.Black.ToArgb(), "ambient black remains off");
        Check((boosted with { Smoothing = 95 }).Smooth(Color.White, Color.Black).ToArgb() == Color.Black.ToArgb(), "black scenes turn off even with strong smoothing");
        var smooth = new AmbientOptions { Smoothing = 95, KeepBlack = false };
        var fading = Color.FromArgb(9, 9, 9);
        for (int i = 0; i < 20; i++) fading = smooth.Smooth(fading, Color.Black);
        Check(fading.ToArgb() == Color.Black.ToArgb(), "ambient smoothing does not leave stuck dim colors");
        Check(new AmbientOptions().Transform(Color.FromArgb(80, 100, 120)).ToArgb() == Color.FromArgb(80, 100, 120).ToArgb(), "neutral ambient controls preserve sampled colors");
        Check(new CaptureArea { X = .9, Y = .8, Width = .5, Height = .5 }.Normalize().ToPixels(new Rectangle(-1920, 0, 1920, 1080)).Right <= 0, "capture rectangles stay inside monitors including negative coordinates");
        Console.WriteLine("All per-mode model checks passed; no hardware accessed.");
    }
    private static void Check(bool ok, string name) { if (!ok) throw new Exception("FAIL: " + name); Console.WriteLine("PASS: " + name); }
}
