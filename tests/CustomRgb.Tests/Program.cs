using System.Drawing;
using System.Reflection;
#if CUSTOM_IMPLEMENTED
using GHelper.USB;
#endif

Check(Assembly.GetExecutingAssembly().GetType("GHelper.USB.CustomRgbScene") != null, "custom section scene exists");
PresetTests.Run();
ModeLightingTests.Run();
#if CUSTOM_IMPLEMENTED
var scene = CustomRgbScene.CreateDefault();
Check(scene.Sections.Length == 8, "scene contains four keyboard and four lightbar sections");
var red = new RgbSection { Style = RgbSectionStyle.Static, Palette = ["#FF0000"] };
var blue = new RgbSection { Style = RgbSectionStyle.Static, Palette = ["#0000FF"] };
scene = scene.Replace([0], red).Replace([4], blue);
Color[] frame = scene.Render(0);
Check(frame[0].R == 255 && frame[4].B == 255, "keyboard and lightbar accept independent colors");
Check(frame[1].ToArgb() != frame[0].ToArgb(), "changing one section leaves neighbors unchanged");
var allRed = scene.Replace([1, 2, 3], red);
Check(allRed.Sections[1].Palette[0] == "#FF0000" && scene.Sections[1].Palette[0] != "#FF0000", "multi-selection edits create a separate scene snapshot");
allRed.Sections[1].Palette[0] = "#00FF00";
Check(allRed.Sections[2].Palette[0] == "#FF0000", "copied section palettes do not alias one another");
foreach (var style in Enum.GetValues<RgbSectionStyle>())
{
    var setting = new RgbSection { Style = style, Palette = ["#FF0000", "#0000FF"], PeriodSeconds = 4 };
    var testScene = scene.Replace([0], setting);
    var first = testScene.Render(0, 0.5)[0];
    Check(first.ToArgb() == testScene.Render(4, 0.5)[0].ToArgb(), style + " loops without cycle drift");
}
var cycle = new RgbSection { Style = RgbSectionStyle.ColorCycle, Palette = ["#FF0000", "#0000FF"], PeriodSeconds = 4 };
scene = scene.Replace([0], cycle).Replace([1], cycle with { PeriodSeconds = 8 });
Check(scene.Render(2)[0].B == 255 && scene.Render(2)[1].R > 0, "sections have independent animation periods");
Check(scene.Replace([0], cycle with { PhasePercent = 50 }).Render(0)[0].ToArgb() == scene.Replace([0], cycle).Render(2)[0].ToArgb(), "phase offset shifts only the selected section");
var threeColors = cycle with { Palette = ["#FF0000", "#00FF00", "#0000FF"] };
Check(scene.Replace([0], threeColors with { Reverse = true }).Render(1)[0].ToArgb() == scene.Replace([0], threeColors).Render(-1)[0].ToArgb()
    && scene.Replace([0], threeColors).Render(1)[0].ToArgb() != scene.Replace([0], threeColors).Render(-1)[0].ToArgb(), "reverse reverses palette progression");
Check(scene.Replace([0], red with { Brightness = 0 }).Render(0)[0].ToArgb() == Color.Black.ToArgb(), "zero brightness turns section off");
Check(scene.Replace([0], red with { Brightness = 50 }).Render(0)[0].R == 128, "brightness scales the selected color");
var strobe = red with { Style = RgbSectionStyle.Strobe, PeriodSeconds = 2, DutyPercent = 25 };
Check(scene.Replace([0], strobe).Render(0)[0].R == 255 && scene.Replace([0], strobe).Render(1)[0].R == 0, "strobe honors duty cycle");
var steps = cycle with { Style = RgbSectionStyle.ColorSteps };
Check(scene.Replace([0], steps).Render(1)[0].R == 255 && scene.Replace([0], steps).Render(2)[0].B == 255, "stepped palette preserves the chosen color order");
var battery = cycle with { Style = RgbSectionStyle.Battery };
Check(scene.Replace([0], battery).Render(0, 0)[0].R == 255 && scene.Replace([0], battery).Render(0, 1)[0].B == 255, "battery style maps charge through the user's palette");
var roundtrip = CustomRgbScene.Parse(scene.ToJson());
Check(roundtrip.Render(1).Select(c => c.ToArgb()).SequenceEqual(scene.Render(1).Select(c => c.ToArgb())), "scene JSON round trip preserves rendered output");
Check(CustomRgbScene.Parse(scene.ToJson()).Sections[0].PeriodSeconds == 4, "JSON preserves section timing");
Throws(() => CustomRgbScene.Parse("{\"Version\":999}"), "unsupported scene version is rejected");
Throws(() => CustomRgbScene.Parse("{}"), "unrelated JSON is not accepted as a lighting profile");
Throws(() => CustomRgbScene.Parse("{\"Version\":1,\"Sections\":[]}"), "wrong section count is rejected");
Throws(() => scene.Replace([0], red with { Palette = ["broken"] }), "malformed colors are rejected");
Throws(() => scene.Replace([0], red with { Palette = ["#FFFF  "] }), "whitespace cannot masquerade as a hex color");
Throws(() => CustomRgbScene.Parse(new string(' ', 65537)), "oversized imports are rejected");
var bounded = scene.Replace([0], cycle with { PeriodSeconds = 0, Brightness = 200, PhasePercent = -5 });
Check(bounded.Sections[0].PeriodSeconds == 0.2 && bounded.Sections[0].Brightness == 100 && bounded.Sections[0].PhasePercent == 0, "imported numerical values are bounded");
Color[] distinct = [Color.Red, Color.Green, Color.Blue, Color.White, Color.Cyan, Color.Magenta, Color.Yellow, Color.Black];
Check(CustomRgbScene.SampleSection(distinct, 0, 0.31).ToArgb() == Color.Green.ToArgb(), "discrete LampArray mapping does not blend neighboring sections");
Check(CustomRgbScene.SampleSection(distinct, 4, 0.67).ToArgb() == Color.Yellow.ToArgb(), "lightbar sections use their own color group");
Console.WriteLine("All custom scene checks passed. No hardware was accessed.");
#endif

static void Check(bool ok, string name) { if (!ok) throw new Exception("FAIL: " + name); Console.WriteLine("PASS: " + name); }
static void Throws(Action action, string name) { try { action(); } catch { Check(true, name); return; } Check(false, name); }
