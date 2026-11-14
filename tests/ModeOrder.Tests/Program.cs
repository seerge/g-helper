using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

// Runs against the built application, with all configuration writes redirected
// to this test executable's directory. Never starts G-Helper's hardware controls.
internal static class Program
{
    private static readonly string FixturePath = Path.Combine(AppContext.BaseDirectory, "config.json");
    private static readonly Type Modes = typeof(GHelper.Fans).Assembly.GetType("GHelper.Mode.Modes")!;
    private static int passed;

    [STAThread]
    private static int Main(string[] args)
    {
        try
        {
            Logger.appPath = AppContext.BaseDirectory;
            Logger.logFile = Path.Combine(AppContext.BaseDirectory, "test.log");
            if (args.Length > 0 && args[0] == "--verify-order")
            {
                Assert(string.Join(",", Call<List<int>>("GetList")) == args[1], "fresh-process order");
                Assert(AppConfig.Get("performance_mode") == 5, "fresh-process selected ID");
                return 0;
            }

            if (args.Length == 2 && args[0] == "--check-config"
                && string.Equals(Path.GetFullPath(args[1]), FixturePath, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("The source configuration must differ from the test fixture path.");

            File.WriteAllText(FixturePath, "{}");
            _ = AppConfig.Get("performance_mode");
            var flags = BindingFlags.Static | BindingFlags.NonPublic;
            typeof(AppConfig).GetField("fallbackConfigFile", flags)!.SetValue(null,
                Path.Combine(AppContext.BaseDirectory, "fallback", "config.json"));
            var timer = (System.Timers.Timer)typeof(AppConfig).GetField("timer", flags)!.GetValue(null)!;
            timer.Interval = TimeSpan.FromHours(1).TotalMilliseconds;

            if (args.Length == 2 && args[0] == "--check-config")
            {
                File.Copy(args[1], FixturePath, overwrite: true);
                Assert((bool)typeof(AppConfig).GetMethod("TryLoadConfig", flags)!.Invoke(null, new object[] { FixturePath })!, "existing configuration load");
                string before = Snapshot();
                var order = Call<List<int>>("GetList");
                Assert(order.Take(3).SequenceEqual(new[] { 2, 0, 1 }), "built-in ordering");
                Assert(Call<Dictionary<int, string>>("GetDictonary").Keys.SequenceEqual(order), "existing configuration menu order");
                Assert(before == Snapshot(), "reading existing configuration changed settings");
                string storedOrder = AppConfig.GetString("custom_mode_order");
                typeof(AppConfig).GetMethod("Timer_Elapsed", flags)!.Invoke(null, new object?[] { null, null });
                Assert((bool)typeof(AppConfig).GetMethod("TryLoadConfig", flags)!.Invoke(null, new object[] { FixturePath })!, "round-trip load");
                Assert(before == Snapshot() && storedOrder == AppConfig.GetString("custom_mode_order"), "writing lost existing configuration");
                Console.WriteLine("PASS existing configuration read/write compatibility, order " + string.Join(",", order));
                return 0;
            }

            Test("legacy order and all selection surfaces", () =>
            {
                Reset();
                Order(2, 0, 1, 3, 5, 9);
                Assert(!AppConfig.Exists("custom_mode_order"), "reads must not migrate configuration");
                Assert(Call<Dictionary<int, string>>("GetDictonary").Keys.SequenceEqual(Call<List<int>>("GetList")), "dictionary order");
            });
            Test("move preserves every other configuration value and selected IDs", () =>
            {
                Reset();
                string before = Snapshot();
                Assert(Call<bool>("Move", 5, -1), "valid move");
                Order(2, 0, 1, 5, 3, 9);
                Assert(Snapshot() == before, "profile settings and saved selections changed");
            });
            Test("opposite move restores the original order", () =>
            {
                Reset();
                Call<bool>("Move", 5, -1);
                Call<bool>("Move", 5, 1);
                Order(2, 0, 1, 3, 5, 9);
            });
            Test("invalid moves do not write configuration", () =>
            {
                Reset();
                foreach (var (mode, direction) in new[] { (3, -1), (9, 1), (0, 1), (1, 1), (2, 1), (4, 1), (-1, 1), (20, -1), (5, 0), (5, 2), (5, int.MinValue) })
                {
                    Assert(!Call<bool>("CanMove", mode, direction), "invalid move enabled");
                    Assert(!Call<bool>("Move", mode, direction), "invalid move accepted");
                }
                Assert(!AppConfig.Exists("custom_mode_order"), "invalid move wrote order");
            });
            Test("duplicates, stale IDs, built-ins and malformed tokens", () =>
            {
                Reset("9,9,0,2,1,-1,20,4,garbage,999999999999999999999, 5 ,3.0,3");
                Order(2, 0, 1, 9, 5, 3);
            });
            Test("malformed stored value types fall back safely", () =>
            {
                foreach (object? value in new object?[] { null, true, 15, new[] { 9, 5, 3 }, new { order = 9 }, "" })
                {
                    Reset(value, includeOrder: true);
                    Order(2, 0, 1, 3, 5, 9);
                }
            });
            Test("partial saved order appends existing profiles", () =>
            {
                Reset("9");
                Order(2, 0, 1, 9, 3, 5);
            });
            Test("add reuses lowest free ID and appends it", () =>
            {
                Reset("9,5,3");
                int added = Call<int>("Add");
                Assert(added == 4, "free ID allocation changed");
                Assert(AppConfig.Get("mode_base_4") == AppConfig.Get("mode_base_5"), "base copy");
                Assert(AppConfig.Get("limit_total_4") == AppConfig.Get("limit_total_5"), "settings copy");
                Order(2, 0, 1, 9, 5, 3, 4);
            });
            Test("delete prunes order and reused ID goes to the end", () =>
            {
                Reset("9,3,5");
                Call<object?>("Remove", 3);
                Order(2, 0, 1, 9, 5);
                Assert(AppConfig.GetString("custom_mode_order") == "9,5", "stale ID remained");
                Assert(Call<int>("Add") == 3, "deleted ID not reused");
                Order(2, 0, 1, 9, 5, 3);
            });
            Test("delete and recreate final custom profile", () =>
            {
                Reset();
                foreach (int mode in new[] { 3, 5, 9 }) Call<object?>("Remove", mode);
                Order(2, 0, 1);
                Assert(!Call<bool>("Move", 3, 1), "nonexistent profile moved");
                // The Fans delete handler selects Balanced after deleting a profile.
                AppConfig.Set("performance_mode", 0);
                Assert(Call<int>("Add") == 3, "first ID");
                Order(2, 0, 1, 3);
                Assert(!Call<bool>("CanMove", 3, -1) && !Call<bool>("CanMove", 3, 1), "singleton boundaries");
            });
            Test("full capacity leaves configuration unchanged", () =>
            {
                Reset();
                for (int i = 3; i < 20; i++) AppConfig.Set("mode_base_" + i, 0);
                string before = Snapshot();
                Assert(Call<int>("Add") == -1, "capacity limit changed");
                Assert(Snapshot() == before, "failed add changed settings");
            });
            Test("forward and reverse cycling follow order and wrap", () =>
            {
                Reset("9,3,5");
                var order = Call<List<int>>("GetList");
                for (int i = 0; i < order.Count; i++)
                {
                    AppConfig.Set("performance_mode", order[i]);
                    Assert(Call<int>("GetNext", false) == order[(i + 1) % order.Count], "forward cycle");
                    Assert(Call<int>("GetNext", true) == order[(i + order.Count - 1) % order.Count], "reverse cycle");
                }
            });
            Test("one thousand moves preserve identity and built-in order", () =>
            {
                Reset();
                string before = Snapshot();
                var random = new Random(4750);
                for (int i = 0; i < 1000; i++)
                {
                    Call<bool>("Move", new[] { 3, 5, 9 }[random.Next(3)], random.Next(2) == 0 ? -1 : 1);
                    var order = Call<List<int>>("GetList");
                    Assert(order.Take(3).SequenceEqual(new[] { 2, 0, 1 }), "built-in order changed");
                    Assert(order.Skip(3).Order().SequenceEqual(new[] { 3, 5, 9 }), "custom IDs changed");
                    Assert(Snapshot() == before, "settings changed during repeated moves");
                }
            });
            Test("production config writer and fresh-process persistence", () =>
            {
                Reset();
                Call<bool>("Move", 5, -1);
                typeof(AppConfig).GetMethod("Timer_Elapsed", flags)!.Invoke(null, new object?[] { null, null });
                Assert(File.ReadAllText(FixturePath).Contains("custom_mode_order"), "order not saved");
                var start = new ProcessStartInfo(Environment.ProcessPath!) { UseShellExecute = false };
                if (string.Equals(Path.GetFileNameWithoutExtension(Environment.ProcessPath), "dotnet", StringComparison.OrdinalIgnoreCase))
                    start.ArgumentList.Add(Assembly.GetExecutingAssembly().Location);
                start.ArgumentList.Add("--verify-order");
                start.ArgumentList.Add("2,0,1,5,3,9");
                using var child = Process.Start(start)!;
                Assert(child.WaitForExit(15000) && child.ExitCode == 0, "fresh-process verification failed");
            });
            timer.Stop();
            Console.WriteLine($"PASS {passed} tests");
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception);
            return 1;
        }
    }

    private static void Reset(object? order = null, bool includeOrder = false)
    {
        var fixture = new Dictionary<string, object?>
        {
            ["performance_mode"] = 5, ["performance_0"] = 3, ["performance_1"] = 9,
            ["performance_usb"] = 5, ["unrelated_setting"] = "unchanged"
        };
        foreach (int mode in new[] { 3, 5, 9 })
        {
            fixture["mode_base_" + mode] = mode % 3;
            fixture["mode_name_" + mode] = "Profile " + mode;
            fixture["limit_total_" + mode] = mode * 5;
            fixture["fan_profile_cpu_" + mode] = "curve-" + mode;
            fixture["cpu_uv_" + mode] = -mode;
            fixture["mode_command_" + mode] = "command-" + mode;
            fixture["future_setting_" + mode] = new { value = mode };
        }
        if (order != null || includeOrder) fixture["custom_mode_order"] = order;
        File.WriteAllText(FixturePath, JsonSerializer.Serialize(fixture));
        bool loaded = (bool)typeof(AppConfig).GetMethod("TryLoadConfig", BindingFlags.Static | BindingFlags.NonPublic)!
            .Invoke(null, new object[] { FixturePath })!;
        Assert(loaded, "fixture loading");
    }

    private static string Snapshot()
    {
        var config = (Dictionary<string, object>)typeof(AppConfig)
            .GetField("config", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
        return JsonSerializer.Serialize(config.Where(pair => pair.Key != "custom_mode_order").OrderBy(pair => pair.Key));
    }

    private static T Call<T>(string method, params object[] args) =>
        (T)Modes.GetMethod(method, BindingFlags.Static | BindingFlags.Public)!.Invoke(null, args)!;

    private static void Order(params int[] expected) =>
        Assert(Call<List<int>>("GetList").SequenceEqual(expected), "unexpected profile order");

    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private static void Test(string name, Action test)
    {
        test();
        passed++;
        Console.WriteLine("PASS " + name);
    }
}
