using Microsoft.Win32;
using System.Runtime.Intrinsics.X86;

namespace PawnIO
{
    /// <summary>
    /// Detects Intel Raptor Lake (13th/14th Gen) processors and verifies
    /// the active microcode revision against the minimum safe baseline (0x12B)
    /// required to mitigate the Vmin Shift Instability (clock tree degradation).
    /// </summary>
    public static class IntelMicrocodeCheck
    {
        private const string CpuRegKey = @"HARDWARE\DESCRIPTION\System\CentralProcessor\0";

        // Affected Raptor Lake CPUIDs and their minimum safe microcode revisions.
        // Source: Intel SA-00950 advisory and microcode release notes.
        private static readonly Dictionary<uint, uint> AffectedCpuIds = new()
        {
            { 0xB0671, 0x12B },  // Raptor Lake-S (desktop i9/i7/i5) + HX mobile
            { 0xB06A2, 0x12B },  // Raptor Lake-HX mobile variant
            { 0x90675, 0x12B },  // Raptor Lake-S B0 stepping
            { 0x90672, 0x12B },  // Raptor Lake-S C0 stepping
        };

        // Intel-specified power limits per SKU for HX-series laptops.
        // Used by the diagnostics panel "Intel Default Limits" button.
        public static readonly Dictionary<string, (int PL1, int PL2)> IntelDefaultLimits = new()
        {
            { "i9-14900HX", (55, 157) },
            { "i9-13950HX", (55, 157) },
            { "i9-13900HX", (55, 157) },
            { "i7-14700HX", (55, 157) },
            { "i7-13700HX", (55, 157) },
            { "i7-13650HX", (45, 115) },
            { "i5-14500HX", (45, 115) },
            { "i5-13500HX", (45, 115) },
        };

        private static bool _initialized;

        /// <summary>True if the CPU is an Intel Raptor Lake affected by Vmin Shift.</summary>
        public static bool IsRaptorLake { get; private set; }

        /// <summary>Raw CPUID (Family_Model_Stepping) from CPUID(EAX=1).</summary>
        public static uint CpuIdValue { get; private set; }

        /// <summary>Currently active microcode revision.</summary>
        public static uint MicrocodeRevision { get; private set; }

        /// <summary>True if Raptor Lake AND microcode revision is below the safe baseline.</summary>
        public static bool IsVulnerable { get; private set; }

        /// <summary>Human-readable status string for UI display.</summary>
        public static string StatusSummary { get; private set; } = string.Empty;

        /// <summary>
        /// Minimum safe microcode revision for the detected CPUID.
        /// Returns 0 if the CPU is not a known affected Raptor Lake.
        /// </summary>
        public static uint MinimumSafeRevision { get; private set; }

        /// <summary>
        /// Initializes detection. Safe to call on any CPU — returns immediately
        /// on AMD or non-Raptor-Lake Intel processors with no side effects.
        /// </summary>
        public static void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            if (CpuInfo.IsAMD || !X86Base.IsSupported)
            {
                StatusSummary = "N/A (AMD)";
                return;
            }

            // Read CPUID(EAX=1) for Family_Model_Stepping
            var (eax, _, _, _) = X86Base.CpuId(1, 0);
            CpuIdValue = (uint)eax;

            if (!AffectedCpuIds.TryGetValue(CpuIdValue, out uint minRev))
            {
                StatusSummary = $"CPUID 0x{CpuIdValue:X} (not Raptor Lake)";
                return;
            }

            IsRaptorLake = true;
            MinimumSafeRevision = minRev;

            // Read microcode revision from the Windows registry (primary method).
            MicrocodeRevision = ReadMicrocodeFromRegistry();

            IsVulnerable = MicrocodeRevision < MinimumSafeRevision;

            StatusSummary = IsVulnerable
                ? $"⚠ Microcode 0x{MicrocodeRevision:X} — VULNERABLE (need ≥ 0x{MinimumSafeRevision:X})"
                : $"✓ Microcode 0x{MicrocodeRevision:X} (Safe)";

            Logger.WriteLine($"Intel Raptor Lake Check: CPUID=0x{CpuIdValue:X}, " +
                             $"Microcode=0x{MicrocodeRevision:X}, " +
                             $"MinSafe=0x{MinimumSafeRevision:X}, " +
                             $"Vulnerable={IsVulnerable}");
        }

        /// <summary>
        /// Reads the microcode revision from the Windows registry.
        /// The OS caches the revision in "Update Revision" as an 8-byte REG_BINARY
        /// in mixed-endian format: high DWORD (bytes 4-7) contains the revision
        /// in little-endian byte order.
        /// </summary>
        private static uint ReadMicrocodeFromRegistry()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(CpuRegKey);
                if (key?.GetValue("Update Revision") is byte[] raw && raw.Length >= 8)
                {
                    // The microcode revision is in the upper DWORD (bytes 4-7),
                    // stored as little-endian within that DWORD.
                    return BitConverter.ToUInt32(raw, 4);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Microcode registry read failed: " + ex.Message);
            }

            return 0;
        }

        /// <summary>
        /// Attempts to read the microcode revision directly from MSR 0x8B via PawnIO.
        /// More authoritative than the registry but requires Ring 0 access.
        /// Returns the revision, or null if PawnIO is unavailable.
        /// </summary>
        public static uint? ReadMicrocodeFromMsr(IntelMsr msr)
        {
            if (msr == null || !msr.IsInitialized) return null;

            try
            {
                return msr.GetMicrocodeRevision();
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Microcode MSR read failed: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Attempts to match the CPU name to an Intel default power limit spec.
        /// Returns null if the SKU is not in the known limits table.
        /// </summary>
        public static (int PL1, int PL2)? GetIntelDefaultLimitsForCpu()
        {
            string name = CpuInfo.Name;
            if (string.IsNullOrEmpty(name)) return null;

            foreach (var kvp in IntelDefaultLimits)
            {
                if (name.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
                    return kvp.Value;
            }

            return null;
        }
    }
}
