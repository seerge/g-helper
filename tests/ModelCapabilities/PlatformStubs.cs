// Only OS/configuration boundaries are substituted. All model rules come from
// the linked app/AppConfig.cs. No WMI, registry or hardware access is performed.
internal static class TestPlatform
{
    public static string Model = "";
    public static string ConfigDirectory = "";
}

internal static class Application
{
    public static string StartupPath => TestPlatform.ConfigDirectory;
}

internal static class Logger
{
    public static void WriteLine(string message) { }
}

// A global Registry shadows the imported Windows-only type in AppConfig.
internal static class Registry
{
    public static TestRegistryKey LocalMachine { get; } = new();
}

internal sealed class TestRegistryKey
{
    public TestRegistryKey OpenSubKey(string name) => null;
    public object GetValue(string name, object fallback) => fallback;
}

public enum AsusFan { CPU, GPU, Mid, XGM }

internal static class AsusACPI
{
    public const int PerformanceTurbo = 1;
    public const int PerformanceSilent = 2;
}

namespace GHelper.Helpers
{
    internal static class ProcessHelper
    {
        public static bool IsRunningAsSystem() => false;
    }
}

namespace GHelper.Mode
{
    internal static class Modes
    {
        public static int GetCurrent() => 0;
        public static int GetCurrentBase() => 0;
    }
}

namespace System.Management
{
    internal sealed class ManagementObjectSearcher : IDisposable
    {
        public ManagementObjectSearcher(string query) { }
        public IEnumerable<ManagementObject> Get() => [new()];
        public void Dispose() { }
    }

    internal sealed class ManagementObject : IDisposable
    {
        public object this[string key] => key == "Model" ? TestPlatform.Model : "";
        public void Dispose() { }
    }
}
