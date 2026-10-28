using System.Diagnostics;
using System.Reflection;

// AppConfig caches the model once, just as the real app does. A fresh process
// per model preserves that behavior rather than altering its private fields.
if (args.Length == 0)
{
    var models = new (string Name, bool NoAura, bool White, bool Oled)[]
    {
        ("ProArt P14 H7407BA", true, true, true),
        ("ASUS ProArt P14 H7407BA_H7407BA", true, true, true),
        ("proart p14 h7407ba", true, true, true),
        ("ProArt P16 H7607BI_H7607BI", true, true, true),
        ("ProArt P16 H7606WI_H7606WI", true, true, true),
        ("ROG Zephyrus G14 GA401IV_GA401IV", true, true, false),
        ("ROG Zephyrus G14 GA401IHR_GA401IHR", false, true, false),
        ("ROG Zephyrus GA502IU_GA502IU", true, true, false),
        ("ProArt PX13 HN7306WV_HN7306WV", true, true, true),
        ("Vivobook M6500XV_M6500XV", true, true, true),
        ("ROG Zephyrus G14 GA403UI_GA403UI", false, false, true),
        ("ProArt P14 H7407XX", false, false, false),
        ("ProArt Unknown H7408XX", false, false, false),
        ("", false, false, false),
    };
    int failures = 0;
    foreach (var model in models)
    {
        var start = new ProcessStartInfo("dotnet") { UseShellExecute = false };
        start.ArgumentList.Add(Assembly.GetExecutingAssembly().Location);
        start.ArgumentList.Add(model.Name);
        start.ArgumentList.Add(model.NoAura.ToString());
        start.ArgumentList.Add(model.White.ToString());
        start.ArgumentList.Add(model.Oled.ToString());
        using var child = Process.Start(start)!;
        child.WaitForExit();
        if (child.ExitCode != 0) failures++;
    }
    Console.WriteLine($"{models.Length - failures}/{models.Length} model cases passed.");
    return failures == 0 ? 0 : 1;
}

TestPlatform.Model = args[0];
TestPlatform.ConfigDirectory = Path.Combine(Path.GetTempPath(), "ghelper-model-tests-" + Guid.NewGuid());
Directory.CreateDirectory(TestPlatform.ConfigDirectory);
File.WriteAllText(Path.Combine(TestPlatform.ConfigDirectory, "config.json"), "{}");
try
{
    Check("NoAura", bool.Parse(args[1]), AppConfig.NoAura());
    Check("IsWhite", bool.Parse(args[2]), AppConfig.IsWhite());
    Check("IsOLED", bool.Parse(args[3]), AppConfig.IsOLED());
    if (args[0].Contains("H7407BA", StringComparison.OrdinalIgnoreCase))
    {
        Check("IsProArt", true, AppConfig.IsProArt());
        Check("IsVivoZenPro", true, AppConfig.IsVivoZenPro());
        Check("IsHardwareFnLock", true, AppConfig.IsHardwareFnLock());
        Check("MediaKeys", false, AppConfig.MediaKeys());
        Check("IsChargeLimit6080", false, AppConfig.IsChargeLimit6080());
        Check("IsIntelHX", false, AppConfig.IsIntelHX());
        Check("IsCPULight", false, AppConfig.IsCPULight());
    }
    Console.WriteLine($"PASS {args[0]}");
    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex.Message);
    return 1;
}
finally
{
    Directory.Delete(TestPlatform.ConfigDirectory, recursive: true);
}

void Check(string rule, bool expected, bool actual)
{
    if (expected != actual)
        throw new Exception($"FAIL {args[0]} {rule}: expected {expected}, got {actual}");
}
