using System;
using System.Management;
using System.Threading;

namespace PawnIO
{
    // Reads CPU name and caption from WMI once and caches the result.
    // Thread-safe: safe to call from multiple threads simultaneously.
    // Does not require administrator rights.
    public static class CpuInfo
    {
        private static string? _name;
        private static string? _caption;
        private static readonly Lazy<(string Name, string Caption)> _data =
            new Lazy<(string, string)>(Load, LazyThreadSafetyMode.ExecutionAndPublication);

        public static string Name    => _data.Value.Name;
        public static string Caption => _data.Value.Caption;

        // CPU undervolting / temperature limits.
        // Defaults match the original RyzenControl values; can be overridden via AppConfig.
        public static int MinCPUUV   => AppConfig.Get("min_uv",      -40);
        public static int MaxCPUUV   => AppConfig.Get("max_uv",        0);
        public static int MinIGPUUV  => AppConfig.Get("min_igpu_uv", -30);
        public static int MaxIGPUUV  => AppConfig.Get("max_igpu_uv",   0);
        public static int MinTemp    => AppConfig.Get("min_temp",      75);
        public const  int MaxTemp    = 98;
        public const  int DefaultTemp= 96;

        public static bool IsAMD()
            => Name.Contains("AMD")    ||
               Name.Contains("Ryzen") ||
               Name.Contains("Athlon") ||
               Name.Contains("Radeon");

        public static bool IsSupportedUV()
            => Name.Contains("RYZEN AI MAX") ||
               Name.Contains("Ryzen AI 9")   ||
               Name.Contains("Ryzen 9")      ||
               Name.Contains("4900H")        ||
               Name.Contains("4800H")        ||
               Name.Contains("4600H");

        public static bool IsSupportedUViGPU()
            => Name.Contains("6900H") ||
               Name.Contains("6800H") ||
               Name.Contains("6600H");

        private static (string Name, string Caption) Load()
        {
            string name    = string.Empty;
            string caption = string.Empty;

            try
            {
                using var searcher = new ManagementObjectSearcher("select Name, Caption from Win32_Processor");
                foreach (ManagementObject obj in searcher.Get())
                {
                    name    = obj["Name"]?.ToString()    ?? string.Empty;
                    caption = obj["Caption"]?.ToString() ?? string.Empty;
                    break; // only need the first processor
                }
            }
            catch { }

            return (name, caption);
        }
    }
}
