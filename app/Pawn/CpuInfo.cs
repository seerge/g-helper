using System;
using System.Management;
using System.Runtime.Intrinsics.X86;
using System.Threading;

namespace PawnIO
{
    // Reads CPU name and caption from WMI once and caches the result.
    // Thread-safe: safe to call from multiple threads simultaneously.
    // Does not require administrator rights.
    public static class CpuInfo
    {
        // Vendor check via CPUID leaf 0 — single CPU instruction, no I/O, no WMI.
        // AMD always reports "AuthenticAMD" in ebx/edx/ecx.
        public static readonly bool IsAMD = DetectAMD();

        private static bool DetectAMD()
        {
            if (!X86Base.IsSupported) return false;
            var (_, ebx, ecx, edx) = X86Base.CpuId(0, 0);
            // "Auth" = ebx, "enti" = edx, "cAMD" = ecx
            return ebx == 0x68747541u && edx == 0x69746E65u && ecx == 0x444D4163u;
        }

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

        public static bool IsSupportedUV()
            => Name.Contains("RYZEN AI MAX") ||
               Name.Contains("Ryzen AI 9")   ||
               Name.Contains("Ryzen 9")      ||
               Name.Contains("4900H")        ||
               Name.Contains("4800H")        ||
               Name.Contains("4600H");

        public static bool IsSupportedUViGPU()
            => Name.Contains("RYZEN AI MAX") || 
               Name.Contains("7945H") || 
               Name.Contains("7845H") ||
               Name.Contains("6900H");

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
