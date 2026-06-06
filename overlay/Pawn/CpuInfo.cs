using Microsoft.Win32;

namespace PawnIO
{
    public static class CpuInfo
    {
        public static readonly bool IsAMD = DetectAMD();

        private static bool DetectAMD()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0");
                var vendor = key?.GetValue("VendorIdentifier")?.ToString() ?? "";
                return vendor.IndexOf("AMD", System.StringComparison.OrdinalIgnoreCase) >= 0;
            }
            catch { return false; }
        }
    }
}
